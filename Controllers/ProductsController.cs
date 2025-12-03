using DoAn_WebBanCayCanh_24DKTPM1A.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity; 
namespace DoAn_WebBanCayCanh_24DKTPM1A.Controllers
{
    public class ProductsController : Controller
    {
        CoiGardenDBEntities db = new CoiGardenDBEntities();
        // GET: Products     
        public ActionResult Tera(int? page, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            ViewBag.ShowCarousel = "True";
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            // Nhóm ID Terrarium (1,2,3,4) do đã set trong sql
            var listIds = new List<int?> { 1, 2, 3, 4 };

            // Khởi tạo truy vấn
            var query = db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(n => listIds.Contains(n.CategoryId));

            // lọc danh mục sản phẩm
            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId);
                ViewBag.CurrentCate = categoryId; // Lưu lại để dùng cho phân trang
            }

            // lọc giá sản phẩm
            if (minPrice.HasValue)
            {
                query = query.Where(n => n.Price >= minPrice);
                ViewBag.MinPrice = minPrice;
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(n => n.Price <= maxPrice);
                ViewBag.MaxPrice = maxPrice;
            }

            // Sắp xếp & Phân trang
            var result = query.OrderByDescending(n => n.CreatedDate).ToPagedList(pageNumber, pageSize);

            // Trả về Partial View nếu là AJAX dùng ajax 
            if (Request.IsAjaxRequest())
            {
                return PartialView("_TeraGrid", result);
            }

            return View(result);
        }
        // xem danh sách
        public ActionResult XemDanhSach(int id)
        {
            // Chuyển hướng về Tera và truyền tham số categoryId
            return RedirectToAction("Tera", new { categoryId = id });
        }
        // Chuyển hướng về Tera và truyền tham số categoryId
        public ActionResult Aqua(int? page, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            ViewBag.ShowCarousel = "True";
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            // Danh sách ID thuộc nhóm Aqua (Thủy sinh) do đã set trong sql
            var listIds = new List<int?> { 5, 6, 7, 4 };
            var query = db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(n => listIds.Contains(n.CategoryId));

            // Lọc theo Danh mục con
            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId);
                ViewBag.CurrentCate = categoryId;
            }

            // Lọc theo Giá
            if (minPrice.HasValue)
            {
                query = query.Where(n => n.Price >= minPrice);
                ViewBag.MinPrice = minPrice;
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(n => n.Price <= maxPrice);
                ViewBag.MaxPrice = maxPrice;
            }

            // Sắp xếp & Phân trang
            var result = query.OrderByDescending(n => n.CreatedDate).ToPagedList(pageNumber, pageSize);

            // Trả về Partial View nếu là Ajax
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AquaGrid", result);
            }

            return View(result);
        }
        // phần chi tiết sản phẩm khi bấm vào sản phẩm sẽ hiện lên trang để biết thông tin và đánh giá
        public ActionResult Single(int id)
        {
            ViewBag.ShowCarousel = "False";

            // Tìm sản phẩm theo ID // tất cả thông tin chỉ là fake-data để làm cho trang web thêm sinh động
            var sanpham = db.Products
                .Include(p => p.Reviews) // Tải luôn đánh giá
                .Include(p => p.Category) // Tải luôn danh mục
                .FirstOrDefault(s => s.Id == id);

            if (sanpham == null)
            {
                return HttpNotFound();
            }

            // Tăng lượt xem
            sanpham.LuotXem = (sanpham.LuotXem ?? 0) + 1;
            db.SaveChanges();

            // Lấy sản phẩm liên quan
            var sanphamLienQuan = db.Products
                .Where(s => s.CategoryId == sanpham.CategoryId && s.Id != sanpham.Id)
                .Take(4)
                .ToList();
            ViewBag.SanphamLienQuan = sanphamLienQuan;

            var topBanChay = db.Products
                .OrderByDescending(s => s.DaBan)
                .Take(5)
                .ToList();
            ViewBag.TopBanChay = topBanChay;

            return View(sanpham);
        }
        //  phần tìm kiếm sản phẩm
        public ActionResult Search(string keyword, int? page)
        {
            ViewBag.ShowCarousel = "False"; // Ẩn banner chạy
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            // Lưu từ khóa lại để hiện ở ô input
            ViewBag.Keyword = keyword;

            // tìm kiếm (Theo tên sản phẩm)
            var query = db.Products.Include(n => n.Category).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm gần đúng (chứa từ khóa)
                query = query.Where(n => n.Name.Contains(keyword));
            }

            // Sắp xếp và phân trang
            var result = query.OrderByDescending(n => n.CreatedDate).ToPagedList(pageNumber, pageSize);

            // Trả về View tìm kiếm
            return View("Search", result);
        }
    }
}