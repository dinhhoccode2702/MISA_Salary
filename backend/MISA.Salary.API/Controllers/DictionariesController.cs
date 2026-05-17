using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MISA.Salary.API.Controllers
{
    /// <summary>
    /// Controller cung cấp danh mục từ điển cho Dropdown
    /// </summary>
    [Route("api/v1/dictionaries")]
    [ApiController]
    public class DictionariesController : ControllerBase
    {
        /// <summary>
        /// Lấy danh sách Loại thành phần
        /// </summary>
        [HttpGet("component-types")]
        public IActionResult GetComponentTypes()
        {
            var data = new List<object>
            {
                new { Value = 1, Name = "Thu nhập" },
                new { Value = 2, Name = "Khấu trừ" },
                new { Value = 3, Name = "Thuế" },
                new { Value = 4, Name = "Bảo hiểm" }
            };
            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách Tính chất
        /// </summary>
        [HttpGet("nature-types")]
        public IActionResult GetNatureTypes()
        {
            var data = new List<object>
            {
                new { Value = 1, Name = "Hàng tháng" },
                new { Value = 2, Name = "Theo đợt" },
                new { Value = 3, Name = "Theo doanh số" }
            };
            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách Kiểu dữ liệu
        /// </summary>
        [HttpGet("data-types")]
        public IActionResult GetDataTypes()
        {
            var data = new List<object>
            {
                new { Value = 1, Name = "Số" },
                new { Value = 2, Name = "Chữ" },
                new { Value = 3, Name = "Ngày" },
                new { Value = 4, Name = "Logic" }
            };
            return Ok(data);
        }
    }
}
