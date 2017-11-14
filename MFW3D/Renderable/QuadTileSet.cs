using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using MFW3D;
using MFW3D.Camera;
using MFW3D.Configuration;
using MFW3D.Net;
using MFW3D.Terrain;
using MFW3D.VisualControl;
using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Xml;
using System.Globalization;
using Utility;

namespace MFW3D.Renderable
{
	/// <summary>
	/// Main class for image tile rendering.  Uses the Terrain Manager to query height values for 3D
	/// terrain rendering.
	/// Relies on an Update thread to refresh the "tiles" based on lat/lon/view range
	/// </summary>
	public class QuadTileSet : RenderableObject
	{
		#region к╫спЁит╠

		bool m_RenderStruts = true;
		protected string m_ServerLogoFilePath;
		protected Image m_ServerLogoImage;

		protected Hashtable m_topmostTiles = new Hashtable();
		protected double m_north;
		protected double m_south;
		protected double m_west;
		protected double m_east;
		protected DateTime m_earlisettime = DateTime.MinValue;
		protected DateTime m_latesttime = DateTime.MaxValue;
		bool renderFileNames = false;

		protected Texture m_iconTexture;
		protected Sprite sprite;
		protected Rectangle m_spriteSize;
		protected ProgressBar progressBar;

		protected Blend m_sourceBlend = Blend.BlendFactor;
		protected Blend m_destinationBlend = Blend.InvBlendFactor;

		// If this value equals CurrentFrameStartTicks the Z buffer needs to be cleared
		protected static long lastRenderTime;

		//public static int MaxConcurrentDownloads = 3;
		protected double m_layerRadius;
		protected bool m_alwaysRenderBaseTiles;
		protected float m_tileDrawSpread;
		protected float m_tileDrawDistance;
		protected bool m_isDownloadingElevation;
		protected int m_numberRetries;
		protected Hashtable m_downloadRequests = new Hashtable();
		protected int m_maxQueueSize = 400;
		protected bool m_terrainMapped;
		protected ImageStore[] m_imageStores;
		protected Camera.CameraBase m_camera;
		protected GeoSpatialDownloadRequest[] m_activeDownloads = new GeoSpatialDownloadRequest[20];
		protected DateTime[] m_downloadStarted = new DateTime[20];
		protected TimeSpan m_connectionWaitTime = TimeSpan.FromMinutes(2);
        protected TimeSpan m_defaultWaitTime = TimeSpan.FromMinutes(2);
		protected DateTime m_connectionWaitStart;
		protected bool m_isConnectionWaiting;
		protected bool m_enableColorKeying;

		protected Effect m_effect = null;
		protected bool m_effectEnabled = true;
		protected string m_effectPath = null;
		protected string m_effectTechnique = null;
        protected Hashtable m_effectHandles = null;
		static protected EffectPool m_effectPool = new EffectPool();
        protected Hashtable m_failedDownloads = new Hashtable();
        //this value should be >= 1
        private int MaxRetriesPerTile = 1;
        private int MaxFailedTilesForTimeout = 5;

		protected TimeSpan m_cacheExpirationTime = TimeSpan.MaxValue;


		#endregion

		#region public members

		/// <summary>
		/// Texture showing download in progress
		/// </summary>
		public static Texture DownloadInProgressTexture;

		/// <summary>
		/// Texture showing queued download
		/// </summary>
		public static Texture DownloadQueuedTexture;

		/// <summary>
		/// Texture showing terrain download in progress
		/// </summary>
		public static Texture DownloadTerrainTexture;



		public int ColorKey; // default: 100% transparent black = transparent

		/// <summary>
		/// If a color range is to be transparent this specifies the brightest transparent color.
		/// The darkest transparent color is set using ColorKey.
		/// </summary>
		public int ColorKeyMax;

		#endregion

		bool m_renderGrayscale = false;
		float m_grayscaleBrightness = 0.0f;

		public float GrayscaleBrightness
		{
			get { return m_grayscaleBrightness; }
			set { m_grayscaleBrightness = value; }
		}

		public bool RenderGrayscale
		{
			get { return m_renderGrayscale; }
			set { m_renderGrayscale = value; }
		}

