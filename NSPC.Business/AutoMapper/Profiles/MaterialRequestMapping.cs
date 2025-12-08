using AutoMapper;
using NSPC.Data.Entity;
using NSPC.Business.Services;
using NSPC.Common;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class MaterialRequestMapping: Profile
    {
        public MaterialRequestMapping()
        {
            CreateMap<sm_MaterialRequest, MaterialRequestViewModel>()
                .ForMember(dest => dest.Construction,
                    x => x.MapFrom(src => src.Construction))
                .ForMember(dest => dest.MaterialRequestItems,
                    x => x.MapFrom(src => src.MaterialRequestItems))
                .ForMember(dest => dest.ListHistoryProcess,
                    x => x.MapFrom(src => src.ListHistoryProcess.ToList().OrderByDescending(x => x.CreatedOnDate)));
            CreateMap<MaterialRequestCreateUpdateModel, sm_MaterialRequest>();
            
            // Mapping Material Request Item
            CreateMap<sm_MaterialRequestItem, MaterialRequestItemViewModel>()
                .ForMember(dest => dest.UnitPrice,
                    x => x.MapFrom(src => src.sm_Product.PurchaseUnitPrice))
                .ForMember(dest => dest.ImportVATPercent,
                    x => x.MapFrom(src => src.sm_Product.ImportVATPercent));
            CreateMap<MaterialRequestItemCreateUpdateModel, sm_MaterialRequestItem>();
        }
    }
}
