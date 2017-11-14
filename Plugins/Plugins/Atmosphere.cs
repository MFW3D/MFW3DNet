using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using MFW3D.Renderable;
using MFW3D.Camera;
using MFW3D;
using System.IO;
using System;

namespace Atmosphere.Plugin
{
	/// <summary>
	/// The plugin (main class)
	/// </summary>
	public class Atmosphere : MFW3D.PluginEngine.Plugin 
	{
		/// <summary>
		/// Name displayed in layer manager
		/// </summary>
		public static string LayerName = "Atmosphere";

		/// <summary>
		/// Plugin entry point - All plugins must implement this function
		/// </summary>
		public override void Load() 
		{
			if(Global.worldWindow.CurrentWorld != null && Global.worldWindow.CurrentWorld.Name.IndexOf("Earth") >= 0)
			{
				AtmosphereLayer layer = new AtmosphereLayer(LayerName, PluginDirectory, Global.worldWindow);
                Global.worldWindow.CurrentWorld.RenderableObjects.Add(layer);
			}
		}

		/// <summary>
		/// Unloads our plugin
		/// </summary>
		public override void Unload() 
		{
			Global.worldWindow.CurrentWorld.RenderableObjects.Remove(LayerName);
		}
	}

	/// <summary>
	/// "Fake" atmosphere
	/// </summary>
	public class AtmosphereLayer : RenderableObject
	{
		public float ZoomFactor = 3;
		static string version = "1.0";
		string settingsFileName = "Atmosphere.ini";
		string pluginPath;
		World world;
		public DrawArgs drawArgs;
		CustomVertex.PositionTextured[] borderVertices = new CustomVertex.PositionTextured[4];
		Texture texture;
		Form pDialog;

		// default texture bitmap
		public string textureFileName = "Earth_Thick.png";

		/// <summary>
		/// Constructor
		/// </summary>
		public AtmosphereLayer(string LayerName, string pluginPath, MFW3D.WorldWindow worldWindow) : base(LayerName)
		{
			this.pluginPath = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), @"Plugins\Atmosphere\");
			this.world = worldWindow.CurrentWorld;
			this.drawArgs = worldWindow.DrawArgs;
			this.RenderPriority = RenderPriority.SurfaceImages;
			ReadSettings();
		}
		/// <summary>
		/// Read saved settings from ini file
		/// </summary>
		public void ReadSettings()
		{
			string line = "";
			try 
			{
				TextReader tr = File.OpenText(Path.Combine(pluginPath, settingsFileName));
				line = tr.ReadLine();
				tr.Close();
			}
			catch(Exception) {}
			if(line != "")
			{
				string[] settingsList = line.Split(';');
				string saveVersion = settingsList[1];	// version when settings where saved
				if(settingsList[1] != null) textureFileName = settingsList[1];
			}
		}

		/// <summary>
		/// Save settings in ini file
		/// </summary>
		public void SaveSettings()
		{
			string line = version + ";" + textureFileName;
			try
			{
				StreamWriter sw = new StreamWriter(Path.Combine(pluginPath, settingsFileName));
				sw.Write(line);
				sw.Close();
			}
			catch(Exception) {}
		}

		#region RenderableObject

		/// <summary>
		/// This is where we do our rendering 
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			if(!isInitialized || World.Settings.EnableAtmosphericScattering)
				return;

			// Camera shortcut ;)
			CameraBase camera = drawArgs.WorldCamera;

			if(camera.Altitude < 1000e3)
				// TODO: Add gradual transition
				return;

			Device device = drawArgs.device;
			
			// Calculate perp1 and perp2 so they form a plane perpendicular to camera vector and crossing earth center
			Vector3 cameraPos = drawArgs.WorldCamera.Position;

			Vector3 perp1 = Vector3.Cross( cameraPos, new Vector3(1,1,1) );
			Vector3 perp2 = Vector3.Cross( perp1, cameraPos );
			perp1.Normalize();
			perp2.Normalize();

