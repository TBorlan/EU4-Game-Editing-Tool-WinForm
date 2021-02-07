using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EU4GET_WF.ImageRendering.Border;
using EU4GET_WF.ImageRendering.Logic;
using Pose;

namespace EU4GET_WF.ImageRendering.UnitTest
{
    [TestClass]
    public class ProvinceBorderTests
    {
        private const int NumberOfColors = 5;

        private static ProvinceBorders _mBorders = null;

        private Color GetColorFromIndex(int index)
        {
            return Color.FromArgb(index * 3, index * 4, index * 5);
        }

        [ClassInitialize]
        public static void LoadBitmap(TestContext _)
        {
            using Bitmap testBitmap = new Bitmap(Properties.Resources.Test);
            {
                _mBorders = ProvinceBorders.GetProvinceBorders(testBitmap, NumberOfColors);
            }
        }

        [TestMethod]
        public void Test_BorderCube2x2()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            manager.Select(GetColorFromIndex(1));
            List<BorderLine> borders = manager.GetBorderLines();
            List<BorderLine> expectLines = new List<BorderLine>(4)
                                           {
                                               new BorderLine(new BorderPoint(1, 1), new BorderPoint(1, 3)),
                                               new BorderLine(new BorderPoint(1, 1), new BorderPoint(3, 1)),
                                               new BorderLine(new BorderPoint(3, 1), new BorderPoint(3, 3)),
                                               new BorderLine(new BorderPoint(1, 3), new BorderPoint(3, 3))
                                           };

            Assert.AreEqual(4,borders.Count);
            CollectionAssert.AreEquivalent(expectLines,borders);

        }

        [TestMethod]
        public void Test_BorderTwoSelectionTwoCubes2x2Separate()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            manager.Select(GetColorFromIndex(2));
            manager.Select(GetColorFromIndex(3));
            List<BorderLine> borders = manager.GetBorderLines();
            List<BorderLine> expectLinesCube1 = new List<BorderLine>(4)
                                           {
                                               new BorderLine(new BorderPoint(4, 1), new BorderPoint(4, 3)),
                                               new BorderLine(new BorderPoint(4, 1), new BorderPoint(6, 1)),
                                               new BorderLine(new BorderPoint(6, 1), new BorderPoint(6, 3)),
                                               new BorderLine(new BorderPoint(4, 3), new BorderPoint(6, 3))
                                           };
            List<BorderLine> expectLinesCube2 = new List<BorderLine>(4)
                                                {
                                                    new BorderLine(new BorderPoint(7, 1), new BorderPoint(7, 3)),
                                                    new BorderLine(new BorderPoint(7, 1), new BorderPoint(9, 1)),
                                                    new BorderLine(new BorderPoint(9, 1), new BorderPoint(9, 3)),
                                                    new BorderLine(new BorderPoint(7, 3), new BorderPoint(9, 3))
                                                };
            Assert.AreEqual(8,borders.Count);
            CollectionAssert.AreEquivalent(expectLinesCube1.Concat(expectLinesCube2).ToList(),borders);

        }

        private class TestSelectionManager : SelectionManager
        {
            public TestSelectionManager(ProvinceBorders borders) : base(borders)
            {
            }

            public List<BorderLine> GetBorderLines()
            {
                return this._mActivePixels.ToList();
            }
        }
    }
}
