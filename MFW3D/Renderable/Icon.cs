//
// Copyright ?2005 NASA.  Available under the NOSA License
//
// Portions copied from JHU_Icon - Copyright ?2005-2006 The Johns Hopkins University 
// Applied Physics Laboratory.  Available under the JHU/APL Open Source Agreement.
//
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

namespace MFW3D.Renderable
{
	/// <summary>
	/// One icon in an icon layer
	/// </summary>
	public class Icon : RenderableObject
    {
        # region private variables
        
        /// <summary>
        /// Indicates if an update is required.  If isUpdated is false then the update will run.
        /// </summary>
        protected bool m_isUpdated = false;

        /// <summary>
        /// Indicates if a new texture needs to be loaded
        /// </summary>
        protected bool m_newTexture = false;

        protected Vector3 m_groundPoint;
        protected Line m_groundStick;
        protected Vector2[] m_groundStickVector;

        protected static int hotColor = Color.White.ToArgb();
        protected static int normalColor = Color.FromArgb(150, 255, 255, 255).ToArgb();
        protected static int nameColor = Color.White.ToArgb();
        protected static int descriptionColor = Color.White.ToArgb();

        // used only in default render
        protected Sprite m_sprite;

        // used only in default render
        protected List<Rectangle> m_labelRectangles;

        protected Matrix lastView = Matrix.Identity;

        /// <summary>
        /// The context menu for this icon
        /// </summary>
        protected ContextMenu m_contextMenu = null;

        /// <summary>
        /// This value is computed by anyone that would try to render this icon.
        /// It isn't a parameter because it is often compared to in order to 
        /// see how this icon should be rendered (PointSprite, Sprite, Mesh).
        /// </summary>
        protected internal float DistanceToIcon
        {
            get { return m_distanceToIcon; }
            set { m_distanceToIcon = value; }
        }
        private float m_distanceToIcon = float.MaxValue;

        #endregion

        #region Properties

        /// <summary>
		/// Whether the name of this icon should always be rendered
		/// </summary>
		public bool NameAlwaysVisible
		{
			get{ return m_nameAlwaysVisible; }
			set{ m_nameAlwaysVisible = value; }
		}
        private bool m_nameAlwaysVisible = false;

        /// <summary>
        /// Whether or not this icon should be rotated.
        /// </summary>
		public bool IsRotated
		{
			get { return m_isRotated; }
			set { m_isRotated = value; }
		}
        private bool m_isRotated = false;
	
        /// <summary>
        /// The angle of rotation to display the icon's texture in Degrees.
        /// </summary>
		public Angle Rotation
		{
			get { return m_rotation; }
			set { m_rotation = value; }
		}
        private Angle m_rotation = Angle.Zero;

        /// <summary>
        /// Latitude (North/South) in decimal degrees
        /// </summary>
        public double Latitude
        {
            get { return m_latitude.Degrees; }
            set
            {
                m_latitude.Degrees = value;
                m_isUpdated = false;
            }
        }
        private Angle m_latitude = Angle.Zero;

        /// <summary>
        /// Longitude (East/West) in decimal degrees
        /// </summary>
        public double Longitude
        {
            get { return m_longitude.Degrees; }
            set
            {
                m_longitude.Degrees = value;
                m_isUpdated = false;
            }

        }
        private Angle m_longitude = Angle.Zero;

        /// <summary>
        /// Latitude as an Angle class
        /// </summary>
        public Angle LatitudeAngle
        {
            get { return m_latitude; }
            set
            {
                m_latitude = value;
                m_isUpdated = false;
            }
        }

        /// <summary>
        /// Longitude as an Angle class
        /// </summary>
        public Angle LongitudeAngle
        {
            get { return m_longitude; }
            set
            {
                m_longitude = value;
                m_isUpdated = false;
            }
        }

        /// <summary>
        /// Latitude as a radian
        /// </summary>
        public double LatitudeRadians
        {
            get { return m_latitude.Radians; }
            set
            {
                m_latitude.Radians = value;
                m_isUpdated = false;
            }
        }

        /// <summary>
        /// Longitude as a radian
        /// </summary>
        public double LongitudeRadians
        {
            get { return m_longitude.Radians; }
            set
            {
                m_longitude.Radians = value;
                m_isUpdated = false;
            }
        }

        /// <summary>
        /// The icon altitude above the surface 
        /// </summary>
        public double Altitude
        {
            get { return m_altitude; }
            set
            {
                m_altitude = value;
                m_isUpdated = false;
            }
        }
        private double m_altitude = 0.0;

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
        /// Icon bitmap path. (Overrides Image)
        /// </summary>
        public string TextureFileName
        {
            get { return m_textureFileName; }
            set
            {
                m_textureFileName = value;
                m_newTexture = true;
                m_isUpdated = false;
            }
        }
        private string m_textureFileName;

