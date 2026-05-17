using System;
using System.Collections.Generic;

namespace MISA.Salary.Common.Exceptions
{
    /// <summary>
    /// Exception tùy chỉnh cho lỗi validate dữ liệu
    /// Khi throw exception này, ExceptionMiddleware sẽ trả về HTTP 400 Bad Request
    /// Chứa danh sách các lỗi validate (key = tên trường, value = thông báo lỗi)
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class ValidateException : Exception
    {
        /// <summary>
        /// Danh sách lỗi validate
        /// Key = tên trường, Value = thông báo lỗi tương ứng
        /// VD: { "SalaryCompositionCode": "Mã thành phần lương không được để trống" }
        /// </summary>
        public Dictionary<string, string> Errors { get; }

        /// <summary>
        /// Khởi tạo ValidateException với message đơn giản
        /// </summary>
        /// <param name="message">Thông báo lỗi chung</param>
        public ValidateException(string message) : base(message)
        {
            Errors = new Dictionary<string, string>();
        }

        /// <summary>
        /// Khởi tạo ValidateException với danh sách lỗi chi tiết
        /// </summary>
        /// <param name="errors">Dictionary chứa tên trường và thông báo lỗi</param>
        public ValidateException(Dictionary<string, string> errors)
            : base("Dữ liệu không hợp lệ.")
        {
            Errors = errors;
        }
    }
}
