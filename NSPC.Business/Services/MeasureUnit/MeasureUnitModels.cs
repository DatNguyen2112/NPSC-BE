using NSPC.Common;

namespace NSPC.Business;

public class MeasureUnitViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public string CreatedByUserName { get; set; }
    public DateTime LastModifiedOnDate { get; set; }
    public string LastModifiedByUserName { get; set; }
}

public class MeasureUnitCreateUpdateModel
{
    public string Code { get; set; }
    public string Name { get; set; }
}

public class MeasureUnitQueryModel : PaginationRequest
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string FullTextSearch { get; set; }
}

