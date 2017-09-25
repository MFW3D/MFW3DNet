//----------------------------------------------------------------------------
// NAME: Location Cursor
// VERSION: 0.1
// DESCRIPTION: Adds a location box to the cursor, including both a grid 
// conversion tool for the 6 figure grid and Latitude/Longitude
// DEVELOPER: Isaac Mann
// WEBSITE: http:\\www.apogee.com.au
// REFERENCES: 
//----------------------------------------------------------------------------
//
// This file is in the Public Domain, and comes with no warranty. 
//
using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.Net;

namespace ApogeeTools.Plugins
{
    /// <summary>
    /// Properties dialog class 
    /// </summary>
    public class PosDispPropertiesDialog : Form
    {
        private GridRefToolLayer gridLayer;
        private Label precisionLabel;
        private TextBox precisionTextBox;
        bool latLonEnabled = true;

        public PosDispPropertiesDialog(GridRefToolLayer gridLayer)
        {
            this.gridLayer = gridLayer;
            InitializeComponent();
        }

        private void sixGridRadioButton_Click(object sender, EventArgs e)
        {
            this.sixGridRadioButton.Checked = true;
            this.latLonRadioButton.Checked = false;
            this.latLonEnabled = false;
        }

        private void latLonRadioButton_Click(object sender, EventArgs e)
        {
            this.latLonRadioButton.Checked = true;
            this.sixGridRadioButton.Checked = false;
            this.latLonEnabled = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (latLonEnabled)
            {
                this.gridLayer.isLatLon = true;
                this.gridLayer.precision = Convert.ToInt16(precisionTextBox.Text);
            }
            else
                this.gridLayer.isLatLon = false;
            this.Hide();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PosDispPropertiesDialog));
            this.sixGridRadioButton = new System.Windows.Forms.RadioButton();
            this.latLonRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.precisionLabel = new System.Windows.Forms.Label();
            this.precisionTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sixGridRadioButton
            // 
            this.sixGridRadioButton.AutoSize = true;
            this.sixGridRadioButton.Checked = false;
            this.sixGridRadioButton.Location = new System.Drawing.Point(12, 12);
            this.sixGridRadioButton.Name = "sixGridRadioButton";
            this.sixGridRadioButton.Size = new System.Drawing.Size(82, 17);
            this.sixGridRadioButton.TabIndex = 0;
            this.sixGridRadioButton.TabStop = true;
            this.sixGridRadioButton.Text = "6-figure Grid";
            this.sixGridRadioButton.UseVisualStyleBackColor = true;
            this.sixGridRadioButton.Click += new System.EventHandler(this.sixGridRadioButton_Click);
            // 
            // latLonRadioButton
            // 
            this.latLonRadioButton.AutoSize = true;
            this.latLonRadioButton.Checked = true;
            this.latLonRadioButton.Location = new System.Drawing.Point(12, 41);
            this.latLonRadioButton.Name = "latLonRadioButton";
            this.latLonRadioButton.Size = new System.Drawing.Size(115, 17);
            this.latLonRadioButton.TabIndex = 1;
            this.latLonRadioButton.TabStop = true;
            this.latLonRadioButton.Text = "Latitude/Longitude";
            this.latLonRadioButton.UseVisualStyleBackColor = true;
            this.latLonRadioButton.Click += new System.EventHandler(this.latLonRadioButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(151, 9);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(85, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(151, 38);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(85, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // precisionLabel
            // 
            this.precisionLabel.AutoSize = true;
            this.precisionLabel.Location = new System.Drawing.Point(12, 70);
            this.precisionLabel.Name = "precisionLabel";
            this.precisionLabel.Size = new System.Drawing.Size(53, 13);
            this.precisionLabel.TabIndex = 4;
            this.precisionLabel.Text = "Precision:";
            // 
            // precisionTextBox
            // 
            this.precisionTextBox.Location = new System.Drawing.Point(71, 67);
            this.precisionTextBox.Name = "precisionTextBox";
            this.precisionTextBox.Size = new System.Drawing.Size(56, 20);
            this.precisionTextBox.TabIndex = 5;
            this.precisionTextBox.Text = "6";
            // 
            // PosDispPropertiesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 100);
            this.Controls.Add(this.precisionTextBox);
            this.Controls.Add(this.precisionLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.latLonRadioButton);
            this.Controls.Add(this.sixGridRadioButton);
            //this.Icon = new System.Drawing.Icon(Path.Combine(  ,"apogee.ico"));
            this.Name = "PosDispPropertiesDialog";
            this.Text = "Position Display Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton sixGridRadioButton;
        private System.Windows.Forms.RadioButton latLonRadioButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }

	/// <summary>
	/// Grid Position Converter Displaying Tool plug-in
	/// </summary>
	public class GridPositionDisplay : WorldWind.PluginEngine.Plugin
	{
		MenuItem menuItem;
		GridRefToolLayer layer;
		Projection proj;

		/// <summary>
		/// Plugin entry point 
		/// </summary>
		public override void Load() 
		{
			layer = new GridRefToolLayer(
				ParentApplication.WorldWindow.CurrentWorld,
				ParentApplication.WorldWindow.DrawArgs );

			layer.TexturePath = Path.Combine( PluginDirectory, "rect.jpg" );
			Console.WriteLine(PluginDirectory);
			try 
			{
				proj = new Projection(new string[]{"proj=utm", "zone=54", "south", "ellps=WGS84", "units=m", "no.defs"});
			} 
			catch 
			{
				Console.WriteLine("projection initialisation failed");
			}
			ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Add(layer);

			menuItem = new MenuItem("Grid position\tQ");
			menuItem.Click += new EventHandler(menuItemClicked);
			ParentApplication.ToolsMenu.MenuItems.Add( menuItem );

			// Subscribe events
			ParentApplication.WorldWindow.MouseMove += new MouseEventHandler(layer.MouseMove);
			ParentApplication.WorldWindow.MouseDown += new MouseEventHandler(layer.MouseDown);
			ParentApplication.WorldWindow.MouseUp += new MouseEventHandler(layer.MouseUp);
			ParentApplication.WorldWindow.KeyUp +=new KeyEventHandler(layer.KeyUp);
		}

		/// <summary>
		/// Unload this plugin
		/// </summary>
		public override void Unload() 
		{
			if(menuItem!=null)
			{
				ParentApplication.ToolsMenu.MenuItems.Remove( menuItem );
				menuItem.Dispose();
				menuItem = null;
			}

			ParentApplication.WorldWindow.MouseMove -= new MouseEventHandler(layer.MouseMove);
			ParentApplication.WorldWindow.MouseDown -= new MouseEventHandler(layer.MouseDown);
			ParentApplication.WorldWindow.MouseUp -= new MouseEventHandler(layer.MouseUp);
			ParentApplication.WorldWindow.KeyUp -= new KeyEventHandler(layer.KeyUp);
			
			ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Remove(layer);
		}

		void menuItemClicked(object sender, EventArgs e)
		{
			layer.IsOn = !layer.IsOn;
			menuItem.Checked = layer.IsOn;
		}
	}


	public class GridRefToolLayer : WorldWind.Renderable.RenderableObject
	{
		
		#region Public data

		/// <summary>
		/// Tool texture path
		/// </summary>
		public string TexturePath;

		///<summary>
		/// Latitude of cursor position
		/// </summary>
		public Angle currentLatitude;

		///<summary>
		/// Longitude of cursor position
		/// </summary>
		public Angle currentLongitude;

        /// <summary>
        /// Whether in 6-figure Grid or Latitude/Longitude mode
        /// </summary>
        public bool isLatLon = true;

        public int precision = 6;

		#endregion

		#region Private parts
		Projection proj;
		DrawArgs m_drawArgs;
		UV longLat;

		string labelText;
		Rectangle labelTextRect;

		CustomVertex.PositionColored[] measurePoint = new CustomVertex.PositionColored[17];
		CustomVertex.TransformedColoredTextured[] rect = new CustomVertex.TransformedColoredTextured[5];
		CustomVertex.TransformedColored[] rectFrame = new CustomVertex.TransformedColored[5];

		bool isPointGotoEnabled;
		Point mouseDownPoint;
		private Texture m_texture;

        private PosDispPropertiesDialog propertiesDialog;

		#endregion

		public GridRefToolLayer(World world, DrawArgs drawArgs) : base("Grid Reference Tool")
		{
			RenderPriority = RenderPriority.Placenames;
			isOn = false;

			m_world = world;
			m_drawArgs = drawArgs;

			// Initialize colors
			for(int i=0;i<rect.Length;i++)
				rect[i].Color = World.Settings.MeasureLineLinearColorXml;
			for(int i=0;i<rectFrame.Length;i++)
				rectFrame[i].Color = unchecked((int)0xff808080L);
			
			rect[1].Tv = 1;
			rect[2].Tu = 1;
			rect[3].Tu = 1;
			rect[3].Tv = 1;
		}

		public void MouseDown( object sender, MouseEventArgs e )
		{
			if(!isOn)
				return;
			// only use to check if mouse was moved?
			mouseDownPoint = DrawArgs.LastMousePosition;
		}

		public void MouseUp( object sender, MouseEventArgs e )
		{
			if(!isOn)
				return;
			// check if mouse was clicked and dragged
			int dx = DrawArgs.LastMousePosition.X - mouseDownPoint.X;
			int dy = DrawArgs.LastMousePosition.Y - mouseDownPoint.Y;
			if(dx*dx+dy*dy > 3*3)
			{
				// Mouse was dragged
				return;
			}
			
			// Cancel selection if right mouse button clicked
			if (e.Button == MouseButtons.Right)
			{
				IsOn = false;
				return;
			}
			
			// Do nothing for all other mouse buttons clicked
			if (e.Button != MouseButtons.Left)
			{
				return;
			}

			m_drawArgs.WorldCamera.PickingRayIntersection(
				e.X,
				e.Y,
				out currentLatitude,
				out currentLongitude);
		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			if(!isOn)
				return;

			Angle lat;
			Angle lon;
			m_drawArgs.WorldCamera.PickingRayIntersection(
				e.X,
				e.Y,
				out lat,
				out lon);

			if(Angle.IsNaN(lat))
				return;
			
			currentLongitude = lon;
			currentLatitude = lat;
		}

		public void KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData==Keys.Q)
			{
				IsOn = !IsOn;
				e.Handled = true;
			}
		}

        public override void BuildContextMenu(ContextMenu menu)
        {
            menu.MenuItems.Add("Properties", new System.EventHandler(OnPropertiesClick));
        }

        protected override void OnPropertiesClick(object sender, EventArgs e)
        {
            if (propertiesDialog != null)
            {
                //Already open
                propertiesDialog.Show();
                return;
            }
            propertiesDialog = new PosDispPropertiesDialog(this);
            propertiesDialog.Show();
        }

		public override bool IsOn
		{
			get
			{
				return base.IsOn;
			}
			set
			{
				if(value==isOn)
					return;

				base.IsOn = value;
				if(isOn)
				{
					// Can't use point goto while measuring
					isPointGotoEnabled = World.Settings.CameraIsPointGoto;
					World.Settings.CameraIsPointGoto = false;
				}
				else
				{
					World.Settings.CameraIsPointGoto = isPointGotoEnabled;
				}
			}
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(!isOn)
				return;

			if(DrawArgs.MouseCursor == CursorType.Arrow) 
			{
				// Use our cursor when the mouse isn't over other elements requiring different cursor
				DrawArgs.MouseCursor = CursorType.Cross;
			}

			if (!CalculateRectPlacement())
				return;

			Device device = drawArgs.device;
			device.RenderState.ZBufferEnable = false;
			device.TextureState[0].ColorOperation = TextureOperation.Disable;
			device.VertexFormat = CustomVertex.PositionColored.Format;

			// Draw the info rect
			device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
			device.SetTexture(0,m_texture);
			device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, rect);

			device.TextureState[0].ColorOperation = TextureOperation.Disable;

			device.DrawUserPrimitives(PrimitiveType.LineStrip, rectFrame.Length-1, rectFrame);
			drawArgs.defaultDrawingFont.DrawText(null, labelText, labelTextRect, DrawTextFormat.None, 0xff << 24);
			
			device.RenderState.ZBufferEnable = true;
		}
		
