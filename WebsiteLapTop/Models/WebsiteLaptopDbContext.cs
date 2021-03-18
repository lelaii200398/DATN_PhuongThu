using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebsiteLaptop.Models
{
    public class WebsiteLaptopDbContext : DbContext
    {
        public WebsiteLaptopDbContext() : base("name=Connect")
        { }
        public virtual DbSet<MProduct> Product { get; set; }
        public virtual DbSet<MCategory> Category { get; set; }
        public virtual DbSet<MLink> Link { get; set; }
        public virtual DbSet<MTopic> Topic { get; set; }
        public virtual DbSet<MPost> Post { get; set; }
        public virtual DbSet<MContact> Contact { get; set; }
        public virtual DbSet<MMenu> Menu { get; set; }
        public virtual DbSet<MOrder> Order { get; set; }
        public virtual DbSet<MOrderDetail> OrderDetail { get; set; }
        public virtual DbSet<MSlider> Slider { get; set; }
        public virtual DbSet<MUser> User { get; set; }
    }
}