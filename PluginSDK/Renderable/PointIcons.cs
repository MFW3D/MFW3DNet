using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;

namespace WorldWind.Renderable
{
	/// <summary>
	/// Holds a collection of icons
	/// </summary>
	public class PointIcons : RenderableObject
	{
        public class PointIcon
        {
            public string Name;
            public string Description;

            public double Lat
            {
                get { return m_lat; }
                set 
                { 
                    m_lat = value;
                    Update();
                }
            }
            private double m_lat;

            public double Lon
            {
                get { return m_lon; }
                set 
                { 
                    m_lon = value;
                    Update();
                }
            }
            private double m_lon;

            public double Alt
            {
                get { return m_alt; }
                set 
                { 
                    m_alt = value;
                    Update();
                }
            }
            private double m_alt;

            public float Size;
            public Color Color;
            public string Url;

            /// <summary>
            /// The cartesian coordinates of this icon.  
            /// Used to be settable but never actually updated the position of the icon.
            /// </summary>
            public Point3d PositionD
            {
                get { return m_positionD; }
            }
            private Point3d m_positionD = new Point3d();

            /// <summary>
            /// Object position (XYZ world coordinates)
            /// </summary>
            public virtual Vector3 Position
            {
                get
                {
                    return this.m_position;
                }
                set
                {
                    this.m_position = value;
                }
            }
            private Vector3 m_position = new Vector3();

            /// <summary>
            /// If LMB pressed calls PerformLMBAction, if RMB pressed calls PerformRMBAction
            /// </summary>
            /// <param name="drawArgs"></param>
            /// <returns></returns>
            public bool PerformSelectionAction(DrawArgs drawArgs)
            {
                if (DrawArgs.IsLeftMouseButtonDown)
                    return PerformLMBAction(drawArgs);

                if (DrawArgs.IsRightMouseButtonDown)
                    return PerformRMBAction(drawArgs);

                return false;
            }

            /// <summary>
            /// Goes to icon if camera positions set.  Also opens URL if it exists
            /// </summary>
            /// <param name="drawArgs"></param>
            /// <returns></returns>
            protected virtual bool PerformLMBAction(DrawArgs drawArgs)
            {
                try
                {
                    // Goto to URL if we have one
                    if (Url != null && !Url.Contains(@"worldwind://"))
                    {
                        if (World.Settings.UseInternalBrowser && Url.StartsWith("http"))
                        {
                            SplitContainer sc = (SplitContainer)drawArgs.parentControl.Parent.Parent;
                            InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                            browser.NavigateTo(Url);
                        }
                        else
                        {
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = Url;
                            psi.Verb = "open";
                            psi.UseShellExecute = true;

                            psi.CreateNoWindow = true;
                            Process.Start(psi);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message.ToString());
                }
                return false;
            }

            /// <summary>
            /// RMB Click Action
            /// </summary>
            /// <param name="drawArgs"></param>
            /// <returns></returns>
            protected virtual bool PerformRMBAction(DrawArgs drawArgs)
            {
                return false;
            }

            private void Update()
            {
                double altitude;

                // Altitude is ASL
                altitude = (World.Settings.VerticalExaggeration * Alt) + DrawArgs.Camera.WorldRadius;

                Position = MathEngine.SphericalToCartesian(Lat, Lon, altitude);

                m_positionD = MathEngine.SphericalToCartesianD(
                    Angle.FromDegrees(Lat),
                    Angle.FromDegrees(Lon),
                    altitude);
            }
        }

        /// <summary>
        /// This is the texture for point sprites
        /// </summary>
        protected IconTexture m_pointTexture;

        protected List<PointSpriteVertex> m_pointSprites = new List<PointSpriteVertex>();

        protected Dictionary<string, PointIcon> m_points = new Dictionary<string, PointIcon>(); 

		protected static int nameColor = Color.White.ToArgb();
		protected static int descriptionColor = Color.White.ToArgb();

        /// <summary>
        /// Bounding box centered at (0,0) used to calculate whether mouse is over point
        /// </summary>
        public Rectangle SelectionRectangle;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icons"/> class 
		/// </summary>
		/// <param name="name">The name of the icons layer</param>
		public PointIcons(string name) : base(name) 
		{
            isInitialized = false;
		}

