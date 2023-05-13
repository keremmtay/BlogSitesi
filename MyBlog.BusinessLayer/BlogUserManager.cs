using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyBlog.Common.Helpers.EmailServices;
using MyBlog.DataAccessLayer.Concrete;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BusinessLayer
{
    public class BlogUserManager : BaseManager<BlogUser>
    {
        public BusinessLayerResult<BlogUser> DeleteUser(int id)
        {
            // Kullanıcıyı silme ile ilgili kodlar yazılacak. Hata olursa hatalar geriye gönderilecek

            BusinessLayerResult<BlogUser> blResult = new BusinessLayerResult<BlogUser>();

            BlogUser user = Find(x => x.Id == id);

            if (user != null)
            {
                // Silme işlemi yapılacak..
                // ilişkili veriler silinecek.. Notes, Comments, Likes tablosundan

                if (Delete(user) == 0)
                {
                    blResult.Errors.Add("Kullanıcı silinemedi.");

                    return blResult;
                }

            }
            else
            {
                blResult.Errors.Add("Kullanıcı bulunamadı.");
            }

            return blResult;
        }

        public BusinessLayerResult<BlogUser> GetUserById(int id)
        {
            BusinessLayerResult<BlogUser> blResult = new BusinessLayerResult<BlogUser>();

            BlogUser user = Find(x => x.Id == id);

            if (user == null)
            {
                blResult.Errors.Add("Kullanıcı bulunamadı.");
            }
            else
            {
                blResult.Result = user;
            }

            return blResult;
        }

        public BusinessLayerResult<BlogUser> Login(LoginViewModel model)
        {
            // Giriş kontrolü: kullanıcı adı var mı varsa girilen password doğru mu?
            // Hesap aktif mi?
            // ModelState ile kontrol yapılmalı
            

            BusinessLayerResult<BlogUser> layerResult = new BusinessLayerResult<BlogUser>();

            layerResult.Result = base.Find(x => x.UserName == model.UserName && x.Password == model.Password);

            if (layerResult.Result != null)
            {
                if (!layerResult.Result.IsActive)
                {
                    layerResult.Errors.Add("Hesabınız aktif değil. Lütfen E-Postanızı kontrol edin.");

                }
            }
            else
            {
                layerResult.Errors.Add("Kullanıcı Adı ya da Şifre hatalı. Ya da kayıtlı kullanıcı değilsiniz.");
            }

            return layerResult;
        }

        public BusinessLayerResult<BlogUser> RegisterUser(RegisterViewModel registerUser)
        {
            BlogUser user = base.Find(x => x.UserName == registerUser.UserName || x.Email == registerUser.Email);
            
            BusinessLayerResult<BlogUser> layerResult = new BusinessLayerResult<BlogUser>();

            if (user != null) 
            {
                if (user.UserName == registerUser.UserName)
                {
                    layerResult.Errors.Add("Kullanıcı adı sistemde kayıtlı.");
                }

                if (user.Email == registerUser.Email)
                {
                    layerResult.Errors.Add("E-posta sistemde kayıtlı.");
                }
            }
            else
            {
                // Kayıt için ilgili kodları buraya yazıyoruz.
                // RegisterViewModel registerUser içindeki bilgileri BlogUser tablosuna ekle
                // Ekledikten sonra girilen email adresine aktivasyon kodunu gönder.

                int result = base.Insert(new BlogUser() 
                { 
                    Name = " ",
                    Surname = " ",
                    UserName = registerUser.UserName,
                    Email = registerUser.Email,
                    Password = registerUser.Password,
                    UserProfileImage = "user-profile.jpg",
                    IsActive = false,
                    IsAdmin = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ModifiedUserName = "system",
                    ActivateGuid = Guid.NewGuid(),
                });

                if (result > 0)
                {
                    // Kaydolan kullanıcıya aktivasyon emaili gönder
                    // Email göndermeyle ilgili kodları buraya yazacağız
                    // Ama önce burayı test edelim

                    layerResult.Result = base.Find(x => x.UserName == registerUser.UserName && x.Email == registerUser.Email);

                    string url = $"https://localhost:7259/Home/UserActivate/{layerResult.Result.ActivateGuid}";

                    IEmailSender emailSender = new EmailSender("smtp.office365.com", 587, true, "deneme1246435@hotmail.com", "d1246435*");

                    emailSender.SendEmailAsync(layerResult.Result.Email, "MyBlog sitesi için üyeliğinizi onaylayın.", $"Merhaba {layerResult.Result.Name}, lütfen email hesabınızı onaylamak için linke <a href='{url}' target='_blank'> tıklayınız</a>.");

                }
            }

            return layerResult;

        }

        public BusinessLayerResult<BlogUser> UpdateProfile(BlogUser userData)
        {
            BusinessLayerResult<BlogUser> blResult = new BusinessLayerResult<BlogUser>();

            // Girilen Email ile UserName var mı yok mu kontrolünü yapmamız gerekiyor. Bunun için de Gelen User nesnesinin Idsinden farklı olan ve Email ya da Username'i olan kayıt var mı yok mu, veritabanından bunun sorgusunu yapıyoruz.

            BlogUser userDb = Find(x => x.Id != userData.Id && (x.Email == userData.Email || x.UserName == userData.UserName));

            if (userDb != null && userDb.Id != userData.Id) 
            {
                if (userDb.UserName == userData.UserName)
                {
                    blResult.Errors.Add("Girdiğiniz kullanıcı adı başka bir üyemiz tarafından kullanılmaktadır. Lütfen farklı bir kullanıcı adı giriniz.");
                }
                if (userDb.Email == userData.Email)
                {
                    blResult.Errors.Add("Girdiğiniz e-posta başka bir üyemiz tarafından kullanılmaktadır. Lütfen farklı bir e-posta giriniz.");
                }

                return blResult;
            }

            // Eğer herhangi bir hata yok ise Update işlemleri ile ilgili kodları yazmalıyım

            blResult.Result = Find(x => x.Id == userData.Id);
            blResult.Result.Name = userData.Name;
            blResult.Result.Surname = userData.Surname;
            blResult.Result.Email = userData.Email;
            blResult.Result.UserName = userData.UserName;
            blResult.Result.Password = userData.Password;
            blResult.Result.ModifiedUserName = userData.UserName;
            blResult.Result.ModifiedDate = DateTime.Now;

            // Fotoğraf geldiyse bunun kontrolünü yapalım

            if (string.IsNullOrEmpty(userData.UserProfileImage) == false)
            {
                blResult.Result.UserProfileImage = userData.UserProfileImage;
            }

            if (Update(blResult.Result) == 0)
            {
                blResult.Errors.Add("Profil güncellenemedi.");
            }

            return blResult;
        }

        public BusinessLayerResult<BlogUser> UserActivate(Guid id)
        {
            BusinessLayerResult<BlogUser> blResult = new BusinessLayerResult<BlogUser>();

            blResult.Result = base.Find(x => x.ActivateGuid==id);

            if (blResult.Result != null)
            {
                // blResult.Result null değlse demek ki ilgili kullanıcı için guid doğru demektir. Bu durumda kullanıcının isActive değerini true olarak değiştirebiliriz.


                if (blResult.Result.IsActive)
                {
                    blResult.Errors.Add("Kullanıcı zaten aktif.");
                }
                else
                {
                    blResult.Result.IsActive = true;
                    base.Update(blResult.Result);
                }
            }
            else
            {
                blResult.Errors.Add("Aktifleştirilecek kullanıcı bulunamadı.");
            }

            return blResult;
        }
    }
}
