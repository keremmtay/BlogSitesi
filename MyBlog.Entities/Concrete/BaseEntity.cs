using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Entities.Concrete
{
    public class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Oluşturulma Tarihi")]
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Güncelleme Tarihi")]
        [ScaffoldColumn(false)]
        public DateTime ModifiedDate { get; set; }
        [Required, StringLength(25) ,DisplayName("Güncelleyen Kullanıcı")]
        [ScaffoldColumn(false)]
        public string ModifiedUserName { get; set; }
    }
}
