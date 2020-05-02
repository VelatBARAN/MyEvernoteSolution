using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("EvernoteUsers")]
    public class EvernoteUser : MyEntityBase
    {
        [DisplayName("Ad"), Required(ErrorMessage = "{0} alanı boş geçilemez."), StringLength(25,ErrorMessage ="{0} alanı max. {1} karakter olmalıdır.")]
        public string Name { get; set; }

        [DisplayName("Soyad"), Required(ErrorMessage = "{0} alanı boş geçilemez."), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage ="{0} alanı boş geçilemez."), StringLength(50, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Usurname { get; set; }

        [DisplayName("E-Mail"), Required(ErrorMessage = "{0} alanı boş geçilemez."), StringLength(50, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez."), StringLength(100, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Password { get; set; }

        [DisplayName("Resim"), StringLength(100),ScaffoldColumn(false)]
        public string ProfileImageFile { get; set; }

        [DisplayName("Aktif Mi?")]
        public bool IsActive { get; set; }
        [DisplayName("Admin Mi?")]
        public bool IsAdmin { get; set; }

        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }

        public virtual List<Note> Notes { get; set; } // Başka bir tablo ile ilişkisi olduğu için virtual olarak tanımlanır
        public virtual List<Comment> Cooments { get; set; }
        public virtual List<Liked> Likes { get; set; }
    }
}
