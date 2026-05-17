using System;

namespace MISA.Salary.Common.Exceptions
{
    /// <summary>
    /// Exception tùy chỉnh khi không tìm thấy bản ghi
    /// Khi throw exception này, ExceptionMiddleware sẽ trả về HTTP 404 Not Found
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Khởi tạo NotFoundException với thông báo lỗi
        /// </summary>
        /// <param name="message">Thông báo lỗi, VD: "Không tìm thấy thành phần lương với ID: xxx"</param>
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
