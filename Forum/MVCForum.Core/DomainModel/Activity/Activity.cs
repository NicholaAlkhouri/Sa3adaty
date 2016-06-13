using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel.Activity
{
    public enum ActivityType
    {
        BadgeAwarded,
        MemberJoined,
        ProfileUpdated,
    }

    [Table("ForumActivity")]
    public class Activity : Entity
    {
        //public Activity()
        //{
        //    Id = GuidComb.GenerateComb();
        //}
        public int Id { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
