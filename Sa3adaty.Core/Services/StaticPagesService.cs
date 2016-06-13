using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class StaticPagesService
    {
         #region Privates
        private DataAccessManager DAManager;
        private LogService logService;
        #endregion

        #region Constructor
        public StaticPagesService(DataAccessManager unit_of_work)
        {
            DAManager = unit_of_work;
            logService = new LogService(unit_of_work);
        }
        #endregion
    }
}
