﻿using System;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumPrivateMessage")]
    public partial class PrivateMessage : Entity
    {
        public PrivateMessage()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public DateTime DateSent { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public bool? IsSentMessage { get; set; }
        public virtual MembershipUser UserTo { get; set; }
        public virtual MembershipUser UserFrom { get; set; }
    }

    public partial class PrivateMessageListItem
    {
        public MembershipUser User { get; set; }
        public DateTime Date { get; set; }
    }
}
