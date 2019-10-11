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
    public partial class ZoomablePictureBox : PictureBox
    {
        #region Members

        /// <summary>
        /// PictureBox class used for drawing image in ZoomablePictueBox
        /// </summary>
        private class CustomPictureBox : PictureBox
        {
            /// <summary>
            /// Sets interpolation so that rednering doesn't distort scaled bitmap
            /// </summary>
            /// <param name="pe"></param>
            protected override void OnPaint(PaintEventArgs pe)
            {
                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                base.OnPaint(pe);
            }
        }

        /// <summary>
        /// Holds result for later ProvinceBorder instance constructor
        /// </summary>
        private Task<ProvinceBorders> mPBTask;

        /// <summary>
        /// Zoom factor of the bitmap
        /// </summary>
        private float mScale;

        /// <summary>
        /// Control used to draw the bitmap on
        /// </summary>
        private CustomPictureBox mImage;
        
        private ProvinceBorders _mProvinceBorders;

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

        /// <summary>
        /// Bitmap which shows the provinces layout
        /// </summary>
        public Bitmap mOriginalBitmap
        {
            get => (Bitmap)_mOriginalBitmap.Clone();  //return shallow copy
            set
            {
                if(value != null)
                {
                    _mOriginalBitmap = (Bitmap)value.Clone(); //gets a shallow copy, so the underlying file is locked
                    // Update height and width members
                    mImageOriginalHeight = mOriginalBitmap.Height;
                    mImageOriginalWidth = mOriginalBitmap.Width;
                    LoadBitmap(); //trigger rendering
                }
            }
        }

        /// width and height of <see cref="mOriginalBitmap"/>
        private int mImageOriginalHeight;
        private int mImageOriginalWidth;

        /// width and height of the margins of the frame in which <see cref="mImage"/> is rendered
        private int mVerticalMargin;
        private int mHorizontalMargin;

        /// width and height of the parent Panel
        private int mParentHeight;
        private int mParentWidth;
        #endregion

        #region Callbacks

        /// <summary>
        /// Triggers scaling and rendering of the <see cref="mImage"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void Callback_mImage_MouseWheel(object obj, MouseEventArgs args)
        {
            CustomPictureBox pictureBox = (CustomPictureBox)obj;
            if (args.Delta != 0 && pictureBox.Image != null)
            {
                Zoom(args.Location, args.Delta > 0);
                Point point = this.Image2Frame(args.Location);
            }
        }

        /// <summary>
        /// Updates parrent panel sizes and renders
        /// </summary>
        /// <param name="obj">Parent <see cref="Panel"/></param>
        /// <param name="args"></param>
        private void Callback_cPictureBoxPanel(object obj, EventArgs args)
        {
            this.mParentHeight = ((Panel)(obj)).Height;
            this.mParentWidth = ((Panel)(obj)).Width;
            this.Render();
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
                CustomPictureBox pictureBox = (CustomPictureBox)obj;
                Point point = this.ScaledBitmap2OriginalBitmap(args.Location);
                Color color = ((Bitmap)(this.mImage.Image)).GetPixel(point.X, point.Y);

                using (Graphics graphics = Graphics.FromHwnd(this.mImage.Handle))
                {
                    GraphicsPath graphicsPath = this.mProvinceBorders.GetProvinceBorder(color);
                    Matrix matrix = new Matrix();
                    matrix.Translate(-1*mScale/2, -1*mScale/2);
                    matrix.Scale(this.mScale, this.mScale);
                    graphicsPath.Transform(matrix);
                    graphics.DrawPath(new Pen(Color.Black, 1), graphicsPath);
                }
            }
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
            point.X =(int)(point.X / this.mScale);
            point.Y = (int)(point.Y / this.mScale);
            return point;
        }
        /// <summary>
        /// Returns the coordinate relative to the Panel of a point 
        /// on <see cref="mImage"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point Image2Frame(Point point)
        {
            point = this.mImage.PointToScreen(point);
            point = this.Parent.PointToClient(point);
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
        /// Scrolls panel so that bitmapPoint and framePoint coordonates are equal on global reference scale
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

        private void LoadBitmap()
        {
            this.mScale = 1.0f;
            this.mParentHeight = this.Parent.Height;
            this.mParentWidth = this.Parent.Width;
            ((Panel)this.Parent).Resize += new EventHandler(Callback_cPictureBoxPanel);
            this.mVerticalMargin = mImageOriginalHeight + 200 > this.mParentHeight ? 100 : (int)((this.mParentHeight - mImageOriginalHeight) / 2.0f);
            this.mHorizontalMargin = mImageOriginalWidth + 200 > this.mParentWidth ? 100 : (int)((this.mParentWidth - mImageOriginalWidth) / 2.0f);
            int height = mImageOriginalHeight + mVerticalMargin * 2;
            int width = mImageOriginalWidth + mHorizontalMargin * 2;
            this.SuspendLayout();
            this.mImage = new CustomPictureBox();
            this.mImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.mImage.Height = this.mImageOriginalHeight;
            this.mImage.Width = this.mImageOriginalWidth;
            this.mImage.Location = new Point(this.mHorizontalMargin, this.mVerticalMargin);
            // mImage and ZoomablePictureBox share the same field
            this.mImage.Image = new Bitmap(this._mOriginalBitmap);
            this.mImage.MouseWheel += new MouseEventHandler(this.Callback_mImage_MouseWheel);
            this.mImage.MouseClick += new MouseEventHandler(this.Callback_mImage_MouseClick);
            this.Controls.Add(this.mImage);
            this.Height = height;
            this.Width = width;
            this.BackColor = Color.DimGray;
            this.ResumeLayout();
            this.mPBTask = new Task<ProvinceBorders>(() =>
            {
                ProvinceBorders borders = ProvinceBorders.GetProvinceBorders(this.mOriginalBitmap);
                return borders;
            });
            this.mPBTask.Start();
        }

        private void Render()
        {
            this.mVerticalMargin = this.mImageOriginalHeight * this.mScale + 200 > this.mParentHeight ? 100 : (int)((this.mParentHeight - this.mImageOriginalHeight * this.mScale) / 2.0f);
            this.mHorizontalMargin = this.mImageOriginalWidth * this.mScale + 200 > this.mParentWidth ? 100 : (int)((this.mParentWidth - this.mImageOriginalWidth * this.mScale) / 2.0f);
            int height = (int)(this.mImageOriginalHeight * this.mScale) + this.mVerticalMargin * 2;
            int width = (int)(this.mImageOriginalWidth * this.mScale) + this.mHorizontalMargin * 2;
            this.SuspendLayout();
            this.Height = height;
            this.Width = width;
            this.mImage.SuspendLayout();
            this.mImage.Height = (int)(this.mImageOriginalHeight * this.mScale);
            this.mImage.Width = (int)(this.mImageOriginalWidth * this.mScale);
            this.mImage.Location = new Point(this.mHorizontalMargin, this.mVerticalMargin);
            this.mImage.ResumeLayout();
            this.ResumeLayout();
        }

        private void Zoom(Point zoomPoint, bool magnify)
        {
            float newScale = Math.Min(6.5f, Math.Max(0.1f, mScale + (magnify ? 0.2f : -0.2f)));

            if (newScale != mScale)
            {
                Point framePoint = this.Image2Frame(zoomPoint);
                Point bitmapPoint = this.ScaledBitmap2OriginalBitmap(zoomPoint);
                mScale = newScale;
                this.Render();
                this.ScrollBitmapPoint2FramePoint(framePoint, bitmapPoint);
            }
        }

        public void DrawBorder(Color color)
        {
            using (GraphicsPath path = this.mProvinceBorders.GetProvinceBorder(color))
            {
                using (Graphics graphics = Graphics.FromHwnd(this.mImage.Handle))
                {
                    graphics.DrawPath(new Pen(Color.Black, 1), path);
                }
            }
        }
        #endregion
    }
}