		public bool RenderStruts
		{
			get { return m_RenderStruts; }
			set { m_RenderStruts = value; }
		}
		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.QuadTileSet"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parentWorld"></param>
		/// <param name="distanceAboveSurface"></param>
		/// <param name="north"></param>
		/// <param name="south"></param>
		/// <param name="west"></param>
		/// <param name="east"></param>
		/// <param name="earliesttime"></param>
		/// <param name="latesttime"></param>
        /// <param name="terrainMapped"></param>
        /// <param name="imageStores"></param>
		public QuadTileSet(
				string name,
				World parentWorld,
				double distanceAboveSurface,
				double north,
				double south,
				double west,
				double east,
			DateTime earliesttime,
			DateTime latesttime,
				bool terrainMapped,
										ImageStore[] imageStores)
			: base(name, parentWorld)
		{
			float layerRadius = (float)(parentWorld.EquatorialRadius + distanceAboveSurface);
			m_north = north;
			m_south = south;
			m_west = west;
			m_east = east;
			m_earlisettime = earliesttime;
			m_latesttime = latesttime;

			// Layer center position
			Position = MathEngine.SphericalToCartesian(
					(north + south) * 0.5f,
					(west + east) * 0.5f,
					layerRadius);

			m_layerRadius = layerRadius;
			m_tileDrawDistance = 3.5f;
			m_tileDrawSpread = 2.9f;
			m_imageStores = imageStores;
			m_terrainMapped = terrainMapped;

			// Default terrain mapped imagery to terrain mapped priority
			if (terrainMapped)
                RenderPriority = RenderPriority.TerrainMappedImages;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.QuadTileSet"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parentWorld"></param>
		/// <param name="distanceAboveSurface"></param>
		/// <param name="north"></param>
		/// <param name="south"></param>
		/// <param name="west"></param>
		/// <param name="east"></param>
		/// <param name="terrainMapped"></param>
		/// <param name="imageStores"></param>
		public QuadTileSet(
			string name,
			World parentWorld,
			double distanceAboveSurface,
			double north,
			double south,
			double west,
			double east,
			bool terrainMapped,
						ImageStore[] imageStores)
			: base(name, parentWorld)
		{
			float layerRadius = (float)(parentWorld.EquatorialRadius + distanceAboveSurface);
			m_north = north;
			m_south = south;
			m_west = west;
			m_east = east;

			// Layer center position
			Position = MathEngine.SphericalToCartesian(
				(north + south) * 0.5f,
				(west + east) * 0.5f,
				layerRadius);

			m_layerRadius = layerRadius;
			m_tileDrawDistance = 3.5f;
			m_tileDrawSpread = 2.9f;
			m_imageStores = imageStores;
			m_terrainMapped = terrainMapped;

			// Default terrain mapped imagery to terrain mapped priority 
			if (terrainMapped)
				RenderPriority = RenderPriority.TerrainMappedImages;
		}

		#region Public properties

		/// <summary>
		/// If images in cache are older than expration time a refresh
		/// from server will be attempted.
		/// </summary>
		public TimeSpan CacheExpirationTime
		{
			get
			{
				return this.m_cacheExpirationTime;
			}
			set
			{
				this.m_cacheExpirationTime = value;
			}
		}

		/// <summary>
		/// Path to a thumbnail image (e.g. for use as a download indicator).
		/// </summary>
		public virtual string ServerLogoFilePath
		{
			get
			{
				return m_ServerLogoFilePath;
			}
			set
			{
				m_ServerLogoFilePath = value;
			}
		}

		public bool RenderFileNames
		{
			get
			{
				return renderFileNames;
			}
			set
			{
				renderFileNames = value;
			}
		}

		/// <summary>
		/// The image referenced by ServerLogoFilePath.
		/// </summary>
		public virtual Image ServerLogoImage
		{
			get
			{
				if (m_ServerLogoImage == null)
				{
					if (m_ServerLogoFilePath == null)
						return null;
					try
					{
						if (File.Exists(m_ServerLogoFilePath))
							m_ServerLogoImage = ImageHelper.LoadImage(m_ServerLogoFilePath);
					}
					catch { }
				}
				return m_ServerLogoImage;
			}
		}

		/// <summary>
		/// Path to a thumbnail image (or download indicator if none available)
		/// </summary>
		public override Image ThumbnailImage
		{
			get
			{
				if (base.ThumbnailImage != null)
					return base.ThumbnailImage;

				return ServerLogoImage;
			}
		}

		/// <summary>
		/// Path to a thumbnail image (e.g. for use as a download indicator).
		/// </summary>
		public virtual bool HasTransparentRange
		{
			get
			{
				return (ColorKeyMax != 0);
			}
		}

		/// <summary>
		/// Source blend when rendering non-opaque layer
		/// </summary>
		public Blend SourceBlend
		{
			get
			{
				return m_sourceBlend;
			}
			set
			{
				m_sourceBlend = value;
			}
		}

		/// <summary>
		/// Destination blend when rendering non-opaque layer
		/// </summary>
		public Blend DestinationBlend
		{
			get
			{
				return m_destinationBlend;
			}
			set
			{
				m_destinationBlend = value;
			}
		}

		/// <summary>
		/// North bound for this QuadTileSet
		/// </summary>
		public double North
		{
			get
			{
				return m_north;
			}
		}

		/// <summary>
		/// West bound for this QuadTileSet
		/// </summary>
		public double West
		{
			get
			{
				return m_west;
			}
		}

		/// <summary>
		/// South bound for this QuadTileSet
		/// </summary>
		public double South
		{
			get
			{
				return m_south;
			}
		}

		/// <summary>
		/// East bound for this QuadTileSet
		/// </summary>
		public double East
		{
			get
			{
				return m_east;
			}
		}

		/// <summary>
		/// Controls if images are rendered using ColorKey (transparent areas)
		/// </summary>
		public bool EnableColorKeying
		{
			get
			{
				return m_enableColorKeying;
			}
			set
			{
				m_enableColorKeying = value;
			}
		}

		public DateTime ConnectionWaitStart
		{
			get
			{
				return m_connectionWaitStart;
			}
		}

		public bool IsConnectionWaiting
		{
			get
			{
				return m_isConnectionWaiting;
			}
		}

		public double LayerRadius
		{
			get
			{
				return m_layerRadius;
			}
			set
			{
				m_layerRadius = value;
			}
		}

		public bool AlwaysRenderBaseTiles
		{
			get
			{
				return m_alwaysRenderBaseTiles;
			}
			set
			{
				m_alwaysRenderBaseTiles = value;
			}
		}

		public float TileDrawSpread
		{
			get
			{
				return m_tileDrawSpread;
			}
			set
			{
				m_tileDrawSpread = value;
			}
		}

		public float TileDrawDistance
		{
			get
			{
				return m_tileDrawDistance;
			}
			set
			{
				m_tileDrawDistance = value;
			}
		}

		public bool IsDownloadingElevation
		{
			get
			{
				return m_isDownloadingElevation;
			}
			set
			{
				m_isDownloadingElevation = value;
			}
		}

		public int NumberRetries
		{
			get
			{
				return m_numberRetries;
			}
			set
			{
				m_numberRetries = value;
			}
		}

		/// <summary>
		/// Controls rendering (flat or terrain mapped)
		/// </summary>
		public bool TerrainMapped
		{
			get { return m_terrainMapped; }
			set { m_terrainMapped = value; }
		}

		public ImageStore[] ImageStores
		{
			get
			{
				return m_imageStores;
			}
			set
			{
				m_imageStores = value;
			}

		}

		/// <summary>
		/// Tiles in the request for download queue
		/// </summary>
		public Hashtable DownloadRequests
		{
			get
			{
				return m_downloadRequests;
			}
		}

		/// <summary>
		/// The camera controlling the layers update logic
		/// </summary>
		public CameraBase Camera
		{
			get
			{
				return m_camera;
			}
			set
			{
				m_camera = value;
			}
		}

		/// <summary>
		/// Path to the effect used to render this tileset; if null, use fixed function pipeline
		/// </summary>
		public string EffectPath
		{
			get
			{
				return m_effectPath;
			}
			set
			{
				m_effectPath = Path.GetFullPath(value);
				Log.Write(Log.Levels.Debug, string.Format("setting effect to {0}", m_effectPath));
				// can't reload here because we need a valid DX device for that and EffectPath
				// may be set before DX is initialized, so set null and reload in Update()
				m_effect = null;
			}
		}

		/// <summary>
		/// The effect used to render this tileset.
		/// </summary>
		public Effect Effect
		{
			get
			{
				return m_effect;
			}
			set
			{
				m_effect = value;
			}
		}

		public bool EffectEnabled
		{
			get
			{
				return m_effectEnabled;
			}
			set
			{
				m_effectEnabled = value;
				if (!m_effectEnabled)
					m_effect = null;
			}
		}

        public Hashtable EffectParameters
        {
            get
            {
                return m_effectHandles;
            }
        }

		#endregion

		override public void Initialize(DrawArgs drawArgs)
		{
			Camera = DrawArgs.Camera;

			// Initialize download rectangles
			if (DownloadInProgressTexture == null)
				DownloadInProgressTexture = CreateDownloadRectangle(
						DrawArgs.Device, World.Settings.DownloadProgressColor, 0);
			if (DownloadQueuedTexture == null)
				DownloadQueuedTexture = CreateDownloadRectangle(
						DrawArgs.Device, World.Settings.DownloadQueuedColor, 0);
			if (DownloadTerrainTexture == null)
				DownloadTerrainTexture = CreateDownloadRectangle(
						DrawArgs.Device, World.Settings.DownloadTerrainRectangleColor, 0);

			try
			{
				lock (m_topmostTiles.SyncRoot)
				{
					foreach (QuadTile qt in m_topmostTiles.Values)
						qt.Initialize();
				}
			}
			catch
			{
			}
			isInitialized = true;


			if (MetaData.ContainsKey("EffectPath"))
			{
				m_effectPath = MetaData["EffectPath"] as string;
			}
			else
			{
				m_effectPath = null;
			}
            //TODO: Possibly add automatic parsing of effect params
            //via annotations
			m_effect = null;
            if(m_effectHandles == null)
                m_effectHandles = new Hashtable();
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		public override void Update(DrawArgs drawArgs)
		{
            if (System.Threading.Thread.CurrentThread.Name != "WorldWindow.WorkerThreadFunc")
                throw new System.InvalidOperationException("QTS.Update() must be called from WorkerThread!");

			if (!isInitialized)
				Initialize(drawArgs);

			ServiceDownloadQueue();

			if(m_effectEnabled && (m_effectPath != null) && !File.Exists(m_effectPath))
			{
				Log.Write(Log.Levels.Warning, string.Format("Effect {0} not found - disabled", m_effectPath));
				m_effectEnabled = false;
			}

			if (m_effectEnabled && m_effectPath != null && m_effect == null)
			{
				string errs = string.Empty;

                m_effectHandles.Clear();

                try
                {
					Log.Write(Log.Levels.Warning, string.Format("Loading effect from {0}", m_effectPath));
					m_effect = Effect.FromFile(DrawArgs.Device, m_effectPath, null, "", ShaderFlags.None, m_effectPool, out errs);

                    // locate effect handles and store for rendering.
                    m_effectHandles.Add("WorldViewProj", m_effect.GetParameter(null, "WorldViewProj"));
                    m_effectHandles.Add("World", m_effect.GetParameter(null, "World"));
                    m_effectHandles.Add("ViewInverse", m_effect.GetParameter(null, "ViewInverse"));
                    for(int i=0; i<8;i++)
                    {
					    string name = string.Format("Tex{0}", i);
					    m_effectHandles.Add(name, m_effect.GetParameter(null, name));
                    }
                    m_effectHandles.Add("Brightness", m_effect.GetParameter(null, "Brightness"));
                    m_effectHandles.Add("Opacity", m_effect.GetParameter(null, "Opacity"));
                    m_effectHandles.Add("TileLevel", m_effect.GetParameter(null, "TileLevel"));
                    m_effectHandles.Add("LightDirection", m_effect.GetParameter(null, "LightDirection"));
                    m_effectHandles.Add("LocalOrigin", m_effect.GetParameter(null, "LocalOrigin"));
                    m_effectHandles.Add("LayerRadius", m_effect.GetParameter(null, "LayerRadius"));
                    m_effectHandles.Add("LocalFrameOrigin", m_effect.GetParameter(null, "LocalFrameOrigin"));
                    m_effectHandles.Add("LocalFrameXAxis", m_effect.GetParameter(null, "LocalFrameXAxis"));
                    m_effectHandles.Add("LocalFrameYAxis", m_effect.GetParameter(null, "LocalFrameYAxis"));
                    m_effectHandles.Add("LocalFrameZAxis", m_effect.GetParameter(null, "LocalFrameZAxis"));
                    //TODO: Add additional named parameters
                    //TODO: Expose parameters via GUI
                }
                catch (Exception ex)
                {
                    Log.Write(Log.Levels.Error, "Effect load caused exception:" + ex.ToString());
					Log.Write(Log.Levels.Warning, "Effect has been disabled.");
					m_effectEnabled = false;
                }

				if (errs != null && errs != string.Empty)
				{
					Log.Write(Log.Levels.Warning, "Could not load effect " + m_effectPath + ": " + errs);
					Log.Write(Log.Levels.Warning, "Effect has been disabled.");
					m_effectEnabled = false;
					m_effect = null;
				}
			}

			if (ImageStores[0].LevelZeroTileSizeDegrees < 180)
			{
				// Check for layer outside view
				double vrd = DrawArgs.Camera.ViewRange.Degrees;
				double latitudeMax = DrawArgs.Camera.Latitude.Degrees + vrd;
				double latitudeMin = DrawArgs.Camera.Latitude.Degrees - vrd;
				double longitudeMax = DrawArgs.Camera.Longitude.Degrees + vrd;
				double longitudeMin = DrawArgs.Camera.Longitude.Degrees - vrd;
				if (latitudeMax < m_south || latitudeMin > m_north || longitudeMax < m_west || longitudeMin > m_east)
					return;
			}

			if (DrawArgs.Camera.ViewRange * 0.5f >
					Angle.FromDegrees(TileDrawDistance * ImageStores[0].LevelZeroTileSizeDegrees))
			{
				lock (m_topmostTiles.SyncRoot)
				{
					foreach (QuadTile qt in m_topmostTiles.Values)
						qt.Dispose();
					m_topmostTiles.Clear();
					ClearDownloadRequests();
				}

				return;
			}

			RemoveInvisibleTiles(DrawArgs.Camera);
			try
			{
				int middleRow = MathEngine.GetRowFromLatitude(DrawArgs.Camera.Latitude, ImageStores[0].LevelZeroTileSizeDegrees);
				int middleCol = MathEngine.GetColFromLongitude(DrawArgs.Camera.Longitude, ImageStores[0].LevelZeroTileSizeDegrees);

				double middleSouth = -90.0f + middleRow * ImageStores[0].LevelZeroTileSizeDegrees;
				double middleNorth = -90.0f + middleRow * ImageStores[0].LevelZeroTileSizeDegrees + ImageStores[0].LevelZeroTileSizeDegrees;
				double middleWest = -180.0f + middleCol * ImageStores[0].LevelZeroTileSizeDegrees;
				double middleEast = -180.0f + middleCol * ImageStores[0].LevelZeroTileSizeDegrees + ImageStores[0].LevelZeroTileSizeDegrees;

				double middleCenterLat = 0.5f * (middleNorth + middleSouth);
				double middleCenterLon = 0.5f * (middleWest + middleEast);

				int tileSpread = 4;
				for (int i = 0; i < tileSpread; i++)
				{
					for (double j = middleCenterLat - i * ImageStores[0].LevelZeroTileSizeDegrees; j <= middleCenterLat + i * ImageStores[0].LevelZeroTileSizeDegrees; j += ImageStores[0].LevelZeroTileSizeDegrees)
					{
						for (double k = middleCenterLon - i * ImageStores[0].LevelZeroTileSizeDegrees; k <= middleCenterLon + i * ImageStores[0].LevelZeroTileSizeDegrees; k += ImageStores[0].LevelZeroTileSizeDegrees)
						{
							int curRow = MathEngine.GetRowFromLatitude(Angle.FromDegrees(j), ImageStores[0].LevelZeroTileSizeDegrees);
							int curCol = MathEngine.GetColFromLongitude(Angle.FromDegrees(k), ImageStores[0].LevelZeroTileSizeDegrees);
							long key = ((long)curRow << 32) + curCol;

							QuadTile qt = (QuadTile)m_topmostTiles[key];
							if (qt != null)
							{
								qt.Update(drawArgs);
								continue;
							}

							// Check for tile outside layer boundaries
							double west = -180.0f + curCol * ImageStores[0].LevelZeroTileSizeDegrees;
							if (west > m_east)
								continue;

							double east = west + ImageStores[0].LevelZeroTileSizeDegrees;
							if (east < m_west)
								continue;

							double south = -90.0f + curRow * ImageStores[0].LevelZeroTileSizeDegrees;
							if (south > m_north)
								continue;

							double north = south + ImageStores[0].LevelZeroTileSizeDegrees;
							if (north < m_south)
								continue;

							qt = new QuadTile(south, north, west, east, 0, this);
							if (DrawArgs.Camera.ViewFrustum.Intersects(qt.BoundingBox))
							{
								lock (m_topmostTiles.SyncRoot)
									m_topmostTiles.Add(key, qt);
								qt.Update(drawArgs);
							}
						}
					}
				}
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
			catch (Exception caught)
			{
				Log.Write(caught);
			}
		}

		protected void RemoveInvisibleTiles(CameraBase camera)
		{
			ArrayList deletionList = new ArrayList();

			lock (m_topmostTiles.SyncRoot)
			{
				foreach (long key in m_topmostTiles.Keys)
				{
					QuadTile qt = (QuadTile)m_topmostTiles[key];
					if (!camera.ViewFrustum.Intersects(qt.BoundingBox))
						deletionList.Add(key);
				}

				foreach (long deleteThis in deletionList)
				{
					QuadTile qt = (QuadTile)m_topmostTiles[deleteThis];
					if (qt != null)
					{
						m_topmostTiles.Remove(deleteThis);
						qt.Dispose();
					}
				}
			}
		}

		public override void Render(DrawArgs drawArgs)
		{
			try
			{
				lock (m_topmostTiles.SyncRoot)
				{
					if (m_topmostTiles.Count <= 0)
					{
						return;
					}

					Device device = DrawArgs.Device;

					// Temporary fix: Clear Z buffer between rendering
					// terrain mapped layers to avoid Z buffer fighting
					//if (lastRenderTime == DrawArgs.CurrentFrameStartTicks)
					device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
					device.RenderState.ZBufferEnable = true;
					lastRenderTime = DrawArgs.CurrentFrameStartTicks;

					//							  if (m_renderPriority < RenderPriority.TerrainMappedImages)
					//									  // No Z buffering needed for "flat" layers
					//									  device.RenderState.ZBufferEnable = false;


					/*	  if (m_opacity < 255 && device.DeviceCaps.DestinationBlendCaps.SupportsBlendFactor)
							{
									// Blend
									device.RenderState.AlphaBlendEnable = true;
									device.RenderState.SourceBlend = m_sourceBlend;
									device.RenderState.DestinationBlend = m_destinationBlend;
									// Set Red, Green and Blue = opacity
									device.RenderState.BlendFactorColor = (m_opacity << 16) | (m_opacity << 8) | m_opacity;
							}
							else if (EnableColorKeying && device.DeviceCaps.TextureCaps.SupportsAlpha)
							{
									device.RenderState.AlphaBlendEnable = true;
									device.RenderState.SourceBlend = Blend.SourceAlpha;
									device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
							}
		*/
					if (!World.Settings.EnableSunShading)
					{
						// Set the render states for rendering of quad tiles.
						// Any quad tile rendering code that adjusts the state should restore it to below values afterwards.
						device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
						device.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.SelectArg1);
						device.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
						device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
						device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.SelectArg1);

						// Be prepared for multi-texturing
						device.SetTextureStageState(1, TextureStageStates.ColorArgument2, (int)TextureArgument.Current);
						device.SetTextureStageState(1, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
						device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, 0);
					}
					device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
					foreach (QuadTile qt in m_topmostTiles.Values)
						qt.Render(drawArgs);

					// Restore device states
					device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, 1);

					if (RenderPriority < RenderPriority.TerrainMappedImages)
						device.RenderState.ZBufferEnable = true;
					/*
														   if (m_opacity < 255 || EnableColorKeying)
														   {
																   // Restore alpha blend state
																   device.RenderState.SourceBlend = Blend.SourceAlpha;
																   device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
														   }*/
				}
			}
			catch
			{
			}
			finally
			{
				if (IsConnectionWaiting)
				{
					if (DateTime.Now.Subtract(TimeSpan.FromSeconds(20)) < ConnectionWaitStart)
					{
                        //the wait time may vary, display the correct wait time in minutes and seconds
                        int minutesWaiting = m_connectionWaitTime.Minutes;
                        String timePart;
                        if (minutesWaiting > 0)
                        {
                            timePart = m_connectionWaitTime.Minutes + " minutes "+m_connectionWaitTime.Seconds+" seconds";
                        }
                        else
                        {
                            timePart = m_connectionWaitTime.Seconds + " seconds";
                        }
                        //this message is rendered in upper right corner
						string s = "Problem connecting to server... Trying again in "+timePart+".\n";
						drawArgs.UpperLeftCornerText += s;
					}
				}

				int i = 0;

				foreach (GeoSpatialDownloadRequest request in m_activeDownloads)
				{
                    
					if (request != null && !request.IsComplete && i < 10)
					{

						RenderDownloadProgress(drawArgs, request, i++);
						// Only render the first
						//break;
					}
                    
				}
                if(i!=0) RenderDownloadProgressSprite(drawArgs, i);
               
			}
		}

