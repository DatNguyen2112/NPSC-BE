using NSPC.Common;
using System;
using System.Collections.Generic;

namespace NSPC.Business
{
    public class BaseRoleModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
        public int Level { get; set; }
    }

    public class RoleModel : BaseRoleModel
    {
        public string Description { get; set; }
    }

    public class RoleDetailModel : RoleModel
    {
        public List<BaseUserModel> ListUser { get; set; }
    }

    public class RoleQueryModel : PaginationRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? IsEmployee{ get; set; }
    }

    public class RoleCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ApplicatonId { get; set; }

        //Plus
        /// <summary>
        /// Danh sách quyền thêm
        /// </summary>
        public List<Guid> ListAddRightId { get; set; }

        /// <summary>
        /// Danh sách người dùng thêm
        /// </summary>
        public List<Guid> ListAddUserId { get; set; }
    }

    public class RoleUpdateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ApplicatonId { get; set; }

        //Plus
        /// <summary>
        /// Danh sách quyền thêm
        /// </summary>
        public List<Guid> ListAddRightId { get; set; }

        /// <summary>
        /// Danh sách người dùng thêm
        /// </summary>
        public List<Guid> ListAddUserId { get; set; }

        //Plus
        /// <summary>
        /// Danh sách quyền xóa
        /// </summary>
        public List<Guid> ListDeleteRightId { get; set; }

        /// <summary>
        /// Danh sách người dùng xóa
        /// </summary>
        public List<Guid> ListDeleteUserId { get; set; }
    }

    public class RoleUserModel
    {
        public Guid Id { get; set; }
    }

    public class UserCreateRoleModel
    {
        public List<Guid> Ids { get; set; }
    }

    public class CopyRoleUserModel
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
    }
}