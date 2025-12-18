using System;
using System.Data;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Helpers;

namespace MedicalStoreSystem.Forms.Products
{
    public partial class StockInventoryForm : Form
    {
        private ProductDAL productDAL = new ProductDAL();

        public StockInventoryForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.WindowState = FormWindowState.Maximized;

            InitializeControls();
        }

        private void InitializeControls()
        {
            // Panel العلوي
            Panel panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 100;
            panelTop.BackColor = System.Drawing.Color.WhiteSmoke;

            // GroupBox للفلاتر
            GroupBox groupFilter = new GroupBox();
            groupFilter.Text = "فلتر العرض";
            groupFilter.Location = new System.Drawing.Point(20, 10);
            groupFilter.Size = new System.Drawing.Size(800, 80);

            // RadioButtons
            RadioButton rbAll = new RadioButton();
            rbAll.Name = "rbAll";
            rbAll.Text = "جميع المنتجات";
            rbAll.Location = new System.Drawing.Point(20, 30);
            rbAll.Checked = true;
            rbAll.CheckedChanged += Filter_CheckedChanged;

            RadioButton rbLowStock = new RadioButton();
            rbLowStock.Name = "rbLowStock";
            rbLowStock.Text = "منتجات قليلة (أقل من الحد الأدنى)";
            rbLowStock.Location = new System.Drawing.Point(200, 30);
            rbLowStock.CheckedChanged += Filter_CheckedChanged;

            RadioButton rbOutOfStock = new RadioButton();
            rbOutOfStock.Name = "rbOutOfStock";
            rbOutOfStock.Text = "منتجات نفذت";
            rbOutOfStock.Location = new System.Drawing.Point(450, 30);
            rbOutOfStock.CheckedChanged += Filter_CheckedChanged;

            RadioButton rbExpiringSoon = new RadioButton();
            rbExpiringSoon.Name = "rbExpiringSoon";
            rbExpiringSoon.Text = "منتجات قريبة الصلاحية (خلال 3 أشهر)";
            rbExpiringSoon.Location = new System.Drawing.Point(20, 55);
            rbExpiringSoon.CheckedChanged += Filter_CheckedChanged;

            groupFilter.Controls.Add(rbAll);
            groupFilter.Controls.Add(rbLowStock);
            groupFilter.Controls.Add(rbOutOfStock);
            groupFilter.Controls.Add(rbExpiringSoon);
            panelTop.Controls.Add(groupFilter);

            // Panel للأزرار
            Panel panelButtons = new Panel();
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Height = 60;
            panelButtons.BackColor = System.Drawing.Color.WhiteSmoke;

            Button btnRefresh = new Button();
            btnRefresh.Text = "تحديث";
            btnRefresh.Location = new System.Drawing.Point(20, 15);
            btnRefresh.Size = new System.Drawing.Size(100, 35);
            btnRefresh.Click += (s, e) => LoadInventory();

            Button btnExport = new Button();
            btnExport.Text = "تصدير Excel";
            btnExport.Location = new System.Drawing.Point(130, 15);
            btnExport.Size = new System.Drawing.Size(100, 35);
            btnExport.Click += BtnExport_Click;

            Button btnPrint = new Button();
            btnPrint.Text = "طباعة";
            btnPrint.Location = new System.Drawing.Point(240, 15);
            btnPrint.Size = new System.Drawing.Size(100, 35);
            btnPrint.Click += BtnPrint_Click;

            Button btnAdjustStock = new Button();
            btnAdjustStock.Text = "تسوية المخزون";
            btnAdjustStock.Location = new System.Drawing.Point(350, 15);
            btnAdjustStock.Size = new System.Drawing.Size(120, 35);
            btnAdjustStock.BackColor = System.Drawing.Color.Orange;
            btnAdjustStock.ForeColor = System.Drawing.Color.White;
            btnAdjustStock.Click += BtnAdjustStock_Click;

            panelButtons.Controls.Add(btnRefresh);
            panelButtons.Controls.Add(btnExport);
            panelButtons.Controls.Add(btnPrint);
            panelButtons.Controls.Add(btnAdjustStock);

            // DataGridView
            DataGridView dgvInventory = new DataGridView();
            dgvInventory.Name = "dgvInventory";
            dgvInventory.Dock = DockStyle.Fill;
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventory.ReadOnly = true;
            dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInventory.MultiSelect = false;

            // StatusStrip
            StatusStrip statusStrip = new StatusStrip();
            ToolStripStatusLabel lblTotal = new ToolStripStatusLabel();
            lblTotal.Name = "lblTotal";
            lblTotal.Text = "إجمالي المنتجات: 0";

            ToolStripStatusLabel lblTotalValue = new ToolStripStatusLabel();
            lblTotalValue.Name = "lblTotalValue";
            lblTotalValue.Text = "قيمة المخزون: 0.00 جنيه";
            lblTotalValue.Spring = true;

            statusStrip.Items.Add(lblTotal);
            statusStrip.Items.Add(lblTotalValue);

            this.Controls.Add(dgvInventory);
            this.Controls.Add(panelButtons);
            this.Controls.Add(panelTop);
            this.Controls.Add(statusStrip);
        }

