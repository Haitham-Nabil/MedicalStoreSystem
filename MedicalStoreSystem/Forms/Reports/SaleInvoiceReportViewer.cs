using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using MedicalStoreSystem.DAL;

namespace MedicalStoreSystem.Forms.Reports
{
    public partial class SaleInvoiceReportViewer : Form
    {
        private int saleID;

        public SaleInvoiceReportViewer(int saleID)
        {
            InitializeComponent();
            this.saleID = saleID;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void SaleInvoiceReportViewer_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. جلب البيانات
                DataSet ds = SaleReportDAL.GetSaleInvoiceDataForRDLC(saleID);

                // 2. إعداد ReportViewer
                reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\RDLC\SaleInvoiceReport.rdlc";

                // 3. مسح مصادر البيانات القديمة
                reportViewer1.LocalReport.DataSources.Clear();

                // 4. إضافة مصادر البيانات
                ReportDataSource rdsHeader = new ReportDataSource("dtSaleHeader", ds.Tables["dtSaleHeader"]);
                ReportDataSource rdsDetails = new ReportDataSource("dtSaleDetails", ds.Tables["dtSaleDetails"]);

                reportViewer1.LocalReport.DataSources.Add(rdsHeader);
                reportViewer1.LocalReport.DataSources.Add(rdsDetails);
                MessageBox.Show(ds.Tables["dtSaleDetails"].Rows.Count.ToString());
                // 5. تحديث وعرض التقرير
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في عرض التقرير:\n{ex.Message}\n\nتأكد من:\n" +
                    "1. وجود ملف SaleInvoiceReport.rdlc في مجلد Reports/RDLC\n" +
                    "2. تطابق أسماء DataSets في RDLC مع الكود",
                    "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
    }
}
