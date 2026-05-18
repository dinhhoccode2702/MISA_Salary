using MISA.Salary.Common.Model;

namespace MISA.Salary.Common.DTOs
{
    public class GridConfigDto
    {
        public string ColumnName { get; set; } = string.Empty;

        public string ColumnCaption { get; set; } = string.Empty;

        public int WidthSize { get; set; }

        public int VisibleStatus { get; set; }

        public int FixedStatus { get; set; }

        public int SortOrder { get; set; }

        public static GridConfigDto FromEntity(GridConfig config)
        {
            return new GridConfigDto
            {
                ColumnName = config.GridConfigColumnName,
                ColumnCaption = config.GridConfigColumnCaption,
                WidthSize = config.GridConfigWidthSize,
                VisibleStatus = config.GridConfigVisibleStatus,
                FixedStatus = config.GridConfigFixedStatus,
                SortOrder = config.GridConfigSortOrder,
            };
        }
    }
}
