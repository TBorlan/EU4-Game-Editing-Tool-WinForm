using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace EU4_Game_Editing_Tool_WinForm
{
    public class SelectionManager
    {
        public SelectionManager(ProvinceBorders provinceBorders)
        {
            this._mActiveProvinces = new List<Color>(5);

            this._mProvinceBorders = provinceBorders;
        }

        private GraphicsPath _mActivePath;

        private List<Color> _mActiveProvinces;

        private HashSet<Point[]> _mActivePixels = new HashSet<Point[]>();

        private ProvinceBorders _mProvinceBorders;

        public GraphicsPath mActivePath
        {
            get
            {
                if (_mActivePath != null)
                {
                    return (GraphicsPath)_mActivePath.Clone();
                }
                return null;
            }
            private set
            {
                if (this._mActivePath != null)
                {
                    this._mActivePath.Dispose();
                }
                this._mActivePath = value;
            }
        }

        public Point[] GetDifferenceLine(Point[] line1, Point[] line2)
        {
            Point difference;
            Point[] result = new Point[2];
            if ((line1[0].X != line2[0].X) || (line1[0].Y != line2[0].Y))
            {
                difference = new Point(line1[0].X - line2[0].X, line1[0].Y - line2[0].Y);
                if(difference.X < 0 || difference.Y < 0)
                {
                    result[1] = new Point(line2[0].X, line2[0].Y);
                }
                else
                {
                    result[1] = new Point(line1[0].X, line1[0].Y);
                }
                result[0] = new Point(result[1].X - Math.Abs(difference.X), result[1].Y - Math.Abs(difference.Y));
            }
            else
            {
                difference = new Point(line1[1].X - line2[1].X, line1[1].Y - line2[1].Y);
                if (difference.X < 0 || difference.Y < 0)
                {
                    result[0] = new Point(line1[1].X, line1[1].Y);
                }
                else
                {
                    result[0] = new Point(line2[1].X, line2[1].Y);
                }
                result[1] = new Point(result[0].X + Math.Abs(difference.X), result[0].Y + Math.Abs(difference.Y));
            }
            return result;
            
        }

        public void Select(Color color)
        {
            if (!this._mActiveProvinces.Contains(color))
            {
                this._mActiveProvinces.Add(color);
                _mProvinceBorders.ComplementVirtualProvince(this._mActivePixels, color);
            }
            else
            {
                this._mActiveProvinces.Remove(color);
                _mProvinceBorders.ComplementVirtualProvince(this._mActivePixels, color);
            }
            if (this._mActiveProvinces.Count != 0)
            {
                this.mActivePath = this._mProvinceBorders.ProcessVirtualProvince(this._mActivePixels);
            }
            else
            {
                this.mActivePath = null;
            }
        }

        private class linesComparer : IEqualityComparer<Point[]>
        {
            public bool Equals(Point[] x, Point[] y)
            {
                if (x.Length > 2 || y.Length > 2)
                {
                    return false;
                }
                if ((x[0] != y[0]) || (x[1] != y[1]))
                {
                    return false;
                }
                return true;
            }

            public int GetHashCode(Point[] obj)
            {
                return obj[0].X * 2000 + obj[0].Y * 4000;
            }
        }
    }
}
