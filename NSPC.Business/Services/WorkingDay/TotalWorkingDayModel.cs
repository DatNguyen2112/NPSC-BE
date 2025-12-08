using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.WorkingDay
{
    public class TotalWorkingDayViewModel
    {
        public Guid id { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? TotalWorkingDay { get; set; }
    }
}
