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
    public class MyEntityBase
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Kayıt Tarihi") , Required]
        public DateTime CreateOn { get; set; }
        [DisplayName("Güncelleme Tarihi"), Required]
        public DateTime ModifiedOn { get; set; }
        [DisplayName("Güncelleyen"), Required,StringLength(50)]
        public string ModifiedUsurname { get; set; }
    }
}
