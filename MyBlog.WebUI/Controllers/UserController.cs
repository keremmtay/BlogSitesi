using Microsoft.AspNetCore.Mvc;
using MyBlog.BusinessLayer;
using MyBlog.Entities.Concrete;
using MyBlog.WebUI.Filters;
using MyBlog.WebUI.Models;
using System.Text.Json;

namespace MyBlog.WebUI.Controllers
{
    [Auth]
    [HandleException]
    public class UserController : Controller
    {
        BlogUserManager _userManager = new BlogUserManager();

        public IActionResult ShowProfile()
        {
            // Profil ilgili View'de gösterilecek

            // Aşağıdaki 2 satırda, veriyi sessiondan aldık. Veri Json formatta sessionda tutuluyordu. Bunu Deserialize yaparak BlogUser türüne çevirdik.  
            
            //string currentUserJson = HttpContext.Session.GetString("currentUser");

            //BlogUser currentUser = JsonSerializer.Deserialize<BlogUser>(currentUserJson);

            // Veritabanından ilgili user nesnesini tekrar istedik.

            BusinessLayerResult<BlogUser> layerResult = _userManager.GetUserById(CurrentSession.CurrentUser.Id);

            if (layerResult.Errors.Count > 0)
            {
                // Kullanıcıya hata mesajı gönderilecek


            }

            return View(layerResult.Result);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            //string currentUserJson = HttpContext.Session.GetString("currentUser");

            //BlogUser currentUser = JsonSerializer.Deserialize<BlogUser>(currentUserJson);

            // Veritabanından ilgili user nesnesini tekrar istedik.

            BusinessLayerResult<BlogUser> layerResult = _userManager.GetUserById(CurrentSession.CurrentUser.Id);

            if (layerResult.Errors.Count > 0)
            {
                // Kullanıcıya hata mesajı gönderilecek

            }

            return View(layerResult.Result);
        }

        [HttpPost]
        public IActionResult EditProfile(BlogUser user, IFormFile? file)
        {
            // Kullanıcıyı veritabanında güncelleyen kodlar yazılacak

            // Aşağıdaki satırlar ile ilgili alanların Validation işlemlerini yapmayacak

            ModelState.Remove("Notes");
            ModelState.Remove("Comments");
            ModelState.Remove("Likes");

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    // Parametredeki file boş gelmediyse, yani dosya geldiyse, bu durumda dosyay eşsiz bir isim ile wwwroot/images altına kaydetmemiz gerekiyor.
                    // Dosya uzantısını alalım.

                    var extension = Path.GetExtension(file.FileName);

                    // Dosya adı için eşsiz bir isim vereceğiz.

                    var imageName = string.Format($"user-{user.Id}{extension}"); // user-10.jpg gibi bir isim oluşturacak

                    // Dosyanın kaydedileceği Path'i ve dosya adını verelim.

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", imageName);

                    // Aşağıdaki satırda da dosya adı ve yolu verilen yere fotoğrafı kaydediyoruz.

                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    user.UserProfileImage = imageName;
                }

                // Artık user nesnesini güncellemek için gerekli kodları yazabiliriz.


                BusinessLayerResult<BlogUser> blResult = _userManager.UpdateProfile(user);

                if (blResult.Errors.Count>0)
                {
                    // hata varsa buraya gideceğiz

                    blResult.Errors.ForEach(x => ModelState.AddModelError("",x));

                    return View(user);
                }

                // hata yok ise
                // güncellenmiş kullanıcı session'a eklenecek.

                //string currentUserJson = JsonSerializer.Serialize<BlogUser>(blResult.Result);

                //HttpContext.Session.SetString("currentUser", currentUserJson);

                CurrentSession.SetUser("currentUser", blResult.Result);

                // ilgili sayfaya yönlendirmeyi yapalım

                return RedirectToAction("ShowProfile");
            }

            return View(user);
        }

        public IActionResult DeleteProfile(int id)
        {
            // Silinecek profilin Id'si geliyor. 

            BusinessLayerResult<BlogUser> blResult = _userManager.DeleteUser(id);

            if (blResult.Errors.Count > 0)
            {
                blResult.Errors.ForEach(x => ModelState.AddModelError("", x));

                // Error sayfası tasarlanacak

                return View("", blResult.Errors);
            }

            CurrentSession.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
