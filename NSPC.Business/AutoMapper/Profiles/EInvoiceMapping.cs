using AutoMapper;
using NSPC.Business.Services.EInvoice;
using NSPC.Data.Data.Entity.EInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class EInvoiceMapping : Profile
    {
        public EInvoiceMapping()
        {
            // Entity to ViewModel mapping configuration
            CreateMap<sm_EInvoice, EInvoiceViewModel>();
            CreateMap<sm_EInvoiceItems, EInvoiceItemsViewModel>();
            CreateMap<sm_EInvoiceVatAnalytics, EInvoiceVatAnalyticsViewModel>();

            // ViewModel to Entity mapping configuration
            CreateMap<EInvoiceCreateUpdateModel, sm_EInvoice>();
            CreateMap<EInvoiceItemsCreateUpdateModel, sm_EInvoiceItems>();
            CreateMap<EInvoiceVatAnalyticsCreateUpdateModel, sm_EInvoiceVatAnalytics>();
        }
    }
}
