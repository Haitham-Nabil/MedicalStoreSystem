using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Forms.Printing;
using MedicalStoreSystem.Forms.Reports;
using MedicalStoreSystem.Helpers;
using MedicalStoreSystem.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;


namespace MedicalStoreSystem.Forms.Sales
{
    public partial class POSForm : Form
    {
        private SaleDAL saleDAL = new SaleDAL();
        private ProductDAL productDAL = new ProductDAL();
        private CustomerDAL customerDAL = new CustomerDAL();

        public POSForm()
        {
            InitializeComponent();


            // RTL
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // حجم ثابت
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Width = 1200;
            this.Height = 750;

            // 🔥 Center Screen يدوي (حل المشكلة)
            this.StartPosition = FormStartPosition.Manual;

            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );

            //this.RightToLeft = RightToLeft.Yes;
            //this.RightToLeftLayout = true;
            //this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true; // لاستقبال أحداث الكيبورد


        }

        private void POSForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadProducts();
            InitializeNewSale();
            SetupDataGridView();

            // تركيز على حقل الباركود
            txtBarcode.Focus();
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = customerDAL.GetActiveCustomers();
                cmbCustomer.DataSource = customers;
                cmbCustomer.DisplayMember = "CustomerName";
                cmbCustomer.ValueMember = "CustomerID";

                // تحديد عميل نقدي افتراضياً
                foreach (Customer customer in customers)
                {
                    if (customer.CustomerCode == "CUST001")
                    {
                        cmbCustomer.SelectedValue = customer.CustomerID;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل العملاء:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = productDAL.GetActiveProducts();
                cmbProduct.DataSource = products;
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";
                cmbProduct.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل المنتجات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            dgvCart.AllowUserToAddRows = false;
            dgvCart.ReadOnly = false;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.RowTemplate.Height = 35; // ارتفاع الصف

            // تنسيق الخط
            dgvCart.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 11F);
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
        }

        private void InitializeNewSale()
        {
            lblSaleNumber.Text = saleDAL.GenerateSaleNumber();
            lblSaleDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            lblCashier.Text = CurrentSession.LoggedInUser.FullName;

            dgvCart.Rows.Clear();

            txtBarcode.Clear();
            cmbProduct.SelectedIndex = -1;
            txtUnitPrice.Clear();
            numQuantity.Value = 1;

            lblTotalAmount.Text = "0.00";
            txtDiscount.Text = "0";
            lblNetAmount.Text = "0.00";
            txtPaid.Text = "0";
            lblChange.Text = "0.00";

            rbCash.Checked = true;

            txtBarcode.Focus();
        }

