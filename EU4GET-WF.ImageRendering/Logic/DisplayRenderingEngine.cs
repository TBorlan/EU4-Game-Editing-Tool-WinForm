using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EU4GET_WF.ImageRendering.Control;

namespace EU4GET_WF.ImageRendering.Logic
{
    class DisplayRenderingEngine
    {
        /// <summary>
        /// Size of the scaled <see cref="DisplayPanel.mImage"/>.
        /// </summary>
        private Size _mVirtualSize;
        
        
        private Size _mPhysicalSize;

        /// <summary>
        /// Size of the bound <see cref="DisplayPanel"/>.
        /// </summary>
        /// <remarks>Setting this value triggers a rendering operation, if rendering is enabled.</remarks>
        private Size mPhysicalSize
        {
            set
            {
                this._mPhysicalSize = value;
                if (!this._mRenderingSuspended)
                {
                    this.ProcessSize();
                }
            }
        }

        /// <summary>
        /// Size and origin of the original bitmap to be displayed.
        /// </summary>
        /// <remarks>The origin of the coordinates is the upper left corner of the bitmap. </remarks>
        private Rectangle _mSelectionRectangle;

        /// <summary>
        /// Size and origin of the display area
        /// </summary>
        /// <remarks>
        /// The origin of the coordinates is the upper left corner of the <see cref="mBoundPanel"/>.
        /// </remarks>
        private Rectangle _mDisplayRectangle;

        private Size _mMargins;

        private Size _mMinMargins;

        /// <summary>
        /// Minimum margins size between the displayed <see cref="Bitmap"/> and
        /// the bound <see cref="mBoundPanel"/>.
        /// </summary>
        /// <value>Setting this value triggers a rendering operation.Getting this value might return a bigger size than the one set,
        /// this depends on the <see cref="mScale"/> used.</value>
        public Size mMargins
        {
            get
            {
                return this._mMargins;
            }
            set
            {
                this._mMinMargins = value;
                //BUG: If should check for false value, not true
                if (this._mRenderingSuspended)
                {
                    this.ProcessSize();
                }
            }
        }

        private float _mScale;

        private DisplayPanel _mDisplayPanel;

        /// <summary>
        /// Flag to determine if rendering operations should execute or not.
        /// </summary>
        /// <remarks>A false value stops executing rendering operations.</remarks>
        private bool _mRenderingSuspended;

        /// <summary>
        /// The <see cref="DisplayPanel"/> to which the engine is bound.
        /// </summary>
        /// <value>Setting this value makes the <see cref="DisplayRenderingEngine"/> to obtain references to the <see cref="DisplayPanel"/> and it's children.</value>
        public DisplayPanel mBoundPanel
        {
            private get
            {
                return this._mDisplayPanel;
            }
            set
            {
                this._mDisplayPanel = value;
                this._mMapDisplay = value.cMapDisplay;
                this._mHScrollBar = value.cHScrollBar;
                this._mVScrollBar = value.cVScrollBar;
            }
        }

        /// <summary>
        /// The scale used to display the <see cref="MapDisplay.mOriginalBitmap"/>.
        /// </summary>
        /// <remarks>Setting this value triggers a rendering operation, if rendering is enabled.</remarks>
        public float mScale
        {
            get
            {
                return this._mScale;
            }
            set
            {
                this._mScale = value;
                this._mVirtualSize = this._mMapDisplay.mOriginalBitmap?.Size ?? Size.Empty;
                //Note: I don't think that truncating is the mathematically correct way.
                this._mVirtualSize.Width = (int)Math.Truncate(this._mVirtualSize.Width * this.mScale);
                this._mVirtualSize.Height = (int)Math.Truncate(this._mVirtualSize.Height * this.mScale);
                //BUG: If should check for false value, not true
                if (this._mRenderingSuspended)
                {
                    this.ProcessSize();
                }
            }
        }

        private VScrollBar _mVScrollBar;

        private HScrollBar _mHScrollBar;

        private MapDisplay _mMapDisplay;

