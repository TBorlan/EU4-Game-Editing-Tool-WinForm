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
#pragma warning disable IDE1006 // Naming Styles
        private const int NumberOfColors = 18;
#pragma warning restore IDE1006 // Naming Styles

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

        [TestMethod]
        public void Test_BorderTwoSelectionTwoCubes2x2Have1LineCommon()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            List<BorderLine> expectLinesBothSelected = new List<BorderLine>(4)
                                                       {
                                                           new BorderLine(new BorderPoint(1, 4), new BorderPoint(5, 4)),
                                                           new BorderLine(new BorderPoint(5, 4), new BorderPoint(5, 6)),
                                                           new BorderLine(new BorderPoint(1, 4), new BorderPoint(1, 6)),
                                                           new BorderLine(new BorderPoint(1, 6), new BorderPoint(5, 6))
                                                       };
            List<BorderLine> expectLinesSecondDeselected = new List<BorderLine>(4)
                                                       {
                                                           new BorderLine(new BorderPoint(1, 4), new BorderPoint(3, 4)),
                                                           new BorderLine(new BorderPoint(3, 4), new BorderPoint(3, 6)),
                                                           new BorderLine(new BorderPoint(1, 4), new BorderPoint(1, 6)),
                                                           new BorderLine(new BorderPoint(1, 6), new BorderPoint(3, 6))
                                                       };
            manager.Select(GetColorFromIndex(4));
            manager.Select(GetColorFromIndex(5));
            List<BorderLine> borders = manager.GetBorderLines();
            Assert.AreEqual(4, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesBothSelected, borders);
            manager.Select(GetColorFromIndex(5));
            borders = manager.GetBorderLines();
            Assert.AreEqual(4, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesSecondDeselected, borders);
        }

        [TestMethod]
        public void Test_BorderTwoSelectionTwoRectangles1x2Have1LineSameLengthWithSegmentCommon()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            List<BorderLine> expectLinesBothSelected = new List<BorderLine>(8)
                                                       {
                                                           new BorderLine(new BorderPoint(6, 5), new BorderPoint(6, 7)),
                                                           new BorderLine(new BorderPoint(6, 7), new BorderPoint(7, 7)),
                                                           new BorderLine(new BorderPoint(7, 6), new BorderPoint(7, 7)),
                                                           new BorderLine(new BorderPoint(7, 6), new BorderPoint(8, 6)),
                                                           new BorderLine(new BorderPoint(8, 4), new BorderPoint(8, 6)),
                                                           new BorderLine(new BorderPoint(7, 4), new BorderPoint(8, 4)),
                                                           new BorderLine(new BorderPoint(7, 4), new BorderPoint(7, 5)),
                                                           new BorderLine(new BorderPoint(6, 5), new BorderPoint(7, 5))
                                                       };
            List<BorderLine> expectLinesSecondDeselected = new List<BorderLine>(4)
                                                           {
                                                               new BorderLine(new BorderPoint(6, 5), new BorderPoint(6, 7)),
                                                               new BorderLine(new BorderPoint(6, 7), new BorderPoint(7, 7)),
                                                               new BorderLine(new BorderPoint(6, 5), new BorderPoint(7, 5)),
                                                               new BorderLine(new BorderPoint(7, 5), new BorderPoint(7, 7))
                                                           };
            manager.Select(GetColorFromIndex(6));
            manager.Select(GetColorFromIndex(7));
            List<BorderLine> borders = manager.GetBorderLines();
            Assert.AreEqual(8, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesBothSelected, borders);
            manager.Select(GetColorFromIndex(7));
            borders = manager.GetBorderLines();
            Assert.AreEqual(4, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesSecondDeselected, borders);
        }

        [TestMethod]
        public void Test_BorderTwoSelectionTwoRectangles1x2Have1LineOverlappingButDifferentLengths()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            List<BorderLine> expectLinesBothSelected = new List<BorderLine>(8)
                                                       {
                                                           new BorderLine(new BorderPoint(2, 7), new BorderPoint(4, 7)),
                                                           new BorderLine(new BorderPoint(2, 7), new BorderPoint(2, 8)),
                                                           new BorderLine(new BorderPoint(4, 7), new BorderPoint(4, 8)),
                                                           new BorderLine(new BorderPoint(1, 9), new BorderPoint(5, 9)),
                                                           new BorderLine(new BorderPoint(1, 8), new BorderPoint(1, 9)),
                                                           new BorderLine(new BorderPoint(1, 8), new BorderPoint(2, 8)),
                                                           new BorderLine(new BorderPoint(4, 8), new BorderPoint(5, 8)),
                                                           new BorderLine(new BorderPoint(5, 8), new BorderPoint(5, 9))
                                                       };
            List<BorderLine> expectLinesSecondDeselected = new List<BorderLine>(4)
                                                           {
                                                               new BorderLine(new BorderPoint(2, 7), new BorderPoint(4, 7)),
                                                               new BorderLine(new BorderPoint(2, 7), new BorderPoint(2, 8)),
                                                               new BorderLine(new BorderPoint(4, 7), new BorderPoint(4, 8)),
                                                               new BorderLine(new BorderPoint(2, 8), new BorderPoint(4, 8))
                                                           };
            manager.Select(GetColorFromIndex(8));
            manager.Select(GetColorFromIndex(9));
            List<BorderLine> borders = manager.GetBorderLines();
            Assert.AreEqual(8, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesBothSelected, borders);
            manager.Select(GetColorFromIndex(9));
            borders = manager.GetBorderLines();
            Assert.AreEqual(4, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesSecondDeselected, borders);
        }

        [TestMethod]
        public void Test_BorderThreeSelectionThreeRectangles1x2Have1LinePartialCommonBetweenAllThree()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            List<BorderLine> expectLinesAllThreeSelected = new List<BorderLine>(8)
                                                       {
                                                           new BorderLine(new BorderPoint(1, 11), new BorderPoint(2, 11)),
                                                           new BorderLine(new BorderPoint(2, 10), new BorderPoint(2, 11)),
                                                           new BorderLine(new BorderPoint(2, 10), new BorderPoint(3, 10)),
                                                           new BorderLine(new BorderPoint(3, 10), new BorderPoint(3, 14)),
                                                           new BorderLine(new BorderPoint(2, 14), new BorderPoint(3, 14)),
                                                           new BorderLine(new BorderPoint(2, 13), new BorderPoint(2, 14)),
                                                           new BorderLine(new BorderPoint(1, 13), new BorderPoint(2, 13)),
                                                           new BorderLine(new BorderPoint(1, 11), new BorderPoint(1, 13))
                                                       };
            List<BorderLine> expectLinesThirdDeselected = new List<BorderLine>(8)
                                                           {
                                                               new BorderLine(new BorderPoint(1, 11), new BorderPoint(2, 11)),
                                                               new BorderLine(new BorderPoint(2, 10), new BorderPoint(2, 11)),
                                                               new BorderLine(new BorderPoint(2, 10), new BorderPoint(3, 10)),
                                                               new BorderLine(new BorderPoint(3, 10), new BorderPoint(3, 12)),
                                                               new BorderLine(new BorderPoint(2, 12), new BorderPoint(3, 12)),
                                                               new BorderLine(new BorderPoint(2, 12), new BorderPoint(2, 13)),
                                                               new BorderLine(new BorderPoint(1, 13), new BorderPoint(2, 13)),
                                                               new BorderLine(new BorderPoint(1, 11), new BorderPoint(1, 13))
                                                           };
            List<BorderLine> expectLinesThirdAndSecondDeselected = new List<BorderLine>(4)
                                                          {
                                                              new BorderLine(new BorderPoint(1, 11), new BorderPoint(2, 11)),
                                                              new BorderLine(new BorderPoint(2, 11), new BorderPoint(2, 13)),
                                                              new BorderLine(new BorderPoint(1, 13), new BorderPoint(2, 13)),
                                                              new BorderLine(new BorderPoint(1, 11), new BorderPoint(1, 13))
                                                          };
            manager.Select(GetColorFromIndex(10));
            manager.Select(GetColorFromIndex(11));
            manager.Select(GetColorFromIndex(12));
            List<BorderLine> borders = manager.GetBorderLines();
            Assert.AreEqual(8, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesAllThreeSelected, borders);
            manager.Select(GetColorFromIndex(12));
            borders = manager.GetBorderLines();
            Assert.AreEqual(8, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesThirdDeselected, borders);
            manager.Select(GetColorFromIndex(11));
            borders = manager.GetBorderLines();
            Assert.AreEqual(4, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesThirdAndSecondDeselected, borders);
        }

        [TestMethod]
        public void Test_BorderTwoSelectionIrregularHave1LineWith2CommonSegments()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            List<BorderLine> expectLinesBothSelected = new List<BorderLine>(8)
                                                       {
                                                           new BorderLine(new BorderPoint(4, 10), new BorderPoint(7, 10)),
                                                           new BorderLine(new BorderPoint(7, 10), new BorderPoint(7, 13)),
                                                           new BorderLine(new BorderPoint(4, 13), new BorderPoint(7, 13)),
                                                           new BorderLine(new BorderPoint(4, 10), new BorderPoint(4, 13)),
                                                           new BorderLine(new BorderPoint(5, 11), new BorderPoint(6, 11)),
                                                           new BorderLine(new BorderPoint(6, 11), new BorderPoint(6, 12)),
                                                           new BorderLine(new BorderPoint(5, 12), new BorderPoint(6, 12)),
                                                           new BorderLine(new BorderPoint(5, 11), new BorderPoint(5, 12))
                                                       };
            List<BorderLine> expectLinesSecondDeselected = new List<BorderLine>(8)
                                                           {
                                                               new BorderLine(new BorderPoint(4, 10), new BorderPoint(6, 10)),
                                                               new BorderLine(new BorderPoint(6, 10), new BorderPoint(6, 11)),
                                                               new BorderLine(new BorderPoint(4, 13), new BorderPoint(6, 13)),
                                                               new BorderLine(new BorderPoint(4, 10), new BorderPoint(4, 13)),
                                                               new BorderLine(new BorderPoint(5, 11), new BorderPoint(6, 11)),
                                                               new BorderLine(new BorderPoint(6, 12), new BorderPoint(6, 13)),
                                                               new BorderLine(new BorderPoint(5, 12), new BorderPoint(6, 12)),
                                                               new BorderLine(new BorderPoint(5, 11), new BorderPoint(5, 12))
                                                           };
            manager.Select(GetColorFromIndex(13));
            manager.Select(GetColorFromIndex(14));
            List<BorderLine> borders = manager.GetBorderLines();
            Assert.AreEqual(8, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesBothSelected, borders);
            manager.Select(GetColorFromIndex(14));
            borders = manager.GetBorderLines();
            Assert.AreEqual(8, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesSecondDeselected, borders);
        }

        [TestMethod]
        public void Test_BorderThreeSelectionThreeRectanglesVariousSizesOneHasCommonLineWithTheRest()
        {
            TestSelectionManager manager = new TestSelectionManager(_mBorders);
            List<BorderLine> expectLinesAllThreeSelected = new List<BorderLine>(8)
                                                           {
                                                               new BorderLine(new BorderPoint(1, 15), new BorderPoint(6, 15)),
                                                               new BorderLine(new BorderPoint(6, 15), new BorderPoint(6, 17)),
                                                               new BorderLine(new BorderPoint(4, 17), new BorderPoint(6, 17)),
                                                               new BorderLine(new BorderPoint(4, 16), new BorderPoint(4, 17)),
                                                               new BorderLine(new BorderPoint(3, 16), new BorderPoint(4, 16)),
                                                               new BorderLine(new BorderPoint(3, 16), new BorderPoint(3, 17)),
                                                               new BorderLine(new BorderPoint(1, 17), new BorderPoint(3, 17)),
                                                               new BorderLine(new BorderPoint(1, 15), new BorderPoint(1, 17))
                                                           };
            List<BorderLine> expectLinesSecondDeselected = new List<BorderLine>(4)
                                                           {
                                                               new BorderLine(new BorderPoint(1, 15), new BorderPoint(6, 15)),
                                                               new BorderLine(new BorderPoint(6, 15), new BorderPoint(6, 16)),
                                                               new BorderLine(new BorderPoint(1, 16), new BorderPoint(6, 16)),
                                                               new BorderLine(new BorderPoint(1, 15), new BorderPoint(1, 16))
                                                           };
            manager.Select(GetColorFromIndex(15));
            manager.Select(GetColorFromIndex(16));
            manager.Select(GetColorFromIndex(17));
            List<BorderLine> borders = manager.GetBorderLines();
            Assert.AreEqual(8, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesAllThreeSelected, borders);
            manager.Select(GetColorFromIndex(16));
            manager.Select(GetColorFromIndex(17));
            borders = manager.GetBorderLines();
            Assert.AreEqual(4, borders.Count);
            CollectionAssert.AreEquivalent(expectLinesSecondDeselected, borders);
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
