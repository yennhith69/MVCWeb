using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCShopping1.Data;
using MVCShopping1.Models;

namespace MVCShopping1.Controllers
{
    public class AdminController : Controller
    {
        private readonly MVCShopping1Context _context;

        public AdminController(MVCShopping1Context context)
        {
            _context = context;
        }

        // ── Kiểm tra có phải Admin không ──
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }

        // ── GET: Admin Login ──
        public IActionResult Login()
        {
            return View();
        }

        // ── POST: Admin Login ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.SetString("AdminName", "Administrator");
                return RedirectToAction("Orders");
            }
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        // ── GET: Admin Logout ──
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("AdminName");
            return RedirectToAction("Login");
        }

        // ── GET: Quản lý Orders ──
        public IActionResult Orders()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // ── GET: Chi tiết OrderDetails ──
        public IActionResult OrderDetails(int orderId)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var details = _context.OrderDetails
                .Where(o => o.OrderID == orderId)
                .Include(o => o.Product)
                .ToList();

            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .FirstOrDefault(o => o.OrderID == orderId);

            ViewBag.OrderID = orderId;
            ViewBag.Order = order;
            ViewBag.Total = details.Sum(o => o.UnitCost * o.Quantity);
            return View(details);
        }

        // ── GET: Sửa Order ──
        public IActionResult EditOrder(int orderId)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .FirstOrDefault(o => o.OrderID == orderId);

            if (order == null) return NotFound();

            ViewBag.Employees = new SelectList(
                _context.Employees.ToList(),
                "EmployeeID", "FullName",
                order.EmployeeID);

            return View(order);
        }

        // ── POST: Lưu sửa Order ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int orderId, int? employeeId, DateTime shipDate)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var order = _context.Orders.Find(orderId);
            if (order == null) return NotFound();

            order.EmployeeID = employeeId;
            order.ShipDate = shipDate;
            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }

        // ── GET: Thống kê doanh thu ──
        public IActionResult Statistics()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var totalRevenue = _context.OrderDetails
                .Sum(od => od.UnitCost * od.Quantity);

            var totalItemsSold = _context.OrderDetails
                .Sum(od => od.Quantity);

            var totalOrders = _context.Orders.Count();

            var topProducts = _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new { od.ProductID, od.Product!.ModelName })
                .Select(g => new
                {
                    ProductName = g.Key.ModelName,
                    TotalQty = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.UnitCost * x.Quantity)
                })
                .OrderByDescending(x => x.TotalQty)
                .Take(5)
                .ToList();

            var revenueByEmployee = _context.Orders
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                .Where(o => o.EmployeeID != null)
                .GroupBy(o => new { o.EmployeeID, o.Employee!.FullName })
                .Select(g => new
                {
                    EmployeeName = g.Key.FullName,
                    OrderCount = g.Count(),
                    TotalRevenue = g.SelectMany(o => o.OrderDetails!)
                                    .Sum(od => od.UnitCost * od.Quantity)
                })
                .ToList();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalItemsSold = totalItemsSold;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TopProducts = topProducts;
            ViewBag.RevenueByEmployee = revenueByEmployee;

            return View();
        }

        // ── GET: Danh sách Employee ──
        public IActionResult EmployeeList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        // ── GET: Thêm Employee ──
        public IActionResult CreateEmployee()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        // ── POST: Lưu Employee mới ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("EmployeeList");
        }
    }
}