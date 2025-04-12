using System.ComponentModel.DataAnnotations;

namespace erpsystem.Server.Models.DTOs
{
    public class CreateOrderItemDto
    {
        [Required]
        public int WarehouseItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPriceNet { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPriceGross { get; set; }

        [Required]
        [Range(0, 1)]
        public decimal VatRate { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }
    }
}
