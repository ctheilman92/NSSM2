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
        Timer timer = new Timer();
        public EventLog _EventLog { get; set; }
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
                var alias = ConfigurationManager.AppSettings["ALIAS"];
                var thisNode = db.Nodes.FirstOrDefault(x => x.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase));

                if (thisNode == null)
                    throw new NullReferenceException("Node instance cannot be null..");

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
                db.SaveChanges();
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

            

            //update current node instance to be available
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            timer.Interval = 60000;
            timer.Enabled = true;
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
