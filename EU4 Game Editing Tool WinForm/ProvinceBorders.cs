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
    class ProvinceBorders
    {

        private ProvinceBorders(Bitmap bitmap)
        {
            mBitmap = bitmap;
            mProvincesLines = new Dictionary<Color, HashSet<Point[]>>(3661);
        }

        private static ProvinceBorders instance;

        public static ProvinceBorders GetProvinceBorders(Bitmap bitmap)
        {
            if (instance == null)
            {
                instance = new ProvinceBorders(bitmap);
                instance.GenerateBordersPaths();
            }
            return instance;
        }

        private Bitmap mBitmap;

        private Dictionary<Color, HashSet<Point[]>> mProvincesLines;

        private void GenerateBordersPaths()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Rectangle rect = new Rectangle(0, 0, this.mBitmap.Width, this.mBitmap.Height);
            BitmapData bmpData = this.mBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, this.mBitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * this.mBitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            int height = this.mBitmap.Height;
            int width = this.mBitmap.Width;
            Color[,] pixelColors = new Color[this.mBitmap.Height, this.mBitmap.Width];
;
            Parallel.For(0, bytes / 3, (int i) =>
                {
                    i *= 3;
                    int pRow = (i / 3) / width;
                    int pCol = (i / 3) % width;
                    pixelColors[pRow, pCol] = Color.FromArgb(rgbValues[i + 2], rgbValues[i + 1], rgbValues[i]);
                });

            this.mBitmap.UnlockBits(bmpData);
            bmpData = null;
            GraphicsPath path = new GraphicsPath();
            Dictionary<Color, HashSet<Point[]>> provinces = new Dictionary<Color, HashSet<Point[]>>(3661);

            object lockObj = new Object();
            Parallel.For(0, this.mBitmap.Height, (int prow) =>
            {
                for (int col = 0; col < width; col++)
                 {
                     Color color = pixelColors[prow, col];
                     lock (lockObj)
                     {
                         if (!provinces.ContainsKey(color))
                         {
                             provinces.Add(color, new HashSet<Point[]>());
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

                     colorLine[1] = new Point(col, prow);
                     colorLine[2] = new Point(-1, 0); // horizontal line
                     lock (lockObj)
                     {
                         HashSet<Point[]> Lines = provinces[color];
                         Lines.Add(colorLine);
                     }
                 }
             });
            Parallel.For(0, width, (int col) =>
            {
                for (int row = 0; row < height; row++)
                {
                    Color color = pixelColors[row, col];
                    lock (lockObj)
                    {
                        if (!provinces.ContainsKey(color))
                        {
                            provinces.Add(color, new HashSet<Point[]>());
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
                    colorLine[1] = new Point(col, row);
                    colorLine[2] = new Point(-2, 0); // vertical line
                    lock (lockObj)
                    {
                        HashSet<Point[]> Lines = provinces[color];
                        Lines.Add(colorLine);
                    }
                }
            });
            object lockobj = new Object();
            Parallel.ForEach(provinces, (KeyValuePair<Color, HashSet<Point[]>> keyValue) =>
             {
                 lock (lockobj)
                 {
                     this.mProvincesLines.Add(keyValue.Key, new HashSet<Point[]>(keyValue.Value.Count / 4));
                 }
                 this.ProccessProvince(keyValue.Value, keyValue.Key);
             });

            Debug.WriteLine("double for time - " + stopwatch.ElapsedMilliseconds.ToString());
        }

        public GraphicsPath GetProvinceBorder(Color color)
        {
            HashSet<Point[]> provinceLines = new HashSet<Point[]>(this.mProvincesLines[color]);
            GraphicsPath path = new GraphicsPath();
            int x;
            Point[] points = provinceLines.First<Point[]>();

            path.StartFigure();
            path.AddLine(points[0], points[1]);
            provinceLines.Remove(points);
            while (provinceLines.Count > 0)
            {
                x = provinceLines.Count;
                foreach (Point[] line in provinceLines)
                {
                    if (path.GetLastPoint() == (PointF)line[0])
                    {
                        path.AddLine(line[0], line[1]);
                        provinceLines.Remove(line);
                        break;
                    }
                }
                if (x == provinceLines.Count)
                {
                    path.StartFigure();
                    points = provinceLines.First<Point[]>();
                    path.AddLine(points[0], points[1]);
                    provinceLines.Remove(points);
                }
            }
            return path;
        }

        private void ProccessProvince(HashSet<Point[]> lines,Color key)
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
            this.TraceHTopLines(hTopPoints, key);
            this.TraceHBottomLines(hBottomPoints, key);
            this.TraceVLeftLines(vLeftPoints, key);
            this.TraceVRightLines(vRightPoints, key);
        }

        private void TraceVLeftLines(List<Point> points, Color key)
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
                this.mProvincesLines[key].Add(line);
                index++;
            }
        }

        private void TraceVRightLines(List<Point> points, Color key)
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
                this.mProvincesLines[key].Add(line);
                index++;
            }
        }

        private void TraceHTopLines(List<Point> points, Color key)
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
                this.mProvincesLines[key].Add(line);
                index++;
            }
        }
        private void TraceHBottomLines(List<Point> points, Color key)
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
                this.mProvincesLines[key].Add(line);
                index++;
            }
        }
    }
}
