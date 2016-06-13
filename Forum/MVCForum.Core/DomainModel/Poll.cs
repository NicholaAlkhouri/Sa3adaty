using System;
using System.Collections.Generic;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
     [Table("ForumPoll")]
    public partial class Poll : Entity
    {
        public Poll()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public bool IsClosed { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual MembershipUser User { get; set; }
        public virtual IList<PollAnswer> PollAnswers { get; set; } 
    }
}
