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
    public class ContactInfoFrontService
    {
        
        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public ContactInfoFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Method

            public int AddContactInfo(ContactInfoViewModel contact_info)
            {
                ContactInfo db_contact = new ContactInfo() { Email = contact_info.Email, Message = contact_info.Message, Name = contact_info.Name, Subject = contact_info.Subject ,AddDate = DateTime.Now };

                DAManager.ContactInfoRepository.Insert(db_contact);
                try
                {
                    DAManager.Save();
                    return db_contact.Id;
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
