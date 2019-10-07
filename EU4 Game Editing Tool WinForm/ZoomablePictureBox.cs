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
        private class CustomPictureBox : PictureBox
        {

            protected override void OnPaint(PaintEventArgs pe)
            {
                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                base.OnPaint(pe);
            }
        }

        private Task<ProvinceBorders> mPBTask;

        private float mScale;

        private CustomPictureBox mImage;

        private ProvinceBorders _mProvinceBorders;

        private ProvinceBorders mProvinceBorders
        {
            get
            {
                if (this._mProvinceBorders != null)
                {
                    return this._mProvinceBorders;
                }
                else
                {
                    this._mProvinceBorders = this.mPBTask.Result;
                    return this._mProvinceBorders;
                }
            }
            set
            {
                this._mProvinceBorders = value;
            }
        }

        private Bitmap _mOriginalBitmap;

        public Bitmap mOriginalBitmap
        {
            get => (Bitmap)_mOriginalBitmap.Clone();
            set
            {
                if(value != null)
                {
                    _mOriginalBitmap = (Bitmap)value.Clone();
                    mImageOriginalHeight = mOriginalBitmap.Height;
                    mImageOriginalWidth = mOriginalBitmap.Width;
                    LoadBitmap();
                }
            }
        }

        private int mImageOriginalHeight;
        private int mImageOriginalWidth;

        private int mVerticalMargin;
        private int mHorizontalMargin;

        private int mParentHeight;
        private int mParentWidth;
        #endregion

        #region Callbacks
        private void Callback_mImage_MouseWheel(object obj, MouseEventArgs args)
        {
            CustomPictureBox pictureBox = (CustomPictureBox)obj;
            if (args.Delta != 0 && pictureBox.Image != null)
            {
                Zoom(args.Location, args.Delta > 0);
                Point point = this.Image2Frame(args.Location);
            }
        }

        private void Callback_cPictureBoxPanel(object obj, EventArgs args)
        {
            this.mParentHeight = ((Panel)(obj)).Height;
            this.mParentWidth = ((Panel)(obj)).Width;
            this.Render();
        }

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
        private Point ScaledBitmap2OriginalBitmap(Point point)
        {
            point.X =(int)(point.X / this.mScale);
            point.Y = (int)(point.Y / this.mScale);
            return point;
        }

        private Point Image2Frame(Point point)
        {
            point = this.mImage.PointToScreen(point);
            point = this.Parent.PointToClient(point);
            return point;
        }

        private Point OriginalBitmap2ScaledBitmap(Point point)
        {
            point.X = (int)(point.X * this.mScale);
            point.Y = (int)(point.Y * this.mScale);
            return point;
        }
        #endregion

        #region Methods
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
            this.mImage.Image = this.mOriginalBitmap;
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
