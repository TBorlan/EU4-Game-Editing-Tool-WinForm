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

        private static Dictionary<Color, Dictionary<int, int[]>[]> mProvincesLines;

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
            mProvincesLines = new Dictionary<Color, Dictionary<int, int[]>[]>(3661);
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

            }
            return path;
        }
        private static void ProccessProvince(HashSet<Point[]> lines)
        {
            Dictionary<int, int[]>[] province = new Dictionary<int, int[]>[];

            foreach (Point[] line in lines)
            {
                if(line[0].X == line[1].X)
                {
                    ProcessHLine(line);
                }
                else
                {
                    ProcessVLine(line);
                }
            }
        }
        private static void ProcessHLine(Point[] line)
        {

        }
        private static void ProcessVLine(Point[] line)
        {

        }
    }
}
