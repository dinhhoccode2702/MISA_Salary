using System;

namespace MISA.Salary.Common.Attributes
{
    /// <summary>
    /// Attribute đánh dấu property là trường bắt buộc nhập (required)
    /// Dùng trong BaseService để validate tự động
    /// Author: MISA (10/05/2026)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MISARequired : Attribute
    {
        /// <summary>
        /// Thông báo lỗi khi trường bị bỏ trống
        /// </summary>
        public string ErrorMessage { get; }

        public MISARequired(string errorMessage = "")
        {
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Attribute đánh dấu property có giới hạn độ dài ký tự
    /// Author: MISA (10/05/2026)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MISAMaxLength : Attribute
    {
        /// <summary>
        /// Số ký tự tối đa cho phép
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        /// Thông báo lỗi khi vượt quá độ dài cho phép
        /// </summary>
        public string ErrorMessage { get; }

        public MISAMaxLength(int maxLength, string errorMessage = "")
        {
            MaxLength = maxLength;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Attribute đánh dấu property là trường không được trùng lặp (unique)
    /// Author: MISA (10/05/2026)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MISAUnique : Attribute
    {
        /// <summary>
        /// Thông báo lỗi khi giá trị bị trùng
        /// </summary>
        public string ErrorMessage { get; }

        public MISAUnique(string errorMessage = "")
        {
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Attribute đánh dấu Primary Key của bảng
    /// Author: MISA (10/05/2026)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MISAPrimaryKey : Attribute
    {
    }

    /// <summary>
    /// Attribute chỉ định tên bảng trong database
    /// Author: MISA (11/05/2026)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MISATableName : Attribute
    {
        public string Name { get; }
        public MISATableName(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Attribute chỉ định tiền tố cho các cột nghiệp vụ của bảng
    /// Author: MISA (11/05/2026)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MISAColumnPrefix : Attribute
    {
        public string Prefix { get; }
        public MISAColumnPrefix(string prefix)
        {
            Prefix = prefix;
        }
    }
}
