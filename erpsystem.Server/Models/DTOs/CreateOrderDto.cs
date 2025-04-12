using System.ComponentModel.DataAnnotations;

namespace erpsystem.Server.Models.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        public int ContractorId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalNetAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalGrossAmount { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "PLN";

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Nowe";

        [StringLength(500)]
        public string Notes { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; }
    }
}
