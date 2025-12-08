using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSPC.Business
{
    public static class InHelper
    {
        /// <summary>
        ///     Tạo Id quyền riêng
        /// </summary>
        /// <returns></returns>
        public static Guid MakeIndependentPermission()
        {
            return new Guid("00000000-0000-0000-0000-000000000000");
        }

        /// <summary>
        ///     Lấy về danh sách RoleId đã kế thừa từ string
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <returns></returns>
        public static List<Guid> LoadRolesInherited(string currInheritedFromRoles)
        {
            try
            {
                if (string.IsNullOrEmpty(currInheritedFromRoles))
                {
                    var result = new List<Guid>();
                    return result;
                }
                var jsonObject = JsonConvert.DeserializeObject<List<Guid>>(currInheritedFromRoles);

                var newlist = new List<Guid>();

                foreach (var item in jsonObject)
                    if (!newlist.Contains(item))
                        newlist.Add(item);
                jsonObject = jsonObject.GroupBy(role => role)
                    .Select(g => g.First())
                    .ToList();

                return jsonObject;
            }
            catch (Exception)
            {
                var result = new List<Guid>();
                return result;
            }
        }

        /// <summary>
        ///     Sinh ra chuỗi Json kế thừa từ 1 danh sách RoleId lưu vào DB
        /// </summary>
        /// <param name="listRoleId"></param>
        /// <returns></returns>
        public static string GenRolesInherited(List<Guid> listRoleId)
        {
            try
            {
                var jsonStr = JsonConvert.SerializeObject(listRoleId);
                return jsonStr;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        ///     Sinh ra chuỗi Json kế thừa từ 1 RoleId lưu vào DB
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GenRolesInherited(Guid roleId)
        {
            var roles = new List<Guid>
            {
                roleId
            };
            return GenRolesInherited(roles);
        }

        /// <summary>
        ///     Bỏ 1 RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string RemoveRolesInherited(string currInheritedFromRoles, Guid roleId)
        {
            var roles = LoadRolesInherited(currInheritedFromRoles) ?? new List<Guid>();
            if (roles.Contains(roleId)) roles.Remove(roleId);
            var result = GenRolesInherited(roles);
            return result;
        }

        /// <summary>
        ///     Bỏ 1 danh sách RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="listRoleId"></param>
        /// <returns></returns>
        public static string RemoveRolesInherited(string currInheritedFromRoles, List<Guid> listRoleId)
        {
            var result = currInheritedFromRoles;
            if (listRoleId == null)
                listRoleId = new List<Guid>();
            foreach (var role in listRoleId) result = RemoveRolesInherited(result, role);
            return result;
        }

        /// <summary>
        ///     Thêm 1 RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string AddRolesInherited(string currInheritedFromRoles, Guid roleId)
        {
            //check RolesInherited truoc
            var roles = LoadRolesInherited(currInheritedFromRoles) ?? new List<Guid>();
            if (!roles.Contains(roleId))
                roles.Add(roleId);
            var result = GenRolesInherited(roles);
            return result;
        }

        /// <summary>
        ///     Thêm 1 danh sách RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="listRoleId"></param>
        /// <returns></returns>
        public static string AddRolesInherited(string currInheritedFromRoles, List<Guid> listRoleId)
        {
            var result = currInheritedFromRoles;
            if (listRoleId == null)
                listRoleId = new List<Guid>();
            foreach (var role in listRoleId) result = AddRolesInherited(result, role);
            return result;
        }
    }
}