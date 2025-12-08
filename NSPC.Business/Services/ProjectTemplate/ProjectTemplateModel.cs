using NSPC.Business.Services.TemplateStage;
using NSPC.Common;
namespace NSPC.Business.Services.ProjectTemplate
{
    public class ProjectTemplateViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TemplateStageViewModel> TemplateStages { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
    public class ProjectTemplateCreateUpdateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TemplateStageCreateUpdateModel> TemplateStages { get; set; }
    }
    public class ProjectTemplateQueryModel : PaginationRequest
    {

    }
}
