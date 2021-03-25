using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteLaptop.Models;
using WebsiteLapTop;
using WebsiteLapTop.Library;

namespace WebsiteLaptop.Controllers
{
    public class AccountController : Controller
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();
        //public AccountController()
        //{
        //    if (System.Web.HttpContext.Current.Session["User_Name"] == null)
        //    {
        //        System.Web.HttpContext.Current.Response.Redirect("~/");
        //    }
        //}

        [HttpPost]
        public JsonResult UserLogin(String User, String Password)
        {
            int count_username = db.User.Where(m => m.Status == 1 && ((m.Phone).ToString() == User || m.Email == User || m.Name == User ) && m.Access == 0).Count();
            if (count_username == 0)
            {

                return Json(new { s = 1 });
            }
            else
            {
                Password = XString.ToMD5(Password);
                //Password = Password;
                var user_acount = db.User.Where(m => m.Status == 1 && ((m.Phone).ToString() == User || m.Email == User || m.Name == User) && m.Password == Password);
                if (user_acount.Count() == 0)
                {
                    return Json(new { s = 2 });
                }
                else
                {
                    var user = user_acount.First();
                    Session["User_Name"] = user.Fullname;
                    Session["User_ID"] = user.ID;
                }
            }
            return Json(new { s = 0 });
        }

        public ActionResult UserLogout(String url)
        {
            Session["User_Name"] = null;
            Session["User_ID"] = null;
            return Redirect("~/" + url);
        }
        public ActionResult ProFile()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        public ActionResult Notification()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        public ActionResult Order()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
                {
                       System.Web.HttpContext.Current.Response.Redirect("~/");
                }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var list = db.Order.Where(m => m.UserID == userid).OrderByDescending(m => m.CreateDate).ToList();
            ViewBag.itemOrder = db.OrderDetail.ToList();
            ViewBag.productOrder = db.Product.ToList();
            return View(list);
        }
        public ActionResult ActionOrder()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            var list = db.Order.ToList();
            ViewBag.Hoanthanh = db.Order.Where(m => m.Status == 3).Count();
            ViewBag.ChoXuLy = db.Order.Where(m => m.Status == 1).Count();
            ViewBag.DangXuLy = db.Order.Where(m => m.Status == 2).Count();
            return View("_ActionOrder", list);
        }
        public ActionResult OrderDetails(int id)
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var checkO = db.Order.Where(m => m.UserID == userid && m.ID == id);
            if (checkO.Count() == 0)
            {
                return this.NotFound();
            }

            var id_order = db.Order.Where(m => m.UserID == userid && m.ID == id).FirstOrDefault();
            ViewBag.Order = id_order;
            var itemOrder = db.OrderDetail.Where(m => m.OrderID == id_order.ID).ToList();
            ViewBag.productOrder = db.Product.ToList();
            return View(itemOrder);
        }
        public ActionResult NotFound()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }

        //[HttpPost]
        //public JsonResult Register(MUser user)
        //{
        //    try
        //    {
        //        var checkPM = db.User.Any(m => m.Phone == user.Phone && m.Email.ToLower().Equals(user.Email.ToLower()));
        //        if (checkPM)
        //        {
        //            return Json(new { Code = 1, Message = "Số điện thoại hoặc Email đã được sử dụng." });
        //        }
        //        user.Gender = "1";
        //        user.Image = "";
        //        user.Access = 0;
        //        user.Status = 1;
        //        user.Password = XString.ToMD5(user.Password);
        //        user.Created_at = DateTime.Now;
        //        user.Created_by = 1;
        //        user.Updated_at = DateTime.Now;
        //        user.Updated_by = 1;

        //        db.User.Add(user);
        //        db.SaveChanges();

        //        return Json(new { Code = 0, Message = "Đăng ký thành công!" });
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { Code = 1, Message = "Đăng ký thất bại!" });
        //        throw e;
        //    }
        //}

        public ActionResult Registers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registers(MUser muser, FormCollection fc)
        {
            string uname = fc["uname"];
            string fname = fc["fname"];
            string Pass = XString.ToMD5(fc["psw"]);
            string Pass2 = XString.ToMD5(fc["repsw"]);
            if (Pass2 != Pass)
            {
                ViewBag.error = "Mật khẩu không khớp";
                return View("registers");
            }
            string email = fc["email"];
            string address = fc["address"];
            string phone = fc["phone"];
            if (ModelState.IsValid)
            {
                var Luser = db.User.Where(m => m.Status == 1 && m.Name == uname && m.Access == 0);
                if (Luser.Count() > 0)
                {
                    ViewBag.error = "Tên Đăng Nhập đã tồn tại. Xin vui lòng thử lại!";
                    return View("registers");
                }
                var Uuser = db.User.Where(m => m.Status == 1 && m.Phone == phone && m.Access == 0);
                if (Uuser.Count() > 0)
                {
                    ViewBag.Error = "Số điện thoại đã được sử dụng đăng ký. Xin vui lòng thử lại!";
                    return View("registers");
                }
                var Auser = db.User.Where(m => m.Status == 1 && m.Email == email && m.Access == 0);
                if (Uuser.Count() > 0)
                {
                    ViewBag.Error = "Email đã được sử dụng đăng ký. Xin vui lòng thử lại!";
                    return View("registers");
                }
                else
                {
                    muser.Image = "defalt.png";
                    muser.Password = Pass;
                    muser.Name = uname;
                    muser.Fullname = fname;
                    muser.Email = email;
                    muser.Address = address;
                    muser.Phone = phone;
                    muser.Gender = "nam";
                    muser.Access = 0;
                    muser.Created_at = DateTime.Now;
                    muser.Updated_at = DateTime.Now;
                    muser.Created_by = 1;
                    muser.Updated_by = 1;
                    muser.Status = 1;
                    db.User.Add(muser);
                    db.SaveChanges();
                    Thongbao.set_flash("Đăng ký tài khoản thành công ", "success");
                    return Redirect("~/dang-ky");
                }

            }
            Thongbao.set_flash("Đăng ký tài khoản thất bại", "danger");
            return Redirect("~/dang-ky");
        }
        
    }
}