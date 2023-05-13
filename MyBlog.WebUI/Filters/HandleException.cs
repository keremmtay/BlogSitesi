using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyBlog.WebUI.Filters
{
    public class HandleException : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;    // hatayı ben yöneteceğim diye bildirimde bulunuyorum.

            context.Result = new RedirectResult("/Home/HasError");
        }
    }
}
