using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Poll
{
    public class ListItemPollViewModel
    {
        [Display(Name = "ID")]
        public int PollId { get; set; }

        [Display(Name = "Question")]
        public string Question { get; set; }

        [Display(Name = "Type")]
        public int Type { get; set; }

        [Display(Name = "OnlineDate")]
        public string  OnlineStartDate { get; set; }

        [Display(Name = "OnlineDate")]
        public string OnlineEndDate { get; set; }

        [Display(Name = "IsPublished")]
        public bool IsPublished { get; set; }

        [Display(Name = "Campaign")]
        public string Campaign { get; set; }
    }
}
