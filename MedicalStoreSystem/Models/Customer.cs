using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalStoreSystem.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string CustomerType { get; set; } // نقدي / آجل
        public decimal CreditLimit { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        public Customer()
        {
            IsActive = true;
            CustomerType = "نقدي";
            CreditLimit = 0;
            InitialBalance = 0;
            CurrentBalance = 0;
            CreatedDate = DateTime.Now;
        }
    }
}
