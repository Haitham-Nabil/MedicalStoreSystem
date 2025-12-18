using System;
using System.IO;

namespace MedicalStoreSystem.Helpers
{
    public static class Logger
    {
        private static string logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public static void Log(string message, string type = "INFO")
        {
            try
            {
                // إنشاء مجلد Logs إذا لم يكن موجوداً
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                string logFile = Path.Combine(logPath,
                    $"Log_{DateTime.Now:yyyy-MM-dd}.txt");

                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {message}";

                File.AppendAllText(logFile, logEntry + Environment.NewLine);
            }
            catch
            {
                // تجاهل أخطاء الـ Logging
            }
        }

        public static void LogError(string message, Exception ex)
        {
            string fullMessage = $"{message}\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
            Log(fullMessage, "ERROR");
        }
    }
}
