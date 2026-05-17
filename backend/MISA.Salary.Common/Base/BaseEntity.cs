using System;

namespace MISA.Salary.Common.Base
{
    /// <summary>
    /// Entity cơ sở chứa các thuộc tính chung cho tất cả bảng trong hệ thống
    /// Các entity cụ thể sẽ kế thừa lớp này
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người tạo bản ghi
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày sửa đổi bản ghi gần nhất
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Người sửa đổi bản ghi gần nhất
        /// </summary>
        public string? ModifiedBy { get; set; }
    }
}
