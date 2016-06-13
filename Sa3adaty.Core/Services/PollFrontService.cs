using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels.Polls;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;
using System.Data.Entity;

namespace Sa3adaty.Core.Services
{
    public class PollFrontService
    {
        
        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public PollFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods
            public PollViewModel GetTodayPoll(int campaign_id)
            {
                Poll today_poll = DAManager.PollsRepository.Get(p => p.CampaignId == campaign_id && p.IsPublished == true && DbFunctions.CreateDateTime(p.OnlineStartDate.Year, p.OnlineStartDate.Month, p.OnlineStartDate.Day, p.OnlineStartDate.Hour, p.OnlineStartDate.Minute, p.OnlineStartDate.Second) <= DateTime.Now && DbFunctions.CreateDateTime(p.OnlineEndDate.Year, p.OnlineEndDate.Month, p.OnlineEndDate.Day, p.OnlineEndDate.Hour, p.OnlineEndDate.Minute, p.OnlineEndDate.Second) >= DateTime.Now, null, "PollAnswers").FirstOrDefault();

                if (today_poll == null)
                    return null;
                PollViewModel result = new PollViewModel() {Description=today_poll.Description, ImageURL = today_poll.ImageURL, Question = today_poll.Question, PollId = today_poll.PollId ,Answers = new List<PollAnswerViewModel>()};

                foreach (PollAnswer pa in today_poll.PollAnswers)
                    result.Answers.Add(new PollAnswerViewModel() { PollAnswer = pa.Answer,PollAnswerId = pa.AnswerId ,IsCorrect = pa.IsCorrect });
                return result;
            }

            public bool AddPollAnswer(int answer_id)
            {
                PollUserAnswer PUA = new PollUserAnswer() { PollAnswerId = answer_id, AnswerDate = DateTime.Now };
                DAManager.PollUsersAnswersRepository.Insert(PUA);
                try
                {
                    DAManager.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public List<PollAnswerResult> GetPollResults(int poll_id)
            {
                Poll poll = DAManager.PollsRepository.Get(p => p.PollId == poll_id , null, "PollAnswers.PollUserAnswers").FirstOrDefault();
                if (poll == null)
                    return new List<PollAnswerResult>();

                List<PollAnswerResult> result = new List<PollAnswerResult>();
                int total = 0;
                List<PollAnswer> pollanswer_list =poll.PollAnswers.ToList();
                foreach (PollAnswer pa in pollanswer_list)
                {
                    total += pa.PollUserAnswers.Count();
                }
                foreach (PollAnswer pa in pollanswer_list)
                {
                    PollAnswerResult temp = new PollAnswerResult() {Answer = pa.Answer,NumberOfVotes = pa.PollUserAnswers.Count(),Percentage = Math.Round(((decimal)pa.PollUserAnswers.Count()/(decimal)total)*100,1) };
                    result.Add(temp);
                }
                return result;
                
            }
        #endregion
    }
}
