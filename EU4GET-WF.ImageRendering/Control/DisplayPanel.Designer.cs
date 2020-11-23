namespace EU4GET_WF.ImageRendering.Control
{
    partial class DisplayPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cVScrollBar = new System.Windows.Forms.VScrollBar();
            this.cHScrollBar = new System.Windows.Forms.HScrollBar();
            this.cMapDisplay = new MapDisplay();
            this.SuspendLayout();
            // 
            // cVScrollBar
            // 
            this.cVScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cVScrollBar.Location = new System.Drawing.Point(1209, 0);
            this.cVScrollBar.Margin = new System.Windows.Forms.Padding(0, 0, 0, 21);
            this.cVScrollBar.Name = "cVScrollBar";
            this.cVScrollBar.Size = new System.Drawing.Size(21, 540);
            this.cVScrollBar.TabIndex = 1;
            // 
            // cHScrollBar
            // 
            this.cHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cHScrollBar.Location = new System.Drawing.Point(0, 540);
            this.cHScrollBar.Margin = new System.Windows.Forms.Padding(0, 0, 21, 0);
            this.cHScrollBar.Name = "cHScrollBar";
            this.cHScrollBar.Size = new System.Drawing.Size(1209, 21);
            this.cHScrollBar.TabIndex = 2;
            // 
            // cMapDisplay
            // 
            this.cMapDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cMapDisplay.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cMapDisplay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cMapDisplay.Location = new System.Drawing.Point(0, 0);
            this.cMapDisplay.Margin = new System.Windows.Forms.Padding(0);
            this.cMapDisplay.mOriginalBitmap = null;
            this.cMapDisplay.Name = "cMapDisplay";
            this.cMapDisplay.Size = new System.Drawing.Size(1209, 540);
            this.cMapDisplay.TabIndex = 0;
            // 
            // DisplayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cHScrollBar);
            this.Controls.Add(this.cVScrollBar);
            this.Controls.Add(this.cMapDisplay);
            this.Name = "DisplayPanel";
            this.Size = new System.Drawing.Size(1230, 561);
            this.ResumeLayout(false);

        }

        #endregion

        internal MapDisplay cMapDisplay;
        internal System.Windows.Forms.VScrollBar cVScrollBar;
        internal System.Windows.Forms.HScrollBar cHScrollBar;
    }
}