        public override XmlNode ToXml(XmlDocument worldDoc)
        {
            XmlNode quadTileNode = worldDoc.CreateElement("QuadTileSet");

            ConfigurationSaver.getRenderableObjectProperties(this, quadTileNode);

            XmlNode dASNode = worldDoc.CreateElement("DistanceAboveSurface");
            double dAS = LayerRadius - World.EquatorialRadius;
            dASNode.AppendChild(worldDoc.CreateTextNode(dAS.ToString(CultureInfo.InvariantCulture)));

            XmlNode terrainNode = worldDoc.CreateElement("TerrainMapped");
            terrainNode.AppendChild(worldDoc.CreateTextNode(TerrainMapped.ToString()));

            XmlNode renderstrutNode = worldDoc.CreateElement("RenderStruts");
            renderstrutNode.AppendChild(worldDoc.CreateTextNode(RenderStruts.ToString()));

            XmlNode effectNode = worldDoc.CreateElement("Effect");
            effectNode.AppendChild(worldDoc.CreateTextNode(EffectPath));

            XmlNode transminNode = worldDoc.CreateElement("TransparentMinValue");
            transminNode.AppendChild(worldDoc.CreateTextNode(ColorKey.ToString(CultureInfo.InvariantCulture)));

            XmlNode transmaxNode = worldDoc.CreateElement("TransparentMaxValue");
            transmaxNode.AppendChild(worldDoc.CreateTextNode(ColorKeyMax.ToString(CultureInfo.InvariantCulture)));

            XmlNode bboxNode = worldDoc.CreateElement("BoundingBox");

            XmlNode north = worldDoc.CreateElement("North");
            XmlNode valueN = worldDoc.CreateElement("Value");
            valueN.AppendChild(worldDoc.CreateTextNode(North.ToString(CultureInfo.InvariantCulture)));
            north.AppendChild(valueN);
            XmlNode south = worldDoc.CreateElement("South");
            XmlNode valueS = worldDoc.CreateElement("Value");
            valueS.AppendChild(worldDoc.CreateTextNode(South.ToString(CultureInfo.InvariantCulture)));
            south.AppendChild(valueS);
            XmlNode east = worldDoc.CreateElement("East");
            XmlNode valueE = worldDoc.CreateElement("Value");
            valueE.AppendChild(worldDoc.CreateTextNode(East.ToString(CultureInfo.InvariantCulture)));
            east.AppendChild(valueE);
            XmlNode west = worldDoc.CreateElement("West");
            XmlNode valueW = worldDoc.CreateElement("Value");
            valueW.AppendChild(worldDoc.CreateTextNode(West.ToString(CultureInfo.InvariantCulture)));
            west.AppendChild(valueW);

            bboxNode.AppendChild(north);
            bboxNode.AppendChild(south);
            bboxNode.AppendChild(east);
            bboxNode.AppendChild(west);

            quadTileNode.AppendChild(dASNode);
            quadTileNode.AppendChild(terrainNode);
            quadTileNode.AppendChild(renderstrutNode);
            quadTileNode.AppendChild(effectNode);
            quadTileNode.AppendChild(transminNode);
            quadTileNode.AppendChild(transmaxNode);
            quadTileNode.AppendChild(bboxNode);

            foreach (ImageStore istore in ImageStores)
                quadTileNode.AppendChild(istore.ToXml(worldDoc));

            //TODO: Cache expiration times

            return quadTileNode;
        }

