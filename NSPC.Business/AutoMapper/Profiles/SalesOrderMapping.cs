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
    public class SalesOrderMapping : Profile
    {
        public SalesOrderMapping()
        {
            CreateMap<sm_SalesOrderItem, SalesOrderItemViewModel>();

            CreateMap<sm_SalesOrder, SalesOrderViewModel>()
            .ForMember(dest => dest.Ware, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.WareCode, LanguageConstants.Default, src.TenantId)))
            .ForMember(dest => dest.PaymentMethod, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.PaymentMethodCode, LanguageConstants.Default, src.TenantId)))
            .ForMember(dest => dest.Status, x => x.MapFrom(src => SalesOrderConstants.FetchStatus(src.StatusCode)))
            .ForMember(dest => dest.Items, x => x.MapFrom(src => src.SalesOrderItems))
            .ForMember(dest => dest.Project, x => x.MapFrom(src => src.mk_DuAn))
            .ForMember(dest => dest.Customer, x => x.MapFrom(src => src.sm_Customer))
            .ForMember(dest => dest.Construction, x => x.MapFrom(src => src.sm_Construction))
            .ForMember(dest => dest.Contract, x => x.MapFrom(src => src.sm_Contract));

            //PurchaseOrderItem
            CreateMap<SalesOrderItemCreateUpdateModel, sm_SalesOrderItem>();

            //PurchaseOrder
            CreateMap<SalesOrderCreateUpdateModel, sm_SalesOrder>();


        }
    }
}
