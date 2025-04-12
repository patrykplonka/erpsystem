namespace erpsystem.Server.Models.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int WarehouseItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceNet { get; set; }
        public decimal UnitPriceGross { get; set; }
        public decimal VatRate { get; set; }
        public decimal Discount { get; set; }
    }
}