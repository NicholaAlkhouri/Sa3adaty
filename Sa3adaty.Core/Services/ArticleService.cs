using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.Core.ViewModels.Admin.Articles;
using System.Web;
using System.IO;
using Sa3adaty.Core.ViewModels.Admin;
using System.Web.Helpers;

namespace Sa3adaty.Core.Services
{
   public  class ArticleService
   {
        #region consts
       static public   int CategoryThumbWidth = 200;
       static public   int CategoryThumbHeight = 200;

       //home page slider image + main article page 
       static public  int ArticleThumbWidth = 684;
       static public  int ArticleThumbHeight = 353;

       ////article page main image
       //static public  int ArticleThumbWidth2 = 700;
       //static public  int ArticleThumbHeight2 = 380;

       //Home Left most recent
       static public  int ArticleThumbWidth3 = 280;
       static public  int ArticleThumbHeight3 = 134;

       //article page recomended articles bottom
       static public  int ArticleThumbWidth4 = 188;
       static public  int ArticleThumbHeight4 = 112;

       ////article page most recent left
       //static public  int ArticleThumbWidth5 = 115;
       //static public  int ArticleThumbHeight5 = 115;

       //article page related articles top
       static public int ArticleThumbWidth6 = 88;
       static public int ArticleThumbHeight6 = 80;

       //category Article Listing
       static public int ArticleThumbWidth7 = 214;
       static public int ArticleThumbHeight7 = 185;

