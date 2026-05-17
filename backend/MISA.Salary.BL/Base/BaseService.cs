using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MISA.Salary.Common.Attributes;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Exceptions;
using MISA.Salary.DL.Base;

namespace MISA.Salary.BL.Base
{
    /// <summary>
    /// Service cơ sở triển khai Generic Pattern
    /// Xử lý validate tự động dựa trên Attributes và gọi Repository tương ứng
    /// Các Service cụ thể có thể override ValidateCustom() để thêm logic validate riêng
    /// Author: MISA (10/05/2026)
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    public class BaseService<T> : IBaseService<T> where T : class
    {
        #region Fields

        /// <summary>
        /// Repository tương ứng (inject qua constructor)
        /// </summary>
        protected readonly IBaseRepository<T> _repository;

        #endregion

        #region Constructor

        public BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public virtual async Task<ServiceResult> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return ServiceResult.Success(data);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            // Nếu không tìm thấy bản ghi -> throw NotFoundException (trả về 404)
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy bản ghi với ID: {id}");
            }

            return ServiceResult.Success(entity);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResult> GetPagingAsync(int pageNumber, int pageSize, string? search = null)
        {
            var result = await _repository.GetPagingAsync(pageNumber, pageSize, search);
            return ServiceResult.Success(result);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResult> InsertAsync(T entity)
        {
            // Validate dữ liệu trước khi insert
            await ValidateEntity(entity, null);

            // Gọi Repository để thêm vào DB
            var result = await _repository.InsertAsync(entity);
            return ServiceResult.Success(result);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResult> UpdateAsync(T entity, Guid id)
        {
            // Kiểm tra bản ghi có tồn tại không
            var existing = await _repository.GetByIdAsync(id);
            
            if (existing == null)
            {
                throw new NotFoundException($"Không tìm thấy bản ghi với ID: {id}");
            }

            // Ví dụ dùng Reflection đơn giản:
            foreach (var prop in typeof(T).GetProperties())
            {
                var incomingValue = prop.GetValue(entity);

                var newValue = prop.GetValue(entity);
                if (newValue != null)
                {
                    prop.SetValue(existing, incomingValue);
                }
            }

            // Validate dữ liệu trước khi update
            await ValidateEntity(existing, id);

            // Gọi Repository để cập nhật DB
            var result = await _repository.UpdateAsync(existing, id);
            return ServiceResult.Success(result);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResult> DeleteAsync(Guid id)
        {
            // Kiểm tra bản ghi có tồn tại không
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new NotFoundException($"Không tìm thấy bản ghi với ID: {id}");
            }

            var result = await _repository.DeleteAsync(id);
            return ServiceResult.Success(result);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResult> DeleteManyAsync(List<Guid> ids)
        {
            var result = await _repository.DeleteManyAsync(ids);
            return ServiceResult.Success(result);
        }

        /// <inheritdoc/>
        public async Task<bool> CheckDuplicateAsync(string columnName, string value, Guid? excludeId = null)
        {
            return await _repository.CheckDuplicateAsync(columnName, value, excludeId);
        }

        #endregion

        #region Validate Methods

        /// <summary>
        /// Validate entity dựa trên các Attributes đã đánh dấu trên properties
        /// Tự động kiểm tra: Required, MaxLength, Unique
        /// </summary>
        /// <param name="entity">Entity cần validate</param>
        /// <param name="id">ID bản ghi (null khi thêm mới, có giá trị khi sửa)</param>
        protected async Task ValidateEntity(T entity, Guid? id)
        {
            var errors = new Dictionary<string, string>();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);

                // 1. Kiểm tra Required: không được để trống
                var requiredAttr = prop.GetCustomAttribute<MISARequired>();
                if (requiredAttr != null)
                {
                    if (value == null || (value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                    {
                        var errorMsg = !string.IsNullOrEmpty(requiredAttr.ErrorMessage)
                            ? requiredAttr.ErrorMessage
                            : $"{prop.Name} không được để trống.";
                        errors[prop.Name] = errorMsg;
                        continue; // Nếu trống thì không cần kiểm tra MaxLength, Unique
                    }
                }

                // 2. Kiểm tra MaxLength: không được vượt quá số ký tự cho phép
                var maxLengthAttr = prop.GetCustomAttribute<MISAMaxLength>();
                if (maxLengthAttr != null && value is string strVal && strVal.Length > maxLengthAttr.MaxLength)
                {
                    var errorMsg = !string.IsNullOrEmpty(maxLengthAttr.ErrorMessage)
                        ? maxLengthAttr.ErrorMessage
                        : $"{prop.Name} không được vượt quá {maxLengthAttr.MaxLength} ký tự.";
                    errors[prop.Name] = errorMsg;
                }

                // 3. Kiểm tra Unique: không được trùng giá trị với bản ghi khác
                var uniqueAttr = prop.GetCustomAttribute<MISAUnique>();
                if (uniqueAttr != null && value != null)
                {
                    var isDuplicate = await _repository.CheckDuplicateAsync(prop.Name, value.ToString()!, id);
                    if (isDuplicate)
                    {
                        var errorMsg = !string.IsNullOrEmpty(uniqueAttr.ErrorMessage)
                            ? uniqueAttr.ErrorMessage
                            : $"{prop.Name} đã tồn tại trong hệ thống.";
                        errors[prop.Name] = errorMsg;
                    }
                }
            }

            // Gọi validate tùy chỉnh của Service con
            await ValidateCustom(entity, id, errors);

            // Nếu có lỗi -> throw ValidateException (trả về HTTP 400)
            if (errors.Any())
            {
                throw new ValidateException(errors);
            }
        }

        /// <summary>
        /// Hook để các Service con override thêm logic validate riêng
        /// VD: kiểm tra IsSystem trước khi xóa, validate business rule đặc thù
        /// </summary>
        protected virtual Task ValidateCustom(T entity, Guid? id, Dictionary<string, string> errors)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
