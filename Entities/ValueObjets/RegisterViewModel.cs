using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ValueObjets
{
    public class RegisterViewModel
    {
        [DisplayName("Ad"),Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Name { get; set; }

        [DisplayName("Soyad"), Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı adı"), Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(50, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Username { get; set; }

        [DisplayName("E-Mail"), Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(50, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır."),
            EmailAddress(ErrorMessage = "{0} alanı için geçerli bir email adresi giriniz.}")]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez."), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır."), DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Şifre(Tekrar)"), Required(ErrorMessage = "{0} alanı boş geçilemez."), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır."),
            DataType(DataType.Password), Compare("Password", ErrorMessage = "{0} ile {1} alanları uyuşmuyor.")]
        public string RePassword { get; set; }
    }
}