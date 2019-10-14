﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        }

        #region Members

        private string mRootFolder;

        private Province[] mProvinces;

        #endregion

        #region Callbacks

        private void Callback_OpenModButton_OnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.mRootFolder = folderBrowserDialog.SelectedPath;
            }

            folderBrowserDialog.Dispose();

            this.cImagePictureBox.mOriginalBitmap = new Bitmap(this.mRootFolder + @"\map\provinces.bmp");

            this.cImagePictureBox.MouseWheel += new MouseEventHandler(this.Callback_PictureBoxPanel_MouseWheel);

            StreamReader reader = DefinitionParser.GetReader(this.mRootFolder + @"\map\definition.csv");

            this.mProvinces = DefinitionParser.ReadAllElements(reader);

            DefinitionParser.CloseReader(reader);
        }

        private void Callback_PictureBoxPanel_MouseWheel(object obj, MouseEventArgs args)
        {
            ((HandledMouseEventArgs)args).Handled = true;
        }

        #endregion
    }
}
