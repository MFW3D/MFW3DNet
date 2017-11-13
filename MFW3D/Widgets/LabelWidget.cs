using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using MFW3D;

namespace MFW3D.NewWidgets
{
	/// <summary>
	/// Summary description for TextLabel.
	/// </summary>
	public class LabelWidget : IWidget
	{
		string m_Text = "";
		System.Drawing.Point m_location = new System.Drawing.Point(0,0);
		System.Drawing.Size m_size = new System.Drawing.Size(0,20);
		bool m_visible = true;
		bool m_enabled = true;
		IWidget m_parentWidget = null;
		object m_tag = null;
		System.Drawing.Color m_ForeColor = System.Drawing.Color.White;
		string m_name = "";
		DrawTextFormat m_Format = DrawTextFormat.NoClip;

		protected int m_borderWidth = 5;

		protected bool m_clearOnRender = false;

		protected bool m_autoSize = true;

		protected bool m_useParentWidth = false;

		protected bool m_useParentHeight = false;

		protected bool m_isInitialized = false;

		public LabelWidget()
		{
			m_location.X = m_borderWidth;
			m_location.Y = m_borderWidth;
		}

		public LabelWidget(string text)
		{
			Text = text;
			m_location.X = m_borderWidth;
			m_location.Y = m_borderWidth;
		}

        public LabelWidget(string text, System.Drawing.Color color, int xLoc, int yLoc, int xSize, int ySize)
        {
            Text = text;
            m_ForeColor = color;
            m_location.X = xLoc;
            m_location.Y = yLoc;
            m_size = new System.Drawing.Size(xSize, ySize);
        }

		#region  Ù–‘
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}
		public System.Drawing.Color ForeColor
		{
			get
			{
				return m_ForeColor;
			}
			set
			{
				m_ForeColor = value;
			}
		}
		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
				m_isInitialized = false;
			}
		}

		public DrawTextFormat Format
		{
			get { return m_Format; }
			set { m_Format = value; }
		}

		public bool ClearOnRender
		{
			get { return m_clearOnRender; }
			set { m_clearOnRender = value; }
		}

		public bool AutoSize
		{
			get { return m_autoSize; }
			set { m_autoSize = value; }
		}

		public bool UseParentWidth
		{
			get { return m_useParentWidth; }
			set { m_useParentWidth = value; }
		}

		public bool UseParentHeight
		{
			get { return m_useParentHeight; }
			set { m_useParentHeight = value; }
		}

		#endregion

		#region IWidget Members

		public IWidget ParentWidget
		{
			get
			{
				return m_parentWidget;
			}
			set
			{
				m_parentWidget = value;
			}
		}

		public bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				m_visible = value;
			}
		}
		protected bool m_countHeight = true;
		protected bool m_countWidth = true;
		public bool CountHeight
		{
			get { return m_countHeight; }
			set { m_countHeight = value; }
		}

		public bool CountWidth		
		{
			get { return m_countWidth; }
			set { m_countWidth = value; }
		}

		public object Tag
		{
			get
			{
				return m_tag;
			}
			set
			{
				m_tag = value;
			}
		}

		public IWidgetCollection ChildWidgets
		{
			get
			{
				// TODO:  Add TextLabel.ChildWidgets getter implementation
				return null;
			}
			set
			{
				// TODO:  Add TextLabel.ChildWidgets setter implementation
			}
		}

		public System.Drawing.Size ClientSize
		{
			get { return m_size; }
			set { m_size = value; }
		}

		public System.Drawing.Size WidgetSize
		{
			get 
			{ 
				if (m_parentWidget != null)
				{
					if (m_useParentWidth)
						m_size.Width = m_parentWidget.ClientSize.Width - (m_borderWidth + m_location.X);
					if (m_useParentHeight)
						m_size.Height = m_parentWidget.ClientSize.Height - (m_borderWidth + m_location.Y);	
				}
				return m_size; 
			}
			set { m_size = value; }
		}

		public bool Enabled
		{
			get { return m_enabled; }
			set { m_enabled = value; }
		}

		public System.Drawing.Point Location
		{
			get { return m_location; }
			set { m_location = value; }
		}

		public System.Drawing.Point ClientLocation
		{
			get { return this.AbsoluteLocation; }
		}

		public System.Drawing.Point AbsoluteLocation
		{
			get
			{
				if(m_parentWidget != null)
				{
					return new System.Drawing.Point(
						m_location.X + m_parentWidget.ClientLocation.X,
						m_location.Y + m_parentWidget.ClientLocation.Y);
					
				}
				else
				{
					return m_location;
				}
			}
		}

		public void ComputeAutoSize (DrawArgs drawArgs)
		{
			Microsoft.DirectX.Direct3D.Font font = drawArgs.defaultDrawingFont;
			if(font==null)
				font = drawArgs.CreateFont( "", 10 );
			Rectangle bounds = font.MeasureString(null, m_Text, m_Format, 0);
			if(m_useParentWidth)
			{
				m_size.Width = this.WidgetSize.Width - m_location.X;
				m_size.Height = bounds.Height * ( (int)(bounds.Width/m_size.Width) + 1);
			}
			else
			{
				m_size.Width = bounds.Width + m_borderWidth;
				m_size.Height = bounds.Height + m_borderWidth;
			}

			if(m_useParentHeight)
				m_size.Height = this.WidgetSize.Height - m_location.Y;

			// This code is iffy - no idea why Y is offset by more than specified.
			if (m_location.X == 0)
			{
				m_location.X = m_borderWidth;
				m_size.Width += m_borderWidth;
			}
			if (m_location.Y == 0)
			{
				m_location.Y = m_borderWidth;
				m_size.Height += m_borderWidth;
			}
		}
			
		public void Initialize(DrawArgs drawArgs)
		{
			if (m_autoSize)
				ComputeAutoSize (drawArgs);
			m_isInitialized = true;
		}

		public void Render(DrawArgs drawArgs)
		{
			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return;			

			if (!m_isInitialized)
				Initialize(drawArgs);

			drawArgs.defaultDrawingFont.DrawText(
				null,
				m_Text,
				new System.Drawing.Rectangle(AbsoluteLocation.X, AbsoluteLocation.Y, m_size.Width, m_size.Height),
				m_Format,
				m_ForeColor);

			if (m_clearOnRender)
			{
				m_Text = "";
				m_isInitialized = false;
			}
		}

		#endregion
	}
}
