using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Globalization;
using Utility;

namespace MFW3D.CMPlugins.OverviewForm
{
	/// <summary>
	/// Summary description for OverviewToolbar.
	/// </summary>
	public class OverviewToolbar : System.Windows.Forms.Control
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MFW3D.WorldWindow m_WorldWindow;
		private Device m_Device3d;
		private PresentParameters m_presentParams;
		private Microsoft.DirectX.Direct3D.Font m_DrawingFont = null;
		
		private ImageTileCache m_ImageTileCache = null;
		private ResourceCache m_ResourceCache = null;
		Color m_BackgroundColor = System.Drawing.Color.Black;

		System.Timers.Timer m_RenderTimer = new System.Timers.Timer(35);

		public OverviewToolbar(MFW3D.WorldWindow ww)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_WorldWindow = ww;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			try
			{
				this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
				// Now perform the rendering m_Device3d initialization
				// Skip DirectX initialization in design mode
				if(!IsInDesignMode())
					InitializeGraphics();
				m_DrawingFont = new Microsoft.DirectX.Direct3D.Font(
					m_Device3d,
					new System.Drawing.Font("Tachoma", 10.0f, FontStyle.Bold));

				m_ImageTileCache = new ImageTileCache(m_Device3d);
				m_ResourceCache = new ResourceCache();
				
				m_RenderTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_RenderTimer_Elapsed);
				m_RenderTimer.Start();

			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
			/*catch (InvalidCallException caught)
			{
				throw new InvalidCallException(
					"Unable to locate a compatible graphics adapter. Make sure you are running the latest version of DirectX.", caught );
			}
			catch (NotAvailableException caught)
			{
				throw new NotAvailableException(
					"Unable to locate a compatible graphics adapter. Make sure you are running the latest version of DirectX.", caught );
			}*/
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
			if(m_Device3d != null && !m_Device3d.Disposed)
			{
				m_Device3d.Dispose();
			}
			base.Dispose( disposing );
		}
		
		/// <summary>
		/// Returns true if executing in Design mode (inside IDE)
		/// </summary>
		/// <returns></returns>
		private static bool IsInDesignMode()
		{
			return Application.ExecutablePath.ToUpper(CultureInfo.InvariantCulture).EndsWith("DEVENV.EXE");
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				if(m_Device3d==null)
				{
					e.Graphics.Clear(SystemColors.Control);
					return;
				}

				Render();
				m_Device3d.Present();
			}
			catch(DeviceLostException)
			{
				try
				{
					AttemptRecovery();

					// Our surface was lost, force re-render
					Render();
					m_Device3d.Present();
				}
				catch(DirectXException)
				{
					// Ignore a 2nd failure
				}
			}
			

			base.OnPaint (e);
		}

		private void Render()
		{
			m_Device3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, m_BackgroundColor, 1.0f, 0);
			
			m_Device3d.BeginScene();
			RenderToolbar();
			m_Device3d.EndScene();
		}

		private void RenderToolbar()
		{
			System.Collections.ArrayList renderList = new ArrayList();
			System.Collections.ArrayList roList = new ArrayList();
			if(m_WorldWindow.CurrentWorld != null)
			{
				for(int i = 0; i < m_WorldWindow.CurrentWorld.RenderableObjects.Count; i++)
				{
					MFW3D.Renderable.RenderableObject ro = (MFW3D.Renderable.RenderableObject)m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects[i];
					getRenderButtons(ro, renderList, roList);
				}
			}

			int xIncr = 0;
			int buttonSize = Height / 2;

			int mouseoverIcon = -1;
			
			int totalWidth = buttonSize * renderList.Count;

			xIncr += Width / 2 - totalWidth / 2;
			

			double d = 0;
			int t = buttonSize;
			if(m_LastMousePosition != Point.Empty && m_LastMousePosition.X >= xIncr && m_LastMousePosition.X <= xIncr + totalWidth)
			{
				int closestIcon = (m_LastMousePosition.X - xIncr) / buttonSize;

				d = 1.0 - Math.Abs((double)(double)m_LastMousePosition.X - (closestIcon * buttonSize + xIncr)) / ((double)buttonSize);
				d *= (int)((Height - buttonSize));
				mouseoverIcon = closestIcon;

				d = Height - buttonSize - 10;
			}
				

			if(mouseoverIcon >= 0)
			{
				buttonSize = (totalWidth - (buttonSize + (int)d)) / (renderList.Count - 1);
				d += totalWidth - (renderList.Count - 1) * buttonSize - buttonSize - d;
			}

			for(int i = 0; i < renderList.Count; i++)
			{
				string buttonKey = (string)renderList[i];
				MFW3D.Renderable.RenderableObject curRo = (MFW3D.Renderable.RenderableObject)roList[i];

				int sizeIncr = 0;
				int yOffset = 7 + Height / 2 - t / 2;
			
				if(mouseoverIcon == i)
				{
					sizeIncr = (int)d;
					yOffset += (int)d / 2 - 2;

					this.m_DrawingFont.DrawText(
						null,
						curRo.Name,
						new Rectangle(
						(xIncr < Width / 2 ? xIncr + buttonSize + sizeIncr + 5 : xIncr - 5 - Width),
						yOffset + 5, //+ buttonSize / 2 + 5,
						Width,
						Height),
						( xIncr < Width / 2 ? DrawTextFormat.NoClip : DrawTextFormat.Right),
						System.Drawing.Color.White);

				}

				RenderButton(buttonKey, xIncr + (buttonSize + sizeIncr) / 2, yOffset, buttonSize + sizeIncr, buttonSize + sizeIncr,
					(curRo != null ? curRo.IsOn : false));



				xIncr += buttonSize + sizeIncr;
			}
		}

		private void getRenderButtons(MFW3D.Renderable.RenderableObject ro, System.Collections.ArrayList renderList, System.Collections.ArrayList roList)
		{
			if(ro.MetaData != null)
			{
				string toolbarImagePath = (string)ro.MetaData["ToolBarImagePath"];
				if(toolbarImagePath != null)
				{
					ResourceCacheEntry cacheEntry = (ResourceCacheEntry)m_ResourceCache[toolbarImagePath];

					if(cacheEntry == null)
					{
						if(System.IO.File.Exists(toolbarImagePath))
						{
							renderList.Add(toolbarImagePath);
							roList.Add(ro);
						}
					}
					else
					{
						renderList.Add(toolbarImagePath);
						roList.Add(ro);
					}
				}
			}

			if(ro is MFW3D.Renderable.RenderableObjectList)
			{
				MFW3D.Renderable.RenderableObjectList rol = (MFW3D.Renderable.RenderableObjectList)ro;
				for(int i = 0; i < rol.Count; i++)
				{
					MFW3D.Renderable.RenderableObject childRo = (MFW3D.Renderable.RenderableObject)rol.ChildObjects[i];
					getRenderButtons(childRo, renderList, roList);
				}
			}
		}
		class IconButton
		{
			public string ImagePath = null;
			public SurfaceDescription SurfaceDescription;
			public Texture Texture = null;
			public int Width = 24;
			public int Height = 24;
		}

		Sprite m_sprite = null;

		CustomVertex.TransformedColored[] m_ChevronVerts = new CustomVertex.TransformedColored[3];
		private void RenderButton(string buttonKey, int x, int y, int width, int height, bool enabled)
		{
			try
			{
				ResourceCacheEntry cacheEntry = m_ResourceCache[buttonKey];
				if(cacheEntry == null)
				{
					cacheEntry = new ResourceCacheEntry();

					IconButton newIconButton = new IconButton();
					newIconButton.ImagePath = buttonKey;
				
					newIconButton.Texture = TextureLoader.FromFile(
						m_Device3d, buttonKey);
					
					newIconButton.SurfaceDescription = newIconButton.Texture.GetLevelDescription(0);

					cacheEntry.Object = newIconButton;

					m_ResourceCache.Add(buttonKey, cacheEntry);
				}

				IconButton iconButton = cacheEntry.Object as IconButton;
			
				if(iconButton.Texture != null && !iconButton.Texture.Disposed)
				{
					if(m_sprite == null)
						m_sprite = new Sprite(m_Device3d);

					m_sprite.Begin(SpriteFlags.AlphaBlend);
					float xscale = (float)width / iconButton.SurfaceDescription.Width;
					float yscale = (float)height / iconButton.SurfaceDescription.Height;
					m_sprite.Transform = Matrix.Scaling(xscale,yscale,0);
					m_sprite.Transform *= Matrix.Translation(x, y, 0); //+ width / 2, y + height / 2, 0);
					
					m_sprite.Draw( iconButton.Texture,
						new Vector3(iconButton.SurfaceDescription.Width / 2, iconButton.SurfaceDescription.Height / 2, 0.0f),//Vector3.Empty,
						Vector3.Empty,
						System.Drawing.Color.White.ToArgb() 
						);
				
					// Reset transform to prepare for text rendering later
					m_sprite.Transform = Matrix.Identity;
					m_sprite.End();

					if(enabled)
					{
						m_ChevronVerts[0].Color = System.Drawing.Color.White.ToArgb();
						m_ChevronVerts[0].X = x - 3;
						m_ChevronVerts[0].Y = y - height / 2 - 5;
						m_ChevronVerts[0].Z = 0.0f;

						m_ChevronVerts[1].Color = System.Drawing.Color.White.ToArgb();
						m_ChevronVerts[1].X = x;
						m_ChevronVerts[1].Y = y - height / 2;
						m_ChevronVerts[1].Z = 0.0f;

						m_ChevronVerts[2].Color = System.Drawing.Color.White.ToArgb();
						m_ChevronVerts[2].X = x + 3;
						m_ChevronVerts[2].Y = y - height / 2 - 5;
						m_ChevronVerts[2].Z = 0.0f;

						m_Device3d.VertexFormat = CustomVertex.TransformedColored.Format;
						m_Device3d.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
						m_Device3d.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
						m_Device3d.TextureState[0].AlphaOperation = TextureOperation.Disable;

						m_Device3d.DrawUserPrimitives(PrimitiveType.TriangleList, 1, m_ChevronVerts);
					}
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		System.Drawing.Point m_LastMousePosition = System.Drawing.Point.Empty;

		protected override void OnMouseLeave(EventArgs e)
		{
			m_LastMousePosition = System.Drawing.Point.Empty;

			base.OnMouseLeave (e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{

			m_LastMousePosition = new Point(e.X, e.Y);
			base.OnMouseMove (e);
		}

		//Point mp = new Point();

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				m_LastMousePosition = new Point(e.X, e.Y);
				System.Collections.ArrayList renderList = new ArrayList();
				System.Collections.ArrayList roList = new ArrayList();
		
				if(m_WorldWindow.CurrentWorld != null)
				{
					for(int i = 0; i < m_WorldWindow.CurrentWorld.RenderableObjects.Count; i++)
					{
						MFW3D.Renderable.RenderableObject ro = (MFW3D.Renderable.RenderableObject)m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects[i];
						getRenderButtons(ro, renderList, roList);
					}
				}

				int xIncr = 0;
				int buttonSize = Height / 2;

				int totalWidth = buttonSize * renderList.Count;

				xIncr += Width / 2 - totalWidth / 2;
			
				if(m_LastMousePosition != Point.Empty && m_LastMousePosition.X >= xIncr && m_LastMousePosition.X <= xIncr + totalWidth)
				{
					int closestIcon = (m_LastMousePosition.X - xIncr) / buttonSize;
				
					MFW3D.Renderable.RenderableObject curRo = (MFW3D.Renderable.RenderableObject)roList[closestIcon];

					if(curRo != null)
						curRo.IsOn = !curRo.IsOn;

				}
			}
			base.OnMouseUp (e);
		}



		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// OverviewToolbar
			// 
			this.ClientSize = new System.Drawing.Size(600, 32);
			this.Name = "OverviewToolbar";
			this.Text = "OverviewToolbar";

		}
		#endregion

		private void m_RenderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(Visible)
				Invalidate();
		}

		private void InitializeGraphics()
		{
			// Set up our presentation parameters
			m_presentParams = new PresentParameters();

			m_presentParams.Windowed = true;
			m_presentParams.SwapEffect = SwapEffect.Discard;
			m_presentParams.AutoDepthStencilFormat = DepthFormat.D16;
			m_presentParams.EnableAutoDepthStencil = true;
			
			int adapterOrdinal = 0;
			try
			{
				// Store the default adapter
				adapterOrdinal = Manager.Adapters.Default.Adapter;
			}
			catch
			{
				// User probably needs to upgrade DirectX or install a 3D capable graphics adapter
				throw new NotAvailableException();
			}

			DeviceType dType = DeviceType.Hardware;

			foreach(AdapterInformation ai in Manager.Adapters)
			{
				if(ai.Information.Description.IndexOf("NVPerfHUD") >= 0)
				{
					adapterOrdinal = ai.Adapter;
					dType = DeviceType.Reference;
				}
			}
			CreateFlags flags = CreateFlags.SoftwareVertexProcessing;

			// Check to see if we can use a pure hardware m_Device3d
			Caps caps = Manager.GetDeviceCaps(adapterOrdinal, DeviceType.Hardware);

			// Do we support hardware vertex processing?
			if(caps.DeviceCaps.SupportsHardwareTransformAndLight)
				//	// Replace the software vertex processing
				flags = CreateFlags.HardwareVertexProcessing;

			// Use multi-threading for now - TODO: See if the code can be changed such that this isn't necessary (Texture Loading for example)
			flags |= CreateFlags.MultiThreaded;

			try
			{
				// Create our m_Device3d
				m_Device3d = new Device(adapterOrdinal, dType, this, flags, m_presentParams);
			}
			catch( Microsoft.DirectX.DirectXException	)
			{
				throw new NotSupportedException("Unable to create the Direct3D m_Device3d.");
			}

			// Hook the m_Device3d reset event
			m_Device3d.DeviceReset += new EventHandler(OnDeviceReset);
			m_Device3d.DeviceResizing += new CancelEventHandler(m_Device3d_DeviceResizing);
			OnDeviceReset(m_Device3d, null);
		}

		private void OnDeviceReset(object sender, EventArgs e)
		{
			// Can we use anisotropic texture minify filter?
			if( m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMinifyAnisotropic)
			{
				m_Device3d.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
			}
			else if( m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMinifyLinear)
			{
				m_Device3d.SamplerState[0].MinFilter = TextureFilter.Linear;
			}

			// What about magnify filter?
			if( m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMagnifyAnisotropic )
			{
				m_Device3d.SamplerState[0].MagFilter = TextureFilter.Anisotropic;
			}
			else if( m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMagnifyLinear )
			{
				m_Device3d.SamplerState[0].MagFilter = TextureFilter.Linear;
			}

			m_Device3d.SamplerState[0].AddressU = TextureAddress.Clamp;
			m_Device3d.SamplerState[0].AddressV = TextureAddress.Clamp;

			m_Device3d.RenderState.Clipping = true;
			m_Device3d.RenderState.CullMode = Cull.Clockwise;
			m_Device3d.RenderState.Lighting = false;
			m_Device3d.RenderState.Ambient = Color.FromArgb(0x40, 0x40, 0x40);

			m_Device3d.RenderState.ZBufferEnable = true;
			m_Device3d.RenderState.AlphaBlendEnable = true;
			m_Device3d.RenderState.SourceBlend = Blend.SourceAlpha;
			m_Device3d.RenderState.DestinationBlend = Blend.InvSourceAlpha;
		}

		/// <summary>
		/// Attempt to restore the 3D m_Device3d
		/// </summary>
		protected void AttemptRecovery()
		{
			try
			{
				m_Device3d.TestCooperativeLevel();
			}
			catch (DeviceLostException)
			{
			}
			catch (DeviceNotResetException)
			{
				try
				{
					m_Device3d.Reset(m_presentParams);
				}
				catch (DeviceLostException)
				{
					// If it's still lost or lost again, just do
					// nothing
				}
			}
		}

		private void m_Device3d_DeviceResizing(object sender, CancelEventArgs e)
		{
			if(this.Size.Width == 0 || this.Size.Height == 0)
			{
				e.Cancel = true;
				return;
			}
		}
	}
}
