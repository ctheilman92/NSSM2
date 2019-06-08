using NSSM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSSM.Core.Services
{
    public static class EntityService
    {
        private static IEntityRepository entityRepository = new EntityRepository();

        public static Node GetNodeInstance()
        {
            return entityRepository.GetNodeInstance();
        }

        public static Project GetProjectInfo(Scan scan)
        {
            return entityRepository.GetProjectInfo(scan);
        }

        public static List<Scan> GetNextScans(int availableConnections)
        {
            return entityRepository.GetNextScans(availableConnections);
        }

        public static void UpdateScan(Scan current, ScanStatus status, int? nodeId, string processErrors)
        {
            entityRepository.UpdateScan(current, status, nodeId, processErrors);
        }

        public static void UpdateNodeInstance(bool isActive)
        {
            entityRepository.UpdateNodeInstance(isActive);
        }
    }
}