        /// <summary>
        /// The icon's texture
        /// </summary>
        public IconTexture IconTexture
        {
            get { return m_iconTexture; }
        }
        private IconTexture m_iconTexture;

        /// <summary>
        /// On-Click browse to location
        /// </summary>
        public string ClickableActionURL
        {
            get
            {
                return m_clickableActionURL;
            }
            set
            {
                isSelectable = value != null;
                m_clickableActionURL = value;
            }
        }
        private string m_clickableActionURL;

        /// <summary>
        /// Whether or not a groundstick should be drawn
        /// </summary>
        public bool DrawGroundStick
        {
            get { return m_drawGroundStick; }
            set
            {
                m_drawGroundStick = value;
                m_isUpdated = false;
            }
        }
        private bool m_drawGroundStick = false;

        /// <summary>
        /// Maximum distance at which to render ground sticks.  Default = max.
        /// </summary>
        public double MaxGroundStickDistance
        {
            get { return m_maxGroundStickDistance; }
            set { m_maxGroundStickDistance = value; }
        }
        private double m_maxGroundStickDistance = double.MaxValue;

        /// <summary>
        /// Whether or not the labels should be decluttered
        /// </summary>
        public bool Declutter
        {
            get { return m_declutter; }
            set { m_declutter = value; }
        }
        private bool m_declutter = false;

        /// <summary>
        /// An ID for this icon.  Depends on the plugin to assign but can be used to uniquely identify this icon
        /// without having an incomprehensible name.
        /// 
        /// TODO: Name is not always unique...determine if this is a problem since it may be used as a key in things.
        /// </summary>
        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }
        protected string m_id;

