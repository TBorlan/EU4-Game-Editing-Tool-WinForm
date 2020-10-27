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
                return this._mDisplayRenderingEngine._mMinMargins;
            }
            set
            {
                this._mDisplayRenderingEngine._mMinMargins = value;
                if (this.cMapDisplay.mOriginalBitmap != null)
                {
                    this._mDisplayRenderingEngine.ProccessSize(this.cMapDisplay, new EventArgs());
                }
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
                    this.InitializeEngine();
                }
            }
        }

        public DisplayPanel(Bitmap map)
        {
            InitializeComponent();
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
            this.mImage = map;         
        }

        public DisplayPanel()
        {
            InitializeComponent();
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
        }

        private void UpdateScollBars(object engine, int[] sizeData)
        {
            this.cHScrollBar.Maximum = sizeData[0];
            this.cHScrollBar.LargeChange = sizeData[1];
            this.cHScrollBar.SmallChange = sizeData[1] / 10;
            this.cVScrollBar.Maximum = sizeData[2];
            this.cVScrollBar.LargeChange = sizeData[3];
            this.cVScrollBar.SmallChange = sizeData[3] / 10;
            this._mDisplayRenderingEngine.ProccessScrollValue(this.cHScrollBar, new EventArgs());
            this._mDisplayRenderingEngine.ProccessScrollValue(this.cVScrollBar, new EventArgs());
        }

        private void Callback_MapDisplay_Pan(object mapDisplay, Point offset)
        {
            if ((this.cHScrollBar.Value + offset.X <= this.cHScrollBar.Maximum - this.cHScrollBar.LargeChange) && (this.cHScrollBar.Value + offset.X >= this.cHScrollBar.Minimum))
            {
                this.cHScrollBar.Value += offset.X;
            }
            if ((this.cVScrollBar.Value + offset.Y <= this.cVScrollBar.Maximum - this.cVScrollBar.LargeChange) && (this.cVScrollBar.Value + offset.Y >= this.cVScrollBar.Minimum))
            {
                this.cVScrollBar.Value += offset.Y;
            }
        }

        private void Enable()
        {
            this.Enabled = true;
            this.Visible = true;
            foreach(Control control in this.Controls)
            {
                control.Enabled = true;
                control.Visible = true;
            }
        }
        private void MapDisposing(object obj, EventArgs args)
        {

        }

        private void InitializeEngine()
        {
            this.cHScrollBar.Scroll += new ScrollEventHandler(this._mDisplayRenderingEngine.ProcessScroll);
            this.cHScrollBar.ValueChanged += new EventHandler(this._mDisplayRenderingEngine.ProccessScrollValue);
            this.cVScrollBar.Scroll += new ScrollEventHandler(this._mDisplayRenderingEngine.ProcessScroll);
            this.cVScrollBar.ValueChanged += new EventHandler(this._mDisplayRenderingEngine.ProccessScrollValue);
            this.cMapDisplay.SizeChanged += new EventHandler(this._mDisplayRenderingEngine.ProccessSize);
            this._mDisplayRenderingEngine.ScrollBarChange += new EventHandler<int[]>(this.UpdateScollBars);
            this.cMapDisplay.Pan += new EventHandler<Point>(this.Callback_MapDisplay_Pan);
            this.cMapDisplay.Paint += new PaintEventHandler(this._mDisplayRenderingEngine.Render);
            this._mDisplayRenderingEngine.Initialize(1.0f, new Size(100, 100), this.cMapDisplay);
            this.cMapDisplay.Disposed += new EventHandler(this.MapDisposing);
            this.Enable();

        }
    }
}
