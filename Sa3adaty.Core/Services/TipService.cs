using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sa3adaty.Core.ViewModels.Admin.Tips;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
   public  class TipService
    {
       #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public TipService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods
            public List<ListItemTipViewModel> GetTips(out int total_count, int CampaignId, int page = 0, int page_size = 10, string search_key = "", string order_by = "OnlineDate", string order_dir = "desc")
            {
                IQueryable<Tip> result;
                if (order_by == "Title")
                    result = DAManager.TipsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Title) : a.OrderByDescending(c => c.Title)), "Campaign");
                else if (order_by == "Campaign")
                    result = DAManager.TipsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Campaign.Name) : a.OrderByDescending(c => c.Campaign.Name)), "Campaign");
                else
                    result = DAManager.TipsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.OnlineDate) : a.OrderByDescending(c => c.OnlineDate)), "Campaign");

                if (CampaignId > 0)
                    result = result.Where(tip => tip.CampaignId == CampaignId);

                if (search_key != "")
                    result = result.Where(tip => tip.Title.Contains(search_key));

                total_count = result.Count();
                result = result.Skip(page).Take(page_size);
                List<Tip> final_res = result.ToList();
                List<ListItemTipViewModel> final_result = new List<ListItemTipViewModel>();
                foreach (Tip tip in final_res)
                    final_result.Add(new ListItemTipViewModel() {Campaign = tip.Campaign.Name,Description = tip.Description,IsPublished = tip.IsPublished ,TipId = tip.TipId,Title = tip.Title , OnlineDate = tip.OnlineDate.ToString() });

                return final_result;
            }

            public SelectList GetSelectListCampaigns(int? selected_value)
            {
                return new SelectList(DAManager.CampaignsRepository.Get().AsEnumerable(), "CampaignId", "Name", selected_value);
            }

            public int AddNewTip(TipViewModel  tip)
            {
                Tip db_tip = new Tip() { AddedDate = DateTime.Now, CampaignId = tip.CampaignId,Description = tip.Description,IsPublished = tip.IsPublished,OnlineDate=tip.OnlineDate,Title=tip.Title };

                DAManager.TipsRepository.Insert(db_tip);

                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }

                return db_tip.TipId;
            }

            public TipViewModel GetTipById(int tip_id)
            {
                var tip = DAManager.TipsRepository.Get(t => t.TipId == tip_id).FirstOrDefault();
                if (tip != null)
                {
                    TipViewModel tip_viewmodel = new TipViewModel() { CampaignId = tip.CampaignId,Description = tip.Description,IsPublished = tip.IsPublished,OnlineDate = tip.OnlineDate,Title = tip.Title,TipId = tip.TipId };


                    return tip_viewmodel;
                }
                else
                    return null;
            }

            public int UpdateTip(TipViewModel tip)
            {
                Tip old_tip = DAManager.TipsRepository.Get(t => t.TipId == tip.TipId).FirstOrDefault();

                if (old_tip != null)
                {
                    old_tip.CampaignId = tip.CampaignId;
                    old_tip.Description = tip.Description;
                    old_tip.IsPublished = tip.IsPublished;
                    old_tip.OnlineDate = tip.OnlineDate;
                    old_tip.TipId = tip.TipId;
                    old_tip.Title = tip.Title;
                }
                else
                    return -1;
                try
                {
                    DAManager.Save();
                    return old_tip.TipId;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }
            }
        #endregion 
    }
}
