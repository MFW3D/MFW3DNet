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

    public class Slider : MFW3D.NewWidgets.IWidget, MFW3D.NewWidgets.IInteractive
    {
        private int clickBuffer = -2;
        private DateTime frameTime = DateTime.MinValue;
        private static TimeSpan frameTimeSpan = TimeSpan.FromMilliseconds(100.0);
        public bool IsLoaded = false;
        private bool isLoading = false;
        private bool isUpdating = false;
        private Color m_BackgroundColor = Color.FromArgb(0, 0, 0, 0);
        protected bool m_countHeight = true;
        protected bool m_countWidth = true;
        private string m_currentHandleUri = null;
        private string m_currentHighImageUri = null;
        private string m_currentLowImageUri = null;
        private bool m_Enabled = true;
        private Color m_ForeColor = Color.White;
        private Point m_HandleLocation = new Point(0, 0);
        private byte m_HandleOpacity = 0xff;
        private Size m_HandleSize = new Size(0, 0);
        private Sprite m_HandleSprite = null;
        private SurfaceDescription m_HandlesurfaceDescription;
        private Texture m_HandleTexture = null;
        private string m_HandleUri = null;
        private Point m_HighImageLocation = new Point(0, 0);
        private Size m_HighImageSize = new Size(0, 0);
        private Sprite m_HighImageSprite = null;
        private SurfaceDescription m_HighImagesurfaceDescription;
        private Texture m_HighImageTexture = null;
        private string m_HighImageUri = null;
        private bool m_IsDragging = false;
        private bool m_isMouseInside = false;
        private Point m_LastMousePosition = new Point(0, 0);
        protected MouseClickAction m_leftClickAction = null;
        private Point m_Location = new Point(0, 0);
        private Point m_LowImageLocation = new Point(0, 0);
        private Size m_LowImageSize = new Size(0, 0);
        private Sprite m_LowImageSprite = null;
        private SurfaceDescription m_LowImagesurfaceDescription;
        private Texture m_LowImageTexture = null;
        private string m_LowImageUri = null;
        private decimal m_MaximumValue = 1M;
        private static byte m_MinimumOpacity = 100;
        private decimal m_MinimumValue = -1M;
        private string m_Name = "";
        private byte m_Opacity = 0xff;
        private static byte m_OpacityRate = 20;
        private MFW3D.NewWidgets.IWidget m_ParentWidget = null;
        protected MouseClickAction m_rightClickAction = null;
        private Size m_Size = new Size(0, 0);
        private SliderType m_SliderType = SliderType.Horizontal;
        private Color m_StickBackgroundColor = Color.FromArgb(0, 0, 0, 0);
        private Point m_StickLocation = new Point(0, 0);
        private Size m_StickSize = new Size(0, 0);
        private object m_Tag = null;
        private string m_Text = "";
        private decimal m_Value = 0M;
        private bool m_Visible = true;

        public event MouseEventHandler handle_OnMouseDownEvent;

        public event MouseEventHandler handle_OnMouseUpEvent;

        public event MouseEventHandler high_OnMouseDownEvent;

        public event MouseEventHandler high_OnMouseUpEvent;

        public event MouseEventHandler low_OnMouseDownEvent;

        public event MouseEventHandler low_OnMouseUpEvent;

        public event EventHandler OnMouseEnterEvent;

        public event EventHandler OnMouseLeaveEvent;

        public Slider(SliderType sliderType)
        {
            this.m_SliderType = sliderType;
        }

        private decimal getHandleValue()
        {
            decimal width = 0M;
            decimal num2 = 0M;
            decimal num3 = 0M;
            if (this.m_SliderType == SliderType.Horizontal)
            {
                width = this.m_StickSize.Width;
                num2 = this.m_HandleLocation.X - this.m_StickLocation.X;
            }
            else
            {
                width = this.m_StickSize.Height;
                num2 = this.m_HandleLocation.Y - this.m_StickLocation.Y;
            }
            if (num2 > 0M)
            {
                num3 = num2 / width;
            }
            return (this.MinimumValue + ((this.MaximumValue - this.MinimumValue) * num3));
        }

        public void Initialize(DrawArgs drawArgs)
        {
            try
            {
                if (!this.isUpdating)
                {
                    this.isUpdating = true;
                    if (this.loadTexture(this.m_LowImageUri, ref this.m_LowImageSize, ref this.m_LowImageTexture, ref this.m_LowImagesurfaceDescription))
                    {
                        this.m_currentLowImageUri = this.m_LowImageUri;
                        if (this.m_SliderType == SliderType.Horizontal)
                        {
                            this.m_LowImageLocation = new Point(0, (this.ClientSize.Height - this.m_LowImageSize.Height) / 2);
                        }
                        else
                        {
                            this.m_LowImageLocation = new Point((this.ClientSize.Width - this.m_LowImageSize.Width) / 2, this.ClientSize.Height - this.m_LowImageSize.Height);
                        }
                        if (this.loadTexture(this.m_HighImageUri, ref this.m_HighImageSize, ref this.m_HighImageTexture, ref this.m_HighImagesurfaceDescription))
                        {
                            this.m_currentHighImageUri = this.m_HighImageUri;
                            if (this.m_SliderType == SliderType.Horizontal)
                            {
                                this.m_HighImageLocation = new Point(this.ClientSize.Width - this.m_HighImageSize.Width, (this.ClientSize.Height - this.m_HighImageSize.Height) / 2);
                            }
                            else
                            {
                                this.m_HighImageLocation = new Point((this.ClientSize.Width - this.m_LowImageSize.Width) / 2, 0);
                            }
                            if (this.m_SliderType == SliderType.Horizontal)
                            {
                                this.m_StickSize = new Size(((this.ClientSize.Width - this.m_LowImageSize.Width) - this.m_HighImageSize.Width) - 4, 4);
                                this.m_StickLocation = new Point((this.m_LowImageLocation.X + this.m_LowImageSize.Width) + 2, (this.ClientSize.Height - this.m_StickSize.Height) / 2);
                            }
                            else
                            {
                                this.m_StickSize = new Size(4, ((this.ClientSize.Height - this.m_LowImageSize.Height) - this.m_HighImageSize.Height) - 4);
                                this.m_StickLocation = new Point((this.ClientSize.Width - this.m_StickSize.Width) / 2, (this.m_HighImageLocation.Y + this.m_HighImageSize.Height) + 2);
                            }
                            if (this.loadTexture(this.m_HandleUri, ref this.m_HandleSize, ref this.m_HandleTexture, ref this.m_HandlesurfaceDescription))
                            {
                                this.m_currentHandleUri = this.m_HandleUri;
                                this.m_HandleLocation = new Point((this.m_StickLocation.X + (this.m_StickSize.Width / 2)) - (this.m_HandleSize.Width / 2), (this.m_StickLocation.Y + (this.m_StickSize.Height / 2)) - (this.m_HandleSize.Height / 2));
                                this.IsLoaded = true;
                                this.isUpdating = false;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Write(exception.Message);
            }
        }

        private bool loadTexture(string stFileName, ref Size size, ref Texture texture, ref SurfaceDescription surfaceDescription)
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
            texture = ImageHelper.LoadTexture(stFileName);
            surfaceDescription = texture.GetLevelDescription(0);
            int width = size.Width;
            int height = size.Height;
            if (size.Width == 0)
            {
                width = surfaceDescription.Width;
            }
            if (size.Height == 0)
            {
                height = surfaceDescription.Height;
            }
            size = new Size(width, height);
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
            if (((e.X > ((this.AbsoluteLocation.X + this.m_HandleLocation.X) + this.clickBuffer)) && (e.X < (((this.AbsoluteLocation.X + this.m_HandleLocation.X) + this.m_HandleSize.Width) - this.clickBuffer))) && ((e.Y > ((this.AbsoluteLocation.Y + this.m_HandleLocation.Y) + this.clickBuffer)) && (e.Y < (((this.AbsoluteLocation.Y + this.m_HandleLocation.Y) + this.m_HandleSize.Height) - this.clickBuffer))))
            {
                this.m_IsDragging = true;
                if (this.handle_OnMouseDownEvent != null)
                {
                    this.handle_OnMouseDownEvent(this, e);
                }
                flag = true;
            }
            if (((!flag && (e.X > ((this.AbsoluteLocation.X + this.m_LowImageLocation.X) + this.clickBuffer))) && ((e.X < (((this.AbsoluteLocation.X + this.m_LowImageLocation.X) + this.m_LowImageSize.Width) - this.clickBuffer)) && (e.Y > ((this.AbsoluteLocation.Y + this.m_LowImageLocation.Y) + this.clickBuffer)))) && (e.Y < (((this.AbsoluteLocation.Y + this.m_LowImageLocation.Y) + this.m_LowImageSize.Height) - this.clickBuffer)))
            {
                if (this.low_OnMouseDownEvent != null)
                {
                    this.low_OnMouseDownEvent(this, e);
                }
                flag = true;
            }
            if (((!flag && (e.X > ((this.AbsoluteLocation.X + this.m_HighImageLocation.X) + this.clickBuffer))) && ((e.X < (((this.AbsoluteLocation.X + this.m_HighImageLocation.X) + this.m_HighImageSize.Width) - this.clickBuffer)) && (e.Y > ((this.AbsoluteLocation.Y + this.m_HighImageLocation.Y) + this.clickBuffer)))) && (e.Y < (((this.AbsoluteLocation.Y + this.m_HighImageLocation.Y) + this.m_HighImageSize.Height) - this.clickBuffer)))
            {
                if (this.high_OnMouseDownEvent != null)
                {
                    this.high_OnMouseDownEvent(this, e);
                }
                flag = true;
            }
            this.m_LastMousePosition = new Point(e.X, e.Y);
            return flag;
        }

        public bool OnMouseEnter(EventArgs e)
        {
            return false;
        }

        public bool OnMouseLeave(EventArgs e)
        {
            bool flag = false;
            this.m_IsDragging = false;
            flag = true;
            this.Value = 0M;
            return flag;
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
            if (this.m_IsDragging)
            {
                if (this.m_SliderType == SliderType.Horizontal)
                {
                    this.m_HandleLocation.X += num;
                }
                else
                {
                    this.m_HandleLocation.Y += num2;
                }
                if (this.m_SliderType == SliderType.Horizontal)
                {
                    if (this.m_HandleLocation.X < this.m_StickLocation.X)
                    {
                        this.m_HandleLocation.X = this.m_StickLocation.X;
                    }
                    if (this.m_HandleLocation.X > ((this.m_StickLocation.X + this.m_StickSize.Width) - this.m_HandleSize.Width))
                    {
                        this.m_HandleLocation.X = (this.m_StickLocation.X + this.m_StickSize.Width) - this.m_HandleSize.Width;
                    }
                }
                else
                {
                    if (this.m_HandleLocation.Y < this.m_StickLocation.Y)
                    {
                        this.m_HandleLocation.Y = this.m_StickLocation.Y;
                    }
                    if (this.m_HandleLocation.Y > ((this.m_StickLocation.Y + this.m_StickSize.Height) - this.m_HandleSize.Height))
                    {
                        this.m_HandleLocation.Y = (this.m_StickLocation.Y + this.m_StickSize.Height) - this.m_HandleSize.Height;
                    }
                }
                this.m_Value = this.getHandleValue();
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
            if (((e.X > ((this.AbsoluteLocation.X + this.m_HandleLocation.X) + this.clickBuffer)) && (e.X < (((this.AbsoluteLocation.X + this.m_HandleLocation.X) + this.m_HandleSize.Width) - this.clickBuffer))) && ((e.Y > ((this.AbsoluteLocation.Y + this.m_HandleLocation.Y) + this.clickBuffer)) && (e.Y < (((this.AbsoluteLocation.Y + this.m_HandleLocation.Y) + this.m_HandleSize.Height) - this.clickBuffer))))
            {
                if (this.handle_OnMouseUpEvent != null)
                {
                    this.handle_OnMouseUpEvent(this, e);
                }
                flag = true;
            }
            if (((!flag && (e.X > ((this.AbsoluteLocation.X + this.m_LowImageLocation.X) + this.clickBuffer))) && ((e.X < (((this.AbsoluteLocation.X + this.m_LowImageLocation.X) + this.m_LowImageSize.Width) - this.clickBuffer)) && (e.Y > ((this.AbsoluteLocation.Y + this.m_LowImageLocation.Y) + this.clickBuffer)))) && (e.Y < (((this.AbsoluteLocation.Y + this.m_LowImageLocation.Y) + this.m_LowImageSize.Height) - this.clickBuffer)))
            {
                if (this.low_OnMouseUpEvent != null)
                {
                    this.low_OnMouseUpEvent(this, e);
                }
                flag = true;
            }
            if (((!flag && (e.X > ((this.AbsoluteLocation.X + this.m_HighImageLocation.X) + this.clickBuffer))) && ((e.X < (((this.AbsoluteLocation.X + this.m_HighImageLocation.X) + this.m_HighImageSize.Width) - this.clickBuffer)) && (e.Y > ((this.AbsoluteLocation.Y + this.m_HighImageLocation.Y) + this.clickBuffer)))) && (e.Y < (((this.AbsoluteLocation.Y + this.m_HighImageLocation.Y) + this.m_HighImageSize.Height) - this.clickBuffer)))
            {
                if (this.high_OnMouseUpEvent != null)
                {
                    this.high_OnMouseUpEvent(this, e);
                }
                flag = true;
            }
            this.Value = 0M;
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
                if (this.m_HandleTexture == null)
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
                            this.m_HandleOpacity = this.m_Opacity;
                        }
                        else
                        {
                            this.m_HandleOpacity = 0;
                            this.m_Opacity = m_MinimumOpacity;
                        }
                    }
                    else
                    {
                        if (this.m_Opacity < (0xff - m_OpacityRate))
                        {
                            this.m_Opacity = (byte) (this.m_Opacity + m_OpacityRate);
                        }
                        else
                        {
                            this.m_Opacity = 0xff;
                        }
                        this.m_HandleOpacity = this.m_Opacity;
                    }
                    this.frameTime = DateTime.Now;
                }
                if ((((this.m_HandleTexture != null) && (this.m_currentHandleUri != this.m_HandleUri)) || ((this.m_LowImageTexture != null) && (this.m_currentLowImageUri != this.m_LowImageUri))) || ((this.m_HighImageTexture != null) && (this.m_currentHighImageUri != this.m_HighImageUri)))
                {
                    this.Initialize(drawArgs);
                }
                Utilities.DrawBox(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.ClientSize.Width, this.ClientSize.Height, 0f, this.m_BackgroundColor.ToArgb(), drawArgs.device);
                Utilities.DrawBox(this.AbsoluteLocation.X + this.m_StickLocation.X, this.AbsoluteLocation.Y + this.m_StickLocation.Y, this.m_StickSize.Width, this.m_StickSize.Height, 0f, this.m_StickBackgroundColor.ToArgb(), drawArgs.device);
                Microsoft.DirectX.Vector2[] linePoints = new Microsoft.DirectX.Vector2[5];
                linePoints[0].X = this.AbsoluteLocation.X + this.m_StickLocation.X;
                linePoints[0].Y = this.AbsoluteLocation.Y + this.m_StickLocation.Y;
                linePoints[1].X = (this.AbsoluteLocation.X + this.m_StickLocation.X) + this.m_StickSize.Width;
                linePoints[1].Y = this.AbsoluteLocation.Y + this.m_StickLocation.Y;
                linePoints[2].X = (this.AbsoluteLocation.X + this.m_StickLocation.X) + this.m_StickSize.Width;
                linePoints[2].Y = (this.AbsoluteLocation.Y + this.m_StickLocation.Y) + this.m_StickSize.Height;
                linePoints[3].X = this.AbsoluteLocation.X + this.m_StickLocation.X;
                linePoints[3].Y = (this.AbsoluteLocation.Y + this.m_StickLocation.Y) + this.m_StickSize.Height;
                linePoints[4].X = this.AbsoluteLocation.X + this.m_StickLocation.X;
                linePoints[4].Y = this.AbsoluteLocation.Y + this.m_StickLocation.Y;
                Utilities.DrawLine(linePoints, Color.FromArgb(this.m_Opacity, this.m_ForeColor.R, this.m_ForeColor.G, this.m_ForeColor.B).ToArgb(), drawArgs.device);
                if ((this.m_HandleTexture != null) && !this.isLoading)
                {
                    drawArgs.device.SetTexture(0, this.m_HandleTexture);
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    Point point = new Point(this.AbsoluteLocation.X + this.m_HandleLocation.X, this.AbsoluteLocation.Y + this.m_HandleLocation.Y);
                    Point point2 = new Point((this.AbsoluteLocation.X + this.m_HandleLocation.X) + this.m_HandleSize.Width, this.AbsoluteLocation.Y + this.m_HandleLocation.Y);
                    new Point(this.AbsoluteLocation.X + this.m_HandleLocation.X, (this.AbsoluteLocation.Y + this.m_HandleLocation.Y) + this.m_HandleSize.Height);
                    Point point3 = new Point((this.AbsoluteLocation.X + this.m_HandleLocation.X) + this.m_HandleSize.Width, (this.AbsoluteLocation.Y + this.m_HandleLocation.Y) + this.m_HandleSize.Height);
                    if (this.m_HandleSprite == null)
                    {
                        this.m_HandleSprite = new Sprite(drawArgs.device);
                    }
                    this.m_HandleSprite.Begin(SpriteFlags.AlphaBlend);
                    float x = ((float) (point2.X - point.X)) / ((float) this.m_HandlesurfaceDescription.Width);
                    float y = ((float) (point3.Y - point2.Y)) / ((float) this.m_HandlesurfaceDescription.Height);
                    this.m_HandleSprite.Transform = Microsoft.DirectX.Matrix.Scaling(x, y, 0f);
                    this.m_HandleSprite.Transform *= Microsoft.DirectX.Matrix.Translation(0.5f * (point.X + point2.X), 0.5f * (point2.Y + point3.Y), 0f);
                    this.m_HandleSprite.Draw(this.m_HandleTexture, new Microsoft.DirectX.Vector3((float) (this.m_HandlesurfaceDescription.Width / 2), (float) (this.m_HandlesurfaceDescription.Height / 2), 0f), Microsoft.DirectX.Vector3.Empty, Color.FromArgb(this.m_HandleOpacity, 0xff, 0xff, 0xff).ToArgb());
                    this.m_HandleSprite.Transform = Microsoft.DirectX.Matrix.Identity;
                    this.m_HandleSprite.End();
                }
                if ((this.m_LowImageTexture != null) && !this.isLoading)
                {
                    drawArgs.device.SetTexture(0, this.m_LowImageTexture);
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    Point point4 = new Point(this.AbsoluteLocation.X + this.m_LowImageLocation.X, this.AbsoluteLocation.Y + this.m_LowImageLocation.Y);
                    Point point5 = new Point((this.AbsoluteLocation.X + this.m_LowImageLocation.X) + this.m_LowImageSize.Width, this.AbsoluteLocation.Y + this.m_LowImageLocation.Y);
                    new Point(this.AbsoluteLocation.X + this.m_LowImageLocation.X, (this.AbsoluteLocation.Y + this.m_LowImageLocation.Y) + this.m_LowImageSize.Height);
                    Point point6 = new Point((this.AbsoluteLocation.X + this.m_LowImageLocation.X) + this.m_LowImageSize.Width, (this.AbsoluteLocation.Y + this.m_LowImageLocation.Y) + this.m_LowImageSize.Height);
                    if (this.m_LowImageSprite == null)
                    {
                        this.m_LowImageSprite = new Sprite(drawArgs.device);
                    }
                    this.m_LowImageSprite.Begin(SpriteFlags.AlphaBlend);
                    float num3 = ((float) (point5.X - point4.X)) / ((float) this.m_LowImagesurfaceDescription.Width);
                    float num4 = ((float) (point6.Y - point5.Y)) / ((float) this.m_LowImagesurfaceDescription.Height);
                    this.m_LowImageSprite.Transform = Microsoft.DirectX.Matrix.Scaling(num3, num4, 0f);
                    this.m_LowImageSprite.Transform *= Microsoft.DirectX.Matrix.Translation(0.5f * (point4.X + point5.X), 0.5f * (point5.Y + point6.Y), 0f);
                    this.m_LowImageSprite.Draw(this.m_LowImageTexture, new Microsoft.DirectX.Vector3((float) (this.m_LowImagesurfaceDescription.Width / 2), (float) (this.m_LowImagesurfaceDescription.Height / 2), 0f), Microsoft.DirectX.Vector3.Empty, Color.FromArgb(this.m_Opacity, 0xff, 0xff, 0xff).ToArgb());
                    this.m_LowImageSprite.Transform = Microsoft.DirectX.Matrix.Identity;
                    this.m_LowImageSprite.End();
                }
                if ((this.m_HighImageTexture != null) && !this.isLoading)
                {
                    drawArgs.device.SetTexture(0, this.m_HighImageTexture);
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    Point point7 = new Point(this.AbsoluteLocation.X + this.m_HighImageLocation.X, this.AbsoluteLocation.Y + this.m_HighImageLocation.Y);
                    Point point8 = new Point((this.AbsoluteLocation.X + this.m_HighImageLocation.X) + this.m_HighImageSize.Width, this.AbsoluteLocation.Y + this.m_HighImageLocation.Y);
                    new Point(this.AbsoluteLocation.X + this.m_HighImageLocation.X, (this.AbsoluteLocation.Y + this.m_HighImageLocation.Y) + this.m_HighImageSize.Height);
                    Point point9 = new Point((this.AbsoluteLocation.X + this.m_HighImageLocation.X) + this.m_HighImageSize.Width, (this.AbsoluteLocation.Y + this.m_HighImageLocation.Y) + this.m_HighImageSize.Height);
                    if (this.m_HighImageSprite == null)
                    {
                        this.m_HighImageSprite = new Sprite(drawArgs.device);
                    }
                    this.m_HighImageSprite.Begin(SpriteFlags.AlphaBlend);
                    float num5 = ((float) (point8.X - point7.X)) / ((float) this.m_HighImagesurfaceDescription.Width);
                    float num6 = ((float) (point9.Y - point8.Y)) / ((float) this.m_HighImagesurfaceDescription.Height);
                    this.m_HighImageSprite.Transform = Microsoft.DirectX.Matrix.Scaling(num5, num6, 0f);
                    this.m_HighImageSprite.Transform *= Microsoft.DirectX.Matrix.Translation(0.5f * (point7.X + point8.X), 0.5f * (point8.Y + point9.Y), 0f);
                    this.m_HighImageSprite.Draw(this.m_HighImageTexture, new Microsoft.DirectX.Vector3((float) (this.m_HighImagesurfaceDescription.Width / 2), (float) (this.m_HighImagesurfaceDescription.Height / 2), 0f), Microsoft.DirectX.Vector3.Empty, Color.FromArgb(this.m_Opacity, 0xff, 0xff, 0xff).ToArgb());
                    this.m_HighImageSprite.Transform = Microsoft.DirectX.Matrix.Identity;
                    this.m_HighImageSprite.End();
                }
            }
        }

        private void setHandlePosition()
        {
            decimal num = this.MaximumValue - this.MinimumValue;
            decimal num2 = this.Value - this.MinimumValue;
            decimal num3 = 0M;
            if (num2 > 0M)
            {
                num3 = num2 / num;
            }
            if (this.m_SliderType == SliderType.Horizontal)
            {
                this.m_HandleLocation.X = (this.m_StickLocation.X + ((int) (this.m_StickSize.Width * num3))) - (this.m_HandleSize.Width / 2);
            }
            else
            {
                this.m_HandleLocation.Y = (this.m_StickLocation.Y + ((int) (this.m_StickSize.Height * num3))) - (this.m_HandleSize.Height / 2);
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

        public Size HandleSize
        {
            get
            {
                return this.m_HandleSize;
            }
            set
            {
                this.m_HandleSize = value;
            }
        }

        public string HandleUri
        {
            get
            {
                return this.m_HandleUri;
            }
            set
            {
                this.m_HandleUri = value;
            }
        }

        public Size HighImageSize
        {
            get
            {
                return this.m_HighImageSize;
            }
            set
            {
                this.m_HighImageSize = value;
            }
        }

        public string HighImageUri
        {
            get
            {
                return this.m_HighImageUri;
            }
            set
            {
                this.m_HighImageUri = value;
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

        public Size LowImageSize
        {
            get
            {
                return this.m_LowImageSize;
            }
            set
            {
                this.m_LowImageSize = value;
            }
        }

        public string LowImageUri
        {
            get
            {
                return this.m_LowImageUri;
            }
            set
            {
                this.m_LowImageUri = value;
            }
        }

        public decimal MaximumValue
        {
            get
            {
                return this.m_MaximumValue;
            }
            set
            {
                this.m_MaximumValue = value;
                if (this.m_MaximumValue < this.m_MinimumValue)
                {
                    this.m_MaximumValue = this.m_MinimumValue;
                }
            }
        }

        public decimal MinimumValue
        {
            get
            {
                return this.m_MinimumValue;
            }
            set
            {
                this.m_MinimumValue = value;
                if (this.m_MinimumValue > this.m_MaximumValue)
                {
                    this.m_MinimumValue = this.m_MaximumValue;
                }
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

        public Color StickBackgroundColor
        {
            get
            {
                return this.m_StickBackgroundColor;
            }
            set
            {
                this.m_StickBackgroundColor = value;
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

        public decimal Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
                if (this.m_Value > this.m_MaximumValue)
                {
                    this.m_Value = this.m_MaximumValue;
                }
                else if (this.m_Value < this.m_MinimumValue)
                {
                    this.m_Value = this.m_MinimumValue;
                }
                this.setHandlePosition();
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
