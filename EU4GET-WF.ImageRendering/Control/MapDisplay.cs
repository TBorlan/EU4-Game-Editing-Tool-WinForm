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

        /// <summary>
        /// Start <see cref="Point"/> of the pan operation.
        /// </summary>
        private Point _mPanPoint;

        /// <summary>
        /// Flag used to determine if a pan operation is ongoing.
        /// </summary>
        private bool _mMiddlePressed;

        private Bitmap _mOriginalBitmap;

        private readonly object _mLockObject = new object();

        /// <summary>
        /// Event raised when mouse is moved while the middle mouse button is pressed.
        /// </summary>
        public event EventHandler<Point> Pan;

        /// <summary>
        /// Event raised when user turns the mouse wheel while the mouse pointer
        /// is in the <see cref="MapDisplay"/> control zone.
        /// </summary>
        public event MouseEventHandler Zoom;

        /// <summary>
        /// Bitmap which shows the provinces layout
        /// </summary>
        /// <value>Get a reference of the underlying <see cref="Bitmap"/>.</value>
        /// <remarks>When setting the bitmap, the control becomes visible.</remarks>
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
                    //NOTE: Why do we need lock here?
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

        /// <summary>
        /// Raise the <see cref="Pan"/> event if <see cref="_mPanPoint"/> flag is set.
        /// </summary>
        /// <param name="e">Contains info about pan size and direction.</param>
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

        /// <summary>
        /// Raise the <see cref="Zoom"/> event.
        /// </summary>
        /// <param name="e">Contains mouse wheel rotation magnitude and sign.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.Zoom?.Invoke(this, e);
            base.OnMouseWheel(e);
        }
    }
}
