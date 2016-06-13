using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumMembershipUserPoints")]
    public partial class MembershipUserPoints : Entity
    {
        public MembershipUserPoints()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public int Points { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual MembershipUser User { get; set; }
    }
}
