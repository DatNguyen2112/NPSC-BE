using NSPC.Business.Services;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class SearchSampleModel
    {

        public Guid Id { get; set; }
        public string Title { get; set; }
        public PurchaseOrderQueryModel Query { get; set; }
    }

    public class SearchSampleCreateUpdateModel
    {
        public string Title { get; set; }
        public string QueryJsonString { get; set; }
    }

    public class SearchSampleQueryModel : PaginationRequest
    {
    }
}
