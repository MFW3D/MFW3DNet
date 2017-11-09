using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MFW3D;
using MFW3D.Renderable;

namespace MFW3D
{
    /// <summary>
    /// GUI to select a file or url to load
    /// </summary>
	public class FileLoader : Form
	{
		private System.Windows.Forms.Label lblFileTextBox;
		private System.Windows.Forms.TextBox tbFileName;
		private System.Windows.Forms.Button btnChooseFile;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private CheckBox chbLoadPermanently;
        private ToolTip toolTip1;


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
            this.components = new System.ComponentModel.Container();
            this.lblFileTextBox = new System.Windows.Forms.Label();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnChooseFile = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.chbLoadPermanently = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblFileTextBox
            // 
            this.lblFileTextBox.AutoSize = true;
            this.lblFileTextBox.Location = new System.Drawing.Point(9, 9);
            this.lblFileTextBox.Name = "lblFileTextBox";
            this.lblFileTextBox.Size = new System.Drawing.Size(126, 13);
            this.lblFileTextBox.TabIndex = 0;
            this.lblFileTextBox.Text = "Enter URL or choose file:";
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(12, 25);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(333, 20);
            this.tbFileName.TabIndex = 1;
            this.tbFileName.TextChanged += new System.EventHandler(this.tbFileName_TextChanged);
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Location = new System.Drawing.Point(351, 24);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(71, 22);
            this.btnChooseFile.TabIndex = 2;
            this.btnChooseFile.Text = "Choose...";
            this.toolTip1.SetToolTip(this.btnChooseFile, "Browse for file.");
            this.btnChooseFile.UseVisualStyleBackColor = true;
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnLoad.Location = new System.Drawing.Point(266, 79);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.toolTip1.SetToolTip(this.btnLoad, "Load selected file.");
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(347, 79);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Close";
            this.toolTip1.SetToolTip(this.btnCancel, "Cancel.");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "WW add-ons (*.xml)|*.xml|WW plugins (*.cs)|*.cs|QuickInstall packages (*.zip)|*.z" +
                "ip";
            this.openFileDialog.Title = "Choose File";
            // 
            // chbLoadPermanently
            // 
            this.chbLoadPermanently.AutoSize = true;
            this.chbLoadPermanently.Checked = true;
            this.chbLoadPermanently.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbLoadPermanently.Enabled = false;
            this.chbLoadPermanently.Location = new System.Drawing.Point(12, 53);
            this.chbLoadPermanently.Name = "chbLoadPermanently";
            this.chbLoadPermanently.Size = new System.Drawing.Size(339, 17);
            this.chbLoadPermanently.TabIndex = 6;
            this.chbLoadPermanently.Text = "Load permanently: file(s) will be moved to the World Wind directory";
            this.chbLoadPermanently.UseVisualStyleBackColor = true;
            // 
            // FileLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.ClientSize = new System.Drawing.Size(434, 114);
            this.Controls.Add(this.chbLoadPermanently);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnChooseFile);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.lblFileTextBox);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.Name = "FileLoader";
            this.Text = "Load File";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		
		/// <summary>
		/// Constructor for the file loader GUI
		/// </summary>
		/// <param name="app"></param>
		public FileLoader()
		{
			InitializeComponent();
		}

        /// <summary>
        /// Constructor for the file loader GUI
        /// </summary>
        /// <param name="app"></param>
        public FileLoader(string fileName)
        {
            InitializeComponent();

            tbFileName.Text = fileName;
        }

		private void btnChooseFile_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() != DialogResult.Cancel)
				tbFileName.Text = openFileDialog.FileName;

            if (!tbFileName.Text.StartsWith("http://") && tbFileName.Text.EndsWith(".xml"))
            {
                chbLoadPermanently.Enabled = true;
                chbLoadPermanently.Checked = true;
            }
            else
            {
                chbLoadPermanently.Enabled = false;
                chbLoadPermanently.Checked = true;
            }
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			if (tbFileName.Text == "")
				return;

			if (tbFileName.Text.Trim().StartsWith(@"http://"))
			{
                Global.QuickInstall(tbFileName.Text);
			}
			else
			{
				if (!File.Exists(tbFileName.Text))
				{
					MessageBox.Show(tbFileName.Text + " does not exist", "Load error");
					return;
				}

                if (tbFileName.Text.EndsWith(".xml") && !chbLoadPermanently.Checked)
                {
                    RenderableObjectList layer = ConfigurationLoader.getRenderableFromLayerFile(tbFileName.Text,
                        Global.worldWindow.CurrentWorld,
                        Global.worldWindow.Cache);

                    Global.worldWindow.CurrentWorld.RenderableObjects.Add(layer);
                    MessageBox.Show("File loaded.");
                }
                else
                {
                    Global.QuickInstall(tbFileName.Text);
                }
			}

            this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

        private void tbFileName_TextChanged(object sender, EventArgs e)
        {
            if (!tbFileName.Text.StartsWith("http://") && tbFileName.Text.EndsWith(".xml"))
            {
                chbLoadPermanently.Enabled = true;
            }
            else
            {
                chbLoadPermanently.Enabled = false;
                chbLoadPermanently.Checked = true;
            }
        }

	}
}