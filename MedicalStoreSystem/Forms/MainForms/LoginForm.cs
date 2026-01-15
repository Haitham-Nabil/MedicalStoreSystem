using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Models;
using MedicalStoreSystem.Helpers;
using Krypton.Toolkit;

namespace MedicalStoreSystem.Forms.MainForms
{
    public partial class LoginForm : Form
    {
        private UserDAL userDAL = new UserDAL();

        public LoginForm()
        {
            InitializeComponent();

            // إعدادات النافذة
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
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


            // محاولة تسجيل الدخول
            User user = userDAL.Login(txtUsername.Text, txtPassword.Text);

            // في LoginForm.cs في دالة btnLogin_Click

            if (user != null)
            {
                CurrentSession.LoggedInUser = user;

                // تسجيل في Log
                Helpers.Logger.Log($"User logged in: {user.Username} - {user.FullName}");

                MessageBox.Show($"مرحباً {user.FullName}!", "تم تسجيل الدخول",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                Helpers.Logger.Log($"Failed login attempt: {txtUsername.Text}", "WARNING");

                MessageBox.Show("اسم المستخدم أو كلمة المرور غير صحيحة", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            // الكود الأصلي معلق مؤقتًا
            /*
            if (user != null)
            {
                // حفظ بيانات المستخدم في الجلسة
                CurrentSession.LoggedInUser = user;

                MessageBox.Show($"مرحباً {user.FullName}!", "تم تسجيل الدخول",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // فتح الشاشة الرئيسية (سنعملها في الخطوة القادمة)
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("اسم المستخدم أو كلمة المرور غير صحيحة", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtUsername.Focus();
            }*/
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }

        // عند الضغط على Enter في Username
        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
                e.Handled = true;
            }
        }

        // عند الضغط على Enter في Password
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
                e.Handled = true;
            }
        }
    }
}
