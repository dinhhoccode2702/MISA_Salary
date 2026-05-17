using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Base;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.DL.Repositories
{
    /// <summary>
    /// Repository cụ thể cho Cấu hình cột bảng
    /// Bổ sung chức năng lấy/lưu cấu hình theo tên bảng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class GridConfigRepository : BaseRepository<GridConfig>, IGridConfigRepository
    {
        public GridConfigRepository(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Lấy danh sách cấu hình cột của một bảng cụ thể
        /// Sắp xếp theo SortOrder tăng dần
        /// </summary>
        public async Task<IEnumerable<GridConfig>> GetByTableNameAsync(string tableName)
        {
            using var connection = GetConnection();
            var sql = "SELECT * FROM pa_grid_config WHERE grid_config_table_name = @TableName ORDER BY grid_config_sort_order ASC";
            return await connection.QueryAsync<GridConfig>(sql, new { TableName = tableName });
        }

        /// <summary>
        /// Lưu lại cấu hình cột: Xóa toàn bộ cấu hình cũ rồi thêm mới
        /// Thao tác trong transaction để đảm bảo tính toàn vẹn dữ liệu
        /// </summary>
        public async Task SaveConfigsAsync(List<GridConfig> configs, string tableName)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Xóa cấu hình cũ của bảng
                await connection.ExecuteAsync(
                    "DELETE FROM pa_grid_config WHERE grid_config_table_name = @TableName",
                    new { TableName = tableName }, transaction);

                // Thêm mới toàn bộ cấu hình
                foreach (var config in configs)
                {
                    config.GridConfigId = Guid.NewGuid();
                    config.GridConfigTableName = tableName;
                }

                var sql = @"INSERT INTO pa_grid_config (grid_config_id, grid_config_column_name, grid_config_column_caption, grid_config_width_size, grid_config_visible_status, grid_config_fixed_status, grid_config_sort_order, grid_config_table_name)
                            VALUES (@GridConfigId, @GridConfigColumnName, @GridConfigColumnCaption, @GridConfigWidthSize, @GridConfigVisibleStatus, @GridConfigFixedStatus, @GridConfigSortOrder, @GridConfigTableName)";
                await connection.ExecuteAsync(sql, configs, transaction);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
