using System;
using System.IO;
using System.Windows.Forms;

using MFW3D;
using MFW3D.Net;
using MFW3D.Renderable;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Utility;

namespace MFW3D
{
	/// <summary>
	/// Summary description for DynamicCloudLayer.
	/// </summary>
	public class DynamicCloudLayer : MFW3D.Renderable.RenderableObject
	{
		string m_remoteUrl;
		string m_imageDirectoryPath;
        /// <summary>
        /// These two values determine cloud altitude and how '3D' they look
        /// </summary>
		double m_cloudBaseAltitude = 35000;
		double m_displacementMultiplier = 150;
		int m_samples = 200;
		System.Timers.Timer m_animationTimer = null;
		int m_frameIntervalMs = 800;
		int m_lastFrameHoldTimeMs = 3000;
		bool m_enableHdrLighting = false;
		System.Collections.ArrayList m_frames = new System.Collections.ArrayList();

		float m_exposureLevel = 1.0f;
        bool m_playing = true;

        /// <summary>
        /// Toggles play/pause for the layer
        /// </summary>
        public bool Playing
        {
            get { return m_playing; }
            set { m_playing = value; }
        }

        /// <summary>
        /// Gets/sets exposure level
        /// </summary>
		public float ExposureLevel
		{
			get{ return m_exposureLevel; }
			set{ m_exposureLevel = value; }
		}

        /// <summary>
        /// Turns Hdr Lighting on/off
        /// </summary>
		public bool EnableHdrLighting
		{
			get{ return m_enableHdrLighting; }
			set
			{
				m_enableHdrLighting = value;
				if(!value && m_effect != null)
				{
					DisposeHdrResources();
				}
			}
		}

        /// <summary>
        /// Creates Dynamic Cloud layer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentWorld"></param>
        /// <param name="remoteUrl"></param>
        /// <param name="imageDirectoryPath"></param>
		public DynamicCloudLayer(
			string name,
			World parentWorld,
			string remoteUrl,
			string imageDirectoryPath) : base(name, parentWorld)
		{
			m_remoteUrl = remoteUrl;
			m_imageDirectoryPath = imageDirectoryPath;
		}

		System.Threading.Thread m_updateThread = null;

        /// <summary>
        /// Builds context menu for renderable
        /// </summary>
        /// <param name="menu"></param>
        public override void BuildContextMenu(ContextMenu menu)
        {
            MenuItem pause = new MenuItem("Pause");
            pause.Click += new EventHandler(pause_Click);
            pause.Checked = !m_playing;
            menu.MenuItems.Add(pause);

            MenuItem seperator = new MenuItem("-");
            menu.MenuItems.Add(seperator);

            base.BuildContextMenu(menu);
        }

        void pause_Click(object sender, EventArgs e)
        {
            m_playing = !m_playing;
            (sender as MenuItem).Checked = !m_playing;
        }


        /// <summary>
        /// Initializes renderable
        /// </summary>
        /// <param name="drawArgs"></param>
		public override void Initialize(DrawArgs drawArgs)
		{
			if(m_animationTimer == null)
			{
				m_animationTimer = new System.Timers.Timer(m_frameIntervalMs);
				m_animationTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_animationTimer_Elapsed);
				m_animationTimer.Start();
			}
		
			m_updateThread = new System.Threading.Thread(new System.Threading.ThreadStart(UpdateImages));
			m_updateThread.IsBackground = true;
			m_updateThread.Start();

			float left		= -1.0f;
			float right		= 1.0f;
			float top		= 1.0f;
			float bottom	= -1.0f;

			m_transformedVertices[0].X = left;
			m_transformedVertices[0].Y = bottom;
			m_transformedVertices[0].Tu = 0;
			m_transformedVertices[0].Tv = 1;

			m_transformedVertices[1].X = left;
			m_transformedVertices[1].Y = top;
			m_transformedVertices[1].Tu = 0;
			m_transformedVertices[1].Tv = 0;

			m_transformedVertices[2].X = right;
			m_transformedVertices[2].Y = bottom;
			m_transformedVertices[2].Tu = 1;
			m_transformedVertices[2].Tv = 1;

