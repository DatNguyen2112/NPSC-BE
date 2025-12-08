using AutoMapper;
using NSPC.Data;
using NSPC.Data.Entity;
using NSPC.Business.Services;
using NSPC.Common;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class SocialMediaMapping : Profile
    {
        public SocialMediaMapping()
        {
            // Social Post mappings
            CreateMap<sm_SocialPost, SocialPostViewModel>()
                .ForMember(dest => dest.Comments,
                    x => x.MapFrom(src => src.Comments.Where(c => c.ParentCommentId == null)
                        .OrderByDescending(c => c.CreatedOnDate)))
                .ForMember(dest => dest.Reactions,
                    x => x.MapFrom(src => src.Reactions));

            CreateMap<sm_SocialPost, SocialPostDetailViewModel>()
                .IncludeAllDerived();

            CreateMap<SocialPostCreateUpdateModel, sm_SocialPost>();

            // Social Comment mappings
            CreateMap<sm_SocialComment, SocialCommentViewModel>()
                .ForMember(dest => dest.ChildComments,
                    x => x.MapFrom(src => src.ChildComments.OrderBy(c => c.CreatedOnDate)))
                .ForMember(dest => dest.Reactions,
                    x => x.MapFrom(src => src.Reactions));

            CreateMap<SocialCommentCreateUpdateModel, sm_SocialComment>();

            // Reaction mappings
            CreateMap<jsonb_SocialReaction, SocialReactionViewModel>()
                .ForMember(dest => dest.Reaction,
                    x => x.MapFrom(src => src.Reaction.ToString()));
        }
    }
}