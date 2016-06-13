using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sa3adaty.Common;
using Sa3adaty.Core.ViewModels.Articles;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;
using System.Data.Entity;

namespace Sa3adaty.Core.Services
{
    public class ArticleFrontService
    {
        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public ArticleFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods
            public ArticleViewModel GetArticleByURL(string url,int current_user_id = 0 )
            {
                Article DB_article;

                DB_article = DAManager.ArticlesRepository.Get(a => a.URL.Trim() == url.Trim() && a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, null, "ArticleCategories.Category,ArticleImages,ArticleImages.Image,Likes,Comments,Comments.Userprofile.UserImages.Image,ArticleTags,Author.AuthorImages.Image").FirstOrDefault();
               
                if (DB_article == null)
                    return null;

                DB_article.NumberOfViews += 1;

                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                }

                ArticleViewModel article_viewModel = new ArticleViewModel()
                {
                    ArticleId = DB_article.ArticleId,
                    MetaDescription = DB_article.MetaDescription,
                    MetaTitle = DB_article.MetaTitle,
                    PublishDate = DB_article.PublishDate,
                    Description = DB_article.Description,
                    Title = DB_article.Title,
                    Views = DB_article.NumberOfViews,
                    LikesCount = DB_article.Likes.Count(),
                    CommentsCount = DB_article.Comments.Count(),
                    URL = DB_article.URL,
                    Author = new AuthorInfoViewModel{ FullName= DB_article.Author.Name ,IsProfileEnabled = DB_article.Author.IsProfileEnabled??false , URL = DB_article.Author.URL  },
                };
                
                if (DB_article.ArticleImages.Count() > 0)
                    article_viewModel.ImageURL = ImageService.GenerateImageFullPath(DB_article.ArticleImages.First().Image.URL,ArticleService.ArticleThumbWidth.ToString(),ArticleService.ArticleThumbHeight.ToString());
                
                //fill author image
                if (DB_article.Author.AuthorImages != null && DB_article.Author.AuthorImages.Count > 0)
                {
                    article_viewModel.Author.ImageURL = ImageService.GenerateImageFullPath(DB_article.Author.AuthorImages.FirstOrDefault().Image.URL, AuthorService.AuthorThumbWidth.ToString(), AuthorService.AuthorThumbHeight.ToString());
                }

                //Fill Article Categoyr
                if (DB_article.ArticleCategories.Count > 0)
                {
                    article_viewModel.CategoryName = DB_article.ArticleCategories.First().Category.Name;
                    article_viewModel.CategoryURL = DB_article.ArticleCategories.First().Category.URL;
                }

                article_viewModel.Comments = new List<CommentViewModel>();

                //Fill Article Comments
                foreach (Comment AC in DB_article.Comments.Where(c=> c.ParentId == null).OrderBy(c=> c.AddDate))
                {
                    article_viewModel.Comments.Add(GetCommentTree(AC));
                }

                //check if current user liked this article
                if (current_user_id != 0)
                {
                    if (DB_article.Likes.Where(l => l.UserId == current_user_id).Count() > 0)
                        article_viewModel.IsLikedByCurrentUser = true;
                }
                
                //Fill tags
                article_viewModel.Tags = new List<string>();
                if (DB_article.ArticleTags != null)
                {
                    foreach(ArticleTag tag in DB_article.ArticleTags)
                    {
                        article_viewModel.Tags.Add(tag.Tag);
                    }
                }

                return article_viewModel;
            }


            public string GetArticleURLById(int article_id)
            {
                Article db_article = DAManager.ArticlesRepository.Get(a => a.ArticleId == article_id).FirstOrDefault();
                if (db_article == null)
                    return "";
                return db_article.URL;
            }

            public string GetArticleURLByImageId(int image_id)
            {
                Image db_image = DAManager.ImagesRepository.Get(i => i.ImageId == image_id, null, "ArticleImages.Article").FirstOrDefault();

                if (db_image == null)
                    return "";

                return db_image.ArticleImages.FirstOrDefault().Article.URL;
            }

