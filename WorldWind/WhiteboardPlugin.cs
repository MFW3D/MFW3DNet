using System;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Windows;
using System.Windows.Forms;

using WorldWind;
using WorldWind.Menu;
using WorldWind.Renderable;

using Collab.jhuapl.Whiteboard;
using WorldWind.NewWidgets;

namespace jhuapl.sample
{
	/// <summary>
	/// 
	/// </summary>
	public class WhiteboardPlugin : WorldWind.PluginEngine.Plugin
	{
		protected WhiteboardMenuButton m_menuButton;
		
		/// <summary>
		/// This is the whiteboard form widget displayed on the worldwind window
		/// </summary>
		public WhiteboardWidget WbForm
		{
			get { return m_whiteboardForm; }
		}
		protected WhiteboardWidget m_whiteboardForm;

		/// <summary>
		/// This is the layer holding all whiteboard drawing elements
		/// </summary>
		public DrawLayer WbLayer
		{
			get { return m_whiteboardLayer; }
		}
		protected DrawLayer m_whiteboardLayer;

		/// <summary>
		/// The whiteboard menu item
		/// </summary>
		public System.Windows.Forms.MenuItem WbMenu
		{
			get { return m_wbMenuItem; }
		}
		protected System.Windows.Forms.MenuItem m_wbMenuItem;


		public System.Windows.Forms.MenuItem InfoMenu
		{
			get { return m_infoMenuItem; }
		}
		protected System.Windows.Forms.MenuItem m_infoMenuItem;


		public WhiteboardPlugin()
		{
		}
	
		public override void Load()
		{
			// Create our whiteboard layer
			m_whiteboardLayer = new DrawLayer("Whiteboard");
			Global.worldWindow.CurrentWorld.RenderableObjects.Add(m_whiteboardLayer);

			// Add our menu button
			m_menuButton = new WhiteboardMenuButton(this.PluginDirectory + @"\Plugins\Whiteboard\images\icons\APL4C.png", this);

			// Add our navigation menu item
			m_wbMenuItem = new System.Windows.Forms.MenuItem();
			m_wbMenuItem.Text = "Hide Whiteboard\tN";
			m_wbMenuItem.Click += new System.EventHandler(WbMenuItem_Click);

			Global.worldWindow.KeyUp += new KeyEventHandler(keyUp);

			if (m_whiteboardForm == null)
			{
                m_whiteboardForm = new WhiteboardWidget("Whiteboard", this.PluginDirectory);
				m_whiteboardForm.Location = new System.Drawing.Point(DrawArgs.NewRootWidget.ClientSize.Width - 401, 120);
				m_whiteboardForm.WidgetSize = new System.Drawing.Size(200, 242);
				m_whiteboardForm.HorizontalScrollbarEnabled = false;
				m_whiteboardForm.HorizontalResizeEnabled = false;
				m_whiteboardForm.Anchor = WidgetEnums.AnchorStyles.Right;
				m_whiteboardForm.Enabled = true;
				m_whiteboardForm.Visible = true;
				m_whiteboardForm.WhiteboardLayer = m_whiteboardLayer;
			}

            DrawArgs.NewRootWidget.ChildWidgets.Add(m_whiteboardForm);

			base.Load ();
		}
	
		public override void Unload()
		{
			// Reset the bottom for the Layer Manager
			m_menuButton.SetPushed(false);
			base.Unload ();
		}

		protected void WbMenuItem_Click(object sender, EventArgs s)
		{
			if (m_whiteboardForm.Enabled)
			{
				m_whiteboardForm.Enabled = false;
				m_wbMenuItem.Text = "Show Whiteboard\tN";
			}
			else
			{
				m_whiteboardForm.Enabled = true;
				m_whiteboardForm.Visible = true;
				m_wbMenuItem.Text = "Hide Whiteboard\tN";
			}
		}

		protected void keyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData==Keys.W)
			{
				WbMenuItem_Click(sender, e);
			} 
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class WhiteboardMenuButton : MenuButton
	{
		#region Private Members

		// The plugin associated with this button object
		internal static WhiteboardPlugin m_plugin;

		protected RootWidget m_rootWidget;

		protected bool m_setFlag = true;

		#endregion

		public WhiteboardMenuButton(string buttonIconPath, WhiteboardPlugin plugin) : base(buttonIconPath)
		{
			m_plugin = plugin;
			m_rootWidget = DrawArgs.NewRootWidget;
			this.Description = "Whiteboard";
			this.SetPushed(true);
		}

		public override void Dispose()
		{
			base.Dispose ();
		}

		public override void Update(DrawArgs drawArgs)
		{
		}

		public override bool IsPushed()
		{
			return m_setFlag;
		}

		public override void SetPushed(bool isPushed)
		{
			m_setFlag = isPushed;
		}

		public override void OnKeyDown(KeyEventArgs keyEvent)
		{
			m_rootWidget.OnKeyDown(keyEvent);
		}

		public override void OnKeyUp(KeyEventArgs keyEvent)
		{
			m_rootWidget.OnKeyUp(keyEvent);
		}

		public override bool OnMouseDown(MouseEventArgs e)
		{
			if(IsPushed())
			{	
				if (m_rootWidget.OnMouseDown(e))
				{
					m_plugin.WbLayer.PauseDrawing();
					return true;
				}
			}
			return m_plugin.WbLayer.OnMouseDown(e);
		}

		public override bool OnMouseMove(MouseEventArgs e)
		{
			if(IsPushed())
			{	
				if (m_rootWidget.OnMouseMove(e))
				{
					m_plugin.WbLayer.PauseDrawing();
					return true;
				}
			}
			return m_plugin.WbLayer.OnMouseMove(e);
		}

		public override bool OnMouseUp(MouseEventArgs e)
		{
			if(this.IsPushed())
			{	
				if (m_rootWidget.OnMouseUp(e))
				{
					m_plugin.WbLayer.PauseDrawing();
					return true;
				}

			}
			return m_plugin.WbLayer.OnMouseUp(e);
		}

		public override bool OnMouseWheel(MouseEventArgs e)
		{
			if(this.IsPushed())
				return m_rootWidget.OnMouseWheel(e);
			else
				return false;
		}


		public override void Render(DrawArgs drawArgs)
		{
			// HACK - check form state to set menu button correcly
			if (m_plugin.WbForm.Visible)
				m_plugin.WbMenu.Text = "Hide Whiteboard\tN";
			else
				m_plugin.WbMenu.Text = "Show Whiteboard\tN";

			// Force rendering of whiteboard layer - should not be needed if CS_Navigator is present but didn't work
			m_plugin.WbLayer.Render(drawArgs);
		}
	}
}