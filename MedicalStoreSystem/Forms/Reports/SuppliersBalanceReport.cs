using System;
using System.Data;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;

namespace MedicalStoreSystem.Forms.Reports
{
    public partial class SuppliersBalanceReport : Form
    {
        public SuppliersBalanceReport()
        {
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SuppliersBalanceReport_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                string query = @"
                    SELECT 
                        SupplierCode AS 'كود المورد',
                        SupplierName AS 'اسم المورد',
                        Mobile AS 'الجوال',
                        Address AS 'العنوان',
                        CurrentBalance AS 'الرصيد الحالي',
                        CASE 
                            WHEN CurrentBalance > 0 THEN 'له'
                            WHEN CurrentBalance < 0 THEN 'علينا'
                            ELSE 'متساوي'
                        END AS 'الحالة'
                    FROM Suppliers
                    WHERE IsActive = 1
                    ORDER BY CurrentBalance DESC";

                DataTable dt = DatabaseConnection.ExecuteDataTable(query, null);
                dgvReport.DataSource = dt;

                // تنسيق الألوان
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    decimal balance = Convert.ToDecimal(row.Cells["الرصيد الحالي"].Value);
                    if (balance > 0)
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral; // علينا
                    else if (balance < 0)
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen; // لنا
                }

                // حساب الإجمالي
                decimal totalBalance = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalBalance += Convert.ToDecimal(row["الرصيد الحالي"]);
                }

                lblTotal.Text = $"إجمالي الأرصدة: {totalBalance:N2} ريال";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل التقرير:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV Files|*.csv";
                sfd.FileName = $"تقرير_أرصدة_الموردين_{DateTime.Now:yyyyMMdd}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    for (int i = 0; i < dgvReport.Columns.Count; i++)
                    {
                        sb.Append(dgvReport.Columns[i].HeaderText);
                        if (i < dgvReport.Columns.Count - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();

                    foreach (DataGridViewRow row in dgvReport.Rows)
                    {
                        for (int i = 0; i < dgvReport.Columns.Count; i++)
                        {
                            sb.Append(row.Cells[i].Value?.ToString() ?? "");
                            if (i < dgvReport.Columns.Count - 1)
                                sb.Append(",");
                        }
                        sb.AppendLine();
                    }

                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                    MessageBox.Show("تم تصدير التقرير بنجاح!", "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصدير:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
