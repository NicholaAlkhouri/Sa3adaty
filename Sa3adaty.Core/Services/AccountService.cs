using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sa3adaty.Core.ViewModels.Admin.User;
using System.Web;
using System.IO;
using Sa3adaty.Core.ViewModels.Admin;
using System.Web.Helpers;
using Sa3adaty.Common;
using System.Data.Entity;

namespace Sa3adaty.Core.Services
{
    public class AccountService
    {
        #region const
        static public int UserSmallThumbWidth = 20;
        static public int UserSmallThumbHeight = 20;

        static public  int UserThumbWidth = 44;
        static public int UserThumbHeight = 44;

        static public int UserThumbWidth2 = 88;
        static public int UserThumbHeight2 = 78;

        static public int UserMediumThumbWidth = 85;
        static public int UserMediumThumbHeight = 85;
        #endregion

        #region Privates
        private DataAccessManager DAManager;
        private LogService logService;
        #endregion

        #region Constructor
        public AccountService(DataAccessManager unit_of_work)
        {
            DAManager = unit_of_work;
            logService = new LogService(unit_of_work);
        }
        #endregion

        #region Public Methods
        //Check if a username exist in the database
        public int IsUserExist(string username)
        {
            UserProfile user = DAManager.UserProfilesRepository.Get(a => a.Name == username).FirstOrDefault();
            if (user == null)
                return -1;
            else return user.UserId;
        }

       

        //Check if an email exist in the database
        public int IsEmailExist(string email)
        {
            UserProfile user = DAManager.UserProfilesRepository.Get(a => a.Email == email).FirstOrDefault();
            if (user == null)
                return -1;
            else return user.UserId;
        }

        //Get username by giving his email address
        public string GetUsernameByEmail(string email)
        {
            UserProfile user = DAManager.UserProfilesRepository.Get(u => u.Email == email).FirstOrDefault();
            if (user == null)
                return null;
            else
                return user.Name;
        }

        public void AddUser(string username,string email)
        {
            DAManager.UserProfilesRepository.Insert(new UserProfile { Name = username, Email = email, RegistrationDate = DateTime.Now, IsApproved = true, IsLockedOut = false, LastLoginDate = DateTime.Now, LastPasswordChangedDate = DateTime.Now, LastLockoutDate = DateTime.Now });
            DAManager.Save();
        }

        public int AddNewUser(UserViewModel user, IEnumerable<HttpPostedFileBase> Images = null)
        {
            UserProfile DBuser = new UserProfile() {DateOfBirth = user.DateOfBirth, Email = user.Email,FirstName = user.FirstName,LastName = user.LastName, MiddleName = user.MiddleName,Name = user.Name,RegistrationDate = DateTime.Now };
            
            DAManager.UserProfilesRepository.Insert(DBuser);

            try
            {
                DAManager.Save();

            }
            catch (Exception ex)
            {
                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                return -1;
            }

            if (user.UserId > 0 && Images != null && Images.Count() > 0)
            {
                AddUserImages(user.UserId, Images);
            }

            return user.UserId;
        }

        public IEnumerable<ImageViewModel> GetUserImages(int user_id)
        {
            UserProfile user = DAManager.UserProfilesRepository.Get(usr => usr.UserId == user_id, null, "UserImages.Image").FirstOrDefault();
            if (user == null)
                return null;

            return user.UserImages.Select(AI => new ImageViewModel() { Caption = AI.Image.Caption, Description = AI.Image.Description, ImageId = AI.ImageId, URL = Path.GetDirectoryName(AI.Image.URL) + "/" + ImageService.GenerateImageFileName(AI.Image.URL, UserThumbWidth.ToString(), UserThumbHeight.ToString()) }).ToList();

        }

        public ImageViewModel GetUserImage(int image_id)
        {
            Image _image = DAManager.ImagesRepository.Get(img => img.ImageId == image_id).FirstOrDefault();
            if (_image == null)
                return null;

            return new ImageViewModel() { Caption = _image.Caption, Description = _image.Description, ImageId = _image.ImageId, URL = _image.URL };

        }

