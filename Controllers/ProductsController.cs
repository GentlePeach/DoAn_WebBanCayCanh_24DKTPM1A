using DoAn_WebBanCayCanh_24DKTPM1A.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity; // QUAN TRỌNG: Thêm dòng này để dùng .Include

namespace DoAn_WebBanCayCanh_24DKTPM1A.Controllers
{
    public class ProductsController : Controller
    {
        CoiGardenDBEntities db = new CoiGardenDBEntities();
        // GET: Products
        // === HÀM TERA (ĐÃ TỐI ƯU) ===
        // Đảm bảo bạn đã: using System.Data.Entity; và using PagedList;

        public ActionResult Tera(int? page, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            ViewBag.ShowCarousel = "True";
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            // 1. Nhóm ID Terrarium (1,2,3,4)
            var listIds = new List<int?> { 1, 2, 3, 4 };

            // 2. Khởi tạo truy vấn
            var query = db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(n => listIds.Contains(n.CategoryId));

            // 3. XỬ LÝ LỌC DANH MỤC
            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId);
                ViewBag.CurrentCate = categoryId; // Lưu lại để dùng cho phân trang
            }

            // 4. XỬ LÝ LỌC GIÁ
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

            // 5. Sắp xếp & Phân trang
            var result = query.OrderByDescending(n => n.CreatedDate).ToPagedList(pageNumber, pageSize);

            // 6. Trả về Partial View nếu là AJAX
            if (Request.IsAjaxRequest())
            {
                return PartialView("_TeraGrid", result);
            }

            return View(result);
        }
        // === HÀM XEM DANH SÁCH (ĐÃ SỬA) ===
        // Sửa lỗi: Thay vì trả về View riêng, ta chuyển hướng về hàm Tera để tận dụng bộ lọc
        public ActionResult XemDanhSach(int id)
        {
            // Chuyển hướng về Tera và truyền tham số categoryId
            return RedirectToAction("Tera", new { categoryId = id });
        }

        // ... (Các hàm khác giữ nguyên)

        // === HÀM AQUA (COPY TỪ TERA & SỬA ID) ===
        // Đảm bảo bạn đã có: using PagedList; và using System.Data.Entity;

        public ActionResult Aqua(int? page, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            ViewBag.ShowCarousel = "True";
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            // 1. Danh sách ID thuộc nhóm Aqua (Thủy sinh)
            // Bạn kiểm tra lại ID trong SQL nhé (Ví dụ: 5, 6, 7, 4)
            var listIds = new List<int?> { 5, 6, 7, 4 };

            // 2. Truy vấn dữ liệu (Dùng .Include để tránh lỗi lag)
            var query = db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(n => listIds.Contains(n.CategoryId));

            // 3. Lọc theo Danh mục con
            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId);
                ViewBag.CurrentCate = categoryId;
            }

            // 4. Lọc theo Giá
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

            // 5. Sắp xếp & Phân trang (QUAN TRỌNG: Phải dùng ToPagedList)
            var result = query.OrderByDescending(n => n.CreatedDate).ToPagedList(pageNumber, pageSize);

            // 6. Trả về Partial View nếu là Ajax
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AquaGrid", result);
            }

            return View(result);
        }
        public ActionResult Single(int id)
        {
            ViewBag.ShowCarousel = "False";

            // 1. Tìm sản phẩm theo ID (Kèm Review để đếm sao)
            var sanpham = db.Products
                .Include(p => p.Reviews) // Tải luôn đánh giá
                .Include(p => p.Category) // Tải luôn danh mục
                .FirstOrDefault(s => s.Id == id);

            if (sanpham == null)
            {
                return HttpNotFound();
            }

            // 2. Tăng lượt xem
            sanpham.LuotXem = (sanpham.LuotXem ?? 0) + 1;
            db.SaveChanges();

            // 3. Lấy sản phẩm liên quan
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
        // Đừng quên using PagedList;

        public ActionResult Search(string keyword, int? page)
        {
            ViewBag.ShowCarousel = "False"; // Ẩn banner chạy
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            // 1. Lưu từ khóa lại để hiện ở ô input
            ViewBag.Keyword = keyword;

            // 2. Truy vấn tìm kiếm (Theo tên sản phẩm)
            var query = db.Products.Include(n => n.Category).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm gần đúng (chứa từ khóa)
                query = query.Where(n => n.Name.Contains(keyword));
            }

            // 3. Sắp xếp và phân trang
            var result = query.OrderByDescending(n => n.CreatedDate).ToPagedList(pageNumber, pageSize);

            // 4. Trả về View tìm kiếm
            return View("Search", result);
        }
    }
}