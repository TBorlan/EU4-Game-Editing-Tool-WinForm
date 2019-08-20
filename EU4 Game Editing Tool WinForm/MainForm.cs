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

            mSelectColor = false;
        }

        private Bitmap mBitmapImage;

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

                this.cImagePictureBox.ImageLocation = loadImageDialog.FileName;
                this.cImagePictureBox.LoadAsync();
                mBitmapImage = new Bitmap(loadImageDialog.FileName);

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
                Point clickPoint = e.Location;
                clickPoint = ((ZoomablePictureBox)sender).PointToScreen(clickPoint);
                using(Bitmap screenImage = new Bitmap(1, 1))
                {
                    using(Graphics graphics = Graphics.FromImage(screenImage))
                    {
                        graphics.CopyFromScreen(clickPoint, new Point(0, 0), new Size(1, 1));
                    }
                    this.cColorPictureBox.BackColor = screenImage.GetPixel(0,0);
                }
            }
        }

        private void CImagePictureBox_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}
