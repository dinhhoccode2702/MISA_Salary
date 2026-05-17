namespace MISA.Salary.Common.Enums
{
    /// <summary>
    /// Enum Loại thành phần lương
    /// Xác định thành phần lương thuộc nhóm nào trong hệ thống tính lương
    /// Author: MISA (10/05/2026)
    /// </summary>
    public enum ComponentType
    {
        /// <summary>Lương (Lương cơ bản, Lương KPI, ...)</summary>
        Luong = 1,

        /// <summary>Phụ cấp (Phụ cấp ăn trưa, Phụ cấp xăng xe, ...)</summary>
        PhuCap = 2,

        /// <summary>Giảm trừ / Khấu trừ (BHXH, Thuế TNCN, ...)</summary>
        GiamTru = 3,

        /// <summary>Chấm công (Số ngày công, Số giờ OT, ...)</summary>
        ChamCong = 4,

        /// <summary>Thuế (Thuế TNCN, ...)</summary>
        Thue = 5,

        /// <summary>Bảo hiểm (BHXH, BHYT, BHTN, ...)</summary>
        BaoHiem = 6,

        /// <summary>Thông tin nhân viên (Số người phụ thuộc, Ngày thử việc, ...)</summary>
        ThongTinNhanVien = 7
    }
}
