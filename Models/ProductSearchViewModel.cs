using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCShopping1.Models
{
    // ── Bảng Products ──
    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string? ModelNumber { get; set; }
        public string? ModelName { get; set; }
        public string? ProductImage { get; set; }
        public decimal UnitCost { get; set; }
        public string? Description { get; set; }
        public Category? Category { get; set; }
    }

    // ── Bảng Categories ──
    public class Category
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public ICollection<Product>? Products { get; set; }
    }

    // ── ViewModel tìm kiếm ──
    public class ProductSearchViewModel
    {
        public List<Product>? Products { get; set; }
        public SelectList? Categories { get; set; }
        public string? SearchString { get; set; }
        public string? Category { get; set; }
    }

    // ── Item trong giỏ hàng ──
    public class ShoppingCartItem
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    // ── Bảng Employee (MỚI) ──
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    // ── Bảng Orders (đã thêm EmployeeID) ──
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int? EmployeeID { get; set; }        
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public Customers? Customer { get; set; }
        public Employee? Employee { get; set; }     
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }

    // ── Bảng OrderDetails ──
    public class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}