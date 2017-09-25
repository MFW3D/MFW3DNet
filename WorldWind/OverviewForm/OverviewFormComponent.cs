using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using Utility;

namespace WorldWind.CMPlugins.OverviewForm
{
	/// <summary>
	/// Summary description for OverviewFormComponent.
	/// </summary>
	public class OverviewFormComponent : System.Windows.Forms.Control
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Device m_Device3d;
		private PresentParameters m_presentParams;
		private WorldWindow m_WorldWindow = null;
		private ImageTileCache m_ImageTileCache = null;
		private ResourceCache m_ResourceCache = null;
		private Microsoft.DirectX.Direct3D.Font m_DrawingFont = null;
		int m_MinIconSize = 64;

		System.Timers.Timer m_RenderTimer = new System.Timers.Timer(35);

		OverviewToolbar m_OverviewToolbar = null;
		
		public OverviewFormComponent(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();
		}

		public OverviewFormComponent(WorldWindow ww, Form parentForm)
		{
			InitializeComponent();

			m_WorldWindow = ww;
			
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

			// The m_Device3d can't be created unless the control is at least 1 x 1 pixels in size
			this.Size = new Size(600,300);
			
			m_OverviewToolbar = new OverviewToolbar(m_WorldWindow);
			m_OverviewToolbar.Size = new Size(Width, m_MinIconSize);
			m_OverviewToolbar.Parent = parentForm;
			m_OverviewToolbar.Visible = false;
			Controls.Add(m_OverviewToolbar);
			m_OverviewToolbar.Location = new Point(0, Height / 2 - m_MinIconSize / 2);
			
			
			try
			{
				// Now perform the rendering m_Device3d initialization
				// Skip DirectX initialization in design mode
				if(!IsInDesignMode())
					InitializeGraphics();

				m_ImageTileCache = new ImageTileCache(m_Device3d);
				m_ResourceCache = new ResourceCache();
				m_DrawingFont = new Microsoft.DirectX.Direct3D.Font(
					m_Device3d,
					new System.Drawing.Font("Tachoma", 12.0f));

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

		protected override void OnMove(EventArgs e)
		{
			base.OnMove (e);
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

				//m_Device3d.RenderState.FillMode = FillMode.WireFrame;

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

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if(m_WorldWindow != null)
			{
				m_WorldWindow.DrawArgs.WorldCamera.ZoomStepped(e.Delta/120.0f);
			}
			base.OnMouseWheel (e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Home:
					m_WorldWindow.DrawArgs.WorldCamera.ZoomStepped( World.Settings.CameraZoomStepKeyboard);
					break;
				case Keys.End:
					m_WorldWindow.DrawArgs.WorldCamera.ZoomStepped( -World.Settings.CameraZoomStepKeyboard);
					break;
			}
			base.OnKeyDown (e);
		}


		/// <summary>
		/// Render the scene.
		/// </summary>
		public void Render()
		{
			try
			{

				// Render the sky according to view - example, close to earth, render sky blue, render space as black
				System.Drawing.Color backgroundColor = System.Drawing.Color.YellowGreen;

				m_Device3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColor, 1.0f, 0);
			
				m_Device3d.BeginScene();

				System.DateTime dt = System.DateTime.Now;
				
				RenderImages(m_WorldWindow.CurrentWorld.RenderableObjects);
		
				RenderViewBox();
		
				if(m_FireMouseUpEvent)
				{
					m_FireMouseUpEvent = false;
				}
				m_Device3d.EndScene();
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
			finally
			{
				
			}
		}

		public int ToolbarSize = 64;
		private void RenderToolBar()
		{
			System.Collections.ArrayList renderList = new ArrayList();
			if(m_WorldWindow.CurrentWorld != null)
			{
				for(int i = 0; i < m_WorldWindow.CurrentWorld.RenderableObjects.Count; i++)
				{
					WorldWind.Renderable.RenderableObject ro = (WorldWind.Renderable.RenderableObject)m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects[i];
					getRenderButtons(ro, renderList);
				}
			}

			int counter = 0;
			int buttonSize = ToolbarSize;
			foreach(string buttonKey in renderList)
			{
				RenderButton(buttonKey, buttonSize / 2 + buttonSize * counter++, Height / 2, buttonSize, buttonSize);
			}
		}

		private void RenderButton(string buttonKey, int x, int y, int width, int height)
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
					m_sprite.Transform *= Matrix.Translation(x + width / 2, y - height / 2, 0);
					
					m_sprite.Draw( iconButton.Texture,
						new Vector3(width, height,0),
						Vector3.Empty,
						System.Drawing.Color.White.ToArgb() 
						);
				
					// Reset transform to prepare for text rendering later
					m_sprite.Transform = Matrix.Identity;
					m_sprite.End();
				}
			
			}
			catch(Exception ex)
			{
				Log.Write(ex);
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

		private void getRenderButtons(WorldWind.Renderable.RenderableObject ro, System.Collections.ArrayList renderList)
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
						}
					}
					else
					{
						renderList.Add(toolbarImagePath);
					}
					
				}
			}

			if(ro is WorldWind.Renderable.RenderableObjectList)
			{
				WorldWind.Renderable.RenderableObjectList rol = (WorldWind.Renderable.RenderableObjectList)ro;
				for(int i = 0; i < rol.Count; i++)
				{
					WorldWind.Renderable.RenderableObject childRo = (WorldWind.Renderable.RenderableObject)rol.ChildObjects[i];
					getRenderButtons(childRo, renderList);
				}
			}
		}

		public System.Drawing.Point GetPointFromCoord(double latitude, double longitude)
		{

			if(Math.Abs(latitude) > 90.0)
				latitude = latitude % 90.0;
			if(Math.Abs(longitude) > 180.0)
				longitude = longitude % 180.0;

			Point p = new Point();

			if(latitude == 90.0)
				p.Y = 0;
			else if(latitude == -90.0)
				p.Y = Height;
			else
			{
				p.Y = (int)(Height - ((latitude + 90.0) / 180.0) * Height);
			}

			if(longitude == -180.0)
				p.X = 0;
			else if(longitude == 180.0)
				p.X = Width;
			else
			{
				p.X = (int)(((longitude + 180.0) / 360.0) * Width);
			}

			return p;
		}

		CustomVertex.TransformedTextured[] m_Verts = new Microsoft.DirectX.Direct3D.CustomVertex.TransformedTextured[4];

		private void RenderImageLayer(WorldWind.Renderable.ImageLayer imageLayer)
		{


			if(imageLayer.ImagePath != null)
			{
				ImageTileCacheEntry tileEntry = m_ImageTileCache[imageLayer.ImagePath];
				
				if(tileEntry == null)
				{
					tileEntry = new ImageTileCacheEntry(
						imageLayer.ImagePath,
						imageLayer.MaxLat, imageLayer.MinLat,
						imageLayer.MinLon, imageLayer.MaxLon);
					tileEntry.Load(m_Device3d);
					m_ImageTileCache.Add(tileEntry);
				}

				RenderImageTileCacheEntry(tileEntry);			
			}
		}

		private void RenderQuadTileSet(WorldWind.Renderable.QuadTileSet quadTileSet)
		{
			try
			{
			//	foreach(WorldWind.Renderable.QuadTile qt in quadTileSet.TopmostTiles.Values)
			//	{
			//		RenderQuadTile(qt);
			//	}
			}
			catch{}

		}

		private void RenderQuadTile(WorldWind.Renderable.QuadTile quadTile)
		{
			if(quadTile.isInitialized)
			{
			//	ImageTileCacheEntry tileEntry = (ImageTileCacheEntry)m_ImageTileCache[quadTile.ImageTileInfo.ImagePath];
//
//				if(tileEntry == null)
//				{
//					tileEntry = new ImageTileCacheEntry(
//						quadTile.ImageTileInfo.ImagePath,
//						quadTile.North, quadTile.South, quadTile.West, quadTile.East);
//					tileEntry.Load(m_Device3d);
//					m_ImageTileCache.Add(tileEntry);
//				}

//				RenderImageTileCacheEntry(tileEntry);
			}
		}

		private void RenderImageTileCacheEntry(ImageTileCacheEntry tileEntry)
		{
			if(tileEntry != null && tileEntry.Texture != null)
			{
				//first, handle case when minLon < maxLon (as opposed to the opposite when the image crosses the -180/180 longitude boundary)
				if(tileEntry.West < tileEntry.East)
				{
					m_Device3d.SetTexture(0, tileEntry.Texture);
							
					m_Device3d.RenderState.ZBufferEnable = false;

					Point ul = GetPointFromCoord(tileEntry.North, tileEntry.West);
					Point ur = GetPointFromCoord(tileEntry.North, tileEntry.East);
					Point ll = GetPointFromCoord(tileEntry.South, tileEntry.West);
					Point lr = GetPointFromCoord(tileEntry.South, tileEntry.East);
									
					if(m_sprite == null)
						m_sprite = new Sprite(m_Device3d);

					m_sprite.Begin(SpriteFlags.AlphaBlend);

					float xscale = (float)(ur.X - ul.X) / (float)tileEntry.SurfaceDescription.Width;
					float yscale = (float)(lr.Y - ur.Y) / (float)tileEntry.SurfaceDescription.Height;
					m_sprite.Transform = Matrix.Scaling(xscale,yscale,0);
					m_sprite.Transform *= Matrix.Translation(0.5f * (ul.X + ur.X), 0.5f * (ur.Y + lr.Y), 0);
					m_sprite.Draw( tileEntry.Texture,
						new Vector3(tileEntry.SurfaceDescription.Width / 2, tileEntry.SurfaceDescription.Height / 2,0),
						Vector3.Empty,
						System.Drawing.Color.White.ToArgb() 
						);
				
					// Reset transform to prepare for text rendering later
					m_sprite.Transform = Matrix.Identity;
					m_sprite.End();
				}
				else
				{
					m_Device3d.SetTexture(0, tileEntry.Texture);
							
					m_Device3d.RenderState.ZBufferEnable = false;

					Point ul = GetPointFromCoord(tileEntry.North, tileEntry.West);
					Point ur = GetPointFromCoord(tileEntry.North, tileEntry.East);
					Point ll = GetPointFromCoord(tileEntry.South, tileEntry.West);
					Point lr = GetPointFromCoord(tileEntry.South, tileEntry.East);

					if(m_sprite == null)
						m_sprite = new Sprite(m_Device3d);

					m_sprite.Begin(SpriteFlags.AlphaBlend);

					float xscale = (float)((Width - ul.X) + ur.X) / (float)tileEntry.SurfaceDescription.Width;
					float yscale = (float)(lr.Y - ur.Y) / (float)tileEntry.SurfaceDescription.Height;
					m_sprite.Transform = Matrix.Scaling(xscale,yscale,0);
					m_sprite.Transform *= Matrix.Translation(ul.X, ur.Y, 0);
					m_sprite.Draw( tileEntry.Texture,
						Vector3.Empty,//new Vector3(tileEntry.SurfaceDescription.Width / 2, tileEntry.SurfaceDescription.Height / 2,0),
						Vector3.Empty,
						System.Drawing.Color.White.ToArgb() 
						);
				
					// Reset transform to prepare for text rendering later
					m_sprite.Transform = Matrix.Identity;

					m_sprite.Transform = Matrix.Scaling(xscale,yscale,0);
					m_sprite.Transform *= Matrix.Translation(ur.X, ur.Y, 0);
					m_sprite.Draw( tileEntry.Texture,
						new Vector3(tileEntry.SurfaceDescription.Width,0 ,0),
						Vector3.Empty,
						System.Drawing.Color.White.ToArgb() 
						);
				
					// Reset transform to prepare for text rendering later
					m_sprite.Transform = Matrix.Identity;
					m_sprite.End();
				}
			}
		}

		private string GetAbsoluteRenderableObjectPath(WorldWind.Renderable.RenderableObject ro)
		{
			if(ro.ParentList != null)
			{
				return GetAbsoluteRenderableObjectPath(ro.ParentList) + "//" + ro.Name;
			}
			else
			{
				return ro.Name;
			}
		}

		private void RenderImages(WorldWind.Renderable.RenderableObjectList rol)
		{
			if(rol.IsOn)
			{
				for(int i = 0; i < rol.ChildObjects.Count; i++)
				{
					WorldWind.Renderable.RenderableObject curObject = (WorldWind.Renderable.RenderableObject)rol.ChildObjects[i];
					if(curObject is WorldWind.Renderable.RenderableObjectList)
					{
						RenderImages(curObject as WorldWind.Renderable.RenderableObjectList);
					}
					if(curObject.IsOn)
					{
						if(curObject is WorldWind.Renderable.ImageLayer)
						{
						
							WorldWind.Renderable.ImageLayer curImageLayer = (WorldWind.Renderable.ImageLayer)curObject;
							RenderImageLayer(curImageLayer);
						
						}
						else if(curObject is WorldWind.Renderable.QuadTileSet)
						{
							WorldWind.Renderable.QuadTileSet curQuadTileSet = (WorldWind.Renderable.QuadTileSet)curObject;
							RenderQuadTileSet(curQuadTileSet); 
						}
						else if(curObject is WorldWind.Renderable.TerrainPath)
						{
							try
							{
								WorldWind.Renderable.TerrainPath curTerrainPath = (WorldWind.Renderable.TerrainPath)curObject;
								RenderTerrainPath(curTerrainPath);
							}
							catch(Exception ex)
							{
								Log.Write(ex);
							}
						}
						else if(curObject is WorldWind.Renderable.TiledPlacenameSet)
						{
							try
							{
								WorldWind.Renderable.TiledPlacenameSet curTiledPlacenameSet = (WorldWind.Renderable.TiledPlacenameSet)curObject;
								RenderTiledPlacenameSet(curTiledPlacenameSet);
							}
							catch(Exception ex)
							{
								Log.Write(ex);
							}
						}
						else if(curObject is WorldWind.Renderable.Icon)
						{
							try
							{
								WorldWind.Renderable.Icon curIcon = (WorldWind.Renderable.Icon)curObject;
								RenderIcon(curIcon);
							}
							catch(Exception ex)
							{
								Log.Write(ex);
							}
						}
						else if(curObject is WorldWind.Renderable.DownloadableImageFromIconSet)
						{
							try
							{
								WorldWind.Renderable.DownloadableImageFromIconSet iconSet = (WorldWind.Renderable.DownloadableImageFromIconSet)curObject;
								RenderDownloadableImageFromIconSet(iconSet);
							}
							catch(Exception ex)
							{
								Log.Write(ex);
							}
						}
					}
				}		
			}
		}

		private void RenderDownloadableImageFromIconSet(WorldWind.Renderable.DownloadableImageFromIconSet iconSet)
		{
			if(iconSet.isInitialized)
			{
				WorldWind.Renderable.DownloadableIcon[] icons = iconSet.DownloadableIcons;
				
				foreach(WorldWind.Renderable.DownloadableIcon icon in icons)
				{
					if(icon.imageLayer != null && icon.imageLayer.isInitialized)
					{
						this.RenderImageLayer(icon.imageLayer);
					}
					ResourceCacheEntry iconResource = m_ResourceCache[icon.IconFilePath];
					if(iconResource == null)
					{
						iconResource = new ResourceCacheEntry();
						iconResource.Object = new WorldWind.Renderable.IconTexture(m_Device3d, icon.IconFilePath);

						m_ResourceCache.Add(icon.IconFilePath, iconResource);
					}

					WorldWind.Renderable.IconTexture iconTexture = (WorldWind.Renderable.IconTexture)iconResource.Object;
				
					Point p = this.GetPointFromCoord(icon.Latitude, icon.Longitude);

					bool isMouseOver = (Math.Sqrt( Math.Pow(p.X - this.m_LastMousePosition.X, 2) + Math.Pow(p.Y - this.m_LastMousePosition.Y, 2)) < icon.Width ? true : false);
					if(m_sprite == null)
						m_sprite = new Sprite(m_Device3d);

					m_sprite.Begin(SpriteFlags.AlphaBlend);

					int color = isMouseOver ? hotColor : normalColor;
			
					// Render label
					if(icon.Name != null && isMouseOver)
					{
						// Render name field
						const int labelWidth = 1000; // Dummy value needed for centering the text
						if(iconTexture == null)
						{
							// Center over target as we have no bitmap
							Rectangle rect = new Rectangle(
								(int)p.X - (labelWidth>>1), 
								(int)(p.Y - (m_DrawingFont.Description.Height >> 1)),
								labelWidth, 
								Height );

							m_DrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.Center, color);
						}
						else
						{
							// Adjust text to make room for icon
							int spacing = (int)(icon.Width * 0.3f);
							if(spacing>10)
								spacing = 10;
							int offsetForIcon = (icon.Width>>1) + spacing;

							Rectangle rect = new Rectangle(
								(int)p.X + offsetForIcon, 
								(int)(p.Y - (m_DrawingFont.Description.Height >> 1)),
								labelWidth, 
								Height );

							m_DrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.WordBreak, color);
						}
					}

					if(iconTexture != null)
					{
						float xscale = (float)icon.Width / iconTexture.Width;
						float yscale = (float)icon.Height / iconTexture.Height;
						m_sprite.Transform = Matrix.Scaling(xscale,yscale,0);
						m_sprite.Transform *= Matrix.Translation(p.X, p.Y, 0);
						m_sprite.Draw( iconTexture.Texture,
							new Vector3(iconTexture.Width>>1, iconTexture.Height>>1,0),
							Vector3.Empty,
							color 
							);
				
						// Reset transform to prepare for text rendering later
						m_sprite.Transform = Matrix.Identity;
					}
					m_sprite.End();
				}
			}
		}

		static int hotColor = Color.White.ToArgb();
		static int normalColor = Color.FromArgb(150,255,255,255).ToArgb();
		static int nameColor = Color.White.ToArgb();
		static int descriptionColor = Color.White.ToArgb();

		bool m_FireMouseUpEvent = false;

		private void RenderIcon(WorldWind.Renderable.Icon icon)
		{
			if(icon.isInitialized && icon.MaximumDisplayDistance > 100000 && icon.MaximumDisplayDistance != 0)
			{
				ResourceCacheEntry iconResource = m_ResourceCache[icon.TextureFileName];
				if(iconResource == null)
				{
					iconResource = new ResourceCacheEntry();
					iconResource.Object = new WorldWind.Renderable.IconTexture(m_Device3d, icon.TextureFileName);

					m_ResourceCache.Add(icon.TextureFileName, iconResource);
				}

				WorldWind.Renderable.IconTexture iconTexture = (WorldWind.Renderable.IconTexture)iconResource.Object;
				
				Point p = this.GetPointFromCoord(icon.Latitude, icon.Longitude);

				bool isMouseOver = (Math.Sqrt( Math.Pow(p.X - this.m_LastMousePosition.X, 2) + Math.Pow(p.Y - this.m_LastMousePosition.Y, 2)) < icon.Width ? true : false);
				if(m_sprite == null)
					m_sprite = new Sprite(m_Device3d);

				m_sprite.Begin(SpriteFlags.AlphaBlend);

				int color = isMouseOver ? hotColor : normalColor;
			
				// Render label
				if(icon.Name != null && isMouseOver)
				{
					// Render name field
					const int labelWidth = 1000; // Dummy value needed for centering the text
					if(iconTexture == null)
					{
						// Center over target as we have no bitmap
						Rectangle rect = new Rectangle(
							(int)p.X - (labelWidth>>1), 
							(int)(p.Y - (m_DrawingFont.Description.Height >> 1)),
							labelWidth, 
							Height );

						m_DrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.Center, color);
					}
					else
					{
						// Adjust text to make room for icon
						int spacing = (int)(icon.Width * 0.3f);
						if(spacing>10)
							spacing = 10;
						int offsetForIcon = (icon.Width>>1) + spacing;

						Rectangle rect = new Rectangle(
							(int)p.X + offsetForIcon, 
							(int)(p.Y - (m_DrawingFont.Description.Height >> 1)),
							labelWidth, 
							Height );

						m_DrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.WordBreak, color);

						
					}
				}

				if(iconTexture != null)
				{
					float xscale = (float)icon.Width / iconTexture.Width;
					float yscale = (float)icon.Height / iconTexture.Height;
					m_sprite.Transform = Matrix.Scaling(xscale,yscale,0);
					m_sprite.Transform *= Matrix.Translation(p.X, p.Y, 0);
					m_sprite.Draw( iconTexture.Texture,
						new Vector3(iconTexture.Width>>1, iconTexture.Height>>1,0),
						Vector3.Empty,
						color 
						);
				
					// Reset transform to prepare for text rendering later
					m_sprite.Transform = Matrix.Identity;
				}

				if(m_FireMouseUpEvent && icon.ClickableActionURL != null)
				{
					if(m_LastMousePosition.X >= p.X - icon.Width / 2 &&
						m_LastMousePosition.X <= p.X + icon.Width / 2 &&								
						m_LastMousePosition.Y >= p.Y - icon.Width / 2 &&
						m_LastMousePosition.Y <= p.Y + icon.Width / 2)
					{
						if(icon.OnClickZoomAltitude != double.NaN || icon.OnClickZoomHeading != double.NaN || icon.OnClickZoomTilt != double.NaN)
						{
							m_WorldWindow.DrawArgs.WorldCamera.SetPosition(
								icon.Latitude,
								icon.Longitude,
								icon.OnClickZoomHeading,
								icon.OnClickZoomAltitude, 
								icon.OnClickZoomTilt);

						}

						IconClickEvent ice = new IconClickEvent(icon.ClickableActionURL);
						System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ice.Go));
						t.Start();
					}
				}
				m_sprite.End();
			}
		}

		class IconClickEvent
		{
			string m_ClickableActionUrl = null;
			public IconClickEvent(string clickableActionUrl)
			{	
				m_ClickableActionUrl = clickableActionUrl;
			}

			public void Go()
			{
				try
				{
					ProcessStartInfo psi = new ProcessStartInfo();
					psi.FileName = m_ClickableActionUrl;
					psi.Verb = "open";
					psi.UseShellExecute = true;

					psi.CreateNoWindow = true;
					Process.Start(psi);
				}
				catch
				{
				}
			}
		}

		System.Collections.Hashtable m_FontHash = new Hashtable();
		Sprite m_sprite = null;

		private void RenderTiledPlacenameSet(WorldWind.Renderable.TiledPlacenameSet tiledPlacenameSet)
		{
			if(tiledPlacenameSet.isInitialized)
			{
				Microsoft.DirectX.Direct3D.Font font = (Microsoft.DirectX.Direct3D.Font)m_FontHash[tiledPlacenameSet.FontDescription];

				if(font == null)
				{
					font = new Microsoft.DirectX.Direct3D.Font(m_Device3d, tiledPlacenameSet.FontDescription);
					m_FontHash.Add(tiledPlacenameSet.FontDescription, font);
				}

				try
				{
						
					for(int i = 0; i < tiledPlacenameSet.PlaceNames.Length; i++)
					{
						Point p = GetPointFromCoord(tiledPlacenameSet.PlaceNames[i].Lat, tiledPlacenameSet.PlaceNames[i].Lon);
						font.DrawText(
							null,
							tiledPlacenameSet.PlaceNames[i].Name,
							new System.Drawing.Rectangle(p.X, p.Y, Width, Height),
							DrawTextFormat.NoClip,
							tiledPlacenameSet.Color);
					}
				}
				catch
				{}
			}
		}

		private void RenderTerrainPath(WorldWind.Renderable.TerrainPath terrainPath)
		{
			//if(terrainPath != null)
			{
				string key = GetAbsoluteRenderableObjectPath(terrainPath);

				ResourceCacheEntry resource = m_ResourceCache[key];

				if(resource == null || (resource != null && resource.LastUpdated < m_LastResize))
				{
					Point prevPoint = Point.Empty;

					System.Collections.ArrayList list = new ArrayList();

					Vector3[] coords = new Vector3[0];

					if(terrainPath.SphericalCoordinates == null || terrainPath.SphericalCoordinates.Length == 0)
					{
						//coords = terrainPath.GetSphericalCoordinates();
						terrainPath.Load();
						
					}
					
					coords = terrainPath.SphericalCoordinates;

					for(int i = 0; i < coords.Length; i++)
					{
						if(prevPoint == Point.Empty)
						{
							// x == latitude, y == longitude (don't ask me why because i don't know)
							prevPoint = GetPointFromCoord(
								coords[i].X,
								coords[i].Y);
							
							list.Add(prevPoint);
						}
						else
						{
							Point newPoint = GetPointFromCoord(
								coords[i].X,
								coords[i].Y);

							if(newPoint.X == prevPoint.X && newPoint.Y == prevPoint.Y)
							{
								continue;
							}
							else
							{
								list.Add(newPoint);
								prevPoint = newPoint;
							}
						}
					}
				

					CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[list.Count];
					for(int i = 0; i < verts.Length; i++)
					{
						Point curP = (Point)list[i];

						verts[i].Color = terrainPath.LineColor;
						verts[i].X = curP.X;
						verts[i].Y = curP.Y;
						verts[i].Z = 0.0f;
					}

					if(resource == null)
					{
						resource = new ResourceCacheEntry();
					}
					else
					{
						resource.LastUpdated = System.DateTime.Now;
					}
					resource.Object = verts;
					m_ResourceCache.Add(key, resource);
				}

				CustomVertex.TransformedColored[] v = (CustomVertex.TransformedColored[])resource.Object;
				
				if(v.Length > 0)
				{
					m_Device3d.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
					m_Device3d.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
					m_Device3d.TextureState[0].AlphaOperation = TextureOperation.Disable;

					m_Device3d.VertexFormat = CustomVertex.TransformedColored.Format;
					m_Device3d.DrawUserPrimitives(PrimitiveType.LineStrip, v.Length - 1, v);
				}
			}
		}

		CustomVertex.TransformedColored[] m_ViewVerts = new Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored[6];
		CustomVertex.TransformedColored[] m_ViewOutlineVerts = new Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored[5];
		
		System.Drawing.Color m_ViewBoxColor = System.Drawing.Color.FromArgb(150, System.Drawing.Color.Red.R, System.Drawing.Color.Red.G, System.Drawing.Color.Red.B);
		System.Drawing.Color m_ViewBoxOutlineColor = System.Drawing.Color.Red;
		System.Drawing.Color m_ViewBoxInsetColor = System.Drawing.Color.Red;
		float m_ViewBoxCenterPointSize = 5.0f;

		System.Drawing.Color m_TargetViewBoxColor = System.Drawing.Color.FromArgb(150, System.Drawing.Color.Purple.R, System.Drawing.Color.Purple.G, System.Drawing.Color.Purple.B);
		System.Drawing.Color m_TargetViewBoxOutlineColor = System.Drawing.Color.Purple;
		int m_MinViewBoxRadius = 10;

		bool m_MouseIsDragging = false;
		bool m_IsPanning = false;
		bool m_RenderTargetViewBox = false;
		
		Point m_MouseDownStartPosition = Point.Empty;
		Point m_LastMousePosition = new Point();
		Point m_TargetRenderBoxPosition = System.Drawing.Point.Empty;
		float m_PanSpeed = 0.2f;
		
		System.DateTime m_LastResize = System.DateTime.Now;
		System.DateTime m_LeftMouseDownTime = System.DateTime.MaxValue;
		System.TimeSpan m_MousePanHoldInterval = TimeSpan.FromSeconds(1);
		
		private void RenderViewBox()
		{
			double halfVr = m_WorldWindow.DrawArgs.WorldCamera.ViewRange.Degrees * 0.5;

			Point center = GetPointFromCoord(m_WorldWindow.DrawArgs.WorldCamera.Latitude.Degrees, m_WorldWindow.DrawArgs.WorldCamera.Longitude.Degrees);
			float dppX = (float)Width / 360.0f;
			float dppY = (float)Height / 180.0f;

			double percentTilt = m_WorldWindow.DrawArgs.WorldCamera.Tilt.Degrees / 90.0;
			double trueHeading = m_WorldWindow.DrawArgs.WorldCamera.Heading.Radians - Math.PI * .5;
			
			Point ul = new Point(
				(int)(center.X + Math.Cos(trueHeading + Math.PI * .25 - (Math.PI * .25 * percentTilt)) * (halfVr * dppX)),
				(int)(center.Y - Math.Sin(trueHeading + Math.PI * .25 - (Math.PI * .25 * percentTilt)) * (halfVr * dppY)));
			
			Point ll = new Point(
				(int)(center.X + Math.Cos(trueHeading + Math.PI * .75) * (halfVr * dppX)),
				(int)(center.Y - Math.Sin(trueHeading + Math.PI * .75) * (halfVr * dppY)));
			
			Point ur = new Point(
				(int)(center.X + Math.Cos(trueHeading - Math.PI * .25 + (Math.PI * .25 * percentTilt)) * (halfVr * dppX)),
				(int)(center.Y - Math.Sin(trueHeading - Math.PI * .25 + (Math.PI * .25 * percentTilt)) * (halfVr * dppY)));
			
			Point lr = new Point(
				(int)(center.X + Math.Cos(trueHeading - Math.PI * .75) * (halfVr * dppX)),
				(int)(center.Y - Math.Sin(trueHeading - Math.PI * .75) * (halfVr * dppY)));

			double viewBoxRadius = Math.Sqrt(Math.Pow(ul.X - center.X, 2) + Math.Pow(ul.Y - center.Y, 2));

			Point insetUl = ul;
			Point insetUr = ur;
			Point insetLl = ll;
			Point insetLr = lr;

			if(viewBoxRadius < m_MinViewBoxRadius)
			{
				ul = new Point(
					(int)(center.X + Math.Cos(trueHeading + Math.PI * .25 - (Math.PI * .25 * percentTilt)) * (m_MinViewBoxRadius * 0.5)),
					(int)(center.Y - Math.Sin(trueHeading + Math.PI * .25 - (Math.PI * .25 * percentTilt)) * (m_MinViewBoxRadius * 0.5)));
			
				ll = new Point(
					(int)(center.X + Math.Cos(trueHeading + Math.PI * .75) * (m_MinViewBoxRadius * 0.5)),
					(int)(center.Y - Math.Sin(trueHeading + Math.PI * .75) * (m_MinViewBoxRadius * 0.5)));
			
				ur = new Point(
					(int)(center.X + Math.Cos(trueHeading - Math.PI * .25 + (Math.PI * .25 * percentTilt)) * (m_MinViewBoxRadius * 0.5)),
					(int)(center.Y - Math.Sin(trueHeading - Math.PI * .25 + (Math.PI * .25 * percentTilt)) * (m_MinViewBoxRadius * 0.5)));
			
				lr = new Point(
					(int)(center.X + Math.Cos(trueHeading - Math.PI * .75) * (m_MinViewBoxRadius * 0.5)),
					(int)(center.Y - Math.Sin(trueHeading - Math.PI * .75) * (m_MinViewBoxRadius * 0.5)));
				
			}

			m_ViewVerts[0].Color = m_ViewBoxColor.ToArgb();
			m_ViewVerts[0].X = (float)center.X;
			m_ViewVerts[0].Y = (float)center.Y;
			m_ViewVerts[0].Z = 0.0f;
			
			m_ViewVerts[1].Color = m_ViewBoxColor.ToArgb();
			m_ViewVerts[1].X = (float)ul.X;
			m_ViewVerts[1].Y = (float)ul.Y;
			m_ViewVerts[1].Z = 0.0f;
			
			m_ViewVerts[2].Color = m_ViewBoxColor.ToArgb();
			m_ViewVerts[2].X = (float)ll.X;
			m_ViewVerts[2].Y = (float)ll.Y;
			m_ViewVerts[2].Z = 0.0f;
			
			m_ViewVerts[3].Color = m_ViewBoxColor.ToArgb();
			m_ViewVerts[3].X = (float)lr.X;
			m_ViewVerts[3].Y = (float)lr.Y;
			m_ViewVerts[3].Z = 0.0f;

			m_ViewVerts[4].Color = m_ViewBoxColor.ToArgb();
			m_ViewVerts[4].X = (float)ur.X;
			m_ViewVerts[4].Y = (float)ur.Y;
			m_ViewVerts[4].Z = 0.0f;

			m_ViewVerts[5].Color = m_ViewBoxColor.ToArgb();
			m_ViewVerts[5].X = (float)ul.X;
			m_ViewVerts[5].Y = (float)ul.Y;
			m_ViewVerts[5].Z = 0.0f;

			m_Device3d.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
			m_Device3d.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;

			m_Device3d.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
			m_Device3d.TextureState[0].AlphaArgument1 = TextureArgument.Diffuse;

			m_Device3d.VertexFormat = CustomVertex.TransformedColored.Format;
			m_Device3d.DrawUserPrimitives(PrimitiveType.TriangleFan, m_ViewVerts.Length - 2, m_ViewVerts);

			for(int i = 0; i < m_ViewOutlineVerts.Length; i++)
			{
				m_ViewOutlineVerts[i].Color = m_ViewBoxOutlineColor.ToArgb();
				m_ViewOutlineVerts[i].X = m_ViewVerts[i+1].X;
				m_ViewOutlineVerts[i].Y = m_ViewVerts[i+1].Y;
				m_ViewOutlineVerts[i].Z = m_ViewVerts[i+1].Z;
			}

			m_Device3d.DrawUserPrimitives(PrimitiveType.LineStrip, m_ViewOutlineVerts.Length - 1, m_ViewOutlineVerts);
			m_Device3d.RenderState.PointSize = m_ViewBoxCenterPointSize;

			m_Device3d.TextureState[0].AlphaOperation = TextureOperation.Disable;
			m_Device3d.DrawUserPrimitives(PrimitiveType.PointList, 1, m_ViewVerts);

			if(viewBoxRadius < m_MinViewBoxRadius)
			{
				m_ViewVerts[1].Color = m_ViewBoxInsetColor.ToArgb();
				m_ViewVerts[1].X = (float)insetUl.X;
				m_ViewVerts[1].Y = (float)insetUl.Y;
				m_ViewVerts[1].Z = 0.0f;
			
				m_ViewVerts[2].Color = m_ViewBoxInsetColor.ToArgb();
				m_ViewVerts[2].X = (float)insetLl.X;
				m_ViewVerts[2].Y = (float)insetLl.Y;
				m_ViewVerts[2].Z = 0.0f;
			
				m_ViewVerts[3].Color = m_ViewBoxInsetColor.ToArgb();
				m_ViewVerts[3].X = (float)insetLr.X;
				m_ViewVerts[3].Y = (float)insetLr.Y;
				m_ViewVerts[3].Z = 0.0f;

				m_ViewVerts[4].Color = m_ViewBoxInsetColor.ToArgb();
				m_ViewVerts[4].X = (float)insetUr.X;
				m_ViewVerts[4].Y = (float)insetUr.Y;
				m_ViewVerts[4].Z = 0.0f;

				m_ViewVerts[5].Color = m_ViewBoxInsetColor.ToArgb();
				m_ViewVerts[5].X = (float)insetUl.X;
				m_ViewVerts[5].Y = (float)insetUl.Y;
				m_ViewVerts[5].Z = 0.0f;

				m_Device3d.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
				m_Device3d.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;

				m_Device3d.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
				m_Device3d.TextureState[0].AlphaArgument1 = TextureArgument.Diffuse;

				m_Device3d.VertexFormat = CustomVertex.TransformedColored.Format;
				m_Device3d.DrawUserPrimitives(PrimitiveType.TriangleFan, m_ViewVerts.Length - 2, m_ViewVerts);
			}

			if(m_RenderTargetViewBox)
				RenderTargetViewBox();

		}

		private void RenderTargetViewBox()
		{
			System.Drawing.Point p1 = new Point(
				(m_MouseDownStartPosition.X < m_LastMousePosition.X ? m_MouseDownStartPosition.X : m_LastMousePosition.X),
				(m_MouseDownStartPosition.Y >= m_LastMousePosition.Y ? m_MouseDownStartPosition.Y : m_LastMousePosition.Y)
				);

			System.Drawing.Point p2 = new Point(
				p1.X,
				(m_MouseDownStartPosition.Y < m_LastMousePosition.Y ? m_MouseDownStartPosition.Y : m_LastMousePosition.Y)
				);

			Point p3 = new Point(
				(m_MouseDownStartPosition.X >= m_LastMousePosition.X ? m_MouseDownStartPosition.X : m_LastMousePosition.X),
				p1.Y
				);

			Point p4 = new Point(
				p3.X,
				p2.Y);

			m_ViewVerts[0].Color = m_TargetViewBoxColor.ToArgb();
			m_ViewVerts[0].X = (float)p2.X;
			m_ViewVerts[0].Y = (float)p2.Y;
			m_ViewVerts[0].Z = 0.0f;
			
			m_ViewVerts[1].Color = m_TargetViewBoxColor.ToArgb();
			m_ViewVerts[1].X = (float)p1.X;
			m_ViewVerts[1].Y = (float)p1.Y;
			m_ViewVerts[1].Z = 0.0f;
			
			m_ViewVerts[2].Color = m_TargetViewBoxColor.ToArgb();
			m_ViewVerts[2].X = (float)p4.X;
			m_ViewVerts[2].Y = (float)p4.Y;
			m_ViewVerts[2].Z = 0.0f;
			
			m_ViewVerts[3].Color = m_TargetViewBoxColor.ToArgb();
			m_ViewVerts[3].X = (float)p3.X;
			m_ViewVerts[3].Y = (float)p3.Y;
			m_ViewVerts[3].Z = 0.0f;

			m_Device3d.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
			m_Device3d.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;

			m_Device3d.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
			m_Device3d.TextureState[0].AlphaArgument1 = TextureArgument.Diffuse;

			m_Device3d.VertexFormat = CustomVertex.TransformedColored.Format;
			m_Device3d.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, m_ViewVerts);


			m_ViewVerts[0].Color = m_TargetViewBoxOutlineColor.ToArgb();
			m_ViewVerts[0].X = (float)p1.X;
			m_ViewVerts[0].Y = (float)p1.Y;
			m_ViewVerts[0].Z = 0.0f;
			
			m_ViewVerts[1].Color = m_TargetViewBoxOutlineColor.ToArgb();
			m_ViewVerts[1].X = (float)p2.X;
			m_ViewVerts[1].Y = (float)p2.Y;
			m_ViewVerts[1].Z = 0.0f;
			
			m_ViewVerts[2].Color = m_TargetViewBoxOutlineColor.ToArgb();
			m_ViewVerts[2].X = (float)p4.X;
			m_ViewVerts[2].Y = (float)p4.Y;
			m_ViewVerts[2].Z = 0.0f;
			
			m_ViewVerts[3].Color = m_TargetViewBoxOutlineColor.ToArgb();
			m_ViewVerts[3].X = (float)p3.X;
			m_ViewVerts[3].Y = (float)p3.Y;
			m_ViewVerts[3].Z = 0.0f;

			m_ViewVerts[4].Color = m_TargetViewBoxOutlineColor.ToArgb();
			m_ViewVerts[4].X = (float)p1.X;
			m_ViewVerts[4].Y = (float)p1.Y;
			m_ViewVerts[4].Z = 0.0f;

		
			m_Device3d.DrawUserPrimitives(PrimitiveType.LineStrip, 4, m_ViewVerts);
		}
		
		private Vector2 GetGeoCoordFromScreenPoint(Point p)
		{
			return new Vector2(
				(float)p.X / (float)Width * 360.0f - 180.0f,
				(float)(Height - p.Y) / (float)Height * 180.0f - 90.0f);
		}



		private bool IsPointInPolygon(Point p, CustomVertex.TransformedColored[] verts)
		{
			// The function will return TRUE if the point x,y is inside the
			// polygon, or FALSE if it is not. If the point x,y is exactly on
			// the edge of the polygon, then the function may return TRUE or
			// FALSE.
			//
			// Note that division by zero is avoided because the division is
			// protected by the "if" clause which surrounds it.


			int      i, j=0         ;
			bool  oddNODES=false ;

			for (i=0; i<verts.Length; i++) 
			{
				j++; 
				if (j==verts.Length) 
					j=0;
				if (verts[i].Y < p.Y && verts[j].Y >= p.Y
					||  verts[j].Y < p.Y && verts[i].Y >= p.Y) 
				{
					if (verts[i].X + (p.Y - verts[i].Y)/(verts[j].Y-verts[i].Y)*(verts[j].X-verts[i].X) < p.X) 
					{
						oddNODES=!oddNODES; 
					}
				}
			}
			return oddNODES; 
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			bool isMouseLeftButtonDown = ((int)e.Button & (int)MouseButtons.Left) != 0;
			bool isMouseRightButtonDown = ((int)e.Button & (int)MouseButtons.Right) != 0;
			if (!m_IsPanning && isMouseLeftButtonDown || isMouseRightButtonDown)
			{
				int dx = m_MouseDownStartPosition.X - e.X;
				int dy = m_MouseDownStartPosition.Y - e.Y;
				int distanceSquared = dx*dx + dy*dy;
				if (distanceSquared > 3*3)
					// Distance > 3 = drag
					m_MouseIsDragging = true;
			}

			if(isMouseLeftButtonDown && m_MouseIsDragging)
			{
				if(!m_RenderTargetViewBox)
				{
					m_RenderTargetViewBox = true;
				}
			}
			else if (!isMouseLeftButtonDown && isMouseRightButtonDown && m_MouseIsDragging)
			{
				float gX = (float)Width / 360.0f;
				float gY = (float)Height / 180.0f;

				float dX = m_LastMousePosition.X - e.X;
				float dY = e.Y - m_LastMousePosition.Y;

				m_WorldWindow.DrawArgs.WorldCamera.Tilt += Angle.FromDegrees(dY / gY);
				//m_WorldWindow.DrawArgs.WorldCamera.Heading += Angle.FromDegrees(dX);
				m_WorldWindow.DrawArgs.WorldCamera.RotationYawPitchRoll( Angle.Zero, Angle.Zero, Angle.FromDegrees(dX / gX) );

			}

			m_LastMousePosition = new Point(e.X, e.Y);

			base.OnMouseMove (e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			m_LeftMouseDownTime = System.DateTime.MaxValue;

			if(m_IsPanning)
			{
				m_IsPanning = false;	
			}
			else if(m_MouseIsDragging)
			{
				m_MouseIsDragging = false;
				if(m_RenderTargetViewBox)
				{
					m_RenderTargetViewBox = false;
					Point centerPoint = new Point(
						(m_MouseDownStartPosition.X + m_LastMousePosition.X) / 2,
						(m_MouseDownStartPosition.Y + m_LastMousePosition.Y) / 2);

					Vector2 gotoPoint = GetGeoCoordFromScreenPoint(centerPoint);

					double rangeX = Math.Abs(m_MouseDownStartPosition.X - m_LastMousePosition.X);
					double rangeY = Math.Abs(m_MouseDownStartPosition.Y - m_LastMousePosition.Y);

					double degreesPerPixelX = 360.0 / Width;
					double degreesPerPixelY = 180.0 / Height;

					rangeX *= degreesPerPixelX;
					rangeY *= degreesPerPixelY;

					m_WorldWindow.GotoLatLonHeadingViewRange(
						gotoPoint.Y, gotoPoint.X, 0.0,
						(rangeX > rangeY ? rangeX: rangeY)
						);

				}
			}
			else if(e.Button == MouseButtons.Left)
			{
				if(m_OverviewToolbar.Visible)
				{
					m_OverviewToolbar.Visible = false;
				}
				else
				{
					Vector2 gotoPoint = GetGeoCoordFromScreenPoint(new Point(e.X, e.Y));
					m_WorldWindow.GotoLatLon(gotoPoint.Y, gotoPoint.X);
				}

				m_FireMouseUpEvent = true;
			}
			else if(e.Button == MouseButtons.Right)
			{
				m_OverviewToolbar.Visible = !m_OverviewToolbar.Visible;
			}
			m_MouseDownStartPosition = Point.Empty;
			
			base.OnMouseUp (e);
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			m_MouseDownStartPosition = new Point(e.X, e.Y);
			if(e.Button == MouseButtons.Left)
			{
				m_LeftMouseDownTime = System.DateTime.Now;
			}
			base.OnMouseDown (e);
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


		/// <summary>
		/// Returns true if executing in Design mode (inside IDE)
		/// </summary>
		/// <returns></returns>
		private static bool IsInDesignMode()
		{
			return Application.ExecutablePath.ToUpper(CultureInfo.InvariantCulture).EndsWith("DEVENV.EXE");
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// OverviewForm
			// 
			this.Name = "OverviewFormComponent";
			this.Text = "OverviewFormComponent";

		}
		#endregion

		protected override void OnMouseLeave(EventArgs e)
		{
			m_LeftMouseDownTime = System.DateTime.MaxValue;

			base.OnMouseLeave (e);
		}

		private void m_RenderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Invalidate();

			if(m_LeftMouseDownTime != System.DateTime.MaxValue)
			{
				System.DateTime currentTime = System.DateTime.Now;
				if(!m_MouseIsDragging && currentTime.Subtract(m_LeftMouseDownTime) >= m_MousePanHoldInterval)
				{
					m_IsPanning = true;
				}
			}
			else
			{
				m_IsPanning = false;
			}

			if(m_IsPanning)
			{
				if(m_WorldWindow != null)
				{
					Vector2 dest = new Vector2(
						m_LastMousePosition.X,
						m_LastMousePosition.Y);

					Point srcP = this.GetPointFromCoord(
						m_WorldWindow.DrawArgs.WorldCamera.Latitude.Degrees, m_WorldWindow.DrawArgs.WorldCamera.Longitude.Degrees);
					Vector2 src = new Vector2(srcP.X, srcP.Y);

					dest = Vector2.Lerp(src, dest, m_PanSpeed);

					Point dstP = new Point((int)dest.X, (int)dest.Y);

					dest = this.GetGeoCoordFromScreenPoint(dstP);
					
					m_WorldWindow.GotoLatLon(dest.Y, dest.X);
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			m_LastResize = System.DateTime.Now;
			if(m_OverviewToolbar != null)
			{
				m_OverviewToolbar.Size = new Size(Width, m_MinIconSize);
				m_OverviewToolbar.Location = new Point(0, Height / 2 - m_MinIconSize / 2);
			}
			base.OnResize (e);
		}


	}
}
