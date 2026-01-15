namespace MedicalStoreSystem.Forms.MainForms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.Separator = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInventory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCategories = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProducts = new System.Windows.Forms.ToolStripMenuItem();
            this.Separator_inv = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStockInventory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSales = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewSale = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSalesList = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSalesReturns = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPurchases = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewPurchase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPurchasesList = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPurchasesReturns = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPeople = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCustomers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSuppliers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCash = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeposit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWithdraw = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExpenses = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCashMovements = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReports = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSalesReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPurchasesReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStockReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProfitReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCashReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCustomersBalance = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSuppliersBalance = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStockMovement = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUsers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSystemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrintSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUserGuide = new System.Windows.Forms.ToolStripMenuItem();
            this.menuKeyboardShortcuts = new System.Windows.Forms.ToolStripMenuItem();
            this.SeparatorHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblUsername = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUserRole = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDate = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panalSide = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.menuStripMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panalSide.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuInventory,
            this.menuSales,
            this.menuPurchases,
            this.menuPeople,
            this.menuCash,
            this.menuReports,
            this.menuSettings,
            this.menuHelp});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1404, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLogout,
            this.Separator,
            this.menuExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(52, 20);
            this.menuFile.Text = "الملف ";
            // 
            // menuLogout
            // 
            this.menuLogout.Name = "menuLogout";
            this.menuLogout.Size = new System.Drawing.Size(146, 22);
            this.menuLogout.Text = "تسجيل خروج";
            this.menuLogout.Click += new System.EventHandler(this.menuLogout_Click);
            // 
            // Separator
            // 
            this.Separator.Name = "Separator";
            this.Separator.Size = new System.Drawing.Size(146, 22);
            this.Separator.Text = "────────────";
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(146, 22);
            this.menuExit.Text = "خروج ";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuInventory
            // 
            this.menuInventory.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCategories,
            this.menuProducts,
            this.Separator_inv,
            this.menuStockInventory});
            this.menuInventory.Name = "menuInventory";
            this.menuInventory.Size = new System.Drawing.Size(53, 20);
            this.menuInventory.Text = "المخزن";
            // 
            // menuCategories
            // 
            this.menuCategories.Name = "menuCategories";
            this.menuCategories.Size = new System.Drawing.Size(146, 22);
            this.menuCategories.Text = "التصنيفات";
            this.menuCategories.Click += new System.EventHandler(this.menuCategories_Click);
            // 
            // menuProducts
            // 
            this.menuProducts.Name = "menuProducts";
            this.menuProducts.Size = new System.Drawing.Size(146, 22);
            this.menuProducts.Text = "الأصناف";
            this.menuProducts.Click += new System.EventHandler(this.menuProducts_Click);
            // 
            // Separator_inv
            // 
            this.Separator_inv.Name = "Separator_inv";
            this.Separator_inv.Size = new System.Drawing.Size(146, 22);
            this.Separator_inv.Text = "────────────";
            // 
            // menuStockInventory
            // 
            this.menuStockInventory.Name = "menuStockInventory";
            this.menuStockInventory.Size = new System.Drawing.Size(146, 22);
            this.menuStockInventory.Text = "جرد المخزن";
            this.menuStockInventory.Click += new System.EventHandler(this.menuStockInventory_Click);
            // 
            // menuSales
            // 
            this.menuSales.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNewSale,
            this.menuSalesList,
            this.menuSalesReturns});
            this.menuSales.Name = "menuSales";
            this.menuSales.Size = new System.Drawing.Size(62, 20);
            this.menuSales.Text = "المبيعات";
            // 
            // menuNewSale
            // 
            this.menuNewSale.Name = "menuNewSale";
            this.menuNewSale.Size = new System.Drawing.Size(163, 22);
            this.menuNewSale.Text = "فاتورة مبيعات";
            this.menuNewSale.Click += new System.EventHandler(this.menuNewSale_Click);
            // 
            // menuSalesList
            // 
            this.menuSalesList.Name = "menuSalesList";
            this.menuSalesList.Size = new System.Drawing.Size(163, 22);
            this.menuSalesList.Text = "المبيعات";
            this.menuSalesList.Click += new System.EventHandler(this.menuSalesList_Click);
            // 
            // menuSalesReturns
            // 
            this.menuSalesReturns.Name = "menuSalesReturns";
            this.menuSalesReturns.Size = new System.Drawing.Size(163, 22);
            this.menuSalesReturns.Text = "مرتجعات المبيعات";
            this.menuSalesReturns.Click += new System.EventHandler(this.menuSalesReturns_Click);
            // 
            // menuPurchases
            // 
            this.menuPurchases.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNewPurchase,
            this.menuPurchasesList,
            this.menuPurchasesReturns});
            this.menuPurchases.Name = "menuPurchases";
            this.menuPurchases.Size = new System.Drawing.Size(71, 20);
            this.menuPurchases.Text = "المشتريات";
            // 
            // menuNewPurchase
            // 
            this.menuNewPurchase.Name = "menuNewPurchase";
            this.menuNewPurchase.Size = new System.Drawing.Size(172, 22);
            this.menuNewPurchase.Text = "فاتورة مشتريات";
            this.menuNewPurchase.Click += new System.EventHandler(this.menuNewPurchase_Click);
            // 
            // menuPurchasesList
            // 
            this.menuPurchasesList.Name = "menuPurchasesList";
            this.menuPurchasesList.Size = new System.Drawing.Size(172, 22);
            this.menuPurchasesList.Text = "المشتريات";
            this.menuPurchasesList.Click += new System.EventHandler(this.menuPurchasesList_Click);
            // 
            // menuPurchasesReturns
            // 
            this.menuPurchasesReturns.Name = "menuPurchasesReturns";
            this.menuPurchasesReturns.Size = new System.Drawing.Size(172, 22);
            this.menuPurchasesReturns.Text = "مرتجعات المشتريات";
            this.menuPurchasesReturns.Click += new System.EventHandler(this.menuPurchasesReturns_Click);
            // 
            // menuPeople
            // 
            this.menuPeople.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCustomers,
            this.menuSuppliers});
            this.menuPeople.Name = "menuPeople";
            this.menuPeople.Size = new System.Drawing.Size(103, 20);
            this.menuPeople.Text = "العملاء والموردين";
            // 
            // menuCustomers
            // 
            this.menuCustomers.Name = "menuCustomers";
            this.menuCustomers.Size = new System.Drawing.Size(116, 22);
            this.menuCustomers.Text = "العملاء";
            this.menuCustomers.Click += new System.EventHandler(this.menuCustomers_Click);
            // 
            // menuSuppliers
            // 
            this.menuSuppliers.Name = "menuSuppliers";
            this.menuSuppliers.Size = new System.Drawing.Size(116, 22);
            this.menuSuppliers.Text = "الموردين";
            this.menuSuppliers.Click += new System.EventHandler(this.menuSuppliers_Click);
            // 
            // menuCash
            // 
            this.menuCash.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDeposit,
            this.menuWithdraw,
            this.menuExpenses,
            this.menuCashMovements});
            this.menuCash.Name = "menuCash";
            this.menuCash.Size = new System.Drawing.Size(47, 20);
            this.menuCash.Text = "الخزنة";
            // 
            // menuDeposit
            // 
            this.menuDeposit.Name = "menuDeposit";
            this.menuDeposit.Size = new System.Drawing.Size(128, 22);
            this.menuDeposit.Text = "إيداع";
            this.menuDeposit.Click += new System.EventHandler(this.menuDeposit_Click);
            // 
            // menuWithdraw
            // 
            this.menuWithdraw.Name = "menuWithdraw";
            this.menuWithdraw.Size = new System.Drawing.Size(128, 22);
            this.menuWithdraw.Text = "سحب";
            // 
            // menuExpenses
            // 
            this.menuExpenses.Name = "menuExpenses";
            this.menuExpenses.Size = new System.Drawing.Size(128, 22);
            this.menuExpenses.Text = "مصروفات";
            // 
            // menuCashMovements
            // 
            this.menuCashMovements.Name = "menuCashMovements";
            this.menuCashMovements.Size = new System.Drawing.Size(128, 22);
            this.menuCashMovements.Text = "حركة الخزنة";
            // 
            // menuReports
            // 
            this.menuReports.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSalesReport,
            this.menuPurchasesReport,
            this.menuStockReport,
            this.menuProfitReport,
            this.menuCashReport,
            this.menuCustomersBalance,
            this.menuSuppliersBalance,
            this.menuStockMovement});
            this.menuReports.Name = "menuReports";
            this.menuReports.Size = new System.Drawing.Size(54, 20);
            this.menuReports.Text = "التقارير";
            // 
            // menuSalesReport
            // 
            this.menuSalesReport.Name = "menuSalesReport";
            this.menuSalesReport.Size = new System.Drawing.Size(178, 22);
            this.menuSalesReport.Text = "تقرير المبيعات";
            this.menuSalesReport.Click += new System.EventHandler(this.menuSalesReport_Click);
            // 
            // menuPurchasesReport
            // 
            this.menuPurchasesReport.Name = "menuPurchasesReport";
            this.menuPurchasesReport.Size = new System.Drawing.Size(178, 22);
            this.menuPurchasesReport.Text = "تقرير المشتريات";
            this.menuPurchasesReport.Click += new System.EventHandler(this.menuPurchasesReport_Click_1);
            // 
            // menuStockReport
            // 
            this.menuStockReport.Name = "menuStockReport";
            this.menuStockReport.Size = new System.Drawing.Size(178, 22);
            this.menuStockReport.Text = "تقرير المخزون";
            // 
            // menuProfitReport
            // 
            this.menuProfitReport.Name = "menuProfitReport";
            this.menuProfitReport.Size = new System.Drawing.Size(178, 22);
            this.menuProfitReport.Text = "تقرير الأرباح";
            // 
            // menuCashReport
            // 
            this.menuCashReport.Name = "menuCashReport";
            this.menuCashReport.Size = new System.Drawing.Size(178, 22);
            this.menuCashReport.Text = "تقرير الخزنة";
            // 
            // menuCustomersBalance
            // 
            this.menuCustomersBalance.Name = "menuCustomersBalance";
            this.menuCustomersBalance.Size = new System.Drawing.Size(178, 22);
            this.menuCustomersBalance.Text = "تقرير ارصدة العملاء";
            this.menuCustomersBalance.Click += new System.EventHandler(this.menuCustomersBalance_Click);
            // 
            // menuSuppliersBalance
            // 
            this.menuSuppliersBalance.Name = "menuSuppliersBalance";
            this.menuSuppliersBalance.Size = new System.Drawing.Size(178, 22);
            this.menuSuppliersBalance.Text = "تقرير ارصدة الموردين";
            this.menuSuppliersBalance.Click += new System.EventHandler(this.menuSuppliersBalance_Click);
            // 
            // menuStockMovement
            // 
            this.menuStockMovement.Name = "menuStockMovement";
            this.menuStockMovement.Size = new System.Drawing.Size(178, 22);
            this.menuStockMovement.Text = "تقرير ارصدة المخزون";
            this.menuStockMovement.Click += new System.EventHandler(this.menuStockMovement_Click);
            // 
            // menuSettings
            // 
            this.menuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUsers,
            this.menuSystemSettings,
            this.menuPrintSettings,
            this.menuBackup});
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.Size = new System.Drawing.Size(65, 20);
            this.menuSettings.Text = "الإعدادات";
            // 
            // menuUsers
            // 
            this.menuUsers.Name = "menuUsers";
            this.menuUsers.Size = new System.Drawing.Size(156, 22);
            this.menuUsers.Text = "المستخدمين";
            this.menuUsers.Click += new System.EventHandler(this.menuUsers_Click);
            // 
            // menuSystemSettings
            // 
            this.menuSystemSettings.Name = "menuSystemSettings";
            this.menuSystemSettings.Size = new System.Drawing.Size(156, 22);
            this.menuSystemSettings.Text = "إعدادات النظام";
            // 
            // menuPrintSettings
            // 
            this.menuPrintSettings.Name = "menuPrintSettings";
            this.menuPrintSettings.Size = new System.Drawing.Size(156, 22);
            this.menuPrintSettings.Text = "إعادادات الطباعه";
            this.menuPrintSettings.Click += new System.EventHandler(this.menuPrintSettings_Click);
            // 
            // menuBackup
            // 
            this.menuBackup.Name = "menuBackup";
            this.menuBackup.Size = new System.Drawing.Size(156, 22);
            this.menuBackup.Text = "النسخ الاحتياطي";
            this.menuBackup.Click += new System.EventHandler(this.menuBackup_Click_1);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUserGuide,
            this.menuKeyboardShortcuts,
            this.SeparatorHelp,
            this.menuAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(58, 20);
            this.menuHelp.Text = "مساعدة";
            // 
            // menuUserGuide
            // 
            this.menuUserGuide.Name = "menuUserGuide";
            this.menuUserGuide.Size = new System.Drawing.Size(188, 22);
            this.menuUserGuide.Text = "دليل الاستخدام";
            this.menuUserGuide.Click += new System.EventHandler(this.menuUserGuide_Click);
            // 
            // menuKeyboardShortcuts
            // 
            this.menuKeyboardShortcuts.Name = "menuKeyboardShortcuts";
            this.menuKeyboardShortcuts.Size = new System.Drawing.Size(188, 22);
            this.menuKeyboardShortcuts.Text = "اختصارات لوحة المفاتيح";
            this.menuKeyboardShortcuts.Click += new System.EventHandler(this.menuKeyboardShortcuts_Click);
            // 
            // SeparatorHelp
            // 
            this.SeparatorHelp.Name = "SeparatorHelp";
            this.SeparatorHelp.Size = new System.Drawing.Size(188, 22);
            this.SeparatorHelp.Text = "────────────";
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(188, 22);
            this.menuAbout.Text = "حول البرنامج";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsername,
            this.lblUserRole,
            this.toolStripStatusLabel1,
            this.lblDate,
            this.lblTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 715);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1404, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblUsername
            // 
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 17);
            this.lblUsername.Text = "المستخدم";
            // 
            // lblUserRole
            // 
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(50, 17);
            this.lblUserRole.Text = "الصلاحية";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusLabel1.Text = "--------";
            // 
            // lblDate
            // 
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(38, 17);
            this.lblDate.Text = "التاريخ";
            // 
            // lblTime
            // 
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(37, 17);
            this.lblTime.Text = "الوقت";
            // 
            // timerClock
            // 
            this.timerClock.Enabled = true;
            this.timerClock.Interval = 1000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Snow;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(6, 323);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(176, 62);
            this.button3.TabIndex = 2;
            this.button3.Text = "جرد المخزن";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Snow;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(6, 244);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(176, 62);
            this.button2.TabIndex = 1;
            this.button2.Text = "إضافة تصنيف";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(6, 163);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 62);
            this.button1.TabIndex = 0;
            this.button1.Text = "إضافة صنف";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panalSide
            // 
            this.panalSide.BackColor = System.Drawing.Color.SlateGray;
            this.panalSide.Controls.Add(this.button5);
            this.panalSide.Controls.Add(this.button4);
            this.panalSide.Controls.Add(this.button3);
            this.panalSide.Controls.Add(this.button1);
            this.panalSide.Controls.Add(this.button2);
            this.panalSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.panalSide.Location = new System.Drawing.Point(0, 24);
            this.panalSide.Name = "panalSide";
            this.panalSide.Size = new System.Drawing.Size(187, 691);
            this.panalSide.TabIndex = 3;
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.LightCoral;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(6, 80);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(176, 62);
            this.button5.TabIndex = 4;
            this.button5.Text = "فاتورة مشتريات";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(6, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(176, 62);
            this.button4.TabIndex = 3;
            this.button4.Text = "فاتورة مبيعات";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(187, 24);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1217, 691);
            this.panelMain.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1404, 737);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panalSide);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panalSide.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuLogout;
        private System.Windows.Forms.ToolStripMenuItem Separator;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuInventory;
        private System.Windows.Forms.ToolStripMenuItem menuCategories;
        private System.Windows.Forms.ToolStripMenuItem menuProducts;
        private System.Windows.Forms.ToolStripMenuItem Separator_inv;
        private System.Windows.Forms.ToolStripMenuItem menuStockInventory;
        private System.Windows.Forms.ToolStripMenuItem menuSales;
        private System.Windows.Forms.ToolStripMenuItem menuNewSale;
        private System.Windows.Forms.ToolStripMenuItem menuSalesList;
        private System.Windows.Forms.ToolStripMenuItem menuSalesReturns;
        private System.Windows.Forms.ToolStripMenuItem menuPurchases;
        private System.Windows.Forms.ToolStripMenuItem menuNewPurchase;
        private System.Windows.Forms.ToolStripMenuItem menuPurchasesList;
        private System.Windows.Forms.ToolStripMenuItem menuPurchasesReturns;
        private System.Windows.Forms.ToolStripMenuItem menuPeople;
        private System.Windows.Forms.ToolStripMenuItem menuCustomers;
        private System.Windows.Forms.ToolStripMenuItem menuSuppliers;
        private System.Windows.Forms.ToolStripMenuItem menuCash;
        private System.Windows.Forms.ToolStripMenuItem menuDeposit;
        private System.Windows.Forms.ToolStripMenuItem menuWithdraw;
        private System.Windows.Forms.ToolStripMenuItem menuExpenses;
        private System.Windows.Forms.ToolStripMenuItem menuCashMovements;
        private System.Windows.Forms.ToolStripMenuItem menuReports;
        private System.Windows.Forms.ToolStripMenuItem menuSalesReport;
        private System.Windows.Forms.ToolStripMenuItem menuPurchasesReport;
        private System.Windows.Forms.ToolStripMenuItem menuStockReport;
        private System.Windows.Forms.ToolStripMenuItem menuProfitReport;
        private System.Windows.Forms.ToolStripMenuItem menuCashReport;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuUsers;
        private System.Windows.Forms.ToolStripMenuItem menuSystemSettings;
        private System.Windows.Forms.ToolStripMenuItem menuBackup;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblUsername;
        private System.Windows.Forms.ToolStripStatusLabel lblUserRole;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblDate;
        private System.Windows.Forms.ToolStripStatusLabel lblTime;
        private System.Windows.Forms.Timer timerClock;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuUserGuide;
        private System.Windows.Forms.ToolStripMenuItem menuKeyboardShortcuts;
        private System.Windows.Forms.ToolStripMenuItem SeparatorHelp;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.ToolStripMenuItem menuPrintSettings;
        private System.Windows.Forms.ToolStripMenuItem menuCustomersBalance;
        private System.Windows.Forms.ToolStripMenuItem menuSuppliersBalance;
        private System.Windows.Forms.ToolStripMenuItem menuStockMovement;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panalSide;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
    }
}