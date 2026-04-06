using System.ComponentModel.DataAnnotations;

namespace MVCShopping1.Models
{
    public class Customers
    {
        // [Key] = báo đây là khóa chính (Primary Key)
        [Key]
        public int CustomerID { get; set; }

        // string? = có thể null, không bắt buộc nhập
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
    }
}