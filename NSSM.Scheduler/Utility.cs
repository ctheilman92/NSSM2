using NSSM.Core.Models;
using NSSM2.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSSM.Scheduler
{
    public static class Utility
    {
        public static string GetAssemblyPath()
        {
            var assemblyUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var assemblyPath = Uri.UnescapeDataString(assemblyUri.LocalPath);
            return Path.GetDirectoryName(assemblyPath);
        }

        public static void LogException(Exception e)
        {
            var logPath = $"Log/{DateTime.Now.ToString("MM-dd-yyyy")}.log";
            var logfullPath = Path.Combine(GetAssemblyPath(), logPath);
            var directory = Path.GetDirectoryName(logfullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.AppendAllText(logfullPath, e.ToString() + Environment.NewLine);
        }

        public static int GetNetsparkerProcessCount()
        {
            throw new NotImplementedException();
        }

        public static Node GetNodeInstance()
        {
            using (var db = GetNSContext())
            {
                var thisAlias = ConfigurationManager.AppSettings["ALIAS"];
                return db.Nodes.FirstOrDefault(x => x.Alias.Equals(thisAlias, StringComparison.OrdinalIgnoreCase));
            }
        }

        public static string GetDnsHostName() => Dns.GetHostEntry("localhost").HostName;
        public static NSContext GetNSContext() => new NSContext("ConnectionString");

    }
}
