using System;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.Forms.Suppliers
{
    public partial class SuppliersForm : Form
    {
        private SupplierDAL supplierDAL = new SupplierDAL();
        private int selectedSupplierID = 0;

        public SuppliersForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SuppliersForm_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            SetInitialState();
        }

        private void LoadSuppliers()
        {
            try
            {
                dgvSuppliers.DataSource = supplierDAL.GetSuppliersDataTable();

                if (dgvSuppliers.Columns["الرقم"] != null)
                {
                    dgvSuppliers.Columns["الرقم"].Visible = false;
                }

                dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            txtSupplierCode.Focus();
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
            txtSupplierName.Focus();
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
            txtSupplierName.Enabled = true;
            txtContactPerson.Enabled = true;
            txtMobile.Enabled = true;
            txtPhone.Enabled = true;
            txtAddress.Enabled = true;
            txtEmail.Enabled = true;
            txtTaxNumber.Enabled = true;
            txtInitialBalance.Enabled = true;
            txtNotes.Enabled = true;
            chkIsActive.Enabled = true;
        }

        private void DisableFields()
        {
            txtSupplierCode.Enabled = false;
            txtSupplierName.Enabled = false;
            txtContactPerson.Enabled = false;
            txtMobile.Enabled = false;
            txtPhone.Enabled = false;
            txtAddress.Enabled = false;
            txtEmail.Enabled = false;
            txtTaxNumber.Enabled = false;
            txtInitialBalance.Enabled = false;
            txtCurrentBalance.Enabled = false;
            txtNotes.Enabled = false;
            chkIsActive.Enabled = false;
        }

        private void ClearFields()
        {
            selectedSupplierID = 0;
            txtSupplierCode.Clear();
            txtSupplierName.Clear();
            txtContactPerson.Clear();
            txtMobile.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtTaxNumber.Clear();
            txtInitialBalance.Text = "0";
            txtCurrentBalance.Text = "0";
            txtNotes.Clear();
            chkIsActive.Checked = true;
        }

        // توليد كود تلقائي
        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            txtSupplierCode.Text = supplierDAL.GenerateSupplierCode();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SetAddState();
            txtSupplierCode.Text = supplierDAL.GenerateSupplierCode();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (string.IsNullOrWhiteSpace(txtSupplierCode.Text))
            {
                MessageBox.Show("يرجى إدخال أو توليد كود المورد", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierCode.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم المورد", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierName.Focus();
                return;
            }

            try
            {
                Supplier supplier = new Supplier
                {
                    SupplierID = selectedSupplierID,
                    SupplierCode = txtSupplierCode.Text.Trim(),
                    SupplierName = txtSupplierName.Text.Trim(),
                    ContactPerson = txtContactPerson.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Mobile = txtMobile.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    TaxNumber = txtTaxNumber.Text.Trim(),
                    InitialBalance = decimal.Parse(txtInitialBalance.Text),
                    CurrentBalance = decimal.Parse(txtCurrentBalance.Text),
                    IsActive = chkIsActive.Checked,
                    Notes = txtNotes.Text.Trim()
                };

                bool success;
                string message;

                if (selectedSupplierID == 0)
                {
                    success = supplierDAL.InsertSupplier(supplier);
                    message = success ? "تم إضافة المورد بنجاح" : "فشل في إضافة المورد (ربما الكود مكرر)";
                }
                else
                {
                    success = supplierDAL.UpdateSupplier(supplier);
                    message = success ? "تم تعديل المورد بنجاح" : "فشل في تعديل المورد";
                }

                if (success)
                {
                    MessageBox.Show(message, "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadSuppliers();
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
            if (selectedSupplierID == 0)
            {
                MessageBox.Show("يرجى اختيار مورد للتعديل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetEditState();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedSupplierID == 0)
            {
                MessageBox.Show("يرجى اختيار مورد للحذف", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("هل أنت متأكد من حذف هذا المورد؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = supplierDAL.DeleteSupplier(selectedSupplierID);

                    if (success)
                    {
                        MessageBox.Show("تم حذف المورد بنجاح", "نجاح",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadSuppliers();
                        SetInitialState();
                    }
                    else
                    {
                        MessageBox.Show("فشل في حذف المورد", "خطأ",
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

        private void dgvSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow row = dgvSuppliers.SelectedRows[0];

                    selectedSupplierID = Convert.ToInt32(row.Cells["الرقم"].Value);

                    Supplier supplier = supplierDAL.GetSupplierByID(selectedSupplierID);

                    if (supplier != null)
                    {
                        txtSupplierCode.Text = supplier.SupplierCode;
                        txtSupplierName.Text = supplier.SupplierName;
                        txtContactPerson.Text = supplier.ContactPerson;
                        txtMobile.Text = supplier.Mobile;
                        txtPhone.Text = supplier.Phone;
                        txtAddress.Text = supplier.Address;
                        txtEmail.Text = supplier.Email;
                        txtTaxNumber.Text = supplier.TaxNumber;
                        txtInitialBalance.Text = supplier.InitialBalance.ToString();
                        txtCurrentBalance.Text = supplier.CurrentBalance.ToString();
                        txtNotes.Text = supplier.Notes;
                        chkIsActive.Checked = supplier.IsActive;

                        SetSelectedState();
                    }
                }
                catch (Exception ex)
                {
                    // تجاهل
                }
            }
        }

        private void dgvSuppliers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEdit_Click(sender, e);
            }
        }
    }
}