			float radius = (float)camera.WorldRadius*1.05f;
			perp1.Scale(radius);
			perp2.Scale(radius);

			
			// Move the plane towards us so it lies on the horizon
			float distHorizon = (float)Math.Sqrt( cameraPos.LengthSq() - world.EquatorialRadius*world.EquatorialRadius );
			Vector3 cameraDir = Vector3.Normalize( cameraPos );
			Vector3 offset = Vector3.Scale( cameraDir, cameraPos.Length() - distHorizon );

			// Calculate the triangle vertices (triangle 1)
			Vector3 ur = -perp1 + perp2 + offset;
			borderVertices[0].X = ur.X;
			borderVertices[0].Y = ur.Y;
			borderVertices[0].Z = ur.Z;
			borderVertices[0].Tu = 0;
			borderVertices[0].Tv = 1;

			ur = perp1 + perp2 + offset;
			borderVertices[1].X = ur.X;
			borderVertices[1].Y = ur.Y;
			borderVertices[1].Z = ur.Z;
			borderVertices[1].Tu = 1;
			borderVertices[1].Tv = 1;

			ur = -perp1 - perp2 + offset;
			borderVertices[2].X = ur.X;
			borderVertices[2].Y = ur.Y;
			borderVertices[2].Z = ur.Z;
			borderVertices[2].Tu = 0;
			borderVertices[2].Tv = 0;

			// Triangle 2 (uses previous 2 vertices as first 2 points (TriangleStrip))
			ur = perp1 - perp2 + offset;
			borderVertices[3].X = ur.X;
			borderVertices[3].Y = ur.Y;
			borderVertices[3].Z = ur.Z;
			borderVertices[3].Tu = 1;
			borderVertices[3].Tv = 0;

			// Save fog status
			bool origFog = device.RenderState.FogEnable;
			device.RenderState.FogEnable = false;

			// Allow us to be overdrawn
			device.RenderState.ZBufferEnable = false;

			// Save original projection
			Matrix origProjection = device.Transform.Projection;

			// Set new one (to avoid being clipped) - probably better ways of doing this?
			float aspectRatio =  (float)device.Viewport.Width / device.Viewport.Height;
			device.Transform.Projection = Matrix.PerspectiveFovRH((float)camera.Fov.Radians, aspectRatio, 1, (float)(2*world.EquatorialRadius) );
			device.TextureState[0].ColorOperation = TextureOperation.BlendCurrentAlpha;

			// Draw our 2 triangles
			device.VertexFormat = CustomVertex.PositionTextured.Format;
			device.SetTexture(0,texture);

