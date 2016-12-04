using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SportsStore.WebUI.Infrastructure.Abstract;
namespace SportsStore.WebUI.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password)
        {
            bool result = false;
            if (username == "admin" && password == "secret")
            {
                FormsAuthentication.SetAuthCookie(username, false);
                result = true;
            }
            return result;
        }
    }
}