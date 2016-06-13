using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumPermission")]
    public partial class Permission : Entity
    {
        public Permission()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsGlobal { get; set; }
    }
}
