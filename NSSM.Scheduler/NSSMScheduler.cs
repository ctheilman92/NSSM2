using NSSM.Core.Models;
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
        public Node _Node { get;set; }
        public override EventLog EventLog
        {
            get
            {
                if (_EventLog == null)
                    _EventLog = new EventLog();
                return _EventLog;
            }
        }

        public NSSMScheduler()
        {
            InitializeComponent();
        }

        private void UpdateNodeInstance(bool isActive)
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

        protected override void OnStart(string[] args)
        {
            try
            {
                EventLog.WriteEntry($"Starting Scheduler service at {DateTime.Now}.");
                UpdateNodeInstance(true);
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
                var availableConnections = Utility.GetNodeInstance().Concurrentscans;
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

                var nextScans = new List<Scan>();
                using (var db = Utility.GetNSContext())
                {
                    nextScans = db.Scans
                        .Where(x => x.IsActive && x.Status == ScanStatus.Pending && x.NodeInstanceId == 0 && !x.InvokeDate.HasValue)
                        .OrderBy(x => x.ModifiedDate).Take(availableConnections).ToList();
                }

                if (nextScans != null)
                {
                    foreach (var newScan in nextScans)
                    {
                        var newScanProcess = new NSProcess(newScan);
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
                UpdateNodeInstance(false);
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }
    }
}
