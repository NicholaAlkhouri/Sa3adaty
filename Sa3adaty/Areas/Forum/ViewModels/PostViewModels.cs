﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel;
using MVCForum.Website.Application;

namespace MVCForum.Website.ViewModels
{
    public class CreateAjaxPostViewModel
    {
        [UIHint(SiteConstants.EditorType), AllowHtml]
        [StringLength(6000)]
        public string PostContent { get; set; }
        public Guid Topic { get; set; }
        public bool DisablePosting { get; set; }
    }

    public class ShowMorePostsViewModel
    {
        public List<PostViewModel> Posts { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public PermissionSet Permissions { get; set; }
        public Topic Topic { get; set; }
    }

    public class PostViewModel
    {
        public Post Post { get; set; }
        public string PermaLink { get; set; }
        public List<Vote> Votes { get; set; } 
        public List<Favourite> Favourites { get; set; } 
        public Topic ParentTopic { get; set; }
        public PermissionSet Permissions { get; set; }
        public bool AllowedToVote { get; set; }
        public bool MemberHasFavourited { get; set; }
    }

    public class ReportPostViewModel
    {
        public Guid PostId { get; set; }
        public string PostCreatorUsername { get; set; }
        
        [Required]
        public string Reason { get; set; }
    }
}