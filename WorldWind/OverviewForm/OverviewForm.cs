using System;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace WorldWind.CMPlugins.OverviewForm
{
	/// <summary>
	/// Summary description for OverviewForm.
	/// </summary>
	public class OverviewForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MenuItem parentMenuItem;
		
		OverviewFormComponent ovFormComponent = null;

		Size m_StartupSize = new Size(600, 300);

		MainApplication m_ParentApplication = null;

		string m_SettingsFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Plugins\\OverviewForm\\settings.txt");

		public OverviewForm(WorldWindow ww, MenuItem menuItem)
		{
			
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Size = m_StartupSize;

			parentMenuItem = menuItem;
			ovFormComponent = new OverviewFormComponent(ww, this);
			ovFormComponent.Dock = DockStyle.Fill;
			ovFormComponent.Parent = this;
			Controls.Add(ovFormComponent);

			LoadSettings();

			this.m_InitialToolbarVisibility = World.Settings.ShowToolbar;
			World.Settings.ShowToolbar = false;

		}

		bool m_InitialToolbarVisibility = true;

		private void LoadSettings()
		{
			FileInfo settingsFile = new FileInfo(m_SettingsFilePath);

			if(!settingsFile.Exists)
			{
				if(!settingsFile.Directory.Exists)
				{
					settingsFile.Directory.Create();
				}
				
				GenerateSettingsFile(m_SettingsFilePath);
			}

			int startx = 0;
			int starty = 0;
			int width = 1024;
			int height = 512;
			int toolbarWidth = 64;
			int ww_startx = 0;
			int ww_starty = 512;
			int ww_width = 512;
			int ww_height = 512;

			using(StreamReader reader = new StreamReader(m_SettingsFilePath))
			{
				string line = reader.ReadLine();
				while(line != null)
				{
					if(!line.StartsWith("//"))
					{
						if(line.StartsWith("startx:"))
						{
							startx = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("starty:"))
						{
							starty = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("Width:"))
						{
							width = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("Height:"))
						{
							height = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("toolbarWidth:"))
						{
							toolbarWidth = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("3dwindow_startx:"))
						{
							ww_startx = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("3dwindow_starty:"))
						{
							ww_starty = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("3dwindow_width:"))
						{
							ww_width = int.Parse(line.Split(':')[1].Trim());
						}
						else if(line.StartsWith("3dwindow_height:"))
						{
							ww_height = int.Parse(line.Split(':')[1].Trim());
						}
					}

					line = reader.ReadLine();
				}
			}

			this.m_ParentApplication.Location = new Point(ww_startx, ww_starty);
			this.m_ParentApplication.Size = new Size(ww_width, ww_height);

			this.Location = new Point(startx, starty);

			this.Size = new Size(width, height);
			this.ovFormComponent.ToolbarSize = toolbarWidth;
		}

		private void GenerateSettingsFile(string settingsFile)
		{
			using(StreamWriter writer = new StreamWriter(settingsFile, false, System.Text.Encoding.ASCII))
			{
				writer.WriteLine("// Settings for overview form");
				writer.WriteLine("// ");
				writer.WriteLine("// startx: startup x location of the form");
				writer.WriteLine("startx: 0");
				writer.WriteLine("// starty: startup y location of the form");
				writer.WriteLine("starty: 0");
				writer.WriteLine("// Width: specifies the startup width of the form");
				writer.WriteLine("Width: 1024");
				writer.WriteLine("// Height: specifies the startup height of the form");
				writer.WriteLine("Height: 512");
				writer.WriteLine("toolbarHeight: 64");
				
				writer.WriteLine("3dwindow_startx: 0");
				writer.WriteLine("3dwindow_starty: 512");
				writer.WriteLine("3dwindow_width: 512");
				writer.WriteLine("3dwindow_height: 512");
			}
		}

		private void SaveSettingsFile(string settingsFile)
		{
			using(StreamWriter writer = new StreamWriter(settingsFile, false, System.Text.Encoding.ASCII))
			{
				writer.WriteLine("// Settings for overview form");
				writer.WriteLine("// ");
				writer.WriteLine("// startx: startup x location of the form");
				writer.WriteLine("startx: {0}", this.Location.X);
				writer.WriteLine("// starty: startup y location of the form");
				writer.WriteLine("starty: {0}", this.Location.Y);
				writer.WriteLine("// Width: specifies the startup width of the form");
				writer.WriteLine("Width: {0}", this.Size.Width);
				writer.WriteLine("// Height: specifies the startup height of the form");
				writer.WriteLine("Height: {0}", this.Size.Height);
				writer.WriteLine("toolbarHeight: {0}", this.ovFormComponent.ToolbarSize);
				
				writer.WriteLine("3dwindow_startx: {0}", this.m_ParentApplication.Location.X);
				writer.WriteLine("3dwindow_starty: {0}", this.m_ParentApplication.Location.Y);
				writer.WriteLine("3dwindow_width: {0}", this.m_ParentApplication.Size.Width);
				writer.WriteLine("3dwindow_height: {0}", this.m_ParentApplication.Size.Height);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Dispose(true);
			parentMenuItem.Checked = false;
			base.OnClosing (e);
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// OverviewForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "OverviewForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Overview Form";

		}
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			this.SaveSettingsFile(this.m_SettingsFilePath);

			World.Settings.ShowToolbar = this.m_InitialToolbarVisibility;
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}


			base.Dispose( disposing );
		}

	}

	public class OverviewFormLoader : WorldWind.PluginEngine.Plugin
	{
		MenuItem m_MenuItem;
		OverviewForm m_Form = null;
		WorldWind.WindowsControlMenuButton m_ToolbarItem = null;

		/// <summary>
		/// Plugin entry point 
		/// </summary>
		public override void Load() 
		{
				m_MenuItem = new MenuItem("Overview Form");
				m_MenuItem.Click += new EventHandler(menuItemClicked);
		}

		/// <summary>
		/// Unload our plugin
		/// </summary>
		public override void Unload() 
		{
			if(m_MenuItem!=null)
			{
				m_MenuItem.Dispose();
				m_MenuItem = null;
			}

			if(m_ToolbarItem != null)
			{
				m_ToolbarItem.Dispose();
				m_ToolbarItem = null;
			}

			if(m_Form != null)
			{
				m_Form.Dispose();
				m_Form = null;
			}
		}
	
		void menuItemClicked(object sender, EventArgs e)
		{
			if(m_Form != null && m_Form.Visible)
			{
				m_Form.Visible = false;
				m_Form.Dispose();
				m_Form = null;
				
				m_MenuItem.Checked = false;
			}
			else
			{
				m_Form = new OverviewForm(Global.worldWindow, m_MenuItem);
			
				m_Form.Visible = true;
				m_MenuItem.Checked = true;
			}
		}
	}
}
