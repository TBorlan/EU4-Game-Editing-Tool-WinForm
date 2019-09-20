using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;


namespace EU4_Game_Editing_Tool_WinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            mSelectColor = false;


        }

 

        private bool mSelectColor;

        private void Callback_OpenImageButton_OnClick(object sender, EventArgs e)
        {
            OpenFileDialog loadImageDialog = new OpenFileDialog()
            {
                Filter = "Image Files (*.bmp)|*.bmp",
                InitialDirectory = "C:/Users/nxf56462/Downloads/Phoenix 3 - DW 5.2/Phoenix"

            };

            if(loadImageDialog.ShowDialog() == DialogResult.OK)
            {

                //this.cImagePictureBox.ImageLocation = loadImageDialog.FileName;
                //this.cImagePictureBox.LoadAsync();
                this.cImagePictureBox.mOriginalBitmap = new Bitmap(loadImageDialog.FileName); ;

            }

            loadImageDialog.Dispose();

            this.cImagePictureBox.MouseWheel += new MouseEventHandler(this.Callback_PictureBoxPanel_MouseWheel);
        }


        private void Callback_PictureBoxPanel_MouseWheel(object obj, MouseEventArgs args)
        {
            ((HandledMouseEventArgs)args).Handled = true;
        }

        private void Callback_SelectColorButton_OnClick(object sender, EventArgs e)
        {
            if (this.cImagePictureBox.Image != null)
            {
                mSelectColor = true;
                this.cImagePictureBox.Cursor = Cursors.Hand;
            }

        }

        private void Callback_ImagePictureBox_OnClick(object sender, MouseEventArgs e)
        {
            if (this.mSelectColor)
            {
                using (Bitmap pixelImage = new Bitmap(1,1))
                {
                    using (Graphics graphics = Graphics.FromImage(pixelImage))
                    {
                        graphics.CopyFromScreen(Control.MousePosition, new Point(0, 0), new Size(1,1));
<<<<<<< HEAD
                        Point point = e.Location;
=======
>>>>>>> 3830d77b195546f1530d70306e2a77ef398cfda3
                    }
                    this.cColorPictureBox.BackColor = pixelImage.GetPixel(0, 0);
                }
            }
        }

        private void CImagePictureBox_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}
