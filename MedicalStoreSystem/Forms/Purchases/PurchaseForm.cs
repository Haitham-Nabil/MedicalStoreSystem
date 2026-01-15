using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Helpers;
using MedicalStoreSystem.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MedicalStoreSystem.Forms.Purchases
{
    public partial class PurchaseForm : Form
    {
        private PurchaseDAL purchaseDAL = new PurchaseDAL();
        private SupplierDAL supplierDAL = new SupplierDAL();
        private ProductDAL productDAL = new ProductDAL();

        public PurchaseForm()
        {
            InitializeComponent();

            /*this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.WindowState = FormWindowState.Maximized;*/
        }

        private void PurchaseForm_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            LoadProducts();
            InitializeNewPurchase();
            SetupDataGridView();
        }

        private void LoadSuppliers()
        {
            try
            {
                cmbSupplier.DataSource = null;
                var suppliers = supplierDAL.GetActiveSuppliers();
                cmbSupplier.DataSource = suppliers;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
                cmbSupplier.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل الموردين:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                cmbProduct.DataSource = null;
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
            dgvItems.AllowUserToAddRows = false;
            dgvItems.ReadOnly = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;
        }

        private void InitializeNewPurchase()
        {
            txtPurchaseNumber.Text = purchaseDAL.GeneratePurchaseNumber();
            dtpPurchaseDate.Value = DateTime.Now;
            cmbSupplier.SelectedIndex = -1;
            txtSupplierInvoice.Clear();

            dgvItems.Rows.Clear();

            cmbProduct.SelectedIndex = -1;
            txtQuantity.Text = "1";
            txtUnitPrice.Text = "0";
            txtLineTotal.Text = "0";
            dtpExpiryDate.Checked = false;
            txtBatchNumber.Clear();

            txtTotalAmount.Text = "0";
            txtDiscountAmount.Text = "0";
            txtNetAmount.Text = "0";
            txtPaidAmount.Text = "0";
            txtRemainingAmount.Text = "0";
            cmbPaymentType.SelectedIndex = 0; // نقدي
            txtNotes.Clear();
        }

        // عند اختيار منتج
        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex != -1)
            {
                try
                {
                    Product product = (Product)cmbProduct.SelectedItem;
                    txtUnitPrice.Text = product.CostPrice.ToString();

                    // إذا المنتج له صلاحية
                    dtpExpiryDate.Checked = product.HasExpiry;

                    CalculateLineTotal();
                }
                catch { }
            }
        }

        // حساب إجمالي السطر  private void CalculateLineTotal()
        private void CalculateLineTotal()
        {
            try
            {
                /*decimal quantity = decimal.Parse(txtQuantity.Text);
                decimal unitPrice = decimal.Parse(txtUnitPrice.Text);
                decimal total = quantity * unitPrice;
                txtLineTotal.Text = total.ToString("F2");*/
                decimal.TryParse(txtQuantity.Text, out decimal quantity);
                decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice);
                decimal lineTotal = quantity * unitPrice;
                txtLineTotal.Text = lineTotal.ToString("F2");
            }
            catch
            {
                txtLineTotal.Text = "0";
            }

            


        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateLineTotal();
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateLineTotal();
        }

        // إضافة صنف للفاتورة
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (cmbProduct.SelectedIndex == -1)
            {
                MessageBox.Show("يرجى اختيار المنتج", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProduct.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQuantity.Text) || decimal.Parse(txtQuantity.Text) <= 0)
            {
                MessageBox.Show("يرجى إدخال الكمية", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUnitPrice.Text) || decimal.Parse(txtUnitPrice.Text) <= 0)
            {
                MessageBox.Show("يرجى إدخال سعر الشراء", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return;
            }

            try
            {
                Product product = (Product)cmbProduct.SelectedItem;

                // التحقق من عدم تكرار المنتج
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    if (Convert.ToInt32(row.Cells["ProductID"].Value) == product.ProductID)
                    {
                        MessageBox.Show("هذا المنتج موجود بالفعل في الفاتورة!", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // إضافة الصف
                int rowIndex = dgvItems.Rows.Add();
                DataGridViewRow newRow = dgvItems.Rows[rowIndex];

                newRow.Cells["ProductID"].Value = product.ProductID;
                newRow.Cells["ProductName"].Value = product.ProductName;
                newRow.Cells["Quantity"].Value = int.Parse(txtQuantity.Text);
                newRow.Cells["UnitPrice"].Value = decimal.Parse(txtUnitPrice.Text);
                newRow.Cells["LineTotal"].Value = decimal.Parse(txtLineTotal.Text);
                newRow.Cells["ExpiryDate"].Value = dtpExpiryDate.Checked ?
                    dtpExpiryDate.Value.ToString("yyyy-MM-dd") : "";
                newRow.Cells["BatchNumber"].Value = txtBatchNumber.Text;
                newRow.Cells["Delete"].Value = "حذف";
                
                // مسح الحقول
                cmbProduct.SelectedIndex = -1;
                txtQuantity.Text = "1";
                txtUnitPrice.Text = "0";
                txtLineTotal.Text = "0";
                dtpExpiryDate.Checked = false;
                txtBatchNumber.Clear();

                cmbProduct.Focus();

                // حساب الإجماليات
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // حذف صنف من الفاتورة
        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvItems.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                DialogResult result = MessageBox.Show("هل تريد حذف هذا الصنف؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgvItems.Rows.RemoveAt(e.RowIndex);
                    CalculateTotals();
                }
            }
        }

        // حساب الإجماليات
        private void CalculateTotals()
        {
            try
            {
                decimal totalAmount = 0;

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    if (row.Cells["LineTotal"].Value != null)
                    {
                        totalAmount += Convert.ToDecimal(row.Cells["LineTotal"].Value);
                    }
                }

                txtTotalAmount.Text = totalAmount.ToString("F2");

                decimal discountAmount = decimal.Parse(txtDiscountAmount.Text);
                decimal netAmount = totalAmount - discountAmount;
                txtNetAmount.Text = netAmount.ToString("F2");

                decimal paidAmount = decimal.Parse(txtPaidAmount.Text);
                decimal remainingAmount = netAmount - paidAmount;
                txtRemainingAmount.Text = remainingAmount.ToString("F2");

                // تلوين المتبقي
                if (remainingAmount > 0)
                    txtRemainingAmount.BackColor = System.Drawing.Color.LightCoral;
                else
                    txtRemainingAmount.BackColor = System.Drawing.Color.LightGreen;
            }
            catch { }
        }

        private void txtDiscountAmount_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void txtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        // حفظ الفاتورة
        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (cmbSupplier.SelectedIndex == -1)
            {
                MessageBox.Show("يرجى اختيار المورد", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSupplier.Focus();
                return;
            }

            if (dgvItems.Rows.Count == 0)
            {
                MessageBox.Show("يرجى إضافة أصناف للفاتورة", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProduct.Focus();
                return;
            }

            // التأكيد
            DialogResult result = MessageBox.Show(
                $"هل تريد حفظ فاتورة المشتريات؟\n\nالمورد: {cmbSupplier.Text}\nالصافي: {txtNetAmount.Text} جنيه",
                "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;

            try
            {
                // إنشاء كائن الفاتورة
                Purchase purchase = new Purchase
                {
                    PurchaseNumber = txtPurchaseNumber.Text,
                    PurchaseDate = dtpPurchaseDate.Value,
                    SupplierID = Convert.ToInt32(cmbSupplier.SelectedValue),
                    SupplierInvoiceNumber = txtSupplierInvoice.Text.Trim(),
                    TotalAmount = decimal.Parse(txtTotalAmount.Text),
                    DiscountAmount = decimal.Parse(txtDiscountAmount.Text),
                    NetAmount = decimal.Parse(txtNetAmount.Text),
                    PaidAmount = decimal.Parse(txtPaidAmount.Text),
                    RemainingAmount = decimal.Parse(txtRemainingAmount.Text),
                    PaymentType = cmbPaymentType.Text,
                    UserID = CurrentSession.LoggedInUser.UserID,
                    Notes = txtNotes.Text.Trim()
                };

                // إضافة التفاصيل
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    PurchaseDetail detail = new PurchaseDetail
                    {
                        ProductID = Convert.ToInt32(row.Cells["ProductID"].Value),
                        Quantity = Convert.ToInt32(row.Cells["Quantity"].Value),
                        UnitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value),
                        TotalPrice = Convert.ToDecimal(row.Cells["LineTotal"].Value),
                        ExpiryDate = !string.IsNullOrWhiteSpace(row.Cells["ExpiryDate"].Value?.ToString()) ?
                            DateTime.Parse(row.Cells["ExpiryDate"].Value.ToString()) : (DateTime?)null,
                        BatchNumber = row.Cells["BatchNumber"].Value?.ToString()
                    };

                    purchase.Details.Add(detail);
                }

                // حفظ الفاتورة
                bool success = purchaseDAL.InsertPurchase(purchase);

                if (success)
                {
                    MessageBox.Show("تم حفظ فاتورة المشتريات بنجاح!\n\nتم تحديث المخزون والخزنة تلقائياً",
                        "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    InitializeNewPurchase();
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (dgvItems.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show("هل تريد إلغاء الفاتورة الحالية؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            InitializeNewPurchase();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // السماح بالأرقام فقط في الكمية
        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // السماح بالأرقام والنقطة في السعر
        private void txtUnitPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // السماح بنقطة واحدة فقط
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDiscountAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtUnitPrice_KeyPress(sender, e);
        }

        private void txtPaidAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtUnitPrice_KeyPress(sender, e);
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchProduct();
            }
        }
        // البحث عن المنتج بالباركود أو الكود
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
                    txtQuantity.Focus();
                    txtQuantity.Select(0, txtQuantity.Text.Length);
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
        // إظهار وإخفاء نافذة البحث
        private void btnSearch_Click(object sender, EventArgs e)
        {
            gbSearch.Visible = !gbSearch.Visible;
            if (gbSearch.Visible)
            {
                txtItemSearch.Clear();
                txtItemSearch.Focus();
                LoadProducts();
            }
        }
        // البحث عند الضغط على إنتر في خانة البحث
        private void txtItemSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchProducts();
                e.Handled = true;
            }
        }

        // البحث عن المنتجات في نافذة البحث
        private void SearchProducts()
        {
            if (string.IsNullOrWhiteSpace(txtItemSearch.Text))
            {
                LoadProducts();
                return;
            }

            try
            {
                string query = @"
                                SELECT 
                                    p.ProductID AS N'الرقم',
                                    p.Barcode AS N'الباركود',
                                    p.ProductName AS N'اسم المنتج',
                                    c.CategoryName AS N'التصنيف',
                                    p.UnitName AS N'الوحدة',
                                    p.CostPrice AS N'سعر الشراء',
                                    p.CurrentStock AS N'الكمية',
                                    s.SupplierName AS N'المورد',
                                    CASE WHEN p.IsActive = 1 THEN N'نشط' ELSE N'غير نشط' END AS N'الحالة'
                                FROM Products p
                                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                                LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                                WHERE 
                                    p.ProductName LIKE @search
                                    OR p.ProductCode LIKE @search
                                    OR p.Barcode LIKE @search
                                ORDER BY p.ProductName";

                    SqlParameter[] parameters = { new SqlParameter("@search", "%" + txtItemSearch.Text.Trim() + "%") };

                dgvItemSearch.DataSource = DAL.DatabaseConnection.ExecuteDataTable(query, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في البحث:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //تحميل إسم الصنف في خانة كود الصنف عند الضغط مرتين على الصف
        private void dgvItemSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = dgvItemSearch.Rows[e.RowIndex];

                    if (row.Cells["الرقم"].Value != DBNull.Value)
                    {
                        int selectedProductID = Convert.ToInt32(row.Cells["الرقم"].Value);

                        // 1. استرجاع البيانات من الداتا لير
                        Product product = productDAL.GetProductByID(selectedProductID);

                        if (product != null)
                        {
                            // 2. ربط القيمة بالـ ComboBox 
                            // تأكد أن cmbProduct يحتوي على هذا الـ ID في قائمته
                            cmbProduct.SelectedValue = product.ProductID;

                            // 3. تحديث الواجهة
                            txtQuantity.Focus();
                            txtQuantity.SelectAll();

                            // 4. إغلاق قائمة البحث (بدل التبديل !)
                            gbSearch.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show("لم يتم العثور على بيانات لهذا الصنف في قاعدة البيانات");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في جلب بيانات الصنف: " + ex.Message);
                }
            }
        }
    }
}
