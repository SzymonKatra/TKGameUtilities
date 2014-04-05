using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TKGameUtilities
{
    public static class DU
    {
        private static Stopwatch m_sw;

        public static Stopwatch SWNew()
        {
            return new Stopwatch();
        }
        public static Stopwatch SWStartNew()
        {
            return Stopwatch.StartNew();
        }
        public static void SWStopAndWrite(Stopwatch sw)
        {
            sw.Stop();
            Console.WriteLine(sw.Elapsed.ToString() + " | ticks: " + sw.ElapsedTicks + " | ms: " + sw.ElapsedMilliseconds);
        }

        public static void TStart()
        {
            m_sw = SWStartNew();
        }
        public static void TStop()
        {
            SWStopAndWrite(m_sw);
        }
    }
}
