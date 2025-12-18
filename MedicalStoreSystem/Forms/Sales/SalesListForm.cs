using System;
using System.Data;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;

namespace MedicalStoreSystem.Forms.Sales
{
    public partial class SalesListForm : Form
    {
        private SaleDAL saleDAL = new SaleDAL();

        public SalesListForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SalesListForm_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            LoadSales();
        }

        private void LoadSales()
        {
            try
            {
                DataTable dt = saleDAL.GetAllSales();
                dgvSales.DataSource = dt;

                if (dgvSales.Columns["الرقم"] != null)
                {
                    dgvSales.Columns["الرقم"].Visible = false;
                }

                dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // تلوين الصفوف حسب حالة الدفع
                foreach (DataGridViewRow row in dgvSales.Rows)
                {
                    //the next line add with chatgpt
                    if (row.IsNewRow) continue;
                    if (row.Cells["حالة الدفع"].Value.ToString() == "آجل")
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                    }
                    /*    
                     *    var value = row.Cells["حالة البند"]?.Value?.ToString()?.Trim();

                           if (!string.IsNullOrEmpty(value) && value == "آجل")
                           {
                                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                           }
                    */
                }

                // حساب الإجماليات
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل البيانات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals()
        {
            try
            {
                decimal totalAmount = 0;
                decimal totalProfit = 0;
                int count = dgvSales.Rows.Count;

                foreach (DataGridViewRow row in dgvSales.Rows)
                {
                    if (row.Cells["الصافي"].Value != null)
                    {
                        totalAmount += Convert.ToDecimal(row.Cells["الصافي"].Value);
                    }
                }

                // حساب الربح (يحتاج استعلام إضافي)
                totalProfit = CalculateProfit();

                lblTotal.Text = $"عدد الفواتير: {count}";
                lblTotalAmount.Text = $"الإجمالي: {totalAmount:F2} جنيه";
                lblProfit.Text = $"الربح التقديري: {totalProfit:F2} جنيه";
            }
            catch { }
        }

        private decimal CalculateProfit()
        {
            try
            {
                string query = @"SELECT ISNULL(SUM((sd.UnitPrice - p.CostPrice) * sd.Quantity), 0)
                               FROM SalesDetails sd
                               INNER JOIN Sales s ON sd.SaleID = s.SaleID
                               INNER JOIN Products p ON sd.ProductID = p.ProductID";

                object result = DAL.DatabaseConnection.ExecuteScalar(query);
                return result != null ? Convert.ToDecimal(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string query = $@"SELECT 
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
                               WHERE s.SaleDate BETWEEN '{dtpFromDate.Value:yyyy-MM-dd}' 
                                   AND '{dtpToDate.Value:yyyy-MM-dd} 23:59:59'
                               ORDER BY s.SaleDate DESC, s.SaleID DESC";

                dgvSales.DataSource = DAL.DatabaseConnection.ExecuteDataTable(query);

                if (dgvSales.Columns["الرقم"] != null)
                {
                    dgvSales.Columns["الرقم"].Visible = false;
                }

                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في البحث:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            LoadSales();
        }

        private void btnTodaySales_Click(object sender, EventArgs e)
        {
            try
            {
                dgvSales.DataSource = saleDAL.GetTodaySales();

                if (dgvSales.Columns["الرقم"] != null)
                {
                    dgvSales.Columns["الرقم"].Visible = false;
                }

                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewSale_Click(object sender, EventArgs e)
        {
            POSForm posForm = new POSForm();
            posForm.ShowDialog();
            LoadSales(); // تحديث البيانات
        }

        private void dgvSales_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    int saleID = Convert.ToInt32(dgvSales.Rows[e.RowIndex].Cells["الرقم"].Value);

                    // عرض تفاصيل الفاتورة
                    var sale = saleDAL.GetSaleByID(saleID);

                    if (sale != null)
                    {
                        string details = "═══════════════════════════════════\n";
                        details += $"رقم الفاتورة: {sale.SaleNumber}\n";
                        details += $"التاريخ: {sale.SaleDate:yyyy-MM-dd HH:mm}\n";
                        details += $"العميل: {sale.CustomerName}\n";
                        details += $"طريقة الدفع: {sale.PaymentType}\n";
                        details += "───────────────────────────────────\n\n";
                        details += "الأصناف:\n";
                        details += "═══════════════════════════════════\n";

                        foreach (var detail in sale.Details)
                        {
                            details += $"• {detail.ProductName}\n";
                            details += $"  {detail.Quantity} × {detail.UnitPrice:F2} = {detail.TotalPrice:F2} جنيه\n\n";
                        }

                        details += "═══════════════════════════════════\n";
                        details += $"الإجمالي:        {sale.TotalAmount:F2} جنيه\n";

                        if (sale.DiscountAmount > 0)
                            details += $"الخصم:           {sale.DiscountAmount:F2} جنيه\n";

                        details += $"الصافي:          {sale.NetAmount:F2} جنيه\n";
                        details += $"المدفوع:         {sale.PaidAmount:F2} جنيه\n";

                        if (sale.RemainingAmount > 0)
                            details += $"المتبقي:         {sale.RemainingAmount:F2} جنيه\n";

                        details += "═══════════════════════════════════\n";

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
        }
    }
}
