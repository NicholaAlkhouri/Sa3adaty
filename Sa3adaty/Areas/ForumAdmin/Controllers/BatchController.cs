﻿using System;
using System.Linq;
using System.Web.Mvc;
using MVCForum.Domain.Constants;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.Areas.Admin.ViewModels;

namespace MVCForum.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public partial class BatchController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicService;

        public BatchController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, 
            ILocalizationService localizationService, ISettingsService settingsService, ICategoryService categoryService, ITopicService topicService) 
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, settingsService)
        {
            _categoryService = categoryService;
            _topicService = topicService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BatchDeleteMembers()
        {
            return View(new BatchDeleteMembersViewModel{AmoutOfDaysSinceRegistered = 0, AmoutOfPosts = 0});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BatchDeleteMembers(BatchDeleteMembersViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var membersToDelete = MembershipService.GetUsersByDaysPostsPoints(viewModel.AmoutOfDaysSinceRegistered,
                                                                                        viewModel.AmoutOfPosts);
                    var count = membersToDelete.Count;
                    foreach (var membershipUser in membersToDelete)
                    {
                        MembershipService.Delete(membershipUser, unitOfWork);
                    }
                    unitOfWork.Commit();
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = string.Format("{0} members deleted", count),
                        MessageType = GenericMessages.success
                    };
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = ex.Message,
                        MessageType = GenericMessages.danger
                    };
                }
            }

            return View();
        }

        public ActionResult BatchMoveTopics()
        {
            var viewModel = new BatchMoveTopicsViewModel
                {
                    Categories = _categoryService.GetAll().ToList()
                };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BatchMoveTopics(BatchMoveTopicsViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var categoryFrom = _categoryService.Get((Guid)viewModel.FromCategory);
                    var categoryTo = _categoryService.Get((Guid)viewModel.ToCategory);

                    var topicsToMove = _topicService.GetRssTopicsByCategory(int.MaxValue, categoryFrom.Id);
                    var count = topicsToMove.Count;

                    foreach (var topic in topicsToMove)
                    {
                        topic.Category = categoryTo;
                    }

                    unitOfWork.SaveChanges();

                    categoryFrom.Topics.Clear();

                    viewModel.Categories = _categoryService.GetAll().ToList();

                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = string.Format("{0} topics moved", count),
                        MessageType = GenericMessages.success
                    };
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = ex.Message,
                        MessageType = GenericMessages.danger
                    };
                }
            }

            return View(viewModel);
        }

    }
}
