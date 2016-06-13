using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.Filters;

namespace Sa3adaty.Controllers
{
    [MaintenanceFilter]
    public class QuoteController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public QuoteController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
        }
        #endregion
        public ActionResult _DailyQuote()
        {

            return PartialView(servicesManager.QuoteService.GetTodayQuote());
        }
    }
}
