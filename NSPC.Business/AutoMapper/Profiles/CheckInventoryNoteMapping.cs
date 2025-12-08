using AutoMapper;
using NSPC.Business.Services;
using NSPC.Common;
using NSPC.Data.Entity;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class CheckInventoryNoteMapping: Profile
    {
        public CheckInventoryNoteMapping()
        {
            CreateMap<sm_InventoryCheckNoteItems, InventoryCheckNoteItemViewModel>()
                .ForMember(dest => dest.Reason,
                    x => x.MapFrom(
                        src => CodeTypeCollection.Instance.FetchCode(src.ReasonInventory, LanguageConstants.Default, src.TenantId)));

            CreateMap<sm_InventoryCheckNote, InventoryCheckNoteViewModel>()
                .ForMember(dest => dest.Ware, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.WareCode, LanguageConstants.Default, src.TenantId)))
                .ForMember(dest => dest.Items, x => x.MapFrom(src => src.Items));
            

            CreateMap<InventoryCheckNoteItemCreateUpdateModel, sm_InventoryCheckNoteItems>();

            CreateMap<InventoryCheckNoteCreateUpdateModel, sm_InventoryCheckNote>();
        }
    } 
}
