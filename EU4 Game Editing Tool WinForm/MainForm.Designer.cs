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
            this.cImagePictureBox = new ZoomablePictureBox();
            this.cZoomInButton = new System.Windows.Forms.Button();
            this.cPictureBoxPanel = new System.Windows.Forms.Panel();
            this.cZoomOutButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cImagePictureBox)).BeginInit();
            this.cPictureBoxPanel.SuspendLayout();
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
            this.cOpenImageButton.Click += new System.EventHandler(this.Callback_OpenImageButton_Click);
            // 
            // cImagePictureBox
            // 
            this.cImagePictureBox.BackColor = System.Drawing.Color.Transparent;
            this.cImagePictureBox.Location = new System.Drawing.Point(0, 0);
            this.cImagePictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.cImagePictureBox.Name = "cImagePictureBox";
            this.cImagePictureBox.Size = new System.Drawing.Size(5616, 2160);
            this.cImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.cImagePictureBox.TabIndex = 2;
            this.cImagePictureBox.TabStop = false;
            // 
            // cZoomInButton
            // 
            this.cZoomInButton.AutoSize = true;
            this.cZoomInButton.BackColor = System.Drawing.Color.Transparent;
            this.cZoomInButton.Enabled = false;
            this.cZoomInButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cZoomInButton.FlatAppearance.BorderSize = 0;
            this.cZoomInButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cZoomInButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cZoomInButton.Location = new System.Drawing.Point(119, 12);
            this.cZoomInButton.Name = "cZoomInButton";
            this.cZoomInButton.Size = new System.Drawing.Size(101, 29);
            this.cZoomInButton.TabIndex = 0;
            this.cZoomInButton.TabStop = false;
            this.cZoomInButton.Text = "Zoom In";
            this.cZoomInButton.UseVisualStyleBackColor = false;
            this.cZoomInButton.Click += new System.EventHandler(this.Callback_ZoomInButton_Click);
            // 
            // cPictureBoxPanel
            // 
            this.cPictureBoxPanel.AutoScroll = true;
            this.cPictureBoxPanel.Controls.Add(this.cImagePictureBox);
            this.cPictureBoxPanel.Location = new System.Drawing.Point(12, 47);
            this.cPictureBoxPanel.Name = "cPictureBoxPanel";
            this.cPictureBoxPanel.Padding = new System.Windows.Forms.Padding(10);
            this.cPictureBoxPanel.Size = new System.Drawing.Size(1315, 470);
            this.cPictureBoxPanel.TabIndex = 3;
            // 
            // cZoomOutButton
            // 
            this.cZoomOutButton.AutoSize = true;
            this.cZoomOutButton.BackColor = System.Drawing.Color.Transparent;
            this.cZoomOutButton.Enabled = false;
            this.cZoomOutButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cZoomOutButton.FlatAppearance.BorderSize = 0;
            this.cZoomOutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cZoomOutButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cZoomOutButton.Location = new System.Drawing.Point(226, 12);
            this.cZoomOutButton.Name = "cZoomOutButton";
            this.cZoomOutButton.Size = new System.Drawing.Size(101, 29);
            this.cZoomOutButton.TabIndex = 4;
            this.cZoomOutButton.TabStop = false;
            this.cZoomOutButton.Text = "Zoom Out";
            this.cZoomOutButton.UseVisualStyleBackColor = false;
            this.cZoomOutButton.Click += new System.EventHandler(this.Callback_ZoomOutButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1339, 529);
            this.Controls.Add(this.cZoomOutButton);
            this.Controls.Add(this.cPictureBoxPanel);
            this.Controls.Add(this.cZoomInButton);
            this.Controls.Add(this.cOpenImageButton);
            this.Name = "MainForm";
            this.Text = "EU4 Menu";
            ((System.ComponentModel.ISupportInitialize)(this.cImagePictureBox)).EndInit();
            this.cPictureBoxPanel.ResumeLayout(false);
            this.cPictureBoxPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cOpenImageButton;
        private ZoomablePictureBox cImagePictureBox;
        private System.Windows.Forms.Button cZoomInButton;
        private System.Windows.Forms.Panel cPictureBoxPanel;
        private System.Windows.Forms.Button cZoomOutButton;
    }
}

