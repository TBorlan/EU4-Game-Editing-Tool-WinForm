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
using ClipperLib;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Reflection;

namespace EU4_Game_Editing_Tool_WinForm
{

    ///<summary>
    ///Helper Class for displaying images
    ///</summary>
    public partial class ZoomablePictureBox : Control
    {

        #region Members
        /// <summary>
        /// Holds result for later ProvinceBorder instance constructor
        /// </summary>
        private Task<ProvinceBorders> mPBTask;

        /// <summary>
        /// Zoom factor of the bitmap
        /// </summary>
        public float mScale;
        
        private ProvinceBorders _mProvinceBorders;

        public HScrollBar _mhScrollBar;

        public VScrollBar _mvScrollBar;

        private int _mHeight;

        private int _mWidth;

        private Point PanPoint;

        private bool MiddlePressed = false;

        private Rectangle mSelectionRectangle;

        private Rectangle mDisplayRectangle;

        private Point mSelectionOrigin = new Point(0, 0);

        private GraphicsPath mActivePaths = null;

        private GraphicsPath mActivePathsTransformed = new GraphicsPath();

        private Region mActiveRegion = new Region();

        private SelectionManager _mSelectionManager;

        private SelectionManager mSelectionManager
        {
            get
            {
                if (this._mSelectionManager == null)
                {
                    _mSelectionManager = new SelectionManager(this.mProvinceBorders);
                }
                return this._mSelectionManager;
            }
        }

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
                    this._mProvinceBorders = this.mPBTask.Result; //get the instance as a task result
                    return this._mProvinceBorders;
                }
            }
            set
            {
                this._mProvinceBorders = value;
            }
        }

        private Bitmap _mOriginalBitmap;

        private Object lockObject = new object();

        /// <summary>
        /// Bitmap which shows the provinces layout
        /// </summary>
        public Bitmap mOriginalBitmap
        {
            get
            {
                lock (this.lockObject)
                {
                    return this._mOriginalBitmap;
                }
            }

        
            set
            {
                if(value != null)
                {                   
                    this._mOriginalBitmap = new Bitmap(value); 
                    this.SetStyle(
                                    ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
                    this.Enabled = true;
                    this.Visible = true;
                    LoadBitmap(); //trigger rendering
                }
            }
        }

        /// <summary>
        /// width and height of <see cref="mOriginalBitmap"/>
        /// </summary>
        private int mImageOriginalHeight;
        private int mImageOriginalWidth;

        /// <summary>
        /// width and height of the margins of the frame in which <see cref="mImage"/> is rendered
        /// </summary>
        private int mVerticalMargin;
        private int mHorizontalMargin;

        private IReadOnlyList<Province> mProvinces;
        #endregion

        #region Callbacks

        /// <summary>
        /// Triggers scaling and rendering of the <see cref="mImage"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void Callback_mImage_MouseWheel(object obj, MouseEventArgs args)
        {
            //CustomPictureBox pictureBox = (CustomPictureBox)obj;
            //if (args.Delta != 0 && pictureBox.mCurrentDisplayedImage != null)
            //{
            //    Zoom(args.Location, args.Delta > 0);
            //    Point point = this.Image2Frame(args.Location);
            //}
        }

        private void Callback_ScrollBars_Scroll(object obj, ScrollEventArgs args)
        {
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                PanPoint = new Point((Size)(e.Location));
                MiddlePressed = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MiddlePressed)
            {
                Point offset = new Point((Size)(PanPoint) - (Size)(e.Location));
                this._mhScrollBar.Value += (this._mhScrollBar.Value + offset.X < 0) || (this._mhScrollBar.Value + offset.X > this._mhScrollBar.Maximum) ? 0 : offset.X;
                this._mvScrollBar.Value += (this._mvScrollBar.Value + offset.Y < 0) || (this._mvScrollBar.Value + offset.Y > this._mvScrollBar.Maximum) ? 0 : offset.Y;
                PanPoint = e.Location;
                this.Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (MiddlePressed)
            {
                if(e.Button == MouseButtons.Middle)
                {
                    MiddlePressed = false;
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.Render();
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(this.mOriginalBitmap, this.mDisplayRectangle, this.mSelectionRectangle, GraphicsUnit.Pixel);

            if (mActivePaths != null)
            {
                Matrix matrix = new Matrix();
                matrix.Translate(-this.mSelectionOrigin.X * this.mScale + this.mDisplayRectangle.X, -this.mSelectionOrigin.Y * this.mScale + this.mDisplayRectangle.Y);
                GraphicsPath paths = (GraphicsPath)this.mActivePaths.Clone();
                paths.Transform(matrix);


                e.Graphics.DrawPath(new Pen(Color.Black, 1), paths);

                paths.Dispose();
            }
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this._mOriginalBitmap != null)
            {
                this.UpdateInternalMarginsAndSize();
            }
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.Zoom(e.Location, e.Delta > 0);
            base.OnMouseWheel(e);
        }

        protected override void OnClick(EventArgs e)
        {
            this.Callback_mImage_MouseClick(this, (MouseEventArgs)e);
            base.OnClick(e);
        }

        /// <summary>
        /// Triggers border generating and drawing of the selected province
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void Callback_mImage_MouseClick(object obj, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                Point point = this.ScaledBitmap2OriginalBitmap(args.Location);
                Color color = (this._mOriginalBitmap).GetPixel(point.X, point.Y);
                mSelectionManager.Select(color);
                GraphicsPath graphicsPath = mSelectionManager.mActivePath;
                if(graphicsPath == null)
                {
                    if (mActivePaths != null)
                    {
                        mActivePaths.Dispose();
                    }
                    mActivePaths = null;
                    Invalidate();
                    return;
                }
                Matrix matrix = new Matrix();
                //matrix.Translate((float)(-0.5*(mScale)), (float)(-0.5*(mScale)));

                matrix.Scale(this.mScale, this.mScale);
                matrix.Translate(-mScale / 2, -mScale / 2, MatrixOrder.Append);
                graphicsPath.Transform(matrix);
                if (mActivePaths != null)
                {
                    mActivePaths.Reset();
                }
                else
                {
                    mActivePaths = new GraphicsPath();
                }
                mActivePaths.AddPath(graphicsPath, false);
                

                //List<IntPoint> path = graphicsPath.PathPoints.Select((PointF p) => new IntPoint(p)).ToList();
                //Clipper clipper = new Clipper();
                //clipper.AddPath(path, PolyType.ptSubject, true);
                //path = mActivePaths.PathPoints.Select((PointF p) => new IntPoint(p)).ToList();
                //clipper.AddPath(path, PolyType.ptClip, true);
                //PolyTree polyTree = new PolyTree();
                //_ = clipper.Execute(ClipType.ctUnion, polyTree, PolyFillType.pftPositive);
                //this.mActivePaths.Reset();
                //foreach (PolyNode node in polyTree.Childs)
                //{
                //    this.mActivePaths.AddPolygon(node.Contour.Select((IntPoint p) => new Point((int)p.X, (int)p.Y)).ToArray());
                //}

                //using (Graphics graphics = Graphics.FromHwnd(this.mImage.Handle))
                //{
                //    GraphicsPath graphicsPath = this.mProvinceBorders.GetProvinceBorder(color);
                //    Matrix matrix = new Matrix();
                //    matrix.Translate(-1*mScale/2, -1*mScale/2);
                //    matrix.Scale(this.mScale, this.mScale);
                //    graphicsPath.Transform(matrix);
                //    graphics.DrawPath(new Pen(Color.Black, 1), graphicsPath);
                //}
                this.Invalidate();
            }
        }

        public void Callback_MainForm_ProvincesParsed(Object obj, ProvinceEventArgs args)
        {
            this.mProvinces = args.provinces;

            this.mPBTask = new Task<ProvinceBorders>(() =>
            {
                ProvinceBorders borders = ProvinceBorders.GetProvinceBorders(((MainForm)Application.OpenForms[0]).mBitmap, this.mProvinces.Count);
                return borders;
            });
            this.mPBTask.Start();
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
            point.X =(int)((point.X - this.mDisplayRectangle.X) / this.mScale) + this.mSelectionOrigin.X;
            point.Y = (int)((point.Y - this.mDisplayRectangle.Y) / this.mScale) + this.mSelectionOrigin.Y;
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
            point.X = (int)(point.X * this.mScale);
            point.Y = (int)(point.Y * this.mScale);
            return point;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Scrolls panel so that bitmapPoint and framePoint coordinates are equal on global reference scale
        /// </summary>
        /// <param name="framePoint">Panel point</param>
        /// <param name="bitmapPoint">Point of unscaled bitmap</param>
        private void ScrollBitmapPoint2FramePoint(Point framePoint, Point bitmapPoint)
        {
            Point scaledPoint = this.OriginalBitmap2ScaledBitmap(bitmapPoint);
            int scrollX = Math.Max(0, scaledPoint.X - framePoint.X + this.mHorizontalMargin);
            int scrollY = Math.Max(0, scaledPoint.Y - framePoint.Y + this.mVerticalMargin);
            ((Panel)(this.Parent)).AutoScrollPosition = new Point(scrollX, scrollY);
        }

        private void UpdateInternalMarginsAndSize()
        {
            this.mVerticalMargin = this.mImageOriginalHeight * this.mScale + 200 > this.Height ? 100 : (int)((this.Height - mImageOriginalHeight * this.mScale) / 2.0f);
            this.mHorizontalMargin = this.mImageOriginalWidth * this.mScale + 200 > this.Width ? 100 : (int)((this.Width - mImageOriginalWidth * this.mScale) / 2.0f);
            this._mHeight = (int)(this.mImageOriginalHeight * this.mScale) + this.mVerticalMargin * 2;
            this._mWidth = (int)(this.mImageOriginalWidth * this.mScale) + this.mHorizontalMargin * 2;
            int localChange = this._mWidth / (int)(10 * this.mScale);
            this._mhScrollBar.Maximum = Math.Max(this._mWidth - this.Width - 1 + localChange, localChange);
            this._mhScrollBar.LargeChange = this._mWidth / (int)(10 * this.mScale);
            localChange = this._mHeight / (int)(10 * this.mScale);
            this._mvScrollBar.Maximum = Math.Max(this._mHeight - this.Height - 1 + localChange, localChange);
            this._mvScrollBar.LargeChange = this._mHeight / (int)(10 * this.mScale);
            this._mhScrollBar.SmallChange = this._mWidth / (int)(100 * this.mScale);
            this._mvScrollBar.SmallChange = this._mHeight / (int)(100 * this.mScale);
        }

        /// <summary>
        /// Called when this <see cref="ZoomablePictureBox"/> gets assigned the provinces bitmap
        /// </summary>
        private void LoadBitmap()
        {
            
            // Start with the original size
            // TODO: Should we start with a scale of 1?
            this.mScale = 5.5f;
            // Update height and width members
            this.mImageOriginalHeight = this.mOriginalBitmap.Height;
            this.mImageOriginalWidth = this.mOriginalBitmap.Width;
            // Find out what the margins should be
            this.UpdateInternalMarginsAndSize();
            this._mhScrollBar.Visible = true;
            this._mvScrollBar.Visible = true;
            this._mhScrollBar.Scroll += this.Callback_ScrollBars_Scroll;
            this._mvScrollBar.Scroll += this.Callback_ScrollBars_Scroll;
            this.SuspendLayout();
            this.BackColor = Color.DimGray;
            this.mSelectionOrigin = new Point(0, 0);
            mActiveRegion.MakeEmpty();
            this.ResumeLayout();
        }

        private void Render()
        {
            // Find out the selection origin
            this.mSelectionOrigin = new Point(Math.Max((int)((this._mhScrollBar.Value - this.mHorizontalMargin) / this.mScale), 0),
                                              Math.Max((int)((this._mvScrollBar.Value - this.mVerticalMargin) / this.mScale), 0));
            // Find out display origin
            Point displayOrigin = new Point(Math.Max(this.mHorizontalMargin - this._mhScrollBar.Value, 0),
                                            Math.Max(this.mVerticalMargin - this._mvScrollBar.Value, 0));

            // Find out display rectangle
            int x, y;
            if (this._mhScrollBar.Value -1 + this._mhScrollBar.LargeChange + this.mHorizontalMargin > this._mhScrollBar.Maximum)
            {
                x = this.Width - (this._mhScrollBar.Value - (int)(this.mImageOriginalWidth * this.mScale) - this.mHorizontalMargin);
            }
            else
            {
                x = this.Width;
            }

            if (this._mvScrollBar.Value - 1 + this._mvScrollBar.LargeChange + this.mVerticalMargin > this._mvScrollBar.Maximum)
            {
                y = this.Height - (this._mvScrollBar.Value - (int)(this.mImageOriginalHeight * this.mScale) - this.mVerticalMargin);
            }
            else
            {
                y = this.Height + 10;
            }

            this.mSelectionRectangle = new Rectangle(this.mSelectionOrigin,
                                                     new Size((int)((x - displayOrigin.X) / this.mScale),
                                                              (int)((y - displayOrigin.Y) / this.mScale )));
            this.mDisplayRectangle = new Rectangle(displayOrigin.X,
                                       displayOrigin.Y,
                                       (int)(this.mSelectionRectangle.Width * this.mScale + 1),
                                       (int)(this.mSelectionRectangle.Height * this.mScale + 1));
        }

        private void Zoom(Point zoomPoint, bool magnify)
        {
            float newScale = Math.Min(20f, Math.Max(0.1f, mScale + (magnify ? 0.2f : -0.2f)));
            Point referencePoint = ScaledBitmap2OriginalBitmap(zoomPoint);
            this.mScale = newScale;
            this.SuspendLayout();
            UpdateInternalMarginsAndSize();
            int scrollX = this.mHorizontalMargin + (int)(referencePoint.X * this.mScale) + zoomPoint.X;
            int scrollY = this.mVerticalMargin + (int)(referencePoint.Y * this.mScale) + zoomPoint.Y;
            if (scrollX > 0 && scrollX < this._mhScrollBar.Maximum)
            {
                this._mhScrollBar.Value = scrollX;
            }
            if (scrollY > 0 && scrollY < this._mvScrollBar.Maximum)
            {
                this._mvScrollBar.Value = scrollY;
            }
            Matrix matrix = new Matrix();
            //matrix.Translate((float)(-0.5*(mScale)), (float)(-0.5*(mScale)));
            GraphicsPath graphicsPath = this.mSelectionManager.mActivePath;
            matrix.Scale(this.mScale, this.mScale);
            matrix.Translate(-mScale / 2, -mScale / 2, MatrixOrder.Append);
            graphicsPath.Transform(matrix);
            if (mActivePaths != null)
            {
                mActivePaths.Reset();
            }
            else
            {
                mActivePaths = new GraphicsPath();
            }
            mActivePaths.AddPath(graphicsPath, false);
            this.ResumeLayout();
            this.Invalidate();

            //if (newScale != mScale)
            //{
            //    Point framePoint = this.Image2Frame(zoomPoint);
            //    // This seems redundant but isn't because scale changes between consecutive calls
            //    Point bitmapPoint = this.ScaledBitmap2OriginalBitmap(zoomPoint);
            //    mScale = newScale;
            //    // TODO: Should be called in the mScale setter
            //    this.Render();
            //    this.ScrollBitmapPoint2FramePoint(framePoint, bitmapPoint);
            //}
        }
        #endregion
    }
}
