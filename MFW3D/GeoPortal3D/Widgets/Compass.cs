namespace Multispectral.GeoPortal3D.Widgets
{
    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using Utility;
    using MFW3D;
    using MFW3D.NewWidgets;
    using MFW3D.Widgets;

    public class Compass : MFW3D.NewWidgets.IWidget, MFW3D.NewWidgets.IInteractive
    {
        private CustomVertex.PositionTextured[] borderVertices = new CustomVertex.PositionTextured[4];
        private int clickBuffer = -2;
        private DrawArgs drawargs = null;
        private DateTime frameTime = DateTime.MinValue;
        private static TimeSpan frameTimeSpan = TimeSpan.FromMilliseconds(100.0);
        public bool IsLoaded = false;
        private bool isLoading = false;
        private bool isUpdating = false;
        private Color m_BackgroundColor = Color.FromArgb(0, 0, 0, 0);
        private Sprite m_CompassSprite = null;
        private SurfaceDescription m_CompasssurfaceDescription;
        private Texture m_CompassTexture = null;
        private string m_CompassUri = null;
        protected bool m_countHeight = true;
        protected bool m_countWidth = true;
        private string m_CurrentCompassUri = null;
        private bool m_Enabled = true;
        private Color m_ForeColor = Color.White;
        private bool m_IsDragging = false;
        private bool m_isMouseInside = false;
        private Point m_LastMousePosition = new Point(0, 0);
        protected MouseClickAction m_leftClickAction = null;
        private Point m_Location = new Point(0, 0);
        private static byte m_MinimumOpacity = 100;
        private string m_Name = "";
        private byte m_Opacity = 0xff;
        private static byte m_OpacityRate = 20;
        private MFW3D.NewWidgets.IWidget m_ParentWidget = null;
        protected MouseClickAction m_rightClickAction = null;
        private Size m_Size = new Size(0, 0);
        private object m_Tag = null;
        private string m_Text = "";
        private bool m_Visible = true;

        public event MouseEventHandler compass_OnMouseDownEvent;

        public event MouseEventHandler compass_OnMouseUpEvent;

        public event EventHandler OnMouseEnterEvent;

        public event EventHandler OnMouseLeaveEvent;

        public void Initialize(DrawArgs drawArgs)
        {
            try
            {
                if (!this.isUpdating)
                {
                    this.isUpdating = true;
                    if (this.loadTexture(drawArgs, this.m_CompassUri, ref this.m_Size, ref this.m_CompassTexture, ref this.m_CompasssurfaceDescription))
                    {
                        this.m_CurrentCompassUri = this.m_CompassUri;
                        this.IsLoaded = true;
                        this.isUpdating = false;
                        this.drawargs = drawArgs;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Write(exception.Message);
            }
        }

        private bool loadTexture(DrawArgs drawArgs, string stFileName, ref Size size, ref Texture texture, ref SurfaceDescription surfaceDescription)
        {
            if (stFileName == null)
            {
                return false;
            }
            if ((texture != null) && !texture.Disposed)
            {
                texture.Dispose();
                texture = null;
            }
            if (!File.Exists(stFileName))
            {
                return false;
            }
            texture = TextureLoader.FromFile(drawArgs.device, stFileName, 0, 0, 1, Usage.None, Microsoft.DirectX.Direct3D.Format.Dxt5, Pool.Managed, Microsoft.DirectX.Direct3D.Filter.Box, Microsoft.DirectX.Direct3D.Filter.Box, 0);
            surfaceDescription = texture.GetLevelDescription(0);
            size = new Size(surfaceDescription.Width, surfaceDescription.Height);
            return true;
        }

        public bool OnKeyDown(KeyEventArgs e)
        {
            return false;
        }

        public bool OnKeyPress(KeyPressEventArgs e)
        {
            return false;
        }

        public bool OnKeyUp(KeyEventArgs e)
        {
            return false;
        }

        public bool OnMouseDown(MouseEventArgs e)
        {
            if (!this.Visible)
            {
                return false;
            }
            bool flag = false;
            if (((e.X > (this.AbsoluteLocation.X + this.clickBuffer)) && (e.X < ((this.AbsoluteLocation.X + this.ClientSize.Width) - this.clickBuffer))) && ((e.Y > (this.AbsoluteLocation.Y + this.clickBuffer)) && (e.Y < ((this.AbsoluteLocation.Y + this.ClientSize.Height) - this.clickBuffer))))
            {
                this.m_IsDragging = true;
                if (this.compass_OnMouseDownEvent != null)
                {
                    this.compass_OnMouseDownEvent(this, e);
                }
                flag = true;
            }
            this.m_LastMousePosition = new Point(e.X, e.Y);
            return flag;
        }

        public bool OnMouseEnter(EventArgs e)
        {
            DrawArgs.MouseCursor = CursorType.Hand;
            return false;
        }

        public bool OnMouseLeave(EventArgs e)
        {
            DrawArgs.MouseCursor = CursorType.Arrow;
            this.m_IsDragging = false;
            return true;
        }

        public bool OnMouseMove(MouseEventArgs e)
        {
            bool flag = false;
            if (!this.Visible)
            {
                return false;
            }
            int num = e.X - this.m_LastMousePosition.X;
            int num2 = e.Y - this.m_LastMousePosition.Y;
            DrawArgs.MouseCursor = CursorType.Hand;
            float num3 = ((float) num) / ((float) this.m_CompasssurfaceDescription.Width);
            float single1 = ((float) num2) / ((float) this.m_CompasssurfaceDescription.Height);
            if (this.m_IsDragging && (this.drawargs != null))
            {
                int num4 = 1;
                if ((e.Y > ((this.AbsoluteLocation.Y + (this.m_Size.Height / 2)) + this.clickBuffer)) && (e.Y < ((this.AbsoluteLocation.Y + this.m_Size.Height) - this.clickBuffer)))
                {
                    num4 = -1;
                }
                Angle roll = Angle.FromRadians((double) ((num4 * -num3) * World.Settings.CameraRotationSpeed));
                this.drawargs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, roll);
                flag = true;
            }
            this.m_LastMousePosition = new Point(e.X, e.Y);
            return flag;
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            if (!this.Visible)
            {
                return false;
            }
            bool flag = false;
            this.m_IsDragging = false;
            if (((e.X > (this.AbsoluteLocation.X + this.clickBuffer)) && (e.X < ((this.AbsoluteLocation.X + this.m_Size.Width) - this.clickBuffer))) && ((e.Y > (this.AbsoluteLocation.Y + this.clickBuffer)) && (e.Y < ((this.AbsoluteLocation.Y + this.m_Size.Height) - this.clickBuffer))))
            {
                if (this.compass_OnMouseUpEvent != null)
                {
                    this.compass_OnMouseUpEvent(this, e);
                }
                flag = true;
            }
            this.m_LastMousePosition = new Point(e.X, e.Y);
            return flag;
        }

        public bool OnMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public void Render(DrawArgs drawArgs)
        {
            if (this.m_Visible)
            {
                if (this.m_CompassTexture == null)
                {
                    this.Initialize(drawArgs);
                }
                if (((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - 10)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.ClientSize.Width) + 10))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y + 5)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.ClientSize.Height) + 5))))
                {
                    if (!this.m_isMouseInside)
                    {
                        this.m_isMouseInside = true;
                        this.OnMouseEnter(null);
                        if (this.OnMouseEnterEvent != null)
                        {
                            this.OnMouseEnterEvent(this, null);
                        }
                    }
                }
                else if (this.m_isMouseInside)
                {
                    this.m_isMouseInside = false;
                    this.OnMouseLeave(null);
                    if (this.OnMouseLeaveEvent != null)
                    {
                        this.OnMouseLeaveEvent(this, null);
                    }
                }
                if (DateTime.Now.Subtract(this.frameTime) > frameTimeSpan)
                {
                    if (!this.m_isMouseInside)
                    {
                        if (this.m_Opacity > m_MinimumOpacity)
                        {
                            this.m_Opacity = (byte) (this.m_Opacity - m_OpacityRate);
                        }
                        else
                        {
                            this.m_Opacity = m_MinimumOpacity;
                        }
                    }
                    else if (this.m_Opacity < (0xff - m_OpacityRate))
                    {
                        this.m_Opacity = (byte) (this.m_Opacity + m_OpacityRate);
                    }
                    else
                    {
                        this.m_Opacity = 0xff;
                    }
                    this.frameTime = DateTime.Now;
                }
                Utilities.DrawBox(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.ClientSize.Width, this.ClientSize.Height, 0f, this.m_BackgroundColor.ToArgb(), drawArgs.device);
                if ((this.m_CompassTexture != null) && !this.isLoading)
                {
                    bool fogEnable = drawArgs.device.RenderState.FogEnable;
                    drawArgs.device.RenderState.FogEnable = false;
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    drawArgs.device.SetTexture(0, this.m_CompassTexture);
                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.BlendCurrentAlpha;
                    Point point = new Point(this.AbsoluteLocation.X, this.AbsoluteLocation.Y);
                    Point point2 = new Point(this.AbsoluteLocation.X + this.m_Size.Width, this.AbsoluteLocation.Y);
                    new Point(this.AbsoluteLocation.X, this.AbsoluteLocation.Y + this.m_Size.Height);
                    Point point3 = new Point(this.AbsoluteLocation.X + this.m_Size.Width, this.AbsoluteLocation.Y + this.m_Size.Height);
                    if (this.m_CompassSprite == null)
                    {
                        this.m_CompassSprite = new Sprite(drawArgs.device);
                    }
                    this.m_CompassSprite.Begin(SpriteFlags.AlphaBlend);
                    float x = ((float) (point2.X - point.X)) / ((float) this.m_CompasssurfaceDescription.Width);
                    float y = ((float) (point3.Y - point2.Y)) / ((float) this.m_CompasssurfaceDescription.Height);
                    this.m_CompassSprite.Transform = Microsoft.DirectX.Matrix.Scaling(x, y, 0f);
                    this.m_CompassSprite.Transform *= Microsoft.DirectX.Matrix.RotationZ((float) -drawArgs.WorldCamera.Heading.Radians);
                    this.m_CompassSprite.Transform *= Microsoft.DirectX.Matrix.Translation(0.5f * (point.X + point2.X), 0.5f * (point2.Y + point3.Y), 0f);
                    this.m_CompassSprite.Draw(this.m_CompassTexture, new Microsoft.DirectX.Vector3((float) (this.m_CompasssurfaceDescription.Width / 2), (float) (this.m_CompasssurfaceDescription.Height / 2), 0f), Microsoft.DirectX.Vector3.Empty, Color.FromArgb(this.m_Opacity, 0xff, 0xff, 0xff).ToArgb());
                    drawArgs.device.RenderState.FogEnable = fogEnable;
                    this.m_CompassSprite.Transform = Microsoft.DirectX.Matrix.Identity;
                    this.m_CompassSprite.End();
                }
            }
        }

        public Point AbsoluteLocation
        {
            get
            {
                if (this.m_ParentWidget != null)
                {
                    return new Point(this.m_Location.X + this.m_ParentWidget.ClientLocation.X, this.m_Location.Y + this.m_ParentWidget.ClientLocation.Y);
                }
                return this.m_Location;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return this.m_BackgroundColor;
            }
            set
            {
                this.m_BackgroundColor = value;
            }
        }

        public MFW3D.NewWidgets.IWidgetCollection ChildWidgets
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Point ClientLocation
        {
            get
            {
                return this.m_Location;
            }
            set
            {
                this.m_Location = value;
            }
        }

        public Size ClientSize
        {
            get
            {
                return this.m_Size;
            }
            set
            {
                this.m_Size = value;
            }
        }

        public string CompassUri
        {
            get
            {
                return this.m_CompassUri;
            }
            set
            {
                this.m_CompassUri = value;
            }
        }

        public bool CountHeight
        {
            get
            {
                return this.m_countHeight;
            }
            set
            {
                this.m_countHeight = value;
            }
        }

        public bool CountWidth
        {
            get
            {
                return this.m_countWidth;
            }
            set
            {
                this.m_countWidth = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.m_Enabled;
            }
            set
            {
                this.m_Enabled = value;
            }
        }

        public Color ForeColor
        {
            get
            {
                return this.m_ForeColor;
            }
            set
            {
                this.m_ForeColor = value;
            }
        }

        public MouseClickAction LeftClickAction
        {
            get
            {
                return this.m_leftClickAction;
            }
            set
            {
                this.m_leftClickAction = value;
            }
        }

        public Point Location
        {
            get
            {
                return this.m_Location;
            }
            set
            {
                this.m_Location = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        public byte Opacity
        {
            get
            {
                return this.m_Opacity;
            }
            set
            {
                this.m_Opacity = value;
            }
        }

        public MFW3D.NewWidgets.IWidget ParentWidget
        {
            get
            {
                return this.m_ParentWidget;
            }
            set
            {
                this.m_ParentWidget = value;
            }
        }

        public MouseClickAction RightClickAction
        {
            get
            {
                return this.m_rightClickAction;
            }
            set
            {
                this.m_rightClickAction = value;
            }
        }

        public object Tag
        {
            get
            {
                return this.m_Tag;
            }
            set
            {
                this.m_Tag = value;
            }
        }

        public string Text
        {
            get
            {
                return this.m_Text;
            }
            set
            {
                this.m_Text = value;
            }
        }

        public bool Visible
        {
            get
            {
                return this.m_Visible;
            }
            set
            {
                this.m_Visible = value;
            }
        }

        public Size WidgetSize
        {
            get
            {
                return this.m_Size;
            }
            set
            {
                this.m_Size = value;
            }
        }
    }
}
