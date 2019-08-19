using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_Game_Editing_Tool_WinForm
{
    public partial class ZoomablePictureBox : PictureBox
    {

        private float mScale;

        private Bitmap mOriginalBitmapImage;

        protected override void OnLoadCompleted(AsyncCompletedEventArgs e)
        {
            base.OnLoadCompleted(e);
            mOriginalBitmapImage = (Bitmap)this.Image;
            mScale = 1;

        }

        public bool ZoomIn()
        {
            if(this.Image == null)
            {
                return false;
            }
            mScale *= 2;
            this.Image.Dispose();
            Size newSize = new Size((int)(mOriginalBitmapImage.Width * mScale), (int)(mOriginalBitmapImage.Height * mScale));
            if (Math.Max(newSize.Width,newSize.Height)> 23000)
            {
                mScale /= 2;
                return false;
            }
            else
            {
                this.Image = new Bitmap(this.mOriginalBitmapImage, newSize);
            }
            return true;
        }

        public bool ZoomOut()
        {
            {
                if (this.Image == null)
                {
                    return false;
                }
                mScale /= 2;
                this.Image.Dispose();
                Size newSize = new Size((int)(mOriginalBitmapImage.Width * mScale), (int)(mOriginalBitmapImage.Height * mScale));
                if (Math.Min(newSize.Width, newSize.Height) < 200)
                {
                    mScale *= 2;
                    return false;
                }
                else
                {
                    this.Image = new Bitmap(this.mOriginalBitmapImage, newSize);
                }
                return true;
            }
        }

    }
       
}
