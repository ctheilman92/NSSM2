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
                if (_NodeInstance == null)
                    _NodeInstance = Utility.GetNodeInstance();
                return _NodeInstance;
            }
        }

        public string ProcessErrors { get; set; }

        private TaskCompletionSource<int> _tcs { get; set; }

        private Process _process { get; set; }
        
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
            _tcs = new TaskCompletionSource<int>();
            _process = new Process { EnableRaisingEvents = true };
            _process.Exited += OnProcessExit;
            _process.ErrorDataReceived += OnErrorData;
            _process.OutputDataReceived += OnOutputData;

            UpdateScan(ScanStatus.Running);

            var reportFileName = $"Report-{CurrentScan.Id}.pdf";
            var vulnerabilityFileName = $"Vulnerabilities-{CurrentScan.Id}.csv";

            var scanPath = (!string.IsNullOrEmpty(CurrentScan.ScanAlias))
                ? Path.Combine(ProjectInfo.SummaryLocation, CurrentScan.ScanAlias)
                : Path.Combine(ProjectInfo.SummaryLocation, $"SCAN_{CurrentScan.Id}");

            if (!Directory.Exists(ProjectInfo.SummaryLocation))
                Directory.CreateDirectory(ProjectInfo.SummaryLocation);

            if (Directory.Exists(Path.GetDirectoryName(scanPath)))
                Directory.Delete(Path.GetDirectoryName(scanPath), true);

            Directory.CreateDirectory(scanPath);

            var arguments = $"/u \"{CurrentScan.TargetUrl}\" ";
            arguments += $"/p \"Default Scan Policy\" /a /s ";
            arguments += $"/r \"{Path.Combine(scanPath, reportFileName)}\" /rt \"Detailed Scan Report\" ";
            arguments += $"/r \"{Path.Combine(scanPath, vulnerabilityFileName)}\" /rt \"Vulnerabilities List (CSV)\" ";

            _process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = arguments,
                FileName = NodeInstance.ExecutableLocation
            };

            _process.Start();
            return _tcs.Task;
        }

        private void OnOutputData(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            _tcs.SetResult(_process.ExitCode);
            _process.Dispose();
            UpdateScan(ScanStatus.Complete);
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
