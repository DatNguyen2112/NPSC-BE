using System;
using System.Collections.Generic;

namespace NSPC.Business
{
    public class BaseNavigationModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string IdPath { get; set; }
        public string Path { get; set; }
        public int Level { get; set; }
        public int? Order { get; set; }
        public bool? Status { get; set; }
        public string QueryParams { get; set; }
    }

    public class NavigationModel : BaseNavigationModel
    {
        public Guid? ParentId { get; set; }
        public string UrlRewrite { get; set; }
        public string IconClass { get; set; }
        public List<NavigationModel> SubChild { get; set; }
        public List<Guid> RoleList { get; set; }
        public string SubUrl { get; set; }
        public int? Type { get; set; }
    }

    // Create request model
    public class NavigationCreateModel
    {
        public NavigationCreateModel()
        {
            Status = true;
            Order = 0;
        }

        public BaseNavigationModel ParentModel { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public int? Order { get; set; }
        public string UrlRewrite { get; set; }
        public string IconClass { get; set; }
        public List<Guid> RoleList { get; set; }
        public string SubUrl { get; set; }
        public int? Type { get; set; }
        public string QueryParams { get; set; }
    }

    // UpdatePaid request model
    public class NavigationUpdateModel
    {
        public BaseNavigationModel ParentModel { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public int? Order { get; set; }
        public string UrlRewrite { get; set; }
        public string IconClass { get; set; }
        public Guid? FromSubNavigation { get; set; }
        public List<Guid> RoleList { get; set; }
        public string SubUrl { get; set; }
        public int? Type { get; set; }
        public string QueryParams { get; set; }
    }
}