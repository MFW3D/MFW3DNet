//----------------------------------------------------------------------------
// NAME: LightController
// VERSION: 0.3
// DESCRIPTION: Allows to set options for the Sun light colors and direction
// DEVELOPER: Patrick Murris
// WEBSITE: http://www.alpix.com/3d/worldwin
//----------------------------------------------------------------------------
// 0.3  Apr  2, 2007    Added color dialogs for light and ambient (shade) WW 1.4.1
// 0.2  Dec 22, 2006    Added graphic representation of heading and elevation
// 0.1  Dec 17, 2006    First version WW 1.4.0
//----------------------------------------------------------------------------


using System.Windows.Forms;
using System.Drawing;
using System;
using WorldWind;

namespace Murris.Plugins
{

    /// <summary>
    /// The plugin (main class)
    /// </summary>
    public class LightController : WorldWind.PluginEngine.Plugin
    {

        static string Name = "Sunlight options";
        LightDialog m_Form;

        /// <summary>
        /// Plugin entry point - All plugins must implement this function
        /// </summary>
        public override void Load()
        {
        }

        /// <summary>
        /// Unloads our plugin
        /// </summary>
        public override void Unload()
        {
            if (m_Form != null)
            {
                if (!m_Form.IsDisposed)
                {
                    m_Form.Dispose();
                }
                m_Form = null;
            }
        }

        /// <summary>
        /// Properties Dialog
        /// </summary>
        public class LightDialog : System.Windows.Forms.Form
        {
            private System.Windows.Forms.Label lblSunShading;
            private System.Windows.Forms.CheckBox chkSunShading;
            private System.Windows.Forms.Label lblSunFixed;
            private System.Windows.Forms.CheckBox chkSunFixed;
            private System.Windows.Forms.Label lblSunHeading;
            private System.Windows.Forms.TrackBar tbSunHeading;
            private System.Windows.Forms.Label lblSunElevation;
            private System.Windows.Forms.TrackBar tbSunElevation;
            private System.Windows.Forms.Label lblColor;
            private System.Windows.Forms.ColorDialog colorDialog;
            private System.Windows.Forms.Button btnLightColorPicker;
            private System.Windows.Forms.Button btnShadeColorPicker;

            private System.Windows.Forms.Button btnOK;
            private System.Windows.Forms.Button btnCancel;

            private bool suspendEvents = true;

            private bool EnableSunShadingBack;
            private bool SunSynchedWithTimeBack;
            private double SunHeadingBack;
            private double SunElevationBack;
            private Color LightColorBack;
            private Color ShadeColorBack;

            public LightDialog(string Name)
            {
                InitializeComponent();
                this.Text = Name;

                // Backup some values if we want to be able to cancel changes
                EnableSunShadingBack = World.Settings.EnableSunShading;
                SunSynchedWithTimeBack = World.Settings.SunSynchedWithTime;
                SunHeadingBack = World.Settings.SunHeading;
                SunElevationBack = World.Settings.SunElevation;
                LightColorBack = World.Settings.LightColor;
                ShadeColorBack = World.Settings.ShadingAmbientColor;
                // Init form values
                InitForm();
            }

            // Load WorldSettings to form controls
            private void InitForm()
            {
                suspendEvents = true;
                chkSunShading.Checked = World.Settings.EnableSunShading;
                chkSunFixed.Checked = !World.Settings.SunSynchedWithTime;
                tbSunHeading.Value = (int)MathEngine.RadiansToDegrees(World.Settings.SunHeading);
                tbSunElevation.Value = (int)MathEngine.RadiansToDegrees(World.Settings.SunElevation);
                this.btnLightColorPicker.BackColor = World.Settings.LightColor;
                this.btnShadeColorPicker.BackColor = World.Settings.ShadingAmbientColor;
                suspendEvents = false;
                FormChanged(null, null);
            }