		public void RenderDownloadProgress(DrawArgs drawArgs, GeoSpatialDownloadRequest request, int offset)
		{
            //int halfIconHeight = 24;
			int halfIconWidth = 24;

			Vector3 projectedPoint = new Vector3(DrawArgs.ParentControl.Width - halfIconWidth - 10, DrawArgs.ParentControl.Height - 34 - 4 * offset, 0.5f);

			// Render progress bar
            if (progressBar == null)
                progressBar = new ProgressBar(40, 4);
            
            progressBar.Draw(drawArgs, projectedPoint.X, projectedPoint.Y + 24, request.ProgressPercent, World.Settings.DownloadProgressColor.ToArgb());
            DrawArgs.Device.RenderState.ZBufferEnable = true;

        }
        public void RenderDownloadProgressSprite(DrawArgs drawArgs,int offset)
        {
            int halfIconHeight = 24;
            int halfIconWidth = 24;

            Vector3 projectedPoint = new Vector3(DrawArgs.ParentControl.Width - halfIconWidth - 10, DrawArgs.ParentControl.Height - 34 - 4 * offset, 0.5f);
			// Render server logo
			if (ServerLogoFilePath == null)
				return;
            
            
			if (m_iconTexture == null)
				m_iconTexture = ImageHelper.LoadIconTexture(ServerLogoFilePath);

            if (sprite == null)
            {
                using (Surface s = m_iconTexture.GetSurfaceLevel(0))
                {
                    SurfaceDescription desc = s.Description;
                    m_spriteSize = new Rectangle(0, 0, desc.Width, desc.Height);
                }

                this.sprite = new Sprite(DrawArgs.Device);
            }
            

			float scaleWidth = (float)2.0f * halfIconWidth / m_spriteSize.Width;
			float scaleHeight = (float)2.0f * halfIconHeight / m_spriteSize.Height;

			this.sprite.Begin(SpriteFlags.AlphaBlend);
			this.sprite.Transform = Matrix.Transformation2D(new Vector2(0.0f, 0.0f), 0.0f, new Vector2(scaleWidth, scaleHeight),
					new Vector2(0, 0),
					0.0f, new Vector2(projectedPoint.X, projectedPoint.Y));

			this.sprite.Draw(m_iconTexture, m_spriteSize,
					new Vector3(1.32f * 48, 1.32f * 48, 0), new Vector3(0, 0, 0),
					World.Settings.DownloadLogoColor);
			this.sprite.End();
		}

