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
        // 1. Lấy giỏ hàng
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

        // 2. Thêm vào giỏ
       // [HttpPost] // Quan trọng: Chỉ nhận lệnh Post để bảo mật
                   // Quay lại dùng ActionResult (Load lại trang)
        public ActionResult AddToCart(int productId, string url)
        {
            // 1. Lấy giỏ hàng hiện tại (SỬA LỖI GẠCH ĐỎ)
            var cart = GetCart();

            // 2. Tìm sản phẩm trong giỏ
            var item = cart.FirstOrDefault(p => p.ProductId == productId);

            if (item != null)
            {
                // Có rồi thì tăng số lượng
                item.Quantity++;
            }
            else
            {
                // Chưa có thì tạo mới
                item = new CartItem(productId);
                cart.Add(item);
            }

            // 3. Lưu lại vào Session
            Session["CartSession"] = cart;

            // Lưu số lượng để hiện lên icon (Badge)
            Session["CartCount"] = cart.Sum(p => p.Quantity);

            // Lưu thông báo để hiện Toastr
            TempData["Notification"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";

            // 4. Chuyển hướng (Load lại trang)
            if (!string.IsNullOrEmpty(url))
            {
                return Redirect(url);
            }

            return RedirectToAction("Index", "Home");
        }
        // 3. Trang giỏ hàng
        public ActionResult Cart()
        {
            var cart = GetCart();
            ViewBag.TotalAmount = cart.Sum(p => p.Total);
            return View(cart);
        }

        // 4. Cập nhật
        [HttpPost]
        public ActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            return RedirectToAction("Cart");
        }

        // 5. Xóa
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
            }
            return RedirectToAction("Cart");
        }
    }
}