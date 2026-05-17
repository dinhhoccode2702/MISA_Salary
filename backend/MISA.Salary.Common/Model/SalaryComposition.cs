using System;
using MISA.Salary.Common.Attributes;
using MISA.Salary.Common.Base;
using MISA.Salary.Common.Enums;

namespace MISA.Salary.Common.Model
{
    /// <summary>
    /// Entity Thành phần lương đang sử dụng (bảng pa_salary_composition)
    /// Chứa thông tin chi tiết của từng thành phần lương mà doanh nghiệp đang sử dụng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class SalaryComposition : BaseEntity
    {
        /// <summary>
        /// ID thành phần lương (Primary Key, GUID)
        /// </summary>
        [MISAPrimaryKey]
        public Guid SalaryCompositionId { get; set; }

        /// <summary>
        /// ID đơn vị áp dụng (FK -> pa_organization.organization_id).
        /// </summary>
        [MISARequired("Đơn vị áp dụng không được để trống.")]
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Mã thành phần lương
        /// </summary>
        [MISARequired("Mã thành phần lương không được để trống.")]
        [MISAMaxLength(255, "Mã thành phần lương không được vượt quá 255 ký tự.")]
        public string SalaryCompositionCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên thành phần lương
        /// </summary>
        [MISARequired("Tên thành phần lương không được để trống.")]
        [MISAMaxLength(255, "Tên thành phần lương không được vượt quá 255 ký tự.")]
        public string SalaryCompositionName { get; set; } = string.Empty;

        /// <summary>
        /// Loại thành phần: 1-Lương, 2-Phụ cấp...
        /// </summary>
        public int SalaryCompositionComponentType { get; set; } = 1;

        /// <summary>
        /// Tính chất: 1-Thu nhập, 2-Khấu trừ...
        /// </summary>
        public int SalaryCompositionNatureType { get; set; } = 1;

        /// <summary>
        /// Định mức: Lưu cấu trúc công thức
        /// </summary>
        public string? SalaryCompositionQuotaFormula { get; set; }

        /// <summary>
        /// Cho phép vượt định mức: 1-Có, 0-Không
        /// </summary>
        public int SalaryCompositionAllowExceedStatus { get; set; } = 0;

        /// <summary>
        /// Kiểu giá trị: 1-Tiền tệ, 2-Hệ số...
        /// </summary>
        public int SalaryCompositionDataType { get; set; } = 1;

        /// <summary>
        /// Loại giá trị: 1-Tự động cộng tổng, 2-Tính theo công thức
        /// </summary>
        public int SalaryCompositionValueType { get; set; } = 1;

        /// <summary>
        /// Công thức giá trị
        /// </summary>
        public string? SalaryCompositionValueFormula { get; set; }

        /// <summary>
        /// Mô tả chi tiết
        /// </summary>
        public string? SalaryCompositionDescription { get; set; }

        /// <summary>
        /// Hiển thị phiếu lương: 1-Có, 0-Không...
        /// </summary>
        public int SalaryCompositionPayslipStatus { get; set; } = 1;

        /// <summary>
        /// Nguồn tạo: 1-Hệ thống, 0-Tự thêm
        /// </summary>
        public int SalaryCompositionIsSystemStatus { get; set; } = 0;

        /// <summary>
        /// Trạng thái: 1-Đang theo dõi, 0-Ngừng theo dõi
        /// </summary>
        public int SalaryCompositionActiveStatus { get; set; } = 1;

        /// <summary>
        /// Tên đơn vị công tác (chỉ dùng để hiển thị, không lưu DB)
        /// </summary>
        public string? OrganizationName { get; set; }
    }
}
