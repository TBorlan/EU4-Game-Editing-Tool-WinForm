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

        private async void Callback_ToggleProvBordersButton_Click(object sender, EventArgs args)
        {
            if (this.cDisplayPanel.mImage != null)
            {
                if (((Button)sender).Enabled)
                {
                    ((Button)sender).Enabled = false;
                    await this.cDisplayPanel._mSelectionManager.ToggleProvinceBorderPaths();
                    ((Button)sender).Enabled = true;
                }
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
