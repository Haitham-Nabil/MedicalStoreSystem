using System;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.Forms.Customers
{
    public partial class CustomersForm : Form
    {
        private CustomerDAL customerDAL = new CustomerDAL();
        private int selectedCustomerID = 0;

        public CustomersForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void CustomersForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            SetInitialState();
            cmbCustomerType.SelectedIndex = 0; // نقدي
        }

        private void LoadCustomers()
        {
            try
            {
                dgvCustomers.DataSource = customerDAL.GetCustomersDataTable();

                if (dgvCustomers.Columns["الرقم"] != null)
                {
                    dgvCustomers.Columns["الرقم"].Visible = false;
                }

                dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            txtCustomerCode.Focus();
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
            txtCustomerName.Focus();
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
            txtCustomerName.Enabled = true;
            txtMobile.Enabled = true;
            txtPhone.Enabled = true;
            txtAddress.Enabled = true;
            txtEmail.Enabled = true;
            cmbCustomerType.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtInitialBalance.Enabled = true;
            txtNotes.Enabled = true;
            chkIsActive.Enabled = true;
        }

        private void DisableFields()
        {
            txtCustomerCode.Enabled = false;
            txtCustomerName.Enabled = false;
            txtMobile.Enabled = false;
            txtPhone.Enabled = false;
            txtAddress.Enabled = false;
            txtEmail.Enabled = false;
            cmbCustomerType.Enabled = false;
            txtCreditLimit.Enabled = false;
            txtInitialBalance.Enabled = false;
            txtCurrentBalance.Enabled = false;
            txtNotes.Enabled = false;
            chkIsActive.Enabled = false;
        }

        private void ClearFields()
        {
            selectedCustomerID = 0;
            txtCustomerCode.Clear();
            txtCustomerName.Clear();
            txtMobile.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            cmbCustomerType.SelectedIndex = 0;
            txtCreditLimit.Text = "0";
            txtInitialBalance.Text = "0";
            txtCurrentBalance.Text = "0";
            txtNotes.Clear();
            chkIsActive.Checked = true;
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            txtCustomerCode.Text = customerDAL.GenerateCustomerCode();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SetAddState();
            txtCustomerCode.Text = customerDAL.GenerateCustomerCode();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (string.IsNullOrWhiteSpace(txtCustomerCode.Text))
            {
                MessageBox.Show("يرجى إدخال أو توليد كود العميل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerCode.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم العميل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return;
            }

            try
            {
                Customer customer = new Customer
                {
                    CustomerID = selectedCustomerID,
                    CustomerCode = txtCustomerCode.Text.Trim(),
                    CustomerName = txtCustomerName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Mobile = txtMobile.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    CustomerType = cmbCustomerType.Text,
                    CreditLimit = decimal.Parse(txtCreditLimit.Text),
                    InitialBalance = decimal.Parse(txtInitialBalance.Text),
                    CurrentBalance = decimal.Parse(txtCurrentBalance.Text),
                    IsActive = chkIsActive.Checked,
                    Notes = txtNotes.Text.Trim()
                };

                bool success;
                string message;

                if (selectedCustomerID == 0)
                {
                    success = customerDAL.InsertCustomer(customer);
                    message = success ? "تم إضافة العميل بنجاح" : "فشل في إضافة العميل (ربما الكود مكرر)";
                }
                else
                {
                    success = customerDAL.UpdateCustomer(customer);
                    message = success ? "تم تعديل العميل بنجاح" : "فشل في تعديل العميل";
                }

                if (success)
                {
                    MessageBox.Show(message, "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadCustomers();
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
            if (selectedCustomerID == 0)
            {
                MessageBox.Show("يرجى اختيار عميل للتعديل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetEditState();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomerID == 0)
            {
                MessageBox.Show("يرجى اختيار عميل للحذف", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("هل أنت متأكد من حذف هذا العميل؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = customerDAL.DeleteCustomer(selectedCustomerID);

                    if (success)
                    {
                        MessageBox.Show("تم حذف العميل بنجاح", "نجاح",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadCustomers();
                        SetInitialState();
                    }
                    else
                    {
                        MessageBox.Show("فشل في حذف العميل", "خطأ",
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

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow row = dgvCustomers.SelectedRows[0];

                    selectedCustomerID = Convert.ToInt32(row.Cells["الرقم"].Value);

                    Customer customer = customerDAL.GetCustomerByID(selectedCustomerID);

                    if (customer != null)
                    {
                        txtCustomerCode.Text = customer.CustomerCode;
                        txtCustomerName.Text = customer.CustomerName;
                        txtMobile.Text = customer.Mobile;
                        txtPhone.Text = customer.Phone;
                        txtAddress.Text = customer.Address;
                        txtEmail.Text = customer.Email;
                        cmbCustomerType.Text = customer.CustomerType;
                        txtCreditLimit.Text = customer.CreditLimit.ToString();
                        txtInitialBalance.Text = customer.InitialBalance.ToString();
                        txtCurrentBalance.Text = customer.CurrentBalance.ToString();
                        txtNotes.Text = customer.Notes;
                        chkIsActive.Checked = customer.IsActive;

                        SetSelectedState();
                    }
                }
                catch (Exception ex)
                {
                    // تجاهل
                }
            }
        }

        private void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEdit_Click(sender, e);
            }
        }

        private void cmbCustomerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // إذا كان نقدي، حد الائتمان = 0
            if (cmbCustomerType.Text == "نقدي")
            {
                txtCreditLimit.Text = "0";
                txtCreditLimit.Enabled = false;
            }
            else
            {
                txtCreditLimit.Enabled = true;
            }
        }
    }
}
