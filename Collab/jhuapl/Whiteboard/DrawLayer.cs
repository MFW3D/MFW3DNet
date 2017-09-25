//========================= (UNCLASSIFIED) ==============================
// Copyright © 2007 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2007 The Johns Hopkins University. 
//
// This software was developed at The Johns Hopkins University/Applied 
// Physics Laboratory (“JHU/APL”) that is the author thereof under the 
// “work made for hire” provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the “Software”), to use the 
// Software without restriction, including without limitation the rights 
// to copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit others to do so, subject to the 
// following conditions: 
//
// 1.  This LICENSE AND DISCLAIMER, including the copyright notice, shall 
//     be included in all copies of the Software, including copies of 
//     substantial portions of the Software; 
//
// 2.  JHU/APL assumes no obligation to provide support of any kind with 
//     regard to the Software.  This includes no obligation to provide 
//     assistance in using the Software nor to provide updated versions of 
//     the Software; and 
//
// 3.  THE SOFTWARE AND ITS DOCUMENTATION ARE PROVIDED AS IS AND WITHOUT 
//     ANY EXPRESS OR IMPLIED WARRANTIES WHATSOEVER.  ALL WARRANTIES 
//     INCLUDING, BUT NOT LIMITED TO, PERFORMANCE, MERCHANTABILITY, FITNESS
//     FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT ARE HEREBY DISCLAIMED.  
//     USERS ASSUME THE ENTIRE RISK AND LIABILITY OF USING THE SOFTWARE.  
//     USERS ARE ADVISED TO TEST THE SOFTWARE THOROUGHLY BEFORE RELYING ON 
//     IT.  IN NO EVENT SHALL THE JOHNS HOPKINS UNIVERSITY BE LIABLE FOR 
//     ANY DAMAGES WHATSOEVER, INCLUDING, WITHOUT LIMITATION, ANY LOST 
//     PROFITS, LOST SAVINGS OR OTHER INCIDENTAL OR CONSEQUENTIAL DAMAGES, 
//     ARISING OUT OF THE USE OR INABILITY TO USE THE SOFTWARE. 
//
using System;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Xml;
using System.ComponentModel;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using WorldWind;
using WorldWind.Menu;
using WorldWind.Renderable;

using Collab.jhuapl.Util;

namespace Collab.jhuapl.Whiteboard
{
	/// <summary>
	/// CS DrawLayer implements whiteboarding capability.  Users can draw basic shapes and
	/// overlay them as terrain mapped paths.
	/// </summary>
	public class DrawLayer : WorldWind.Renderable.RenderableObjectList, IMenu
	{
		/// <summary>
		/// The current drawmode.
		/// None = Draw nothing.
		/// Hotspot = Adds an icon where you left mouse click.
		/// PostIt = Adds a text block to where you left mouse click.
		/// Polyline = Add a point per left mouse click. Right mouse to stop.		
		/// Polygon = Add a point per left mouse click. Right mouse to stop.  Last segment to close polygon automatically added.
		/// Freehand = Add points starting on mouse down until mouse up.'
		/// Disabled = This draw layer is read-only.
		/// </summary>
		public enum DrawMode
		{
			None,
			Hotspot,
			PostIt,
			Polyline,
			Polygon,
			Freehand,
			Incident,
			Disabled,
			Delete
		}
		
		// Where the mouse down even occured.  If this is different than mouse up then dragging occured.
		Point m_mouseDownPoint;

		// The draw color
		Color m_color = System.Drawing.Color.Red;

		// A unique to this draw layer path id.  Simply increments.
		int m_lastPathId = 0;

		// The altitude to draw the shapes and objects = terrain height + default height
		float m_drawAlt = 0;

		// The standard height to draw.
		float m_height = (float) 0.0;

		/// <summary>
		/// The path being built
		/// </summary>
		TerrainPath m_currPathLine;

		/// <summary>
		/// The hotspot built;
		/// </summary>
		Hotspot m_hotspot;

		Icons m_hotspotLayer;

		HotspotForm m_hotspotDialog;

		/// <summary>
		/// A guideline to show where the line segment would be drawn if you left click
		/// </summary>
		CustomVertex.PositionColored[] m_guideLine = new CustomVertex.PositionColored[17];

		/// <summary>
		/// The current drawing mode.  Set to the desired shape when starting and reset 
		/// to none when complete.
		/// </summary>
		private DrawMode m_currDrawMode = DrawMode.None;

		/// <summary>
		/// Latitude of first position
		/// </summary>
		Angle m_firstLat;

		/// <summary>
		/// Longitude of first position
		/// </summary>
		Angle m_firstLon;

		/// <summary>
		/// Altitude of first position
		/// </summary>
		double m_firstAlt;

		/// <summary>
		/// Latitude of last position
		/// </summary>
		Angle m_lastLat;

		/// <summary>
		/// Longitude of last position
		/// </summary>
		Angle m_lastLon;

		Point m_lastMousePoint;

		bool m_drawModeChange = false;

		#region Properties

		/// <summary>
		/// Determines if the draw layer is writeable.  True = writeable, False = Read only.
		/// </summary>
		public bool Writeable
		{
			get
			{
				return m_writeable;
			}
			set
			{
				if (m_writeable != value)
				{
					m_writeable = value;
					if (m_writeable)
					{
						DrawingMode = DrawMode.None;
						DrawLock = false;
					}
					else
					{
						DrawingMode = DrawMode.Disabled;
						DrawLock = false;
					};
				}
			}
		}
        bool m_writeable = true;


		/// <summary>
		/// Determines if the draw layer stays in a draw mode when the current shape is complete.
		/// True = keep drawing, False = go back to the none state when current drawing complete.
		/// </summary>
		public bool DrawLock
		{
			get
			{
				return m_drawLock;
			}
			set
			{
				m_drawLock = value;
			}
		}
        bool m_drawLock = false;

        /// <summary>
        /// The specified drawing mode.  Used when locks is enabled to reset the current
        /// draw mode for the next shape.
        /// </summary>
		public DrawMode DrawingMode
		{
			get
			{
				return m_drawMode;
			}
			set
			{
				// If we're changing the drawing mode
				if (m_drawMode != value)
				{
					switch (value)
					{
						case DrawMode.None:
							StopDrawing();
							break;
						case DrawMode.Hotspot:
							DrawHotspot();
							break;
						case DrawMode.Incident:
							DrawIncident();
							break;
						case DrawMode.PostIt:
							DrawPostIt();
							break;
						case DrawMode.Polyline:
							DrawPolyline();
							break;
						case DrawMode.Polygon:
							DrawPolygon();
							break;
						case DrawMode.Freehand:
							DrawFreehand();
							break;
						case DrawMode.Disabled:
							StopDrawing();
							Writeable = false;
							break;
						default:
							break;
					}
				}
			}
		}
        private DrawMode m_drawMode = DrawMode.None;

