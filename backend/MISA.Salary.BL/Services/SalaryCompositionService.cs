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
    /// Service xử lý nghiệp vụ cho Thành phần lương
    /// Kế thừa BaseService và bổ sung: phân trang nâng cao, nhân bản, chuyển trạng thái
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class SalaryCompositionService : BaseService<SalaryComposition>, ISalaryCompositionService
    {
        /// <summary>
        /// Repository cụ thể (cast từ IBaseRepository sang ISalaryCompositionRepository)
        /// </summary>
        private readonly ISalaryCompositionRepository _salaryCompositionRepo;

        public SalaryCompositionService(ISalaryCompositionRepository repository) : base(repository)
        {
            _salaryCompositionRepo = repository;
        }

        /// <summary>
        /// Lấy danh sách phân trang với bộ lọc nâng cao
        /// - Tìm kiếm theo Mã/Tên
        /// - Lọc theo Trạng thái (Đang theo dõi / Ngừng theo dõi)
        /// - Lọc theo Đơn vị công tác
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
        /// Nhân bản một thành phần lương (copy dữ liệu, tạo mã mới)
        /// </summary>
        public async Task<ServiceResult> CloneAsync(Guid id)
        {
            // Lấy bản ghi gốc
            var original = await _repository.GetByIdAsync(id);
            if (original == null)
            {
                throw new NotFoundException($"Không tìm thấy thành phần lương với ID: {id}");
            }

            // Tạo bản sao với ID mới
            original.SalaryCompositionId = Guid.NewGuid();
            original.SalaryCompositionCode = $"{original.SalaryCompositionCode}_Copy";
            original.SalaryCompositionName = $"{original.SalaryCompositionName} - Bản sao";
            original.SalaryCompositionIsSystemStatus = 0; // Bản sao thì không phải hệ thống
            original.CreatedDate = DateTime.Now;
            original.ModifiedDate = DateTime.Now;

            // Kiểm tra mã mới có trùng không, nếu trùng thêm số
            int counter = 1;
            while (await _repository.CheckDuplicateAsync("salary_composition_code", original.SalaryCompositionCode))
            {
                original.SalaryCompositionCode = $"{original.SalaryCompositionCode}_Copy_{counter}";
                counter++;
            }

            var result = await _repository.InsertAsync(original);
            return ServiceResult.Success(original);
        }

        /// <summary>
        /// Chuyển trạng thái thành phần lương (Sử dụng lệnh UPDATE tập trung)
        /// Đang theo dõi (1) ↔ Ngừng theo dõi (0)
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
        /// Override InsertAsync để gán ID mới và timestamp trước khi thêm
        /// </summary>
        public override async Task<ServiceResult> InsertAsync(SalaryComposition entity)
        {
            entity.SalaryCompositionId = Guid.NewGuid();
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            return await base.InsertAsync(entity);
        }

        /// <summary>
        /// Override UpdateAsync để cập nhật timestamp
        /// </summary>
        public override async Task<ServiceResult> UpdateAsync(SalaryComposition entity, Guid id)
        {
            entity.ModifiedDate = DateTime.Now;
            return await base.UpdateAsync(entity, id);
        }

        /// <summary>
        /// Override DeleteAsync để kiểm tra thành phần lương hệ thống không được xóa
        /// </summary>
        public override async Task<ServiceResult> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy thành phần lương với ID: {id}");
            }

            // Kiểm tra: Không cho phép xóa thành phần lương thuộc hệ thống (1 là hệ thống)
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
    }
}
