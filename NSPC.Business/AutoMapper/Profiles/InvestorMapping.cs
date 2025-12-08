using AutoMapper;
using NSPC.Business.Services;
using NSPC.Business.Services.InvestorType;
using NSPC.Data.Entity;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class InvestorMapping: Profile
    {
        public InvestorMapping()
        {
            CreateMap<sm_Investor, InvestorViewModel>()
                .ForMember(dest => dest.InvestorType,
                    x => x.MapFrom(src => src.InvestorType));
            CreateMap<InvestorCreateModel, sm_Investor>();
            
            CreateMap<sm_InvestorType, InvestorTypeViewModel>()
                .ForMember(dest => dest.Investor,
                    x => x.MapFrom(src => src.Investor));
            CreateMap<InvestorTypeCreateModel, sm_InvestorType>();

            CreateMap<sm_Investor, InvestorDTO>();
        }
    }
}
