using System;
using System.Drawing;
using System.Windows.Forms;

namespace EU4GET_WF.GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.InitializeComponent();
        }

        #region Events and Delegates

        public event EventHandler ProvincesParsed;

        #endregion

        #region Callbacks

        private void Callback_OpenModButton_Click(object sender, EventArgs e)
        {
            
            
            using (Bitmap image = new Bitmap(@"C:\Users\Tudor\Desktop\temp\provinces.bmp"))
            {
                this.cDisplayPanel.mImage = image;
            }

            
        }

        #endregion

        #region Invokes

        protected virtual void OnProvincesParsed(EventArgs args)
        {
            EventHandler @event = this.ProvincesParsed;
            @event?.Invoke(this, args);
        }

        #endregion

        #region Data Methods

        #endregion
    }
}
