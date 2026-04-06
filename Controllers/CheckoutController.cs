using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MVCShopping1.Data;
using MVCShopping1.Models;
using System.Text.Json;

namespace MVCShopping.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MVCShopping1Context _context;

        public CheckoutController(MVCShopping1Context context)
        {
            _context = context;
        }

        // GET: Trang xác nhận đơn
        public IActionResult Index()
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "ShoppingCart");
            return View(cart);
        }

        // POST: Xác nhận đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm()
        {
            // Kiểm tra đã đăng nhập chưa
            var userName = HttpContext.Session.GetString("UserName");
            if (userName == null)
                return RedirectToAction("Index", "Login");

            // Tìm customer trong DB
            var customer = _context.Customers
                .FirstOrDefault(c => c.FullName == userName);
            if (customer == null)
                return RedirectToAction("Index", "Login");

            var cart = GetCart();
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "ShoppingCart");

            // Tạo đơn hàng
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                ShipDate = DateTime.Now.AddDays(3)
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Lưu để có OrderID

            // Tạo chi tiết đơn hàng
            foreach (var item in cart)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitCost = item.Price
                });
            }
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success", new { orderId = order.OrderID });
        }

        // GET: Đặt hàng thành công
        public IActionResult Success(int orderId)
        {
            ViewBag.OrderID = orderId;
            return View();
        }

        // GET: Lịch sử đơn hàng
        public IActionResult MyOrders()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName == null)
                return RedirectToAction("Index", "Login");

            var customer = _context.Customers
                .FirstOrDefault(c => c.FullName == userName);
            if (customer == null)
                return RedirectToAction("Index", "Login");

            var orders = _context.Orders
                .Where(o => o.CustomerID == customer.CustomerID)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: Chi tiết đơn hàng
        public IActionResult OrderDetails(int orderId)
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName == null)
                return RedirectToAction("Index", "Login");

            var details = _context.OrderDetails
                .Where(o => o.OrderID == orderId)
                .Include(o => o.Product)
                .ToList();

            ViewBag.OrderID = orderId;
            ViewBag.Total = details.Sum(o => o.UnitCost * o.Quantity);
            return View(details);
        }

        private List<ShoppingCartItem> GetCart()
        {
            var json = HttpContext.Session.GetString("Cart");
            return json == null
                ? new List<ShoppingCartItem>()
                : JsonSerializer.Deserialize<List<ShoppingCartItem>>(json)!;
        }

        private void SaveCart(List<ShoppingCartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }
}