		public override void Dispose()
		{
			isInitialized = false;

			// flush downloads
            lock (m_downloadRequests.SyncRoot)
            {
                m_downloadRequests.Clear();

                for (int i = 0; i < World.Settings.MaxSimultaneousDownloads; i++)
                {
                    if (m_activeDownloads[i] != null)
                    {
                        m_activeDownloads[i].Dispose();
                        m_activeDownloads[i] = null;
                    }
                }
            }

            lock (m_failedDownloads.SyncRoot)
            {
                //clear this hashtable                
                m_failedDownloads.Clear();
            }

			foreach (QuadTile qt in m_topmostTiles.Values)
				qt.Dispose();

			if (m_iconTexture != null)
			{
				m_iconTexture.Dispose();
				m_iconTexture = null;
			}

			if (this.sprite != null)
			{
				this.sprite.Dispose();
				this.sprite = null;
			}
		}

		public virtual void ResetCacheForCurrentView(MFW3D.Camera.CameraBase camera)
		{
			//					  if (!ImageStore.IsDownloadableLayer)
			//							  return;

			ArrayList deletionList = new ArrayList();
			//reset "root" tiles that intersect current view
			lock (m_topmostTiles.SyncRoot)
			{
				foreach (long key in m_topmostTiles.Keys)
				{
					QuadTile qt = (QuadTile)m_topmostTiles[key];
					if (camera.ViewFrustum.Intersects(qt.BoundingBox))
					{
						qt.ResetCache();
						deletionList.Add(key);
					}
				}

				foreach (long deletionKey in deletionList)
					m_topmostTiles.Remove(deletionKey);
			}

            //reset the numberRetries and timeout, also reset failedDownloads Hashtable
            m_isConnectionWaiting = false;
            //m_connectionWaitStart = System.DateTime.MinValue;
            m_failedDownloads.Clear();
            m_numberRetries = 0;
            Log.Write(Log.Levels.Verbose, "QTS", " ResetCacheForCurrentView: (" + m_isConnectionWaiting + "," + m_connectionWaitStart + "," +m_failedDownloads.Count +"," +m_numberRetries);
                        
		}

