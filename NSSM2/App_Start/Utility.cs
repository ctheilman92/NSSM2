using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NSSM2.App_Start
{
    public class Utility
    {
        public static void LogError(Exception ex)
        {
            var directory = GetErrorLogDirectory();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string errToLog = $"Date: {DateTime.Now.ToString()}{Environment.NewLine}{ex.ToString()}{Environment.NewLine}";
            File.AppendAllText(GetErrorLogFilepathName(), errToLog);
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                string binDirectory = Directory.GetParent(path).FullName;
                string webAppDirectory = Directory.GetParent(binDirectory).FullName;
                return Directory.GetParent(webAppDirectory).FullName;
            }
        }

        public static string GetErrorLog()
        {
            string errorLog = GetErrorLog(DateTime.Now);
            return errorLog;
        }

        public static string GetErrorLog(DateTime date)
        {
            string strErrorLog = string.Empty;
            string fileName = GetErrorLogFilepathName();

            if (File.Exists(fileName))
            {
                using (TextReader tr = new StreamReader(fileName))
                {
                    strErrorLog = tr.ReadToEnd();
                }
                if (string.IsNullOrEmpty(strErrorLog))
                {
                    strErrorLog = "No Error Log Found";

                }
            }
            else
            {
                strErrorLog = "No Log file exists";
            }

            return strErrorLog;
        }

        public static void ClearErrorLog()
        {
            ClearErrorLog(DateTime.Now);
        }

        public static void ClearErrorLog(DateTime date)
        {
            string fileName = GetErrorLogFilepathName();
            if (File.Exists(fileName))
            {
                using (TextWriter tw = new StreamWriter(fileName, false))
                {
                    tw.Write("");
                    tw.Flush();
                    tw.Close();
                }
            }
        }

        public static string GetErrorLogDirectory() => $"{Path.Combine(AssemblyDirectory, "Logs")}";

        public static string GetErrorLogFileName() => $"{DateTime.Now.ToString("MM-dd-yyyy")}.log";

        public static string GetErrorLogFilepathName() => $"{Path.Combine(AssemblyDirectory, "Logs", GetErrorLogFileName())}";

        public static string GetGUIDFile(string extension) => $"{Guid.NewGuid()}.{extension.Remove(extension.LastIndexOf("."), 1)}";
    }
}