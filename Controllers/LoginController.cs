using Microsoft.AspNetCore.Mvc;
using MVCShopping1.Data;
using MVCShopping1.Models;



namespace MVCShopping1.Controllers
{
    public class LoginController : Controller
    {
        private readonly MVCShopping1Context _context;

        // ASP.NET tự inject _context khi controller được tạo
        public LoginController(MVCShopping1Context context)
        {
            _context = context;
        }

        // GET: /Login  → hiện form đăng nhập
        public IActionResult Index()
        {
            var model = new Customers();
            return View(model);
        }

        // POST: /Login  → xử lý khi nhấn nút Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Customers model)
        {
            if (!string.IsNullOrEmpty(model.EmailAddress)
                && !string.IsNullOrEmpty(model.Password))
            {
                // Tìm user khớp email VÀ password trong DB
                var user = _context.Customers.FirstOrDefault(u =>
                    u.EmailAddress == model.EmailAddress &&
                    u.Password == model.Password);

                if (user != null)
                {
                    // Lưu tên vào Session
                    HttpContext.Session.SetString("UserName", user.FullName ?? "");
                    return RedirectToAction("Welcome");
                }
                else
                {
                    ModelState.AddModelError("", "Sai email hoặc mật khẩu!");
                }
            }
            return View(model);
        }

        // GET: /Login/Welcome
        public IActionResult Welcome()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName != null)
            {
                ViewBag.UserName = userName;
                return View();
            }
            return RedirectToAction("Index");
        }

        // GET: /Login/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ Session
            return RedirectToAction("Index");
        }
    }
}