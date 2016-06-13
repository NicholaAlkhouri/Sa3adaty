using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Poll
{
    public class PollAnswerViewModel
    {
        [Display(Name = "ID")]
        public int PollAnswerId { get; set; }

        [Display(Name = "ID")]
        public int PollId { get; set; }

        [Display(Name = "Answer")]
        public string Answer { get; set; }

        [Display(Name = "IsCorrect")]
        public bool IsCorrect { get; set; }

    }
}
