using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Base;

namespace MISA.Salary.DL.Interfaces
{
    /// <summary>
    /// Interface Repository cho Cấu hình cột bảng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public interface IGridConfigRepository : IBaseRepository<GridConfig>
    {
        /// <summary>
        /// Lấy danh sách cấu hình cột theo tên bảng
        /// </summary>
        /// <param name="tableName">Tên bảng (VD: "SalaryComposition")</param>
        /// <returns>Danh sách cấu hình cột</returns>
        Task<IEnumerable<GridConfig>> GetByTableNameAsync(string tableName);

        /// <summary>
        /// Lưu cấu hình cột (xóa cũ, thêm mới toàn bộ)
        /// </summary>
        /// <param name="configs">Danh sách cấu hình mới</param>
        /// <param name="tableName">Tên bảng</param>
        Task SaveConfigsAsync(List<GridConfig> configs, string tableName);
    }
}
