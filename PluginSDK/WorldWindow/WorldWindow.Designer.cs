using System;

namespace WorldWind
{
    partial class WorldWindow
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

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (m_WorkerThread != null && m_WorkerThread.IsAlive)
                {
                    m_WorkerThreadRunning = false;
                    m_WorkerThread.Abort();
                }

                m_FpsTimer.Stop();
                if (m_World != null)
                {
                    m_World.Dispose();
                    m_World = null;
                }
                if (this.drawArgs != null)
                {
                    this.drawArgs.Dispose();
                    this.drawArgs = null;
                }
                if (this._menuBar != null)
                {
                    this._menuBar.Dispose();
                    this._menuBar = null;
                }

                m_Device3d.Dispose();

                //if (m_downloadIndicator != null)
                //{
                //    m_downloadIndicator.Dispose();
                //    m_downloadIndicator = null;
                //}

            }

            base.Dispose(disposing);
            GC.SuppressFinalize(this);

        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WorldWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "WorldWindow";
            this.Size = new System.Drawing.Size(586, 606);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
