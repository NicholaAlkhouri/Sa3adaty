using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Tips
{
    public class TipViewModel
    {
        [Display(Name = "ID")]
        public int TipId { get; set; }
        
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [UIHint("ForeignKey")]
        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }

        [Required]
        [Display(Name = "Online Date")]
        public DateTime? OnlineDate { get; set; }

        [Required]
        [Display(Name = "Published")]
        public bool IsPublished { get; set; }
    }
}
