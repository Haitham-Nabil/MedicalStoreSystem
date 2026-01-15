using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Forms.Products;
using MedicalStoreSystem.Forms.Purchases;
using MedicalStoreSystem.Forms.Sales;
using MedicalStoreSystem.Helpers;
using System;
using System.Windows.Forms;
using System.Drawing;


namespace MedicalStoreSystem.Forms.MainForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ImproveUI();
            // إعدادات النافذة
            this.WindowState = FormWindowState.Maximized;
            this.Text = "نظام إدارة المستلزمات الطبية";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.IsMdiContainer = true; // لفتح النوافذ بداخلها
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // عرض معلومات المستخدم
            if (CurrentSession.IsLoggedIn)
            {
                lblUsername.Text = $"المستخدم: {CurrentSession.LoggedInUser.FullName}";
                lblUserRole.Text = $"الصلاحية: {GetRoleNameInArabic(CurrentSession.LoggedInUser.Role)}";
            }

            // عرض التاريخ والوقت
            UpdateDateTime();

            // ضبط الصلاحيات
            SetPermissions();

            // رسالة ترحيب
            ShowWelcomeMessage();
        }

        // تحديث التاريخ والوقت
        private void timerClock_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            lblDate.Text = $"التاريخ: {DateTime.Now.ToString("yyyy/MM/dd")}";
            lblTime.Text = $"الوقت: {DateTime.Now.ToString("hh:mm:ss tt")}";
        }

        // ترجمة الصلاحيات للعربية
        private string GetRoleNameInArabic(string role)
        {
            switch (role)
            {
                case "Admin": return "مدير النظام";
                case "Cashier": return "كاشير";
                case "StoreKeeper": return "أمين مخزن";
                case "Accountant": return "محاسب";
                default: return role;
            }
        }

        // ضبط الصلاحيات
        private void SetPermissions()
        {
            if (CurrentSession.IsCashier)
            {
                // الكاشير: المبيعات فقط
                menuInventory.Enabled = false;
                menuPurchases.Enabled = false;
                menuPeople.Enabled = false;
                menuCash.Enabled = false;
                menuSettings.Enabled = false;
            }
            else if (CurrentSession.IsStoreKeeper)
            {
                // أمين المخزن: المخزن والمشتريات
                menuSales.Enabled = false;
                menuCash.Enabled = false;
                menuReports.Enabled = false;
                menuSettings.Enabled = false;
            }
            else if (CurrentSession.IsAccountant)
            {
                // المحاسب: التقارير والخزنة فقط
                menuInventory.Enabled = false;
                menuSales.Enabled = false;
                menuPurchases.Enabled = false;
                menuSettings.Enabled = false;
            }
            // Admin له كل الصلاحيات (لا نعطل أي شيء)
        }

        // رسالة ترحيب
        /*private void ShowWelcomeMessage()
        {
            Label lblWelcome = new Label();
            lblWelcome.Text = $"مرحباً {CurrentSession.LoggedInUser.FullName}\n\nنظام إدارة المستلزمات الطبية";
            lblWelcome.Font = new System.Drawing.Font("Tahoma", 20, System.Drawing.FontStyle.Bold);
            lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblWelcome.Dock = DockStyle.Fill;
            lblWelcome.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);

            panelMain.Controls.Add(lblWelcome);
        }*/
        private void ShowWelcomeMessage()
        {
            Panel dashboardPanel = new Panel();
            dashboardPanel.Dock = DockStyle.Fill;
            dashboardPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            dashboardPanel.AutoScroll = true;

            // عنوان الترحيب
            Label lblWelcome = new Label();
            lblWelcome.Text = $"مرحباً {CurrentSession.LoggedInUser.FullName}";
            lblWelcome.Font = new System.Drawing.Font("Tahoma", 18, System.Drawing.FontStyle.Bold);
            lblWelcome.Location = new System.Drawing.Point(20, 20);
            lblWelcome.AutoSize = true;
            lblWelcome.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);

            // بطاقات الإحصائيات
            int cardY = 80;
            int cardSpacing = 20;

            // بطاقة مبيعات اليوم
            Panel cardSales = CreateDashboardCard("مبيعات اليوم", GetTodaySalesTotal().ToString("F2") + " جنيه",
                System.Drawing.Color.FromArgb(46, 204, 113), 20, cardY);

            // بطاقة عدد المنتجات
            Panel cardProducts = CreateDashboardCard("عدد المنتجات", GetProductsCount().ToString(),
                System.Drawing.Color.FromArgb(52, 152, 219), 280, cardY);

            // بطاقة المنتجات القليلة
            Panel cardLowStock = CreateDashboardCard("منتجات قليلة", GetLowStockCount().ToString(),
                System.Drawing.Color.FromArgb(231, 76, 60), 540, cardY);

            // إضافة العناصر
            dashboardPanel.Controls.Add(lblWelcome);
            dashboardPanel.Controls.Add(cardSales);
            dashboardPanel.Controls.Add(cardProducts);
            dashboardPanel.Controls.Add(cardLowStock);

            panelMain.Controls.Add(dashboardPanel);
        }

        private Panel CreateDashboardCard(string title, string value, System.Drawing.Color color, int x, int y)
        {
            Panel card = new Panel();
            card.Size = new System.Drawing.Size(240, 120);
            card.Location = new System.Drawing.Point(x, y);
            card.BackColor = color;
            card.Cursor = Cursors.Hand;

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.Location = new System.Drawing.Point(10, 15);
            lblTitle.AutoSize = true;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new System.Drawing.Font("Tahoma", 20, System.Drawing.FontStyle.Bold);
            lblValue.ForeColor = System.Drawing.Color.White;
            lblValue.Location = new System.Drawing.Point(10, 50);
            lblValue.AutoSize = true;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            return card;
        }

        private decimal GetTodaySalesTotal()
        {
            try
            {
                SaleDAL saleDAL = new SaleDAL();
                return saleDAL.GetTodaySalesTotal();
            }
            catch
            {
                return 0;
            }
        }

        private int GetProductsCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Products WHERE IsActive = 1";
                object result = DAL.DatabaseConnection.ExecuteScalar(query);
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        private int GetLowStockCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Products WHERE CurrentStock <= MinStock AND IsActive = 1";
                object result = DAL.DatabaseConnection.ExecuteScalar(query);
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        // فتح نافذة فرعية
        private void OpenChildForm(Form childForm)
        {
            // إغلاق أي نوافذ مفتوحة في الـ Panel
            panelMain.Controls.Clear();

            // إعدادات النافذة الفرعية
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // إضافتها للـ Panel
            panelMain.Controls.Add(childForm);
            panelMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        // ═══════════════ أحداث القوائم ═══════════════

        // قائمة الملف
        private void menuLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("هل تريد تسجيل الخروج؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CurrentSession.Logout();
                this.Close();

                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("هل تريد الخروج من البرنامج؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // قائمة المخزن
        private void menuCategories_Click(object sender, EventArgs e)
        {
           // OpenChildForm(new CategoriesForm());
           OpenFormInPanel(new CategoriesForm());
        }

        private void menuProducts_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء شاشة المنتجات في الخطوة القادمة", "قريباً",
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);
            //OpenChildForm(new Forms.Products.ProductsForm());
            OpenFormInPanel(new ProductsForm());
        }

        private void menuPurchasesList_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.Purchases.PurchasesListForm());
        }

        private void menuStockInventory_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("سيتم إنشاء شاشة جرد المخزن لاحقاً", "قريباً",
            //   MessageBoxButtons.OK, MessageBoxIcon.Information);
            //OpenChildForm(new Forms.Products.StockInventoryForm());
            OpenFormInPanel(new Forms.Products.StockInventoryForm());
        }

        // قائمة المبيعات
        private void menuNewSale_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء شاشة فاتورة المبيعات لاحقاً", "قريباً",
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);
            POSForm posForm = new Forms.Sales.POSForm();
            posForm.ShowDialog();
        }

        private void menuSalesList_Click(object sender, EventArgs e)
        {
           // OpenChildForm(new Forms.Sales.SalesListForm());
            OpenFormInPanel(new Forms.Sales.SalesListForm());
        }
        // قائمة المشتريات
        private void menuNewPurchase_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء شاشة فاتورة المشتريات لاحقاً", "قريباً",
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);
            /*PurchaseForm purchaseForm = new Forms.Purchases.PurchaseForm();
            purchaseForm.ShowDialog();*/
            OpenFormInPanel(new Forms.Purchases.PurchaseForm());
        }

        // قائمة العملاء والموردين
        private void menuCustomers_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء شاشة العملاء لاحقاً", "قريباً",
            // MessageBoxButtons.OK, MessageBoxIcon.Information);
            //OpenChildForm(new Forms.Customers.CustomersForm());
            OpenChildForm(new Forms.Customers.CustomersForm());
        }

        private void menuSuppliers_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء شاشة الموردين لاحقاً", "قريباً",
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenChildForm(new Forms.Suppliers.SuppliersForm());
        }

        // قائمة الخزنة
        private void menuDeposit_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء شاشة الإيداع لاحقاً", "قريباً",
            // MessageBoxButtons.OK, MessageBoxIcon.Information);
            Forms.Cash.CashTransactionForm cashForm = new Forms.Cash.CashTransactionForm();
            cashForm.ShowDialog();
        }

        // قائمة التقارير
        private void menuSalesReport_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء التقارير لاحقاً", "قريباً",
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);


            //OpenChildForm(new Forms.Reports.SalesReportForm());
            OpenChildForm(new Forms.Reports.DetailedSalesReportForm());
        }

        // قائمة الإعدادات
        private void menuUsers_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("سيتم إنشاء شاشة المستخدمين لاحقاً", "قريباً",
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenChildForm(new Forms.Settings.UsersForm());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("هل تريد الخروج من البرنامج؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }





        ////// تحديث بعد إضافة صلاحيات المستخدمين
        ///
        // في قسم أحداث القوائم

        // قائمة الخزنة
        private void menuWithdraw_Click(object sender, EventArgs e)
        {
            Forms.Cash.CashTransactionForm cashForm = new Forms.Cash.CashTransactionForm();
            cashForm.ShowDialog();
        }

        private void menuExpenses_Click(object sender, EventArgs e)
        {
            Forms.Cash.CashTransactionForm cashForm = new Forms.Cash.CashTransactionForm();
            cashForm.ShowDialog();
        }

        private void menuCashMovements_Click(object sender, EventArgs e)
        {
            Forms.Cash.CashTransactionForm cashForm = new Forms.Cash.CashTransactionForm();
            cashForm.ShowDialog();
        }

        // قائمة التقارير
        private void menuPurchasesReport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("سيتم إضافة تقرير المشتريات لاحقاً", "قريباً",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuStockReport_Click(object sender, EventArgs e)
        {
            // تقرير المخزون البسيط
            ShowStockReport();
        }

        private void menuProfitReport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("سيتم إضافة تقرير الأرباح لاحقاً", "قريباً",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuCashReport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("سيتم إضافة تقرير الخزنة لاحقاً", "قريباً",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // قائمة الإعدادات

        private void menuSystemSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("سيتم إضافة إعدادات النظام لاحقاً", "قريباً",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuBackup_Click(object sender, EventArgs e)
        {
            Forms.Settings.BackupForm backupForm = new Forms.Settings.BackupForm();
            backupForm.ShowDialog();
        }

        // تقرير المخزون البسيط
        private void ShowStockReport()
        {
            try
            {
                Form reportForm = new Form();
                reportForm.Text = "تقرير المخزون";
                reportForm.Size = new System.Drawing.Size(900, 600);
                reportForm.StartPosition = FormStartPosition.CenterParent;
                reportForm.RightToLeft = RightToLeft.Yes;
                reportForm.RightToLeftLayout = true;

                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.ReadOnly = true;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                string query = @"SELECT 
                           p.ProductCode AS 'كود المنتج',
                           p.ProductName AS 'اسم المنتج',
                           c.CategoryName AS 'التصنيف',
                           p.CurrentStock AS 'الكمية الحالية',
                           p.MinStock AS 'الحد الأدنى',
                           p.CostPrice AS 'سعر الشراء',
                           p.SalePrice AS 'سعر البيع',
                           (p.CurrentStock * p.CostPrice) AS 'قيمة المخزون',
                           CASE 
                               WHEN p.CurrentStock = 0 THEN 'نفذ'
                               WHEN p.CurrentStock <= p.MinStock THEN 'قليل'
                               ELSE 'متوفر'
                           END AS 'الحالة'
                       FROM Products p
                       LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                       WHERE p.IsActive = 1
                       ORDER BY p.CurrentStock";

                dgv.DataSource = DAL.DatabaseConnection.ExecuteDataTable(query);

                // تلوين الصفوف
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string status = row.Cells["الحالة"].Value.ToString();
                    if (status == "نفذ")
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    else if (status == "قليل")
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                }

                reportForm.Controls.Add(dgv);
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //تحسينات المرحلة السادسه
        // في Constructor أو Form_Load

        private void ImproveUI()
        {
            // تحسين شكل القوائم
            menuStripMain.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            menuStripMain.ForeColor = System.Drawing.Color.White;
            menuStripMain.Font = new System.Drawing.Font("Tahoma", 10F);

            // تحسين StatusStrip
            statusStrip1.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            statusStrip1.ForeColor = System.Drawing.Color.White;

            // تحسين Panel
            panelMain.BackColor = System.Drawing.Color.WhiteSmoke;
        }


        // نهاية تحسينات المرحلة السادسه

        // تحسينات المرحلة السابعة
        // في تصميم القائمة أضف:
        // ❓ مساعدة (menuHelp)
        //    ├── دليل الاستخدام (menuUserGuide)
        //    ├── اختصارات لوحة المفاتيح (menuKeyboardShortcuts)
        //    ├── ──────────── (Separator)
        //    └── حول البرنامج (menuAbout)

        private void menuUserGuide_Click(object sender, EventArgs e)
        {
            string guide = @"دليل الاستخدام السريع:

                            ══════════════════════════════════

                            📦 المخزون:
                            • التصنيفات: إدارة تصنيفات المنتجات
                            • المنتجات: إضافة وتعديل المنتجات
                            • جرد المخزن: عرض حالة المخزون

                            💰 المبيعات:
                            • فاتورة مبيعات: نقطة البيع (POS)
                            • المبيعات: عرض جميع الفواتير
                            • مرتجعات: إدارة المرتجعات

                            🛒 المشتريات:
                            • فاتورة مشتريات: إضافة مشتريات جديدة
                            • المشتريات: عرض جميع فواتير المشتريات

                            👥 العملاء والموردين:
                            • إدارة بيانات العملاء والموردين
                            • متابعة الأرصدة

                            💵 الخزنة:
                            • إيداع وسحب
                            • تسجيل المصروفات
                            • عرض حركة الخزنة

                            📊 التقارير:
                            • تقارير المبيعات والمشتريات
                            • تقرير المخزون
                            • تقرير الأرباح

                            ⚙️ الإعدادات:
                            • إدارة المستخدمين والصلاحيات
                            • النسخ الاحتياطي

                            ══════════════════════════════════";

            MessageBox.Show(guide, "دليل الاستخدام",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuKeyboardShortcuts_Click(object sender, EventArgs e)
        {
            string shortcuts = @"اختصارات لوحة المفاتيح:

                                ══════════════════════════════════

                                نقطة البيع (POS):
                                • F1: التركيز على حقل الباركود
                                • F2: اختيار منتج
                                • F3: تعديل الكمية
                                • F4: إضافة خصم
                                • F5: المدفوع
                                • F9: فاتورة جديدة
                                • F10: إتمام البيع
                                • F12: إلغاء الفاتورة
                                • ESC: خروج

                                ══════════════════════════════════

                                عام:
                                • Enter: تأكيد/الانتقال للحقل التالي
                                • ESC: إلغاء/رجوع
                                • Ctrl+S: حفظ (في معظم الشاشات)
                                • Ctrl+N: جديد (في معظم الشاشات)

                                ══════════════════════════════════";

            MessageBox.Show(shortcuts, "اختصارات لوحة المفاتيح",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void menuSalesReturns_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.Sales.SalesReturnsForm());
        }

        private void menuPurchasesReturns_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.Purchases.PurchaseReturnsForm());

        }

        private void menuPrintSettings_Click(object sender, EventArgs e)
        {
            Forms.Settings.PrintSettingsForm frm = new Forms.Settings.PrintSettingsForm();
            frm.ShowDialog();
        }

        private void menuCustomersBalance_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.Reports.CustomersBalanceReport());
        }

        private void menuSuppliersBalance_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.Reports.SuppliersBalanceReport());
        }

        private void menuStockMovement_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.Reports.StockMovementReport());
        }

        //open form in panel
        private void OpenFormInPanel(Form frm)
        {
            panelMain.Controls.Clear();

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;   // مهم
            frm.StartPosition = FormStartPosition.Manual;

            // توسيط حسب حجم الفورم نفسه
            frm.Location = new Point(
                Math.Max(0, (panelMain.Width - frm.Width) / 2),
                Math.Max(0, (panelMain.Height - frm.Height) / 2)
            );

            panelMain.Controls.Add(frm);
            frm.Show();
        }
        

        // هذا زر مؤقت سيتم حذفة
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new ProductsForm());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new CategoriesForm());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Forms.Products.StockInventoryForm());
        }

        private void menuBackup_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new Forms.Settings.BackupForm());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            POSForm posForm = new Forms.Sales.POSForm();
            posForm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Forms.Purchases.PurchaseForm());
        }

        private void menuPurchasesReport_Click_1(object sender, EventArgs e)
        {

        }
        // نهاية الزر المؤقت
    }
}
