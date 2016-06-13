using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Tips
{
    public class ListItemTipViewModel
    {
        [Display(Name = "ID")]
        public int TipId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "OnlineDate")]
        public string OnlineDate { get; set; }

        [Display(Name = "IsPublished")]
        public bool IsPublished { get; set; }

        [Required]
        [Display(Name = "Campaign")]
        public string Campaign { get; set; }
            
    }
}
