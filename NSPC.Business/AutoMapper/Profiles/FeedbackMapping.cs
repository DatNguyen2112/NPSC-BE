using AutoMapper;
using NSPC.Business.Services;
using NSPC.Business.Services.Feedback;
using NSPC.Common;
using NSPC.Data.Data.Entity.Feedback;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class FeedbackMapping : Profile
    {
        public FeedbackMapping()
        {
            CreateMap<sm_Feedback, FeedbackViewModel>();
            CreateMap<sm_Feedback, FeedbackCreateModel>();

            CreateMap<FeedbackCreateModel, sm_Feedback>();
            CreateMap<FeedbackViewModel, sm_Feedback>();
        }
    }
}
