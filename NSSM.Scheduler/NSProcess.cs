using NSSM.Core.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace NSSM.Scheduler
{
    public class NSProcess
    {
        public Scan _CurrentScan { get; set; }
        private Project _ProjectInfo { get; set; }
        private Node _NodeInstance { get; set; }
        private TaskCompletionSource<int> _tcs { get; set; }

        public string processErrors { get; set; }


        private Process process { get; set; }
        
        public NSProcess(Scan scan, Project projectInfo, Node nodeInstance)
        {
            _CurrentScan = scan;
            _ProjectInfo = projectInfo;
            _NodeInstance = nodeInstance;
        }

        public Task<int> ExecuteScanAsync()
        {
            _tcs = new TaskCompletionSource<int>();
            process = new Process { EnableRaisingEvents = true };

            Utility.UpdateScan(_CurrentScan, ScanStatus.Running, _NodeInstance.Id, string.Empty);

            var reportFileName = $"Report-{_CurrentScan.Id}.pdf";
            var vulnerabilityFileName = $"Vulnerabilities-{_CurrentScan.Id}.csv";

            var scanPath = (!string.IsNullOrEmpty(_CurrentScan.ScanAlias))
                ? Path.Combine(_ProjectInfo.SummaryLocation, _CurrentScan.ScanAlias)
                : Path.Combine(_ProjectInfo.SummaryLocation, $"SCAN_{_CurrentScan.Id}");

            if (!Directory.Exists(_ProjectInfo.SummaryLocation))
                Directory.CreateDirectory(_ProjectInfo.SummaryLocation);

            if (!Directory.Exists(scanPath))
                Directory.CreateDirectory(scanPath);

            var arguments = $"/u \"{_CurrentScan.TargetUrl}\" ";
            arguments += $"/p \"Default Scan Policy\" /a /s ";
            arguments += $"/r \"{Path.Combine(scanPath, reportFileName)}\" /rt \"Detailed Scan Report\" ";
            arguments += $"/r \"{Path.Combine(scanPath, vulnerabilityFileName)}\" /rt \"Vulnerabilities List (CSV)\" ";

            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = arguments,
                FileName = _NodeInstance.ExecutableLocation,
                WorkingDirectory = Path.GetDirectoryName(_NodeInstance.ExecutableLocation),
                //UserName = "netsparker_sa",
                //Password = GetSecurePassword("F@l#tvCzP5e8Ga7FC%N%qQSzRETQBmKR%oN#r2sKF6nd@x0^HMd!8iyHtec^X&v@")
            };

            process.Exited += OnProcessExit;
            process.ErrorDataReceived += OnErrorData;
            process.OutputDataReceived += OnOutputData;

            process.Start();
            return _tcs.Task;
        }

        private SecureString GetSecurePassword(string password)
        {
            var secureString = new SecureString();
            foreach (char c in password.ToCharArray())
            {
                secureString.AppendChar(c);
            }

            return secureString;
        }

        private void OnOutputData(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            Utility.UpdateScan(_CurrentScan, ScanStatus.Complete, _NodeInstance.Id, processErrors);
            _tcs.SetResult(process.ExitCode);
            process.Dispose();
        }

        public void OnErrorData(object pSender, DataReceivedEventArgs pError)
        {
            processErrors = pError.Data;
            Utility.UpdateScan(_CurrentScan, ScanStatus.Error, _NodeInstance.Id, processErrors);
        }
    }
}
