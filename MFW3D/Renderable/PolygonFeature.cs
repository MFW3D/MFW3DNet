using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using GeometryUtility;
using PolygonCuttingEar;

using Tao.OpenGl;
using Utility;

namespace MFW3D
{
	/// <summary>
	/// Creates 2D or 3D polygons.  ClampedToGround polygons are drawn as tiled images using ProjectedVectorRenderer.
	/// </summary>
	public class PolygonFeature : MFW3D.Renderable.RenderableObject
	{        
        CustomVertex.PositionNormalColored[] m_vertices = null;
		double m_distanceAboveSurface = 0;
		float m_verticalExaggeration = World.Settings.VerticalExaggeration;
		double m_minimumDisplayAltitude = 0;
		double m_maximumDisplayAltitude = double.MaxValue;
		System.Drawing.Color m_outlineColor = System.Drawing.Color.Black;
		bool m_outline = false;
        float m_outlineWidth = 1.0f;
		LineFeature[] m_lineFeature = null;
		AltitudeMode m_altitudeMode = AltitudeMode.ClampedToGround;
		public BoundingBox BoundingBox = null;
		bool m_extrude = false;
		bool m_fill = true;
        Vector3 m_localOrigin;
        bool m_extrudeUpwards;
        double m_extrudeHeight=1;
        bool m_extrudeToGround = false;

        protected System.Drawing.Color m_polygonColor = System.Drawing.Color.Yellow;
        protected LinearRing m_outerRing = null;
        protected LinearRing[] m_innerRings = null;

        /// <summary>
        /// Enables or disables depth buffering - disable if you are having terrain collision rendering issues.
        /// </summary>
        public bool ZBufferEnable
        {
            get { return m_ZBufferEnable; }
            set { m_ZBufferEnable = value; }
        }
        bool m_ZBufferEnable = true;

        /// <summary>
        /// Whether polygon should be extruded
        /// </summary>
        public bool Extrude
        {
            get { return m_extrude; }
            set
            { 
                m_extrude = value;
                if(m_vertices != null)
                    UpdateVertices();
            }
        }

        /// <summary>
        /// Whether extrusion should be upwards
        /// </summary>
        public bool ExtrudeUpwards
        {
            get { return m_extrudeUpwards; }
            set
            { 
                m_extrudeUpwards = value;
                if (m_vertices != null)
                    UpdateVertices();
            }
        }

        /// <summary>
        /// Distance to extrude
        /// </summary>
        public double ExtrudeHeight
        {
            get { return m_extrudeHeight; }
            set
            { 
                m_extrudeHeight = value;
                if (m_vertices != null)
                    UpdateVertices();
             }
        }

        /// <summary>
        /// Whether polygon should be extruded to the ground (completely overrides other extrusion options)
        /// </summary>
        public bool ExtrudeToGround
        {
            get { return m_extrudeToGround; }
            set { m_extrude = value;
                m_extrudeToGround = value;
                if(m_vertices != null)
                    UpdateVertices();
            }
        }


        public float OutlineWidth
        {
            get { return m_outlineWidth; }
            set { m_outlineWidth = value; }
        }

		public bool Fill
		{
			get{ return m_fill; }
			set
			{
				m_fill = value;
				if(m_vertices != null)
					UpdateVertices();
			}
		}

		public AltitudeMode AltitudeMode
		{
			get{ return m_altitudeMode; }
			set{ m_altitudeMode = value; }
		}

		public System.Drawing.Color OutlineColor
		{
			get{ return m_outlineColor; }
			set
			{
				m_outlineColor = value;
				if(m_vertices != null)
				{
					UpdateVertices();
				}
			}
		}

		public bool Outline
		{
			get{ return m_outline; }
			set
			{
				m_outline = value;
				if(m_vertices != null)
				{
					UpdateVertices();
				}
			}
		}

