using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.DAL
{
    public class SupplierDAL
    {
        // جلب جميع الموردين
        public List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();

            string query = @"SELECT SupplierID, SupplierCode, SupplierName, ContactPerson, 
                           Phone, Mobile, Address, Email, TaxNumber, InitialBalance, 
                           CurrentBalance, IsActive, Notes, CreatedDate 
                           FROM Suppliers 
                           ORDER BY SupplierName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    suppliers.Add(MapRowToSupplier(row));
                }
            }

            return suppliers;
        }

        // جلب الموردين النشطين فقط
        public List<Supplier> GetActiveSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();

            string query = @"SELECT SupplierID, SupplierCode, SupplierName, ContactPerson, 
                           Phone, Mobile, Address, Email, TaxNumber, InitialBalance, 
                           CurrentBalance, IsActive, Notes, CreatedDate 
                           FROM Suppliers 
                           WHERE IsActive = 1 
                           ORDER BY SupplierName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    suppliers.Add(MapRowToSupplier(row));
                }
            }

            return suppliers;
        }

        // جلب مورد واحد بالـ ID
        public Supplier GetSupplierByID(int supplierID)
        {
            string query = @"SELECT SupplierID, SupplierCode, SupplierName, ContactPerson, 
                           Phone, Mobile, Address, Email, TaxNumber, InitialBalance, 
                           CurrentBalance, IsActive, Notes, CreatedDate 
                           FROM Suppliers 
                           WHERE SupplierID = @SupplierID";

            SqlParameter[] parameters = {
                new SqlParameter("@SupplierID", supplierID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                return MapRowToSupplier(dt.Rows[0]);
            }

            return null;
        }

        // إضافة مورد جديد
        public bool InsertSupplier(Supplier supplier)
        {
            // التحقق من عدم تكرار الكود
            if (IsSupplierCodeExists(supplier.SupplierCode))
                return false;

            string query = @"INSERT INTO Suppliers 
                           (SupplierCode, SupplierName, ContactPerson, Phone, Mobile, 
                           Address, Email, TaxNumber, InitialBalance, CurrentBalance, 
                           IsActive, Notes, CreatedDate) 
                           VALUES 
                           (@SupplierCode, @SupplierName, @ContactPerson, @Phone, @Mobile, 
                           @Address, @Email, @TaxNumber, @InitialBalance, @CurrentBalance, 
                           @IsActive, @Notes, @CreatedDate)";

            SqlParameter[] parameters = GetSupplierParameters(supplier);

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // تعديل مورد
        public bool UpdateSupplier(Supplier supplier)
        {
            string query = @"UPDATE Suppliers 
                           SET SupplierCode = @SupplierCode,
                               SupplierName = @SupplierName,
                               ContactPerson = @ContactPerson,
                               Phone = @Phone,
                               Mobile = @Mobile,
                               Address = @Address,
                               Email = @Email,
                               TaxNumber = @TaxNumber,
                               InitialBalance = @InitialBalance,
                               CurrentBalance = @CurrentBalance,
                               IsActive = @IsActive,
                               Notes = @Notes
                           WHERE SupplierID = @SupplierID";

            List<SqlParameter> parametersList = new List<SqlParameter>(GetSupplierParameters(supplier));
            parametersList.Add(new SqlParameter("@SupplierID", supplier.SupplierID));

            int result = DatabaseConnection.ExecuteNonQuery(query, parametersList.ToArray());
            return result > 0;
        }

        // حذف مورد (تعطيل)
        public bool DeleteSupplier(int supplierID)
        {
            string query = @"UPDATE Suppliers SET IsActive = 0 WHERE SupplierID = @SupplierID";

            SqlParameter[] parameters = {
                new SqlParameter("@SupplierID", supplierID)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // التحقق من وجود كود المورد
        public bool IsSupplierCodeExists(string supplierCode, int? excludeSupplierID = null)
        {
            string query = @"SELECT COUNT(*) FROM Suppliers WHERE SupplierCode = @SupplierCode";

            if (excludeSupplierID.HasValue)
                query += " AND SupplierID != @SupplierID";

            SqlParameter[] parameters = excludeSupplierID.HasValue ?
                new SqlParameter[] {
                    new SqlParameter("@SupplierCode", supplierCode),
                    new SqlParameter("@SupplierID", excludeSupplierID.Value)
                } :
                new SqlParameter[] {
                    new SqlParameter("@SupplierCode", supplierCode)
                };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // جلب DataTable للعرض
        public DataTable GetSuppliersDataTable()
        {
            string query = @"SELECT 
                               SupplierID AS 'الرقم',
                               SupplierCode AS 'كود المورد',
                               SupplierName AS 'اسم المورد',
                               ContactPerson AS 'الشخص المسؤول',
                               Mobile AS 'الموبايل',
                               Phone AS 'التليفون',
                               Address AS 'العنوان',
                               CurrentBalance AS 'الرصيد الحالي',
                               CASE WHEN IsActive = 1 THEN 'نشط' ELSE 'غير نشط' END AS 'الحالة'
                           FROM Suppliers 
                           ORDER BY SupplierName";

            return DatabaseConnection.ExecuteDataTable(query);
        }

        // توليد كود مورد تلقائي
        public string GenerateSupplierCode()
        {
            string query = @"SELECT MAX(CAST(SUBSTRING(SupplierCode, 4, LEN(SupplierCode)) AS INT)) 
                           FROM Suppliers 
                           WHERE SupplierCode LIKE 'SUP%'";

            object result = DatabaseConnection.ExecuteScalar(query);

            int nextNumber = 1;
            if (result != null && result != DBNull.Value)
            {
                nextNumber = Convert.ToInt32(result) + 1;
            }

            return $"SUP{nextNumber:D3}"; // SUP001, SUP002, etc.
        }

        // دوال مساعدة
        private Supplier MapRowToSupplier(DataRow row)
        {
            return new Supplier
            {
                SupplierID = Convert.ToInt32(row["SupplierID"]),
                SupplierCode = row["SupplierCode"].ToString(),
                SupplierName = row["SupplierName"].ToString(),
                ContactPerson = row["ContactPerson"].ToString(),
                Phone = row["Phone"].ToString(),
                Mobile = row["Mobile"].ToString(),
                Address = row["Address"].ToString(),
                Email = row["Email"].ToString(),
                TaxNumber = row["TaxNumber"].ToString(),
                InitialBalance = Convert.ToDecimal(row["InitialBalance"]),
                CurrentBalance = Convert.ToDecimal(row["CurrentBalance"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                Notes = row["Notes"].ToString(),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private SqlParameter[] GetSupplierParameters(Supplier supplier)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@SupplierCode", supplier.SupplierCode),
                new SqlParameter("@SupplierName", supplier.SupplierName),
                new SqlParameter("@ContactPerson", supplier.ContactPerson ?? (object)DBNull.Value),
                new SqlParameter("@Phone", supplier.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Mobile", supplier.Mobile ?? (object)DBNull.Value),
                new SqlParameter("@Address", supplier.Address ?? (object)DBNull.Value),
                new SqlParameter("@Email", supplier.Email ?? (object)DBNull.Value),
                new SqlParameter("@TaxNumber", supplier.TaxNumber ?? (object)DBNull.Value),
                new SqlParameter("@InitialBalance", supplier.InitialBalance),
                new SqlParameter("@CurrentBalance", supplier.CurrentBalance),
                new SqlParameter("@IsActive", supplier.IsActive),
                new SqlParameter("@Notes", supplier.Notes ?? (object)DBNull.Value),
                new SqlParameter("@CreatedDate", supplier.CreatedDate)
            };
        }
    }
}
