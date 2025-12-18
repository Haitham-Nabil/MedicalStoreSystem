using MedicalStoreSystem.Helpers;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace MedicalStoreSystem.Forms.Settings
{
    public partial class PrintSettingsForm : Form
    {
        public PrintSettingsForm()
        {
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void PrintSettingsForm_Load(object sender, EventArgs e)
        {
            // تحميل الطابعات المتاحة
            LoadPrinters();

            // تحميل الإعدادات الحالية
            LoadSettings();
        }

        private void LoadPrinters()
        {
            cmbPrinters.Items.Clear();
            cmbPrinters.Items.Add("الطابعة الافتراضية");

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cmbPrinters.Items.Add(printer);
            }

            if (cmbPrinters.Items.Count > 0)
                cmbPrinters.SelectedIndex = 0;
        }

        private void LoadSettings()
        {
            chkAutoPrint.Checked = PrintHelper.AutoPrint;
            chkPrintPreview.Checked = PrintHelper.PrintPreview;

            if (!string.IsNullOrEmpty(PrintHelper.DefaultPrinterName))
            {
                int index = cmbPrinters.Items.IndexOf(PrintHelper.DefaultPrinterName);
                if (index >= 0)
                    cmbPrinters.SelectedIndex = index;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                PrintHelper.AutoPrint = chkAutoPrint.Checked;
                PrintHelper.PrintPreview = chkPrintPreview.Checked;

                if (cmbPrinters.SelectedIndex > 0)
                    PrintHelper.DefaultPrinterName = cmbPrinters.SelectedItem.ToString();
                else
                    PrintHelper.DefaultPrinterName = "";

                MessageBox.Show("تم حفظ إعدادات الطباعة بنجاح!", "نجاح",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTestPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument testDoc = new PrintDocument();
                testDoc.PrintPage += TestDoc_PrintPage;

                if (cmbPrinters.SelectedIndex > 0)
                {
                    testDoc.PrinterSettings.PrinterName = cmbPrinters.SelectedItem.ToString();
                }

                PrintPreviewDialog preview = new PrintPreviewDialog();
                preview.Document = testDoc;
                preview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في طباعة التجربة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 14, FontStyle.Bold);
            StringFormat centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;

            float centerX = e.PageBounds.Width / 2;
            float yPos = 100;

            g.DrawString("اختبار الطباعة", font, Brushes.Black, centerX, yPos, centerFormat);
            yPos += 40;
            g.DrawString("Print Test - طباعة تجريبية", font, Brushes.Black, centerX, yPos, centerFormat);
            yPos += 40;
            g.DrawString("نظام إدارة الصيدليات", new Font("Arial", 12), Brushes.Black, centerX, yPos, centerFormat);

            e.HasMorePages = false;
        }
    }
}
