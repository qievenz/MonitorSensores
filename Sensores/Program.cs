using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Sensores
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
#if DEBUG
            ServicioMonitorSensores myService = new ServicioMonitorSensores();
            myService.OnDebug();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServicioMonitorSensores()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
