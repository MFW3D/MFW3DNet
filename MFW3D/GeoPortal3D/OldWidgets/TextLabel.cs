namespace Multispectral.GeoPortal3D.OldWidgets
{
    using Microsoft.DirectX.Direct3D;
    using System;
    using System.Drawing;
    using MFW3D;
    using MFW3D.Widgets;

    public class TextLabel : IWidget
    {
        private bool m_Enabled;
        private Color m_ForeColor;
        private float m_ftFontSize;
        private Point m_Location;
        private string m_Name;
        private IWidget m_ParentWidget;
        private Size m_Size;
        private string m_stFontName;
        private object m_Tag;
        private string m_Text;
        private Microsoft.DirectX.Direct3D.Font m_TextFont;
        private bool m_Visible;

        public TextLabel()
        {
            this.m_Text = "";
            this.m_Location = new Point(0, 0);
            this.m_Size = new Size(0, 20);
            this.m_Visible = true;
            this.m_Enabled = true;
            this.m_ParentWidget = null;
            this.m_Tag = null;
            this.m_ForeColor = Color.White;
            this.m_Name = "";
            this.m_stFontName = "Arial";
            this.m_ftFontSize = 10f;
            this.m_TextFont = null;
        }

        public TextLabel(string stFontName, float ftFontSize)
        {
            this.m_Text = "";
            this.m_Location = new Point(0, 0);
            this.m_Size = new Size(0, 20);
            this.m_Visible = true;
            this.m_Enabled = true;
            this.m_ParentWidget = null;
            this.m_Tag = null;
            this.m_ForeColor = Color.White;
            this.m_Name = "";
            this.m_stFontName = "Arial";
            this.m_ftFontSize = 10f;
            this.m_TextFont = null;
            this.m_stFontName = stFontName;
            this.m_ftFontSize = ftFontSize;
        }

        public void Render(DrawArgs drawArgs)
        {
            if (this.m_Visible)
            {
                if (this.m_TextFont == null)
                {
                    try
                    {
                        System.Drawing.Font font = new System.Drawing.Font(this.m_stFontName, this.m_ftFontSize, FontStyle.Regular);
                        this.m_TextFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, font);
                    }
                    catch (Exception)
                    {
                    }
                }
                this.m_TextFont.DrawText(null, this.m_Text, new Rectangle(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.m_Size.Width, this.m_Size.Height), DrawTextFormat.WordBreak, this.m_ForeColor);
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
    }
}
