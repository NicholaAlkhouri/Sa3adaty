using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Poll;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class AdminPollController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public AdminPollController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion

        //
        //GET: /Admin/AdminPoll/List
        public ActionResult List(int CampaignId = 0)
        {
            ViewBag.SelectedPage = Navigator.Items.LISTPOLL;
            PollsListViewModel view_model = new PollsListViewModel() { CampaignId = CampaignId };
            FillPollCampaigns(CampaignId);
            return View(view_model);
        }

        public JsonResult _PollList(int draw, int start = 0, int length = 2, int CampaignId = 0)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            int total_count;
            result.data = servicesManager.PollService.GetPolls(out total_count, CampaignId, start, length, search, order_by, order_dir);

            result.recordsTotal = total_count;
            result.recordsFiltered = total_count;
            return Json(result);
        }

        //
        //GET: /Admin/AdminPoll/New
        public ActionResult New()
        {
            ViewBag.SelectedPage = Navigator.Items.NEWPOLL;
            FillPollCampaigns(null);

            PollViewModel view_model = new PollViewModel() { OnlineStartDate = DateTime.Now, OnlineEndDate = DateTime.Now };
            view_model.Answers = new List<PollAnswerViewModel>();
            view_model.Answers.Add(new PollAnswerViewModel());
            return View(view_model);
        }

        //
        //Post: /Admin/AdminTip/New
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(PollViewModel poll, HttpPostedFileBase poll_image)
        {
            ViewBag.SelectedPage = Navigator.Items.NEWPOLL;
            FillPollCampaigns(poll.CampaignId);

            if (ModelState.IsValid)
            {
                if (poll_image != null && !ImageService.IsValid(poll_image))
                {
                    TempData["ErrorMessage"] = "Poll Failed To Add, Invalid Image File";
                }
                else
                {
                    int new_id = servicesManager.PollService.AddNewPoll(poll,poll_image);

                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Poll Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "Poll Failed To Add";
                }
            }

            return View(poll);
        }

        //
        //GET: /Admin/AdminPoll/Edit/{category-id}
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedPage = Navigator.Items.POLLS;
            PollViewModel tip = servicesManager.PollService.GetPollById(id);

            FillPollCampaigns(tip.CampaignId);

            if (tip == null)
                return RedirectToAction("List");

            return View(tip);
        }

        //
        //POST: /Admin/AdminPoll/Edit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PollViewModel poll, HttpPostedFileBase poll_image)
        {
            ViewBag.SelectedPage = Navigator.Items.POLLS;
            if (ModelState.IsValid)
            {
                if (poll_image != null && !ImageService.IsValid(poll_image))
                {
                    TempData["ErrorMessage"] = "Poll Failed To Add, Invalid Image File";
                }
                else
                {
                    int new_id = servicesManager.PollService.UpdatePoll(poll,poll_image);
                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Poll Updated Successfully";
                        return RedirectToAction("Edit", poll.PollId);
                    }
                    else
                        TempData["ErrorMessage"] = "Poll Failed To Update";
                }

            }
            FillPollCampaigns(poll.CampaignId);

            return View(poll);
        }
    
        #region Helpers Foreigns
        private void FillPollCampaigns(int? selected_value)
        {
            SelectList result = servicesManager.PollService.GetSelectListCampaigns(selected_value);
            ViewData["CampaignId_Data"] = result;
        }
        #endregion

    }
}
