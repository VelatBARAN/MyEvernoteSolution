using MyEvernote.Common;
using MyEvernote.Entities;
using MyEvernote.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEvernote.WebApp.Init
{
    public class WebCommon : ICommon  // Global.asax.cs yani sistem ayağa kalktığında WebCommon() adında bir class oluşturduk. Dönen tür ICommon interface olduüu için burdada impement ettik.
    {
        public string GetCurrentUsername()
        {
            EvernoteUser user = CurrentSession.User;

            if (user != null)
                return user.Usurname;
            else
                return "system";
        }
    }
}