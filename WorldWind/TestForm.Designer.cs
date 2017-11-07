namespace WorldWind
{
    partial class TestForm
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
            this.worldWindow = new WorldWind.WorldWindow();
            this.SuspendLayout();
            // 
            // worldWindow
            // 
            this.worldWindow.Cache = null;
            this.worldWindow.Caption = "";
            this.worldWindow.CurrentWorld = null;
            this.worldWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worldWindow.IsRenderDisabled = false;
            this.worldWindow.Location = new System.Drawing.Point(0, 0);
            this.worldWindow.Name = "worldWindow";
            this.worldWindow.Size = new System.Drawing.Size(561, 392);
            this.worldWindow.TabIndex = 0;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 392);
            this.Controls.Add(this.worldWindow);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private WorldWindow worldWindow;
    }
}