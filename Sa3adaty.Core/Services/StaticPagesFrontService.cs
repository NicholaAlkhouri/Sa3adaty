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
    public class StaticPagesFrontService
    {
        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
        public StaticPagesFrontService(DataAccessManager unit_of_work)
        {
            DAManager = unit_of_work;
            logService = new LogService(unit_of_work);
        }
        #endregion

        #region Method
        public StaticPageViewModel GetStaticPage(string section_code)
        {
            StaticPage db_page =  DAManager.StaticPagesRepository.Get(a => a.SectionCode == section_code).FirstOrDefault();

            if (db_page == null)
            {
                return null;
            }

            StaticPageViewModel res = new StaticPageViewModel() {Description = db_page.Description, Section = db_page.Section  };

            return res;

        }
        #endregion
    }
}
