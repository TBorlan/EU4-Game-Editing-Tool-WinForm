using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EU4GET_WF.ImageRendering.OptimizationTest.Tests
{
    internal class SpeedCounter
    {
        private static readonly HashSet<SpeedCounter> _mCounter = new HashSet<SpeedCounter>(10);

        private readonly string _mName;

        private readonly Stopwatch _mStopwatch;

        private readonly List<long> _mTimeEntries = new List<long>(10);

        public static SpeedCounter GetCounter(string name)
        {
            SpeedCounter tempCounter;
            if ((tempCounter = _mCounter.FirstOrDefault(counter => counter._mName == name)) == null)
            {
                tempCounter = new SpeedCounter(name);
                _mCounter.Add(tempCounter);
            }

            return tempCounter;
        }       

        private SpeedCounter(string name)
        {
            this._mName = name;
            this._mStopwatch = new Stopwatch();
            _mCounter.Add(this);
        }

        public void Start()
        {
            this._mStopwatch.Start();
        }

        public void Stop()
        {
            this._mStopwatch.Stop();
            this._mTimeEntries.Add(this._mStopwatch.ElapsedMilliseconds);
            this._mStopwatch.Reset();
        }

        public long mMinimum
        {
            get
            {
                return this._mTimeEntries.Min();
            }
        }

        public long mMaximum
        {
            get
            {
                return this._mTimeEntries.Max();
            }
        }

        public double mAverage
        {
            get
            {
                return this._mTimeEntries.Average();
            }
        }

        public long mSum
        {
            get
            {
                long sum = 0;
                foreach (long variable in this._mTimeEntries)
                {
                    sum += variable;
                }

                return sum;
            }
        }

        public long mLast
        {
            get
            {
                return _mTimeEntries.Last();
            }
        }
    }
}
