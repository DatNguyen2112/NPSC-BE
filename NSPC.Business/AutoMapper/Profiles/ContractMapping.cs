using AutoMapper;
using NSPC.Business.Services.Contract;
using NSPC.Common;
using NSPC.Data.Data.Entity.Contract;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class ContractMapping : Profile
    {
        public ContractMapping()
        {
            // Entity to ViewModel mapping configuration
            CreateMap<AppendixAttachment, AppendixAttachmentViewModel>()
                .ForMember(dest => dest.FileUrl, x => x.MapFrom(src => Utils.FetchHost(src.FilePath)));
            CreateMap<ContractAppendixItem, ContractAppendixItemViewModel>();
            CreateMap<sm_Contract, ContractViewModel>()
                .IncludeAllDerived()
                .ForMember(dest => dest.TemplateStageName,
                    x => x.MapFrom(src => src.TemplateStage != null ? src.TemplateStage.Name : string.Empty))
                .ForMember(dest => dest.ImplementationStatus,
                    x => x.MapFrom(src => src.ImplementationStatus.ToString()))
                .ForMember(dest => dest.AcceptanceDocumentStatus,
                    x => x.MapFrom(src => src.AcceptanceDocumentStatus.ToString()))
                .ForMember(dest => dest.InvoiceStatus,
                    x => x.MapFrom(src => src.InvoiceStatus.ToString()))
                .ForMember(dest => dest.SupplementaryContractRequired,
                    x => x.MapFrom(src => src.SupplementaryContractRequired.ToString()))
                .ForMember(dest => dest.CreatedByUserName,
                    x => x.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : src.CreatedByUserName))
                .ForMember(dest => dest.LastModifiedByUserName,
                    x => x.MapFrom(src =>
                        src.LastModifiedByUser != null ? src.LastModifiedByUser.Name : src.LastModifiedByUserName));
            CreateMap<sm_Contract, ContractDetailViewModel>()
                .IncludeAllDerived();

            // ViewModel to Entity mapping configuration
            CreateMap<ContractCreateUpdateModel, sm_Contract>();
        }
    }
}