using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.DAL
{
    public class CustomerDAL
    {
        // جلب جميع العملاء
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            string query = @"SELECT CustomerID, CustomerCode, CustomerName, Phone, Mobile, 
                           Address, Email, CustomerType, CreditLimit, InitialBalance, 
                           CurrentBalance, IsActive, Notes, CreatedDate 
                           FROM Customers 
                           ORDER BY CustomerName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    customers.Add(MapRowToCustomer(row));
                }
            }

            return customers;
        }

        // جلب العملاء النشطين
        public List<Customer> GetActiveCustomers()
        {
            List<Customer> customers = new List<Customer>();

            string query = @"SELECT CustomerID, CustomerCode, CustomerName, Phone, Mobile, 
                           Address, Email, CustomerType, CreditLimit, InitialBalance, 
                           CurrentBalance, IsActive, Notes, CreatedDate 
                           FROM Customers 
                           WHERE IsActive = 1 
                           ORDER BY CustomerName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    customers.Add(MapRowToCustomer(row));
                }
            }

            return customers;
        }

        // جلب عميل بالـ ID
        public Customer GetCustomerByID(int customerID)
        {
            string query = @"SELECT CustomerID, CustomerCode, CustomerName, Phone, Mobile, 
                           Address, Email, CustomerType, CreditLimit, InitialBalance, 
                           CurrentBalance, IsActive, Notes, CreatedDate 
                           FROM Customers 
                           WHERE CustomerID = @CustomerID";

            SqlParameter[] parameters = {
                new SqlParameter("@CustomerID", customerID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                return MapRowToCustomer(dt.Rows[0]);
            }

            return null;
        }

        // إضافة عميل
        public bool InsertCustomer(Customer customer)
        {
            if (IsCustomerCodeExists(customer.CustomerCode))
                return false;

            string query = @"INSERT INTO Customers 
                           (CustomerCode, CustomerName, Phone, Mobile, Address, Email, 
                           CustomerType, CreditLimit, InitialBalance, CurrentBalance, 
                           IsActive, Notes, CreatedDate) 
                           VALUES 
                           (@CustomerCode, @CustomerName, @Phone, @Mobile, @Address, @Email, 
                           @CustomerType, @CreditLimit, @InitialBalance, @CurrentBalance, 
                           @IsActive, @Notes, @CreatedDate)";

            SqlParameter[] parameters = GetCustomerParameters(customer);

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // تعديل عميل
        public bool UpdateCustomer(Customer customer)
        {
            string query = @"UPDATE Customers 
                           SET CustomerCode = @CustomerCode,
                               CustomerName = @CustomerName,
                               Phone = @Phone,
                               Mobile = @Mobile,
                               Address = @Address,
                               Email = @Email,
                               CustomerType = @CustomerType,
                               CreditLimit = @CreditLimit,
                               InitialBalance = @InitialBalance,
                               CurrentBalance = @CurrentBalance,
                               IsActive = @IsActive,
                               Notes = @Notes
                           WHERE CustomerID = @CustomerID";

            List<SqlParameter> parametersList = new List<SqlParameter>(GetCustomerParameters(customer));
            parametersList.Add(new SqlParameter("@CustomerID", customer.CustomerID));

            int result = DatabaseConnection.ExecuteNonQuery(query, parametersList.ToArray());
            return result > 0;
        }

        // حذف عميل
        public bool DeleteCustomer(int customerID)
        {
            string query = @"UPDATE Customers SET IsActive = 0 WHERE CustomerID = @CustomerID";

            SqlParameter[] parameters = {
                new SqlParameter("@CustomerID", customerID)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // التحقق من كود العميل
        public bool IsCustomerCodeExists(string customerCode, int? excludeCustomerID = null)
        {
            string query = @"SELECT COUNT(*) FROM Customers WHERE CustomerCode = @CustomerCode";

            if (excludeCustomerID.HasValue)
                query += " AND CustomerID != @CustomerID";

            SqlParameter[] parameters = excludeCustomerID.HasValue ?
                new SqlParameter[] {
                    new SqlParameter("@CustomerCode", customerCode),
                    new SqlParameter("@CustomerID", excludeCustomerID.Value)
                } :
                new SqlParameter[] {
                    new SqlParameter("@CustomerCode", customerCode)
                };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // جلب DataTable
        public DataTable GetCustomersDataTable()
        {
            string query = @"SELECT 
                               CustomerID AS 'الرقم',
                               CustomerCode AS 'كود العميل',
                               CustomerName AS 'اسم العميل',
                               Mobile AS 'الموبايل',
                               Phone AS 'التليفون',
                               CustomerType AS 'النوع',
                               CurrentBalance AS 'الرصيد',
                               CASE WHEN IsActive = 1 THEN 'نشط' ELSE 'غير نشط' END AS 'الحالة'
                           FROM Customers 
                           ORDER BY CustomerName";

            return DatabaseConnection.ExecuteDataTable(query);
        }

        // توليد كود تلقائي
        public string GenerateCustomerCode()
        {
            string query = @"SELECT MAX(CAST(SUBSTRING(CustomerCode, 5, LEN(CustomerCode)) AS INT)) 
                           FROM Customers 
                           WHERE CustomerCode LIKE 'CUST%'";

            object result = DatabaseConnection.ExecuteScalar(query);

            int nextNumber = 1;
            if (result != null && result != DBNull.Value)
            {
                nextNumber = Convert.ToInt32(result) + 1;
            }

            return $"CUST{nextNumber:D3}";
        }

        // دوال مساعدة
        private Customer MapRowToCustomer(DataRow row)
        {
            return new Customer
            {
                CustomerID = Convert.ToInt32(row["CustomerID"]),
                CustomerCode = row["CustomerCode"].ToString(),
                CustomerName = row["CustomerName"].ToString(),
                Phone = row["Phone"].ToString(),
                Mobile = row["Mobile"].ToString(),
                Address = row["Address"].ToString(),
                Email = row["Email"].ToString(),
                CustomerType = row["CustomerType"].ToString(),
                CreditLimit = Convert.ToDecimal(row["CreditLimit"]),
                InitialBalance = Convert.ToDecimal(row["InitialBalance"]),
                CurrentBalance = Convert.ToDecimal(row["CurrentBalance"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                Notes = row["Notes"].ToString(),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private SqlParameter[] GetCustomerParameters(Customer customer)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@CustomerCode", customer.CustomerCode),
                new SqlParameter("@CustomerName", customer.CustomerName),
                new SqlParameter("@Phone", customer.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Mobile", customer.Mobile ?? (object)DBNull.Value),
                new SqlParameter("@Address", customer.Address ?? (object)DBNull.Value),
                new SqlParameter("@Email", customer.Email ?? (object)DBNull.Value),
                new SqlParameter("@CustomerType", customer.CustomerType),
                new SqlParameter("@CreditLimit", customer.CreditLimit),
                new SqlParameter("@InitialBalance", customer.InitialBalance),
                new SqlParameter("@CurrentBalance", customer.CurrentBalance),
                new SqlParameter("@IsActive", customer.IsActive),
                new SqlParameter("@Notes", customer.Notes ?? (object)DBNull.Value),
                new SqlParameter("@CreatedDate", customer.CreatedDate)
            };
        }
    }
}
