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

        private static byte[] mRGBData;

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
            mRGBData = rgbValues;
            int col, row;
            GraphicsPath path = new GraphicsPath();
            Dictionary<Color, HashSet<Point[]>> provinces = new Dictionary<Color, HashSet<Point[]>>(3661);
            for (row = 0; row < mBitmapData.Height; row++)
            {
                for (col = 0; col < mBitmapData.Width; col++)
                {
                    Color color = GetRGB(row, col);
                    if (!provinces.ContainsKey(color))
                    {
                        provinces.Add(color, new HashSet<Point[]>());
                    }
                    Point[] colorLine = new Point[2];
                    colorLine[0] = new Point(row, col);
                    do
                    {
                        col++;
                    } while (color == GetRGB(row, col) && col < mBitmapData.Width);

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
                    Color color = GetRGB(row, col);
                    Point[] colorLine = new Point[2];
                    colorLine[0] = new Point(row, col);
                    do
                    {
                        row++;
                    } while (color == GetRGB(row, col) && row < mBitmapData.Height);

                    row--;
                    colorLine[1] = new Point(row, col);
                    HashSet<Point[]> Lines = provinces[color];
                    Lines.Add(colorLine);
                }
            }
            return path;
        }
        
        private static int Index2Row(int index)
        {

            return index / Math.Abs(mBitmapData.Stride);
        }

        private static int Positon2Index(int row, int col)
        {
            switch (row)
            {
                case int x when row < 0:
                    row = 0;
                    break;
                case int x when row > mBitmap.Height - 1:
                    row = mBitmap.Height - 1;
                    break;
                default:
                    break;
            }
            switch (col)
            {
                case int x when col < 0:
                    col = 0;
                    break;
                case int x when col > mBitmap.Width - 1:
                    col = mBitmap.Width - 1;
                    break;
                default:
                    break;
            }
            return row * mBitmapData.Stride + col * 3;
        }

        private static int Index2Col(int index)
        {
            return (index / 3) % mBitmap.Width;
        }

        private static Color GetRGB(int row, int col)
        {
            int index = Positon2Index(row, col);
            Color color = Color.FromArgb(mRGBData[index],
                                         mRGBData[index + 1],
                                         mRGBData[index + 2]);
            return color;
        }
    }
}