			drawArgs.device.Transform.World = Matrix.Translation(
				(float)-drawArgs.WorldCamera.ReferenceCenter.X,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Y,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Z
				);

			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, borderVertices);


			// Restore device states
			drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
			device.Transform.Projection = origProjection;
			device.RenderState.ZBufferEnable = true;
			device.RenderState.FogEnable = origFog;
		}

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				texture = TextureLoader.FromFile(drawArgs.device, Path.Combine(pluginPath, textureFileName));
				isInitialized = true;	
			}
			catch
			{
				isOn = false;
				MessageBox.Show("Error loading texture " + Path.Combine(pluginPath, textureFileName) + ".","Layer initialization failed.", MessageBoxButtons.OK, 
					MessageBoxIcon.Error );
			}
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
			if(!isInitialized)
				Initialize(drawArgs);
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
			isInitialized = false;
			if(texture!=null)
			{
				texture.Dispose();
				texture = null;
			}
		}

		/// <summary>
		/// Gets called when user left clicks.
		/// RenderableObject abstract member (needed)
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}


		/// <summary>
		/// Properties context menu clicked.
		/// </summary>
		public new void OnPropertiesClick(object sender, EventArgs e)
		{
			if(pDialog != null && ! pDialog.IsDisposed)
				// Already open
				return;

			// Display the dialog
			pDialog = new propertiesDialog(this);
			pDialog.Show();

		}

		/// <summary>
		/// Properties Dialog
		/// </summary>
		public class propertiesDialog : System.Windows.Forms.Form
		{
			private System.Windows.Forms.Label lblTexture;
			private System.Windows.Forms.ComboBox cboTexture;
			private System.Windows.Forms.Button btnOK;
			private System.Windows.Forms.Button btnCancel;
			private AtmosphereLayer layer;

			public propertiesDialog( AtmosphereLayer layer )
			{
				InitializeComponent();
				//this.Icon = WorldWind.PluginEngine.Plugin.Icon;
				this.layer = layer;
				// Init texture list with *.png
				DirectoryInfo di = new DirectoryInfo(layer.pluginPath);
				FileInfo[] imgFiles = di.GetFiles("*.png");
				cboTexture.Items.AddRange(imgFiles);
				// select current bitmap
				int i = cboTexture.FindString(layer.textureFileName);
				if(i != -1) cboTexture.SelectedIndex = i;
			}

			#region Windows Form Designer generated code
			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.btnCancel = new System.Windows.Forms.Button();
				this.btnOK = new System.Windows.Forms.Button();
				this.lblTexture = new System.Windows.Forms.Label();
				this.cboTexture = new System.Windows.Forms.ComboBox();
				this.SuspendLayout();
				// 
				// btnCancel
				// 
				this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				this.btnCancel.Location = new System.Drawing.Point(311, 59);
				this.btnCancel.Name = "btnCancel";
				this.btnCancel.TabIndex = 0;
				this.btnCancel.Text = "Cancel";
				this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
				// 
				// btnOK
				// 
				this.btnOK.Location = new System.Drawing.Point(224, 59);
				this.btnOK.Name = "btnOK";
				this.btnOK.TabIndex = 1;
				this.btnOK.Text = "OK";
				this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
				// 
				// lblTexture
				// 
				this.lblTexture.AutoSize = true;
				this.lblTexture.Location = new System.Drawing.Point(16, 28);
				this.lblTexture.Name = "lblTexture";
				this.lblTexture.Size = new System.Drawing.Size(82, 16);
				this.lblTexture.TabIndex = 2;
				this.lblTexture.Text = "Atmosphere :";
				// 
				// cboTexture
				// 
				this.cboTexture.Location = new System.Drawing.Point(96, 25);
				this.cboTexture.Name = "cboTexture";
				this.cboTexture.Size = new System.Drawing.Size(296, 21);
				this.cboTexture.TabIndex = 3;
				this.cboTexture.Text = "Select texture file";
				this.cboTexture.DropDownStyle = ComboBoxStyle.DropDownList;
				this.cboTexture.MaxDropDownItems = 10;
				// 
				// frmFavorites
				// 
				this.AcceptButton = this.btnOK;
				this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				this.CancelButton = this.btnCancel;
				this.ClientSize = new System.Drawing.Size(406, 94);
				this.ControlBox = false;
				this.Controls.Add(this.cboTexture);
				this.Controls.Add(this.lblTexture);
				this.Controls.Add(this.btnOK);
				this.Controls.Add(this.btnCancel);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "pDialog";
				this.ShowInTaskbar = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				//this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
				//this.Location = new System.Drawing.Point(layer.drawArgs.CurrentMousePosition.X + 10, layer.drawArgs.CurrentMousePosition.Y - 10);
				this.Text = "Atmosphere properties";
				this.TopMost = true;
				this.ResumeLayout(false);

			}
			#endregion

			private void btnOK_Click(object sender, System.EventArgs e)
			{
				if(cboTexture.SelectedItem != null) 
				{
					//System.Windows.Forms.MessageBox.Show("Texture : " + cboTexture.SelectedItem.ToString());
					layer.Dispose();
					layer.textureFileName = cboTexture.SelectedItem.ToString();
					layer.Initialize(layer.drawArgs);
					layer.SaveSettings();
				}
				// Close this form
				this.Close();
			}

			private void btnCancel_Click(object sender, System.EventArgs e)
			{

				// Close this form
				this.Close();
			}





		}

		#endregion

	}
}
