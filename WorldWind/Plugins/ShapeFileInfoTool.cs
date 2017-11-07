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
		/// <summary>
		/// Load the plugin
		/// </summary>
		public override void Load() 
		{
			// Subscribe events
			Global.worldWindow.MouseMove += new MouseEventHandler(onMouseMove);
			Global.worldWindow.KeyUp += new KeyEventHandler(onKeyUp);
		}

		/// <summary>
		/// Unload our plugin
		/// </summary>
		public override void Unload() 
		{
			// Unsubscribe events
			Global.worldWindow.MouseMove -= new MouseEventHandler(onMouseMove);
			Global.worldWindow.KeyUp -= new KeyEventHandler(onKeyUp);
			
		}
		

		public void onMouseMove(object sender, MouseEventArgs e)
		{
			//return if the tool is not "switched on" 

			//TODO: code
		}

		public void onKeyUp(object sender, KeyEventArgs e)
		{
			if((e.KeyCode==Keys.Q)&&(e.Control))
			{
			}
		}
	}
}