			m_transformedVertices[3].X = right;
			m_transformedVertices[3].Y = top;
			m_transformedVertices[3].Tu = 1;
			m_transformedVertices[3].Tv = 0;

			if(drawArgs.device.DeviceCaps.VertexShaderVersion.Major > 1 &&
				drawArgs.device.DeviceCaps.PixelShaderVersion.Major > 1)
			{
				m_enableHdrLighting = true;
			}
	
			isInitialized = true;
		}

		private void UpdateImages()
		{
			try
			{
				System.Collections.ArrayList remoteImageList = new System.Collections.ArrayList();

				try
                {
                    // Offline check
                    if (World.Settings.WorkOffline)
                        throw new Exception("Offline mode active.");

					WebDownload download = new WebDownload(m_remoteUrl);
					FileInfo currentIndexFile = new FileInfo(m_imageDirectoryPath + "\\current.txt");
					if(!currentIndexFile.Directory.Exists)
						currentIndexFile.Directory.Create();

					download.DownloadFile(currentIndexFile.FullName, DownloadType.Unspecified);
					download.Dispose();

					using(StreamReader reader = currentIndexFile.OpenText())
					{
						string data = reader.ReadToEnd();

						string[] parts = data.Split('"');

						for(int i = 0; i < parts.Length; i++)
						{
							if(parts[i].StartsWith("/GlobalCloudsArchive/PastDay/"))
							{
								remoteImageList.Add(parts[i].Replace("/GlobalCloudsArchive/PastDay/", ""));
							}
						}
					}
				}
				catch(Exception ex)
				{
					Log.Write(ex);
				}

				System.Collections.ArrayList imageDisplayList = new System.Collections.ArrayList();

				DirectoryInfo cloudImageDirectory = new DirectoryInfo(m_imageDirectoryPath);

				FileInfo[] archiveImageFiles = new FileInfo[0];

				if(cloudImageDirectory.Exists)
				{
					archiveImageFiles = cloudImageDirectory.GetFiles("*.png");
				}

				for(int i = remoteImageList.Count - 1; i >= 0; i--)
				{
					string currentRemoteFile = (string)remoteImageList[i];

					bool found = false;
					for(int j = 0; j < archiveImageFiles.Length; j++)
					{
						if(archiveImageFiles[j].Name == currentRemoteFile.Replace(".jpg", ".png"))
							found = true;
					}

					if(!found)
                    {
                        // Offline check
                        if (World.Settings.WorkOffline)
                            throw new Exception("Offline mode active.");

						FileInfo jpgFile = new FileInfo(cloudImageDirectory.FullName + "\\" + currentRemoteFile);
						WebDownload download = new WebDownload(
							m_remoteUrl + "/" + currentRemoteFile);

						if(!jpgFile.Directory.Exists)
							jpgFile.Directory.Create();

						download.DownloadFile(jpgFile.FullName, DownloadType.Unspecified);

						ImageHelper.CreateAlphaPngFromBrightness(jpgFile.FullName, jpgFile.FullName.Replace(".jpg", ".png"));
					}
				}

				//reload the list of downloaded images
				if(cloudImageDirectory.Exists)
				{
					archiveImageFiles = cloudImageDirectory.GetFiles("*.png");
				}

				for(int i = archiveImageFiles.Length - 1; i >= 0; i--)
				{
					imageDisplayList.Insert(0, archiveImageFiles[i].FullName);
				}

				double north = 90, south = -90, west = -180, east = 180;

				for(int i = 0; i < imageDisplayList.Count; i++)
				{
					string currentImageFile = (string)imageDisplayList[i];
					
					string ddsFilePath = currentImageFile.Replace(".png", ".dds");
					if(!File.Exists(ddsFilePath))
					{
						ImageHelper.ConvertToDxt3(currentImageFile, ddsFilePath, false);
					}
					DynamicCloudFrame frame = new DynamicCloudFrame();
					frame.ImageFile = currentImageFile;
					frame.Texture = ImageHelper.LoadTexture(ddsFilePath);
					CreateMesh(north, south, west, east, m_samples, currentImageFile, ref frame.Vertices, ref frame.Indices);

					m_frames.Add(frame);
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		private void CreateMesh(
			double north,
			double south,
			double west,
			double east,
			int samples,
			string displacementFile,
			ref CustomVertex.PositionTextured[] vertices,
			ref short[] indices)
		{
			int upperBound = samples - 1;
			float scaleFactor = (float)1/upperBound;
			double latrange = Math.Abs(north - south);
			double lonrange;
			if(west < east)
				lonrange = east - west;
			else
				lonrange = 360.0f + east - west;

			System.Drawing.Bitmap heightMap = null;

			try
			{
				heightMap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(displacementFile);
			}
			catch
			{}

			vertices = new CustomVertex.PositionTextured[samples * samples];
			for(int i = 0; i < samples; i++)
			{
				for(int j = 0; j < samples; j++)
				{	
					double height = m_cloudBaseAltitude * World.Settings.VerticalExaggeration;
					if(heightMap != null)
					{
						double percentX = (double)j / (double)upperBound;
						double percentY = (double)i / (double)upperBound;

						if(percentX > 1)
							percentX = 1;
						if(percentY > 1)
							percentY = 1;

						int x = (int)(percentX * (heightMap.Width - 1));
						int y = (int)(percentY * (heightMap.Height - 1));
						height += m_displacementMultiplier * heightMap.GetPixel(x,y).R;
					}

					Vector3 pos = MathEngine.SphericalToCartesian( 
						north - scaleFactor*latrange*i,
						west + scaleFactor*lonrange*j, 
						this.World.EquatorialRadius + height);
					
					vertices[i*samples + j].X = pos.X;
					vertices[i*samples + j].Y = pos.Y;
					vertices[i*samples + j].Z = pos.Z;
					
					vertices[i*samples + j].Tu = j*scaleFactor;
					vertices[i*samples + j].Tv = i*scaleFactor;
				}
			}

			indices = new short[2 * upperBound * upperBound * 3];
			for(int i = 0; i < upperBound; i++)
			{
				for(int j = 0; j < upperBound; j++)
				{
					indices[(2*3*i*upperBound) + 6*j] = (short)(i*samples + j);
					indices[(2*3*i*upperBound) + 6*j + 1] = (short)((i+1)*samples + j);
					indices[(2*3*i*upperBound) + 6*j + 2] = (short)(i*samples + j+1);
	
					indices[(2*3*i*upperBound) + 6*j + 3] = (short)(i*samples + j+1);
					indices[(2*3*i*upperBound) + 6*j + 4] = (short)((i+1)*samples + j);
					indices[(2*3*i*upperBound) + 6*j + 5] = (short)((i+1)*samples + j+1);
				}
			}

			if(heightMap != null)
				heightMap.Dispose();
		}

        /// <summary>
        /// Updates renderable
        /// </summary>
        /// <param name="drawArgs"></param>
		public override void Update(DrawArgs drawArgs)
		{
			if(!isInitialized)
				Initialize(drawArgs);
		}

        /// <summary>
        /// Renders object
        /// </summary>
        /// <param name="drawArgs"></param>
		public override void Render(DrawArgs drawArgs)
		{
			try
			{
				if(!isInitialized)
					return;

                if (!drawArgs.CurrentWorld.IsEarth)
                    return;

				if(m_frames == null || m_frames.Count == 0)
					return;

                DynamicCloudFrame frame = (DynamicCloudFrame)m_frames[(m_currentFrame < m_frames.Count ? m_currentFrame : m_frames.Count - 1)];

				drawArgs.device.Transform.World = Matrix.Translation(
					(float)-drawArgs.WorldCamera.ReferenceCenter.X,
					(float)-drawArgs.WorldCamera.ReferenceCenter.Y,
					(float)-drawArgs.WorldCamera.ReferenceCenter.Z
					);

				if(m_enableHdrLighting)
				{
					RenderShaders(drawArgs, frame);
				}
				else
				{
					RenderFixedPipeline(drawArgs, frame);
				}

			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		private void RenderFixedPipeline(DrawArgs drawArgs, DynamicCloudFrame frame)
		{
			drawArgs.device.SetTexture(0, frame.Texture);
			drawArgs.device.VertexFormat = CustomVertex.PositionTextured.Format;

			drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
			drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;

			drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
			drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;

			drawArgs.device.DrawIndexedUserPrimitives(
				PrimitiveType.TriangleList,
				0,
				frame.Vertices.Length,
				frame.Indices.Length / 3,
				frame.Indices,
				true,
				frame.Vertices);

			drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
		}

		private void RenderShaders(DrawArgs drawArgs, DynamicCloudFrame frame)
		{
			if(m_effect == null)
			{
				drawArgs.device.DeviceReset += new EventHandler(device_DeviceReset);
				device_DeviceReset(drawArgs.device, null);
			}

			Surface currentRenderTarget = drawArgs.device.GetRenderTarget(0);
			drawArgs.device.SetRenderTarget(0, s1);
			drawArgs.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(0,0,0,0).ToArgb(), 1.0f, 0);

			drawArgs.device.RenderState.CullMode = Cull.None;
			drawArgs.device.VertexFormat = CustomVertex.PositionTextured.Format;
			drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
			drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Add;
			drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;

			m_effect.Technique = "VSClouds";
			m_effect.SetValue("World", drawArgs.device.Transform.World);
			m_effect.SetValue("View", drawArgs.device.Transform.View);
			m_effect.SetValue("Projection", drawArgs.device.Transform.Projection);
			m_effect.SetValue("Tex0", frame.Texture);
			m_effect.SetValue("RenderMap", m_surface1);
			m_effect.SetValue("FullResMap", m_surface1);
				
			//	drawArgs.device.Indices = m_indexBuffer;
			//	drawArgs.device.SetStreamSource(0, m_vertexBuffer, 0);
				
			int numPasses = m_effect.Begin(0);
			for(int i = 0; i < numPasses; i++)
			{
				m_effect.BeginPass(i);
				drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, 
					frame.Vertices.Length, frame.Indices.Length / 3, frame.Indices, true, frame.Vertices);
				
				//	drawArgs.device.DrawIndexedPrimitives(
				//		PrimitiveType.TriangleList,
				//		0,
				//		0,
				//		frame.Vertices.Length,
				//		0,
				//		frame.Indices.Length / 3);

				m_effect.EndPass();
			}

			m_effect.End();

			float pixelSizeX = -1.0f / (drawArgs.screenWidth / 2.0f);
			float pixelSizeY = 1.0f / (drawArgs.screenHeight / 2.0f);
			m_effect.SetValue("pixelSize", new Vector4(pixelSizeX, pixelSizeY, 1.0f, 1.0f));
			m_effect.SetValue("ExposureLevel", m_exposureLevel);
				
			drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;

			drawArgs.device.SetRenderTarget(0, s2);

			drawArgs.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(0,0,0,0).ToArgb(), 1.0f, 0);
			m_effect.Technique = "ScaleBuffer";

			m_effect.SetValue("RenderMap", m_surface2);
			numPasses = m_effect.Begin(0);
			for(int i = 0; i < numPasses; i++)
			{
				m_effect.BeginPass(i);
				drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, m_transformedVertices);
				m_effect.EndPass();
			}
			m_effect.End();

			drawArgs.device.SetRenderTarget(0, s3);
			drawArgs.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(0,0,0,0).ToArgb(), 1.0f, 0);
			m_effect.Technique = "GaussY";

			Vector4[] vSampleOffsets = new Vector4[16];
			float[] fSampleOffsets = new float[16];
			Vector4[] vSampleWeights = new Vector4[16];

			CreateTexCoordNTexelWeights(
				drawArgs.screenWidth / 4,
				ref fSampleOffsets,
				ref vSampleWeights,
				7.0f,
				1.5f);

			for(int i=0; i < 16; i++)
			{
				vSampleOffsets[i] = new Vector4(0.0f, fSampleOffsets[i], 0, 0);
			}

			m_effect.SetValue("TexelWeight", vSampleWeights);
			m_effect.SetValue("vertTapOffs", vSampleOffsets);
				
			numPasses = m_effect.Begin(0);
			for(int i = 0; i < numPasses; i++)
			{
				m_effect.BeginPass(i);
				drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, m_transformedVertices);
				m_effect.EndPass();
			}
			m_effect.End();

