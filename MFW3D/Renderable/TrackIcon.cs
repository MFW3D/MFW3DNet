using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Text;

using MFW3D.NewWidgets;
using System.Collections.Generic;

namespace MFW3D.Renderable
{
	/// <summary>
	/// One icon in an icon layer
	/// </summary>
	public class TrackIcon : MFW3D.Renderable.Icon
    {
        protected bool m_gotoMe = false;

        /// <summary>
        /// The heading for this object (0-360) in degrees
        /// </summary>
        public double Heading
        {
            get { return Rotation.Degrees; }
            set
            {
                Rotation = Angle.FromDegrees(value);
            }
        }

        /// <summary>
        /// The speed of this object (knots)
        /// </summary>
        public double Speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }

        /// <summary>
        /// The speed of this object (knots)
        /// </summary>
        protected double m_speed;

        /// <summary>
        /// The date time of the last update
        /// </summary>
        public DateTime UpdateTime
        {
            get { return m_updateTime; }
            set { m_updateTime = value; }
        }
        private DateTime m_updateTime;

        /// <summary>
        /// The date time of the source for this update
        /// </summary>
        public DateTime SourceTime
        {
            get { return m_sourceTime; }
            set { m_sourceTime = value; }
        }
        private DateTime m_sourceTime;

        /// <summary>
        /// Whether this object is a surface track that should hug the ground.
        /// Default is false.
        /// </summary>
        public bool IsSurfaceTrack
        {
            get { return m_isSurfaceTrack; }
            set { m_isSurfaceTrack = value; }
        }
        private bool m_isSurfaceTrack = false;

        #region Hook form

        protected FormWidget m_hookForm = null;

        protected SimpleTreeNodeWidget m_hookTreeNode = null;

        protected SimpleTreeNodeWidget m_hookGeneralTreeNode = null;

        protected SimpleTreeNodeWidget m_hookDetailTreeNode = null;

        protected SimpleTreeNodeWidget m_hookDescTreeNode = null;

        protected LabelWidget m_hookGeneralLabel = null;

        protected LabelWidget m_hookDetailLabel = null;

        protected LabelWidget m_hookDescLabel = null;

        #endregion

        #region Secondary textures (for unit size, acknowledgements, etc)

        /// <summary>
        /// Flag that indicates if the secondary icon texture should be shown.
        /// </summary>
        protected bool m_iconTexture2Show = false;

        public bool IconTexture2Show
        {
            get
            {
                return m_iconTexture2Show;
            }
            set
            {
                m_iconTexture2Show = value;
            }
        }

        /// <summary>
        /// Secondary texture bitmap path. (Overrides Image)
        /// </summary>
        public string Texture2FileName
        {
            get { return m_iconTexture2Name; }
            set
            {
                m_iconTexture2Name = value;
                m_newTexture = true;
                m_isUpdated = false;
            }
        }
        protected string m_iconTexture2Name;

        /// <summary>
        /// The secondary icon texture - typically a unit size
        /// </summary>
        protected IconTexture m_iconTexture2 = null;

        /// <summary>
        /// Texture 2 rotation angle relative to north
        /// </summary>
        public Angle Texture2Rotation
        {
            get { return m_texture2Rotation; }
            set { m_texture2Rotation = value; }
        }
        private Angle m_texture2Rotation = Angle.Zero;

        /// <summary>
        /// Whether or not to rotate texture 2 differently than main texture
        /// </summary>
        public bool Texture2IsRotatedDifferent
        {
            get { return m_texture2IsRotatedDifferent; }
            set { m_texture2IsRotatedDifferent = value; }
        }
        private bool m_texture2IsRotatedDifferent = false;

        /// <summary>
        /// Whether or not to use the main rotation (aka heading) as the rotation angle.
        /// Set this (and Texture2IsRotated to true) if you are using it for a heading indicator
        /// This allows you to not to rotate the main icon while providing heading.
        /// </summary>
        public bool Texture2UseHeading
        {
            get { return m_texture2UseHeading; }
            set { m_texture2UseHeading = value; }
        }
        private bool m_texture2UseHeading = false;

