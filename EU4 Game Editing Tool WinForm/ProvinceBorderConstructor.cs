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
    static class ProvinceBorderConstructor
    {

        private static BitmapData mBitmapData;

        private static Bitmap mBitmap;

        private static Color[,] mPixelColors;

        private static Dictionary<Color, HashSet<Point[]>> mProvincesLines;

        public static GraphicsPath GenerateBordersPath(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            mBitmapData = bmpData;
            mBitmap = bitmap;
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            int col, row;
            mPixelColors = new Color[bitmap.Height,bitmap.Width];
            for (int i = 0; i < bytes; i += 3)
            {
                row = (i / 3) / bitmap.Width;
                col = (i / 3) % bitmap.Width;
                mPixelColors[row,col] = Color.FromArgb(rgbValues[i], rgbValues[i + 1], rgbValues[i + 2]);
            }
            bitmap.UnlockBits(bmpData);
            GraphicsPath path = new GraphicsPath();
            Dictionary<Color, HashSet<Point[]>> provinces = new Dictionary<Color, HashSet<Point[]>>(3661);
            mProvincesLines = new Dictionary<Color, HashSet<Point[]>>(3661);
            for (row = 0; row < mBitmapData.Height; row++)
            {
                for (col = 0; col < mBitmapData.Width; col++)
                {
                    Color color = mPixelColors[row,col];
                    if (!provinces.ContainsKey(color))
                    {
                        provinces.Add(color, new HashSet<Point[]>());
                    }
                    Point[] colorLine = new Point[3];
                    colorLine[0] = new Point(col, row);
                    do
                    {
                        col++;
                    } while (col < mBitmapData.Width && color == mPixelColors[row,col]);

                    col--;
                    colorLine[1] = new Point(col, row);
                    colorLine[2] = new Point(-1, 0); // horizontal line
                    HashSet<Point[]> Lines = provinces[color];
                    Lines.Add(colorLine);
                }
            }
            for (col = 0; col < mBitmapData.Width; col++)
            {
                for (row = 0; row < mBitmapData.Height; row++)
                {
                    Color color = mPixelColors[row,col];
                    Point[] colorLine = new Point[3];
                    colorLine[0] = new Point(col, row);
                    do
                    {
                        row++;
                    } while (row < mBitmapData.Height && color == mPixelColors[row, col]);

                    row--;
                    colorLine[1] = new Point(col, row);
                    colorLine[2] = new Point(-2, 0); // vertical line
                    HashSet<Point[]> Lines = provinces[color];
                    Lines.Add(colorLine);
                }
            }
            foreach(KeyValuePair<Color, HashSet<Point[]>> keyValue in provinces)
            {
                mProvincesLines.Add(keyValue.Key, new HashSet<Point[]>());
                ProccessProvince(keyValue.Value,keyValue.Key);
            }
            int k = 0;
            foreach(HashSet<Point[]> provinceLines in mProvincesLines.Values)
            {
                Point[] points = provinceLines.First<Point[]>();
                path.AddLine(points[0], points[1]);
                while (provinceLines.Count > 0)
                {
                    foreach(Point[] line in provinceLines)
                    {
                        if(path.GetLastPoint() == (PointF)line[0])
                        {
                            path.AddLine(line[0],line[1]);
                            provinceLines.Remove(line);
                            break;
                        }
                    }
                }
                k++;
                if(k > 1)
                {
                    return path;
                }
            }
            return path;
        }

        private static void ProccessProvince(HashSet<Point[]> lines,Color key)
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
            TraceHTopLines(hTopPoints, key);
            TraceHBottomLines(hBottomPoints, key);
            TraceVLeftLines(vLeftPoints, key);
            TraceVRightLines(vRightPoints, key);
        }

        private static void TraceVLeftLines(List<Point> points, Color key)
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
                mProvincesLines[key].Add(line);
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

        private static void TraceVRightLines(List<Point> points, Color key)
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
                line[1] = new Point(points[index].X, points[index].Y + 1);
                mProvincesLines[key].Add(line);
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

        private static void TraceHTopLines(List<Point> points, Color key)
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
                mProvincesLines[key].Add(line);
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
        private static void TraceHBottomLines(List<Point> points, Color key)
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
                line[0] = new Point(points[0].X, points[index].Y + 1);
                line[1] = new Point(points[index].X + 1, points[index].Y + 1);
                mProvincesLines[key].Add(line);
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
