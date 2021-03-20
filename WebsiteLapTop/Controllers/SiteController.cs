using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteLaptop.Models;
using WebsiteLapTop;

namespace WebsiteBanDoGiaDung.Controllers
{
    public class SiteController : Controller
    {
        private WebsiteLaptopDbContext db = new WebsiteLaptopDbContext();
        // GET: Site
        public ActionResult Index(String slug = "")
        {
            int pageNumber = 1;
            Session["keywords"] = null;
            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                pageNumber = int.Parse(Request.QueryString["page"].ToString());
            }
            if (slug == "")
            {
                return this.Home();
            }
            else if (slug == "account")
            {
                return Redirect("~/");
            }
            else
            {
                var link = db.Link.Where(m => m.Slug == slug);
                if (link.Count() > 0)
                {
                    var res = link.First();
                    if (res.Type == "page")
                    {
                        return this.PostPage(slug);
                    }
                    else if (res.Type == "topic")
                    {
                        return this.PostTopic(slug, pageNumber);
                    }
                    else if (res.Type == "category")
                    {
                        return this.ProductCategory(slug, pageNumber);
                    }
                }
                else
                {
                    if (db.Product.Where(m => m.Slug == slug && m.Status == 1).Count() > 0)
                    {
                        return this.ProductDetail(slug);
                    }
                    else if (db.Post.Where(m => m.Slug == slug && m.Type == "post" && m.Status == 1).Count() > 0)
                    {
                        return this.PostDetail(slug);
                    }
                }
                return this.Error(slug);
            }
        }
        public ActionResult Home()
        {
            ViewBag.NewProduct = db.Product.Where(m => m.Status == 1).OrderByDescending(m => m.Created_at).ToList();
            ViewBag.PromotionProduct = db.Product.Where(m => m.Status == 1 && m.Discount == 1).OrderByDescending(m => m.Created_at).ToList();
            return View("Home", db.Category.Where(m => m.Status == 1 && m.ParentID != 0).ToList());
        }
        public ActionResult Error(String slug)
        {
            return View("Error");
        }
        /// <summary>
        /// Post, Page
        /// </summary>
        public ActionResult PostPage(String slug)
        {
            return View("PostPage", db.Post.Where(m => m.Slug == slug && m.Status == 1 && m.Type == "page").First());
        }
        public ActionResult Post(int? page)
        {
            ViewBag.S_News = db.Post.Where(m => m.Position == "slider" && m.Status == 1 && m.Type == "post").OrderByDescending(m => m.Created_at).Take(5).ToList();
            ViewBag.TopicName = db.Topic.Where(m => m.Status == 1).ToList();
            ViewBag.Right_News = db.Post.Where(m => m.Status == 1 && m.Type == "post" && m.Position == "default").OrderByDescending(m => m.Created_at).Take(7).ToList();
            var post = db.Post.Where(m => m.Status == 1 && m.Type == "post" && m.Position != "slider").OrderByDescending(m => m.Created_at).ToPagedList(page ?? 1, 2);

            return View(post);
        }
        public ActionResult PostTopic(String slug, int? page)
        {
            var Topic_ID = db.Topic.Where(m => m.Slug == slug).Select(m => m.ID).First();

            ViewBag.S_News = db.Post.Where(m => m.Position == "slider" && m.Status == 1 && m.Type == "post" && m.TopicID == Topic_ID).OrderByDescending(m => m.Created_at).Take(5).ToList();
            ViewBag.TopicName = db.Topic.Where(m => m.Status == 1).ToList();
            ViewBag.breadcrumb = db.Topic.Where(m => m.ID == Topic_ID).First();
            ViewBag.Right_News = db.Post.Where(m => m.Status == 1 && m.Type == "post" && m.TopicID == Topic_ID).OrderByDescending(m => m.Created_at).Take(7).ToList();
            var post = db.Post.Where(m => m.Status == 1 && m.Type == "post" && m.Position != "slider" && m.TopicID == Topic_ID).OrderByDescending(m => m.Created_at).ToPagedList(page ?? 1, 2);
            ViewBag.Slug = slug;
            return View("PostTopic", post);
        }
        public ActionResult PostDetail(String slug)
        {
            var postDetail = db.Post.Where(m => m.Slug == slug && m.Status == 1 && m.Type == "post").First();
            ViewBag.TopicName = db.Topic.Where(m => m.Status == 1).OrderByDescending(m=>m.Created_at).ToList();
            ViewBag.S_News = db.Post.Where(m => m.Status == 1 && m.Type == "post").OrderByDescending(m => m.Created_at).Take(7).ToList();
            ViewBag.listOther = db.Post.Where(m => m.Status == 1 && m.TopicID == postDetail.TopicID && m.ID != postDetail.ID && m.Position == "default").OrderByDescending(m => m.Created_at).ToList();
            ViewBag.breadcrumb = db.Topic.Where(m => m.ID == postDetail.TopicID).First();

            return View("PostDetail", postDetail);
        }

