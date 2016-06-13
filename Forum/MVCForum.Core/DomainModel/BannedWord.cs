using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumBannedWord")]
    public partial class BannedWord
    {
        public BannedWord()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Word { get; set; }
        public bool? IsStopWord { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
