using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sensores.UnitTests
{
    [TestClass]
    public class Monitoreo
    {
        //a) Escribir al menos dos tests que validen la funcionalidad alguna de las funcionalidades requeridas.
        [TestMethod]
        public void Monitoreo_OK()
        {
            int[] valores = new int[] { 10, 7, 3, 8 };
            var monitor = new Monitor(valores);
            var resultado = monitor.Monitoreo();
            Assert.AreEqual(resultado, 0);
        }
        [TestMethod]
        public void Monitoreo_Anomalia1()
        {
            int[] valores = new int[] { 34, 23, 44, 1 };
            var monitor = new Monitor(valores);
            var resultado = monitor.Monitoreo();
            Assert.AreEqual(resultado, 1);
        }
        [TestMethod]
        public void Monitoreo_Anomalia2()
        {
            int[] valores = new int[] { 40, 40, 45, 6 };
            var monitor = new Monitor(valores);
            var resultado = monitor.Monitoreo();
            Assert.AreEqual(resultado, 2);
        }
    }
}
