using System.Collections.Generic;
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

        private List<Color> _mActiveProvinces;

        private HashSet<BorderLine> _mActivePixels = new HashSet<BorderLine>();

        private ProvinceBorders _mProvinceBorders;

        public GraphicsPath mActivePath
        {
            get
            {
                if (this._mActivePath != null)
                {
                    return (GraphicsPath)this._mActivePath.Clone();
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
                this._mProvinceBorders.ComplementVirtualProvince(ref this._mActivePixels, color);
            }
            else
            {
                this._mActiveProvinces.Remove(color);
                this._mProvinceBorders.ComplementVirtualProvince(ref this._mActivePixels, color);
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