        /// <summary>
        /// Flag that indicates if the tertiatary icon texture should be shown.
        /// </summary>
        protected bool m_iconTexture3Show = false;

        /// <summary>
        /// The name of the tertiary icon texture
        /// </summary>
        protected string m_iconTexture3Name;

        /// <summary>
        /// The secondary icon texture - typically a heading indicator or status
        /// </summary>
        protected IconTexture m_iconTexture3 = null;

        /// <summary>
        /// Texture 3 rotation angle relative to north
        /// </summary>
        public Angle Texture3Rotation
        {
            get { return m_texture3Rotation; }
            set { m_texture3Rotation = value; }
        }
        private Angle m_texture3Rotation = Angle.Zero;

        /// <summary>
        /// Whether or not to rotate texture 3 differently from main texture
        /// </summary>
        public bool Texture3IsRotatedDifferent
        {
            get { return m_texture3IsRotatedDifferent; }
            set { m_texture3IsRotatedDifferent = value; }
        }
        private bool m_texture3IsRotatedDifferent = false;

        /// <summary>
        /// Whether or not to use the main rotation (aka heading) as the rotation angle.
        /// Set this (and Texture3IsRotated to true) if you are using it for a heading indicator
        /// This allows you to not to rotate the main icon while providing heading.
        /// </summary>
        public bool Texture3UseHeading
        {
            get { return m_texture3UseHeading; }
            set { m_texture3UseHeading = value; }
        }
        private bool m_texture3UseHeading = false;

        #endregion

        #region Track History

        /// <summary>
        /// Maximum number of history points
        /// </summary>
        public int MaxPoints = 600;

        /// <summary>
        /// History points data
        /// </summary>
        public class PosData
        {
            public double lat;
            public double lon;
            public double alt;
            public double spd;
            public double hdg;
            public DateTime updateTime;
            public DateTime sourceTime;
        }

        /// <summary>
        /// Whether or not to render the history trail
        /// </summary>
        public bool RenderTrail
        {
            get { return m_renderTrail; }
            set { m_renderTrail = value; }
        }
        private bool m_renderTrail = true;

        /// <summary>
        /// The history list
        /// </summary>
        public LinkedList<PosData> History
        {
            get { return m_history; }
            set { m_history = value; }
        }
        private LinkedList<PosData> m_history = new LinkedList<PosData>();

        /// <summary>
        /// The linefeature that displays the track history
        /// </summary>
        protected LineFeature m_lineFeature = null;

        /// <summary>
        /// Distance which to start showing the trail
        /// </summary>
        public double TrailShowDistance
        {
            get { return m_trailShowDistance; }
            set { m_trailShowDistance = value; }
        }
        private double m_trailShowDistance = 300000;

        #endregion

        #region 3-D Model

        /// <summary>
        /// Whether or not to render a 3-D model
        /// </summary>
        public bool RenderModel
        {
            get { return m_renderModel; }
            set 
            { 
                if (m_renderModel != value)
                {
                    m_renderModel = value;
                    m_modelFeatureError = false;

                    if (m_renderModel)
                        updateModel(false);
                }
            }
        }
        private bool m_renderModel = true;

        /// <summary>
        /// Whether this model feature failed to load
        /// </summary>
        protected bool m_modelFeatureError = false;

        /// <summary>
        /// The ModelFeature representing the detailed view of this icon
        /// </summary>
        protected ModelFeature m_modelFeature = null;

