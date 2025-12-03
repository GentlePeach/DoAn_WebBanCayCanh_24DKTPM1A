using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAn_WebBanCayCanh_24DKTPM1A.Models;
using PagedList; 

namespace DoAn_WebBanCayCanh_24DKTPM1A.Areas.Admin.Controllers
{
    public class KhachHangsController : Controller
    {
        private CoiGardenDBEntities db = new CoiGardenDBEntities();

        // GET: Admin/KhachHangs
        public ActionResult Index(int? page)
        {
            // 1. Phân trang: 10 khách hàng mỗi trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            // 2. Sắp xếp: Khách mới nhất lên đầu
            var khachHangs = db.KhachHangs.OrderByDescending(k => k.MaKH);

            return View(khachHangs.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/KhachHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null) return HttpNotFound();
            return View(khachHang);
        }

        // GET: Admin/KhachHangs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                // Tự động gán ngày tạo
                khachHang.NgayTao = DateTime.Now;

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null) return HttpNotFound();
            return View(khachHang);
        }

        // POST: Admin/KhachHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                // Logic cập nhật an toàn:
                var khOld = db.KhachHangs.Find(khachHang.MaKH);
                if (khOld != null)
                {
                    khOld.HoTen = khachHang.HoTen;
                    khOld.Email = khachHang.Email;
                    khOld.DienThoai = khachHang.DienThoai;
                    khOld.DiaChi = khachHang.DiaChi;

                    // Chỉ cập nhật mật khẩu nếu Admin nhập mới, không thì giữ nguyên
                    if (!string.IsNullOrEmpty(khachHang.MatKhau))
                    {
                        khOld.MatKhau = khachHang.MatKhau;
                    }

                    // Không cập nhật NgayTao và TaiKhoan

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null) return HttpNotFound();
            return View(khachHang);
        }

        // POST: Admin/KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhachHang khachHang = db.KhachHangs.Find(id);
            db.KhachHangs.Remove(khachHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}