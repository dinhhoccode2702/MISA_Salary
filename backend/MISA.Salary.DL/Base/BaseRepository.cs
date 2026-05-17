using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using MISA.Salary.Common.Attributes;
using MISA.Salary.Common.DTOs;
using MySqlConnector;

namespace MISA.Salary.DL.Base
{
    /// <summary>
    /// Repository cơ sở triển khai Generic Pattern
    /// Sử dụng Dapper để thao tác với MySQL, tự động sinh SQL dựa trên tên Entity
    /// Quy ước: Tên bảng trong DB = "pa_" + tên Entity viết thường (PascalCase → snake_case)
    /// Author: MISA (10/05/2026)
    /// </summary>
    /// <typeparam name="T">Kiểu entity tương ứng với bảng</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region Fields

        /// <summary>
        /// Chuỗi kết nối đến MySQL database
        /// </summary>
        protected readonly string _connectionString;

        /// <summary>
        /// Tên bảng trong database (tự động lấy từ tên class entity hoặc attribute)
        /// </summary>
        protected readonly string _tableName;

        /// <summary>
        /// Tên cột Primary Key (tự động tìm property có attribute [MISAPrimaryKey])
        /// </summary>
        protected readonly string _primaryKeyName;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo BaseRepository
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối MySQL</param>
        public BaseRepository(string connectionString)
        {
            _connectionString = connectionString;

            // Ưu tiên lấy tên bảng từ attribute [MISATableName]
            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            if (tableAttr != null)
            {
                _tableName = tableAttr.Name;
            }
            else
            {
                // Mặc định: pa_ + snake_case(EntityName)
                _tableName = "pa_" + ToSnakeCase(typeof(T).Name);
            }

            // Tìm property có attribute [MISAPrimaryKey] để lấy tên Primary Key
            var pkProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<MISAPrimaryKey>() != null);
            _primaryKeyName = pkProperty?.Name ?? $"{typeof(T).Name}Id";
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Chuyển đổi PascalCase sang snake_case
        /// </summary>
        private string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return string.Concat(text.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tạo kết nối mới đến MySQL
        /// </summary>
        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection = GetConnection();
            // Audit fields giờ không có prefix nên dùng trực tiếp modified_date
            var sql = $"SELECT * FROM {_tableName} ORDER BY created_date DESC";
            return await connection.QueryAsync<T>(sql);
        }

