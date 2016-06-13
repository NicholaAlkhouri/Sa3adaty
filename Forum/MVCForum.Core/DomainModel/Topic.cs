﻿using System;
using System.Collections.Generic;
using System.Linq;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumTopic")]
    public partial class Topic : Entity
    {
        public Topic()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Solved { get; set; }
        public string Slug { get; set; }
        public int Views { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }

        public virtual Post LastPost { get; set; }
        public virtual Category Category { get; set; }
        public virtual IList<Post> Posts { get; set; }
        public virtual IList<TopicTag> Tags { get; set; }
        public virtual MembershipUser User { get; set; }
        public virtual IList<TopicNotification> TopicNotifications { get; set; }
        public virtual IList<Favourite> Favourites { get; set; }
        public virtual Poll Poll { get; set; }
        public bool? Pending { get; set; }
        public string NiceUrl
        {
            get { return UrlTypes.GenerateUrl(UrlType.Topic, Slug); }
        }
        public int VoteCount
        {
            get { return Posts.Select(x => x.VoteCount).Sum(); }
        }
    }
}
