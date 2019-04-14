using NSSM.Core.Models;
using NSSM2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSSM.Scheduler
{
    public class NSProcess
    {
        private Scan CurrentScan { get; set; }
        private Project _ProjectInfo { get; set; }
        public Project ProjectInfo
        {
            get
            {
                if (_ProjectInfo == null)
                {
                    if (CurrentScan == null)
                        throw new ArgumentException("Current scan is unavailable. Cannot process project information..");
                    using (var db = Utility.GetNSContext())
                    {
                        return db.Projects.FirstOrDefault(x => x.Id == CurrentScan.ProjectId);
                    }
                }
                return _ProjectInfo;
            }
        }
        private Node _NodeInstance { get; set; }
        public Node NodeInstance
        {
            get
            {
                if (this.NodeInstance == null)
                    _NodeInstance = Utility.GetNodeInstance();
                return _NodeInstance;
            }
        }
        private string ReportPath
        {
            get
            {
                return (CurrentScan == null) 
                    ? string.Empty 
                    : Path.Combine(CurrentScan.ExportPath, $"Report-{CurrentScan.TargetUrl}-{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}.pdf");
            }
        }
        private string VulnerabilitiesPath
        {
            get
            {
                return (CurrentScan == null) 
                    ? string.Empty 
                    : Path.Combine(CurrentScan.ExportPath, $"Vulnerabilities-{CurrentScan.TargetUrl}-{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}.csv");
            }
        }

        public string ProcessErrors { get; set; }
        

        public NSProcess()
        {
            CurrentScan = GetNextScan();
        }

        //for UNIT TESTS
        public NSProcess(Scan scan)
        {
            CurrentScan = scan;
        }
        public Task<int> ExecuteScanAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            //will make async later if able to execute passive scan 
            Process proc = new Process { EnableRaisingEvents = true, };
            proc.StartInfo.FileName = NodeInstance.ExecutableLocation;
            proc.StartInfo.Arguments = $"/u {CurrentScan.TargetUrl} ";
            proc.StartInfo.Arguments += $"/p \"Default Scan Policy\" /a /s ";
            proc.StartInfo.Arguments += $"/r \"{ReportPath}\" /rt \"Detailed Scan Report\" ";
            proc.StartInfo.Arguments += $"/r {VulnerabilitiesPath} /rt \"Vulnerabilities List (CSV)\" ";
            proc.Exited += (sender, args) =>
            {
                tcs.SetResult(proc.ExitCode);
                proc.Dispose();
            };

            proc.Start();

            return tcs.Task;
        }

        public void UpdateScan(ScanStatus status)
        {
            CurrentScan.Status = status;
            switch (status)
            {
                case ScanStatus.None:
                    break;
                case ScanStatus.Pending:
                    break;
                case ScanStatus.Running:
                    CurrentScan.InvokeDate = DateTime.Now;
                    CurrentScan.NodeInstanceId = NodeInstance.Id;
                    break;
                case ScanStatus.Error:
                    CurrentScan.Error = ProcessErrors;
                    break;
                case ScanStatus.Complete:
                    CurrentScan.EndDate = DateTime.Now;
                    break;
                default:
                    break;
            }

            using (var db = Utility.GetNSContext())
            {
                db.Entry(CurrentScan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public Scan GetNextScan()
        {
            using (var db = Utility.GetNSContext())
            {
                return db.Scans.Where(x => x.Status == ScanStatus.Pending)
                    .OrderBy(x => x.CreatedDate).FirstOrDefault();
            }
        }
    }
}
