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
    class SelectionManager
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
    }
}
