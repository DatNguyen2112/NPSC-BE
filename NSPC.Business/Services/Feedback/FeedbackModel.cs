using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.Feedback
{
    public class FeedbackViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string[] Module { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; }
    } 
    public class FeedbackCreateModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Module { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; }
    }
}
