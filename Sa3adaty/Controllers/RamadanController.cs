using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.Core.ViewModels.Polls;
using Sa3adaty.Core.ViewModels.Tips;
using Sa3adaty.DAL.Infrastructure;
using WebMatrix.WebData;

namespace Sa3adaty.Controllers
{
    public class RamadanController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;
        private int RamadanCampaign;

        #region Constructor
        public RamadanController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
            RamadanCampaign = Convert.ToInt32(ConfigurationManager.AppSettings["RamadanCampaignId"]);
        }
        #endregion



        public ActionResult _RamadanDailyTip()
        {
            TipViewModel tip = servicesManager.TipFrontService.GetTodayTip(RamadanCampaign);

            return PartialView(tip);
        }

        public ActionResult _RamadanDailyPoll()
        {
            PollViewModel tip = servicesManager.PollFrontService.GetTodayPoll(RamadanCampaign);

            return PartialView(tip);
        }

        public JsonResult _SubmitPollAnswer(int poll_id, int answer_id)
        {
            if (servicesManager.PollFrontService.AddPollAnswer(answer_id))
            {
                return Json(new { success = true, data = servicesManager.PollFrontService.GetPollResults(poll_id), poll_id = poll_id });
            }
            else
            {
                return Json(new { success = false, poll_id = poll_id });
            }
        }
    }
}
