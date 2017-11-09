using System;

namespace WorldWind.PluginEngine
{
	/// <summary>
	///插件基础类
	/// </summary>
	public abstract class Plugin
	{
		//插件加载目录
		protected string m_PluginDirectory;
		//插件是否加载
		protected bool m_isLoaded;
        //插件目录
        public virtual string PluginDirectory
		{
			get
			{
				return m_PluginDirectory;
			}
		}
		//标记当前插件是否加载
		public virtual bool IsLoaded
		{
			get
			{
				return m_isLoaded;
			}
		}

		//加载插件
		public virtual void Load()
		{
			// Override with plugin initialization code.
		}

		//卸载插件
		public virtual void Unload()
		{
			// Override with plugin dispose code.
		}
        //插件加载
        public virtual void PluginLoad( string pluginDirectory )
		{
			if(m_isLoaded)
				// Already loaded
				return;
			m_PluginDirectory = pluginDirectory;
			Load();
			m_isLoaded = true;
		}
		//插件卸载
		public virtual void PluginUnload()
		{
			Unload();
			m_isLoaded = false;
		}
	}
}
