using System;
using EU4GET_WF.ImageRendering.Control;

namespace EU4GET_WF.ImageRendering.Logic
{
    class DisplayRenderingEngine
    {
        // Scaled bitmap size
        private Size _mVirtualSize;
        // MapDisplay Size

        private Size _mPhysicalSize;
        private Size mPhysicalSize
        {
            get => this._mPhysicalSize;
            set
            {
                this._mPhysicalSize = value;
                if (!this._mRenderingSuspended)
                {
                    this.ProccessSize();
                }
            }
        }

        private Rectangle _mSelectionRectangle;

        private Rectangle _mDisplayRectangle;

        private Size _mMargins;

        private Size _mMinMargins;

        public Size mMargins
        {
            get => this._mMargins;
            set
            {
                this._mMinMargins = value;
                if (this._mRenderingSuspended)
                {
                    this.ProccessSize();
                }
            }
        }

        private float _mScale;

        private DisplayPanel _mDisplayPanel;

        private bool _mRenderingSuspended;

        public DisplayPanel mBoundPanel
        {
            private get => this._mDisplayPanel;
            set
            {
                this._mDisplayPanel = value;
                this._mMapDisplay = value.cMapDisplay;
                this._mHScrollBar = value.cHScrollBar;
                this._mVScrollBar = value.cVScrollBar;
            }
        }

        public float mScale
        {
            get => this._mScale;
            set
            {
                this._mScale = value;
                this._mVirtualSize = this._mMapDisplay.mOriginalBitmap?.Size ?? Size.Empty;
                this._mVirtualSize.Width = (int)Math.Truncate(this._mVirtualSize.Width * this.mScale);
                this._mVirtualSize.Height = (int)Math.Truncate(this._mVirtualSize.Height * this.mScale);
                if (this._mRenderingSuspended)
                {
                    this.ProccessSize();
                }
            }
        }

        private VScrollBar _mVScrollBar;

        private HScrollBar _mHScrollBar;

        private MapDisplay _mMapDisplay;

        public void Initialize(float scale, Size marginsSize)
        {
            if (!this._mRenderingSuspended)
            {
                this._mRenderingSuspended = true;
                this.mScale = scale;
                this.mMargins = marginsSize;
                this._mRenderingSuspended = false;
                this.ProccessSize();
            }
            else
            {
                this.mScale = scale;
                this.mMargins = marginsSize;
            }
        }
        public void Initialize(float scale)
        {
            this.Initialize(scale, this._mMinMargins);
        }
        public void Initialize(Size marginsSize)
        {
            this.Initialize(this._mScale, marginsSize);
        }

        public void Initialize()
        {
            this.Initialize(1.0f, Size.Empty);
        }

        public void Bind(DisplayPanel parent)
        {
            this.mBoundPanel = parent;
            this._mPhysicalSize = parent.cMapDisplay.Size;
            this._mHScrollBar.Scroll += this.ProcessScroll;
            this._mVScrollBar.Scroll += this.ProcessScroll;
            this.mBoundPanel.SizeChanged += this.GetSize;
            this._mMapDisplay.Paint += this.Render;
            this._mMapDisplay.Pan += this.GetScrollOffset;
            this._mMapDisplay.Zoom += this.GetZoom;
            this._mMapDisplay.MouseClick += this.GetSelectedColor;
        }

        public void Unbind()
        {
            this._mDisplayPanel.SizeChanged -= this.GetSize;
            this._mDisplayPanel = null;
            this._mMapDisplay.Paint -= this.Render;
            this._mMapDisplay.Pan -= this.GetScrollOffset;
            this._mMapDisplay.Zoom -= this.GetZoom;
            this._mMapDisplay.MouseClick -= this.GetSelectedColor;
            this._mMapDisplay = null;
            this._mHScrollBar.Scroll -= this.ProcessScroll;
            this._mHScrollBar = null;
            this._mVScrollBar.Scroll -= this.ProcessScroll;
            this._mVScrollBar = null;
        }



        public void ProcessScroll(object scrollBar, ScrollEventArgs args)
        {
            if (!this._mRenderingSuspended)
            {
                this.ProccessView();
            }
        }

