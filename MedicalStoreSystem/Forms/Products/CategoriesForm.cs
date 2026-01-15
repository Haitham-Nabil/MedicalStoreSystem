using System;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Models;
using MedicalStoreSystem.Helpers;
using Krypton.Toolkit;
//KryptonForm
namespace MedicalStoreSystem.Forms.Products
{
    //public partial class CategoriesForm : Form
    public partial class CategoriesForm : KryptonForm
    {
        private CategoryDAL categoryDAL = new CategoryDAL();
        private int selectedCategoryID = 0;

        public CategoriesForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void CategoriesForm_Load(object sender, EventArgs e)
        {
            LoadCategories();
            SetInitialState();
        }

        // تحميل البيانات في DataGridView
        private void LoadCategories()
        {
            try
            {
                dgvCategories.DataSource = categoryDAL.GetCategoriesDataTable();

                // إخفاء عمود الـ ID
                if (dgvCategories.Columns["الرقم"] != null)
                {
                    dgvCategories.Columns["الرقم"].Visible = false;
                }

                // تنسيق الأعمدة
                dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل البيانات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // الحالة الابتدائية للأزرار
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

        // حالة الإضافة
        private void SetAddState()
        {
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = true;

            ClearFields();
            EnableFields();
            txtCategoryName.Focus();
        }

        // حالة التعديل
        private void SetEditState()
        {
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = true;

            EnableFields();
            txtCategoryName.Focus();
        }

        // حالة الاختيار
        private void SetSelectedState()
        {
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
            btnCancel.Enabled = true;

            DisableFields();
        }

        // تفعيل الحقول
        private void EnableFields()
        {
            txtCategoryName.Enabled = true;
            txtDescription.Enabled = true;
            chkIsActive.Enabled = true;
        }

        // تعطيل الحقول
        private void DisableFields()
        {
            txtCategoryName.Enabled = false;
            txtDescription.Enabled = false;
            chkIsActive.Enabled = false;
        }

        // مسح الحقول
        private void ClearFields()
        {
            selectedCategoryID = 0;
            txtCategoryName.Clear();
            txtDescription.Clear();
            chkIsActive.Checked = true;
        }

        // ═══════════════ أحداث الأزرار ═══════════════

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SetAddState();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                //MessageBox.Show("يرجى إدخال اسم التصنيف", "تنبيه",
                 //   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 KryptonMessageBox.Show("يرجى إدخال اسم التصنيف", "تنبيه",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            try
            {
                Category category = new Category
                {
                    CategoryID = selectedCategoryID,
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                bool success;
                string message;

                if (selectedCategoryID == 0)
                {
                    // إضافة جديد
                    success = categoryDAL.InsertCategory(category);
                    message = success ? "تم إضافة التصنيف بنجاح" : "فشل في إضافة التصنيف";
                }
                else
                {
                    // تعديل
                    success = categoryDAL.UpdateCategory(category);
                    message = success ? "تم تعديل التصنيف بنجاح" : "فشل في تعديل التصنيف";
                }

                if (success)
                {
                    MessageBox.Show(message, "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadCategories();
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
            if (selectedCategoryID == 0)
            {
                MessageBox.Show("يرجى اختيار تصنيف للتعديل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetEditState();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCategoryID == 0)
            {
                MessageBox.Show("يرجى اختيار تصنيف للحذف", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // التحقق من وجود منتجات تحت هذا التصنيف
            if (categoryDAL.HasProducts(selectedCategoryID))
            {
                MessageBox.Show("لا يمكن حذف هذا التصنيف لأنه يحتوي على منتجات", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("هل أنت متأكد من حذف هذا التصنيف؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = categoryDAL.DeleteCategory(selectedCategoryID);

                    if (success)
                    {
                        MessageBox.Show("تم حذف التصنيف بنجاح", "نجاح",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadCategories();
                        SetInitialState();
                    }
                    else
                    {
                        MessageBox.Show("فشل في حذف التصنيف", "خطأ",
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

        // عند اختيار صف من DataGridView
        private void dgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow row = dgvCategories.SelectedRows[0];

                    // الحصول على الـ ID من العمود المخفي
                    selectedCategoryID = Convert.ToInt32(row.Cells["الرقم"].Value);

                    // جلب بيانات التصنيف
                    Category category = categoryDAL.GetCategoryByID(selectedCategoryID);

                    if (category != null)
                    {
                        txtCategoryName.Text = category.CategoryName;
                        txtDescription.Text = category.Description;
                        chkIsActive.Checked = category.IsActive;

                        SetSelectedState();
                    }
                }
                catch (Exception ex)
                {
                    // تجاهل الأخطاء البسيطة في الاختيار
                }
            }
        }

        // عند الضغط دبل كليك على صف
        private void dgvCategories_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEdit_Click(sender, e);
            }
        }
    }
}
