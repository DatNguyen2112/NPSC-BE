using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services;
using NSPC.Data.Entity;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class CustomerServiceCommentMapping: Profile
    {
        public CustomerServiceCommentMapping()
        {
            CreateMap<sm_CustomerServiceComment, CustomerServiceCommentViewModel>()
                .ForMember(dest => dest.AvatarUrl, x => x.MapFrom(src => src.CreatedByUser.AvatarUrl));
            CreateMap<CustomerServiceCommentCreateModel, sm_CustomerServiceComment>();
        }
    }
}

