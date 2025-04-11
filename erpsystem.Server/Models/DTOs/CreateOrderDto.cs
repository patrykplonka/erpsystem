using System.ComponentModel.DataAnnotations;

namespace erpsystem.Server.Models.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Numer zamówienia jest wymagany.")]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "Kontrahent jest wymagany.")]
        public int ContractorId { get; set; }

        [Required(ErrorMessage = "Data zamówienia jest wymagana.")]
        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Wartość netto musi być większa lub równa 0.")]
        public decimal TotalNetAmount { get; set; }

        public decimal TotalGrossAmount { get; set; }

        public decimal Discount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "PLN";

        [Required(ErrorMessage = "Status jest wymagany.")]
        [StringLength(20)]
        public string Status { get; set; } = "Nowe";

        public string Notes { get; set; }

        [Required(ErrorMessage = "Pozycje zamówienia są wymagane.")]
        public List<CreateOrderItemDto> Items { get; set; }
    }
}
