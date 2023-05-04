using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EU4GET_WF.ImageRendering.OptimizationTest.Ver1.Border;
using EU4GET_WF.ImageRendering.OptimizationTest.Ver1.Logic;

namespace EU4GET_WF.ImageRendering.OptimizationTest.Ver1.Control
{
    public partial class DisplayPanel : UserControl
    {
        private readonly DisplayRenderingEngine _mDisplayRenderingEngine;

        public SelectionManager _mSelectionManager;

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
                    this.cMapDisplay.mOriginalBitmap = new Bitmap((Image)value);
                    _ = this.EnablePanel(this.cMapDisplay.mOriginalBitmap);
                }
            }
        }

        public DisplayPanel(Bitmap map)
        {
            this.InitializeComponent();
            //TODO: Create pair functions to hide or show panel
            this.Enabled = false;
            this.Visible = false;
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
            this._mDisplayRenderingEngine.Bind(this);
            this._mDisplayRenderingEngine.ResumeRendering();
            this.mImage = map;
        }

        public DisplayPanel()
        {
            this.InitializeComponent();
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
