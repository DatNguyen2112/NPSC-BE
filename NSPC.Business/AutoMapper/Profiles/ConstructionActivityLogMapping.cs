using AutoMapper;
using NSPC.Business.Services;
using NSPC.Data.Entity;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class ConstructionActivityLogMapping: Profile
    {
        public ConstructionActivityLogMapping()
        {
            CreateMap<sm_ConstructionActivityLog, ConstructionActivityLogViewModel>();
            CreateMap<ConstructionActivityLogCreateModel, sm_ConstructionActivityLog>();
        }
    }
}
