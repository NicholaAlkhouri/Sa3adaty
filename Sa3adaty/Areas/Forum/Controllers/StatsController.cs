﻿using System.Web.Mvc;
using MVCForum.Domain.Constants;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public partial class StatsController : BaseController
    {
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;

        public StatsController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, 
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, ITopicService topicService, IPostService postService) : 
            base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _topicService = topicService;
            _postService = postService;
        }

        [ChildActionOnly]
        public PartialViewResult GetMainStats()
        {
            var viewModel = new MainStatsViewModel
                                {
                                    //LatestMembers = MembershipService.GetLatestUsers(10),
                                    MemberCount = MembershipService.MemberCount(),
                                    TopicCount = _topicService.TopicCount(),
                                    PostCount = _postService.PostCount()
                                };
            return PartialView(viewModel);
        }


        [ChildActionOnly]
        public PartialViewResult GetNewMembers()
        {
            var viewModel = new MainStatsViewModel
            {
                LatestMembers = MembershipService.GetLatestUsers(5)
            };
            return PartialView(viewModel);
        }
    }
}
