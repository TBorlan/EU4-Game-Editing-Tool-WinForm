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
            this.cOpenModButton.Click += new System.EventHandler(this.Callback_OpenModButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1339, 529);
            this.Controls.Add(this.cOpenModButton);
            this.Name = "MainForm";
            this.Text = "EU4 Menu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Controls
        private System.Windows.Forms.Button cOpenModButton;
        #endregion

    }
}

