using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSPC.Business
{
    public class ApplicationModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ApplicationCreateUpdateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }

    public class ApplicationQueryModel : PaginationRequest
    {

    }
}
