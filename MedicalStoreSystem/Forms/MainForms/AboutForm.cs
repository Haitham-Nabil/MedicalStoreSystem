using System;
using System.Windows.Forms;

namespace MedicalStoreSystem.Forms.MainForms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "حول البرنامج";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = System.Drawing.Color.White;

            // Logo/Icon
            PictureBox picLogo = new PictureBox();
            picLogo.Size = new System.Drawing.Size(100, 100);
            picLogo.Location = new System.Drawing.Point(200, 20);
            picLogo.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            picLogo.SizeMode = PictureBoxSizeMode.CenterImage;

            // عنوان البرنامج
            Label lblTitle = new Label();
            lblTitle.Text = "نظام إدارة المستلزمات الطبية";
            lblTitle.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(50, 140);
            lblTitle.Size = new System.Drawing.Size(400, 30);
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);

            // الإصدار
            Label lblVersion = new Label();
            lblVersion.Text = "الإصدار 1.0";
            lblVersion.Font = new System.Drawing.Font("Tahoma", 11F);
            lblVersion.Location = new System.Drawing.Point(50, 180);
            lblVersion.Size = new System.Drawing.Size(400, 25);
            lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblVersion.ForeColor = System.Drawing.Color.Gray;

            // الوصف
            Label lblDescription = new Label();
            lblDescription.Text = "نظام متكامل لإدارة المستلزمات الطبية\n" +
                                 "يشمل المخزون، المبيعات، المشتريات،\n" +
                                 "العملاء، الموردين، والخزنة";
            lblDescription.Font = new System.Drawing.Font("Tahoma", 10F);
            lblDescription.Location = new System.Drawing.Point(50, 220);
            lblDescription.Size = new System.Drawing.Size(400, 80);
            lblDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // حقوق النشر
            Label lblCopyright = new Label();
            lblCopyright.Text = $"© {DateTime.Now.Year} - جميع الحقوق محفوظة";
            lblCopyright.Font = new System.Drawing.Font("Tahoma", 9F);
            lblCopyright.Location = new System.Drawing.Point(50, 310);
            lblCopyright.Size = new System.Drawing.Size(400, 20);
            lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblCopyright.ForeColor = System.Drawing.Color.Gray;

            // زر إغلاق
            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Location = new System.Drawing.Point(200, 330);
            btnClose.Size = new System.Drawing.Size(100, 30);
            btnClose.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            btnClose.ForeColor = System.Drawing.Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(picLogo);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblVersion);
            this.Controls.Add(lblDescription);
            this.Controls.Add(lblCopyright);
            this.Controls.Add(btnClose);
        }
    }
}
