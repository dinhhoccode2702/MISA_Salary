using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MISA.Salary.Common.Model;
using MISA.Salary.DL.Base;
using MISA.Salary.DL.Interfaces;

namespace MISA.Salary.DL.Repositories
{
    /// <summary>
    /// Repository cụ thể cho Danh mục thành phần lương hệ thống
    /// Author: MISA (11/05/2026)
    /// </summary>
    public class SalarySystemRepository : BaseRepository<SalarySystem>, ISalarySystemRepository
    {
        public SalarySystemRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