        public bool ReplaceUserImages(int user_id, IEnumerable<HttpPostedFileBase> Images, string caption = "", string description = "")
        {
            UserProfile db_user = DAManager.UserProfilesRepository.Get(u => u.UserId == user_id,null,"UserImages.Image").FirstOrDefault();

            if (db_user != null)
            {
                if (db_user.UserImages.Count() > 0)
                {
                    UpdateUserImage(db_user.UserImages.FirstOrDefault().ImageId, Images.FirstOrDefault(),"", caption, description);

                }
                else
                {
                  
                    AddUserImages(user_id, Images,"", caption, description);
                }

                return true;
            }

            return false;
        }

        public string GetUserNameById(int user_id)
        {
            UserProfile user = DAManager.UserProfilesRepository.Get(u => u.UserId == user_id).FirstOrDefault();

            if (user == null)
                return "";

            return user.Name;
        }

        public string GetUserNameByImageId(int image_id)
        {
            Image img = DAManager.ImagesRepository.Get(i => i.ImageId == image_id, null, "UserImages.UserProfile").FirstOrDefault() ;

            if (img == null || img.UserImages.FirstOrDefault() == null)
                return "";

            return img.UserImages.FirstOrDefault().UserProfile.Name;
        }



        public bool AddUserImages(int user_id, IEnumerable<HttpPostedFileBase> Images,string user_name ="", string caption = "", string description = "")
        {
            UserProfile user = DAManager.UserProfilesRepository.Get(us => us.UserId == user_id).FirstOrDefault();

            if (user == null)
                return false;

            if (Images != null)
                foreach (HttpPostedFileBase image in Images)
                {
                    if (image != null)
                    {
                        Image img = new Image() { AddDate = DateTime.Now, Caption = caption, Description = description, URL = "" };


                        DAManager.ImagesRepository.Insert(img);
                        user.UserImages.Add(new UserImage() { UserProfile = user, Image = img });
                        try
                        {
                            DAManager.Save();

                            //Original
                            string file_name = "";
                            if (user_name == "")
                            {
                                file_name = (img.ImageId.ToString() + "-" + image.FileName).Replace(" ", "-");
                            }
                            else
                            {
                                file_name = (img.ImageId.ToString() + "-" + user_name + Path.GetExtension(image.FileName)).Replace(" ", "-");
                            }

                            System.Drawing.Image web_image = System.Drawing.Image.FromStream(image.InputStream);
                           

                            //save Original Image
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                            //Save Thumnails 200x200
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserThumbWidth, UserThumbHeight);
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserMediumThumbWidth, UserMediumThumbHeight);
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserSmallThumbWidth, UserSmallThumbHeight);
                            ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserThumbWidth2, UserThumbHeight2);

                            //Update the DB value
                            img.URL = ImageService.GetImagesDirectory() + file_name;
                            user.Avatar = img.URL;
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

