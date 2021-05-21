using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EU4GET_WF.ImageRendering.Border;
using EU4GET_WF.ImageRendering.Logic;

namespace EU4GET_WF.ImageRendering.Control
{
    public partial class DisplayPanel : UserControl
    {
        #region Fields,Properties&Constructors

        private readonly DisplayRenderingEngine _mDisplayRenderingEngine;

        public SelectionManager _mSelectionManager;

        /// <summary>
        /// Gets or sets the margins size between the <see cref="DisplayPanel"/> frame and
        /// the <see cref="mImage"/>.
        /// </summary>
        /// <remarks>Value set has a meaning of minimum margin value to be used, so returned value may be bigger than value set,
        /// depending on the scaling used to render the <see cref="mImage"/>.</remarks>
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
        /// Gets or sets the scaling used to display the <see cref="mImage"/>.
        /// </summary>
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
        /// Get or set the displayed image.
        /// </summary>
        /// <remarks>When setting a value, the object used is internally copied, so that the object can then
        /// be disposed or changed by user. If object is used to replace an existing bitmap, the old bitmap is disposed safely, so no
        /// memory leak can occur.</remarks>
        /// <value>Returns a reference to the internal bitmap used, so any change made to the returned value will
        /// reflect in the image stored.</value>
        public Bitmap mImage
        {
            get
            {
                return this.cMapDisplay.mOriginalBitmap;
            }
            set
            {
                // If null perform nothing
                if (value != null)
                {
                    this.cMapDisplay.mOriginalBitmap?.Dispose();
                    this.cMapDisplay.mOriginalBitmap = new Bitmap(value);
                    _ = this.EnablePanel(this.cMapDisplay.mOriginalBitmap);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="DisplayPanel"/> and
        /// sets the <see cref="mImage"/> value to <paramref name="map"/>.
        /// </summary>
        public DisplayPanel(Bitmap map)
        {
            this.InitializeComponent();
            this.Enabled = false;
            this.Visible = false;
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
            this._mDisplayRenderingEngine.Bind(this);
            this._mDisplayRenderingEngine.ResumeRendering();
            this.mImage = map;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DisplayPanel"/>.
        /// </summary>
        /// <remarks>The <see cref="DisplayPanel"/> will be invisible until the <see cref="mImage"/> is set.</remarks>
        public DisplayPanel()
        {
            this.InitializeComponent();
            this.Enabled = false;
            this.Visible = false;
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
            this._mDisplayRenderingEngine.Bind(this);
            this._mDisplayRenderingEngine.SuspendRendering();
        }

        #endregion

        private async Task<ProvinceBorders> GetProvinceBordersAsync(Bitmap bitmap)
        {
            ProvinceBorders provinceBorders = await Task.Run<ProvinceBorders>(() =>
           {
               ProvinceBorders borders = ProvinceBorders.GetProvinceBorders(bitmap, 3000);
               return borders;
           });
            return provinceBorders;
        }

        /// <summary>
        /// Set up the internal workings of the <see cref="DisplayPanel"/>.
        /// </summary>
        /// <param name="bitmap">Value of the <see cref="mImage"/>.</param>
        /// <remarks>Always called when setting <see cref="mImage"/>.</remarks>
        /// <returns></returns>
        private async Task EnablePanel(Bitmap bitmap)
        {
            // If no selection manager, initialize it
            // Done only once
            this._mSelectionManager ??= new SelectionManager(await this.GetProvinceBordersAsync(bitmap));
            // Register Event handler to refresh Display every time mActivePath is changed
            //BUG When changing mImage, new event handler is registered, but old one is not deleted
            this._mSelectionManager.PathUpdate += (object sender, EventArgs args) => { this.cMapDisplay.Refresh(); };
            this._mDisplayRenderingEngine.Initialize(1.0f, new Size(100, 100));
            this.Enabled = true;
            this.Visible = true;
            this._mDisplayRenderingEngine.ResumeRendering();
        }
    }   
}
