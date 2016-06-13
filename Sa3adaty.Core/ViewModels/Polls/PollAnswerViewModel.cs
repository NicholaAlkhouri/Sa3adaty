using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Polls
{
    public class PollAnswerViewModel
    {
        public int PollAnswerId { get; set; }

        public string PollAnswer { get; set; }

        public bool IsCorrect { get; set; }
    }
}
