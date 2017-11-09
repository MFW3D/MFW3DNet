using System;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using MFW3D;
using MFW3D.Renderable;
using MFW3D.Net;

namespace NLT.Plugins
{
	/// <summary>
	/// retrieves information from shapefiles
	/// </summary>
	public class ShapeFileInfoTool : MFW3D.PluginEngine.Plugin 
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