        public bool UpdateUserImage(int image_id, HttpPostedFileBase Image,string user_name ="", string caption = "", string description = "")
        {

            Image old_image = DAManager.ImagesRepository.Get(im => im.ImageId == image_id, null, "UserImages.UserProfile").FirstOrDefault();
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
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), UserThumbWidth.ToString(), UserThumbHeight.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), UserSmallThumbWidth.ToString(), UserSmallThumbHeight.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), UserMediumThumbWidth.ToString(), UserMediumThumbHeight.ToString());
                    ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + old_image.URL), UserThumbWidth2.ToString(), UserThumbHeight2.ToString());

                    //Original
                    string file_name ="";
                    if (user_name == "")
                    {
                        file_name = (old_image.ImageId.ToString() + "-" + Image.FileName).Replace(" ", "-");
                    }
                    else
                    {
                        file_name = (old_image.ImageId.ToString() + "-" + user_name + Path.GetExtension(Image.FileName)).Replace(" ", "-");
                    }
                    

                    System.Drawing.Image web_image = System.Drawing.Image.FromStream(Image.InputStream);

                    //save Original Image
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name);

                    //Save Thumnails 200x200
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserThumbWidth, UserThumbHeight);
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserThumbWidth2, UserThumbHeight2);
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserSmallThumbWidth, UserSmallThumbHeight);
                    ImageService.SaveImage((System.Drawing.Image)web_image.Clone(), file_name, UserMediumThumbWidth, UserMediumThumbHeight);

                    //Update the DB value
                    old_image.URL = ImageService.GetImagesDirectory() + file_name;
                    if(old_image.UserImages.FirstOrDefault()!= null)
                    {
                        old_image.UserImages.FirstOrDefault().UserProfile.Avatar = old_image.URL;
                    }
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

        public bool DeleteUserimage(int image_id)
        {
            Image img = DAManager.ImagesRepository.Get(im => im.ImageId == image_id, null, "UserImages.UserProfile").FirstOrDefault();
            if (img == null)
                return false;

            foreach (UserImage  AI in img.UserImages.ToList())
            {
                DAManager.UserImagesRepository.Delete(AI);
            }

            DAManager.ImagesRepository.Delete(img);

            try
            {
                string FileName = Path.GetFileName(img.URL);
                string Directory = Path.GetDirectoryName(img.URL);

                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL));
                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), UserThumbWidth.ToString(), UserThumbHeight.ToString());
                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), UserThumbWidth2.ToString(), UserThumbHeight2.ToString());
                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), UserSmallThumbWidth.ToString(), UserSmallThumbHeight.ToString());
                ImageService.DeleteImage(HttpContext.Current.Server.MapPath("~" + img.URL), UserMediumThumbWidth.ToString(), UserMediumThumbHeight.ToString());

                if(img.UserImages.FirstOrDefault() != null)
                {
                    img.UserImages.FirstOrDefault().UserProfile.Avatar = "";
                }

                DAManager.Save();
                return true;
            }
            catch (Exception ex)
            {
                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                return false;
            }
        }
        
        public int UpdateUser(UserViewModel user)
        {
            UserProfile  old_usr = DAManager.UserProfilesRepository.Get(usr => usr.UserId== user.UserId,null,"webpages_Roles").FirstOrDefault();

            if (old_usr != null)
            {
                old_usr.FirstName = user.FirstName;
                old_usr.LastName = user.LastName;
                old_usr.MiddleName = user.MiddleName;
                old_usr.DateOfBirth = user.DateOfBirth;
                old_usr.Gender = user.Gender;
            }
            else
                return -1;
            try
            {
                DAManager.Save();
                return old_usr.UserId;
            }
            catch (Exception ex)
            {
                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                return -1;
            }
        }

        public UserViewModel GetUserById(int user_id)
        {
            var user = DAManager.UserProfilesRepository.Get(usr => usr.UserId == user_id, null,"webpages_Roles").FirstOrDefault();
            if (user != null)
            {
                UserViewModel user_viewmodel = new UserViewModel() {DateOfBirth = user.DateOfBirth,Email = user.Email,FirstName = user.FirstName,Gender = user.Gender,LastName = user.LastName, MiddleName = user.MiddleName,Name=user.Name,RegistrationDate=user.RegistrationDate,UserId = user.UserId };
                if (user.webpages_Roles.Where(role => role.RoleName == "Admin").Count() > 0)
                    user_viewmodel.IsAdmin = true;

                return user_viewmodel;
            }
            else
                return null;
        }

        public int GetUserIdByName(string userName)
        {
            UserProfile db_user = DAManager.UserProfilesRepository.Get(u => u.Name == userName).FirstOrDefault();

            if (db_user == null)
                return -1;

            return db_user.UserId;
        }

        public List<ListItemUserViewModel> GetUsers(out int total_count, int page = 0, int page_size = 10, string search_key = "", string order_by = "Name", string order_dir = "asc")
        {
            IQueryable<UserProfile> result;
            if (order_by == "Email")
                result = DAManager.UserProfilesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Email) : a.OrderByDescending(c => c.Email)));
            else
                result = DAManager.UserProfilesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.RegistrationDate) : a.OrderByDescending(c => c.RegistrationDate)));


            if (search_key != "")
                result = result.Where(cat => cat.Email.Contains(search_key) || cat.Name.Contains(search_key));

            total_count = result.Count();
            result = result.Skip(page).Take(page_size);
            List<UserProfile> final_res = result.ToList();
            List<ListItemUserViewModel> final_result = new List<ListItemUserViewModel>();
            foreach (UserProfile user in final_res)
                final_result.Add(new ListItemUserViewModel() { UserId = user.UserId, Email = user.Email, Name = user.Name, RegistrationDate = user.RegistrationDate.ToString() });

            return final_result;
        }


        #endregion

        #region Sucscriptions
        public bool IsSubscribed(string email, int campagin_id)
        {
            Subscription old = DAManager.SubscriptionsRepository.Get(s => s.Email == email && s.CampaignId == campagin_id).FirstOrDefault();
            if (old == null)
                return false;

            else return true;
        }

        public string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 64 )
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
        public bool Subscribe(int? user_id,string email, int campagin_id)
        {
            if (!StringsUtility.IsValidEmail(email))
                return false;

            Subscription old = DAManager.SubscriptionsRepository.Get(s => s.Email == email && s.CampaignId == campagin_id).FirstOrDefault();

            if (old != null)
            {
                old.Active = true;
            }
            else
            {

                Subscription new_subscription = new Subscription() { Active = true, CampaignId = campagin_id, UserId = user_id==-1?null:user_id , Email = email };
                new_subscription.UnsubscripeToken = RandomString();
                DAManager.SubscriptionsRepository.Insert(new_subscription);
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

        public bool Unsubscribe(int user_id, int campagin_id)
        {
            UserProfile user = DAManager.UserProfilesRepository.Get(u => u.UserId == user_id).FirstOrDefault();

            Subscription sub = DAManager.SubscriptionsRepository.Get(s => s.Email == user.Email && s.CampaignId == campagin_id).FirstOrDefault();

            if (sub == null)
                return true;

            sub.Active = false;

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

        public bool Unsubscribe(string unsubscribe_token, int campagin_id)
        {
            Subscription sub = DAManager.SubscriptionsRepository.Get(s => s.UnsubscripeToken == unsubscribe_token  && s.CampaignId == campagin_id).FirstOrDefault();

            if (sub == null)
                return true;

            sub.Active = false;

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

        public bool EmailSent(int user_id, int campagin_id)
        {
            Subscription sub = DAManager.SubscriptionsRepository.Get(s => s.UserId == user_id && s.CampaignId == campagin_id).FirstOrDefault();

            sub.LastSend = DateTime.Now;
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

        public bool EmailSent(string email, int campagin_id)
        {
            Subscription sub = DAManager.SubscriptionsRepository.Get(s => s.Email == email && s.CampaignId == campagin_id).FirstOrDefault();

            sub.LastSend = DateTime.Now;
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

        public void LogSubscription(string username, string user_id, string email,string directory_path )
        {
            DateTime now = DateTime.Now;
            string file_name = now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                if (!Directory.Exists(directory_path))
                    Directory.CreateDirectory(directory_path);

                File.AppendAllText(directory_path + "/" + file_name, now.ToString() + "...." + email + "...." + username +"...." + user_id + Environment.NewLine);
            }
            catch (Exception ex)
            {
                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                //Log the exception here
            }
        }

        public List<SubscriptionViewModel> GetSubscriptionEmails(int campaign_id, int count)
        {
            DateTime today = DateTime.Now;
            IQueryable<Subscription> result = DAManager.SubscriptionsRepository.Get(s=> s.Active == true && s.CampaignId == campaign_id  && (s.LastSend == null ||
                DbFunctions.CreateDateTime(s.LastSend.Value.Year, s.LastSend.Value.Month, s.LastSend.Value.Day, 23, 59, 59) < DateTime.Now), s => s.OrderBy(a => a.LastSend), "UserProfile");

            result = result.Take(count);

            return result.Select(s => new SubscriptionViewModel() {Email = s.Email, UserId = s.UserId, UserName=s.UserProfile == null ?"" : s.UserProfile.Name, UnsubscriptionToken = s.UnsubscripeToken   }).ToList();
        }

        #endregion


    }
}
