﻿using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Domain.Interfaces.Services
{
    public partial interface ITopicService
    {
        Topic SanitizeTopic(Topic topic);
        IList<Topic> GetAll();
        IList<Topic> GetHighestViewedTopics(int amountToTake);
        IList<Topic> GetPopularTopics(DateTime? from, DateTime? to, int amountToShow = 20);
        Topic Add(Topic topic);
        IList<Topic> GetTodaysTopics(int amountToTake);
        PagedList<Topic> GetRecentTopics(int pageIndex, int pageSize, int amountToTake);        
        IList<Topic> GetRecentRssTopics(int amountToTake);
        IList<Topic> GetTopicsByUser(int memberId);
        IList<Topic> GetTopicsByLastPost(List<Guid> postIds);
        PagedList<Topic> GetPagedTopicsByCategory(int pageIndex, int pageSize, int amountToTake, Guid categoryId);
        PagedList<Topic> GetPagedPendingTopics(int pageIndex, int pageSize);
        IList<Topic> GetRssTopicsByCategory(int amountToTake, Guid categoryId);
        PagedList<Topic> GetPagedTopicsByTag(int pageIndex, int pageSize, int amountToTake, string tag);
        PagedList<Topic> SearchTopics(int pageIndex, int pageSize, int amountToTake, string searchTerm);
        PagedList<Topic> GetTopicsByCsv(int pageIndex, int pageSize, int amountToTake, List<Guid> topicIds);
        PagedList<Topic> GetMembersActivity(int pageIndex, int pageSize, int amountToTake, int memberGuid);
        IList<Topic> GetTopicsByCsv(int amountToTake, List<Guid> topicIds);
        IList<Topic> GetSolvedTopicsByMember(int memberId);
        Topic GetTopicBySlug(string slug);
        Topic Get(Guid topicId);
        List<Topic> Get(List<Guid> topicIds);
        void Delete(Topic topic);
        int TopicCount();
        Post AddLastPost(Topic topic, string postContent);
        
        /// <summary>
        /// Mark a topic as solved
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="post"></param>
        /// <param name="marker"></param>
        /// <param name="solutionWriter"></param>
        /// <returns>True if topic has been marked as solved</returns>
        bool SolveTopic(Topic topic, Post post, MembershipUser marker, MembershipUser solutionWriter);
    }
}
