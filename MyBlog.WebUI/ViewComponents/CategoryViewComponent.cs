using Microsoft.AspNetCore.Mvc;
using MyBlog.BusinessLayer;
using MyBlog.Entities.Concrete;

namespace MyBlog.WebUI.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            CategoryManager manager = new CategoryManager();

            List<Category> categories = manager.List();

            return View(categories);

            // veya List yazmadan return View(manager.GetCategories());
        }
    }
}
