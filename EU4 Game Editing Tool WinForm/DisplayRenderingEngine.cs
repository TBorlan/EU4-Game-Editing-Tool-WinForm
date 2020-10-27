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
        // Scaled bitmap size
        private Size _mVirtualSize;
        // MapDisplay Size
        private Size _mPhysicalSize;

        private Rectangle _mSelectionRectangle;

        private Rectangle _mDisplayRectangle;

        private Size _mMargins;

        public Size _mMinMargins;

        private float _mScale;

        public MapDisplay mMapDisplay
        {
            set;

            private get;
        }

        public event EventHandler<int[]> ScrollBarChange;

        public void Initialize(float scale, Size marginsSize, MapDisplay display)
        {
            this._mScale = scale;
            this.mMapDisplay = display;
            this._mVirtualSize = new Size((int)(this.mMapDisplay.mOriginalBitmap.Width * this._mScale), (int)(this.mMapDisplay.mOriginalBitmap.Height * this._mScale));
            this._mMinMargins = marginsSize;
            this._mPhysicalSize = display.Size;
            this.ProccessSize(this.mMapDisplay, new EventArgs());
        }

        public void ProcessScroll(object scrollBar, ScrollEventArgs args)
        {
            // Find out the selection origin
            // Find out display origin
            // Find out display size
            Point displayOrigin, selectionOrigin;
            int displayWidth, displayHeight;
            if (args.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                selectionOrigin = new Point(Math.Max((int)((args.NewValue - this._mMargins.Width) / this._mScale), 0),
                                            this._mSelectionRectangle.Y);
                displayOrigin = new Point(Math.Max(this._mMargins.Width - args.NewValue, 0),
                                          this._mDisplayRectangle.Y);
                // It means we should see some upper margins
                if (args.NewValue - 1 + ((HScrollBar)scrollBar).LargeChange + this._mMargins.Width > ((HScrollBar)scrollBar).Maximum)
                {
                    displayWidth = this._mPhysicalSize.Width - displayOrigin.X - ((this._mVirtualSize.Width + this._mMargins.Width) - (args.NewValue - 1 + ((HScrollBar)scrollBar).LargeChange));
                }
                else
                {
                    displayWidth = this._mPhysicalSize.Width - displayOrigin.X;
                }
                displayHeight = this._mDisplayRectangle.Height;
            }
            else
            {

                selectionOrigin = new Point(this._mSelectionRectangle.X,
                                            Math.Max((int)((args.NewValue - this._mMargins.Height) / this._mScale), 0));
                displayOrigin = new Point(this._mDisplayRectangle.X,
                                          Math.Max(this._mMargins.Height - args.NewValue, 0));
                if (args.NewValue - 1 + ((VScrollBar)scrollBar).LargeChange + this._mMargins.Height > ((VScrollBar)scrollBar).Maximum)
                {
                    displayHeight = this._mPhysicalSize.Height - displayOrigin.Y - ((this._mVirtualSize.Height + this._mMargins.Height) - (args.NewValue - 1 + ((VScrollBar)scrollBar).LargeChange));
                }
                else
                {
                    displayHeight = this._mPhysicalSize.Height - displayOrigin.Y;
                }
                displayWidth = this._mDisplayRectangle.Width;
            }



            this._mSelectionRectangle = new Rectangle(selectionOrigin,
                                                      new Size((int)(displayWidth / this._mScale),
                                                               (int)(displayHeight / this._mScale)));
            this._mDisplayRectangle = new Rectangle(displayOrigin,
                                                    new Size(displayWidth,
                                                             displayHeight));
            this.mMapDisplay?.Refresh();
        }
    
        public void ProccessScrollValue(object scrollBar, EventArgs args)
        {
            ScrollOrientation orientation;
            if ( scrollBar is HScrollBar )
            {
                orientation = ScrollOrientation.HorizontalScroll;
            }
            else
            {
                orientation = ScrollOrientation.VerticalScroll;
            }
            ScrollEventArgs scrollEventArgs = new ScrollEventArgs(ScrollEventType.EndScroll, ((ScrollBar)scrollBar).Value, orientation);
            this.ProcessScroll(scrollBar, scrollEventArgs);
        }
    

        private void OnScrollbarChange(Size hScrollSize, Size vScrollSize)
        {
            this.ScrollBarChange?.Invoke(this, new int[] { hScrollSize.Width, hScrollSize.Height, vScrollSize.Width, vScrollSize.Height });
        }

        public void ProccessSize(object mapDisplay, EventArgs args)
        {
            this._mPhysicalSize = ((MapDisplay)mapDisplay).Size;
            this._mMargins = new Size(this._mVirtualSize.Width + (2 * this._mMinMargins.Width) > this._mPhysicalSize.Width ? this._mMinMargins.Width : (int)((this._mPhysicalSize.Width - this._mVirtualSize.Width) / 2.0f),
                                      this._mVirtualSize.Height + 2 * this._mMinMargins.Height > this._mPhysicalSize.Height ? this._mMinMargins.Height : (int)((this._mPhysicalSize.Height - this._mVirtualSize.Height) / 2.0f));
            int max = this._mVirtualSize.Width + 2 * this._mMargins.Width - 1;
            int maxChange = max > this._mPhysicalSize.Width ? this._mPhysicalSize.Width : max;
            Size hScrollSize = new Size(max, maxChange);
            max = this._mVirtualSize.Height + 2 * this._mMargins.Height - 1;
            maxChange = max > this._mPhysicalSize.Height ? this._mPhysicalSize.Height : max;
            Size vScrollSize = new Size(max, maxChange);
            this.OnScrollbarChange(hScrollSize, vScrollSize);
            this.mMapDisplay?.Refresh();
        }

        public void Render(object mapDisplay, PaintEventArgs args)
        {
            args.Graphics.DrawImage(((MapDisplay)mapDisplay).mOriginalBitmap, this._mDisplayRectangle, this._mSelectionRectangle, GraphicsUnit.Pixel);
        }
    }
}