        private void StockInventoryForm_Load(object sender, EventArgs e)
        {
            LoadInventory();
        }

        private void LoadInventory(string filter = "ALL")
        {
            try
            {
                string query = @"SELECT 
                                   p.ProductID AS 'الرقم',
                                   p.ProductCode AS 'كود المنتج',
                                   p.Barcode AS 'الباركود',
                                   p.ProductName AS 'اسم المنتج',
                                   c.CategoryName AS 'التصنيف',
                                   p.UnitName AS 'الوحدة',
                                   p.CurrentStock AS 'الكمية الحالية',
                                   p.MinStock AS 'الحد الأدنى',
                                   p.CostPrice AS 'سعر التكلفة',
                                   p.SalePrice AS 'سعر البيع',
                                   (p.CurrentStock * p.CostPrice) AS 'قيمة المخزون',
                                   CASE 
                                       WHEN p.CurrentStock = 0 THEN 'نفذ'
                                       WHEN p.CurrentStock <= p.MinStock THEN 'قليل'
                                       ELSE 'متوفر'
                                   END AS 'الحالة',
                                   s.SupplierName AS 'المورد'
                               FROM Products p
                               LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                               LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                               WHERE p.IsActive = 1";

                switch (filter)
                {
                    case "LOW":
                        query += " AND p.CurrentStock <= p.MinStock AND p.CurrentStock > 0";
                        break;
                    case "OUT":
                        query += " AND p.CurrentStock = 0";
                        break;
                    case "EXPIRING":
                        // هنا نحتاج جدول منفصل لتواريخ الصلاحية
                        // للتبسيط سنعرض المنتجات التي لها صلاحية
                        query += " AND p.HasExpiry = 1";
                        break;
                }

                query += " ORDER BY p.CurrentStock, p.ProductName";

                DataTable dt = DatabaseConnection.ExecuteDataTable(query);

                DataGridView dgv = this.Controls.Find("dgvInventory", true)[0] as DataGridView;
                dgv.DataSource = dt;

                if (dgv.Columns["الرقم"] != null)
                    dgv.Columns["الرقم"].Visible = false;

                // تلوين الصفوف
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    string status = row.Cells["الحالة"].Value.ToString();
                    if (status == "نفذ")
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    else if (status == "قليل")
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                }

                // حساب الإجماليات
                CalculateTotals(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل البيانات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals(DataTable dt)
        {
            try
            {
                decimal totalValue = 0;
                int count = dt.Rows.Count;

                foreach (DataRow row in dt.Rows)
                {
                    if (row["قيمة المخزون"] != DBNull.Value)
                        totalValue += Convert.ToDecimal(row["قيمة المخزون"]);
                }

                StatusStrip statusStrip = this.Controls.Find("statusStrip1", true)[0] as StatusStrip;
                if (statusStrip == null)
                    statusStrip = this.Controls[this.Controls.Count - 1] as StatusStrip;

                foreach (ToolStripItem item in statusStrip.Items)
                {
                    if (item.Name == "lblTotal")
                        item.Text = $"إجمالي المنتجات: {count}";
                    else if (item.Name == "lblTotalValue")
                        item.Text = $"قيمة المخزون: {totalValue:F2} جنيه";
                }
            }
            catch { }
        }

        private void Filter_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                string filter = "ALL";
                if (rb.Name == "rbLowStock") filter = "LOW";
                else if (rb.Name == "rbOutOfStock") filter = "OUT";
                else if (rb.Name == "rbExpiringSoon") filter = "EXPIRING";

                LoadInventory(filter);
            }
        }

