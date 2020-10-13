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

        private Size _mMargins;

        public Size mMargins
        {
            get
            {
                return this._mMargins;
            }
            set
            {
                // Call some update on render engine
            }
        }

        public DisplayPanel(Bitmap map)
        {
            InitializeComponent();
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
            this.cMapDisplay.mOriginalBitmap = map;
            
        }

        public DisplayPanel()
        {
            InitializeComponent();
            this._mDisplayRenderingEngine = new DisplayRenderingEngine();
        }

        private void UpdateScollBars(object engine, int[] sizeData)
        {

        }

        private void Callback_MapDisplay_Pan(object mapDisplay, Point offset)
        {

        }

        private void InitializeEngine()
        {
            this.cHScrollBar.Scroll += new ScrollEventHandler(this._mDisplayRenderingEngine.ProcessScroll);
            this.cVScrollBar.Scroll += new ScrollEventHandler(this._mDisplayRenderingEngine.ProcessScroll);
            this.SizeChanged += new EventHandler(this._mDisplayRenderingEngine.ProccessSize);
            this._mDisplayRenderingEngine.ScrollBarChange += new EventHandler<int[]>(this.UpdateScollBars);
            this.cMapDisplay.Pan += new EventHandler<Point>(this.Callback_MapDisplay_Pan);
        }
    }
}
