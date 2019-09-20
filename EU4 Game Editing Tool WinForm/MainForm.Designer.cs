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
            this.cOpenImageButton = new System.Windows.Forms.Button();
            this.cPictureBoxPanel = new System.Windows.Forms.Panel();
            this.cColorPictureBox = new System.Windows.Forms.PictureBox();
            this.cSelectColorButton = new System.Windows.Forms.Button();
            this.cImagePictureBox = new EU4_Game_Editing_Tool_WinForm.ZoomablePictureBox();
            this.cPictureBoxPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cColorPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cOpenImageButton
            // 
            this.cOpenImageButton.AutoSize = true;
            this.cOpenImageButton.BackColor = System.Drawing.Color.Transparent;
            this.cOpenImageButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cOpenImageButton.FlatAppearance.BorderSize = 0;
            this.cOpenImageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cOpenImageButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cOpenImageButton.Location = new System.Drawing.Point(12, 12);
            this.cOpenImageButton.Name = "cOpenImageButton";
            this.cOpenImageButton.Size = new System.Drawing.Size(101, 29);
            this.cOpenImageButton.TabIndex = 0;
            this.cOpenImageButton.TabStop = false;
            this.cOpenImageButton.Text = "Open Picture";
            this.cOpenImageButton.UseVisualStyleBackColor = false;
            this.cOpenImageButton.Click += new System.EventHandler(this.Callback_OpenImageButton_OnClick);
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
            // cColorPictureBox
            // 
            this.cColorPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cColorPictureBox.Location = new System.Drawing.Point(226, 12);
            this.cColorPictureBox.Name = "cColorPictureBox";
            this.cColorPictureBox.Size = new System.Drawing.Size(100, 29);
            this.cColorPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.cColorPictureBox.TabIndex = 4;
            this.cColorPictureBox.TabStop = false;
            // 
            // cSelectColorButton
            // 
            this.cSelectColorButton.AutoSize = true;
            this.cSelectColorButton.BackColor = System.Drawing.Color.Transparent;
            this.cSelectColorButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cSelectColorButton.FlatAppearance.BorderSize = 0;
            this.cSelectColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cSelectColorButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cSelectColorButton.Location = new System.Drawing.Point(119, 12);
            this.cSelectColorButton.Name = "cSelectColorButton";
            this.cSelectColorButton.Size = new System.Drawing.Size(101, 29);
            this.cSelectColorButton.TabIndex = 5;
            this.cSelectColorButton.TabStop = false;
            this.cSelectColorButton.Text = "Select Color";
            this.cSelectColorButton.UseVisualStyleBackColor = false;
            this.cSelectColorButton.Click += new System.EventHandler(this.Callback_SelectColorButton_OnClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1339, 529);
            this.Controls.Add(this.cSelectColorButton);
            this.Controls.Add(this.cColorPictureBox);
            this.Controls.Add(this.cPictureBoxPanel);
            this.Controls.Add(this.cOpenImageButton);
            this.Name = "MainForm";
            this.Text = "EU4 Menu";
            this.cPictureBoxPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cOpenImageButton;
        private ZoomablePictureBox cImagePictureBox;
        private System.Windows.Forms.Panel cPictureBoxPanel;
        private System.Windows.Forms.PictureBox cColorPictureBox;
        private System.Windows.Forms.Button cSelectColorButton;
    }
}