		#region RenderableObject methods

        public override void Update(DrawArgs drawArgs)
        {
        }

		public override void Initialize(DrawArgs drawArgs)
		{
			if(!isOn)
				return;

            if (!isInitialized)
            {
                string textureFilePath = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), @"Data\Icons\sol_wh.gif");

                m_pointTexture = new IconTexture(drawArgs.device, textureFilePath);

                // This could be off since the texture is scaled
                SelectionRectangle = new Rectangle(0, 0, m_pointTexture.Width, m_pointTexture.Height);
                
                // Center the box at (0,0)
                SelectionRectangle.Offset(-this.SelectionRectangle.Width / 2, -this.SelectionRectangle.Height / 2);
            }

			isInitialized = true;
		}


		public override void Dispose()
		{
            try
            {
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            int closestIconDistanceSquared = int.MaxValue;
            PointIcon closestIcon = null;

            foreach (PointIcon point in m_points.Values)
            {

                // don't check if we aren't even in view
                if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(point.Position))
                {

                    // check if inside the icons bounding box
                    Vector3 referenceCenter = new Vector3(
                        (float)drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                    Vector3 projectedPoint = drawArgs.WorldCamera.Project(point.Position - referenceCenter);

                    int dx = DrawArgs.LastMousePosition.X - (int)projectedPoint.X;
                    int dy = DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y;

                    if (SelectionRectangle.Contains(dx, dy))
                    {
                        // Mouse is over, check whether this icon is closest
                        int distanceSquared = dx * dx + dy * dy;
                        if (distanceSquared < closestIconDistanceSquared)
                        {
                            closestIconDistanceSquared = distanceSquared;
                            closestIcon = point;
                        }
                    }
                }
            }

            // if no other object has handled the selection let the closest icon try
            if (closestIcon != null)
            {
                return closestIcon.PerformSelectionAction(drawArgs);
            }

			return false;
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(!isOn)
				return;

			if(!isInitialized)
				Initialize(drawArgs);

            m_pointSprites.Clear();

            int closestIconDistanceSquared = int.MaxValue;
            PointIcon closestIcon = null;

            // build list of all points in view
            foreach (PointIcon point in m_points.Values)
            {
                try
                {
                    // don't bother to do anything else if we aren't even in view
                    if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(point.Position))
                    {
                        Vector3 translationVector = new Vector3(
                        (float)(point.PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                        (float)(point.PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                        (float)(point.PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                        Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

                        // check if inside bounding box of icon
                        int dx = DrawArgs.LastMousePosition.X - (int)projectedPoint.X;
                        int dy = DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y;
                        if (SelectionRectangle.Contains(dx, dy))
                        {
                            // Mouse is over, check whether this icon is closest
                            int distanceSquared = dx * dx + dy * dy;
                            if (distanceSquared < closestIconDistanceSquared)
                            {
                                closestIconDistanceSquared = distanceSquared;
                                closestIcon = point;
                            }
                        }

                        PointSpriteVertex pv = new PointSpriteVertex(translationVector.X, translationVector.Y, translationVector.Z, point.Size, point.Color.ToArgb());
                        m_pointSprites.Add(pv);
                    }
                }
                catch 
                {
                }
                finally
                {
                }
            }

            // render point sprites if any in the list
            try
            {
                if (m_pointSprites.Count > 0)
                {
                    // save device state
                    Texture origTexture = drawArgs.device.GetTexture(0);
                    VertexFormats origVertexFormat = drawArgs.device.VertexFormat;
                    float origPointScaleA = drawArgs.device.RenderState.PointScaleA;
                    float origPointScaleB = drawArgs.device.RenderState.PointScaleB;
                    float origPointScaleC = drawArgs.device.RenderState.PointScaleC;
                    bool origPointSpriteEnable = drawArgs.device.RenderState.PointSpriteEnable;
                    bool origPointScaleEnable = drawArgs.device.RenderState.PointScaleEnable;
                    Blend origSourceBlend = drawArgs.device.RenderState.SourceBlend;
                    Blend origDestBlend = drawArgs.device.RenderState.DestinationBlend;

                    // set device to do point sprites
                    drawArgs.device.SetTexture(0, m_pointTexture.Texture);
                    drawArgs.device.VertexFormat = VertexFormats.Position | VertexFormats.PointSize | VertexFormats.Diffuse;
                    drawArgs.device.RenderState.PointScaleA = 1f;
                    drawArgs.device.RenderState.PointScaleB = 0f;
                    drawArgs.device.RenderState.PointScaleC = 0f;
                    drawArgs.device.RenderState.PointSpriteEnable = true;
                    drawArgs.device.RenderState.PointScaleEnable = true;
                    //drawArgs.device.RenderState.SourceBlend = Blend.One;
                    //drawArgs.device.RenderState.DestinationBlend = Blend.BlendFactor;

                    drawArgs.device.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.Modulate);
                    drawArgs.device.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
                    drawArgs.device.SetTextureStageState(0, TextureStageStates.ColorArgument2, (int)TextureArgument.Diffuse);

                    // Draw all visible points
                    drawArgs.device.DrawUserPrimitives(PrimitiveType.PointList, m_pointSprites.Count, m_pointSprites.ToArray());    

                    // Draw label and description of mouseover point
                    if (closestIcon != null)
                    {
                    }

                    // restore device state
                    drawArgs.device.SetTexture(0, origTexture);
                    drawArgs.device.VertexFormat = origVertexFormat;
                    drawArgs.device.RenderState.PointScaleA = origPointScaleA;
                    drawArgs.device.RenderState.PointScaleB = origPointScaleB;
                    drawArgs.device.RenderState.PointScaleC = origPointScaleC;
                    drawArgs.device.RenderState.PointSpriteEnable = origPointSpriteEnable;
                    drawArgs.device.RenderState.PointScaleEnable = origPointScaleEnable;
                    drawArgs.device.RenderState.SourceBlend = origSourceBlend;
                    drawArgs.device.RenderState.DestinationBlend = origDestBlend;
                }
            }
            catch
            {
            }
		}

		#endregion

        /// <summary>
        /// Add a new fast icon point.  Overwrites any existing fast icon point with the same name. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="url"></param>
        public void AddPoint(string name, string description, double lat, double lon, double alt, float size, Color color, string url)
        {
            if (m_points.ContainsKey(name))
                m_points.Remove(name);

        }

        /// <summary>
        /// Adds the provided point.  Overwrites any existing point with the same key.
        /// </summary>
        /// <param name="point">PointIcon to add.</param>
        public void AddPoint(PointIcon point)
        {
            if (m_points.ContainsKey(point.Name))
                m_points.Remove(point.Name);

            m_points.Add(point.Name, point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        public void UpdatePoint(string name, double lat, double lon, double alt)
        {
            if (m_points.ContainsKey(name))
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="url"></param>
        public void UpdatePoint(string name, string description, double lat, double lon, double alt, float size, Color color, string url)
        {
            if (m_points.ContainsKey(name))
            {
            }
        }

        /// <summary>
        /// Changes the point color
        /// </summary>
        /// <param name="name">name of point</param>
        /// <param name="color">new color for this point</param>
        public void UpdatePointColor(string name, Color color)
        {
            if (m_points.ContainsKey(name))
            {
                m_points[name].Color = color;
            }
        }

        /// <summary>
        /// Changes the rendered scaling size for this point
        /// </summary>
        /// <param name="name">name of point</param>
        /// <param name="size">new scaling size</param>
        public void UpdatePointSize(string name, float size)
        {
            if (m_points.ContainsKey(name))
            {
                m_points[name].Size = size;
            }
        }

        /// <summary>
        /// Returns the point if it exists
        /// </summary>
        /// <param name="name">Name of point</param>
        /// <returns>Point found or null if not</returns>
        public PointIcon GetPoint(string name)
        {
            return m_points[name];
        }

        /// <summary>
        /// Removes point if it exists
        /// </summary>
        /// <param name="name">name of point to remove</param>
        public void RemovePoint(string name)
        {
            if (m_points.ContainsKey(name))
                m_points.Remove(name);
        }
	}
}
