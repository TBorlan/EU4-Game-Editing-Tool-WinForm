using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using EU4GET_WF.ImageRendering.Border;

namespace EU4GET_WF.ImageRendering.Logic
{
    public class SelectionManager
    {
        public SelectionManager(ProvinceBorders provinceBorders)
        {
            this._mActiveProvinces = new List<Color>(5);

            this._mProvinceBorders = provinceBorders;
        }

        private GraphicsPath _mActivePath;

        protected readonly List<Color> _mActiveProvinces;

        protected HashSet<BorderLine> _mActivePixels = new HashSet<BorderLine>();

        private readonly ProvinceBorders _mProvinceBorders;

        public GraphicsPath mActivePath
        {
            get
            {
                return (GraphicsPath) this._mActivePath?.Clone();
            }
            private set
            {
                this._mActivePath?.Dispose();
                this._mActivePath = value;
            }
        }

        public void Select(Color color)
        {
            if (!this._mActiveProvinces.Contains(color))
            {
                this._mActiveProvinces.Add(color);
                this._mProvinceBorders.ComplementVirtualProvince(ref this._mActivePixels, color);
            }
            else
            {
                this._mActiveProvinces.Remove(color);
                this._mProvinceBorders.ComplementVirtualProvince(ref this._mActivePixels, color);
            }

            this.mActivePath = this._mActiveProvinces.Count != 0
                ? this._mProvinceBorders.ProcessVirtualProvince(this._mActivePixels)
                : null;
        }
    }
}
