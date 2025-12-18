using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.DAL
{
    public class ProductDAL
    {
        // جلب جميع المنتجات مع أسماء التصنيفات والموردين
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            string query = @"SELECT p.ProductID, p.ProductCode, p.Barcode, p.ProductName, 
                           p.CategoryID, c.CategoryName, p.UnitName, p.CostPrice, 
                           p.SalePrice, p.MinStock, p.CurrentStock, p.SupplierID, 
                           s.SupplierName, p.IsActive, p.HasExpiry, p.Notes, p.CreatedDate
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           ORDER BY p.ProductName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    products.Add(MapRowToProduct(row));
                }
            }

            return products;
        }

        // جلب المنتجات النشطة فقط
        public List<Product> GetActiveProducts()
        {
            List<Product> products = new List<Product>();

            string query = @"SELECT p.ProductID, p.ProductCode, p.Barcode, p.ProductName, 
                           p.CategoryID, c.CategoryName, p.UnitName, p.CostPrice, 
                           p.SalePrice, p.MinStock, p.CurrentStock, p.SupplierID, 
                           s.SupplierName, p.IsActive, p.HasExpiry, p.Notes, p.CreatedDate
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           WHERE p.IsActive = 1
                           ORDER BY p.ProductName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    products.Add(MapRowToProduct(row));
                }
            }

            return products;
        }

        // جلب منتج واحد بالـ ID
        public Product GetProductByID(int productID)
        {
            string query = @"SELECT p.ProductID, p.ProductCode, p.Barcode, p.ProductName, 
                           p.CategoryID, c.CategoryName, p.UnitName, p.CostPrice, 
                           p.SalePrice, p.MinStock, p.CurrentStock, p.SupplierID, 
                           s.SupplierName, p.IsActive, p.HasExpiry, p.Notes, p.CreatedDate
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           WHERE p.ProductID = @ProductID";

            SqlParameter[] parameters = {
                new SqlParameter("@ProductID", productID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                return MapRowToProduct(dt.Rows[0]);
            }

            return null;
        }

        // البحث عن منتج بالباركود
        public Product GetProductByBarcode(string barcode)
        {
            string query = @"SELECT p.ProductID, p.ProductCode, p.Barcode, p.ProductName, 
                           p.CategoryID, c.CategoryName, p.UnitName, p.CostPrice, 
                           p.SalePrice, p.MinStock, p.CurrentStock, p.SupplierID, 
                           s.SupplierName, p.IsActive, p.HasExpiry, p.Notes, p.CreatedDate
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           WHERE p.Barcode = @Barcode AND p.IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@Barcode", barcode)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                return MapRowToProduct(dt.Rows[0]);
            }

            return null;
        }

        // إضافة منتج جديد
        public bool InsertProduct(Product product)
        {
            // التحقق من عدم تكرار الكود
            if (IsProductCodeExists(product.ProductCode))
                return false;

            string query = @"INSERT INTO Products 
                           (ProductCode, Barcode, ProductName, CategoryID, UnitName, 
                           CostPrice, SalePrice, MinStock, CurrentStock, SupplierID, 
                           IsActive, HasExpiry, Notes, CreatedDate) 
                           VALUES 
                           (@ProductCode, @Barcode, @ProductName, @CategoryID, @UnitName, 
                           @CostPrice, @SalePrice, @MinStock, @CurrentStock, @SupplierID, 
                           @IsActive, @HasExpiry, @Notes, @CreatedDate)";

            SqlParameter[] parameters = GetProductParameters(product);

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // تعديل منتج
        public bool UpdateProduct(Product product)
        {
            string query = @"UPDATE Products 
                           SET ProductCode = @ProductCode,
                               Barcode = @Barcode,
                               ProductName = @ProductName,
                               CategoryID = @CategoryID,
                               UnitName = @UnitName,
                               CostPrice = @CostPrice,
                               SalePrice = @SalePrice,
                               MinStock = @MinStock,
                               CurrentStock = @CurrentStock,
                               SupplierID = @SupplierID,
                               IsActive = @IsActive,
                               HasExpiry = @HasExpiry,
                               Notes = @Notes
                           WHERE ProductID = @ProductID";

            List<SqlParameter> parametersList = new List<SqlParameter>(GetProductParameters(product));
            parametersList.Add(new SqlParameter("@ProductID", product.ProductID));

            int result = DatabaseConnection.ExecuteNonQuery(query, parametersList.ToArray());
            return result > 0;
        }

        // حذف منتج (تعطيل)
        public bool DeleteProduct(int productID)
        {
            string query = @"UPDATE Products SET IsActive = 0 WHERE ProductID = @ProductID";

            SqlParameter[] parameters = {
                new SqlParameter("@ProductID", productID)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // التحقق من وجود كود المنتج
        public bool IsProductCodeExists(string productCode, int? excludeProductID = null)
        {
            string query = @"SELECT COUNT(*) FROM Products WHERE ProductCode = @ProductCode";

            if (excludeProductID.HasValue)
                query += " AND ProductID != @ProductID";

            SqlParameter[] parameters = excludeProductID.HasValue ?
                new SqlParameter[] {
                    new SqlParameter("@ProductCode", productCode),
                    new SqlParameter("@ProductID", excludeProductID.Value)
                } :
                new SqlParameter[] {
                    new SqlParameter("@ProductCode", productCode)
                };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // جلب المنتجات القليلة في المخزون
        public DataTable GetLowStockProducts()
        {
            string query = @"SELECT 
                               p.ProductCode AS 'كود المنتج',
                               p.ProductName AS 'اسم المنتج',
                               c.CategoryName AS 'التصنيف',
                               p.CurrentStock AS 'الكمية الحالية',
                               p.MinStock AS 'الحد الأدنى'
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           WHERE p.CurrentStock <= p.MinStock AND p.IsActive = 1
                           ORDER BY p.CurrentStock";

            return DatabaseConnection.ExecuteDataTable(query);
        }

        // جلب DataTable للعرض
        public DataTable GetProductsDataTable()
        {
            string query = @"SELECT 
                               p.ProductID AS 'الرقم',
                               p.ProductCode AS 'كود المنتج',
                               p.Barcode AS 'الباركود',
                               p.ProductName AS 'اسم المنتج',
                               c.CategoryName AS 'التصنيف',
                               p.UnitName AS 'الوحدة',
                               p.CostPrice AS 'سعر الشراء',
                               p.SalePrice AS 'سعر البيع',
                               p.CurrentStock AS 'الكمية',
                               p.MinStock AS 'الحد الأدنى',
                               s.SupplierName AS 'المورد',
                               CASE WHEN p.IsActive = 1 THEN 'نشط' ELSE 'غير نشط' END AS 'الحالة'
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           ORDER BY p.ProductName";

            return DatabaseConnection.ExecuteDataTable(query);
        }

        // توليد كود منتج تلقائي
        public string GenerateProductCode()
        {
            string query = @"SELECT MAX(CAST(SUBSTRING(ProductCode, 4, LEN(ProductCode)) AS INT)) 
                           FROM Products 
                           WHERE ProductCode LIKE 'PRD%'";

            object result = DatabaseConnection.ExecuteScalar(query);

            int nextNumber = 1;
            if (result != null && result != DBNull.Value)
            {
                nextNumber = Convert.ToInt32(result) + 1;
            }

            return $"PRD{nextNumber:D3}"; // PRD001, PRD002, etc.
        }

        // تحديث المخزون
        public bool UpdateStock(int productID, int quantity)
        {
            string query = @"UPDATE Products 
                           SET CurrentStock = CurrentStock + @Quantity 
                           WHERE ProductID = @ProductID";

            SqlParameter[] parameters = {
                new SqlParameter("@ProductID", productID),
                new SqlParameter("@Quantity", quantity)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // دوال مساعدة
        private Product MapRowToProduct(DataRow row)
        {
            return new Product
            {
                ProductID = Convert.ToInt32(row["ProductID"]),
                ProductCode = row["ProductCode"].ToString(),
                Barcode = row["Barcode"].ToString(),
                ProductName = row["ProductName"].ToString(),
                CategoryID = row["CategoryID"] != DBNull.Value ? Convert.ToInt32(row["CategoryID"]) : 0,
                CategoryName = row["CategoryName"].ToString(),
                UnitName = row["UnitName"].ToString(),
                CostPrice = Convert.ToDecimal(row["CostPrice"]),
                SalePrice = Convert.ToDecimal(row["SalePrice"]),
                MinStock = Convert.ToInt32(row["MinStock"]),
                CurrentStock = Convert.ToInt32(row["CurrentStock"]),
                SupplierID = row["SupplierID"] != DBNull.Value ? Convert.ToInt32(row["SupplierID"]) : 0,
                SupplierName = row["SupplierName"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                HasExpiry = Convert.ToBoolean(row["HasExpiry"]),
                Notes = row["Notes"].ToString(),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private SqlParameter[] GetProductParameters(Product product)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@ProductCode", product.ProductCode),
                new SqlParameter("@Barcode", product.Barcode ?? (object)DBNull.Value),
                new SqlParameter("@ProductName", product.ProductName),
                new SqlParameter("@CategoryID", product.CategoryID > 0 ? (object)product.CategoryID : DBNull.Value),
                new SqlParameter("@UnitName", product.UnitName ?? (object)DBNull.Value),
                new SqlParameter("@CostPrice", product.CostPrice),
                new SqlParameter("@SalePrice", product.SalePrice),
                new SqlParameter("@MinStock", product.MinStock),
                new SqlParameter("@CurrentStock", product.CurrentStock),
                new SqlParameter("@SupplierID", product.SupplierID > 0 ? (object)product.SupplierID : DBNull.Value),
                new SqlParameter("@IsActive", product.IsActive),
                new SqlParameter("@HasExpiry", product.HasExpiry),
                new SqlParameter("@Notes", product.Notes ?? (object)DBNull.Value),
                new SqlParameter("@CreatedDate", product.CreatedDate)
            };
        }
    }
}
