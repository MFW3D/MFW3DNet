using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using System;
using WorldWind.Net;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.VisualControl;
using Utility;
using CarbonTools.Core.Base;
using CarbonTools.Core.OGCCapabilities;

namespace WorldWind
{
    /// <summary>
    /// WMS Browser dialog.
    /// </summary>
    public class WMSBrowserNG : System.Windows.Forms.Form
    {
        string wms_skeleton_path = Path.Combine(
            Path.Combine(Global.Settings.ConfigPath, "Earth"),
            Path.Combine("Tools", "wmsskeleton.xml"));

        private WorldWindow worldWindow;

        private System.Drawing.Point mouseLocationProgressBarAnimation = new Point(0, 0);

        private System.Timers.Timer animationTimer = new System.Timers.Timer();
        private System.Collections.ArrayList animationFrames = new ArrayList();
        private System.Collections.Queue downloadQueue = new Queue();
        private System.Windows.Forms.ContextMenu contextMenuLegendUrl;
        private ComboBox wmsGetCapstextbox;
        private Label label4;
        private Button wmsbutton;
        private ComboBox comboBox2;
        private Label label5;
        private GroupBox xmlsaveGroupBox;
        private Label label6;
        private TextBox textBox3;
        private CheckBox checkBox1;
        private Label label7;
        private Label label3;
        private TextBox textBox1;
        private Button savewmsbutton;
        private Label label2;
        private GroupBox groupBox1;
        //private CarbonTools.Controls.PictureBoxOGC pictureBoxOGC1;
        private Panel panelLower;
        private Panel panelContents;
        private Label ConfLabel;
		private System.Windows.Forms.ProgressBar pictureBoxProgressBar;
		private System.Windows.Forms.ProgressBar treeViewProgressBar;
		private Label label9;
		private Label label8;
        private ToolTip toolTip1;
        private IContainer components;
        private Label PasswordLabel;
        private TextBox PasswordTextBox;
        private Label UserNameLabel;
        private TextBox UsernameTextBox;
        private CarbonTools.Controls.TreeViewOGCCapabilities treeOgcCaps;


