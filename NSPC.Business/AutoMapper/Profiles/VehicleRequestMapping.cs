using AutoMapper;
using NSPC.Business.Services.VehicleRequest;
using NSPC.Data.Data.Entity.VehicleRequest;
using NSPC.Data.Data.Entity.ActivityHistory;
using System;

namespace NSPC.Business.AutoMapper.Profiles
{
    /// <summary>
    /// AutoMapper profile for VehicleRequest entity
    /// </summary>
    public class VehicleRequestMapping : Profile
    {
        public VehicleRequestMapping()
        {
            // Entity to ViewModel mapping configuration
            CreateMap<sm_VehicleRequest, VehicleRequestViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));

            // Entity to DetailViewModel mapping configuration
            CreateMap<sm_VehicleRequest, VehicleRequestDetailViewModel>()
                .IncludeBase<sm_VehicleRequest, VehicleRequestViewModel>()
                .ForMember(dest => dest.CreatedByUserName,
                    opt =>
                        opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : src.CreatedByUserName));

            // Activity history mapping
            CreateMap<sm_ActiviyHisroty, ActivityHistoryViewModel>();

            // ViewModel to Entity mapping configuration
            CreateMap<VehicleRequestCreateUpdateModel, sm_VehicleRequest>()
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => Enum.Parse<VehicleRequestPriority>(src.Priority)));
        }
    }
}