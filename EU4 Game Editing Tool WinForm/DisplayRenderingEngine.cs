using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_Game_Editing_Tool_WinForm
{
    class DisplayRenderingEngine
    {
        Size _mVirtualSize;

        private Rectangle _mSelectionRectangle;

        private Rectangle _mDisplayRectangle;

        private float _mScale;

        public event EventHandler<int[]> ScrollBarChange;

        private void Initialize()
        {

        }

        public void ProcessScroll(object scrollBar, ScrollEventArgs args)
        {

        }

        private 

        private void OnScrollbarChange()
        {

            this.ScrollBarChange?.Invoke(this, )
        }

        public void ProccessSize(object panel, EventArgs args)
        {

        }

        private void Process()
    }
}
