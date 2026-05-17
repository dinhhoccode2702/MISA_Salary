using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.Common.Model;

namespace MISA.Salary.API.Controllers
{
    /// <summary>
    /// Controller xử lý API cho Thành phần lương
    /// Kế thừa BaseController + bổ sung: phân trang nâng cao, nhân bản, chuyển trạng thái
    /// Route: /api/v1/SalaryCompositions
    /// Author: MISA (10/05/2026)
    /// </summary>
    [Route("api/v1/[controller]")]
    public class SalaryCompositionsController : BaseController<SalaryComposition>
    {
        private readonly ISalaryCompositionService _salaryService;

        public SalaryCompositionsController(ISalaryCompositionService service) : base(service)
        {
            _salaryService = service;
        }

        /// <summary>
        /// [GET] Lấy danh sách phân trang với bộ lọc
        /// Endpoint: GET /api/v1/SalaryCompositions/paging?page=1&pageSize=20&search=&status=&organizationId=
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="pageSize">Số bản ghi/trang</param>
        /// <param name="search">Tìm kiếm theo Mã/Tên</param>
        /// <param name="status">Lọc theo trạng thái (0 hoặc 1)</param>
        /// <param name="organizationId">Lọc theo đơn vị công tác</param>
        [HttpGet("paging")]
        public override async Task<IActionResult> GetPaging(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            var query = HttpContext.Request.Query;
            var keyword = query.TryGetValue("keyword", out var keywordValue)
                ? keywordValue.ToString()
                : search;

            int? status = null;
            if (int.TryParse(query["status"], out var statusValue))
            {
                status = statusValue;
            }

            Guid? orgId = null;
            var orgIdRaw = query.TryGetValue("organizationId", out var organizationIdValue)
                ? organizationIdValue.ToString()
                : query["orgId"].ToString();
            if (Guid.TryParse(orgIdRaw, out var parsedOrgId))
            {
                orgId = parsedOrgId;
            }

            int? type = null;
            if (int.TryParse(query["type"], out var typeValue))
            {
                type = typeValue;
            }

            int? nature = null;
            if (int.TryParse(query["nature"], out var natureValue))
            {
                nature = natureValue;
            }

            var sort = query["sort"].ToString();
            var filter = query["filter"].ToString();
            var take = pageSize > 0 ? pageSize : 20;
            var skip = page > 1 ? (page - 1) * take : 0;

            var result = await _salaryService.GetPagingWithFilterAsync(skip, take, keyword, status, orgId, type, nature, sort, filter);
            return Ok(result);
        }

        /// <summary>
        /// [POST] Nhân bản thành phần lương
        /// Endpoint: POST /api/v1/SalaryCompositions/{id}/clone
        /// Copy toàn bộ dữ liệu, tạo mã mới có hậu tố "_Copy"
        /// </summary>
        [HttpPost("{id}/clone")]
        public async Task<IActionResult> Clone(Guid id)
        {
            var result = await _salaryService.CloneAsync(id);
            return StatusCode(201, result);
        }

        /// <summary>
        /// [PATCH] Cập nhật trạng thái thành phần lương
        /// Endpoint: PATCH /api/v1/SalaryCompositions/{id}/status
        /// Body: { "status": 0 } hoặc { "status": 1 }
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusUpdateDto dto)
        {
            var result = await _salaryService.ToggleStatusAsync(id, dto.Status);
            return Ok(result);
        }

        [HttpPost("bulk-import")]
        public async Task<IActionResult> BulkImport([FromBody] BulkImportRequest request)
        {
            var result = await _salaryService.BulkImportAsync(request.SystemIds, request.OrganizationId);
            return StatusCode(201, result);
        }

        // Bạn có thể định nghĩa DTO này ngay trong file Controller hoặc folder DTOs
    }

    /// <summary>
    /// DTO nhận giá trị trạng thái mới từ request body
    /// </summary>
    public class StatusUpdateDto
    {
        public int Status { get; set; }
    }

    public class BulkImportRequest
    {
        public List<Guid> SystemIds { get; set; } = new List<Guid>();
        public Guid OrganizationId { get; set; }
    }

}
