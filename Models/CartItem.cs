using System;
using System.Linq;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Models
{
    public class CartItem
    {
        // Khởi tạo Database (để lấy thông tin sản phẩm khi tạo mới)
        CoiGardenDBEntities db = new CoiGardenDBEntities();

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; } // Dùng decimal cho tiền tệ là chuẩn nhất
        public int Quantity { get; set; }

        // Tính thành tiền (Read-only)
        public decimal Total
        {
            get { return Price * Quantity; }
        }

        // Constructor mặc định (cần thiết cho một số trường hợp serialize)
        public CartItem() { }

        // Constructor có tham số: Truyền ID vào tự tìm thông tin
        public CartItem(int id)
        {
            ProductId = id;
            var product = db.Products.Find(id);
            if (product != null)
            {
                ProductName = product.Name;
                ProductImage = product.Image;
                // Ép kiểu về decimal nếu trong DB là double hoặc null
                Price = product.Price.HasValue ? (decimal)product.Price.Value : 0;
                Quantity = 1;
            }
        }
    }
}