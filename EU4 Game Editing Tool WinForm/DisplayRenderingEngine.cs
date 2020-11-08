using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_Game_Editing_Tool_WinForm
{
    class DisplayRenderingEngine
    {
        // Scaled bitmap size, no margins included
        private Size _mVirtualSize;

        // MapDisplay Size
        private Size _mPhysicalSize;

        /// <summary>
        /// Get or set the physical size of the <seealso cref="MapDisplay"/>
        /// </summary>
        /// <remarks>
        /// When setting, if <seealso cref="_mRenderingSuspended"/> is not set,
        /// triggers margins and scrollbar value recalculation.
        /// </remarks>
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

        // Size of the bitmap section currently displayed
        private Rectangle _mSelectionRectangle;

        // Self-explanatory
        private Rectangle _mDisplayRectangle;

        // Value of horizontal and vertical margins
        private Size _mMargins;

        // Minimum value of horizontal and vertical margins
        private Size _mMinMargins;

        /// <summary>
        /// Get the current margin size or set the minimum size
        /// </summary>
        /// <remarks>
        /// When setting, if <seealso cref="_mRenderingSuspended"/> is not set,
        /// triggers margins and scrollbar value recalculation.
        /// </remarks>
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

        // Zoom factor of the displayed bitmap
        private float _mScale;

        // Parent DisplayPanel
        private DisplayPanel _mDisplayPanel;

        // Suspend rendering and view params recalculation
        private bool _mRenderingSuspended;

        /// <summary>
        /// Get or set the parent panel
        /// </summary>
        /// <remarks>
        /// When setting, also sets the reference to the control's scrollbars and MapDisplay
        /// </remarks>
        public DisplayPanel mBoundPanel
        {
            get => this._mDisplayPanel;
            private set
            {
                this._mDisplayPanel = value;
                this._mMapDisplay = value.cMapDisplay;
                this._mHScrollBar = value.cHScrollBar;
                this._mVScrollBar = value.cVScrollBar;
            }
        }

        /// <summary>
        /// Get or set the zoom factor
        /// </summary>
        /// <remarks>
        /// When setting, triggers margins and scrollbar value recalculation
        /// </remarks>
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

        /// <summary>
        /// Sets rendering parameters
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="marginsSize"></param>
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

        /// <summary>
        /// Sets rendering parameters
        /// </summary>
        /// <param name="scale"></param>
        public void Initialize(float scale)
        {
            this.Initialize(scale, this._mMinMargins);
        }

        /// <summary>
        /// Sets rendering parameters
        /// </summary>
        /// <param name="marginsSize"></param>
        public void Initialize(Size marginsSize)
        {
            this.Initialize(this._mScale, marginsSize);
        }

        /// <summary>
        /// Sets no zoom factor and no margins rendering parameters
        /// </summary>
        public void Initialize()
        {
            this.Initialize(1.0f, Size.Empty);
        }

        /// <summary>
        /// Binds to <paramref name="parent"/> and installs callbacks for necessary events
        /// </summary>
        /// <param name="parent"></param>
        /// <remarks>
        /// When disposing the parent panel, make sure to first call <seealso cref="Unbind"/>
        /// </remarks>
        public void Bind(DisplayPanel parent)
        {
            this.mBoundPanel = parent;
            this._mPhysicalSize = parent.cMapDisplay.Size;
            this._mHScrollBar.Scroll += this.ProcessScroll;
            this._mVScrollBar.Scroll += this.ProcessScroll;
            this._mMapDisplay.SizeChanged += this.GetSize;
            this._mMapDisplay.Paint += this.Render;
            this._mMapDisplay.Pan += this.GetScrollOffset;
            this._mMapDisplay.Zoom += this.GetZoom;
            this._mMapDisplay.MouseClick += this.GetSelectedColor;
        }

        /// <summary>
        /// Deletes references to controls and uninstalls event handlers
        /// </summary>
        public void Unbind()
        {
            this._mMapDisplay.SizeChanged -= this.GetSize;
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


        /// <summary>
        /// Event handler for scroll events from the scrollbars
        /// </summary>
        /// <param name="scrollBar"></param>
        /// <param name="args"></param>
        public void ProcessScroll(object scrollBar, ScrollEventArgs args)
        {
            if (!this._mRenderingSuspended)
            {
                this.ProccessView();
            }
        }

        /// <summary>
        /// Create the <seealso cref="_mSelectionRectangle"/> and <seealso cref="_mDisplayRectangle"/> and trigger rendering
        /// </summary>
        private void ProccessView()
        {
            // Find out the selection origin
            // Find out display origin
            // Find out display size
            Point displayOrigin, selectionOrigin;
            int displayWidth, displayHeight;

            selectionOrigin = new Point(Math.Max((int)((_mHScrollBar.Value - this._mMargins.Width) / this._mScale), 0),
                                            Math.Max((int)((_mVScrollBar.Value - this._mMargins.Height) / this._mScale), 0));
            displayOrigin = new Point(Math.Max(this._mMargins.Width - _mHScrollBar.Value, 0),
                                          Math.Max(this._mMargins.Height - _mVScrollBar.Value, 0));
            // It means we see some right margin
            if (_mHScrollBar.Value - 1 + _mHScrollBar.LargeChange + this._mMargins.Width > _mHScrollBar.Maximum)
            {
                displayWidth = this.mPhysicalSize.Width - displayOrigin.X - ((this._mVirtualSize.Width + this._mMargins.Width) - (_mHScrollBar.Value - 1 + _mHScrollBar.LargeChange));
            }
            else
            {
                displayWidth = this.mPhysicalSize.Width - displayOrigin.X;
            }
            // It means we see some lower margin
            if (_mVScrollBar.Value - 1 + _mVScrollBar.LargeChange + this._mMargins.Height > _mVScrollBar.Maximum)
            {
                displayHeight = this.mPhysicalSize.Height - displayOrigin.Y - ((this._mVirtualSize.Height + this._mMargins.Height) - (_mVScrollBar.Value - 1 + _mVScrollBar.LargeChange));
            }
            else
            {
                displayHeight = this.mPhysicalSize.Height - displayOrigin.Y;
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

        /// <summary>
        /// Event handler for <seealso cref="Control.SizeChanged"/> <see langword="event"/>
        /// </summary>
        /// <param name="mapDisplay"></param>
        /// <param name="args"></param>
        private void GetSize(object mapDisplay, EventArgs args)
        {
            this.mPhysicalSize = ((Control)mapDisplay).Size;
        }

        /// <summary>
        /// Event handler for <seealso cref="MapDisplay.Pan"/> <see langword="event"/>
        /// </summary>
        /// <param name="mapDisplay"></param>
        /// <param name="offset"></param>
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

        /// <summary>
        /// Event handler for <seealso cref="MapDisplay.Zoom"/> <see langword="event"/
        /// </summary>
        /// <param name="mapDisplay"></param>
        /// <param name="args"></param>
        private void GetZoom(object mapDisplay, MouseEventArgs args)
        {
            Point point = Point.Subtract(args.Location, (Size)this._mDisplayRectangle.Location);
            point = this.TranslateScaledToOriginal(point);
            // This is the real location on the bitmap where we made the zoom
            Point referencePoint = Point.Add(this._mSelectionRectangle.Location, (Size)point);
            this._mRenderingSuspended = true;
            this.mScale = Math.Min(20f, Math.Max(0.1f, _mScale + (args.Delta > 0 ? 0.2f : -0.2f)));
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
                Point translatedLowerRight = Point.Subtract(args.Location, new Size(this._mDisplayRectangle.Right, _mDisplayRectangle.Bottom));
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


        /// <summary>
        /// Resumes layout and rendering functionality
        /// </summary>
        public void ResumeRendering()
        {
            this._mRenderingSuspended = false;
            this.ProccessSize();
        }

        /// <summary>
        /// Suspends layout and rendering functionality
        /// </summary>
        public void SuspendRendering()
        {
            this._mRenderingSuspended = true;
        }

        /// <summary>
        /// Recalculate margin values, scrollbar maximum and large change values
        /// </summary>
        public void ProccessSize()
        {
            this._mMargins = new Size(this._mVirtualSize.Width + (2 * this._mMinMargins.Width) > this.mPhysicalSize.Width ? this._mMinMargins.Width : (int)((this.mPhysicalSize.Width - this._mVirtualSize.Width) / 2.0f),
                                      this._mVirtualSize.Height + 2 * this._mMinMargins.Height > this.mPhysicalSize.Height ? this._mMinMargins.Height : (int)((this.mPhysicalSize.Height - this._mVirtualSize.Height) / 2.0f));
            this._mHScrollBar.Maximum = this._mVirtualSize.Width + 2 * this._mMargins.Width - 1;
            this._mHScrollBar.LargeChange = this._mHScrollBar.Maximum > this.mPhysicalSize.Width ? this.mPhysicalSize.Width : this._mHScrollBar.Maximum;
            // After resizing, if value becomes illegal, scroll to the first legal value
            if (this._mHScrollBar.Value > this._mHScrollBar.Maximum - this._mHScrollBar.LargeChange + 1)
            {
                this._mHScrollBar.Value = this._mHScrollBar.Maximum - this._mHScrollBar.LargeChange + 1;
            }
            this._mVScrollBar.Maximum = this._mVirtualSize.Height + 2 * this._mMargins.Height - 1;
            this._mVScrollBar.LargeChange = this._mVScrollBar.Maximum > this.mPhysicalSize.Height ? this.mPhysicalSize.Height : this._mVScrollBar.Maximum;
            // After resizing, if value becomes illegal, scroll to the first legal value
            if (this._mVScrollBar.Value > this._mVScrollBar.Maximum - this._mVScrollBar.LargeChange + 1)
            {
                this._mVScrollBar.Value = this._mVScrollBar.Maximum - this._mVScrollBar.LargeChange + 1;
            }
            if (!this._mRenderingSuspended)
            {
                this.ProccessView();
            }
        }

        /// <summary>
        /// Paints the bitmap as per layout params calculated in <seealso cref="ProccessView"/>
        /// </summary>
        /// <param name="mapDisplay"></param>
        /// <param name="args"></param>
        public void Render(object mapDisplay, PaintEventArgs args)
        {
            if (this._mMapDisplay.mOriginalBitmap != null)
            {
                args.Graphics.DrawImage(((MapDisplay)mapDisplay).mOriginalBitmap, this._mDisplayRectangle, this._mSelectionRectangle, GraphicsUnit.Pixel);
                if (this._mDisplayPanel._mSelectionManager.mActivePath != null)
                {
                    Matrix matrix = new Matrix();
                    matrix.Scale(this.mScale, this.mScale);
                    matrix.Translate((float)(Math.Round((double)(-mScale / 2))), (float)(Math.Round((double)(-mScale / 2))), MatrixOrder.Append);
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
