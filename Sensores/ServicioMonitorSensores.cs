using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Messaging;
using log4net;
using System.Reflection;
using log4net.Config;
using System.Threading;

namespace Sensores
{
    public partial class ServicioMonitorSensores : ServiceBase
    {
        //Log
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private System.Timers.Timer timer = null;

        private System.Messaging.MessageQueue mq = null;

        static ManualResetEvent signal = new ManualResetEvent(false);
        static int count = 0;
        
        //Array de colas. Mediciones encoladas de cada sensor.
        Queue<int>[] Sensores = null;

        public ServicioMonitorSensores()
        {
            InitializeComponent();
            XmlConfigurator.Configure();
            Sensores = new Queue<int>[4];
            for (int i = 0; i < Sensores.Length; i++)
            {
                Sensores[i] = new Queue<int>();
            }
            
        }

        public void OnDebug()
        {
            OnStart(null);


            Console.ReadLine();
        }

        protected override void OnStart(string[] args)
        {
            //HTTP listener
            //http://127.0.0.1:8081/sensores?var1=15&var2=514&var3=125&var4=145
            HTTPListener listen = new HTTPListener();
            listen.Listen();

            //Recepcion mensaje
            //Los sensores envían 2 mediciones por segundo (en forma independiente y potencialmente simultánea)
            mq = new MessageQueue(@".\private$\Sensores");
            mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(int[]) });
            mq.ReceiveCompleted += new ReceiveCompletedEventHandler(MyReceiveCompleted);
            mq.BeginReceive();

            //Procesamiento princpal
            timer = new System.Timers.Timer();
            //El sistema de procesamiento, por limitaciones de hardware, sólo puede procesar información 2 veces por minuto.
            timer.Interval = 30000; // 30 seconds = 30000
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            timer.Enabled = true;
            timer.Start();

            signal.WaitOne();
        }

        private void MyReceiveCompleted(object sender, ReceiveCompletedEventArgs asyncResult)
        {
            try
            {
                MessageQueue mq = (MessageQueue)sender;
                string mensaje = "";
                
                Message mensajeMQ = mq.EndReceive(asyncResult.AsyncResult);
                var mensajeArr = (int[])mensajeMQ.Body;
                if (mensajeArr.Length == 4) //Recibi los 4 valores juntos
                {
                    for (int i = 0; i < mensajeArr.Length; i++)
                    {
                        Sensores[i].Enqueue(mensajeArr[i]);
                    }
                }
                else if (mensajeArr.Length == 2)    //Recibi el valor de uno de los sensores
                {
                    //Almaceno los valores enviados por los sensores
                    Sensores[mensajeArr[0] - 1].Enqueue(mensajeArr[1]);
                }
                //Todos los mensajes recibidos deben ser loggeados
                mensaje = "Valores recibidos: " + string.Join(",", mensajeArr);
                Console.WriteLine(mensaje);
                _log.Info(mensaje);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            count += 1;
            if (count == 10)
            {
                signal.Set();
            }
            mq.BeginReceive();
            return;
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            timer.Enabled = false;
            ExecuteProcess();
        }

        private void ExecuteProcess()
        {
            try
            {
                //Validar que los 4 sensores hayan enviado mediciones para hacer el proceso
                bool todosTienenValor = true;
                int[] valores = new int[4];
                int sensor = 0;
                foreach (var item in Sensores)
                {
                    if (item.Count == 0)
                    {
                        todosTienenValor = false;
                        break;
                    }
                    else
                    {
                        valores[sensor] = item.Peek();
                    }
                    sensor++;
                }
                if (todosTienenValor)
                {
                    foreach (var item in Sensores)
                    {
                        item.Dequeue();
                    }
                    Monitor monitor = new Monitor(valores);
                    monitor.Monitoreo();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            var a = 0;
        }
    }
}
