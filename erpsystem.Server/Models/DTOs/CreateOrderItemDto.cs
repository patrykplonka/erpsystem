using System.ComponentModel.DataAnnotations;

namespace erpsystem.Server.Models.DTOs
{
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "Produkt jest wymagany.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Nazwa produktu jest wymagana.")]
        public string ProductName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Ilość musi być większa od 0.")]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cena jednostkowa netto musi być większa lub równa 0.")]
        public decimal UnitPriceNet { get; set; }

        public decimal UnitPriceGross { get; set; }

        [Range(0, 100, ErrorMessage = "Stawka VAT musi być między 0 a 100.")]
        public decimal TaxRate { get; set; }

        public decimal Discount { get; set; }
    }
}
