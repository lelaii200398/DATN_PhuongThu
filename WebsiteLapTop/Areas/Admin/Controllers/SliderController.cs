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
using WebsiteLaptop.Models;
using WebsiteLapTop.Library;

namespace WebsiteLapTop.Areas.Admin.Controllers
{
    public class SliderController : BaseController
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();

        public ActionResult Index()
        {
            ViewBag.countTrash = db.Slider.Where(m => m.Status == 0).Count();
            return View(db.Slider.Where(m => m.Status != 0).ToList());
        }
        public ActionResult Trash()
        {
            return View(db.Slider.Where(m => m.Status == 0).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            MSlider mSlider = db.Slider.Find(id);
            if (mSlider == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(mSlider);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MSlider mSlider)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mSlider.Name);
                mSlider.Url = strSlug;
                mSlider.Created_at = DateTime.Now;
                mSlider.Created_by = int.Parse(Session["Admin_ID"].ToString());
                mSlider.Updated_at = DateTime.Now;
                mSlider.Updated_by = int.Parse(Session["Admin_ID"].ToString());

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mSlider.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Path/Slider/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Slider.Add(mSlider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mSlider);

        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            MSlider mSlider = db.Slider.Find(id);
            if (mSlider == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(mSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MSlider mSlider)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mSlider.Name);

                mSlider.Updated_at = DateTime.Now;
                mSlider.Updated_by = int.Parse(Session["Admin_ID"].ToString());

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mSlider.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Content/Library/images/Slider/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Entry(mSlider).State = EntityState.Modified;
                db.SaveChanges();
                Thongbao.set_flash("Cập nhập thông tin slider thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(mSlider);
        }

        // GET: Admin/Slider/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MSlider mSlider = db.Slider.Find(id);
            if (mSlider == null)
            {
                return HttpNotFound();
            }
            return View(mSlider);
        }

        // POST: Admin/Slider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MSlider mSlider = db.Slider.Find(id);
            db.Slider.Remove(mSlider);
            db.SaveChanges();
            Thongbao.set_flash("Đã xóa vĩnh viễn slider!", "success");
            return RedirectToAction("Trash");
        }

        public ActionResult DelTrash(int id)
        {
            MSlider mSlider = db.Slider.Find(id);
            if (mSlider == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }

            mSlider.Status = 0;
            mSlider.Updated_at = DateTime.Now;
            mSlider.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mSlider).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }

        public ActionResult ReTrash(int? id)
        {
            MSlider mSlider = db.Slider.Find(id);
            if (mSlider == null)
            {
                Thongbao.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Trash", "Slider");
            }
            mSlider.Status = 2;

            mSlider.Updated_at = DateTime.Now;
            mSlider.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mSlider).State = EntityState.Modified;
            db.SaveChanges();
            Thongbao.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "Slider");
        }
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            MSlider mSlider = db.Slider.Find(id);
            mSlider.Status = (mSlider.Status == 1) ? 2 : 1;

            mSlider.Updated_at = DateTime.Now;
            mSlider.Updated_by = 1;
            db.Entry(mSlider).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = mSlider.Status });
        }
    }
}
