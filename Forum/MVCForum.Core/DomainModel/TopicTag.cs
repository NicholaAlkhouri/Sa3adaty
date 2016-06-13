﻿using System;
using System.Collections.Generic;
using MVCForum.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCForum.Domain.DomainModel
{
    [Table("ForumTopicTag")]
    public partial class TopicTag : Entity
    {
        public TopicTag()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Tag { get; set; }
        public string Slug { get; set; }

        public string NiceUrl
        {
            get
            {
                var url = UrlTypes.GenerateUrl(UrlType.Tag, StringUtils.RemoveAccents(Slug));
                if (url.Contains(".") && !url.EndsWith("/"))
                {
                    // We do this for tags that have a full stop in the name
                    // or you just get a 404 error
                    url = string.Concat(url, "/");
                }
                return url;
            }
        }

        public virtual IList<Topic> Topics { get; set; }
    }
}
