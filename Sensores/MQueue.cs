using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reflection;

namespace Sensores
{
    public class MQueue
    {
        //Log
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MessageQueue mq = null;

        public MQueue()
        {
            mq = new MessageQueue(@".\private$\Sensores");
            XmlConfigurator.Configure();
        }
        public MQueue(string colaMQ)
        {
            mq = new MessageQueue(@colaMQ);
            XmlConfigurator.Configure();
        }

        internal bool EnviarMensaje(List<double> valores)
        {
            bool resultado = true;
            string mensaje = "";
            try
            {
                mq.Send(valores);
                mensaje = "Valores añadidos a la cola: " + string.Join(",", valores);
                Console.WriteLine(mensaje);
                _log.Info(mensaje);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                mensaje = "Error al enviar el mensaje a la cola. Revisar log.";
                Console.WriteLine(mensaje);
                resultado = false;
            }
            return resultado;
        }
        internal bool EnviarMensaje(int[] valores)
        {
            bool resultado = true;
            string mensaje = "";
            try
            { 
                mq.Send(valores);
                mensaje = "Valores añadidos a la cola: " + string.Join(",", valores);
                Console.WriteLine(mensaje);
                _log.Info(mensaje);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _log.Error(ex);
                mensaje = "Error al enviar el mensaje a la cola. Revisar log.";
                Console.WriteLine(mensaje);
                resultado = false;
            }
            return resultado;
        }

        internal bool EnviarMensaje(string sensor, string valor)
        {
            bool resultado = true;
            int[] sensorValor = new int[2];
            string mensaje = "";
            try
            {
                sensorValor[0] = int.Parse(sensor, System.Globalization.CultureInfo.InvariantCulture);
                sensorValor[1] = int.Parse(valor, System.Globalization.CultureInfo.InvariantCulture);
                mq.Send(sensorValor);
                mensaje = "Valores añadidos a la cola: " + string.Join(",", sensor, valor);
                Console.WriteLine(mensaje);
                _log.Info(mensaje);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                mensaje = "Error al enviar el mensaje a la cola. Revisar log.";
                Console.WriteLine(mensaje);
                resultado = false;
            }
            return resultado;
        }
    }
}
