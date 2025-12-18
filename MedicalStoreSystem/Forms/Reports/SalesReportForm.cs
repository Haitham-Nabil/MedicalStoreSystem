using System;
using System.Data;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;

namespace MedicalStoreSystem.Forms.Reports
{
    public partial class SalesReportForm : Form
    {
        public SalesReportForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SalesReportForm_Load(object sender, EventArgs e)
        {
            // آخر 30 يوم
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string query = $@"
                    SELECT 
                        CAST(s.SaleDate AS DATE) AS 'التاريخ',
                        COUNT(s.SaleID) AS 'عدد الفواتير',
                        SUM(s.TotalAmount) AS 'الإجمالي',
                        SUM(s.DiscountAmount) AS 'الخصم',
                        SUM(s.NetAmount) AS 'الصافي',
                        SUM((sd.UnitPrice - p.CostPrice) * sd.Quantity) AS 'الربح'
                    FROM Sales s
                    INNER JOIN SalesDetails sd ON s.SaleID = sd.SaleID
                    INNER JOIN Products p ON sd.ProductID = p.ProductID
                    WHERE s.SaleDate BETWEEN '{dtpFromDate.Value:yyyy-MM-dd}' 
                        AND '{dtpToDate.Value:yyyy-MM-dd} 23:59:59'
                    GROUP BY CAST(s.SaleDate AS DATE)
                    ORDER BY CAST(s.SaleDate AS DATE) DESC";

                DataTable dt = DatabaseConnection.ExecuteDataTable(query);
                dgvReport.DataSource = dt;

                dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // حساب الإجماليات
                if (dt != null && dt.Rows.Count > 0)
                {
                    int totalInvoices = 0;
                    decimal totalSales = 0;
                    decimal totalProfit = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        totalInvoices += Convert.ToInt32(row["عدد الفواتير"]);
                        totalSales += Convert.ToDecimal(row["الصافي"]);
                        totalProfit += Convert.ToDecimal(row["الربح"]);
                    }

                    lblTotalInvoices.Text = totalInvoices.ToString();
                    lblTotalSales.Text = totalSales.ToString("F2") + " جنيه";
                    lblTotalProfit.Text = totalProfit.ToString("F2") + " جنيه";
                }
                else
                {
                    lblTotalInvoices.Text = "0";
                    lblTotalSales.Text = "0.00 جنيه";
                    lblTotalProfit.Text = "0.00 جنيه";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في إنشاء التقرير:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