		bool CalculateRectPlacement()
		{
			String a;
			String b;
			UV temp = new UV();
            float[] labelPoint = { measurePoint[0].X, measurePoint[0].Y };
			if (labelPoint[0] < 0 || labelPoint[1] < 0)
			{
				// prevent selecting on invalid mouse position
				return false;
			}
			try 
			{
                int zone = Convert.ToInt32(currentLongitude.Degrees / 6.0 + 31);
                string hemi = "north";
                if (currentLatitude.Degrees < 0.0)
                    hemi = "south";

                proj = new Projection(new string[] { "proj=utm", "zone=" + zone, hemi, "ellps=WGS84", "units=m", "no.defs" });
        	}
			catch 
			{
                Console.WriteLine("Calc Rect Placement: Projection initialisation failed");
			}
			// Calculate label box position/size; catch projection calculation error
			longLat = new UV(currentLongitude.Radians,currentLatitude.Radians);
			if (proj != null)
			{
				try
				{
					// Change the longitude/latitude coordinates to a grid reference
					temp = proj.Forward(longLat);
				}
				catch
				{
					Console.WriteLine("Projection: lat/lon to grid reference failed.");
				}
				a = ""+Math.Round(temp.U/100)%1000;
				b = ""+Math.Round(temp.V/100)%1000;
				a = a.PadLeft(3,'0');
				b = b.PadLeft(3,'0');

				try
				{
					proj.Dispose();
				}
				catch 
				{
					Console.WriteLine("Projection not disposed of.");
				}
			}
			else 
			{
				a = "Area out of bounds";
				b = "";
			}
            // Check for current mode
            if (this.isLatLon)
            {
                double lat = Math.Round(currentLatitude.Degrees, 6);
                double lon = Math.Round(currentLongitude.Degrees, 6);
                labelText = string.Format("Latitude: " + lat + "°");
                labelText += string.Format("\nLongitude: " + lon + "°");
            }
            else
            {
                // string, declared in private section
                labelText = string.Format("Grid reference\n");
                labelText += string.Format(a + b);
            }
			
			// rectangle, declared in private section
			labelTextRect = 
				m_drawArgs.defaultDrawingFont.MeasureString(null, labelText, DrawTextFormat.None, 0);
			Rectangle tsize = labelTextRect;

			const int xPad = 4;
			const int yPad = 1;
			tsize.Inflate( xPad, yPad );
			labelTextRect.Offset(DrawArgs.LastMousePosition.X+10,DrawArgs.LastMousePosition.Y+9);
			tsize.Offset(DrawArgs.LastMousePosition.X+10,DrawArgs.LastMousePosition.Y+9);
			
			// main body of box background
			rect[0].X = tsize.Left;
			rect[0].Y = tsize.Top;
			rect[1].X = rect[0].X;
			rect[1].Y = tsize.Bottom;
			rect[2].X = tsize.Right;
			rect[2].Y = rect[0].Y;
			rect[3].X = rect[2].X;
			rect[3].Y = rect[1].Y;
			rect[4].X = rect[0].X;
			rect[4].Y = rect[1].Y;

			// outline of box background
			rectFrame[0].X = tsize.Left;
			rectFrame[0].Y = tsize.Top;
			rectFrame[1].X = rectFrame[0].X;
			rectFrame[1].Y = tsize.Bottom;
			rectFrame[2].X = tsize.Right;
			rectFrame[2].Y = rectFrame[1].Y;
			rectFrame[3].X = rectFrame[2].X;
			rectFrame[3].Y = rectFrame[0].Y;
			rectFrame[4].X = rectFrame[0].X;
			rectFrame[4].Y = rectFrame[0].Y;
			
			return true;
		}

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
			isInitialized = true;
			if (m_texture == null)
				updateTextures(null,null);
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
			if (!isInitialized)
				Initialize(drawArgs);
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
			isInitialized = false;
			if (m_texture!=null)
			{
				m_texture.Dispose();
				m_texture = null;
			}
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		/// <summary>
		/// Loads or downloads the bitmaps
		/// </summary>
		void LoadTextures(string path)
		{
            if (File.Exists(path)) 
            {
			    try
			    {
                    m_texture = ImageHelper.LoadTexture(path);
			    }
			    catch 
                {
                    Console.WriteLine(path);
                }
			    return;
            }
        }