		public void ClearDownloadRequests()
		{
			lock (m_downloadRequests.SyncRoot)
			{
				m_downloadRequests.Clear();
			}
		}

		public virtual void AddToDownloadQueue(CameraBase camera, GeoSpatialDownloadRequest newRequest)
		{
			string key = newRequest.QuadTile.ToString();
            newRequest.QuadTile.WaitingForDownload = true;
			lock (m_downloadRequests.SyncRoot)
			{
                if (m_downloadRequests.Contains(key))
					return;

                //go through download requests, remove any requests that have failed before
                if (IsTileFailed(newRequest.QuadTile.Level,newRequest.QuadTile.Col,newRequest.QuadTile.Row))
                {
                    
                    //increment number of retries overall for this tile (more than 5 failed downloads and anti-hammer goes into effect for wait time)
                    Log.Write(Log.Levels.Verbose, "QTS", "AddToDownloadQueue: Requested tile will not download again.");
                    newRequest.QuadTile.WaitingForDownload = false;
                    
                }
                

                if(newRequest.QuadTile.WaitingForDownload == true)
                {

                    Log.Write(Log.Levels.Verbose, "QTS", "AddToDownloadQueue: " + newRequest.QuadTile.ToString());

				    m_downloadRequests.Add(key, newRequest);

				    if (m_downloadRequests.Count >= m_maxQueueSize)
				    {
					    //remove spatially farthest request
					    GeoSpatialDownloadRequest farthestRequest = null;
					    Angle curDistance = Angle.Zero;
					    Angle farthestDistance = Angle.Zero;
					    foreach (GeoSpatialDownloadRequest curRequest in m_downloadRequests.Values)
					    {
						    curDistance = MathEngine.SphericalDistance(
										    curRequest.QuadTile.CenterLatitude,
										    curRequest.QuadTile.CenterLongitude,
										    camera.Latitude,
										    camera.Longitude);

						    if (curDistance > farthestDistance)
						    {
							    farthestRequest = curRequest;
							    farthestDistance = curDistance;
						    }
					    }

					    farthestRequest.Dispose();
					    farthestRequest.QuadTile.DownloadRequests.Remove(farthestRequest);
					    m_downloadRequests.Remove(farthestRequest.QuadTile);
				    }
			    }
            }

			ServiceDownloadQueue();
		}