        /// <summary>
        /// Whther or not the mouse is currently in drag mode for drawlayers
        /// </summary>
        public bool MouseDragMode
        {
            get { return m_mouseDragMode; }
            set { m_mouseDragMode = value; }
        }
        protected static bool m_mouseDragMode;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of this drawing layer</param>
		public DrawLayer(string name) : base (name)
		{
			// initialize colors
			for(int i = 0; i < m_guideLine.Length; i++)
				m_guideLine[i].Color = World.Settings.MeasureLineLinearColorXml;

			m_hotspotDialog = new HotspotForm();
			m_hotspotDialog.Hide();

            m_world = DrawArgs.CurrentWorldStatic;
		}
	
		public override void Dispose()
		{
			this.isInitialized = false;
		}
	
		#region Drawing Methods

		/// <summary>
		/// Draw a line from the last point to current position
		/// </summary>
		void BuildGuideLine( Angle lat, Angle lon )
		{
			Angle angularDistance = World.ApproxAngularDistance( m_lastLat, m_lastLon, lat, lon );

			for(int i=0; i<m_guideLine.Length; i++)
			{
				float t = (float)i / (m_guideLine.Length-1);
				Vector3 cart = DrawArgs.CurrentWorldStatic.IntermediateGCPoint(t, m_lastLat, m_lastLon, lat, lon,
					angularDistance );

				m_guideLine[i].X = cart.X;
				m_guideLine[i].Y = cart.Y;
				m_guideLine[i].Z = cart.Z;
			}
		}

		/// <summary>
		/// Stops any drawing in progress and reset the drawmode to none.
		/// </summary>
		public void StopDrawing()
		{
			switch (m_currDrawMode)
			{
				// if we are in hotspot mode
				case DrawMode.Hotspot:
				{
					stopHotspot();
					break;
				}
					
				// if we are in incident mode
				case DrawMode.Incident:
				{
					stopIncident();
					break;
				}
				// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					stopPoly();
					break;
				}

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
					stopFreehand();
					break;
				}
			}

