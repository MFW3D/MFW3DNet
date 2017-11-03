using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Renderable;
using Utility;

namespace WorldWind
{
	/// <summary>
	/// Summary description for PlaceBuilder.
	/// </summary>
	public class PlaceBuilder : System.Windows.Forms.Form
	{
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.TextBox textBoxSaveFilePath;
		private System.Windows.Forms.Label labelSavePath;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelLatitude;
		private System.Windows.Forms.Label labelLongitude;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonBuild;
		private System.Windows.Forms.TextBox textBoxColumn;
		private System.Windows.Forms.TextBox textBoxSourceFile;
		private System.Windows.Forms.Label labelSource;
		private System.Windows.Forms.StatusBar statusBar;
		private WorldWindow worldWindow;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.PlaceBuilder"/> class.
		/// </summary>
		/// <param name="ww"></param>
		public PlaceBuilder( WorldWindow ww )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.worldWindow = ww;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.textBoxSaveFilePath = new System.Windows.Forms.TextBox();
            this.labelSavePath = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.labelLatitude = new System.Windows.Forms.Label();
            this.labelLongitude = new System.Windows.Forms.Label();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonBuild = new System.Windows.Forms.Button();
            this.textBoxColumn = new System.Windows.Forms.TextBox();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.textBoxSourceFile = new System.Windows.Forms.TextBox();
            this.labelSource = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxSaveFilePath
            // 
            this.textBoxSaveFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSaveFilePath.Location = new System.Drawing.Point(8, 78);
            this.textBoxSaveFilePath.Name = "textBoxSaveFilePath";
            this.textBoxSaveFilePath.Size = new System.Drawing.Size(277, 21);
            this.textBoxSaveFilePath.TabIndex = 3;
            // 
            // labelSavePath
            // 
            this.labelSavePath.Location = new System.Drawing.Point(8, 60);
            this.labelSavePath.Name = "labelSavePath";
            this.labelSavePath.Size = new System.Drawing.Size(120, 18);
            this.labelSavePath.TabIndex = 2;
            this.labelSavePath.Text = "&Save File Path:";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Location = new System.Drawing.Point(8, 198);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(277, 85);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(18, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "Feature Name";
            // 
            // labelLatitude
            // 
            this.labelLatitude.Location = new System.Drawing.Point(18, 146);
            this.labelLatitude.Name = "labelLatitude";
            this.labelLatitude.Size = new System.Drawing.Size(120, 25);
            this.labelLatitude.TabIndex = 5;
            this.labelLatitude.Text = "Latitude";
            // 
            // labelLongitude
            // 
            this.labelLongitude.Location = new System.Drawing.Point(18, 172);
            this.labelLongitude.Name = "labelLongitude";
            this.labelLongitude.Size = new System.Drawing.Size(120, 25);
            this.labelLongitude.TabIndex = 6;
            this.labelLongitude.Text = "Longitude";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.Location = new System.Drawing.Point(199, 292);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(90, 25);
            this.buttonAdd.TabIndex = 10;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonBuild
            // 
            this.buttonBuild.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBuild.Location = new System.Drawing.Point(152, 121);
            this.buttonBuild.Name = "buttonBuild";
            this.buttonBuild.Size = new System.Drawing.Size(133, 69);
            this.buttonBuild.TabIndex = 7;
            this.buttonBuild.Text = "&Build";
            this.buttonBuild.Click += new System.EventHandler(this.buttonBuild_Click);
            // 
            // textBoxColumn
            // 
            this.textBoxColumn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxColumn.Location = new System.Drawing.Point(8, 292);
            this.textBoxColumn.Name = "textBoxColumn";
            this.textBoxColumn.Size = new System.Drawing.Size(181, 21);
            this.textBoxColumn.TabIndex = 9;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 323);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(296, 24);
            this.statusBar.TabIndex = 11;
            // 
            // textBoxSourceFile
            // 
            this.textBoxSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSourceFile.Location = new System.Drawing.Point(8, 26);
            this.textBoxSourceFile.Name = "textBoxSourceFile";
            this.textBoxSourceFile.Size = new System.Drawing.Size(277, 21);
            this.textBoxSourceFile.TabIndex = 1;
            // 
            // labelSource
            // 
            this.labelSource.Location = new System.Drawing.Point(8, 9);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(120, 17);
            this.labelSource.TabIndex = 0;
            this.labelSource.Text = "S&ource File:";
            // 
            // PlaceBuilder
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(296, 347);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.textBoxSourceFile);
            this.Controls.Add(this.textBoxColumn);
            this.Controls.Add(this.textBoxSaveFilePath);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.buttonBuild);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.labelLongitude);
            this.Controls.Add(this.labelLatitude);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.labelSavePath);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(250, 336);
            this.Name = "PlaceBuilder";
            this.Text = "Place Builder";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(this.textBoxColumn.Text.Length > 0)
				{
					this.listView1.Items.Add(this.textBoxColumn.Text);
				}
			}
			catch
			{}
		}

		private void buttonBuild_Click(object sender, System.EventArgs e)
		{
			try
			{
				Thread t = new Thread(new ThreadStart(BuilderFunc));
				t.Name = "PlaceBuilder.BuilderFunc";
				t.IsBackground = true;
				t.Start();
			}
			catch
			{}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			this.Visible = false;
			this.worldWindow.Focus();
			base.OnClosing (e);
		}

		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) 
		{
			switch(e.KeyCode) 
			{
				case Keys.Escape:
					Close();
					e.Handled = true;
					break;
				case Keys.F4:
					if(e.Modifiers==Keys.Control)
					{
						Close();
						e.Handled = true;
					}
					break;
			}

			base.OnKeyUp(e);
		}
		
		delegate
			void updateStatusBarDelegate(string statusMsg);

		/// <summary>
		/// Updates status bar message (thread safe)
		/// </summary>
		private void updateStatusBar(string statusMsg) 
		{
			// Make sure we're on the right thread
			if( this.InvokeRequired ) 
			{
				// Update asynchronously
				updateStatusBarDelegate dlgt = new updateStatusBarDelegate(updateStatusBar);
				this.BeginInvoke(dlgt, new object[] { statusMsg });
				return;
			}

			this.statusBar.Text = statusMsg;
		}

		private void BuilderFunc()
		{
			// Warning: not running in UI thread
			try
			{
				updateStatusBar("Building...");
				if(!File.Exists(this.textBoxSourceFile.Text))
				{
					updateStatusBar("Source File Not Found.");
					return;
				}
			
				string[] additionalKeys = new string[this.listView1.Items.Count];
				for(int i = 0; i < this.listView1.Items.Count; i++)
				{
					additionalKeys[i] = this.listView1.Items[i].Text;
				}

				WorldWindPlacenameList pl = new WorldWindPlacenameList();

				using (StreamReader reader = new StreamReader(this.textBoxSourceFile.Text))
				{
					int counter = 0;
					string line = reader.ReadLine();
					while (line != null)
					{
						string[] lineParts = line.Split('\t');

						if (lineParts.Length != additionalKeys.Length + 3)
						{
							updateStatusBar("Invalid source file.");
							return;
						}

						string name = lineParts[0];
						float lat = Single.Parse(lineParts[1], CultureInfo.InvariantCulture);
						float lon = Single.Parse(lineParts[2], CultureInfo.InvariantCulture);

						Hashtable metaData = new Hashtable(additionalKeys.Length);

						for (int i = 0; i < additionalKeys.Length; i++)
						{
							metaData.Add(additionalKeys[i], lineParts[3 + i]);
						}

						pl.AddPlacename(name, lat, lon, metaData);

						line = reader.ReadLine();
						counter++;
						if (counter % 1000 == 0)
							updateStatusBar(counter.ToString());
					}
				}

				pl.SavePlacenameList(this.textBoxSaveFilePath.Text);

				updateStatusBar("Done.");
			}
			catch(Exception caught)
			{
				updateStatusBar(caught.Message);
				Log.Write(caught);
			}
		}
	}
}