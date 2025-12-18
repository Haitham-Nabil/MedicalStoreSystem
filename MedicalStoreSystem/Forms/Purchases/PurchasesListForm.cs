using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Forms.Printing;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace MedicalStoreSystem.Forms.Purchases
{
    public partial class PurchasesListForm : Form
    {
        private PurchaseDAL purchaseDAL = new PurchaseDAL();

        public PurchasesListForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void PurchasesListForm_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            LoadPurchases();
        }

        private void LoadPurchases()
        {
            try
            {
                DataTable dt = purchaseDAL.GetAllPurchases();
                dgvPurchases.DataSource = dt;

                if (dgvPurchases.Columns["الرقم"] != null)
                {
                    dgvPurchases.Columns["الرقم"].Visible = false;
                }

                dgvPurchases.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // حساب الإجمالي
                CalculateTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل البيانات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotal()
        {
            try
            {
                decimal totalAmount = 0;
                int count = dgvPurchases.Rows.Count;

                foreach (DataGridViewRow row in dgvPurchases.Rows)
                {
                    if (row.Cells["الصافي"].Value != null)
                    {
                        totalAmount += Convert.ToDecimal(row.Cells["الصافي"].Value);
                    }
                }

                lblTotal.Text = $"عدد الفواتير: {count}";
                lblTotalAmount.Text = $"الإجمالي: {totalAmount:F2} جنيه";
            }
            catch { }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string query = $@"SELECT 
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
                               WHERE p.PurchaseDate BETWEEN '{dtpFromDate.Value:yyyy-MM-dd}' 
                                   AND '{dtpToDate.Value:yyyy-MM-dd}'
                               ORDER BY p.PurchaseDate DESC, p.PurchaseID DESC";

                dgvPurchases.DataSource = DAL.DatabaseConnection.ExecuteDataTable(query);
                CalculateTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في البحث:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            LoadPurchases();
        }

        private void btnNewPurchase_Click(object sender, EventArgs e)
        {
            PurchaseForm purchaseForm = new PurchaseForm();
            purchaseForm.ShowDialog();
            LoadPurchases(); // تحديث البيانات بعد الإغلاق
        }

        /*private void dgvPurchases_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    int purchaseID = Convert.ToInt32(dgvPurchases.Rows[e.RowIndex].Cells["الرقم"].Value);

                    // عرض تفاصيل الفاتورة
                    var purchase = purchaseDAL.GetPurchaseByID(purchaseID);

                    if (purchase != null)
                    {
                        string details = $"رقم الفاتورة: {purchase.PurchaseNumber}\n";
                        details += $"التاريخ: {purchase.PurchaseDate:yyyy-MM-dd}\n";
                        details += $"المورد: {purchase.SupplierName}\n";
                        details += $"الصافي: {purchase.NetAmount:F2} جنيه\n\n";
                        details += "الأصناف:\n";
                        details += "═══════════════════\n";

                        foreach (var detail in purchase.Details)
                        {
                            details += $"• {detail.ProductName}\n";
                            details += $"  الكمية: {detail.Quantity} × {detail.UnitPrice:F2} = {detail.TotalPrice:F2} جنيه\n";
                            if (detail.ExpiryDate.HasValue)
                                details += $"  الصلاحية: {detail.ExpiryDate:yyyy-MM-dd}\n";
                            details += "\n";
                        }

                        MessageBox.Show(details, "تفاصيل الفاتورة",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }*/

        // امر الطباعه الجديد
        // في dgvPurchases_CellDoubleClick استبدل الكود بالتالي:
        private void dgvPurchases_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int purchaseID = Convert.ToInt32(dgvPurchases.Rows[e.RowIndex].Cells["PurchaseID"].Value);

                DialogResult result = MessageBox.Show("اختر الإجراء:\n\nنعم: طباعة الفاتورة\nلا: عرض التفاصيل فقط",
                    "إجراء", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    PrintPurchaseInvoice(purchaseID);
                }
                else if (result == DialogResult.No)
                {
                    ShowPurchaseDetails(purchaseID);
                }
            }
        }

        private void PrintPurchaseInvoice(int purchaseID)
        {
            try
            {
                // جلب بيانات الفاتورة
                string queryInvoice = @"
            SELECT p.PurchaseID, p.PurchaseNumber, p.PurchaseDate, p.SupplierInvoiceNumber,
                   p.TotalAmount, p.DiscountAmount, p.NetAmount, p.PaidAmount, p.RemainingAmount,
                   s.SupplierName, u.UserName
            FROM Purchases p
            INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
            INNER JOIN Users u ON p.UserID = u.UserID
            WHERE p.PurchaseID = @PurchaseID";

                SqlParameter[] paramsInvoice = { new SqlParameter("@PurchaseID", purchaseID) };
                DataTable dtInvoice = DatabaseConnection.ExecuteDataTable(queryInvoice, paramsInvoice);

                // جلب أصناف الفاتورة
                string queryItems = @"
            SELECT pd.PurchaseDetailID, pr.ProductName, pd.Quantity, pd.UnitPrice, 
                   pd.TotalPrice, pd.ExpiryDate, pd.BatchNumber
            FROM PurchaseDetails pd
            INNER JOIN Products pr ON pd.ProductID = pr.ProductID
            WHERE pd.PurchaseID = @PurchaseID
            ORDER BY pd.PurchaseDetailID";

                SqlParameter[] paramsItems = { new SqlParameter("@PurchaseID", purchaseID) };
                DataTable dtItems = DatabaseConnection.ExecuteDataTable(queryItems, paramsItems);

                // طباعة الفاتورة
                PurchaseInvoicePrint printInvoice = new PurchaseInvoicePrint(dtInvoice, dtItems);
                printInvoice.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في طباعة الفاتورة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowPurchaseDetails(int purchaseID)
        {
            try
            {
                // عرض التفاصيل كما كان من قبل
                string query = @"
            SELECT p.PurchaseNumber, p.PurchaseDate, s.SupplierName, p.SupplierInvoiceNumber,
                   p.TotalAmount, p.DiscountAmount, p.NetAmount, p.PaidAmount, p.RemainingAmount,
                   u.UserName, p.Notes
            FROM Purchases p
            INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
            INNER JOIN Users u ON p.UserID = u.UserID
            WHERE p.PurchaseID = @PurchaseID";

                SqlParameter[] parameters = { new SqlParameter("@PurchaseID", purchaseID) };
                DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string details = $"رقم الفاتورة: {row["PurchaseNumber"]}\n" +
                                   $"التاريخ: {Convert.ToDateTime(row["PurchaseDate"]):yyyy/MM/dd}\n" +
                                   $"المورد: {row["SupplierName"]}\n" +
                                   $"فاتورة المورد: {row["SupplierInvoiceNumber"]}\n" +
                                   $"الإجمالي: {row["TotalAmount"]:N2} ريال\n" +
                                   $"الخصم: {row["DiscountAmount"]:N2} ريال\n" +
                                   $"الصافي: {row["NetAmount"]:N2} ريال\n" +
                                   $"المدفوع: {row["PaidAmount"]:N2} ريال\n" +
                                   $"المتبقي: {row["RemainingAmount"]:N2} ريال\n" +
                                   $"المستخدم: {row["UserName"]}\n" +
                                   $"ملاحظات: {row["Notes"]}";

                    MessageBox.Show(details, "تفاصيل فاتورة المشتريات",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في عرض التفاصيل:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //نهاية أمر الطباعه الجديد

    }
}
