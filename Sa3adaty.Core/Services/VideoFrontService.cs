using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Common;
using Sa3adaty.Core.ViewModels.Articles;
using Sa3adaty.Core.ViewModels.Videos;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;
using System.Data.Entity;

namespace Sa3adaty.Core.Services
{
    public class VideoFrontService
    {
         #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public VideoFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods
            public VideoViewModel GetVideoByURL(string url, int current_user_id = 0)
            {
                Video DB_video;

                DB_video = DAManager.VideosRepository.Get(a => a.URL.Trim() == url.Trim() && a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, null, "VideoImages,VideoImages.Image,VideoComments,VideoComments.Userprofile.UserImages.Image,VideoTags,Author.AuthorImages.Image,VideoCategory").FirstOrDefault();

                if (DB_video == null)
                    return null;

                DB_video.NumberOfViews += 1;

                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                }

                VideoViewModel video_viewModel = new VideoViewModel()
                {
                    VideoId = DB_video.VideoId,
                    MetaDescription = DB_video.MetaDescription,
                    MetaTitle = DB_video.MetaTitle,
                    PublishDate = DB_video.PublishDate,
                    Description = DB_video.Description,
                    Title = DB_video.Title,
                    Views = DB_video.NumberOfViews,
                    //LikesCount = DB_videoLikes.Count(),
                    CommentsCount = DB_video.VideoComments.Count(),
                    URL = DB_video.URL,
                    Duration = DB_video.Duration,
                    YoutubeId = DB_video.YoutubeId,
                    CategoryName = DB_video.VideoCategory.Name,
                    CategoryURL = DB_video.VideoCategory.URL,
                    Author = new AuthorInfoViewModel { FullName = DB_video.Author.Name ,AuthorId = DB_video.AuthorId.Value,URL = DB_video.Author.URL, IsProfileEnabled = DB_video.Author.IsProfileEnabled??false  },
                };

                if (DB_video.VideoImages.Count() > 0)
                    video_viewModel.ImageURL = ImageService.GenerateImageFullPath(DB_video.VideoImages.First().Image.URL, VideoService.VideoThumbWidth.ToString(), VideoService.VideoThumbHeight.ToString());

                //fill author image
                if (DB_video.Author.AuthorImages != null && DB_video.Author.AuthorImages.Count > 0)
                {
                    video_viewModel.Author.ImageURL = ImageService.GenerateImageFullPath(DB_video.Author.AuthorImages.FirstOrDefault().Image.URL, AuthorService.AuthorThumbWidth.ToString(), AuthorService.AuthorThumbHeight.ToString());
                }

                video_viewModel.Comments = new List<CommentViewModel>();

                //Fill Article Comments
                foreach (VideoComment AC in DB_video.VideoComments.Where(c => c.ParentId == null).OrderBy(c => c.AddDate))
                {
                    video_viewModel.Comments.Add(GetCommentTree(AC));
                }

                //check if current user liked this article
                //if (current_user_id != 0)
                //{
                //    if (DB_video.Likes.Where(l => l.UserId == current_user_id).Count() > 0)
                //        article_viewModel.IsLikedByCurrentUser = true;
                //}

                //Fill tags
                video_viewModel.Tags = new List<string>();
                if (DB_video.VideoTags != null)
                {
                    foreach (VideoTag  tag in DB_video.VideoTags)
                    {
                        video_viewModel.Tags.Add(tag.Tag);
                    }
                }

                return video_viewModel;
            }

            public static CommentViewModel GetCommentTree(VideoComment db_comment)
            {
                CommentViewModel comment = new CommentViewModel() { CommentId = db_comment.CommentId, ParentId = db_comment.ParentId, AddedDate = db_comment.AddDate, Text = db_comment.Text, Username = db_comment.UserProfile.Name };
                if (db_comment.UserProfile.UserImages.Count() > 0)
                    comment.UserImageURL = ImageService.GenerateImageFullPath(db_comment.UserProfile.UserImages.First().Image.URL, AccountService.UserThumbWidth.ToString(), AccountService.UserThumbHeight.ToString());
                comment.Replies = new List<CommentViewModel>();

                foreach (VideoComment sub_comment in db_comment.VideoComment1.OrderBy(c => c.AddDate))
                    comment.Replies.Add(GetCommentTree(sub_comment));

                return comment;
            }

