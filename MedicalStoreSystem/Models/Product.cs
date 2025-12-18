using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalStoreSystem.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } // للعرض فقط
        public string UnitName { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int MinStock { get; set; }
        public int CurrentStock { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } // للعرض فقط
        public bool IsActive { get; set; }
        public bool HasExpiry { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        public Product()
        {
            IsActive = true;
            HasExpiry = true;
            MinStock = 10;
            CurrentStock = 0;
            CreatedDate = DateTime.Now;
        }

        // حساب نسبة الربح
        public decimal ProfitMargin
        {
            get
            {
                if (CostPrice > 0)
                    return ((SalePrice - CostPrice) / CostPrice) * 100;
                return 0;
            }
        }

        // التحقق من نقص المخزون
        public bool IsLowStock
        {
            get { return CurrentStock <= MinStock; }
        }
    }
}
