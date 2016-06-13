using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Articles;
using Sa3adaty.Core.ViewModels.Admin.Videos;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class VideoService
    {
        static public int CategoryThumbWidth = 200;
        static public int CategoryThumbHeight = 200;

        //home page slider image + main video page 
        static public int VideoThumbWidth = 684;
        static public int VideoThumbHeight = 353;

        ////video page main image
        //static public  int videoThumbWidth2 = 700;
        //static public  int videoThumbHeight2 = 380;

        //Home Left most recent
        static public int VideoThumbWidth3 = 280;
        static public int VideoThumbHeight3 = 134;

        //video page recomended videos bottom
        static public int VideoThumbWidth4 = 188;
        static public int VideoThumbHeight4 = 112;


        static public int VideoThumbWidth6 = 88;
        static public int VideoThumbHeight6 = 80;

        //category video Listing
        static public int VideoThumbWidth7 = 214;
        static public int VideoThumbHeight7 = 185;

        //Home Under Slider
        static public int VideoThumbWidth8 = 332;
        static public int VideoThumbHeight8 = 221;


        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public VideoService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Videos
            public bool IsVideoURLExist(string url, int video_id = 0)
            {
                Video video = DAManager.VideosRepository.Get(a => a.URL == url.Trim()).FirstOrDefault();

                if (video == null || (video_id != 0 && video_id == video.VideoId))
                    return false;

                return true;
            }
            public string GetVideoURL(int video_id)
            {
                Video db_video = DAManager.VideosRepository.GetByID(video_id);
                if (db_video == null)
                    return "";
                else return db_video.URL;
            }

            public string GetvideoURLByImage(int image_id)
            {
                VideoImage img = DAManager.VideoImagesRepository.Get(i => i.ImageId == image_id, null, "Video").FirstOrDefault();
                if (img == null)
                    return "";
                else
                    return img.Video.URL;
            }

            public VideoViewModel GetVideoById(int video_id)
            {
                var video = DAManager.VideosRepository.Get(v => v.VideoId == video_id, null, "VideoTags").FirstOrDefault();
                if (video != null)
                {
                    VideoViewModel video_viewmodel = new VideoViewModel() { VideoId = video.VideoId, Title = video.Title, Description = video.Description, IsPublished = video.IsPublished, MetaDescription = video.MetaDescription, MetaTitle = video.MetaTitle, AuthorId = video.AuthorId, PublishDate = video.PublishDate, CountOfViews = video.NumberOfViews, URL = video.URL, YoutubeId = video.YoutubeId };
                    video_viewmodel.DurationMinutes = (int)((video.Duration ?? 0) / 60);
                    video_viewmodel.DurationSeconds = (int)((video.Duration ?? 0) % 60);


                    video_viewmodel.CategoryId = video.CategoryId;
                    
                    video_viewmodel.Tags = new List<string>();
                    if (video.VideoTags.Count > 0)
                        foreach (VideoTag AT in video.VideoTags)
                            video_viewmodel.Tags.Add(AT.Tag);

                    return video_viewmodel;
                }
                else
                    return null;
            }

            public int AddNewVideo(VideoViewModel video, IEnumerable<HttpPostedFileBase> Images = null)
            {
                Video vid = new Video() { Title = video.Title, Description = video.Description, PublishDate = video.PublishDate, MetaDescription = video.MetaDescription, MetaTitle = video.MetaTitle, IsPublished = video.IsPublished, AuthorId = video.AuthorId, URL = video.URL ,YoutubeId = video.YoutubeId,CategoryId= video.CategoryId  };
                vid.AddDate = DateTime.Now;
                vid.Duration = (video.DurationMinutes * 60) + video.DurationSeconds;
                DAManager.VideosRepository.Insert(vid);

                //insert tags
                if (video.Tags != null && video.Tags.Count > 0)
                {
                    vid.VideoTags = new List<VideoTag>();
                    foreach (string tag in video.Tags)
                        vid.VideoTags.Add(new VideoTag() { Video = vid, Tag = tag });
                }
                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }

                if (vid.VideoId > 0 && Images != null && Images.Count() > 0)
                {
                    AddVideoImages(vid.VideoId, Images, vid.URL);
                }

                return vid.VideoId;
            }

            public bool AddVideoImages(int video_id, IEnumerable<HttpPostedFileBase> Images, string video_title = "", string caption = "", string description = "")
            {
                Video video = DAManager.VideosRepository.Get(v => v.VideoId == video_id).FirstOrDefault();

                if (video == null)
                    return false;

                if (Images != null)
                    foreach (HttpPostedFileBase image in Images)
                    {
                        if (image != null)
                        {
                            Image img = new Image() { AddDate = DateTime.Now, Caption = caption, Description = description, URL = "" };


                            DAManager.ImagesRepository.Insert(img);
                            video.VideoImages.Add(new VideoImage() { Video  = video, Image = img });
                            try
                            {
                                DAManager.Save();

                                //Original
                                string file_name = "";
                                if (video_title == "")
                                {
                                    file_name = (img.ImageId.ToString() + "-" + image.FileName).Replace(" ", "-");
                                }
                                else
                                {
                                    file_name = (img.ImageId.ToString() + "-" + video_title + Path.GetExtension(image.FileName)).Replace(" ", "-");
                                }


                                System.Drawing.Image web_image = System.Drawing.Image.FromStream(image.InputStream);

                                //save Original Image
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                                //Save Thumnails
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth, VideoThumbHeight);
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth3, VideoThumbHeight3);
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth4, VideoThumbHeight4);
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth6, VideoThumbHeight6);
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth7, VideoThumbHeight7);
                                ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth8, VideoThumbHeight8);

                                //Update the DB value
                                img.URL = ImageService.GetImagesDirectory() + file_name;
                                DAManager.Save();
                            }
                            catch (Exception ex)
                            {
                                return false;
                            }
                        }
                    }
                return true;
            }

            public bool UpdateVideoImage(int image_id, HttpPostedFileBase Image, string video_title = "", string caption = "", string description = "")
            {

                Image old_image = DAManager.ImagesRepository.Get(im => im.ImageId == image_id).FirstOrDefault();
                if (old_image == null)
                    return false;

                old_image.Caption = caption;
                old_image.Description = description;

                if (Image != null)
                {
                    try
                    {
                        string FileName = Path.GetFileName(old_image.URL);
                        string Directory = Path.GetDirectoryName(old_image.URL);

                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL));
                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth.ToString(), VideoThumbHeight.ToString());
                        //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth2.ToString(), VideoThumbHeight2.ToString());
                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth3.ToString(), VideoThumbHeight3.ToString());
                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth4.ToString(), VideoThumbHeight4.ToString());
                        //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth5.ToString(), VideoThumbHeight5.ToString());
                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth6.ToString(), VideoThumbHeight6.ToString());
                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth7.ToString(), VideoThumbHeight7.ToString());
                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), VideoThumbWidth8.ToString(), VideoThumbHeight8.ToString());

                        //Original
                        string file_name = "";
                        if (video_title == "")
                        {
                            file_name = (old_image.ImageId.ToString() + "-" + Image.FileName).Replace(" ", "-");
                        }
                        else
                        {
                            file_name = (old_image.ImageId.ToString() + "-" + video_title + Path.GetExtension(Image.FileName)).Replace(" ", "-");
                        }


                        System.Drawing.Image web_image = System.Drawing.Image.FromStream(Image.InputStream);

                        //save Original Image
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                        //Save Thumnails 200x200
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth, VideoThumbHeight);
                        //ImageService.SaveImage(web_image.Clone(), file_name, VideoThumbWidth2, VideoThumbHeight2);
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth3, VideoThumbHeight3);
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth4, VideoThumbHeight4);
                        //ImageService.SaveImage(web_image.Clone(), file_name, VideoThumbWidth5, VideoThumbHeight5);
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth6, VideoThumbHeight6);
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth7, VideoThumbHeight7);
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, VideoThumbWidth8, VideoThumbHeight8);

                        //Update the DB value
                        old_image.URL = ImageService.GetImagesDirectory() + file_name;

                    }
                    catch (Exception ex)
                    {
                        logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                        return false;
                    }
                }
                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return false;
                }
                return true;
            }

            public int UpdateVideo(VideoViewModel video)
            {
                Video old_v = DAManager.VideosRepository.Get(v => v.VideoId == video.VideoId, null, "VideoTags").FirstOrDefault();

                if (old_v != null)
                {
                    old_v.Title = video.Title;
                    old_v.MetaTitle = video.MetaTitle;
                    old_v.MetaDescription = video.MetaDescription;
                    old_v.IsPublished = video.IsPublished;
                    old_v.Description = video.Description;
                    old_v.AuthorId = video.AuthorId;
                    old_v.PublishDate = video.PublishDate;
                    old_v.URL = video.URL;
                    old_v.YoutubeId = video.YoutubeId;
                    old_v.CategoryId = video.CategoryId;
                    old_v.Duration = (video.DurationMinutes * 60) + video.DurationSeconds;
                    //if (article.Tags == null)
                    //    article.Tags = new List<string>();
                    //remove delted tags
                    foreach (VideoTag VT in old_v.VideoTags.ToList())
                    {
                        if (video.Tags != null && video.Tags.Contains(VT.Tag))
                        {
                            video.Tags.Remove(VT.Tag);
                        }
                        else
                        {
                            old_v.VideoTags.Remove(VT);
                        }
                    }

                    //Add new tags
                    if (video.Tags != null)
                    {
                        foreach (string new_tag in video.Tags)
                        {
                            old_v.VideoTags.Add(new VideoTag() { Video  = old_v, Tag = new_tag });
                        }
                    }
                }
                else
                    return -1;
                try
                {
                    DAManager.Save();
                    return old_v.VideoId;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }
            }

            public IEnumerable<ImageViewModel> GetVideoImages(int video_id)
            {
                Video video = DAManager.VideosRepository.Get(v => v.VideoId == video_id , null, "VideoImages.Image").FirstOrDefault();
                if (video == null)
                    return null;

                return video.VideoImages.Select(AI => new ImageViewModel() { Caption = AI.Image.Caption, Description = AI.Image.Description, ImageId = AI.ImageId, URL = Path.GetDirectoryName(AI.Image.URL) + "/" + ImageService.GenerateImageFileName(AI.Image.URL, VideoThumbWidth.ToString(), VideoThumbHeight.ToString()) }).ToList();

            }

            public ImageViewModel GetVideoImage(int image_id)
            {
                Image _image = DAManager.ImagesRepository.Get(img => img.ImageId == image_id).FirstOrDefault();
                if (_image == null)
                    return null;

                return new ImageViewModel() { Caption = _image.Caption, Description = _image.Description, ImageId = _image.ImageId, URL = _image.URL };

            }

            public bool DeleteVideoimage(int image_id)
            {
                Image img = DAManager.ImagesRepository.Get(im => im.ImageId == image_id, null, "VideoImages").FirstOrDefault();
                if (img == null)
                    return false;

                foreach (VideoImage AI in img.VideoImages.ToList())
                {
                    DAManager.VideoImagesRepository.Delete(AI);
                }

                DAManager.ImagesRepository.Delete(img);

                try
                {
                    string FileName = Path.GetFileName(img.URL);
                    string Directory = Path.GetDirectoryName(img.URL);

                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL));
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth.ToString(), VideoThumbHeight.ToString());
                    //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth2.ToString(), VideoThumbHeight2.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth3.ToString(), VideoThumbHeight3.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth4.ToString(), VideoThumbHeight4.ToString());
                    //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth5.ToString(), VideoThumbHeight5.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth6.ToString(), VideoThumbHeight6.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth7.ToString(), VideoThumbHeight7.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), VideoThumbWidth8.ToString(), VideoThumbHeight8.ToString());

                    DAManager.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return false;
                }
            }
            public List<ListItemVideoViewModel> GetVideos(out int total_count, int CategoryId, int page = 0, int page_size = 10, string search_key = "", string order_by = "PublishDate", string order_dir = "desc")
            {
                IQueryable<Video> result;
                if (order_by == "Title")
                    result = DAManager.VideosRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Title) : a.OrderByDescending(c => c.Title)));
                else if (order_by == "IsPublished")
                    result = DAManager.VideosRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.IsPublished) : a.OrderByDescending(c => c.IsPublished)));
                else
                    result = DAManager.VideosRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.PublishDate) : a.OrderByDescending(c => c.PublishDate)));


                if (CategoryId > 0)
                    result = result.Where(art => art.CategoryId == CategoryId);

                if (search_key != "")
                    result = result.Where(cat => cat.Title.Contains(search_key));

                total_count = result.Count();
                result = result.Skip(page).Take(page_size);
                List<Video> final_res = result.ToList();
                List<ListItemVideoViewModel> final_result = new List<ListItemVideoViewModel>();
                foreach (Video vi in final_res)
                    final_result.Add(new ListItemVideoViewModel() { VideoId = vi.VideoId, Title = vi.Title, PublishDate = vi.PublishDate.ToString(), IsPublished = vi.IsPublished });

                return final_result;
            }
            public bool DeleteVideo(int video_id)
            {
                DAManager.VideosRepository.Delete(video_id);
                try
                {
                    DAManager.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return false;
                }
            }

            public List<ListItemVideoCommentViewModel> GetComments(out int total_count, int video_id, int page = 0, int page_size = 10, string search_key = "", string order_by = "AddDate", string order_dir = "desc")
            {
                IQueryable<VideoComment> result;
                if (order_by == "UserName")
                    result = DAManager.VideoCommentsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.UserProfile.Name) : a.OrderByDescending(c => c.UserProfile.Name)), "Comment,Video,Comment.UserProfile");
                else
                    result = DAManager.VideoCommentsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.AddDate) : a.OrderByDescending(c => c.AddDate)), "Video,UserProfile");

                if (video_id > 0)
                    result = result.Where(c => c.VideoId == video_id);

                if (search_key != "")
                    result = result.Where(cat => cat.Text.Contains(search_key));

                total_count = result.Count();
                result = result.Skip(page).Take(page_size);
                List<VideoComment> final_res = result.ToList();
                List<ListItemVideoCommentViewModel> final_result = new List<ListItemVideoCommentViewModel>();
                foreach (VideoComment ar in final_res)
                    final_result.Add(new ListItemVideoCommentViewModel() { AddDateString = ar.AddDate.ToString(), CommentId = ar.CommentId, Text = ar.Text, Username = ar.UserProfile.Name, VideoId = ar.VideoId });

                return final_result;
            }
            public bool DeleteComment(int comment_id)
            {
                List<VideoComment> comments = DAManager.VideoCommentsRepository.Get(c => c.CommentId == comment_id || c.ParentId == comment_id, null).ToList();
                if (comments == null || comments.Count() == 0)
                    return false;

                foreach (VideoComment C in comments.ToList())
                {
                    DAManager.CommentsRepository.Delete(C);
                }
                try
                {
                    DAManager.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            public SelectList GetSelectListCategories(int? selected_value)
            {
                return new SelectList(DAManager.VideoCategoriesRepository.Get().AsEnumerable(), "CategoryId", "Name", selected_value);
            }
            public bool IsCategoryURLExist(string url, int category_id = 0)
            {
                VideoCategory category = DAManager.VideoCategoriesRepository.Get(a => a.URL == url.Trim()).FirstOrDefault();

                if (category == null || (category_id != 0 && category_id == category.CategoryId))
                    return false;

                return true;
            }
            public int AddNewCategroy(CategoryViewModel category, HttpPostedFileBase _Image = null)
            {
                VideoCategory cat = new VideoCategory() { Name = category.Name, AddDate = DateTime.Now, MetaDescription = category.MetaDescription, MetaTitle = category.MetaTitle, IsPublished = category.IsPublished, URL = category.URL };
                DAManager.VideoCategoriesRepository.Insert(cat);
                try
                {
                    DAManager.Save();

                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }

                if (cat.CategoryId > 0 && _Image != null)
                {
                    AddCategoryImage(cat.CategoryId, _Image);
                }
                return cat.CategoryId;
            }

            public bool AddCategoryImage(int category_id, HttpPostedFileBase _Image, string category_title = "", string caption = "", string description = "")
            {
                VideoCategory category = DAManager.VideoCategoriesRepository.Get(cat => cat.CategoryId == category_id).FirstOrDefault();
                if (category == null)
                    return false;
                if (category.Image != null)
                    DeleteCategoryimage(category.ImageId.Value);

                if (_Image != null)
                {
                    Image img = new Image() { AddDate = DateTime.Now, Caption = caption, Description = description, URL = "" };

                    DAManager.ImagesRepository.Insert(img);
                    category.Image = img;
                    try
                    {
                        //Save Image in the data base to get its id.
                        DAManager.Save();

                        //set the image file name
                        string file_name = "";
                        if (category_title == "")
                        {
                            file_name = (img.ImageId.ToString() + "-" + _Image.FileName).Replace(" ", "-");
                        }
                        else
                        {
                            file_name = (img.ImageId.ToString() + "-" + category_title + Path.GetExtension(_Image.FileName)).Replace(" ", "-");
                        }


                        System.Drawing.Image web_image = System.Drawing.Image.FromStream(_Image.InputStream);

                        //save Original Image
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                        //Save Thumnails 
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, CategoryThumbWidth, CategoryThumbHeight);

                        //Update the DB value
                        img.URL = ImageService.GetImagesDirectory() + file_name;
                        DAManager.Save();
                    }
                    catch (Exception ex)
                    {
                        logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                        return false;
                    }
                }
                    
                return true;
            }

            public IEnumerable<ImageViewModel> GetCategoryImages(int category_id)
            {
                VideoCategory category = DAManager.VideoCategoriesRepository.Get(cat => cat.CategoryId == category_id, null, "Image").FirstOrDefault();
                if (category == null)
                    return null;

                List<ImageViewModel> result = new List<ImageViewModel>();
                if(category.Image != null)
                    result.Add(new ImageViewModel() { Caption = category.Image.Caption, Description = category.Image.Description, ImageId = category.Image.ImageId, URL = Path.GetDirectoryName(category.Image.URL) + "/" + ImageService.GenerateImageFileName(category.Image.URL, CategoryThumbWidth.ToString(), CategoryThumbHeight.ToString()) });
                return result;

            }
            public ImageViewModel GetCategoryImage(int image_id)
            {
                Image _image = DAManager.ImagesRepository.Get(img => img.ImageId == image_id).FirstOrDefault();
                if (_image == null)
                    return null;

                return new ImageViewModel() { Caption = _image.Caption, Description = _image.Description, ImageId = _image.ImageId, URL = _image.URL };

            }
            public bool DeleteCategoryimage(int image_id)
            {
                Image img = DAManager.ImagesRepository.Get(im => im.ImageId == image_id, null).FirstOrDefault();
                if (img == null)
                    return false;

                foreach (VideoCategory CI in img.VideoCategories )
                {
                    CI.ImageId = null;
                }

                DAManager.ImagesRepository.Delete(img);

                try
                {
                    string FileName = Path.GetFileName(img.URL);
                    string Directory = Path.GetDirectoryName(img.URL);

                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL));
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), CategoryThumbWidth.ToString(), CategoryThumbHeight.ToString());
                    DAManager.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return false;
                }
            }
            public bool UpdateCategoryImage(int image_id, HttpPostedFileBase Image, string category_title = "", string caption = "", string description = "")
            {
                ImageService image_manager = new ImageService();
                Image old_image = DAManager.ImagesRepository.Get(im => im.ImageId == image_id).FirstOrDefault();
                if (old_image == null)
                    return false;

                old_image.Caption = caption;
                old_image.Description = description;

                if (Image != null)
                {
                    try
                    {

                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL));
                        ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), CategoryThumbWidth.ToString(), CategoryThumbHeight.ToString());


                        //Original
                        string file_name = "";
                        if (category_title == "")
                        {
                            file_name = (old_image.ImageId.ToString() + "-" + Image.FileName).Replace(" ", "-");
                        }
                        else
                        {
                            file_name = (old_image.ImageId.ToString() + "-" + category_title + Path.GetExtension(Image.FileName)).Replace(" ", "-");
                        }


                        System.Drawing.Image web_image = System.Drawing.Image.FromStream(Image.InputStream);

                        //save Original Image
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                        //Save Thumnails 200x200
                        ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, CategoryThumbWidth, CategoryThumbHeight);

                        //Update the DB value
                        old_image.URL = ImageService.GetImagesDirectory() + file_name;

                    }
                    catch (Exception ex)
                    {
                        logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                        return false;
                    }
                }
                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return false;
                }
                return true;
            }
            public int UpdateCategory(CategoryViewModel category)
            {
                VideoCategory old_cat = DAManager.VideoCategoriesRepository.Get(cat => cat.CategoryId == category.CategoryId).FirstOrDefault();

                if (old_cat != null)
                {
                    old_cat.Name = category.Name;
                    old_cat.MetaTitle = category.MetaTitle;
                    old_cat.MetaDescription = category.MetaDescription;
                    old_cat.IsPublished = category.IsPublished;
                    old_cat.URL = category.URL;
                }
                else
                    return -1;
                try
                {
                    DAManager.Save();
                    return old_cat.CategoryId;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }
            }
            public bool DeleteCategory(int category_id)
            {
                DAManager.VideoCategoriesRepository.Delete(category_id);
                try
                {
                    DAManager.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return false;
                }
            }
            public CategoryViewModel GetCategoryById(int category_id)
            {
                var category = DAManager.VideoCategoriesRepository.Get(cat => cat.CategoryId == category_id).FirstOrDefault();
                if (category != null)
                    return new CategoryViewModel() { CategoryId = category.CategoryId, Name = category.Name, IsPublished = category.IsPublished, MetaDescription = category.MetaDescription, MetaTitle = category.MetaTitle, URL = category.URL };
                else
                    return null;
            }
            public List<CategoryViewModel> GetCategories(int page = 0, int page_size = 10, string search_key = "", string order_by = "Name", string order_dir = "asc")
            {
                IQueryable<VideoCategory> result;
                if (order_by == "CategoryId")
                    result = DAManager.VideoCategoriesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.CategoryId) : a.OrderByDescending(c => c.CategoryId)));
                else
                    result = DAManager.VideoCategoriesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Name) : a.OrderByDescending(c => c.Name)));

                if (search_key != "")
                    result = result.Where(cat => cat.Name.Contains(search_key));

                result = result.Skip(page).Take(page_size);
                return result.Select(cat => new CategoryViewModel() { CategoryId = cat.CategoryId, Name = cat.Name }).ToList();
            }
            public int GetOnlineCategoriesCount()
            {
                int result = DAManager.VideoCategoriesRepository.Get().Count();
                return result;
            }
        #endregion
    }
}
