using System;
using System.Threading.Tasks;
using Dapper;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Base;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.DL.Repositories
{
    /// <summary>
    /// Repository cụ thể cho Thành phần lương
    /// Override GetPaging để JOIN lấy tên đơn vị công tác
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class SalaryCompositionRepository : BaseRepository<SalaryComposition>, ISalaryCompositionRepository
    {
        public SalaryCompositionRepository(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Lấy danh sách thành phần lương có phân trang, JOIN với bảng Organization để lấy tên đơn vị
        /// Hỗ trợ tìm kiếm theo Mã/Tên, lọc theo Trạng thái và Đơn vị công tác, Sắp xếp động
        /// </summary>
        public async Task<PagingResult<SalaryComposition>> GetPagingWithOrgAsync(
            int skip, 
            int take,
            string? keyword = null, 
            int? status = null, 
            Guid? organizationId = null,
            int? type = null,
            int? nature = null,
            string? sort = null,
            string? filter = null)
        {
            using var connection = GetConnection();

            // 1. Xây dựng điều kiện WHERE
            var whereClause = "WHERE 1=1";
            var dynamicParams = new DynamicParameters();

            // Tìm kiếm theo Mã hoặc Tên thành phần lương
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                whereClause += " AND (sc.salary_composition_code LIKE @Search OR sc.salary_composition_name LIKE @Search)";
                dynamicParams.Add("Search", $"%{keyword}%");
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                whereClause += " AND sc.salary_composition_active_status = @Status";
                dynamicParams.Add("Status", status.Value);
            }

            // Lọc theo đơn vị công tác
            if (organizationId.HasValue)
            {
                whereClause += " AND sc.organization_id = @OrganizationId";
                dynamicParams.Add("OrganizationId", organizationId.Value);
            }

            // Lọc theo loại thành phần
            if (type.HasValue)
            {
                whereClause += " AND sc.salary_composition_component_type = @Type";
                dynamicParams.Add("Type", type.Value);
            }

            // Lọc theo tính chất
            if (nature.HasValue)
            {
                whereClause += " AND sc.salary_composition_nature_type = @Nature";
                dynamicParams.Add("Nature", nature.Value);
            }

            // 2. Xây dựng mệnh đề ORDER BY động
            // Mặc định sắp xếp theo ngày sửa đổi mới nhất
            var orderClause = "sc.modified_date DESC";
            
            // if (!string.IsNullOrEmpty(sort))
            // {
            //     // Logic đơn giản: Nếu sort truyền lên dạng chuỗi JSON của DevExtreme
            //     // Trong thực tế sẽ dùng thư viện Parser, ở đây ta xử lý cơ bản để bạn nắm nguyên lý
            //     if (sort.Contains("SalaryCompositionCode")) orderClause = "sc.salary_composition_code " + (sort.Contains("desc\":true") ? "DESC" : "ASC");
            //     else if (sort.Contains("SalaryCompositionName")) orderClause = "sc.salary_composition_name " + (sort.Contains("desc\":true") ? "DESC" : "ASC");
            //     else if (sort.Contains("OrganizationName")) orderClause = "o.organization_name " + (sort.Contains("desc\":true") ? "DESC" : "ASC");
            // }

            // 3. Đếm tổng số bản ghi (để FE tính số trang)
            var countSql = $@"SELECT COUNT(*) FROM pa_salary_composition sc
                              LEFT JOIN pa_organization o ON sc.organization_id = o.organization_id
                              {whereClause}";
            var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, dynamicParams);

            // 4. Lấy dữ liệu phân trang
            var dataSql = $@"SELECT sc.*, o.organization_name as OrganizationName
                             FROM pa_salary_composition sc
                             LEFT JOIN pa_organization o ON sc.organization_id = o.organization_id
                             {whereClause}
                             ORDER BY {orderClause}
                             LIMIT @Take OFFSET @Skip";
            
            dynamicParams.Add("Take", take);
            dynamicParams.Add("Skip", skip);
            
            var data = await connection.QueryAsync<SalaryComposition>(dataSql, dynamicParams);

            return new PagingResult<SalaryComposition>
            {
                Data = data,
                TotalRecords = totalRecords,
                TotalPages = take > 0 ? (int)Math.Ceiling((double)totalRecords / take) : 1,
                CurrentPage = take > 0 ? (skip / take) + 1 : 1,
                PageSize = take
            };
        }

        public async Task<int> BulkImportAsync(List<Guid> systemIds, Guid organizationId)
        {
            using var connection = GetConnection();
            // Câu lệnh SQL lấy dữ liệu từ bảng hệ thống và đẩy sang bảng thực tế của khách hàng
            // UUID() trong MySQL sẽ sinh ID mới cho từng dòng
            var sql = @"
                INSERT INTO pa_salary_composition (
                    salary_composition_id, organization_id, salary_composition_code, 
                    salary_composition_name, salary_composition_component_type, 
                    salary_composition_nature_type, salary_composition_quota_formula, 
                    salary_composition_allow_exceed_status, salary_composition_data_type, 
                    salary_composition_value_type, salary_composition_value_formula, 
                    salary_composition_description, salary_composition_payslip_status, 
                    salary_composition_is_system_status, salary_composition_active_status
                )
                SELECT 
                    UUID(), @OrganizationId, salary_system_code, 
                    salary_system_name, salary_system_component_type, 
                    salary_system_nature_type, salary_system_quota_formula, 
                    0, salary_system_data_type, 
                    2, salary_system_value_formula, 
                    salary_system_description, 1, 
                    1, 1
                FROM pa_salary_system s
                WHERE s.salary_system_id IN @Ids
                AND NOT EXISTS (
                    SELECT 1 FROM pa_salary_composition c 
                    WHERE c.organization_id = @OrganizationId
                      AND c.salary_composition_code = s.salary_system_code
                )";

            var parameters = new { Ids = systemIds, OrganizationId = organizationId };
            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<bool> CheckDuplicateCodeInOrganizationAsync(Guid organizationId, string salaryCompositionCode, Guid? excludeId = null)
        {
            using var connection = GetConnection();
            var sql = @"SELECT COUNT(*)
                        FROM pa_salary_composition
                        WHERE organization_id = @OrganizationId
                          AND salary_composition_code = @SalaryCompositionCode";

            var parameters = new DynamicParameters();
            parameters.Add("OrganizationId", organizationId);
            parameters.Add("SalaryCompositionCode", salaryCompositionCode);

            if (excludeId.HasValue)
            {
                sql += " AND salary_composition_id != @ExcludeId";
                parameters.Add("ExcludeId", excludeId.Value);
            }

            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// Cập nhật trạng thái của thành phần lương
        /// </summary>
        public async Task<int> UpdateStatusAsync(Guid id, int status)
        {
            using var connection = GetConnection();
            var sql = "UPDATE pa_salary_composition SET salary_composition_active_status = @Status, modified_date = @ModifiedDate WHERE salary_composition_id = @Id";
            return await connection.ExecuteAsync(sql, new { Id = id, Status = status, ModifiedDate = DateTime.Now });
        }
    }
}
