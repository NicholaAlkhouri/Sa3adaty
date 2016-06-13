using System;
using System.Collections.Generic;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
     [Table("webpages_Roles")]
    public partial class MembershipRole : Entity
    {
        //public MembershipRole()
        //{
        //    Id = GuidComb.GenerateComb();
        //}
        [Column("RoleId")]
        public int Id { get; set; }
        public string RoleName { get; set; }
        public virtual IList<MembershipUser> Users { get; set; }
        public virtual Settings Settings { get; set; }

        // Category Permissions
        public virtual IList<CategoryPermissionForRole> CategoryPermissionForRole { get; set; }

        // Global Permissions
        public virtual IList<GlobalPermissionForRole> GlobalPermissionForRole { get; set; }

        public virtual Dictionary<Guid, Dictionary<Permission, bool>> GetCategoryPermissionTable()
        {
            var permissionRows = new Dictionary<Guid, Dictionary<Permission, bool>>();

            foreach (var catPermissionForRole in CategoryPermissionForRole)
            {
                if (!permissionRows.ContainsKey(catPermissionForRole.Category.Id))
                {
                    var permissionList = new Dictionary<Permission, bool>();

                    permissionRows.Add(catPermissionForRole.Category.Id, permissionList);
                }

                if (!permissionRows[catPermissionForRole.Category.Id].ContainsKey(catPermissionForRole.Permission))
                {
                    permissionRows[catPermissionForRole.Category.Id].Add(catPermissionForRole.Permission, catPermissionForRole.IsTicked);
                }
                else
                {
                    permissionRows[catPermissionForRole.Category.Id][catPermissionForRole.Permission] = catPermissionForRole.IsTicked;
                }
                

            }
            return permissionRows;
        }

    }
}
