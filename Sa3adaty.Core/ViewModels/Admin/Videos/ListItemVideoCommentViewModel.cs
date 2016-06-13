using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Videos
{
    public class ListItemVideoCommentViewModel
    {
        [Display(Name = "ID")]
        public int CommentId { get; set; }

        [Display(Name = "Video ID")]
        public int VideoId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Text")]
        public string Text { get; set; }

        [Display(Name = "Add Date")]
        public string AddDateString { get; set; }
    }
}
