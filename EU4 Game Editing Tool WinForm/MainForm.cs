using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_Game_Editing_Tool_WinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private Bitmap mBitmapImage;


        private void Callback_OpenImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadImageDialog = new OpenFileDialog()
            {
                Filter = "Image Files (*.bmp)|*.bmp",
                InitialDirectory = "C:/Users/nxf56462/Downloads/Phoenix 3 - DW 5.2/Phoenix"

            };

            if(loadImageDialog.ShowDialog() == DialogResult.OK)
            {

                this.cImagePictureBox.ImageLocation = loadImageDialog.FileName;
                this.cImagePictureBox.LoadAsync();
                mBitmapImage = new Bitmap(loadImageDialog.FileName);

            }

            loadImageDialog.Dispose();
            this.cZoomInButton.Enabled = true;
            this.cZoomOutButton.Enabled = true;

            
        }

        private void Callback_ZoomInButton_Click(object sender, EventArgs e)
        {
            //if (!this.mBitmapImage.ZoomIn())
            //{
            //    this.cZoomInButton.Enabled = false;
            //}
            //else
            //{
            //    this.cImagePictureBox.Image = this.mBitmapImage.Image;
            //    this.cImagePictureBox.Invalidate();
            //    this.cZoomOutButton.Enabled = true;
            //}
            if (!this.cImagePictureBox.ZoomIn())
            {
                this.cZoomInButton.Enabled = false;
            }
            this.cZoomOutButton.Enabled = true;
        }

        private void Callback_ZoomOutButton_Click(object sender, EventArgs e)
        {
            if (!this.cImagePictureBox.ZoomOut())
            {
                this.cZoomOutButton.Enabled = false;
            }
            this.cZoomInButton.Enabled = true;
        }
        
    }
}