		public double DistanceAboveSurface
		{
			get{ return m_distanceAboveSurface; }
			set
			{ 
				m_distanceAboveSurface = value; 
				if(m_vertices != null)
					UpdateVertices();
			}
		}

		public double MinimumDisplayAltitude
		{
			get{ return m_minimumDisplayAltitude; }
			set{ m_minimumDisplayAltitude = value; }
		}

		public double MaximumDisplayAltitude
		{
			get{ return m_maximumDisplayAltitude; }
			set{ m_maximumDisplayAltitude = value; }
		}

		public override byte Opacity
		{
			get
			{
				return base.Opacity;
			}
			set
			{
				base.Opacity = value;
				if(m_vertices != null)
					UpdateVertices();
			}
		}

        /// <summary>
        /// Allow runtime updates of outerring
        /// </summary>
        public LinearRing OuterRing
        {
            get { return m_outerRing; }
            set{
                //update out ring
                m_outerRing = value;
                //update bounding box
                CalcBoundingBox();
                //update tessalated polygon
                UpdateVertices();
            }
            
        }
        // Public accessor to the geographic coordinates of the bounding box
        public GeographicBoundingBox GeographicBoundingBox
        {
            get { return m_geographicBoundingBox; }
        }

        /// <summary>
        /// Polygon Feature Constructor
        /// </summary>
        /// <param name="name">Name of the layer</param>
        /// <param name="parentWorld">Base world</param>
        /// <param name="outerRing">Polygon's outer boundary</param>
        /// <param name="innerRings">Inner Hole's</param>
        /// <param name="polygonColor">Colour of the rendered object</param>
		public PolygonFeature(
			string name, 
			World parentWorld, 
			LinearRing outerRing,
			LinearRing[] innerRings,
			System.Drawing.Color polygonColor) : base(name, parentWorld)
		{
			RenderPriority = MFW3D.Renderable.RenderPriority.LinePaths;
			m_outerRing = outerRing;
			m_innerRings = innerRings;
			m_polygonColor = polygonColor;
            m_world = parentWorld;

            CalcBoundingBox();
		}


        /// <summary>
        /// Ring Polygon Feature Constructor
        /// </summary>
        /// <param name="name">Name of the layer</param>
        /// <param name="parentWorld">Base world</param>
        /// <param name="lat">Lat Center of Ring</param>
        /// <param name="lon">Lon Center of Ring</param>
        /// <param name="alt">Altitude in meters</param>
        /// <param name="radius">Radius of Ring in meters</param>
        /// <param name="numPoints">Number of points desired</param>
        /// <param name="polygonColor">Colour of the rendered object</param>
        public PolygonFeature(
            string name,
            World parentWorld,
            Angle lat,
            Angle lon,
            double alt,
            double radius,
            int numPoints,
            System.Drawing.Color polygonColor)
            : base(name, parentWorld)
        {
            RenderPriority = MFW3D.Renderable.RenderPriority.LinePaths;
            m_polygonColor = polygonColor;
            m_world = parentWorld;

            UpdateCircle(lat, lon, alt, radius, numPoints);
        }

