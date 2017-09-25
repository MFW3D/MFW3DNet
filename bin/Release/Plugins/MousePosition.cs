//----------------------------------------------------------------------------
// NAME: Mouse Position
// VERSION: 0.2
// DESCRIPTION: Displays information about the mouse location on the globe.
// DEVELOPER: Patrick Murris
// WEBSITE: http://www.alpix.com/3d/worldwin
//----------------------------------------------------------------------------
// 0.2	March 26, 2007	Added metric/imperial units support
//			Added distance from camera target and heading
// 0.1	March 16, 2007	First version - using new PickingRayIntersectionWithTerrain()
//			from Bjorn Reppen aka "Mashi" Clock.cs
//----------------------------------------------------------------------------
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.Camera;

namespace Murris.Plugins
{
	public class MousePosPlugin : WorldWind.PluginEngine.Plugin
	{
		MousePosLayer layer;
		
		/// <summary>
		/// Plugin entry point - All plugins must implement this function
		/// </summary>
		public override void Load()
		{
			// Add us to the list of renderable objects - this puts us in the render loop
			layer = new MousePosLayer(Application);
			Application.WorldWindow.CurrentWorld.RenderableObjects.Add(layer);
			Application.WorldWindow.MouseMove += new MouseEventHandler(MouseMove);

		}

		/// <summary>
		/// Unloads our plugin
		/// </summary>
		public override void Unload()
		{
			Application.WorldWindow.CurrentWorld.RenderableObjects.Remove(layer.Name);
			ParentApplication.WorldWindow.MouseMove -= new MouseEventHandler(MouseMove);

		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			layer.MouseX = e.X;
			layer.MouseY = e.Y;
		}
	}

	/// <summary>
	/// Renderable object : display on-screen info text
	/// </summary>
	public class MousePosLayer : RenderableObject
	{
		int color1 = Color.GhostWhite.ToArgb();
		int color2 = Color.Black.ToArgb();
		const int distanceFromCorner = 5;
		MainApplication ww;

		public int MouseX = 0;
		public int MouseY = 0;

		public MousePosLayer(MainApplication app) : base("Mouse position", Vector3.Empty, Quaternion.Identity)
		{
			this.ww = app;
			
			// We want to be drawn on top of everything else
			this.RenderPriority = RenderPriority.Icons;

			// true to make this layer active on startup, this is equal to the checked state in layer manager
			this.IsOn = true;
		}

		/// <summary>
		/// Plugin entry point - All plugins must implement this function
		/// </summary>
		public void Load()
		{
			// Add us to the list of renderable objects - this puts us in the render loop
			ww.WorldWindow.CurrentWorld.RenderableObjects.Add(this);
		}

		/// <summary>
		/// This is where we do our rendering 
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			// Draw the mouse position info using default font in lower right corner
			CameraBase camera = drawArgs.WorldCamera;
			Angle lat, lon = Angle.NaN;
			string text = "";
			double alt = 0;
			float dist, dist1, dist2 = 0f;

			if(camera.Altitude > 60e3) {
				// Regular picking at sphere
				camera.PickingRayIntersection(MouseX, MouseY, out lat, out lon);
			} else {
				// Picking at terrain
				camera.PickingRayIntersectionWithTerrain(MouseX, MouseY, out lat, out lon, 
					ww.WorldWindow.CurrentWorld);
			}
			if(!Angle.IsNaN(lat)) {
				text += String.Format("Lat {0:f4}°  Lon {1:f4}°  ", lat.Degrees, lon.Degrees);

				// Get altitude from cache - no download waiting (may return NaN)
				alt = ww.WorldWindow.CurrentWorld.TerrainAccessor.GetCachedElevationAt(lat.Degrees, lon.Degrees);
				if (World.Settings.DisplayUnits == Units.Metric)
					{ text += double.IsNaN(alt) ? " " : String.Format("Elev {0:f1}m  ", alt);}
				else
					{ text += double.IsNaN(alt) ? " " : String.Format("Elev {0:f0}ft  ", alt * 3.2808399); }
				if(double.IsNaN(alt)) alt = 0;


				// Distance from camera target - cartesian
				double camAlt = ww.WorldWindow.CurrentWorld.TerrainAccessor.GetCachedElevationAt(camera.Latitude.Degrees, camera.Longitude.Degrees);
				if(double.IsNaN(camAlt)) camAlt = 0;
				Vector3 cameraPos = MathEngine.SphericalToCartesian(camera.Latitude, camera.Longitude, camAlt + camera.WorldRadius);
				Vector3 targetPos = MathEngine.SphericalToCartesian(lat, lon, alt + camera.WorldRadius);
				Vector3 toTarget = targetPos - cameraPos;
				dist1 = toTarget.Length();

				// Distance from camera target on ground - great circle
				Angle distA = MathEngine.SphericalDistance(camera.Latitude, camera.Longitude, lat, lon);
				dist2 = (float)(camera.WorldRadius * distA.Radians);

				// Use max distance between cartesian (straight line) and great circle
				dist = Math.Max(dist1, dist2);

				if (World.Settings.DisplayUnits == Units.Metric)
					{ 
					if (dist < 1000) text += String.Format("Dist {0:f1}m  ", dist);
					else text += String.Format("Dist {0:f1}Km  ", dist / 1000f);
					}
				else
					{ 
					if (dist < 2000) text += String.Format("Dist {0:f0}ft  ", dist * 3.2808399);
					else text += String.Format("Dist {0:f1}mi  ", dist * 3.2808399 / 5280);
					}

				// Heading
				Angle heading = MathEngine.Azimuth(camera.Latitude, camera.Longitude, lat, lon);
				text += String.Format("Heading {0:f1}°", heading.Degrees);

				// Use our cursor when the mouse isn't over other elements requiring different cursor
				if(DrawArgs.MouseCursor == CursorType.Arrow)
					DrawArgs.MouseCursor = CursorType.Cross;


			}
			// Draw text
			if(text != "") {
				Rectangle bounds = drawArgs.defaultDrawingFont.MeasureString(null, text, DrawTextFormat.None, 0);
				drawArgs.defaultDrawingFont.DrawText(null, text, drawArgs.screenWidth-bounds.Width-distanceFromCorner, drawArgs.screenHeight-bounds.Height-distanceFromCorner, 
					color2);
				drawArgs.defaultDrawingFont.DrawText(null, text, drawArgs.screenWidth-bounds.Width-distanceFromCorner-1, drawArgs.screenHeight-bounds.Height-distanceFromCorner-1, 
					color1);
			}
		}

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}
	}
}
