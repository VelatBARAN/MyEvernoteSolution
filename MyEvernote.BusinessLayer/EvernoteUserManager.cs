using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Common.Helpers;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MyEvernote.BusinessLayer
{
    public class EvernoteUserManager : ManagerBase<EvernoteUser> /// miras alarak repo_user metodunu aşağıda tanımlamaya gerek kalmadı.
    {
        // BusinessLayer de çalışan tüm kodlar UI,UI Mobil,Wİndows gibi katmanlarda da çalışması gerekir. Sadece Web teki UI katmanında çalışmaması gerekir..

        //private Repository<EvernoteUser> repo_user = new Repository<EvernoteUser>();
        private BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            EvernoteUser user = Find(x => x.Usurname == data.Username || x.Email == data.Email);

            if (user != null)
            {
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Bu E-Email adresi zaten kayıtlı.");
                }
                if (user.Usurname == data.Username)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyExists, "Bu Kullanıcı adı zaten kayıtlı.");
                }
            }
            else
            {
                int dbResult = base.Insert(new EvernoteUser() // ManagerBAse deki Insert metodu old. gösterir.
                {
                    Name = data.Name,
                    Surname = data.Surname,
                    Usurname = data.Username,
                    Email = data.Email,
                    ProfileImageFile = "user.png",
                    Password = Crypto.Hash(data.Password.ToString(), "MD5"),
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false
                });

                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == data.Email && x.Usurname == data.Username);

                    // ToDo : Aktivasyon maili atılacak
                    //layerResult.Result.ActivateGuidl
                    //TODO : user active edildiğinde : sitemize hoşgeldiniz,site kuralları gibi bilgilendirme mesajı yapılabilir.   

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Name} {res.Result.Surname};<br><br> Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";
                    MailHelper.SendMail(body, res.Result.Email, "MyEvernote Hesap Aktifleştirme", true);
                }
            }

            return res; // geri dönüş tipi -->  BusinessLayerResult<EvernoteUser> olacak
        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {
            res.Result = Find(x => x.Id == id);

            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı Bulunamadı.");
            }

            return res;

        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            //Giriş kontrolü 
            //Hesap Akive edilmiş mi
            //EvernoteUser user = repo_user.Find(x => x.Usurname == data.Username && x.Password == data.Password); aşağıdaki sorgu gibi de kkullanılabilir.Tıpkı Register metodu gibi
            string hashPassword = Crypto.Hash(data.Password.ToString(), "MD5");
            res.Result = Find(x => x.Usurname == data.Username && x.Password == hashPassword);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen e-posta adresinizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı veya şifre uyuşmuyor.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser data)
        {
            EvernoteUser db_user = Find(x => x.Email == data.Email || x.Usurname == data.Usurname);

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Usurname == data.Usurname)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyExists, "Kullanıcı adı zaten kayıtlı");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Mail adresi zaten kayıtlı");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Usurname = data.Usurname;
            res.Result.Email = data.Email;
            if (data.Password != null)
            {
                res.Result.Password = Crypto.Hash(data.Password.ToString(), "MD5");
            }
            else
            {
                res.Result.Password = db_user.Password;
            }

            if (string.IsNullOrEmpty(data.ProfileImageFile) == false)
            {
                res.Result.ProfileImageFile = data.ProfileImageFile;
            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdate, "Profil Güncellenemedi");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {
            res.Result = Find(x => x.Id == id);

            if (res.Result != null)
            {
                if (Delete(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunmadı");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            res.Result = Find(x => x.ActivateGuid == activateId);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActivated, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }

                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdDoesNotExists, "Aktifleştirilecek kullanıcı bulunamadı.");
            }

            return res;
        }

        // yeni bir kullanıcı eklerken dönen hataları kontrol etmek için yeni bi Insert metodu oluşturduk. Bu metodu tanımlarken metod gizleme(method hidding) new leme tekniğini kullandık.
        // Bu teknik aynı isimde bir metod varsa alttakinin değil yazdığın metodu kullanmasını sağlar.
        // Bu metodu ManagerBase deki insert metodu int değer dönderdiği için oluşturdu. Çünkü biz dönen hataları kontrol edebilmek için bize BusinessLayerResult tüeünden bir değer lazım.
        // Method hidding
        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data) // ManagerBAse deki Insert i ezdik.
        {
            EvernoteUser user = Find(x => x.Usurname == data.Usurname || x.Email == data.Email);
            res.Result = data;

            if (user != null)
            {
                if (user.Email == res.Result.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Bu E-Email adresi zaten kayıtlı.");
                }
                if (user.Usurname == res.Result.Usurname)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyExists, "Bu Kullanıcı adı zaten kayıtlı.");
                }
            }
            else
            {
                res.Result.ProfileImageFile = "user.png";
                res.Result.ActivateGuid = Guid.NewGuid();
                res.Result.Password = Crypto.Hash(data.Password.ToString(), "MD5");

                if(base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı eklenirken hata oluştu.");
                }              
            }

            return res; // geri dönüş tipi -->  BusinessLayerResult<EvernoteUser> olacak
        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            EvernoteUser db_user = Find(x => x.Email == data.Email || x.Usurname == data.Usurname);
            res.Result = data;

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Usurname == data.Usurname)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyExists, "Kullanıcı adı zaten kayıtlı");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Mail adresi zaten kayıtlı");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Usurname = data.Usurname;
            res.Result.Email = data.Email;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;
            if (data.Password != null)
            {
                res.Result.Password = Crypto.Hash(data.Password.ToString(), "MD5");
            }
            else
            {
                res.Result.Password = db_user.Password;
            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdate, "Kullanıcı Güncellenemedi");
            }

            return res;
        }
    }
}
