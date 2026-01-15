using System;
using System.Data;
using System.Data.SqlClient;

namespace MedicalStoreSystem.DAL
{
    public class SaleReportDAL
    {
        /// <summary>
        /// جلب بيانات فاتورة المبيعات لتقرير RDLC
        /// </summary>
        public static DataSet GetSaleInvoiceDataForRDLC(int saleID)
        {
            try
            {
                DataSet ds = new DataSet("SaleInvoiceDataSet");

                // 1. جلب معلومات الفاتورة (Header)
                string queryHeader = @"
                    SELECT 
                        s.SaleNumber,
                        s.SaleDate,
                        ISNULL(c.CustomerName, 'عميل نقدي') AS CustomerName,
                        ISNULL(c.Mobile, '') AS CustomerMobile,
                        u.UserName AS CashierName,
                        s.TotalAmount,
                        s.DiscountAmount,
                        s.NetAmount,
                        s.PaidAmount,
                        s.RemainingAmount,
                        CASE s.PaymentType
                            WHEN 'Cash' THEN 'نقدي'
                            WHEN 'Credit' THEN 'آجل'
                            WHEN 'Visa' THEN 'شبكة'
                            ELSE s.PaymentType
                        END AS PaymentType,
                        ISNULL(s.Notes, '') AS Notes,
                        'شركة المصرية للمستلزمات الطبية' AS CompanyName,
                        'شارع مستشفي الحجاز امام الإدارة الصحية' AS CompanyAddress,
                        '01010004312' AS CompanyPhone,
                        'xxxxxxxx' AS CompanyTaxNumber
                    FROM Sales s
                    LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                    INNER JOIN Users u ON s.UserID = u.UserID
                    WHERE s.SaleID = @SaleID";

                SqlParameter[] paramsHeader = { new SqlParameter("@SaleID", saleID) };
                DataTable dtHeader = DatabaseConnection.ExecuteDataTable(queryHeader, paramsHeader);
                dtHeader.TableName = "dtSaleHeader";
                ds.Tables.Add(dtHeader);

                // 2. جلب تفاصيل الأصناف (Details)
                string queryDetails = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY sd.SaleDetailID) AS RowNumber,
                        p.ProductName,
                        sd.Quantity,
                        sd.UnitPrice,
                        sd.TotalPrice
                    FROM SalesDetails sd
                    INNER JOIN Products p ON sd.ProductID = p.ProductID
                    WHERE sd.SaleID = @SaleID
                    ORDER BY sd.SaleDetailID";

                SqlParameter[] paramsDetails = { new SqlParameter("@SaleID", saleID) };
                DataTable dtDetails = DatabaseConnection.ExecuteDataTable(queryDetails, paramsDetails);
                dtDetails.TableName = "dtSaleDetails";
                ds.Tables.Add(dtDetails);

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في جلب بيانات التقرير: {ex.Message}");
            }
        }
    }
}
