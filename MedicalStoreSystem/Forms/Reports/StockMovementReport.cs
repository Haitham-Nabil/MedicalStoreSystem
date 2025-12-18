using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;

namespace MedicalStoreSystem.Forms.Reports
{
    public partial class StockMovementReport : Form
    {
        public StockMovementReport()
        {
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void StockMovementReport_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;

            cmbMovementType.Items.Clear();
            cmbMovementType.Items.Add("الكل");
            cmbMovementType.Items.Add("Purchase");
            cmbMovementType.Items.Add("Sale");
            cmbMovementType.Items.Add("Return");
            cmbMovementType.Items.Add("Adjustment");
            cmbMovementType.SelectedIndex = 0;

            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                string query = @"
                    SELECT 
                        sm.MovementDate AS 'التاريخ',
                        p.ProductName AS 'المنتج',
                        sm.MovementType AS 'نوع الحركة',
                        sm.Quantity AS 'الكمية',
                        sm.Notes AS 'ملاحظات',
                        u.UserName AS 'المستخدم'
                    FROM StockMovements sm
                    INNER JOIN Products p ON sm.ProductID = p.ProductID
                    LEFT JOIN Users u ON sm.UserID = u.UserID
                    WHERE sm.MovementDate BETWEEN @FromDate AND @ToDate";

                if (cmbMovementType.SelectedIndex > 0)
                {
                    query += " AND sm.MovementType = @MovementType";
                }

                query += " ORDER BY sm.MovementDate DESC";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                    new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
                };

                DataTable dt;
                if (cmbMovementType.SelectedIndex > 0)
                {
                    dt = DatabaseConnection.ExecuteDataTable(query, new SqlParameter[] {
                        parameters[0],
                        parameters[1],
                        new SqlParameter("@MovementType", cmbMovementType.SelectedItem.ToString())
                    });
                }
                else
                {
                    dt = DatabaseConnection.ExecuteDataTable(query, parameters);
                }

                dgvReport.DataSource = dt;

                // تنسيق الألوان حسب نوع الحركة
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.IsNewRow) continue;
                    string movementType = row.Cells["نوع الحركة"].Value.ToString();
                    switch (movementType)
                    {
                        case "Purchase":
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "Sale":
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            break;
                        case "Return":
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                            break;
                        case "Adjustment":
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                            break;
                    }
                }

                lblTotal.Text = $"إجمالي الحركات: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل التقرير:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // نفس كود التصدير السابق
        }
    }
}
