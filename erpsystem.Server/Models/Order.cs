using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace erpsystem.Server.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        public int ContractorId { get; set; }
        public Contractor Contractor { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Required]
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

        public bool IsDeleted { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}