using AutoMapper;
using NSPC.Data;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class IdmRightMapRoleMapping : Profile
    {
        public IdmRightMapRoleMapping()
        {
            CreateMap<IdmRightMapRole, RightMapRoleViewModel>()
                .ForMember(dest => dest.RightName, x => x.MapFrom(src => src.Right.Name));
        }
    }
}