        /// <summary>
        /// Initialize the engine and trigger a rendering operation, if rendering is enabled.
        /// </summary>
        /// <param name="scale">Scale used to display the <see cref="MapDisplay.mOriginalBitmap"/></param>
        /// <param name="marginsSize">Minimum margins to use.</param>
        /// <seealso cref="mMargins"/>
        /// <see cref="mScale"/>
        public void Initialize(float scale, Size marginsSize)
        {
            if (!this._mRenderingSuspended)
            {
                //BUG: If flag is true, rendering will execute
                this._mRenderingSuspended = true;
                this.mScale = scale;
                this.mMargins = marginsSize;
                this._mRenderingSuspended = false;
                this.ProcessSize();
            }
            else
            {
                this.mScale = scale;
                this.mMargins = marginsSize;
            }
        }

        /// <summary>
        /// Initialize the engine with margins set to <see cref="Size.Empty"/> and trigger a rendering operation, if rendering is enabled. 
        /// </summary>
        /// <param name="scale">Scale used to display the <see cref="MapDisplay.mOriginalBitmap"/></param>
        /// <seealso cref="mMargins"/>
        /// <see cref="mScale"/>
        public void Initialize(float scale)
        {
            this.Initialize(scale, this._mMinMargins);
        }

        /// <summary>
        /// Initialize the engine with scale parameter set to <see langword="1.0f"/> and trigger a rendering operation, if rendering is enabled.
        /// </summary>
        /// <param name="marginsSize">Minimum margins to use.</param>
        /// <seealso cref="mMargins"/>
        /// <see cref="mScale"/>
        public void Initialize(Size marginsSize)
        {
            this.Initialize(this._mScale, marginsSize);
        }

        /// <summary>
        /// Initialize the engine with scale parameter set to <see langword="1.0f"/> and margins set to <see cref="Size.Empty"/> and trigger a rendering operation, if rendering is enabled. 
        /// </summary>
        /// <seealso cref="mMargins"/>
        /// <see cref="mScale"/>
        public void Initialize()
        {
            this.Initialize(1.0f, Size.Empty);
        }

        /// <summary>
        /// Sets the <see cref="mBoundPanel"/> value and registers callback to events.
        /// </summary>
        /// <param name="parent">Value to set the <see cref="mBoundPanel"/> property.</param>
        public void Bind(DisplayPanel parent)
        {
            this.mBoundPanel = parent;
            //NOTE: Why is the value and not the property used?
            this._mPhysicalSize = parent.cMapDisplay.Size;
            this._mHScrollBar.Scroll += this.ProcessScroll;
            this._mVScrollBar.Scroll += this.ProcessScroll;
            this.mBoundPanel.SizeChanged += this.GetSize;
            this._mMapDisplay.Paint += this.Render;
            this._mMapDisplay.Pan += this.GetScrollOffset;
            this._mMapDisplay.Zoom += this.GetZoom;
            this._mMapDisplay.MouseClick += this.GetSelectedColor;
        }

        /// <summary>
        /// Deletes references to <see cref="mBoundPanel"/> and it's children and unregister callbacks.
        /// </summary>
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

        /// <summary>
        /// Executes <see cref="ProcessView"/> when a scrollbar has moved.
        /// </summary>
        /// <param name="scrollBar">The scrollbar which raised the event.</param>
        /// <param name="args">Amount scrolled.</param>
        public void ProcessScroll(object scrollBar, ScrollEventArgs args)
        {
            //NOTE: Don't think it's necessary to have the if here
            if (!this._mRenderingSuspended)
            {
                this.ProcessView();
            }
        }

        /// <summary>
        /// Calculate and set <see cref="_mSelectionRectangle"/> and <see cref="_mDisplayRectangle"/> and trigger a rendering operation, if rendering is enabled.
        /// </summary>
        private void ProcessView()
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

        /// <summary>
        /// Updates the <see cref="mPhysicalSize"/> property.
        /// </summary>
        /// <param name="mapDisplay">Reference to the bound <see cref="MapDisplay"/>.</param>
        /// <param name="args"></param>
        /// <remarks>Executes when the <see cref="mBoundPanel"/> raises a <see cref="System.Windows.Forms.Control.SizeChanged"/> <see langword="event"/>.</remarks>
        private void GetSize(object mapDisplay, EventArgs args)
        {
            this.mPhysicalSize = ((System.Windows.Forms.Control)mapDisplay).Size;
        }

