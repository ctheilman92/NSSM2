using System.ServiceProcess;

namespace NSSM.Scheduler
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new NSSMScheduler()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
