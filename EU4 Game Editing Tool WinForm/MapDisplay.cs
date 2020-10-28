using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace EU4_Game_Editing_Tool_WinForm
{
    ///<summary>
    ///Helper Class for displaying images
    ///</summary>
    public partial class MapDisplay : Control
    {
        #region Members

        /// <summary>
        /// Holds result for later ProvinceBorder instance constructor
        /// </summary>
        private Task<ProvinceBorders> _mPBTask;
        
        private ProvinceBorders _mProvinceBorders;

        private Point _mPanPoint;

        private bool _mMiddlePressed = false;

        private GraphicsPath _mActivePaths = null;

        private GraphicsPath _mActivePathsTransformed = new GraphicsPath();

        private Region _mActiveRegion = new Region();

        private Bitmap _mOriginalBitmap;

        private Object _mLockObject = new object();

        private IReadOnlyList<Province> _mProvinces;

        public event EventHandler<Point> Pan;

        public event MouseEventHandler Zoom;

        /// <summary>
        /// Represents the instance used to generate province borders
        /// </summary>
        private ProvinceBorders mProvinceBorders
        {
            get
            {
                if (this._mProvinceBorders != null)
                {
                    return this._mProvinceBorders;
                }
                else //if singleton not yet built
                {
                    this._mProvinceBorders = this._mPBTask.Result; //get the instance as a task result
                    return this._mProvinceBorders;
                }
            }
            set
            {
                this._mProvinceBorders = value;
            }
        }

        /// <summary>
        /// Bitmap which shows the provinces layout
        /// </summary>
        public Bitmap mOriginalBitmap
        {
            get
            {
                lock (this._mLockObject)
                {
                    return this._mOriginalBitmap;
                }
            }

        
            set
            {
                if(value != null)
                {
                    this._mOriginalBitmap = value;
                    this.SetStyle(ControlStyles.UserPaint 
                                  | ControlStyles.AllPaintingInWmPaint 
                                  | ControlStyles.OptimizedDoubleBuffer,
                                  true);
                    this.Enabled = true;
                    this.Visible = true;
                    this.LoadBitmap(); //trigger rendering
                }
            }
        }

        #endregion

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                _mPanPoint = new Point((Size)(e.Location));
                _mMiddlePressed = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_mMiddlePressed)
            {
                Point offset = new Point((Size)(_mPanPoint) - (Size)(e.Location));
                _mPanPoint = e.Location;
                Pan?.Invoke(this, offset);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_mMiddlePressed)
            {
                if(e.Button == MouseButtons.Middle)
                {
                    _mMiddlePressed = false;
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //this.Render();
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            //e.Graphics.DrawImage(this.mOriginalBitmap, this._mDisplayRectangle, this._mSelectionRectangle, GraphicsUnit.Pixel);

            //if (_mActivePaths != null)
            //{
            //    Matrix matrix = new Matrix();
            //    matrix.Translate(-this._mSelectionOrigin.X * this._mScale + this._mDisplayRectangle.X, -this._mSelectionOrigin.Y * this._mScale + this._mDisplayRectangle.Y);
            //    GraphicsPath paths = (GraphicsPath)this._mActivePaths.Clone();
            //    paths.Transform(matrix);


            //    e.Graphics.DrawPath(new Pen(Color.Black, 1), paths);

            //    paths.Dispose();
            //}
            base.OnPaint(e);
            
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.Zoom?.Invoke(this, e);
            base.OnMouseWheel(e);
        }

        #region Callbacks

        /// <summary>
        /// Triggers border generating and drawing of the selected province
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void Callback_mImage_MouseClick(object obj, MouseEventArgs args)
        {

        }

        private void UpdateInternalMarginsAndSize()
        {
            //this._mVerticalMargin = this._mImageOriginalHeight * this._mScale + 200 > this.Height ? 100 : (int)((this.Height - _mImageOriginalHeight * this._mScale) / 2.0f);
            //this._mHorizontalMargin = this._mImageOriginalWidth * this._mScale + 200 > this.Width ? 100 : (int)((this.Width - _mImageOriginalWidth * this._mScale) / 2.0f);
            //this._mHeight = (int)(this._mImageOriginalHeight * this._mScale) + this._mVerticalMargin * 2;
            //this._mWidth = (int)(this._mImageOriginalWidth * this._mScale) + this._mHorizontalMargin * 2;
            //int localChange = this._mWidth / (int)(10 * this._mScale);
            //this._mHScrollBar.Maximum = Math.Max(this._mWidth - this.Width - 1 + localChange, localChange);
            //this._mHScrollBar.LargeChange = this._mWidth / (int)(10 * this._mScale);
            //localChange = this._mHeight / (int)(10 * this._mScale);
            //this._mVScrollBar.Maximum = Math.Max(this._mHeight - this.Height - 1 + localChange, localChange);
            //this._mVScrollBar.LargeChange = this._mHeight / (int)(10 * this._mScale);
            //this._mHScrollBar.SmallChange = this._mWidth / (int)(100 * this._mScale);
            //this._mVScrollBar.SmallChange = this._mHeight / (int)(100 * this._mScale);
        }

        private void Callback_MainForm_ProvincesParsed(Object obj, EventArgs args)
        {
            this._mProvinces = ((MainForm)(MainForm.ActiveForm)).mProvinces;

            this._mPBTask = new Task<ProvinceBorders>(() =>
            {
                ProvinceBorders borders = ProvinceBorders.GetProvinceBorders(this.mOriginalBitmap, this._mProvinces.Count);
                return borders;
            });
            this._mPBTask.Start();
        }

        #endregion

        #region Point Translations

        /// <summary>
        /// Returns the coordinate relative to the unscaled <see cref="mOriginalBitmap"/> 
        /// of a point on a scaled <see cref="mOriginalBitmap"/> 
        /// </summary>
        /// <param name="point">Point on the scaled <see cref="mOriginalBitmap"/></param>
        /// <returns>Coordinate relative to unscaled <see cref="mOriginalBitmap"/></returns>
        private Point ScaledBitmap2OriginalBitmap(Point point)
        {
            //point.X =(int)(point.X / this._mScale);
            //point.Y = (int)(point.Y / this._mScale);
            return point;
        }

        /// <summary>
        /// Returns the coordinate relative to the scaled <see cref="mOriginalBitmap"/> 
        /// of a point on a unscaled <see cref="mOriginalBitmap"/> 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point OriginalBitmap2ScaledBitmap(Point point)
        {
            //point.X = (int)(point.X * this._mScale);
            //point.Y = (int)(point.Y * this._mScale);
            return point;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Scrolls panel so that bitmapPoint and framePoint coordonates are equal on global reference scale
        /// </summary>
        /// <param name="framePoint">Panel point</param>
        /// <param name="bitmapPoint">Point of unscaled bitmap</param>
        private void ScrollBitmapPoint2FramePoint(Point framePoint, Point bitmapPoint)
        {
            //Point scaledPoint = this.OriginalBitmap2ScaledBitmap(bitmapPoint);
            //int scrollX = Math.Max(0, scaledPoint.X - framePoint.X + this._mHorizontalMargin);
            //int scrollY = Math.Max(0, scaledPoint.Y - framePoint.Y + this._mVerticalMargin);
            //((Panel)(this.Parent)).AutoScrollPosition = new Point(scrollX, scrollY);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
        }

        private void LoadBitmap()
        {
            //// Start with the original size
            //// TODO: Should we start with a scale of 1?
            //this._mScale = 5.5f;
            //// Update height and width members
            //this._mImageOriginalHeight = this.mOriginalBitmap.Height;
            //this._mImageOriginalWidth = this.mOriginalBitmap.Width;
            //// Find out what the margins should be
            //this.UpdateInternalMarginsAndSize();
            //this._mHScrollBar.Visible = true;
            //this._mVScrollBar.Visible = true;
            //this._mHScrollBar.Scroll += this.Callback_ScrollBars_Scroll;
            //this._mVScrollBar.Scroll += this.Callback_ScrollBars_Scroll;
            //this.SuspendLayout();
            //this.BackColor = Color.DimGray;
            //this._mSelectionOrigin = new Point(0, 0);
            //_mActiveRegion.MakeEmpty();
            //this.ResumeLayout();
        }

        private void Render()
        {
            //// Find out the selection origin
            //this._mSelectionOrigin = new Point(Math.Max((int)((this._mHScrollBar.Value - this._mHorizontalMargin) / this._mScale), 0),
            //                                  Math.Max((int)((this._mVScrollBar.Value - this._mVerticalMargin) / this._mScale), 0));
            //// Find out display origin
            //Point displayOrigin = new Point(Math.Max(this._mHorizontalMargin - this._mHScrollBar.Value, 0),
            //                                Math.Max(this._mVerticalMargin - this._mVScrollBar.Value, 0));

            //// Find out display rectangle
            //int x, y;
            //if (this._mHScrollBar.Value -1 + this._mHScrollBar.LargeChange + this._mHorizontalMargin > this._mHScrollBar.Maximum)
            //{
            //    x = this.Width - (this._mHScrollBar.Value - (int)(this._mImageOriginalWidth * this._mScale) - this._mHorizontalMargin);
            //}
            //else
            //{
            //    x = this.Width;
            //}

            //if (this._mVScrollBar.Value - 1 + this._mVScrollBar.LargeChange + this._mVerticalMargin > this._mVScrollBar.Maximum)
            //{
            //    y = this.Height - (this._mVScrollBar.Value - (int)(this._mImageOriginalHeight * this._mScale) - this._mVerticalMargin);
            //}
            //else
            //{
            //    y = this.Height + 10;
            //}

            //this._mSelectionRectangle = new Rectangle(this._mSelectionOrigin,
            //                                         new Size((int)((x - displayOrigin.X) / this._mScale),
            //                                                  (int)((y - displayOrigin.Y) / this._mScale )));
            //this._mDisplayRectangle = new Rectangle(displayOrigin.X,
            //                           displayOrigin.Y,
            //                           (int)(this._mSelectionRectangle.Width * this._mScale + 1),
            //                           (int)(this._mSelectionRectangle.Height * this._mScale + 1));
        }

        //private void Zoom(Point zoomPoint, bool magnify)
        //{
        //    //float newScale = Math.Min(20f, Math.Max(0.1f, _mScale + (magnify ? 0.2f : -0.2f)));
        //    //Point referencePoint = ScaledBitmap2OriginalBitmap(zoomPoint);
        //    //this._mScale = newScale;
        //    //this.SuspendLayout();
        //    //UpdateInternalMarginsAndSize();
        //    //int scrollX = this._mHorizontalMargin + (int)(referencePoint.X * this._mScale) - zoomPoint.X;
        //    //int scrollY = this._mVerticalMargin + (int)(referencePoint.Y * this._mScale) - zoomPoint.Y;
        //    //if (scrollX > 0 && scrollX < this._mHScrollBar.Maximum)
        //    //{
        //    //    this._mHScrollBar.Value = scrollX;
        //    //}
        //    //if (scrollY > 0 && scrollY < this._mVScrollBar.Maximum)
        //    //{
        //    //    this._mVScrollBar.Value = scrollY;
        //    //}
        //    //this.ResumeLayout();
        //    //this.Invalidate();
        //}
        #endregion
    }
}
