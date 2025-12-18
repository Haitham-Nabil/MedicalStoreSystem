using System;
using System.Data;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;

namespace MedicalStoreSystem.Forms.Reports
{
    public partial class DetailedSalesReportForm : Form
    {
        public DetailedSalesReportForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.WindowState = FormWindowState.Maximized;

            InitializeControls();
        }

        private void InitializeControls()
        {
            // Panel الفلاتر
            Panel panelFilters = new Panel();
            panelFilters.Dock = DockStyle.Top;
            panelFilters.Height = 150;
            panelFilters.BackColor = System.Drawing.Color.WhiteSmoke;

            GroupBox groupFilters = new GroupBox();
            groupFilters.Text = "الفترة والفلاتر";
            groupFilters.Location = new System.Drawing.Point(20, 10);
            groupFilters.Size = new System.Drawing.Size(900, 130);

            Label lblFrom = new Label();
            lblFrom.Text = "من تاريخ:";
            lblFrom.Location = new System.Drawing.Point(20, 30);
            lblFrom.AutoSize = true;

            DateTimePicker dtpFrom = new DateTimePicker();
            dtpFrom.Name = "dtpFrom";
            dtpFrom.Location = new System.Drawing.Point(100, 28);
            dtpFrom.Size = new System.Drawing.Size(150, 25);
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.Value = DateTime.Now.AddMonths(-1);

            Label lblTo = new Label();
            lblTo.Text = "إلى تاريخ:";
            lblTo.Location = new System.Drawing.Point(270, 30);
            lblTo.AutoSize = true;

            DateTimePicker dtpTo = new DateTimePicker();
            dtpTo.Name = "dtpTo";
            dtpTo.Location = new System.Drawing.Point(350, 28);
            dtpTo.Size = new System.Drawing.Size(150, 25);
            dtpTo.Format = DateTimePickerFormat.Short;

            Label lblReportType = new Label();
            lblReportType.Text = "نوع التقرير:";
            lblReportType.Location = new System.Drawing.Point(20, 70);
            lblReportType.AutoSize = true;

            ComboBox cmbReportType = new ComboBox();
            cmbReportType.Name = "cmbReportType";
            cmbReportType.Location = new System.Drawing.Point(100, 68);
            cmbReportType.Size = new System.Drawing.Size(200, 25);
            cmbReportType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReportType.Items.AddRange(new string[] {
                "ملخص يومي",
                "تفصيلي بالفواتير",
                "حسب المنتج",
                "حسب العميل",
                "حسب الكاشير"
            });
            cmbReportType.SelectedIndex = 0;

            Button btnGenerate = new Button();
            btnGenerate.Text = "عرض التقرير";
            btnGenerate.Location = new System.Drawing.Point(320, 66);
            btnGenerate.Size = new System.Drawing.Size(120, 30);
            btnGenerate.BackColor = System.Drawing.Color.Green;
            btnGenerate.ForeColor = System.Drawing.Color.White;
            btnGenerate.Click += BtnGenerate_Click;

            Button btnPrint = new Button();
            btnPrint.Text = "طباعة";
            btnPrint.Location = new System.Drawing.Point(450, 66);
            btnPrint.Size = new System.Drawing.Size(100, 30);
            btnPrint.Click += BtnPrint_Click;

            Button btnExport = new Button();
            btnExport.Text = "تصدير Excel";
            btnExport.Location = new System.Drawing.Point(560, 66);
            btnExport.Size = new System.Drawing.Size(100, 30);
            btnExport.Click += BtnExport_Click;

            groupFilters.Controls.Add(lblFrom);
            groupFilters.Controls.Add(dtpFrom);
            groupFilters.Controls.Add(lblTo);
            groupFilters.Controls.Add(dtpTo);
            groupFilters.Controls.Add(lblReportType);
            groupFilters.Controls.Add(cmbReportType);
            groupFilters.Controls.Add(btnGenerate);
            groupFilters.Controls.Add(btnPrint);
            groupFilters.Controls.Add(btnExport);

            panelFilters.Controls.Add(groupFilters);

            // DataGridView
            DataGridView dgvReport = new DataGridView();
            dgvReport.Name = "dgvReport";
            dgvReport.Dock = DockStyle.Fill;
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReport.ReadOnly = true;
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // StatusStrip
            StatusStrip statusStrip = new StatusStrip();

            ToolStripStatusLabel lblTotalInvoices = new ToolStripStatusLabel();
            lblTotalInvoices.Name = "lblTotalInvoices";
            lblTotalInvoices.Text = "عدد الفواتير: 0";

            ToolStripStatusLabel lblTotalSales = new ToolStripStatusLabel();
            lblTotalSales.Name = "lblTotalSales";
            lblTotalSales.Text = "إجمالي المبيعات: 0.00 جنيه";
            lblTotalSales.Spring = true;

            ToolStripStatusLabel lblTotalProfit = new ToolStripStatusLabel();
            lblTotalProfit.Name = "lblTotalProfit";
            lblTotalProfit.Text = "إجمالي الربح: 0.00 جنيه";

            statusStrip.Items.Add(lblTotalInvoices);
            statusStrip.Items.Add(lblTotalSales);
            statusStrip.Items.Add(lblTotalProfit);

            this.Controls.Add(dgvReport);
            this.Controls.Add(panelFilters);
            this.Controls.Add(statusStrip);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                DateTimePicker dtpFrom = this.Controls.Find("dtpFrom", true)[0] as DateTimePicker;
                DateTimePicker dtpTo = this.Controls.Find("dtpTo", true)[0] as DateTimePicker;
                ComboBox cmbReportType = this.Controls.Find("cmbReportType", true)[0] as ComboBox;

                string query = "";

                switch (cmbReportType.SelectedIndex)
                {
                    case 0: // ملخص يومي
                        query = $@"
                            SELECT 
                                CAST(s.SaleDate AS DATE) AS 'التاريخ',
                                COUNT(DISTINCT s.SaleID) AS 'عدد الفواتير',
                                SUM(s.TotalAmount) AS 'الإجمالي',
                                SUM(s.DiscountAmount) AS 'الخصم',
                                SUM(s.NetAmount) AS 'الصافي',
                                SUM(s.PaidAmount) AS 'المدفوع',
                                SUM((sd.UnitPrice - p.CostPrice) * sd.Quantity) AS 'الربح'
                            FROM Sales s
                            INNER JOIN SalesDetails sd ON s.SaleID = sd.SaleID
                            INNER JOIN Products p ON sd.ProductID = p.ProductID
                            WHERE s.SaleDate BETWEEN '{dtpFrom.Value:yyyy-MM-dd}' 
                                AND '{dtpTo.Value:yyyy-MM-dd} 23:59:59'
                            GROUP BY CAST(s.SaleDate AS DATE)
                            ORDER BY CAST(s.SaleDate AS DATE) DESC";
                        break;

                    case 1: // تفصيلي بالفواتير
                        query = $@"
                            SELECT 
                                s.SaleNumber AS 'رقم الفاتورة',
                                s.SaleDate AS 'التاريخ',
                                c.CustomerName AS 'العميل',
                                u.FullName AS 'الكاشير',
                                s.TotalAmount AS 'الإجمالي',
                                s.DiscountAmount AS 'الخصم',
                                s.NetAmount AS 'الصافي',
                                s.PaymentType AS 'طريقة الدفع',
                                (SELECT SUM((sd.UnitPrice - p.CostPrice) * sd.Quantity)
                                 FROM SalesDetails sd
                                 INNER JOIN Products p ON sd.ProductID = p.ProductID
                                 WHERE sd.SaleID = s.SaleID) AS 'الربح'
                            FROM Sales s
                            LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                            LEFT JOIN Users u ON s.UserID = u.UserID
                            WHERE s.SaleDate BETWEEN '{dtpFrom.Value:yyyy-MM-dd}' 
                                AND '{dtpTo.Value:yyyy-MM-dd} 23:59:59'
                            ORDER BY s.SaleDate DESC";
                        break;

                    case 2: // حسب المنتج
                        query = $@"
                            SELECT 
                                p.ProductCode AS 'كود المنتج',
                                p.ProductName AS 'اسم المنتج',
                                c.CategoryName AS 'التصنيف',
                                SUM(sd.Quantity) AS 'الكمية المباعة',
                                AVG(sd.UnitPrice) AS 'متوسط سعر البيع',
                                SUM(sd.TotalPrice) AS 'إجمالي المبيعات',
                                AVG(p.CostPrice) AS 'متوسط التكلفة',
                                SUM((sd.UnitPrice - p.CostPrice) * sd.Quantity) AS 'الربح'
                            FROM SalesDetails sd
                            INNER JOIN Sales s ON sd.SaleID = s.SaleID
                            INNER JOIN Products p ON sd.ProductID = p.ProductID
                            LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                            WHERE s.SaleDate BETWEEN '{dtpFrom.Value:yyyy-MM-dd}' 
                                AND '{dtpTo.Value:yyyy-MM-dd} 23:59:59'
                            GROUP BY p.ProductCode, p.ProductName, c.CategoryName, p.CostPrice
                            ORDER BY SUM(sd.TotalPrice) DESC";
                        break;

                    case 3: // حسب العميل
                        query = $@"
                            SELECT 
                                c.CustomerCode AS 'كود العميل',
                                c.CustomerName AS 'اسم العميل',
                                COUNT(s.SaleID) AS 'عدد الفواتير',
                                SUM(s.NetAmount) AS 'إجمالي المبيعات',
                                AVG(s.NetAmount) AS 'متوسط الفاتورة',
                                c.CurrentBalance AS 'الرصيد الحالي'
                            FROM Sales s
                            INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                            WHERE s.SaleDate BETWEEN '{dtpFrom.Value:yyyy-MM-dd}' 
                                AND '{dtpTo.Value:yyyy-MM-dd} 23:59:59'
                            GROUP BY c.CustomerCode, c.CustomerName, c.CurrentBalance
                            ORDER BY SUM(s.NetAmount) DESC";
                        break;

                    case 4: // حسب الكاشير
                        query = $@"
                            SELECT 
                                u.Username AS 'اسم المستخدم',
                                u.FullName AS 'الاسم الكامل',
                                COUNT(s.SaleID) AS 'عدد الفواتير',
                                SUM(s.NetAmount) AS 'إجمالي المبيعات',
                                AVG(s.NetAmount) AS 'متوسط الفاتورة',
                                MIN(s.SaleDate) AS 'أول فاتورة',
                                MAX(s.SaleDate) AS 'آخر فاتورة'
                            FROM Sales s
                            INNER JOIN Users u ON s.UserID = u.UserID
                            WHERE s.SaleDate BETWEEN '{dtpFrom.Value:yyyy-MM-dd}' 
                                AND '{dtpTo.Value:yyyy-MM-dd} 23:59:59'
                            GROUP BY u.Username, u.FullName
                            ORDER BY SUM(s.NetAmount) DESC";
                        break;
                }

                DataTable dt = DatabaseConnection.ExecuteDataTable(query);

                DataGridView dgv = this.Controls.Find("dgvReport", true)[0] as DataGridView;
                dgv.DataSource = dt;

                // حساب الإجماليات
                CalculateTotals(dt, cmbReportType.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في إنشاء التقرير:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals(DataTable dt, int reportType)
        {
            try
            {
                decimal totalSales = 0;
                decimal totalProfit = 0;
                int totalInvoices = 0;

                if (reportType == 0 || reportType == 1) // ملخص يومي أو تفصيلي
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (reportType == 0)
                            totalInvoices += Convert.ToInt32(row["عدد الفواتير"]);
                        else
                            totalInvoices++;

                        if (row["الصافي"] != DBNull.Value)
                            totalSales += Convert.ToDecimal(row["الصافي"]);

                        if (row["الربح"] != DBNull.Value)
                            totalProfit += Convert.ToDecimal(row["الربح"]);
                    }
                }
                else if (reportType == 2) // حسب المنتج
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["إجمالي المبيعات"] != DBNull.Value)
                            totalSales += Convert.ToDecimal(row["إجمالي المبيعات"]);

                        if (row["الربح"] != DBNull.Value)
                            totalProfit += Convert.ToDecimal(row["الربح"]);
                    }
                }
                else // حسب العميل أو الكاشير
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        totalInvoices += Convert.ToInt32(row["عدد الفواتير"]);

                        if (row["إجمالي المبيعات"] != DBNull.Value)
                            totalSales += Convert.ToDecimal(row["إجمالي المبيعات"]);
                    }
                }

                StatusStrip statusStrip = this.Controls[this.Controls.Count - 1] as StatusStrip;
                foreach (ToolStripItem item in statusStrip.Items)
                {
                    if (item.Name == "lblTotalInvoices")
                        item.Text = $"عدد الفواتير: {totalInvoices}";
                    else if (item.Name == "lblTotalSales")
                        item.Text = $"إجمالي المبيعات: {totalSales:F2} جنيه";
                    else if (item.Name == "lblTotalProfit")
                        item.Text = $"إجمالي الربح: {totalProfit:F2} جنيه";
                }
            }
            catch { }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            DataGridView dgv = this.Controls.Find("dgvReport", true)[0] as DataGridView;

            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintReport(dgv);
        }

        private void PrintReport(DataGridView dgv)
        {
            // سيتم تفصيل الطباعة في الجزء التالي
            MessageBox.Show("سيتم فتح نافذة الطباعة...", "طباعة",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            DataGridView dgv = this.Controls.Find("dgvReport", true)[0] as DataGridView;

            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ExportToCSV(dgv);
        }

        private void ExportToCSV(DataGridView dgv)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv";
                saveDialog.FileName = $"تقرير_المبيعات_{DateTime.Now:yyyy-MM-dd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(
                        saveDialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // Headers
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            sw.Write(dgv.Columns[i].HeaderText);
                            if (i < dgv.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // Rows
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString() ?? "");
                                if (i < dgv.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("تم التصدير بنجاح!", "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في التصدير:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}