using System.ComponentModel.DataAnnotations;

namespace erpsystem.Server.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public int WarhouseItemId { get; set; } 
        public WarehouseItem WarehouseItem { get; set; } 

        [Required]
        public string ProductName { get; set; } 

        [Required]
        public decimal Quantity { get; set; } 

        [Required]
        public decimal UnitPriceNet { get; set; } 

        public decimal UnitPriceGross { get; set; } 

        [Required]
        public decimal TaxRate { get; set; } 

        public decimal Discount { get; set; } 
    }
}