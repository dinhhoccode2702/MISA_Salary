using Microsoft.AspNetCore.Mvc;
using MISA.Salary.BL.Base;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.Common.Model;

namespace MISA.Salary.API.Controllers
{
    /// <summary>
    /// Controller xử lý danh mục Cơ cấu tổ chức
    /// </summary>
    [Route("api/v1/organizations")]
    public class OrganizationsController : BaseController<Organization>
    {
        public OrganizationsController(IOrganizationService service) : base(service)
        {
        }
    }
}
