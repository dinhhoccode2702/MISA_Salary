using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Model;
using MISA.Salary.BL.Base;

namespace MISA.Salary.BL.Interfaces
{
    /// <summary>
    /// Interface Service cho Cấu hình cột bảng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public interface IGridConfigService : IBaseService<GridConfig>
    {
        /// <summary>
        /// Lấy cấu hình cột theo tên bảng
        /// </summary>
        Task<ServiceResult> GetByTableNameAsync(string tableName);

        /// <summary>
        /// Lưu cấu hình cột
        /// </summary>
        Task<ServiceResult> SaveConfigsAsync(List<GridConfig> configs, string tableName);
    }
}
