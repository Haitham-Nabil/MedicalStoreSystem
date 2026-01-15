using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.DAL
{
    public class SaleDAL
    {
        // جلب جميع المبيعات
        public DataTable GetAllSales()
        {
            string query = @"SELECT 
                               s.SaleID AS 'الرقم',
                               s.SaleNumber AS 'رقم الفاتورة',
                               s.SaleDate AS 'التاريخ',
                               c.CustomerName AS 'العميل',
                               s.TotalAmount AS 'الإجمالي',
                               s.DiscountAmount AS 'الخصم',
                               s.NetAmount AS 'الصافي',
                               s.PaidAmount AS 'المدفوع',
                               s.RemainingAmount AS 'المتبقي',
                               s.PaymentType AS 'طريقة الدفع',
                               CASE WHEN s.IsPaid = 1 THEN 'مدفوع' ELSE 'آجل' END AS 'حالة الدفع'
                           FROM Sales s
                           LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                           ORDER BY s.SaleDate DESC, s.SaleID DESC";

            return DatabaseConnection.ExecuteDataTable(query);
        }

        // جلب فاتورة مبيعات بالتفاصيل
        public Sale GetSaleByID(int saleID)
        {
            // جلب رأس الفاتورة
            string query = @"SELECT s.SaleID, s.SaleNumber, s.SaleDate, 
                           s.CustomerID, c.CustomerName, s.TotalAmount, 
                           s.DiscountAmount, s.NetAmount, s.PaidAmount, 
                           s.RemainingAmount, s.PaymentType, s.IsPaid,
                           s.UserID, s.Notes, s.CreatedDate
                           FROM Sales s
                           LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                           WHERE s.SaleID = @SaleID";

            SqlParameter[] parameters = {
                new SqlParameter("@SaleID", saleID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Sale sale = new Sale
                {
                    SaleID = Convert.ToInt32(row["SaleID"]),
                    SaleNumber = row["SaleNumber"].ToString(),
                    SaleDate = Convert.ToDateTime(row["SaleDate"]),
                    CustomerID = Convert.ToInt32(row["CustomerID"]),
                    CustomerName = row["CustomerName"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    DiscountAmount = Convert.ToDecimal(row["DiscountAmount"]),
                    NetAmount = Convert.ToDecimal(row["NetAmount"]),
                    PaidAmount = Convert.ToDecimal(row["PaidAmount"]),
                    RemainingAmount = Convert.ToDecimal(row["RemainingAmount"]),
                    PaymentType = row["PaymentType"].ToString(),
                    IsPaid = Convert.ToBoolean(row["IsPaid"]),
                    UserID = Convert.ToInt32(row["UserID"]),
                    Notes = row["Notes"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                };

                // جلب تفاصيل الفاتورة
                sale.Details = GetSaleDetails(saleID);

                return sale;
            }

            return null;
        }

        // جلب تفاصيل فاتورة المبيعات
        public List<SaleDetail> GetSaleDetails(int saleID)
        {
            List<SaleDetail> details = new List<SaleDetail>();

            string query = @"SELECT sd.SaleDetailID, sd.SaleID, sd.ProductID,
                           pr.ProductName, sd.Quantity, sd.UnitPrice, sd.TotalPrice,
                           sd.CreatedDate
                           FROM SalesDetails sd
                           LEFT JOIN Products pr ON sd.ProductID = pr.ProductID
                           WHERE sd.SaleID = @SaleID";

            SqlParameter[] parameters = {
                new SqlParameter("@SaleID", saleID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    details.Add(new SaleDetail
                    {
                        SaleDetailID = Convert.ToInt32(row["SaleDetailID"]),
                        SaleID = Convert.ToInt32(row["SaleID"]),
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                        TotalPrice = Convert.ToDecimal(row["TotalPrice"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    });
                }
            }

            return details;
        }

        // إضافة فاتورة مبيعات جديدة
        public int InsertSale(Sale sale)
        {
            SqlConnection connection = DatabaseConnection.GetConnection();
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. إدراج رأس الفاتورة
                string queryHeader = @"INSERT INTO Sales 
                                     (SaleNumber, SaleDate, CustomerID, TotalAmount, 
                                     DiscountAmount, NetAmount, PaidAmount, RemainingAmount,
                                     PaymentType, IsPaid, UserID, Notes, CreatedDate)
                                     VALUES
                                     (@SaleNumber, @SaleDate, @CustomerID, @TotalAmount,
                                     @DiscountAmount, @NetAmount, @PaidAmount, @RemainingAmount,
                                     @PaymentType, @IsPaid, @UserID, @Notes, @CreatedDate);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlCommand cmdHeader = new SqlCommand(queryHeader, connection, transaction);
                cmdHeader.Parameters.AddWithValue("@SaleNumber", sale.SaleNumber);
                cmdHeader.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                cmdHeader.Parameters.AddWithValue("@CustomerID", sale.CustomerID);
                cmdHeader.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);
                cmdHeader.Parameters.AddWithValue("@DiscountAmount", sale.DiscountAmount);
                cmdHeader.Parameters.AddWithValue("@NetAmount", sale.NetAmount);
                cmdHeader.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
                cmdHeader.Parameters.AddWithValue("@RemainingAmount", sale.RemainingAmount);
                cmdHeader.Parameters.AddWithValue("@PaymentType", sale.PaymentType);
                cmdHeader.Parameters.AddWithValue("@IsPaid", sale.IsPaid);
                cmdHeader.Parameters.AddWithValue("@UserID", sale.UserID);
                cmdHeader.Parameters.AddWithValue("@Notes", sale.Notes ?? (object)DBNull.Value);
                cmdHeader.Parameters.AddWithValue("@CreatedDate", sale.CreatedDate);

                int saleID = Convert.ToInt32(cmdHeader.ExecuteScalar());

                // 2. إدراج التفاصيل وتحديث المخزون
                string queryDetails = @"INSERT INTO SalesDetails 
                                      (SaleID, ProductID, Quantity, UnitPrice, TotalPrice, CreatedDate)
                                      VALUES
                                      (@SaleID, @ProductID, @Quantity, @UnitPrice, @TotalPrice, @CreatedDate)";

                string queryUpdateStock = @"UPDATE Products 
                                          SET CurrentStock = CurrentStock - @Quantity 
                                          WHERE ProductID = @ProductID";

                string queryStockMovement = @"INSERT INTO StockMovements 
                                            (MovementDate, ProductID, MovementType, Quantity, 
                                            ReferenceType, ReferenceID, UserID, CreatedDate)
                                            VALUES
                                            (@MovementDate, @ProductID, @MovementType, @Quantity,
                                            @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                foreach (var detail in sale.Details)
                {
                    // التحقق من توفر الكمية
                    string queryCheckStock = @"SELECT CurrentStock FROM Products WHERE ProductID = @ProductID";
                    SqlCommand cmdCheck = new SqlCommand(queryCheckStock, connection, transaction);
                    cmdCheck.Parameters.AddWithValue("@ProductID", detail.ProductID);
                    int currentStock = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (currentStock < detail.Quantity)
                    {
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show(
                            $"الكمية المتاحة من '{detail.ProductName}' غير كافية!\nالمتاح: {currentStock}",
                            "تنبيه", System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning);
                        return -1;
                    }

                    // إدراج التفصيل
                    SqlCommand cmdDetail = new SqlCommand(queryDetails, connection, transaction);
                    cmdDetail.Parameters.AddWithValue("@SaleID", saleID);
                    cmdDetail.Parameters.AddWithValue("@ProductID", detail.ProductID);
                    cmdDetail.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    cmdDetail.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                    cmdDetail.Parameters.AddWithValue("@TotalPrice", detail.TotalPrice);
                    cmdDetail.Parameters.AddWithValue("@CreatedDate", detail.CreatedDate);
                    cmdDetail.ExecuteNonQuery();

                    // تحديث المخزون
                    SqlCommand cmdStock = new SqlCommand(queryUpdateStock, connection, transaction);
                    cmdStock.Parameters.AddWithValue("@ProductID", detail.ProductID);
                    cmdStock.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    cmdStock.ExecuteNonQuery();

                    // تسجيل حركة المخزن
                    SqlCommand cmdMovement = new SqlCommand(queryStockMovement, connection, transaction);
                    cmdMovement.Parameters.AddWithValue("@MovementDate", DateTime.Now);
                    cmdMovement.Parameters.AddWithValue("@ProductID", detail.ProductID);
                    cmdMovement.Parameters.AddWithValue("@MovementType", "سحب");
                    cmdMovement.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    cmdMovement.Parameters.AddWithValue("@ReferenceType", "مبيعات");
                    cmdMovement.Parameters.AddWithValue("@ReferenceID", saleID);
                    cmdMovement.Parameters.AddWithValue("@UserID", sale.UserID);
                    cmdMovement.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmdMovement.ExecuteNonQuery();
                }

                // 3. تسجيل في الخزنة
                if (sale.PaidAmount > 0)
                {
                    string queryCash = @"INSERT INTO CashTransactions 
                                       (TransactionDate, TransactionType, Amount, Description,
                                       ReferenceType, ReferenceID, UserID, CreatedDate)
                                       VALUES
                                       (@TransactionDate, @TransactionType, @Amount, @Description,
                                       @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                    SqlCommand cmdCash = new SqlCommand(queryCash, connection, transaction);
                    cmdCash.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                    cmdCash.Parameters.AddWithValue("@TransactionType", "مبيعات");
                    cmdCash.Parameters.AddWithValue("@Amount", sale.PaidAmount); // موجب لأنه إيداع
                    cmdCash.Parameters.AddWithValue("@Description", $"تحصيل مبيعات - فاتورة رقم {sale.SaleNumber}");
                    cmdCash.Parameters.AddWithValue("@ReferenceType", "مبيعات");
                    cmdCash.Parameters.AddWithValue("@ReferenceID", saleID);
                    cmdCash.Parameters.AddWithValue("@UserID", sale.UserID);
                    cmdCash.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmdCash.ExecuteNonQuery();
                }

                // 4. تحديث رصيد العميل (إذا كان آجل)
                if (sale.RemainingAmount > 0)
                {
                    string queryUpdateCustomer = @"UPDATE Customers 
                                                 SET CurrentBalance = CurrentBalance + @Amount 
                                                 WHERE CustomerID = @CustomerID";

                    SqlCommand cmdCustomer = new SqlCommand(queryUpdateCustomer, connection, transaction);
                    cmdCustomer.Parameters.AddWithValue("@Amount", sale.RemainingAmount);
                    cmdCustomer.Parameters.AddWithValue("@CustomerID", sale.CustomerID);
                    cmdCustomer.ExecuteNonQuery();
                }

                transaction.Commit();
                return saleID;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();

                System.Windows.Forms.MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return -1;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        // توليد رقم فاتورة تلقائي
        public string GenerateSaleNumber()
        {
            string query = @"SELECT MAX(CAST(SUBSTRING(SaleNumber, 4, LEN(SaleNumber)) AS INT)) 
                           FROM Sales 
                           WHERE SaleNumber LIKE 'SAL%'";

            object result = DatabaseConnection.ExecuteScalar(query);

            int nextNumber = 1;
            if (result != null && result != DBNull.Value)
            {
                nextNumber = Convert.ToInt32(result) + 1;
            }

            return $"SAL{nextNumber:D6}"; // SAL000001, SAL000002, etc.
        }

        // جلب مبيعات اليوم
        public DataTable GetTodaySales()
        {
            string query = @"SELECT 
                               s.SaleID AS 'الرقم',
                               s.SaleNumber AS 'رقم الفاتورة',
                               s.SaleDate AS 'الوقت',
                               c.CustomerName AS 'العميل',
                               s.NetAmount AS 'الصافي',
                               s.PaymentType AS 'طريقة الدفع'
                           FROM Sales s
                           LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                           WHERE CAST(s.SaleDate AS DATE) = CAST(GETDATE() AS DATE)
                           ORDER BY s.SaleDate DESC";

            return DatabaseConnection.ExecuteDataTable(query);
        }

        // حساب مبيعات اليوم
        public decimal GetTodaySalesTotal()
        {
            string query = @"SELECT ISNULL(SUM(NetAmount), 0) 
                           FROM Sales 
                           WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE)";

            object result = DatabaseConnection.ExecuteScalar(query);
            return result != null ? Convert.ToDecimal(result) : 0;
        }
    }
}