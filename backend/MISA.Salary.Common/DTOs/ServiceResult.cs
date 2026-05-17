namespace MISA.Salary.Common.DTOs
{
    /// <summary>
    /// DTO chuẩn cho tất cả response trả về từ API
    /// Format bắt buộc: { IsSuccess, ErrorCode, DevMsg, UserMsg, Data }
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// Kết quả thực hiện: true = thành công, false = thất bại
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Mã lỗi nội bộ (dùng cho dev tra cứu)
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Thông báo lỗi dành cho Developer (chi tiết kỹ thuật)
        /// </summary>
        public string? DevMsg { get; set; }

        /// <summary>
        /// Thông báo lỗi dành cho User (hiển thị trên giao diện)
        /// </summary>
        public string? UserMsg { get; set; }

        /// <summary>
        /// Dữ liệu trả về (có thể là object, list, hoặc null)
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Tạo ServiceResult thành công
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <returns>ServiceResult với IsSuccess = true</returns>
        public static ServiceResult Success(object? data = null)
        {
            return new ServiceResult
            {
                IsSuccess = true,
                Data = data
            };
        }

        /// <summary>
        /// Tạo ServiceResult thất bại
        /// </summary>
        /// <param name="devMsg">Thông báo cho developer</param>
        /// <param name="userMsg">Thông báo cho user</param>
        /// <param name="errorCode">Mã lỗi</param>
        /// <param name="data">Dữ liệu bổ sung (VD: danh sách lỗi validate)</param>
        /// <returns>ServiceResult với IsSuccess = false</returns>
        public static ServiceResult Failure(string? devMsg = null, string? userMsg = null, string? errorCode = null, object? data = null)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                DevMsg = devMsg,
                UserMsg = userMsg ?? "Có lỗi xảy ra, vui lòng liên hệ MISA để được hỗ trợ.",
                ErrorCode = errorCode,
                Data = data
            };
        }
    }
}
