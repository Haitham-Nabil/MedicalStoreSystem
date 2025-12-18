using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Helpers;

namespace MedicalStoreSystem.Forms.Cash
{
    public partial class CashTransactionForm : Form
    {
        public CashTransactionForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void CashTransactionForm_Load(object sender, EventArgs e)
        {
            cmbTransactionType.SelectedIndex = 0;
            LoadTodayTransactions();
            CalculateSummary();
        }

        private void LoadTodayTransactions()
        {
            try
            {
                string query = @"SELECT 
                                   TransactionID AS 'الرقم',
                                   TransactionDate AS 'التاريخ',
                                   TransactionType AS 'النوع',
                                   Amount AS 'المبلغ',
                                   Description AS 'الوصف'
                               FROM CashTransactions
                               WHERE CAST(TransactionDate AS DATE) = CAST(GETDATE() AS DATE)
                               ORDER BY TransactionDate DESC";

                dgvTransactions.DataSource = DatabaseConnection.ExecuteDataTable(query);

                if (dgvTransactions.Columns["الرقم"] != null)
                {
                    dgvTransactions.Columns["الرقم"].Visible = false;
                }

                dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // تلوين الصفوف
                foreach (DataGridViewRow row in dgvTransactions.Rows)
                {
                    decimal amount = Convert.ToDecimal(row.Cells["المبلغ"].Value);
                    if (amount > 0)
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                    else
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في تحميل البيانات:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateSummary()
        {
            try
            {
                // الرصيد الافتتاحي (رصيد نهاية اليوم السابق)
                string queryOpening = @"SELECT ISNULL(SUM(Amount), 0) 
                                      FROM CashTransactions 
                                      WHERE CAST(TransactionDate AS DATE) < CAST(GETDATE() AS DATE)";

                decimal openingBalance = Convert.ToDecimal(DatabaseConnection.ExecuteScalar(queryOpening));
                lblOpeningBalance.Text = openingBalance.ToString("F2") + " جنيه";

                // إجمالي إيداعات اليوم
                string queryDeposits = @"SELECT ISNULL(SUM(Amount), 0) 
                                       FROM CashTransactions 
                                       WHERE CAST(TransactionDate AS DATE) = CAST(GETDATE() AS DATE)
                                       AND Amount > 0";

                decimal totalDeposits = Convert.ToDecimal(DatabaseConnection.ExecuteScalar(queryDeposits));
                lblTotalDeposits.Text = totalDeposits.ToString("F2") + " جنيه";

                // إجمالي مسحوبات اليوم
                string queryWithdrawals = @"SELECT ISNULL(ABS(SUM(Amount)), 0) 
                                          FROM CashTransactions 
                                          WHERE CAST(TransactionDate AS DATE) = CAST(GETDATE() AS DATE)
                                          AND Amount < 0";

                decimal totalWithdrawals = Convert.ToDecimal(DatabaseConnection.ExecuteScalar(queryWithdrawals));
                lblTotalWithdrawals.Text = totalWithdrawals.ToString("F2") + " جنيه";

                // الرصيد الحالي
                decimal currentBalance = openingBalance + totalDeposits - totalWithdrawals;
                lblCurrentBalance.Text = currentBalance.ToString("F2") + " جنيه";

                // تلوين الرصيد
                if (currentBalance >= 0)
                    lblCurrentBalance.BackColor = System.Drawing.Color.LightGreen;
                else
                    lblCurrentBalance.BackColor = System.Drawing.Color.LightCoral;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ في حساب الملخص:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من الإدخال
            if (string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("يرجى إدخال المبلغ", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            decimal amount;
            if (!decimal.TryParse(txtAmount.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("يرجى إدخال مبلغ صحيح", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("يرجى إدخال الوصف", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescription.Focus();
                return;
            }

            try
            {
                string transactionType = cmbTransactionType.Text;

                // إذا كان سحب أو مصروفات، اجعل المبلغ سالب
                if (transactionType == "سحب" || transactionType == "مصروفات")
                    amount = -amount;

                // إدراج العملية
                string query = @"INSERT INTO CashTransactions 
                               (TransactionDate, TransactionType, Amount, Description, 
                               ReferenceType, ReferenceID, UserID, CreatedDate)
                               VALUES
                               (@TransactionDate, @TransactionType, @Amount, @Description,
                               @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                SqlParameter[] parameters = {
                    new SqlParameter("@TransactionDate", DateTime.Now),
                    new SqlParameter("@TransactionType", transactionType),
                    new SqlParameter("@Amount", amount),
                    new SqlParameter("@Description", txtDescription.Text.Trim()),
                    new SqlParameter("@ReferenceType", DBNull.Value),
                    new SqlParameter("@ReferenceID", DBNull.Value),
                    new SqlParameter("@UserID", CurrentSession.LoggedInUser.UserID),
                    new SqlParameter("@CreatedDate", DateTime.Now)
                };

                int result = DatabaseConnection.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBox.Show("تم حفظ العملية بنجاح", "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // مسح الحقول
                    txtAmount.Clear();
                    txtDescription.Clear();
                    cmbTransactionType.SelectedIndex = 0;

                    // تحديث البيانات
                    LoadTodayTransactions();
                    CalculateSummary();

                    txtAmount.Focus();
                }
                else
                {
                    MessageBox.Show("فشل في حفظ العملية", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dgvTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = dgvTransactions.Rows[e.RowIndex];

                    string details = "تفاصيل العملية:\n\n";
                    details += $"النوع: {row.Cells["النوع"].Value}\n";
                    details += $"المبلغ: {row.Cells["المبلغ"].Value} جنيه\n";
                    details += $"التاريخ: {Convert.ToDateTime(row.Cells["التاريخ"].Value):yyyy-MM-dd HH:mm}\n";
                    details += $"الوصف: {row.Cells["الوصف"].Value}";

                    MessageBox.Show(details, "تفاصيل العملية",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { }
            }
        }
    }
}
