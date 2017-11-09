//----------------------------------------------------------------------------
// NAME: Planimetric Measure Tool
// VERSION: 1.0
// DESCRIPTION: Planimetric Measure Tool
// DEVELOPER: New Generation Measure Tool by Tisham Dhar aka "what_nick"
// WEBSITE: http://whatnick.blogspot.com
// REFERENCES: 
//----------------------------------------------------------------------------
//
// 
//
using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using MFW3D;
using MFW3D.Renderable;
using MFW3D.Net;
using System.Xml;

//Shape support
using MapTools;

//Graph support
using ZedGraph;

namespace MeasureToolNewgen.Plugins
{
    /// <summary>
    /// Winforms class to show measurement results till
    /// the Worldwind Widgets mature
    /// </summary>
    class MeasureWinResult : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label measureResult;
        //private System.ComponentModel.IContainer components;
        private ToolStrip toolStrip1;
        private ToolStripButton btnClearLines;
        private System.Windows.Forms.Label measureLabel;
        private ToolStripButton btnPlotTerrain;
        private ToolStripButton btnLineMode;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnStop;
        private MeasureToolNG plugin;

        private string multiIconPath = Application.StartupPath + "\\Plugins\\Measure\\linemode_multi_button.png";
        private System.Windows.Forms.Label areaResult;
        private System.Windows.Forms.Label areaLabel;
        private string singleIconPath = Application.StartupPath + "\\Plugins\\Measure\\linemode_single_button.png";

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeasureWinResult));
            this.measureLabel = new System.Windows.Forms.Label();
            this.measureResult = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLineMode = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnClearLines = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPlotTerrain = new System.Windows.Forms.ToolStripButton();
            this.areaResult = new System.Windows.Forms.Label();
            this.areaLabel = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // measureLabel
            // 
            this.measureLabel.AutoSize = true;
            this.measureLabel.Location = new System.Drawing.Point(13, 36);
            this.measureLabel.Name = "measureLabel";
            this.measureLabel.Size = new System.Drawing.Size(40, 13);
            this.measureLabel.TabIndex = 0;
            this.measureLabel.Text = "Length";
            this.measureLabel.Click += new System.EventHandler(this.measureLabel_Click);
            // 
            // measureResult
            // 
            this.measureResult.AutoSize = true;
            this.measureResult.Location = new System.Drawing.Point(59, 36);
            this.measureResult.Name = "measureResult";
            this.measureResult.Size = new System.Drawing.Size(22, 13);
            this.measureResult.TabIndex = 1;
            this.measureResult.Text = "0.0";
            this.measureResult.Click += new System.EventHandler(this.measureResult_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLineMode,
            this.btnStop,
            this.btnClearLines,
            this.toolStripSeparator1,
            this.btnPlotTerrain});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(213, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLineMode
            // 
            this.btnLineMode.CheckOnClick = true;
            this.btnLineMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLineMode.Image = ((System.Drawing.Image)(resources.GetObject("btnLineMode.Image")));
            this.btnLineMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLineMode.Name = "btnLineMode";
            this.btnLineMode.Size = new System.Drawing.Size(23, 22);
            this.btnLineMode.Text = "Toggle Line Mode";
            this.btnLineMode.Click += new System.EventHandler(this.btnLineMode_Click);
            // 
            // btnStop
            // 
            this.btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(23, 22);
            this.btnStop.Text = "toolStripButton1";
            // 
            // btnClearLines
            // 
            this.btnClearLines.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClearLines.Image = ((System.Drawing.Image)(resources.GetObject("btnClearLines.Image")));
            this.btnClearLines.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearLines.Name = "btnClearLines";
            this.btnClearLines.Size = new System.Drawing.Size(23, 22);
            this.btnClearLines.Text = "Clear all lines";
            this.btnClearLines.Click += new System.EventHandler(this.clearAllLinesButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnPlotTerrain
            // 
            this.btnPlotTerrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlotTerrain.Image = ((System.Drawing.Image)(resources.GetObject("btnPlotTerrain.Image")));
            this.btnPlotTerrain.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlotTerrain.Name = "btnPlotTerrain";
            this.btnPlotTerrain.Size = new System.Drawing.Size(23, 22);
            this.btnPlotTerrain.Text = "Plot terrain profile";
            this.btnPlotTerrain.Click += new System.EventHandler(this.btnPlotTerrain_Click);
            // 
            // areaResult
            // 
            this.areaResult.AutoSize = true;
            this.areaResult.Location = new System.Drawing.Point(59, 66);
            this.areaResult.Name = "areaResult";
            this.areaResult.Size = new System.Drawing.Size(22, 13);
            this.areaResult.TabIndex = 5;
            this.areaResult.Text = "0.0";
            // 
            // areaLabel
            // 
            this.areaLabel.AutoSize = true;
            this.areaLabel.Location = new System.Drawing.Point(13, 66);
            this.areaLabel.Name = "areaLabel";
            this.areaLabel.Size = new System.Drawing.Size(29, 13);
            this.areaLabel.TabIndex = 4;
            this.areaLabel.Text = "Area";
            // 
            // MeasureWinResult
            // 
            this.ClientSize = new System.Drawing.Size(213, 88);
            this.Controls.Add(this.areaResult);
            this.Controls.Add(this.areaLabel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.measureResult);
            this.Controls.Add(this.measureLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeasureWinResult";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "MeasureTool Control";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public MeasureWinResult(MeasureToolNG plugin)
        {
            this.plugin = plugin;

            //Do layout first
            InitializeComponent();

            if (World.Settings.MeasureMode == MeasureMode.Multi)
            {
                this.btnLineMode.Checked = true;
                this.btnLineMode.Image = new Bitmap(multiIconPath);
                //this.btnStop.Enabled = true;
            }

            //initialize graph stuff
            /*GraphPane pane = this.zedGraphControl1.GraphPane;
            pane.XAxis.Title.Text = "Distance (m)";
            pane.XAxis.Title.FontSpec.Size = 10f;
            pane.YAxis.Title.Text = "Elevation (m)";
            pane.YAxis.Title.FontSpec.Size = 10f;
            pane.Title.IsVisible = false;
            pane.Legend.IsVisible = false;*/
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            plugin.ChangePluginStatus();

        }

        #region Update Methods
        public void LengthText(string text)
        {
            measureResult.Text = text;
        }

        public void AreaText(string text)
        {
            areaResult.Text = text;
        }

        //TODO: Make a better graph style
        /*public void CreateGraph(ArrayList elevProfile)
        {
            if (elevProfile != null)
            {
                // get a reference to the GraphPane
                GraphPane myPane = this.zedGraphControl1.GraphPane;

                PointPairList list1 = new PointPairList();

                foreach (WorldWind.Point2d pt in elevProfile)
                {
                    list1.Add(pt.X, pt.Y);
                }


                LineItem myCurve = myPane.AddCurve("Terrain", list1, Color.Red);

                // Tell ZedGraph to refigure the
                // axes since the data have changed
                this.zedGraphControl1.AxisChange();
            }
        }*/
        #endregion

        private void measureLabel_Click(object sender, EventArgs e)
        {

        }

        private void measureResult_Click(object sender, EventArgs e)
        {

        }

        private void clearAllLinesButton_Click(object sender, EventArgs e)
        {
            plugin.ClearLines();
            /*this.zedGraphControl1.GraphPane.CurveList.Clear();
            this.zedGraphControl1.AxisChange();*/
        }

        private void btnPlotTerrain_Click(object sender, EventArgs e)
        {
            plugin.PlotTerrain();
        }

        private void btnLineMode_Click(object sender, EventArgs e)
        {
            if (btnLineMode.Checked == true)
            {
                World.Settings.MeasureMode = MeasureMode.Multi;
                this.btnLineMode.Image = new Bitmap(multiIconPath);
                //this.btnStop.Enabled = true;
            }
            else
            {
                World.Settings.MeasureMode = MeasureMode.Single;
                this.btnLineMode.Image = new Bitmap(singleIconPath);
                this.btnStop.Enabled = false;
            }
        }
    }

    /// <summary>
    /// Planimetric Measure Tool plug-in
    /// </summary>
    public class MeasureToolNG : MFW3D.PluginEngine.Plugin
    {
        MenuItem menuItem;
        MeasureToolLayer layer;
        MeasureAreaLayer arealayer;
        MeasureWinResult resultwindow;
        TerrainGraphForm tg;

        /// <summary>
        /// Plugin entry point 
        /// </summary>
        public override void Load()
        {
            World curworld = Global.worldWindow.CurrentWorld;
            layer = new MeasureToolLayer(curworld,
                Global.worldWindow.DrawArgs);

            LinearRing ring = new LinearRing();
            ring.Points = new Point3d[0];

            arealayer = new MeasureAreaLayer(curworld,
                Global.worldWindow.DrawArgs,
                ring);

            resultwindow = new MeasureWinResult(this);

            curworld.RenderableObjects.Add(layer);
            curworld.RenderableObjects.Add(arealayer);
            layer.LineAdd += resultwindow.LengthText;
            arealayer.PolyAdd += resultwindow.AreaText;
            //layer.Profiler += resultwindow.CreateGraph;
            //DrawArgs.NewRootWidget.ChildWidgets.Add(resultwindow);

            menuItem = new MenuItem("Measure\tM");
            menuItem.Click += new EventHandler(menuItemClicked);
            // Subscribe events for line measurement
            Global.worldWindow.MouseMove += new MouseEventHandler(layer.MouseMove);
            Global.worldWindow.MouseDown += new MouseEventHandler(layer.MouseDown);
            Global.worldWindow.MouseUp += new MouseEventHandler(layer.MouseUp);
            Global.worldWindow.KeyUp += new KeyEventHandler(this.KeyUp);

            //Subscribe events for area measurement
            Global.worldWindow.MouseMove += new MouseEventHandler(arealayer.MouseMove);
            Global.worldWindow.MouseDown += new MouseEventHandler(arealayer.MouseDown);
            Global.worldWindow.MouseUp += new MouseEventHandler(arealayer.MouseUp);

            //Set tool inactive by default
            layer.IsOn = false;
            arealayer.IsOn = false;
            menuItem.Checked = layer.IsOn;
            resultwindow.Visible = layer.IsOn;
        }

        /// <summary>
        /// Unload our plugin
        /// </summary>
        public override void Unload()
        {
            if (menuItem != null)
            {
                menuItem.Dispose();
                menuItem = null;
            }

            Global.worldWindow.MouseMove -= new MouseEventHandler(layer.MouseMove);
            Global.worldWindow.MouseDown -= new MouseEventHandler(layer.MouseDown);
            Global.worldWindow.MouseUp -= new MouseEventHandler(layer.MouseUp);
            Global.worldWindow.KeyUp -= new KeyEventHandler(this.KeyUp);

            Global.worldWindow.MouseMove -= new MouseEventHandler(arealayer.MouseMove);
            Global.worldWindow.MouseDown -= new MouseEventHandler(arealayer.MouseDown);
            Global.worldWindow.MouseUp -= new MouseEventHandler(arealayer.MouseUp);

            Global.worldWindow.CurrentWorld.RenderableObjects.Remove(layer);
            Global.worldWindow.CurrentWorld.RenderableObjects.Remove(arealayer);
            //DrawArgs.NewRootWidget.Remove(resultwindow);
        }

        /// <summary>
        /// Remove all the line segments
        /// </summary>
        public void ClearLines()
        {
            layer.Clear();
            layer.LineAdd(ConvertUnits.GetDisplayString(0.0));
            layer.Nodes.Clear();
        }

        /// <summary>
        /// Measuretool is enabled via the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemClicked(object sender, EventArgs e)
        {
            layer.IsOn = !layer.IsOn;
            menuItem.Checked = layer.IsOn;
            resultwindow.Visible = layer.IsOn;
        }

        /// <summary>
        /// Measure tool is activated via the M key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyUp(object sender, KeyEventArgs e)
        {
            //TODO: Respond to M key to activate and decativate Measure Tool
            if (e.KeyData == Keys.M)
            {
                ChangePluginStatus();
                e.Handled = true;
            }

            if (e.KeyData == Keys.T)
            {
                if (layer.IsOn)
                    PlotTerrain();
            }
        }

        /// <summary>
        /// Plot Terrrain Cross Section graph
        /// </summary>
        public void PlotTerrain()
        {
            ArrayList profile = layer.ElevationProfile();
            if (profile != null)
            {
                tg = new TerrainGraphForm(profile);
                tg.Show();
            }
            else
                MessageBox.Show("No line exists.", "Measuretool Error");
        }

        /// <summary>
        /// Turn plugin on and off
        /// </summary>
        public void ChangePluginStatus()
        {
            layer.IsOn = !layer.IsOn;
            menuItem.Checked = layer.IsOn;
            resultwindow.Visible = layer.IsOn;
        }
    }

    #region Delegates
    /// <summary>
    /// Updates GUI with new length in line mode
    /// </summary>
    /// <param name="LengthText"></param>
    public delegate void LineAddHandler(string LengthText);
    /// <summary>
    /// Update GUI with new area in polygon mode 
    /// </summary>
    /// <param name="AreaText"></param>
    public delegate void PolyAddHandler(string AreaText);
    #endregion

    /// <summary>
    /// This class is a Mouse gesture sensitive
    /// Line Drawing class.
    /// The mouse gestures should function as follows
    /// in 
    /// 1: Single Line mode
    ///     * Left Click begins a single measurement line
    ///     * Mouse move dynamically updates line
    ///     * Second left click confirms the position of the line
    ///     * Right Click clears points and restarts measurements
    /// 2: Multiline Mode
    ///     * Left Click begins Multilne
    ///     * Mouse move as above
    ///     * Each subsequent left click adds a segment to the line
    ///     * Right Click completes the measurement
    ///     * Second right click clears points and restarts measurements
    /// </summary>
    class MeasureToolLayer : LineFeature
    {
        public enum MeasureState
        {
            Idle,
            Measuring,
            Complete
        }

        #region Public Delegate Variables
        public LineAddHandler LineAdd;
        #endregion

        #region Private Variables

        private Point mouseDownPoint;
        private DrawArgs m_drawArgs;
        private bool isPointGotoEnabled;
        private double gclength;
        private MeasureState state = MeasureState.Idle;//Default state is idle
        private ArrayList m_nodes;
        private bool justClicked = false;

        #endregion

        #region Accessor Methods

        public override bool IsOn
        {
            get { return base.IsOn; }
            set
            {
                if (value == isOn)
                    return;

                base.IsOn = value;
                if (isOn)
                {
                    // Can't use point goto while measuring
                    isPointGotoEnabled = World.Settings.CameraIsPointGoto;
                    World.Settings.CameraIsPointGoto = false;
                    state = MeasureState.Idle;
                }
                else
                {
                    World.Settings.CameraIsPointGoto = isPointGotoEnabled;
                }
            }
        }

        /// <summary>
        /// Get and sets length based on great circle calculation
        /// </summary>
        public double GCLength
        {
            get { return gclength; }
            set { gclength = value; }
        }

        /// <summary>
        /// Gets and sets length based on following terrain
        /// </summary>
        public double TerrainLength
        {
            //return great circle length for now...
            get { return GCLength; }
        }

        /// <summary>
        /// Gets the Point3d array of nodes for the linefeature
        /// </summary>
        public ArrayList Nodes
        {
            get { return m_nodes; }
            set { m_nodes = value; }
        }

        public MeasureMode MeasureMode
        {
            get { return World.Settings.MeasureMode; }
            set { World.Settings.MeasureMode = value; }
        }

        #endregion

        public MeasureToolLayer(World world, DrawArgs drawArgs)
            : base("Measure Tool", world, new Point3d[0], Color.Red)
        {
            m_drawArgs = drawArgs;
            this.AltitudeMode = AltitudeMode.RelativeToGround;
            m_nodes = new ArrayList();
            this.LineWidth = 5f;
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (DrawArgs.MouseCursor == CursorType.Arrow)
                // Use our cursor when the mouse isn't over other elements requiring different cursor
                DrawArgs.MouseCursor = CursorType.Measure;

            base.Render(drawArgs);
        }

        #region Mouse Handlers
        /// <summary>
        /// Respond to mouse movements with dynamic line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseMove(object sender, MouseEventArgs e)
        {
            // Implementation of dynamic generation of line segments for a 
            // preview as the mouse moves
            if (isOn)
            {
                if (state == MeasureState.Measuring)
                {
                    if (justClicked == true)
                    {
                        m_points.RemoveLast();
                    }
                    justClicked = true;
                    Point currentPosition = DrawArgs.LastMousePosition;
                    Angle Latitude, Longitude;
                    if (m_drawArgs.WorldCamera.Altitude > 500000)
                    {
                        m_drawArgs.WorldCamera.PickingRayIntersection(
                            currentPosition.X,
                            currentPosition.Y,
                            out Latitude,
                            out Longitude);
                    }
                    else
                    {
                        m_drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(
                            currentPosition.X,
                            currentPosition.Y,
                            out Latitude,
                            out Longitude,
                            m_drawArgs.CurrentWorld);
                    }
                    Point3d lastPoint = new Point3d(Longitude.Degrees, Latitude.Degrees, 0);
                    Point3d startPoint = m_points.First.Value;
                    // if distance is less than the specfied, snap the point to the first point
                    double distance = Math.Sqrt(Math.Pow((startPoint.X - lastPoint.X), 2.0) + Math.Pow((startPoint.Y - lastPoint.Y), 2.0));
                    // distance is relative to the zoom level to allow for
                    // various levels of precision
                    double earthDist = m_drawArgs.WorldCamera.Altitude / 10000000;
                    if (distance < earthDist)
                    {
                        lastPoint = startPoint;
                    }
                    AddPoint(lastPoint);
                }
            }
        }

        /// <summary>
        /// Store position of mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (!isOn)
                return;
            mouseDownPoint = DrawArgs.LastMousePosition;
        }

        /// <summary>
        /// Add point or deactivate tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (!isOn)
                return;
            // Test if mouse was clicked and dragged
            if (MouseDragged())
                return;
            // Cancel selection if right mouse button clicked
            if (e.Button == MouseButtons.Right)
            {
                MouseRightClick();
                return;
            }
            // Do nothing for all other mouse buttons clicked
            if (e.Button != MouseButtons.Left)
                return;

            Angle Latitude, Longitude;
            if (m_drawArgs.WorldCamera.Altitude > 500000)
            {
                m_drawArgs.WorldCamera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out Latitude,
                    out Longitude);
            }
            else
            {
                m_drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out Latitude,
                    out Longitude,
                    m_drawArgs.CurrentWorld);
            }

            // Need to ensure Line Feature subsegments along the curvature
            // of the planet
            if (World.Settings.MeasureMode == MeasureMode.Single)
            {
                // check if nodes array size is less than 2 and add point
                if (Nodes.Count < 2)
                {
                    Point3d p = new Point3d(Longitude.Degrees, Latitude.Degrees, 0);
                    Nodes.Add(p);
                    AddPoint(p);
                    LineAdd(ConvertUnits.GetDisplayString(CalcLength()));
                    if (m_points.Count == 1)
                    {
                        state = MeasureState.Measuring;
                        justClicked = false;
                    }
                    else
                    {
                        state = MeasureState.Complete;
                    }
                }
            }
            else if (World.Settings.MeasureMode == MeasureMode.Multi)
            {
                Point3d p = new Point3d(Longitude.Degrees, Latitude.Degrees, 0);
                Nodes.Add(p);
                AddPoint(p);
                LineAdd(ConvertUnits.GetDisplayString(CalcLength()));
                state = MeasureState.Measuring;
                justClicked = false;
            }
        }

        /// <summary>
        /// Detect drag to allow globe movements
        /// </summary>
        /// <returns></returns>
        private bool MouseDragged()
        {
            int dx = DrawArgs.LastMousePosition.X - mouseDownPoint.X;
            int dy = DrawArgs.LastMousePosition.Y - mouseDownPoint.Y;
            if (dx * dx + dy * dy > 3 * 3)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Deactivate tool on right click
        /// </summary>
        private void MouseRightClick()
        {
            // Switch states in multiline mode
            if (state != MeasureState.Idle)
            {
                switch (state)
                {
                    case MeasureState.Measuring:
                        state = MeasureState.Complete;
                        return;
                    // clear when completed segment
                    case MeasureState.Complete:
                        //if (World.Settings.MeasureMode == MeasureMode.Multi) 
                        //{
                        this.Points = new Point3d[0];
                        this.Nodes.Clear();
                        this.NeedsUpdate = true;
                        LineAdd(ConvertUnits.GetDisplayString(0.0));
                        //}
                        state = MeasureState.Idle;
                        return;
                }
            }
            else
                this.IsOn = false;
        }
        #endregion

        #region Calculation Methods

        /// <summary>
        /// This Method calculates total length of the linefeature
        /// </summary>
        /// <returns>Length of the line</returns>
        private double CalcLength()
        {
            double accumulatedlength = 0.0;
            for (int i = 1; i < Points.Length; i++)
            {
                Point3d previous = Points[i - 1];
                Point3d current = Points[i];
                Angle angularDistance = World.ApproxAngularDistance
                    (Angle.FromDegrees(previous.X),
                    Angle.FromDegrees(previous.Y),
                    Angle.FromDegrees(current.X),
                    Angle.FromDegrees(current.Y));
                accumulatedlength += angularDistance.Radians * m_world.EquatorialRadius;
            }
            this.GCLength = accumulatedlength;
            return accumulatedlength;
        }

        public ArrayList ElevationProfile()
        {
            World world = this.m_world;

            int maxSamples = 200;

            Angle startLat, startLon, endLat, endLon = Angle.Zero;
            ArrayList terrainLine = new ArrayList();

            if (Nodes.Count > 1)
            {
                double totalArcLengthDegrees = 0;

                //divide up the number of available samples proportional to length
                for (int node = 1; node < Nodes.Count; node++)
                {
                    Point3d endPoint = (Point3d)Nodes[node];
                    Point3d startPoint = (Point3d)Nodes[node - 1];

                    endLon = Angle.FromDegrees(endPoint.X);
                    endLat = Angle.FromDegrees(endPoint.Y);
                    startLon = Angle.FromDegrees(startPoint.X);
                    startLat = Angle.FromDegrees(startPoint.Y);

                    Angle angularDistance = World.ApproxAngularDistance(startLat, startLon, endLat, endLon);

                    totalArcLengthDegrees += angularDistance.Degrees;
                }

                MessageBox.Show("Total length: " + totalArcLengthDegrees.ToString() + " deg");
                // make a terrain profile spanning each of the line segments in turn
                for (int node = 1; node < Nodes.Count; node++)
                {
                    //MessageBox.Show(Nodes.Length.ToString() + ", " + node.ToString());
                    Point3d endPoint = (Point3d)Nodes[node];
                    Point3d startPoint = (Point3d)Nodes[node - 1];

                    endLon = Angle.FromDegrees(endPoint.X);
                    endLat = Angle.FromDegrees(endPoint.Y);
                    startLon = Angle.FromDegrees(startPoint.X);
                    startLat = Angle.FromDegrees(startPoint.Y);

                    Angle angularDistance = World.ApproxAngularDistance(startLat, startLon, endLat, endLon);

                    int samples = (int)(maxSamples * angularDistance.Degrees / totalArcLengthDegrees);

                    double sampsPerDegree = samples / angularDistance.Degrees;
                    double elev0 = world.TerrainAccessor.GetElevationAt(startLat.Degrees, startLon.Degrees, sampsPerDegree);
                    double x0 = 0;

                    if (node != 1)
                        x0 = ((Point2d)terrainLine[terrainLine.Count - 1]).X;

                    // total "horizontal" distance
                    double dist = angularDistance.Radians * (world.EquatorialRadius + elev0);

                    // horizontal spacing of points in profile
                    double deltaX = dist / (samples - 1);

                    MFW3D.Point2d initialPoint = new MFW3D.Point2d(x0, elev0);

                    if (node == 1)
                        terrainLine.Add(initialPoint);

                    double stepSize = 1.0 / (double)samples;

                    for (int i = 1; i < samples; i++)
                    {
                        Angle lat, lon = Angle.Zero;

                        // desired fraction of arc length along great circle
                        double t = i * stepSize;
                        World.IntermediateGCPoint((float)t, startLat, startLon, endLat, endLon, angularDistance, out lat, out lon);

                        // elevation at this point
                        double elevi = (double)world.TerrainAccessor.GetElevationAt(lat.Degrees, lon.Degrees, sampsPerDegree);

                        // horizontal length from startpoint of profile
                        double xi = deltaX * i + x0;

                        MFW3D.Point2d pointI = new MFW3D.Point2d(xi, elevi);
                        terrainLine.Add(pointI);
                    }
                }
                return terrainLine;
            }
            return null;
        }
        #endregion
    }

    /// <summary>
    /// This class is used for area measurements
    /// extends polygon feature to be sensitive to
    /// mousemovements
    /// </summary>
    class MeasureAreaLayer : PolygonFeature
    {
        #region Private variables
        private DrawArgs m_drawArgs;
        private ArrayList m_nodes;
        private Point mouseDownPoint;
        private MeasureState state = MeasureState.Idle;
        private bool isPointGotoEnabled;
        #endregion

        public enum MeasureState
        {
            Idle,
            Measuring,
            Complete
        }

        #region Public Delegate Variables
        public PolyAddHandler PolyAdd;
        #endregion

        #region Accessor Methods
        /// <summary>
        /// Access measurement polygon nodes
        /// </summary>
        public ArrayList Nodes
        {
            get { return m_nodes; }
            set { m_nodes = value; }
        }

        /// <summary>
        /// Overridden ison method to disable point to go
        /// </summary>
        public override bool IsOn
        {
            get { return base.IsOn; }
            set
            {
                if (value == isOn)
                    return;

                base.IsOn = value;
                if (isOn)
                {
                    // Can't use point goto while measuring
                    isPointGotoEnabled = World.Settings.CameraIsPointGoto;
                    World.Settings.CameraIsPointGoto = false;
                    state = MeasureState.Idle;
                }
                else
                {
                    World.Settings.CameraIsPointGoto = isPointGotoEnabled;
                }
            }
        }
        #endregion

        #region RenderableObject Methods
        /// <summary>
        /// Constructor presets polygonfeature properties
        /// </summary>
        /// <param name="world"></param>
        /// <param name="drawArgs"></param>
        public MeasureAreaLayer(World world, DrawArgs drawArgs, LinearRing outerRing)
            : base("Measure Area", world, outerRing, null, Color.Red)
        {
            m_drawArgs = drawArgs;
            this.AltitudeMode = AltitudeMode.RelativeToGround;
            m_nodes = new ArrayList();
            this.Outline = false;
            this.Fill = true;
            this.Extrude = false;
        }

        /// <summary>
        /// Allow changing of mouse cursor during rendering
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Render(DrawArgs drawArgs)
        {
            if (DrawArgs.MouseCursor == CursorType.Arrow)
                // Use our cursor when the mouse isn't over other elements requiring different cursor
                DrawArgs.MouseCursor = CursorType.Measure;

            base.Render(drawArgs);
        }
        #endregion

        #region Mouse Handlers
        /// <summary>
        /// Respond to Mousemovements with dynamic polygon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseMove(object sender, MouseEventArgs e)
        {
            // Implementation of dynamic generation of line segments for a 
            // preview as the mouse moves
            if (isOn)
            {
                if (state == MeasureState.Measuring)
                {
                    /*
                    if (justClicked == true)
                    {
                    }
                    justClicked = true;
                    */
                    LinearRing currentRing = this.OuterRing;
                    Point currentPosition = DrawArgs.LastMousePosition;
                    Angle Latitude, Longitude;
                    if (m_drawArgs.WorldCamera.Altitude > 500000)
                    {
                        m_drawArgs.WorldCamera.PickingRayIntersection(
                            currentPosition.X,
                            currentPosition.Y,
                            out Latitude,
                            out Longitude);
                    }
                    else
                    {
                        m_drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(
                            currentPosition.X,
                            currentPosition.Y,
                            out Latitude,
                            out Longitude,
                            m_drawArgs.CurrentWorld);
                    }
                    Point3d lastPoint = new Point3d(Longitude.Degrees, Latitude.Degrees, 0);
                    Point3d startPoint = currentRing.Points[0];
                    // if distance is less than the specfied, snap the point to the first point
                    double distance = Math.Sqrt(Math.Pow((startPoint.X - lastPoint.X), 2.0) + Math.Pow((startPoint.Y - lastPoint.Y), 2.0));
                    // distance is relative to the zoom level to allow for
                    // various levels of precision
                    double earthDist = m_drawArgs.WorldCamera.Altitude / 10000000;
                    if (distance < earthDist)
                    {
                        lastPoint = startPoint;
                    }
                    if (currentRing.Points.Length > 2)
                        currentRing.Points[currentRing.Points.Length - 2] = lastPoint;
                    this.OuterRing = currentRing;
                }
            }
        }

        /// <summary>
        /// Store last mousedown point to detect dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (!isOn)
                return;
            mouseDownPoint = DrawArgs.LastMousePosition;
        }

        /// <summary>
        /// Complete click response, add point or deactivate tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (!isOn)
                return;
            // Test if mouse was clicked and dragged
            if (MouseDragged())
                return;
            // Cancel selection if right mouse button clicked
            if (e.Button == MouseButtons.Right)
            {
                MouseRightClick();
                return;
            }
            // Do nothing for all other mouse buttons clicked
            if (e.Button != MouseButtons.Left)
                return;

            Angle Latitude, Longitude;
            if (m_drawArgs.WorldCamera.Altitude > 500000)
            {
                m_drawArgs.WorldCamera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out Latitude,
                    out Longitude);
            }
            else
            {
                m_drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out Latitude,
                    out Longitude,
                    m_drawArgs.CurrentWorld);
            }

            if (World.Settings.MeasureMode == MeasureMode.Multi)
            {
                Point3d p = new Point3d(Longitude.Degrees, Latitude.Degrees, 0);
                Nodes.Add(p);
                state = MeasureState.Measuring;
                LinearRing currentRing = this.OuterRing;
                //create newRing
                LinearRing newRing = new LinearRing();
                Point3d[] newPoints = new Point3d[currentRing.Points.Length + 1];
                //copy current points over
                if (currentRing.Points.Length > 2)
                {
                    for (int i = 0; i < currentRing.Points.Length - 1; i++)
                        newPoints[i] = currentRing.Points[i];
                    newPoints[newPoints.Length - 2] = p;
                    newPoints[newPoints.Length - 1] = currentRing.Points[0];

                    Console.WriteLine(CalcEllipsoidArea());
                }
                else if (currentRing.Points.Length == 2)
                {
                    newPoints = new Point3d[currentRing.Points.Length + 2];
                    for (int i = 0; i < currentRing.Points.Length; i++)
                        newPoints[i] = currentRing.Points[i];
                    newPoints[newPoints.Length - 2] = p;
                    newPoints[newPoints.Length - 1] = currentRing.Points[0];
                }
                else
                {
                    for (int i = 0; i < currentRing.Points.Length; i++)
                        newPoints[i] = currentRing.Points[i];
                    newPoints[newPoints.Length - 1] = p;
                }
                newRing.Points = newPoints;
                this.OuterRing = newRing;
                PolyAdd(ConvertUnits.GetAreaDisplayString(CalcArea()));
            }
        }

        /// <summary>
        /// Allow movement of globe while measuring
        /// </summary>
        /// <returns></returns>
        private bool MouseDragged()
        {
            int dx = DrawArgs.LastMousePosition.X - mouseDownPoint.X;
            int dy = DrawArgs.LastMousePosition.Y - mouseDownPoint.Y;
            if (dx * dx + dy * dy > 3 * 3)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Deactivate tool on right click
        /// </summary>
        private void MouseRightClick()
        {
            // Switch states in multiline mode
            if (state != MeasureState.Idle)
            {
                switch (state)
                {
                    case MeasureState.Measuring:
                        state = MeasureState.Complete;
                        return;
                    // clear when completed segment
                    case MeasureState.Complete:
                        //if (World.Settings.MeasureMode == MeasureMode.Multi) 
                        //{
                        LinearRing newRing = new LinearRing();
                        newRing.Points = new Point3d[0];
                        PolyAdd(ConvertUnits.GetAreaDisplayString(0.0));
                        this.OuterRing = newRing;
                        this.Nodes.Clear();
                        //this.NeedsUpdate = true;

                        //}
                        state = MeasureState.Idle;
                        return;
                }
            }
            else
                this.IsOn = false;
        }
        #endregion

        #region Calculation Methods

        /// <summary>
        /// Perform area calculations using planet model
        /// 1.Basic calculation using sphere model
        /// 2.Refinements using ellipsoid and terrain
        /// </summary>
        /// <returns></returns>
        public double CalcArea()
        {
            //TODO:Implement Sphere based calculation
            //TODO:Implement Ellipsoid based calculation(use grass)
            //http://mpa.itc.it/markus/grass63progman/area__poly1_8c-source.html
            //TODO:Implement Terrain based calculation
            //http://grass.ibiblio.org/gdp/html_grass63/r.surf.area.html
            return CalcEllipsoidArea();
        }


        /************************************************************
         * Ellipsoid based area calculation method borrowed from 
         * GRASS GIS.
         * Refer to: http://mpa.itc.it/markus/grass63progman/area__poly1_8c.html
         * ***********************************************************/
        /// <summary>
        /// Calculate the polygon area using a spherical earth model
        /// </summary>
        /// <returns></returns>
        static double QA, QB, QC;
        static double QbarA, QbarB, QbarC, QbarD;
        static double AE;
        static double Qp;
        static double E;
        static double M_PI_2 = Math.PI / 2.0;
        static double M_PI = Math.PI;
        static double TWOPI = Math.PI * 2;

        /// <summary>
        /// Ellipsoid helper method
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static double Q(double x)
        {
            double sinx, sinx2;

            sinx = Math.Sin(x);
            sinx2 = sinx * sinx;

            return sinx * (1 + sinx2 * (QA + sinx2 * (QB + sinx2 * QC)));
        }

        /// <summary>
        /// Ellipsoid helper method
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static double Qbar(double x)
        {
            double cosx, cosx2;

            cosx = Math.Cos(x);
            cosx2 = cosx * cosx;

            return cosx * (QbarA + cosx2 * (QbarB + cosx2 * (QbarC + cosx2 * QbarD)));
        }

        /// <summary>
        /// Initialize ellipsoid with axis length and eccentricity
        /// </summary>
        /// <param name="a">Semi-Major axis in Meters</param>
        /// <param name="e2">Ellipsoid eccentricity squared </param>
        /// <returns></returns>
        int G_begin_ellipsoid_polygon_area(double a, double e2)
        {
            double e4, e6;

            e4 = e2 * e2;
            e6 = e4 * e2;

            AE = a * a * (1 - e2);

            QA = (2.0 / 3.0) * e2;
            QB = (3.0 / 5.0) * e4;
            QC = (4.0 / 7.0) * e6;

            QbarA = -1.0 - (2.0 / 3.0) * e2 - (3.0 / 5.0) * e4 - (4.0 / 7.0) * e6;
            QbarB = (2.0 / 9.0) * e2 + (2.0 / 5.0) * e4 + (4.0 / 7.0) * e6;
            QbarC = -(3.0 / 25.0) * e4 - (12.0 / 35.0) * e6;
            QbarD = (4.0 / 49.0) * e6;

            Qp = Q(M_PI_2);
            E = 4 * M_PI * Qp * AE;
            if (E < 0.0) E = -E;

            return 0;
        }

        /// <summary>
        /// Calculate area from a set of lat-lon points
        /// </summary>
        /// <param name="points">List of points</param>
        /// <returns></returns>
        double G_ellipsoid_polygon_area(Point3d[] points)
        {
            double x1, y1, x2, y2, dx, dy;
            double Qbar1, Qbar2;
            double area;

            int n = points.Length;

            x2 = Angle.FromDegrees(points[n - 1].X).Radians;
            y2 = Angle.FromDegrees(points[n - 1].Y).Radians;
            Qbar2 = Qbar(y2);

            area = 0.0;

            int i = 0;

            while (--n >= 0)
            {
                x1 = x2;
                y1 = y2;
                Qbar1 = Qbar2;


                x2 = Angle.FromDegrees(points[i].X).Radians; ;
                y2 = Angle.FromDegrees(points[i].Y).Radians; ;
                Qbar2 = Qbar(y2);

                i++;

                if (x1 > x2)
                    while (x1 - x2 > M_PI)
                        x2 += TWOPI;
                else if (x2 > x1)
                    while (x2 - x1 > M_PI)
                        x1 += TWOPI;

                dx = x2 - x1;
                area += dx * (Qp - Q(y2));

                if ((dy = y2 - y1) != 0.0)
                    area += dx * Q(y2) - (dx / dy) * (Qbar2 - Qbar1);
            }
            if ((area *= AE) < 0.0)
                area = -area;

            /* kludge - if polygon circles the south pole the area will be
             * computed as if it cirlced the north pole. The correction is
             * the difference between total surface area of the earth and
            * the "north pole" area.
             */
            if (area > E) area = E;
            if (area > E / 2) area = E - area;

            return area;
        }

        /// <summary>
        /// Calculate area of polygon based on ellipsoid
        /// </summary>
        /// <returns></returns>
        double CalcEllipsoidArea()
        {
            //Initialize ellipsoid(start with sphere)
            G_begin_ellipsoid_polygon_area(m_world.EquatorialRadius, 0);
            double area = G_ellipsoid_polygon_area(OuterRing.Points);
            return area;
        }
        #endregion
    }

    /// <summary>
    /// This class displays measurement results in a Worldwind Style widget
    /// </summary>
    class MeasureResult : MFW3D.NewWidgets.FormWidget
    {
        private MFW3D.NewWidgets.TextLabel measureLabel = null;
        private MFW3D.NewWidgets.TextLabel measureResult = null;

        public MeasureResult()
            : base("Measurement Results")
        {
            //Populate Widget
            Initialize();
        }

        private void Initialize()
        {
            this.Name = "Measurement Results";
            this.ClientSize = new System.Drawing.Size(500, 183);
            this.ParentWidget = DrawArgs.NewRootWidget;

            measureLabel = new MFW3D.NewWidgets.TextLabel();
            measureLabel.Name = "Measurement Type";
            measureLabel.Text = "Length";
            measureLabel.Visible = true;
            measureLabel.ParentWidget = this;
            measureLabel.CountHeight = false;
            measureLabel.CountWidth = true;
            measureLabel.ClientLocation = new System.Drawing.Point(40, 65);


            measureResult = new MFW3D.NewWidgets.TextLabel();
            measureResult.Name = "Measurement Result";
            measureResult.Text = "0.0";
            measureResult.Visible = true;
            measureResult.ParentWidget = this;
            measureResult.CountHeight = false;
            measureResult.CountWidth = true;
            measureResult.ClientLocation = new System.Drawing.Point(140, 65);

            this.ChildWidgets.Add(measureLabel);
        }

        #region update Method
        public void LengthText(string text)
        {
            measureResult.Text = text;
        }
        #endregion
    }

    /// <summary>
    /// Form to display the terrain profile plot
    /// </summary>
    internal class TerrainGraphForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl1.EditModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)));
            this.zedGraphControl1.IsAutoScrollRange = false;
            this.zedGraphControl1.IsEnableHEdit = false;
            this.zedGraphControl1.IsEnableHPan = true;
            this.zedGraphControl1.IsEnableHZoom = true;
            this.zedGraphControl1.IsEnableVEdit = false;
            this.zedGraphControl1.IsEnableVPan = true;
            this.zedGraphControl1.IsEnableVZoom = true;
            this.zedGraphControl1.IsPrintFillPage = true;
            this.zedGraphControl1.IsPrintKeepAspectRatio = true;
            this.zedGraphControl1.IsScrollY2 = false;
            this.zedGraphControl1.IsShowContextMenu = true;
            this.zedGraphControl1.IsShowCopyMessage = true;
            this.zedGraphControl1.IsShowCursorValues = false;
            this.zedGraphControl1.IsShowHScrollBar = false;
            this.zedGraphControl1.IsShowPointValues = false;
            this.zedGraphControl1.IsShowVScrollBar = false;
            this.zedGraphControl1.IsSynchronizeXAxes = false;
            this.zedGraphControl1.IsSynchronizeYAxes = false;
            this.zedGraphControl1.IsZoomOnMouseCenter = false;
            this.zedGraphControl1.LinkButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl1.LinkModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)));
            this.zedGraphControl1.Location = new System.Drawing.Point(12, 12);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl1.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.zedGraphControl1.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zedGraphControl1.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.zedGraphControl1.PointDateFormat = "g";
            this.zedGraphControl1.PointValueFormat = "G";
            this.zedGraphControl1.ScrollMaxX = 0;
            this.zedGraphControl1.ScrollMaxY = 0;
            this.zedGraphControl1.ScrollMaxY2 = 0;
            this.zedGraphControl1.ScrollMinX = 0;
            this.zedGraphControl1.ScrollMinY = 0;
            this.zedGraphControl1.ScrollMinY2 = 0;
            this.zedGraphControl1.Size = new System.Drawing.Size(597, 247);
            this.zedGraphControl1.TabIndex = 0;
            this.zedGraphControl1.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl1.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.zedGraphControl1.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.zedGraphControl1.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.zedGraphControl1.ZoomStepFraction = 0.1;
            // 
            // TerrainGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 282);
            this.Controls.Add(this.zedGraphControl1);
            this.Name = "TerrainGraph";
            this.Text = "TerrainGraph";
            this.Resize += new System.EventHandler(this.TerrainGraph_Resize);
            this.Load += new System.EventHandler(this.TerrainGraph_Load);
            this.ResumeLayout(false);
        }
        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private ArrayList elevProfile;

        public TerrainGraphForm(ArrayList profile)
        {
            this.elevProfile = profile;
            InitializeComponent();
        }

        private void TerrainGraph_Load(object sender, EventArgs e)
        {
            // Setup the graph
            CreateGraph(zedGraphControl1);
            // Size the control to fill the form with a margin
            SetSize();
        }

        private void SetSize()
        {
            zedGraphControl1.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zedGraphControl1.Size = new Size(ClientRectangle.Width - 20,
                                    ClientRectangle.Height - 20);
        }

        private void TerrainGraph_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        //TODO: Make a better graph style
        private void CreateGraph(ZedGraphControl zgc)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            myPane.XAxis.Scale.MaxGrace = 0.05;

            // Set the Titles
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = "Distance (m)";
            myPane.YAxis.Title.Text = "Elevation (m)";

            PointPairList list1 = new PointPairList();

            foreach (MFW3D.Point2d pt in elevProfile)
            {
                list1.Add(pt.X, pt.Y);
            }

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            //LineItem myCurve = myPane.AddCurve("Terrain",
            //	  list1, Color.Red, SymbolType.Diamond);

            LineItem myCurve = myPane.AddCurve("Terrain", list1, Color.Red, SymbolType.None);
            // Generate a blue curve with circle
            // symbols, and "Piper" in the legend
            //LineItem myCurve2 = myPane.AddCurve("Piper",
            //	  list2, Color.Blue, SymbolType.Circle);

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.AxisChange();
        }
    }
}