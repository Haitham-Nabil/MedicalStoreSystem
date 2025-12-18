using System;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Models;
using MedicalStoreSystem.Helpers;

namespace MedicalStoreSystem.Forms.Settings
{
    public partial class UsersForm : Form
    {
        private UserDAL userDAL = new UserDAL();
        private int selectedUserID = 0;

        public UsersForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void UsersForm_Load(object sender, EventArgs e)
        {
            // التحقق من الصلاحية
            if (!CurrentSession.IsAdmin)
            {
                MessageBox.Show("ليس لديك صلاحية الوصول لهذه الشاشة", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            LoadUsers();
            SetInitialState();
        }

        private void LoadUsers()
        {
            try
            {
                dgvUsers.DataSource = userDAL.GetUsersDataTable();

                if (dgvUsers.Columns["الرقم"] != null)
                {
                    dgvUsers.Columns["الرقم"].Visible = false;
                }

                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

            ClearFields();
            EnableFields();
            txtUsername.Focus();
        }

        private void SetEditState()
        {
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = true;

            EnableFields();
            txtUsername.Enabled = false; // لا يمكن تعديل اسم المستخدم
            txtFullName.Focus();
        }

        private void SetSelectedState()
        {
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
            btnCancel.Enabled = true;

            DisableFields();
        }

        private void EnableFields()
        {
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            txtFullName.Enabled = true;
            cmbRole.Enabled = true;
            txtPhone.Enabled = true;
            txtEmail.Enabled = true;
            chkIsActive.Enabled = true;
        }

        private void DisableFields()
        {
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            txtFullName.Enabled = false;
            cmbRole.Enabled = false;
            txtPhone.Enabled = false;
            txtEmail.Enabled = false;
            chkIsActive.Enabled = false;
        }

        private void ClearFields()
        {
            selectedUserID = 0;
            txtUsername.Clear();
            txtPassword.Clear();
            txtFullName.Clear();
            cmbRole.SelectedIndex = -1;
            txtPhone.Clear();
            txtEmail.Clear();
            chkIsActive.Checked = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SetAddState();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("يرجى إدخال اسم المستخدم", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("يرجى إدخال كلمة المرور", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("يرجى إدخال الاسم الكامل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            if (cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("يرجى اختيار الصلاحية", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return;
            }

            try
            {
                User user = new User
                {
                    UserID = selectedUserID,
                    Username = txtUsername.Text.Trim(),
                    Password = txtPassword.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Role = cmbRole.Text,
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                bool success;
                string message;

                if (selectedUserID == 0)
                {
                    success = userDAL.InsertUser(user);
                    message = success ? "تم إضافة المستخدم بنجاح" : "فشل في إضافة المستخدم (ربما اسم المستخدم مكرر)";
                }
                else
                {
                    success = userDAL.UpdateUser(user);
                    message = success ? "تم تعديل المستخدم بنجاح" : "فشل في تعديل المستخدم";
                }

                if (success)
                {
                    MessageBox.Show(message, "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadUsers();
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
            if (selectedUserID == 0)
            {
                MessageBox.Show("يرجى اختيار مستخدم للتعديل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // منع تعديل المستخدم الحالي لنفسه
            if (selectedUserID == CurrentSession.LoggedInUser.UserID)
            {
                MessageBox.Show("لا يمكنك تعديل بياناتك الخاصة من هنا", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetEditState();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserID == 0)
            {
                MessageBox.Show("يرجى اختيار مستخدم للحذف", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // منع حذف المستخدم الحالي
            if (selectedUserID == CurrentSession.LoggedInUser.UserID)
            {
                MessageBox.Show("لا يمكنك حذف حسابك الخاص!", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("هل أنت متأكد من حذف هذا المستخدم؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = userDAL.DeleteUser(selectedUserID);

                    if (success)
                    {
                        MessageBox.Show("تم حذف المستخدم بنجاح", "نجاح",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadUsers();
                        SetInitialState();
                    }
                    else
                    {
                        MessageBox.Show("فشل في حذف المستخدم", "خطأ",
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

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow row = dgvUsers.SelectedRows[0];

                    selectedUserID = Convert.ToInt32(row.Cells["الرقم"].Value);

                    var users = userDAL.GetAllUsers();
                    User user = users.Find(u => u.UserID == selectedUserID);

                    if (user != null)
                    {
                        txtUsername.Text = user.Username;
                        txtPassword.Text = user.Password;
                        txtFullName.Text = user.FullName;
                        cmbRole.Text = user.Role;
                        txtPhone.Text = user.Phone;
                        txtEmail.Text = user.Email;
                        chkIsActive.Checked = user.IsActive;

                        SetSelectedState();
                    }
                }
                catch { }
            }
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEdit_Click(sender, e);
            }
        }
    }
}
