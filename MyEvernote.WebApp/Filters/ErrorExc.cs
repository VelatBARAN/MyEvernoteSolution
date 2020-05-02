using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Filters
{
    public class ErrorExc : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.Controller.TempData["LastError"] = filterContext.Exception;  // oluşan hatayo atadık . Tempdata sadece 1 request lik değer dönderir.

            filterContext.ExceptionHandled = true; // hatayı benim yöneteceğimi gösterir
            filterContext.Result = new RedirectResult("/Home/HasError");
        }
    }
}