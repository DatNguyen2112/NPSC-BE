using NSPC.Business.Services.WorkItem;
using NSPC.Common;
namespace NSPC.Business.Services.TemplateStage
{
    public class TemplateStageViewModel
    {
        public Guid Id { get; set; }
        public int StepOrder { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }
    public class TemplateStageCreateUpdateModel
    {
        public Guid? Id { get; set; }
        public int StepOrder { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ProjectTemplateId { get; set; }
        public List<TaskCreateUpdateModel> Tasks { get; set; }
    }
    public class TemplateStageQueryModel : PaginationRequest
    {

    }
}