		/// <summary>
		/// Removes a request from the download queue.
		/// </summary>
		public virtual void RemoveFromDownloadQueue(GeoSpatialDownloadRequest removeRequest)
		{
            Log.Write(Log.Levels.Verbose, "QTS", "RemoveFromDownloadQueue: " + removeRequest.QuadTile.ToString());

            lock (m_downloadRequests.SyncRoot)
			{
				string key = removeRequest.QuadTile.ToString();
				GeoSpatialDownloadRequest request = (GeoSpatialDownloadRequest)m_downloadRequests[key];
				if (request != null)
				{
					m_downloadRequests.Remove(key);
					request.QuadTile.DownloadRequests.Remove(request);
				}
			}
		}

		/// <summary>
		/// Starts downloads when there are threads available
		/// </summary>
		public virtual void ServiceDownloadQueue()
		{
			if (m_downloadRequests.Count > 0)
                Log.Write(Log.Levels.Debug, "QTS", "ServiceDownloadQueue: " + m_downloadRequests.Count + " requests waiting");

			lock (m_downloadRequests.SyncRoot)
			{
				for (int i = 0; i < World.Settings.MaxSimultaneousDownloads; i++)
				{
					if (m_activeDownloads[i] == null)
						continue;

					if (!m_activeDownloads[i].IsComplete)
						continue;

                    Log.Write(Log.Levels.Debug, "QTS", "ServiceDownloadQueue: removing finished" + m_activeDownloads[i].QuadTile.ToString());

                    // remove from request queue
                    m_downloadRequests.Remove(m_activeDownloads[i]);

					m_activeDownloads[i].Cancel();
					m_activeDownloads[i].Dispose();
					m_activeDownloads[i] = null;
				}
                
                
                //If there have been 'MaxFailedTilesForTimeout' or currently in a Timeout
                if (NumberRetries >= MaxFailedTilesForTimeout || m_isConnectionWaiting)
				{
                    
                    // Anti hammer in effect
					if (!m_isConnectionWaiting)
					{
                        Log.Write(Log.Levels.Debug, "QTS", "ServiceDownloadQueue: Anti-Hammer[" + NumberRetries + "," + m_isConnectionWaiting + "," + m_connectionWaitStart.ToShortTimeString() + "] ");
                        //use default timeout (2 minutes)
                        setTimeoutAndWait(m_defaultWaitTime);
					}

					//after waiting for wait time clear the number of retries
                    if (DateTime.Now.Subtract(m_connectionWaitTime) > m_connectionWaitStart)
					{
						NumberRetries = 0;
						m_isConnectionWaiting = false;
					}
                    ClearDownloadRequests();
					return;
				}

				// Queue new downloads
				for (int i = 0; i < World.Settings.MaxSimultaneousDownloads; i++)
				{
					if (m_activeDownloads[i] != null)
						continue;

					if (m_downloadRequests.Count <= 0)
						continue;

					m_activeDownloads[i] = GetClosestDownloadRequest();
                    if (m_activeDownloads[i] != null)
                    {
                        Log.Write(Log.Levels.Debug, "GSDR", "Activated download for " + m_activeDownloads[i].ToString() + " in slot " + i);
                        m_downloadStarted[i] = DateTime.Now;
                        m_activeDownloads[i].StartDownload();
                        
                    }
                    else
                    {
                        
                        m_downloadRequests.Clear();
                        break;
                    }
				}
			}
		}

