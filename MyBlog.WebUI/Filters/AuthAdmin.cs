using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyBlog.WebUI.Models;

namespace MyBlog.WebUI.Filters
{
    public class AuthAdmin : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (CurrentSession.CurrentUser != null && !CurrentSession.CurrentUser.IsAdmin)
            {
                context.Result = new RedirectResult("/Home/AccessDenied");
            }
        }
    }
}
