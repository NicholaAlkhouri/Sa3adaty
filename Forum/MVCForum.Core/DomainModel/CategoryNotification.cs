using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumCategoryNotification")]
    public partial class CategoryNotification : Entity
    {
        public CategoryNotification()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public virtual Category Category { get; set; }
        public virtual MembershipUser User { get; set; }
    }
}
