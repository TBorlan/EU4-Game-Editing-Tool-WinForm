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
    public partial class ZoomablePictureBox : PictureBox
    {

        private class CustomPictureBox : PictureBox
        {
            protected override void OnPaint(PaintEventArgs pe)
            {
                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                ZoomablePictureBox pb = (ZoomablePictureBox)this.Parent;
                base.OnPaint(pe);
            }
        }

        private float mScale;

        private Bitmap _mOriginalBitmap;

        private CustomPictureBox mImage;

        public Bitmap mOriginalBitmap
        {
            get => _mOriginalBitmap;
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

        private void LoadBitmap()
        {
            this.mScale = 1.0f;
            this.mParentHeight = this.Parent.Height;
            this.mParentWidth = this.Parent.Width;
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
            Graphics.FromHwnd(this.mImage.Handle).InterpolationMode = InterpolationMode.NearestNeighbor;
            this.Controls.Add(this.mImage);
            this.Height = height;
            this.Width = width;
            this.BackColor = Color.DimGray;
            this.ResumeLayout();
        }

        private void Callback_mImage_MouseWheel(object obj, MouseEventArgs args)
        {
            CustomPictureBox pictureBox = (CustomPictureBox)obj;
            if (args.Delta != 0 && pictureBox.Image != null)
            {
                Zoom(args.Location, args.Delta > 0);
                Point point = this.Image2Frame(args.Location);
            }
        }

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

        private void ScrollBitmapPoint2FramePoint(Point framePoint, Point bitmapPoint)
        {
            Point scaledPoint = this.OriginalBitmap2ScaledBitmap(bitmapPoint);
            int scrollX = Math.Max(0, scaledPoint.X - framePoint.X + this.mHorizontalMargin);
            int scrollY = Math.Max(0, scaledPoint.Y - framePoint.Y + this.mVerticalMargin);
            ((Panel)(this.Parent)).AutoScrollPosition = new Point(scrollX, scrollY);
        }

        private void Zoom(Point zoomPoint, bool magnify)
        {
            float newScale = Math.Min(6.5f, Math.Max(0.1f, mScale + (magnify ? 0.2f : -0.2f)));

            if (newScale != mScale)
            {
                Point framePoint = this.Image2Frame(zoomPoint);
                Point bitmapPoint = this.ScaledBitmap2OriginalBitmap(zoomPoint);
                this.mVerticalMargin = this.mImageOriginalHeight * newScale + 200 > this.mParentHeight ? 100 : (int)((this.mParentHeight - this.mImageOriginalHeight * newScale) / 2.0f);
                this.mHorizontalMargin = this.mImageOriginalWidth * newScale + 200 > this.mParentWidth ? 100 : (int)((this.mParentWidth - this.mImageOriginalWidth * newScale) / 2.0f);
                int height = (int)(this.mImageOriginalHeight * newScale) + this.mVerticalMargin * 2;
                int width = (int)(this.mImageOriginalWidth * newScale) + this.mHorizontalMargin * 2;
                this.SuspendLayout();
                this.Height = height;
                this.Width = width;
                this.mImage.SuspendLayout();
                this.mImage.Height = (int)(this.mImageOriginalHeight * newScale);
                this.mImage.Width = (int)(this.mImageOriginalWidth * newScale);
                this.mImage.Location = new Point(this.mHorizontalMargin, this.mVerticalMargin);
                this.mImage.ResumeLayout();
                this.ResumeLayout();
                mScale = newScale;
                this.ScrollBitmapPoint2FramePoint(framePoint, bitmapPoint);
            }
        }
    }      
}
