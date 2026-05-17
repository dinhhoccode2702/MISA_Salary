using MISA.Salary.Common.Model;
using MISA.Salary.DL.Base;

namespace MISA.Salary.DL.Interfaces
{
    /// <summary>
    /// Interface Repository cho Đơn vị công tác
    /// Kế thừa IBaseRepository, hiện tại chưa cần phương thức riêng
    /// Author: MISA (10/05/2026)
    /// </summary>
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
    }
}
