using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAn_WebBanCayCanh_24DKTPM1A.Models;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private CoiGardenDBEntities db = new CoiGardenDBEntities();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            // Lấy danh sách Order, kèm thông tin Khách Hàng, sắp xếp ngày mới nhất lên trên
            var orders = db.Orders.Include("KhachHang").OrderByDescending(o => o.CreatedDate);
            return View(orders.ToList());
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Code cũ chỉ có: Order order = db.Orders.Find(id);
            // Code MỚI: Dùng Include để lấy thêm Chi tiết và Sản phẩm
            Order order = db.Orders
                            .Include("OrderDetails")           // Lấy bảng chi tiết
                            .Include("OrderDetails.Product")   // Lấy luôn thông tin cây cảnh (để hiện tên, ảnh)
                            .Include("KhachHang")              // Lấy thông tin khách hàng
                            .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Admin/Orders/Create
        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            // 1. Lấy danh sách Khách hàng
            // Lưu ý: Kiểm tra lại tên cột khóa chính là 'MaKH' hay 'Id'?
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "HoTen");

            // 2. Lấy danh sách Sản phẩm (Để chọn mua cây gì)
            // Dựa vào hình ảnh bạn gửi: Product dùng cột 'Id' và 'Name'
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            return View();
        }

        // POST: Admin/Orders/Create
        // POST: Admin/Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm 2 tham số: ProductId và Quantity từ Form gửi lên
        public ActionResult Create(Order order, int? ProductId, int? Quantity)
        {
            if (ModelState.IsValid)
            {
                // --- BƯỚC 1: TẠO VỎ ĐƠN HÀNG (Order) ---
                order.CreatedDate = DateTime.Now; // Lưu ngày hiện tại
                order.Status = 1;                 // 1 = Chờ xử lý
                order.TotalAmount = 0;            // Tạm tính là 0

                db.Orders.Add(order);
                db.SaveChanges(); // Lưu xong dòng này, order.Id sẽ tự sinh ra

                // --- BƯỚC 2: TẠO CHI TIẾT ĐƠN HÀNG (OrderDetail) ---
                // Nếu Admin có chọn sản phẩm và số lượng hợp lệ
                if (ProductId != null && Quantity > 0)
                {
                    // Tìm sản phẩm trong DB để lấy giá tiền hiện tại
                    var product = db.Products.Find(ProductId);

                    if (product != null)
                    {
                        // Tạo mới dòng Chi tiết
                        var detail = new OrderDetail();
                        detail.OrderId = order.Id;      // Lấy ID của đơn vừa tạo ở trên
                        detail.ProductId = product.Id;  // ID sản phẩm
                        detail.Quantity = Quantity;
                        detail.Price = product.Price;   // Lấy giá từ bảng Product

                        db.OrderDetails.Add(detail);

                        // --- BƯỚC 3: CẬP NHẬT TỔNG TIỀN CHO ĐƠN HÀNG ---
                        // (Giá x Số lượng)
                        // Lưu ý: product.Price trong hình là Nullable (có dấu ?), nên cần check null
                        decimal price = product.Price ?? 0;
                        order.TotalAmount = price * Quantity;

                        db.SaveChanges(); // Lưu lại lần nữa
                    }
                }

                return RedirectToAction("Index");
            }

            // Nếu lỗi thì nạp lại danh sách để hiện lại form
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "HoTen", order.MaKH);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
            return View(order);
        }
        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Nếu muốn đổi khách hàng thì giữ dòng này, không thì xóa
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "HoTen", order.MaKH);

            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                // 1. Tìm đơn hàng cũ trong database
                var orderDB = db.Orders.Find(order.Id);

                if (orderDB != null)
                {
                    // 2. Chỉ cập nhật những cái Admin được phép sửa
                    orderDB.Status = order.Status;              // Cập nhật trạng thái
                    orderDB.PaymentStatus = order.PaymentStatus; // Cập nhật thanh toán

                    // Cập nhật thông tin người nhận (nếu Admin có sửa)
                    orderDB.CustomerName = order.CustomerName;
                    orderDB.CustomerPhone = order.CustomerPhone;
                    orderDB.CustomerAddress = order.CustomerAddress;
                    orderDB.CustomerEmail = order.CustomerEmail;
                    orderDB.Note = order.Note;

                    // 3. NHỮNG CÁI KHÔNG CẬP NHẬT (Giữ nguyên của cũ)
                    // - CreatedDate (Ngày đặt giữ nguyên)
                    // - TotalAmount (Tổng tiền giữ nguyên)
                    // - MaKH (Khách mua giữ nguyên)

                    // 4. Lưu lại
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // Nếu lỗi thì load lại view
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "HoTen", order.MaKH);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
