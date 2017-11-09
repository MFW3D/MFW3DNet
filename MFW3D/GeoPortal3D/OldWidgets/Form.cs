namespace Multispectral.GeoPortal3D.OldWidgets
{
    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using System;
    using System.Drawing;
    using System.Drawing.Text;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using MFW3D;
    using MFW3D.Widgets;

    public class Form : IWidget, IInteractive
    {
        public float dblAltitude = float.MinValue;
        public float dblLatitude = float.MinValue;
        public float dblLongitude = float.MinValue;
        private bool isResizingBottom = false;
        private bool isResizingLeft = false;
        private bool isResizingLL = false;
        private bool isResizingLR = false;
        private bool isResizingRight = false;
        private bool isResizingTop = false;
        private bool isResizingUL = false;
        private bool isResizingUR = false;
        private bool m_AutoHideHeader = false;
        private Color m_BackgroundColor = Color.FromArgb(170, 40, 40, 40);
        private Color m_BorderColor = Color.GhostWhite;
        private IWidgetCollection m_ChildWidgets = new WidgetCollection();
        private bool m_Enabled = true;
        private Color m_HeaderColor = Color.FromArgb(170, 40, 40, 40);
        private int m_HeaderHeight = 20;
        private bool m_HideBorder = false;
        private bool m_IsDragging = false;
        private Point m_LastMousePosition = new Point(0, 0);
        private Point m_Location = new Point(0, 0);
        private string m_Name = "";
        private Microsoft.DirectX.Vector2[] m_OutlineVerts = new Microsoft.DirectX.Vector2[4];
        private Microsoft.DirectX.Vector2[] m_OutlineVertsHeader = new Microsoft.DirectX.Vector2[4];
        private IWidget m_ParentWidget = null;
        private bool m_Resizeble = true;
        private Size m_Size = new Size(300, 200);
        private object m_Tag = null;
        private string m_Text = "";
        private Color m_TextColor = Color.GhostWhite;
        private Microsoft.DirectX.Direct3D.Font m_TextFont = null;
        private bool m_Visible = true;
        private Microsoft.DirectX.Direct3D.Font m_WorldWindDingsFont = null;
        private Size minSize = new Size(20, 20);
        private int resizeBuffer = 5;
        private WorldWindow ww;

        public event Multispectral.GeoPortal3D.OldWidgets.VisibleChangedHandler OnVisibleChanged;

        public Form(WorldWindow ww, double dblLatitude, double dblLongitude, double dblAltitude)
        {
            this.ww = ww;
            this.dblLatitude = (float) dblLatitude;
            this.dblLongitude = (float) dblLongitude;
            this.dblAltitude = (float) dblAltitude;
        }

        [DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpszFilename);
        private bool isPointInCloseBox(Point absolutePoint)
        {
            int num = 10;
            int num2 = 2;
            int num3 = this.m_Size.Width - 15;
            return (((absolutePoint.X >= (this.m_Location.X + num3)) && (absolutePoint.X <= ((this.m_Location.X + num3) + num))) && ((absolutePoint.Y >= (this.m_Location.Y + num2)) && (absolutePoint.Y <= ((this.m_Location.Y + num2) + num))));
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
            bool flag = false;
            bool flag2 = false;
            if (!this.m_Visible)
            {
                return false;
            }
            if (((e.X >= this.m_Location.X) && (e.X <= (this.m_Location.X + this.m_Size.Width))) && ((e.Y >= this.m_Location.Y) && (e.Y <= (this.m_Location.Y + this.m_Size.Height))))
            {
                this.m_ParentWidget.ChildWidgets.BringToFront(this);
                flag2 = true;
            }
            if (this.m_Resizeble)
            {
                if (((e.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (e.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((e.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (e.Y < (this.AbsoluteLocation.Y + this.resizeBuffer))))
                {
                    this.isResizingUL = true;
                }
                else if (((e.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (e.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((e.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (e.Y < (this.AbsoluteLocation.Y + this.resizeBuffer))))
                {
                    this.isResizingUR = true;
                }
                else if (((e.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (e.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((e.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (e.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                {
                    this.isResizingLL = true;
                }
                else if (((e.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (e.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((e.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (e.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                {
                    this.isResizingLR = true;
                }
                else if (((e.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (e.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((e.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (e.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                {
                    this.isResizingLeft = true;
                }
                else if (((e.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (e.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((e.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (e.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                {
                    this.isResizingRight = true;
                }
                else if (((e.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (e.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((e.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (e.Y < (this.AbsoluteLocation.Y + this.resizeBuffer))))
                {
                    this.isResizingTop = true;
                }
                else if (((e.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (e.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((e.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (e.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                {
                    this.isResizingBottom = true;
                }
                else if (((e.X >= this.m_Location.X) && (e.X <= (this.AbsoluteLocation.X + this.ClientSize.Width))) && ((e.Y >= this.AbsoluteLocation.Y) && (e.Y <= (this.AbsoluteLocation.Y + this.m_HeaderHeight))))
                {
                    this.m_IsDragging = true;
                    flag = true;
                }
            }
            else if (((e.X >= this.m_Location.X) && (e.X <= (this.AbsoluteLocation.X + this.ClientSize.Width))) && ((e.Y >= this.AbsoluteLocation.Y) && (e.Y <= (this.AbsoluteLocation.Y + this.m_HeaderHeight))))
            {
                this.m_IsDragging = true;
                flag = true;
            }
            this.m_LastMousePosition = new Point(e.X, e.Y);
            if (!flag)
            {
                for (int i = 0; i < this.m_ChildWidgets.Count; i++)
                {
                    if (!flag && (this.m_ChildWidgets[i] is IInteractive))
                    {
                        flag = (this.m_ChildWidgets[i] as IInteractive).OnMouseDown(e);
                    }
                }
            }
            if (!flag && flag2)
            {
                flag = true;
            }
            return flag;
        }

        public bool OnMouseEnter(EventArgs e)
        {
            return false;
        }

        public bool OnMouseLeave(EventArgs e)
        {
            return false;
        }

        public bool OnMouseMove(MouseEventArgs e)
        {
            bool flag = false;
            if (!this.m_Visible)
            {
                return false;
            }
            int num = e.X - this.m_LastMousePosition.X;
            int num2 = e.Y - this.m_LastMousePosition.Y;
            if (this.m_Resizeble)
            {
                if ((this.isResizingTop || this.isResizingUL) || this.isResizingUR)
                {
                    this.m_Location.Y += num2;
                    this.m_Size.Height -= num2;
                }
                else if ((this.isResizingBottom || this.isResizingLL) || this.isResizingLR)
                {
                    this.m_Size.Height += num2;
                }
                else if ((this.isResizingRight || this.isResizingUR) || this.isResizingLR)
                {
                    this.m_Size.Width += num;
                }
                else if ((this.isResizingLeft || this.isResizingUL) || this.isResizingLL)
                {
                    this.m_Location.X += num;
                    this.m_Size.Width -= num;
                }
            }
            if (this.m_IsDragging)
            {
                this.m_Location.X += num;
                this.m_Location.Y += num2;
                if (this.m_Location.X < 0)
                {
                    this.m_Location.X = 0;
                }
                if (this.m_Location.Y < 0)
                {
                    this.m_Location.Y = 0;
                }
                if ((this.m_Location.Y + this.m_Size.Height) > DrawArgs.ParentControl.Height)
                {
                    this.m_Location.Y = DrawArgs.ParentControl.Height - this.m_Size.Height;
                }
                if ((this.m_Location.X + this.m_Size.Width) > DrawArgs.ParentControl.Width)
                {
                    this.m_Location.X = DrawArgs.ParentControl.Width - this.m_Size.Width;
                }
                flag = true;
            }
            if (this.m_Size.Width < this.minSize.Width)
            {
                this.m_Size.Width = this.minSize.Width;
            }
            if (this.m_Size.Height < this.minSize.Height)
            {
                this.m_Size.Height = this.minSize.Height;
            }
            this.m_LastMousePosition = new Point(e.X, e.Y);
            for (int i = 0; i < this.m_ChildWidgets.Count; i++)
            {
                if (this.m_ChildWidgets[i] is IInteractive)
                {
                    flag = (this.m_ChildWidgets[i] as IInteractive).OnMouseMove(e);
                }
            }
            bool flag2 = false;
            if (((e.X >= this.m_Location.X) && (e.X <= (this.m_Location.X + this.m_Size.Width))) && ((e.Y >= this.m_Location.Y) && (e.Y <= (this.m_Location.Y + this.m_Size.Height))))
            {
                flag2 = true;
            }
            if (!flag && flag2)
            {
                flag = true;
            }
            return flag;
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            bool flag = false;
            if (!this.m_Visible)
            {
                return false;
            }
            if ((e.Button == MouseButtons.Left) && this.m_IsDragging)
            {
                this.m_IsDragging = false;
            }
            bool flag2 = false;
            if (((e.X >= this.m_Location.X) && (e.X <= (this.m_Location.X + this.m_Size.Width))) && ((e.Y >= this.m_Location.Y) && (e.Y <= (this.m_Location.Y + this.m_Size.Height))))
            {
                flag2 = true;
            }
            if (flag2 && this.isPointInCloseBox(new Point(e.X, e.Y)))
            {
                this.Visible = false;
                flag = true;
            }
            for (int i = 0; i < this.m_ChildWidgets.Count; i++)
            {
                if (this.m_ChildWidgets[i] is IInteractive)
                {
                    flag = (this.m_ChildWidgets[i] as IInteractive).OnMouseUp(e);
                }
            }
            if (!flag && flag2)
            {
                flag = true;
            }
            if (this.isResizingTop)
            {
                this.isResizingTop = false;
            }
            if (this.isResizingBottom)
            {
                this.isResizingBottom = false;
            }
            if (this.isResizingLeft)
            {
                this.isResizingLeft = false;
            }
            if (this.isResizingRight)
            {
                this.isResizingRight = false;
            }
            if (this.isResizingUL)
            {
                this.isResizingUL = false;
            }
            if (this.isResizingUR)
            {
                this.isResizingUR = false;
            }
            if (this.isResizingLL)
            {
                this.isResizingLL = false;
            }
            if (this.isResizingLR)
            {
                this.isResizingLR = false;
            }
            return flag;
        }

        public bool OnMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public virtual void Render(DrawArgs drawArgs)
        {
            if (this.m_Visible)
            {
                if (this.dblLatitude != double.MinValue)
                {
                    try
                    {
                        drawArgs.device.RenderState.ZBufferEnable = true;
                        Cull cullMode = drawArgs.device.RenderState.CullMode;
                        drawArgs.device.RenderState.CullMode = Cull.None;
                        Point3d pointd = MathEngine.SphericalToCartesianD(Angle.FromDegrees((double) this.dblLatitude), Angle.FromDegrees((double) this.dblLongitude), (this.ww.CurrentWorld.EquatorialRadius + (this.ww.CurrentWorld.TerrainAccessor.GetElevationAt((double) this.dblLatitude, (double) this.dblLongitude, 100.0) * World.Settings.VerticalExaggeration)) + this.dblAltitude);
                        Microsoft.DirectX.Vector3 point = new Microsoft.DirectX.Vector3((float) (pointd.X - drawArgs.WorldCamera.ReferenceCenter.X), (float) (pointd.Y - drawArgs.WorldCamera.ReferenceCenter.Y), (float) (pointd.Z - drawArgs.WorldCamera.ReferenceCenter.Z));
                        Microsoft.DirectX.Vector3 vector2 = drawArgs.WorldCamera.Project(point);
                        Microsoft.DirectX.Vector3 vector3 = new Microsoft.DirectX.Vector3();
                        Microsoft.DirectX.Vector3 vector4 = new Microsoft.DirectX.Vector3();
                        vector3.X = this.m_Location.X + (((float) this.m_Size.Width) / 3f);
                        vector3.Y = this.m_Location.Y + this.m_Size.Height;
                        vector4.X = this.m_Location.X + (2f * (((float) this.m_Size.Width) / 3f));
                        vector4.Y = this.m_Location.Y + this.m_Size.Height;
                        CustomVertex.TransformedColored[] vertexStreamZeroData = new CustomVertex.TransformedColored[3];
                        vertexStreamZeroData[0].X = vector3.X;
                        vertexStreamZeroData[0].Y = vector3.Y;
                        vertexStreamZeroData[0].Z = 0f;
                        vertexStreamZeroData[0].Color = this.m_BackgroundColor.ToArgb();
                        vertexStreamZeroData[1].X = vector2.X;
                        vertexStreamZeroData[1].Y = vector2.Y;
                        vertexStreamZeroData[1].Z = 0f;
                        vertexStreamZeroData[1].Color = this.m_BackgroundColor.ToArgb();
                        vertexStreamZeroData[2].X = vector4.X;
                        vertexStreamZeroData[2].Y = vector4.Y;
                        vertexStreamZeroData[2].Z = 0f;
                        vertexStreamZeroData[2].Color = this.m_BackgroundColor.ToArgb();
                        drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
                        drawArgs.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
                        drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertexStreamZeroData.Length - 2, vertexStreamZeroData);
                        drawArgs.device.RenderState.CullMode = cullMode;
                    }
                    catch (Exception)
                    {
                    }
                }
                if (this.m_TextFont == null)
                {
                    System.Drawing.Font font = new System.Drawing.Font("Arial", 12f, FontStyle.Italic | FontStyle.Bold);
                    this.m_TextFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, font);
                }
                if (this.m_WorldWindDingsFont == null)
                {
                    AddFontResource(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                    PrivateFontCollection fonts = new PrivateFontCollection();
                    fonts.AddFontFile(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                    System.Drawing.Font font2 = new System.Drawing.Font(fonts.Families[0], 12f);
                    this.m_WorldWindDingsFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, font2);
                }
                if (this.m_Resizeble)
                {
                    if (((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < (this.AbsoluteLocation.Y + this.resizeBuffer))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNWSE;
                    }
                    else if (((DrawArgs.LastMousePosition.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < (this.AbsoluteLocation.Y + this.resizeBuffer))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNESW;
                    }
                    else if (((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((DrawArgs.LastMousePosition.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNESW;
                    }
                    else if (((DrawArgs.LastMousePosition.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNWSE;
                    }
                    else if ((((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height)))) || (((DrawArgs.LastMousePosition.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height)))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeWE;
                    }
                    else if ((((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < (this.AbsoluteLocation.Y + this.resizeBuffer)))) || (((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height)))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNS;
                    }
                }
                if (this.ClientSize.Height > drawArgs.parentControl.Height)
                {
                    this.ClientSize = new Size(this.ClientSize.Width, drawArgs.parentControl.Height);
                }
                if (this.ClientSize.Width > drawArgs.parentControl.Width)
                {
                    this.ClientSize = new Size(drawArgs.parentControl.Width, this.ClientSize.Height);
                }
                if (!this.m_AutoHideHeader || (((DrawArgs.LastMousePosition.X >= this.m_Location.X) && (DrawArgs.LastMousePosition.X <= (this.m_Location.X + this.m_Size.Width))) && ((DrawArgs.LastMousePosition.Y >= this.m_Location.Y) && (DrawArgs.LastMousePosition.Y <= (this.m_Location.Y + this.m_Size.Height)))))
                {
                    Utilities.DrawBox(this.m_Location.X, this.m_Location.Y, this.m_Size.Width, this.m_HeaderHeight, 0f, this.m_HeaderColor.ToArgb(), drawArgs.device);
                    this.m_WorldWindDingsFont.DrawText(null, "E", new Rectangle((this.m_Location.X + this.m_Size.Width) - 15, this.m_Location.Y + 2, this.m_Size.Width, this.m_Size.Height), DrawTextFormat.NoClip, Color.White.ToArgb());
                    this.m_OutlineVertsHeader[0].X = this.AbsoluteLocation.X;
                    this.m_OutlineVertsHeader[0].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                    this.m_OutlineVertsHeader[1].X = this.AbsoluteLocation.X;
                    this.m_OutlineVertsHeader[1].Y = this.AbsoluteLocation.Y;
                    this.m_OutlineVertsHeader[2].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                    this.m_OutlineVertsHeader[2].Y = this.AbsoluteLocation.Y;
                    this.m_OutlineVertsHeader[3].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                    this.m_OutlineVertsHeader[3].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                    if (!this.m_HideBorder)
                    {
                        Utilities.DrawLine(this.m_OutlineVertsHeader, this.m_BorderColor.ToArgb(), drawArgs.device);
                    }
                }
                Utilities.DrawBox(this.m_Location.X, this.m_Location.Y + this.m_HeaderHeight, this.m_Size.Width, this.m_Size.Height - this.m_HeaderHeight, 0f, this.m_BackgroundColor.ToArgb(), drawArgs.device);
                for (int i = this.m_ChildWidgets.Count - 1; i >= 0; i--)
                {
                    IWidget widget = this.m_ChildWidgets[i];
                    if (widget != null)
                    {
                        if ((widget.ParentWidget == null) || (widget.ParentWidget != this))
                        {
                            widget.ParentWidget = this;
                        }
                        widget.Render(drawArgs);
                    }
                }
                this.m_OutlineVerts[0].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                this.m_OutlineVerts[0].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                this.m_OutlineVerts[1].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                this.m_OutlineVerts[1].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;
                this.m_OutlineVerts[2].X = this.AbsoluteLocation.X;
                this.m_OutlineVerts[2].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;
                this.m_OutlineVerts[3].X = this.AbsoluteLocation.X;
                this.m_OutlineVerts[3].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                if (!this.m_HideBorder)
                {
                    Utilities.DrawLine(this.m_OutlineVerts, this.m_BorderColor.ToArgb(), drawArgs.device);
                }
            }
        }

        public virtual void Render2(DrawArgs drawArgs)
        {
            if (this.m_Visible)
            {
                if ((this.dblLatitude != double.MinValue) && ((drawArgs.WorldCamera.Altitude > 100.0) || (World.Settings.VerticalExaggeration >= 1f)))
                {
                    try
                    {
                        drawArgs.device.RenderState.ZBufferEnable = true;
                        Cull cullMode = drawArgs.device.RenderState.CullMode;
                        drawArgs.device.RenderState.CullMode = Cull.None;
                        drawArgs.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Position;
                        drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
                        Microsoft.DirectX.Matrix matrix = Microsoft.DirectX.Matrix.Translation((float) -drawArgs.WorldCamera.ReferenceCenter.X, (float) -drawArgs.WorldCamera.ReferenceCenter.Y, (float) -drawArgs.WorldCamera.ReferenceCenter.Z);
                        drawArgs.device.Transform.World = matrix;
                        Microsoft.DirectX.Vector3 vector = MathEngine.SphericalToCartesian((double) this.dblLatitude, (double) this.dblLongitude, (this.ww.CurrentWorld.EquatorialRadius + (this.ww.CurrentWorld.TerrainAccessor.GetElevationAt((double) this.dblLatitude, (double) this.dblLongitude, 100.0) * World.Settings.VerticalExaggeration)) + this.dblAltitude);
                        Microsoft.DirectX.Vector3 vector2 = new Microsoft.DirectX.Vector3();
                        Microsoft.DirectX.Vector3 vector3 = new Microsoft.DirectX.Vector3();
                        vector2.X = this.m_Location.X + (((float) this.m_Size.Width) / 3f);
                        vector2.Y = (this.m_Location.Y + this.m_Size.Height) - 5;
                        vector2.Z = 0f;
                        vector2.Unproject(drawArgs.device.Viewport, drawArgs.WorldCamera.ProjectionMatrix, drawArgs.WorldCamera.ViewMatrix, drawArgs.device.Transform.World);
                        vector3.X = this.m_Location.X + (2f * (((float) this.m_Size.Width) / 3f));
                        vector3.Y = (this.m_Location.Y + this.m_Size.Height) - 5;
                        vector3.Z = 0f;
                        vector3.Unproject(drawArgs.device.Viewport, drawArgs.WorldCamera.ProjectionMatrix, drawArgs.WorldCamera.ViewMatrix, drawArgs.device.Transform.World);
                        CustomVertex.PositionColored[] vertexStreamZeroData = new CustomVertex.PositionColored[3];
                        vertexStreamZeroData[0].X = vector.X;
                        vertexStreamZeroData[0].Y = vector.Y;
                        vertexStreamZeroData[0].Z = vector.Z;
                        vertexStreamZeroData[0].Color = this.m_BackgroundColor.ToArgb();
                        vertexStreamZeroData[1].X = vector2.X;
                        vertexStreamZeroData[1].Y = vector2.Y;
                        vertexStreamZeroData[1].Z = vector2.Z;
                        vertexStreamZeroData[1].Color = this.m_BackgroundColor.ToArgb();
                        vertexStreamZeroData[2].X = vector3.X;
                        vertexStreamZeroData[2].Y = vector3.Y;
                        vertexStreamZeroData[2].Z = vector3.Z;
                        vertexStreamZeroData[2].Color = this.m_BackgroundColor.ToArgb();
                        drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertexStreamZeroData.Length - 2, vertexStreamZeroData);
                        drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
                        drawArgs.device.RenderState.CullMode = cullMode;
                        drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
                if (this.m_TextFont == null)
                {
                    System.Drawing.Font font = new System.Drawing.Font("Arial", 12f, FontStyle.Italic | FontStyle.Bold);
                    this.m_TextFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, font);
                }
                if (this.m_WorldWindDingsFont == null)
                {
                    AddFontResource(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                    PrivateFontCollection fonts = new PrivateFontCollection();
                    fonts.AddFontFile(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                    System.Drawing.Font font2 = new System.Drawing.Font(fonts.Families[0], 12f);
                    this.m_WorldWindDingsFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, font2);
                }
                if (this.m_Resizeble)
                {
                    if (((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < (this.AbsoluteLocation.Y + this.resizeBuffer))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNWSE;
                    }
                    else if (((DrawArgs.LastMousePosition.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < (this.AbsoluteLocation.Y + this.resizeBuffer))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNESW;
                    }
                    else if (((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((DrawArgs.LastMousePosition.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNESW;
                    }
                    else if (((DrawArgs.LastMousePosition.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNWSE;
                    }
                    else if ((((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < (this.AbsoluteLocation.X + this.resizeBuffer))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height)))) || (((DrawArgs.LastMousePosition.X > ((this.AbsoluteLocation.X - this.resizeBuffer) + this.ClientSize.Width)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height)))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeWE;
                    }
                    else if ((((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > (this.AbsoluteLocation.Y - this.resizeBuffer)) && (DrawArgs.LastMousePosition.Y < (this.AbsoluteLocation.Y + this.resizeBuffer)))) || (((DrawArgs.LastMousePosition.X > (this.AbsoluteLocation.X - this.resizeBuffer)) && (DrawArgs.LastMousePosition.X < ((this.AbsoluteLocation.X + this.resizeBuffer) + this.ClientSize.Width))) && ((DrawArgs.LastMousePosition.Y > ((this.AbsoluteLocation.Y - this.resizeBuffer) + this.ClientSize.Height)) && (DrawArgs.LastMousePosition.Y < ((this.AbsoluteLocation.Y + this.resizeBuffer) + this.ClientSize.Height)))))
                    {
                        DrawArgs.MouseCursor = CursorType.SizeNS;
                    }
                }
                if (this.ClientSize.Height > drawArgs.parentControl.Height)
                {
                    this.ClientSize = new Size(this.ClientSize.Width, drawArgs.parentControl.Height);
                }
                if (this.ClientSize.Width > drawArgs.parentControl.Width)
                {
                    this.ClientSize = new Size(drawArgs.parentControl.Width, this.ClientSize.Height);
                }
                if (!this.m_AutoHideHeader || (((DrawArgs.LastMousePosition.X >= this.m_Location.X) && (DrawArgs.LastMousePosition.X <= (this.m_Location.X + this.m_Size.Width))) && ((DrawArgs.LastMousePosition.Y >= this.m_Location.Y) && (DrawArgs.LastMousePosition.Y <= (this.m_Location.Y + this.m_Size.Height)))))
                {
                    Utilities.DrawBox(this.m_Location.X, this.m_Location.Y, this.m_Size.Width, this.m_HeaderHeight, 0f, this.m_HeaderColor.ToArgb(), drawArgs.device);
                    this.m_WorldWindDingsFont.DrawText(null, "E", new Rectangle((this.m_Location.X + this.m_Size.Width) - 15, this.m_Location.Y + 2, this.m_Size.Width, this.m_Size.Height), DrawTextFormat.NoClip, Color.White.ToArgb());
                    this.m_OutlineVertsHeader[0].X = this.AbsoluteLocation.X;
                    this.m_OutlineVertsHeader[0].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                    this.m_OutlineVertsHeader[1].X = this.AbsoluteLocation.X;
                    this.m_OutlineVertsHeader[1].Y = this.AbsoluteLocation.Y;
                    this.m_OutlineVertsHeader[2].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                    this.m_OutlineVertsHeader[2].Y = this.AbsoluteLocation.Y;
                    this.m_OutlineVertsHeader[3].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                    this.m_OutlineVertsHeader[3].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                    if (!this.m_HideBorder)
                    {
                        Utilities.DrawLine(this.m_OutlineVertsHeader, this.m_BorderColor.ToArgb(), drawArgs.device);
                    }
                }
                Utilities.DrawBox(this.m_Location.X, this.m_Location.Y + this.m_HeaderHeight, this.m_Size.Width, this.m_Size.Height - this.m_HeaderHeight, 0f, this.m_BackgroundColor.ToArgb(), drawArgs.device);
                for (int i = this.m_ChildWidgets.Count - 1; i >= 0; i--)
                {
                    IWidget widget = this.m_ChildWidgets[i];
                    if (widget != null)
                    {
                        if ((widget.ParentWidget == null) || (widget.ParentWidget != this))
                        {
                            widget.ParentWidget = this;
                        }
                        widget.Render(drawArgs);
                    }
                }
                this.m_OutlineVerts[0].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                this.m_OutlineVerts[0].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                this.m_OutlineVerts[1].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                this.m_OutlineVerts[1].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;
                this.m_OutlineVerts[2].X = this.AbsoluteLocation.X;
                this.m_OutlineVerts[2].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;
                this.m_OutlineVerts[3].X = this.AbsoluteLocation.X;
                this.m_OutlineVerts[3].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;
                if (!this.m_HideBorder)
                {
                    Utilities.DrawLine(this.m_OutlineVerts, this.m_BorderColor.ToArgb(), drawArgs.device);
                }
            }
        }

        public Point AbsoluteLocation
        {
            get
            {
                if (this.m_ParentWidget != null)
                {
                    return new Point(this.m_Location.X + this.m_ParentWidget.AbsoluteLocation.X, this.m_Location.Y + this.m_ParentWidget.AbsoluteLocation.Y);
                }
                return this.m_Location;
            }
        }

        public bool AutoHideHeader
        {
            get
            {
                return this.m_AutoHideHeader;
            }
            set
            {
                this.m_AutoHideHeader = value;
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

        public Color BorderColor
        {
            get
            {
                return this.m_BorderColor;
            }
            set
            {
                this.m_BorderColor = value;
            }
        }

        public IWidgetCollection ChildWidgets
        {
            get
            {
                return this.m_ChildWidgets;
            }
            set
            {
                this.m_ChildWidgets = value;
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

        public Color HeaderColor
        {
            get
            {
                return this.m_HeaderColor;
            }
            set
            {
                this.m_HeaderColor = value;
            }
        }

        public int HeaderHeight
        {
            get
            {
                return this.m_HeaderHeight;
            }
            set
            {
                this.m_HeaderHeight = value;
            }
        }

        public bool HideBorder
        {
            get
            {
                return this.m_HideBorder;
            }
            set
            {
                this.m_HideBorder = value;
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

        public IWidget ParentWidget
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

        public bool Resizeble
        {
            get
            {
                return this.m_Resizeble;
            }
            set
            {
                this.m_Resizeble = value;
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

        public Microsoft.DirectX.Direct3D.Font TextFont
        {
            get
            {
                return this.m_TextFont;
            }
            set
            {
                this.m_TextFont = value;
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
                if (this.m_Visible != value)
                {
                    this.m_Visible = value;
                    if (this.OnVisibleChanged != null)
                    {
                        this.OnVisibleChanged(this, value);
                    }
                }
            }
        }
    }
}
