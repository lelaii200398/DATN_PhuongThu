using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteLaptop.Models;

namespace WebsiteBanDoGiaDung.Controllers
{
    public class ModuleController : Controller
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();
        // GET: Module
        public ActionResult Categories()
        {
            return View("_Categories", db.Category.Where(m=> m.Status == 1).ToList());
        }
        public ActionResult SlideShow()
        {
            // Position = 1 = SlideShow
            return View("_SlideShow", db.Slider.Where(m => m.Status == 1 && m.Position == "1").ToList());
        }
        public ActionResult Header()
        {
            ViewBag.Promotion = db.Post.Where(m => m.Status == 1 && m.Type == "post" && m.TopicID == 22).OrderByDescending(m => m.Created_at).Take(3).ToList();
            var list = db.Category.Where(m => m.Status == 1).ToList();
            return View("_Header", list);
        }
        public ActionResult Footer()
        {
            //ViewBag.Title = db.Menu.Where(m => m.Status == 1 && m.Positon == "footer" && m.ParentID == 0).Take(2).ToList();
            return View("_Footer", db.Menu.Where(m => m.Status == 1 && m.Positon == "footer").ToList());
        }
        public ActionResult HomeSlideShow()
        {
            ViewBag.Slider = db.Slider.Where(m => m.Status == 1 && m.Position == "2").OrderByDescending(m => m.Created_at).Take(2).ToList();
            return View("_HomeSlideShow", db.Post.Where(m => m.Status == 1 && m.Type == "post" && m.Position == "slider").ToList());
        }
        public ActionResult Popu()
        {
            return View("_Popu");
        }
        public ActionResult ListCategory()
        {
            var list = db.Category.Where(m => m.Status == 1 && m.ParentID == 0).ToList();
            return View("_ListCategory", list);
        }
        public ActionResult NewsHome()
        {
            return View("_NewsHome", db.Post.Where(m => m.Status == 1 && m.Type == "post" && m.Position == "default").OrderByDescending(m => m.Created_at).Take(5).ToList());
        }
        public ActionResult Login()
        {
            return View("_Login");
        }
        public ActionResult MainMenu()
        {
            return View("_MainMenu", db.Menu.Where(m => m.Status == 1 && m.Positon == "header").ToList());
        }
        // partial page load with ajax
        public ActionResult MiTC()
        {
            return View("_MiTC");
        }
        public ActionResult ICart()
        {
            return View("_ICart");
        }
        public ActionResult ListPage()
        {
           
            return View("_ListPage", db.Post.Where(m => m.Status == 1 && m.Type == "page").ToList());
        }
    }
}