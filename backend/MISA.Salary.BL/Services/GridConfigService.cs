using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.BL.Base;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.BL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ cho Cấu hình cột bảng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class GridConfigService : BaseService<GridConfig>, IGridConfigService
    {
        private readonly IGridConfigRepository _gridConfigRepo;

        public GridConfigService(IGridConfigRepository repository) : base(repository)
        {
            _gridConfigRepo = repository;
        }

        /// <summary>
        /// Lấy cấu hình cột theo tên bảng
        /// </summary>
        public async Task<ServiceResult> GetByTableNameAsync(string tableName)
        {
            var data = await _gridConfigRepo.GetByTableNameAsync(tableName);
            return ServiceResult.Success(data);
        }

        /// <summary>
        /// Lưu cấu hình cột (replace toàn bộ)
        /// </summary>
        public async Task<ServiceResult> SaveConfigsAsync(List<GridConfig> configs, string tableName)
        {
            await _gridConfigRepo.SaveConfigsAsync(configs, tableName);
            return ServiceResult.Success();
        }
    }
}
