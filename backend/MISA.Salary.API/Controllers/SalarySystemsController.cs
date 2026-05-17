using Microsoft.AspNetCore.Mvc;
using MISA.Salary.BL.Base;
using MISA.Salary.Common.Model;
using MISA.Salary.BL.Interfaces;

namespace MISA.Salary.API.Controllers
{
    /// <summary>
    /// Controller xử lý danh mục Thành phần lương hệ thống
    /// </summary>
    [Route("api/v1/salary-systems")]
    public class SalarySystemsController : BaseController<SalarySystem>
    {
        public SalarySystemsController(ISalarySystemService service) : base(service)
        {
        }

        // BaseController đã cung cấp sẵn GetAll và GetPaging ("paging")
        // Giữ nguyên Route theo prompt.yaml
    }
}
