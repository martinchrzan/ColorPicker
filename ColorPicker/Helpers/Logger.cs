using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker.Helpers
{
    public static class Logger
    {
        public static string ApplicationLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ColorPicker");

        static Logger()
        {
            if (!Directory.Exists(ApplicationLogPath))
            {
                Directory.CreateDirectory(ApplicationLogPath);
            }

            var logFilePath = Path.Combine(ApplicationLogPath, "Log_" + DateTime.Now.ToString(@"yyyy-MM-dd") + ".txt");

            Trace.Listeners.Add(new TextWriterTraceListener(logFilePath));

            Trace.AutoFlush = true;
        }

        public static void LogError(string message)
        {
            Log(message, "ERROR");
        }

        public static void LogError(string message, Exception ex)
        {
            Log(message + Environment.NewLine +
                ex.Message + Environment.NewLine +
                "Inner exception: " + Environment.NewLine +
                ex.InnerException?.Message + Environment.NewLine +
                "Stack trace: " + Environment.NewLine +
                ex.StackTrace,
                "ERROR");
        }

        public static void LogWarning(string message)
        {
            Log(message, "WARNING");
        }

        public static void LogInfo(string message)
        {
            Log(message, "INFO");
        }

        private static void Log(string message, string type)
        {
            Trace.WriteLine(type + ": " + DateTime.Now.TimeOfDay);
            Trace.Indent();
            Trace.WriteLine(GetCallerInfo());
            Trace.WriteLine(message);
            Trace.Unindent();
        }

        private static string GetCallerInfo()
        {
            StackTrace stackTrace = new StackTrace();

            var methodName = stackTrace.GetFrame(3)?.GetMethod();
            var className = methodName?.DeclaringType.Name;
            return "[Method]: " + methodName.Name + " [Class]: " + className;
        }
    }
}
