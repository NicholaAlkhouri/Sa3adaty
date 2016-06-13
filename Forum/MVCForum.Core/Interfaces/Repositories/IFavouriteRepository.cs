﻿using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Domain.Interfaces.Repositories
{
    public partial interface IFavouriteRepository
    {
        Favourite Add(Favourite dialogueFavourite);
        Favourite Delete(Favourite dialogueFavourite);
        List<Favourite> GetAll();
        List<Favourite> GetAllPostFavourites(List<Guid> postIds);
        List<Favourite> GetAllByMember(int memberId);
        Favourite GetByMemberAndPost(int memberId, Guid postId);
        List<Favourite> GetByTopic(Guid topicId);
    }
}
