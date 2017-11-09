namespace MFW3D.GeoRSS
{
    partial class GeoRssForm
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
            this.geoRSSFeedControl1 = new MFW3D.GeoRSS.GeoRSSFeedControl();
            this.SuspendLayout();
            // 
            // geoRSSFeedControl1
            // 
            this.geoRSSFeedControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.geoRSSFeedControl1.Location = new System.Drawing.Point(0, 0);
            this.geoRSSFeedControl1.Name = "geoRSSFeedControl1";
            this.geoRSSFeedControl1.Size = new System.Drawing.Size(430, 411);
            this.geoRSSFeedControl1.TabIndex = 0;
            // 
            // GeoRssForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 411);
            this.Controls.Add(this.geoRSSFeedControl1);
            this.Name = "GeoRssForm";
            this.Text = "GeoRssForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GeoRssForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private GeoRSSFeedControl geoRSSFeedControl1;
    }
}