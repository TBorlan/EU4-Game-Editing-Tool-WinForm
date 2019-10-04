﻿using System;
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

            _mSelectColor = false;
            this.cSelectColorButton.Click += new EventHandler(this.Callback_SelectColorButton_OnClick);
        }

        #region Members
        private bool _mSelectColor;

        public bool mSelectColor
        {
            get
            {
                return _mSelectColor;
            }
        }
        #endregion

        #region Callbacks
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
            this.cImagePictureBox.Click += new EventHandler(this.Callback_ImagePictureBox_OnClick); 
        }

        private void Callback_PictureBoxPanel_MouseWheel(object obj, MouseEventArgs args)
        {
            ((HandledMouseEventArgs)args).Handled = true;
        }

        private void Callback_SelectColorButton_OnClick(object sender, EventArgs e)
        {
            if (this.cImagePictureBox.mOriginalBitmap != null)
            {
                _mSelectColor = true;
                this.cImagePictureBox.Cursor = Cursors.Hand;
            }

        }

        private void Callback_ImagePictureBox_OnClick(object sender, EventArgs e)
        {
            if (this.mSelectColor)
            {
                using (Bitmap pixelImage = new Bitmap(1,1))
                {
                    using (Graphics graphics = Graphics.FromImage(pixelImage))
                    {
                        graphics.CopyFromScreen(Control.MousePosition, new Point(0, 0), new Size(1,1));

                        Point point = ((MouseEventArgs)(e)).Location;

                    }
                    this.cColorPictureBox.BackColor = pixelImage.GetPixel(0, 0);
                    this.cImagePictureBox.DrawBorder(pixelImage.GetPixel(0, 0));
                }
            }
        }

        private void CImagePictureBox_MouseClick(object sender, MouseEventArgs e)
        {

        }
        #endregion
    }
}
