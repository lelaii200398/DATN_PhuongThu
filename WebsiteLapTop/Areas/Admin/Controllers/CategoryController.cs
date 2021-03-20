using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteLaptop.Models;
using WebsiteLapTop;
using WebsiteLapTop.Library;

namespace WebsiteLaptop.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();

        // GET: Admin/Category
        public ActionResult Index()
        {
            ViewBag.count_trash = db.Category.Where(m => m.Status == 0).Count();
            var list = db.Category.Where(m => m.Status != 0).ToList();
            ViewBag.GetAllCategory = list;
            foreach (var row in list)
            {
                var temp_link = db.Link.Where(m => m.Type == "category" && m.TableId == row.ID);
                if (temp_link.Count() > 0)
                {
                    var row_link = temp_link.First();
                    row_link.Name = row.Name;
                    row_link.Slug = row.Slug;
                    db.Entry(row_link).State = EntityState.Modified;
                }
                else
                {
                    var row_link = new MLink();
                    row_link.Name = row.Name;
                    row_link.Slug = row.Slug;
                    row_link.Type = "category";
                    row_link.TableId = row.ID;
                    db.Link.Add(row_link);
                }
            }
            db.SaveChanges();
            return View(list);
        }

        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            MCategory MCategory = db.Category.Find(id);
            if (MCategory == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(MCategory);
        }

        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MCategory MCategory)
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);
            if (ModelState.IsValid)
            {
                if (MCategory.ParentID == null)
                {
                    MCategory.ParentID = 0;
                }
                String Slug = XString.ToAscii(MCategory.Name);
                CheckSlug check = new CheckSlug();

                if (!check.KiemTraSlug("Category", Slug, null))
                {
                    Thongbao.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Create", "Category");
                }

                MCategory.Slug = Slug;
                MCategory.Created_at = DateTime.Now;
                MCategory.Created_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
                MCategory.Updated_at = DateTime.Now;
                MCategory.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;

                db.Category.Add(MCategory);
                db.SaveChanges();
                Thongbao.set_flash("Danh mục đã được thêm!", "success");
                return RedirectToAction("Index", "Category");
            }

            Thongbao.set_flash("Có lỗi xảy ra khi thêm danh mục!", "warning");
            return View(MCategory);
        }
        public ActionResult Edit(int? id)
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);
            MCategory MCategory = db.Category.Find(id);
            if (MCategory == null)
            {
                Thongbao.set_flash("404!", "warning");
                return RedirectToAction("Index", "Category");
            }
            return View(MCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MCategory MCategory)
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);
            if (ModelState.IsValid)
            {
                String Slug = XString.ToAscii(MCategory.Name);
                int ID = MCategory.ID;
                if (db.Category.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Thongbao.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.Topic.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Thongbao.set_flash("Tên danh mục đã tồn tại trong TOPIC, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.Post.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Thongbao.set_flash("Tên danh mục đã tồn tại trong POST, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.Product.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Thongbao.set_flash("Tên danh mục đã tồn tại trong PRODUCT, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }

                MCategory.Slug = Slug;

                MCategory.Updated_at = DateTime.Now;
                MCategory.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;

                db.Entry(MCategory).State = EntityState.Modified;
                db.SaveChanges();
                Thongbao.set_flash("Cập nhật thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(MCategory);
        }
        public ActionResult DelTrash(int id)
        {
            MCategory MCategory = db.Category.Find(id);
            if (MCategory == null)
            {
                Thongbao.set_flash("Không tồn tại danh mục cần xóa vĩnh viễn!", "warning");
                return RedirectToAction("Index");
            }
            int count_child = db.Category.Where(m => m.ParentID == id).Count();
            if (count_child != 0)
            {
                Thongbao.set_flash("Không thể xóa, danh mục có chứa danh mục con!", "warning");
                return RedirectToAction("Index");
            }
            MCategory.Status = 0;

            MCategory.Created_at = DateTime.Now;
            MCategory.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            MCategory.Updated_at = DateTime.Now;
            MCategory.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(MCategory).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        public ActionResult ReTrash(int? id)
        {
            MCategory MCategory = db.Category.Find(id);
            if (MCategory == null)
            {
                Thongbao.set_flash("Không tồn tại danh mục!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            MCategory.Status = 2;

            MCategory.Updated_at = DateTime.Now;
            MCategory.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(MCategory).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "Category");
        }
        //trash
        public ActionResult Trash()
        {
            var list = db.Category.Where(m => m.Status == 0).ToList();

            return View(list);
        }
        //doi trang thai
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            MCategory MCategory = db.Category.Find(id);
            MCategory.Status = (MCategory.Status == 1) ? 2 : 1;

            MCategory.Updated_at = DateTime.Now;
            MCategory.Updated_by = 1/*int.Parse(Session["Admin_ID"].ToString());*/;
            db.Entry(MCategory).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Status = MCategory.Status
            });
        }
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại danh mục cần xóa!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            MCategory MCategory = db.Category.Find(id);
            if (MCategory == null)
            {
                Thongbao.set_flash("Không tồn tại danh mục cần xóa!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            return View(MCategory);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MCategory MCategory = db.Category.Find(id);
            db.Category.Remove(MCategory);
            db.SaveChanges();
            Thongbao.set_flash("Đã xóa hoàn toàn danh mục!", "success");
            return RedirectToAction("Trash", "Category");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
