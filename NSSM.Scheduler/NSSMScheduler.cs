using NSSM.Core.Models;
using NSSM.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace NSSM.Scheduler
{
    public partial class NSSMScheduler : ServiceBase
    {
        private Timer _Timer = new Timer();
        public EventLog _EventLog { get; set; }
        public override EventLog EventLog
        {
            get
            {
                if (_EventLog == null)
                {
                    _EventLog = new EventLog();
                    EventLog.Source = "NssmScheduler";
                    EventLog.Log = "NssmSchedulerLog";
                }
                return _EventLog;
            }
        }

        private Node _NodeInstance;
        public Node NodeInstance
        {
            get
            {
                if (_NodeInstance == null)
                    _NodeInstance = EntityService.GetNodeInstance();
                return _NodeInstance;
            }
        }

        public NSSMScheduler()
        {
            InitializeComponent();
            if (!EventLog.SourceExists("NssmScheduler"))
            {
                EventLog.CreateEventSource("NssmScheduler", "NssmSchedulerLog");
            }
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                EventLog.WriteEntry($"Starting Scheduler service at {DateTime.Now}.");
                EntityService.UpdateNodeInstance(true);
            }
            catch(Exception ex)
            {
                Utility.LogException(ex);
            }

            _Timer = new Timer(60000) { AutoReset = true };
            _Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _Timer.Enabled = true;
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var availableConnections = EntityService.GetNodeInstance().Concurrentscans;
                var scansCount = Utility.GetNetsparkerProcessCount();
                if (scansCount > 0)
                {
                    if (scansCount >= availableConnections)
                    {
                        EventLog.WriteEntry($"This NSSM service is using all available processes - [{DateTime.Now}]");
                        return;
                    }

                    availableConnections = availableConnections - scansCount;
                    EventLog.WriteEntry($"** Beginning exectution of {availableConnections} netsparker scan son this node! ");
                }

                var nextScans = EntityService.GetNextScans(availableConnections);
                if (nextScans != null)
                {
                    foreach (var newScan in nextScans)
                    {
                        var nodeInstance = EntityService.GetNodeInstance();
                        var project = EntityService.GetProjectInfo(newScan);

                        var newScanProcess = new NSProcess(newScan, project, nodeInstance);
                        await newScanProcess.ExecuteScanAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                EventLog.WriteEntry("Stopping Scheduler service.");
                EntityService.UpdateNodeInstance(false);
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }
    }
}
