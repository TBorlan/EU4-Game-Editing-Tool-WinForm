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
                    Point[] colorLine = new Point[2];
                    colorLine[0] = new Point(row, col);
                    do
                    {
                        col++;
                    } while (col < mBitmapData.Width && color == mPixelColors[row,col]);

                    col--;
                    colorLine[1] = new Point(row, col);
                    HashSet<Point[]> Lines = provinces[color];
                    Lines.Add(colorLine);
                }
            }
            for (col = 0; col < mBitmapData.Width; col++)
            {
                for (row = 0; row < mBitmapData.Height; row++)
                {
                    Color color = mPixelColors[row,col];
                    Point[] colorLine = new Point[2];
                    colorLine[0] = new Point(row, col);
                    do
                    {
                        row++;
                    } while (row < mBitmapData.Height && color == mPixelColors[row, col]);

                    row--;
                    colorLine[1] = new Point(row, col);
                    HashSet<Point[]> Lines = provinces[color];
                    Lines.Add(colorLine);
                }
            }
            foreach(KeyValuePair<Color, HashSet<Point[]>> keyValue in provinces)
            {
                mProvincesLines.Add(keyValue.Key, new HashSet<Point[]>());
                ProccessProvince(keyValue.Value,keyValue.Key);
                int i = 0;
                foreach(Point[] line in mProvincesLines[keyValue.Key])
                {
                    path.AddLine(line[0], line[1]);
                    i++;
                    if (i < 20)
                    {
                        break;
                    }
                }
            }
            return path;
        }

        private static void ProccessProvince(HashSet<Point[]> lines,Color key)
        {
            List<Point> vPoints = new List<Point>(lines.Count / 2);
            List<Point> hPoints = new List<Point>(lines.Count / 2);

            foreach (Point[] line in lines)
            {
                if(line[0].X == line[1].X)
                {
                    hPoints.AddRange(line);
                }
                else
                {
                    vPoints.AddRange(line);
                }
            }
            vPoints.Sort((Point p1, Point p2) =>
            {
                int val1 = p1.Y * 2160 + p1.X;
                int val2 = p2.Y * 2160 + p2.X;
                return val1.CompareTo(val2);
            });
            hPoints.Sort((Point p1, Point p2) =>
            {
                int val1 = p1.X * 5616 + p1.Y;
                int val2 = p2.X * 5616 + p2.Y;
                return val1.CompareTo(val2);
            });
            TraceHLines(hPoints, key);
            TraceVLines(vPoints, key);
        }
        private static int ComparePoints(Point p1, Point p2)
        {
            int val1 = p1.X * 2500 + p1.Y;
            int val2 = p2.X * 2500 + p2.Y;
            return val1.CompareTo(val2);
        }
        private static void TraceVLines(List<Point> points, Color key)
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
                        y2 = points[++index].X;
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
        private static void TraceHLines(List<Point> points, Color key)
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
    }
}