			m_drawMode = DrawMode.None;
			m_currDrawMode = DrawMode.None;
		}

		/// <summary>
		/// Stops any current drawing in progress and sets the current draw mode to none.  This
		/// allows the next mouse down event to start a new drawing using the desired draw mode.
		/// </summary>
		public void PauseDrawing()
		{
			switch (m_currDrawMode)
			{
				// if we are in hotspot mode
				case DrawMode.Hotspot:
				{
					stopHotspot();
					break;
				}				
					
				// if we are in incident mode
				case DrawMode.Incident:
				{
					stopIncident();
					break;
				}

				// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					stopPoly();
					break;
				}

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
					stopFreehand();
					break;
				}
			}
			m_currDrawMode = DrawMode.None;
		}

		/// <summary>
		/// Sets up drawing of a hotspot (icon) on the layer.  Pops up a dialog asking for 
		/// hotspot bitmap, name, description and URL.
		/// </summary>
		public void DrawHotspot()
		{
			StopDrawing();
			m_drawMode = DrawMode.Hotspot;
		}

		/// <summary>
		/// Sets up drawing of a incident (icon) on the layer.  Pops up a dialog asking for 
		/// incident bitmap, name, description and URL.
		/// </summary>
		public void DrawIncident()
		{
			StopDrawing();
			m_drawMode = DrawMode.Incident;
		}

		/// <summary>
		/// Sets up drawing of a text box on the layer.  Pops up a dialog asking for text to 
		/// insert.
		/// </summary>
		public void DrawPostIt()
		{
			StopDrawing();
			m_drawMode = DrawMode.PostIt;
		}

		/// <summary>
		/// Sets up drawing of a polyline.
		/// </summary>
		public void DrawPolyline()
		{
			StopDrawing();
			m_drawMode = DrawMode.Polyline;
		}

		/// <summary>
		/// Sets up drawing of a polygon
		/// </summary>
		public void DrawPolygon()
		{
			StopDrawing();
			m_drawMode = DrawMode.Polygon;
		}

		/// <summary>
		/// Sets up drawing of a freehand shape.
		/// </summary>
		public void DrawFreehand()
		{
			StopDrawing();
			m_drawMode = DrawMode.Freehand;
		}

		/// <summary>
		/// Stops the drawing of a polyline or polygon by completing the WW path object.
		/// </summary>
		/// <returns>Success or failure of building the final path object.</returns>
		private bool stopPoly()
		{					
			Logger.Write(2, "DRAW", m_currPathLine.Name, "Stop Poly" + m_currPathLine.Name);

			// if we are a polygon add the starting point as the last point
			if (m_currDrawMode == DrawMode.Polygon)
			{
				// add intermediate points if distance is great
				Angle dist = World.ApproxAngularDistance(m_lastLat, m_lastLon, m_firstLat, m_firstLon);
				if (dist.Degrees > 10)
				{
					for(int i=0; i < 10; i++)
					{
						float t = (float)i / 9;
						Angle iLat, iLon;

						WorldWind.World.IntermediateGCPoint(t, m_lastLat, m_lastLon, m_firstLat, m_firstLon,
							dist, out iLat, out iLon );

						m_currPathLine.Add((float)iLat.Degrees, (float) iLon.Degrees, m_drawAlt);
					}
				}

				m_currPathLine.Add((float) m_firstLat.Degrees, (float) m_firstLon.Degrees, m_drawAlt);
				Logger.Write(3, "DRAW", m_firstLat.Degrees, m_firstLon.Degrees, m_drawAlt, m_currPathLine.Name, "Adding Point to path " + m_currPathLine.Name);
			}

			// copy to a LineFeature
            Point3d[] points = new Point3d[m_currPathLine.SphericalCoordinatesList.Count];

			for (int i = 0; i < m_currPathLine.SphericalCoordinatesList.Count; i++)
			{
                Vector3 outCoord = m_currPathLine.SphericalCoordinatesList[i];

				points[i] = new Point3d(outCoord.Y, outCoord.X, outCoord.Z);
			}

			// TODO fix display altitude
			LineFeature path = new LineFeature(m_currPathLine.Name, DrawArgs.CurrentWorldStatic, points, m_color);
			path.LineWidth = (float) 3.0;

			// delete current path
			Remove(m_currPathLine.Name);

			// add the new path
			Add(path);

			Debug.WriteLine(path.Name + ": " + path.NumPoints);

			SendPath(m_currDrawMode, path);

			// clear out positions
			m_lastMousePoint = Point.Empty;
			m_mouseDownPoint = Point.Empty;

			// set current drawmode off
			m_currDrawMode = DrawMode.None;
			MouseDragMode = true;

			return true;
		}

		/// <summary>
		/// Stops the drawing of a freehand shape.
		/// </summary>
		/// <returns>Success or failure of building the final path object.</returns>
		private bool stopFreehand()
		{
			// copy to a LineFeature
            Point3d[] points = new Point3d[m_currPathLine.SphericalCoordinatesList.Count];

            for (int i = 0; i < m_currPathLine.SphericalCoordinatesList.Count; i++)
			{
				Vector3 outCoord = m_currPathLine.SphericalCoordinatesList[i];

				points[i] = new Point3d(outCoord.Y, outCoord.X, outCoord.Z);
			}

			// TODO fix display altitude
			LineFeature path = new LineFeature(m_currPathLine.Name, DrawArgs.CurrentWorldStatic, points, m_color);
			path.LineWidth = (float) 3.0;
			path.MaximumDisplayAltitude = double.MaxValue;
			path.MinimumDisplayAltitude = 0;

			// delete current path
			Remove(m_currPathLine.Name);

			// add the new path
			Add(path);

            Debug.WriteLine(m_currPathLine.Name + ": " + m_currPathLine.SphericalCoordinatesList.Count);

			SendPath(m_currDrawMode, path);

			// clear out positions
			m_lastMousePoint = Point.Empty;
			m_mouseDownPoint = Point.Empty;

			// set current drawmode off
			m_currDrawMode = DrawMode.None;
			MouseDragMode = true;

			Logger.Write(2, "DRAW", m_currPathLine.Name, "Stop Freehand" + m_currPathLine.Name);

			return true;
		}

		/// <summary>
		/// Stops the drawing of Hotspots.
		/// </summary>
		/// <returns>True</returns>
		private bool stopHotspot()
		{
			// clear out positions
			m_lastMousePoint = Point.Empty;
			m_mouseDownPoint = Point.Empty;

			// set current drawmode off
			m_currDrawMode = DrawMode.None;
			MouseDragMode = true;

			return true;
		}

		/// <summary>
		/// Stops the drawing of Incidents.
		/// </summary>
		/// <returns>True</returns>
		private bool stopIncident()
		{
			// clear out positions
			m_lastMousePoint = Point.Empty;
			m_mouseDownPoint = Point.Empty;

			// set current drawmode off
			m_currDrawMode = DrawMode.None;
			MouseDragMode = true;

			return true;
		}

		/// <summary>
		/// Gets the next available "unique" path id.
		/// </summary>
		/// <returns>Next path id</returns>
		private int getAvailablePathId()
		{
			m_lastPathId++;
			return m_lastPathId;
		}

		#endregion

		#region UI Event Methods

		/// <summary>
		/// Called when a key is pressed down.
		/// </summary>
		/// <param name="keyEvent"></param>
		public void OnKeyDown(System.Windows.Forms.KeyEventArgs keyEvent)
		{
			// TODO:  Add JHU_DrawLayer.OnKeyDown implementation
		}
	
		/// <summary>
		/// Called when a key is released.
		/// </summary>
		/// <param name="keyEvent"></param>
		public void OnKeyUp(System.Windows.Forms.KeyEventArgs keyEvent)
		{
			// TODO:  Add JHU_DrawLayer.OnKeyUp implementation

		}
	
		/// <summary>
		/// Called when a mouse button is pressed.  Depending on the draw mode this will either
		/// start the drawing of a shape, add a point to the shape or draw the new object.
		/// </summary>
		/// <param name="e">Mouse event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			// If we aren't active just ignore
			if ( (!isOn) && (m_drawMode != DrawMode.None) )
				return false;

			Angle lat;
			Angle lon;

			// get the current mouse position
            if (DrawArgs.Camera.Altitude > 500000)
            {
                DrawArgs.Camera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out lat,
                    out lon);
            }
            else
            {
                DrawArgs.Camera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out lat,
                    out lon,
                    DrawArgs.CurrentWorldStatic);
            }


			// if we're off the globe ignore
			if (Angle.IsNaN(lat))
			{
				return false;
			}

			// save start position
			m_mouseDownPoint.X = e.X;
			m_mouseDownPoint.Y = e.Y;

			m_lastMousePoint = m_mouseDownPoint;

			// if we aren't currently drawing then start
			if (m_currDrawMode == DrawMode.None)
			{
				m_currDrawMode = m_drawMode;
				m_drawModeChange = true;
			}

			switch (m_currDrawMode)
			{
				// if we are in click mode (polys and points)
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					break;
				}
				case DrawMode.Hotspot:
				{
					// if the right mouse button was hit we're done with the poly
					if (e.Button == MouseButtons.Right)
					{
						stopHotspot();
						return true;
					}

					// ignore if we've been dragged
					int dx = e.X - m_lastMousePoint.X;
					int dy = e.Y - m_lastMousePoint.Y;
					if(dx*dx+dy*dy > 3*3)
					{
						m_mouseDownPoint = Point.Empty;

						return false;
					}

					// if this is the first click then
					if (m_drawModeChange)
					{
						int pathId = this.getAvailablePathId();	
						m_drawAlt = m_height;

						// Prompt for hotspot name
						m_hotspotDialog.HotspotName = m_currDrawMode + "-" + pathId;
						m_hotspotDialog.ShowDialog();

						if (m_hotspotDialog.DialogResult == DialogResult.OK)
						{
							m_hotspot = new Hotspot (
								m_hotspotDialog.HotspotName, 
								lat.Degrees,
								lon.Degrees,
								m_drawAlt,
								m_color,
								m_hotspotDialog.URL,
								m_hotspotDialog.Description);

                            if (m_hotspotDialog.SaveCameraAngle)
                            {
                                m_hotspot.OnClickZoomAltitude = DrawArgs.Camera.Altitude;
                                m_hotspot.OnClickZoomHeading = DrawArgs.Camera.Heading.Degrees;
                                m_hotspot.OnClickZoomTilt = DrawArgs.Camera.Tilt.Degrees;
                            }

							if (m_hotspotLayer == null)
							{
								m_hotspotLayer = new Icons("Hotspot Layer");
								m_hotspotLayer.IsOn = true;
								Add(m_hotspotLayer);
							}

							m_hotspotLayer.Add(m_hotspot);

							Logger.Write(2, "DRAW", lat.Degrees, lon.Degrees, m_drawAlt, m_hotspotDialog.HotspotName, "Added Hotspot " + m_hotspotDialog.HotspotName);

							SendHotspot(m_hotspot);
						}

						if (!m_drawLock)
							m_drawMode = DrawMode.None;											

						stopHotspot();
						return true;
					}
					break;
				}				
				case DrawMode.Incident:
				{
					// if the right mouse button was hit we're done with the poly
					if (e.Button == MouseButtons.Right)
					{
						stopIncident();
						return true;
					}
//
//					// ignore if we've been dragged
//					int dx = e.X - m_lastMousePoint.X;
//					int dy = e.Y - m_lastMousePoint.Y;
//					if(dx*dx+dy*dy > 3*3)
//					{
//						m_mouseDownPoint = Point.Empty;
//
//						return false;
//					}
//
//					// if this is the first click then
//					if (m_drawModeChange)
//					{
//						int pathId = this.getAvailablePathId();	
//						m_drawAlt = m_height;
//
//						// Prompt for hotspot name & type
//						m_iconDialog.IconName = "New" + m_iconDialog.IconTypeName + "-" + pathId;
//						m_iconDialog.IconIdNum = pathId;
//						m_iconDialog.IconType = this.CurrIconType;
//						m_iconDialog.ShowDialog();
//
//						if (m_iconDialog.DialogResult == DialogResult.OK)
//						{
//							// See if already in track table
//							m_incident = m_activeTracks[m_iconDialog.IconName] as CS_TrackData;
//
//							if (m_incident == null)
//							{
//								m_incident = new CS_TrackData (
//									"Operator",
//									m_iconDialog.IconName,
//									m_iconDialog.IconName,
//									m_iconDialog.IconType,
//									JHU_Enums.Affiliations.HOSTILE,
//									JHU_Enums.BattleDimensions.GROUND,
//									m_iconDialog.Description,
//									m_iconDialog.Description,
//									lat.Degrees,
//									lon.Degrees,
//									m_drawAlt,
//									0,
//									0,
//									"",
//									DateTime.UtcNow,
//									DateTime.UtcNow,
//									"");
//
//								this.CurrIconType = m_iconDialog.IconTypeName;
//
//								Logger.Write(2, "DRAW", lat.Degrees, lon.Degrees, m_drawAlt, m_iconDialog.IconName, "Added Incident " + m_iconDialog.IconName);
//
//								// add to master table
//								m_activeTracks.Add(m_incident);
//							}
//							else
//							{
//								// Update potentially changing values
//								m_incident.Type = m_iconDialog.IconType;
//								m_incident.SetPosition(lat.Degrees, lon.Degrees, m_drawAlt);
//
//								m_incident.UpdateTime = DateTime.UtcNow;
//								m_incident.Description = m_iconDialog.Description;
//							}
//
//							SendTrack(m_incident);
//						}
//
//						if (!m_drawLock)
//							m_drawMode = DrawMode.None;											
//
//						stopIncident();
//						return true;
//					}
					break;
				}
				case DrawMode.PostIt:
				{
					// Actually add the object on mouse up, not mouse down.
					// If mouse is dragged too much between down and up then ignore this click.
					break;
				}

				// if we are in freehand draw then start drawing.
				case DrawMode.Freehand:
				{
					MouseDragMode = false;

					m_drawAlt = DrawArgs.Camera.TerrainElevation + m_height;

					int pathId = this.getAvailablePathId();
					m_currPathLine = new TerrainPath("Shape" + "-" + pathId + ".wwb", 
						DrawArgs.CurrentWorldStatic, 
						0.0,
                        (DrawArgs.Camera.Altitude * 1.1) + 100000, 
						"Shape" + "-" + pathId + ".wwb", 
						m_drawAlt, 
						m_color,
						DrawArgs.CurrentWorldStatic.TerrainAccessor);

					Add(m_currPathLine);

					// add point to path line
					m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, m_drawAlt);

					// save the first point
					m_firstLat = lat;
					m_firstLon = lon;
					m_firstAlt = m_drawAlt;

					m_currPathLine.IsOn = true;

					Logger.Write(2, "DRAW", m_firstLat.Degrees, m_firstLon.Degrees, m_drawAlt, m_currPathLine.Name, "Starting Freehand path " + m_currPathLine.Name);

					if (!m_drawLock)
						m_drawMode = DrawMode.None;

					break;
				}
					// We aren't drawing.  Just ignore this event.
				default:
					return false;
			}

			return true;
		}
	
		/// <summary>
		/// Called when the mose moves.
		/// </summary>
		/// <param name="e">Mouse event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			// If we aren't active just ignore
			if ( (!isOn) && (m_drawMode != DrawMode.None) )
				return false;

			Angle lat;
			Angle lon;

			// get the current mouse position
            if (DrawArgs.Camera.Altitude > 500000)
            {
                DrawArgs.Camera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out lat,
                    out lon);
            }
            else
            {
                DrawArgs.Camera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out lat,
                    out lon,
                    DrawArgs.CurrentWorldStatic);
            }

			// if we're off the globe ignore
			if (Angle.IsNaN(lat))
			{
				return false;
			}

			switch (m_currDrawMode)
			{

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
					// if we've moved more than a jiggle
					int dx = e.X - m_lastMousePoint.X;
					int dy = e.Y - m_lastMousePoint.Y;
					if(dx*dx+dy*dy > 2*2)
					{
						m_drawAlt = DrawArgs.Camera.TerrainElevation + m_height;

						// add point to the path line
						m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, m_drawAlt);

						// save last position
						m_lastLat = lat;
						m_lastLon = lon;

						m_lastMousePoint.X = e.X;
						m_lastMousePoint.Y = e.Y;
					}
					break;
				}

					// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					// if this isn't the first point draw a guide line
					if (!m_drawModeChange)
						BuildGuideLine(lat, lon);

					break;
				}

				case DrawMode.Hotspot:
				case DrawMode.Incident:
				case DrawMode.PostIt:
				{
					// No need to do anything.  A little mouse jiggle wont cancel this
					// drawing.  Determine if it is too much on mouse up.  Keep this
					// so we return true at the end and not let someone else handle 
					// this mouse event (or the camera might move or angle).
					break;
				}

				// We aren't drawing.  Just ignore this event.
				default:
					return false;
			}

			return true;
		}

		/// <summary>
		/// Called when a mouse button is released.
		/// </summary>
		/// <param name="e">Mouse event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			// If we aren't active just ignore
			if ( (!isOn) && (m_drawMode != DrawMode.None) )
				return false;

			// if we were off the globe on mouse down ignore
			if (m_mouseDownPoint == Point.Empty)
			{
				return false;
			}

			Angle lat;
			Angle lon;

            // get the current mouse position
            if (DrawArgs.Camera.Altitude > 500000)
            {
                DrawArgs.Camera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out lat,
                    out lon);
            }
            else
            {
                DrawArgs.Camera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out lat,
                    out lon,
                    DrawArgs.CurrentWorldStatic);
            }

			// if we're off the globe clear modes and ignore
			if (Angle.IsNaN(lat))
			{
				// if they let go in a free hand draw off the map cancel path?
				if (m_currDrawMode == DrawMode.Freehand)
				{
					stopFreehand();
				}

				m_mouseDownPoint = Point.Empty;
				m_lastMousePoint = Point.Empty;

				return false;
			}

			m_drawAlt = DrawArgs.Camera.TerrainElevation + m_height;

			switch (m_currDrawMode)
			{
					// if we're not drawing ignore
				case DrawMode.None:
					return false;

				case DrawMode.Hotspot:
				{
					break;
				}

				case DrawMode.Incident:
				{
					break;
				}
					// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					// if the right mouse button was hit we're done with the poly
					if (e.Button == MouseButtons.Right)
					{
						stopPoly();

						return true;
					}

					// ignore if we've been dragged
					int dx = e.X - m_lastMousePoint.X;
					int dy = e.Y - m_lastMousePoint.Y;
					if(dx*dx+dy*dy > 3*3)
					{
						m_mouseDownPoint = Point.Empty;

						return false;
					}

					// if this is the first click then
					if (m_drawModeChange)
					{
						MouseDragMode = false;

                        m_drawAlt = DrawArgs.Camera.TerrainElevation + m_height;

						// create a new path list
						int pathId = this.getAvailablePathId();	
						m_currPathLine = new TerrainPath(m_currDrawMode + "-" + pathId + ".wwb", 
							DrawArgs.CurrentWorldStatic, 
							0.0, 
							DrawArgs.Camera.Altitude + 100000, 
							m_currDrawMode + "-" + pathId + ".wwb",
							m_drawAlt, 
							m_color,
							DrawArgs.CurrentWorldStatic.TerrainAccessor);

						Add(m_currPathLine);

						// add point to the path line
						m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, m_drawAlt);

						Logger.Write(2, "DRAW", lat.Degrees, lon.Degrees, m_drawAlt, m_currPathLine.Name, "Starting Poly path " + m_currPathLine.Name);

						m_currPathLine.IsOn = true;
						this.IsOn = true;

						// save the first point
						m_firstLat = lat;
						m_firstLon = lon;
						m_firstAlt = m_drawAlt;

						// save last position
						m_lastLat = lat;
						m_lastLon = lon;

						m_drawModeChange = false;

						if (!m_drawLock)
							m_drawMode = DrawMode.None;
					}
					else
					{
						// add intermediate points if distance is great
						Angle dist = World.ApproxAngularDistance(m_lastLat, m_lastLon, lat, lon);
						if (dist.Degrees > 10)
						{
							for(int i=0; i < 10; i++)
							{
								float t = (float)i / 9;
								Angle iLat, iLon;

								WorldWind.World.IntermediateGCPoint(t, m_lastLat, m_lastLon, lat, lon,
									dist, out iLat, out iLon );

								m_currPathLine.Add((float) iLat.Degrees, (float) iLon.Degrees, m_drawAlt);
							}
						}

						// add point to the path line
						m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, m_drawAlt);

						Logger.Write(3, "DRAW", lat.Degrees, lon.Degrees, m_drawAlt, m_currPathLine.Name, "Added point to poly path " + m_currPathLine.Name);

						// save last position
						m_lastLat = lat;
						m_lastLon = lon;
					}

					break;
				}

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
					// save the last point
					m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, m_drawAlt);

					stopFreehand();

					break;
				}
			}

			return true;
		}
	
		/// <summary>
		/// Called when the mouse wheel is used.  Not implemented.
		/// </summary>
		/// <param name="e">Mouse Event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			// TODO:  Add JHU_DrawLayer.OnMouseWheel implementation
			return false;
		}
	
		#endregion

		#region UI Thread Methods

		public override void Render(WorldWind.DrawArgs drawArgs)
		{
			if(!isOn)
				return;

			if (!Initialized)
				return;

			if ((m_hotspotLayer != null) && (!m_hotspotLayer.Initialized))
				m_hotspotLayer.Initialize(drawArgs);

			// render all the child objects (paths)
			base.Render(drawArgs);

			switch (m_currDrawMode)
			{
				// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{

					Device device = drawArgs.device;
					device.RenderState.ZBufferEnable = false;
					device.TextureState[0].ColorOperation = TextureOperation.Disable;
					device.VertexFormat = CustomVertex.PositionColored.Format;

					// Draw the measure line + ends
					Vector3 referenceCenter = new Vector3(
						(float)drawArgs.WorldCamera.ReferenceCenter.X,
						(float)drawArgs.WorldCamera.ReferenceCenter.Y,
						(float)drawArgs.WorldCamera.ReferenceCenter.Z);

					drawArgs.device.Transform.World = Matrix.Translation(
						-referenceCenter
						);

					device.DrawUserPrimitives(PrimitiveType.LineStrip, m_guideLine.Length-1, m_guideLine);

					drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;

					device.RenderState.ZBufferEnable = true;
					break;
				}
			}
		}
	
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
            return base.PerformSelectionAction(drawArgs);
		}
	
		public override void Update(DrawArgs drawArgs)
		{
			// if we aren't initialized go ahead and init
			if(!isInitialized)
				Initialize(drawArgs);

			base.Update(drawArgs);
		}
	
		public override void Initialize(DrawArgs drawArgs)
		{
			base.Initialize(drawArgs);

			isInitialized = true;
		}

		#endregion

		#region Communication Methods
		public void SendPath(DrawMode mode, LineFeature path)
		{
			// SavePathAsXML(mode, path);
			// Send somewhere
		}

		public void SendHotspot(Hotspot hotspot)
		{
			// SaveHotspotAsXML(hotspot)
			// Send somewhere
		}

		#endregion

		#region XML Support Methods

		public string SavePathAsXML(DrawMode type, LineFeature path)
		{
			string result; 
			try
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter xw = new XmlTextWriter(sw);
				xw.Formatting = Formatting.Indented;

				CollabObj ds = new CollabObj();

				CollabObj.InfoRow ir = ds.Info.NewInfoRow();
				ir.Name = path.Name;
				ir.Type = type.ToString();
				ir.Color = path.LineColor.Name;
				ir.LineWidth = path.LineWidth;
				ir.MinAlt = path.MinimumDisplayAltitude;
				ir.MaxAlt = path.MaximumDisplayAltitude;
				ir.NumPoints = path.Points.Length;
				ds.Info.AddInfoRow(ir);

				CollabObj.PointsRow pr;

				foreach (Point3d item in path.Points)
				{
					pr = ds.Points.NewPointsRow();

					pr.Lat = item.X;
					pr.Lon = item.Y;
					pr.Alt = item.Z;
					ds.Points.AddPointsRow(pr);
				}

				ds.AcceptChanges();
				ds.WriteXml(xw);

				result = sw.ToString();

				sw.Close();
			}
			catch(Exception ex)
			{
				System.Console.WriteLine(ex.Message.ToString());			
				result = string.Empty;
			}

			return result;
		}

		public string SaveHotspotAsXML(Hotspot hotspot)
		{
			string result; 
			try
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter xw = new XmlTextWriter(sw);
				xw.Formatting = Formatting.Indented;

				CollabObj ds = new CollabObj();

				CollabObj.InfoRow ir = ds.Info.NewInfoRow();
				ir.Name = hotspot.Name;
				ir.Type = DrawMode.Hotspot.ToString();
				ir.Icon = "Hotspot";
				ir.Desc = hotspot.Description;
				ir.Affiliation = ""; // hotspot.Affiliation.ToString();
				ir.Dimension = ""; // hotspot.BattleDimension.ToString();
				ir.Color = m_color.Name;
				ir.NumPoints = 1;
                ir.URL = hotspot.ClickableActionURL;
				ds.Info.AddInfoRow(ir);

				CollabObj.PointsRow pr = ds.Points.NewPointsRow();
				pr.Lat = hotspot.Latitude;
				pr.Lon = hotspot.Longitude;
				pr.Alt = hotspot.Altitude;
				ds.Points.AddPointsRow(pr);

                CollabObj.CameraRow cr = ds.Camera.NewCameraRow();
                cr.Altitude = m_hotspot.OnClickZoomAltitude;
                cr.Heading = m_hotspot.OnClickZoomHeading;
                cr.Tilt = m_hotspot.OnClickZoomTilt;

				ds.AcceptChanges();
				ds.WriteXml(xw);
				result = sw.ToString();

				sw.Close();
			}
			catch(Exception ex)
			{
				System.Console.WriteLine(ex.Message.ToString());			
				result = string.Empty;
			}

			return result;
		}

		public string SaveTrackAsXML(WorldWind.Renderable.Icon track)
		{
			string result; 
			try
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter xw = new XmlTextWriter(sw);
				xw.Formatting = Formatting.Indented;


				CollabObj ds = new CollabObj();

				CollabObj.InfoRow ir = ds.Info.NewInfoRow();
				ir.Name = track.Name;
				ir.Type = DrawMode.Incident.ToString();
				ir.Icon = "";  // generated from bitmap
				ir.Desc = track.Description;
				ir.Affiliation = ""; // track.Affiliation.ToString();
				ir.Dimension = ""; // track.BattleDimension.ToString();
				ir.Color = m_color.Name;
				ir.NumPoints = 1;
				ds.Info.AddInfoRow(ir);

				CollabObj.PointsRow pr = ds.Points.NewPointsRow();
				pr.Lat = track.Latitude;
				pr.Lon = track.Longitude;
				pr.Alt = track.Altitude;
				ds.Points.AddPointsRow(pr);

				ds.AcceptChanges();
				ds.WriteXml(xw);
				result = sw.ToString();

				sw.Close();
			}
			catch(Exception ex)
			{
				System.Console.WriteLine(ex.Message.ToString());			
				result = string.Empty;
			}

			return result;
		}


		public LineFeature LoadLineFromXML (CollabObj ds)
		{
			string name; 
			string type;
			string iconName; 
			string description; 
			string affiliation; 
			string dimension; 
			string color; 
			double lineWidth;
			double minAlt;
			double maxAlt;
			int numPoints; 
			
			CollabObj.InfoRow ir = (CollabObj.InfoRow) ds.Info.Rows[0];

			name = ir.Name;
			type = ir.Type;

			iconName = ir.Icon;

			description = ir.Desc;
			affiliation = ir.Affiliation;
			dimension = ir.Dimension;
			color = ir.Color;
			lineWidth = ir.LineWidth;
			numPoints = ir.NumPoints;

			minAlt = ir.MinAlt;
			maxAlt = ir.MaxAlt;

			if (maxAlt <= 0)
				maxAlt = double.MaxValue;

			CollabObj.PointsRow pr; 

			Point3d[] points = new Point3d[numPoints];

			for (int i=0; i < numPoints; i++)
			{
				pr = (CollabObj.PointsRow) ds.Points.Rows[i];

				points[i] = new Point3d(pr.Lon, pr.Lat, pr.Alt);
			}

			int pathId = this.getAvailablePathId();

			LineFeature path = new LineFeature(type + "-" + pathId + ".wwb", DrawArgs.CurrentWorldStatic, points, Color.FromName(color));
			path.LineWidth = (float) lineWidth;

			return path;
		}


		public Hotspot LoadHotspotFromXML (CollabObj ds)
		{
			string name; 
			string type;
			string iconName; 
			string description; 
			string affiliation; 
			string dimension; 
			string color; 
			int numPoints; 
			float lat; 
			float lon; 
			float alt; 

			CollabObj.InfoRow ir = (CollabObj.InfoRow) ds.Info.Rows[0];

			name = ir.Name;
			type = ir.Type;

			iconName = ir.Icon;

			description = ir.Desc;
			affiliation = ir.Affiliation;
			dimension = ir.Dimension;
			color = ir.Color;
			numPoints = ir.NumPoints;

			CollabObj.PointsRow pr = (CollabObj.PointsRow) ds.Points.Rows[0];

			lat = (float) pr.Lat;
			lon = (float) pr.Lon;
			alt = (float) pr.Alt;

			int id = this.getAvailablePathId();

			Hotspot hotspot = new Hotspot("Hotspot-" + id.ToString(),
				lat,
				lon,
				alt,
				Color.FromName(color),
				ir.URL,
				description);

            CollabObj.CameraRow cr = (CollabObj.CameraRow) ds.Camera.Rows[0];

             m_hotspot.OnClickZoomAltitude = cr.Altitude;
             m_hotspot.OnClickZoomHeading = cr.Heading;
             m_hotspot.OnClickZoomTilt = cr.Tilt;

			return hotspot;
		}
