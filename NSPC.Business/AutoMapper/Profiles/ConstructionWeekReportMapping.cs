using AutoMapper;
using NSPC.Data.Entity;
using NSPC.Business.Services.ConstructionWeekReport;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class ConstructionWeekReportMapping: Profile
    {
        public ConstructionWeekReportMapping()
        {
            CreateMap<sm_ConstructionWeekReport, ConstructionWeekReportViewModel>();
            CreateMap<ConstructionWeekReportCreateModel, sm_ConstructionWeekReport>();
        }
    }
}
