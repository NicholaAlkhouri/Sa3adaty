﻿using System;
using System.Collections.Generic;
using System.Linq;
using MVCForum.Data.Context;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Repositories;

namespace MVCForum.Data.Repositories
{
    public partial class PollVoteRepository : IPollVoteRepository
    {
        private readonly MVCForumContext _context;
        public PollVoteRepository(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }


        public List<PollVote> GetAllPollVotes()
        {
            return _context.PollVote.ToList();
        }

        public PollVote Add(PollVote pollVote)
        {
            return _context.PollVote.Add(pollVote);
        }

        public bool HasUserVotedAlready(Guid answerId, int userId)
        {
            var vote = _context.PollVote.FirstOrDefault(x => x.PollAnswer.Id == answerId && x.User.Id == userId);
            return (vote != null);
        }

        public PollVote Get(Guid id)
        {
            return _context.PollVote.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(PollVote pollVote)
        {
            _context.PollVote.Remove(pollVote);
        }
    }
}
