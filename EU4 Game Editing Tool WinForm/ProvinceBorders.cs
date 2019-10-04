using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


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

        private Color[,] mPixelColors;

        private Dictionary<Color, HashSet<Point[]>> mProvincesLines;

        private void GenerateBordersPaths()
        {

            Rectangle rect = new Rectangle(0, 0, this.mBitmap.Width, this.mBitmap.Height);
            BitmapData bmpData = this.mBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, this.mBitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * this.mBitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            int col, row;
            mPixelColors = new Color[this.mBitmap.Height, this.mBitmap.Width];
            for (int i = 0; i < bytes; i += 3)
            {
                row = (i / 3) / this.mBitmap.Width;
                col = (i / 3) % this.mBitmap.Width;
                mPixelColors[row, col] = Color.FromArgb(rgbValues[i + 2], rgbValues[i + 1], rgbValues[i]);
            }
            this.mBitmap.UnlockBits(bmpData);
            bmpData = null;
            GraphicsPath path = new GraphicsPath();
            Dictionary<Color, HashSet<Point[]>> provinces = new Dictionary<Color, HashSet<Point[]>>(3661);
            for (row = 0; row < this.mBitmap.Height; row++)
            {
                for (col = 0; col < this.mBitmap.Width; col++)
                {
                    Color color = mPixelColors[row, col];
                    if (!provinces.ContainsKey(color))
                    {
                        provinces.Add(color, new HashSet<Point[]>());
                    }
                    Point[] colorLine = new Point[3];
                    colorLine[0] = new Point(col, row);
                    do
                    {
                        col++;
                    } while (col < this.mBitmap.Width && color == mPixelColors[row, col]);

                    col--;
                    colorLine[1] = new Point(col, row);
                    colorLine[2] = new Point(-1, 0); // horizontal line
                    HashSet<Point[]> Lines = provinces[color];
                    Lines.Add(colorLine);
                }
            }
            for (col = 0; col < this.mBitmap.Width; col++)
            {
                for (row = 0; row < this.mBitmap.Height; row++)
                {
                    Color color = mPixelColors[row, col];
                    Point[] colorLine = new Point[3];
                    colorLine[0] = new Point(col, row);
                    do
                    {
                        row++;
                    } while (row < this.mBitmap.Height && color == mPixelColors[row, col]);

                    row--;
                    colorLine[1] = new Point(col, row);
                    colorLine[2] = new Point(-2, 0); // vertical line
                    HashSet<Point[]> Lines = provinces[color];
                    Lines.Add(colorLine);
                }
            }
            this.mPixelColors = null;
            foreach (KeyValuePair<Color, HashSet<Point[]>> keyValue in provinces)
            {
                this.mProvincesLines.Add(keyValue.Key, new HashSet<Point[]>());
                this.ProccessProvince(keyValue.Value, keyValue.Key);
            }
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
            int index, y1, y2, x;
            while (points.Count > 0)
            {
                index = 0;
                x = points[0].X;
                y1 = points[0].Y;
                y2 = y1;
                do
                {
                    y1 = y2;
                    try
                    {
                        y2 = points[++index].Y;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        break;
                    }
                } while ((y2 - y1 == 1) && (points[index].X == x) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = points[0];
                line[1] = new Point(points[index].X, points[index].Y + 1);
                this.mProvincesLines[key].Add(line);
                if (index > 0)
                {
                    points.RemoveRange(0, index);
                }
                else
                {
                    points.RemoveAt(0);
                }
            }
        }

        private void TraceVRightLines(List<Point> points, Color key)
        {
            int index, y1, y2, x;
            while (points.Count > 0)
            {
                index = 0;
                x = points[0].X;
                y1 = points[0].Y;
                y2 = y1;
                do
                {
                    y1 = y2;
                    try
                    {
                        y2 = points[++index].Y;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        break;
                    }
                } while ((y2 - y1 == 1) && (points[index].X == x) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = new Point(points[0].X + 1, points[0].Y);
                line[1] = new Point(points[index].X + 1, points[index].Y + 1);
                this.mProvincesLines[key].Add(line);
                if (index > 0)
                {
                    points.RemoveRange(0, index);
                }
                else
                {
                    points.RemoveAt(0);
                }
            }
        }

        private void TraceHTopLines(List<Point> points, Color key)
        {
            int index, x1, x2, y;
            while (points.Count > 0)
            {
                index = 0;
                y = points[0].Y;
                x1 = points[0].X;
                x2 = x1;
                do
                {
                    x1 = x2;
                    try
                    {
                        x2 = points[++index].X;
                    }
                    catch(ArgumentOutOfRangeException ex)
                    {
                        break;
                    }
                } while ((x2 - x1 == 1) && (points[index].Y == y) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = points[0];
                line[1] = new Point(points[index].X + 1, points[index].Y);
                this.mProvincesLines[key].Add(line);
                if (index > 0)
                {
                    points.RemoveRange(0, index);
                }
                else
                {
                    points.RemoveAt(0);
                }
            }
        }
        private void TraceHBottomLines(List<Point> points, Color key)
        {
            int index, x1, x2, y;
            while (points.Count > 0)
            {
                index = 0;
                y = points[0].Y;
                x1 = points[0].X;
                x2 = x1;
                do
                {
                    x1 = x2;
                    try
                    {
                        x2 = points[++index].X;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        break;
                    }
                } while ((x2 - x1 == 1) && (points[index].Y == y) && (index < points.Count));
                index--;
                Point[] line = new Point[2];
                line[0] = new Point(points[0].X, points[0].Y + 1);
                line[1] = new Point(points[index].X + 1, points[index].Y + 1);
                this.mProvincesLines[key].Add(line);
                if (index > 0)
                {
                    points.RemoveRange(0, index);
                }
                else
                {
                    points.RemoveAt(0);
                }
            }
        }
    }
}