        /// <summary>
        /// Used to create or update this polygon as a circle.  Typically called if a new radius is desired.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="radius"></param>
        /// <param name="numPoints"></param>
        /// <param name="polygonColor"></param>
        public virtual void UpdateCircle(
            Angle lat,
            Angle lon,
            double alt,
            double radius,
            int numPoints)
        {
            // Original circle code by ink_polaroid from this thread:
            // http://bbs.keyhole.com/ubb/showflat.php/Cat/0/Number/23634/page/vc/fpart/all/vc/1
            // http://dev.bt23.org/keyhole/circlegen/output.phps

            // build the outer ring
            LinearRing outerRing = new LinearRing();
            Point3d[] points = new Point3d[numPoints];

            double dist_rad = radius / m_world.EquatorialRadius;
            Angle lat_rad = Angle.FromDegrees(0);
            Angle lon_rad = Angle.FromDegrees(0);
            double dlon_rad = 0;
            double d_alt = alt;
            double numDegrees = 0;
            numDegrees = 360 / numPoints;
            Angle curr = Angle.FromDegrees(0);
            for (int i = 0; i < numPoints; i++)
            {
                curr.Degrees = (double)i * numDegrees;

                lat_rad.Radians = Math.Asin(Math.Sin(lat.Radians) * Math.Cos(dist_rad) + Math.Cos(lat.Radians) * Math.Sin(dist_rad) * Math.Cos(curr.Radians));
                dlon_rad = Math.Atan2(Math.Sin(curr.Radians) * Math.Sin(dist_rad) * Math.Cos(lat.Radians), Math.Cos(dist_rad) - Math.Sin(lat.Radians) * Math.Sin(lat_rad.Radians));
                lon_rad.Radians = ((lon.Radians + dlon_rad + Math.PI) % (2 * Math.PI)) - Math.PI;

                //This algorithm is limited to distances such that dlon <pi/2
                points[i] = new Point3d(lon_rad.Degrees, lat_rad.Degrees, d_alt);
            }

            outerRing.Points = points;

            m_outerRing = outerRing;
            m_innerRings = null;

            CalcBoundingBox();

            isInitialized = false;
        }

		Polygon m_polygon = null;
		GeographicBoundingBox m_geographicBoundingBox = null;

        /// <summary>
        /// Method to update polygon bounding box
        /// </summary>
        protected void CalcBoundingBox()
        {
            double minY = double.MaxValue;
			double maxY = double.MinValue;
			double minX = double.MaxValue;
			double maxX = double.MinValue;
			double minZ = double.MaxValue;
			double maxZ = double.MinValue;

            if (m_outerRing != null && m_outerRing.Points.Length > 0)
            {
                for (int i = 0; i < m_outerRing.Points.Length; i++)
                {
                    if (m_outerRing.Points[i].X < minX)
                        minX = m_outerRing.Points[i].X;
                    if (m_outerRing.Points[i].X > maxX)
                        maxX = m_outerRing.Points[i].X;

                    if (m_outerRing.Points[i].Y < minY)
                        minY = m_outerRing.Points[i].Y;
                    if (m_outerRing.Points[i].Y > maxY)
                        maxY = m_outerRing.Points[i].Y;

                    if (m_outerRing.Points[i].Z < minZ)
                        minZ = m_outerRing.Points[i].Z;
                    if (m_outerRing.Points[i].Z > maxZ)
                        maxZ = m_outerRing.Points[i].Z;
                }

                // set a uniform Z for all the points
                for (int i = 0; i < m_outerRing.Points.Length; i++)
                {
                    if (m_outerRing.Points[i].Z != maxZ)
                        m_outerRing.Points[i].Z = maxZ;
                }

                if (m_innerRings != null && m_innerRings.Length > 0)
                {
                    for (int n = 0; n < m_innerRings.Length; n++)
                    {
                        for (int i = 0; i < m_innerRings[n].Points.Length; i++)
                        {
                            if (m_innerRings[n].Points[i].Z != maxZ)
                                m_innerRings[n].Points[i].Z = maxZ;
                        }
                    }
                }
            }

			m_geographicBoundingBox = new GeographicBoundingBox(maxY, minY, minX, maxX, minZ, maxZ);

			minZ += m_world.EquatorialRadius;
			maxZ += m_world.EquatorialRadius;

			BoundingBox = new BoundingBox(
				(float)minY, (float)maxY, (float)minX, (float)maxX, (float)minZ, (float)maxZ);
        }

        /// <summary>
        /// Intialize Polygon with Tessalation
        /// </summary>
        /// <param name="drawArgs"></param>
		public override void Initialize(DrawArgs drawArgs)
		{
			UpdateVertices();
			isInitialized = true;
		}

		System.Collections.ArrayList primList = new ArrayList();
		System.Collections.ArrayList tessList = new ArrayList();
		PrimitiveType m_primitiveType = PrimitiveType.PointList;

