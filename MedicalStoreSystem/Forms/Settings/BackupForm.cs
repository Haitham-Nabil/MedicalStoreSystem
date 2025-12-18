using System;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MedicalStoreSystem.Forms.Settings
{
    public partial class BackupForm : Form
    {
        public BackupForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeControls();
        }

        private void InitializeControls()
        {
            // GroupBox
            GroupBox groupBox = new GroupBox();
            groupBox.Text = "النسخ الاحتياطي";
            groupBox.Location = new System.Drawing.Point(20, 20);
            groupBox.Size = new System.Drawing.Size(540, 250);

            // Label
            Label lblPath = new Label();
            lblPath.Text = "مسار الحفظ:";
            lblPath.Location = new System.Drawing.Point(20, 40);
            lblPath.AutoSize = true;

            // TextBox
            TextBox txtPath = new TextBox();
            txtPath.Name = "txtPath";
            txtPath.Location = new System.Drawing.Point(20, 70);
            txtPath.Size = new System.Drawing.Size(400, 25);
            txtPath.Text = @"C:\MedicalStoreBackup";

            // Button Browse
            Button btnBrowse = new Button();
            btnBrowse.Text = "استعراض...";
            btnBrowse.Location = new System.Drawing.Point(430, 68);
            btnBrowse.Size = new System.Drawing.Size(80, 27);
            btnBrowse.Click += (s, e) => {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dialog.SelectedPath;
                }
            };

            // Label Info
            Label lblInfo = new Label();
            lblInfo.Text = "سيتم إنشاء ملف بإسم: MedicalStoreDB_backup_yyyy-MM-dd_HHmmss.bak";
            lblInfo.Location = new System.Drawing.Point(20, 110);
            lblInfo.Size = new System.Drawing.Size(500, 40);
            lblInfo.ForeColor = System.Drawing.Color.Gray;

            // Button Backup
            Button btnBackup = new Button();
            btnBackup.Text = "بدء النسخ الاحتياطي";
            btnBackup.Location = new System.Drawing.Point(20, 160);
            btnBackup.Size = new System.Drawing.Size(200, 40);
            btnBackup.BackColor = System.Drawing.Color.Green;
            btnBackup.ForeColor = System.Drawing.Color.White;
            btnBackup.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            btnBackup.Click += (s, e) => CreateBackup(txtPath.Text);

            // ProgressBar
            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "progressBar";
            progressBar.Location = new System.Drawing.Point(20, 210);
            progressBar.Size = new System.Drawing.Size(490, 25);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = false;

            groupBox.Controls.Add(lblPath);
            groupBox.Controls.Add(txtPath);
            groupBox.Controls.Add(btnBrowse);
            groupBox.Controls.Add(lblInfo);
            groupBox.Controls.Add(btnBackup);
            groupBox.Controls.Add(progressBar);

            this.Controls.Add(groupBox);
        }

        private void CreateBackup(string backupPath)
        {
            try
            {
                // التحقق من المسار
                if (string.IsNullOrWhiteSpace(backupPath))
                {
                    MessageBox.Show("يرجى تحديد مسار الحفظ", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // إنشاء المجلد إذا لم يكن موجوداً
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                // اسم الملف
                string fileName = $"MedicalStoreDB_backup_{DateTime.Now:yyyy-MM-dd_HHmmss}.bak";
                string fullPath = Path.Combine(backupPath, fileName);

                // عرض ProgressBar
                ProgressBar progressBar = this.Controls.Find("progressBar", true)[0] as ProgressBar;
                if (progressBar != null)
                    progressBar.Visible = true;

                // الاتصال بقاعدة البيانات
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MedicalStoreDB"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string backupQuery = $@"BACKUP DATABASE MedicalStoreDB 
                                          TO DISK = '{fullPath}' 
                                          WITH FORMAT, INIT, 
                                          NAME = 'MedicalStoreDB Full Backup', 
                                          SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                    using (SqlCommand command = new SqlCommand(backupQuery, connection))
                    {
                        command.CommandTimeout = 300; // 5 دقائق
                        command.ExecuteNonQuery();
                    }
                }

                // إخفاء ProgressBar
                if (progressBar != null)
                    progressBar.Visible = false;

                MessageBox.Show($"تم النسخ الاحتياطي بنجاح!\n\nالملف: {fileName}\nالمسار: {backupPath}",
                    "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // إخفاء ProgressBar
                ProgressBar progressBar = this.Controls.Find("progressBar", true)[0] as ProgressBar;
                if (progressBar != null)
                    progressBar.Visible = false;

                MessageBox.Show("حدث خطأ في النسخ الاحتياطي:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
