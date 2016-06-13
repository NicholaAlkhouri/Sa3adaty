﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using MVCForum.Data.Context;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Repositories;
using MVCForum.Utilities;

namespace MVCForum.Data.Repositories
{
    public partial class PrivateMessageRepository : IPrivateMessageRepository
    {
        private readonly MVCForumContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"> </param>
        public PrivateMessageRepository(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        //public IPagedList<PrivateMessage> GetPagedSentMessagesByUser(int pageIndex, int pageSize, MembershipUser user)
        //{
        //    var totalCount = _context.PrivateMessage
        //                        .Include(x => x.UserTo)
        //                        .Include(x => x.UserFrom)
        //                        .Count(x => x.UserFrom.Id == user.Id && x.IsSentMessage == true);

        //    // Get the topics using an efficient
        //    var results = _context.PrivateMessage
        //                        .Include(x => x.UserTo)
        //                        .Include(x => x.UserFrom)
        //                        .Where(x => x.UserFrom.Id == user.Id)
        //                        .Where(x => x.IsSentMessage == true)
        //                        .OrderByDescending(x => x.DateSent)
        //                        .Skip((pageIndex - 1) * pageSize)
        //                        .Take(pageSize)
        //                        .ToList();

        //    // Return a paged list
        //    return new PagedList<PrivateMessage>(results, pageIndex, pageSize, totalCount);
        //}

        //public IPagedList<PrivateMessage> GetPagedReceivedMessagesByUser(int pageIndex, int pageSize, MembershipUser user)
        //{


        //    var totalCount = _context.PrivateMessage
        //                        .Include(x => x.UserTo)
        //                        .Include(x => x.UserFrom)                                
        //                        .OrderByDescending(x => x.DateSent)
        //                        .Count(x => (x.UserTo.Id == user.Id || x.UserFrom.Id == user.Id) && x.IsSentMessage != true);

        //    var results = _context.PrivateMessage
        //                        .Include(x => x.UserTo)
        //                        .Include(x => x.UserFrom)
        //                        .OrderByDescending(x => x.DateSent)
        //                        .Where(x => (x.UserTo.Id == user.Id || x.UserFrom.Id == user.Id) && x.IsSentMessage != true)
        //                        .Skip((pageIndex - 1) * pageSize)
        //                        .Take(pageSize)
        //                        .ToList();


        //    // Return a paged list
        //    return new PagedList<PrivateMessage>(results, pageIndex, pageSize, totalCount);
        //}

        public IPagedList<PrivateMessageListItem> GetUsersPrivateMessages(int pageIndex, int pageSize, MembershipUser user)
        {
            //TODO - Need to rework this as it's not very efficient

            // Get all received pms
            var members = user.PrivateMessagesReceived.Where(x => x.IsSentMessage == true).OrderByDescending(x => x.DateSent).DistinctBy(x => x.UserFrom.Id).Select(x => new PrivateMessageListItem
            {
                Date = x.DateSent,
                User = x.UserFrom
            }).ToList();

            // Get all sent pms + ISSent
            var sentToMembers = user.PrivateMessagesSent.Where(x => x.IsSentMessage != true).OrderByDescending(x => x.DateSent).DistinctBy(x => x.UserTo.Id).Select(x => new PrivateMessageListItem
            {
                Date = x.DateSent,
                User = x.UserTo
            }).ToList();

            // Add lists
            members.AddRange(sentToMembers);

            // Get the full amount
            var uniqueMembers = members.OrderByDescending(x => x.Date).DistinctBy(x => x.User.Id).ToList();
            var uniqueMembersCount = uniqueMembers.Count;

            // Now distinct the lists again
            members = uniqueMembers.Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            // Return a paged list
            return new PagedList<PrivateMessageListItem>(members, pageIndex, pageSize, uniqueMembersCount);
        }

        public PrivateMessage GetLastSentPrivateMessage(int id)
        {
            return _context.PrivateMessage
                                .Include(x => x.UserTo)
                                .Include(x => x.UserFrom)
                                .FirstOrDefault(x => x.UserFrom.Id == id);
        }

        public PrivateMessage GetMatchingSentPrivateMessage(DateTime date, int senderId, int receiverId)
        {
            return _context.PrivateMessage
                                .Include(x => x.UserTo)
                                .Include(x => x.UserFrom)
                                .FirstOrDefault(x => x.DateSent == date && x.UserFrom.Id == senderId && x.UserTo.Id == receiverId && x.IsSentMessage == true);
        }

        public IList<PrivateMessage> GetAllSentByUser(int id)
        {
            return _context.PrivateMessage
                                .Include(x => x.UserTo)
                                .Include(x => x.UserFrom)
                                .Where(x => x.UserFrom.Id == id)
                                .OrderByDescending(x => x.DateSent)
                                .ToList();
        }

        public IList<PrivateMessage> GetAllReceivedByUser(int id)
        {
            return _context.PrivateMessage
                                .Where(x => x.UserTo.Id == id)
                                .OrderByDescending(x => x.DateSent)
                                .ToList();
        }

        public IList<PrivateMessage> GetAllByUserToAnotherUser(int senderId, int receiverId)
        {
            return _context.PrivateMessage
                                .Include(x => x.UserTo)
                                .Include(x => x.UserFrom)
                                .Where(x => x.UserFrom.Id == senderId && x.UserTo.Id == receiverId)
                                .OrderByDescending(x => x.DateSent)
                                .ToList();
        }

        public int NewPrivateMessageCount(int userId)
        {
            return _context.PrivateMessage
                            .Include(x => x.UserTo)
                            .Include(x => x.UserFrom)
                            .Count(x => x.UserTo.Id == userId && !x.IsRead && x.IsSentMessage != true);
        }

        public PrivateMessage Add(PrivateMessage item)
        {
            return _context.PrivateMessage.Add(item);
        }

        public PrivateMessage Get(Guid id)
        {
            return _context.PrivateMessage
                            .Include(x => x.UserTo)
                            .Include(x => x.UserFrom)
                            .FirstOrDefault(x => x.Id == id);
        }

        public void Delete(PrivateMessage item)
        {
            _context.PrivateMessage.Remove(item);
        }

        public void Update(PrivateMessage item)
        {
            // Check there's not an object with same identifier already in context
            if (_context.PrivateMessage.Local.Select(x => x.Id == item.Id).Any())
            {
                throw new ApplicationException("Object already exists in context - you do not need to call Update. Save occurs on Commit");
            }
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
