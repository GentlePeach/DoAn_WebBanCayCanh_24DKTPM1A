using DoAn_WebBanCayCanh_24DKTPM1A.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity; // QUAN TRỌNG: Thêm dòng này để dùng .Include

namespace DoAn_WebBanCayCanh_24DKTPM1A.Controllers
{
    public class HomeController : Controller
    {
        CoiGardenDBEntities db = new CoiGardenDBEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.ShowCarousel = "True";
            // Lấy 4 sản phẩm mới nhất
            var sanpham = db.Products.OrderByDescending(n => n.CreatedDate).Take(4).ToList();
            return View(sanpham);
        }

        public ActionResult About()
        {
            ViewBag.ShowCarousel = "False";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.ShowCarousel = "False";
            return View();
        }
       
    }
}