            public static  CommentViewModel GetCommentTree(Comment db_comment)
            {
                CommentViewModel comment = new CommentViewModel() {CommentId= db_comment.CommentId, ParentId = db_comment.ParentId , AddedDate = db_comment.AddDate, Text = db_comment.Text,Username =  db_comment.UserProfile.Name };
                if (db_comment.UserProfile.UserImages.Count() > 0)
                    comment.UserImageURL = ImageService.GenerateImageFullPath(db_comment.UserProfile.UserImages.First().Image.URL,AccountService.UserThumbWidth.ToString(),AccountService.UserThumbHeight.ToString());
                comment.Replies = new List<CommentViewModel>();

                foreach (Comment sub_comment in db_comment.Comment1.OrderBy(c => c.AddDate))
                    comment.Replies.Add(GetCommentTree(sub_comment));

                return comment;
            }

            public CommentViewModel GetCommentById(int comment_id)
            {
                Comment db_comment = DAManager.CommentsRepository.Get(c => c.CommentId == comment_id , null, "UserProfile.UserImages.Image").FirstOrDefault();
                if (db_comment != null)
                    return GetCommentTree(db_comment);

                return null;
            }

            public int AddComment(int article_id, int user_id, string text,int? parent_id = null)
            {
                Comment db_comment = new Comment() { AddDate = DateTime.Now, Text = text, UserID = user_id, ArticleId = article_id,ParentId = parent_id  };
                DAManager.CommentsRepository.Insert(db_comment);
                try
                {
                    DAManager.Save();
                    return db_comment.CommentId;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }
            }
            
            public List<ListArticleViewModel> GetLatestArticles(int count, int image_width, int image_height, List<int> except = null)
            {
                IQueryable<Article> db_articles = DAManager.ArticlesRepository.Get(a => a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => ar.PublishDate), "Likes,Comments,ArticleImages.Image").Take(count);
                if (except != null)
                {
                    foreach (int ex in except)
                    {
                        db_articles = db_articles.Where(a => a.ArticleId != ex);
                    }
                }

                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();

                foreach (Article article in db_articles.ToList())
                {
                    ListArticleViewModel article_item = new ListArticleViewModel()
                    {
                        Title = article.Title,
                        CommentsCount = article.Comments.Count(),
                        URL = article.URL,
                        LikesCount = article.Likes.Count(),
                        ArticleId = article.ArticleId,
                    };
                    if (article.ArticleImages.Count() > 0)
                        article_item.ImageURL = article.ArticleImages.First().Image.URL;

                    final_result.Add(article_item);
                }

