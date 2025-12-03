using DoAn_WebBanCayCanh_24DKTPM1A.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private CoiGardenDBEntities db = new CoiGardenDBEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            // Thống kê nhanh để hiện lên Dashboard
            ViewBag.SoLuongSanPham = db.Products.Count();
            ViewBag.SoLuongDonHang = db.Orders.Count();
            ViewBag.SoLuongKhachHang = db.KhachHangs.Count();
            ViewBag.SoLuongDanhMuc = db.Categories.Count();

            return View();
        }
    }
}