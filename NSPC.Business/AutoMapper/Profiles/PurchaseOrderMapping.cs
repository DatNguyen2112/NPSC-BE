using AutoMapper;
using NSPC.Business.Services;
using NSPC.Common;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class PurchaseOrderMapping : Profile
    {
        public PurchaseOrderMapping()
        {
            CreateMap<sm_PurchaseOrderItem, PurchaseOrderItemViewModel>();

            CreateMap<sm_PurchaseOrder, PurchaseOrderViewModel>()
            .ForMember(dest => dest.Ware, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.WareCode, LanguageConstants.Default, src.TenantId)))
            .ForMember(dest => dest.Items, x => x.MapFrom(src => src.Items))
            .ForMember(dest => dest.Supplier, x => x.MapFrom(src => src.sm_Supplier))
            .ForMember(dest => dest.Project, x => x.MapFrom(src => src.mk_DuAn))
            .ForMember(dest => dest.Contract, x => x.MapFrom(src => src.sm_Contract))
            .ForMember(dest => dest.Construction, x => x.MapFrom(src => src.sm_Construction));;
            //PurchaseOrderItem
            CreateMap<PurchaseOrderItemCreateUpdateModel, sm_PurchaseOrderItem>();

            //PurchaseOrder
            CreateMap<PurchaseOrderCreateUpdateModel, sm_PurchaseOrder>();


        }
    }
}
