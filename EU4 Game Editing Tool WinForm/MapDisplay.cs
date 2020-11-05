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
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

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

        #endregion

        #region Point Translations
        #endregion

        #region Methods
        #endregion
    }
}
