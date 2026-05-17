using MISA.Salary.Common.Model;
using MISA.Salary.DL.Base;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.DL.Repositories
{
    /// <summary>
    /// Repository cụ thể cho Đơn vị công tác
    /// Sử dụng hoàn toàn các phương thức từ BaseRepository
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
