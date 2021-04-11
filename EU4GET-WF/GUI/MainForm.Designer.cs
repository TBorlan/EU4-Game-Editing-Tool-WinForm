using EU4GET_WF.ImageRendering.Control;

namespace EU4GET_WF.GUI
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.cDisplayPanel = new EU4GET_WF.ImageRendering.Control.DisplayPanel();
            this.cToggleProvBordersButton = new System.Windows.Forms.Button();
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
            this.cOpenModButton.Size = new System.Drawing.Size(84, 29);
            this.cOpenModButton.TabIndex = 0;
            this.cOpenModButton.TabStop = false;
            this.cOpenModButton.Text = "Open Mod";
            this.cOpenModButton.UseVisualStyleBackColor = false;
            this.cOpenModButton.Click += new System.EventHandler(this.Callback_OpenModButton_Click);
            // 
            // cDisplayPanel
            // 
            this.cDisplayPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cDisplayPanel.Enabled = false;
            this.cDisplayPanel.Location = new System.Drawing.Point(12, 44);
            this.cDisplayPanel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.cDisplayPanel.mImage = null;
            this.cDisplayPanel.mMargins = new System.Drawing.Size(604, 270);
            this.cDisplayPanel.mScale = 0F;
            this.cDisplayPanel.Name = "cDisplayPanel";
            this.cDisplayPanel.Size = new System.Drawing.Size(1318, 476);
            this.cDisplayPanel.TabIndex = 1;
            this.cDisplayPanel.Visible = false;
            this.cDisplayPanel.Click += new System.EventHandler(this.Callback_ToggleProvBordersButton_Click);
            // 
            // cToggleProvBordersButton
            // 
            this.cToggleProvBordersButton.AutoSize = true;
            this.cToggleProvBordersButton.BackColor = System.Drawing.Color.Transparent;
            this.cToggleProvBordersButton.FlatAppearance.BorderSize = 0;
            this.cToggleProvBordersButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cToggleProvBordersButton.Location = new System.Drawing.Point(102, 12);
            this.cToggleProvBordersButton.Name = "cToggleProvBordersButton";
            this.cToggleProvBordersButton.Size = new System.Drawing.Size(175, 29);
            this.cToggleProvBordersButton.TabIndex = 2;
            this.cToggleProvBordersButton.Text = "Toggle Province Borders";
            this.cToggleProvBordersButton.UseVisualStyleBackColor = false;
            this.cToggleProvBordersButton.Click += new System.EventHandler(this.Callback_ToggleProvBordersButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1339, 529);
            this.Controls.Add(this.cToggleProvBordersButton);
            this.Controls.Add(this.cDisplayPanel);
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

        private DisplayPanel cDisplayPanel;
        private System.Windows.Forms.Button cToggleProvBordersButton;
    }
}

