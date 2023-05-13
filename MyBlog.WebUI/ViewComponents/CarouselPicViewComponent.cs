using Microsoft.AspNetCore.Mvc;
using MyBlog.BusinessLayer;
using MyBlog.Entities.Concrete;

namespace MyBlog.WebUI.ViewComponents
{
    public class CarouselPicViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            CarouselPicManager manager = new CarouselPicManager();

            List<CarouselPic> carouselPics = manager.List();

            return View(carouselPics);
        }
    }
}
