using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_Game_Editing_Tool_WinForm
{
    public partial class DisplayPanel : UserControl
    {
        private DisplayRenderingEngine _mDisplayRenderingEngine;

        public SelectionManager _mSelectionManager;

        /// <summary>
        /// Get or set the margins used when displaying the <seealso cref="mImage"/>.
        /// </summary>
        /// <remarks>
        /// Value returned may be different than the one set, depending on the size of <seealso cref="DisplayPanel.mImage"/>
        /// or <seealso cref="DisplayPanel.mScale"/> value.
        /// </remarks>
        public Size mMargins
        {
            get
            {
                return this._mDisplayRenderingEngine.mMargins;
            }
            set
            {
                this._mDisplayRenderingEngine.Initialize(value);
            }
        }
        /// <summary>
        /// Get or set the zoom factor of the <seealso cref="mImage"/>.
        /// </summary>
        /// <remarks>
        /// You can also change this value using the mouse wheel instead of programmatically changing it .
        /// </remarks>
        public float mScale
        {
            get
            {
                return this._mDisplayRenderingEngine.mScale;
            }
            set
            {
                this._mDisplayRenderingEngine.Initialize(value);
            }
        }
        /// <summary>
        /// Get or set the bitmap currently displayed.
        /// </summary>
        /// <remarks>
        /// When assigning the bitmap, an internal copy is created so the bitmap assigned can be safely disposed.
        /// </remarks>
        public Bitmap mImage
        {
            get
            {
                // Return reference
                return this.cMapDisplay.mOriginalBitmap;
            }
            set
            {
                // If null perform nothing
                if (value != null)
                {
                    // Dispose the old reference
                    this.cMapDisplay.mOriginalBitmap?.Dispose();
                    this.cMapDisplay.mOriginalBitmap = new Bitmap((Image)value);
                    // Enable the controls
                    this.EnablePanel(this.cMapDisplay.mOriginalBitmap);
                }
            }
        }

        public DisplayPanel(Bitmap map)
        {
            InitializeComponent();
            this.Enabled = false;
            this.Visible = false;
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
            this._mDisplayRenderingEngine.Bind(this);
            this._mDisplayRenderingEngine.ResumeRendering();
            this.mImage = map;
        }

        public DisplayPanel()
        {
            InitializeComponent();
            this.Enabled = false;
            this.Visible = false;
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
            this._mDisplayRenderingEngine.Bind(this);
            this._mDisplayRenderingEngine.SuspendRendering();
        }

        private async Task<ProvinceBorders> GetProvinceBordersAsync(Bitmap bitmap)
        {
            ProvinceBorders provinceBorders = await Task.Run<ProvinceBorders>(() =>
            {
                ProvinceBorders borders = ProvinceBorders.GetProvinceBorders(bitmap, 3000);
                return borders;
            });
            return provinceBorders;
        }

        private async Task EnablePanel(Bitmap bitmap)
        {
            this._mSelectionManager ??= new SelectionManager(await this.GetProvinceBordersAsync(bitmap));
            this._mDisplayRenderingEngine.Initialize(1.0f, new Size(100, 100));
            this.Enabled = true;
            this.Visible = true;
            this._mDisplayRenderingEngine.ResumeRendering();
        }
    }
}
