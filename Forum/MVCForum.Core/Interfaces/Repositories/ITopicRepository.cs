﻿using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Domain.Interfaces.Repositories
{
    public partial interface ITopicRepository
    {
        IList<Topic> GetAll();
        IList<Topic> GetHighestViewedTopics(int amountToTake);
        IList<Topic> GetPopularTopics(DateTime from, DateTime to, int amountToShow);
        IList<Topic> GetTodaysTopics(int amountToTake);
        IList<Topic> GetSolvedTopicsByMember(int memberId);
        PagedList<Topic> GetRecentTopics(int pageIndex, int pageSize, int amountToTake);
        IList<Topic> GetRecentRssTopics(int amountToTake);
        IList<Topic> GetTopicsByUser(int memberId);
        IList<Topic> GetAllTopicsByCategory(Guid categoryId);
        IList<Topic> GetTopicsByLastPost(List<Guid> postIds);
        PagedList<Topic> GetPagedPendingTopics(int pageIndex, int pageSize);
        PagedList<Topic> GetPagedTopicsByCategory(int pageIndex, int pageSize, int amountToTake, Guid categoryId);
        PagedList<Topic> GetPagedTopicsAll(int pageIndex, int pageSize, int amountToTake);
        PagedList<Topic> SearchTopics(int pageIndex, int pageSize, int amountToTake, List<string> searchTerm);
        PagedList<Topic> GetTopicsByCsv(int pageIndex, int pageSize, int amountToTake, List<Guid> topicIds);
        IList<Topic> GetTopicsByCsv(int amountToTake, List<Guid> topicIds);
        IList<Topic> GetRssTopicsByCategory(int amountToTake, Guid categoryId);
        PagedList<Topic> GetPagedTopicsByTag(int pageIndex, int pageSize, int amountToTake, string tag);
        PagedList<Topic> GetMembersActivity(int pageIndex, int pageSize, int amountToTake, int memberGuid);
        Topic GetTopicBySlug(string slug);
        IList<Topic> GetTopicBySlugLike(string slug);
        int TopicCount();
        Topic Add(Topic item);
        Topic Get(Guid id);
        List<Topic> Get(List<Guid> ids);
        void Delete(Topic item);
    }
}
