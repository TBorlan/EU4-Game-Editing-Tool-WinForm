using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace EU4GET_WF.ImageRendering.Border
{
    public class ProvinceBorders
    {

        private ProvinceBorders(int provinceCount)
        {
            this._mProvinceCount = provinceCount;
            //mProvincesLines = new Dictionary<Color, HashSet<Point[]>>(this.mProvinceCount);
        }

        #region Members

        private static ProvinceBorders _mInstance;

        private Dictionary<Color, HashSet<BorderLine>> _mProvincesLines;

        private readonly int _mProvinceCount;

        private Dictionary<Color, HashSet<BorderPoint[]>> _mProvincesPoints;

        private Dictionary<Color, GraphicsPath> _mProvincePaths;

        #endregion

        #region Instance Generation

        public static ProvinceBorders GetProvinceBorders(Bitmap bitmap, int provinceCount)
        {
            if (_mInstance == null)
            {
                _mInstance = new ProvinceBorders(provinceCount);
                _mInstance.GenerateBordersPaths(bitmap);
            }
            return _mInstance;
        }

        private void GenerateBordersPaths(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            int height = bitmap.Height;
            int width = bitmap.Width;
            Color[,] pixelColors = new Color[bitmap.Height, bitmap.Width];
            // Store pixel color in matrix
            Parallel.For(0, bytes / 3, (int i) =>
                {
                    i *= 3;
                    int pRow = (i / 3) / width;
                    int pCol = (i / 3) % width;
                    pixelColors[pRow, pCol] = Color.FromArgb(rgbValues[i + 2], rgbValues[i + 1], rgbValues[i]);                   
                });           
            bitmap.UnlockBits(bmpData);
            // Stores vertical and horizontal border pixels of a province
            this._mProvincesPoints = new Dictionary<Color, HashSet<BorderPoint[]>>(this._mProvinceCount);

            object lockObj = new Object();
            //Traverse lines
            Parallel.For(0, height, (int prow) =>
            {
                for (int col = 0; col < width; col++)
                {
                    Color color = pixelColors[prow, col];
                    lock (lockObj)
                    {
                        if (!this._mProvincesPoints.ContainsKey(color))
                        {
                            this._mProvincesPoints.Add(color, new HashSet<BorderPoint[]>());
                        }
                    }
                    BorderPoint[] colorLine = new BorderPoint[3];
                    colorLine[0] = new BorderPoint(col, prow);
                    do
                    {
                        if(col < width)
                        {
                            if(color == pixelColors[prow, col])
                            {
                                col++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    } while (true);
                    col--;
                    colorLine[1] = new BorderPoint(col, prow);
                    colorLine[2] = new BorderPoint(-1, 0); // horizontal line
                    lock (lockObj)
                    {
                        HashSet<BorderPoint[]> lines = this._mProvincesPoints[color];
                        lines.Add(colorLine);
                    }
                }
            });
            //Traverse columns
            Parallel.For(0, width, (int col) =>
            {
                for (int row = 0; row < height; row++)
                {
                    Color color = pixelColors[row, col];
                    lock (lockObj)
                    {
                        if (!this._mProvincesPoints.ContainsKey(color))
                        {
                            this._mProvincesPoints.Add(color, new HashSet<BorderPoint[]>());
                        }
                    }
                    BorderPoint[] colorLine = new BorderPoint[3];
                    colorLine[0] = new BorderPoint(col, row);
                    do
                    {
                        if (row < height)
                        {
                            if (color == pixelColors[row, col])
                            {
                                row++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    } while (true);
                    row--;
                    colorLine[1] = new BorderPoint(col, row);
                    colorLine[2] = new BorderPoint(-2, 0); // vertical line
                    lock (lockObj)
                    {
                        HashSet<BorderPoint[]> lines = this._mProvincesPoints[color];
                        lines.Add(colorLine);
                    }
                }
            });
            this._mProvincesLines = new Dictionary<Color, HashSet<BorderLine>>(this._mProvincesPoints.Count);
            // Get border lines from border pixels
            object lockobj = new Object();
            Parallel.ForEach(this._mProvincesPoints, (KeyValuePair<Color, HashSet<BorderPoint[]>> keyValue) =>
            {
                lock (lockobj)
                {
                    this._mProvincesLines.Add(keyValue.Key, new HashSet<BorderLine>(keyValue.Value.Count / 4));
                }
                this.ProcessProvince(keyValue.Value, this._mProvincesLines[keyValue.Key]);

            });     
        }

        #endregion

        #region Path Generation

        /// <summary>
        /// Returns the <see cref="GraphicsPath"/> that borders the province
        /// </summary>
        /// <param name="color">Color of the province</param>
        /// <returns></returns>
        public GraphicsPath GetProvinceBorder(Color color)
        {
            HashSet<BorderLine> provinceLines = new HashSet<BorderLine>(this._mProvincesLines[color]);
            if (this._mProvincePaths == null)
            {
                this._mProvincePaths = new Dictionary<Color, GraphicsPath>(this._mProvincesLines.Count);
            }
            this._mProvincePaths[color] = this.BuildPath(provinceLines);
            return (GraphicsPath)this._mProvincePaths[color].Clone();
        }

        private GraphicsPath BuildPath(HashSet<BorderLine> lines)
        {
            lines = new HashSet<BorderLine>(lines);
            GraphicsPath path = new GraphicsPath();
            int x;
            BorderLine points = lines.First();

            path.StartFigure();
            path.AddLine(points.mStart, points.mEnd);
            lines.Remove(points);
            while (lines.Count > 0)
            {
                x = lines.Count;
                foreach (BorderLine line in lines.Where(line => path.GetLastPoint() == (PointF)line.mStart))
                {
                    path.AddLine(line.mStart, line.mEnd);
                    lines.Remove(line);
                    break;
                }
                if (x == lines.Count)
                {
                    path.StartFigure();
                    points = lines.First();
                    path.AddLine(points.mStart, points.mEnd);
                    lines.Remove(points);
                }
            }
            return path;
        }

        public void ComplementVirtualProvince(ref HashSet<BorderLine> provinceLines, Color complementingProvince)
        {
            //foreach (BorderLine line1 in this._mProvincesLines[complementingProvince])
            //{
            //    Point[][] newLine = new Point[2][];
            //    newLine[0] = new Point[3];
            //    Point[][] removeLine = new Point[2][];
            //    bool found = false;
            //    foreach (BorderLine line2 in provinceLines)
            //    //{
            //    //    if (pixel1[2] == pixel2[2])
            //    //    {

            //    //        if ((pixel1[0] == pixel2[0]) && (pixel1[1] == pixel2[1]))
            //    //        {
            //    //            if (pixel2[2].X == -2)
            //    //            {
            //    //                removeLine[0] = pixel2;
            //    //            }
            //    //            removeLine[0] = pixel2;
            //    //            newLine[0] = null;
            //    //            break;
            //    //        }
            //    //        else
            //    //        {
            //    //            Point testPoint1 = new Point();
            //    //            Point testPoint2 = new Point();
            //    //            testPoint1 = pixel1[1];
            //    //            testPoint2 = pixel2[1];
            //    //            if (pixel2[2].X == -1)
            //    //            {
            //    //                testPoint1.X += 1;
            //    //                testPoint2.X += 1;
            //    //            }
            //    //            else
            //    //            {
            //    //                testPoint1.Y += 1;
            //    //                testPoint2.Y += 1;
            //    //            }
            //    //            if (testPoint2 == pixel1[0])
            //    //            {
            //    //                if (found)
            //    //                {
            //    //                    newLine[0][0] = pixel2[0];
            //    //                    removeLine[1] = pixel2;
            //    //                    break;
            //    //                }
            //    //                newLine[0][0] = pixel2[0];
            //    //                newLine[0][1] = pixel1[1];
            //    //                newLine[0][2] = pixel2[2];
            //    //                removeLine[0] = pixel2;
            //    //                found = true;
            //    //            }
            //    //            else if (pixel2[0] == testPoint1)
            //    //            {
            //    //                if (found)
            //    //                {
            //    //                    newLine[0][1] = pixel2[1];
            //    //                    removeLine[1] = pixel2;
            //    //                    break;
            //    //                }
            //    //                newLine[0][0] = pixel1[0];
            //    //                newLine[0][1] = pixel2[1];
            //    //                newLine[0][2] = pixel2[2];
            //    //                removeLine[0] = pixel2;
            //    //                found = true;
            //    //            }
            //    //            else if (pixel2[0] == pixel1[0])
            //    //            {
            //    //                newLine[0][0] = pixel1[1] + (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
            //    //                newLine[0][1] = pixel2[1];
            //    //                newLine[0][2] = pixel2[2];
            //    //                removeLine[0] = pixel2;
            //    //                break;
            //    //            }
            //    //            else if (testPoint2 == testPoint1)
            //    //            {
            //    //                newLine[0][0] = pixel2[0];
            //    //                newLine[0][1] = pixel1[0] - (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
            //    //                newLine[0][2] = pixel2[2];
            //    //                removeLine[0] = pixel2;
            //    //                break;
            //    //            }
            //    //            else if (((pixel1[0].X == pixel2[0].X) && (pixel2[2].X == -2)) || ((pixel1[0].Y == pixel2[0].Y) && (pixel2[2].X == -1)))
            //    //            {
            //    //                Size start = (Size)pixel1[0] - (Size)pixel2[0];
            //    //                Size end = (Size)pixel1[1] - (Size)pixel2[1];
            //    //                if (((start.Width + start.Height) > 0) && ((end.Width + end.Height) < 0))
            //    //                {
            //    //                    newLine[0][0] = pixel2[0];
            //    //                    newLine[0][1] = pixel1[0] - (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
            //    //                    newLine[0][2] = pixel2[2];
            //    //                    newLine[1] = new Point[3];
            //    //                    newLine[1][0] = pixel1[1] + (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
            //    //                    newLine[1][1] = pixel2[1];
            //    //                    newLine[1][2] = pixel2[2];
            //    //                    removeLine[0] = pixel2;
            //    //                    break;
            //    //                }
            //    //            }
            //    //        }
            //    //    }
            //    //}
            //    if (removeLine[0] == null)
            //    {
            //        newLine[0] = new Point[3];
            //        newLine[0][0] = pixel1[0];
            //        newLine[0][1] = pixel1[1];
            //        newLine[0][2] = pixel1[2];
            //    }
            //    else
            //    {
            //        for (int i = 0; (i < 2) && (removeLine[i] != null); i++)
            //        {
            //            provincePixels.Remove(removeLine[i]);
            //        }
            //    }
            //    for (int i = 0; (i < 2) && (newLine[i] != null); i++)
            //    {
            //        provincePixels.Add(newLine[i]);
            //    }
            //}
            List<BorderLine> lineToAdd = new List<BorderLine>(this._mProvincesLines[complementingProvince].Count);
            HashSet<BorderLine> lineToRemove = new HashSet<BorderLine>(this._mProvincesLines[complementingProvince].Count);
            foreach (BorderLine line1 in this._mProvincesLines[complementingProvince])
            {
                bool found = false;
                foreach (BorderLine line2 in provinceLines)
                {
                    BorderLine[] newLines = line1.Exclude(line2);
                    if (newLines != null)
                    {
                        if (newLines.Any())
                        {
                            if (lineToAdd.Count > 0)
                            {      
                                for (int i = 0; i < lineToAdd.Count; i++)
                                {
                                    bool[] outerInclude = new bool[newLines.Count()];
                                    bool innerInclude = false;
                                    for (int j = 0; j < newLines.Count<BorderLine>(); j++)
                                    {
                                        BorderLine temp;
                                        if ((lineToAdd[i] != newLines[j]) && ((temp = lineToAdd[i].Intersect(newLines[j])) != BorderLine.EmptyLine))
                                        {
                                            lineToAdd.Add(temp);
                                            innerInclude = true;
                                            outerInclude[j] = true;
                                        }
                                        else
                                        {
                                            outerInclude[j] = false;
                                        }
                                    }
                                    if (innerInclude)
                                    {
                                        lineToAdd.Remove(lineToAdd[i]);
                                        for (int j = 0; j < newLines.Count(); j++)
                                        {
                                            if (outerInclude[j])
                                            {
                                                lineToAdd.Add(newLines[j]);
                                            }
                                        }
                                    }
                                }
                                if (newLines.Any())
                                {
                                    lineToAdd.AddRange(newLines);
                                }
                            }
                            else
                            {
                                lineToAdd.AddRange(newLines);
                            }
                        }
                        lineToRemove.Add(line2);
                        found = true;
                    }
                }
                if (!found)
                {
                    lineToAdd.Add(line1);
                }
            }
            provinceLines = provinceLines.Except(lineToRemove).ToHashSet();
            provinceLines.UnionWith(lineToAdd);
        }

        public GraphicsPath ProcessVirtualProvince(HashSet<BorderLine> lines)
        {
            HashSet<BorderLine> result = new HashSet<BorderLine>(lines);
            return this.BuildPath(result);

        }

        #endregion

        #region Line Tracing

        private void ProcessProvince(HashSet<BorderPoint[]> lines, HashSet<BorderLine> result)
        {
            List<BorderPoint> vRightPoints = new List<BorderPoint>(lines.Count / 2);
            List<BorderPoint> vLeftPoints = new List<BorderPoint>(lines.Count / 2);
            List<BorderPoint> hTopPoints = new List<BorderPoint>(lines.Count / 2);
            List<BorderPoint> hBottomPoints = new List<BorderPoint>(lines.Count / 2);
            foreach (BorderPoint[] line in lines)
            {
                if (line[2].mX == -1)
                {
                    vRightPoints.Add(line[1]);
                    vLeftPoints.Add(line[0]);
                }
                else
                {
                    hTopPoints.Add(line[0]);
                    hBottomPoints.Add(line[1]);
                }
            }
            vRightPoints.Sort((BorderPoint p1, BorderPoint p2) =>
            {
                int val1 = p1.mX * 2160 + p1.mY;
                int val2 = p2.mX * 2160 + p2.mY;
                return val1.CompareTo(val2);
            });
            vLeftPoints.Sort((BorderPoint p1, BorderPoint p2) =>
            {
                int val1 = p1.mX * 2160 + p1.mY;
                int val2 = p2.mX * 2160 + p2.mY;
                return val1.CompareTo(val2);
            });
            hTopPoints.Sort((BorderPoint p1, BorderPoint p2) =>
            {
                int val1 = p1.mY * 5616 + p1.mX;
                int val2 = p2.mY * 5616 + p2.mX;
                return val1.CompareTo(val2);
            });
            hBottomPoints.Sort((BorderPoint p1, BorderPoint p2) =>
            {
                int val1 = p1.mY * 5616 + p1.mX;
                int val2 = p2.mY * 5616 + p2.mX;
                return val1.CompareTo(val2);
            });

            this.TraceHTopLines(hTopPoints, result);
            this.TraceHBottomLines(hBottomPoints, result);
            this.TraceVLeftLines(vLeftPoints, result);
            this.TraceVRightLines(vRightPoints, result);
            

        }

        private void TraceVLeftLines(List<BorderPoint> points, HashSet<BorderLine> result)
        {
            int index=0, y1, y2, x, start;
            while (points.Count > index)
            {
                start = index;
                x = points[index].mX;
                y1 = points[index].mY;
                y2 = y1;
                do
                {
                    y1 = y2;
                    index++;
                    if (points.Count > index)
                    {
                        y2 = points[index].mY;
                    }
                    else
                    {
                        break;
                    }
                } while ((y2 - y1 == 1) && (points[index].mX == x) && (index < points.Count));
                index--;
                BorderLine line = new BorderLine(points[start], new Point(points[index].mX, points[index].mY + 1));
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        private void TraceVRightLines(List<BorderPoint> points, HashSet<BorderLine> result)
        {
            int index=0, y1, y2, x, start;
            while (points.Count > index)
            {
                start = index;
                x = points[index].mX;
                y1 = points[index].mY;
                y2 = y1;
                do
                {
                    y1 = y2;
                    index++;
                    if (points.Count > index)
                    {
                        y2 = points[index].mY;
                    }
                    else
                    {
                        break;
                    }
                } while ((y2 - y1 == 1) && (points[index].mX == x) && (index < points.Count));
                index--;
                BorderLine line = new BorderLine(new Point(points[start].mX + 1, points[start].mY), new Point(points[index].mX + 1, points[index].mY + 1));
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        private void TraceHTopLines(List<BorderPoint> points, HashSet<BorderLine> result)
        {
            int index = 0, x1, x2, y, start;

            while (points.Count > index)
            {
                start = index;
                y = points[index].mY;
                x1 = points[index].mX;
                x2 = x1;
                do
                {
                    x1 = x2;
                    index++;
                    if (points.Count > index)
                    {
                        x2 = points[index].mX;
                    }
                    else
                    {
                        break;
                    }
                } while ((x2 - x1 == 1) && (points[index].mY == y) && (index < points.Count));
                index--;
                BorderLine line = new BorderLine(points[start], new Point(points[index].mX + 1, points[index].mY));
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        private void TraceHBottomLines(List<BorderPoint> points, HashSet<BorderLine> result)
        {
            int index=0, x1, x2, y, start;
            while (points.Count > index)
            {
                start = index;
                y = points[index].mY;
                x1 = points[index].mX;
                x2 = x1;
                do
                {
                    x1 = x2;
                    index++;
                    if (points.Count > index)
                    {
                        x2 = points[index].mX;
                    }
                    else
                    {
                        break;
                    }
                } while ((x2 - x1 == 1) && (points[index].mY == y) && (index < points.Count));
                index--;
                BorderLine line = new BorderLine(new Point(points[start].mX, points[start].mY + 1), new Point(points[index].mX + 1, points[index].mY + 1));
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        #endregion
    }
}
