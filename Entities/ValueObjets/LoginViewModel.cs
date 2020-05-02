using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ValueObjets
{
    public class LoginViewModel
    {
        [DisplayName("Kulanıcı adı"), Required(ErrorMessage ="{0} alanı boş geçilemez."), StringLength(50, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Username { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage ="{0} alanı boş geçilemez."),DataType(DataType.Password), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Password { get; set; }
    }
}