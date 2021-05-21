using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using EU4GET_WF.ImageRendering.Border;

namespace EU4GET_WF.ImageRendering.Logic
{
    public class SelectionManager
    {
        /// <summary>
        /// Raises when <see cref="mActivePath"/> is set.
        /// </summary>
        public event EventHandler PathUpdate; 

        /// <summary>
        /// Creates a new instance of <see cref="SelectionManager"/>.
        /// </summary>
        /// <param name="provinceBorders">Provides <see cref="GraphicsPath"/> to the instance.</param>
        public SelectionManager(ProvinceBorders provinceBorders)
        {
            this._mActiveProvinces = new List<Color>(5);

            this._mProvinceBorders = provinceBorders;
        }

        private GraphicsPath _mSelectionPath;

        private GraphicsPath _mActivePath;

        private GraphicsPath _mProvincesPaths = null;

        private bool _mToggled = false;

        protected readonly List<Color> _mActiveProvinces;

        protected HashSet<BorderLine> _mActivePixels = new HashSet<BorderLine>();

        private readonly ProvinceBorders _mProvinceBorders;

        /// <summary>
        /// Gets the <see cref="GraphicsPath"/> representing the outline of the shapes selected.
        /// </summary>
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
                if (this._mToggled)
                {
                    if (value != null)
                    {
                        this._mActivePath.AddPath(this._mProvincesPaths, false);
                    }
                    else
                    {
                        this._mActivePath = (GraphicsPath) this._mProvincesPaths.Clone();
                    }
                }

                this.PathUpdate?.Invoke(this, new EventArgs());
            }
        }

        public async void Select(Color color)
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

            this._mSelectionPath = this._mActiveProvinces.Count != 0
                ? await this._mProvinceBorders.ProcessVirtualProvinceAsync(this._mActivePixels)
                : null;
            this.mActivePath = this._mSelectionPath;
        }

        /// <summary>
        /// Appends or remove the global outline <see cref="GraphicsPath"/> of the provinces. 
        /// </summary>
        /// <remarks>This function shouldn't be awaited.</remarks>
        /// <returns></returns>
        public async Task ToggleProvinceBorderPaths()
        {
            if (this._mProvincesPaths == null)
            {
                this._mProvincesPaths = await this._mProvinceBorders.GetAllProvinceBordersAsync();
            }
            this._mToggled = !this._mToggled;
            this.mActivePath = _mSelectionPath;
        }
    }
}
