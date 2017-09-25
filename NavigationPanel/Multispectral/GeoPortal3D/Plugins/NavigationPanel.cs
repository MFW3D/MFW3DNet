namespace Multispectral.GeoPortal3D.Plugins
{
    using Multispectral.GeoPortal3D.Widgets;
    using System;
    using System.Drawing;
    using System.Timers;
    using System.Windows.Forms;
    using WorldWind;
    using WorldWind.Camera;
    using WorldWind.NewWidgets;
    using WorldWind.PluginEngine;
    using System.IO;

    public class NavigationPanel : WorldWind.PluginEngine.Plugin
    {
        private string basePath = "";
        private Compass m_compass = null;
        private MenuItem m_menuItem = null;
        private System.Timers.Timer m_UpdateTilt = new System.Timers.Timer(100.0);
        private FormWidget m_window = null;
        private Slider slider = null;
        private Slider sliderZoom = null;

        public void high_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (this.slider != null)
            {
                this.slider.Value = this.slider.MaximumValue;
            }
        }

        public override void Load()
        {
            this.basePath = this.PluginDirectory;
            this.m_menuItem = new MenuItem("NavigationPanel");
            this.m_menuItem.Click += new EventHandler(this.m_menuItem_Click);
            this.m_menuItem.Checked = World.Settings.ShowCompass;
            this.ParentApplication.ToolsMenu.MenuItems.Add(this.m_menuItem);
            this.m_window = new FormWidget("NavigationPanel");
            this.m_window.Name = "NavigationPanel";
            this.m_window.ClientSize = new Size(0xac, 240);
            int y = 0;
            int num2 = 0;
            if (World.Settings.ShowToolbar)
            {
                y += 0x2d;
            }
            this.m_window.Anchor = WidgetEnums.AnchorStyles.Right | WidgetEnums.AnchorStyles.Top;
            this.m_window.ParentWidget = DrawArgs.NewRootWidget;
            this.m_window.Location = new Point((DrawArgs.NewRootWidget.ClientSize.Width - this.m_window.ClientSize.Width) - num2, y);
            this.m_window.Text = "Double Click to Re-Open";
            this.m_window.AutoHideHeader = true;
            this.m_window.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
            this.m_window.HeaderEnabled = false;
            this.m_window.Visible = true;
            this.m_window.BorderEnabled = false;
            this.m_window.VerticalScrollbarEnabled = false;
            this.m_window.HorizontalScrollbarEnabled = false;
            this.m_window.VerticalResizeEnabled = false;
            this.m_window.HorizontalResizeEnabled = false;
            this.m_window.Enabled = true;
            this.slider = new Slider(SliderType.Horizontal);
            this.slider.Name = "Tilt Slider";
            this.slider.HandleUri = this.basePath + @"\img\handle.vertical.png";
            this.slider.HighImageUri = this.basePath + @"\img\highimage.png";
            this.slider.LowImageUri = this.basePath + @"\img\lowimage.png";
            this.slider.HandleSize = new Size(10, 0x15);
            this.slider.LowImageSize = new Size(0x13, 0x12);
            this.slider.HighImageSize = new Size(0x13, 0x12);
            this.slider.ForeColor = Color.FromArgb(0x80, 0xff, 0xff, 0xff);
            this.slider.StickBackgroundColor = Color.FromArgb(40, 0, 0, 0);
            this.slider.Visible = true;
            this.slider.ClientLocation = new Point(0, 0);
            this.slider.ClientSize = new Size(0x8e, 30);
            this.slider.MaximumValue = 6M;
            this.slider.MinimumValue = -6M;
            this.slider.ParentWidget = this.m_window;
            this.slider.OnMouseEnterEvent += new EventHandler(this.slider_OnMouseEnterEvent);
            this.slider.OnMouseLeaveEvent += new EventHandler(this.slider_OnMouseLeaveEvent);
            this.slider.low_OnMouseDownEvent += new MouseEventHandler(this.low_OnMouseDown);
            this.slider.high_OnMouseDownEvent += new MouseEventHandler(this.high_OnMouseDown);
            this.slider.CountHeight = false;
            this.slider.CountWidth = true;
            this.m_window.ChildWidgets.Add(this.slider);
            this.sliderZoom = new Slider(SliderType.Vertical);
            this.sliderZoom.Name = "Zoom Slider";
            this.sliderZoom.HandleUri = this.basePath + @"\img\handle.horizontal.png";
            this.sliderZoom.HighImageUri = this.basePath + @"\img\zoom.in.png";
            this.sliderZoom.LowImageUri = this.basePath + @"\img\zoom.out.png";
            this.sliderZoom.HandleSize = new Size(0x15, 10);
            this.sliderZoom.LowImageSize = new Size(0x13, 0x12);
            this.sliderZoom.HighImageSize = new Size(0x13, 0x12);
            this.sliderZoom.ForeColor = Color.FromArgb(0x80, 0xff, 0xff, 0xff);
            this.sliderZoom.StickBackgroundColor = Color.FromArgb(40, 0, 0, 0);
            this.sliderZoom.Visible = true;
            this.sliderZoom.ClientLocation = new Point(this.m_window.ClientSize.Width - 30, 0x23);
            this.sliderZoom.ClientSize = new Size(30, 0x8e);
            this.sliderZoom.MaximumValue = 1M;
            this.sliderZoom.MinimumValue = -1M;
            this.sliderZoom.ParentWidget = this.m_window;
            this.sliderZoom.OnMouseEnterEvent += new EventHandler(this.slider_OnMouseEnterEvent);
            this.sliderZoom.OnMouseLeaveEvent += new EventHandler(this.slider_OnMouseLeaveEvent);
            this.sliderZoom.low_OnMouseDownEvent += new MouseEventHandler(this.zoomhigh_OnMouseDown);
            this.sliderZoom.high_OnMouseDownEvent += new MouseEventHandler(this.zoomlow_OnMouseDown);
            this.sliderZoom.CountHeight = false;
            this.sliderZoom.CountWidth = true;
            this.m_window.ChildWidgets.Add(this.sliderZoom);
            this.m_compass = new Compass();
            this.m_compass.Name = "Tilt Slider";
            this.m_compass.CompassUri = this.basePath + @"\img\Newman_compass.png";
            this.m_compass.ForeColor = Color.FromArgb(0x80, 0xff, 0xff, 0xff);
            this.m_compass.Visible = true;
            this.m_compass.ClientLocation = new Point(5, 0x23);
            this.m_compass.ClientSize = new Size(0x80, 0x80);
            this.m_compass.ParentWidget = this.m_window;
            this.m_compass.CountHeight = false;
            this.m_compass.CountWidth = true;
            this.m_window.ChildWidgets.Add(this.m_compass);
            DrawArgs.NewRootWidget.ChildWidgets.Add(this.m_window);
            this.m_UpdateTilt.Elapsed += new ElapsedEventHandler(this.m_UpdateTilt_Elapsed);
            base.Load();
        }

        public void low_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (this.slider != null)
            {
                this.slider.Value = this.slider.MinimumValue;
            }
        }

        private void m_form_OnVisibleChanged(object o, bool state)
        {
            this.m_menuItem.Checked = state;
        }

        private void m_menuItem_Click(object sender, EventArgs e)
        {
            this.m_menuItem.Checked = !this.m_menuItem.Checked;
            if (this.m_window != null)
            {
                this.m_window.Visible = this.m_menuItem.Checked;
            }
        }

        private void m_UpdateTilt_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.slider != null)
            {
                CameraBase worldCamera = base.m_Application.WorldWindow.DrawArgs.WorldCamera;
                worldCamera.Tilt += Angle.FromDegrees((double) this.slider.Value);
            }
            if ((this.sliderZoom != null) && (((double) Math.Abs(this.sliderZoom.Value)) > 0.1))
            {
                CameraBase base2 = base.m_Application.WorldWindow.DrawArgs.WorldCamera;
                base2.Altitude += base.m_Application.WorldWindow.DrawArgs.WorldCamera.Altitude * ((double) this.sliderZoom.Value);
            }
        }

        private void slider_OnMouseEnterEvent(object sender, EventArgs e)
        {
            this.m_UpdateTilt.Start();
        }

        private void slider_OnMouseLeaveEvent(object sender, EventArgs e)
        {
            this.m_UpdateTilt.Stop();
        }

        public override void Unload()
        {
            if (this.m_window != null)
            {
                DrawArgs.NewRootWidget.ChildWidgets.Remove(this.m_window);
                this.m_window.Dispose();
                this.m_window = null;
            }
            this.ParentApplication.ToolsMenu.MenuItems.Remove(this.m_menuItem);
            base.Unload();
        }

        public void zoomhigh_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (this.sliderZoom != null)
            {
                this.sliderZoom.Value = this.slider.MaximumValue;
            }
        }

        public void zoomlow_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (this.sliderZoom != null)
            {
                this.sliderZoom.Value = this.slider.MinimumValue;
            }
        }
    }
}
