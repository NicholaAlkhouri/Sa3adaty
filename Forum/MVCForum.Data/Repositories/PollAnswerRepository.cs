﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using MVCForum.Data.Context;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Repositories;

namespace MVCForum.Data.Repositories
{
    public partial class PollAnswerRepository : IPollAnswerRepository
    {
        private readonly MVCForumContext _context;
        public PollAnswerRepository(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public List<PollAnswer> GetAllPollAnswers()
        {
            return _context.PollAnswer
                    .Include(x => x.Poll).ToList();
        }

        public List<PollAnswer> GetAllPollAnswersByPoll(Poll poll)
        {
            var answers = _context.PollAnswer
                    .Include(x => x.Poll)
                    .AsNoTracking()
                    .Where(x => x.Poll.Id == poll.Id).ToList();
            return answers;
        }

        public PollAnswer Add(PollAnswer pollAnswer)
        {
            return _context.PollAnswer.Add(pollAnswer);
        }

        public PollAnswer Get(Guid id)
        {
            return _context.PollAnswer.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(PollAnswer pollAnswer)
        {
            _context.PollAnswer.Remove(pollAnswer);
        }

    }
}
