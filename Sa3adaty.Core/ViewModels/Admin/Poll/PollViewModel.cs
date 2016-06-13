using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Poll
{
    public class PollViewModel
    {
        [Display(Name = "ID")]
        public int PollId    { get; set; }

        [Display(Name = "Type")]
        public int Type { get; set; }

        [Required]
        [UIHint("ForeignKey")]
        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }

        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Start Date")]
        public DateTime OnlineStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime OnlineEndDate { get; set; }

        [Required]
        [Display(Name = "IsPublished")]
        public bool IsPublished { get; set; }

        
        [Display(Name = "ImageURL")]
        public string ImageURL { get; set; }

        public List<PollAnswerViewModel> Answers { get; set; }
    }
}
