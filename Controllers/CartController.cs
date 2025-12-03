using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_WebBanCayCanh_24DKTPM1A.Models;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Controllers
{
    public class CartController : Controller
    {
        // Khai báo Database
        CoiGardenDBEntities db = new CoiGardenDBEntities();

        // Lấy giỏ hàng
        private List<CartItem> GetCart()
        {
            var cart = Session["CartSession"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["CartSession"] = cart;
            }
            return cart;
        }

        // Thêm vào giỏ
        public ActionResult AddToCart(int productId, string url)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == productId);

            if (item != null) item.Quantity++;
            else cart.Add(new CartItem(productId));

            Session["CartSession"] = cart;
            Session["CartCount"] = cart.Sum(p => p.Quantity);
            TempData["Notification"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";

            if (!string.IsNullOrEmpty(url)) return Redirect(url);
            return RedirectToAction("Index", "Home");
        }

        // Trang giỏ hàng
        public ActionResult Cart()
        {
            var cart = GetCart();
            ViewBag.TotalAmount = cart.Sum(p => p.Total);
            return View(cart);
        }

        // Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null) item.Quantity = quantity;
            return RedirectToAction("Cart");
        }

        // Xóa sản phẩm
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null) cart.Remove(item);
            return RedirectToAction("Cart");
        }
        // checkout- tthanh ttoabsn đơn hàng
        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction("Index", "Home");

            ViewBag.TotalAmount = cart.Sum(p => p.Total);
            return View(cart);
        }

        // lưu rồi chuyển về file sql
        [HttpPost]
        public ActionResult Checkout(FormCollection form)
        {
            try
            {
                var cart = GetCart();
                if (cart.Count == 0) return RedirectToAction("Index", "Home");

                // tạo đơn hàng mới
                Order order = new Order();
                order.CustomerName = form["CustomerName"];
                order.CustomerPhone = form["CustomerPhone"];
                order.CustomerAddress = form["CustomerAddress"];
                order.CustomerEmail = form["CustomerEmail"];
                order.Note = form["Note"];
                order.CreatedDate = DateTime.Now;
                order.PaymentStatus = "Chưa thanh toán";
                if (Session["TaiKhoan"] != null)
                {
                    KhachHang kh = (KhachHang)Session["TaiKhoan"];
                    order.MaKH = kh.MaKH; // Lưu mã khách vào đơn hàng
                }
                
                db.Orders.Add(order);

                // Tính tổng tiền
                order.TotalAmount = (decimal)cart.Sum(p => p.Total);

                db.Orders.Add(order);
                db.SaveChanges(); // Lưu để lấy OrderId

                // chi tiết đơn hàng
                foreach (var item in cart)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.OrderId = order.Id;
                    detail.ProductId = item.ProductId;
                    detail.Quantity = item.Quantity;
                    detail.UnitPrice = (decimal)item.Price;
                    detail.Total = (decimal)item.Total;

                    db.OrderDetails.Add(detail);
                }
                db.SaveChanges(); // Lưu xong vào SQL

               // xóa đi đơn khônng cần 

                Session["CartSession"] = null; // Xóa giỏ hàng
                Session["CartCount"] = 0; // Reset icon giỏ hàng

                return RedirectToAction("OrderSuccess");
            }
            catch (Exception ex)
            {
                // Nếu lỗi thì hiện lại trang Checkout và báo lỗi
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
                return View(GetCart());
            }
        }

        // chuyển tới trang nếu đơn thành công
        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}