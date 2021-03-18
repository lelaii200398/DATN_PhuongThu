namespace WebsiteLaptop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("Product")]
    public class MProduct
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn nhập tên sản phẩm")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Img { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập chi tiết sản phẩm")]
        public string Detail { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        public string Description { get; set; }
        public int Number { get; set; }
        public int Price { get; set; }
        public int Price_sale { get; set; }

        public string Metakey { get; set; }

        public string Metadesc { get; set; }

        public DateTime? Created_at { get; set; }

        public int? Created_by { get; set; }

        public DateTime? Updated_at { get; set; }

        public int? Updated_by { get; set; }

        public int Status { get; set; }

        public int Discount { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int CatID { get; set; }
        //public int CategoryId { set; get; }
        //[ForeignKey("CategoryId")]
        //public virtual MCategory Category { get; set; }
    }
}