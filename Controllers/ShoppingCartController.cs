using Microsoft.AspNetCore.Mvc;

using MVCShopping1.Data;
using MVCShopping1.Models;
using System.Text.Json;

namespace MVCShopping.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly MVCShopping1Context _context;

        public ShoppingCartController(MVCShopping1Context context)
        {
            _context = context;
        }

        // GET: Xem giỏ hàng
        public IActionResult Index()
        {
            return View(GetCart());
        }

        // POST: Thêm vào giỏ
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var cart = GetCart();
            var existing = cart.FirstOrDefault(i => i.ProductID == productId);

            if (existing == null)
            {
                cart.Add(new ShoppingCartItem
                {
                    ProductID = productId,
                    ProductName = product.ModelName,
                    Price = product.UnitCost,
                    Quantity = quantity
                });
            }
            else
            {
                existing.Quantity += quantity;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: Cập nhật số lượng
        [HttpPost]
        public IActionResult Update(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                if (quantity <= 0) cart.Remove(item);
                else item.Quantity = quantity;
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: Xóa sản phẩm
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductID == productId);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Đọc giỏ từ Session
        private List<ShoppingCartItem> GetCart()
        {
            var json = HttpContext.Session.GetString("Cart");
            return json == null
                ? new List<ShoppingCartItem>()
                : JsonSerializer.Deserialize<List<ShoppingCartItem>>(json)!;
        }

        // Lưu giỏ vào Session
        private void SaveCart(List<ShoppingCartItem> cart)
        {
            HttpContext.Session.SetString("Cart",
                JsonSerializer.Serialize(cart));
        }
    }
}