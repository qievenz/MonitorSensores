using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sensores
{
    public class Monitor
    {
        //Log
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //Constuctores
        public Monitor(List<int> valores)
        {
            XmlConfigurator.Configure();
            int s;
            int m;
            Valores = new List<int>();
            Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["Constante S"], out s);
            Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["Constante M"], out m);
            ConstanteS = s;
            ConstanteM = m;
            Valores = valores;
        }
        public Monitor(int[] valores)
        {
            XmlConfigurator.Configure();
            int s;
            int m;
            Valores = new List<int>();
            Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["Constante S"], out s);
            Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["Constante M"], out m);
            ConstanteS = s;
            ConstanteM = m;
            Valores.AddRange(valores);
        }

        //Propiedades
        public List<int> Valores { get; set; }
        public int ConstanteS { get; set; }
        public int ConstanteM { get; set; }

        //Metodos
        public int Monitoreo()
        {
            int resultado = 0;

            try
            {
                //asi como también registrar información al momento de su procesamiento
                string mensaje = "Input: " + (String.Join(", ", Valores.ToArray())) + " / " + "S: " + ConstanteS + " / " + "M: " + ConstanteM;
                Console.WriteLine(mensaje);
                _log.Info(mensaje);

                //Anomalia 1
                //La diferencia entre el valor mínimo y máximo recibido sea mayor a una constante S
                if (Valores.Max() - Valores.Min() > ConstanteS)
                {
                    //mensaje = "Anomalia encontrada: diferencia = " + (input.Max() - input.Min()) + ". S = " + s;
                    mensaje = "Anomalia encontrada: diferencia mayor a S.";
                    Console.WriteLine(mensaje);
                    _log.Info(mensaje);
                    resultado = 1;
                }

                //Anomalia 2
                //El valor promedio sea superior a una constante M
                if ((Valores.Average()) > ConstanteM)
                {
                    //mensaje = "Anomalia encontrada: promedio = " + (input.Average()) + ". S = " + m;
                    mensaje = "Anomalia encontrada: promedio mayor a M.";
                    Console.WriteLine(mensaje);
                    _log.Info(mensaje);
                    resultado = 2;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                resultado = -1;
            }
            return resultado;
        }
    }
}
