using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Domain.Interfaces.Repositories
{
    public partial interface IPrivateMessageRepository
    {
        IPagedList<PrivateMessageListItem> GetUsersPrivateMessages(int pageIndex, int pageSize, MembershipUser user);
        PrivateMessage GetLastSentPrivateMessage(int id);
        PrivateMessage GetMatchingSentPrivateMessage(DateTime date, int senderId, int receiverId);
        IList<PrivateMessage> GetAllSentByUser(int id);
        IList<PrivateMessage> GetAllReceivedByUser(int id);
        IList<PrivateMessage> GetAllByUserToAnotherUser(int senderId, int receiverId);
        int NewPrivateMessageCount(int userId);
        PrivateMessage Add(PrivateMessage item);
        PrivateMessage Get(Guid id);
        void Delete(PrivateMessage item);
        void Update(PrivateMessage item);
    }
}