            public int AddComment(int video_id, int user_id, string text, int? parent_id = null)
            {
                VideoComment db_comment = new VideoComment() { AddDate = DateTime.Now, Text = text, UserId = user_id, VideoId = video_id, ParentId = parent_id };
                DAManager.VideoCommentsRepository.Insert(db_comment);
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

            public CommentViewModel GetCommentById(int comment_id)
            {
                VideoComment db_comment = DAManager.VideoCommentsRepository.Get(c => c.CommentId == comment_id, null, "UserProfile.UserImages.Image").FirstOrDefault();
                if (db_comment != null)
                    return GetCommentTree(db_comment);

                return null;
            }

            public VideoCategoryViewModel GetVideosTag(string tag, int page, int page_size, int image_width, int image_height, int description_length = 40)
            {
                tag = tag.Replace("-", "_");
                TagOfVideo  db_tag = DAManager.TagOfVideoRepository.Get(t => t.TagName == tag).FirstOrDefault();
                if (db_tag == null)
                {
                    return null;
                }

                VideoCategoryViewModel result_viewmodel = new VideoCategoryViewModel() { Name = db_tag.FrontTitle, MetaDescription = db_tag.MetaDescription, MetaTitle = db_tag.MetaTitle };
                List<ListVideoViewModel> result = new List<ListVideoViewModel>();

                IQueryable<VideoTag> tag_videos = DAManager.VideoTagsRepository.Get(at => at.Tag == tag && at.Video.IsPublished == true && DbFunctions.CreateDateTime(at.Video.PublishDate.Year, at.Video.PublishDate.Month, at.Video.PublishDate.Day, at.Video.PublishDate.Hour, at.Video.PublishDate.Minute, at.Video.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => ar.Video.PublishDate), "Video.VideoImages.Image").Skip((page - 1) * page_size).Take(page_size);

                foreach (VideoTag AC in tag_videos.ToList())
                {
                    ListVideoViewModel video = new ListVideoViewModel() { Title = AC.Video.Title, URL = AC.Video.URL };
                    video.Description = StringsUtility.StripHTML(AC.Video.Description);
                    if (video.Description.Length > description_length)
                    {
                        video.Description = video.Description.Substring(0, description_length);
                    }
                    if (AC.Video.VideoImages.Count() > 0)
                        video.ImageURL = ImageService.GenerateImageFullPath(AC.Video.VideoImages.First().Image.URL, image_width.ToString(), image_height.ToString());

                    result.Add(video);
                }



                result_viewmodel.Videos = result;

                int total_videos = DAManager.VideoTagsRepository.Get(at => at.Tag == tag && at.Video.IsPublished == true && DbFunctions.CreateDateTime(at.Video.PublishDate.Year, at.Video.PublishDate.Month, at.Video.PublishDate.Day, at.Video.PublishDate.Hour, at.Video.PublishDate.Minute, at.Video.PublishDate.Second) <= DateTime.Now).Count();

                //Set pagination properties
                result_viewmodel.PageNumber = page;
                result_viewmodel.PageSize = page_size;
                result_viewmodel.TotalItems = total_videos;

                return result_viewmodel;
            }

            public List<ListVideoViewModel> GetLatestVideos(int count, int image_width, int image_height, List<int> except = null)
            {
                IQueryable<Video> db_videos = DAManager.VideosRepository.Get(a => a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => ar.PublishDate), "VideoImages.Image").Take(count);
                if (except != null)
                {
                    foreach (int ex in except)
                    {
                        db_videos = db_videos.Where(a => a.VideoId != ex);
                    }
                }

                List<ListVideoViewModel> final_result = new List<ListVideoViewModel>();

                foreach (Video video in db_videos.ToList())
                {
                    ListVideoViewModel video_item = new ListVideoViewModel()
                    {
                        Title = video.Title,
                        //CommentsCount = article.Comments.Count(),
                        URL = video.URL,
                       // LikesCount = article.Likes.Count(),
                        VideoId = video.VideoId,
                        DurationMinutes = String.Format("{0:00}", (int)((video.Duration ?? 0) / 60)),
                        DurationSeconds = String.Format("{0:00}", (int)((video.Duration ?? 0) % 60))
                    };
                    if (video.VideoImages.Count() > 0)
                        video_item.ImageURL = video.VideoImages.First().Image.URL;

                    final_result.Add(video_item);
                }

                return FormatImageURL(final_result, image_width, image_height);
            }

