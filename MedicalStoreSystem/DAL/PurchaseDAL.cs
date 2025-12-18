using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.DAL
{
    public class PurchaseDAL
    {
        // جلب جميع المشتريات
        public DataTable GetAllPurchases()
        {
            string query = @"SELECT 
                               p.PurchaseID AS 'الرقم',
                               p.PurchaseNumber AS 'رقم الفاتورة',
                               p.PurchaseDate AS 'التاريخ',
                               s.SupplierName AS 'المورد',
                               p.SupplierInvoiceNumber AS 'رقم فاتورة المورد',
                               p.TotalAmount AS 'الإجمالي',
                               p.DiscountAmount AS 'الخصم',
                               p.NetAmount AS 'الصافي',
                               p.PaidAmount AS 'المدفوع',
                               p.RemainingAmount AS 'المتبقي',
                               p.PaymentType AS 'طريقة الدفع'
                           FROM Purchases p
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           ORDER BY p.PurchaseDate DESC, p.PurchaseID DESC";

            return DatabaseConnection.ExecuteDataTable(query);
        }

        // جلب فاتورة مشتريات بالتفاصيل
        public Purchase GetPurchaseByID(int purchaseID)
        {
            // جلب رأس الفاتورة
            string query = @"SELECT p.PurchaseID, p.PurchaseNumber, p.PurchaseDate, 
                           p.SupplierID, s.SupplierName, p.SupplierInvoiceNumber,
                           p.TotalAmount, p.DiscountAmount, p.NetAmount, 
                           p.PaidAmount, p.RemainingAmount, p.PaymentType,
                           p.UserID, p.Notes, p.CreatedDate
                           FROM Purchases p
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           WHERE p.PurchaseID = @PurchaseID";

            SqlParameter[] parameters = {
                new SqlParameter("@PurchaseID", purchaseID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Purchase purchase = new Purchase
                {
                    PurchaseID = Convert.ToInt32(row["PurchaseID"]),
                    PurchaseNumber = row["PurchaseNumber"].ToString(),
                    PurchaseDate = Convert.ToDateTime(row["PurchaseDate"]),
                    SupplierID = Convert.ToInt32(row["SupplierID"]),
                    SupplierName = row["SupplierName"].ToString(),
                    SupplierInvoiceNumber = row["SupplierInvoiceNumber"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    DiscountAmount = Convert.ToDecimal(row["DiscountAmount"]),
                    NetAmount = Convert.ToDecimal(row["NetAmount"]),
                    PaidAmount = Convert.ToDecimal(row["PaidAmount"]),
                    RemainingAmount = Convert.ToDecimal(row["RemainingAmount"]),
                    PaymentType = row["PaymentType"].ToString(),
                    UserID = Convert.ToInt32(row["UserID"]),
                    Notes = row["Notes"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                };

                // جلب تفاصيل الفاتورة
                purchase.Details = GetPurchaseDetails(purchaseID);

                return purchase;
            }

            return null;
        }

        // جلب تفاصيل فاتورة المشتريات
        public List<PurchaseDetail> GetPurchaseDetails(int purchaseID)
        {
            List<PurchaseDetail> details = new List<PurchaseDetail>();

            string query = @"SELECT pd.PurchaseDetailID, pd.PurchaseID, pd.ProductID,
                           pr.ProductName, pd.Quantity, pd.UnitPrice, pd.TotalPrice,
                           pd.ExpiryDate, pd.BatchNumber, pd.CreatedDate
                           FROM PurchaseDetails pd
                           LEFT JOIN Products pr ON pd.ProductID = pr.ProductID
                           WHERE pd.PurchaseID = @PurchaseID";

            SqlParameter[] parameters = {
                new SqlParameter("@PurchaseID", purchaseID)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    details.Add(new PurchaseDetail
                    {
                        PurchaseDetailID = Convert.ToInt32(row["PurchaseDetailID"]),
                        PurchaseID = Convert.ToInt32(row["PurchaseID"]),
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                        TotalPrice = Convert.ToDecimal(row["TotalPrice"]),
                        ExpiryDate = row["ExpiryDate"] != DBNull.Value ?
                            Convert.ToDateTime(row["ExpiryDate"]) : (DateTime?)null,
                        BatchNumber = row["BatchNumber"].ToString(),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    });
                }
            }

            return details;
        }

        // إضافة فاتورة مشتريات جديدة
        public bool InsertPurchase(Purchase purchase)
        {
            SqlConnection connection = DatabaseConnection.GetConnection();
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. إدراج رأس الفاتورة
                string queryHeader = @"INSERT INTO Purchases 
                                     (PurchaseNumber, PurchaseDate, SupplierID, SupplierInvoiceNumber,
                                     TotalAmount, DiscountAmount, NetAmount, PaidAmount, RemainingAmount,
                                     PaymentType, UserID, Notes, CreatedDate)
                                     VALUES
                                     (@PurchaseNumber, @PurchaseDate, @SupplierID, @SupplierInvoiceNumber,
                                     @TotalAmount, @DiscountAmount, @NetAmount, @PaidAmount, @RemainingAmount,
                                     @PaymentType, @UserID, @Notes, @CreatedDate);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlCommand cmdHeader = new SqlCommand(queryHeader, connection, transaction);
                cmdHeader.Parameters.AddWithValue("@PurchaseNumber", purchase.PurchaseNumber);
                cmdHeader.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                cmdHeader.Parameters.AddWithValue("@SupplierID", purchase.SupplierID);
                cmdHeader.Parameters.AddWithValue("@SupplierInvoiceNumber", purchase.SupplierInvoiceNumber ?? (object)DBNull.Value);
                cmdHeader.Parameters.AddWithValue("@TotalAmount", purchase.TotalAmount);
                cmdHeader.Parameters.AddWithValue("@DiscountAmount", purchase.DiscountAmount);
                cmdHeader.Parameters.AddWithValue("@NetAmount", purchase.NetAmount);
                cmdHeader.Parameters.AddWithValue("@PaidAmount", purchase.PaidAmount);
                cmdHeader.Parameters.AddWithValue("@RemainingAmount", purchase.RemainingAmount);
                cmdHeader.Parameters.AddWithValue("@PaymentType", purchase.PaymentType);
                cmdHeader.Parameters.AddWithValue("@UserID", purchase.UserID);
                cmdHeader.Parameters.AddWithValue("@Notes", purchase.Notes ?? (object)DBNull.Value);
                cmdHeader.Parameters.AddWithValue("@CreatedDate", purchase.CreatedDate);

                int purchaseID = Convert.ToInt32(cmdHeader.ExecuteScalar());

                // 2. إدراج التفاصيل وتحديث المخزون
                string queryDetails = @"INSERT INTO PurchaseDetails 
                                      (PurchaseID, ProductID, Quantity, UnitPrice, TotalPrice, 
                                      ExpiryDate, BatchNumber, CreatedDate)
                                      VALUES
                                      (@PurchaseID, @ProductID, @Quantity, @UnitPrice, @TotalPrice,
                                      @ExpiryDate, @BatchNumber, @CreatedDate)";

                string queryUpdateStock = @"UPDATE Products 
                                          SET CurrentStock = CurrentStock + @Quantity 
                                          WHERE ProductID = @ProductID";

                string queryStockMovement = @"INSERT INTO StockMovements 
                                            (MovementDate, ProductID, MovementType, Quantity, 
                                            ReferenceType, ReferenceID, UserID, CreatedDate)
                                            VALUES
                                            (@MovementDate, @ProductID, @MovementType, @Quantity,
                                            @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                foreach (var detail in purchase.Details)
                {
                    // إدراج التفصيل
                    SqlCommand cmdDetail = new SqlCommand(queryDetails, connection, transaction);
                    cmdDetail.Parameters.AddWithValue("@PurchaseID", purchaseID);
                    cmdDetail.Parameters.AddWithValue("@ProductID", detail.ProductID);
                    cmdDetail.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    cmdDetail.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                    cmdDetail.Parameters.AddWithValue("@TotalPrice", detail.TotalPrice);
                    cmdDetail.Parameters.AddWithValue("@ExpiryDate", detail.ExpiryDate ?? (object)DBNull.Value);
                    cmdDetail.Parameters.AddWithValue("@BatchNumber", detail.BatchNumber ?? (object)DBNull.Value);
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
                    cmdMovement.Parameters.AddWithValue("@MovementType", "إضافة");
                    cmdMovement.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    cmdMovement.Parameters.AddWithValue("@ReferenceType", "مشتريات");
                    cmdMovement.Parameters.AddWithValue("@ReferenceID", purchaseID);
                    cmdMovement.Parameters.AddWithValue("@UserID", purchase.UserID);
                    cmdMovement.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmdMovement.ExecuteNonQuery();
                }

                // 3. تسجيل في الخزنة (إذا دفع نقدي)
                if (purchase.PaidAmount > 0)
                {
                    string queryCash = @"INSERT INTO CashTransactions 
                                       (TransactionDate, TransactionType, Amount, Description,
                                       ReferenceType, ReferenceID, UserID, CreatedDate)
                                       VALUES
                                       (@TransactionDate, @TransactionType, @Amount, @Description,
                                       @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                    SqlCommand cmdCash = new SqlCommand(queryCash, connection, transaction);
                    cmdCash.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                    cmdCash.Parameters.AddWithValue("@TransactionType", "مشتريات");
                    cmdCash.Parameters.AddWithValue("@Amount", -purchase.PaidAmount); // سالب لأنه صرف
                    cmdCash.Parameters.AddWithValue("@Description", $"دفع مشتريات - فاتورة رقم {purchase.PurchaseNumber}");
                    cmdCash.Parameters.AddWithValue("@ReferenceType", "مشتريات");
                    cmdCash.Parameters.AddWithValue("@ReferenceID", purchaseID);
                    cmdCash.Parameters.AddWithValue("@UserID", purchase.UserID);
                    cmdCash.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmdCash.ExecuteNonQuery();
                }

                // 4. تحديث رصيد المورد (إذا كان آجل)
                if (purchase.RemainingAmount > 0)
                {
                    string queryUpdateSupplier = @"UPDATE Suppliers 
                                                 SET CurrentBalance = CurrentBalance + @Amount 
                                                 WHERE SupplierID = @SupplierID";

                    SqlCommand cmdSupplier = new SqlCommand(queryUpdateSupplier, connection, transaction);
                    cmdSupplier.Parameters.AddWithValue("@Amount", purchase.RemainingAmount);
                    cmdSupplier.Parameters.AddWithValue("@SupplierID", purchase.SupplierID);
                    cmdSupplier.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();

                System.Windows.Forms.MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        // توليد رقم فاتورة تلقائي
        public string GeneratePurchaseNumber()
        {
            string query = @"SELECT MAX(CAST(SUBSTRING(PurchaseNumber, 4, LEN(PurchaseNumber)) AS INT)) 
                           FROM Purchases 
                           WHERE PurchaseNumber LIKE 'PUR%'";

            object result = DatabaseConnection.ExecuteScalar(query);

            int nextNumber = 1;
            if (result != null && result != DBNull.Value)
            {
                nextNumber = Convert.ToInt32(result) + 1;
            }

            return $"PUR{nextNumber:D6}"; // PUR000001, PUR000002, etc.
        }
    }
}
