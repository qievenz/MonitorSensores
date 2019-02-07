using System;
using System.Collections.Generic;
using System.Messaging;

namespace Consola
{
    class Program
    {
        static void Main(string[] args)
        {
            //b) Desde la consola se deberá poder ejecutar un caso en el los 4 sensores generen información aleatoria que será procesada por el sistema de monitoreo
            int[] valoresArr = new int[4];
            MessageQueue mq = new MessageQueue(@".\private$\Sensores");
            //List<int> valoresLista = new List<int>() { 1, 2, 4, 5 };

            Console.Write("Ingrese los valores de los 4 sensores:" + Environment.NewLine);
            for (int i = 0; i < valoresArr.Length; i++)
            {
                Console.Write("     Sensor " + (i+1) +": ");
                valoresArr[i] = int.Parse(Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
            }
            mq.Send(valoresArr);
            Console.WriteLine("Valores añadidos a la cola: " + string.Join(",", valoresArr));

            //mq.Send(valoresLista);
           //Console.WriteLine("Valores añadidos a la cola: " + string.Join(",", valoresLista));
        }
    }
}
