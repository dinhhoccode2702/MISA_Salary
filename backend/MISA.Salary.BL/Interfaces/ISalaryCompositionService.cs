using System;
using System.Threading.Tasks;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Model;
using MISA.Salary.BL.Base;

namespace MISA.Salary.BL.Interfaces
{
    /// <summary>
    /// Interface Service cho Thành phần lương
    /// Bổ sung các chức năng riêng: phân trang nâng cao, nhân bản, chuyển trạng thái
    /// Author: MISA (10/05/2026)
    /// </summary>
    public interface ISalaryCompositionService : IBaseService<SalaryComposition>
    {
        /// <summary>
        /// Lấy danh sách phân trang với lọc theo Status và OrganizationId
        /// </summary>
        Task<ServiceResult> GetPagingWithFilterAsync(
            int skip, 
            int take,
            string? keyword = null, 
            int? status = null, 
            Guid? organizationId = null,
            int? type = null,
            int? nature = null,
            string? sort = null,
            string? filter = null);

        /// <summary>
        /// Nhân bản một thành phần lương (copy dữ liệu, tạo mã mới)
        /// </summary>
        Task<ServiceResult> CloneAsync(Guid id);

        /// <summary>
        /// Chuyển trạng thái: Đang theo dõi ↔ Ngừng theo dõi
        /// </summary>
        Task<ServiceResult> ToggleStatusAsync(Guid id, int newStatus);

        // Thay đổi tham số truyền vào
        Task<ServiceResult> BulkImportAsync(List<Guid> systemIds, Guid organizationId);
    }
}
