using System;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace WorldWind.CMPlugins.OverviewForm
{
	/// <summary>
	/// Summary description for ImageTileCache.
	/// </summary>
	public class ImageTileCache
	{
		Device m_Device3d = null;
		System.Collections.Hashtable m_ImageTileHash = new System.Collections.Hashtable();

		System.Timers.Timer m_CleanupTimer = new System.Timers.Timer(5000);

		public ImageTileCache(Device device)
		{
			m_Device3d = device;
			m_CleanupTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_CleanupTimer_Elapsed);
			m_CleanupTimer.Start();
		}

		public void Add(ImageTileCacheEntry newEntry)
		{
			if(!m_ImageTileHash.Contains(newEntry.Resource))
			{
				m_ImageTileHash.Add(newEntry.Resource, newEntry);
			}
		}

		public ImageTileCacheEntry this[string key]
		{
			get
			{ 
				ImageTileCacheEntry tile = m_ImageTileHash[key] as ImageTileCacheEntry;
				if(tile != null && (tile.Texture == null || tile.Texture.Disposed))
				{
					tile.Load(m_Device3d);
				}

				if(tile != null)
				{
					tile.LastAccessed = System.DateTime.Now;
				}
				return tile;
			}
		}

		public void Dispose()
		{
			m_CleanupTimer.Stop();
			
		}
		
		private void m_CleanupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				System.Collections.ArrayList deletionKeys = new System.Collections.ArrayList();
				//lock(m_ImageTileHash.SyncRoot)
				{
					foreach(string key in m_ImageTileHash.Keys)
					{
						ImageTileCacheEntry tile = (ImageTileCacheEntry)m_ImageTileHash[key];

						if(System.DateTime.Now.Subtract(tile.LastAccessed) > System.TimeSpan.FromSeconds(30))
						{
							deletionKeys.Add(key);
						}
					}
				}

				foreach(string key in deletionKeys)
				{
					ImageTileCacheEntry tile = (ImageTileCacheEntry)m_ImageTileHash[key];
					m_ImageTileHash.Remove(key);
					if(tile.Texture != null && !tile.Texture.Disposed)
					{
						tile.Texture.Dispose();
					}
				}
			}
			catch
			{}
		}
	}

	public class ImageTileCacheEntry
	{
		double m_North;
		double m_South;
		double m_East;
		double m_West;


		Texture m_Texture = null;
		string m_Resource = null;

		public SurfaceDescription SurfaceDescription;
		System.DateTime m_LastAccessed = System.DateTime.Now;


		public ImageTileCacheEntry(string resource,
			double north,
			double south,
			double west,
			double east)
		{
			m_Resource = resource;
			m_North = north;
			m_South = south;
			m_East = east;
			m_West = west;
		}

		public void Load(Device device)
		{
			if(System.IO.File.Exists(m_Resource))
			{
				//Log.Write(Log.Levels.Debug, "Loading: " + m_Resource);
			
				m_Texture = TextureLoader.FromFile(device, m_Resource, 0, 0, 1, Usage.None, Format.Dxt3, Pool.Managed, Filter.Box, Filter.Box, 0);
				SurfaceDescription = m_Texture.GetLevelDescription(0);
			}
		}

		public System.DateTime LastAccessed
		{
			get{ return m_LastAccessed; }
			set{ m_LastAccessed = value; }
		}

		public string Resource
		{
			get{ return m_Resource;}
		}

		public Texture Texture
		{
			get{ return m_Texture; }
		}

		public double North
		{
			get{ return m_North;}
		}

		public double South
		{
			get{ return m_South;}
		}

		public double West
		{
			get{ return m_West;}
		}

		public double East
		{
			get{ return m_East; }
		}
	}
}
