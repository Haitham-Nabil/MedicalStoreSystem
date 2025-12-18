using System;
using System.Collections.Generic;

namespace MedicalStoreSystem.Models
{
    public class Sale
    {
        public int SaleID { get; set; }
        public string SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } // للعرض
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentType { get; set; }
        public bool IsPaid { get; set; }
        public int UserID { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        // قائمة الأصناف
        public List<SaleDetail> Details { get; set; }

        public Sale()
        {
            SaleDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            Details = new List<SaleDetail>();
            PaymentType = "نقدي";
            IsPaid = true;
        }
    }

    public class SaleDetail
    {
        public int SaleDetailID { get; set; }
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } // للعرض
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }

        public SaleDetail()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
