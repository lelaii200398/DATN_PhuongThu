﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteLaptop;
using WebsiteLaptop.Library;
using WebsiteLaptop.Models;

namespace WebsiteLapTop.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();
        public ActionResult Index()
        {
            ViewBag.countTrash = db.User.Where(m => m.Status == 0).Count();
            return View(db.User.Where(m => m.Status != 0).ToList());
        }
        public ActionResult Trash()
        {
            return View(db.User.Where(m => m.Status == 0).ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MUser mUser)
        {
            if (ModelState.IsValid)
            {
                String avatar = XString.ToAscii(mUser.Fullname);
                mUser.Password = XString.ToMD5(mUser.Password);
                mUser.Created_at = DateTime.Now;
                mUser.Created_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
                mUser.Updated_at = DateTime.Now;
                mUser.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = avatar + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mUser.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Library/images/user"), filename);
                    file.SaveAs(Strpath);
                }

                db.User.Add(mUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mUser);
        }


        public ActionResult DelTrash(int id)
        {
            MUser mUser = db.User.Find(id);
            if (mUser == null)
            {
                Thongbao.set_flash("Không tồn tại User trên!", "warning");
                return RedirectToAction("Index");
            }

            mUser.Status = 0;

            mUser.Created_at = DateTime.Now;
            mUser.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            mUser.Updated_at = DateTime.Now;
            mUser.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(mUser).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Đã xóa vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }

        // Delete to trash
        public ActionResult ReTrash(int? id)
        {
            MUser mUser = db.User.Find(id);
            if (mUser == null)
            {
                Thongbao.set_flash("Không tồn tại User!", "warning");
                return RedirectToAction("Trash", "User");
            }
            mUser.Status = 2;

            mUser.Updated_at = DateTime.Now;
            mUser.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(mUser).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "User");
        }

        // GET: Admin/User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUser mUser = db.User.Find(id);
            if (mUser == null)
            {
                return HttpNotFound();
            }
            return View(mUser);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUser mUser = db.User.Find(id);
            if (mUser == null)
            {
                return HttpNotFound();
            }
            return View(mUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MUser mUser)
        {
            if (ModelState.IsValid)
            {

                String avatar = XString.ToAscii(mUser.Fullname);
                mUser.Password = XString.ToMD5(mUser.Password);
                mUser.Created_at = DateTime.Now;
                mUser.Created_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
                mUser.Updated_at = DateTime.Now;
                mUser.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = avatar + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mUser.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Library/images/user"), filename);
                    file.SaveAs(Strpath);
                }
                db.Entry(mUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mUser);
        }

        // GET: Admin/User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUser mUser = db.User.Find(id);
            if (mUser == null)
            {
                return HttpNotFound();
            }
            return View(mUser);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MUser mUser = db.User.Find(id);
            db.User.Remove(mUser);
            db.SaveChanges();
            Thongbao.set_flash("Đã xóa hoàn User!", "success");
            return RedirectToAction("Trash");
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            MUser mUser = db.User.Find(id);
            mUser.Status = (mUser.Status == 1) ? 2 : 1;

            mUser.Updated_at = DateTime.Now;
            mUser.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(mUser).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Status = mUser.Status
            });
        }
    }
}
