namespace erpsystem.Server.Models.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPriceNet { get; set; }
        public decimal UnitPriceGross { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Discount { get; set; }
    }
}