		private void f(System.IntPtr vertexData) 
		{
			try
			{
				double[] v = new double[3];
				System.Runtime.InteropServices.Marshal.Copy(vertexData, v, 0, 3);

				Point3d p = new Point3d(v[0], v[1], 0);
				tessList.Add(p);
			} 
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		} 

		private void e() 
		{
			finishTesselation((Point3d[])tessList.ToArray(typeof(Point3d)));
		}

		private void b(int which) 
		{
			tessList.Clear();
			switch(which)
			{
				case 4:
					m_primitiveType = PrimitiveType.TriangleList;
					break;
				case 5:
					m_primitiveType = PrimitiveType.TriangleStrip;
					break;
				case 6:
					m_primitiveType = PrimitiveType.TriangleFan;
					break;
			}
			primTypes.Add(m_primitiveType);
		}

		private void r(int which) 
		{
			Log.Write(Log.Levels.Error, "error: " + which.ToString());
		}

		private void getTessellation()
		{
			try
			{
			
				primList.Clear();
				primTypes.Clear();

				System.Collections.ArrayList pointList = new ArrayList();
				for(int i = 0; i < m_outerRing.Points.Length; i++)
				{
					double[] p = new double[3];
					p[0] = m_outerRing.Points[i].X;
					p[1] = m_outerRing.Points[i].Y;
					p[2] = m_outerRing.Points[i].Z;
					
					pointList.Add(p);
				}

				Glu.GLUtesselator tess = Glu.gluNewTess();
				Glu.gluTessCallback(tess, Glu.GLU_TESS_BEGIN, new Glu.TessBeginCallback(b));
				Glu.gluTessCallback(tess, Glu.GLU_TESS_END, new Glu.TessEndCallback(e));

				Glu.gluTessCallback(tess, Glu.GLU_TESS_ERROR, new Glu.TessErrorCallback(r));
				Glu.gluTessCallback(tess, Glu.GLU_TESS_VERTEX, new Glu.TessVertexCallback(f));

				Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
				Glu.gluTessBeginContour(tess);
		
				for(int i = 0; i < pointList.Count - 1; i++)
				{
					double[] p = (double[])pointList[i];
					Glu.gluTessVertex(tess, p, p);
				}
				Glu.gluTessEndContour(tess);
				
				if(m_innerRings != null && m_innerRings.Length > 0)
				{
					for(int i = 0; i < m_innerRings.Length; i++)
					{
						Glu.gluTessBeginContour(tess);
						for(int j = m_innerRings[i].Points.Length - 1; j >= 0; j--)
						{
							double[] p = new double[3];
							p[0] = m_innerRings[i].Points[j].X;
							p[1] = m_innerRings[i].Points[j].Y;
							p[2] = m_innerRings[i].Points[j].Z;
							Glu.gluTessVertex(tess, p, p);
						}
						Glu.gluTessEndContour(tess);
					}
				}
				
				Glu.gluTessEndPolygon(tess);
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		ArrayList primTypes = new ArrayList();

		private void finishTesselation(Point3d[] tesselatorList)
		{
            int alpha = (int)(((double)m_polygonColor.A/255 * (double)this.Opacity/255)*255);
			int polygonColor = System.Drawing.Color.FromArgb(alpha, m_polygonColor.R, m_polygonColor.G, m_polygonColor.B).ToArgb();
			CustomVertex.PositionNormalColored[] vertices = new CustomVertex.PositionNormalColored[tesselatorList.Length];

            // precalculate feature center (at zero elev)
            Point3d center = new Point3d(0, 0, 0);
            for (int i = 0; i < m_outerRing.Points.Length; i++)
            {
                center += MathEngine.SphericalToCartesianD(
                    Angle.FromDegrees(m_outerRing.Points[i].Y),
                    Angle.FromDegrees(m_outerRing.Points[i].X),
                    World.EquatorialRadius 
                    );
            }
            center = center * (1.0 / m_outerRing.Points.Length);
            // round off to nearest 10^5.
            
            center.X = ((int)(center.X / 10000.0)) * 10000.0;
            center.Y = ((int)(center.Y / 10000.0)) * 10000.0;
            center.Z = ((int)(center.Z / 10000.0)) * 10000.0;
            
            m_localOrigin = center.Vector3;

			for(int i = 0; i < vertices.Length; i++)
			{
                Point3d sphericalPoint = tesselatorList[i];
                //System.Console.WriteLine("Point" + sphericalPoint.X+" "+sphericalPoint.Y+" "+sphericalPoint.Z);
					
				double terrainHeight = 0;
				if(m_altitudeMode == AltitudeMode.RelativeToGround)
				{
					if(World.TerrainAccessor != null)
					{
						terrainHeight = World.TerrainAccessor.GetElevationAt(
							sphericalPoint.Y,
							sphericalPoint.X,
							(100.0 / DrawArgs.Camera.ViewRange.Degrees)
							);
					}
				}

                Point3d xyzPoint = MathEngine.SphericalToCartesianD(
                    Angle.FromDegrees(sphericalPoint.Y),
                    Angle.FromDegrees(sphericalPoint.X),
                    World.EquatorialRadius 
                    + m_verticalExaggeration * 
                    (m_geographicBoundingBox.MaximumAltitude 
                    + m_distanceAboveSurface + terrainHeight));

                //Vector3 xyzVector = (xyzPoint + center).Vector3;
                
				vertices[i].Color = polygonColor;
				vertices[i].X = (float)(xyzPoint.X - center.X);
                vertices[i].Y = (float)(xyzPoint.Y - center.Y);
                vertices[i].Z = (float)(xyzPoint.Z - center.Z);
                vertices[i].Nx = 0;
                vertices[i].Ny = 0;
                vertices[i].Nz = 0;
			}

			primList.Add(vertices);

            ComputeNormals((PrimitiveType) primTypes[primList.Count - 1], vertices);
		}

        private void ComputeNormals(PrimitiveType primType, CustomVertex.PositionNormalColored[] vertices)
        {
            // normal vector computation: 
            // triangle list: (0 1 2) (3 4 5) (6 7 8) ... (0+3n 1+3n 2+3n)
            // triangle strip: (0 1 2) (1 3 2) (2 3 4) (3 5 4) ... (0+n 1+n 2+n)
            // triangle fan: (0 1 2) (0 2 3) (0 3 4) ... (0 1+n 2+n)
            // to handle it all at once, we use two factors for n: f0 for the first and f12 for the other two
            // notice how triangle strips reverse CW/CCW with each triangle!

            int triCount, f0, f12;
            bool flipflop = false;
            switch(primType)
            {
                case PrimitiveType.TriangleFan:
                    triCount = vertices.Length - 2;
                    f0 = 0;
                    f12 = 1;
                    break;
                case PrimitiveType.TriangleList:
                    triCount = vertices.Length / 3;
                    f0 = f12 = 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    triCount = vertices.Length - 2;
                    f0 = f12 = 1;
                    flipflop = true;
                    break;
                default:
                    // cannot calculate normals for non-polygon features
                    return;
            }

            for (int i = 0; i < triCount; i++)
            {
                // local triangle
                Vector3 a = vertices[0 + i * f0].Position;
                Vector3 b = vertices[1 + i * f12].Position;
                Vector3 c = vertices[2 + i * f12].Position;

                // normal vector
                Vector3 n = Vector3.Normalize(Vector3.Cross(b - a, c - a));
                if (flipflop && ((i & 1) == 1))
                    n = -n;
                vertices[0 + i * f0 ].Normal += n;
                vertices[1 + i * f12].Normal += n;
                vertices[2 + i * f12].Normal += n;
            }

            // all done, re-normalize (required for strips and fans only, but we do it in any case)
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
        }

        protected void UpdateVertices()
		{
			m_verticalExaggeration = World.Settings.VerticalExaggeration;

			if(m_altitudeMode == AltitudeMode.ClampedToGround)
			{
				if(m_polygon != null)
				{
					m_polygon.Remove = true;
					m_polygon = null;
				}

				m_polygon = new Polygon();
				m_polygon.outerBoundary = m_outerRing;
				m_polygon.innerBoundaries = m_innerRings;
				m_polygon.PolgonColor = m_polygonColor;
				m_polygon.Fill = m_fill;
				m_polygon.ParentRenderable = this;
                m_polygon.LineWidth = m_outlineWidth;
                m_polygon.Outline = m_outline;
                m_polygon.OutlineColor = m_outlineColor;
				this.World.ProjectedVectorRenderer.Add(m_polygon);

				if(m_vertices != null)
					m_vertices = null;
				
				if(m_lineFeature != null)
				{
                    for (int i = 0; i < m_lineFeature.Length; i++)
                        m_lineFeature[i].Dispose();

					m_lineFeature = null;
				}
				
				return;
			}

			getTessellation();
			
			if(m_extrude || m_outline)
			{
                if (m_lineFeature != null)
                {
                    for (int i = 0; i < m_lineFeature.Length; i++)
                    {
                        m_lineFeature[i].Dispose();
                    }
                    m_lineFeature = null;
                }

				m_lineFeature = new LineFeature[1 + (m_innerRings != null && m_innerRings.Length > 0 ? m_innerRings.Length : 0)];

				Point3d[] linePoints = new Point3d[m_outerRing.Points.Length + 1];
				for(int i = 0; i < m_outerRing.Points.Length; i++)
				{
					linePoints[i] = m_outerRing.Points[i];
				}

				linePoints[linePoints.Length - 1] = m_outerRing.Points[0];
				
				m_lineFeature[0] = new LineFeature(Name, World, linePoints, m_polygonColor);
				m_lineFeature[0].DistanceAboveSurface = m_distanceAboveSurface;
				m_lineFeature[0].MinimumDisplayAltitude = m_minimumDisplayAltitude;
				m_lineFeature[0].MaximumDisplayAltitude = m_maximumDisplayAltitude;
				m_lineFeature[0].AltitudeMode = AltitudeMode;
				m_lineFeature[0].Opacity = Opacity;
				m_lineFeature[0].Outline = m_outline;
				m_lineFeature[0].LineColor = m_outlineColor;
                m_lineFeature[0].Opacity = this.Opacity;
				m_lineFeature[0].Extrude = m_extrude;
                if (m_extrude || m_extrudeToGround)
                {
                    m_lineFeature[0].ExtrudeUpwards = m_extrudeUpwards;
                    m_lineFeature[0].ExtrudeHeight = m_extrudeHeight;
                    m_lineFeature[0].ExtrudeToGround = m_extrudeToGround;
                    m_lineFeature[0].PolygonColor = m_outlineColor;
                }

				if(m_innerRings != null && m_innerRings.Length > 0)
				{
					for(int i = 0; i < m_innerRings.Length; i++)
					{
						Point3d[] innerPoints = new Point3d[m_innerRings[i].Points.Length + 1];
						for(int j = 0; j < m_innerRings[i].Points.Length; j++)
						{
							innerPoints[j] = m_innerRings[i].Points[j];
						}

						innerPoints[innerPoints.Length - 1] = m_innerRings[i].Points[0];
				
						m_lineFeature[1 + i] = new LineFeature(Name, World, innerPoints, m_polygonColor);
						m_lineFeature[1 + i].DistanceAboveSurface = m_distanceAboveSurface;
						m_lineFeature[1 + i].MinimumDisplayAltitude = m_minimumDisplayAltitude;
						m_lineFeature[1 + i].MaximumDisplayAltitude = m_maximumDisplayAltitude;
						m_lineFeature[1 + i].AltitudeMode = AltitudeMode;
						m_lineFeature[1 + i].Opacity = Opacity;
						m_lineFeature[1 + i].Outline = m_outline;
						m_lineFeature[1 + i].LineColor = m_outlineColor;
                        m_lineFeature[1 + i].Opacity = this.Opacity;
						m_lineFeature[1 + i].Extrude = m_extrude;
                        if (m_extrude || m_extrudeToGround)
                        {
                            m_lineFeature[1+i].ExtrudeUpwards = m_extrudeUpwards;
                            m_lineFeature[1 + i].ExtrudeHeight = m_extrudeHeight;
                            m_lineFeature[1 + i].ExtrudeToGround = m_extrudeToGround;
                            m_lineFeature[1 + i].PolygonColor = m_outlineColor;
                        }
					}
				}
			}
			else
			{
				if(m_lineFeature != null && m_lineFeature.Length > 0)
				{
					for(int i = 0; i < m_lineFeature.Length; i++)
					{
						if(m_lineFeature[i] != null)
						{
							m_lineFeature[i].Dispose();
							m_lineFeature[i] = null;
						}	
					}
					m_lineFeature = null;
				}
			}
		}

		public override void Dispose()
		{
			if(m_polygon != null)
			{
				m_polygon.Remove = true;
				m_polygon = null;
			}

			if(m_lineFeature != null)
			{
				for(int i = 0; i < m_lineFeature.Length; i++)
				{
					if(m_lineFeature[i] != null)
						m_lineFeature[i].Dispose();
				}
				m_lineFeature = null;
			}
		}

        //store reference centre
        private Point3d referenceCentre = new Point3d();

		public override void Update(DrawArgs drawArgs)
		{
			try
			{
                referenceCentre = drawArgs.WorldCamera.ReferenceCenter;
                if ((m_extrude || m_outline) && m_lineFeature == null)
                    UpdateVertices();

                if (drawArgs.WorldCamera.Altitude >= m_minimumDisplayAltitude && drawArgs.WorldCamera.Altitude <= m_maximumDisplayAltitude)
				{
					if(!isInitialized)
						Initialize(drawArgs);

					if(m_verticalExaggeration != World.Settings.VerticalExaggeration || (m_lineFeature == null && m_extrude))
					{
						UpdateVertices();
					}

					if(m_lineFeature != null)
					{
						for(int i = 0; i < m_lineFeature.Length; i++)
						{
							if(m_lineFeature[i] != null)
								m_lineFeature[i].Update(drawArgs);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		public override void Render(DrawArgs drawArgs)
		{
            using (new DirectXProfilerEvent("PolygonFeature::Render"))
            {
                if (!isInitialized /*|| m_vertices == null*/ || drawArgs.WorldCamera.Altitude < m_minimumDisplayAltitude || drawArgs.WorldCamera.Altitude > m_maximumDisplayAltitude)
                {
                    return;
                }

                if (!drawArgs.WorldCamera.ViewFrustum.Intersects(BoundingBox))
                    return;

                // save state
                Cull currentCull = drawArgs.device.RenderState.CullMode;
                bool currentZBufferEnable = drawArgs.device.RenderState.ZBufferEnable;

                try
                {
                    drawArgs.device.RenderState.CullMode = Cull.None;
                    drawArgs.device.RenderState.ZBufferEnable = m_ZBufferEnable;

                    drawArgs.device.Transform.World = Matrix.Translation(
                        (float)-drawArgs.WorldCamera.ReferenceCenter.X + m_localOrigin.X,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Y + m_localOrigin.Y,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Z + m_localOrigin.Z
                        );

                    if (World.Settings.EnableSunShading)
                    {
                        Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                        Vector3 sunVector = new Vector3(
                            (float)sunPosition.X,
                            (float)sunPosition.Y,
                            (float)sunPosition.Z);

                        drawArgs.device.RenderState.Lighting = true;
                        Material material = new Material();
                        material.Diffuse = Color.White;
                        material.Ambient = Color.White;

                        drawArgs.device.Material = material;
                        drawArgs.device.RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                        drawArgs.device.RenderState.NormalizeNormals = true;
                        drawArgs.device.RenderState.AlphaBlendEnable = true;

                        drawArgs.device.Lights[0].Enabled = true;
                        drawArgs.device.Lights[0].Type = LightType.Directional;
                        drawArgs.device.Lights[0].Diffuse = Color.White;
                        drawArgs.device.Lights[0].Direction = sunVector;

                        drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                        drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
                        drawArgs.device.TextureState[0].ColorArgument2 = TextureArgument.TextureColor;
                    }
                    else
                    {
                        drawArgs.device.RenderState.Lighting = false;
                        drawArgs.device.RenderState.Ambient = World.Settings.StandardAmbientColor;
                    }

                    //if(m_vertices != null)
                    if (this.Fill)
                    {
                        if (primList.Count > 0)
                        {
                            drawArgs.device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                            drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
                            for (int i = 0; i < primList.Count; i++)
                            {
                                int vertexCount = 0;
                                PrimitiveType primType = (PrimitiveType)primTypes[i];
                                CustomVertex.PositionNormalColored[] vertices = (CustomVertex.PositionNormalColored[])primList[i];

                                if (primType == PrimitiveType.TriangleList)
                                    vertexCount = vertices.Length / 3;
                                else
                                    vertexCount = vertices.Length - 2;

                                drawArgs.device.DrawUserPrimitives(
                                    primType,//PrimitiveType.TriangleList, 
                                    vertexCount,
                                    vertices);
                            }
                        }
                    }

                    if (m_lineFeature != null)
                    {
                        for (int i = 0; i < m_lineFeature.Length; i++)
                        {
                            if (m_lineFeature[i] != null)
                                m_lineFeature[i].Render(drawArgs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
                finally
                {
                    // restore device state
                    drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
                    drawArgs.device.RenderState.CullMode = currentCull;
                    drawArgs.device.RenderState.ZBufferEnable = currentZBufferEnable;
                }
            }
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
            Point currentPosition = DrawArgs.LastMousePosition;
            Angle Latitude, Longitude;
            drawArgs.WorldCamera.PickingRayIntersection(
                currentPosition.X,
                currentPosition.Y,
                out Latitude,
                out Longitude);
            Point3d queryPoint = new Point3d(Longitude.Degrees, Latitude.Degrees, 0);

            if (!GeographicBoundingBox.Contains(queryPoint))
                return false;
            //TODO: Test holes/inner rings(if any) if point is in hole return false
            if (this.m_innerRings != null)
            {
                foreach (LinearRing ring in m_innerRings)
                {
                    bool isIn = pointInRing(ring.Points, queryPoint);
                    if (isIn)
                        return false;
                }
            }

            //TODO: Test outer ring
            return pointInRing(OuterRing.Points, queryPoint);
		}

        /// <summary>
        /// Utility method to check if point is in a simple polygon
        /// Uses Winding number/Jordan Curve Theorem
        /// http://en.wikipedia.org/wiki/Jordan_curve_theorem
        /// </summary>
        /// <param name="points">Polygon ring points</param>
        /// <param name="queryPoint">Test point</param>
        /// <returns>Location status</returns>
        private bool pointInRing(Point3d[] points, Point3d queryPoint)
        {
            bool isIn = false;
            int i, j = 0;
            for (i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                if ((((points[i].Y <= queryPoint.Y) && (queryPoint.Y < points[j].Y)) || ((points[j].Y <= queryPoint.Y)
                    && (queryPoint.Y < points[i].Y))) && (queryPoint.X < (points[j].X - points[i].X)
                    * (queryPoint.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    isIn = !isIn;
                }
            }

            return isIn;
        }
	}
}
