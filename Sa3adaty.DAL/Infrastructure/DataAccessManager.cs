using Sa3adaty.DAL.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sa3adaty.DAL.Repositories;


namespace Sa3adaty.DAL.Infrastructure
{
    public class DataAccessManager
    {
        #region Privates
        private const string OBJECT_CONTEXT_KEY = "DbContext";

        public  Sa3adatyEntities context;
        
        private UserProfilesRepository userProfileRepository;
        private ArticlesRepository articlesRepository;
        private CategoriesRepository  categoriesRepository;
        private ImagesRepository  imagesRepository;
        private CategoryImagesRepository  categoryImagesRepository;
        private ArticleImagesRepository articleImagesRepository;
        private ArticleCategoriesRepository articleCategoriesRepository;
        private CommentsRepository commentsRepository;
        private LikesRepository articleLikesRepository;
        private AuthorsRepository authorsRepository;
        private UserImagesRepository userImagesRepository;
        private QuotesRepository quotesRepository;
        private SubscriptionsRepository subscriptionRepository;
        private CountriesRepository countriesRepository;
        private ContactInfoRepository contactInfoRepository;
        private AdvertiseInfoRepository advertiseInfoRepository;
        private StaticPagesRepository staticPagesRepository;
        private LogsRepository logsRepository;
        private ArticleTagsRepository articleTagsRepository;
        private AuthorImagesRepository authorImagesRepository;
        private TipsRepository tipsRepository;
        private CampaignsRepository campaignsRepository;
        private TagsRepository tagsRepository;
        private PollsRepository pollsRepository;
        private PollAnswersRepository pollAnswersRepository;
        private PollUsersAnswersRepository pollUsersAnswersRepository;
        private VideosRepository videosRepository;
        private TagOfVideoRepository tagOfVideosRepository;
        private VideoTagsRepository videoTagsRepository;
        private VideoImagesRepository videoImagesRepository;
        private VideoCommentsRepository videoCommentsRepository;
        private VideoCategoriesRepository videoCategoriesRepository;
        #endregion 

        #region Constructor
        public DataAccessManager()
        {
            context = (Sa3adatyEntities)ContextManager.GetDBContext(OBJECT_CONTEXT_KEY);

            //Turning off lazy loading will prefent automatically loading of entities from the database. http://msdn.microsoft.com/en-us/data/jj574232.aspx
            context.Configuration.LazyLoadingEnabled = false;
        }
        #endregion

        #region Repositories
        public VideoCategoriesRepository VideoCategoriesRepository
        {
            get
            {
                if (this.videoCategoriesRepository == null)
                {
                    this.videoCategoriesRepository = new VideoCategoriesRepository(context);
                }
                return videoCategoriesRepository;
            }
        }
        public VideoCommentsRepository VideoCommentsRepository
        {
            get
            {
                if (this.videoCommentsRepository == null)
                {
                    this.videoCommentsRepository = new VideoCommentsRepository(context);
                }
                return videoCommentsRepository;
            }
        }
        public VideoTagsRepository VideoTagsRepository
        {
            get
            {
                if (this.videoTagsRepository == null)
                {
                    this.videoTagsRepository = new VideoTagsRepository(context);
                }
                return videoTagsRepository;
            }
        }
        public VideoImagesRepository VideoImagesRepository
        {
            get
            {
                if (this.videoImagesRepository == null)
                {
                    this.videoImagesRepository = new VideoImagesRepository(context);
                }
                return videoImagesRepository;
            }
        }   
        public TagOfVideoRepository TagOfVideoRepository
        {
            get
            {
                if (this.tagOfVideosRepository == null)
                {
                    this.tagOfVideosRepository = new TagOfVideoRepository(context);
                }
                return tagOfVideosRepository;
            }
        }   
        public VideosRepository VideosRepository
        {
            get
            {
                if (this.videosRepository == null)
                {
                    this.videosRepository = new VideosRepository(context);
                }
                return videosRepository;
            }
        }   
        public PollUsersAnswersRepository PollUsersAnswersRepository
        {
            get
            {
                if (this.pollUsersAnswersRepository == null)
                {
                    this.pollUsersAnswersRepository = new PollUsersAnswersRepository(context);
                }
                return pollUsersAnswersRepository;
            }
        }   
        public PollAnswersRepository PollAnswersRepository
        {
            get
            {
                if (this.pollAnswersRepository == null)
                {
                    this.pollAnswersRepository = new PollAnswersRepository(context);
                }
                return pollAnswersRepository;
            }
        }        

        public PollsRepository PollsRepository
        {
            get
            {
                if (this.pollsRepository == null)
                {
                    this.pollsRepository = new PollsRepository(context);
                }
                return pollsRepository;
            }
        }        

        public TagsRepository TagsRepository
        {
            get
            {
                if (this.tagsRepository == null)
                {
                    this.tagsRepository = new TagsRepository(context);
                }
                return tagsRepository;
            }
        }        

        public CampaignsRepository CampaignsRepository
        {
            get
            {
                if (this.campaignsRepository == null)
                {
                    this.campaignsRepository = new CampaignsRepository(context);
                }
                return campaignsRepository;
            }
        }        

