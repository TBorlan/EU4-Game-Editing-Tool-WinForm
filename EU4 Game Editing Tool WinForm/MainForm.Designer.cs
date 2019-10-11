namespace EU4_Game_Editing_Tool_WinForm
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cOpenModButton = new System.Windows.Forms.Button();
            this.cPictureBoxPanel = new System.Windows.Forms.Panel();
            this.cImagePictureBox = new EU4_Game_Editing_Tool_WinForm.ZoomablePictureBox();
            this.cPictureBoxPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cOpenModButton
            // 
            this.cOpenModButton.AutoSize = true;
            this.cOpenModButton.BackColor = System.Drawing.Color.Transparent;
            this.cOpenModButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cOpenModButton.FlatAppearance.BorderSize = 0;
            this.cOpenModButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cOpenModButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cOpenModButton.Location = new System.Drawing.Point(12, 12);
            this.cOpenModButton.Name = "cOpenModButton";
            this.cOpenModButton.Size = new System.Drawing.Size(101, 29);
            this.cOpenModButton.TabIndex = 0;
            this.cOpenModButton.TabStop = false;
            this.cOpenModButton.Text = "Open Mod";
            this.cOpenModButton.UseVisualStyleBackColor = false;
            this.cOpenModButton.Click += new System.EventHandler(this.Callback_OpenModButton_OnClick);
            // 
            // cPictureBoxPanel
            // 
            this.cPictureBoxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cPictureBoxPanel.AutoScroll = true;
            this.cPictureBoxPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cPictureBoxPanel.Controls.Add(this.cImagePictureBox);
            this.cPictureBoxPanel.Location = new System.Drawing.Point(12, 47);
            this.cPictureBoxPanel.Name = "cPictureBoxPanel";
            this.cPictureBoxPanel.Padding = new System.Windows.Forms.Padding(10);
            this.cPictureBoxPanel.Size = new System.Drawing.Size(1315, 470);
            this.cPictureBoxPanel.TabIndex = 3;
            // 
            // cImagePictureBox
            // 
            this.cImagePictureBox.Location = new System.Drawing.Point(0, 0);
            this.cImagePictureBox.Name = "cImagePictureBox";
            this.cImagePictureBox.Size = new System.Drawing.Size(100, 50);
            this.cImagePictureBox.TabIndex = 0;
            this.cImagePictureBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1339, 529);
            this.Controls.Add(this.cPictureBoxPanel);
            this.Controls.Add(this.cOpenModButton);
            this.Name = "MainForm";
            this.Text = "EU4 Menu";
            this.cPictureBoxPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Controls
        private System.Windows.Forms.Button cOpenModButton;
        private ZoomablePictureBox cImagePictureBox;
        private System.Windows.Forms.Panel cPictureBoxPanel;
        #endregion

    }
}

