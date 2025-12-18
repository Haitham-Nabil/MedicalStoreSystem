using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace MedicalStoreSystem.Helpers
{
    public class PrintHelper
    {
        // معلومات الشركة (من قاعدة البيانات أو إعدادات التطبيق)
        public static string CompanyName = "الشركة الطبية المتحدة";
        public static string CompanyAddress = "شارع الملك فهد - الرياض";
        public static string CompanyPhone = "0112345678";
        public static string CompanyTaxNumber = "300123456789003";

        // إعدادات الطباعة
        public static bool AutoPrint = false;
        public static string DefaultPrinterName = "";
        public static bool PrintPreview = true;

        /// <summary>
        /// طباعة أو معاينة المستند
        /// </summary>
        public static void PrintDocument(PrintDocument doc)
        {
            try
            {
                if (!string.IsNullOrEmpty(DefaultPrinterName))
                {
                    doc.PrinterSettings.PrinterName = DefaultPrinterName;
                }

                if (PrintPreview)
                {
                    PrintPreviewDialog preview = new PrintPreviewDialog();
                    preview.Document = doc;
                    preview.Width = 900;
                    preview.Height = 700;
                    preview.StartPosition = FormStartPosition.CenterScreen;
                    preview.ShowDialog();
                }
                else
                {
                    doc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الطباعة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
