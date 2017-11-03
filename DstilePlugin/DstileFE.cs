//----------------------------------------------------------------------------
// NAME: Dstile GUI Form
// VERSION: 0.1
// DESCRIPTION: Allows Drag and Drop Import of Imagery
// DEVELOPER: Tisham Dhar
// WEBSITE: http:\\www.apogee.com.au
// REFERENCES: 
//----------------------------------------------------------------------------
//
// Plugin was developed by Apogee Imaging International
// This file is in the Public Domain, and comes with no warranty. 

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.Terrain;

//Import GDAL Utilities
using GDAL;
using OSR;

namespace DstileGUI
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    class DstileFrontEnd : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox inputFileTextBox;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RadioButton imageRadioButton;
        private System.Windows.Forms.RadioButton demRadioButton;
        private System.Windows.Forms.TextBox layerNameTextBox;
        private System.Windows.Forms.Label tileSizeLabel;
        private TextBox targetXMLTextBox;
        private Button createXMLButton;
        private ListBox tileListBox;

        //private Thread dstileThread;

        private Boolean srcProjNeeded = true;
        private Label srcProjLabel;
        private TextBox srcProjTextBox;
        private String arguments = "";
        private Button choosefw;
        private TextBox fwlocText;
        private Button button1;
        private TextBox destFolderTextBox;
        private DstilePlugin srcplugin;
        private Label extentLabel;
        private Label northlab;
        private Label southlab;
        private Label westlab;
        private Label eastlab;
        private System.Windows.Forms.Timer timer1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel currentStatusLabel;
        private ToolStripProgressBar progressBar1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private IContainer components;
        private ListBox formatListBox;
        private Label formatLabel;

        private object[] lztsdvalues= new object[] {
            20,
            18,
            15,
            12,
            10,
            9,
            6,
            5,
            4,
            3,
            2,
            1,
            0.5,
            0.25,
            0.125,
            0.0625};

        private object[] formatvalues = new object[] {
            "png",
            "jpg",
            "bmp",
            "dds"};
        

        //private XmlDocument xmlDoc = null;

        public DstileFrontEnd(DstilePlugin source)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.tileListBox.Items.AddRange(lztsdvalues);
            this.formatListBox.Items.AddRange(formatvalues);

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            //Load XML Configuration
            srcplugin = source;
            LoadSaveXmlConfig(true);
        }

        /// <summary>
        /// Restore/Save XML Configuration
        /// </summary>
        /// <param name="load">Specifies whether the settings are being loaded (true) or saved (false)</param>
        private void LoadSaveXmlConfig(bool load)
        {
            try
            {
                //TODO: Need a better way to locate Plugin Directory and Load settings
                //					if(m_layer.isInitialized)
                //					{
                string PluginDirectory = srcplugin.PluginDirectory;
                string settings = Path.Combine(PluginDirectory, "Settings.xml");
                Console.WriteLine(settings);
                //create file if it does not exist
                if (!System.IO.File.Exists(settings))
                {
                    getFWLoc();
                    XmlDocument doc = new XmlDocument();
                    XmlNode settingsRoot = doc.CreateElement("DstileSettings");
                    XmlNode fwtools = doc.CreateElement("FWTools");
                    fwtools.InnerText = fwlocText.Text;
                    settingsRoot.AppendChild(fwtools);
                    doc.AppendChild(settingsRoot);
                    doc.Save(settings);
                    return;
                }
                if (System.IO.File.Exists(settings))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(settings);
                    XmlNodeList list = doc.GetElementsByTagName("FWTools");
                    foreach (XmlNode node in list)
                    {
                        if (load)
                            fwlocText.Text = node.InnerText;
                            
                        else
                            node.InnerText = fwlocText.Text;

                    }
                    if (load)
                    {
                        //TODO:Create Layer from Settings
                        XmlNodeList layerlist = doc.GetElementsByTagName("QuadTileSet");
                        //Reuse configuration loader or some sort of fromXML method
                        
                    }
                    else
                    {
                        //TODO: Add settings for layers
                        XmlNode layersNode = srcplugin.DstileLayers.ToXml(doc);
                        XmlNode settingsRoot = doc.FirstChild;
                        settingsRoot.AppendChild(layersNode);
                    }
                    doc.Save(settings);
                    //						
                }
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DstileFrontEnd));
            this.inputFileTextBox = new System.Windows.Forms.TextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.processButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.imageRadioButton = new System.Windows.Forms.RadioButton();
            this.demRadioButton = new System.Windows.Forms.RadioButton();
            this.layerNameTextBox = new System.Windows.Forms.TextBox();
            this.tileSizeLabel = new System.Windows.Forms.Label();
            this.targetXMLTextBox = new System.Windows.Forms.TextBox();
            this.createXMLButton = new System.Windows.Forms.Button();
            this.tileListBox = new System.Windows.Forms.ListBox();
            this.srcProjLabel = new System.Windows.Forms.Label();
            this.srcProjTextBox = new System.Windows.Forms.TextBox();
            this.choosefw = new System.Windows.Forms.Button();
            this.fwlocText = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.destFolderTextBox = new System.Windows.Forms.TextBox();
            this.extentLabel = new System.Windows.Forms.Label();
            this.northlab = new System.Windows.Forms.Label();
            this.southlab = new System.Windows.Forms.Label();
            this.westlab = new System.Windows.Forms.Label();
            this.eastlab = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.currentStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.formatListBox = new System.Windows.Forms.ListBox();
            this.formatLabel = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputFileTextBox
            // 
            this.inputFileTextBox.Location = new System.Drawing.Point(24, 63);
            this.inputFileTextBox.Name = "inputFileTextBox";
            this.inputFileTextBox.Size = new System.Drawing.Size(368, 20);
            this.inputFileTextBox.TabIndex = 0;
            this.inputFileTextBox.Text = "Input file";
            this.inputFileTextBox.TextChanged += new System.EventHandler(this.inputFileTextBox_TextChanged);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(408, 63);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(73, 20);
            this.openButton.TabIndex = 1;
            this.openButton.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // processButton
            // 
            this.processButton.Enabled = false;
            this.processButton.Location = new System.Drawing.Point(408, 346);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(73, 20);
            this.processButton.TabIndex = 8;
            this.processButton.Text = "Process";
            this.processButton.Click += new System.EventHandler(this.processButton_Click);
            // 
            // imageRadioButton
            // 
            this.imageRadioButton.Checked = true;
            this.imageRadioButton.Enabled = false;
            this.imageRadioButton.Location = new System.Drawing.Point(24, 231);
            this.imageRadioButton.Name = "imageRadioButton";
            this.imageRadioButton.Size = new System.Drawing.Size(104, 20);
            this.imageRadioButton.TabIndex = 6;
            this.imageRadioButton.TabStop = true;
            this.imageRadioButton.Text = "Image";
            this.imageRadioButton.CheckedChanged += new System.EventHandler(this.imageRadioButton_CheckedChanged);
            // 
            // demRadioButton
            // 
            this.demRadioButton.Enabled = false;
            this.demRadioButton.Location = new System.Drawing.Point(121, 231);
            this.demRadioButton.Name = "demRadioButton";
            this.demRadioButton.Size = new System.Drawing.Size(104, 20);
            this.demRadioButton.TabIndex = 7;
            this.demRadioButton.Text = "DEM";
            this.demRadioButton.CheckedChanged += new System.EventHandler(this.demRadioButton_CheckedChanged);
            // 
            // layerNameTextBox
            // 
            this.layerNameTextBox.Location = new System.Drawing.Point(24, 140);
            this.layerNameTextBox.Name = "layerNameTextBox";
            this.layerNameTextBox.Size = new System.Drawing.Size(144, 20);
            this.layerNameTextBox.TabIndex = 4;
            this.layerNameTextBox.Text = "Layer name";
            this.layerNameTextBox.TextChanged += new System.EventHandler(this.layerNameTextBox_TextChanged);
            // 
            // tileSizeLabel
            // 
            this.tileSizeLabel.Location = new System.Drawing.Point(21, 185);
            this.tileSizeLabel.Name = "tileSizeLabel";
            this.tileSizeLabel.Size = new System.Drawing.Size(70, 20);
            this.tileSizeLabel.TabIndex = 13;
            this.tileSizeLabel.Text = "Tile Size:";
            // 
            // targetXMLTextBox
            // 
            this.targetXMLTextBox.Enabled = false;
            this.targetXMLTextBox.Location = new System.Drawing.Point(24, 310);
            this.targetXMLTextBox.Name = "targetXMLTextBox";
            this.targetXMLTextBox.ReadOnly = true;
            this.targetXMLTextBox.Size = new System.Drawing.Size(368, 20);
            this.targetXMLTextBox.TabIndex = 10;
            this.targetXMLTextBox.Text = "Target XML";
            // 
            // createXMLButton
            // 
            this.createXMLButton.Enabled = false;
            this.createXMLButton.Location = new System.Drawing.Point(408, 310);
            this.createXMLButton.Name = "createXMLButton";
            this.createXMLButton.Size = new System.Drawing.Size(73, 20);
            this.createXMLButton.TabIndex = 9;
            this.createXMLButton.Text = "Create XML";
            this.createXMLButton.UseVisualStyleBackColor = true;
            this.createXMLButton.Click += new System.EventHandler(this.createXMLButton_Click);
            // 
            // tileListBox
            // 
            this.tileListBox.Font = new System.Drawing.Font("Arial", 9F);
            this.tileListBox.FormattingEnabled = true;
            this.tileListBox.ItemHeight = 15;
            this.tileListBox.Location = new System.Drawing.Point(77, 183);
            this.tileListBox.Name = "tileListBox";
            this.tileListBox.Size = new System.Drawing.Size(78, 19);
            this.tileListBox.TabIndex = 20;
            // 
            // srcProjLabel
            // 
            this.srcProjLabel.AutoSize = true;
            this.srcProjLabel.Location = new System.Drawing.Point(21, 102);
            this.srcProjLabel.Name = "srcProjLabel";
            this.srcProjLabel.Size = new System.Drawing.Size(94, 13);
            this.srcProjLabel.TabIndex = 17;
            this.srcProjLabel.Text = "Source Projection:";
            // 
            // srcProjTextBox
            // 
            this.srcProjTextBox.Location = new System.Drawing.Point(121, 99);
            this.srcProjTextBox.Name = "srcProjTextBox";
            this.srcProjTextBox.ReadOnly = true;
            this.srcProjTextBox.Size = new System.Drawing.Size(271, 20);
            this.srcProjTextBox.TabIndex = 18;
            this.srcProjTextBox.Text = "Not required";
            // 
            // choosefw
            // 
            this.choosefw.Location = new System.Drawing.Point(408, 21);
            this.choosefw.Name = "choosefw";
            this.choosefw.Size = new System.Drawing.Size(73, 21);
            this.choosefw.TabIndex = 22;
            this.choosefw.Text = "Choose";
            this.choosefw.Click += new System.EventHandler(this.fwloc_Click);
            // 
            // fwlocText
            // 
            this.fwlocText.Location = new System.Drawing.Point(24, 22);
            this.fwlocText.Name = "fwlocText";
            this.fwlocText.ReadOnly = true;
            this.fwlocText.Size = new System.Drawing.Size(368, 20);
            this.fwlocText.TabIndex = 21;
            this.fwlocText.Text = "FWTools installation location";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(408, 272);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 20);
            this.button1.TabIndex = 23;
            this.button1.Text = "Change";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // destFolderTextBox
            // 
            this.destFolderTextBox.Enabled = false;
            this.destFolderTextBox.Location = new System.Drawing.Point(24, 272);
            this.destFolderTextBox.Name = "destFolderTextBox";
            this.destFolderTextBox.ReadOnly = true;
            this.destFolderTextBox.Size = new System.Drawing.Size(368, 20);
            this.destFolderTextBox.TabIndex = 24;
            this.destFolderTextBox.Text = "Cache Target";
            // 
            // extentLabel
            // 
            this.extentLabel.AutoSize = true;
            this.extentLabel.Location = new System.Drawing.Point(405, 183);
            this.extentLabel.Name = "extentLabel";
            this.extentLabel.Size = new System.Drawing.Size(42, 13);
            this.extentLabel.TabIndex = 25;
            this.extentLabel.Text = "Extents";
            // 
            // northlab
            // 
            this.northlab.AutoSize = true;
            this.northlab.Location = new System.Drawing.Point(407, 140);
            this.northlab.Name = "northlab";
            this.northlab.Size = new System.Drawing.Size(33, 13);
            this.northlab.TabIndex = 26;
            this.northlab.Text = "North";
            // 
            // southlab
            // 
            this.southlab.AutoSize = true;
            this.southlab.Location = new System.Drawing.Point(407, 231);
            this.southlab.Name = "southlab";
            this.southlab.Size = new System.Drawing.Size(35, 13);
            this.southlab.TabIndex = 27;
            this.southlab.Text = "South";
            // 
            // westlab
            // 
            this.westlab.AutoSize = true;
            this.westlab.Location = new System.Drawing.Point(346, 183);
            this.westlab.Name = "westlab";
            this.westlab.Size = new System.Drawing.Size(32, 13);
            this.westlab.TabIndex = 28;
            this.westlab.Text = "West";
            // 
            // eastlab
            // 
            this.eastlab.AutoSize = true;
            this.eastlab.Location = new System.Drawing.Point(471, 183);
            this.eastlab.Name = "eastlab";
            this.eastlab.Size = new System.Drawing.Size(28, 13);
            this.eastlab.TabIndex = 29;
            this.eastlab.Text = "East";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.currentStatusLabel,
            this.progressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 377);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(512, 22);
            this.statusStrip1.TabIndex = 32;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabel1.Text = "Status:";
            // 
            // currentStatusLabel
            // 
            this.currentStatusLabel.Name = "currentStatusLabel";
            this.currentStatusLabel.Size = new System.Drawing.Size(55, 17);
            this.currentStatusLabel.Text = "Waiting...";
            // 
            // progressBar1
            // 
            this.progressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // formatListBox
            // 
            this.formatListBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formatListBox.FormattingEnabled = true;
            this.formatListBox.ItemHeight = 15;
            this.formatListBox.Location = new System.Drawing.Point(250, 183);
            this.formatListBox.Name = "formatListBox";
            this.formatListBox.Size = new System.Drawing.Size(63, 19);
            this.formatListBox.TabIndex = 33;
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(202, 185);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(42, 13);
            this.formatLabel.TabIndex = 34;
            this.formatLabel.Text = "Format:";
            // 
            // DstileFrontEnd
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(512, 399);
            this.Controls.Add(this.formatLabel);
            this.Controls.Add(this.formatListBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.eastlab);
            this.Controls.Add(this.westlab);
            this.Controls.Add(this.southlab);
            this.Controls.Add(this.northlab);
            this.Controls.Add(this.extentLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.destFolderTextBox);
            this.Controls.Add(this.choosefw);
            this.Controls.Add(this.fwlocText);
            this.Controls.Add(this.srcProjTextBox);
            this.Controls.Add(this.srcProjLabel);
            this.Controls.Add(this.tileListBox);
            this.Controls.Add(this.createXMLButton);
            this.Controls.Add(this.targetXMLTextBox);
            this.Controls.Add(this.tileSizeLabel);
            this.Controls.Add(this.layerNameTextBox);
            this.Controls.Add(this.demRadioButton);
            this.Controls.Add(this.imageRadioButton);
            this.Controls.Add(this.processButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.inputFileTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DstileFrontEnd";
            this.Text = "Dstile Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// Opens a file selector dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadButton_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                String fileName = openFileDialog1.FileName;
                locateData(fileName);
            }
        }
        
        public void locateData(string fileName)
        {
            inputFileTextBox.Text = fileName;
            if (File.Exists(fileName))
                currentStatusLabel.Text = "Data located";
            string strfileName = Path.GetFullPath(fileName);
            fileName = "\"" + fileName + "\"";
            Console.WriteLine("** File input: " + fileName);
            /*
            // only while testing - need to know/read in relative location
            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(fwlocText.Text,"bin//gdalinfo.exe"));
            if (!psi.EnvironmentVariables.ContainsKey("GDAL_DATA"))
                psi.EnvironmentVariables.Add("GDAL_DATA", Path.Combine(fwlocText.Text, "data"));
            if (!psi.EnvironmentVariables.ContainsKey("PROJ_LIB"))
                psi.EnvironmentVariables.Add("PROJ_LIB", Path.Combine(fwlocText.Text, "proj_lib"));
            if (!psi.EnvironmentVariables.ContainsKey("GEOTIFF_CSV"))
                psi.EnvironmentVariables.Add("GEOTIFF_CSV", Path.Combine(fwlocText.Text, "data"));
            if (!psi.EnvironmentVariables.ContainsKey("GDAL_DRIVER_PATH"))
                psi.EnvironmentVariables.Add("GDAL_DRIVER_PATH", Path.Combine(fwlocText.Text, "gdal_plugins"));
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Arguments = fileName;
            psi.RedirectStandardOutput = true;
            process.StartInfo = psi;
            process.EnableRaisingEvents = true;
            process.Start();

            StreamReader streamReader = process.StandardOutput;
            String output = null;
            Regex rxul = new Regex(@"\bUpper Left\b");
            Regex rxlr = new Regex(@"\bLower Right\b");
            //Look for projection SRS Authority
            Regex srcProjRx = new Regex(@"AUTHORITY\[");
            //Assume source projection is needed
            this.srcProjNeeded = true;
            while ((output = streamReader.ReadLine()) != null)
            {
                Console.WriteLine("** Gdal Output: " + output);
                if (srcProjRx.IsMatch(output))
                {
                    String projAuth = output.Substring(output.IndexOf("[") + 1, output.IndexOf("]") - output.IndexOf("[") - 1);
                    srcProjTextBox.Text = projAuth;
                    srcProjNeeded = false;
                }
                if (rxul.IsMatch(output))
                {
                    upperLeft = getCoordinates(output);
                    northlab.Text = Convert.ToString(Math.Round(upperLeft[0], 2));
                    westlab.Text = Convert.ToString(Math.Round(upperLeft[1], 2));
                }
                else if (rxlr.IsMatch(output))
                {
                    lowerRight = getCoordinates(output);
                    southlab.Text = Convert.ToString(Math.Round(lowerRight[0], 2));
                    eastlab.Text = Convert.ToString(Math.Round(lowerRight[1], 2));
                }
                // Do more stuff here. Like... I dunno...
            }
            process.WaitForExit();
            process.Close();
            process.Dispose();
            */
            if (System.Environment.GetEnvironmentVariable("GDAL_DATA") == null)
                System.Environment.SetEnvironmentVariable("GDAL_DATA", Path.Combine(fwlocText.Text, "data\\"));
            gdal.AllRegister();

            Console.WriteLine("GDAL Drivers:"+gdal.GetDriverCount());
            GDAL.Dataset dataset = gdal.Open(strfileName, 0);

            // WW's srs
            //SpatialReference llsrs = new SpatialReference("");
            //llsrs.ImportFromEPSG(4326);
            string epsgwkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.2572235629972,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";
            SpatialReference llsrs = new SpatialReference(epsgwkt);

            // input srs
            SpatialReference srs;

            try
            {
                // get srs of datset
                string proj = dataset.GetProjection();
                if (proj != "")
                {
                    srcProjNeeded = false;
                    srs = new SpatialReference(proj);
                    string proj4def;
                    srs.ExportToProj4(out proj4def);
                    srcProjTextBox.Text = proj4def;
                }
                else
                {
                    UserSrsInputForm srcProjForm = new UserSrsInputForm();
                    //TODO: Populate the combo box in the form with a list of projections...

                    if (srcProjForm.ShowDialog() == DialogResult.OK)
                    {
                        //TODO: allow for proj.4 strings as well
                        if(!srcProjForm.Projection.Contains("EPSG"))
                            throw new Exception("Invalid projection string from user");

                        string[] srcProjString = srcProjForm.Projection.Split(':');
                        int epsg = int.Parse(srcProjString[1]);
                        srs = new SpatialReference("");
                        srs.ImportFromEPSG(epsg);

                        string proj4def;
                        srs.ExportToProj4(out proj4def);
                        srcProjTextBox.Text = proj4def;
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error getting input projection");
                this.Close();
                return;
            }

            try
            {
                CoordinateTransformation coordtrans = new CoordinateTransformation(srs, llsrs);
                double[] transform = new double[6];
                dataset.GetGeoTransform(transform);
                int xsize = dataset.RasterXSize;
                int ysize = dataset.RasterYSize;
                double topleftx = transform[0];
                double toplefty = transform[3];
                double bottomrightx = transform[0] + xsize * transform[1];
                double bottomrighty = transform[3] + ysize * transform[5];

                double[] topleft = { topleftx, toplefty };
                double[] bottomright = { bottomrightx, bottomrighty };

                coordtrans.TransformPoint(topleft);
                coordtrans.TransformPoint(bottomright);

                northlab.Text = Convert.ToString(Math.Round(topleft[1], 2));
                westlab.Text = Convert.ToString(Math.Round(topleft[0], 2));
                southlab.Text = Convert.ToString(Math.Round(bottomright[1], 2));
                eastlab.Text = Convert.ToString(Math.Round(bottomright[0], 2));

                //TODO: Make this an educated guess based on image extent
                //and resolution
                double width = bottomright[0] - topleft[0];
                for (int i = 0; i < lztsdvalues.Length; i++)
                    if (width > Convert.ToDouble(lztsdvalues[i].ToString()))
                    {
                        tileListBox.SelectedIndex = i;
                        break;
                    }
                if (tileListBox.SelectedItem == null)
                    tileListBox.SelectedIndex = lztsdvalues.Length - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in image bound coordinate transformation");
                this.Close();
                return;
            }

            //Detect data type and decide whether to tile as Image or DEM
            int dtype = dataset.GetRasterBand(1).DataType;
            if( dtype == gdalconst.GDT_Byte)
            {
                Console.WriteLine("Imagery Tiling Enabled");
                imageRadioButton.Checked = true;
                formatListBox.SelectedIndex = 0;
            }
            else if( dtype == gdalconst.GDT_Float32 || dtype == gdalconst.GDT_Int16)
            {
                Console.WriteLine("DEM Tiling Enabled");
                demRadioButton.Checked = true;
            }
            else
            {
                Console.WriteLine("Unsupported Data Type! Quitting");
                imageRadioButton.Checked = false;
                demRadioButton.Checked = false;
            }

            dataset.Dispose();
            /*
            //TODO: Use proj.4 to reproject non-lat-lon coordinates to epsg:4326
            if (srcProjTextBox.Text.ToLower() != "epsg:4326")
            {
                MessageBox.Show("Reprojection of the extents is needed");
            }
            */
        }

        /// <summary>
        /// Starts the processing of the selected file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processButton_Click(object sender, System.EventArgs e)
        {
            /*
            if (processButton.Text == "Process")
            {
                this.progressBar1.Value = 100;
                String fileName = inputFileTextBox.Text;
                String layerName = layerNameTextBox.Text;
                string lztsdString = tileListBox.SelectedItem.ToString();
                if (lztsdString == null)
                    //no lztsd selected making a guess
                    tileListBox.SelectedIndex = tileListBox.TopIndex;
                decimal lztsd = Convert.ToDecimal(tileListBox.SelectedItem.ToString());
                // check if source projection is required, and if so, one has been entered
                Boolean cont = true;
                if (this.srcProjNeeded && srcProjTextBox.Text == "")
                {
                    cont = false;
                }
                if (cont)
                {
                    if (File.Exists(fileName))
                    {
                        //set dem switch
                        String demSwitch = "";
                        if (demRadioButton.Checked)
                        {
                            demSwitch = " --dem";
                        }
                        // Check if the dstile needs the source projection
                        String srcProj = null;
                        if (this.srcProjNeeded)
                            srcProj = " --srcProj " + srcProjTextBox.Text;
                        else
                            srcProj = "";
                        System.IO.Directory.CreateDirectory(@"" + destFolderTextBox.Text);
                        arguments = String.Format("tile --lztsd {3} --wwcache" +
                            srcProj + " --overviews{2} \"{1}\" \"{0}\"",
                                fileName,
                                destFolderTextBox.Text,
                                demSwitch,
                                lztsd);
                        currentStatusLabel.Text = "Processing (this may take a while)";
                        //currentStatusLabel.Update();
                        //Console.WriteLine(arguments);
                        statusStrip1.Refresh();
                        Thread.Sleep(1000);
                        processButton.Text = "Stop";
                        // only while testing - need to know/read in relative location

                        ThreadStart threadStart = new ThreadStart(dstileProcess);
                        Thread dstileThread = new Thread(threadStart);
                        dstileThread.Name = "Dstile thread";
                        dstileThread.IsBackground = true;
                        dstileThread.Start();
                    }
                }
                else
                {
                    currentStatusLabel.Text = "Source projection required";
                }
            }
            else
            {
                if (dstileThread != null)
                    dstileThread.Abort();
                this.processButton.Text = "Process";
                this.progressBar1.Value = 0;
                this.currentStatusLabel.Text = "Stopped";
            }
            */
            //Create and Load a GDALDataStore
            if (imageRadioButton.Checked)
                loadGDALVrt();
            else if (demRadioButton.Checked)
                loadGDALVrtDem();
        }

        private void chooseDestButton_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                String dirName = folderDialog.SelectedPath;
                destFolderTextBox.Text = dirName;
                if (wwIsValidated(dirName))
                    currentStatusLabel.Text = "NEXTIMAGE Installation located";
                else
                    currentStatusLabel.Text = "NEXTIMAGE Installation not found";

            }
        }

        private bool wwIsValidated(String dirname)
        {
            //validate existense of Worldwind.exe
            String Worldwindpath = Path.Combine(dirname, "WorldWind.exe");
            if (File.Exists(Worldwindpath))
                return true;
            return false;
        }

        private void destFolderTextBox_TextChanged(object sender, System.EventArgs e)
        {
            String dirname = destFolderTextBox.Text;
            if (wwIsValidated(dirname))
                updateXMLPath();
        }

        private void inputFileTextBox_TextChanged(object sender, System.EventArgs e)
        {
            String fname = inputFileTextBox.Text;
            if (File.Exists(fname))
                currentStatusLabel.Text = "Data located";
        }

        private void demRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            updateXMLPath();
        }

        private void imageRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            updateXMLPath();
        }

        private void updateXMLPath()
        {
            if (!imageRadioButton.Checked)
            {
                targetXMLTextBox.Text = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "Config\\Earth.xml");
                destFolderTextBox.Text = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "Cache\\Earth\\SRTM\\" + layerNameTextBox.Text);
            }
            else
            {
                targetXMLTextBox.Text = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "Config\\Earth\\" + layerNameTextBox.Text + ".xml");
                destFolderTextBox.Text = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "Data\\Earth\\" + layerNameTextBox.Text);
            }
        }

        private void createXMLButton_Click(object sender, EventArgs e)
        {
            /*
            if (!File.Exists(targetXMLTextBox.Text))
            {
                //otherwise copy over skeleton.xml
                if (imageRadioButton.Checked)
                {
                    try
                    {
                        //Try to put new location tags and names in XML
                        XmlDocument doc = new XmlDocument();
                        doc.Load(Path.Combine(srcplugin.PluginDirectory, "skeleton.xml"));
                        XmlNodeList nlist = doc.GetElementsByTagName("Name");
                        foreach (XmlNode layernode in nlist)
                            layernode.InnerText = layerNameTextBox.Text;
                        nlist = doc.GetElementsByTagName("PermanentDirectory");
                        foreach (XmlNode permdir in nlist)
                            permdir.InnerText = destFolderTextBox.Text;
                        // Get the limits of the image, insert into XML
                        nlist = doc.GetElementsByTagName("North");
                        foreach (XmlNode northLine in nlist)
                            northLine.FirstChild.InnerText = Convert.ToString(northlab.Text);
                        nlist = doc.GetElementsByTagName("South");
                        foreach (XmlNode southLine in nlist)
                            southLine.FirstChild.InnerText = Convert.ToString(southlab.Text);
                        nlist = doc.GetElementsByTagName("West");
                        foreach (XmlNode westLine in nlist)
                            westLine.FirstChild.InnerText = Convert.ToString(westlab.Text);
                        nlist = doc.GetElementsByTagName("East");
                        foreach (XmlNode eastLine in nlist)
                            eastLine.FirstChild.InnerText = Convert.ToString(eastlab.Text);
                        nlist = doc.GetElementsByTagName("LevelZeroTileSizeDegrees");
                        foreach (XmlNode lztsd in nlist)
                        {
                            tileListBox.SelectedIndex = tileListBox.TopIndex;
                            lztsd.InnerText = tileListBox.SelectedItem.ToString();
                        }
                        // Save document
                        doc.Save(targetXMLTextBox.Text);
                    }
                    catch
                    {
                        currentStatusLabel.Text = "Skeleton file copy failed";
                    }
                }
                else
                {
                    //Load up Earth.xml and try to insert Higher Resolution subset
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(layerNameTextBox.Text);

                        doc.Save(layerNameTextBox.Text);
                    }
                    catch
                    {
                        currentStatusLabel.Text = "Earth.xml edit failed";
                    }
                }
            }
            
            // if XML exists open it in notepad
            ThreadStart threadStart = new ThreadStart(openNotepad);
            Thread notepadThread = new Thread(threadStart);
            notepadThread.Name = "Notepad thread";
            notepadThread.IsBackground = true;
            notepadThread.Start();
            */

            //TODO:Create relevant XML in Dstile Plugin Directory
            //I suggest 1 master XML with all dstile loaded images
            //any newly created layers are appeneded to it
            //If a layer with the same name exists a GDALImageStore
            //is appended to it. This can be worked to mosaic the
            //images on the fly

            this.processButton.Enabled = true;
        }

        private void openNotepad()
        {
            ProcessStartInfo psi = new ProcessStartInfo("notepad.exe");
            psi.UseShellExecute = false;
            Process p = new Process();

            psi.Arguments = targetXMLTextBox.Text;
            p.EnableRaisingEvents = true;
            p = Process.Start(psi);
            p.WaitForExit();
            p.Close();
            p.Dispose();
        }

        private void layerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            updateXMLPath();
            this.targetXMLTextBox.Enabled = true;
            this.createXMLButton.Enabled = true;
        }

        /// <summary>
        /// Method to split up a line from the gdalinfo output
        /// </summary>
        /// <param name="input">gdalinfo output</param>
        /// <returns>double array of Coordinates</returns>
        private Double[] getCoordinates(String input)
        {
            String[] splitOutput = input.Split(new char[] { '(', ',', ')' });
            for (int i = 0; i < splitOutput.GetLength(0); i++)
            {
                splitOutput[i] = splitOutput[i].Trim();
            }
            Double[] output = { Convert.ToDouble(splitOutput[1]), Convert.ToDouble(splitOutput[2]) };
            return output;
        }

        //variable to store tiling progress
        //private int tilingProgres = 0;

        private void dstileProcess()
        {
            this.timer1.Interval = 1;
            this.timer1.Enabled = true;
            this.timer1.Start();
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(fwlocText.Text,"bin//dstile.exe"));
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process process = new Process();
            psi.Arguments = arguments;
            psi.RedirectStandardOutput = true;
            if (!psi.EnvironmentVariables.ContainsKey("GDAL_DATA"))
                psi.EnvironmentVariables.Add("GDAL_DATA", Path.Combine(fwlocText.Text, "data"));
            if (!psi.EnvironmentVariables.ContainsKey("PROJ_LIB"))
                psi.EnvironmentVariables.Add("PROJ_LIB", Path.Combine(fwlocText.Text, "proj_lib"));
            if (!psi.EnvironmentVariables.ContainsKey("GEOTIFF_CSV"))
                psi.EnvironmentVariables.Add("GEOTIFF_CSV", Path.Combine(fwlocText.Text, "data"));
            if (!psi.EnvironmentVariables.ContainsKey("GDAL_DRIVER_PATH"))
                psi.EnvironmentVariables.Add("GDAL_DRIVER_PATH", Path.Combine(fwlocText.Text, "gdal_plugins"));
            process.EnableRaisingEvents = true;
            process = Process.Start(psi);

            StreamReader streamReader = process.StandardOutput;
            //String output = null;
            
            //TODO: Add progress bar updating here
            while (streamReader.Peek() >= 0)
            {
                Console.Write((char)streamReader.Read());
            }

            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                Console.WriteLine("Finished successfully");
                Console.WriteLine("Loading Layer");
                //WorldWind.ConfigurationLoader.getRenderableFromLayerFile(this.targetXMLTextBox.Text,srcplugin.Global.worldWindow.CurrentWorld);
                RenderableObjectList layers = ConfigurationLoader.
                    getRenderableFromLayerFile(this.targetXMLTextBox.Text,
                    Global.worldWindow.CurrentWorld,
                    Global.worldWindow.Cache);
                Global.worldWindow.CurrentWorld.RenderableObjects.Add(layers);
                //this.processButton.Text = "Process";
            }
            //currentStatusLabel.Text = "Finished successfully";
            else
                Console.WriteLine("Tiling failed");
            //currentStatusLabel.Text = "Tiling failed";
            process.Dispose();
            Console.WriteLine();

            
        }

        /// <summary>
        /// This method can ultimately be used for
        /// loading a file for on the fly tiling
        /// </summary>
        private void loadGDALVrt()
        {
            //TODO: Use autocreate warped VRT to tile on the fly
            gdal.AllRegister();
            GDAL.Dataset src_ds = gdal.Open(inputFileTextBox.Text,gdalconst.GA_ReadOnly);

            if (System.Environment.GetEnvironmentVariable("GDAL_DATA")==null)
                System.Environment.SetEnvironmentVariable("GDAL_DATA", Path.Combine(fwlocText.Text, "data\\"));

            SpatialReference src_srs;

            if (srcProjNeeded)
                src_srs = new SpatialReference(srcProjTextBox.Text);
            else
                src_srs = new SpatialReference(src_ds.GetProjection());

            string epsgwkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.2572235629972,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";
            SpatialReference dst_srs = new SpatialReference(epsgwkt);
            GDAL.Dataset dst_ds = gdal.AutoCreateWarpedVRT(src_ds, src_ds.GetProjection(),
                epsgwkt, gdalconst.GRA_Cubic,0.0);

            double[] dst_transform = new double[6];
            //TODO: There is bug in the AutoCreateWarpedVRT function which does not update
            //the transform in the VRT to latlon units, osr needs to be used to correct
            //the transform.This is a hack and GDAL core needs to be fixed
            CoordinateTransformation coordtrans = new CoordinateTransformation(src_srs, dst_srs);

            dst_ds.GetGeoTransform(dst_transform);
            int xsize = dst_ds.RasterXSize;
            int ysize = dst_ds.RasterYSize;
            double topleftx = dst_transform[0];
            double toplefty = dst_transform[3];
            double bottomrightx = dst_transform[0] + xsize * dst_transform[1];
            double bottomrighty = dst_transform[3] + ysize * dst_transform[5];

            double[] topleft = { topleftx, toplefty };
            double[] bottomright = { bottomrightx, bottomrighty };

            coordtrans.TransformPoint(topleft);
            coordtrans.TransformPoint(bottomright);

            //Repopulate dst_transform
            dst_transform[0] = topleft[0];
            dst_transform[1] = (bottomright[0] - topleft[0]) / xsize;
            dst_transform[3] = topleft[1];
            dst_transform[5] = (bottomright[1] - topleft[1]) / ysize;

            dst_ds.SetGeoTransform(dst_transform);

            //Set up QuadTileSet with GDALImageStore
            //HACK:What is x-y resolutions are not equal ?
            double baseres = dst_transform[1]*256;
            double lztsd = Convert.ToDouble(tileListBox.SelectedItem);
            string format = Convert.ToString(formatListBox.SelectedItem);
            int tilelevels = (int)Math.Ceiling(Math.Log(lztsd / baseres) / Math.Log(2.0));
            World curworld = Global.worldWindow.CurrentWorld;
            GDALImageStore gdalstore = 
                new GDALImageStore(layerNameTextBox.Text,inputFileTextBox.Text,
                dst_ds,format);
            gdalstore.LevelCount = tilelevels;
            gdalstore.LevelZeroTileSizeDegrees = lztsd;
            gdalstore.DataDirectory = Path.Combine(srcplugin.PluginDirectory,
                String.Format("Data\\{0}\\{1}\\",
                curworld.Name, layerNameTextBox.Text));
            //Create DataDirectory if it does not exist and clear it
            //Warn if deleting contents
            if (Directory.Exists(gdalstore.DataDirectory))
                Directory.Delete(gdalstore.DataDirectory,true);
            Directory.CreateDirectory(gdalstore.DataDirectory);
            ImageStore[] stores = { gdalstore };
            QuadTileSet qts = new QuadTileSet(layerNameTextBox.Text,
                curworld,0,
                Convert.ToDouble(northlab.Text),
                Convert.ToDouble(southlab.Text),
                Convert.ToDouble(westlab.Text),
                Convert.ToDouble(eastlab.Text),true,
                stores);
            qts.ColorKey = 0;
            qts.ColorKeyMax = 10;
            qts.RenderStruts = false;

            //TODO: Reuse Plugin Based Dstile layer
            //TODO: Modify to allow adding with user selected base layer name
            RenderableObjectList dstilelayer = this.srcplugin.DstileLayers;
            //TODO: Allow option to stop render loop and complete tiling before adding
            dstilelayer.Add(qts);
            //curworld.RenderableObjects.Add(dstilelayer);
        }

        /// <summary>
        /// This method can ultimately be used for
        /// loading a file for on the fly tiling
        /// </summary>
        private void loadGDALVrtDem()
        {
            //TODO: Use autocreate warped VRT to tile on the fly
            gdal.AllRegister();
            GDAL.Dataset src_ds = gdal.Open(inputFileTextBox.Text, gdalconst.GA_ReadOnly);

            if (System.Environment.GetEnvironmentVariable("GDAL_DATA") == null)
                System.Environment.SetEnvironmentVariable("GDAL_DATA", Path.Combine(fwlocText.Text, "data\\"));

            SpatialReference src_srs;

            if (srcProjNeeded)
                src_srs = new SpatialReference(srcProjTextBox.Text);
            else
                src_srs = new SpatialReference(src_ds.GetProjection());

            string epsgwkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.2572235629972,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";
            SpatialReference dst_srs = new SpatialReference(epsgwkt);
            GDAL.Dataset dst_ds = gdal.AutoCreateWarpedVRT(src_ds, src_ds.GetProjection(),
                epsgwkt, gdalconst.GRA_Cubic, 0.0);

            double[] dst_transform = new double[6];
            //TODO: There is bug in the AutoCreateWarpedVRT function which does not update
            //the transform in the VRT to latlon units, osr needs to be used to correct
            //the transform.This is a hack and GDAL core needs to be fixed
            CoordinateTransformation coordtrans = new CoordinateTransformation(src_srs, dst_srs);

            dst_ds.GetGeoTransform(dst_transform);
            int xsize = dst_ds.RasterXSize;
            int ysize = dst_ds.RasterYSize;
            double topleftx = dst_transform[0];
            double toplefty = dst_transform[3];
            double bottomrightx = dst_transform[0] + xsize * dst_transform[1];
            double bottomrighty = dst_transform[3] + ysize * dst_transform[5];

            double[] topleft = { topleftx, toplefty };
            double[] bottomright = { bottomrightx, bottomrighty };

            coordtrans.TransformPoint(topleft);
            coordtrans.TransformPoint(bottomright);

            //Repopulate dst_transform
            dst_transform[0] = topleft[0];
            dst_transform[1] = (bottomright[0] - topleft[0]) / xsize;
            dst_transform[3] = topleft[1];
            dst_transform[5] = (bottomright[1] - topleft[1]) / ysize;

            dst_ds.SetGeoTransform(dst_transform);

            //Set up QuadTileSet with GDALImageStore
            //HACK:What is x-y resolutions are not equal ?
            double baseres = dst_transform[1] * 256;
            double lztsd = Convert.ToDouble(tileListBox.SelectedItem);
            int tilelevels = (int)Math.Ceiling(Math.Log(lztsd / baseres) / Math.Log(2.0));
            
            World curworld = Global.worldWindow.CurrentWorld;
            string datadir = Path.Combine(srcplugin.PluginDirectory,
                String.Format("Data\\{0}\\DEM\\{1}\\",
                curworld.Name, layerNameTextBox.Text));

            GDALTerrainAccessor gdalterrain = new GDALTerrainAccessor(Convert.ToDouble(northlab.Text),
                Convert.ToDouble(southlab.Text),
                Convert.ToDouble(westlab.Text),
                Convert.ToDouble(eastlab.Text), dst_ds, layerNameTextBox.Text, lztsd, tilelevels, datadir);

            //Create DataDirectory if it does not exist and clear it
            //Warn if deleting contents
            if (Directory.Exists(datadir))
                Directory.Delete(datadir, true);
            Directory.CreateDirectory(datadir);

            //TODO:Add option to complete terrain tiling before adding
            curworld.TerrainAccessor.AddHigherResolutionSubset(gdalterrain);
        }

        private void fwloc_Click(object sender, EventArgs e)
        {
            getFWLoc();
        }
        
        private void getFWLoc()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select FWTools Location";
            if (folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                String dirName = folderDialog.SelectedPath;
                if (File.Exists(Path.Combine(dirName, "bin/dstile.exe")) && File.Exists(Path.Combine(dirName, "bin/gdalinfo.exe")))
                {
                    fwlocText.Text = dirName;
                    LoadSaveXmlConfig(false);
                }
                else
                    MessageBox.Show("FWTools not located");
            }
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            LoadSaveXmlConfig(false);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.progressBar1.Value = 0;
            if (this.progressBar1.Value >= 0 && this.progressBar1.Value < 100)
                this.progressBar1.Value++;
            else
            {
                this.timer1.Enabled = false;
                this.progressBar1.Value = 0;
                this.processButton.Text = "Process";
            }
        }
    }
}