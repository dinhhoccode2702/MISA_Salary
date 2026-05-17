using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.Common.DTOs;

namespace MISA.Salary.BL.Base
{
    /// <summary>
    /// Interface Service cơ sở (Generic Pattern)
    /// Định nghĩa các phương thức business logic chung cho tất cả các entity
    /// Author: MISA (10/05/2026)
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    public interface IBaseService<T> where T : class
    {
        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        Task<ServiceResult> GetAllAsync();

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        Task<ServiceResult> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        Task<ServiceResult> GetPagingAsync(int pageNumber, int pageSize, string? search = null);

        /// <summary>
        /// Thêm mới bản ghi (validate trước khi insert)
        /// </summary>
        Task<ServiceResult> InsertAsync(T entity);

        /// <summary>
        /// Cập nhật bản ghi (validate trước khi update)
        /// </summary>
        Task<ServiceResult> UpdateAsync(T entity, Guid id);

        /// <summary>
        /// Xóa bản ghi theo ID
        /// </summary>
        Task<ServiceResult> DeleteAsync(Guid id);

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        Task<ServiceResult> DeleteManyAsync(List<Guid> ids);

        /// <summary>
        /// Kiểm tra trùng lặp dữ liệu
        /// </summary>
        Task<bool> CheckDuplicateAsync(string columnName, string value, Guid? excludeId = null);
    }
}
