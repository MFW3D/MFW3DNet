using System;

namespace WorldWind.PluginEngine
{
	/// <summary>
	/// Base class to be derived by all plugins (loaded by PluginCompiler)
	/// Keep as light-weight as possible to keep plugin simple to write.
	/// </summary>
	public abstract class Plugin
	{

		/// <summary>
		/// The directory from which this plugin was loaded.
		/// </summary>
		protected string m_PluginDirectory;

		/// <summary>
		/// Plugin running flag (true while running, reset when exiting plugin)
		/// </summary>
		protected bool m_isLoaded;

		/// <summary>
		/// The location this plugin was loaded from.
		/// </summary>
		public virtual string PluginDirectory
		{
			get
			{
				return m_PluginDirectory;
			}
		}

		/// <summary>
		/// Whether the plugin is currently running.
		/// </summary>
		public virtual bool IsLoaded
		{
			get
			{
				return m_isLoaded;
			}
		}

		/// <summary>
		/// Load the plugin.  This is the plugin entry point.
		/// </summary>
		public virtual void Load()
		{
			// Override with plugin initialization code.
		}

		/// <summary>
		/// Unload the plugin. Plugins that modify World Wind or 
		/// runs in background should override this method.
		/// </summary>
		public virtual void Unload()
		{
			// Override with plugin dispose code.
		}

		/// <summary>
		/// Base class load, calls Load. 
		/// </summary>
		/// <param name="parent"></param>
		public virtual void PluginLoad( string pluginDirectory )
		{
			if(m_isLoaded)
				// Already loaded
				return;
			m_PluginDirectory = pluginDirectory;
			Load();
			m_isLoaded = true;
		}

		/// <summary>
		/// Base class unload, calls Unload. 
		/// </summary>
		public virtual void PluginUnload()
		{
			Unload();
			m_isLoaded = false;
		}
	}
}