//
//
//		public CS_TrackData LoadTrackFromXML (CollabObj ds)
//		{
//			string name; 
//			string type;
//			string iconName; 
//			string description; 
//			string affiliation; 
//			string dimension; 
//			string color; 
//			int numPoints; 
//			float lat; 
//			float lon; 
//			float alt; 
//
//			CollabObj.InfoRow ir = (CollabObj.InfoRow) ds.Info.Rows[0];
//
//			name = ir.Name;
//			type = ir.Type;
//
//			iconName = ir.Icon;
//
//			description = ir.Desc;
//			affiliation = ir.Affiliation;
//			dimension = ir.Dimension;
//			color = ir.Color;
//			numPoints = ir.NumPoints;
//
//			CollabObj.PointsRow pr = (CollabObj.PointsRow) ds.Points.Rows[0];
//
//			lat = (float) pr.Lat;
//			lon = (float) pr.Lon;
//			alt = (float) pr.Alt;
//
//			CS_TrackData track = new CS_TrackData("CollabSpace",
//				name,
//				name,
//				iconName,
//				(JHU_Enums.Affiliations) Enum.Parse(typeof(JHU_Enums.Affiliations), affiliation, true),
//				(JHU_Enums.BattleDimensions) Enum.Parse(typeof(JHU_Enums.BattleDimensions), dimension, true),
//				description,
//				description,
//				lat,
//				lon,
//				alt,
//				0.0,
//				0.0,
//				"",
//				DateTime.UtcNow,
//				DateTime.UtcNow,
//				"");
//
//			return track;
//		}

		#endregion

		#region Context Menu Methods

		public override void BuildContextMenu(ContextMenu menu)
		{
			// initialize context menu
			MenuItem topMenuItem = new MenuItem(Name);
			switch (DrawingMode)
			{
				case DrawMode.None:
					topMenuItem.Text = Name + ": None";
					break;
				case DrawMode.Hotspot:
					topMenuItem.Text = Name + ": Hotspot";
					break;
				case DrawMode.Incident:
					topMenuItem.Text = Name + ": Incident";
					break;
				case DrawMode.PostIt:
					topMenuItem.Text = Name + ": PostIt";
					break;
				case DrawMode.Polyline:
					topMenuItem.Text = Name + ": Polyline";
					break;
				case DrawMode.Polygon:
					topMenuItem.Text = Name + ": Polygon";
					break;
				case DrawMode.Freehand:
					topMenuItem.Text = Name + ": Freehand";
					break;
				case DrawMode.Disabled:
					topMenuItem.Text =  Name + ": Disabled (Read Only)";
					break;
				default:
					break;
			}

			if (m_drawLock)
			{
				topMenuItem.Text += " (Locked)";
			}

			MenuItem stopMenuItem = new MenuItem("Stop Drawing", new EventHandler(whiteboardStopMenuItem_Click));
			if (DrawingMode == DrawMode.None)
			{
				stopMenuItem.Enabled = false;
			}

			MenuItem lockMenuItem = new MenuItem("Enable Draw Lock", new EventHandler(whiteboardLockMenuItem_Click));
			if (DrawLock)
			{
				lockMenuItem.Text = "Disable Draw Lock";
			}

			MenuItem drawMenuItem = new MenuItem("Draw modes...");

			MenuItem hotspotMenuItem = new MenuItem("Hotspot", new EventHandler(whiteboardHotspotMenuItem_Click));
			drawMenuItem.MenuItems.Add(hotspotMenuItem);

			MenuItem incidentMenuItem = new MenuItem("Incident", new EventHandler(whiteboardIncidentMenuItem_Click));
			drawMenuItem.MenuItems.Add(incidentMenuItem);

			MenuItem postItMenuItem = new MenuItem("PostIt", new EventHandler(whiteboardPostItMenuItem_Click));
			drawMenuItem.MenuItems.Add(postItMenuItem);
			postItMenuItem.Enabled = false;

			MenuItem polygonMenuItem = new MenuItem("Polygon", new EventHandler(whiteboardPolygonMenuItem_Click));
			drawMenuItem.MenuItems.Add(polygonMenuItem);

			MenuItem polylineMenuItem = new MenuItem("Polyline", new EventHandler(whiteboardPolyLineMenuItem_Click));
			drawMenuItem.MenuItems.Add(polylineMenuItem);
	
			MenuItem freehandMenuItem = new MenuItem("Freehand", new EventHandler(whiteboardFreehandMenuItem_Click));
			drawMenuItem.MenuItems.Add(freehandMenuItem);

			MenuItem colorMenuItem = new MenuItem("Draw colors... "+ m_color.ToString());

			MenuItem redMenuItem = new MenuItem("Red", new EventHandler(whiteboardRedMenuItem_Click));
			colorMenuItem.MenuItems.Add(redMenuItem);

			MenuItem blueMenuItem = new MenuItem("Blue", new EventHandler(whiteboardBlueMenuItem_Click));
			colorMenuItem.MenuItems.Add(blueMenuItem);

			MenuItem greenMenuItem = new MenuItem("Green", new EventHandler(whiteboardGreenMenuItem_Click));
			colorMenuItem.MenuItems.Add(greenMenuItem);

			MenuItem yellowMenuItem = new MenuItem("Yellow", new EventHandler(whiteboardYellowMenuItem_Click));
			colorMenuItem.MenuItems.Add(yellowMenuItem);

			MenuItem orangeMenuItem = new MenuItem("Orange", new EventHandler(whiteboardOrangeMenuItem_Click));
			colorMenuItem.MenuItems.Add(orangeMenuItem);

			MenuItem whiteMenuItem = new MenuItem("White", new EventHandler(whiteboardWhiteMenuItem_Click));
			colorMenuItem.MenuItems.Add(whiteMenuItem);

			MenuItem turquoiseMenuItem = new MenuItem("Turquoise", new EventHandler(whiteboardTurquoiseMenuItem_Click));
			colorMenuItem.MenuItems.Add(turquoiseMenuItem);

            MenuItem clearMenuItem = new MenuItem("Clear WhiteBoard", new EventHandler(whiteboardClearMenuItem_Click));

            MenuItem shareMenuItem = new MenuItem("Share WhiteBoard", new EventHandler(whiteboardShareMenuItem_Click));

            MenuItem getSharedMenuItem = new MenuItem("Get Shared WhiteBoard", new EventHandler(whiteboardGetSharedMenuItem_Click));

			if (!Writeable)
			{
				topMenuItem.Enabled = false;
				lockMenuItem.Enabled = false;
				stopMenuItem.Enabled = false;
				drawMenuItem.Enabled = false;
				hotspotMenuItem.Enabled = false;
				incidentMenuItem.Enabled = false;
				polygonMenuItem.Enabled = false;
				polylineMenuItem.Enabled = false;
				freehandMenuItem.Enabled = false;
				colorMenuItem.Enabled = false;
			}

			menu.MenuItems.Add(topMenuItem);
			menu.MenuItems.Add(stopMenuItem);
			menu.MenuItems.Add(lockMenuItem);
			menu.MenuItems.Add(drawMenuItem);
			menu.MenuItems.Add(colorMenuItem);
            menu.MenuItems.Add(clearMenuItem);
            menu.MenuItems.Add(shareMenuItem);
            menu.MenuItems.Add(getSharedMenuItem);
		}

        void whiteboardClearMenuItem_Click(object sender, EventArgs s)
        {
            StopDrawing();
            this.RemoveAll();
            Logger.Write(1, "DRAW", "", "Whiteboard Clear Menu Item Clicked");
        }

        void whiteboardShareMenuItem_Click(object sender, EventArgs s)
        {
//            m_whiteboardDialog.ShowDialog();
//
//            if (m_whiteboardDialog.DialogResult == DialogResult.OK)
//            {
//                ShareLatestShapes(m_whiteboardDialog.OverlayName);
//            }

            Logger.Write(1, "DRAW", "", "Whiteboard Share Menu Item Clicked");
        }

        void whiteboardGetSharedMenuItem_Click(object sender, EventArgs s)
        {
//            m_getWhiteboardDialog.ShowDialog();
//
//            if (m_getWhiteboardDialog.DialogResult == DialogResult.OK)
//            {
//                GetSharedOverlay(m_getWhiteboardDialog.OverlayName);
//            }

            Logger.Write(1, "DRAW", "", "Whiteboard Get Shared Menu Item Clicked");
        }

		void whiteboardStopMenuItem_Click(object sender, EventArgs s)
		{
			DrawingMode = DrawMode.None;
			Logger.Write(1, "DRAW", "", "Whiteboard Stop Menu Item Clicked");
		}

		void whiteboardLockMenuItem_Click(object sender, EventArgs s)
		{
			// Toggle the draw lock
			DrawLock = !DrawLock;
			Logger.Write(1, "DRAW", "", "Whiteboard Drawlock Menu Item Clicked");
		}

		void whiteboardPolyLineMenuItem_Click(object sender, EventArgs s)
		{
			DrawingMode = DrawMode.Polyline;
			Logger.Write(1, "DRAW", "", "Whiteboard Polyline Menu Item Clicked");
		}

		void whiteboardHotspotMenuItem_Click(object sender, EventArgs s)
		{
			DrawingMode = DrawMode.Hotspot;
			Logger.Write(1, "DRAW", "", "Whiteboard Hotspot Menu Item Clicked");
		}
		
		void whiteboardIncidentMenuItem_Click(object sender, EventArgs s)
		{
			DrawingMode = DrawMode.Incident;
			Logger.Write(1, "DRAW", "", "Whiteboard Incident Menu Item Clicked");
		}

		void whiteboardPostItMenuItem_Click(object sender, EventArgs s)
		{
			DrawingMode = DrawMode.PostIt;
			Logger.Write(1, "DRAW", "", "Whiteboard PostIt Menu Item Clicked");

		}

		void whiteboardPolygonMenuItem_Click(object sender, EventArgs s)
		{
			DrawingMode = DrawMode.Polygon;
			Logger.Write(1, "DRAW", "", "Whiteboard Polygon Menu Item Clicked");
		}

		void whiteboardFreehandMenuItem_Click(object sender, EventArgs s)
		{
			DrawingMode = DrawMode.Freehand;
			Logger.Write(1, "DRAW", "", "Whiteboard Freehand Menu Item Clicked");
		}

		void whiteboardRedMenuItem_Click(object sender, EventArgs s)
		{
			m_color = System.Drawing.Color.Red;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Red Item Clicked");
		}

		void whiteboardBlueMenuItem_Click(object sender, EventArgs s)
		{
			m_color = System.Drawing.Color.Blue;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Blue Item Clicked");
		}

		void whiteboardGreenMenuItem_Click(object sender, EventArgs s)
		{
			m_color = System.Drawing.Color.LightGreen;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Green Item Clicked");
		}

		void whiteboardYellowMenuItem_Click(object sender, EventArgs s)
		{
			m_color = System.Drawing.Color.Yellow;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Yellow Item Clicked");
		}
	
		void whiteboardOrangeMenuItem_Click(object sender, EventArgs s)
		{
			m_color = System.Drawing.Color.Orange;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Orange Item Clicked");
		}

		void whiteboardWhiteMenuItem_Click(object sender, EventArgs s)
		{
			m_color = System.Drawing.Color.White;
			Logger.Write(1, "DRAW", "", "Whiteboard Color White Item Clicked");
		}

		void whiteboardTurquoiseMenuItem_Click(object sender, EventArgs s)
		{
			m_color = System.Drawing.Color.Turquoise;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Turquose Item Clicked");
		}

		#endregion

	}
}
