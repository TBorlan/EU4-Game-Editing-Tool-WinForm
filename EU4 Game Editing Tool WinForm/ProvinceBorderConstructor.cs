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
        
        public static GraphicsPath GenerateBordersPath(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            mBitmapData = bmpData;
            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            int col, row;
            for (int iterator = 0; iterator < bytes; iterator += 3)
            {
                col = Index2Col(iterator);
                row = Index2Row(iterator);

                Pixel pixel = new Pixel(row,col);
                Pixel left = new Pixel();
                Pixel top = new Pixel();
                Pixel right = new Pixel();
                Pixel bottom = new Pixel();

                pixel.mColor.R = rgbValues[iterator];
                pixel.mColor.G = rgbValues[iterator + 1];
                pixel.mColor.B = rgbValues[iterator + 2];

                left.R = rgbValues[]
            }

            
        }
        private static int Index2Row(int index)
        {
            return index / Math.Abs(mBitmapData.Stride);
        }

        private static int Positon2Index(int row, int col)
        {
            return row * mBitmapData.Stride + col * 3;
        }
        private static int Index2Col(int index)
        {
            return (index / 3) % Math.Abs(mBitmapData.Stride);
        }



    }
}
