using System;

namespace WorldWind.PluginEngine
{
	/// <summary>
	///���������
	/// </summary>
	public abstract class Plugin
	{
		//�������Ŀ¼
		protected string m_PluginDirectory;
		//����Ƿ����
		protected bool m_isLoaded;
        //���Ŀ¼
        public virtual string PluginDirectory
		{
			get
			{
				return m_PluginDirectory;
			}
		}
		//��ǵ�ǰ����Ƿ����
		public virtual bool IsLoaded
		{
			get
			{
				return m_isLoaded;
			}
		}

		//���ز��
		public virtual void Load()
		{
			// Override with plugin initialization code.
		}

		//ж�ز��
		public virtual void Unload()
		{
			// Override with plugin dispose code.
		}
        //�������
        public virtual void PluginLoad( string pluginDirectory )
		{
			if(m_isLoaded)
				// Already loaded
				return;
			m_PluginDirectory = pluginDirectory;
			Load();
			m_isLoaded = true;
		}
		//���ж��
		public virtual void PluginUnload()
		{
			Unload();
			m_isLoaded = false;
		}
	}
}
