using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjets;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [ErrorExc]
    public class HomeController : Controller
    {
       private CategoryManager categoryManager = new CategoryManager();
       private NoteManager noteManager = new NoteManager();
       private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();

        [HttpGet]
        public ActionResult Index()
        {
            // CaategoryController üzerinden gelen View Talebi
            //if(TempData["categoryNotes"] != null)
            //{
            //    List<Note> notes = TempData["categoryNotes"] as List<Note>;
            //    return View(notes);
            //}

            return View(noteManager.ListQueryable().Where(x=>x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
            //return View(noteManager.GetAllNotesQueryable().OrderByDescending(x => x.ModifiedOn).ToList()); // yukarıdaki sorgu ile aynı çalışır. toList() dediğin zaman sorgu çalışır.
        }

        // 2. Yol -->  
        [HttpGet]
        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Category cat = categoryManager.Find(x=>x.Id == id.Value);

            //if (cat == null)
            //{
            //    return HttpNotFound();
            //}

            //return View("Index", cat.Notes.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());

            List<Note> notes = noteManager.ListQueryable().Where(x => x.CategoryId == id.Value && x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList();

            return View("Index", notes);

        }

        [HttpGet]
        public ActionResult MostLiked()
        {
            return View("Index", noteManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [Auth]
        [HttpGet]
        public ActionResult ShowProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu.",
                    RedirectingUrl = "/Home/Index",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }

        [Auth]
        [HttpGet]
        public ActionResult EditProfile()
        {
            //EvernoteUser currentUser = Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu.",
                    RedirectingUrl = "/Home/ShowProfile",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EvernoteUser model, HttpPostedFileBase ProfileImageFile)
        {
            ModelState.Remove("ModifiedUsurname");
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                if (ProfileImageFile != null && (ProfileImageFile.ContentType == "image/jpg" ||
                                                       ProfileImageFile.ContentType == "image/png" ||
                                                       ProfileImageFile.ContentType == "image/jpeg"))
                {
                    string filename = $"user_{model.Id}.{ProfileImageFile.ContentType.Split('/')[1]}";
                    ProfileImageFile.SaveAs(Server.MapPath($"~/images/{filename}"));
                    model.ProfileImageFile = filename;
                }

                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.UpdateProfile(model);
                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Items = res.Errors,
                        Title = "Profil Güncellenemedi",
                        RedirectingUrl = "/Home/EditProfile"
                    };

                    return View("Error", errorNotifyObj);
                }

                //Session["login"] = res.Result;          // Profil güncellendiği için session güncellendi.
                CurrentSession.Set<EvernoteUser>("login", res.Result);  // Profil güncellendiği için session güncellendi.

                return RedirectToAction("ShowProfile");
            }

            return View(model);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            //EvernoteUser currentUser = Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.RemoveUserById(CurrentSession.User.Id);

            if(res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi",
                    RedirectingUrl = "/Home/EditProfile"
                };

                return View("Error", errorNotifyObj);
            }

            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult TestNotify()
        {
            ErrorViewModel model = new ErrorViewModel()
            {
                Header = "Yönlendirme",
                Title = "Bilgi Test",
                RedirectingTimeout = 5000,
                Items = new List<ErrorMessageObj>() { new ErrorMessageObj() { Message = "test erorr 1 " }, new ErrorMessageObj(){Message = "test error 2" } }
            };
            return View("Error", model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            //Yönlendirme
            //Sessiona bilgi saklama
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.LoginUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    //if((res.Errors.Find(x=>x.Code == ErrorMessageCode.UserIsNotActive) != null))
                    //{
                    //    ViewBag.SetLink = "http://Home/UserActivate/123-456-789-85264";
                    //}

                    return View(model);
                }

               //["login"] = res.Result;                 //Sessiona bilgi saklama
                CurrentSession.Set<EvernoteUser>("login", res.Result);  //Sessiona bilgi saklama
                return RedirectToAction("Index");  //Yönlendirme
            }

            return View(model);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            //Kullanıcı username kontrolü
            //Kullanıcı email kontrolü
            ///Kayıt işlemi
            ///Aktivasyon mesajı gönderimi

            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message)); // dönen hata mesajlarını ModelState ekleyecem.Böylece tüm hatalar ValidatationSummary de gösterilecek
                    return View(model); // hata olduğu için modeli geri dönderdik.
                }

                OkViewModel notifyObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login",
                };
                notifyObj.Items.Add("Lütfen e-posta adresine gönderdiğimiz aktivasyon linkine tıklayarak hesabınızı aktive ediniz.Hesabınızı aktive etmeden not ekleyemez ve beğenme yapamazsınız.");

                return View("Ok",notifyObj);
            }
            //if (ModelState.IsValid)
            //{

            //    if(model.Email == "baranvelat021@gmail.com")
            //    {
            //        ModelState.AddModelError("", "email kullanılıyor"); //ModelState.AddModelError() metodu dönen hatayı View tarafında ValidationSummary tarafından görüntülenir.

            //    }

            //    if(model.Username == "welatbaran")
            //    {
            //        ModelState.AddModelError("", "kullanıcı adı kullanılıyor");

            //    }

            //    foreach (var item in ModelState)
            //    {
            //        if(item.Value.Errors.Count > 0)
            //        {
            //            return View(model);
            //        }
            //    }

            //    return RedirectToAction("RegisterOk");
            //}

            return View(model); // geçerli değilse modeli sayfaya aynen geri gönderiyoruz   
        }

        [HttpGet]
        public ActionResult UserActivate(Guid id)
        {
            // Kullanıcı aktivayon sağlanacak
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                TempData["errors"] = res.Errors;

                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    RedirectingUrl = "/Home/Index",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            OkViewModel okNotifyObj = new OkViewModel()
            {
                Title = "Hesabınız Aktifleştirildi",
                RedirectingUrl = "/Home/Login",
            };
            okNotifyObj.Items.Add("Sitemize Hoşgeldiniz. Artık not paylaşabilir ve beğenme yapabilirsiniz.");

            return View("Ok",okNotifyObj);
        }

        public ActionResult AccessDenied()
        {
            WarningViewModel warningNotifyObj = new WarningViewModel()
            {
                Header = "Yetkisiz Erişim",
                Title = "Bu sayfaya erişim yetkiniz bulunmamaktadır.",
                RedirectingUrl = "/Home/Index"
            };
            return View("Warning", warningNotifyObj);
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}