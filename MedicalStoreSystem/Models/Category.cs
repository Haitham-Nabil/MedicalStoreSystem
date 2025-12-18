using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MedicalStoreSystem.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Constructor فارغ
        public Category()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor بالبيانات
        public Category(int categoryID, string categoryName, string description, bool isActive)
        {
            CategoryID = categoryID;
            CategoryName = categoryName;
            Description = description;
            IsActive = isActive;
            CreatedDate = DateTime.Now;
        }
    }
}
