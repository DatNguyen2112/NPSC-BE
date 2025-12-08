using NSPC.Common;
using NSPC.Data;
using System;

namespace NSPC.Business
{
    public class BaseParameterModel : BaseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupCode { get; set; }
        public bool IsSystem { get; set; }
    }

    public class ParameterModel : BaseParameterModel
    {
        public string Description { get; set; }
    }

    public class ParameterQueryModel : PaginationRequest
    {
        public string Value { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
    }

    public class ParameterCreateModel
    {
        public string Name { get; set; }

        public string GroupCode { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public class ParameterUpdateManyModel
    {
        public List<ParameterUpdateValueModel> List { get; set; }
    }

    public class ParameterUpdateValueModel
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    public class ParameterUpdateModel
    {
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}