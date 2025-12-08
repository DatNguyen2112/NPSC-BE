using NSPC.Common;

namespace NSPC.Business.Services.ProjectTemplate
{
    public interface IProjectTemplateHandler
    {
        Task<Response<ProjectTemplateViewModel>> Create(ProjectTemplateCreateUpdateModel model);
        Task<Response<ProjectTemplateViewModel>> Update(Guid id, ProjectTemplateCreateUpdateModel model);
        Task<Response<ProjectTemplateViewModel>> GetById(Guid id);
        Task<Response<Pagination<ProjectTemplateViewModel>>> GetPage(ProjectTemplateQueryModel query);
        Task<Response<ProjectTemplateViewModel>> Delete(Guid id);
    }
}
