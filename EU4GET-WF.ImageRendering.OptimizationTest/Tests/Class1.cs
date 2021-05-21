using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Profiler.SelfApi;
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
#if TESTPERFORMANCE
            DotTrace.EnsurePrerequisite(downloadTo:packageDir);
            DotTrace.Config config = new DotTrace.Config();
            Directory.CreateDirectory(Path.Combine(snapshotDir, "Performance", Version));
            config.SaveToDir(Path.Combine( snapshotDir, "Performance", Version));
            DotTrace.Attach(config);
            for (int index = 1; index <= 100; index++)
            {
                Console.WriteLine("Running Test Border Generation no. {0}", index);
                SpeedCounter counter = SpeedCounter.GetCounter("ProvinceBordersConstructor");
                DotTrace.StartCollectingData();
                counter.Start();
                TestBordersGeneration();
                counter.Stop();
                DotTrace.StopCollectingData();
                Console.WriteLine("Test no. {0} finished in {1} seconds", index, (double)SpeedCounter.GetCounter("ProvinceBordersConstructor").mLast/1000);
            }
            DotTrace.SaveData();
            DotTrace.Detach();
            Console.WriteLine("Average test speed {0}", (double)SpeedCounter.GetCounter("ProvinceBordersConstructor").mAverage/1000);
            Console.WriteLine("Total test time {0}", (double)SpeedCounter.GetCounter("ProvinceBordersConstructor").mSum/1000);
            Console.ReadKey();
#elif TESTMEMORY
            DotMemory.EnsurePrerequisite(downloadTo:packageDir);
            DotMemory.Config config = new DotMemory.Config();
            Directory.CreateDirectory(Path.Combine(snapshotDir, "Memory", Version));
            config.SaveToDir(Path.Combine(snapshotDir, "Memory", Version));
            DotMemory.Attach(config);
            DotMemory.GetSnapshot("Setup finished");
            TestBordersGeneration();
            DotMemory.GetSnapshot("Border Generation Finished");
            DotMemory.Detach();
#endif
        }
        public static void TestBordersGeneration()
        {
            Bitmap bitmap = new Bitmap(Properties.Resources.provinces);
            Border.ProvinceBorders borders = Border.ProvinceBorders.GetProvinceBorders(bitmap, 3600);
            bitmap.Dispose();
        }
    }
}
