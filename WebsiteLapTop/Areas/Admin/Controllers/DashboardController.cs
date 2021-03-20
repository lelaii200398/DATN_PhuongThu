using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteLaptop.Models;

namespace WebsiteLapTop.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            ViewBag.CountOrderSuccess = db.Order.Where(m => m.Status == 3).Count();
            ViewBag.CountOrderCancel = db.Order.Where(m => m.Status == 1).Count();
            ViewBag.CountContactDoneReply = db.Contact.Where(m => m.Flag == 0).Count();
            ViewBag.CountUser = db.User.Where(m => m.Status != 0).Count();
            return View();
        }
    }
}