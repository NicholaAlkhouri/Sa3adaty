using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;
using System.Data.Entity;

namespace Sa3adaty.Core.Services
{
    public class SiteMapService
    {
          #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public SiteMapService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods

            public void GenerateSiteMap()
            {
                XmlTextWriter writer = new XmlTextWriter(HttpContext.Current.Server.MapPath("~/sitemap_temp.xml"), System.Text.Encoding.UTF8);

                try
                {
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    writer.WriteAttributeString("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");
                    writer.WriteAttributeString("xmlns", "video", null, "http://www.google.com/schemas/sitemap-video/1.1");
                    //Static Pages
                    WriteStaticPages(writer);
                   

                    //Tags
                    foreach (Tag c in GetTags())
                    {
                        WriteTagURL(c, writer);
                    }

                    //foreach time get 100 article and write them to the file
                    int batch = 0;
                    IEnumerable<Article> articles = GetArticlesBatch(100, batch++);
                    while (articles.Count() > 0)
                    {
                        foreach (Article ar in articles)
                        {
                            WriteArticleURL(ar,writer );
                        }

                        articles = GetArticlesBatch(100, batch++);
                    }

                    //foreach time get 100 Video and write them to the file
                    batch = 0;
                    IEnumerable<Video> videos = GetVideosBatch(100, batch++);
                    while (videos.Count() > 0)
                    {
                        foreach (Video ar in videos)
                        {
                            WriteVideoURL(ar, writer);
                        }

                        videos = GetVideosBatch(100, batch++);
                    }

                    writer.WriteEndElement();
                    writer.Close();

                    File.Copy(HttpContext.Current.Server.MapPath("~/sitemap_temp.xml"), HttpContext.Current.Server.MapPath("~/sitemap.xml"), true);
                    File.Delete(HttpContext.Current.Server.MapPath("~/sitemap_temp.xml"));
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                }
                finally
                {
                    writer.Close();
                }
            }

            public void WriteStaticPages(XmlTextWriter writer)
            {
                writer.WriteStartElement("url");
                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com");
                writer.WriteEndElement();
                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("url");
                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com/عن-سعادتي");
                writer.WriteEndElement();
                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("url");
                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com/شروط-الاستخدام");
                writer.WriteEndElement();
                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("url");
                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com/اعلن-معنا");
                writer.WriteEndElement();
                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            
            public void WriteArticleURL(Article article, XmlTextWriter writer)
            {
                writer.WriteStartElement("url");

                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com/مقالات/" + article.URL);
                writer.WriteEndElement();


                //image
                if(article.ArticleImages.Count > 0)
                {
                    writer.WriteStartElement("image:image");
                    writer.WriteStartElement("image:loc");
                    writer.WriteString("http://sa3adaty.com" + ImageService.GenerateImageFullPath(article.ArticleImages.First().Image.URL, ArticleService.ArticleThumbWidth.ToString(), ArticleService.ArticleThumbHeight.ToString()));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }


                //change frequency
                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            public void WriteVideoURL(Video video, XmlTextWriter writer)
            {
                writer.WriteStartElement("url");

                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com/فيديو/" + video.URL);
                writer.WriteEndElement();

                writer.WriteStartElement("video:video");

                writer.WriteStartElement("video:title");
                writer.WriteString(video.MetaTitle);
                writer.WriteEndElement(); //Title End

                //Thumbnail
                if (video.VideoImages.Count > 0)
                {
                    writer.WriteStartElement("video:thumbnail_loc");
                    writer.WriteString("http://sa3adaty.com" + ImageService.GenerateImageFullPath(video.VideoImages.First().Image.URL, VideoService.VideoThumbWidth.ToString(), VideoService.VideoThumbHeight.ToString()));
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("video:description");
                writer.WriteString(video.MetaDescription);
                writer.WriteEndElement(); //Description End

                writer.WriteStartElement("video:player_loc");
                writer.WriteAttributeString("allow_embed", "yes");
                writer.WriteString("http://www.youtube.com/embed/"+video.YoutubeId);
                writer.WriteEndElement(); //Player Location End

                writer.WriteStartElement("video:duration");
                writer.WriteString(video.Duration.ToString());
                writer.WriteEndElement(); //Duration End

                writer.WriteStartElement("video:view_count");
                writer.WriteString(video.NumberOfViews.ToString());
                writer.WriteEndElement(); //View End

                writer.WriteStartElement("video:publication_date");
                writer.WriteString(video.PublishDate.ToString("yyyy-MM-ddThh:mm:sszzz"));
                writer.WriteEndElement(); //publication End

                writer.WriteStartElement("video:uploader");
                writer.WriteString("موقع سعادتي");
                writer.WriteEndElement(); //uploader End

                writer.WriteEndElement(); //Video ENd


                writer.WriteEndElement();
            }

            public void WriteCategoryURL(Category category, XmlTextWriter writer)
            {
                writer.WriteStartElement("url");

                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com/"+category.URL);
                writer.WriteEndElement();

                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();
                
                writer.WriteEndElement();
            }

            public void WriteTagURL(Tag tag, XmlTextWriter writer)
            {
                writer.WriteStartElement("url");

                writer.WriteStartElement("loc");
                writer.WriteString("http://sa3adaty.com/" + tag.TagName );
                writer.WriteEndElement();

                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            public IEnumerable<Article> GetArticlesBatch(int batch_size, int batch_number)
            {
                IQueryable<Article> articles = DAManager.ArticlesRepository.Get(a => a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, a => a.OrderBy(ar => ar.ArticleId), "ArticleImages.Image").Skip(batch_size * batch_number).Take(batch_size);

                return articles.AsEnumerable();
            }

            public IEnumerable<Video> GetVideosBatch(int batch_size, int batch_number)
            {
                IQueryable<Video> videos = DAManager.VideosRepository.Get(a => a.IsPublished == true && DbFunctions.CreateDateTime(a.PublishDate.Year, a.PublishDate.Month, a.PublishDate.Day, a.PublishDate.Hour, a.PublishDate.Minute, a.PublishDate.Second) <= DateTime.Now, a => a.OrderBy(ar => ar.VideoId), "VideoImages.Image").Skip(batch_size * batch_number).Take(batch_size);

                return videos.AsEnumerable();
            }

            public IEnumerable<Category> GetCategories()
            {
                IQueryable<Category> categories = DAManager.CategoriesRepository.Get(a => a.IsPublished == true);

                return categories.AsEnumerable();
            }
            public IEnumerable<Tag> GetTags()
            {
                IQueryable<Tag> tags = DAManager.TagsRepository.Get();

                return tags.AsEnumerable();
            }
        #endregion 
    }
}
