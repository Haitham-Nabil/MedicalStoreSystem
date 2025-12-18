using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Forms.MainForms;
using System;
using System.IO;
using System.Windows.Forms;

namespace MedicalStoreSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // معالجة الأخطاء غير المتوقعة
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // فتح شاشة تسجيل الدخول أولاً
            LoginForm loginForm = new LoginForm();

            // فحص النسخ الاحتياطي التلقائي
            CheckAutoBackup();

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // فتح الشاشة الرئيسية
                Application.Run(new MainForm());
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Helpers.Logger.LogError("Application Thread Exception", e.Exception);

            MessageBox.Show(
                "حدث خطأ غير متوقع في البرنامج.\n\nيرجى التواصل مع الدعم الفني.",
                "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Helpers.Logger.LogError("Unhandled Exception", ex);
            }

            MessageBox.Show(
                "حدث خطأ حرج في البرنامج.\n\nسيتم إغلاق البرنامج الآن.",
                "خطأ حرج", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }






        //////// ********************************************************************
        ///
        private static void CheckAutoBackup()
        {
            try
            {
                if (Properties.Settings.Default.AutoBackup)
                {
                    DateTime lastBackup = Properties.Settings.Default.LastBackupDate;
                    int backupDays = Properties.Settings.Default.BackupDays;

                    if ((DateTime.Now - lastBackup).TotalDays >= backupDays)
                    {
                        PerformAutoBackup();
                    }
                }
            }
            catch
            {
                // تجاهل الأخطاء في النسخ الاحتياطي التلقائي
            }
        }

        private static void PerformAutoBackup()
        {
            try
            {
                string backupPath = Properties.Settings.Default.BackupPath;
                if (string.IsNullOrEmpty(backupPath))
                    return;

                if (!Directory.Exists(backupPath))
                    Directory.CreateDirectory(backupPath);

                string backupFileName = $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string backupFullPath = Path.Combine(backupPath, backupFileName);

                string query = $@"
                    BACKUP DATABASE [MedicalStoreDB] 
                    TO DISK = '{backupFullPath}' 
                    WITH FORMAT, INIT, 
                    NAME = 'Auto Backup', 
                    SKIP, NOREWIND, NOUNLOAD";

                DatabaseConnection.ExecuteNonQuery(query, null);

                // تحديث تاريخ آخر نسخة احتياطية
                Properties.Settings.Default.LastBackupDate = DateTime.Now;
                Properties.Settings.Default.Save();

                // حذف النسخ القديمة (الاحتفاظ بآخر 10 نسخ فقط)
                CleanOldBackups(backupPath);
            }
            catch
            {
                // تجاهل الأخطاء
            }
        }

        private static void CleanOldBackups(string backupPath)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(backupPath);
                FileInfo[] files = di.GetFiles("AutoBackup_*.bak");

                if (files.Length > 10)
                {
                    Array.Sort(files, (x, y) => y.CreationTime.CompareTo(x.CreationTime));
                    for (int i = 10; i < files.Length; i++)
                    {
                        files[i].Delete();
                    }
                }
            }
            catch
            {
                // تجاهل الأخطاء
            }
        }




        ///////////////////*********************************************************************
    }
}
