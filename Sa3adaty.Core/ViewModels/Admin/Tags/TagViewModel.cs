using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Tags
{
    public class TagViewModel
    {
        [Display(Name = "ID")]
        public int TagId { get; set; }

        [Required]
        [Display(Name = "Tag")]
        public string TagName { get; set; }

        [Display(Name = "Front Title")]
        public string FrontTitle { get; set; }

        [Display(Name = "Front Description")]
        public string FrontDescription { get; set; }

        [Required]
        [Display(Name = "MetaTitle")]
        public string MetaTitle { get; set; }

        [Required]
        [Display(Name = "MetaDescription")]
        public string MetaDescription { get; set; }
    }
}
