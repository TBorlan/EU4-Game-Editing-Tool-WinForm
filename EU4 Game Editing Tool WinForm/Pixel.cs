using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace EU4_Game_Editing_Tool_WinForm
{
    class Pixel : IEqualityComparer<Pixel>
    {
        public Color mColor { get; set; }
        public Point mPosition { get; set; }
        public Pixel(int x, int y)
        {
            this.mColor = new Color();
            this.mPosition = new Point(x, y);
        }
        bool IEqualityComparer<Pixel>.Equals(Pixel x, Pixel y)
        {
            return (x.mColor == y.mColor) && (x.mPosition == y.mPosition);
        }

        int IEqualityComparer<Pixel>.GetHashCode(Pixel obj)
        {
           return mPosition.GetHashCode() ^ mColor.GetHashCode();
        }
    }
}
