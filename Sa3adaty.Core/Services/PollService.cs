using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core.ViewModels.Admin.Poll;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class PollService
    {
         #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public PollService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods
            public SelectList GetSelectListCampaigns(int? selected_value)
            {
                return new SelectList(DAManager.CampaignsRepository.Get().AsEnumerable(), "CampaignId", "Name", selected_value);
            }

            public List<ListItemPollViewModel> GetPolls(out int total_count, int CampaignId, int page = 0, int page_size = 10, string search_key = "", string order_by = "OnlineDate", string order_dir = "desc")
            {
                IQueryable<Poll> result;
                if (order_by == "Question")
                    result = DAManager.PollsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Question) : a.OrderByDescending(c => c.Question)), "Campaign");
                else if (order_by == "Campaign")
                    result = DAManager.PollsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Campaign.Name) : a.OrderByDescending(c => c.Campaign.Name)), "Campaign");
                else
                    result = DAManager.PollsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.OnlineStartDate) : a.OrderByDescending(c => c.OnlineStartDate)), "Campaign");

                if (CampaignId > 0)
                    result = result.Where(p => p.CampaignId == CampaignId);

                if (search_key != "")
                    result = result.Where(p => p.Question.Contains(search_key));

                total_count = result.Count();
                result = result.Skip(page).Take(page_size);
                List<Poll> final_res = result.ToList();
                List<ListItemPollViewModel> final_result = new List<ListItemPollViewModel>();
                foreach (Poll poll in final_res)
                    final_result.Add(new ListItemPollViewModel() { Campaign = poll.Campaign.Name, Question = poll.Question, IsPublished = poll.IsPublished, PollId = poll.PollId, OnlineEndDate = poll.OnlineEndDate.ToString(), OnlineStartDate = poll.OnlineStartDate.ToString(), Type = poll.Type });

                return final_result;
            }

            public int AddNewPoll(PollViewModel poll ,HttpPostedFileBase poll_image = null)
            {
                Poll db_poll = new Poll() { AddedDate = DateTime.Now, CampaignId = poll.CampaignId, IsPublished = poll.IsPublished, OnlineStartDate = poll.OnlineStartDate, OnlineEndDate = poll.OnlineEndDate, Question = poll.Question, Type = poll.Type, Description = poll.Description };

                DAManager.PollsRepository.Insert(db_poll);

                foreach (PollAnswerViewModel pa in poll.Answers)
                {
                    if (pa.Answer != null && pa.Answer.Trim() != "")
                    {
                        db_poll.PollAnswers.Add(new PollAnswer() { Answer = pa.Answer, IsCorrect = pa.IsCorrect });
                    }
                }

                try
                {
                    DAManager.Save();
                    
                    //Add the image 
                    if (poll_image != null)
                        AddPollImage(db_poll.PollId, poll_image);
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }

                return db_poll.PollId;
            }

            public bool AddPollImage(int poll_id,HttpPostedFileBase Poll_Image)
            {
                Poll poll = DAManager.PollsRepository.Get(p=> p.PollId == poll_id).FirstOrDefault();
                if (poll == null)
                    return false;

                //Delete old Image
                if (poll.ImageURL != null && poll.ImageURL != "")
                {
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + poll.ImageURL));
                    
                }

                if (Poll_Image != null)
                        {
                            try
                            {
                                //Save Image in the data base to get its id.
                                DAManager.Save();

                                //set the image file name
                                string file_name = "";
                               
                                file_name = ("poll_"+ poll_id + "-" + Poll_Image.FileName).Replace(" ", "-");

                                System.Drawing.Image web_image = System.Drawing.Image.FromStream(Poll_Image.InputStream);

                                //save Original Image
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);
                                                             
                                //Update the DB value
                                poll.ImageURL = ImageService.GetImagesDirectory() + file_name;
                                DAManager.Save();
                            }
                            catch (Exception ex)
                            {
                                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                                return false;
                            }
                        
                    }
                return true;
            }


            public int UpdatePoll(PollViewModel poll, HttpPostedFileBase poll_image = null)
            {
                Poll old_poll = DAManager.PollsRepository.Get(p=> p.PollId == poll.PollId ,null,"PollAnswers" ).FirstOrDefault();

                if (old_poll != null)
                {
                    old_poll.CampaignId = poll.CampaignId;
                    old_poll.IsPublished = poll.IsPublished;
                    old_poll.OnlineStartDate = poll.OnlineStartDate;
                    old_poll.OnlineEndDate = poll.OnlineEndDate;
                    old_poll.Question = poll.Question;
                    old_poll.Type = poll.Type;
                    old_poll.Description = poll.Description;
                }
                else
                    return -1;

                //Delete unwanted unswers
                foreach (PollAnswer pa in old_poll.PollAnswers.ToList())
                {
                    PollAnswerViewModel pavm = poll.Answers.Where(pp => pp.PollAnswerId == pa.AnswerId).FirstOrDefault();
                    if (pavm == null || pavm.Answer == null || pavm.Answer.Trim() == "")
                        DAManager.PollAnswersRepository.Delete(pa);
                }

                foreach (PollAnswerViewModel pa in poll.Answers)
                {
                    if (pa.Answer != null && pa.Answer.Trim() != "")
                    {
                    
                    //New Answer
                    if (pa.PollAnswerId <= 0)
                    {
                        old_poll.PollAnswers.Add(new PollAnswer() { Answer = pa.Answer, IsCorrect = pa.IsCorrect,PollId = poll.PollId  });
                    }
                    else//Old updated
                    {
                        PollAnswer t = old_poll.PollAnswers.Where(a => a.AnswerId == pa.PollAnswerId).FirstOrDefault();
                        if (t != null)
                        {
                            t.Answer = pa.Answer;
                            t.IsCorrect = pa.IsCorrect;
                        }
                    }
                    }
                }

                try
                {
                    DAManager.Save();
                    //Add the image 
                    if (poll_image != null)
                        AddPollImage(old_poll.PollId, poll_image);

                    return old_poll.PollId;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }
            }

            public PollViewModel  GetPollById(int poll_id)
            {
                var poll = DAManager.PollsRepository.Get(p => p.PollId == poll_id, null, "PollAnswers").FirstOrDefault();
                if (poll != null)
                {
                    PollViewModel poll_viewmodel = new PollViewModel() { Description = poll.Description, CampaignId = poll.CampaignId, IsPublished = poll.IsPublished,OnlineEndDate = poll.OnlineEndDate, OnlineStartDate = poll.OnlineStartDate, PollId = poll_id, Question = poll.Question, Type = poll.Type, ImageURL = poll.ImageURL };
                    poll_viewmodel.Answers = new List<PollAnswerViewModel>();
                    foreach (PollAnswer pa in poll.PollAnswers)
                        poll_viewmodel.Answers.Add(new PollAnswerViewModel() {Answer = pa.Answer,IsCorrect = pa.IsCorrect,PollAnswerId = pa.AnswerId,PollId = pa.PollId  });
                    return poll_viewmodel;
                }
                else
                    return null;
            }
        #endregion 
    }
}
