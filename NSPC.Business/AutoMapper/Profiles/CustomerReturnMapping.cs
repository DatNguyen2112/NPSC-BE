using NSPC.Business.Services;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using NSPC.Common;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class CustomerReturnMapping: Profile
    {
        public CustomerReturnMapping()
        {
            CreateMap<sm_Return_Order_Item, CustomerReturnItemViewModel>();

            CreateMap<sm_Return_Order, CustomerReturnViewModel>()
            .ForMember(dest => dest.Ware, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.WareCode, LanguageConstants.Default, src.TenantId)))
            .ForMember(dest => dest.Reason, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.ReasonCode, LanguageConstants.Default, src.TenantId)))
            .ForMember(dest => dest.Project, x => x.MapFrom(src => src.mk_DuAn))
            .ForMember(dest => dest.OrderItems, x => x.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.ConstructionName, x => x.MapFrom(src => src.sm_Construction.Name));

            CreateMap<CustomerReturnItemCreateUpdateModel, sm_Return_Order_Item>();

            CreateMap<CustomerReturnCreateUpdateModel, sm_Return_Order>();


        }
    }
}
