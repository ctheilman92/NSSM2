using NSSM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSSM.Core.Services
{
    internal interface IEntityRepository
    {
        Node GetNodeInstance();
        Project GetProjectInfo(Scan scan);
        List<Scan> GetNextScans(int availableConnections);
        void UpdateScan(Scan current, ScanStatus status, int? nodeId, string processErrors);
        void UpdateNodeInstance(bool isActive);
    }
}
