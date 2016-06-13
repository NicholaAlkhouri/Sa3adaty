using System.Data.Entity.ModelConfiguration;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Data.Mapping
{
    public class MembershipRoleMapping : EntityTypeConfiguration<MembershipRole>
    {
        public MembershipRoleMapping()
        {
            HasKey(x => x.Id);

            HasMany(x => x.CategoryPermissionForRole)
                .WithRequired(x => x.MembershipRole)
                .Map(x => x.MapKey("RoleId"))
                .WillCascadeOnDelete();

            HasMany(x => x.GlobalPermissionForRole)
                .WithRequired(x => x.MembershipRole)
                .Map(x => x.MapKey("RoleId"))
                .WillCascadeOnDelete();
        }
    }
}
