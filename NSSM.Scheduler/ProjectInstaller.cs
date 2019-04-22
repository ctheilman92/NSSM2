using NSSM.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace NSSM.Scheduler
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void ServiceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            using (var db = Utility.GetNSContext())
            {
                var nodeAlias = Environment.MachineName;
                if (!db.Nodes.Any(x => !x.IsDeleted && x.Alias.Equals(nodeAlias)))
                {
                    db.Nodes.Add(new Node
                    {
                        Alias = nodeAlias,
                        Domain = "CROWE",
                        Concurrentscans = 2,
                    });

                    db.SaveChanges();
                }
            }
        }
    }
}
