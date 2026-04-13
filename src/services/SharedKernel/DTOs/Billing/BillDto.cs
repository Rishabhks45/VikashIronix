using System;
using System.Collections.Generic;

namespace SharedKernel.DTOs.Billing
{
    public class BillDto
    {
        public Guid Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        
        // Relational Read-Only
        public string CustomerName { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string ShopAddress { get; set; } = string.Empty;

        public DateTime BillDate { get; set; } = DateTime.Now;
        public decimal SubTotal { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal BhadaAmount { get; set; }
        public decimal LabourAmount { get; set; }
        public decimal RoundingAdjustment { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaid { get; set; } // Represented as "A/C (Bank)" in the manual ledger
        public decimal CashPaid { get; set; }   // Represented as "Cash (Liquid)" in the manual ledger
        public decimal BalanceDue { get; set; } // Represented as "Due" in the manual ledger
        public decimal TotalWeightKg { get; set; } // Represented as "Case" in the manual ledger
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft"; // Draft, Pending, Paid, Part-Paid, Cancelled

        public DateTime CreatedAt { get; set; }
        
        // Children
        public List<BillItemDto> Items { get; set; } = new();
    }

    public class BillItemDto
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public Guid MaterialId { get; set; }
        
        // Relational Read-Only
        public string MaterialName { get; set; } = string.Empty;
        public string Shape { get; set; } = string.Empty;
        
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; } = 18.00m;
        public decimal TotalPrice { get; set; }
    }

    public class BillSaveResult
    {
        public Guid InsertedId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
    }
}