       //Home Under Slider
       static public int ArticleThumbWidth8 = 332;
       static public int ArticleThumbHeight8 = 221;
       #endregion

        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public ArticleService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Categories
      public List<CategoryViewModel> GetCategories(int page = 0, int page_size = 10,string search_key = "",string order_by = "Name",string order_dir = "asc")
      {
          IQueryable<Category> result;
          if(order_by=="CategoryId")
              result = DAManager.CategoriesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.CategoryId) : a.OrderByDescending(c => c.CategoryId)));
          else
              result = DAManager.CategoriesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Name) : a.OrderByDescending(c => c.Name)));

          if (search_key != "")
              result = result.Where(cat => cat.Name.Contains(search_key));

          result = result.Skip(page).Take(page_size);
          return result.Select(cat => new CategoryViewModel() { CategoryId = cat.CategoryId, Name = cat.Name }).ToList();
      }

      public int  GetOnlineCategoriesCount()
      {
          int result = DAManager.CategoriesRepository.Get().Count();
          return result;
      }

       public SelectList GetSelectListCategories(int? selected_value)
       {
           return new SelectList(DAManager.CategoriesRepository.Get().AsEnumerable(), "CategoryId", "Name", selected_value);
       }

       public int AddNewCategroy(CategoryViewModel category, IEnumerable<HttpPostedFileBase> Images = null)
       {
           Category cat = new Category() { Name = category.Name ,AddDate = DateTime.Now,MetaDescription=category.MetaDescription,MetaTitle=category.MetaTitle, IsPublished = category.IsPublished  ,URL = category.URL  };
           DAManager.CategoriesRepository.Insert(cat);
           try
           {
               DAManager.Save();
               
           }
           catch (Exception ex)
           {
               logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
               return -1;
           }

           if(cat.CategoryId>0 && Images != null && Images.Count()> 0)
           {
               AddCategoryImages(cat.CategoryId, Images );
           }
           return cat.CategoryId;
       }

       public int UpdateCategory(CategoryViewModel category)
       {
           Category old_cat = DAManager.CategoriesRepository.Get(cat => cat.CategoryId == category.CategoryId).FirstOrDefault();

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

       public CategoryViewModel GetCategoryById(int category_id)
       {
           var category = DAManager.CategoriesRepository.Get(cat=> cat.CategoryId == category_id).FirstOrDefault();
           if (category != null)
               return new CategoryViewModel() {CategoryId = category.CategoryId, Name = category.Name,IsPublished= category.IsPublished, MetaDescription = category.MetaDescription, MetaTitle = category.MetaTitle, URL=category.URL   };
           else
               return null;
       }

       public bool AddCategoryImages(int category_id, IEnumerable<HttpPostedFileBase> Images,string category_title ="", string caption = "", string description = "")
       {
           Category category = DAManager.CategoriesRepository.Get(cat => cat.CategoryId == category_id).FirstOrDefault();
           if (category == null)
               return false;
           if(Images != null )
               foreach (HttpPostedFileBase image in Images)
               {
                   if (image != null)
                   {
                       Image img = new Image() { AddDate = DateTime.Now, Caption = caption, Description = description, URL = "" };

                       DAManager.ImagesRepository.Insert(img);
                       category.CategoryImages.Add(new CategoryImage() { Category = category, Image = img });
                       try
                       {
                           //Save Image in the data base to get its id.
                           DAManager.Save();

                           //set the image file name
                           string file_name = "";
                           if (category_title == "")
                           {
                               file_name = (img.ImageId.ToString() + "-" + image.FileName).Replace(" ", "-");
                           }
                           else
                           {
                               file_name = (img.ImageId.ToString() + "-" + category_title + Path.GetExtension(image.FileName)).Replace(" ", "-");
                           }
                    

                           System.Drawing.Image web_image = System.Drawing.Image.FromStream(image.InputStream);

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
               }
           return true;
       }

       public bool UpdateCategoryImage(int image_id, HttpPostedFileBase Image,string category_title ="", string caption = "", string description = "")
       {
           ImageService image_manager = new ImageService();
           Image old_image = DAManager.ImagesRepository.Get(im=> im.ImageId == image_id ).FirstOrDefault();
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

       public bool DeleteCategoryimage(int image_id)
       {
           Image img = DAManager.ImagesRepository.Get(im => im.ImageId == image_id, null, "CategoryImages").FirstOrDefault();
           if (img == null)
               return false;

           foreach (CategoryImage CI in img.CategoryImages.ToList())
           {
               DAManager.CategoryImagesRepository.Delete(CI);
           }

           DAManager.ImagesRepository.Delete(img);
            
           try
           {
               string FileName = Path.GetFileName(img.URL);
               string Directory = Path.GetDirectoryName(img.URL);

               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL));
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL),CategoryThumbWidth.ToString(),CategoryThumbHeight.ToString());
               DAManager.Save();
               return true;
           }
           catch (Exception ex)
           {
               logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
               return false;
           }
       }

       public IEnumerable<ImageViewModel> GetCategoryImages(int category_id)
       {
           Category category = DAManager.CategoriesRepository.Get(cat => cat.CategoryId == category_id,null,"CategoryImages.Image").FirstOrDefault();
           if (category == null)
               return null;

           return category.CategoryImages.Select(CI => new ImageViewModel() { Caption = CI.Image.Caption, Description = CI.Image.Description, ImageId = CI.ImageId, URL =  Path.GetDirectoryName(CI.Image.URL) + "/" + ImageService.GenerateImageFileName(CI.Image.URL,CategoryThumbWidth.ToString(),CategoryThumbHeight.ToString()) }).ToList();
               
       }

       public ImageViewModel GetCategoryImage(int image_id)
       {
           Image _image = DAManager.ImagesRepository.Get(img => img.ImageId == image_id).FirstOrDefault();
           if (_image == null)
               return null;

           return new ImageViewModel() { Caption = _image.Caption, Description = _image.Description, ImageId = _image.ImageId ,URL = _image.URL };

       }

       public bool DeleteCategory(int category_id)
       {
           DAManager.CategoriesRepository.Delete(category_id);
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

       public bool DeleteArticle(int article_id)
       {
           DAManager.ArticlesRepository.Delete(article_id);
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
       #endregion

        #region Articles

       public List<ListItemArticleViewModel> GetArticles(out int total_count, int CategoryId, int page = 0, int page_size = 10, string search_key = "", string order_by = "PublishDate", string order_dir = "desc")
       {
           IQueryable<Article> result;
           if (order_by == "Title")
               result = DAManager.ArticlesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Title) : a.OrderByDescending(c => c.Title)), "ArticleCategories");
           else if (order_by == "IsPublished")
               result = DAManager.ArticlesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.IsPublished) : a.OrderByDescending(c => c.IsPublished)), "ArticleCategories");
           else
               result = DAManager.ArticlesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.PublishDate) : a.OrderByDescending(c => c.PublishDate)),"ArticleCategories");

           if(CategoryId >0 )
               result = result.Where(art=> art.ArticleCategories.Where(cat=> cat.CategoryId == CategoryId ).Count() > 0);

           if (search_key != "")
               result = result.Where(cat => cat.Title.Contains(search_key));

           total_count = result.Count();
           result = result.Skip(page).Take(page_size);
           List<Article> final_res = result.ToList();
           List<ListItemArticleViewModel> final_result = new List<ListItemArticleViewModel>();
           foreach (Article ar in final_res)
               final_result.Add(new ListItemArticleViewModel() { ArticleId = ar.ArticleId, Title = ar.Title, PublishDate = ar.PublishDate.ToString(),IsPublished = ar.IsPublished  });

           return final_result;
       }

       public int AddNewArticle(ArticleViewModel article, IEnumerable<HttpPostedFileBase> Images = null)
       {
           Article art = new Article() { Title = article.Title, Description = ClearArticleDescription(article.Description), PublishDate = article.PublishDate, MetaDescription = article.MetaDescription, MetaTitle = article.MetaTitle, IsPublished = article.IsPublished, AuthorId = article.AuthorId, URL = article.URL };
           art.AddDate = DateTime.Now;
           DAManager.ArticlesRepository.Insert(art);

           //insert tags
           if(article.Tags != null && article.Tags.Count > 0)
            {
               art.ArticleTags = new List<ArticleTag>();
               foreach (string tag in article.Tags)
                   art.ArticleTags.Add(new ArticleTag() { Article = art, Tag = tag });
           }
           try
           {
               DAManager.Save();

               //Add the category relation
               art.ArticleCategories.Add(new ArticleCategory (){ArticleId = art.ArticleId, CategoryId = article.CategoryId });

               DAManager.Save();
             
           }
           catch (Exception ex)
           {
               logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
               return -1;
           }

           if (art.ArticleId > 0 && Images != null && Images.Count() > 0)
           {
               AddArticleImages(art.ArticleId, Images,art.URL);
           }

           return art.ArticleId;
       }
       public string ClearArticleDescription(string descripion)
       {
           return descripion.Replace("[endif]", "").Replace("[if !supportLineBreakNewLine]", "");
       }

       public int UpdateArticle(ArticleViewModel  article)
       {
           Article old_art = DAManager.ArticlesRepository.Get(art => art.ArticleId == article.ArticleId, null, "ArticleCategories,ArticleTags").FirstOrDefault();

           if (old_art != null)
           {
               old_art.Title = article.Title;
               old_art.MetaTitle = article.MetaTitle;
               old_art.MetaDescription = article.MetaDescription;
               old_art.IsPublished = article.IsPublished;
               old_art.Description = ClearArticleDescription(article.Description);
               old_art.AuthorId = article.AuthorId;
               old_art.PublishDate = article.PublishDate;
               old_art.URL = article.URL;

               //if (article.Tags == null)
               //    article.Tags = new List<string>();
               //remove delted tags
               foreach (ArticleTag AT in old_art.ArticleTags.ToList())
               {
                   if (article.Tags != null && article.Tags.Contains(AT.Tag))
                   {
                      article.Tags.Remove(AT.Tag);
                   }
                   else
                   {
                        old_art.ArticleTags.Remove(AT);
                   }
               }

               //Add new tags
               if (article.Tags != null)
               {
                   foreach (string new_tag in article.Tags)
                   {
                       old_art.ArticleTags.Add(new ArticleTag() { Article = old_art, Tag = new_tag });
                   }
               }

               //Delete old categories relations
               foreach (ArticleCategory AC in old_art.ArticleCategories.ToList())
               {
                   DAManager.ArticleCategoriesRepository.Delete(AC);
               }
               
               //Add the category relation
               DAManager.ArticleCategoriesRepository.Insert(new ArticleCategory() { ArticleId = article.ArticleId, CategoryId = article.CategoryId });
           }
           else
               return -1;
           try
           {
               DAManager.Save();
               return old_art.ArticleId;
           }
           catch (Exception ex)
           {
               logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
               return -1;
           }
       }

       public ArticleViewModel GetArticleById(int article_id)
       {
           var article = DAManager.ArticlesRepository.Get(art => art.ArticleId  == article_id ,null,"ArticleCategories,ArticleTags").FirstOrDefault();
           if (article != null)
           {
               ArticleViewModel article_viewmodel = new ArticleViewModel() { ArticleId = article.ArticleId, Title = article.Title, Description = article.Description, IsPublished = article.IsPublished, MetaDescription = article.MetaDescription, MetaTitle = article.MetaTitle, AuthorId = article.AuthorId,PublishDate = article.PublishDate,CountOfViews = article.NumberOfViews , URL = article.URL  };
               if (article.ArticleCategories.Count > 0)
               {
                   article_viewmodel.CategoryId = article.ArticleCategories.FirstOrDefault().CategoryId;
               }

               article_viewmodel.Tags = new List<string>();
               if (article.ArticleTags.Count > 0)
                   foreach (ArticleTag AT in article.ArticleTags)
                       article_viewmodel.Tags.Add(AT.Tag);

               return article_viewmodel;
           }
           else
               return null;
       }

       public bool AddArticleImages(int article_id, IEnumerable<HttpPostedFileBase> Images,string article_title ="", string caption = "", string description = "")
       {
           Article article = DAManager.ArticlesRepository.Get(art => art.ArticleId == article_id).FirstOrDefault();
           
           if (article == null)
               return false;
          
           if (Images != null)
               foreach (HttpPostedFileBase image in Images)
               {
                   if (image != null)
                   {
                       Image img = new Image() { AddDate = DateTime.Now, Caption = caption, Description = description, URL = "" };


                       DAManager.ImagesRepository.Insert(img);
                       article.ArticleImages.Add(new ArticleImage() { Article = article, Image = img });
                       try
                       {
                           DAManager.Save();

                           //Original
                           string file_name = "";
                           if (article_title == "")
                           {
                               file_name = (img.ImageId.ToString() + "-" + image.FileName).Replace(" ","-");
                           }
                           else
                           {
                               file_name = (img.ImageId.ToString() + "-" + article_title + Path.GetExtension(image.FileName)).Replace(" ", "-");
                           }
                          

                           System.Drawing.Image web_image = System.Drawing.Image.FromStream(image.InputStream);

                           //save Original Image
                           ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                           //Save Thumnails
                           ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth, ArticleThumbHeight);
                           //ImageService.SaveImage(web_image.Clone(), file_name, ArticleThumbWidth2, ArticleThumbHeight2);
                           ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth3, ArticleThumbHeight3);
                           ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth4, ArticleThumbHeight4);
                           //ImageService.SaveImage(web_image.Clone(), file_name, ArticleThumbWidth5, ArticleThumbHeight5);
                           ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth6, ArticleThumbHeight6);
                           ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth7, ArticleThumbHeight7);
                           ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth8, ArticleThumbHeight8);

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

       public bool UpdateArticleImage(int image_id, HttpPostedFileBase Image,string article_title ="", string caption = "", string description = "")
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
                   ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth.ToString(), ArticleThumbHeight.ToString());
                   //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth2.ToString(), ArticleThumbHeight2.ToString());
                   ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth3.ToString(), ArticleThumbHeight3.ToString());
                   ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth4.ToString(), ArticleThumbHeight4.ToString());
                   //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth5.ToString(), ArticleThumbHeight5.ToString());
                   ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth6.ToString(), ArticleThumbHeight6.ToString());
                   ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth7.ToString(), ArticleThumbHeight7.ToString());
                   ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), ArticleThumbWidth8.ToString(), ArticleThumbHeight8.ToString());

                   //Original
                   string file_name = "";
                   if (article_title == "")
                   {
                       file_name = (old_image.ImageId.ToString() + "-" + Image.FileName).Replace(" ", "-");
                   }
                   else
                   {
                       file_name = (old_image.ImageId.ToString() + "-" + article_title + Path.GetExtension(Image.FileName)).Replace(" ", "-");
                   }
                  

                   System.Drawing.Image web_image = System.Drawing.Image.FromStream(Image.InputStream);

                   //save Original Image
                   ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                   //Save Thumnails 200x200
                   ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth, ArticleThumbHeight);
                   //ImageService.SaveImage(web_image.Clone(), file_name, ArticleThumbWidth2, ArticleThumbHeight2);
                   ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth3, ArticleThumbHeight3);
                   ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth4, ArticleThumbHeight4);
                   //ImageService.SaveImage(web_image.Clone(), file_name, ArticleThumbWidth5, ArticleThumbHeight5);
                   ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth6, ArticleThumbHeight6);
                   ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth7, ArticleThumbHeight7);
                   ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, ArticleThumbWidth8, ArticleThumbHeight8);

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

       public bool DeleteArticleimage(int image_id)
       {
           Image img = DAManager.ImagesRepository.Get(im => im.ImageId == image_id, null, "ArticleImages").FirstOrDefault();
           if (img == null)
               return false;

           foreach (ArticleImage  AI in img.ArticleImages.ToList())
           {
               DAManager.ArticleImagesRepository.Delete(AI);
           }

           DAManager.ImagesRepository.Delete(img);

           try
           {
               string FileName = Path.GetFileName(img.URL);
               string Directory = Path.GetDirectoryName(img.URL);
               
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL));
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL),ArticleThumbWidth.ToString(),ArticleThumbHeight.ToString());
               //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), ArticleThumbWidth2.ToString(), ArticleThumbHeight2.ToString());
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), ArticleThumbWidth3.ToString(), ArticleThumbHeight3.ToString());
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), ArticleThumbWidth4.ToString(), ArticleThumbHeight4.ToString());
               //ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), ArticleThumbWidth5.ToString(), ArticleThumbHeight5.ToString());
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), ArticleThumbWidth6.ToString(), ArticleThumbHeight6.ToString());
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), ArticleThumbWidth7.ToString(), ArticleThumbHeight7.ToString());
               ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), ArticleThumbWidth8.ToString(), ArticleThumbHeight8.ToString());

               DAManager.Save();
               return true;
           }
           catch (Exception ex)
           {
               logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
               return false;
           }
       }

       public IEnumerable<ImageViewModel> GetArticleImages(int article_id)
       {
           Article article = DAManager.ArticlesRepository.Get(art => art.ArticleId == article_id, null, "ArticleImages.Image").FirstOrDefault();
           if (article == null)
               return null;

           return article.ArticleImages.Select(AI => new ImageViewModel() { Caption = AI.Image.Caption, Description = AI.Image.Description, ImageId = AI.ImageId, URL = Path.GetDirectoryName(AI.Image.URL) + "/" +ImageService.GenerateImageFileName(AI.Image.URL,ArticleThumbWidth.ToString(),ArticleThumbHeight.ToString())}).ToList();

       }

       public ImageViewModel GetArticleImage(int image_id)
       {
           Image _image = DAManager.ImagesRepository.Get(img => img.ImageId == image_id).FirstOrDefault();
           if (_image == null)
               return null;

           return new ImageViewModel() { Caption = _image.Caption, Description = _image.Description, ImageId = _image.ImageId, URL = _image.URL };

       }

       public string GetArticleURL(int article_id)
       {
           Article db_article = DAManager.ArticlesRepository.GetByID(article_id);
           if (db_article == null)
               return "";
           else return db_article.URL;
       }

       public string GetArticleURLByImage(int image_id)
       {
           ArticleImage img = DAManager.ArticleImagesRepository.Get(i => i.ImageId == image_id,null,"Article").FirstOrDefault();
           if (img == null)
               return "";
           else
               return img.Article.URL;
       }

       public bool IsArticleURLExist(string url,int article_id = 0)
       {
           Article article = DAManager.ArticlesRepository.Get(a => a.URL == url.Trim()).FirstOrDefault();

           if (article == null || ( article_id != 0 && article_id == article.ArticleId ))
               return false;

           return true;
       }

       public bool IsCategoryURLExist(string url, int category_id = 0)
       {
           Category category = DAManager.CategoriesRepository.Get(a => a.URL == url.Trim()).FirstOrDefault();

           if (category == null || (category_id != 0 && category_id == category.CategoryId))
               return false;

           return true;
       }
       #endregion

       #region Comments
       public List<ListItemArticleCommentViewModel> GetComments(out int total_count,int article_id, int page = 0, int page_size = 10, string search_key = "", string order_by = "AddDate", string order_dir = "desc")
       {
           IQueryable<Comment> result;
           if (order_by == "UserName")
               result = DAManager.CommentsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.UserProfile.Name) : a.OrderByDescending(c => c.UserProfile.Name)),"Comment,Article,Comment.UserProfile");
           else
               result = DAManager.CommentsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.AddDate) : a.OrderByDescending(c => c.AddDate)), "Article,UserProfile");

           if (article_id > 0)
               result = result.Where(c => c.ArticleId == article_id);
          
           if (search_key != "")
               result = result.Where(cat => cat.Text.Contains(search_key));

           total_count = result.Count();
           result = result.Skip(page).Take(page_size);
           List<Comment> final_res = result.ToList();
           List<ListItemArticleCommentViewModel> final_result = new List<ListItemArticleCommentViewModel>();
           foreach (Comment ar in final_res)
               final_result.Add(new ListItemArticleCommentViewModel() {AddDateString = ar.AddDate.ToString(),CommentId = ar.CommentId,Text = ar.Text,Username=ar.UserProfile.Name,ArticleId = ar.ArticleId  });

           return final_result;
       }
       public bool DeleteComment(int comment_id)
       {
           List<Comment> comments = DAManager.CommentsRepository.Get(c => c.CommentId == comment_id || c.ParentId == comment_id, null).ToList();
           if (comments == null || comments.Count() == 0 )
               return false;

           foreach (Comment C in comments.ToList())
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
       #endregion

       #region
       public SelectList GetSelectListAuthors(int? selected_value)
       {
           return new SelectList(DAManager.AuthorsRepository.Get().AsEnumerable(), "AuthorId", "Name", selected_value);
       }
       #endregion
   }
}
