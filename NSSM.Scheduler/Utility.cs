﻿using NSSM.Core.Models;
using NSSM2.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

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
            var nsProcs = Process.GetProcessesByName("Netsparker");
            return nsProcs.Count();
        }

    }
}
