using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Authors
{
    public class AuthorViewModel
    {
        [Display(Name = "ID")]
        public int AuthorId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Enable Profile")]
        public bool IsProfileEnabled { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Facebook Page")]
        public string FacebookPage { get; set; }

        [Display(Name = "Facebook ID")]
        public string FacebookID { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "URL")]
        public string URL { get; set; }

        [Display(Name = "MetaTitle")]
        public string MetaTitle { get; set; }

        [Display(Name = "MetaDescription")]
        public string MetaDescription { get; set; }
    }
}
