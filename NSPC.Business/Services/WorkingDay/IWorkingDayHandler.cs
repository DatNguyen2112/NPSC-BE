using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.WorkingDay
{
    public interface IWorkingDayHandler
    {
        Task<Response> BootstrapWorkingDays(int year);
        Task<Response<List<WorkingDayViewModel>>> GetWorkingDays(int year, int? month, int? day);
        Task<Response> UpdateWorkingDay(int year, int month, int day, WorkingDayUpdateModel model);
        Task<Response<List<TotalWorkingDayViewModel>>> GetTotalWorkingDay(int year);
    }
}
