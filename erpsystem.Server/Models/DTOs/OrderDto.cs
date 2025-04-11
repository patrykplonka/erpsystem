namespace erpsystem.Server.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int ContractorId { get; set; }
        public ContractorDTO Contractor { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal TotalNetAmount { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal Discount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}