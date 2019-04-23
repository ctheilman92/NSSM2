using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSSM.Core.Models;
using NSSM.Scheduler;
using NSSM2.Core;

namespace NSSM2.Tests.Controllers
{

    [TestClass]
    public class UnitTest1 : TestBase
    {

        [TestMethod]
        public async Task TestExecuteScan()
        {
            Project testProj = null;
            List<Scan> scans = null;
            using (var db = new NSContext(_ConnectionString))
            {
                testProj = db.Projects.FirstOrDefault();
                scans = db.Scans.Where(x => x.ProjectId == testProj.Id).ToList();
            }

            Debug.Assert(testProj != null);
            Debug.Assert(scans != null);

            var nsCredentials = new NSCredendtials(string.Empty, "netsparker_sa", "F@l#tvCzP5e8Ga7FC%N%qQSzRETQBmKR%oN#r2sKF6nd@x0^HMd!8iyHtec^X&v@");
            using (var nsImpersonate = new NSImpersonation(nsCredentials))
            {
                foreach (var scan in scans)
                {
                    var procScan = new NSProcess(scan, testProj);
                    var result = await procScan.ExecuteScanAsync();
                    Debug.Assert(result == 0);
                }
            }
        }

        [TestMethod]
        public void TestGetNode()
        {
            var thisNode = Utility.GetNodeInstance();
            Debug.Assert(thisNode != null && thisNode.Id > 0);
        }

        [TestMethod]
        public async Task TestCreateProject()
        {
            var rowsEffected = 0;
            var testProj = new Project
            {
                ProjectName = "Unit Test Project",
                SummaryLocation = @"C:\Users\Raul Martinez\Desktop\Output\",
            };

            var scans = new List<Scan>
            {
                new Scan
                {
                    TargetUrl = @"http://aspnet.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan0",
                    Status = ScanStatus.Pending,
                },
                new Scan
                {
                    TargetUrl = @"http://php.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan1",
                    Status = ScanStatus.Pending,
                },
                new Scan
                {
                    TargetUrl = @"http://aspnet.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan2",
                    Status = ScanStatus.Pending,
                },
                new Scan
                {
                    TargetUrl = @"http://php.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan3",
                    Status = ScanStatus.Pending,
                },
            };

            using (var db = new NSContext(_ConnectionString))
            {
                testProj.CreatedById = db.Members.FirstOrDefault().Id;
                db.Projects.Add(testProj);
                rowsEffected += db.SaveChanges();

                scans = scans.Select(x => { x.ProjectId = testProj.Id; return x; }).ToList();
                db.Scans.AddRange(scans);
                rowsEffected += await db.SaveChangesAsync();

                Debug.Assert(rowsEffected > 0);
            }
        }
    }
}
