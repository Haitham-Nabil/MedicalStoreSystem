using System;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.Forms.Products
{
    public partial class ProductsForm : Form
    {
        private ProductDAL productDAL = new ProductDAL();
        private CategoryDAL categoryDAL = new CategoryDAL();
        private SupplierDAL supplierDAL = new SupplierDAL();
        private int selectedProductID = 0;

        public ProductsForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void ProductsForm_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadSuppliers();
            LoadProducts();
            SetInitialState();
        }

        private void LoadCategories()
        {
            try
            {
                cmbCategory.DataSource = null;
                var categories = categoryDAL.GetActiveCategories();
                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
                cmbCategory.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل التصنيفات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                dgvProducts.DataSource = productDAL.GetProductsDataTable();

                if (dgvProducts.Columns["الرقم"] != null)
                {
                    dgvProducts.Columns["الرقم"].Visible = false;
                }

                dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // تلوين المنتجات القليلة
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    if (row.Cells["الكمية"].Value != null &&
                        row.Cells["الحد الأدنى"].Value != null)
                    {
                        int currentStock = Convert.ToInt32(row.Cells["الكمية"].Value);
                        int minStock = Convert.ToInt32(row.Cells["الحد الأدنى"].Value);

                        if (currentStock <= minStock)
                        {
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل البيانات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = false;
            btnGenerateCode.Enabled = false;

            ClearFields();
            DisableFields();
        }

        private void SetAddState()
        {
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = true;
            btnGenerateCode.Enabled = true;

            ClearFields();
            EnableFields();
            txtProductCode.Focus();
        }

        private void SetEditState()
        {
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = true;
            btnGenerateCode.Enabled = false;

            EnableFields();
            txtProductName.Focus();
        }

        private void SetSelectedState()
        {
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
            btnCancel.Enabled = true;
            btnGenerateCode.Enabled = false;

            DisableFields();
        }

        private void EnableFields()
        {
            txtBarcode.Enabled = true;
            txtProductName.Enabled = true;
            cmbCategory.Enabled = true;
            cmbUnit.Enabled = true;
            txtCostPrice.Enabled = true;
            txtSalePrice.Enabled = true;
            txtMinStock.Enabled = true;
            cmbSupplier.Enabled = true;
            chkHasExpiry.Enabled = true;
            txtNotes.Enabled = true;
            chkIsActive.Enabled = true;
        }

        private void DisableFields()
        {
            txtProductCode.Enabled = false;
            txtBarcode.Enabled = false;
            txtProductName.Enabled = false;
            cmbCategory.Enabled = false;
            cmbUnit.Enabled = false;
            txtCostPrice.Enabled = false;
            txtSalePrice.Enabled = false;
            txtMinStock.Enabled = false;
            txtCurrentStock.Enabled = false;
            cmbSupplier.Enabled = false;
            chkHasExpiry.Enabled = false;
            txtNotes.Enabled = false;
            chkIsActive.Enabled = false;
        }

        private void ClearFields()
        {
            selectedProductID = 0;
            txtProductCode.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            cmbCategory.SelectedIndex = -1;
            cmbUnit.SelectedIndex = -1;
            txtCostPrice.Text = "0";
            txtSalePrice.Text = "0";
            txtMinStock.Text = "10";
            txtCurrentStock.Text = "0";
            cmbSupplier.SelectedIndex = -1;
            chkHasExpiry.Checked = true;
            txtNotes.Clear();
            chkIsActive.Checked = true;
            lblProfitMargin.Text = "0%";
        }

        // حساب نسبة الربح
        private void CalculateProfitMargin()
        {
            try
            {
                decimal costPrice = decimal.Parse(txtCostPrice.Text);
                decimal salePrice = decimal.Parse(txtSalePrice.Text);

                if (costPrice > 0)
                {
                    decimal profit = ((salePrice - costPrice) / costPrice) * 100;
                    lblProfitMargin.Text = $"{profit:F2}%";
                    lblProfitMargin.ForeColor = profit >= 0 ?
                        System.Drawing.Color.Green : System.Drawing.Color.Red;
                }
                else
                {
                    lblProfitMargin.Text = "0%";
                }
            }
            catch
            {
                lblProfitMargin.Text = "0%";
            }
        }

        private void txtCostPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateProfitMargin();
        }

        private void txtSalePrice_TextChanged(object sender, EventArgs e)
        {
            CalculateProfitMargin();
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            txtProductCode.Text = productDAL.GenerateProductCode();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SetAddState();
            txtProductCode.Text = productDAL.GenerateProductCode();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (string.IsNullOrWhiteSpace(txtProductCode.Text))
            {
                MessageBox.Show("يرجى إدخال أو توليد كود المنتج", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductCode.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم المنتج", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductName.Focus();
                return;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("يرجى اختيار التصنيف", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            try
            {
                Product product = new Product
                {
                    ProductID = selectedProductID,
                    ProductCode = txtProductCode.Text.Trim(),
                    Barcode = txtBarcode.Text.Trim(),
                    ProductName = txtProductName.Text.Trim(),
                    CategoryID = Convert.ToInt32(cmbCategory.SelectedValue),
                    UnitName = cmbUnit.Text.Trim(),
                    CostPrice = decimal.Parse(txtCostPrice.Text),
                    SalePrice = decimal.Parse(txtSalePrice.Text),
                    MinStock = int.Parse(txtMinStock.Text),
                    CurrentStock = int.Parse(txtCurrentStock.Text),
                    SupplierID = cmbSupplier.SelectedIndex != -1 ?
                        Convert.ToInt32(cmbSupplier.SelectedValue) : 0,
                    HasExpiry = chkHasExpiry.Checked,
                    IsActive = chkIsActive.Checked,
                    Notes = txtNotes.Text.Trim()
                };

                bool success;
                string message;

                if (selectedProductID == 0)
                {
                    success = productDAL.InsertProduct(product);
                    message = success ? "تم إضافة المنتج بنجاح" : "فشل في إضافة المنتج (ربما الكود مكرر)";
                }
                else
                {
                    success = productDAL.UpdateProduct(product);
                    message = success ? "تم تعديل المنتج بنجاح" : "فشل في تعديل المنتج";
                }

                if (success)
                {
                    MessageBox.Show(message, "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadProducts();
                    SetInitialState();
                }
                else
                {
                    MessageBox.Show(message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedProductID == 0)
            {
                MessageBox.Show("يرجى اختيار منتج للتعديل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetEditState();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductID == 0)
            {
                MessageBox.Show("يرجى اختيار منتج للحذف", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("هل أنت متأكد من حذف هذا المنتج؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = productDAL.DeleteProduct(selectedProductID);

                    if (success)
                    {
                        MessageBox.Show("تم حذف المنتج بنجاح", "نجاح",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadProducts();
                        SetInitialState();
                    }
                    else
                    {
                        MessageBox.Show("فشل في حذف المنتج", "خطأ",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetInitialState();
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow row = dgvProducts.SelectedRows[0];

                    selectedProductID = Convert.ToInt32(row.Cells["الرقم"].Value);

                    Product product = productDAL.GetProductByID(selectedProductID);

                    if (product != null)
                    {
                        txtProductCode.Text = product.ProductCode;
                        txtBarcode.Text = product.Barcode;
                        txtProductName.Text = product.ProductName;
                        cmbCategory.SelectedValue = product.CategoryID;
                        cmbUnit.Text = product.UnitName;
                        txtCostPrice.Text = product.CostPrice.ToString();
                        txtSalePrice.Text = product.SalePrice.ToString();
                        txtMinStock.Text = product.MinStock.ToString();
                        txtCurrentStock.Text = product.CurrentStock.ToString();
                        cmbSupplier.SelectedValue = product.SupplierID > 0 ?
                            (object)product.SupplierID : null;
                        chkHasExpiry.Checked = product.HasExpiry;
                        txtNotes.Text = product.Notes;
                        chkIsActive.Checked = product.IsActive;

                        CalculateProfitMargin();
                        SetSelectedState();
                    }
                }
                catch (Exception ex)
                {
                    // تجاهل
                }
            }
        }

        private void dgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEdit_Click(sender, e);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadProducts();
                return;
            }

            try
            {
                string searchTerm = txtSearch.Text.Trim();
                string query = $@"SELECT 
                               p.ProductID AS 'الرقم',
                               p.ProductCode AS 'كود المنتج',
                               p.Barcode AS 'الباركود',
                               p.ProductName AS 'اسم المنتج',
                               c.CategoryName AS 'التصنيف',
                               p.UnitName AS 'الوحدة',
                               p.CostPrice AS 'سعر الشراء',
                               p.SalePrice AS 'سعر البيع',
                               p.CurrentStock AS 'الكمية',
                               p.MinStock AS 'الحد الأدنى',
                               s.SupplierName AS 'المورد',
                               CASE WHEN p.IsActive = 1 THEN 'نشط' ELSE 'غير نشط' END AS 'الحالة'
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                           WHERE p.ProductName LIKE '%{searchTerm}%' 
                              OR p.ProductCode LIKE '%{searchTerm}%'
                              OR p.Barcode LIKE '%{searchTerm}%'
                           ORDER BY p.ProductName";

                dgvProducts.DataSource = DAL.DatabaseConnection.ExecuteDataTable(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في البحث:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadProducts();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.Handled = true;
            }
        }


    }
}