        private void BtnAdjustStock_Click(object sender, EventArgs e)
        {
            // فتح نافذة تسوية المخزون
            StockAdjustmentForm adjustForm = new StockAdjustmentForm();
            adjustForm.ShowDialog();
            LoadInventory();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            // تصدير لـ Excel (مبسط)
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv";
                saveDialog.FileName = $"جرد_المخزون_{DateTime.Now:yyyy-MM-dd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    DataGridView dgv = this.Controls.Find("dgvInventory", true)[0] as DataGridView;
                    ExportToCSV(dgv, saveDialog.FileName);

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

        private void ExportToCSV(DataGridView dgv, string filePath)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Headers
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (dgv.Columns[i].Visible)
                    {
                        sw.Write(dgv.Columns[i].HeaderText);
                        if (i < dgv.Columns.Count - 1)
                            sw.Write(",");
                    }
                }
                sw.WriteLine();

                // Rows
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        if (dgv.Columns[i].Visible)
                        {
                            sw.Write(row.Cells[i].Value?.ToString() ?? "");
                            if (i < dgv.Columns.Count - 1)
                                sw.Write(",");
                        }
                    }
                    sw.WriteLine();
                }
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            // طباعة التقرير
            PrintInventoryReport();
        }

        private void PrintInventoryReport()
        {
            try
            {
                DataGridView dgv = this.Controls.Find("dgvInventory", true)[0] as DataGridView;

                // إنشاء تقرير نصي
                string report = "═══════════════════════════════════════════\n";
                report += "          تقرير جرد المخزون          \n";
                report += "═══════════════════════════════════════════\n\n";
                report += $"التاريخ: {DateTime.Now:yyyy-MM-dd HH:mm}\n";
                report += $"المستخدم: {CurrentSession.LoggedInUser.FullName}\n";
                report += "───────────────────────────────────────────\n\n";

                decimal totalValue = 0;
                int count = 0;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["اسم المنتج"].Value != null)
                    {
                        count++;
                        string productName = row.Cells["اسم المنتج"].Value.ToString();
                        int quantity = Convert.ToInt32(row.Cells["الكمية الحالية"].Value);
                        decimal costPrice = Convert.ToDecimal(row.Cells["سعر التكلفة"].Value);
                        decimal value = Convert.ToDecimal(row.Cells["قيمة المخزون"].Value);
                        string status = row.Cells["الحالة"].Value.ToString();

                        report += $"{count}. {productName}\n";
                        report += $"   الكمية: {quantity} - القيمة: {value:F2} جنيه - الحالة: {status}\n\n";

                        totalValue += value;
                    }
                }

                report += "═══════════════════════════════════════════\n";
                report += $"إجمالي المنتجات: {count}\n";
                report += $"قيمة المخزون: {totalValue:F2} جنيه\n";
                report += "═══════════════════════════════════════════\n";

                // عرض في نافذة للطباعة
                ShowPrintPreview(report, "تقرير جرد المخزون");
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في الطباعة:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowPrintPreview(string content, string title)
        {
            Form printForm = new Form();
            printForm.Text = title;
            printForm.Size = new System.Drawing.Size(800, 600);
            printForm.StartPosition = FormStartPosition.CenterParent;
            printForm.RightToLeft = RightToLeft.Yes;

            TextBox txtPreview = new TextBox();
            txtPreview.Multiline = true;
            txtPreview.ScrollBars = ScrollBars.Both;
            txtPreview.Dock = DockStyle.Fill;
            txtPreview.Font = new System.Drawing.Font("Courier New", 10F);
            txtPreview.Text = content;
            txtPreview.ReadOnly = true;

            Panel panelButtons = new Panel();
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Height = 50;

            Button btnPrint = new Button();
            btnPrint.Text = "طباعة";
            btnPrint.Location = new System.Drawing.Point(350, 10);
            btnPrint.Size = new System.Drawing.Size(100, 30);
            btnPrint.Click += (s, e) => {
                // هنا يمكن إضافة كود الطباعة الفعلي
                MessageBox.Show("تم إرسال التقرير للطابعة", "طباعة",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            panelButtons.Controls.Add(btnPrint);
            printForm.Controls.Add(txtPreview);
            printForm.Controls.Add(panelButtons);

            printForm.ShowDialog();
        }
    }
}
