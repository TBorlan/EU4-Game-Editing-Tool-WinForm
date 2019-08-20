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

        private Matrix mTransform;

        protected override void OnLoadCompleted(AsyncCompletedEventArgs e)
        {
            base.OnLoadCompleted(e);
            mScale = 1.0f;
            if (mTransform != null)
            {
                mTransform.Dispose();
            }
            mTransform = new Matrix();

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics graphics = pe.Graphics;
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            if (mTransform != null)
            {
                graphics.Transform = mTransform;
            }
            base.OnPaint(pe);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.Focus();
            if(this.Focused && e.Delta != 0 && this.Image!=null)
            {
                Point zoomPoint = this.PointToClient(this.PointToScreen(e.Location));
                Zoom(zoomPoint, e.Delta > 0);
            }
        }

        private void Zoom(Point zoomPoint, bool magnify)
        {
            float newScale = Math.Min(10f, Math.Max(0.1f, mScale + (magnify ? 0.1f : -0.1f)));

            if (newScale != mScale)
            {
                float scalingFactor = newScale / mScale;
                mScale = newScale;
                mTransform.Translate(-zoomPoint.X, -zoomPoint.Y, MatrixOrder.Append);
                mTransform.Scale(scalingFactor, scalingFactor, MatrixOrder.Append);
                mTransform.Translate(zoomPoint.X, zoomPoint.Y, MatrixOrder.Append);
                this.Invalidate();
            }
        }
    }      
}
