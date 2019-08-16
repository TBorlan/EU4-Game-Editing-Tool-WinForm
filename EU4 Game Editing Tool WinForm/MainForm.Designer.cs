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
            this.openFileButton = new System.Windows.Forms.Button();
            this.MapPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileButton
            // 
            this.openFileButton.Location = new System.Drawing.Point(12, 12);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(98, 29);
            this.openFileButton.TabIndex = 0;
            this.openFileButton.Text = "Open Image";
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this.OpenFileButtonClick);
            // 
            // MapPictureBox
            // 
            this.MapPictureBox.Location = new System.Drawing.Point(12, 47);
            this.MapPictureBox.Name = "MapPictureBox";
            this.MapPictureBox.Size = new System.Drawing.Size(1334, 470);
            this.MapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.MapPictureBox.TabIndex = 1;
            this.MapPictureBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1339, 529);
            this.Controls.Add(this.MapPictureBox);
            this.Controls.Add(this.openFileButton);
            this.Name = "MainForm";
            this.Text = "EU4 Menu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.PictureBox MapPictureBox;
    }
}

