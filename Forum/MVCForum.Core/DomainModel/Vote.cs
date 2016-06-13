using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
     [Table("ForumVote")]
    public partial class Vote : Entity
    {
        public Vote()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public int Amount { get; set; }

        public virtual MembershipUser User { get; set; }
        public virtual Post Post { get; set; }
        public virtual MembershipUser VotedByMembershipUser { get; set; }
        public virtual DateTime? DateVoted { get; set; }
    }
}
