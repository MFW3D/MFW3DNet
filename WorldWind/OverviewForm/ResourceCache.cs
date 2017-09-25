using System;
using System.Collections;

namespace WorldWind.CMPlugins.OverviewForm
{
	/// <summary>
	/// Summary description for ResourceCache.
	/// </summary>
	public class ResourceCache
	{

		System.Collections.Hashtable m_ResourceHash = new System.Collections.Hashtable();

		System.Timers.Timer m_CleanupTimer = new System.Timers.Timer(5000);

		public ResourceCache()
		{
			m_CleanupTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_CleanupTimer_Elapsed);
			m_CleanupTimer.Start();
		}

		public void Add(string key, ResourceCacheEntry newEntry)
		{
			if(!m_ResourceHash.Contains(key))
			{
				m_ResourceHash.Add(key, newEntry);
			}
		}

		public ResourceCacheEntry this[string key]
		{
			get
			{ 
				ResourceCacheEntry tile = m_ResourceHash[key] as ResourceCacheEntry;
				
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
				foreach(string key in m_ResourceHash.Keys)
				{
					ResourceCacheEntry tile = (ResourceCacheEntry)m_ResourceHash[key];

					if(System.DateTime.Now.Subtract(tile.LastAccessed) > System.TimeSpan.FromSeconds(30))
					{
						deletionKeys.Add(key);
					}
				}
			}

				foreach(string key in deletionKeys)
				{
					ResourceCacheEntry tile = (ResourceCacheEntry)m_ResourceHash[key];
					m_ResourceHash.Remove(key);
				}
			}
			catch
			{}
		}
	}

	public class ResourceCacheEntry
	{

		System.DateTime m_LastUpdated = System.DateTime.Now;
		System.DateTime m_LastAccessed = System.DateTime.Now;
		System.Object m_Object = null;

		public System.Object Object
		{
			get{ return m_Object; }
			set{ m_Object = value; }
		}

		public System.DateTime LastAccessed
		{
			get{ return m_LastAccessed; }
			set{ m_LastAccessed = value; }
		}

		public System.DateTime LastUpdated
		{
			get{ return m_LastUpdated; }
			set{ m_LastUpdated = value;}
		}

		public ResourceCacheEntry()
		{

		}
	}
}