        public TipsRepository TipsRepository
        {
            get
            {
                if (this.tipsRepository == null)
                {
                    this.tipsRepository = new TipsRepository(context);
                }
                return tipsRepository;
            }
        }

        public AuthorImagesRepository AuthorImagesRepository
        {
            get
            {
                if (this.authorImagesRepository == null)
                {
                    this.authorImagesRepository = new AuthorImagesRepository(context);
                }
                return authorImagesRepository;
            }
        }

        public UserProfilesRepository UserProfilesRepository
        {
             get
            {
                if (this.userProfileRepository == null)
                {
                    this.userProfileRepository = new UserProfilesRepository(context);
                }
                return userProfileRepository;
            }
        }

        public ArticleTagsRepository ArticleTagsRepository
        {
            get
            {
                if (this.articleTagsRepository == null)
                {
                    this.articleTagsRepository = new ArticleTagsRepository(context);
                }
                return articleTagsRepository;
            }
        }

        public ArticlesRepository ArticlesRepository
        {
            get
            {
                if (this.articlesRepository == null)
                {
                    this.articlesRepository = new ArticlesRepository(context);
                }
                return articlesRepository;
            }
        }

        public LogsRepository LogsRepository
        {
            get
            {
                if (this.logsRepository == null)
                {
                    this.logsRepository = new LogsRepository(context);
                }
                return logsRepository;
            }
        }

        public StaticPagesRepository StaticPagesRepository
        {
            get
            {
                if (this.staticPagesRepository == null)
                {
                    this.staticPagesRepository = new StaticPagesRepository(context);
                }
                return staticPagesRepository;
            }
        }


        public CategoryImagesRepository  CategoryImagesRepository
        {
            get
            {
                if (this.categoryImagesRepository == null)
                {
                    this.categoryImagesRepository = new CategoryImagesRepository(context);
                }
                return categoryImagesRepository;
            }
        }

        public ArticleImagesRepository ArticleImagesRepository
        {
            get
            {
                if (this.articleImagesRepository == null)
                {
                    this.articleImagesRepository = new ArticleImagesRepository(context);
                }
                return articleImagesRepository;
            }
        }

        public CategoriesRepository CategoriesRepository
        {
            get
            {
                if (this.categoriesRepository == null)
                {
                    this.categoriesRepository = new CategoriesRepository(context);
                }
                return categoriesRepository;
            }
        }
        
        public ImagesRepository  ImagesRepository
        {
            get
            {
                if (this.imagesRepository == null)
                {
                    this.imagesRepository = new ImagesRepository(context);
                }
                return imagesRepository;
            }
        }

        public AuthorsRepository AuthorsRepository
        {
            get
            {
                if (this.authorsRepository == null)
                {
                    this.authorsRepository = new AuthorsRepository(context);
                }
                return authorsRepository;
            }
        }

        public ArticleCategoriesRepository  ArticleCategoriesRepository
        {
            get
            {
                if (this.articleCategoriesRepository == null)
                {
                    this.articleCategoriesRepository = new ArticleCategoriesRepository(context);
                }
                return articleCategoriesRepository;
            }
        }

        public UserImagesRepository UserImagesRepository
        {
            get
            {
                if (this.userImagesRepository == null)
                {
                    this.userImagesRepository = new UserImagesRepository(context);
                }
                return userImagesRepository;
            }
        }

        public QuotesRepository QuotesRepository
        {
            get
            {
                if (this.quotesRepository == null)
                {
                    this.quotesRepository = new QuotesRepository(context);
                }
                return quotesRepository;
            }
        }

        public CommentsRepository CommentsRepository
        {
            get
            {
                if (this.commentsRepository == null)
                {
                    this.commentsRepository = new CommentsRepository(context);
                }
                return commentsRepository;
            }
        }

        public SubscriptionsRepository SubscriptionsRepository
        {
            get
            {
                if (this.subscriptionRepository == null)
                {
                    this.subscriptionRepository = new SubscriptionsRepository(context);
                }
                return subscriptionRepository;
            }
        }

        public LikesRepository ArticleLikesRepository
        {
            get
            {
                if (this.articleLikesRepository == null)
                {
                    this.articleLikesRepository = new LikesRepository(context);
                }
                return articleLikesRepository;
            }
        }

        public CountriesRepository CountriesRepository
        {
            get
            {
                if (this.countriesRepository == null)
                {
                    this.countriesRepository = new CountriesRepository(context);
                }
                return countriesRepository;
            }
        }

        public ContactInfoRepository ContactInfoRepository
        {
            get
            {
                if (this.contactInfoRepository == null)
                {
                    this.contactInfoRepository = new ContactInfoRepository(context);
                }
                return contactInfoRepository;
            }
        }
        public AdvertiseInfoRepository AdvertiseInfoRepository
        {
            get
            {
                if (this.advertiseInfoRepository == null)
                {
                    this.advertiseInfoRepository = new AdvertiseInfoRepository(context);
                }
                return advertiseInfoRepository;
            }
        }
        #endregion
       
        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
