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
        public int WarehouseItemId { get; set; }
        public WarehouseItem WarehouseItem { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } 

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPriceNet { get; set; }

        public decimal UnitPriceGross { get; set; }

        [Required]
        [Range(0, 1)]
        public decimal VatRate { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }
    }
}