            #region Windows Form Designer generated code
            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                this.btnCancel = new System.Windows.Forms.Button();
                this.btnOK = new System.Windows.Forms.Button();
                this.lblSunShading = new System.Windows.Forms.Label();
                this.chkSunShading = new System.Windows.Forms.CheckBox();
                this.lblSunFixed = new System.Windows.Forms.Label();
                this.chkSunFixed = new System.Windows.Forms.CheckBox();
                this.lblSunHeading = new System.Windows.Forms.Label();
                this.tbSunHeading = new System.Windows.Forms.TrackBar();
                this.lblSunElevation = new System.Windows.Forms.Label();
                this.tbSunElevation = new System.Windows.Forms.TrackBar();
                this.lblColor = new System.Windows.Forms.Label();
                this.colorDialog = new System.Windows.Forms.ColorDialog();
                this.btnLightColorPicker = new System.Windows.Forms.Button();
                this.btnShadeColorPicker = new System.Windows.Forms.Button();
                ((System.ComponentModel.ISupportInitialize)(this.tbSunHeading)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.tbSunElevation)).BeginInit();
                this.SuspendLayout();
                //
                // btnCancel
                //
                this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.btnCancel.Location = new System.Drawing.Point(211, 225);
                this.btnCancel.Name = "btnCancel";
                this.btnCancel.Size = new System.Drawing.Size(75, 23);
                this.btnCancel.TabIndex = 0;
                this.btnCancel.Text = "Cancel";
                this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                //
                // btnOK
                //
                this.btnOK.Location = new System.Drawing.Point(124, 225);
                this.btnOK.Name = "btnOK";
                this.btnOK.Size = new System.Drawing.Size(75, 23);
                this.btnOK.TabIndex = 1;
                this.btnOK.Text = "OK";
                this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                //
                // lblSunShading
                //
                this.lblSunShading.AutoSize = true;
                this.lblSunShading.Location = new System.Drawing.Point(16, 18);
                this.lblSunShading.Name = "lblSunShading";
                this.lblSunShading.Size = new System.Drawing.Size(32, 13);
                this.lblSunShading.TabIndex = 2;
                this.lblSunShading.Text = "Sun :";
                //
                // chkSunShading
                //
                this.chkSunShading.Location = new System.Drawing.Point(98, 15);
                this.chkSunShading.Name = "chkSunShading";
                this.chkSunShading.Size = new System.Drawing.Size(104, 24);
                this.chkSunShading.TabIndex = 3;
                this.chkSunShading.Text = "Enable sunlight";
                this.chkSunShading.CheckedChanged += new System.EventHandler(this.FormChanged);
                //
                // lblSunFixed
                //
                this.lblSunFixed.AutoSize = true;
                this.lblSunFixed.Location = new System.Drawing.Point(16, 49);
                this.lblSunFixed.Name = "lblSunFixed";
                this.lblSunFixed.Size = new System.Drawing.Size(64, 13);
                this.lblSunFixed.TabIndex = 4;
                this.lblSunFixed.Text = "Orientation :";
                //
                // chkSunFixed
                //
                this.chkSunFixed.Location = new System.Drawing.Point(98, 46);
                this.chkSunFixed.Name = "chkSunFixed";
                this.chkSunFixed.Size = new System.Drawing.Size(104, 24);
                this.chkSunFixed.TabIndex = 5;
                this.chkSunFixed.Text = "Fixed";
                this.chkSunFixed.CheckedChanged += new System.EventHandler(this.FormChanged);
                //
                // lblSunHeading
                //
                this.lblSunHeading.AutoSize = true;
                this.lblSunHeading.Location = new System.Drawing.Point(16, 80);
                this.lblSunHeading.Name = "lblSunHeading";
                this.lblSunHeading.Size = new System.Drawing.Size(53, 13);
                this.lblSunHeading.TabIndex = 6;
                this.lblSunHeading.Text = "Heading :";
                //
                // tbSunHeading
                //
                this.tbSunHeading.LargeChange = 15;
                this.tbSunHeading.Location = new System.Drawing.Point(92, 77);
                this.tbSunHeading.Maximum = 180;
                this.tbSunHeading.Minimum = -180;
                this.tbSunHeading.Name = "tbSunHeading";
                this.tbSunHeading.Size = new System.Drawing.Size(160, 37);
                this.tbSunHeading.TabIndex = 7;
                this.tbSunHeading.TickFrequency = 30;
                this.tbSunHeading.ValueChanged += new System.EventHandler(this.FormChanged);
                //
                // lblSunElevation
                //
                this.lblSunElevation.AutoSize = true;
                this.lblSunElevation.Location = new System.Drawing.Point(16, 125);
                this.lblSunElevation.Name = "lblSunElevation";
                this.lblSunElevation.Size = new System.Drawing.Size(57, 13);
                this.lblSunElevation.TabIndex = 8;
                this.lblSunElevation.Text = "Elevation :";
                //
                // tbSunElevation
                //
                this.tbSunElevation.Location = new System.Drawing.Point(92, 122);
                this.tbSunElevation.Maximum = 90;
                this.tbSunElevation.Minimum = -10;
                this.tbSunElevation.Name = "tbSunElevation";
                this.tbSunElevation.Size = new System.Drawing.Size(160, 37);
                this.tbSunElevation.TabIndex = 9;
                this.tbSunElevation.TickFrequency = 10;
                this.tbSunElevation.ValueChanged += new System.EventHandler(this.FormChanged);
                //
                // lblColor
                //
                this.lblColor.AutoSize = true;
                this.lblColor.Location = new System.Drawing.Point(16, 180);
                this.lblColor.Name = "lblColor";
                this.lblColor.Size = new System.Drawing.Size(57, 13);
                this.lblColor.TabIndex = 10;
                this.lblColor.Text = "Colors :";
                //
                // btnLightColorPicker
                //
                this.btnLightColorPicker.Location = new System.Drawing.Point(100, 177);
                this.btnLightColorPicker.Name = "btnLightColorPicker";
                this.btnLightColorPicker.Size = new System.Drawing.Size(80, 23);
                this.btnLightColorPicker.TabIndex = 11;
                this.btnLightColorPicker.Text = "Light Color";
                this.btnLightColorPicker.UseVisualStyleBackColor = true;
                this.btnLightColorPicker.Click += new System.EventHandler(this.btnLightColorPicker_Click);
                //
                // btnShadeColorPicker
                //
                this.btnShadeColorPicker.Location = new System.Drawing.Point(200, 177);
                this.btnShadeColorPicker.Name = "btnShadeColorPicker";
                this.btnShadeColorPicker.Size = new System.Drawing.Size(80, 23);
                this.btnShadeColorPicker.TabIndex = 12;
                this.btnShadeColorPicker.Text = "Shade Color";
                this.btnShadeColorPicker.UseVisualStyleBackColor = true;
                this.btnShadeColorPicker.ForeColor = Color.White;
                this.btnShadeColorPicker.Click += new System.EventHandler(this.btnShadeColorPicker_Click);
                //
                // LightDialog
                //
                this.AcceptButton = this.btnOK;
                this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
                this.CancelButton = this.btnCancel;
                this.ClientSize = new System.Drawing.Size(306, 265);
                this.ControlBox = false;
                this.Controls.Add(this.lblSunShading);
                this.Controls.Add(this.chkSunShading);
                this.Controls.Add(this.lblSunFixed);
                this.Controls.Add(this.chkSunFixed);
                this.Controls.Add(this.lblSunHeading);
                this.Controls.Add(this.tbSunHeading);
                this.Controls.Add(this.lblSunElevation);
                this.Controls.Add(this.tbSunElevation);
                this.Controls.Add(this.lblColor);
                this.Controls.Add(this.btnLightColorPicker);
                this.Controls.Add(this.btnShadeColorPicker);
                this.Controls.Add(this.btnOK);
                this.Controls.Add(this.btnCancel);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "LightDialog";
                this.ShowInTaskbar = false;
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                this.TopMost = true;
                ((System.ComponentModel.ISupportInitialize)(this.tbSunHeading)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.tbSunElevation)).EndInit();
                this.ResumeLayout(false);
                this.PerformLayout();

            }
            #endregion

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                DrawLightGraphics();
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                InitForm();
                this.Invalidate();
            }


            private void FormChanged(object sender, System.EventArgs e)
            {
                if (!suspendEvents)
                {
                    // Update WW setings from form
                    World.Settings.EnableSunShading = chkSunShading.Checked;
                    World.Settings.SunSynchedWithTime = !chkSunFixed.Checked;
                    World.Settings.SunHeading = MathEngine.DegreesToRadians(tbSunHeading.Value);
                    World.Settings.SunElevation = MathEngine.DegreesToRadians(tbSunElevation.Value);
                    // Disable controls if needed
                    if (chkSunShading.Checked)
                    {
                        chkSunFixed.Enabled = true;
                        btnLightColorPicker.Enabled = true;
                        btnShadeColorPicker.Enabled = true;
                    }
                    else
                    {
                        chkSunFixed.Enabled = false;
                        btnLightColorPicker.Enabled = false;
                        btnShadeColorPicker.Enabled = false;
                    }
                    if (chkSunFixed.Checked && chkSunFixed.Enabled)
                    {
                        tbSunHeading.Enabled = true;
                        tbSunElevation.Enabled = true;
                    }
                    else
                    {
                        tbSunHeading.Enabled = false;
                        tbSunElevation.Enabled = false;
                    }
                    // Draw heading and elevation graphics
                    DrawLightGraphics();
                }
            }


            // Draw light heading and elevation graphics
            private void DrawLightGraphics()
            {
                Graphics g = this.CreateGraphics();
                int w = 32;
                int h = 32;
                int x, y;
                Pen pen1 = new Pen(Color.Black, 1);
                Pen pen2 = new Pen(Color.Red, 1);
                //g.SmoothingMode = SmoothingMode.HighQuality;
                // Heading
                g.ResetTransform();
                g.TranslateTransform(256f, 77f);
                g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, w + 1, h + 1);        // Clear
                if (tbSunHeading.Enabled)
                {
                    g.DrawEllipse(pen1, 0, 0, w, h);        // Circle
                    g.DrawLine(pen1, 0, h / 2, w, h / 2);       // Horiz. line
                    g.DrawLine(pen1, w / 2, 0, w / 2, h);       // Vert. line
                    x = (int)(w / 2 * (float)Math.Sin(World.Settings.SunHeading));
                    y = (int)(h / 2 * (float)Math.Cos(World.Settings.SunHeading));
                    g.DrawLine(pen2, w / 2, h / 2, w / 2 + x, h / 2 - y);  // Heading line
                }
                // Elevation
                g.ResetTransform();
                g.TranslateTransform(256f, 122f);
                g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, w + 1, h + 10);       // Clear
                if (tbSunElevation.Enabled)
                {
                    g.DrawArc(pen1, -w, 0, w + w, h + h, -tbSunElevation.Minimum, -tbSunElevation.Maximum + tbSunElevation.Minimum);    // Arc
                    g.DrawLine(pen1, 0, h, w, h);   // Horiz. line
                    g.DrawLine(pen1, 0, 0, 0, h);   // Vert. line
                    x = (int)(w * (float)Math.Cos(World.Settings.SunElevation));
                    y = (int)(h * (float)Math.Sin(World.Settings.SunElevation));
                    g.DrawLine(pen2, 0, h, x, h - y);  // Elevation
                }
            }

            private void btnLightColorPicker_Click(object sender, EventArgs e)
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    World.Settings.LightColor = colorDialog.Color;
                    btnLightColorPicker.BackColor = World.Settings.LightColor;
                }
            }

            private void btnShadeColorPicker_Click(object sender, EventArgs e)
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    World.Settings.ShadingAmbientColor = colorDialog.Color;
                    btnShadeColorPicker.BackColor = World.Settings.ShadingAmbientColor;
                }
            }


            private void btnOK_Click(object sender, System.EventArgs e)
            {
                // Close this form
                this.Close();
            }

            private void btnCancel_Click(object sender, System.EventArgs e)
            {
                // Restore values from backup
                World.Settings.EnableSunShading = EnableSunShadingBack;
                World.Settings.SunSynchedWithTime = SunSynchedWithTimeBack;
                World.Settings.SunHeading = SunHeadingBack;
                World.Settings.SunElevation = SunElevationBack;
                World.Settings.LightColor = LightColorBack;
                World.Settings.ShadingAmbientColor = ShadeColorBack;
                // Close this form
                this.Close();
            }

        }       // End class LightDialog




    }  // End class LightController


}