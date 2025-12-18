using System;
using System.Collections.Generic;

namespace MedicalStoreSystem.Models
{
    public class Purchase
    {
        public int PurchaseID { get; set; }
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } // للعرض
        public string SupplierInvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentType { get; set; }
        public int UserID { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        // قائمة الأصناف
        public List<PurchaseDetail> Details { get; set; }

        public Purchase()
        {
            PurchaseDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            Details = new List<PurchaseDetail>();
            PaymentType = "نقدي";
        }
    }

    public class PurchaseDetail
    {
        public int PurchaseDetailID { get; set; }
        public int PurchaseID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } // للعرض
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string BatchNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        public PurchaseDetail()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
