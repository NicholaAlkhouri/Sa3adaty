using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Authors;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class AuthorService
    {
        #region consts
 
            static public int AuthorThumbWidth = 50;
            static public int AuthorThumbHeight = 50;

            static public int AuthorThumbWidth1 = 200;
            static public int AuthorThumbHeight1 = 200;
        #endregion

        #region Privates
        private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public AuthorService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        public List<AuthorViewModel> GetAuthors(out int total_count, int page = 0, int page_size = 10, string search_key = "", string order_by = "Name", string order_dir = "asc")
            {
                IQueryable<Author> result;
                
                result = DAManager.AuthorsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Name) : a.OrderByDescending(c => c.Name)));


                if (search_key != "")
                    result = result.Where(cat => cat.Name.Contains(search_key));

                total_count = result.Count();
                result = result.Skip(page).Take(page_size);
                List<Author> final_res = result.ToList();
                List<AuthorViewModel> final_result = new List<AuthorViewModel>();
                foreach (Author auth in final_res)
                    final_result.Add(new AuthorViewModel() {AuthorId= auth.AuthorId, Name= auth.Name  });

                return final_result;
            }

        public int AddNewAuthor(AuthorViewModel author, IEnumerable<HttpPostedFileBase> Images = null)
            {
                Author DBAuthor = new Author() {Name = author.Name , AddDate = DateTime.Now,Description = author.Description,Email = author.Email,FacebookPage = author.FacebookPage,Title = author.Title ,URL = author.URL,IsProfileEnabled = author.IsProfileEnabled ,MetaDescription = author.MetaDescription , MetaTitle= author.MetaTitle,FacebookId = author.FacebookID};

                DAManager.AuthorsRepository.Insert(DBAuthor);

                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }

                if (DBAuthor.AuthorId > 0 && Images != null && Images.Count() > 0)
                {
                    AddAuthorImages(DBAuthor.AuthorId, Images);
                }

                return DBAuthor.AuthorId;
            }

        public bool AddAuthorImages(int author_id, IEnumerable<HttpPostedFileBase> Images, string english_author_name = "", string caption = "", string description = "")
        {
            Author author = DAManager.AuthorsRepository.Get(auth => auth.AuthorId == author_id ).FirstOrDefault();

            if (author == null)
                return false;

            if (Images != null)
                foreach (HttpPostedFileBase image in Images)
                {
                    if (image != null)
                    {
                        Image img = new Image() { AddDate = DateTime.Now, Caption = caption, Description = description, URL = "" };


                        DAManager.ImagesRepository.Insert(img);
                        author.AuthorImages.Add(new AuthorImage() { Author = author, Image = img });
                        try
                        {
                            DAManager.Save();

                            //Original
                            string file_name = "";
                            if (english_author_name == "")
                            {
                                file_name = (img.ImageId.ToString() + "-" + image.FileName).Replace(" ", "-");
                            }
                            else
                            {
                                file_name = (img.ImageId.ToString() + "-" + english_author_name + Path.GetExtension(image.FileName)).Replace(" ", "-");
                            }


                            System.Drawing.Image web_image = System.Drawing.Image.FromStream(image.InputStream);

                            //save Original Image
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                            //Save Thumnails
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, AuthorThumbWidth, AuthorThumbHeight);
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, AuthorThumbWidth1, AuthorThumbHeight1);

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

        public bool UpdateAuthorImage(int image_id, HttpPostedFileBase Image, string english_author_title = "", string caption = "", string description = "")
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
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), AuthorThumbWidth.ToString(), AuthorThumbHeight.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), AuthorThumbWidth1.ToString(), AuthorThumbHeight1.ToString());

                    //Original
                    string file_name = "";
                    if (english_author_title == "")
                    {
                        file_name = (old_image.ImageId.ToString() + "-" + Image.FileName).Replace(" ","-");
                    }
                    else
                    {
                        file_name = (old_image.ImageId.ToString() + "-" + english_author_title + Path.GetExtension(Image.FileName)).Replace(" ", "-");
                    }


                    System.Drawing.Image web_image = System.Drawing.Image.FromStream(Image.InputStream);

                    //save Original Image
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                    //Save Thumnails 200x200
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, AuthorThumbWidth, AuthorThumbHeight);
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, AuthorThumbWidth1, AuthorThumbHeight1);

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

        public IEnumerable<ImageViewModel> GetAuthorImages(int author_id)
        {
            Author author = DAManager.AuthorsRepository.Get(auth => auth.AuthorId == author_id , null, "AuthorImages.Image").FirstOrDefault();
            if (author == null)
                return null;

            return author.AuthorImages.Select(AI => new ImageViewModel() { Caption = AI.Image.Caption, Description = AI.Image.Description, ImageId = AI.ImageId, URL = Path.GetDirectoryName(AI.Image.URL) + "/" + ImageService.GenerateImageFileName(AI.Image.URL, AuthorThumbWidth1.ToString(), AuthorThumbHeight1.ToString()) }).ToList();

        }

        public ImageViewModel GetAuthorImage(int image_id)
        {
            Image _image = DAManager.ImagesRepository.Get(img => img.ImageId == image_id).FirstOrDefault();
            if (_image == null)
                return null;

            return new ImageViewModel() { Caption = _image.Caption, Description = _image.Description, ImageId = _image.ImageId, URL = _image.URL };

        }

        public bool DeleteAuthorimage(int image_id)
        {
            Image img = DAManager.ImagesRepository.Get(im => im.ImageId == image_id, null, "AuthorImages").FirstOrDefault();
            if (img == null)
                return false;

            foreach (AuthorImage  AI in img.AuthorImages.ToList())
            {
                DAManager.AuthorImagesRepository.Delete(AI);
            }

            DAManager.ImagesRepository.Delete(img);

            try
            {
                string FileName = Path.GetFileName(img.URL);
                string Directory = Path.GetDirectoryName(img.URL);

                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL));
                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), AuthorThumbWidth.ToString(), AuthorThumbHeight.ToString());
                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), AuthorThumbWidth1.ToString(), AuthorThumbHeight1.ToString());

                DAManager.Save();
                return true;
            }
            catch (Exception ex)
            {
                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                return false;
            }
        }

        public AuthorViewModel GetAuthorById(int author_id)
        {
            var author = DAManager.AuthorsRepository.Get(q => q.AuthorId == author_id).FirstOrDefault();
            if (author != null)
            {
                AuthorViewModel author_viewmodel = new AuthorViewModel() { AuthorId =author.AuthorId,Name= author.Name,Description = author.Description,Email = author.Email,FacebookPage = author.FacebookPage,Title = author.Title,URL=author.URL, IsProfileEnabled = author.IsProfileEnabled??false ,MetaDescription = author.MetaDescription, MetaTitle = author.MetaTitle,FacebookId = author.FacbookId  };

                return author_viewmodel;
            }
            else
                return null;
        }

        public int UpdateAuthor(AuthorViewModel author)
        {
            Author DBAuthor = DAManager.AuthorsRepository.Get(a => a.AuthorId == author.AuthorId ).FirstOrDefault();

            if (DBAuthor != null)
            {
                DBAuthor.Name = author.Name;
                DBAuthor.Description = author.Description;
                DBAuthor.Email = author.Email;
                DBAuthor.FacebookPage = author.FacebookPage;
                DBAuthor.Title = author.Title;
                DBAuthor.URL = author.URL;
                DBAuthor.IsProfileEnabled = author.IsProfileEnabled;
                DBAuthor.MetaTitle = author.MetaTitle;
                DBAuthor.MetaDescription = author.MetaDescription;
                DBAuthor.FacebookId = author.FacebookId;
            }
            else
                return -1;
            try
            {
                DAManager.Save();
                return DBAuthor.AuthorId;
            }
            catch (Exception ex)
            {
                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                return -1;
            }
        }

        public bool DeleteAuthor(int author_id)
        {
            DAManager.AuthorsRepository.Delete(author_id);

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

    }
}
