using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteLaptop.Models
{
    [Table("User")]
    public class MUser
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public string Fullname { get; set; }
        public string Name { get; set; }
        //[StringLength(12)]
        //[Required(ErrorMessage = "Độ dài mật khẩu phải từ 6 ký tự trở lên bao gồm 1->9 a->z @~#$#")]
        public string Password { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }//Giới tính
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public int Access { get; set; }

        public int Status { get; set; }
        public DateTime Created_at { get; set; }
        public int Created_by { get; set; }
        public DateTime Updated_at { get; set; }
        public int Updated_by { get; set; }
    }
}