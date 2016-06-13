﻿using System;
using System.Linq;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.Interfaces.Repositories;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Utilities;

namespace MVCForum.Services
{
    public partial class BannedWordService : IBannedWordService
    {
        private readonly IBannedWordRepository _bannedWordRepository;

        public BannedWordService(IBannedWordRepository bannedWordRepository)
        {
            _bannedWordRepository = bannedWordRepository;
        }

        public BannedWord Add(BannedWord bannedWord)
        {
            return _bannedWordRepository.Add(bannedWord);
        }

        public void Delete(BannedWord bannedWord)
        {
            _bannedWordRepository.Delete(bannedWord);
        }

        public IList<BannedWord> GetAll(bool onlyStopWords = false)
        {
            if (onlyStopWords)
            {
                return _bannedWordRepository.GetAll().Where(x => x.IsStopWord == true).ToList();   
            }
            return _bannedWordRepository.GetAll().Where(x => x.IsStopWord != true).ToList();
        }

        public BannedWord Get(Guid id)
        {
            return _bannedWordRepository.Get(id);
        }

        public PagedList<BannedWord> GetAllPaged(int pageIndex, int pageSize)
        {
            return _bannedWordRepository.GetAllPaged(pageIndex, pageSize);
        }

        public PagedList<BannedWord> GetAllPaged(string search, int pageIndex, int pageSize)
        {
            return _bannedWordRepository.GetAllPaged(search, pageIndex, pageSize);
        }

        public string SanitiseBannedWords(string content)
        {
            var bannedWords = GetAll().ToList();
            if (bannedWords.Any())
            {
                return SanitiseBannedWords(content, bannedWords.Select(x => x.Word).ToList());
            }
            return content;
        }

        public string SanitiseBannedWords(string content, IList<string> words)
        {
            if (words != null && words.Any())
            {
                var censor = new CensorUtils(words);
                return censor.CensorText(content);
            }
            return content;
        }
    }
}
