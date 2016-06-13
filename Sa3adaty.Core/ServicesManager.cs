using Sa3adaty.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sa3adaty.Core.Services;

namespace Sa3adaty.Core
{
    public class ServicesManager
    {
        #region Privates
        private DataAccessManager dataAccessManager;
        private AccountService accountService;
        private ArticleService articleService;
        private ImageService imageService;
        private QuoteService quoteService;
        private AuthorService authorService;
        private StaticPagesService staticPagesService;
        private SiteMapService siteMapService;
        private TipService tipService;
        private TagService tagService;
        private PollService pollService;
        private VideoService videoService;
        private TagOfVideoService tagOfVideoService;

        #region Front Services
        private ArticleFrontService articleFrontService;
        private AccountFrontService accountFrontService;
        private CountryFrontSrevice countryFrontService;
        private AdvertiseInfoFrontService advertiseInfoFrontService;
        private ContactInfoFrontService contactInfoFrontService;
        private StaticPagesFrontService staticPagesFrontService;
        private PollFrontService pollFrontService;
        private TipFrontService tipFrontService;
        private VideoFrontService videoFrontService;
        private AuthorFrontService authorFrontService;
        #endregion
        #endregion

        #region Constructor
        public ServicesManager(DataAccessManager access_manager)
        {
            dataAccessManager = access_manager;
        }
        #endregion

        #region Properties
        public DataAccessManager DataAccessManager
        {
            get { return dataAccessManager; }
        }

        public AuthorFrontService AuthorFrontService
        {
            get
            {
                if (this.authorFrontService == null)
                {
                    this.authorFrontService = new AuthorFrontService(dataAccessManager);
                }
                return authorFrontService;
            }
        }
        public TagOfVideoService TagOfVideoService
        {
            get
            {
                if (this.tagOfVideoService == null)
                {
                    this.tagOfVideoService = new TagOfVideoService(dataAccessManager);
                }
                return tagOfVideoService;
            }
        }

        public TipFrontService TipFrontService
        {
            get
            {
                if (this.tipFrontService == null)
                {
                    this.tipFrontService = new TipFrontService(dataAccessManager);
                }
                return tipFrontService;
            }
        }

        public VideoService VideoService
        {
            get
            {
                if (this.videoService == null)
                {
                    this.videoService = new VideoService(dataAccessManager);
                }
                return videoService;
            }
        }
        public VideoFrontService VideoFrontService
        {
            get
            {
                if (this.videoFrontService == null)
                {
                    this.videoFrontService = new VideoFrontService(dataAccessManager);
                }
                return videoFrontService;
            }
        }

        public PollFrontService PollFrontService
        {
            get
            {
                if (this.pollFrontService == null)
                {
                    this.pollFrontService = new PollFrontService(dataAccessManager);
                }
                return pollFrontService;
            }
        }

        public PollService PollService
        {
            get
            {
                if (this.pollService == null)
                {
                    this.pollService = new PollService(dataAccessManager);
                }
                return pollService;
            }
        }

        public SiteMapService SiteMapService
        {
            get
            {
                if (this.siteMapService == null)
                {
                    this.siteMapService = new SiteMapService(dataAccessManager);
                }
                return siteMapService;
            }
        }

        public TipService TipService
        {
            get
            {
                if (this.tipService == null)
                {
                    this.tipService = new TipService(dataAccessManager);
                }
                return tipService;
            }
        }

        public TagService TagService
        {
            get
            {
                if (this.tagService == null)
                {
                    this.tagService = new TagService(dataAccessManager);
                }
                return tagService;
            }
        }

        public StaticPagesService StaticPagesService
        {
            get
            {
                if (this.staticPagesService == null)
                {
                    this.staticPagesService = new StaticPagesService(dataAccessManager);
                }
                return staticPagesService;
            }
        }
        public StaticPagesFrontService StaticPagesFrontService
        {
            get
            {
                if (this.staticPagesFrontService == null)
                {
                    this.staticPagesFrontService = new StaticPagesFrontService(dataAccessManager);
                }
                return staticPagesFrontService;
            }
        }

        public AccountService AccountService
        {
            get
            {
                if (this.accountService == null)
                {
                    this.accountService = new AccountService(dataAccessManager);
                }
                return accountService;
            }
        }
        public CountryFrontSrevice  CountryFrontService
        {
            get
            {
                if (this.countryFrontService == null)
                {
                    this.countryFrontService = new CountryFrontSrevice(dataAccessManager);
                }
                return countryFrontService;
            }
        }


        public AccountFrontService  AccountFrontService
        {
            get
            {
                if (this.accountFrontService == null)
                {
                    this.accountFrontService = new AccountFrontService(dataAccessManager);
                }
                return accountFrontService;
            }
        }

        public ArticleService ArticleService
        {
            get
            {
                if (this.articleService == null)
                {
                    this.articleService = new ArticleService(dataAccessManager);
                }
                return articleService;
            }
        }
        
        public ArticleFrontService ArticleFrontService
        {
            get
            {
                if (this.articleFrontService == null)
                {
                    this.articleFrontService = new ArticleFrontService(dataAccessManager);
                }
                return articleFrontService;
            }
        }

        public ImageService ImageService
        {
            get
            {
                if (this.imageService == null)
                {
                    this.imageService = new ImageService();
                }
                return imageService;
            }
        }

        public QuoteService  QuoteService
        {
            get
            {
                if (this.quoteService == null)
                {
                    this.quoteService = new QuoteService(dataAccessManager);
                }
                return quoteService;
            }
        }

        public AuthorService AuthorService
        {
            get
            {
                if (this.authorService == null)
                {
                    this.authorService = new AuthorService(dataAccessManager);
                }
                return authorService;
            }
        }

        public AdvertiseInfoFrontService AdvertiseInfoFrontService
        {
            get
            {
                if (this.advertiseInfoFrontService == null)
                {
                    this.advertiseInfoFrontService = new AdvertiseInfoFrontService(dataAccessManager);
                }
                return advertiseInfoFrontService;
            }
        }

        public ContactInfoFrontService ContactInfoFrontService
        {
            get
            {
                if (this.contactInfoFrontService == null)
                {
                    this.contactInfoFrontService = new ContactInfoFrontService(dataAccessManager);
                }
                return contactInfoFrontService;
            }
        }
     
        #endregion









    }
}
