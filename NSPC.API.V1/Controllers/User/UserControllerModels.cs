using System;
using System.Collections.Generic;

namespace NSPC.Api
{
    public class UpdateSystemRightRoleMapAppsRequest
    {
        public List<Guid> ListApplicationId { get; set; }
        public List<Guid> ListRightId { get; set; }
        public List<Guid> ListRoleId { get; set; }
    }
}