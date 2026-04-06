using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using MVCShopping1.Data;
using MVCShopping1.Models;

namespace MVCShopping.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MVCShopping1Context _context;

        public ProductsController(MVCShopping1Context context)
        {
            _context = context;
        }

        // GET: /Products/Search  → hiện form tìm kiếm
        public async Task<IActionResult> Search()
        {
            var vm = new ProductSearchViewModel
            {
                // Lấy danh sách category cho dropdown
                Categories = new SelectList(
                    await _context.Categories.ToListAsync(),
                    "CategoryID",    // giá trị submit lên server
                    "CategoryName")  // text hiển thị trong dropdown
            };
            return View(vm);
        }

        // POST: /Products/Search  → lọc và trả kết quả
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string searchString, string category)
        {
            // Lấy tất cả sản phẩm, kèm thông tin Category (JOIN)
            var products = from p in _context.Products.Include(p => p.Category)
                           select p;

            // Lọc theo tên nếu có nhập
            if (!string.IsNullOrEmpty(searchString))
                products = products.Where(s => s.ModelName.Contains(searchString));

            // Lọc theo category nếu có chọn
            if (!string.IsNullOrEmpty(category))
                products = products.Where(x => x.CategoryID.ToString() == category);

            var vm = new ProductSearchViewModel
            {
                Categories = new SelectList(
                    await _context.Categories.ToListAsync(),
                    "CategoryID", "CategoryName"),
                Products = await products.ToListAsync()
            };
            return View(vm);
        }
    }
}