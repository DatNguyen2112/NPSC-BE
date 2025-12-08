using AutoMapper;
using NSPC.Business.Services;
using NSPC.Data.Entity;
using NSPC.Common;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class WarehouseTransferNoteMapping : Profile
    {
        public WarehouseTransferNoteMapping()
        {
            CreateMap<sm_WarehouseTransferNoteItem, WarehouseTransferNoteItemViewModel>();

            CreateMap<sm_WarehouseTransferNote, WarehouseTransferNoteViewModel>()
                .ForMember(dest => dest.ExportWarehouse,
                    x => x.MapFrom(
                        src => CodeTypeCollection.Instance.FetchCode(src.ExportWarehouseCode, LanguageConstants.Default, src.TenantId)))
                .ForMember(dest => dest.ImportWarehouse,
                    x => x.MapFrom(
                        src => CodeTypeCollection.Instance.FetchCode(src.ImportWarehouseCode, LanguageConstants.Default, src.TenantId)))
                .ForMember(dest => dest.Items, x => x.MapFrom(src => src.Items));
            
            CreateMap<WarehouseTransferNoteItemCreateUpdateModel, sm_WarehouseTransferNoteItem>();
            CreateMap<WarehouseTransferNoteCreateModel, sm_WarehouseTransferNote>();
        }
    }
}

