using MyEvernote.Common;
using MyEvernote.WebApp.Init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyEvernote.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // yazýlan bu metodla App.Common metodu artýk WebCommon() class ile çlýþacak. Burada bu metod yazýlmasaydý eðer Common katmanýndaki DefaultCommon metoduyla çalýþacaktýr.
            // Artýk Common katmanýndaki App class ýna WebCommon()  metodonu set ettik.
            App.Common = new WebCommon(); 
        }
    }
}