        /// <summary>
        /// The file path to the model - default is a generic commercial airliner
        /// </summary>         
        public string ModelFilePath
        {
            get { return m_modelFilePath; }
            set 
            {
                // if the path has changed, delete old model and create new one
                if (value != m_modelFilePath)
                {
                    m_modelFilePath = value;
                    m_modelFeature = null;
                    m_modelFeatureError = false;
                    updateModel(false);
                }
            }
        }
        private string m_modelFilePath = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), @"Data\Icons\unknown_commercial.x");

        /// <summary>
        /// Distance at which to start showing model
        /// </summary>
        public double ModelShowDistance
        {
            get { return m_modelShowDistance; }
            set { m_modelShowDistance = value; }
        }
        private double m_modelShowDistance = 200000;


        /// <summary>
        /// X Rotation for model.  Used when model is flipped on its side.
        /// </summary>
        public float ModelRotX
        {
            get { return m_modelRotX; }
            set { m_modelRotX = value; }
        }
        private float m_modelRotX = 180;

        public float ModelScale
        {
            get { return m_modelScale; }
            set { m_modelScale = value; }
        }
        private float m_modelScale = 100.0f;

        
        #endregion

        /// <summary>
		/// Initializes a new instance of a <see cref= "T:WorldWind.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		public TrackIcon(string name,
			double latitude, 
			double longitude) : base( name, latitude, longitude )
		{
            InitIcon();
		}

		/// <summary>
		/// Initializes a new instance of a <see cref= "T:WorldWind.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		public TrackIcon(string name,
			double latitude, 
			double longitude,
            double heightAboveSurface) : base(name, latitude, longitude, heightAboveSurface)
		{
            InitIcon();
		}

        /// <summary>
        /// Initializes a new instance of a <see cref= "T:WorldWind.Renderable.Icon"/> class 
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
        public TrackIcon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			string TextureFileName,
			int width,
			int height,
			string actionURL) : base( name, description, latitude, longitude, heightAboveSurface, TextureFileName, width, height, actionURL )
		{
            InitIcon();
        }

        private void InitIcon()
        {
            // Default our Id to Name since most track objects need it
            Id = Name;

            // we always want to use the hot color
            AlwaysHighlight = true;

            // force this to false because we always want icons with altitude to stay in the sky
            UseZeroVE = false;

            // update the detailed model view
            updateModel(false);

            // use point sprites
            UsePointSprite = true;

            // update sub object to initial positions
            updateSubObjects();
        }

		#region Overrriden methods

		public override void Initialize(DrawArgs drawArgs)
		{
            m_gotoMe = false;

            isSelectable = true;

            base.Initialize(drawArgs);

            // note base initialize SHOULD set is initialized flag
		}

        /// <summary>
        /// Updates where we are if the camera has changed position (and thereby might be using higher resolution terrain
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Update(DrawArgs drawArgs)
        {
            if (!this.IsOn)
                return;

            if (!m_isUpdated || (drawArgs.WorldCamera.ViewMatrix != lastView))
            {
                if (!m_isUpdated)
                {
                    updateSubObjects();
                }

                // call this to make sure elevation is reset
                if (m_modelFeature != null)
                    m_modelFeature.Update(drawArgs);

                base.Update(drawArgs);

                // note base update SHOULD set is updated flag
            }
        }

        protected override void BuildIconTexture(DrawArgs drawArgs)
        {
            base.BuildIconTexture(drawArgs);

            try
            {

                // if secondary icon enabled
                if (m_iconTexture2Show && m_iconTexture2Name.Trim() != String.Empty)
                {
                    if (m_iconTexture2 != null)
                    {
                        m_iconTexture2.ReferenceCount--;
                    }

                    m_iconTexture2 = (IconTexture)DrawArgs.Textures[m_iconTexture2Name];
                    if (m_iconTexture2 == null)
                    {
                        m_iconTexture2 = new IconTexture(drawArgs.device, m_iconTexture2Name);
                        DrawArgs.Textures.Add(m_iconTexture2Name, m_iconTexture2);
                    }

                    if (m_iconTexture2 != null)
                    {
                        m_iconTexture2.ReferenceCount++;
                    }
                }

                // if teritary icon enabled
                if (m_iconTexture3Show && m_iconTexture3Name.Trim() != String.Empty)
                {
                    if (m_iconTexture3 != null)
                    {
                        m_iconTexture3.ReferenceCount--;
                    }

                    m_iconTexture3 = (IconTexture)DrawArgs.Textures[m_iconTexture3Name];
                    if (m_iconTexture3 == null)
                    {
                        m_iconTexture3 = new IconTexture(drawArgs.device, m_iconTexture3Name);
                        DrawArgs.Textures.Add(m_iconTexture3Name, m_iconTexture3);
                    }

                    if (m_iconTexture3 != null)
                    {
                        m_iconTexture3.ReferenceCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }

            m_newTexture = false;

        }

        /// <summary>
        /// Helper function to render icon description.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        protected override void RenderDescription(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color)
        {
            string description = GeneralInfo() + DetailedInfo() + DescriptionInfo();

            if (description != null)
            {
                // Render description field
                DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Bottom;
                int left = 10;
                if (World.Settings.ShowLayerManager)
                    left += World.Settings.LayerManagerWidth;
                Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10);

                // Draw description
                rect.Offset(1, -1);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, descriptionColor);
            }
        }

        /// <summary>
        /// Called before icon render.  If the user has clicked on one of the GoTos head there now.
        /// Renders 3-D model and history trails.  If you want to only show models or trails on
        /// mouseover set the TrailShowDistance or ModelShowDistance to 0.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public override void PreRender(DrawArgs drawArgs, bool isMouseOver)
        {
            base.PreRender(drawArgs, isMouseOver);

            if (RenderTrail && ((DistanceToIcon < TrailShowDistance) || isMouseOver || IsHooked))
            {
                if (RenderTrail && m_lineFeature != null)
                {
                    if (!m_lineFeature.Initialized || m_lineFeature.NeedsUpdate)
                        m_lineFeature.Update(drawArgs);

                    m_lineFeature.Render(drawArgs);
                }
            }

            if (RenderModel && ((DistanceToIcon < ModelShowDistance) || isMouseOver || IsHooked))
            {
                if (m_modelFeature == null)
                    updateModel(isMouseOver);

                if (!m_modelFeature.Initialized)
                    m_modelFeature.Update(drawArgs);

                m_modelFeature.Render(drawArgs);
            }

            if (m_gotoMe)
                GoTo(drawArgs);
        }

        /// <summary>
        /// Called after icon render.  Always updates the hook form so the hook form still updates position even when the icon
        /// is out of view.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public override void PostRender(DrawArgs drawArgs, bool isMouseOver)
        {
            base.PostRender(drawArgs, isMouseOver);

            UpdateHookForm();
        }

        /// <summary>
        /// Helper function to render icon texture.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color">the color to render the icon</param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected override void RenderTexture(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, bool isMouseOver)
        {
            Matrix scaleTransform;
            Matrix rotationTransform;

            Matrix rotation2Transform;
            Matrix rotation3Transform;

            //Do Altitude depedent scaling for KMLIcons
            if (AutoScaleIcon)
            {
                float factor = 1;
                if (DistanceToIcon > MinIconZoomDistance)
                    factor -= (float)((DistanceToIcon - MinIconZoomDistance) / DistanceToIcon);
                if (factor < MinScaleFactor) factor = MinScaleFactor;

                XScale = factor * ((float)Width / this.IconTexture.Width);
                YScale = factor * ((float)Height / this.IconTexture.Height);
            }

            //scale and rotate image
            scaleTransform = Matrix.Scaling(this.XScale, this.YScale, 0);

            if (IsRotated)
                rotationTransform = Matrix.RotationZ((float)Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
            else
                rotationTransform = Matrix.Identity;

            sprite.Transform = scaleTransform * rotationTransform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
            sprite.Draw(this.IconTexture.Texture,
                new Vector3(this.IconTexture.Width >> 1, this.IconTexture.Height >> 1, 0),
                Vector3.Empty,
                color);

            if (m_iconTexture2Show)
            {
                Matrix tmpMatrix = sprite.Transform; 

                if (Texture2IsRotatedDifferent)
                {
                    if (Texture2UseHeading)
                        rotation2Transform = Matrix.RotationZ((float)Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
                    else
                        rotation2Transform = Matrix.RotationZ((float)Texture2Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
 
                   sprite.Transform = scaleTransform * rotation2Transform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
                }

                sprite.Draw(m_iconTexture2.Texture,
                    new Vector3(m_iconTexture2.Width >> 1, m_iconTexture2.Height >> 1, 0),
                    Vector3.Empty,
                    normalColor);

                // restore the main texture transform
                if (Texture2IsRotatedDifferent)
                    sprite.Transform = tmpMatrix;
            }

            if (m_iconTexture3Show)
            {
                if (Texture3IsRotatedDifferent)
                {
                    if (Texture3UseHeading)
                        rotation3Transform = Matrix.RotationZ((float)Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
                    else
                        rotation3Transform = Matrix.RotationZ((float)Texture3Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);

                    sprite.Transform = scaleTransform * rotation3Transform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
                }

                sprite.Draw(m_iconTexture3.Texture,
                    new Vector3(m_iconTexture3.Width >> 1, m_iconTexture3.Height >> 1, 0),
                    Vector3.Empty,
                    normalColor);
            }
            
            // Reset transform to prepare for text rendering later
            sprite.Transform = Matrix.Identity;
        }

        /// <summary>
        /// Forces display of lable if we are hooked
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color"></param>
        /// <param name="labelRectangles"></param>
        /// <param name="isMouseOver"></param>
        protected override void RenderLabel(DrawArgs drawArgs, Microsoft.DirectX.Direct3D.Sprite sprite, Microsoft.DirectX.Vector3 projectedPoint, int color, System.Collections.Generic.List<System.Drawing.Rectangle> labelRectangles, bool isMouseOver)
        {
            base.RenderLabel(drawArgs, sprite, projectedPoint, color, labelRectangles, (isMouseOver || IsHooked));
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="spd"></param>
        /// <param name="hdg"></param>
        /// <param name="time"></param>
        public virtual void SetPosition(double lat, double lon, double alt, double spd, double hdg, DateTime time)
        {
            // Set new values
            Latitude = lat;
            Longitude = lon;
            Altitude = alt;
            Speed = spd;
            Heading = hdg;

            SourceTime = time;
            UpdateTime = DateTime.Now;

            // Recalculate XYZ coordinates
            m_isUpdated = false;
        }

		#endregion

        /// <summary>
        /// Helper class that updates history trail and model position
        /// </summary>
        protected virtual void updateSubObjects()
        {
            if (IsSurfaceTrack)
            {
                Altitude = 0;
                IsAGL = true;  // force this just in case so we aren't under terrain
            }

            // Save current values
            PosData pos = new PosData();

            pos.lat = Latitude;
            pos.lon = Longitude;
            pos.alt = Altitude;
            pos.spd = Speed;
            pos.hdg = Rotation.Degrees;
            pos.sourceTime = m_sourceTime;
            pos.updateTime = m_updateTime;

            try
            {
                m_history.AddFirst(pos);

                if (m_history.Count > MaxPoints)
                    m_history.RemoveLast();

                // Add to line feature
                if (RenderTrail && m_lineFeature == null)
                {
                    m_lineFeature = new LineFeature(name + "_trail", DrawArgs.CurrentWorldStatic, null, null);
                    m_lineFeature.LineColor = System.Drawing.Color.White;
                    m_lineFeature.MaxPoints = MaxPoints;

                    if (IsAGL)
                        m_lineFeature.AltitudeMode = AltitudeMode.RelativeToGround;
                    else
                        m_lineFeature.AltitudeMode = AltitudeMode.Absolute;

                    m_lineFeature.Opacity = 128;
                    m_lineFeature.LineWidth = 3;

                    if (IsSurfaceTrack)
                    {
                        m_lineFeature.Extrude = false;
                        m_lineFeature.ExtrudeHeight = 0;
                        m_lineFeature.LineWidth = 1;
                    }
                    else
                    {
                        m_lineFeature.Extrude = true;
                        m_lineFeature.ExtrudeHeight = 1;
                        m_lineFeature.LineWidth = 1;
                        m_lineFeature.ExtrudeToGround = false;
                    }
                }

                if (RenderTrail)
                    m_lineFeature.AddPoint(Longitude, Latitude, Altitude);

                if (RenderModel && m_modelFeature != null)
                {
                    m_modelFeature.Longitude = (float)Longitude;
                    m_modelFeature.Latitude = (float)Latitude;
                    m_modelFeature.Altitude = (float)Altitude;

                    float rot = (float)(180 - Rotation.Degrees);
                    if (rot < 0)
                        rot += 360;
                    m_modelFeature.RotZ = rot;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// updates the ModelFeature to the new mesh, position and color
        /// </summary>
        public virtual void updateModel(bool forceUpdate)
        {
            if (forceUpdate || (RenderModel && !m_modelFeatureError && (DistanceToIcon < ModelShowDistance)))
            {
                if (m_modelFeature == null)
                {
                    try
                    {
                        // do for current models that happen to point right 90.
                        float rotZ = (float)(180 - Rotation.Degrees);
                        if (rotZ < 0)
                            rotZ += 360;

                        // use generic commercial airliner mesh
                        m_modelFeature = new ModelFeature(name,
                            DrawArgs.CurrentWorldStatic,
                            m_modelFilePath,
                            (float)Latitude,
                            (float)Longitude,
                            (float)Altitude,
                            ModelScale,
                            ModelRotX,
                            0,
                            rotZ);

                        m_modelFeature.IsElevationRelativeToGround = IsAGL;
                    }
                    catch
                    {
                        m_modelFeature = null;
                        m_modelFeatureError = true;
                    }
                }

                if (m_modelFeature != null)
                    m_modelFeature.TintColor = PointSpriteColor;
            }
        }

        public void UpdateHookForm()
        {
            // update hook form
            if (m_hookForm != null)
            {
                if (m_hookForm.Enabled)
                {
                    m_hookForm.Text = this.DMS();
                    m_hookGeneralLabel.Text = this.GeneralInfo();
                    m_hookDetailLabel.Text = this.DetailedInfo();
                    m_hookDescLabel.Text = this.DescriptionInfo();
                }
                else
                {
                    m_hookForm.Dispose();

                    m_hookTreeNode = null;
                    m_hookGeneralTreeNode = null;
                    m_hookDetailTreeNode = null;
                    m_hookDescTreeNode = null;
                    m_hookGeneralLabel = null;
                    m_hookDetailLabel = null;
                    m_hookDescLabel = null;

                    m_hookForm = null;
                    IsHooked = false;
                }
            }
        }

        public void GoTo(DrawArgs drawArgs)
        {
            drawArgs.WorldCamera.SetPosition(
                Latitude,
                Longitude,
                OnClickZoomHeading,
                OnClickZoomAltitude,
                OnClickZoomTilt);
            m_gotoMe = false;
        }

        public void GoTo()
        {
            m_gotoMe = true;
        }

        public virtual string GeneralInfo()
        {
            StringBuilder outString = new StringBuilder();

            outString.AppendFormat("{0:-10} {1}\n", "Id:", m_id);
            outString.AppendFormat("{0:-10} {1}\n", "Name:", Name);
            outString.AppendFormat("{0:-10} {1:00.00000}\n", "Lat:", Latitude);
            outString.AppendFormat("{0:-10} {1:000.00000}\n", "Lon:", Longitude);
            outString.AppendFormat("{0:-10} {1:F0}\n", "Alt:", Altitude);
            outString.AppendFormat("{0:-10} {1:F0}\n", "Spd:", Speed);
            outString.AppendFormat("{0:-10} {1:F0}\n", "Hdg:", Rotation.Degrees);

            return outString.ToString();
        }

        public virtual string DetailedInfo()
        {
            StringBuilder outString = new StringBuilder();

            outString.AppendFormat("{0:-10} {1}\n", "URL:", ClickableActionURL);

            return outString.ToString();
        }

        public virtual string DescriptionInfo()
        {
            StringBuilder outString = new StringBuilder();

            outString.AppendFormat("{0:-10} {1}\n", "Desc:", this.Description);

            return outString.ToString();
        }

        public virtual string Degrees()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.AppendFormat("Lat: {0:00.00000}", this.Latitude); //, (this.Latitude>=0) ? "N":"S" );
            retStr.AppendFormat(" Lon: {0:000.00000}", this.Longitude); //, (this.Longitude>=0)? "E":"W");
            retStr.AppendFormat(" Alt: {0:F0}", this.Altitude);

            return retStr.ToString();
        }

        /// <summary>
        /// Returns a string with this object's position in Degrees Minutes Seconds format
        /// </summary>
        /// <returns>Lat and Lon in DMS and Alt in meters</returns>
        public virtual string DMS()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.AppendFormat("Lat: {0}", WidgetUtilities.Degrees2DMS(this.Latitude, 'N', 'S'));
            retStr.AppendFormat(" Lon: {0}", WidgetUtilities.Degrees2DMS(this.Longitude, 'E', 'W'));
            retStr.AppendFormat(" Alt: {0:F0}", this.Altitude);

            return retStr.ToString();
        }

        void IconGotoMenuItem_Click(object sender, EventArgs s)
        {
            m_gotoMe = true;
        }

        void IconHookMenuItem_Click(object sender, EventArgs s)
        {
            if (m_hookForm == null)
            {
                m_hookForm = new FormWidget(" " + this.Name);

                m_hookForm.WidgetSize = new System.Drawing.Size(200, 250);
                m_hookForm.Location = new System.Drawing.Point(200, 120);
                m_hookForm.DestroyOnClose = true;

                m_hookTreeNode = new SimpleTreeNodeWidget("Info");
                m_hookTreeNode.IsRadioButton = true;
                m_hookTreeNode.Expanded = true;
                m_hookTreeNode.EnableCheck = false;

                m_hookGeneralLabel = new LabelWidget("");
                m_hookGeneralLabel.ClearOnRender = true;
                m_hookGeneralLabel.Format = DrawTextFormat.WordBreak;
                m_hookGeneralLabel.Location = new System.Drawing.Point(0, 0);
                m_hookGeneralLabel.AutoSize = true;
                m_hookGeneralLabel.UseParentWidth = false;

                m_hookGeneralTreeNode = new SimpleTreeNodeWidget("General");
                m_hookGeneralTreeNode.IsRadioButton = true;
                m_hookGeneralTreeNode.Expanded = true;
                m_hookGeneralTreeNode.EnableCheck = false;

                m_hookGeneralTreeNode.Add(m_hookGeneralLabel);
                m_hookTreeNode.Add(m_hookGeneralTreeNode);

                m_hookDetailLabel = new LabelWidget("");
                m_hookDetailLabel.ClearOnRender = true;
                m_hookDetailLabel.Format = DrawTextFormat.WordBreak;
                m_hookDetailLabel.Location = new System.Drawing.Point(0, 0);
                m_hookDetailLabel.AutoSize = true;
                m_hookDetailLabel.UseParentWidth = false;

                m_hookDetailTreeNode = new SimpleTreeNodeWidget("Detail");
                m_hookDetailTreeNode.IsRadioButton = true;
                m_hookDetailTreeNode.Expanded = true;
                m_hookDetailTreeNode.EnableCheck = false;

                m_hookDetailTreeNode.Add(m_hookDetailLabel);
                m_hookTreeNode.Add(m_hookDetailTreeNode);

                m_hookDescTreeNode = new SimpleTreeNodeWidget("Description");
                m_hookDescTreeNode.IsRadioButton = true;
                m_hookDescTreeNode.Expanded = true;
                m_hookDescTreeNode.EnableCheck = false;

                m_hookDescLabel = new LabelWidget("");
                m_hookDescLabel.ClearOnRender = true;
                m_hookDescLabel.Format = DrawTextFormat.WordBreak;
                m_hookDescLabel.Location = new System.Drawing.Point(0, 0);
                m_hookDescLabel.AutoSize = true;
                m_hookDescLabel.UseParentWidth = true;

                m_hookDescTreeNode.Add(m_hookDescLabel);
                m_hookTreeNode.Add(m_hookDescTreeNode);

                m_hookForm.Add(m_hookTreeNode);

                DrawArgs.NewRootWidget.ChildWidgets.Add(m_hookForm);
            }

            UpdateHookForm();
            m_hookForm.Enabled = true;
            m_hookForm.Visible = true;
            IsHooked = true;
        }

        void IconURLMenuItem_Click(object sender, EventArgs s)
        {
            try
            {
                if (!ClickableActionURL.Contains(@"worldwind://"))
                {
                    if (World.Settings.UseInternalBrowser && ClickableActionURL.StartsWith("http"))
                    {
                        SplitContainer sc = (SplitContainer)DrawArgs.ParentControl.Parent.Parent;
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
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }
        }
	}
}
