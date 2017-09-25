namespace Multispectral.GeoPortal3D.Plugins
{
    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using System;
    using System.Drawing;
    using Utility;
    using WorldWind;
    using WorldWind.NewWidgets;

    public class NavigationRing : IWidget
    {
        private float innerRadius = 0.5f;
        private CustomVertex.PositionColored[] m_arrowVertices = null;
        protected bool m_countHeight = true;
        protected bool m_countWidth = true;
        private Microsoft.DirectX.Direct3D.Font m_drawingFont = null;
        private bool m_Enabled = true;
        private CustomVertex.PositionColored[] m_eVertices = null;
        private Color m_ForeColor = Color.White;
        private System.Drawing.Font m_localFont = null;
        private Point m_Location = new Point(0, 0);
        private string m_Name = "";
        private CustomVertex.PositionColored[] m_nVertices = null;
        private IWidget m_ParentWidget = null;
        private Size m_Size = new Size(150, 150);
        private CustomVertex.PositionColored[] m_sVertices = null;
        private object m_Tag = null;
        private bool m_Visible = true;
        private CustomVertex.PositionColored[] m_wVertices = null;
        private Color normalColor = Color.FromArgb(0xff, Color.Gray);
        private Color northColor = Color.FromArgb(0xff, Color.Red);
        private float outerRadius = 0.7f;
        private int samples = 0x10;

        private void CreateArrows(ref CustomVertex.PositionColored[] vertices)
        {
            float num = 1.1f;
            float num2 = 0.7f;
            float num3 = 0.1f;
            int num4 = this.northColor.ToArgb();
            int num5 = this.normalColor.ToArgb();
            vertices = new CustomVertex.PositionColored[12];
            vertices[0].X = 0f;
            vertices[0].Y = num;
            vertices[0].Z = 0f;
            vertices[0].Color = num4;
            vertices[1].X = num3;
            vertices[1].Y = num2;
            vertices[1].Z = 0f;
            vertices[1].Color = num4;
            vertices[2].X = -num3;
            vertices[2].Y = num2;
            vertices[2].Z = 0f;
            vertices[2].Color = num4;
            vertices[3].X = -num;
            vertices[3].Y = 0f;
            vertices[3].Z = 0f;
            vertices[3].Color = num5;
            vertices[4].X = -num2;
            vertices[4].Y = num3;
            vertices[4].Z = 0f;
            vertices[4].Color = num5;
            vertices[5].X = -num2;
            vertices[5].Y = -num3;
            vertices[5].Z = 0f;
            vertices[5].Color = num5;
            vertices[6].X = 0f;
            vertices[6].Y = -num;
            vertices[6].Z = 0f;
            vertices[6].Color = num5;
            vertices[7].X = -num3;
            vertices[7].Y = -num2;
            vertices[7].Z = 0f;
            vertices[7].Color = num5;
            vertices[8].X = num3;
            vertices[8].Y = -num2;
            vertices[8].Z = 0f;
            vertices[8].Color = num5;
            vertices[9].X = num;
            vertices[9].Y = 0f;
            vertices[9].Z = 0f;
            vertices[9].Color = num5;
            vertices[10].X = num2;
            vertices[10].Y = -num3;
            vertices[10].Z = 0f;
            vertices[10].Color = num5;
            vertices[11].X = num2;
            vertices[11].Y = num3;
            vertices[11].Z = 0f;
            vertices[11].Color = num5;
        }

        private void CreateVertices(ref CustomVertex.PositionColored[] vertices, float startAngle, float endAngle, int steps, int color)
        {
            float num = endAngle - startAngle;
            vertices = new CustomVertex.PositionColored[(steps + 1) * 2];
            for (int i = 0; i <= steps; i++)
            {
                float num3 = ((((float) i) / ((float) steps)) * num) + startAngle;
                vertices[2 * i].X = ((float) Math.Cos((double) num3)) * this.outerRadius;
                vertices[2 * i].Y = ((float) Math.Sin((double) num3)) * this.outerRadius;
                vertices[2 * i].Z = 0f;
                vertices[2 * i].Color = color;
                vertices[(2 * i) + 1].X = ((float) Math.Cos((double) num3)) * this.innerRadius;
                vertices[(2 * i) + 1].Y = ((float) Math.Sin((double) num3)) * this.innerRadius;
                vertices[(2 * i) + 1].Z = 0f;
                vertices[(2 * i) + 1].Color = color;
            }
        }

        public void Initialize(DrawArgs drawArgs)
        {
        }

        public void Render(DrawArgs drawArgs)
        {
            try
            {
                if (this.m_Visible)
                {
                    Cull cullMode = drawArgs.device.RenderState.CullMode;
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    this.RenderCompass(drawArgs);
                    drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
                    drawArgs.device.RenderState.CullMode = cullMode;
                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
                    drawArgs.device.RenderState.ZBufferEnable = true;
                }
            }
            catch (Exception exception)
            {
                Log.Write(exception);
            }
        }

        private void RenderCompass(DrawArgs drawArgs)
        {
            if (this.m_nVertices == null)
            {
                this.CreateVertices(ref this.m_nVertices, 0.7853982f, 2.356194f, this.samples, this.northColor.ToArgb());
            }
            if (this.m_wVertices == null)
            {
                this.CreateVertices(ref this.m_wVertices, 2.450442f, 3.832743f, this.samples, this.normalColor.ToArgb());
            }
            if (this.m_sVertices == null)
            {
                this.CreateVertices(ref this.m_sVertices, 3.926991f, 5.497787f, this.samples, this.normalColor.ToArgb());
            }
            if (this.m_eVertices == null)
            {
                this.CreateVertices(ref this.m_eVertices, 5.592035f, 6.974336f, this.samples, this.normalColor.ToArgb());
            }
            if (this.m_arrowVertices == null)
            {
                this.CreateArrows(ref this.m_arrowVertices);
            }
            drawArgs.device.Transform.World = Microsoft.DirectX.Matrix.RotationZ((float) -drawArgs.WorldCamera.Heading.Radians);
            drawArgs.device.Transform.View = Microsoft.DirectX.Matrix.LookAtLH(new Microsoft.DirectX.Vector3(0f, 0f, 2f), new Microsoft.DirectX.Vector3(0f, 0f, 0f), new Microsoft.DirectX.Vector3(0f, 1f, 0f));
            drawArgs.device.Transform.Projection = Microsoft.DirectX.Matrix.PerspectiveFovLH(3.141593f, 1f, 0f, 10f);
            Cull cullMode = drawArgs.device.RenderState.CullMode;
            drawArgs.device.RenderState.CullMode = Cull.None;
            drawArgs.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Position;
            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, this.m_nVertices.Length - 2, this.m_nVertices);
            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, this.m_wVertices.Length - 2, this.m_wVertices);
            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, this.m_eVertices.Length - 2, this.m_eVertices);
            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, this.m_sVertices.Length - 2, this.m_sVertices);
            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleList, this.m_arrowVertices.Length / 3, this.m_arrowVertices);
            drawArgs.device.RenderState.CullMode = cullMode;
            drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
            drawArgs.device.Transform.View = drawArgs.WorldCamera.ViewMatrix;
            drawArgs.device.Transform.Projection = drawArgs.WorldCamera.ProjectionMatrix;
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

        public IWidgetCollection ChildWidgets
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

        public System.Drawing.Font Font
        {
            get
            {
                return this.m_localFont;
            }
            set
            {
                this.m_localFont = value;
                if (this.m_drawingFont != null)
                {
                    this.m_drawingFont.Dispose();
                    this.m_drawingFont = new Microsoft.DirectX.Direct3D.Font(DrawArgs.Device, this.m_localFont);
                }
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
