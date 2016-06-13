using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Articles
{
    public class CategoryViewModel
    {
        [Display(Name = "ID")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Meta Title")]
        public string MetaTitle { get; set; }

        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }

        [Required]
        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Required]
        [Display(Name = "URL")]
        public string URL { get; set; }
    }

    public class CategoriesListViewModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<CategoryViewModel> Categories { get; set; }

    }
}
