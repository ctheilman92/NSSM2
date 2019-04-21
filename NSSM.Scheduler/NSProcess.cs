using NSSM.Core.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NSSM.Scheduler
{
    public class NSProcess
    {
        public Scan CurrentScan { get; set; }

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
                if (CurrentScan == null)
                    return string.Empty;
                var scanExportPath = Path.Combine(ProjectInfo.SummaryLocation, CurrentScan.ScanAlias);

                return (!string.IsNullOrEmpty(CurrentScan.ScanAlias))
                    ? Path.Combine(scanExportPath, $"Report-{CurrentScan.TargetUrl}-{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}.pdf")
                    : Path.Combine(scanExportPath, $"Report-SCAN#{CurrentScan.Id}-{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}.pdf");
            }
        }

        private string VulnerabilitiesPath
        {
            get
            {
                if (CurrentScan == null)
                    return string.Empty;
                var scanExportPath = Path.Combine(ProjectInfo.SummaryLocation, CurrentScan.ScanAlias);

                return (!string.IsNullOrEmpty(CurrentScan.ScanAlias))
                    ? Path.Combine(CurrentScan.ExportPath, $"Vulnerabilities-SCAN#{CurrentScan.ScanAlias}-{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}.csv")
                    : Path.Combine(CurrentScan.ExportPath, $"Vulnerabilities-SCAN#{CurrentScan.Id}-{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}.csv");
            }
        }

        public string ProcessErrors { get; set; }
        
        public NSProcess(Scan scan)
        {
            CurrentScan = scan;
        }
        public NSProcess(Scan scan, Project project)
        {
            CurrentScan = scan;
            _ProjectInfo = project;
        }

        public Task<int> ExecuteScanAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            Process proc = new Process { EnableRaisingEvents = true, };
            proc.StartInfo.FileName = NodeInstance.ExecutableLocation;
            proc.StartInfo.Arguments = $"/u {CurrentScan.TargetUrl} ";
            proc.StartInfo.Arguments += $"/p \"Default Scan Policy\" /a /s ";
            proc.StartInfo.Arguments += $"/r \"{ReportPath}\" /rt \"Detailed Scan Report\" ";
            proc.StartInfo.Arguments += $"/r \"{VulnerabilitiesPath}\" /rt \"Vulnerabilities List (CSV)\" ";

            proc.ErrorDataReceived += OnErrorData;

            proc.Exited += (sender, args) =>
            {
                UpdateScan(ScanStatus.Complete);
                tcs.SetResult(proc.ExitCode);
                proc.Dispose();
            };

            proc.Start();
            return tcs.Task;
        }

        public void OnErrorData(object pSender, DataReceivedEventArgs pError)
        {
            ProcessErrors = pError.Data;
            UpdateScan(ScanStatus.Error);
        }

        public void UpdateScan(ScanStatus status)
        {
            CurrentScan.Status = status;
            switch (status)
            {
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
    }
}
