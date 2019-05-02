using NSSM.Core.Models;
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
        public static string _ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["NSCONTEXTCONNSTRING"].ConnectionString;
            }
        }

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

        public static Node GetNodeInstance()
        {
            using (var db = Utility.GetNSContext())
            {
                var thisAlias = Environment.MachineName;
                var thisNode = db.Nodes.ToList().FirstOrDefault(x => x.Alias.Equals(thisAlias));

                if (thisNode == null)
                {
                    thisNode = new Node
                    {
                        Alias = Environment.MachineName,
                        Concurrentscans = 2,
                        AdminMemberId = 1, //this needs to be changed later
                        CreatedbyId = 1,
                    };

                    db.Nodes.Add(thisNode);
                    if (db.SaveChanges() == 0)
                        throw new Exception("Could not create Node Instance..");
                }

                return thisNode;
            }
        }

        public static Project GetProjectInfo(Scan scan)
        {
            using (var db = Utility.GetNSContext())
            {
                return db.Projects.FirstOrDefault(x => x.Id == scan.ProjectId);
            }
        }

        public static List<Scan> GetNextScans(int availableConnections)
        {
            using (var db = Utility.GetNSContext())
            {
                return db.Scans.Where(x => x.IsActive && x.Status == ScanStatus.Pending && x.NodeInstanceId == 0 && !x.InvokeDate.HasValue)
                    .OrderBy(x => x.ModifiedDate).Take(availableConnections).ToList();
            }
        }

        public static void UpdateScan(Scan currentScan, ScanStatus status, int? NodeId, string processErrors)
        {
            currentScan.Status = status;
            switch (status)
            {
                case ScanStatus.Running:
                    currentScan.InvokeDate = DateTime.Now;
                    currentScan.NodeInstanceId = NodeId;
                    break;
                case ScanStatus.Error:
                    currentScan.Error = processErrors;
                    break;
                case ScanStatus.Complete:
                    currentScan.EndDate = DateTime.Now;
                    break;
                default:
                    break;
            }

            using (var db = Utility.GetNSContext())
            {
                db.Entry(currentScan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void UpdateNodeInstance(bool isActive)
        {
            using (var db = Utility.GetNSContext())
            {
                var alias = Environment.MachineName;
                var thisNode = db.Nodes.FirstOrDefault(x => x.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase));

                if (thisNode == null)
                {
                    db.Nodes.Add(new Node
                    {
                        Alias = alias,
                        Domain = "CROWE",
                        Concurrentscans = 2,
                        IsActive = true,
                        StartTime = DateTime.Now,
                    });

                    if (db.SaveChanges() == 0)
                        throw new ArgumentException("Could not create node record..");
                }
                else
                {
                    if (isActive)
                    {
                        thisNode.IsActive = true;
                        thisNode.StartTime = DateTime.Now;
                        thisNode.StopTime = null;
                    }
                    else
                    {
                        thisNode.IsActive = false;
                        thisNode.StopTime = DateTime.Now;
                        thisNode.StartTime = null;
                    }

                    db.Entry(thisNode).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() == 0)
                        throw new ArgumentException("Could not update node record..");
                }
            }
        }

        public static NSContext GetNSContext() => new NSContext(_ConnectionString);

    }
}
