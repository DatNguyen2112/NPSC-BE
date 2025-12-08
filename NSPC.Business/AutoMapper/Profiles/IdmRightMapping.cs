using AutoMapper;
using NSPC.Data;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class IdmRightMapping: Profile
    {
        public IdmRightMapping()
        {
            CreateMap<IdmRight, RightViewModel>();
            CreateMap<RightCreateUpdateModel, IdmRight>();
        }
    }
}

