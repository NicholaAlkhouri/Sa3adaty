using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumPollVote")]
    public partial class PollVote
    {
        public PollVote()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public virtual PollAnswer PollAnswer { get; set; }
        public virtual MembershipUser User { get; set; }
    }
}
