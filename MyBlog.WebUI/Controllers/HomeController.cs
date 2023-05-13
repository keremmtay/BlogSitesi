using Microsoft.AspNetCore.Mvc;
using MyBlog.BusinessLayer;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.ViewModels;
using MyBlog.WebUI.Filters;
using MyBlog.WebUI.Models;
using System.Diagnostics;
using System.Text.Json;

namespace MyBlog.WebUI.Controllers
{
    [HandleException]
    public class HomeController : Controller
    {

        CategoryManager _categoryManager = new CategoryManager();

        NoteManager _noteManager = new NoteManager();

        BlogUserManager _blogUserManager = new BlogUserManager();

        public IActionResult Index()
        {
            FakeDataManager.CreateFakeData();

            return View(_noteManager.List().OrderByDescending(x => x.ModifiedDate).ToList());
        }

        public IActionResult SelectCategory(int? id)
        {
            if (id == null)
            {
                return BadRequest();  // HttpStatusCode
            }

            Category category = _categoryManager.GetCategoryByIdWithNotes(id.Value);

            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Index" , category.Notes);
        }

        public IActionResult MostLiked()
        {
            return View("Index", _noteManager.List().OrderByDescending(x => x.LikeCount).ToList());
        }

        public IActionResult LatestEntries()
        {
            return View("Index", _noteManager.List().OrderByDescending(x => x.ModifiedDate).ToList());
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Girilen bilgilerin kontrolü yapılacak...
            // Bilgiler doğru ise Index sayfasına yönlendirilecek
            // Kullanıcı adını session'da saklaması gibi

            if (ModelState.IsValid)
            {
                BusinessLayerResult<BlogUser> blResult = _blogUserManager.Login(model);

                if (blResult.Errors.Count > 0)
                {
                    blResult.Errors.ForEach(x => ModelState.AddModelError("", x));
                }

                // Kullanıcı sessiona'a eklenecek. Önce Json'a çeviriyorum.

                //string currentUserJson = JsonSerializer.Serialize<BlogUser>(blResult.Result);

                //HttpContext.Session.SetString("currentUser", currentUserJson);

                CurrentSession.SetUser("currentUser", blResult.Result);

                return RedirectToAction("Index", "Home");

            }

            return View(model);
        }

        public IActionResult Logout()
        {
            CurrentSession.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {

            // Verilerin doğruluğu kontrol edilir. Validation işlemleri
            // Validation işleminden geçerse veritabanına kaydı yapılacak
            // E-posta gönderilecek (aktivasyon için)

            if (ModelState.IsValid)
            {

                #region Eski Kodlar

                // Aşağıda yapılan karşılaştırmalar UI tarafında yapılmaz. Bu işler BusinessLayer tarafında yapılması gereken işlerdir.
                // Aşağıdaki kodlar, diğer kodların çalışıp çalışmadığı test amaçlı yazıldı.

                //bool hasError = false;

                //if (model.UserName == "aaa")
                //{
                //    ModelState.AddModelError("", "Kullanıcı adı kullanılıyor..");
                //    hasError = true;
                //}
                //if (model.Email == "abc@abc.com")
                //{
                //    ModelState.AddModelError("", "Girdiğiniz e-posta başkası tarafından kullanılıyor.");
                //    hasError = true;
                //}

                //if (hasError) 
                //{
                //    return View(model);
                //}

                //return RedirectToAction("RegisterSuccess");


                //  CV'YE YAZILACAKLAR

                // KURSLAR

                // Çalışma Bakanlığı Nitelikli Bilişim Uzmanı Yetiştirme Projesi -Kurumsal Kaynak Planlama ve Veri Analizi(680 saat)
                // Fatih Sultan Mehmet Vakıf Üniversitesi Sürekli Eğitim Merkezi

                // SERTİFİKALAR

                // Microsoft AZ204
                // ICCW Yazılım Uzmanlığı
                // Milli Eğitim Bakanlığı Kurumsal Kaynak Planlama ve Veri Analizi
                // Fatih Sultan Mehmet Vakıf Üniversitesi Yazılım Uzmanlığı
                // İşkur Kurumsal Kaynak Planlama ve Veri Analizi

                #endregion

                BusinessLayerResult<BlogUser> blResult = _blogUserManager.RegisterUser(model);

                if (blResult.Errors.Count>0)
                {
                    // Hataları ekranda göster..

                    blResult.Errors.ForEach(x => ModelState.AddModelError("", x));

                    // AddModelError içindeki hata mesajlarını ekranda görebiliyorum ama BusinessLayerResult'ta Errors List'ten gelen hata mesajlarını göremiyorum.

                    // Yukarıdaki kod ile Errors List'teki hataları ModelState.AddModelError içine ekleyerek, hata mesajlarının ekranda görünmesini sağlıyorum.

                    return View(model);
                }

                return RedirectToAction("RegisterSuccess");
            }


            return View(model);
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }

        public IActionResult UserActivate(Guid id)
        {
            // Mail hesabına gelen link ile çalışacak olan Action burasıdır.

            BusinessLayerResult<BlogUser> blResult = _blogUserManager.UserActivate(id);

            if (blResult.Errors.Count>0)
            {
                TempData["errors"]=blResult.Errors;

                return RedirectToAction("ActivateUserCancel");
            }

            return RedirectToAction("ActivateUserOk");
        }

        public IActionResult ActivateUserOk()
        {
            return View();
        }

        public IActionResult ActivateUserCancel()
        {
            List<string> errors = null;
            if (TempData["errors"] != null)
            {
                errors = TempData["errors"] as List<string>;
            }

            return View(errors);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult HasError()
        {
            return View();
        }



    }
}