using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Core;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Tips;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class AdminTipController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
            public AdminTipController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion


            //
            //GET: /Admin/AdminTip/List
            public ActionResult List(int CampaignId = 0)
            {
                ViewBag.SelectedPage = Navigator.Items.LISTTIPS;
                TipsListViewModel view_model = new TipsListViewModel() { CampaignId = CampaignId };
                FillTipsCampaigns(CampaignId);
                return View(view_model);
            }

            public JsonResult _TipsList(int draw, int start = 0, int length = 2, int CampaignId = 0)
            {
                string search = Request.Form["search[value]"];
                string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
                string order_dir = Request.Form["order[0][dir]"];

                DataTableViewModel result = new DataTableViewModel();
                result.draw = draw;
                int total_count;
                result.data = servicesManager.TipService.GetTips(out total_count, CampaignId , start, length, search, order_by, order_dir);

                result.recordsTotal = total_count;
                result.recordsFiltered = total_count;
                return Json(result);
            }

            //
            //GET: /Admin/AdminTip/New
            public ActionResult New()
            {
                ViewBag.SelectedPage = Navigator.Items.NEWTIP;
                FillTipsCampaigns(null);

                return View();
            }

            //
            //Post: /Admin/AdminTip/New
            [HttpPost]
            [ValidateInput(false)]
            public ActionResult New(TipViewModel tip)
            {
                ViewBag.SelectedPage = Navigator.Items.NEWTIP;
                FillTipsCampaigns(tip.CampaignId);

                if (ModelState.IsValid)
                {
                    int new_id = servicesManager.TipService.AddNewTip(tip);

                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Tip Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "Tip Failed To Add";
                }

                return View(tip);
            }

            //
            //GET: /Admin/AdminTip/Edit/{category-id}
            public ActionResult Edit(int id)
            {
                ViewBag.SelectedPage = Navigator.Items.TIPS;
                TipViewModel  tip = servicesManager.TipService.GetTipById(id);

                FillTipsCampaigns(tip.CampaignId);
               
                if (tip == null)
                    return RedirectToAction("List");

                return View(tip);
            }

            //
            //POST: /Admin/AdminTip/Edit
            [HttpPost]
            [ValidateInput(false)]
            public ActionResult Edit(TipViewModel tip)
            {
                ViewBag.SelectedPage = Navigator.Items.TIPS;
                if (ModelState.IsValid)
                {
                    int new_id = servicesManager.TipService.UpdateTip(tip);
                        if (new_id > 0)
                        {
                            TempData["SuccessMessage"] = "Tip Updated Successfully";
                            return RedirectToAction("Edit", tip.TipId);
                        }
                        else
                            TempData["ErrorMessage"] = "Tip Failed To Update";
                  
                }
                FillTipsCampaigns(tip.CampaignId);

                return View(tip);
            }

            #region Helpers Foreigns
                private void FillTipsCampaigns(int? selected_value)
                {
                    SelectList result = servicesManager.TipService.GetSelectListCampaigns(selected_value);
                    ViewData["CampaignId_Data"] = result;
                }
            #endregion

    }
}
