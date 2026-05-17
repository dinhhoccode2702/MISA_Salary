using System;
using System.Threading.Tasks;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Base;

namespace MISA.Salary.DL.Interfaces
{
    /// <summary>
    /// Interface Repository cho Thành phần lương
    /// Kế thừa IBaseRepository và bổ sung các phương thức riêng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public interface ISalaryCompositionRepository : IBaseRepository<SalaryComposition>
    {
        /// <summary>
        /// Lấy danh sách thành phần lương có phân trang nâng cao
        /// </summary>
        Task<PagingResult<SalaryComposition>> GetPagingWithOrgAsync(
            int skip, 
            int take,
            string? keyword = null, 
            int? status = null, 
            Guid? organizationId = null,
            int? type = null,
            int? nature = null,
            string? sort = null,
            string? filter = null);
        
        // Thêm vào interface
        Task<int> BulkImportAsync(List<Guid> systemIds, Guid organizationId);

        /// <summary>
        /// Kiểm tra trùng mã thành phần trong phạm vi một đơn vị.
        /// </summary>
        Task<bool> CheckDuplicateCodeInOrganizationAsync(Guid organizationId, string salaryCompositionCode, Guid? excludeId = null);

        /// <summary>
        /// Cập nhật trạng thái của thành phần lương
        /// </summary>
        Task<int> UpdateStatusAsync(Guid id, int status);
    }
}
