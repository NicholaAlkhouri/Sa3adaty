using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels.Account;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public  class AccountFrontService
    {
        #region const
        static public int UserThumbWidth = 44;
        static public int UserThumbHeight = 44;
        #endregion

        #region Privates
        private DataAccessManager DAManager;
        private LogService logService;
        #endregion

        #region Constructor
            public AccountFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Public Methods
            public SimpleUserViewModel GetSimpleUserById(int user_id)
            {
                UserProfile db_user = DAManager.UserProfilesRepository.Get(u => u.UserId == user_id, null, "UserImages.Image").FirstOrDefault();
                if (db_user == null)
                    return null;

                SimpleUserViewModel user = new SimpleUserViewModel() { UserId = db_user.UserId, Name = db_user.Name };
                if(db_user.UserImages.Count()>0)
                    user.ImageUrl = ImageService.GenerateImageFullPath(db_user.UserImages.First().Image.URL,AccountService.UserThumbWidth.ToString(),AccountService.UserThumbHeight.ToString());

                return user;
                
            }

            public bool UpdateAccount(ManageAccountViewModel model)
            {
                
                UserProfile db_user = DAManager.UserProfilesRepository.Get(u => u.UserId == model.UserId).FirstOrDefault();

                if (db_user == null)
                    return false;

                db_user.Name = model.Name;
                db_user.CountryId = model.Country;
                db_user.Gender = model.Gender== 1 ? true : false ;

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

            public ManageAccountViewModel GetUserAccountInfo(int user_id)
            {
                UserProfile db_user = DAManager.UserProfilesRepository.Get(u => u.UserId == user_id, null, "UserImages.Image").FirstOrDefault();
                if (db_user == null)
                    return null;

                ManageAccountViewModel result = new ManageAccountViewModel() {
                    Email= db_user.Email,
                    Name = db_user.Name ,
                    Gender = db_user.Gender == null?0: (db_user.Gender.Value ?1:0) ,
                    Country = db_user.CountryId == null ? 0 : db_user.CountryId.Value,
                    Subscribe = IsSubscribed(db_user.Email,1),
                    UserId = db_user.UserId 
                };

                if (db_user.UserImages.Count > 0)
                {
                    result.Image = ImageService.GenerateImageFullPath(db_user.UserImages.First().Image.URL, AccountService.UserThumbWidth2.ToString(), AccountService.UserThumbHeight2.ToString());
                }

                return result;
            }

            public bool IsSubscribed(string email, int campagin_id)
            {
                Subscription old = DAManager.SubscriptionsRepository.Get(s => s.Email == email && s.CampaignId == campagin_id).FirstOrDefault();
                if (old == null || old.Active == false )
                    return false;

                else return true;
            }
#endregion 
    }
}
