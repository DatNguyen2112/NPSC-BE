using NSPC.Business.Services.NguyenVatLieu;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.WorkingDay
{
    public class WorkingDayViewModel
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int LunarDay { get; set; }
        public int LunarMonth { get; set; }
        public int LunarYear { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public bool IsOverride { get; set; }
        public string LastModifiedByUserName { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }
    public class WorkingDayUpdateModel
    {
        public string Type { get; set; }
        public string Note { get; set; }
    }
    
    public class WorkingDayQueryModel : PaginationRequest
    {
    }
}