        private void ProccessView()
        {
            // Find out the selection origin
            // Find out display origin
            // Find out display size
            Point displayOrigin, selectionOrigin;
            int displayWidth, displayHeight;

            selectionOrigin = new Point(Math.Max((int)((this._mHScrollBar.Value - this._mMargins.Width) / this._mScale), 0),
                                            Math.Max((int)((this._mVScrollBar.Value - this._mMargins.Height) / this._mScale), 0));
            displayOrigin = new Point(Math.Max(this._mMargins.Width - this._mHScrollBar.Value, 0),
                                          Math.Max(this._mMargins.Height - this._mVScrollBar.Value, 0));
            // It means we should see some upper margins
            if (this._mHScrollBar.Value - 1 + this._mHScrollBar.LargeChange + this._mMargins.Width > this._mHScrollBar.Maximum)
            {
                displayWidth = this._mPhysicalSize.Width - displayOrigin.X - ((this._mVirtualSize.Width + this._mMargins.Width) - (this._mHScrollBar.Value - 1 + this._mHScrollBar.LargeChange));
            }
            else
            {
                displayWidth = this._mPhysicalSize.Width - displayOrigin.X;
            }
            if (this._mVScrollBar.Value - 1 + this._mVScrollBar.LargeChange + this._mMargins.Height > this._mVScrollBar.Maximum)
            {
                displayHeight = this._mPhysicalSize.Height - displayOrigin.Y - ((this._mVirtualSize.Height + this._mMargins.Height) - (this._mVScrollBar.Value - 1 + this._mVScrollBar.LargeChange));
            }
            else
            {
                displayHeight = this._mPhysicalSize.Height - displayOrigin.Y;
            }

            this._mSelectionRectangle = new Rectangle(selectionOrigin,
                                                      new Size((int)(displayWidth / this._mScale),
                                                               (int)(displayHeight / this._mScale)));
            this._mDisplayRectangle = new Rectangle(displayOrigin,
                                                    new Size(displayWidth,
                                                             displayHeight));
            if (!this._mRenderingSuspended)
            {
                this._mMapDisplay?.Refresh();
            }
        }

        private void GetSize(object mapDisplay, EventArgs args)
        {
            this.mPhysicalSize = ((Control)mapDisplay).Size;
        }

        private void GetScrollOffset(object mapDisplay, Point offset)
        {
            if (!this._mRenderingSuspended)
            {
                if (((this._mHScrollBar.Value + offset.X) >= this._mHScrollBar.Minimum) && ((this._mHScrollBar.Value + offset.X) <= (this._mHScrollBar.Maximum - this._mHScrollBar.LargeChange + 1)))
                {
                    this._mHScrollBar.Value += offset.X;
                }
                if (((this._mVScrollBar.Value + offset.Y) >= this._mVScrollBar.Minimum) && ((this._mVScrollBar.Value + offset.Y) <= (this._mVScrollBar.Maximum - this._mVScrollBar.LargeChange + 1)))
                {
                    this._mVScrollBar.Value += offset.Y;
                }
                this.ProccessView();
            }
        }

        private void GetZoom(object mapDisplay, MouseEventArgs args)
        {
            Point point = Point.Subtract(args.Location, (Size)this._mDisplayRectangle.Location);
            point = this.TranslateScaledToOriginal(point);
            // This is the real location on the bitmap where we made the zoom
            Point referencePoint = Point.Add(this._mSelectionRectangle.Location, (Size)point);
            this._mRenderingSuspended = true;
            this.mScale = Math.Min(20f, Math.Max(0.1f, this._mScale + (args.Delta > 0 ? 0.2f : -0.2f)));
            this.ProccessSize();
            this.TranslateOriginalToScaled(referencePoint);
            referencePoint = Point.Add(referencePoint, this.mMargins);
            referencePoint = Point.Subtract(referencePoint, (Size)(args.Location));       
            if ((referencePoint.X > 0) && (referencePoint.X < (this._mHScrollBar.Maximum - this._mHScrollBar.LargeChange + 1)))
            {
                this._mHScrollBar.Value = referencePoint.X;
            }
            if ((referencePoint.Y > 0) && (referencePoint.Y < (this._mVScrollBar.Maximum - this._mVScrollBar.LargeChange + 1)))
            {
                this._mVScrollBar.Value = referencePoint.Y;
            }
            this._mRenderingSuspended = false;
            this.ProccessView();
        }

