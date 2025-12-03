using System;
using System.Linq;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Models
{
    public class CartItem
    {
        CoiGardenDBEntities db = new CoiGardenDBEntities();

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; } 
        public int Quantity { get; set; }
        public decimal Total
        {
            get { return Price * Quantity; }
        }
        public CartItem() { }

        // Truyền ID vào tự tìm thông tin
        public CartItem(int id)
        {
            ProductId = id;
            var product = db.Products.Find(id);
            if (product != null)
            {
                ProductName = product.Name;
                ProductImage = product.Image;
                Price = product.Price.HasValue ? (decimal)product.Price.Value : 0;
                Quantity = 1;
            }
        }
    }
}