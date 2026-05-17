using Microsoft.AspNetCore.Mvc;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MISA.Salary.API.Controllers
{
    /// <summary>
    /// Controller xử lý cấu hình hiển thị Grid của người dùng
    /// </summary>
    [Route("api/v1/grid-configs")]
    [ApiController]
    public class GridConfigsController : ControllerBase
    {
        private readonly IGridConfigService _gridConfigService;

        public GridConfigsController(IGridConfigService service) 
        {
            _gridConfigService = service;
        }

        /// <summary>
        /// Lấy cấu hình Grid theo tên bảng
        /// </summary>
        /// <param name="tableName">Tên bảng (VD: SalaryComposition)</param>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string tableName)
        {
            var result = await _gridConfigService.GetByTableNameAsync(tableName);
            return Ok(result);
        }

        /// <summary>
        /// Lưu cấu hình Grid
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Save([FromBody] List<GridConfig> configs, [FromQuery] string tableName)
        {
            var result = await _gridConfigService.SaveConfigsAsync(configs, tableName);
            return Ok(result);
        }
    }
}
