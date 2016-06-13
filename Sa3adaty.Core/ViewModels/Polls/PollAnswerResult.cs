using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Polls
{
    public class PollAnswerResult
    {
        public string Answer { get; set; }

        public int NumberOfVotes { get; set; }

        public decimal Percentage { get; set; }
    }
}