        /// <inheritdoc/>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            using var connection = GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE {ToSnakeCase(_primaryKeyName)} = @Id";
            return await connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }

        /// <inheritdoc/>
        public async Task<PagingResult<T>> GetPagingAsync(int pageNumber, int pageSize,
            string? search = null, string? additionalWhere = null, object? parameters = null)
        {
            using var connection = GetConnection();

            // Xây dựng mệnh đề WHERE
            var whereClause = "WHERE 1=1";
            var dynamicParams = new DynamicParameters(parameters);

            // Tìm kiếm theo Code hoặc Name (nếu entity có các trường này)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var props = typeof(T).GetProperties().Select(p => p.Name).ToList();
                var searchConditions = new List<string>();

                // Tìm các trường có chứa "Code" hoặc "Name" để tìm kiếm
                foreach (var prop in props)
                {
                    if (prop.Contains("Code") || prop.Contains("Name"))
                    {
                        searchConditions.Add($"{ToSnakeCase(prop)} LIKE @Search");
                    }
                }

                if (searchConditions.Any())
                {
                    whereClause += $" AND ({string.Join(" OR ", searchConditions)})";
                    dynamicParams.Add("Search", $"%{search}%");
                }
            }

            // Thêm điều kiện lọc bổ sung (VD: lọc theo Status, OrganizationId)
            if (!string.IsNullOrWhiteSpace(additionalWhere))
            {
                whereClause += $" AND {additionalWhere}";
            }

            // Tính tổng số bản ghi thỏa mãn điều kiện
            var countSql = $"SELECT COUNT(*) FROM {_tableName} {whereClause}";
            var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, dynamicParams);

            // Lấy dữ liệu phân trang
            var offset = (pageNumber - 1) * pageSize;
            var sortColumn = "modified_date";
            
            // Tìm cột SortOrder (nếu có) để ưu tiên sắp xếp
            var sortOrderClause = "";
            var sortOrderProp = typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Contains("SortOrder"));

            if (sortOrderProp != null)
            {
                sortOrderClause = $"{ToSnakeCase(sortOrderProp.Name)} ASC, ";
            }

            var dataSql = $"SELECT * FROM {_tableName} {whereClause} ORDER BY {sortOrderClause}{sortColumn} DESC LIMIT @PageSize OFFSET @Offset";
            dynamicParams.Add("PageSize", pageSize);
            dynamicParams.Add("Offset", offset);
            var data = await connection.QueryAsync<T>(dataSql, dynamicParams);

            return new PagingResult<T>
            {
                Data = data,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        /// <inheritdoc/>
        public async Task<int> InsertAsync(T entity)
        {
            using var connection = GetConnection();

            // Lấy danh sách properties của entity (bỏ qua property chỉ đọc hoặc computed)
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite 
                    && p.Name != "OrganizationName"
                    && !p.Name.EndsWith("CreatedDate")
                    && !p.Name.EndsWith("ModifiedDate")
                    && !p.Name.EndsWith("CreatedBy")
                    && !p.Name.EndsWith("ModifiedBy"))
                .ToList();
            
            // Chuyển property name sang snake_case cho SQL column
            var columnNames = string.Join(", ", properties.Select(p => ToSnakeCase(p.Name)));
            var paramNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            var sql = $"INSERT INTO {_tableName} ({columnNames}) VALUES ({paramNames})";
            return await connection.ExecuteAsync(sql, entity);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateAsync(T entity, Guid id)
        {
            using var connection = GetConnection();

            // Lấy danh sách properties (bỏ qua PK và các trường computed/audit)
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite
                    && p.Name != _primaryKeyName
                    && p.Name != "OrganizationName"
                    && !p.Name.EndsWith("CreatedDate")
                    && !p.Name.EndsWith("CreatedBy")
                    && !p.Name.EndsWith("ModifiedBy"))
                .ToList();
            
            // Chuyển property name sang snake_case cho SQL column
            var setClause = string.Join(", ", properties.Select(p => $"{ToSnakeCase(p.Name)} = @{p.Name}"));

            var sql = $"UPDATE {_tableName} SET {setClause} WHERE {ToSnakeCase(_primaryKeyName)} = @EntityId";

            var dynamicParams = new DynamicParameters(entity);
            dynamicParams.Add("EntityId", id);

            return await connection.ExecuteAsync(sql, dynamicParams);
        }

        /// <inheritdoc/>
        public async Task<int> DeleteAsync(Guid id)
        {
            using var connection = GetConnection();
            var sql = $"DELETE FROM {_tableName} WHERE {ToSnakeCase(_primaryKeyName)} = @Id";
            return await connection.ExecuteAsync(sql, new { Id = id });
        }

        /// <inheritdoc/>
        public async Task<int> DeleteManyAsync(List<Guid> ids)
        {
            using var connection = GetConnection();
            var sql = $"DELETE FROM {_tableName} WHERE {ToSnakeCase(_primaryKeyName)} IN @Ids";
            return await connection.ExecuteAsync(sql, new { Ids = ids });
        }

        /// <inheritdoc/>
        public async Task<bool> CheckDuplicateAsync(string columnName, string value, Guid? excludeId = null)
        {
            using var connection = GetConnection();
            // Đảm bảo columnName cũng được chuyển sang snake_case nếu truyền vào dạng PascalCase
            var snakeColumnName = ToSnakeCase(columnName);
            var sql = $"SELECT COUNT(*) FROM {_tableName} WHERE {snakeColumnName} = @Value";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("Value", value);

            // Khi sửa bản ghi, loại trừ chính bản ghi đó ra khỏi kiểm tra
            if (excludeId.HasValue)
            {
                sql += $" AND {ToSnakeCase(_primaryKeyName)} != @ExcludeId";
                dynamicParams.Add("ExcludeId", excludeId.Value);
            }

            var count = await connection.ExecuteScalarAsync<int>(sql, dynamicParams);
            return count > 0;
        }

        #endregion
    }
}
