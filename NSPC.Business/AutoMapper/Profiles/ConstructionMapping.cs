using AutoMapper;
using MongoDB.Driver.Linq;
using NSPC.Business.Services;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Entity;
using NSPC.Common;
using NSPC.Data.Data.Entity.AdvanceRequest;
using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Data.Entity.InventoryNote;
using NSPC.Business.Services.ExecutionTeams;
using NSPC.Data;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class ConstructionMapping: Profile
    {
        public ConstructionMapping()
        {

            CreateMap<sm_Construction, ConstructionViewModel>()
                .ForMember(dest => dest.Contracts,
                    x => x.MapFrom(src => src.sm_Contract))
                .ForMember(dest => dest.ProjectTemplate,
                    x => x.MapFrom(src => src.sm_ProjectTemplate))
                .ForMember(dest => dest.ActivityLogs,
                    x => x.MapFrom(src => src.sm_ConstructionActivityLog.OrderByDescending(x => x.CreatedOnDate)))
                .ForMember(dest => dest.ExecutionTeams,
                    x => x.MapFrom(src => src.sm_ExecutionTeams))
                .ForMember(dest => dest.IssueManagements,
                    x => x.MapFrom(src => src.sm_IssueManagements))
                .ForMember(dest => dest.IsHasIssue,
                    x => x.MapFrom(src => src.sm_IssueManagements.Any(x => x.Status != StatusIssue.COMPLETED)))
                .ForMember(dest => dest.Voltage,
                    x => x.MapFrom(src =>
                        CodeTypeCollection.Instance.FetchCode(src.VoltageTypeCode, LanguageConstants.Default,
                            src.TenantId)))
                .ForMember(dest => dest.Investor,
                    x => x.MapFrom(src => src.sm_Investor));
            CreateMap<ConstructionCreateUpdateModel, sm_Construction>()
                .ForMember(dest => dest.sm_ExecutionTeams,
                    x => x.MapFrom(src => src.ExecutionTeams));

            CreateMap<sm_ExecutionTeams, ExecutionTeamsViewModel>()
                .ForMember(dest => dest.PhongBan,
                    x => x.MapFrom(src =>
                        CodeTypeCollection.Instance.FetchCode(src.MaPhongBan, LanguageConstants.Default,
                            src.TenantId)))
                .ForMember(dest => dest.ToThucHien,
                    x => x.MapFrom(src => CodeTypeItemsCollection.Instance.FetchItemsCode(src.MaTo, LanguageConstants.Default, src.TenantId)));
            CreateMap<ExecutionTeamsCreateModel, sm_ExecutionTeams>();
            
            // Mapping Investor DTO
            CreateMap<sm_Investor, InvestorDTOViewModel>();
            CreateMap<sm_InvestorType, InvestorTypeDTO>();
            
            // Mapping Issue Management DTO
            CreateMap<sm_IssueManagement, IssueManagementDTO>();
            
            // Mapping Task View DTO
            CreateMap<sm_Task, TaskViewModelDTO>()
                .ForMember(dest => dest.TemplateStageId,
                    x => x.MapFrom(src => src.IdTemplateStage));
            
            // Mapping Contract DTO
            CreateMap<sm_Contract, ContractDTO>()
                .ForMember(dest => dest.ConsultingService,
                    x => x.MapFrom(src => src.ConsultingService))
                .ForMember(dest => dest.ImplementationStatus,
                    x => x.MapFrom(src => src.ImplementationStatus.ToString()))
                .ForMember(dest => dest.CreatedByUserName,
                    x => x.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : src.CreatedByUserName))
                .ForMember(dest => dest.LastModifiedByUserName,
                    x => x.MapFrom(src =>
                        src.LastModifiedByUser != null ? src.LastModifiedByUser.Name : src.LastModifiedByUserName));
        }
    }
}

