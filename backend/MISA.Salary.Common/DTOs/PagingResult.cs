using System.Collections.Generic;

namespace MISA.Salary.Common.DTOs
{
    /// <summary>
    /// DTO chứa kết quả phân trang dữ liệu
    /// Dùng cho các API GET danh sách có phân trang
    /// Author: MISA (10/05/2026)
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của bản ghi</typeparam>
    public class PagingResult<T>
    {
        /// <summary>
        /// Danh sách dữ liệu của trang hiện tại
        /// </summary>
        public IEnumerable<T>? Data { get; set; }

        /// <summary>
        /// Tổng số bản ghi thỏa mãn điều kiện (không tính phân trang)
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Trang hiện tại (1-based)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Số bản ghi trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }
    }
}
