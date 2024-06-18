using System;
using System.IO;

namespace MinecraftGuardian
{
    public class AppTools
    {

        public static void Log(string action, bool success)
        {
            string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }


            string logFilePath = Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd}.txt");
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {action} {(success ? "成功" : "失敗")}";

            try
            {
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"無法寫入日誌: {ex.Message}");
            }
        }
    }
}