			drawArgs.device.SetRenderTarget(0, s2);
			drawArgs.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(0,0,0,0).ToArgb(), 1.0f, 0);
			m_effect.Technique = "GaussX";

			m_effect.SetValue("RenderMap", m_surface3);

			for(int i=0; i < 16; i++)
			{
				vSampleOffsets[i] = new Vector4(fSampleOffsets[i], 0, 0, 0);
			}

			m_effect.SetValue("horzTapOffs", vSampleOffsets);

			numPasses = m_effect.Begin(0);
			for(int i = 0; i < numPasses; i++)
			{
				m_effect.BeginPass(i);
				drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, m_transformedVertices);
				m_effect.EndPass();
			}
			m_effect.End();

			drawArgs.device.SetRenderTarget(0, currentRenderTarget);
			m_effect.Technique = "Screenblit";
			m_effect.SetValue("RenderMap", m_surface1);

			numPasses = m_effect.Begin(0);
			for(int i = 0; i < numPasses; i++)
			{
				m_effect.BeginPass(i);
				drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, m_transformedVertices);
				m_effect.EndPass();
			}
			m_effect.End();
		}

		Effect m_effect = null;

		CustomVertex.TransformedTextured[] m_transformedVertices = new Microsoft.DirectX.Direct3D.CustomVertex.TransformedTextured[4];
	
	//	static VertexBuffer m_vertexBuffer = null;
	//	static IndexBuffer m_indexBuffer = null;

		private void device_DeviceReset(object sender, EventArgs e)
		{
			Device device = (Device)sender;

			string outerrors = "";

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream effectStream = assembly.GetManifestResourceStream("MFW3D.Shaders.clouds.fx");

			m_effect = Effect.FromStream(
				device,
				effectStream,
				null,
				null,
				ShaderFlags.None,
				null,
				out outerrors);
					
			if(outerrors != null && outerrors.Length > 0)
				Log.Write(Log.Levels.Error, outerrors);
			
			m_surface1 = new Texture(
				device,
				DrawArgs.ParentControl.Width,
				DrawArgs.ParentControl.Height,
				1,
				Usage.RenderTarget,
				Format.A16B16G16R16F,
				Pool.Default);

			m_surface2 = new Texture(
				device,
				DrawArgs.ParentControl.Width / 2,
				DrawArgs.ParentControl.Height / 2,
				1,
				Usage.RenderTarget,
				Format.A16B16G16R16F,
				Pool.Default);

			m_surface3 = new Texture(
				device,
				DrawArgs.ParentControl.Width / 2,
				DrawArgs.ParentControl.Height / 2,
				1,
				Usage.RenderTarget,
				Format.A8R8G8B8,
				Pool.Default);

			s1 = m_surface1.GetSurfaceLevel(0);
			s2 = m_surface2.GetSurfaceLevel(0);
			s3 = m_surface3.GetSurfaceLevel(0);
		}

		Texture m_surface1;
		Texture m_surface2;
		Texture m_surface3;

		Surface s1;
		Surface s2;
		Surface s3;

		//-----------------------------------------------------------------------------
		// Name: CreateTexCoordNTexelWeights
		// Desc: Get the texture coordinate offsets to be used inside the Bloom
		//       pixel shader.
		//-----------------------------------------------------------------------------
		private void CreateTexCoordNTexelWeights(int dwRenderTargetSize,
			ref float[] fTexCoordOffset,
			ref Vector4[] vTexelWeight,
			float fDeviation,
			float fMultiplier )
		{
			int i=0;

			// width or height depending on which Gauss filter is applied
			// the vertical or horizontal filter
			float tu = 1.0f / (float)dwRenderTargetSize; 

			// Fill the center texel
			float weight = fMultiplier * GaussianDistribution( 0, 0, fDeviation );
			vTexelWeight[0] = new Vector4( weight, weight, weight, 1.0f );

			fTexCoordOffset[0] = 0.0f;

			// Fill the first half
			for( i=1; i < 8; i++ )
			{
				// Get the Gaussian intensity for this offset
				weight = fMultiplier * GaussianDistribution( (float)i, 0, fDeviation );
				fTexCoordOffset[i] = i * tu;

				vTexelWeight[i] = new Vector4( weight, weight, weight, 1.0f );
			}

			// Mirror to the second half
			for( i=8; i < 15; i++ )
			{
				vTexelWeight[i] = vTexelWeight[i-7];
				fTexCoordOffset[i] = -fTexCoordOffset[i-7];
			}
		}

		//-----------------------------------------------------------------------------
		// Name: GaussianDistribution
		// Desc: Helper function for CreateTexCoordNTexelWeights function to compute the 
		//       2 parameter Gaussian distribution using the given standard deviation
		//       rho
		//		 This might be optimized by using a look-up table
		//-----------------------------------------------------------------------------
		float GaussianDistribution( float x, float y, float rho )
		{
			float g = 1.0f / (float)Math.Sqrt( 2.0f * Math.PI * rho * rho );
			g *= (float)Math.Exp( -(x * x + y * y)/(2 * rho * rho));

			return g;
		}

        /// <summary>
        /// Disposes renderable
        /// </summary>
		public override void Dispose()
		{
			isInitialized = false;

			if(m_frames != null)
			{
				for(int i = 0; i < m_frames.Count; i++)
				{
					DynamicCloudFrame frame = (DynamicCloudFrame)m_frames[i];
					if(frame.Texture != null)
					{
						frame.Texture.Dispose();
						frame.Texture = null;
					}
				}

				m_frames.Clear();
			}

			if(m_effect != null)
			{
				m_effect.Dispose();
				m_effect = null;
			}
			if(m_surface1 != null)
			{
				m_surface1.Dispose();
				m_surface1 = null;
			}

			if(m_surface2 != null)
			{
				m_surface2.Dispose();
				m_surface2 = null;
			}
		
			if(m_surface3 != null)
			{
				m_surface3.Dispose();
				m_surface3 = null;
			}

			if(s1 != null)
			{
				s1.Dispose();
				s1 = null;
			}

			if(s2 != null)
			{
				s2.Dispose();
				s2 = null;
			}

			if(s3 != null)
			{
				s3.Dispose();
				s3 = null;
			}
		}

		private void DisposeHdrResources()
		{
			if(s1 != null)
			{
				s1.Dispose();
				s1 = null;
			}

			if(s2 != null)
			{
				s2.Dispose();
				s2 = null;
			}

			if(s3 != null)
			{
				s3.Dispose();
				s3 = null;
			}

			if(m_surface1 != null)
			{
				m_surface1.Dispose();
				m_surface1 = null;
			}

			if(m_surface2 != null)
			{
				m_surface2.Dispose();
				m_surface2 = null;
			}

			if(m_surface3 != null)
			{
				m_surface3.Dispose();
				m_surface3 = null;
			}

			if(m_effect != null)
			{
				m_effect.Dispose();
				m_effect = null;
			}

			
		}

        /// <summary>
        /// Executes upon selection
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <returns></returns>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		private class DynamicCloudFrame
		{
			public string ImageFile;
			public Texture Texture;

			public CustomVertex.PositionTextured[] Vertices = null;
			public short[] Indices = null;
		}

		int m_currentFrame = 0;
		bool m_isUpdating = false;
		
		private void m_animationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(m_isUpdating)
				return;

            if (!m_playing)
                return;

			m_isUpdating = true;
			try
			{
				if(m_frames.Count == 0)
					return;

				if(m_currentFrame + 1 >= m_frames.Count)
				{
					System.Threading.Thread.Sleep(m_lastFrameHoldTimeMs);
					m_currentFrame = 0;
				}
				else
				{
					m_currentFrame++;
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
			finally
			{
				m_isUpdating = false;
			}
		}
	}
}
