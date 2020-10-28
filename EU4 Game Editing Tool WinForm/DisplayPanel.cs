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
                    this.cMapDisplay.mOriginalBitmap = new Bitmap(value);
                    this.Enabled = true;
                    this.Visible = true;
                    this._mDisplayRenderingEngine.Initialize(1.0f, new Size(100, 100));
                    this._mDisplayRenderingEngine.ResumeRendering();
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
    }
}
