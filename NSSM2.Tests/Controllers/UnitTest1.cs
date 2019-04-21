using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSSM.Core.Models;
using NSSM.Scheduler;
using NSSM2.Core;

namespace NSSM2.Tests.Controllers
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestExecuteScan()
        {
            var member = new Member
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = "ctheilman92@gmail.com",
            };

            var testProj = new Project
            {
                ProjectName = "Unit Test Project",
                SummaryLocation = @"C:\Users\Raul Martinez\Desktop\Output\",
                CreatedById = 1,
            };

            var scans = new List<Scan>
            {
                new Scan
                {
                    TargetUrl = @"http://aspnet.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan0",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1,
                },
                new Scan
                {
                    TargetUrl = @"http://php.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan1",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1,
                },
                new Scan
                {
                    TargetUrl = @"http://aspnet.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan2",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1,
                },
                new Scan
                {
                    TargetUrl = @"http://php.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan3",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1,
                },
            };
            using (var db = Utility.GetNSContext())
            {
                db.Members.Add(member);
                db.Projects.Add(testProj);
                db.Scans.AddRange(scans);
                Debug.Assert(db.SaveChanges() > 0);


            }

            foreach (var scan in scans)
            {
                var procScan = new NSProcess(scan, testProj);
                await procScan.ExecuteScanAsync();
            }
        }
    }
}
