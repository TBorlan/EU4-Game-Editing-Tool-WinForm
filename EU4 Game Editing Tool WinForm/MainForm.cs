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

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.DrawString("some text", this.Font, Brushes.AliceBlue, 0, 0);
            BackColor = Color.Black;
        }

        public OpenFileDialog loadImageDialog = new OpenFileDialog()
        {
            Filter = "Image Files (*.bmp)|*.bmp",
            InitialDirectory = "C:/Users/nxf56462/Downloads/Phoenix 3 - DW 5.2/Phoenix"
        };

        private void OpenFileButtonClick(object sender, EventArgs args)
        {
            if(loadImageDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Uri filePath = new Uri(loadImageDialog.FileName);
                    Image provinces = Image.FromFile(loadImageDialog.FileName);
                    MapPictureBox.Image = provinces;
                    
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }
        }

    }
}
