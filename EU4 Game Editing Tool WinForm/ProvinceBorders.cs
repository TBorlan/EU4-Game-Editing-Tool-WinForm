using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


namespace EU4_Game_Editing_Tool_WinForm
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

        private Dictionary<Color, HashSet<Point[]>> _mProvincesLines;

        private readonly int _mProvinceCount;

        private Dictionary<Color, HashSet<Point[]>> _mProvincesPoints;

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
            BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            int height = bitmap.Height;
            int width = bitmap.Width;
            Color[,] pixelColors = new Color[bitmap.Height, bitmap.Width];
            // Store pixel color in matrix
            try
            {
                Parallel.For(0, bytes / 3, (int i) =>
                    {
                        i *= 3;
                        int pRow = (i / 3) / width;
                        int pCol = (i / 3) % width;
                        try
                        {
                            pixelColors[pRow, pCol] = Color.FromArgb(rgbValues[i + 2], rgbValues[i + 1], rgbValues[i]);
                        }
                        catch (Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show(i.ToString());
                        }
                    });
            }
            catch (AggregateException e)
            {
                //System.Windows.Forms.MessageBox.Show(e.InnerExceptions.ToString());
            }
            bitmap.UnlockBits(bmpData);
            // Stores vertical and horizontal border pixels of a province
            this._mProvincesPoints = new Dictionary<Color, HashSet<Point[]>>(this._mProvinceCount);

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
                             this._mProvincesPoints.Add(color, new HashSet<Point[]>());
                         }
                     }
                     Point[] colorLine = new Point[3];
                     colorLine[0] = new Point(col, prow);
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
                     colorLine[1] = new Point(col, prow);
                     colorLine[2] = new Point(-1, 0); // horizontal line
                     lock (lockObj)
                     {
                         HashSet<Point[]> Lines = this._mProvincesPoints[color];
                         Lines.Add(colorLine);
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
                            this._mProvincesPoints.Add(color, new HashSet<Point[]>());
                        }
                    }
                    Point[] colorLine = new Point[3];
                    colorLine[0] = new Point(col, row);
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
                    colorLine[1] = new Point(col, row);
                    colorLine[2] = new Point(-2, 0); // vertical line
                    lock (lockObj)
                    {
                        HashSet<Point[]> Lines = this._mProvincesPoints[color];
                        Lines.Add(colorLine);
                    }
                }
            });
            this._mProvincesLines = new Dictionary<Color, HashSet<Point[]>>(this._mProvincesPoints.Count);
            // Get border lines from border pixels
            object lockobj = new Object();
            Parallel.ForEach(this._mProvincesPoints, (KeyValuePair<Color, HashSet<Point[]>> keyValue) =>
             {
                 lock (lockobj)
                 {
                     this._mProvincesLines.Add(keyValue.Key, new HashSet<Point[]>(keyValue.Value.Count / 4));
                 }
                 this.ProccessProvince(keyValue.Value, this._mProvincesLines[keyValue.Key]);
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
            HashSet<Point[]> provinceLines = new HashSet<Point[]>(this._mProvincesLines[color]);
            if (this._mProvincePaths == null)
            {
                this._mProvincePaths = new Dictionary<Color, GraphicsPath>(this._mProvincesLines.Count);
            }
            this._mProvincePaths[color] = this.BuildPath(provinceLines);
            return (GraphicsPath)this._mProvincePaths[color].Clone();
        }

        public GraphicsPath BuildPath(HashSet<Point[]> lines)
        {
            lines = new HashSet<Point[]>(lines);
            GraphicsPath path = new GraphicsPath();
            int x;
            Point[] points = lines.First<Point[]>();

            path.StartFigure();
            path.AddLine(points[0], points[1]);
            lines.Remove(points);
            while (lines.Count > 0)
            {
                x = lines.Count;
                foreach (Point[] line in lines)
                {
                    if (path.GetLastPoint() == (PointF)line[0])
                    {
                        path.AddLine(line[0], line[1]);
                        lines.Remove(line);
                        break;
                    }
                }
                if (x == lines.Count)
                {
                    path.StartFigure();
                    points = lines.First<Point[]>();
                    path.AddLine(points[0], points[1]);
                    lines.Remove(points);
                }
            }
            return path;
        }

        public void ComplementVirtualProvince(HashSet<Point[]> provincePixels, Color complementingProvince)
        {
            foreach (Point[] pixel1 in this._mProvincesPoints[complementingProvince])
            {
                Point[][] newLine = new Point[2][];
                newLine[0] = new Point[3];
                Point[][] removeLine = new Point[2][];
                bool found = false;
                foreach (Point[] pixel2 in provincePixels)
                {
                    if (pixel1[2] == pixel2[2])
                    {

                        if ((pixel1[0] == pixel2[0]) && (pixel1[1] == pixel2[1]))
                        {
                            if (pixel2[2].X == -2)
                            {
                                removeLine[0] = pixel2;
                            }
                            removeLine[0] = pixel2;
                            newLine[0] = null;
                            break;
                        }
                        else
                        {
                            Point testPoint1 = new Point();
                            Point testPoint2 = new Point();
                            testPoint1 = pixel1[1];
                            testPoint2 = pixel2[1];
                            if (pixel2[2].X == -1)
                            {
                                testPoint1.X += 1;
                                testPoint2.X += 1;
                            }
                            else
                            {
                                testPoint1.Y += 1;
                                testPoint2.Y += 1;
                            }
                            if (testPoint2 == pixel1[0])
                            {
                                if (found)
                                {
                                    newLine[0][0] = pixel2[0];
                                    removeLine[1] = pixel2;
                                    break;
                                }
                                newLine[0][0] = pixel2[0];
                                newLine[0][1] = pixel1[1];
                                newLine[0][2] = pixel2[2];
                                removeLine[0] = pixel2;
                                found = true;
                            }
                            else if (pixel2[0] == testPoint1)
                            {
                                if (found)
                                {
                                    newLine[0][1] = pixel2[1];
                                    removeLine[1] = pixel2;
                                    break;
                                }
                                newLine[0][0] = pixel1[0];
                                newLine[0][1] = pixel2[1];
                                newLine[0][2] = pixel2[2];
                                removeLine[0] = pixel2;
                                found = true;
                            }
                            else if (pixel2[0] == pixel1[0])
                            {
                                newLine[0][0] = pixel1[1] + (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
                                newLine[0][1] = pixel2[1];
                                newLine[0][2] = pixel2[2];
                                removeLine[0] = pixel2;
                                break;
                            }
                            else if (testPoint2 == testPoint1)
                            {
                                newLine[0][0] = pixel2[0];
                                newLine[0][1] = pixel1[0] - (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
                                newLine[0][2] = pixel2[2];
                                removeLine[0] = pixel2;
                                break;
                            }
                            else if (((pixel1[0].X == pixel2[0].X) && (pixel2[2].X == -2)) || ((pixel1[0].Y == pixel2[0].Y) && (pixel2[2].X == -1)))
                            {
                                Size start = (Size)pixel1[0] - (Size)pixel2[0];
                                Size end = (Size)pixel1[1] - (Size)pixel2[1];
                                if (((start.Width + start.Height) > 0) && ((end.Width + end.Height) < 0))
                                {
                                    newLine[0][0] = pixel2[0];
                                    newLine[0][1] = pixel1[0] - (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
                                    newLine[0][2] = pixel2[2];
                                    newLine[1] = new Point[3];
                                    newLine[1][0] = pixel1[1] + (pixel2[2].X == -1 ? new Size(1, 0) : new Size(0, 1));
                                    newLine[1][1] = pixel2[1];
                                    newLine[1][2] = pixel2[2];
                                    removeLine[0] = pixel2;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (removeLine[0] == null)
                {
                    newLine[0] = new Point[3];
                    newLine[0][0] = pixel1[0];
                    newLine[0][1] = pixel1[1];
                    newLine[0][2] = pixel1[2];
                }
                else
                {
                    for (int i = 0; (i < 2) && (removeLine[i] != null); i++)
                    {
                        provincePixels.Remove(removeLine[i]);
                    }
                }
                for (int i = 0; (i < 2) && (newLine[i] != null); i++)
                {
                    provincePixels.Add(newLine[i]);
                }
            }
        }

        public GraphicsPath ProcessVirtualProvince(HashSet<Point[]> lines)
        {
            HashSet<Point[]> result = new HashSet<Point[]>(lines.Count / 4);
            this.ProccessProvince(lines, result);
            return this.BuildPath(result);

        }

        #endregion

        #region Line Tracing

        private void ProccessProvince(HashSet<Point[]> lines, HashSet<Point[]> result)
        {
            List<Point> vRightPoints = new List<Point>(lines.Count / 4);
            List<Point> vLeftPoints = new List<Point>(lines.Count / 4);
            List<Point> hTopPoints = new List<Point>(lines.Count / 4);
            List<Point> hBottomPoints = new List<Point>(lines.Count / 4);
            foreach (Point[] line in lines)
            {
                if(line[2].X == -1)
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
            vRightPoints.Sort((Point p1, Point p2) =>
            {
                int val1 = p1.X * 2160 + p1.Y;
                int val2 = p2.X * 2160 + p2.Y;
                return val1.CompareTo(val2);
            });
            vLeftPoints.Sort((Point p1, Point p2) =>
            {
                int val1 = p1.X * 2160 + p1.Y;
                int val2 = p2.X * 2160 + p2.Y;
                return val1.CompareTo(val2);
            });
            hTopPoints.Sort((Point p1, Point p2) =>
            {
                int val1 = p1.Y * 5616 + p1.X;
                int val2 = p2.Y * 5616 + p2.X;
                return val1.CompareTo(val2);
            });
            hBottomPoints.Sort((Point p1, Point p2) =>
            {
                int val1 = p1.Y * 5616 + p1.X;
                int val2 = p2.Y * 5616 + p2.X;
                return val1.CompareTo(val2);
            });
            this.TraceHTopLines(hTopPoints, result);
            this.TraceHBottomLines(hBottomPoints, result);
            this.TraceVLeftLines(vLeftPoints, result);
            this.TraceVRightLines(vRightPoints, result);
        }

        private void TraceVLeftLines(List<Point> points, HashSet<Point[]> result)
        {
            int index=0, y1, y2, x, start;
            while (points.Count > index)
            {
                start = index;
                x = points[index].X;
                y1 = points[index].Y;
                y2 = y1;
                do
                {
                    y1 = y2;
                    index++;
                    if (points.Count > index)
                    {
                        y2 = points[index].Y;
                    }
                    else
                    {
                        break;
                    }
                } while ((y2 - y1 == 1) && (points[index].X == x) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = points[start];
                line[1] = new Point(points[index].X, points[index].Y + 1);
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        private void TraceVRightLines(List<Point> points, HashSet<Point[]> result)
        {
            int index=0, y1, y2, x, start;
            while (points.Count > index)
            {
                start = index;
                x = points[index].X;
                y1 = points[index].Y;
                y2 = y1;
                do
                {
                    y1 = y2;
                    index++;
                    if (points.Count > index)
                    {
                        y2 = points[index].Y;
                    }
                    else
                    {
                        break;
                    }
                } while ((y2 - y1 == 1) && (points[index].X == x) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = new Point(points[start].X + 1, points[start].Y);
                line[1] = new Point(points[index].X + 1, points[index].Y + 1);
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        private void TraceHTopLines(List<Point> points, HashSet<Point[]> result)
        {
            int index = 0, x1, x2, y, start;
            while (points.Count > index)
            {
                start = index;
                y = points[index].Y;
                x1 = points[index].X;
                x2 = x1;
                do
                {
                    x1 = x2;
                    index++;
                    if (points.Count > index)
                    {
                        x2 = points[index].X;
                    }
                    else
                    {
                        break;
                    }
                } while ((x2 - x1 == 1) && (points[index].Y == y) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = points[start];
                line[1] = new Point(points[index].X + 1, points[index].Y);
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        private void TraceHBottomLines(List<Point> points, HashSet<Point[]> result)
        {
            int index=0, x1, x2, y, start;
            while (points.Count > index)
            {
                start = index;
                y = points[index].Y;
                x1 = points[index].X;
                x2 = x1;
                do
                {
                    x1 = x2;
                    index++;
                    if (points.Count > index)
                    {
                        x2 = points[index].X;
                    }
                    else
                    {
                        break;
                    }
                } while ((x2 - x1 == 1) && (points[index].Y == y) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = new Point(points[start].X, points[start].Y + 1);
                line[1] = new Point(points[index].X + 1, points[index].Y + 1);
                Thread.MemoryBarrier();
                result.Add(line);
                index++;
            }
        }

        #endregion
    }
}
