﻿namespace erpsystem.Server.Models.DTOs
{
    public class WarehouseMovementsDTO
    {
        public int Id { get; set; }
        public int WarehouseItemId { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}