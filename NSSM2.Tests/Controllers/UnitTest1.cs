using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSSM.Core.Models;
using NSSM.Scheduler;
using NSSM2.Core;

namespace NSSM2.Tests.Controllers
{
    public static class DBLogger
    {
        public static void Log(string message)
        {
            Debug.WriteLine("EF OUTPUT: {0} ", message);
        }
    }


    [TestClass]
    public class TestBase
    {
        public string _ConnectionString
        {
            get
            {
                return "Server=tcp:crowenetsparker.database.windows.net,1433;Initial Catalog=netsparker;Persist Security Info=False;User ID=netsparker_sql;Password=Q2di$P7O@sqAfJn*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            }
        }

        private NSContext GetNSContext()
        {
            var nsContext = new NSContext(_ConnectionString);
            nsContext.Database.Log = DBLogger.Log;
            return nsContext;
        }
    }


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
            foreach (var scan in scans)
            {
                var procScan = new NSProcess(scan, testProj);
                var result = await procScan.ExecuteScanAsync();
                Debug.Assert(result > 0);
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
                },
                new Scan
                {
                    TargetUrl = @"http://php.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan1",
                },
                new Scan
                {
                    TargetUrl = @"http://aspnet.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan2",
                },
                new Scan
                {
                    TargetUrl = @"http://php.testsparker.com/",
                    ExportPath = @"C:\Users\Raul Martinez\Desktop\Output\Scan3",
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
