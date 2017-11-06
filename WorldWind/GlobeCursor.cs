using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.PluginEngine;


namespace Withak.Plugins
{
	public class GlobeCursorPlugin : WorldWind.PluginEngine.Plugin
	{

		KMLIcon ic;
		Icons ics;
		Bitmap cursBmp = new Bitmap("Plugins\\cursorIcon.png");

		public override void Load()
		{

			ics = new Icons("Globe cursor");

			ic = new KMLIcon("", 0f, 0f, "", 0f);
			ic.Image = cursBmp;
			ic.Width = 16;
			ic.Height = 16;
			
			ics.Add(ic);
			
			Global.worldWindow.CurrentWorld.RenderableObjects.Add(ics);

			Global.worldWindow.MouseMove += new MouseEventHandler(MouseMove);

			base.Load();

		}

		public override void Unload()
		{
			Global.worldWindow.CurrentWorld.RenderableObjects.Remove(ics);
			Global.worldWindow.MouseMove -= new MouseEventHandler(MouseMove);


			base.Unload();
		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			Angle lat,lon = Angle.NaN;
			Global.worldWindow.DrawArgs.WorldCamera.PickingRayIntersection(
				e.X,e.Y,out lat, out lon);
			ic.SetPosition((float)lat.Degrees, (float)lon.Degrees, 0f);
		}
		
	}
}
