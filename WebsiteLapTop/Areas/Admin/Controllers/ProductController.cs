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
    public class ProductController : Controller
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();

        // GET: Admin/Product
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Product.Where(m => m.Status == 0).Count();
            var list = from p in db.Product
                       join c in db.Category
                       on p.CatID equals c.Id
                       where p.Status != 0
                       where p.CatID == c.Id
                       orderby p.Created_at descending
                       select new ProductCategory()
                       {
                           ProductId = p.Id,
                           ProductImg = p.Img,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           ProductDiscount = p.Discount,
                           CategoryName = c.Name
                       };
            return View(list.ToList());
        }
        public ActionResult Trash()
        {
            var list = from p in db.Product
                       join c in db.Category
                       on p.CatID equals c.Id
                       where p.Status == 0
                       where p.CatID == c.Id
                       orderby p.Created_at descending
                       select new ProductCategory()
                       {
                           ProductId = p.Id,
                           ProductImg = p.Img,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           ProductDiscount = p.Discount,
                           CategoryName = c.Name
                       };
            return View(list.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            MProduct mProduct = db.Product.Find(id);
            if (mProduct == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(mProduct);
        }

        public ActionResult Create()
        {
            MCategory mCategory = new MCategory();
            ViewBag.ListCat = new SelectList(db.Category.Where(m => m.Status != 0), "ID", "Name", 0);
            //ViewBag.ListCat = new SelectList(db.Category.ToList(), "ID", "Name", 0);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MProduct mProduct)
        {
            ViewBag.ListCat = new SelectList(db.Category.Where(m => m.Status != 0), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                mProduct.Price = mProduct.Price;
                mProduct.Price_sale = mProduct.Price_sale;

                String strSlug = XString.ToAscii(mProduct.Name);
                mProduct.Slug = strSlug;
                mProduct.Created_at = DateTime.Now;
                mProduct.Created_by = 1;
                mProduct.Updated_at = DateTime.Now;
                mProduct.Updated_by = 1;

                // Upload file
                var file = Request.Files["Img"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mProduct.Img = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Library/images/products/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Product.Add(mProduct);
                db.SaveChanges();
                Thongbao.set_flash("Thêm mới sản phẩm thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(mProduct);
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.ListCat = new SelectList(db.Category.ToList(), "ID", "Name", 0);
            MProduct mProduct = db.Product.Find(id);
            if (mProduct == null)
            {
                Thongbao.set_flash("404!", "warning");
                return RedirectToAction("Index", "Product");
            }
            return View(mProduct);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MProduct mProduct)
        {
            ViewBag.ListCat = new SelectList(db.Category.ToList(), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mProduct.Name);
                mProduct.Slug = strSlug;

                mProduct.Updated_at = DateTime.Now;
                mProduct.Updated_by = 1;

                // Upload file
                var file = Request.Files["Img"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mProduct.Img = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Library/images/products/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Entry(mProduct).State = EntityState.Modified;
                db.SaveChanges();
                Thongbao.set_flash("Đã cập nhật lại thông tin sản phẩm!", "success");
                return RedirectToAction("Index");
            }
            return View(mProduct);
        }

        public ActionResult DelTrash(int? id)
        {
            MProduct mProduct = db.Product.Find(id);
            mProduct.Status = 0;

            mProduct.Updated_at = DateTime.Now;
            mProduct.Updated_by = 1;
            db.Entry(mProduct).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        public ActionResult Undo(int? id)
        {
            MProduct mProduct = db.Product.Find(id);
            mProduct.Status = 2;

            mProduct.Updated_at = DateTime.Now;
            mProduct.Updated_by = 1;
            db.Entry(mProduct).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại !", "warning");
                return RedirectToAction("Trash");
            }
            MProduct mProduct = db.Product.Find(id);
            if (mProduct == null)
            {
                Thongbao.set_flash("Không tồn tại !", "warning");
                return RedirectToAction("Trash");
            }
            return View(mProduct);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MProduct mProduct = db.Product.Find(id);
            db.Product.Remove(mProduct);
            db.SaveChanges();
            Thongbao.set_flash("Đã xóa vĩnh viễn sản phẩm!", "danger");
            return RedirectToAction("Trash");
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            MProduct mProduct = db.Product.Find(id);
            mProduct.Status = (mProduct.Status == 1) ? 2 : 1;

            mProduct.Updated_at = DateTime.Now;
            mProduct.Updated_by = 1;
            db.Entry(mProduct).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = mProduct.Status });
        }
        [HttpPost]
        public JsonResult changeDiscount(int id)
        {
            MProduct mProduct = db.Product.Find(id);
            mProduct.Discount = (mProduct.Discount == 1) ? 2 : 1;

            mProduct.Updated_at = DateTime.Now;
            mProduct.Updated_by = 1;
            db.Entry(mProduct).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Discount = mProduct.Discount });
        }
    }
}
