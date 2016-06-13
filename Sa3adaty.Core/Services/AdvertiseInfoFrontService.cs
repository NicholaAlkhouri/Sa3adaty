using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class AdvertiseInfoFrontService
    {
        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public AdvertiseInfoFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Method

        public int AddAdvertiseInfo(AdvertiseInfoViewModel advertise_info)
        {
            AdvertiseInfo db_advertise = new AdvertiseInfo() { Email = advertise_info.Email,CompanyName = advertise_info.CompanyName , Message = advertise_info.Message, Name = advertise_info.Name, Subject = advertise_info.Subject, AddDate = DateTime.Now };

            DAManager.AdvertiseInfoRepository.Insert(db_advertise);
            try
            {
                DAManager.Save();
                return db_advertise.Id;
            }
            catch (Exception ex)
            {
                logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                return -1;
            }
        }
        #endregion 
    }
}
