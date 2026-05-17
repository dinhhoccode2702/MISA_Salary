using System;
using MISA.Salary.Common.Attributes;
using MISA.Salary.Common.Base;

namespace MISA.Salary.Common.Model
{
    /// <summary>
    /// Entity Đơn vị công tác / Cơ cấu tổ chức (bảng pa_organization)
    /// Dữ liệu dạng cây: mỗi đơn vị có thể có đơn vị cha (ParentId)
    /// Dùng cho combobox lọc và DxTreeList
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class Organization : BaseEntity
    {
        /// <summary>
        /// ID đơn vị công tác
        /// </summary>
        [MISAPrimaryKey]
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Mã đơn vị công tác
        /// </summary>
        [MISARequired("Mã đơn vị công tác không được để trống")]
        public string OrganizationCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên đơn vị công tác
        /// </summary>
        [MISARequired("Tên đơn vị công tác không được để trống")]
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// ID đơn vị cha
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Trạng thái: 1-Đang hoạt động, 0-Ngừng hoạt động
        /// </summary>
        public int OrganizationStatus { get; set; } = 1;
    }
}
