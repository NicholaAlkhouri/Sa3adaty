using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels.Tips;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class TipFrontService
    {
         #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public TipFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods
            public TipViewModel GetTodayTip(int campaign_id)
            {
                Tip today_tip = DAManager.TipsRepository.Get(t => t.CampaignId == campaign_id && t.IsPublished == true && (t.OnlineDate !=null && t.OnlineDate.Value.Year == DateTime.Now.Year && t.OnlineDate.Value.Month == DateTime.Now.Month  && t.OnlineDate.Value.Day == DateTime.Now.Day )).FirstOrDefault();

                if (today_tip == null)
                    return null;

                return new TipViewModel { Description = today_tip.Description, Title = today_tip.Title };
            }
        #endregion
    }
}
