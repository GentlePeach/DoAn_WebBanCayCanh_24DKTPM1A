using System;
using System.Linq;
using System.Web.Mvc;
using DoAn_WebBanCayCanh_24DKTPM1A.Models;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Controllers
{
    public class AccountController : Controller
    {
        CoiGardenDBEntities db = new CoiGardenDBEntities();

        // phần đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                var check = db.KhachHangs.FirstOrDefault(s => s.TaiKhoan == kh.TaiKhoan);
                if (check == null)
                {
                    db.KhachHangs.Add(kh);
                    db.SaveChanges();
                    ViewBag.Success = "Đăng ký thành công! Hãy đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Tài khoản đã tồn tại";
                    return View();
                }
            }
            return View();
        }

        // phần đăng nhập   
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string user, string password)
        {
            // Tìm người dùng có TaiKhoan và MatKhau khớp với dữ liệu nhập
            var kh = db.KhachHangs.SingleOrDefault(s => s.TaiKhoan == user && s.MatKhau == password);

            if (kh != null)
            {
                // 2. Lưu thông tin vào Session để hệ thống ghi nhớ đăng nhập
                Session["TaiKhoan"] = kh;
                Session["HoTen"] = kh.HoTen; // Lưu thêm tên để hiển thị "Xin chào..."

                // 3. PHÂN LUỒNG NGƯỜI DÙNG (Đây là phần bạn cần)
                // Kiểm tra: Nếu tên đăng nhập là "admin" (viết thường) thì cho vào trang Quản trị
                if (user.ToLower() == "admin")
                {
                    // Chuyển hướng đến Action Index của HomeController trong Area "Admin"
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    // Các tài khoản khác thì về trang chủ bán hàng bình thường
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Đăng nhập thất bại
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
        }

        // đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa hết Session
            return RedirectToAction("Login");
        }

        // phần lịch sử đơn hàng
        public ActionResult History()
        {
            // Kiểm tra đã đăng nhập chưa
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("Login");
            }

            // Lấy thông tin khách từ Session
            KhachHang kh = (KhachHang)Session["TaiKhoan"];

            // Tìm các đơn hàng có MaKH trùng với khách đang đăng nhập
            var listOrder = db.Orders
                              .Where(n => n.MaKH == kh.MaKH)
                              .OrderByDescending(n => n.CreatedDate) // Mới nhất lên đầu
                              .ToList();

            return View(listOrder);
        }
    }
}