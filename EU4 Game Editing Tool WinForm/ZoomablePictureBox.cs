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

        private float mScale;

        private Bitmap _mOriginalBitmap;

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
            mScale = 1.0f;
            mParentHeight = this.Parent.Height;
            mParentWidth = this.Parent.Width;
            mVerticalMargin = mImageOriginalHeight + 200 > this.mParentHeight ? 100 : (int)((this.mParentHeight - mImageOriginalHeight) / 2.0f);
            mHorizontalMargin = mImageOriginalWidth + 200 > this.mParentWidth ? 100 : (int)((this.mParentWidth - mImageOriginalWidth) / 2.0f);
            int height = mImageOriginalHeight + mVerticalMargin*2;
            int width = mImageOriginalWidth + mHorizontalMargin*2;
            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetResolution(this.mOriginalBitmap.HorizontalResolution, this.mOriginalBitmap.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Beige);
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.DrawImage(this.mOriginalBitmap, new Point(mHorizontalMargin, mVerticalMargin));
            }
            try
            {
                this.Image.Dispose();
            }
            catch
            {
                ;
            }
            finally
            {
                this.Image = bitmap;
            }

        }

        private Point TransformPosition(Point point)
        {
            point.X =(int)((point.X - this.mHorizontalMargin * this.mScale) / this.mScale);
            point.Y = (int)((point.Y - this.mVerticalMargin * this.mScale) / this.mScale);
            return point;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics graphics = pe.Graphics;
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            //if (mTransform != null)
            //{
                //graphics.Transform = mTransform;
                //this.Width = (int)((float)this.mOriginalWidth * mScale);
                //this.Height = (int)((float)this.mOriginalHeight * mScale);
                //Point point = ((ScrollableControl)(this.Parent)).AutoScrollOffset;
                //point.X = (int)mScalinigFactor*point.X + point.X;
                //point.Y = (int)mScalinigFactor * point.Y + point.Y;
                //((ScrollableControl)(this.Parent)).AutoScrollOffset = point;
            //}
            base.OnPaint(pe);

        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.Focus();
            if(this.Focused && e.Delta != 0 && this.Image!=null)
            {
                Zoom(e.Location, e.Delta > 0);
            }
            Point point = this.TransformPosition(e.Location);
            
        }

        private void Zoom(Point zoomPoint, bool magnify)
        {
            float newScale = Math.Min(5f, Math.Max(0.1f, mScale + (magnify ? 0.4f : -0.1f)));

            if (newScale != mScale)
            {
                mVerticalMargin = this.mImageOriginalHeight * newScale + 200 > this.mParentHeight ? 100 : (int)((this.mParentHeight - mImageOriginalHeight * newScale) / 2.0f);
                mHorizontalMargin = this.mImageOriginalWidth * newScale + 200 > this.mParentWidth ? 100 : (int)((this.mParentWidth - mImageOriginalWidth * newScale) / 2.0f);
                int height = (int)(mImageOriginalHeight * newScale) + mVerticalMargin * 2;
                int width = (int)(mImageOriginalWidth * newScale) + mHorizontalMargin * 2;
                if( newScale < 3)
                {
                    Bitmap bitmap = new Bitmap(width, height);
                    bitmap.SetResolution(this.mOriginalBitmap.HorizontalResolution, this.mOriginalBitmap.VerticalResolution);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(Color.Beige);
                        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        //graphics.DrawImage(this.mOriginalBitmap, new Point(mHorizontalMargin, mVerticalMargin));
                        Rectangle rect = new Rectangle(this.mHorizontalMargin, this.mVerticalMargin,
                                                        (int)(mImageOriginalWidth * newScale),
                                                        (int)(mImageOriginalHeight * newScale));
                        graphics.DrawImage(this.mOriginalBitmap, rect);
                    }
                    if(this.SizeMode == PictureBoxSizeMode.Zoom)
                    {
                        this.SizeMode = PictureBoxSizeMode.AutoSize;
                        this.Image.Dispose();
                        this.Image = bitmap;
                        this.Invalidate();
                    }
                    else
                    {
                        this.Image.Dispose();
                        this.Image = bitmap;
                        this.Invalidate();
                    }
                }
                else
                {
                    this.SuspendLayout();
                    this.Height = height;
                    this.Width = width;
                    if (this.SizeMode == PictureBoxSizeMode.AutoSize)
                    {
                        this.SizeMode = PictureBoxSizeMode.Zoom;
                        this.Invalidate();
                        this.ResumeLayout();
                    }
                    else
                    {
                        this.SizeMode = PictureBoxSizeMode.Zoom;
                        this.ResumeLayout();
                    }
                }
                mScale = newScale;
                //float scalingFactor = newScale / mScale;
                //mScalinigFactor = scalingFactor;
                //mScale = newScale;
                ////mTransform.Translate(-zoomPoint.X, -zoomPoint.Y, MatrixOrder.Append);
                //mTransform.Scale(scalingFactor, scalingFactor, MatrixOrder.Append);

                //zoomPoint.X = magnify ? - (int)((float)zoomPoint.X * scalingFactor) :  (int)((float)zoomPoint.X * scalingFactor);
                //zoomPoint.Y = magnify ?  - (int)((float)zoomPoint.Y * scalingFactor) :  (int)((float)zoomPoint.Y * scalingFactor);
                //mTransform.Translate(zoomPoint.X , zoomPoint.Y , MatrixOrder.Append);
                //this.Invalidate();
                //this.Height = (int)(mOriginalHeight * newScale);
                //this.Width = (int)(mOriginalWidth * newScale);
            }
        }
    }      
}
