using System;
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
    public class PostController : Controller
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();

        public ActionResult Index()
        {
            ViewBag.countTrash = db.Post.Where(m => m.Status == 0 && m.Type == "post").Count();
            var list = from p in db.Post
                       join t in db.Topic
                       on p.TopicID equals t.ID
                       where p.Status != 0
                       orderby p.Created_at descending
                       select new PostTopic()
                       {
                           PostId = p.ID,
                           PostImg = p.Image,
                           PostName = p.Title,
                           PostStatus = p.Status,
                           TopicName = t.Name
                       };
            return View(list.ToList());
        }
        public ActionResult Trash()
        {
            var list = from p in db.Post
                       join t in db.Topic
                       on p.TopicID equals t.ID
                       where p.Status == 0
                       orderby p.Created_at descending
                       select new PostTopic()
                       {
                           PostId = p.ID,
                           PostImg = p.Image,
                           PostName = p.Title,
                           PostStatus = p.Status,
                           TopicName = t.Name
                       };
            return View(list.ToList());
        }
        // Create
        public ActionResult Create()
        {
            MTopic mTopic = new MTopic();
            ViewBag.ListTopic = new SelectList(db.Topic.ToList(), "ID", "Name", 0);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MPost mPost)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mPost.Title);
                mPost.Slug = strSlug;
                mPost.Type = "post";
                mPost.Created_at = DateTime.Now;
                mPost.Created_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
                mPost.Updated_at = DateTime.Now;
                mPost.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mPost.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Library/images/post/"), filename);
                    file.SaveAs(Strpath);
                }
                db.Post.Add(mPost);
                db.SaveChanges();
                Thongbao.set_flash("Đã thêm bài viết mới!", "success");
                return RedirectToAction("Index");
            }
            return View(mPost);
        }
        // Edit
        public ActionResult Edit(int? id)
        {
            MTopic mTopic = new MTopic();
            ViewBag.ListTopic = new SelectList(db.Topic.ToList(), "ID", "Name", 0);
            MPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Thongbao.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            return View(mPost);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MPost mPost)
        {
            MTopic mTopic = new MTopic();
            ViewBag.ListTopic = new SelectList(db.Topic.ToList(), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mPost.Title);
                mPost.Slug = strSlug;
                mPost.Type = "post";
                mPost.Updated_at = DateTime.Now;
                mPost.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mPost.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Library/images/post/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Entry(mPost).State = EntityState.Modified;
                db.SaveChanges();
                Thongbao.set_flash("Đã cập nhật lại bài viết!", "success");
                return RedirectToAction("Index");
            }
            return View(mPost);
        }
        public ActionResult DelTrash(int? id)
        {
            MPost mPost = db.Post.Find(id);
            mPost.Status = 0;

            mPost.Updated_at = DateTime.Now;
            mPost.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(mPost).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Đã chuyển vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        public ActionResult Undo(int? id)
        {
            MPost mPost = db.Post.Find(id);
            mPost.Status = 2;

            mPost.Updated_at = DateTime.Now;
            mPost.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(mPost).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            MPost mPost = db.Post.Find(id);
            mPost.Status = (mPost.Status == 1) ? 2 : 1;

            mPost.Updated_at = DateTime.Now;
            mPost.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(mPost).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = mPost.Status });
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            MPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Thongbao.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            return View(mPost);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            MPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Thongbao.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            return View(mPost);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MPost mPost = db.Post.Find(id);
            db.Post.Remove(mPost);
            db.SaveChanges();
            Thongbao.set_flash("Đã xóa vĩnh viễn", "danger");
            return RedirectToAction("Trash");
        }

    }
}
