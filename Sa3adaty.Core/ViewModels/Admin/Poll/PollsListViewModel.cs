using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Poll
{
    public class PollsListViewModel
    {
        [UIHint("ItemsList")]
        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }
    }
}
