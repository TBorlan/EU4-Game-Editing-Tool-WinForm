using System;
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
using EU4_Game_Editing_Tool_WinForm.FileParsing;
using System.IO;


namespace EU4_Game_Editing_Tool_WinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region Events and Delegates

        public delegate void ProvinceParsedEventHandler(object obj, ProvinceEventArgs args);

        public event ProvinceParsedEventHandler ProvincesParsed;

        #endregion

        #region Members

        private string mRootFolder;

        public Bitmap mBitmap;

        private Province[] _mProvinces = new Province[3662];

        public IReadOnlyList<Province> mProvinces { get => (Array.AsReadOnly<Province>(this._mProvinces)); }

        #endregion

        #region Callbacks

        private void Callback_OpenModButton_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            //if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            //{
            //    this.mRootFolder = folderBrowserDialog.SelectedPath;
            //}

            //folderBrowserDialog.Dispose();
            this.mRootFolder = @"C:\Users\nxf56462\Downloads\Phoenix 3 - DW 5.2\Phoenix";

            this.cImagePictureBox._mhScrollBar = this.hScrollBar1;
            this.cImagePictureBox._mvScrollBar = this.vScrollBar1;

            this.mBitmap = new Bitmap(this.mRootFolder + @"\map\provinces.bmp");

            this.cImagePictureBox.mOriginalBitmap = this.mBitmap;

            this.cImagePictureBox.MouseWheel += new MouseEventHandler(this.Callback_PictureBoxPanel_MouseWheel);

            this.ProvincesParsed += this.cImagePictureBox.Callback_MainForm_ProvincesParsed;

            //Lets do a quick province parsing
            using (StreamReader reader = File.OpenText(this.mRootFolder + @"\map\definition.csv"))
            {
                reader.ReadLine(); // skip the header
                int index = 0;
                while (reader.EndOfStream)
                {
                    string[] tokens = reader.ReadLine().Split(';');
                    if (tokens.Length >= 4)
                    {
                        _mProvinces[index] = new Province();
                        int.TryParse(tokens[0], out _mProvinces[index].id);
                        int r, g, b;
                        int.TryParse(tokens[1], out r);
                        int.TryParse(tokens[1], out g);
                        int.TryParse(tokens[1], out b);
                        _mProvinces[index].color = Color.FromArgb(r, g, b);
                    }
                }
            }
            //Invoke the province event and lets draw some borders
            OnProvincesParsed(new ProvinceEventArgs(mProvinces));
        }

        private void Callback_PictureBoxPanel_MouseWheel(object obj, MouseEventArgs args)
        {
            ((HandledMouseEventArgs)args).Handled = true;
        }

        #endregion

        #region Invokes

        protected virtual void OnProvincesParsed(ProvinceEventArgs args)
        {
            ProvinceParsedEventHandler @event = this.ProvincesParsed;
            @event?.Invoke(this, args);
        }

        #endregion

        #region Data Methods

        void LoadProvinceData()
        {

        }

        #endregion

        private void cPictureBoxPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
