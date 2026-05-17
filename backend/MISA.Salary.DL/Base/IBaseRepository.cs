using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.Common.DTOs;

namespace MISA.Salary.DL.Base
{
    /// <summary>
    /// Interface Repository cơ sở (Generic Pattern)
    /// Định nghĩa các phương thức CRUD chung cho tất cả các bảng
    /// Các Repository cụ thể sẽ kế thừa interface này
    /// Author: MISA (10/05/2026)
    /// </summary>
    /// <typeparam name="T">Kiểu entity tương ứng với bảng trong DB</typeparam>
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Lấy tất cả bản ghi trong bảng
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy bản ghi theo ID (Primary Key)
        /// </summary>
        /// <param name="id">ID của bản ghi (GUID)</param>
        /// <returns>Bản ghi tìm được hoặc null</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy danh sách bản ghi có phân trang, tìm kiếm
        /// </summary>
        /// <param name="pageNumber">Số trang (1-based)</param>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <param name="search">Từ khóa tìm kiếm (tìm theo Code + Name)</param>
        /// <param name="additionalWhere">Điều kiện lọc bổ sung (WHERE clause)</param>
        /// <param name="parameters">Tham số cho điều kiện lọc bổ sung</param>
        /// <returns>Kết quả phân trang</returns>
        Task<PagingResult<T>> GetPagingAsync(int pageNumber, int pageSize, string? search = null,
            string? additionalWhere = null, object? parameters = null);

        /// <summary>
        /// Thêm mới bản ghi vào DB
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng bị ảnh hưởng (1 = thành công)</returns>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// Cập nhật bản ghi trong DB
        /// </summary>
        /// <param name="entity">Entity chứa dữ liệu mới</param>
        /// <param name="id">ID bản ghi cần cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng (1 = thành công)</returns>
        Task<int> UpdateAsync(T entity, Guid id);

        /// <summary>
        /// Xóa bản ghi theo ID
        /// </summary>
        /// <param name="id">ID bản ghi cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng (1 = thành công)</returns>
        Task<int> DeleteAsync(Guid id);

        /// <summary>
        /// Xóa nhiều bản ghi theo danh sách ID
        /// </summary>
        /// <param name="ids">Danh sách ID cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteManyAsync(List<Guid> ids);

        /// <summary>
        /// Kiểm tra giá trị của một trường có bị trùng trong DB không
        /// Dùng cho validate unique (VD: kiểm tra mã TPL trùng)
        /// </summary>
        /// <param name="columnName">Tên cột cần kiểm tra</param>
        /// <param name="value">Giá trị cần kiểm tra</param>
        /// <param name="excludeId">ID bản ghi loại trừ (khi sửa, loại trừ chính nó)</param>
        /// <returns>true nếu đã tồn tại (trùng), false nếu chưa</returns>
        Task<bool> CheckDuplicateAsync(string columnName, string value, Guid? excludeId = null);
    }
}