        // البحث بالباركود أو الكود
        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchProduct();
            }
        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            SearchProduct();
        }

        private void SearchProduct()
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                return;

            try
            {
                string searchTerm = txtBarcode.Text.Trim();

                // البحث بالباركود أو الكود
                Product product = productDAL.GetProductByBarcode(searchTerm);

                if (product == null)
                {
                    // البحث بالكود
                    var products = productDAL.GetActiveProducts();
                    foreach (Product p in products)
                    {
                        if (p.ProductCode.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                        {
                            product = p;
                            break;
                        }
                    }
                }

                if (product != null)
                {
                    cmbProduct.SelectedValue = product.ProductID;
                    numQuantity.Focus();
                    numQuantity.Select(0, numQuantity.Text.Length);
                }
                else
                {
                    MessageBox.Show("المنتج غير موجود!", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في البحث:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // عند اختيار منتج
        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex != -1)
            {
                try
                {
                    Product product = (Product)cmbProduct.SelectedItem;
                    txtUnitPrice.Text = product.SalePrice.ToString("F2");

                    // التحقق من المخزون
                    if (product.CurrentStock <= 0)
                    {
                        MessageBox.Show($"المنتج '{product.ProductName}' غير متوفر في المخزون!",
                            "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (product.CurrentStock <= product.MinStock)
                    {
                        MessageBox.Show($"تحذير: المنتج '{product.ProductName}' كميته قليلة!\nالمتبقي: {product.CurrentStock}",
                            "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch { }
            }
        }

        // إضافة للسلة
        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            AddToCart();
        }

        private void numQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                AddToCart();
            }
        }

        private void AddToCart()
        {
            // التحقق من الإدخال
            if (cmbProduct.SelectedIndex == -1)
            {
                MessageBox.Show("يرجى اختيار المنتج", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProduct.Focus();
                return;
            }

            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("يرجى إدخال الكمية", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numQuantity.Focus();
                return;
            }

            try
            {
                Product product = (Product)cmbProduct.SelectedItem;
                int quantity = (int)numQuantity.Value;
                decimal unitPrice = decimal.Parse(txtUnitPrice.Text);
                decimal totalPrice = quantity * unitPrice;

                // التحقق من المخزون
                if (product.CurrentStock < quantity)
                {
                    MessageBox.Show($"الكمية المتاحة غير كافية!\nالمتاح: {product.CurrentStock}",
                        "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // التحقق من عدم تكرار المنتج
                bool found = false;
                foreach (DataGridViewRow row in dgvCart.Rows)
                {
                    if (Convert.ToInt32(row.Cells["ProductID"].Value) == product.ProductID)
                    {
                        // زيادة الكمية
                        int oldQuantity = Convert.ToInt32(row.Cells["الكمية"].Value);
                        int newQuantity = oldQuantity + quantity;

                        if (product.CurrentStock < newQuantity)
                        {
                            MessageBox.Show($"الكمية المتاحة غير كافية!\nالمتاح: {product.CurrentStock}",
                                "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        row.Cells["الكمية"].Value = newQuantity;
                        row.Cells["الإجمالي"].Value = newQuantity * unitPrice;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // إضافة صف جديد
                    int rowIndex = dgvCart.Rows.Add();
                    DataGridViewRow newRow = dgvCart.Rows[rowIndex];

                    newRow.Cells["ProductID"].Value = product.ProductID;
                    newRow.Cells["ProductName"].Value = product.ProductName;
                    newRow.Cells["unitPrice"].Value = unitPrice;
                    newRow.Cells["quantity"].Value = quantity;
                    newRow.Cells["totalPrice"].Value = totalPrice;
                }

                // مسح الحقول
                txtBarcode.Clear();
                cmbProduct.SelectedIndex = -1;
                txtUnitPrice.Clear();
                numQuantity.Value = 1;

                txtBarcode.Focus();

                // حساب الإجماليات
                CalculateTotals();

                // صوت نجاح (اختياري)
                System.Media.SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // حذف من السلة
        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCart.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                dgvCart.Rows.RemoveAt(e.RowIndex);
                CalculateTotals();
            }
        }

        // تعديل الكمية مباشرة من الجدول
        private void dgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCart.Columns["quantity"].Index)
            {
                try
                {
                    DataGridViewRow row = dgvCart.Rows[e.RowIndex];
                    int quantity = Convert.ToInt32(row.Cells["quantity"].Value);
                    decimal unitPrice = Convert.ToDecimal(row.Cells["unitPrice"].Value);

                    row.Cells["totalPrice"].Value = quantity * unitPrice;
                    CalculateTotals();
                }
                catch
                {
                    MessageBox.Show("يرجى إدخال رقم صحيح للكمية", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // حساب الإجماليات
        private void CalculateTotals()
        {
            try
            {
                decimal totalAmount = 0;

                foreach (DataGridViewRow row in dgvCart.Rows)
                {
                    if (row.Cells["TotalPrice"].Value != null)
                    {
                        totalAmount += Convert.ToDecimal(row.Cells["totalPrice"].Value);
                    }
                }

                lblTotalAmount.Text = totalAmount.ToString("F2");

                decimal discount = 0;
                if (!string.IsNullOrWhiteSpace(txtDiscount.Text))
                    discount = decimal.Parse(txtDiscount.Text);

                decimal netAmount = totalAmount - discount;
                lblNetAmount.Text = netAmount.ToString("F2");

                decimal paid = 0;
                if (!string.IsNullOrWhiteSpace(txtPaid.Text))
                    paid = decimal.Parse(txtPaid.Text);

                decimal change = paid - netAmount;
                lblChange.Text = change.ToString("F2");

                // تلوين المتبقي/الباقي
                if (change >= 0)
                {
                    lblChange.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblChange.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch { }
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void txtPaid_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        // إتمام البيع
        private void btnComplete_Click(object sender, EventArgs e)
        {
            CompleteSale();
        }

        private void CompleteSale()
        {
            // التحقق من وجود أصناف
            if (dgvCart.Rows.Count == 0)
            {
                MessageBox.Show("يرجى إضافة أصناف للفاتورة", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }

            // التحقق من العميل
            if (cmbCustomer.SelectedIndex == -1)
            {
                MessageBox.Show("يرجى اختيار العميل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCustomer.Focus();
                return;
            }

            try
            {
                decimal netAmount = decimal.Parse(lblNetAmount.Text);
                decimal paidAmount = 0;

                if (!string.IsNullOrWhiteSpace(txtPaid.Text))
                    paidAmount = decimal.Parse(txtPaid.Text);

                // التحقق من المدفوع
                if (rbCash.Checked && paidAmount < netAmount)
                {
                    DialogResult result = MessageBox.Show(
                        $"المدفوع أقل من المطلوب!\nالمطلوب: {netAmount:F2}\nالمدفوع: {paidAmount:F2}\n\nهل تريد المتابعة كفاتورة آجلة؟",
                        "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        txtPaid.Focus();
                        return;
                    }

                    rbCredit.Checked = true;
                }

                // التأكيد النهائي
                string paymentType = rbCash.Checked ? "نقدي" : (rbCredit.Checked ? "آجل" : "فيزا");
                string confirmMessage = $"إتمام عملية البيع؟\n\n";
                confirmMessage += $"العميل: {cmbCustomer.Text}\n";
                confirmMessage += $"الصافي: {netAmount:F2} جنيه\n";
                confirmMessage += $"المدفوع: {paidAmount:F2} جنيه\n";
                confirmMessage += $"طريقة الدفع: {paymentType}";

                if (paidAmount > netAmount)
                    confirmMessage += $"\n\nالباقي للعميل: {(paidAmount - netAmount):F2} جنيه";
                else if (paidAmount < netAmount)
                    confirmMessage += $"\n\nالمتبقي: {(netAmount - paidAmount):F2} جنيه";

                DialogResult confirmResult = MessageBox.Show(confirmMessage, "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                    return;

                // إنشاء كائن الفاتورة
                Sale sale = new Sale
                {
                    SaleNumber = lblSaleNumber.Text,
                    SaleDate = DateTime.Now,
                    CustomerID = Convert.ToInt32(cmbCustomer.SelectedValue),
                    TotalAmount = decimal.Parse(lblTotalAmount.Text),
                    DiscountAmount = decimal.Parse(txtDiscount.Text),
                    NetAmount = netAmount,
                    PaidAmount = paidAmount,
                    RemainingAmount = netAmount - paidAmount,
                    PaymentType = paymentType,
                    IsPaid = (paidAmount >= netAmount),
                    UserID = CurrentSession.LoggedInUser.UserID,
                    Notes = ""
                };

                // إضافة التفاصيل
                foreach (DataGridViewRow row in dgvCart.Rows)
                {
                    SaleDetail detail = new SaleDetail
                    {
                        ProductID = Convert.ToInt32(row.Cells["ProductID"].Value),
                        ProductName = row.Cells["ProductName"].Value.ToString(),
                        Quantity = Convert.ToInt32(row.Cells["Quantity"].Value),
                        UnitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value),
                        TotalPrice = Convert.ToDecimal(row.Cells["TotalPrice"].Value)
                    };

                    sale.Details.Add(detail);
                }

                // حفظ الفاتورة
                //bool success = saleDAL.InsertSale(sale);
                SaleDAL saleDAL = new SaleDAL();
                int saleID = saleDAL.InsertSale(sale);

                //if (success)
                if (saleID > 0)
                {
                    // رسالة نجاح
                    string successMessage = "تم إتمام عملية البيع بنجاح!\n\n";
                    successMessage += $"رقم الفاتورة: {sale.SaleNumber}\n";
                    successMessage += $"الصافي: {netAmount:F2} جنيه\n";

                    if (paidAmount > netAmount)
                    {
                        decimal change = paidAmount - netAmount;
                        successMessage += $"\n✓ الباقي للعميل: {change:F2} جنيه";
                    }

                    MessageBox.Show(successMessage, "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // طباعة الفاتورة (اختياري)
                    /*DialogResult printResult = MessageBox.Show("هل تريد طباعة الفاتورة؟", "طباعة",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (printResult == DialogResult.Yes)
                    {
                        PrintInvoice(sale);
                    }*/
                    // طباعة الفاتورة (متقدمة)
                    // طباعة الفاتورة
                    DialogResult result = MessageBox.Show("هل تريد طباعة الفاتورة؟", "طباعة",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                    if (result == DialogResult.Yes)
                    {
                        PrintInvoice(saleID);
                    }
                    

                    // فاتورة جديدة
                    InitializeNewSale();
                }
                else
                {
                    MessageBox.Show("فشل في حفظ الفاتورة", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // طباعة الفاتورة (بسيطة)
        /*private void PrintInvoice(Sale sale)
        {
            try
            {
                // يمكن استخدام Crystal Reports أو طباعة نصية بسيطة
                string invoice = "═══════════════════════════════════\n";
                invoice += "      نظام إدارة المستلزمات الطبية      \n";
                invoice += "═══════════════════════════════════\n\n";
                invoice += $"رقم الفاتورة: {sale.SaleNumber}\n";
                invoice += $"التاريخ: {sale.SaleDate:yyyy-MM-dd HH:mm}\n";
                invoice += $"العميل: {cmbCustomer.Text}\n";
                invoice += $"الكاشير: {CurrentSession.LoggedInUser.FullName}\n";
                invoice += "───────────────────────────────────\n\n";
                invoice += "الأصناف:\n";
                invoice += "───────────────────────────────────\n";

                foreach (var detail in sale.Details)
                {
                    invoice += $"{detail.ProductName}\n";
                    invoice += $"  {detail.Quantity} × {detail.UnitPrice:F2} = {detail.TotalPrice:F2} جنيه\n\n";
                }

                invoice += "═══════════════════════════════════\n";
                invoice += $"الإجمالي:        {sale.TotalAmount:F2} جنيه\n";

                if (sale.DiscountAmount > 0)
                    invoice += $"الخصم:           {sale.DiscountAmount:F2} جنيه\n";

                invoice += $"الصافي:          {sale.NetAmount:F2} جنيه\n";
                invoice += $"المدفوع:         {sale.PaidAmount:F2} جنيه\n";

                if (sale.RemainingAmount != 0)
                {
                    if (sale.RemainingAmount > 0)
                        invoice += $"المتبقي:         {sale.RemainingAmount:F2} جنيه\n";
                    else
                        invoice += $"الباقي:          {Math.Abs(sale.RemainingAmount):F2} جنيه\n";
                }

                invoice += "═══════════════════════════════════\n";
                invoice += $"طريقة الدفع: {sale.PaymentType}\n\n";
                invoice += "       شكراً لتعاملكم معنا       \n";
                invoice += "═══════════════════════════════════\n";

                // عرض في نافذة للطباعة
                Form printForm = new Form();
                printForm.Text = "طباعة الفاتورة";
                printForm.Size = new System.Drawing.Size(400, 600);
                printForm.StartPosition = FormStartPosition.CenterParent;
                printForm.RightToLeft = RightToLeft.Yes;

                TextBox txtInvoice = new TextBox();
                txtInvoice.Multiline = true;
                txtInvoice.ScrollBars = ScrollBars.Vertical;
                txtInvoice.Dock = DockStyle.Fill;
                txtInvoice.Font = new System.Drawing.Font("Courier New", 10F);
                txtInvoice.Text = invoice;
                txtInvoice.ReadOnly = true;

                Panel panelButtons = new Panel();
                panelButtons.Dock = DockStyle.Bottom;
                panelButtons.Height = 50;

                Button btnPrint = new Button();
                btnPrint.Text = "طباعة";
                btnPrint.Location = new System.Drawing.Point(150, 10);
                btnPrint.Size = new System.Drawing.Size(100, 30);
                btnPrint.Click += (s, ev) => {
                    // هنا يمكن إضافة كود الطباعة الفعلي
                    MessageBox.Show("تم إرسال الفاتورة للطابعة", "طباعة",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    printForm.Close();
                };

                panelButtons.Controls.Add(btnPrint);
                printForm.Controls.Add(txtInvoice);
                printForm.Controls.Add(panelButtons);

                printForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في الطباعة:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/


        //طباعة الفاتورة (متقدمة)****************************************************************************************
        private void PrintInvoice(int saleID)
        {
            try
            {
                // جلب بيانات الفاتورة
                string queryInvoice = @"
            SELECT s.SaleID, s.SaleNumber, s.SaleDate, s.TotalAmount, 
                   s.DiscountAmount, s.NetAmount, s.PaidAmount, s.RemainingAmount,
                   ISNULL(c.CustomerName, 'عميل نقدي') AS CustomerName,
                   u.UserName AS CashierName
            FROM Sales s
            LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
            INNER JOIN Users u ON s.UserID = u.UserID
            WHERE s.SaleID = @SaleID";

                SqlParameter[] paramsInvoice = { new SqlParameter("@SaleID", saleID) };
                DataTable dtInvoice = DatabaseConnection.ExecuteDataTable(queryInvoice, paramsInvoice);

                // جلب أصناف الفاتورة
                string queryItems = @"
            SELECT sd.SaleDetailID, p.ProductName, sd.Quantity, sd.UnitPrice, sd.TotalPrice
            FROM SalesDetails sd
            INNER JOIN Products p ON sd.ProductID = p.ProductID
            WHERE sd.SaleID = @SaleID
            ORDER BY sd.SaleDetailID";

                SqlParameter[] paramsItems = { new SqlParameter("@SaleID", saleID) };
                DataTable dtItems = DatabaseConnection.ExecuteDataTable(queryItems, paramsItems);

                // طباعة الفاتورة
                SaleInvoicePrint printInvoice = new SaleInvoicePrint(dtInvoice, dtItems);
                printInvoice.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في طباعة الفاتورة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // نهاية طباعة الفاتورة************************************************************************************************

        //RDLC طباعة الفاتوره بطريقة
        /*private void PrintInvoice(int saleID)
        {
            try
            {
                // خيار: طباعة عادية أو RDLC Report
                DialogResult result = MessageBox.Show(
                    "اختر نوع التقرير:\n\nنعم: تقرير RDLC (احترافي)\nلا: طباعة عادية",
                    "نوع التقرير",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show(Convert.ToString(saleID));
                    // عرض تقرير RDLC
                    SaleInvoiceReportViewer reportViewer = new SaleInvoiceReportViewer(saleID);
                    reportViewer.ShowDialog();
                }
                else if (result == DialogResult.No)
                {
                    // الطباعة العادية (الكود السابق)
                    string queryInvoice = @"
                SELECT s.SaleID, s.SaleNumber, s.SaleDate, s.TotalAmount, 
                       s.DiscountAmount, s.NetAmount, s.PaidAmount, s.RemainingAmount,
                       ISNULL(c.CustomerName, 'عميل نقدي') AS CustomerName,
                       u.UserName AS CashierName
                FROM Sales s
                LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                INNER JOIN Users u ON s.UserID = u.UserID
                WHERE s.SaleID = @SaleID";

                    SqlParameter[] paramsInvoice = { new SqlParameter("@SaleID", saleID) };
                    DataTable dtInvoice = DatabaseConnection.ExecuteDataTable(queryInvoice, paramsInvoice);

                    string queryItems = @"
                SELECT sd.SaleDetailID, p.ProductName, sd.Quantity, sd.UnitPrice, sd.TotalPrice
                FROM SalesDetails sd
                INNER JOIN Products p ON sd.ProductID = p.ProductID
                WHERE sd.SaleID = @SaleID
                ORDER BY sd.SaleDetailID";

                    SqlParameter[] paramsItems = { new SqlParameter("@SaleID", saleID) };
                    DataTable dtItems = DatabaseConnection.ExecuteDataTable(queryItems, paramsItems);

                    Printing.SaleInvoicePrint printInvoice = new Printing.SaleInvoicePrint(dtInvoice, dtItems);
                    printInvoice.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في طباعة الفاتورة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/
        // نهاية RDLC طباعة الفاتوره بطريقة

        // فاتورة جديدة
        private void btnNewSale_Click(object sender, EventArgs e)
        {
            if (dgvCart.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show("هل تريد إلغاء الفاتورة الحالية؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            InitializeNewSale();
        }

        // إلغاء الفاتورة
        private void btnCancelSale_Click(object sender, EventArgs e)
        {
            if (dgvCart.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show("هل تريد إلغاء الفاتورة والخروج؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        // اختصارات الكيبورد
        private void POSForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:
                    btnNewSale_Click(sender, e);
                    e.Handled = true;
                    break;

                case Keys.F10:
                    btnComplete_Click(sender, e);
                    e.Handled = true;
                    break;

                case Keys.F12:
                    btnCancelSale_Click(sender, e);
                    e.Handled = true;
                    break;

                case Keys.F1:
                    txtBarcode.Focus();
                    e.Handled = true;
                    break;

                case Keys.F2:
                    cmbProduct.Focus();
                    e.Handled = true;
                    break;

                case Keys.F3:
                    numQuantity.Focus();
                    e.Handled = true;
                    break;

                case Keys.F4:
                    txtDiscount.Focus();
                    e.Handled = true;
                    break;

                case Keys.F5:
                    txtPaid.Focus();
                    e.Handled = true;
                    break;

                case Keys.Escape:
                    if (dgvCart.Rows.Count == 0)
                        this.Close();
                    e.Handled = true;
                    break;
            }
        }

        // السماح بالأرقام فقط
        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtPaid_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtDiscount_KeyPress(sender, e);

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnComplete_Click(sender, e);
            }
        }

        // عند تغيير طريقة الدفع
        private void rbCash_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCash.Checked)
            {
                txtPaid.Enabled = true;
                txtPaid.Text = lblNetAmount.Text;
                txtPaid.SelectAll();
            }
        }

        private void rbCredit_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCredit.Checked)
            {
                txtPaid.Text = "0";
                txtPaid.Enabled = false;
            }
        }

        private void rbVisa_CheckedChanged(object sender, EventArgs e)
        {
            if (rbVisa.Checked)
            {
                txtPaid.Enabled = true;
                txtPaid.Text = lblNetAmount.Text;
                txtPaid.SelectAll();
            }
        }

        private void dgvCart_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCart.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                DialogResult result = MessageBox.Show("هل تريد حذف هذا الصنف؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgvCart.Rows.RemoveAt(e.RowIndex);
                    CalculateTotals();
                }
            }
        }

    }
}
