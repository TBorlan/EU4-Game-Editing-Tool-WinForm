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
    /// <summary>
    /// Represents an abstraction for shapes present in the provinces.bmp file and their outlines.
    /// </summary>
    public class ProvinceBorders
    {
        #region Fields,Properties&Constructors 

        private ProvinceBorders(int provinceCount)
        {
            this._mProvinceCount = provinceCount;
            //mProvincesLines = new Dictionary<Color, HashSet<Point[]>>(this.mProvinceCount);
        }

        /// <summary>
        /// Holds reference to singleton instance
        /// </summary>
        private static ProvinceBorders _mInstance;

        /// <summary>
        /// Maps every province, defined by <see cref="Color"/>, to a collection of <see cref="BorderLine"/> that defines an outline.
        /// </summary>
        private Dictionary<Color, HashSet<BorderLine>> _mProvincesLines;

        private readonly int _mProvinceCount;

        private Dictionary<Color, GraphicsPath> _mProvincePaths;

        #endregion

        #region Instance Generation

        /// <summary>
        /// Get or generate the singleton instance of <see cref="ProvinceBorders"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap memory stream of the provinces.bmp file.</param>
        /// <param name="provinceCount">Number of shapes present in the file.</param>
        /// <returns>The singleton instance of <see cref="ProvinceBorders"/>.</returns>
        public static ProvinceBorders GetProvinceBorders(Bitmap bitmap, int provinceCount)
        {
            if (_mInstance == null)
            {
                _mInstance = new ProvinceBorders(provinceCount);
                // Generate an intermediate bitmap that has a width dividable by 4 
                // This is required so that stride returns the actual number of bytes per row
                Bitmap internalBitmap = new Bitmap(bitmap.Width + (4 - bitmap.Width % 4), bitmap.Height, PixelFormat.Format24bppRgb);
                Graphics graphics = Graphics.FromImage(internalBitmap);
                graphics.PageUnit = GraphicsUnit.Pixel;
                graphics.Clear(Color.FromArgb(255,255,255));
                graphics.DrawImageUnscaled(bitmap,0,0);
                graphics.Dispose();
                _mInstance.GenerateBordersPaths(internalBitmap);
                internalBitmap.Dispose();

            }
            return _mInstance;
        }

        private void GenerateBordersPaths(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * bitmap.Height;
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
            Dictionary<Color, HashSet<BorderPoint[]>> provincesPoints = new Dictionary<Color, HashSet<BorderPoint[]>>(this._mProvinceCount);

            object lockObj = new Object();
            //NOTE: Could traverse lines and columns in a single for
            object lockObj = new Object();
            //Traverse lines
            Parallel.For(0, height, (int prow) =>
            {
                for (int col = 0; col < width; col++)
                {
                    Color color = pixelColors[prow, col];
                    lock (lockObj)
                    {
                        if (!provincesPoints.ContainsKey(color))
                        {
                            provincesPoints.Add(color, new HashSet<BorderPoint[]>());
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
                        HashSet<BorderPoint[]> lines = provincesPoints[color];
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
                        HashSet<BorderPoint[]> lines = provincesPoints[color];
                        lines.Add(colorLine);
                    }
                }
            });
            this._mProvincesLines = new Dictionary<Color, HashSet<BorderLine>>(provincesPoints.Count);
            // Get border lines from border pixels
            object lockobj = new Object();
            Parallel.ForEach(provincesPoints, (KeyValuePair<Color, HashSet<BorderPoint[]>> keyValue) =>
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
        /// Assemble the give collection of <see cref="BorderLine"/> into a <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="lines">The collection of <see cref="BorderLine"/> that define one or more closed shapes.</param>
        /// <returns><see cref="GraphicsPath"/> made up of one or more closed shapes.</returns>
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
                //TODO: GetLastPoint allocates object inefficiently. Should move outside lambda function and use a variable inside it
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

        /// <summary>
        /// Update the <paramref name="provinceLines"/> collection so that it contains <see cref="BorderLine"/> present in it
        /// or present in the outline of a shape define by <paramref name="complementingProvince"/> color, but not in both.
        /// </summary>
        /// <remarks>If <see cref="BorderLine"/> present in both collection only partially overlap, the updated <paramref name="provinceLines"/> will
        /// contain the segments resulted after a <see cref="BorderLine.Exclude(BorderLine)"/> operation.</remarks>
        /// <param name="provinceLines">The collection to be updated.</param>
        /// <param name="complementingProvince">The color of the shape whose outline will be used in the complement operation.</param>
        public void ComplementVirtualProvince(ref HashSet<BorderLine> provinceLines, Color complementingProvince)
        {
            HashSet<BorderLine> linesToRemove = new HashSet<BorderLine>(3);
            HashSet<BorderLine> linesFound = new HashSet<BorderLine>(3);
            foreach (BorderLine lineToAddImmutable in this._mProvincesLines[complementingProvince])
            {
                BorderLine lineToAdd = lineToAddImmutable;
                bool found = false;
                linesToRemove.Clear();
                linesFound.Clear();
                BorderLine foundLine = BorderLine.EmptyLine;
                foreach (BorderLine lineExisting in provinceLines)
                {
                    //Search for any overlapping
                    if (lineExisting.IsOverlapping(lineToAdd))
                    {
                        found = true;
                        linesFound.Add(lineExisting);
                    }
                    //Search if two lines are in fact a bigger line
                    if (lineExisting.IsContinuous(lineToAdd))
                    {
                        linesToRemove.Add(lineExisting);
                        lineToAdd = lineToAdd.Concatenate(lineExisting);
                        found = true;
                    }
                }
                if (found == false)
                {
                    provinceLines.Add(lineToAdd);
                }
                else
                {
                    // Remove lines marked for removal
                    provinceLines.ExceptWith(linesToRemove);
                    // If found to overlap with only one line, it's easy then
                    if (linesFound.Count == 1)
                    {
                        provinceLines.ExceptWith(linesFound);
                        BorderLine[] lines = lineToAdd.Exclude(linesFound.First());
                        if (lines != null)
                        {
                            provinceLines.UnionWith(lines);
                        }
                    }
                    else if (linesFound.Count > 1)
                    {
                        foreach (BorderLine lineFound in linesFound)
                        {
                            lineToAdd = lineFound.Exclude(lineToAdd)[0];
                        }
                        provinceLines.ExceptWith(linesFound);
                        provinceLines.Add(lineToAdd);
                    }
                    else
                    {
                        provinceLines.Add(lineToAdd);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a closed multi-line <see cref="GraphicsPath"/> made up of
        /// <see cref="BorderLine"/> present in the <paramref name="lines"/> collection.
        /// </summary>
        /// <param name="lines">The <see cref="BorderLine"/> to be assembled in a <see cref="GraphicsPath"/>.</param>
        /// <returns></returns>
        public GraphicsPath ProcessVirtualProvince(HashSet<BorderLine> lines)
        {
            HashSet<BorderLine> result = new HashSet<BorderLine>(lines);
            return this.BuildPath(result);

        }

        /// <summary>
        /// Returns a closed multi-line <see cref="GraphicsPath"/> made up of
        /// <see cref="BorderLine"/> present in the <paramref name="lines"/> collection.
        /// </summary>
        /// <remarks>This function doesn't need to be awaited to run asynchronously, but it will return
        /// a <see cref="Task"/> instead of a <see cref="GraphicsPath"/> object.</remarks>
        /// <param name="lines">The <see cref="BorderLine"/> to be assembled in a <see cref="GraphicsPath"/>.</param>
        /// <returns></returns>
        public async Task<GraphicsPath> ProcessVirtualProvinceAsync(HashSet<BorderLine> lines)
        {
            HashSet<BorderLine> result = new HashSet<BorderLine>(lines);
            return await Task.Run(() => this.BuildPath(result));
        }

        /// <summary>
        /// Asynchronously generates a <see cref="GraphicsPath"/> representing the concatenated
        /// outlines of every shape in the bitmap file.
        /// </summary>
        /// <returns>The <see cref="GraphicsPath"/> if the function is awaited, otherwise a <see cref="Task{GraphicsPath}"/>.</returns>
        /// <remarks>This method doesn't have to be awaited to run asynchronously.</remarks>
        internal async Task<GraphicsPath> GetAllProvinceBordersAsync()
        {
            return await Task<GraphicsPath>.Run(() =>
                                                {
                                                    GraphicsPath path = new GraphicsPath();
                                                    foreach (KeyValuePair<Color, HashSet<BorderLine>> provincesLine in
                                                        this._mProvincesLines)
                                                    {
                                                        path.AddPath(this.BuildPath(provincesLine.Value), false);
                                                    }

                                                    return path;
                                                });
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
