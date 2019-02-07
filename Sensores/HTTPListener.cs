using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sensores
{
    //PLUS: Permitir que el sistema de monitoreo reciba los mensajes mediante HTTP.
    public class HTTPListener
    {
        //Log
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly HttpListener Listener = new HttpListener();

        public HTTPListener()
        {
            Listener.Prefixes.Add("http://localhost:8081/");
            Listener.Prefixes.Add("http://127.0.0.1:8081/");
            XmlConfigurator.Configure();
        }

        public async Task Listen()
        {
            try
            {
                Listener.Start();
            }
            catch (HttpListenerException hlex)
            {
                _log.Error(hlex);
                return;
            }
            while (Listener.IsListening)
            {
                var context = await Listener.GetContextAsync();
                try
                {
                    await ProcessRequestAsync(context);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
            Listener.Close();
        }

        private static async Task ProcessRequestAsync(HttpListenerContext context)
        {
            string respuesta = "";
            // Get HTTP stream
            var body = await new StreamReader(context.Request.InputStream).ReadToEndAsync();
            HttpListenerRequest request = context.Request;
            MQueue cola = new MQueue();

            if (request.RawUrl.StartsWith("/sensores"))
            {
                try
                {
                    //Get parametros
                    var options = context.Request.QueryString;
                    var keys = options.AllKeys;
                    //Añadir a la cola
                    int[] valores = new int[keys.Length];
                    for (int i = 0; i < valores.Length; i++)
                    {
                        cola.EnviarMensaje(keys[i], options[keys[i]]);
                    }/*
                    if (keys.Length == 1)
                    {
                        cola.EnviarMensaje(keys[0], options[keys[0]]);
                        respuesta = "OK";
                    }
                    else
                    {
                        int[] valores = new int[keys.Length];
                        for (int i = 0; i < valores.Length; i++)
                        {
                            valores[i] = int.Parse(options[keys[i]], System.Globalization.CultureInfo.InvariantCulture);
                        }
                        cola.EnviarMensaje(valores);
                        respuesta = "OK";
                    }*/
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
                //Respond
                byte[] b = Encoding.UTF8.GetBytes(respuesta);
                context.Response.StatusCode = 200;
                context.Response.KeepAlive = false;
                context.Response.ContentLength64 = b.Length;
                var output = context.Response.OutputStream;
                await output.WriteAsync(b, 0, b.Length);
                context.Response.Close();
            }
        }
    }
}