        /// <summary>
        /// Any user defined icon related data to hang onto.
        /// 
        /// TODO:  Should this be in RO?
        /// </summary>
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }
        protected object m_tag;

        /// <summary>
        /// If true the icon will autoscale based on altitude
        /// </summary>
        public bool AutoScaleIcon
        {
            get { return m_autoScaleIcon; }
            set { m_autoScaleIcon = value; }
        }
        protected bool m_autoScaleIcon = false;

        /// <summary>
        /// At what distance to start autoscaling
        /// </summary>
        public int MinIconZoomDistance
        {
            get { return m_minIconZoomDistance; }
            set { m_minIconZoomDistance = value; }
        }
        private int m_minIconZoomDistance = 2500000;

        /// <summary>
        /// The smallest to scale this icon based on zoom (default = 20%)
        /// </summary>
        public float MinScaleFactor
        {
            get { return m_minScaleFactor; }
            set { m_minScaleFactor = value; }
        }
        private float m_minScaleFactor = 0.20f;

        /// <summary>
        /// True if altitude is in AGL, False if ASL.  Default is AGL.
        /// </summary>
        public bool IsAGL
        {
            get { return m_isAGL; }
            set { m_isAGL = value; }
        }
        protected bool m_isAGL = true;

        /// <summary>
        /// True if Vertical Exaggeration should be used in computing altitude.  
        /// Default is true.
        /// </summary>
        public bool UseVE
        {
            get { return m_useVE; }
            set { m_useVE = value; }
        }
        protected bool m_useVE = true;        
        
        /// <summary>
        /// True if a zero Vertical Exaggeration should be used in computing altitude.  
        /// If set then a VE of 0 forces altitude to 0 since its multiplied.  Ignored if UseVE is false.
        /// Default is true.
        /// </summary>
        public bool UseZeroVE
        {
            get { return m_useZeroVE; }
            set { m_useZeroVE = value; }
        }
        protected bool m_useZeroVE = true;

        /// <summary>
        /// Whether or not this will change color on mouseover (default = true)
        /// </summary>
        public bool AlwaysHighlight
        {
            get
            {
                return m_alwaysHighlight;
            }
            set
            {
                m_alwaysHighlight = value;
            }
        }
        protected bool m_alwaysHighlight = false;

        /// <summary>
        /// Whether or not this will change color on mouseover (default = true)
        /// </summary>
        public bool DisableMouseoverHighlight
        {
            get
            {
                return m_disableMouseoverHighlight;
            }
            set
            {
                m_disableMouseoverHighlight = value;
            }
        }
        protected bool m_disableMouseoverHighlight = false;

        public bool OnClickZoomTo
        {
            get { return m_onClickZoomTo; }
            set { m_onClickZoomTo = value; }
        }
        protected bool m_onClickZoomTo = true;

        /// <summary>
        /// Whether or not we should render as a point sprite above a certain camera altitude.
        /// </summary>
        public bool UsePointSprite
        {
            get { return m_usePointSprite; }
            set { m_usePointSprite = value; }
        }
        private bool m_usePointSprite = false;

        /// <summary>
        /// Distance at which to render as PointSprite rather than as a sprite.  Default = 15000000m
        /// </summary>
        public double PointSpriteDistance
        {
            get { return m_pointSpriteDistance; }
            set { m_pointSpriteDistance = value; }
        }
        private double m_pointSpriteDistance = 1500000;

        /// <summary>
        /// Color to render the PointSprite.  Just sets lighting color.
        /// </summary>
        public Color PointSpriteColor
        {
            get { return m_pointSpriteColor; }
            set { m_pointSpriteColor = value; }
        }
        private Color m_pointSpriteColor = Color.Violet;

        /// <summary>
        /// The size to render this pointsprite
        /// </summary>
        public float PointSpriteSize
        {
            get { return m_pointSpriteSize; }
            set { m_pointSpriteSize = value; }
        }
        private float m_pointSpriteSize = .01f;

        /// <summary>
        /// Whether or not to render the texture.  Useful if you want to render groundstick with pointsprites 
        /// but not actually show this icon as a sprite when you do.  
        /// Set MaxGroundStickDistance to PointSpriteDistance, TrailShowDistance or ModelShowDistance 
        /// AlwaysRenderPointSprite to true and this to false.
        /// </summary>
        public bool TextureRenderEnabled
        {
            get { return m_textureRenderEnabled; }
            set { m_textureRenderEnabled = value; }
        }
        private bool m_textureRenderEnabled = true;

        /// <summary>
        /// Whether to always render as a pointsprite even when we render as a sprite
        /// </summary>
        public bool AlwaysRenderPointSprite
        {
            get { return m_alwaysRenderPointSprite; }
            set { m_alwaysRenderPointSprite = value; }
        }
        private bool m_alwaysRenderPointSprite = false;

        /// <summary>
        /// Whether or not this icon has been "hooked" to show descrption all of the time.
        /// </summary>
        /// <remarks>
        /// Is never set to true for base Icon objects but only those the implement hooking (TrackIcon) but
        /// this is here to allow for use by Icons to behave differently (specifically PointSprite behaviors).
        /// </remarks>
        public bool IsHooked
        {
            get { return m_isHooked; }
            set { m_isHooked = value; }
        }
        private bool m_isHooked = false;

        #endregion

        public double OnClickZoomAltitude = double.NaN;
        public double OnClickZoomHeading = double.NaN;
        public double OnClickZoomTilt = double.NaN;
        public string SaveFilePath = null;
        public System.DateTime LastRefresh = System.DateTime.MinValue;
        public System.TimeSpan RefreshInterval = System.TimeSpan.MaxValue;

		System.Collections.ArrayList overlays = new ArrayList();
		
		//not a good way to handle this
		public void OverlayOnOpen(object o, EventArgs e)
		{
			System.Windows.Forms.MenuItem mi = (System.Windows.Forms.MenuItem)o;

			foreach(ScreenOverlay overlay in overlays)
			{
				if(overlay == null)
					continue;

				if(overlay.Name.Equals(mi.Text))
				{
					if(!overlay.IsOn)
						overlay.IsOn = true;
				}
			}
		}

		public ScreenOverlay[] Overlays
		{
			get
			{
				if(overlays == null)
				{
					return null;
				}
				else
				{
					return (ScreenOverlay[])overlays.ToArray(typeof(ScreenOverlay));
				}
			}
		}

		public void AddOverlay(ScreenOverlay overlay)
		{
			if(overlay != null)
				overlays.Add(overlay);
		}

		public void RemoveOverlay(ScreenOverlay overlay)
		{
			for(int i = 0; i < overlays.Count; i++)
			{
				ScreenOverlay curOverlay = (ScreenOverlay)overlays[i];
				if(curOverlay.IconImagePath == overlay.IconImagePath && overlay.Name == curOverlay.Name)
				{
					overlays.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Icon image.  Leave TextureFileName=null if using Image.  
		/// Caller is responsible for disposing the Bitmap when the layer is removed, 
		/// either by calling Dispose on Icon or on the Image directly.
		/// </summary>
        public Bitmap Image
        {
            get { return m_image; }
            set 
            {
                m_image = value;
                m_newTexture = true;
                m_isUpdated = false;
            }
        }
        private Bitmap m_image;

		/// <summary>
		/// Icon on-screen rendered width (pixels).  Defaults to icon image width.  
		/// If source image file is not a valid GDI+ image format, width may be increased to closest power of 2.
		/// </summary>
        public int Width
        {
            get { return m_width; }
            set
            {
                m_width = value;
                m_newTexture = true;
                m_isUpdated = false;
            }
        }
        private int m_width;

		/// <summary>
		/// Icon on-screen rendered height (pixels).  Defaults to icon image height.  
		/// If source image file is not a valid GDI+ image format, height may be increased to closest power of 2.
		/// </summary>
        public int Height
        {
            get { return m_height; }
            set
            {
                m_height = value;
                m_newTexture = true;
                m_isUpdated = false;
            }
        }
        private int m_height;

        /// <summary>
        ///  Icon X scaling computed by dividing icon width by texture width
        /// </summary>
        public float XScale;

        /// <summary>
        ///  Icon Y scaling computed by dividing icon height by texture height 
        /// </summary>
        public float YScale;

		/// <summary>
		/// The maximum distance (meters) the icon will be visible from
		/// </summary>
		public double MaximumDisplayDistance = double.MaxValue;

		/// <summary>
		/// The minimum distance (meters) the icon will be visible from
		/// </summary>
		public double MinimumDisplayDistance;

		/// <summary>
		/// Bounding box centered at (0,0) used to calculate whether mouse is over icon/label
		/// </summary>
		public Rectangle SelectionRectangle;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		public Icon(string name,
			double latitude, 
			double longitude) : base( name )
		{
			m_latitude.Degrees = latitude;
			m_longitude.Degrees = longitude;
			this.RenderPriority = RenderPriority.Icons;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		public Icon(string name,
			double latitude, 
			double longitude,
            double heightAboveSurface) : this(name, latitude, longitude)
		{
			m_altitude = heightAboveSurface;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icon"/> class 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="heightAboveSurface"></param>
        /// <param name="TextureFileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="actionURL"></param>
        public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			string TextureFileName,
			int width,
			int height,
			string actionURL) : this( name, latitude, longitude, heightAboveSurface )
		{
			this.Description = description;

			this.TextureFileName = TextureFileName;
			this.Width = width;
			this.Height = height;
            m_clickableActionURL = actionURL;
            isSelectable = actionURL != null;
		}

		#region Obsolete

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		[Obsolete]
		public Icon(string name,
			double latitude, 
			double longitude,
			double heightAboveSurface, 
			World parentWorld ) : base( name )
		{
			m_latitude.Degrees = latitude;
            m_longitude.Degrees = longitude;
			this.Altitude = heightAboveSurface;
			this.RenderPriority = RenderPriority.Icons;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		[Obsolete]
		public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			World parentWorld, 
			Bitmap image,
			int width,
			int height,
			string actionURL) : base( name )
		{
			this.Description = description;
            m_latitude.Degrees = latitude;
            m_longitude.Degrees = longitude;
			this.Altitude = heightAboveSurface;
			this.m_image = image;
			this.Width = width;
			this.Height = height;
			ClickableActionURL = actionURL;
			this.RenderPriority = RenderPriority.Icons;
            isSelectable = actionURL != null;

		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="heightAboveSurface"></param>
		/// <param name="parentWorld"></param>
		/// <param name="TextureFileName"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="actionURL"></param>
		[Obsolete]
		public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			World parentWorld, 
			string TextureFileName,
			int width,
			int height,
			string actionURL) : base( name )
		{
			this.Description = description;
            m_latitude.Degrees = latitude;
            m_longitude.Degrees = longitude;
			this.Altitude = heightAboveSurface;
			this.TextureFileName = TextureFileName;
			this.Width = width;
			this.Height = height;
			ClickableActionURL = actionURL;
			this.RenderPriority = RenderPriority.Icons;
            isSelectable = actionURL != null;

		}

		#endregion

		/// <summary>
		/// Sets the geographic position of the icon.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
        public virtual void SetPosition(double latitude, double longitude)
		{
            m_latitude.Degrees = latitude;
            m_longitude.Degrees = longitude;

			// Recalculate XYZ coordinates
            m_isUpdated = false;
        }

		/// <summary>
		/// Sets the geographic position of the icon.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="altitude">The icon altitude above sea level.</param>
        public virtual void SetPosition(double latitude, double longitude, double altitude)
		{
            m_latitude.Degrees = latitude;
            m_longitude.Degrees = longitude;
			m_altitude = altitude;

			// Recalculate XYZ coordinates
            m_isUpdated = false;
        }

		#region RenderableObject methods

		public override void Initialize(DrawArgs drawArgs)
		{
            // get icon texture
            m_iconTexture = null;

            BuildIconTexture(drawArgs);

            if (m_drawGroundStick)
            {
                if (m_groundStick == null)
                    m_groundStick = new Line(drawArgs.device);

                if (m_groundStickVector == null)
                    m_groundStickVector = new Vector2[2];

                m_groundStick.Antialias = true;
            }

			isInitialized = true;
		}

		/// <summary>
		/// Disposes the icon (when disabled)
		/// </summary>
		public override void Dispose()
		{
            try
            {
                IconTexture iconTexture = null;
                // decrement our count from textures - the icons class will clean up
                if ((TextureFileName != null) && (TextureFileName.Trim() != String.Empty))
                    iconTexture = (IconTexture)DrawArgs.Textures[TextureFileName];
                else if (m_image != null)
                    iconTexture = (IconTexture)DrawArgs.Textures[m_image];

                if (iconTexture != null)
                {
                    iconTexture.ReferenceCount--;
                }

                if (m_sprite != null)
                {
                    m_sprite.Dispose();
                    m_sprite = null;
                }

                isInitialized = false;

            }
            finally
            {
                // base.Dispose();
            }

		}

        /// <summary>
        /// If LMB pressed calls PerformLMBAction, if RMB pressed calls PerformRMBAction
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <returns></returns>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
            if (!isSelectable)
                return false;

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
                // Goto icon
                if (OnClickZoomTo && (OnClickZoomAltitude != double.NaN 
                    || OnClickZoomHeading != double.NaN || OnClickZoomTilt != double.NaN))
                {
                    drawArgs.WorldCamera.SetPosition(
                        Latitude,
                        Longitude,
                        OnClickZoomHeading,
                        OnClickZoomAltitude,
                        OnClickZoomTilt);
                }

                // Goto to URL if we have one
                if (ClickableActionURL!=null && !ClickableActionURL.Contains(@"worldwind://"))
                {
                    if (World.Settings.UseInternalBrowser && ClickableActionURL.StartsWith("http"))
                    {
                        SplitContainer sc = (SplitContainer)drawArgs.parentControl.Parent.Parent;
                        InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                        browser.NavigateTo(ClickableActionURL);
                    }
                    else
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = ClickableActionURL;
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
        /// Does something with overlays...
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <returns></returns>
        protected virtual bool PerformRMBAction(DrawArgs drawArgs)
        {
            try
            {
                if (m_contextMenu == null)
                {
                    m_contextMenu = new ContextMenu();
                }

                m_contextMenu.Show(DrawArgs.ParentControl, DrawArgs.LastMousePosition);

                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }
            return false;
        }

        /// Updates where we are if the camera has changed position (and thereby might be using higher resolution terrain
        /// </summary>
        /// <param name="drawArgs"></param>
		public override void Update(DrawArgs drawArgs)
		{
            if (!this.IsOn)
                return;

            if (!this.isInitialized)
                this.Initialize(drawArgs);

            if (!m_isUpdated || (drawArgs.WorldCamera.ViewMatrix != lastView))
            {
                double elevation = drawArgs.WorldCamera.WorldRadius;
                double altitude;

                // altitude = World.Settings.VerticalExaggeration * Altitude;

                // Added this because if VE is set to zero then all floating icons fall to the earth.
                if (UseVE && (UseZeroVE || World.Settings.VerticalExaggeration > 0.1))
                    altitude = World.Settings.VerticalExaggeration * Altitude;
                else
                    altitude = Altitude;

                if (drawArgs.CurrentWorld.TerrainAccessor != null && drawArgs.WorldCamera.Altitude < 300000)
                {
                    double samplesPerDegree = 50.0 / drawArgs.WorldCamera.ViewRange.Degrees;
                    elevation += drawArgs.CurrentWorld.TerrainAccessor.GetElevationAt(m_latitude.Degrees, m_longitude.Degrees, samplesPerDegree) * World.Settings.VerticalExaggeration;
                }

                // we do this rather than zero out elevation because ground stick needs elevation if it exists.
                if (IsAGL)
                {
                    Position = MathEngine.SphericalToCartesian(m_latitude, m_longitude, altitude + elevation);

                    m_positionD = MathEngine.SphericalToCartesianD(
                        m_latitude,
                        m_longitude,
                        altitude + elevation);
                }
                else
                {
                    Position = MathEngine.SphericalToCartesian(m_latitude, m_longitude, altitude + drawArgs.WorldCamera.WorldRadius);

                    m_positionD = MathEngine.SphericalToCartesianD(
                        m_latitude,
                        m_longitude,
                        altitude + drawArgs.WorldCamera.WorldRadius);
                }

                if (m_drawGroundStick)
                {
                    if (m_groundStick == null)
                        m_groundStick = new Line(drawArgs.device);

                    if (m_groundStickVector == null)
                        m_groundStickVector = new Vector2[2];

                    m_groundPoint = MathEngine.SphericalToCartesian(Latitude, Longitude, elevation);
                }

				lastView = drawArgs.WorldCamera.ViewMatrix;
			}

            // should overlays update every time?
            if (overlays != null)
			{
                for (int i = 0; i < overlays.Count; i++)
				{
					ScreenOverlay curOverlay = (ScreenOverlay)overlays[i];
                    if (curOverlay != null)
					{
						curOverlay.Update(drawArgs);
					}
				}
			}

            if (m_newTexture)
            {
                BuildIconTexture(drawArgs);
            }

            m_isUpdated = true;
		}


        /// <summary>
        /// Builds the icon texture based on the saved texturefile name
        /// </summary>
        /// <param name="drawArgs"></param>
        protected virtual void BuildIconTexture(DrawArgs drawArgs)
        {
            try
            {
                object key = null;

                if (m_iconTexture != null)
                {
                    m_iconTexture.ReferenceCount--;
                }

                if ((TextureFileName != null) && (TextureFileName.Trim() != String.Empty))
                {
                    // Icon image from file
                    m_iconTexture = (IconTexture)DrawArgs.Textures[TextureFileName];
                    if (m_iconTexture == null)
                    {
                        key = TextureFileName;
                        m_iconTexture = new IconTexture(drawArgs.device, TextureFileName);
                    }
                  
                }
                else
                {
                    // Icon image from bitmap
                    if (this.m_image != null)
                    {
                        m_iconTexture = (IconTexture)DrawArgs.Textures[this.m_image];
                        if (m_iconTexture == null)
                        {
                            // Create new texture from image
                            key = this.m_image;
                            m_iconTexture = new IconTexture(drawArgs.device, this.m_image);
                        }
                    }
                }
                
                if (m_iconTexture != null)
                {
                    m_iconTexture.ReferenceCount++;

                    if (key != null)
                    {
                        // New texture, cache it
                        DrawArgs.Textures.Add(key, m_iconTexture);
                    }

                    // Use default dimensions if not set
                    if (this.Width == 0)
                        this.Width = m_iconTexture.Width;
                    if (this.Height == 0)
                        this.Height = m_iconTexture.Height;
                }

                // Compute mouse over bounding boxes
                if (m_iconTexture == null)
                {
                    // Label only 
                    this.SelectionRectangle = drawArgs.defaultDrawingFont.MeasureString(null, this.Name, DrawTextFormat.None, 0);
                }
                else
                {
                    // Icon only
                    this.SelectionRectangle = new Rectangle(0, 0, this.Width, this.Height);
                }

                // Center the box at (0,0)
                this.SelectionRectangle.Offset(-this.SelectionRectangle.Width / 2, -this.SelectionRectangle.Height / 2);

                if (m_iconTexture != null)
                {
                    this.XScale = (float)this.Width / m_iconTexture.Width;
                    this.YScale = (float)this.Height / m_iconTexture.Height;

                }
                else
                {
                    this.XScale = 1.0f;
                    this.YScale = 1.0f;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }

            m_newTexture = false;

        }

        /// <summary>
        /// Render the icon.  This can be pretty slow so you should only stick an Icon on an Icons layer.
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Render(DrawArgs drawArgs)
        {
            DistanceToIcon = Vector3.Length(this.Position - drawArgs.WorldCamera.Position);

            // Do whatever pre-rendering we have to do
            PreRender(drawArgs, false);

            // If we're in view render
            if ((drawArgs.WorldCamera.ViewFrustum.ContainsPoint(this.Position)) &&
                (DistanceToIcon <= this.MaximumDisplayDistance) &&
                (DistanceToIcon >= this.MinimumDisplayDistance))
            {
                Vector3 translationVector = new Vector3(
                    (float)(PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                    (float)(PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                    (float)(PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

                if (m_sprite == null)
                    m_sprite = new Sprite(drawArgs.device);

                if (m_labelRectangles == null)
                    m_labelRectangles = new List<Rectangle>();

                // Clear or we never redraw our label
                m_labelRectangles.Clear();

                // Check icons for within "visual" range
                m_sprite.Begin(SpriteFlags.AlphaBlend);

                FastRender(drawArgs, m_sprite, projectedPoint, false, m_labelRectangles);

                m_sprite.End();
            }

            // do whatever post rendering stuff we have to do
            PostRender(drawArgs, false);
        }

        /// <summary>
        /// Fast render is used to batch the renders of all icons on a layer into a single Sprite.Begin and End block.
        /// </summary>
        /// <param name="drawArgs">The drawing arguments</param>
        /// <param name="sprite">The sprite to use for drawing</param>
        /// <param name="projectedPoint">Where we are</param>
        /// <param name="isMouseOver">Whether we should render as a mouseover icon</param>
        public void FastRender(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, bool isMouseOver, List<Rectangle> labelRectangles)
        {
            if (!this.isInitialized)
            {
                this.m_isUpdated = false;
                return;
            }

            int color = normalColor;

            if ((!m_disableMouseoverHighlight && isMouseOver) || m_alwaysHighlight)
                color = hotColor;

            // Render the label if necessary
            RenderLabel(drawArgs, sprite, projectedPoint, color, labelRectangles, isMouseOver);
            
            // render the icon image
            if (TextureRenderEnabled && (m_iconTexture != null))
            {
                RenderTexture(drawArgs, sprite, projectedPoint, color, isMouseOver);
            }

            RenderGroundStick(drawArgs, sprite, projectedPoint, color, isMouseOver);

            if (isMouseOver)
                RenderDescription(drawArgs, sprite, projectedPoint, color);

        }

        /// <summary>
        /// Renders the overlays for this icon
        /// </summary>
        /// <param name="drawArgs"></param>
        public virtual void RenderOverlay(DrawArgs drawArgs)
        {
            if (overlays != null)
            {
                for (int i = 0; i < overlays.Count; i++)
                {
                    ScreenOverlay curOverlay = (ScreenOverlay)overlays[i];
                    if (curOverlay != null && curOverlay.IsOn)
                    {
                        curOverlay.Render(drawArgs);
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to render icon label.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color"></param>
        /// <param name="labelRectangles"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected virtual void RenderLabel(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, List<Rectangle> labelRectangles, bool isMouseOver)
        {
            if ((this.Name != null) && ((m_iconTexture == null) || isMouseOver || NameAlwaysVisible))
            {
                if (m_iconTexture == null)
                {
                    // Original Icon Label Render code

                    // Center over target as we have no bitmap
                    Rectangle realrect = drawArgs.defaultDrawingFont.MeasureString(m_sprite, Name, DrawTextFormat.WordBreak, color);
                    realrect.X = (int)projectedPoint.X - (realrect.Width >> 1);
                    realrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    bool bDraw = true;

                    // Only not show if declutter is turned on and we aren't always supposed to be seen
                    if (Declutter && !NameAlwaysVisible)
                    {
                        foreach (Rectangle drawnrect in labelRectangles)
                        {
                            if (realrect.IntersectsWith(drawnrect))
                            {
                                bDraw = false;
                                break;
                            }
                        }
                    }

                    if (bDraw)
                    {
                        labelRectangles.Add(realrect);

                        drawArgs.defaultDrawingFont.DrawText(m_sprite, Name, realrect, DrawTextFormat.WordBreak, color);
                    }
                }
                else
                {
                    // KML Label Render Code with Declutter

                    // Adjust text to make room for icon
                    int spacing = (int)(Width * 0.3f);
                    if (spacing > 5)
                        spacing = 5;
                    int offsetForIcon = (Width >> 1) + spacing;

                    // Text to the right
                    Rectangle rightrect = drawArgs.defaultDrawingFont.MeasureString(m_sprite, Name, DrawTextFormat.WordBreak, color);
                    rightrect.X = (int)projectedPoint.X + offsetForIcon;
                    rightrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    // Text to the left
                    Rectangle leftrect = drawArgs.defaultDrawingFont.MeasureString(m_sprite, Name, DrawTextFormat.WordBreak, color);
                    leftrect.X = (int)projectedPoint.X - offsetForIcon - rightrect.Width;
                    leftrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    bool bDrawRight = true;
                    bool bDrawLeft = true;

                    // Only not show if declutter is turned on and we aren't always supposed to be seen
                    if (Declutter && !NameAlwaysVisible)
                    {
                        foreach (Rectangle drawnrect in labelRectangles)
                        {
                            if (rightrect.IntersectsWith(drawnrect))
                            {
                                bDrawRight = false;
                            }
                            if (leftrect.IntersectsWith(drawnrect))
                            {
                                bDrawLeft = false;
                            }
                            if (!bDrawRight && !bDrawLeft)
                            {
                                break;
                            }
                        }
                    }

                    // draw either right or left if we have space.  If we don't too bad.
                    if (bDrawRight)
                    {
                        labelRectangles.Add(rightrect);
                        //drawArgs.defaultDrawingFont.DrawText(m_sprite, Name, rightrect, DrawTextFormat.WordBreak, color);
                        drawArgs.defaultDrawingFont.DrawText(null, Name, rightrect, DrawTextFormat.WordBreak, color);
                    }
                    else if (bDrawLeft)
                    {
                        labelRectangles.Add(leftrect);
                        drawArgs.defaultDrawingFont.DrawText(m_sprite, Name, leftrect, DrawTextFormat.WordBreak, color);
                    }
                }

            }
        }

        /// <summary>
        /// Helper function to render icon texture.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color">the color to render the icon</param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected virtual void RenderTexture(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, bool isMouseOver)
        {
            Matrix scaleTransform;
            Matrix rotationTransform;

            //Do Altitude depedent scaling for KMLIcons
            if (AutoScaleIcon)
            {
                float factor = 1;
                if (DistanceToIcon > MinIconZoomDistance)
                    factor -= (float)((DistanceToIcon - MinIconZoomDistance) / DistanceToIcon);
                if (factor < MinScaleFactor) factor = MinScaleFactor;

                XScale = factor * ((float)Width / m_iconTexture.Width);
                YScale = factor * ((float)Height / m_iconTexture.Height);
            }

            //scale and rotate image
            scaleTransform = Matrix.Scaling(this.XScale, this.YScale, 0);

            if (m_isRotated)
                rotationTransform = Matrix.RotationZ((float)m_rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
            else
                rotationTransform = Matrix.Identity;

            sprite.Transform = scaleTransform * rotationTransform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
            sprite.Draw(m_iconTexture.Texture,
                new Vector3(m_iconTexture.Width >> 1, m_iconTexture.Height >> 1, 0),
                Vector3.Empty,
                color);

            // Reset transform to prepare for text rendering later
            sprite.Transform = Matrix.Identity;

        }

        /// <summary>
        /// Helper function to render the groundstick
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected virtual void RenderGroundStick(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, bool isMouseOver)
        {
            if (m_drawGroundStick)
            {
                // distance to icon is set by the renderer
                if ((DistanceToIcon < MaxGroundStickDistance) || isMouseOver)
                {
                    Vector3 referenceCenter = new Vector3(
                        (float)drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                    Vector3 projectedGroundPoint = drawArgs.WorldCamera.Project(m_groundPoint - referenceCenter);

                    m_groundStick.Begin();
                    m_groundStickVector[0].X = projectedPoint.X;
                    m_groundStickVector[0].Y = projectedPoint.Y;
                    m_groundStickVector[1].X = projectedGroundPoint.X;
                    m_groundStickVector[1].Y = projectedGroundPoint.Y;

                    m_groundStick.Draw(m_groundStickVector, color);
                    m_groundStick.End();
                }
            }
        }

        /// <summary>
        /// Helper function to render icon description.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        protected virtual void RenderDescription(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color)
        {
            string description = this.Description;

            if (description == null)
                description = ClickableActionURL;

            if (description != null)
            {
                // Render description field
                DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Bottom;
                int left = 10;
                if (World.Settings.showLayerManager)
                    left += World.Settings.layerManagerWidth;
                Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10);

                // Draw outline
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                rect.Offset(2, 0);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                rect.Offset(0, 2);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                rect.Offset(-2, 0);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                // Draw description
                rect.Offset(1, -1);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, descriptionColor);
            }
        }

        /// <summary>
        /// Does whatever you need to do before you render the icon.  Occurs even if the icon isn't visible!
        /// Occurs OUTSIDE of Sprite.begin.  Other RO's can be rendered here.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public virtual void PreRender(DrawArgs drawArgs, bool isMouseOver)
        {
            RenderOverlay(drawArgs);
        }

        /// <summary>
        /// Does whatever you need to do after you render the icon.  Occurs even if the icon isn't visible!
        /// Occurs INSIDE of Sprite.begin.  Other RO's wont render here.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public virtual void PostRender(DrawArgs drawArgs, bool isMouseOver)
        {
        }

        /// <summary>
        /// Do this if we don't actually get rendered (not in view, too far, etc)
        /// </summary>
        /// <param name="drawArgs"></param>
        public virtual void NoRender(DrawArgs drawArgs)
        {
        }

		#endregion

        protected void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
		}
	}
}
