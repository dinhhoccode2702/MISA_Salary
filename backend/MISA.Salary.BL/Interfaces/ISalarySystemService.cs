using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Model;
using MISA.Salary.BL.Base;

namespace MISA.Salary.BL.Interfaces
{
    /// <summary>
    /// Interface Service cho Danh mục thành phần lương hệ thống
    /// Author: MISA (11/05/2026)
    /// </summary>
    public interface ISalarySystemService : IBaseService<SalarySystem>
    {
        /// <summary>
        /// Đưa một hoặc nhiều thành phần lương từ danh mục hệ thống vào danh sách sử dụng
        /// </summary>
        /// <param name="ids">Danh sách ID thành phần lương hệ thống</param>
        /// <param name="organizationId">ID đơn vị sử dụng</param>
        Task<ServiceResult> AddToListAsync(List<Guid> ids, Guid organizationId);
    }
}
