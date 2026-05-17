using System;
using MISA.Salary.Common.Attributes;
using MISA.Salary.Common.Base;

namespace MISA.Salary.Common.Model
{
    /// <summary>
    /// Entity Danh mục thành phần lương hệ thống (bảng pa_salary_system)
    /// Dùng cho tab "Danh mục của hệ thống".
    /// Author: MISA (11/05/2026)
    /// </summary>
    [MISATableName("pa_salary_system")]
    public class SalarySystem : BaseEntity
    {
        /// <summary>
        /// ID thành phần lương hệ thống (Primary Key, GUID)
        /// </summary>
        [MISAPrimaryKey]
        public Guid SalarySystemId { get; set; }

        /// <summary>
        /// Mã thành phần lương hệ thống
        /// </summary>
        public string SalarySystemCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên thành phần lương hệ thống
        /// </summary>
        public string SalarySystemName { get; set; } = string.Empty;

        /// <summary>
        /// Loại thành phần: 1-Lương, 2-Phụ cấp, 3-Phúc lợi, 4-Bảo hiểm...
        /// </summary>
        public int SalarySystemComponentType { get; set; } = 1;

        /// <summary>
        /// Tính chất: 1-Thu nhập, 2-Khấu trừ, 3-Chịu thuế, 4-Miễn thuế...
        /// </summary>
        public int SalarySystemNatureType { get; set; } = 1;

        /// <summary>
        /// Kiểu giá trị: 1-Tiền tệ, 2-Hệ số, 3-Chữ, 4-Phần trăm
        /// </summary>
        public int SalarySystemDataType { get; set; } = 1;

        /// <summary>
        /// Định mức: Lưu chuỗi công thức
        /// </summary>
        public string? SalarySystemQuotaFormula { get; set; }

        /// <summary>
        /// Giá trị: Lưu chuỗi công thức
        /// </summary>
        public string? SalarySystemValueFormula { get; set; }

        /// <summary>
        /// Mô tả diễn giải
        /// </summary>
        public string? SalarySystemDescription { get; set; }
    }
}
