using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyBlog.WebUI.Models;

namespace MyBlog.WebUI.Filters
{
    public class Auth : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (CurrentSession.CurrentUser == null)
            {
                context.Result = new RedirectResult("/Home/Login");
            }
        }
    }
}
