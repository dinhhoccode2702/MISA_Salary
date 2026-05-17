using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.BL.Base;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Exceptions;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.BL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ cho Thành phần lương.
    /// Kế thừa BaseService và bổ sung: phân trang nâng cao, nhân bản, chuyển trạng thái.
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class SalaryCompositionService : BaseService<SalaryComposition>, ISalaryCompositionService
    {
        /// <summary>
        /// Repository cụ thể của Thành phần lương.
        /// </summary>
        private readonly ISalaryCompositionRepository _salaryCompositionRepo;

        public SalaryCompositionService(ISalaryCompositionRepository repository) : base(repository)
        {
            _salaryCompositionRepo = repository;
        }

        /// <summary>
        /// Lấy danh sách phân trang với bộ lọc nâng cao:
        /// tìm kiếm theo mã/tên, lọc trạng thái, lọc đơn vị áp dụng.
        /// </summary>
        public async Task<ServiceResult> GetPagingWithFilterAsync(
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
            var result = await _salaryCompositionRepo.GetPagingWithOrgAsync(
                skip, take, keyword, status, organizationId, type, nature, sort, filter);
            return ServiceResult.Success(result);
        }

        /// <summary>
        /// Nhân bản một thành phần lương, tạo ID và mã mới.
        /// </summary>
        public async Task<ServiceResult> CloneAsync(Guid id)
        {
            // Lấy bản ghi gốc trước khi tạo bản sao.
            var original = await _repository.GetByIdAsync(id);
            if (original == null)
            {
                throw new NotFoundException($"Không tìm thấy thành phần lương với ID: {id}");
            }

            // Tạo bản sao với ID mới.
            original.SalaryCompositionId = Guid.NewGuid();
            original.SalaryCompositionCode = $"{original.SalaryCompositionCode}_Copy";
            original.SalaryCompositionName = $"{original.SalaryCompositionName} - Bản sao";
            original.SalaryCompositionIsSystemStatus = 0;
            original.CreatedDate = DateTime.Now;
            original.ModifiedDate = DateTime.Now;

            // Kiểm tra mã mới trong cùng đơn vị, nếu trùng thì thêm hậu tố số.
            int counter = 1;
            while (await _salaryCompositionRepo.CheckDuplicateCodeInOrganizationAsync(
                original.OrganizationId,
                original.SalaryCompositionCode))
            {
                original.SalaryCompositionCode = $"{original.SalaryCompositionCode}_Copy_{counter}";
                counter++;
            }

            var result = await _repository.InsertAsync(original);
            return ServiceResult.Success(original);
        }

        /// <summary>
        /// Chuyển trạng thái thành phần lương.
        /// Đang theo dõi (1) hoặc Ngừng theo dõi (0).
        /// </summary>
        public async Task<ServiceResult> ToggleStatusAsync(Guid id, int newStatus)
        {
            var result = await _salaryCompositionRepo.UpdateStatusAsync(id, newStatus);
            if (result == 0)
            {
                throw new NotFoundException($"Không tìm thấy thành phần lương với ID: {id}");
            }
            return ServiceResult.Success(result);
        }

        /// <summary>
        /// Chuẩn hóa dữ liệu, gán ID mới và timestamp trước khi thêm.
        /// </summary>
        public override async Task<ServiceResult> InsertAsync(SalaryComposition entity)
        {
            NormalizeSalaryComposition(entity);
            entity.SalaryCompositionId = Guid.NewGuid();
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            return await base.InsertAsync(entity);
        }

        /// <summary>
        /// Chuẩn hóa dữ liệu và cập nhật timestamp trước khi sửa.
        /// </summary>
        public override async Task<ServiceResult> UpdateAsync(SalaryComposition entity, Guid id)
        {
            NormalizeSalaryComposition(entity);
            entity.ModifiedDate = DateTime.Now;
            return await base.UpdateAsync(entity, id);
        }

        /// <summary>
        /// Không cho phép xóa thành phần lương thuộc danh mục hệ thống.
        /// </summary>
        public override async Task<ServiceResult> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy thành phần lương với ID: {id}");
            }

            // Kiểm tra cứng để dữ liệu hệ thống không bị xóa nhầm.
            if (entity.SalaryCompositionIsSystemStatus == 1)
            {
                throw new ValidateException("Không thể xóa thành phần lương thuộc danh mục hệ thống.");
            }

            return await base.DeleteAsync(id);
        }

        public override async Task<ServiceResult> DeleteManyAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return ServiceResult.Failure("Danh sách ID cần xóa không được để trống.");
            }

            foreach (var id in ids)
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new NotFoundException($"Không tìm thấy thành phần lương với ID: {id}");
                }

                if (entity.SalaryCompositionIsSystemStatus == 1)
                {
                    throw new ValidateException("Không thể xóa thành phần lương thuộc danh mục hệ thống.");
                }
            }

            return await base.DeleteManyAsync(ids);
        }

        
        public async Task<ServiceResult> BulkImportAsync(List<Guid> systemIds, Guid organizationId)
        {
            if (systemIds == null || systemIds.Count == 0)
            {
                return ServiceResult.Failure("Danh sách ID hệ thống không được để trống.");
            }

            if (organizationId == Guid.Empty)
            {
                return ServiceResult.Failure("Đơn vị áp dụng không được để trống.");
            }

            var rowAffected = await _salaryCompositionRepo.BulkImportAsync(systemIds, organizationId);

            return ServiceResult.Success(rowAffected);
        }

        protected override async Task ValidateCustom(SalaryComposition entity, Guid? id, Dictionary<string, string> errors)
        {
            if (entity.OrganizationId == Guid.Empty)
            {
                errors[nameof(SalaryComposition.OrganizationId)] = "Đơn vị áp dụng không được để trống.";
                return;
            }

            if (string.IsNullOrWhiteSpace(entity.SalaryCompositionCode))
            {
                return;
            }

            var isDuplicate = await _salaryCompositionRepo.CheckDuplicateCodeInOrganizationAsync(
                entity.OrganizationId,
                entity.SalaryCompositionCode,
                id);

            if (isDuplicate)
            {
                errors[nameof(SalaryComposition.SalaryCompositionCode)] =
                    "Mã thành phần lương đã tồn tại trong đơn vị áp dụng.";
            }
        }

        private static void NormalizeSalaryComposition(SalaryComposition entity)
        {
            entity.SalaryCompositionCode = entity.SalaryCompositionCode?.Trim() ?? string.Empty;
            entity.SalaryCompositionName = entity.SalaryCompositionName?.Trim() ?? string.Empty;
            entity.SalaryCompositionDescription = entity.SalaryCompositionDescription?.Trim();
        }
    }
}
