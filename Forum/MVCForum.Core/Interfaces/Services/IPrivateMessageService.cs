using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Domain.Interfaces.Services
{
    public partial interface IPrivateMessageService
    {
        PrivateMessage SanitizeMessage(PrivateMessage privateMessage);
        PrivateMessage Add(PrivateMessage message);
        PrivateMessage Get(Guid id);
        void Save(PrivateMessage id);
        IPagedList<PrivateMessageListItem> GetUsersPrivateMessages(int pageIndex, int pageSize, MembershipUser user);
        PrivateMessage GetLastSentPrivateMessage(int id);
        PrivateMessage GetMatchingSentPrivateMessage(DateTime date, int senderId, int receiverId);
        IList<PrivateMessage> GetAllSentByUser(int id);
        int NewPrivateMessageCount(int userId);
        IList<PrivateMessage> GetAllReceivedByUser(int id);
        IList<PrivateMessage> GetAllByUserToAnotherUser(int senderId, int receiverId);
        void DeleteMessage(PrivateMessage message);
    }
}
