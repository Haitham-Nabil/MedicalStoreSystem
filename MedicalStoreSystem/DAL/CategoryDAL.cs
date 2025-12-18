using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.DAL
{
    public class CategoryDAL
    {
        // جلب جميع التصنيفات
        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            string query = @"SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate 
                           FROM Categories 
                           ORDER BY CategoryName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    categories.Add(new Category
                    {
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"].ToString(),
                        Description = row["Description"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    });
                }
            }

            return categories;
        }

        // جلب التصنيفات النشطة فقط
        public List<Category> GetActiveCategories()
        {
            List<Category> categories = new List<Category>();

            string query = @"SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate 
                           FROM Categories 
                           WHERE IsActive = 1 
                           ORDER BY CategoryName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    categories.Add(new Category
                    {
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"].ToString(),
                        Description = row["Description"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    });
                }
            }

            return categories;
        }

        // جلب تصنيف واحد بالـ ID
        public Category GetCategoryByID(int categoryID)
        {
            string query = @"SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate 
                           FROM Categories 
                           WHERE CategoryID = @CategoryID";

            SqlParameter[] parameters = {
                new SqlParameter("@CategoryID", categoryID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Category
                {
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    CategoryName = row["CategoryName"].ToString(),
                    Description = row["Description"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                };
            }

            return null;
        }

        // إضافة تصنيف جديد
        public bool InsertCategory(Category category)
        {
            string query = @"INSERT INTO Categories (CategoryName, Description, IsActive, CreatedDate) 
                           VALUES (@CategoryName, @Description, @IsActive, @CreatedDate)";

            SqlParameter[] parameters = {
                new SqlParameter("@CategoryName", category.CategoryName),
                new SqlParameter("@Description", category.Description ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", category.IsActive),
                new SqlParameter("@CreatedDate", category.CreatedDate)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // تعديل تصنيف
        public bool UpdateCategory(Category category)
        {
            string query = @"UPDATE Categories 
                           SET CategoryName = @CategoryName,
                               Description = @Description,
                               IsActive = @IsActive
                           WHERE CategoryID = @CategoryID";

            SqlParameter[] parameters = {
                new SqlParameter("@CategoryID", category.CategoryID),
                new SqlParameter("@CategoryName", category.CategoryName),
                new SqlParameter("@Description", category.Description ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", category.IsActive)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // حذف تصنيف (تعطيل)
        public bool DeleteCategory(int categoryID)
        {
            string query = @"UPDATE Categories SET IsActive = 0 WHERE CategoryID = @CategoryID";

            SqlParameter[] parameters = {
                new SqlParameter("@CategoryID", categoryID)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // التحقق من وجود منتجات تحت التصنيف
        public bool HasProducts(int categoryID)
        {
            string query = @"SELECT COUNT(*) FROM Products WHERE CategoryID = @CategoryID";

            SqlParameter[] parameters = {
                new SqlParameter("@CategoryID", categoryID)
            };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // جلب DataTable للعرض في DataGridView
        public DataTable GetCategoriesDataTable()
        {
            string query = @"SELECT 
                               CategoryID AS 'الرقم',
                               CategoryName AS 'اسم التصنيف',
                               Description AS 'الوصف',
                               CASE WHEN IsActive = 1 THEN 'نشط' ELSE 'غير نشط' END AS 'الحالة',
                               CreatedDate AS 'تاريخ الإنشاء'
                           FROM Categories 
                           ORDER BY CategoryName";

            return DatabaseConnection.ExecuteDataTable(query);
        }
    }
}
