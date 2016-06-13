using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Articles
{
    public class ListItemArticleViewModel
    {
        [Display(Name = "ID")]
        public int ArticleId { get; set; }
        
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Publish Date")]
        public string  PublishDate { get; set; }

        [Display(Name = "IsPublished")]
        public bool IsPublished { get; set; }
    }
}
