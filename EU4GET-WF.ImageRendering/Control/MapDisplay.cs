using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace EU4GET_WF.ImageRendering.Control
{
    ///<summary>
    ///Helper Class for displaying images
    ///</summary>
    public partial class MapDisplay : System.Windows.Forms.Control
    {
        #region Members

        private Point _mPanPoint;

        private bool _mMiddlePressed;

        private Bitmap _mOriginalBitmap;

        private readonly object _mLockObject = new object();

        public event EventHandler<Point> Pan;

        public event MouseEventHandler Zoom;

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
                    lock (this._mLockObject)
                    {
                        this._mOriginalBitmap = value;
                    }
                    this.SetStyle(ControlStyles.UserPaint 
                                  | ControlStyles.AllPaintingInWmPaint 
                                  | ControlStyles.OptimizedDoubleBuffer,
                                  true);
                    this.Enabled = true;
                    this.Visible = true;
                }
            }
        }

        #endregion

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                this._mPanPoint = new Point((Size)(e.Location));
                this._mMiddlePressed = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this._mMiddlePressed)
            {
                Point offset = new Point((Size)(this._mPanPoint) - (Size)(e.Location));
                this._mPanPoint = e.Location;
                this.Pan?.Invoke(this, offset);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this._mMiddlePressed)
            {
                if(e.Button == MouseButtons.Middle)
                {
                    this._mMiddlePressed = false;
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            base.OnPaint(e);
            
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.Zoom?.Invoke(this, e);
            base.OnMouseWheel(e);
        }
    }
}
