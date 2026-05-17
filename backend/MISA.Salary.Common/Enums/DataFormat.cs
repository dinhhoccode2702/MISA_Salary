namespace MISA.Salary.Common.Enums
{
    /// <summary>
    /// Enum Kiểu giá trị hiển thị của thành phần lương
    /// Xác định định dạng dữ liệu khi hiển thị trên giao diện
    /// Author: MISA (10/05/2026)
    /// </summary>
    public enum DataFormat
    {
        /// <summary>Định dạng số (1,234)</summary>
        Number = 1,

        /// <summary>Định dạng tiền tệ (1,234,000 đ)</summary>
        Currency = 2,

        /// <summary>Định dạng phần trăm (50%)</summary>
        Percentage = 3,

        /// <summary>Định dạng ngày (dd/MM/yyyy)</summary>
        Date = 4,

        /// <summary>Định dạng chuỗi ký tự</summary>
        Text = 5
    }
}