        public WMSBrowserNG(WorldWindow ww)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.worldWindow = ww;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuLegendUrl = new System.Windows.Forms.ContextMenu();
            this.treeOgcCaps = new CarbonTools.Controls.TreeViewOGCCapabilities();
            this.wmsGetCapstextbox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.wmsbutton = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.xmlsaveGroupBox = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.savewmsbutton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxProgressBar = new System.Windows.Forms.ProgressBar();
            this.panelLower = new System.Windows.Forms.Panel();
            this.panelContents = new System.Windows.Forms.Panel();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.UserNameLabel = new System.Windows.Forms.Label();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.treeViewProgressBar = new System.Windows.Forms.ProgressBar();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ConfLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.xmlsaveGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelLower.SuspendLayout();
            this.panelContents.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeOgcCaps
            // 
            this.treeOgcCaps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(151)))), ((int)(((byte)(151)))));
            this.treeOgcCaps.Credentials = null;
            this.treeOgcCaps.Location = new System.Drawing.Point(2, 179);
            this.treeOgcCaps.Name = "treeOgcCaps";
            this.treeOgcCaps.Proxy = null;
            this.treeOgcCaps.Size = new System.Drawing.Size(509, 181);
            this.treeOgcCaps.TabIndex = 5;
            this.toolTip1.SetToolTip(this.treeOgcCaps, "All data available from the requested WMS server.  Highlight an item to show a pr" +
                    "eview image.");
            this.treeOgcCaps.URL = "";
            this.treeOgcCaps.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeOgcCaps_AfterSelect);
            this.treeOgcCaps.OperationDone += new System.EventHandler(this.treeOgcCaps_OperationDone);
            // 
            // wmsGetCapstextbox
            // 
            this.wmsGetCapstextbox.DropDownWidth = 480;
            this.wmsGetCapstextbox.Items.AddRange(new object[] {
            "http://www.ga.gov.au/bin/getmap.pl?request=capabilities",
            "http://www.geosignal.org/cgi-bin/wmsmap?",
            "http://wms.globexplorer.com/gexservlets/wms?"});
            this.wmsGetCapstextbox.Location = new System.Drawing.Point(10, 137);
            this.wmsGetCapstextbox.Name = "wmsGetCapstextbox";
            this.wmsGetCapstextbox.Size = new System.Drawing.Size(399, 21);
            this.wmsGetCapstextbox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.wmsGetCapstextbox, "Enter a WMS server address or select one from the list.  Note World Wnd only supp" +
                    "orts WGS:84 data.");
            this.wmsGetCapstextbox.SelectedIndexChanged += new System.EventHandler(this.wmsGetCapstextbox_SelectedIndexChanged);
            this.wmsGetCapstextbox.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "WMS url:";
            // 
            // wmsbutton
            // 
            this.wmsbutton.Location = new System.Drawing.Point(415, 137);
            this.wmsbutton.Name = "wmsbutton";
            this.wmsbutton.Size = new System.Drawing.Size(91, 21);
            this.wmsbutton.TabIndex = 1;
            this.wmsbutton.Text = "Get WMS Tree";
            this.toolTip1.SetToolTip(this.wmsbutton, "Downloads the list of available imagery from the WMS server");
            this.wmsbutton.UseVisualStyleBackColor = true;
            this.wmsbutton.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "png",
            "jpeg"});
            this.comboBox2.Location = new System.Drawing.Point(196, 10);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(71, 21);
            this.comboBox2.TabIndex = 6;
            this.comboBox2.Text = "jpeg";
            this.toolTip1.SetToolTip(this.comboBox2, "Select image format to request, jpeg is best for imagery and png for vectors.");
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(119, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Image Format";
            // 
            // xmlsaveGroupBox
            // 
            this.xmlsaveGroupBox.Controls.Add(this.label6);
            this.xmlsaveGroupBox.Controls.Add(this.textBox3);
            this.xmlsaveGroupBox.Controls.Add(this.checkBox1);
            this.xmlsaveGroupBox.Controls.Add(this.label7);
            this.xmlsaveGroupBox.Controls.Add(this.label3);
            this.xmlsaveGroupBox.Controls.Add(this.textBox1);
            this.xmlsaveGroupBox.Controls.Add(this.savewmsbutton);
            this.xmlsaveGroupBox.Controls.Add(this.label2);
            this.xmlsaveGroupBox.Enabled = false;
            this.xmlsaveGroupBox.Location = new System.Drawing.Point(21, 37);
            this.xmlsaveGroupBox.Name = "xmlsaveGroupBox";
            this.xmlsaveGroupBox.Size = new System.Drawing.Size(268, 156);
            this.xmlsaveGroupBox.TabIndex = 14;
            this.xmlsaveGroupBox.TabStop = false;
            this.xmlsaveGroupBox.Text = "Create layer";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(17, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Specify layer title:";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(151)))), ((int)(((byte)(151)))));
            this.textBox3.Enabled = false;
            this.textBox3.ForeColor = System.Drawing.Color.White;
            this.textBox3.Location = new System.Drawing.Point(112, 43);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(133, 20);
            this.textBox3.TabIndex = 12;
            this.textBox3.Text = "WMS Layer";
            this.toolTip1.SetToolTip(this.textBox3, "The name which will be shown in your layer manager for this data.");
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged_1);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(61, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(179, 17);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Use default layer title from server";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Warning: overwrites existing files";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "XML filename:";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(151)))), ((int)(((byte)(151)))));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(98, 81);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(133, 20);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "wms";
            this.toolTip1.SetToolTip(this.textBox1, "The filename for the data, this will be stored in your Config\\PlanetName director" +
                    "y.");
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // savewmsbutton
            // 
            this.savewmsbutton.Location = new System.Drawing.Point(82, 107);
            this.savewmsbutton.Name = "savewmsbutton";
            this.savewmsbutton.Size = new System.Drawing.Size(109, 27);
            this.savewmsbutton.TabIndex = 5;
            this.savewmsbutton.Text = "Save as XML";
            this.toolTip1.SetToolTip(this.savewmsbutton, "Saves the file as XML and adds it to the layer manager");
            this.savewmsbutton.UseVisualStyleBackColor = true;
            this.savewmsbutton.Click += new System.EventHandler(this.savewmsbutton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = ".xml";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBoxProgressBar);
            this.groupBox1.Location = new System.Drawing.Point(295, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 183);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // pictureBoxProgressBar
            // 
            this.pictureBoxProgressBar.Location = new System.Drawing.Point(3, 170);
            this.pictureBoxProgressBar.Name = "pictureBoxProgressBar";
            this.pictureBoxProgressBar.Size = new System.Drawing.Size(208, 17);
            this.pictureBoxProgressBar.TabIndex = 16;
            // 
            // panelLower
            // 
            this.panelLower.Controls.Add(this.groupBox1);
            this.panelLower.Controls.Add(this.xmlsaveGroupBox);
            this.panelLower.Controls.Add(this.label5);
            this.panelLower.Controls.Add(this.comboBox2);
            this.panelLower.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLower.Location = new System.Drawing.Point(0, 459);
            this.panelLower.Name = "panelLower";
            this.panelLower.Size = new System.Drawing.Size(702, 209);
            this.panelLower.TabIndex = 0;
            this.panelLower.Paint += new System.Windows.Forms.PaintEventHandler(this.panelLower_Paint);
            // 
            // panelContents
            // 
            this.panelContents.Controls.Add(this.PasswordLabel);
            this.panelContents.Controls.Add(this.PasswordTextBox);
            this.panelContents.Controls.Add(this.UserNameLabel);
            this.panelContents.Controls.Add(this.UsernameTextBox);
            this.panelContents.Controls.Add(this.treeViewProgressBar);
            this.panelContents.Controls.Add(this.treeOgcCaps);
            this.panelContents.Controls.Add(this.label9);
            this.panelContents.Controls.Add(this.label8);
            this.panelContents.Controls.Add(this.wmsbutton);
            this.panelContents.Controls.Add(this.ConfLabel);
            this.panelContents.Controls.Add(this.label4);
            this.panelContents.Controls.Add(this.wmsGetCapstextbox);
            this.panelContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContents.Location = new System.Drawing.Point(0, 0);
            this.panelContents.Name = "panelContents";
            this.panelContents.Size = new System.Drawing.Size(702, 459);
            this.panelContents.TabIndex = 4;
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(100, 76);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(53, 13);
            this.PasswordLabel.TabIndex = 24;
            this.PasswordLabel.Text = "Password";
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(102, 94);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(84, 20);
            this.PasswordTextBox.TabIndex = 23;
            // 
            // UserNameLabel
            // 
            this.UserNameLabel.AutoSize = true;
            this.UserNameLabel.Location = new System.Drawing.Point(7, 76);
            this.UserNameLabel.Name = "UserNameLabel";
            this.UserNameLabel.Size = new System.Drawing.Size(55, 13);
            this.UserNameLabel.TabIndex = 22;
            this.UserNameLabel.Text = "Username";
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(10, 94);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(83, 20);
            this.UsernameTextBox.TabIndex = 21;
            // 
            // treeViewProgressBar
            // 
            this.treeViewProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(151)))), ((int)(((byte)(151)))));
            this.treeViewProgressBar.Location = new System.Drawing.Point(298, 163);
            this.treeViewProgressBar.Name = "treeViewProgressBar";
            this.treeViewProgressBar.Size = new System.Drawing.Size(208, 10);
            this.treeViewProgressBar.TabIndex = 17;
            this.toolTip1.SetToolTip(this.treeViewProgressBar, "Loading .... Please Wait.");
            this.treeViewProgressBar.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(394, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "?Fill in options and click \"Save as XML\" to add the all of the layers to World W" +
                "ind";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(473, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "?Enter a WMS url in the url bar below and click \"Get WMS Tree\" to see the conten" +
                "ts of the server";
            // 
            // ConfLabel
            // 
            this.ConfLabel.AutoSize = true;
            this.ConfLabel.Location = new System.Drawing.Point(2, 163);
            this.ConfLabel.Name = "ConfLabel";
            this.ConfLabel.Size = new System.Drawing.Size(84, 13);
            this.ConfLabel.TabIndex = 4;
            this.ConfLabel.Text = "Available Layers";
            // 
            // toolTip1
            // 
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // WMSBrowserNG
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.ClientSize = new System.Drawing.Size(702, 668);
            this.Controls.Add(this.panelContents);
            this.Controls.Add(this.panelLower);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(504, 483);
            this.Name = "WMSBrowserNG";
            this.Text = "WMS Importer";
            this.toolTip1.SetToolTip(this, "Import WMS imagery to a World Wind layer.");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.xmlsaveGroupBox.ResumeLayout(false);
            this.xmlsaveGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panelLower.ResumeLayout(false);
            this.panelLower.PerformLayout();
            this.panelContents.ResumeLayout(false);
            this.panelContents.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //TODO:Use regex to check getcapabilties URL
            //if (!wmsGetCapstextbox.Text.StartsWith("http://") && !wmsGetCapstextbox.Contains("(G|g)et(C|c)apabilities"))
                //Console.WriteLine("WMS GETCAPS URL VALIDATED");
                //MessageBox.Show("Invalid GetCaps URL");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeOgcCaps.Nodes.Clear();
            treeOgcCaps.URL = wmsGetCapstextbox.Text;
            //if (treeOgcCaps.URL.Contains("seyeqweb01dul"))
            //    treeOgcCaps.Credentials = new NetworkCredential("admin", "admin");

            if (UsernameTextBox.Text != "")
            {
                NetworkCredential creds = new NetworkCredential(UsernameTextBox.Text, PasswordTextBox.Text);
                //NetworkCredential basicCred = creds.GetCredential(new Uri(wmsGetCapstextbox.Text), "Basic");
                treeOgcCaps.Credentials = creds;
             }
            
            //treeOgcCaps.Version = "1.3.0";

            //treeOgcCaps.Proxy = ProxyHelper.DetermineProxyForUrl(treeOgcCaps.URL, true, true, "http://192.168.25.13/proxy.pac", "", "");

            //treeOgcCaps.ServiceType = OGCServiceTypes.WMS;
            //CarbonTools.Core.OGCCapabilities.DataOGCCapabilities foo = new DataOGCCapabilities();
            //CarbonTools.Core.WMS.SourceWMS swms = new CarbonTools.Core.WMS.SourceWMS();
            //SourceOGCCapabilities bar = new SourceOGCCapabilities();
            
			treeViewProgressBar.Visible = true;
            treeOgcCaps.GetCapabilities();

            //treeOgcCaps.node
            

			System.Windows.Forms.Timer treeViewTimer = new System.Windows.Forms.Timer();
			treeViewTimer.Enabled = true;
			treeViewTimer.Interval = 50;
			treeViewTimer.Tick += new EventHandler(treeViewTimer_Tick);


        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void savewmsbutton_Click(object sender, EventArgs e)
        {
            


			string wmslayerset = "<LayerSet Name=\"" + textBox3.Text + "\" ShowOnlyOneLayer=\"false\" ShowAtStartup=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"LayerSet.xsd\">\n";
            wmslayerset += ParseNodeChildren(treeOgcCaps.Nodes[0], UsernameTextBox.Text, PasswordTextBox.Text);
			wmslayerset += "</LayerSet>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(wmslayerset);
            string xmlFileName = textBox1.Text + ".xml";
                        	
            string wmsSave = Path.Combine(
            Path.Combine(Global.Settings.ConfigPath, worldWindow.CurrentWorld.ToString()),
            xmlFileName);
			try
			{
				doc.Save(wmsSave);
			}
			catch (System.Xml.XmlException ex)
			{
				MessageBox.Show("Couldn't write \"" + wmsSave + "\":\n\n" + ex.Message);
			}

            RenderableObjectList layers = ConfigurationLoader.getRenderableFromLayerFile(wmsSave, worldWindow.CurrentWorld, worldWindow.Cache);
            worldWindow.CurrentWorld.RenderableObjects.Add(layers);

        }

        private string ConvertLayerToWMS(LayerItem layer, string Username, string Password)
        {

			//TODO: Add layer abstracts from wms as <Description> in xml
			//TODO: Add legends as screenoverlays where applicable

            string skeleton;
            string FILE_NAME = this.wms_skeleton_path;
            string mapfilepath;
            string imageType = comboBox2.Text;

            skeleton = ReadSkeleton(FILE_NAME);
            skeleton = skeleton.Replace(@"$USERNAME", Username);
            skeleton = skeleton.Replace(@"$PASSWORD", Password);
            skeleton = skeleton.Replace(@"$NAME", ConvertToXMLEntities(layer.Title));
            skeleton = skeleton.Replace(@"$NBB", layer.LLBoundingBox.MaxY.ToString(CultureInfo.InvariantCulture));
			skeleton = skeleton.Replace(@"$SBB", layer.LLBoundingBox.MinY.ToString(CultureInfo.InvariantCulture));
			skeleton = skeleton.Replace(@"$EBB", layer.LLBoundingBox.MaxX.ToString(CultureInfo.InvariantCulture));
			skeleton = skeleton.Replace(@"$WBB", layer.LLBoundingBox.MinX.ToString(CultureInfo.InvariantCulture));
            //replace request getcapabilities with getmap
            //string wmsGetMap = wmsGetCapstextbox.Text.ToLowerInvariant().Replace("request=getcapabilities", "request=getmap");
            string wmsGetMap = Regex.Replace(wmsGetCapstextbox.Text.ToLowerInvariant(), "\\?.*$", "");

            //check for wms that requires path to .map file in url
            if (wmsGetCapstextbox.Text.ToLowerInvariant().Contains("map="))
            {
                mapfilepath = Regex.Match(wmsGetCapstextbox.Text, "map=.*\\.map").Value;
                skeleton = skeleton.Replace(@"$LAYERNAME", layer.Name + @"&amp;TRANSPARENT=TRUE&amp;BGCOLOR=0x000000&amp;" + mapfilepath);
            }
            else
            {
                skeleton = skeleton.Replace(@"$LAYERNAME", layer.Name + @"&amp;TRANSPARENT=TRUE&amp;BGCOLOR=0x000000");
            }

            skeleton = skeleton.Replace(@"$SERVER", ConvertToXMLEntities(wmsGetMap));

            skeleton = skeleton.Replace(@"$IMAGETYPE", ConvertToXMLEntities(imageType));

            return skeleton;
        }


        private static string ReadSkeleton(string FILE_NAME)
        {
            string skeleton = null;
            if (!File.Exists(FILE_NAME))
            {
                MessageBox.Show(FILE_NAME + " does not exist");
                return skeleton;
            }

            String input;
            using (StreamReader sr = File.OpenText(FILE_NAME))
            {
                while ((input = sr.ReadLine()) != null)
                {
                    skeleton += input + new string((Char)13, 1);
                }
                sr.Close();
            }
            return skeleton;
        }

        private string ConvertToXMLEntities(string text)
        {
            text = Regex.Replace(text, "&", "&amp;");
            text = Regex.Replace(text, "<", "&lt;");
            text = Regex.Replace(text, ">", "&gt;");
            text = Regex.Replace(text, "\"", "&quot;");
            //text = Regex.Replace( text, " ", "&nbsp;" );
            //text = Regex.Replace( text, "$", "<br />" );
            return text;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panelLower_Paint(object sender, PaintEventArgs e)
        {

        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
			treeOgcCaps.Nodes.Clear();
			wmsGetCapstextbox.Text = "";
			textBox3.Text = "";

            e.Cancel = true;
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void treeOgcCaps_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Check if selected node exists 
            if (treeOgcCaps.SelectedNode == null) return;
            // Get selected layer data 
            LayerItem layer = treeOgcCaps.SelectedNode.Tag as LayerItem;
            if (layer == null) return;

            // Set map view paramerters 

            //pictureBoxOGC1.URL = treeOgcCaps.URL;
            //pictureBoxOGC1.ServiceType = OGCServiceTypes.WMS;
            //pictureBoxOGC1.LayerName = layer.Name;  // Set layer name 
			//pictureBoxProgressBar.Value = 0;
            // Update image 
            //pictureBoxOGC1.GetImage();

			//System.Windows.Forms.Timer pictureBoxTimer = new System.Windows.Forms.Timer();
			//pictureBoxTimer.Enabled = true;
			//pictureBoxTimer.Interval = 50;
			//pictureBoxTimer.Tick += new EventHandler(pictureBoxTimer_Tick);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                label6.Enabled = true;
                textBox3.Enabled = true;
            }
            else if (checkBox1.Checked == true)
            {
				LayerItem li = treeOgcCaps.Nodes[0].Tag as LayerItem;
				textBox3.Text = li.Title;

                label6.Enabled = false;
                textBox3.Enabled = false;
            }

        }
        
        /// <summary>
        /// Call back after available layers returned from WMS url. 
        /// Populates the first node as the default selection in the 'specify layer title' text box. Enables 'Create Layer' group box (Save as XML) in the UI.
        /// If no layers are returned by URL, display message box error to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void treeOgcCaps_OperationDone(object sender, EventArgs e)
		{
            //if the URL is not good, it can crash WW, handle exception better
            if (treeOgcCaps.Nodes != null && treeOgcCaps.Nodes.Count > 0)
            {
                TreeNode tn = treeOgcCaps.Nodes[0];
                LayerItem li = tn.Tag as LayerItem;

                textBox3.Text = li.Title;
                xmlsaveGroupBox.Enabled = true;
            }
            else
            {
                String logMsg = "WMS url did not return any valid layers " + wmsGetCapstextbox.Text;
                Log.Write(Log.Levels.Error, "WBNG", logMsg);
                MessageBox.Show("WMS url did not return any valid layers.");
            }
			treeViewProgressBar.Visible = false;
		}





		private string ParseNodeChildren(TreeNode node, string Username, string Password)
		{
			string returned = null;

			if (node.Nodes.Count != 0)
			{
				
				LayerItem li = node.Tag as LayerItem;

				if (node.Level != 0)
				{
					returned += "<ChildLayerSet Name=\"" + li.Title + "\" ShowOnlyOneLayer=\"false\" ShowAtStartup=\"true\">\n";
				}

				foreach (TreeNode subNode in node.Nodes)
				{
					returned += ParseNodeChildren(subNode, Username, Password);
				}

				if (node.Level != 0)
				{
					returned += "</ChildLayerSet>\n";
				}

				return returned;
			}
			else
			{

				LayerItem li = node.Tag as LayerItem;
				returned += ConvertLayerToWMS(li, Username, Password);
				return returned;
			}

		}

		//private void pictureBoxTimer_Tick(object sender, EventArgs e)
		//{
		//	pictureBoxProgressBar.Value = pictureBoxOGC1.GetProgress(100);
		//}

		private void treeViewTimer_Tick(object sender, EventArgs e)
		{
			treeViewProgressBar.Value = treeOgcCaps.GetProgress(100);
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void wmsGetCapstextbox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

    }
}