        private void GetSelectedColor(object mapDisplay, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                Point translatedUpperLeft = Point.Subtract(args.Location, (Size)(this._mDisplayRectangle.Location));
                Point translatedLowerRight = Point.Subtract(args.Location, new Size(this._mDisplayRectangle.Right, this._mDisplayRectangle.Bottom));
                if ((translatedUpperLeft.X >= 0) && (translatedUpperLeft.Y >= 0) && (translatedLowerRight.X <= 0) && (translatedLowerRight.Y <= 0))
                {
                    Point selectedPoint = this.TranslateScaledToOriginal(translatedUpperLeft);
                    selectedPoint = Point.Add(selectedPoint, (Size)(this._mSelectionRectangle.Location));
                    Color selectedColor = this._mMapDisplay.mOriginalBitmap.GetPixel(selectedPoint.X, selectedPoint.Y);
                    this._mDisplayPanel._mSelectionManager.Select(selectedColor);
                    if (!this._mRenderingSuspended)
                    {
                        this.ProccessView();
                    }
                }
            }
        }

        private Point TranslateScaledToOriginal(Point selection)
        {
            Point translated = selection;
            translated.X = (int)(translated.X / this.mScale);
            translated.Y = (int)(translated.Y / this.mScale);
            return translated;
        }

        private Point TranslateOriginalToScaled(Point selection)
        {
            Point translated = selection;
            translated.X = (int)(translated.X * this.mScale);
            translated.Y = (int)(translated.Y * this.mScale);
            return translated;
        }


        public void ResumeRendering()
        {
            this._mRenderingSuspended = false;
            this.ProccessSize();
        }

        public void SuspendRendering()
        {
            this._mRenderingSuspended = true;
        }

        public void ProccessSize()
        {
            this._mMargins = new Size(this._mVirtualSize.Width + (2 * this._mMinMargins.Width) > this._mPhysicalSize.Width ? this._mMinMargins.Width : (int)((this._mPhysicalSize.Width - this._mVirtualSize.Width) / 2.0f),
                                      this._mVirtualSize.Height + 2 * this._mMinMargins.Height > this._mPhysicalSize.Height ? this._mMinMargins.Height : (int)((this._mPhysicalSize.Height - this._mVirtualSize.Height) / 2.0f));
            this._mHScrollBar.Maximum = this._mVirtualSize.Width + 2 * this._mMargins.Width - 1;
            this._mHScrollBar.LargeChange = this._mHScrollBar.Maximum > this._mPhysicalSize.Width ? this._mPhysicalSize.Width : this._mHScrollBar.Maximum;
            if(this._mHScrollBar.Value > this._mHScrollBar.Maximum - this._mHScrollBar.LargeChange + 1)
            {
                this._mHScrollBar.Value = this._mHScrollBar.Maximum - this._mHScrollBar.LargeChange + 1;
            }
            this._mVScrollBar.Maximum = this._mVirtualSize.Height + 2 * this._mMargins.Height - 1;
            this._mVScrollBar.LargeChange = this._mVScrollBar.Maximum > this._mPhysicalSize.Height ? this._mPhysicalSize.Height : this._mVScrollBar.Maximum;
            if (this._mVScrollBar.Value > this._mVScrollBar.Maximum - this._mVScrollBar.LargeChange + 1)
            {
                this._mVScrollBar.Value = this._mVScrollBar.Maximum - this._mVScrollBar.LargeChange + 1;
            }
            if (!this._mRenderingSuspended)
            {
                this.ProccessView();
            }
        }

        public void Render(object mapDisplay, PaintEventArgs args)
        {
            if (this._mMapDisplay.mOriginalBitmap != null)
            {
                args.Graphics.DrawImage(((MapDisplay)mapDisplay).mOriginalBitmap, this._mDisplayRectangle, this._mSelectionRectangle, GraphicsUnit.Pixel);
                if (this._mDisplayPanel._mSelectionManager.mActivePath != null)
                {
                    Matrix matrix = new Matrix();
                    matrix.Scale(this.mScale, this.mScale);
                    matrix.Translate((float)(Math.Round((double)(-this.mScale / 2))), (float)(Math.Round((double)(-this.mScale / 2))), MatrixOrder.Append);
                    matrix.Translate(-this._mSelectionRectangle.X * this._mScale + this._mDisplayRectangle.X, -this._mSelectionRectangle.Y * this._mScale + this._mDisplayRectangle.Y, MatrixOrder.Append);
                    GraphicsPath paths = this._mDisplayPanel._mSelectionManager.mActivePath;
                    paths.Transform(matrix);
                    args.Graphics.DrawPath(new Pen(Color.Black, 1), paths);
                    paths.Dispose();
                }
            }
        }
    }
}
