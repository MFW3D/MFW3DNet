using System;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.Net;

namespace NLT.Plugins
{
	/// <summary>
	/// retrieves information from shapefiles
	/// </summary>
	public class ShapeFileInfoTool : WorldWind.PluginEngine.Plugin 
	{
		MenuItem m_MenuItem;

		/// <summary>
		/// Load the plugin
		/// </summary>
		public override void Load() 
		{
			//The menu Entry
			m_MenuItem = new MenuItem("Shapefile info\tCtrl+Q");
			m_MenuItem.Click += new EventHandler(menuItemClicked);

			// Subscribe events
			Global.worldWindow.MouseMove += new MouseEventHandler(onMouseMove);
			Global.worldWindow.KeyUp += new KeyEventHandler(onKeyUp);
		}

		/// <summary>
		/// Unload our plugin
		/// </summary>
		public override void Unload() 
		{
			if(m_MenuItem!=null)
			{
				m_MenuItem.Dispose();
				m_MenuItem = null;
			}			

			// Unsubscribe events
			Global.worldWindow.MouseMove -= new MouseEventHandler(onMouseMove);
			Global.worldWindow.KeyUp -= new KeyEventHandler(onKeyUp);
			
		}
		

		void menuItemClicked(object sender, EventArgs e)
		{	
			if(m_MenuItem.Checked)
			{
				Global.worldWindow.MouseMove -= new MouseEventHandler(onMouseMove);				
				m_MenuItem.Checked=false;
			}
			else
			{
				Global.worldWindow.MouseMove += new MouseEventHandler(onMouseMove);				
				m_MenuItem.Checked=true;
			}			
		}		

		public void onMouseMove(object sender, MouseEventArgs e)
		{
			//return if the tool is not "switched on" 
			if(!m_MenuItem.Checked)
				return;

			//TODO: code
		}

		public void onKeyUp(object sender, KeyEventArgs e)
		{
			if((e.KeyCode==Keys.Q)&&(e.Control))
			{
				if(m_MenuItem.Checked)
				{
					Global.worldWindow.MouseMove -= new MouseEventHandler(onMouseMove);					
					m_MenuItem.Checked=false;
				}
				else
				{
					Global.worldWindow.MouseMove += new MouseEventHandler(onMouseMove);					
					m_MenuItem.Checked=true;
				}		
			}
		}
	}
}
