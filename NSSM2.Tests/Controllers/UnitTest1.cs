using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSSM.Core.Models;
using NSSM.Scheduler;

namespace NSSM2.Tests.Controllers
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestExecuteScan()
        {
            var testProj = new Project
            {
                ProjectName = "Unit Test Project",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProjectAdminId = 1,
                Id=1,
                IsActive = true,
            };

            var scans = new List<Scan>
            {
                new Scan
                {
                    TargetUrl = "",
                    CreatedById = 1,
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan0",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1
                },
                new Scan
                {
                    TargetUrl = "",
                    CreatedById = 1,
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan1",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1
                },
                new Scan
                {
                    TargetUrl = "",
                    CreatedById = 1,
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan2",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1
                },
                new Scan
                {
                    TargetUrl = "",
                    CreatedById = 1,
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan3",
                    CreatedDate = DateTime.Now,
                    ProjectId = 1
                },
            };

            foreach (var scan in scans)
            {
                var procScan = new NSProcess(scan, testProj);
                await procScan.ExecuteScanAsync();
            }
        }
    }
}