		/// <summary>
		/// Finds the "best" tile from queue
		/// </summary>
		public virtual GeoSpatialDownloadRequest GetClosestDownloadRequest()
		{
			GeoSpatialDownloadRequest closestRequest = null;
			float largestArea = float.MinValue;
            //FIX -> Preliminar solution. request the same tile several times
            bool isactive = false;
			lock (m_downloadRequests.SyncRoot)
			{
				foreach (GeoSpatialDownloadRequest curRequest in m_downloadRequests.Values)
				{
                    // make sure it's not in the active list

                    foreach (GeoSpatialDownloadRequest activeRequest in m_activeDownloads)
                    {
                        if (activeRequest == curRequest)
                        {
                            isactive = true;
                            break;
                        }
                        if (activeRequest != null && curRequest != null && activeRequest.LocalFilePath == curRequest.LocalFilePath)
                        {
                            isactive = true;
                            break;
                        }
                    }
                    if (isactive == true) continue;
                    
                    // FIXME: this check is essentially obsoleted by the previous loop
					if (curRequest.IsDownloading)
						continue;

					QuadTile qt = curRequest.QuadTile;
                    if (!m_camera.ViewFrustum.Intersects(qt.BoundingBox))
                    {
                        string key = curRequest.QuadTile.ToString();
                        m_downloadRequests.Remove(key);
                        break;
                    }

					float screenArea = qt.BoundingBox.CalcRelativeScreenArea(m_camera);
					if (screenArea > largestArea)
					{
						largestArea = screenArea;
						closestRequest = curRequest;
					}
				}
			}

			return closestRequest;
		}

		/// <summary>
		/// Creates a tile download indication texture
		/// </summary>
		static protected Texture CreateDownloadRectangle(Device device, Color color, int padding)
		{
			int mid = 128;
			using (Bitmap i = new Bitmap(2 * mid, 2 * mid))
			using (Graphics g = Graphics.FromImage(i))
			using (Pen pen = new Pen(color))
			{
				int width = mid - 1 - 2 * padding;
				g.DrawRectangle(pen, padding, padding, width, width);
				g.DrawRectangle(pen, mid + padding, padding, width, width);
				g.DrawRectangle(pen, padding, mid + padding, width, width);
				g.DrawRectangle(pen, mid + padding, mid + padding, width, width);

				Texture texture = new Texture(device, i, Usage.None, Pool.Managed);
				return texture;
			}
		}

        /// <summary>
        /// When a DownloadRequest fails when a server returns a 404 error this will record that failure for the QuadTile
        /// If the QuadTile fails to be downloaded after 'MaxRetriesPerTile' retries the QuadTileSet will not download it again to prevent
        /// hammering the server.
        /// </summary>
        /// <param name="downloadRequest"></param>
        public void RecordFailedRequest(GeoSpatialDownloadRequest downloadRequest)
        {
            //use the QuadTile's String as a key, assuming this would be unique
            String quadTileKey = downloadRequest.QuadTile.ToString();
            QuadTile qt = downloadRequest.QuadTile;
            
            int numberFailures = 1;
            if (m_failedDownloads.ContainsKey(quadTileKey))
            {
                //already has failed once, increment the number of failures
                numberFailures = (int) m_failedDownloads[quadTileKey];
                numberFailures++;
                m_failedDownloads[quadTileKey] =  numberFailures;
                
            }else{
                //first time failed, add to the hashTable
                m_failedDownloads.Add(quadTileKey, numberFailures);
            }
        }

        /// <summary>
        /// Allows for setting the Timeout for this QuadTileSet for a specified length of time
        /// where downloads will be put on hold.
        /// </summary>
        /// <param name="waitTime">How long to set wait</param>
        public void setTimeoutAndWait(TimeSpan waitTime)
        {
            m_connectionWaitStart = DateTime.Now;
            m_isConnectionWaiting = true;
            m_connectionWaitTime = waitTime;
        }

        /// <summary>
        /// Checks if the requested download has been flagged as a failed download
        /// </summary>
        /// <param name="level"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean IsTileFailed(int level, int x, int y)
        {
            Boolean isTileFailed = false;
            //construct key
            String key = String.Format("QuadTile:Set={0} Level={1} X={2} Y={3}",
                this.Name, level, x, y);
            //TODO calculate tiles below failed tile as well. ie need to look up at lower levels for failures
            if(m_failedDownloads.ContainsKey(key))
            {
                isTileFailed = true;
            }
            return isTileFailed;
        }

	}

    
}
