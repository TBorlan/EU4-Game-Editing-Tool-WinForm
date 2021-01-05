using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JetBrains.Profiler.SelfApi.DotTrace;
#if VER1
using Border = EU4GET_WF.ImageRendering.OptimizationTest.Ver1.Border;
#endif

namespace EU4GET_WF.ImageRendering.OptimizationTest.Tests
{
    static class Tests
    {
#if VER1
        private const string Version = "VER1";
#endif
        public static void Main()
        {
            String packageDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "packages");
            String snapshotDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Snapshots");
            EnsurePrerequisite(downloadTo:packageDir);
            Config config = new Config();
            Directory.CreateDirectory(Path.Combine(snapshotDir, "Performance", Version));
            config.SaveToDir(Path.Combine( snapshotDir, "Performance", Version));
            Attach(config);
            for (int index = 1; index <= 100; index++)
            {
                Console.WriteLine("Running Test Border Generation no. {0}", index);
                SpeedCounter counter = SpeedCounter.GetCounter("ProvinceBordersConstructor");
                StartCollectingData();
                counter.Start();
                TestBordersGeneration();
                counter.Stop();
                StopCollectingData();
                Console.WriteLine("Test no. {0} finished in {1} seconds", index, (double)SpeedCounter.GetCounter("ProvinceBordersConstructor").mLast/1000);
            }
            SaveData();
            Detach();
            Console.WriteLine("Average test speed {0}", (double)SpeedCounter.GetCounter("ProvinceBordersConstructor").mAverage/1000);
            Console.WriteLine("Total test time {0}", (double)SpeedCounter.GetCounter("ProvinceBordersConstructor").mSum/1000);
            Console.ReadKey();
        }
        public static void TestBordersGeneration()
        {
#if DISABLESINGLETON
            Bitmap bitmap = new Bitmap(@"C:\Users\nxf56462\Desktop\extendedtimeline\map\provinces.bmp");
            Border.ProvinceBorders borders = Border.ProvinceBorders.GetProvinceBorders(bitmap, 3600);
#endif
        }
    }
}
