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


namespace EU4_Game_Editing_Tool_WinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region Events and Delegates

        public event EventHandler ProvincesParsed;

        #endregion

        #region Members

        private string mRootFolder;

        private Province[] _mProvinces;

        public IReadOnlyList<Province> mProvinces { get => (Array.AsReadOnly<Province>(this._mProvinces)); }

        #endregion

        #region Callbacks

        private void Callback_OpenModButton_Click(object sender, EventArgs e)
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

            this._mProvinces = DefinitionParser.ReadAllElements(reader);

            DefinitionParser.CloseReader(reader);

            this.OnProvincesParsed(null);
        }

        private void Callback_PictureBoxPanel_MouseWheel(object obj, MouseEventArgs args)
        {
            ((HandledMouseEventArgs)args).Handled = true;
        }

        #endregion

        #region Invokes

        protected virtual void OnProvincesParsed(EventArgs args)
        {
            EventHandler @event = this.ProvincesParsed;
            @event?.Invoke(this, args);
        }

        #endregion
    }
}
