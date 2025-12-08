using AutoMapper;
using NSPC.Business.Services.InventoryNote;
using NSPC.Data.Data.Entity.InventoryNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class InventoryNoteMapping : Profile
    {
        public InventoryNoteMapping() {
            // Entity to ViewModel mapping configuration
            CreateMap<sm_InventoryNote, InventoryNoteViewModel>()
                .ForMember(dest => dest.InventoryName, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.InventoryCode, "vn", src.TenantId).Title))
                .ForMember(dest => dest.TransactionTypeName, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.TransactionTypeCode, "vn", src.TenantId).Title))
                .ForMember(dest => dest.Contract,
                    x => x.MapFrom(src => src.sm_Contract))
                .ForMember(dest => dest.Construction,
                    x => x.MapFrom(src => src.sm_Construction));
            CreateMap<sm_InventoryNoteItem, InventoryNoteItemViewModel>();
            CreateMap<sm_InventoryNote, InventoryNoteViewModelInProject>()
                .ForMember(dest => dest.InventoryName, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.InventoryCode, "vn", src.TenantId).Title));

            // ViewModel to Entity mapping configuration
            CreateMap<InventoryNoteCreateUpdateModel, sm_InventoryNote>();
            CreateMap<InventoryNoteItemCreateUpdateModel, sm_InventoryNoteItem>();
        }
    }
}
