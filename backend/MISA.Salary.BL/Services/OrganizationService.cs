using MISA.Salary.BL.Base;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.BL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ cho Đơn vị công tác
    /// Sử dụng hoàn toàn logic từ BaseService
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class OrganizationService : BaseService<Organization>, IOrganizationService
    {
        public OrganizationService(IOrganizationRepository repository) : base(repository)
        {
        }
    }
}
