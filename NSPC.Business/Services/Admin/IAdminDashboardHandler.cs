using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IAdminDashboardHandler
    {
        Task<Response<AdminDashboardViewModel>> GetAdminInfo();
        /*Task<Response<List<MonthlyStatisticModel>>> GetMonthlyStatistic();
        Task<Response<List<PackageStatisticModel>>> GetPackageStatistic();*/
    }
}
