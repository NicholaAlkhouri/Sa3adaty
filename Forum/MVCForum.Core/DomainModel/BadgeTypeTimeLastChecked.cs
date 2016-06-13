using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumBadgeTypeTimeLastChecked")]
    public partial class BadgeTypeTimeLastChecked : Entity
    {
        public BadgeTypeTimeLastChecked()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string BadgeType { get; set; }
        public DateTime TimeLastChecked { get; set; }

        public virtual MembershipUser User { get; set; }
    }
}
