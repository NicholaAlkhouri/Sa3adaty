using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Controllers
{
    public class SiteMapController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;
        
        #region Constructor
        public SiteMapController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
        }
        #endregion

        //
        // GET: /SiteMap/

        public ActionResult Generate(string id )
        {
            if (id == "abcd1234")
            {
                servicesManager.SiteMapService.GenerateSiteMap();
            }
            return View();
        }

    }
}
