using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using MedicalStoreSystem.Helpers;

namespace MedicalStoreSystem.Forms.Printing
{
    public class ThermalSaleInvoice
    {
        private DataTable dtInvoice;
        private DataTable dtItems;
        private PrintDocument printDocument;

        public ThermalSaleInvoice(DataTable invoiceData, DataTable itemsData)
        {
            dtInvoice = invoiceData;
            dtItems = itemsData;

            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // إعداد حجم الطباعة الحرارية 80mm
            PaperSize paperSize = new PaperSize("Thermal80mm", 315, 0); // العرض 80mm
            printDocument.DefaultPageSettings.PaperSize = paperSize;
            printDocument.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);
        }

        public void Print()
        {
            try
            {
                PrintHelper.PrintDocument(printDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الطباعة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fontHeader = new Font("Arial", 12, FontStyle.Bold);
            Font fontBold = new Font("Arial", 9, FontStyle.Bold);
            Font fontRegular = new Font("Arial", 8, FontStyle.Regular);
            Font fontSmall = new Font("Arial", 7, FontStyle.Regular);

            Brush blackBrush = Brushes.Black;
            float yPos = 10;
            float leftMargin = 10;
            float centerX = 155; // مركز العرض (315 / 2)

            StringFormat centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;

            // 1. رأس الفاتورة
            g.DrawString(PrintHelper.CompanyName, fontHeader, blackBrush, centerX, yPos, centerFormat);
            yPos += 18;

            g.DrawString(PrintHelper.CompanyAddress, fontSmall, blackBrush, centerX, yPos, centerFormat);
            yPos += 12;

            g.DrawString($"هاتف: {PrintHelper.CompanyPhone}", fontSmall, blackBrush, centerX, yPos, centerFormat);
            yPos += 12;

            g.DrawString($"ض.ق: {PrintHelper.CompanyTaxNumber}", fontSmall, blackBrush, centerX, yPos, centerFormat);
            yPos += 15;

            // خط فاصل
            g.DrawLine(Pens.Black, leftMargin, yPos, 305, yPos);
            yPos += 8;

            // 2. عنوان الفاتورة
            g.DrawString("فاتورة مبيعات", fontBold, blackBrush, centerX, yPos, centerFormat);
            yPos += 15;

            // 3. معلومات الفاتورة
            if (dtInvoice.Rows.Count > 0)
            {
                DataRow invoice = dtInvoice.Rows[0];
                string invoiceNumber = invoice["SaleNumber"].ToString();
                DateTime invoiceDate = Convert.ToDateTime(invoice["SaleDate"]);
                string customerName = invoice["CustomerName"].ToString();
                string cashierName = invoice["CashierName"].ToString();

                g.DrawString($"رقم الفاتورة: {invoiceNumber}", fontRegular, blackBrush, leftMargin, yPos);
                yPos += 12;

                g.DrawString($"التاريخ: {invoiceDate:yyyy/MM/dd HH:mm}", fontRegular, blackBrush, leftMargin, yPos);
                yPos += 12;

                g.DrawString($"العميل: {customerName}", fontRegular, blackBrush, leftMargin, yPos);
                yPos += 12;

                g.DrawString($"الكاشير: {cashierName}", fontRegular, blackBrush, leftMargin, yPos);
                yPos += 15;
            }

            // خط فاصل
            g.DrawLine(Pens.Black, leftMargin, yPos, 305, yPos);
            yPos += 8;

            // 4. رأس جدول الأصناف
            g.DrawString("الصنف", fontBold, blackBrush, leftMargin, yPos);
            g.DrawString("الكمية", fontBold, blackBrush, 170, yPos);
            g.DrawString("السعر", fontBold, blackBrush, 220, yPos);
            g.DrawString("الإجمالي", fontBold, blackBrush, 270, yPos);
            yPos += 12;

            g.DrawLine(Pens.Gray, leftMargin, yPos, 305, yPos);
            yPos += 5;

            // 5. طباعة الأصناف
            foreach (DataRow item in dtItems.Rows)
            {
                string productName = item["ProductName"].ToString();
                decimal quantity = Convert.ToDecimal(item["Quantity"]);
                decimal price = Convert.ToDecimal(item["UnitPrice"]);
                decimal total = Convert.ToDecimal(item["TotalPrice"]);

                // اختصار اسم الصنف إذا كان طويلاً
                if (productName.Length > 20)
                    productName = productName.Substring(0, 17) + "...";

                g.DrawString(productName, fontSmall, blackBrush, leftMargin, yPos);
                g.DrawString(quantity.ToString("N1"), fontSmall, blackBrush, 170, yPos);
                g.DrawString(price.ToString("N2"), fontSmall, blackBrush, 220, yPos);
                g.DrawString(total.ToString("N2"), fontSmall, blackBrush, 270, yPos);
                yPos += 12;
            }

            // 6. الإجماليات
            yPos += 5;
            g.DrawLine(Pens.Black, leftMargin, yPos, 305, yPos);
            yPos += 8;

            if (dtInvoice.Rows.Count > 0)
            {
                DataRow invoice = dtInvoice.Rows[0];
                decimal totalAmount = Convert.ToDecimal(invoice["TotalAmount"]);
                decimal discountAmount = Convert.ToDecimal(invoice["DiscountAmount"]);
                decimal netAmount = Convert.ToDecimal(invoice["NetAmount"]);
                decimal paidAmount = Convert.ToDecimal(invoice["PaidAmount"]);
                decimal remainingAmount = Convert.ToDecimal(invoice["RemainingAmount"]);

                g.DrawString("الإجمالي:", fontBold, blackBrush, leftMargin, yPos);
                g.DrawString($"{totalAmount:N2} ريال", fontBold, blackBrush, 220, yPos);
                yPos += 12;

                if (discountAmount > 0)
                {
                    g.DrawString("الخصم:", fontRegular, blackBrush, leftMargin, yPos);
                    g.DrawString($"{discountAmount:N2} ريال", fontRegular, blackBrush, 220, yPos);
                    yPos += 12;
                }

                g.DrawString("الصافي:", fontBold, blackBrush, leftMargin, yPos);
                g.DrawString($"{netAmount:N2} ريال", fontBold, blackBrush, 220, yPos);
                yPos += 12;

                g.DrawString("المدفوع:", fontRegular, blackBrush, leftMargin, yPos);
                g.DrawString($"{paidAmount:N2} ريال", fontRegular, blackBrush, 220, yPos);
                yPos += 12;

                if (remainingAmount > 0)
                {
                    g.DrawString("المتبقي:", fontBold, Brushes.Red, leftMargin, yPos);
                    g.DrawString($"{remainingAmount:N2} ريال", fontBold, Brushes.Red, 220, yPos);
                    yPos += 15;
                }
            }

            // 7. ذيل الفاتورة
            yPos += 10;
            g.DrawLine(Pens.Gray, leftMargin, yPos, 305, yPos);
            yPos += 8;

            g.DrawString("شكراً لزيارتكم", fontRegular, blackBrush, centerX, yPos, centerFormat);
            yPos += 12;
            g.DrawString("برنامج إدارة الصيدليات", fontSmall, blackBrush, centerX, yPos, centerFormat);
            yPos += 12;

            e.HasMorePages = false;
        }
    }
}
