namespace MISA.Salary.Common.Enums
{
    /// <summary>
    /// Enum Tính chất của thành phần lương
    /// Quyết định cách thành phần lương ảnh hưởng đến thuế và lương ròng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public enum Nature
    {
        /// <summary>Thu nhập chịu thuế TNCN</summary>
        ThuNhapChiuThue = 1,

        /// <summary>Thu nhập miễn thuế TNCN</summary>
        ThuNhapMienThue = 2,

        /// <summary>Khấu trừ được giảm trừ khi tính thuế</summary>
        KhauTruGiamTru = 3,

        /// <summary>Khấu trừ không được giảm trừ khi tính thuế</summary>
        KhauTruKhongGiamTru = 4,

        /// <summary>Khác (thông tin bổ sung, căn cứ tính lương)</summary>
        Khac = 5
    }
}