            public List<ListVideoViewModel> GetAuthorVideos(int count,int author_id, int image_width, int image_height, List<int> except = null)
            {
                IQueryable<Video> db_videos = DAManager.VideosRepository.Get(a => a.AuthorId == author_id && a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => Guid.NewGuid()), "VideoImages.Image").Take(count);
                if (except != null)
                {
                    foreach (int ex in except)
                    {
                        db_videos = db_videos.Where(a => a.VideoId != ex);
                    }
                }

                List<ListVideoViewModel> final_result = new List<ListVideoViewModel>();

                foreach (Video video in db_videos.ToList())
                {
                    ListVideoViewModel video_item = new ListVideoViewModel()
                    {
                        Title = video.Title,
                        //CommentsCount = article.Comments.Count(),
                        URL = video.URL,
                        // LikesCount = article.Likes.Count(),
                        VideoId = video.VideoId,
                        DurationMinutes = String.Format("{0:00}", (int)((video.Duration ?? 0) / 60)),
                        DurationSeconds = String.Format("{0:00}", (int)((video.Duration ?? 0) % 60))
                    };
                    if (video.VideoImages.Count() > 0)
                        video_item.ImageURL = video.VideoImages.First().Image.URL;

                    final_result.Add(video_item);
                }

                return FormatImageURL(final_result, image_width, image_height);
            }
        
            public List<ListVideoViewModel> FormatImageURL(List<ListVideoViewModel> video_list, int image_width, int image_height)
            {
                foreach (ListVideoViewModel video in video_list)
                {
                    if (video.ImageURL != null && video.ImageURL != "")
                        video.ImageURL = ImageService.GenerateImageFullPath(video.ImageURL, image_width.ToString(), image_height.ToString());
                }
                return video_list;
            }

            public List<ListVideoViewModel> GetRandomVideos(int count, int image_width, int image_height, List<int> except = null)
            {
                IQueryable<Video> db_videos = DAManager.VideosRepository.Get(a => a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, a => a.OrderByDescending(ar => Guid.NewGuid()), "VideoImages.Image").Take(count);
                if (except != null)
                {
                    foreach (int ex in except)
                    {
                        db_videos = db_videos.Where(a => a.VideoId != ex);
                    }
                }

                List<ListVideoViewModel> final_result = new List<ListVideoViewModel>();

                foreach (Video video in db_videos.ToList())
                {
                    ListVideoViewModel video_item = new ListVideoViewModel()
                    {
                        Title = video.Title,
                        //CommentsCount = article.Comments.Count(),
                        URL = video.URL,
                        // LikesCount = article.Likes.Count(),
                        VideoId = video.VideoId,
                        DurationMinutes = String.Format("{0:00}", (int)((video.Duration ?? 0) / 60)),
                        DurationSeconds = String.Format("{0:00}", (int)((video.Duration ?? 0) % 60))
                    };
                    if (video.VideoImages.Count() > 0)
                        video_item.ImageURL = video.VideoImages.First().Image.URL;

                    final_result.Add(video_item);
                }

                return FormatImageURL(final_result, image_width, image_height);
            }
        #endregion
    }
}