        /// <summary>
        /// Refresh the <see langword="Value"/> property of the <see cref="_mHScrollBar"/> and <see cref="_mVScrollBar"/> controls.
        /// </summary>
        /// <remarks>Executes when a <see cref="MapDisplay.Pan"/> <see langword="event"/> is fired and triggers the <see cref="ProcessView"/> operation.</remarks>
        /// <param name="mapDisplay">Reference to the <see cref="MapDisplay"/> that fired the event.</param>
        /// <param name="offset">Pan size and direction.</param>
        //NOTE: Currently we treat pan operation and user changing the scrollbars as two separate branches. My opinion is that we could treat them as a unified operation by using the ScrollBar.ValueChanged event. This way, we should have a flag that halts processing until both scrollbars' values are set.
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
                this.ProcessView();
            }
        }

        /// <summary>
        /// Computes <see langword="Value"/> property of the <see cref="_mHScrollBar"/> and <see cref="_mVScrollBar"/> controls according to the new <see cref="mScale"/> value.
        /// </summary>
        /// <remarks>This function triggers when a <see cref="MapDisplay.Zoom"/> <see langword="event"/> is fired.</remarks>
        /// <param name="mapDisplay">Reference to the <see cref="MapDisplay"/> that fired the event.</param>
        /// <param name="args">Zoom magnitude and sign.</param>
        private void GetZoom(object mapDisplay, MouseEventArgs args)
        {
            Point point = Point.Subtract(args.Location, (Size)this._mDisplayRectangle.Location);
            point = this.TranslateScaledToOriginal(point);
            // This is the real location on the bitmap where we made the zoom
            Point referencePoint = Point.Add(this._mSelectionRectangle.Location, (Size)point);
            this._mRenderingSuspended = true;
            this.mScale = Math.Min(20f, Math.Max(0.1f, this._mScale + (args.Delta > 0 ? 0.2f : -0.2f)));
            this.ProcessSize();
            referencePoint = this.TranslateOriginalToScaled(referencePoint);
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
            this.ProcessView();
        }

        /// <summary>
        /// Translates the coordinates of a selection on <see cref="mBoundPanel"/> to coordinates on the underlying <see cref="Bitmap"/> and triggers <see cref="SelectionManager.Select"/>.
        /// </summary>
        /// <remarks>This function triggers when a <see cref="System.Windows.Forms.Control.MouseClick"/> <see langword="event"/> is fired.</remarks>
        /// <param name="mapDisplay">Reference to the <see cref="MapDisplay"/> that fired the event.</param>
        /// <param name="args">Coordinates of the selection.</param>
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
                        this.ProcessView();
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
            this.ProcessSize();
        }

        public void SuspendRendering()
        {
            this._mRenderingSuspended = true;
        }

        /// <summary>
        /// Updates <see cref="_mHScrollBar"/> and <see cref="_mVScrollBar"/> size parameters and updates <see cref="mMargins"/> value.
        /// </summary>
        public void ProcessSize()
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
                this.ProcessView();
            }
        }

        /// <summary>
        /// Draws the required bitmap slice at the required scale and draws over the required <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="mapDisplay">Reference to the <see cref="Control"/> to draw on.</param>
        /// <param name="args"></param>
        public void Render(object mapDisplay, PaintEventArgs args)
        {
            if (this._mMapDisplay.mOriginalBitmap != null)
            {
                args.Graphics.DrawImage(((MapDisplay)mapDisplay).mOriginalBitmap, this._mDisplayRectangle, this._mSelectionRectangle, GraphicsUnit.Pixel);
                if (this._mDisplayPanel._mSelectionManager.mActivePath != null)
                {
                    // It used to be mScale here but for some reason it's not equal to the division below
                    float scaleX = (float)this._mDisplayRectangle.Width / (float)this._mSelectionRectangle.Width;
                    float scaleY = (float)this._mDisplayRectangle.Height / (float)this._mSelectionRectangle.Height;
                    Matrix matrix = new Matrix();
                    matrix.Scale(scaleX, scaleY);
                    matrix.Translate((float)(Math.Round(-this.mScale / 2)), (float)(Math.Round(-this.mScale / 2)), MatrixOrder.Append);
                    matrix.Translate(-this._mSelectionRectangle.X * scaleX + this._mDisplayRectangle.X, -this._mSelectionRectangle.Y * scaleY + this._mDisplayRectangle.Y, MatrixOrder.Append);
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
