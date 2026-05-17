using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.BL.Base;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.BL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ cho Danh mục thành phần lương hệ thống
    /// Author: MISA (11/05/2026)
    /// </summary>
    public class SalarySystemService : BaseService<SalarySystem>, ISalarySystemService
    {
        private readonly ISalarySystemRepository _systemRepo;
        private readonly ISalaryCompositionRepository _compositionRepo;

        public SalarySystemService(
            ISalarySystemRepository systemRepo,
            ISalaryCompositionRepository compositionRepo) : base(systemRepo)
        {
            _systemRepo = systemRepo;
            _compositionRepo = compositionRepo;
        }

        /// <summary>
        /// Đưa thành phần lương từ danh mục hệ thống vào danh sách sử dụng
        /// Quy trình:
        /// 1. Lấy thông tin các TPL hệ thống theo danh sách ID
        /// 2. Tạo bản sao cho mỗi TPL trong bảng pa_salary_composition với OrganizationId tương ứng.
        /// </summary>
        public async Task<ServiceResult> AddToListAsync(List<Guid> ids, Guid organizationId)
        {
            var addedCount = 0;
            if (organizationId == Guid.Empty)
            {
                return ServiceResult.Failure("Đơn vị áp dụng không được để trống.");
            }

            foreach (var id in ids)
            {
                // Lấy thông tin TPL hệ thống
                var systemItem = await _systemRepo.GetByIdAsync(id);
                if (systemItem == null) continue;

                // Tạo bản ghi mới trong bảng SalaryComposition
                var newComposition = new SalaryComposition
                {
                    SalaryCompositionId = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    SalaryCompositionCode = systemItem.SalarySystemCode,
                    SalaryCompositionName = systemItem.SalarySystemName,
                    SalaryCompositionComponentType = systemItem.SalarySystemComponentType,
                    SalaryCompositionNatureType = systemItem.SalarySystemNatureType,
                    SalaryCompositionDataType = systemItem.SalarySystemDataType,
                    SalaryCompositionQuotaFormula = systemItem.SalarySystemQuotaFormula,
                    SalaryCompositionValueFormula = systemItem.SalarySystemValueFormula,
                    SalaryCompositionDescription = systemItem.SalarySystemDescription,
                    SalaryCompositionIsSystemStatus = 1,  // Đánh dấu đến từ hệ thống
                    SalaryCompositionActiveStatus = 1,    // Mặc định đang theo dõi
                    SalaryCompositionPayslipStatus = 1,   // Mặc định hiển thị phiếu lương
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                // Mã thành phần chỉ cần duy nhất trong phạm vi một đơn vị.
                var isDuplicate = await _compositionRepo.CheckDuplicateCodeInOrganizationAsync(
                    newComposition.OrganizationId,
                    newComposition.SalaryCompositionCode);
                
                if (!isDuplicate)
                {
                    await _compositionRepo.InsertAsync(newComposition);
                    addedCount++;
                }
            }

            return ServiceResult.Success(new { AddedCount = addedCount });
        }
    }
}
