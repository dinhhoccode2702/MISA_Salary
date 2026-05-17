using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISA.Salary.BL.Interfaces;

namespace MISA.Salary.API.Controllers
{
    /// <summary>
    /// Controller xử lý các nghiệp vụ kiểm tra dữ liệu realtime
    /// </summary>
    [Route("api/v1/validations")]
    [ApiController]
    public class ValidationsController : ControllerBase
    {
        private readonly ISalaryCompositionService _service;

        public ValidationsController(ISalaryCompositionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Kiểm tra trùng mã thành phần lương
        /// </summary>
        /// <param name="code">Mã cần check</param>
        /// <param name="id">ID bản ghi (nếu đang sửa)</param>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckCode([FromQuery] string code, [FromQuery] Guid? id = null)
        {
            // Sử dụng logic check duplicate đã có trong BaseRepository thông qua Service
            var isDuplicate = await _service.CheckDuplicateAsync("salary_composition_code", code, id);
            return Ok(isDuplicate);
        }

        /// <summary>
        /// Kiểm tra cú pháp công thức (Stub)
        /// </summary>
        [HttpPost("check-formula")]
        public IActionResult CheckFormula([FromBody] FormulaCheckRequest request)
        {
            // Trong thực tế sẽ có bộ parser công thức. Ở đây trả về true để FE tiếp tục.
            return Ok(true);
        }
    }

    public class FormulaCheckRequest
    {
        public string Formula { get; set; } = string.Empty;
    }
}
