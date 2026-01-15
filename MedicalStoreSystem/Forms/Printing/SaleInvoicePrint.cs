using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using MedicalStoreSystem.Helpers;

namespace MedicalStoreSystem.Forms.Printing
{
    public class SaleInvoicePrint
    {
        private DataTable dtInvoice;
        private DataTable dtItems;
        private PrintDocument printDocument;
        private int currentItemIndex = 0;

        public SaleInvoicePrint(DataTable invoiceData, DataTable itemsData)
        {
            dtInvoice = invoiceData;
            dtItems = itemsData;

            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // إعداد حجم الورق A4
            printDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            printDocument.DefaultPageSettings.Margins = new Margins(30, 30, 30, 30);
        }

        public void Print()
        {
            try
            {
                currentItemIndex = 0;
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
            Font fontHeader = new Font("Arial", 18, FontStyle.Bold);
            Font fontTitle = new Font("Arial", 14, FontStyle.Bold);
            Font fontBold = new Font("Arial", 11, FontStyle.Bold);
            Font fontRegular = new Font("Arial", 10, FontStyle.Regular);
            Font fontSmall = new Font("Arial", 9, FontStyle.Regular);

            Brush blackBrush = Brushes.Black;
            Brush grayBrush = Brushes.Gray;
            Pen blackPen = new Pen(Color.Black, 2);
            Pen grayPen = new Pen(Color.LightGray, 1);

            float yPos = 40;
            float leftMargin = e.MarginBounds.Left;
            float rightMargin = e.MarginBounds.Right;
            float centerX = (leftMargin + rightMargin) / 2;

            StringFormat centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;

            StringFormat rightFormat = new StringFormat();
            rightFormat.Alignment = StringAlignment.Far;

            // 1. رأس الفاتورة
            g.DrawString(PrintHelper.CompanyName, fontHeader, blackBrush, centerX, yPos, centerFormat);
            yPos += 25;

            g.DrawString(PrintHelper.CompanyAddress, fontSmall, grayBrush, centerX, yPos, centerFormat);
            yPos += 18;

            g.DrawString($"هاتف: {PrintHelper.CompanyPhone} | الرقم الضريبي: {PrintHelper.CompanyTaxNumber}",
                fontSmall, grayBrush, centerX, yPos, centerFormat);
            yPos += 25;

            // خط فاصل
            g.DrawLine(blackPen, leftMargin, yPos, rightMargin, yPos);
            yPos += 15;

            // 2. عنوان الفاتورة
            g.DrawString("فاتورة مبيعات", fontTitle, blackBrush, centerX, yPos, centerFormat);
            yPos += 30;

            // 3. معلومات الفاتورة
            if (dtInvoice.Rows.Count > 0)
            {
                DataRow invoice = dtInvoice.Rows[0];

                string invoiceNumber = invoice["SaleNumber"].ToString();
                DateTime invoiceDate = Convert.ToDateTime(invoice["SaleDate"]);
                string customerName = invoice["CustomerName"].ToString();
                string cashierName = invoice["CashierName"].ToString();

                // رقم الفاتورة والتاريخ
                g.DrawString($"رقم الفاتورة: {invoiceNumber}", fontBold, blackBrush, leftMargin, yPos);
                g.DrawString($"التاريخ: {invoiceDate:yyyy/MM/dd}", fontBold, blackBrush, rightMargin, yPos, rightFormat);
                yPos += 22;

                g.DrawString($"العميل: {customerName}", fontRegular, blackBrush, leftMargin, yPos);
                g.DrawString($"الكاشير: {cashierName}", fontRegular, blackBrush, rightMargin, yPos, rightFormat);
                yPos += 30;
            }

            // 4. رأس جدول الأصناف
            g.DrawLine(grayPen, leftMargin, yPos, rightMargin, yPos);
            yPos += 5;

            float col1 = leftMargin + 30;  // م
            float col2 = leftMargin + 100; // الصنف
            float col3 = rightMargin - 250; // الكمية
            float col4 = rightMargin - 150; // السعر
            float col5 = rightMargin - 50;  // الإجمالي

            g.DrawString("م", fontBold, blackBrush, col1, yPos);
            g.DrawString("الصنف", fontBold, blackBrush, col2, yPos);
            g.DrawString("الكمية", fontBold, blackBrush, col3, yPos);
            g.DrawString("السعر", fontBold, blackBrush, col4, yPos);
            g.DrawString("الإجمالي", fontBold, blackBrush, col5, yPos, rightFormat);
            yPos += 22;

            g.DrawLine(grayPen, leftMargin, yPos, rightMargin, yPos);
            yPos += 10;

            // 5. طباعة الأصناف
            int itemsPerPage = 15;
            int itemCount = 0;
            float maxY = e.MarginBounds.Bottom - 150;

            for (int i = currentItemIndex; i < dtItems.Rows.Count; i++)
            {
                if (yPos > maxY)
                {
                    currentItemIndex = i;
                    e.HasMorePages = true;
                    return;
                }

                DataRow item = dtItems.Rows[i];
                string productName = item["ProductName"].ToString();
                decimal quantity = Convert.ToDecimal(item["Quantity"]);
                decimal price = Convert.ToDecimal(item["UnitPrice"]);
                decimal total = Convert.ToDecimal(item["TotalPrice"]);

                g.DrawString((i + 1).ToString(), fontRegular, blackBrush, col1, yPos);
                g.DrawString(productName, fontRegular, blackBrush, col2, yPos);
                g.DrawString(quantity.ToString("N2"), fontRegular, blackBrush, col3, yPos);
                g.DrawString(price.ToString("N2"), fontRegular, blackBrush, col4, yPos);
                g.DrawString(total.ToString("N2"), fontRegular, blackBrush, col5, yPos, rightFormat);

                yPos += 22;
                itemCount++;
            }

            // 6. الإجماليات
            yPos += 10;
            g.DrawLine(grayPen, leftMargin, yPos, rightMargin, yPos);
            yPos += 15;

            if (dtInvoice.Rows.Count > 0)
            {
                DataRow invoice = dtInvoice.Rows[0];
                decimal totalAmount = Convert.ToDecimal(invoice["TotalAmount"]);
                decimal discountAmount = Convert.ToDecimal(invoice["DiscountAmount"]);
                decimal netAmount = Convert.ToDecimal(invoice["NetAmount"]);
                decimal paidAmount = Convert.ToDecimal(invoice["PaidAmount"]);
                decimal remainingAmount = Convert.ToDecimal(invoice["RemainingAmount"]);

                float labelX = rightMargin - 150;
                float valueX = rightMargin;

                g.DrawString("الإجمالي:", fontBold, blackBrush, labelX, yPos);
                g.DrawString($"{totalAmount:N2} جنية", fontBold, blackBrush, valueX, yPos, rightFormat);
                yPos += 22;

                if (discountAmount > 0)
                {
                    g.DrawString("الخصم:", fontRegular, blackBrush, labelX, yPos);
                    g.DrawString($"{discountAmount:N2} جنية", fontRegular, blackBrush, valueX, yPos, rightFormat);
                    yPos += 22;
                }

                g.DrawString("الصافي:", fontBold, blackBrush, labelX, yPos);
                g.DrawString($"{netAmount:N2} جنية", fontBold, blackBrush, valueX, yPos, rightFormat);
                yPos += 22;

                g.DrawString("المدفوع:", fontRegular, blackBrush, labelX, yPos);
                g.DrawString($"{paidAmount:N2} جنية", fontRegular, blackBrush, valueX, yPos, rightFormat);
                yPos += 22;

                if (remainingAmount > 0)
                {
                    g.DrawString("المتبقي:", fontBold, Brushes.Red, labelX, yPos);
                    g.DrawString($"{remainingAmount:N2} جنية", fontBold, Brushes.Red, valueX, yPos, rightFormat);
                    yPos += 22;
                }
            }

            // 7. ذيل الفاتورة
            yPos = e.MarginBounds.Bottom - 80;
            g.DrawLine(grayPen, leftMargin, yPos, rightMargin, yPos);
            yPos += 10;

            g.DrawString("شكراً لتعاملكم معنا", fontRegular, grayBrush, centerX, yPos, centerFormat);
            yPos += 20;
            g.DrawString("هذه الفاتورة تمت طباعتها إلكترونياً ولا تحتاج إلى توقيع",
                fontSmall, grayBrush, centerX, yPos, centerFormat);

            e.HasMorePages = false;
        }
    }
}