        /// <summary>
        /// Product
        /// </summary>
        public ActionResult ProductHome(int catid)
        {
            List<int> listcatid = new List<int>();
            listcatid.Add(catid);

            var list2 = db.Category.Where(m => m.ParentID == catid).Select(m => m.ID).ToList();
            foreach (var id2 in list2)
            {
                listcatid.Add(id2);
                var list3 = db.Category.Where(m => m.ParentID == id2).Select(m => m.ID).ToList();
                foreach (var id3 in list3)
                {
                    listcatid.Add(id3);
                }
            }

            var list = db.Product.Where(m => m.Status == 1 && listcatid.Contains(m.CateID)).Take(12).OrderByDescending(m => m.Created_at);
            return View("_ProductHome", list);
        }
        public ActionResult Product(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var list = db.Product.Where(m => m.Status == 1).OrderByDescending(m => m.Created_at).ToPagedList(pageNumber, pageSize);

            return View(list);
        }
        public ActionResult ProductCategory(String slug, int pageNumber)
        {
            int pageSize = 8;
            var row_cat = db.Category.Where(m => m.Slug == slug).First();
            List<int> listcatid = new List<int>();
            listcatid.Add(row_cat.ID);

            var list2 = db.Category.Where(m => m.ParentID == row_cat.ID).Select(m => m.ID).ToList();
            foreach (var id2 in list2)
            {
                listcatid.Add(id2);
                var list3 = db.Category.Where(m => m.ParentID == id2).Select(m => m.ID).ToList();
                foreach (var id3 in list3)
                {
                    listcatid.Add(id3);
                }
            }

            var list = db.Product.Where(m => m.Status == 1 && listcatid.Contains(m.CateID)).OrderByDescending(m => m.Created_at);

            ViewBag.CountingTheProduct = list.Count();
            ViewBag.Slug = slug;
            ViewBag.CatName = row_cat.Name;
            return View("ProductCategory", list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ProductDetail(String slug)
        {
            var getP = db.Product.Where(m => m.Slug == slug && m.Status == 1).First();




            ViewBag.listOther = db.Product.Where(m => m.Status == 1 && m.CateID == getP.CateID && m.ID != getP.ID).OrderByDescending(m => m.Created_at).ToList();

            return View("ProductDetail", getP);
        }

        public ActionResult Search(String key, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var list = db.Product.Where(m => m.Status == 1);
            if (String.IsNullOrEmpty(key.Trim()))
            {
                return RedirectToAction("Index", "Site");
            }
            else
            {
                list = list.Where(m => m.Name.Contains(key)).OrderByDescending(m => m.Created_at);
            }
            ViewBag.Count = list.Count();
            Session["keywords"] = key;
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SubmitContact(MContact mContact)
        {
            mContact.Fullname = Request.Form["Fullname"];
            mContact.Email = Request.Form["Email"];
            mContact.Phone = Convert.ToInt32(Request.Form["Phone"]);
            mContact.Title = Request.Form["Title"];
            mContact.Detail = Request.Form["Detail"];
            mContact.Status = 1;
            mContact.Created_at = DateTime.Now;
            mContact.Updated_at = DateTime.Now;
            mContact.Updated_by = 1;
            
            db.Contact.Add(mContact);
            db.SaveChanges();
            Thongbao.set_flash("Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. Xin cảm ơn!", "success");
            return RedirectToAction("Contact", "Site");
        }
    }
}