using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using DoAn_WebBanCayCanh_24DKTPM1A.Models;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private CoiGardenDBEntities db = new CoiGardenDBEntities();

        // GET: Admin/Products
        public ActionResult Index(int? page)
        {
            // 1. Quy định số lượng sản phẩm trên mỗi trang
            int pageSize = 5;

            // 2. Xác định trang hiện tại (nếu null thì mặc định là trang 1)
            int pageNumber = (page ?? 1);

            // 3. Lấy dữ liệu và MỚI NHẤT lên đầu (Bắt buộc phải có OrderBy)
            var products = db.Products
                             .Include(p => p.Category)
                             .OrderByDescending(x => x.Id); // Sắp xếp theo ID giảm dần

            // 4. Trả về dạng PagedList
            return View(products.ToPagedList(pageNumber, pageSize));
        }
        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }
        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
 // QUAN TRỌNG: Phải có tham số 'HttpPostedFileBase ImageFile'
       public ActionResult Create(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // 1. Xử lý lưu ảnh
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Lấy tên file
                    string fileName = System.IO.Path.GetFileName(ImageFile.FileName);

                    // Tạo đường dẫn lưu vào thư mục ~/images/
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/"), fileName);

                    // Lưu file vật lý
                    ImageFile.SaveAs(path);

                    // Lưu tên file vào object Product để đẩy xuống DB
                    product.Image = fileName;
                }
                else
                {
                    product.Image = ""; // Hoặc gán ảnh mặc định nếu muốn
                }
                // 2. Tự động gán các thông tin Admin không nhập
                product.CreatedDate = DateTime.Now;
                product.DaBan = 0;
                product.LuotXem = 0;

                // 3. Lưu vào Database
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // 1. Lấy dữ liệu cũ trong DB ra trước
                var productInDb = db.Products.Find(product.Id);
                if (productInDb != null)
                {
                    // 2. Cập nhật các thông tin Text
                    productInDb.Name = product.Name;
                    productInDb.Price = product.Price;
                    productInDb.Description = product.Description;
                    productInDb.CategoryId = product.CategoryId;
                    productInDb.Quantity = product.Quantity;
                    // ... cập nhật nốt các trường khác nếu cần

                    // 3. XỬ LÝ ẢNH: Chỉ thay đổi nếu có upload ảnh mới
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        string fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("~/images/"), fileName);
                        ImageFile.SaveAs(path);
                        productInDb.Image = fileName; // Gán ảnh mới
                    }
                    // Nếu ImageFile == null thì giữ nguyên productInDb.Image cũ

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            // --- ĐOẠN CODE MỚI: XÓA FILE ẢNH ---
            if (!string.IsNullOrEmpty(product.Image))
            {
                string imagePath = Server.MapPath("~/images/" + product.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); // Xóa file vật lý khỏi ổ cứng
                }
            }
            // ------------------------------------
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}