        protected void updateTextures(object sender, EventArgs e)
        {
            LoadTextures(TexturePath);
        }
	}

	/// <summary>
	/// Sorry for lack of description, but this struct is kinda difficult 
	/// to describe since it supports so many coordinate systems.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct UV
	{ 
		public double U;
		public double V; 

		public UV(double u, double v)
		{
			this.U = u;
			this.V = v;
		}
	}
	/// <summary>
	/// C# wrapper for proj.4 projection filter
	/// http://proj.maptools.org/
	/// </summary>
	public class Projection : IDisposable
	{
		IntPtr projPJ;
		[DllImport("proj.dll")]
		static extern IntPtr pj_init(int argc, string[] args);

		[DllImport("proj.dll")]
		static extern string pj_free(IntPtr projPJ);

		[DllImport("proj.dll")]
		static extern UV pj_fwd(UV uv, IntPtr projPJ);

		/// <summary>
		/// XY -> Lat/lon
		/// </summary>
		/// <param name="uv"></param>
		/// <param name="projPJ"></param>
		/// <returns></returns>
		[DllImport("proj.dll")]
		static extern UV pj_inv(UV uv, IntPtr projPJ);

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="initParameters">Proj.4 style list of options.
		/// <sample>new string[]{ "proj=utm", "ellps=WGS84", "no.defs", "zone=32" }</sample>
		/// </param>
		public Projection(string[] initParameters)
		{
			projPJ = pj_init(initParameters.Length, initParameters);
			if( projPJ == IntPtr.Zero)
				throw new ApplicationException("Projection initialisation failed."); 
		}

		/// <summary>
		/// Forward (Go from specified projection to lat/lon)
		/// </summary>
		/// <param name="uv"></param>
		/// <returns></returns>
		public UV Forward(UV uv)
		{
			return pj_fwd(uv, projPJ); 
		}

		/// <summary>
		/// Inverse (Go from lat/lon to specified projection)
		/// </summary>
		/// <param name="uv"></param>
		/// <returns></returns>
		public UV Inverse(UV uv)
		{
			return pj_inv(uv, projPJ); 
		}

		public void Dispose()
		{
			if(projPJ!=IntPtr.Zero)
			{
				pj_free(projPJ);
				projPJ = IntPtr.Zero;
			}
		}
	}
}