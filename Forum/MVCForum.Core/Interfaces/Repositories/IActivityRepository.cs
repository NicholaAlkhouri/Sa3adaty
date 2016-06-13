﻿using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.Activity;

namespace MVCForum.Domain.Interfaces.Repositories
{
    public partial interface IActivityRepository
    {
        /// <summary>
        /// Get activities
        /// </summary>
        /// <returns></returns>
        IEnumerable<Activity> GetAll();

        /// <summary>
        /// Gets all activities by search data field for a Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        IEnumerable<Activity> GetDataFieldByGuid(int guid);
            
        /// <summary>
        /// Add a new activity (expected id already assigned)
        /// </summary>
        /// <param name="newActivity"></param>
        Activity Add(Activity newActivity);

        /// <summary>
        /// Pages list of activities
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedList<Activity> GetPagedGroupedActivities(int pageIndex, int pageSize);

        PagedList<Activity> SearchPagedGroupedActivities(string search, int pageIndex, int pageSize);

        Activity Get(int id);

        void Delete(Activity item);
        void Update(Activity item);
    }
}
