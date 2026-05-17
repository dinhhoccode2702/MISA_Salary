using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISA.Salary.BL.Base;
using MISA.Salary.Common.DTOs;

namespace MISA.Salary.API.Controllers
{
    /// <summary>
    /// Controller cơ sở (Generic Pattern)
    /// Định nghĩa sẵn các HTTP Methods CRUD chuẩn cho tất cả các entity
    /// Các Controller cụ thể kế thừa và có thể thêm endpoint riêng
    /// Author: MISA (10/05/2026)
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    [ApiController]
    public class BaseController<T> : ControllerBase where T : class
    {
        /// <summary>
        /// Service tương ứng (inject qua constructor)
        /// </summary>
        protected readonly IBaseService<T> _service;

        public BaseController(IBaseService<T> service)
        {
            _service = service;
        }

        /// <summary>
        /// [GET] Lấy tất cả bản ghi
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// [GET] Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID bản ghi (GUID)</param>
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// [GET] Lấy danh sách phân trang
        /// </summary>
        /// <param name="page">Số trang (mặc định 1)</param>
        /// <param name="pageSize">Số bản ghi/trang (mặc định 20)</param>
        /// <param name="search">Từ khóa tìm kiếm</param>
        [HttpGet("paging")]
        public virtual async Task<IActionResult> GetPaging(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            var result = await _service.GetPagingAsync(page, pageSize, search);
            return Ok(result);
        }

        /// <summary>
        /// [POST] Thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Dữ liệu entity từ request body</param>
        [HttpPost]
        public virtual async Task<IActionResult> Insert([FromBody] T entity)
        {
            var result = await _service.InsertAsync(entity);
            return StatusCode(201, result);
        }

        /// <summary>
        /// [PUT] Cập nhật bản ghi theo ID
        /// </summary>
        /// <param name="id">ID bản ghi cần cập nhật</param>
        /// <param name="entity">Dữ liệu mới từ request body</param>
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] T entity)
        {
            var result = await _service.UpdateAsync(entity, id);
            return Ok(result);
        }

        /// <summary>
        /// [DELETE] Xóa bản ghi theo ID
        /// </summary>
        /// <param name="id">ID bản ghi cần xóa</param>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// [DELETE] Xóa nhiều bản ghi (batch delete)
        /// </summary>
        /// <param name="ids">Danh sách ID cần xóa từ request body</param>
        [HttpDelete("batch")]
        public virtual async Task<IActionResult> DeleteMany([FromBody] List<Guid> ids)
        {
            var result = await _service.DeleteManyAsync(ids);
            return Ok(result);
        }
    }
}
