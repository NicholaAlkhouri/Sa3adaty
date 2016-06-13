using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Polls
{
    public class PollViewModel
    {
        public int PollId { get; set; }

        public string  Question { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }

        public List<PollAnswerViewModel> Answers { get; set; }
    }
}
