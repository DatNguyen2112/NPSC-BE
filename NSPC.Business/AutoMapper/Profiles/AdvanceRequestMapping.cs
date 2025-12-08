using AutoMapper;
using Microsoft.VisualBasic;
using NSPC.Business.Services.AdvanceRequest;
using NSPC.Business.Services.EInvoice;
using NSPC.Common;
using NSPC.Data.Data.Entity.AdvanceRequest;
using NSPC.Data.Data.Entity.EInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class AdvanceRequestMapping : Profile
    {
        public AdvanceRequestMapping() {
            // Entity to ViewModel mapping configuration
            CreateMap<sm_AdvanceRequest, AdvanceRequestViewModel>()
                .ForMember(dest => dest.Construction,
                    x => x.MapFrom(src => src.sm_Construction));
            CreateMap<sm_AdvanceRequestItems, AdvanceRequestItemsViewModel>();

            // ViewModel to Entity mapping configuration
            CreateMap<AdvanceRequestCreateUpdateModel, sm_AdvanceRequest>();
            CreateMap<AdvanceRequestItemsCreateUpdateModel, sm_AdvanceRequestItems>();
        }
    }
}