                return FormatImageURL(final_result, image_width, image_height);
            }

            public List<ListArticleViewModel> GetLatestArticleOfEachCategory(int image_width, int image_height,int description_length = 100)
            {
                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();

                List<Category> online_categories = DAManager.CategoriesRepository.Get(c => c.IsPublished == true).ToList();

                IQueryable<ArticleCategory> AC = null;
                foreach (Category c in online_categories)
                {
                    if (AC == null)
                        AC = DAManager.ArticleCategoriesRepository.Get(a => a.Article.IsPublished == true && DbFunctions.CreateDateTime(a.Article.PublishDate.Year, a.Article.PublishDate.Month, a.Article.PublishDate.Day, a.Article.PublishDate.Hour, a.Article.PublishDate.Minute, a.Article.PublishDate.Second) <= DateTime.Now && a.CategoryId == c.CategoryId, a => a.OrderByDescending(ar => ar.Article.PublishDate), "Article,Article.ArticleImages.Image").Take(1).AsQueryable();
                    else
                        AC = AC.Union(DAManager.ArticleCategoriesRepository.Get(a => a.Article.IsPublished == true && DbFunctions.CreateDateTime(a.Article.PublishDate.Year, a.Article.PublishDate.Month, a.Article.PublishDate.Day, a.Article.PublishDate.Hour, a.Article.PublishDate.Minute, a.Article.PublishDate.Second) <= DateTime.Now && a.CategoryId == c.CategoryId, a => a.OrderByDescending(ar => ar.Article.PublishDate), "Article,Article.ArticleImages.Image").Take(1).AsQueryable());
                }


                foreach (ArticleCategory article_c in AC.ToList())
                {
                    ListArticleViewModel item = new ListArticleViewModel() {URL = article_c.Article.URL, Title = article_c.Article.Title,PublishDate = article_c.Article.PublishDate };
                    if (article_c.Article.ArticleImages.Count() > 0)
                        item.ImageURL = article_c.Article.ArticleImages.First().Image.URL;

                    item.Description = StringsUtility.StripHTML(article_c.Article.Description);
                    if (item.Description.Length > description_length)
                    {
                        item.Description = item.Description.Substring(0, description_length);
                    }

                    final_result.Add(item);
                }


                return FormatImageURL(final_result.OrderByDescending(a=> a.PublishDate).ToList(),image_width,image_height);
            }

            public List<ListArticleViewModel> GetHomeAllLatest(int count, List<ListArticleViewModel> except,int description_size = 100)
            {
                IQueryable<Article> db_articles = DAManager.ArticlesRepository.Get(a => a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => ar.PublishDate), "Likes,Comments,ArticleImages.Image");
                if (except != null)
                {
                    foreach (ListArticleViewModel ex in except)
                    {
                        db_articles = db_articles.Where(a => a.URL != ex.URL);
                    }
                }

                db_articles = db_articles.Take(count);

                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();

                foreach (Article article in db_articles.ToList())
                {
                    ListArticleViewModel article_item = new ListArticleViewModel()
                    {
                        Title = article.Title,
                        PublishDate = article.PublishDate,
                        CommentsCount = article.Comments.Count(),
                        URL = article.URL,
                        LikesCount = article.Likes.Count()
                    };
                    article_item.Description = Regex.Replace(article.Description, @"<[^>]*>", String.Empty);
                    if (article_item.Description.Length > description_size)
                    {
                        article_item.Description = article_item.Description.Substring(0, description_size);
                    }

                    if (article.ArticleImages.Count() > 0)
                        article_item.ImageURL = article.ArticleImages.First().Image.URL;

                    final_result.Add(article_item);
                }

                return final_result;
            }

            public List<ListArticleViewModel> GetArticlesPerFeatureCategories(int count, List<ListArticleViewModel> except,int image_width, int image_height, int description_size = 100)
            {
                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();
                List<string> except_list = except.Select(a => a.URL).ToList();
                List<Category> online_categories = DAManager.CategoriesRepository.Get(c => c.IsPublished == true && c.FeatureOrder != null ).ToList();

                IQueryable<ArticleCategory> AC = null;
                foreach (Category c in online_categories)
                {
                    if (AC == null)
                        AC = DAManager.ArticleCategoriesRepository.Get(a => !except_list.Contains(a.Article.URL) && a.Article.IsPublished == true && DbFunctions.CreateDateTime(a.Article.PublishDate.Year, a.Article.PublishDate.Month, a.Article.PublishDate.Day, a.Article.PublishDate.Hour, a.Article.PublishDate.Minute, a.Article.PublishDate.Second) <= DateTime.Now && a.CategoryId == c.CategoryId, a => a.OrderByDescending(ar => ar.Article.PublishDate), "Article,Article.ArticleImages.Image").Take(count).AsQueryable();
                    else
                        AC = AC.Union(DAManager.ArticleCategoriesRepository.Get(a => !except_list.Contains(a.Article.URL) && a.Article.IsPublished == true && DbFunctions.CreateDateTime(a.Article.PublishDate.Year, a.Article.PublishDate.Month, a.Article.PublishDate.Day, a.Article.PublishDate.Hour, a.Article.PublishDate.Minute, a.Article.PublishDate.Second) <= DateTime.Now && a.CategoryId == c.CategoryId, a => a.OrderByDescending(ar => ar.Article.PublishDate), "Article,Article.ArticleImages.Image").Take(count).AsQueryable());
                }


                foreach (ArticleCategory article_c in AC.ToList())
                {
                    ListArticleViewModel item = new ListArticleViewModel() {CategoryId = article_c.CategoryId  , URL = article_c.Article.URL, Title = article_c.Article.Title, PublishDate = article_c.Article.PublishDate };
                    if (article_c.Article.ArticleImages.Count() > 0)
                        item.ImageURL = article_c.Article.ArticleImages.First().Image.URL;

                    item.Description = StringsUtility.StripHTML(article_c.Article.Description);
                    if (item.Description.Length > description_size)
                    {
                        item.Description = item.Description.Substring(0, description_size);
                    }

                    final_result.Add(item);
                }


                return FormatImageURL(final_result.OrderByDescending(a => a.PublishDate).ToList(), image_width, image_height);
            }

            public List<ListArticleViewModel> GetRelatedArticles(int article_id, int count, int image_width, int image_height)
            {
                //get article category and then random 3 articles from that category
                Article original_article = DAManager.ArticlesRepository.Get(a => a.ArticleId == article_id, null, "ArticleCategories").FirstOrDefault();

                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();

                if (original_article == null)
                    return final_result;

                if (original_article.ArticleCategories.Count() > 0)
                {
                    int category_id = original_article.ArticleCategories.First().CategoryId;

                    IQueryable<ArticleCategory> related_articles = DAManager.ArticleCategoriesRepository.Get(ac => ac.CategoryId == category_id && ac.Article.IsPublished == true && DbFunctions.CreateDateTime(ac.Article.PublishDate.Year, ac.Article.PublishDate.Month, ac.Article.PublishDate.Day, ac.Article.PublishDate.Hour, ac.Article.PublishDate.Minute, ac.Article.PublishDate.Second) <= DateTime.Now, ac => ac.OrderByDescending(a => a.Article.AddDate), "Article,Article.ArticleImages,Article.ArticleImages.Image").Take(count);

                    List<ArticleCategory> res = related_articles.ToList();

                    foreach (ArticleCategory article in res)
                    {
                        ListArticleViewModel article_item = new ListArticleViewModel()
                        {
                            Title = article.Article.Title,
                            CommentsCount = article.Article.Comments.Count(),
                            URL = article.Article.URL,
                            LikesCount = article.Article.Likes.Count()
                        };
                        if (article.Article.ArticleImages.Count() > 0)
                            article_item.ImageURL = article.Article.ArticleImages.First().Image.URL;

                        final_result.Add(article_item);
                    }


                }

                return FormatImageURL(final_result, image_width, image_height);
            }

            public List<ListArticleViewModel> GetRelatedArticles(int article_id, string article_title, int count, int image_width, int image_height, List<int> except_ids)
            {
                //get comma separated list from except_ids
                string comma_list ="";
                foreach (int id in except_ids)
                    comma_list += id.ToString() + ",";

                if(comma_list != "" )
                    comma_list = comma_list.Substring(0,comma_list.Length-1);

                //get article category and then random 3 articles from that category
                List<SearchArticles_Result> search_result = DAManager.ArticlesRepository.SearchArticles(article_title, 1, 6, comma_list);

                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();

                foreach (SearchArticles_Result article in search_result)
                {
                    ListArticleViewModel article_item = new ListArticleViewModel()
                    {
                        Title = article.Title,
                        //CommentsCount = article.Article.Comments.Count(),
                        URL = article.URL,
                        //LikesCount = article.Article.Likes.Count()
                    };
                    if (article.ImageURL != "")
                        article_item.ImageURL = article.ImageURL;

                    final_result.Add(article_item);
                }
                final_result = FormatImageURL(final_result,image_width,image_height);

                if (final_result.Count() < count)
                {
                    final_result = final_result.Union(GetRelatedArticles(article_id, count - final_result.Count(), image_width, image_height)).ToList();
                }

                return final_result;
            }

            public List<ListArticleViewModel> GetRelatedArticles( string article_title, int count, int image_width, int image_height, List<int> except_ids)
            {
                //get comma separated list from except_ids
                string comma_list = "";
                if (except_ids != null)
                    foreach (int id in except_ids)
                        comma_list += id.ToString() + ",";

                if (comma_list != "")
                    comma_list = comma_list.Substring(0, comma_list.Length - 1);

                //get article category and then random 3 articles from that category
                List<SearchArticles_Result> search_result = DAManager.ArticlesRepository.SearchArticles(article_title, 1, 6, comma_list);

                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();

                foreach (SearchArticles_Result article in search_result)
                {
                    ListArticleViewModel article_item = new ListArticleViewModel()
                    {
                        Title = article.Title,
                        //CommentsCount = article.Article.Comments.Count(),
                        URL = article.URL,
                        //LikesCount = article.Article.Likes.Count()
                    };
                    if (article.ImageURL != "")
                        article_item.ImageURL = article.ImageURL;

                    final_result.Add(article_item);
                }
                final_result = FormatImageURL(final_result, image_width, image_height);

                return final_result;
            }
            public List<ListArticleViewModel> GetRecommendedArticles(int article_id, int count, int image_width, int image_height)
            {
                //get article category and then random 3 articles from that category
                Article original_article = DAManager.ArticlesRepository.Get(a => a.ArticleId == article_id, null, "ArticleCategories").FirstOrDefault();

                List<ListArticleViewModel> final_result = new List<ListArticleViewModel>();

                if (original_article == null)
                    return final_result;

                if (original_article.ArticleCategories.Count() > 0)
                {
                    int category_id = original_article.ArticleCategories.First().CategoryId;

                    IQueryable<ArticleCategory> related_articles = DAManager.ArticleCategoriesRepository.Get(ac => ac.CategoryId == category_id && ac.Article.IsPublished == true && DbFunctions.CreateDateTime(ac.Article.PublishDate.Year, ac.Article.PublishDate.Month, ac.Article.PublishDate.Day, ac.Article.PublishDate.Hour, ac.Article.PublishDate.Minute, ac.Article.PublishDate.Second) <= DateTime.Now, ac => ac.OrderByDescending(a => a.Article.NumberOfViews), "Article,Article.ArticleImages,Article.ArticleImages.Image").Take(3);

                    List<ArticleCategory> res = related_articles.ToList();

                    foreach (ArticleCategory article in res)
                    {
                        ListArticleViewModel article_item = new ListArticleViewModel()
                        {
                            Title = article.Article.Title,
                            CommentsCount = article.Article.Comments.Count(),
                            URL = article.Article.URL,
                            LikesCount = article.Article.Likes.Count()
                        };
                        if (article.Article.ArticleImages.Count() > 0)
                            article_item.ImageURL = article.Article.ArticleImages.First().Image.URL;

                        final_result.Add(article_item);
                    }


                }


                return FormatImageURL( final_result,image_width,image_height);
            }

            public List<ListArticleViewModel> FormatImageURL(List<ListArticleViewModel> article_list, int image_width, int image_height)
            {
                foreach (ListArticleViewModel article in article_list)
                {
                    if (article.ImageURL != null && article.ImageURL != "")
                        article.ImageURL = ImageService.GenerateImageFullPath(article.ImageURL, image_width.ToString(), image_height.ToString());
                }
                return article_list;
            }

            public bool LikeArticle(int article_id, int user_id)
            {
                //Check if already liked 
                Like old = DAManager.ArticleLikesRepository.Get(l => l.UserId == user_id && l.ArticleId == article_id).FirstOrDefault();
                if (old != null)
                    return true;
                else
                {
                    Like new_like = new Like() { UserId = user_id, ArticleId = article_id, AddedDate = DateTime.Now };
                    DAManager.ArticleLikesRepository.Insert(new_like);
                    try {
                        DAManager.Save();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                        return false;
                    }
                }
            }

            public CategoryViewModel GetCategoryByURL(string url,int page, int page_size,int image_width, int image_height,int description_lenght = 40 )
            {
                Category DB_category = DAManager.CategoriesRepository.Get(c => c.URL == url && c.IsPublished == true).FirstOrDefault();

                if(DB_category == null)
                    return null;

                CategoryViewModel result = new CategoryViewModel() { MetaDescription = DB_category.MetaDescription, MetaTitle = DB_category.MetaTitle, Name = DB_category.Name ,CategoryId = DB_category.CategoryId, URL = DB_category.URL  };
                result.Articles = GetCategoryArticles(DB_category.CategoryId, page, page_size, image_width, image_height, description_lenght);

                int total_articles = DAManager.ArticleCategoriesRepository.Get(a => a.Article.IsPublished == true && DbFunctions.CreateDateTime(a.Article.PublishDate.Year, a.Article.PublishDate.Month, a.Article.PublishDate.Day, a.Article.PublishDate.Hour, a.Article.PublishDate.Minute, a.Article.PublishDate.Second) <= DateTime.Now && a.CategoryId == DB_category.CategoryId).Count();

                //Set pagination properties
                result.PageNumber = page;
                result.PageSize = page_size;
                result.TotalItems = total_articles;

                return result;
            }

            public CategoryViewModel GetArticlesTag(string tag, int page, int page_size, int image_width, int image_height, int description_length = 40)
            {
                tag = tag.Replace("-", "_");
                Tag db_tag = DAManager.TagsRepository.Get(t => t.TagName == tag).FirstOrDefault();
                if (db_tag == null)
                {
                    return null;
                }

                CategoryViewModel result_viewmodel = new CategoryViewModel() { Name = db_tag.FrontTitle , MetaDescription = db_tag.MetaDescription, MetaTitle = db_tag.MetaTitle };
                List<ListArticleViewModel> result = new List<ListArticleViewModel>();

                IQueryable<ArticleTag> tag_articles = DAManager.ArticleTagsRepository.Get(at => at.Tag == tag && at.Article.IsPublished == true && DbFunctions.CreateDateTime(at.Article.PublishDate.Year, at.Article.PublishDate.Month, at.Article.PublishDate.Day, at.Article.PublishDate.Hour, at.Article.PublishDate.Minute, at.Article.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => ar.Article.PublishDate), "Article.ArticleImages.Image,Article.Likes,Article.Comments").Skip((page - 1) * page_size).Take(page_size);

                foreach (ArticleTag AC in tag_articles.ToList())
                {
                    ListArticleViewModel article = new ListArticleViewModel() { CommentsCount = AC.Article.Comments.Count(), LikesCount = AC.Article.Likes.Count(), Title = AC.Article.Title, URL = AC.Article.URL };
                    article.Description = StringsUtility.StripHTML(AC.Article.Description);
                    if (article.Description.Length > description_length)
                    {
                        article.Description = article.Description.Substring(0, description_length);
                    }
                    if (AC.Article.ArticleImages.Count() > 0)
                        article.ImageURL = ImageService.GenerateImageFullPath(AC.Article.ArticleImages.First().Image.URL, image_width.ToString(), image_height.ToString());

                    result.Add(article);
                }

                
                
                result_viewmodel.Articles = result;

                int total_articles = DAManager.ArticleTagsRepository.Get(at => at.Tag == tag && at.Article.IsPublished == true && DbFunctions.CreateDateTime(at.Article.PublishDate.Year, at.Article.PublishDate.Month, at.Article.PublishDate.Day, at.Article.PublishDate.Hour, at.Article.PublishDate.Minute, at.Article.PublishDate.Second) <= DateTime.Now).Count();
                
                //Set pagination properties
                result_viewmodel.PageNumber = page;
                result_viewmodel.PageSize = page_size;
                result_viewmodel.TotalItems = total_articles;
                            
                return result_viewmodel;
            }

            public List<ListArticleViewModel> GetTagArticles(string tag, int page, int page_size, int image_width, int image_height, int description_length = 40)
            {
                tag = tag.Replace("-", "_");
                List<ListArticleViewModel> result = new List<ListArticleViewModel>();

                IQueryable<ArticleTag> tag_articles = DAManager.ArticleTagsRepository.Get(at => at.Tag == tag && at.Article.IsPublished == true && DbFunctions.CreateDateTime(at.Article.PublishDate.Year, at.Article.PublishDate.Month, at.Article.PublishDate.Day, at.Article.PublishDate.Hour, at.Article.PublishDate.Minute, at.Article.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => ar.Article.PublishDate), "Article.ArticleImages.Image,Article.Likes,Article.Comments").Skip((page - 1) * page_size).Take(page_size);

                foreach (ArticleTag AC in tag_articles.ToList())
                {
                    ListArticleViewModel article = new ListArticleViewModel() { CommentsCount = AC.Article.Comments.Count(), LikesCount = AC.Article.Likes.Count(), Title = AC.Article.Title, URL = AC.Article.URL };
                    article.Description = StringsUtility.StripHTML(AC.Article.Description);
                    if (article.Description.Length > description_length)
                    {
                        article.Description = article.Description.Substring(0, description_length);
                    }
                    if (AC.Article.ArticleImages.Count() > 0)
                        article.ImageURL = ImageService.GenerateImageFullPath(AC.Article.ArticleImages.First().Image.URL, image_width.ToString(), image_height.ToString());

                    result.Add(article);
                }

                return result;
            }

            public string GetCategoryURLById(int category_id)
            {
                Category db_cat = DAManager.CategoriesRepository.Get(c => c.CategoryId == category_id).FirstOrDefault();

                if (db_cat == null)
                    return "";

                return db_cat.URL;
            }

            public string GetCategoryURLByImageId(int image_id)
            {
                Image db_image = DAManager.ImagesRepository.Get(i => i.ImageId == image_id, null, "CategoryImages.Category").FirstOrDefault();

                if (db_image == null)
                    return "";

                return db_image.CategoryImages.FirstOrDefault().Category.URL;
            }
            public List<ListArticleViewModel> GetCategoryArticles(int category_id, int page, int page_size, int image_width, int image_height,int description_length = 40)
            {
                List<ListArticleViewModel> result = new List<ListArticleViewModel>();

                IQueryable<ArticleCategory> category_articles = DAManager.ArticleCategoriesRepository.Get(ac => ac.CategoryId == category_id && ac.Article.IsPublished == true && DbFunctions.CreateDateTime(ac.Article.PublishDate.Year, ac.Article.PublishDate.Month, ac.Article.PublishDate.Day, ac.Article.PublishDate.Hour, ac.Article.PublishDate.Minute, ac.Article.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => ar.Article.PublishDate), "Article.ArticleImages.Image,Article.Likes,Article.Comments").Skip(page * page_size).Take(page_size);
                
                foreach (ArticleCategory AC in category_articles.ToList())
                {
                    ListArticleViewModel article = new ListArticleViewModel(){ CommentsCount = AC.Article.Comments.Count(),LikesCount = AC.Article.Likes.Count(),Title = AC.Article.Title,URL = AC.Article.URL};
                    article.Description = StringsUtility.StripHTML(AC.Article.Description);
                    if (article.Description.Length > description_length)
                    {
                        article.Description = article.Description.Substring(0, description_length);
                    }
                    if(AC.Article.ArticleImages.Count() > 0)
                        article.ImageURL = ImageService.GenerateImageFullPath(AC.Article.ArticleImages.First().Image.URL, image_width.ToString(), image_height.ToString());
                    
                    result.Add(article);
                }

                return result;
            }

            public List<ListArticleViewModel> GetSearchResult(string search_key, int page, int page_size, int image_width, int image_height, int description_length = 40)
            {
                //clear search query 
                search_key = ClearSearchQuery(search_key);

                List<ListArticleViewModel> result = new List<ListArticleViewModel>();

                //IQueryable<Article> category_articles = DAManager.ArticlesRepository.Get(a => a.IsPublished == true && EntityFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now && (a.Title.Contains(search_key)), a => a.OrderByDescending(ar => ar.PublishDate), "ArticleImages.Image,Likes,Comments").Skip((page-1) * page_size).Take(page_size);

                List<SearchArticles_Result> category_articles = DAManager.ArticlesRepository.SearchArticles(search_key, page, page_size);
                foreach (SearchArticles_Result AC in category_articles)
                {
                    ListArticleViewModel article = new ListArticleViewModel() { Title = AC.Title, URL = AC.URL };
                    article.Description = StringsUtility.StripHTML(AC.Description);
                    if (article.Description.Length > description_length)
                    {
                        article.Description = article.Description.Substring(0, description_length);
                    }
                    if (AC.ImageURL != "")
                        article.ImageURL = ImageService.GenerateImageFullPath(AC.ImageURL, image_width.ToString(), image_height.ToString());

                    result.Add(article);
                }
                
                return result;
            }

            public int GetSearchTotalCount(string search_key, int page, int page_size)
            {
                List<ListArticleViewModel> result = new List<ListArticleViewModel>();

                IQueryable<Article> category_articles = DAManager.ArticlesRepository.Get(a => a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now && (a.Title.Contains(search_key)));

                return category_articles.Count();
            }
            public List<ListCategoryViewModel> GetFeaturedCategories()
            {
                return DAManager.CategoriesRepository.Get(c => c.IsPublished, c=> c.OrderBy(a=> a.FeatureOrder)).Select(c=> new ListCategoryViewModel(){CategoryId = c.CategoryId, Name= c.Name, URL = c.URL }).ToList();
            }

         
            public string ClearSearchQuery(string search_query)
            {
                // Replace invalid characters with empty strings. 
                try
                {
                    return Regex.Replace(search_query, @"[^\w\.@-]", "",
                                         RegexOptions.None, TimeSpan.FromSeconds(1.5));
                }
                // If we timeout when replacing invalid characters,  
                // we should return Empty. 
                catch (RegexMatchTimeoutException)
                {
                    return String.Empty;
                }
            }
        #endregion
    }
}
