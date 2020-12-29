using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic = EU4GET_WF.ImageRendering.OptimizationTest.Ver1.Logic;
using Border = EU4GET_WF.ImageRendering.OptimizationTest.Ver1.Border;

namespace EU4GET_WF.ImageRendering.OptimizationTest.Tests
{
    static class Tests
    {
        public static void Main()
        {
            // Setup dotTrace
            // Foreach test, start tracing, and after, stop tracing
            // Also use speedcounters to record tests
            // Collect results and save snapshot
            // Create csv with counter results
            TestBordersGenerationx100();
            Console.ReadKey();
        }
        public static void TestBordersGenerationx100()
        {
#if DISABLESINGLETON
            Bitmap bitmap = new Bitmap(@"C:\Users\Tudor\Desktop\temp\provinces.bmp");
            for (int i = 0; i < 100; i++)
            {
                SpeedCounter counter = SpeedCounter.GetCounter("ProvinceBordersConstructor");
                counter.Start();
                Border.ProvinceBorders borders = Border.ProvinceBorders.GetProvinceBorders(bitmap, 3600);
                counter.Stop();
            }
            Console.Write(SpeedCounter.GetCounter("ProvinceBordersConstructor").mSum);
#endif
        }
    }
}
