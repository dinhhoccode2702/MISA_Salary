using System;
using MISA.Salary.Common.Attributes;
using MISA.Salary.Common.Base;

namespace MISA.Salary.Common.Model
{
    /// <summary>
    /// Entity Cấu hình cột bảng (bảng pa_grid_config)
    /// Lưu trữ thông tin cấu hình hiển thị các cột trong DataGrid
    /// Cho phép user tùy chỉnh ẩn/hiện, ghim cột, thay đổi thứ tự cột
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class GridConfig : BaseEntity
    {
        /// <summary>
        /// ID cấu hình (Primary Key, GUID)
        /// </summary>
        [MISAPrimaryKey]
        public Guid GridConfigId { get; set; }

        /// <summary>
        /// Tên bảng áp dụng cấu hình (VD: salary_composition_list)
        /// </summary>
        public string GridConfigTableName { get; set; } = string.Empty;

        /// <summary>
        /// Tên trường dữ liệu (Field Name)
        /// </summary>
        public string GridConfigColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Tiêu đề hiển thị (Caption)
        /// </summary>
        public string GridConfigColumnCaption { get; set; } = string.Empty;

        /// <summary>
        /// Độ rộng cột (px)
        /// </summary>
        public int GridConfigWidthSize { get; set; } = 150;

        /// <summary>
        /// Hiển thị cột: 1-Có, 0-Không
        /// </summary>
        public int GridConfigVisibleStatus { get; set; } = 1;

        /// <summary>
        /// Ghim cột: 0-Không ghim, 1-Ghim trái, 2-Ghim phải
        /// </summary>
        public int GridConfigFixedStatus { get; set; } = 0;

        /// <summary>
        /// Thứ tự hiển thị của cột
        /// </summary>
        public int GridConfigSortOrder { get; set; } = 0;
    }
}
