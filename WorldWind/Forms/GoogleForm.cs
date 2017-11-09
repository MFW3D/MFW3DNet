using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using WorldWind;
using WorldWind.Net;
using WorldWind.Renderable;
using WorldWind.Terrain;
namespace bNb.Plugins
{
    #region GoogleMapsFORM
    public class GoogleMapsForm : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.RadioButton rbRoad;
        private System.Windows.Forms.RadioButton rbAerial;
        private System.Windows.Forms.RadioButton rbDebug;
        private System.Windows.Forms.Button btnLocateMe;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel lnkBnb;
        private System.Windows.Forms.LinkLabel lnkLocalLive;
        private System.Windows.Forms.Button btnLocalLive;
        private System.Windows.Forms.TrackBar tbZoomLevel;
        private System.Windows.Forms.Label lblZoomLevel;
        private System.Windows.Forms.CheckBox cbLayerIsOn;
        private System.Windows.Forms.CheckBox cbTerrain;
        private System.Windows.Forms.Button btnFindBusiness;
        private System.Windows.Forms.TextBox txtNameCategory;
        private System.Windows.Forms.TextBox txtCityState;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button btnFindAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnTrimCache;
        private System.Windows.Forms.NumericUpDown numTrimCache;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnRemovePushPins;
        private System.Windows.Forms.Label zoomlabel;
        private System.Windows.Forms.GroupBox groupLanguage;
        private System.Windows.Forms.RadioButton radioEN;
        private System.Windows.Forms.RadioButton radioCN;
        private System.Windows.Forms.RadioButton rbHybrid;
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbRoad = new System.Windows.Forms.RadioButton();
            this.rbAerial = new System.Windows.Forms.RadioButton();
            this.rbHybrid = new System.Windows.Forms.RadioButton();
            this.rbDebug = new System.Windows.Forms.RadioButton();
            this.btnLocateMe = new System.Windows.Forms.Button();
            this.lnkBnb = new System.Windows.Forms.LinkLabel();
            this.lnkLocalLive = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLocalLive = new System.Windows.Forms.Button();
            this.tbZoomLevel = new System.Windows.Forms.TrackBar();
            this.lblZoomLevel = new System.Windows.Forms.Label();
            this.cbLayerIsOn = new System.Windows.Forms.CheckBox();
            this.cbTerrain = new System.Windows.Forms.CheckBox();
            this.txtNameCategory = new System.Windows.Forms.TextBox();
            this.txtCityState = new System.Windows.Forms.TextBox();
            this.btnFindBusiness = new System.Windows.Forms.Button();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.btnFindAddress = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnTrimCache = new System.Windows.Forms.Button();
            this.numTrimCache = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnRemovePushPins = new System.Windows.Forms.Button();
            this.groupLanguage = new System.Windows.Forms.GroupBox();
            this.radioEN = new System.Windows.Forms.RadioButton();
            this.radioCN = new System.Windows.Forms.RadioButton();
            this.zoomlabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbZoomLevel)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrimCache)).BeginInit();
            this.groupLanguage.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbRoad
            // 
            this.rbRoad.Enabled = false;
            this.rbRoad.Location = new System.Drawing.Point(19, 17);
            this.rbRoad.Name = "rbRoad";
            this.rbRoad.Size = new System.Drawing.Size(77, 26);
            this.rbRoad.TabIndex = 1;
            this.rbRoad.Text = "Road";
            this.rbRoad.CheckedChanged += new System.EventHandler(this.group_CheckedChanged);
            // 
            // rbAerial
            // 
            this.rbAerial.Checked = true;
            this.rbAerial.Location = new System.Drawing.Point(19, 43);
            this.rbAerial.Name = "rbAerial";
            this.rbAerial.Size = new System.Drawing.Size(85, 26);
            this.rbAerial.TabIndex = 2;
            this.rbAerial.TabStop = true;
            this.rbAerial.Text = "Aerial";
            this.rbAerial.CheckedChanged += new System.EventHandler(this.group_CheckedChanged);
            // 
            // rbHybrid
            // 
            this.rbHybrid.Enabled = false;
            this.rbHybrid.Location = new System.Drawing.Point(19, 69);
            this.rbHybrid.Name = "rbHybrid";
            this.rbHybrid.Size = new System.Drawing.Size(85, 26);
            this.rbHybrid.TabIndex = 3;
            this.rbHybrid.Text = "Hybrid";
            this.rbHybrid.CheckedChanged += new System.EventHandler(this.group_CheckedChanged);
            // 
            // rbDebug
            // 
            this.rbDebug.Location = new System.Drawing.Point(19, 95);
            this.rbDebug.Name = "rbDebug";
            this.rbDebug.Size = new System.Drawing.Size(77, 26);
            this.rbDebug.TabIndex = 5;
            this.rbDebug.Text = "Debug";
            this.rbDebug.CheckedChanged += new System.EventHandler(this.group_CheckedChanged);
            // 
            // btnLocateMe
            // 
            this.btnLocateMe.Enabled = false;
            this.btnLocateMe.Location = new System.Drawing.Point(336, 8);
            this.btnLocateMe.Name = "btnLocateMe";
            this.btnLocateMe.Size = new System.Drawing.Size(173, 25);
            this.btnLocateMe.TabIndex = 6;
            this.btnLocateMe.Text = "\'Locate Me\' by IP Address";
            this.btnLocateMe.Visible = false;
            this.btnLocateMe.Click += new System.EventHandler(this.btnLocateMe_Click);
            // 
            // lnkBnb
            // 
            this.lnkBnb.Enabled = false;
            this.lnkBnb.Location = new System.Drawing.Point(168, 344);
            this.lnkBnb.Name = "lnkBnb";
            this.lnkBnb.Size = new System.Drawing.Size(336, 25);
            this.lnkBnb.TabIndex = 7;
            this.lnkBnb.TabStop = true;
            this.lnkBnb.Text = "http://www.brains-N-brawn.com/veWorldWind/";
            this.lnkBnb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkBnb.Visible = false;
            this.lnkBnb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBnb_LinkClicked);
            // 
            // lnkLocalLive
            // 
            this.lnkLocalLive.Enabled = false;
            this.lnkLocalLive.Location = new System.Drawing.Point(360, 368);
            this.lnkLocalLive.Name = "lnkLocalLive";
            this.lnkLocalLive.Size = new System.Drawing.Size(150, 25);
            this.lnkLocalLive.TabIndex = 8;
            this.lnkLocalLive.TabStop = true;
            this.lnkLocalLive.Text = "http://maps.google.com/";
            this.lnkLocalLive.Visible = false;
            this.lnkLocalLive.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLocalLive_LinkClicked);
            // 
            // label1
            // 
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(152, 368);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 25);
            this.label1.TabIndex = 9;
            this.label1.Text = "data provided by Google from";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label1.Visible = false;
            // 
            // btnLocalLive
            // 
            this.btnLocalLive.Enabled = false;
            this.btnLocalLive.Location = new System.Drawing.Point(336, 40);
            this.btnLocalLive.Name = "btnLocalLive";
            this.btnLocalLive.Size = new System.Drawing.Size(168, 25);
            this.btnLocalLive.TabIndex = 10;
            this.btnLocalLive.Text = "Open in local.live.com";
            this.btnLocalLive.Visible = false;
            this.btnLocalLive.Click += new System.EventHandler(this.btnLocalLive_Click);
            // 
            // tbZoomLevel
            // 
            this.tbZoomLevel.Location = new System.Drawing.Point(10, 233);
            this.tbZoomLevel.Maximum = 13;
            this.tbZoomLevel.Minimum = 3;
            this.tbZoomLevel.Name = "tbZoomLevel";
            this.tbZoomLevel.Size = new System.Drawing.Size(134, 42);
            this.tbZoomLevel.TabIndex = 12;
            this.tbZoomLevel.Value = 8;
            this.tbZoomLevel.ValueChanged += new System.EventHandler(this.tbZoomLevel_ValueChanged);
            // 
            // lblZoomLevel
            // 
            this.lblZoomLevel.Enabled = false;
            this.lblZoomLevel.Location = new System.Drawing.Point(144, 216);
            this.lblZoomLevel.Name = "lblZoomLevel";
            this.lblZoomLevel.Size = new System.Drawing.Size(24, 17);
            this.lblZoomLevel.TabIndex = 13;
            this.lblZoomLevel.Text = "8";
            this.lblZoomLevel.Click += new System.EventHandler(this.lblZoomLevel_Click);
            // 
            // cbLayerIsOn
            // 
            this.cbLayerIsOn.Checked = true;
            this.cbLayerIsOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLayerIsOn.Location = new System.Drawing.Point(10, 9);
            this.cbLayerIsOn.Name = "cbLayerIsOn";
            this.cbLayerIsOn.Size = new System.Drawing.Size(86, 25);
            this.cbLayerIsOn.TabIndex = 14;
            this.cbLayerIsOn.Text = "Layer On";
            this.cbLayerIsOn.CheckedChanged += new System.EventHandler(this.cbLayerIsOn_CheckedChanged);
            // 
            // cbTerrain
            // 
            this.cbTerrain.Location = new System.Drawing.Point(10, 34);
            this.cbTerrain.Name = "cbTerrain";
            this.cbTerrain.Size = new System.Drawing.Size(96, 26);
            this.cbTerrain.TabIndex = 15;
            this.cbTerrain.Text = "Terrain On";
            this.cbTerrain.CheckedChanged += new System.EventHandler(this.cbTerrain_CheckedChanged);
            // 
            // txtNameCategory
            // 
            this.txtNameCategory.Enabled = false;
            this.txtNameCategory.Location = new System.Drawing.Point(280, 69);
            this.txtNameCategory.Name = "txtNameCategory";
            this.txtNameCategory.Size = new System.Drawing.Size(211, 21);
            this.txtNameCategory.TabIndex = 16;
            this.txtNameCategory.Text = "";
            this.txtNameCategory.Visible = false;
            // 
            // txtCityState
            // 
            this.txtCityState.Enabled = false;
            this.txtCityState.Location = new System.Drawing.Point(280, 112);
            this.txtCityState.Name = "txtCityState";
            this.txtCityState.Size = new System.Drawing.Size(211, 21);
            this.txtCityState.TabIndex = 17;
            this.txtCityState.Text = "";
            this.txtCityState.Visible = false;
            // 
            // btnFindBusiness
            // 
            this.btnFindBusiness.Enabled = false;
            this.btnFindBusiness.Location = new System.Drawing.Point(376, 138);
            this.btnFindBusiness.Name = "btnFindBusiness";
            this.btnFindBusiness.Size = new System.Drawing.Size(115, 25);
            this.btnFindBusiness.TabIndex = 18;
            this.btnFindBusiness.Text = "Find Businesses";
            this.btnFindBusiness.Visible = false;
            this.btnFindBusiness.Click += new System.EventHandler(this.btnFindBusiness_Click);
            // 
            // txtAddress
            // 
            this.txtAddress.Enabled = false;
            this.txtAddress.Location = new System.Drawing.Point(280, 181);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(211, 21);
            this.txtAddress.TabIndex = 19;
            this.txtAddress.Text = "";
            this.txtAddress.Visible = false;
            // 
            // btnFindAddress
            // 
            this.btnFindAddress.Enabled = false;
            this.btnFindAddress.Location = new System.Drawing.Point(376, 232);
            this.btnFindAddress.Name = "btnFindAddress";
            this.btnFindAddress.Size = new System.Drawing.Size(115, 25);
            this.btnFindAddress.TabIndex = 20;
            this.btnFindAddress.Text = "Find Address";
            this.btnFindAddress.Visible = false;
            this.btnFindAddress.Click += new System.EventHandler(this.btnFindAddress_Click);
            // 
            // label3
            // 
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(168, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 25);
            this.label3.TabIndex = 21;
            this.label3.Text = "Name/Category";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(208, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 25);
            this.label4.TabIndex = 22;
            this.label4.Text = "City/State";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(216, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 25);
            this.label5.TabIndex = 23;
            this.label5.Text = "Address";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Visible = false;
            // 
            // label6
            // 
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(280, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 25);
            this.label6.TabIndex = 24;
            this.label6.Text = "e.g. hooters";
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(280, 138);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 25);
            this.label7.TabIndex = 25;
            this.label7.Text = "e.g. wisconsin";
            this.label7.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDebug);
            this.groupBox1.Controls.Add(this.rbHybrid);
            this.groupBox1.Controls.Add(this.rbAerial);
            this.groupBox1.Controls.Add(this.rbRoad);
            this.groupBox1.Location = new System.Drawing.Point(10, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(110, 129);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map Type";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 272);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 43);
            this.label2.TabIndex = 27;
            this.label2.Text = "a lower zoom level will render Google Maps tiles at higher altitudes";
            // 
            // label8
            // 
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(280, 207);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(202, 25);
            this.label8.TabIndex = 28;
            this.label8.Text = "e.g. one microsoft way, redmond";
            this.label8.Visible = false;
            // 
            // btnTrimCache
            // 
            this.btnTrimCache.Enabled = false;
            this.btnTrimCache.Location = new System.Drawing.Point(368, 304);
            this.btnTrimCache.Name = "btnTrimCache";
            this.btnTrimCache.Size = new System.Drawing.Size(124, 25);
            this.btnTrimCache.TabIndex = 29;
            this.btnTrimCache.Text = "Trim Cached Files";
            this.btnTrimCache.Visible = false;
            this.btnTrimCache.Click += new System.EventHandler(this.btnTrimCache_Click);
            // 
            // numTrimCache
            // 
            this.numTrimCache.Enabled = false;
            this.numTrimCache.Location = new System.Drawing.Point(376, 272);
            this.numTrimCache.Name = "numTrimCache";
            this.numTrimCache.Size = new System.Drawing.Size(48, 21);
            this.numTrimCache.TabIndex = 30;
            this.numTrimCache.Value = new System.Decimal(new int[] {
                    7,
                    0,
                    0,
                    0});
            this.numTrimCache.Visible = false;
            // 
            // label9
            // 
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(224, 272);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 24);
            this.label9.TabIndex = 31;
            this.label9.Text = "Delete tiles more than";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label9.Visible = false;
            // 
            // label10
            // 
            this.label10.Enabled = false;
            this.label10.Location = new System.Drawing.Point(424, 272);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 24);
            this.label10.TabIndex = 32;
            this.label10.Text = "days old";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label10.Visible = false;
            // 
            // btnRemovePushPins
            // 
            this.btnRemovePushPins.Enabled = false;
            this.btnRemovePushPins.Location = new System.Drawing.Point(232, 232);
            this.btnRemovePushPins.Name = "btnRemovePushPins";
            this.btnRemovePushPins.Size = new System.Drawing.Size(135, 25);
            this.btnRemovePushPins.TabIndex = 33;
            this.btnRemovePushPins.Text = "Remove PushPins";
            this.btnRemovePushPins.Visible = false;
            this.btnRemovePushPins.Click += new System.EventHandler(this.btnRemovePushPins_Click);
            // 
            // groupLanguage
            // 
            this.groupLanguage.Controls.Add(this.radioEN);
            this.groupLanguage.Controls.Add(this.radioCN);
            this.groupLanguage.Location = new System.Drawing.Point(8, 328);
            this.groupLanguage.Name = "groupLanguage";
            this.groupLanguage.Size = new System.Drawing.Size(144, 56);
            this.groupLanguage.TabIndex = 34;
            this.groupLanguage.TabStop = false;
            this.groupLanguage.Text = "Language";
            // 
            // radioEN
            // 
            this.radioEN.Location = new System.Drawing.Point(8, 24);
            this.radioEN.Name = "radioEN";
            this.radioEN.Size = new System.Drawing.Size(72, 24);
            this.radioEN.TabIndex = 0;
            this.radioEN.Text = "English";
            this.radioEN.CheckedChanged += new System.EventHandler(this.radioEN_CheckedChanged);
            // 
            // radioCN
            // 
            this.radioCN.Checked = true;
            this.radioCN.Location = new System.Drawing.Point(88, 24);
            this.radioCN.Name = "radioCN";
            this.radioCN.Size = new System.Drawing.Size(48, 24);
            this.radioCN.TabIndex = 0;
            this.radioCN.TabStop = true;
            this.radioCN.Text = "中文";
            this.radioCN.CheckedChanged += new System.EventHandler(this.radioCN_CheckedChanged);
            // 
            // zoomlabel
            // 
            this.zoomlabel.Location = new System.Drawing.Point(8, 216);
            this.zoomlabel.Name = "zoomlabel";
            this.zoomlabel.Size = new System.Drawing.Size(136, 17);
            this.zoomlabel.TabIndex = 13;
            this.zoomlabel.Text = "starting zoom level :";
            this.zoomlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.zoomlabel.Click += new System.EventHandler(this.zoomlabel_Click);
            // 
            // GoogleMapsForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(162, 407);
            this.Controls.Add(this.lblZoomLevel);
            this.Controls.Add(this.groupLanguage);
            this.Controls.Add(this.btnRemovePushPins);
            this.Controls.Add(this.btnTrimCache);
            this.Controls.Add(this.numTrimCache);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnFindBusiness);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnFindAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.txtCityState);
            this.Controls.Add(this.txtNameCategory);
            this.Controls.Add(this.cbTerrain);
            this.Controls.Add(this.cbLayerIsOn);
            this.Controls.Add(this.tbZoomLevel);
            this.Controls.Add(this.btnLocalLive);
            this.Controls.Add(this.lnkLocalLive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lnkBnb);
            this.Controls.Add(this.btnLocateMe);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.zoomlabel);
            this.Controls.Add(this.label8);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoogleMapsForm";
            this.Text = "GoogleMaps v0.2";
            this.Load += new System.EventHandler(this.GoogleMapsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbZoomLevel)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numTrimCache)).EndInit();
            this.groupLanguage.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
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
            //remove from renderable objects
            lock (m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects.SyncRoot)
            {
                veLayer.IsOn = false;
                foreach (WorldWind.Renderable.RenderableObject ro in m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects)
                {
                    if (ro is WorldWind.Renderable.RenderableObjectList && ro.Name.IndexOf("Images") >= 0)
                    {
                        WorldWind.Renderable.RenderableObjectList imagesList = ro as WorldWind.Renderable.RenderableObjectList;
                        //imagesList.ChildObjects.Insert(0, m_RenderableList);
                        //insert it at the end of the list
                        imagesList.ChildObjects.Remove(veLayer);
                        break;
                    }
                }
                veLayer.Dispose();
            }
            //Save GUI settings
            string settingspath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\VirtualEarth\\Settings.xml";
            VESettings.SaveSettingsToFile(settingspath, settings);
            base.Dispose(disposing);
        }
        private WorldWind.WorldWindow m_WorldWindow = null;
        public WorldWind.WorldWindow WorldWindow
        {
            get { return m_WorldWindow; }
        }

        public bool IsDebug
        {
            get
            {
                return rbDebug.Checked;
            }
        }
        public int StartZoomLevel
        {
            get
            {
                return tbZoomLevel.Value;
            }
        }
        private VeReprojectTilesLayer veLayer;
        public VeReprojectTilesLayer VeLayer
        {
            get { return veLayer; }
        }
        private string cacheDirectory;
        private string pushPinTexture;
        private VESettings settings = new VESettings();

        public GoogleMapsForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            try
            {
                veLayer = new VeReprojectTilesLayer("Google Maps",  this);
                lock (m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects.SyncRoot)
                {
                    foreach (WorldWind.Renderable.RenderableObject ro in m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects)
                    {
                        if (ro is WorldWind.Renderable.RenderableObjectList && ro.Name.IndexOf("Images") >= 0)
                        {
                            WorldWind.Renderable.RenderableObjectList imagesList = ro as WorldWind.Renderable.RenderableObjectList;
                            //imagesList.ChildObjects.Insert(0, m_RenderableList);
                            //insert it at the end of the list
                            imagesList.ChildObjects.Insert(imagesList.ChildObjects.Count - 1, veLayer);
                            break;
                        }
                    }
                }
                cacheDirectory = String.Format("{0}\\GoogleMaps", m_WorldWindow.Cache.CacheDirectory);
                if (Directory.Exists(cacheDirectory) == true)
                {
                    DirectoryInfo diCache = new DirectoryInfo(cacheDirectory);
                    //for debug, delete the entire cache
                    //diCache.Delete(true);
                }
                //#if DEBUG
                pushPinTexture = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\GoogleMaps\\GoogleMapsPushPin.png";
                //#else
                //    pushPinTexture = GoogleMapsPlugin.PluginDir + @"\GoogleMapsPushPin.png";
                //#endif
                if (File.Exists(pushPinTexture) == false)
                {
                    Utility.Log.Write("pushPinTexture not found " + pushPinTexture);
                }
                //check that proj.dll is installed correctly, else set plugin to off
                string projDllPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\proj.dll";
                if (File.Exists(projDllPath) == false)
                {
                    //TODO turned off for debugging
                    veLayer.IsOn = false;
                    throw new Exception("'proj.dll' needs to be in the same directory where WorldWind.exe is installed");
                }
                //Load up settings if they exist
                string settingspath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\GoogleMaps\\Settings.xml";
                if (File.Exists(settingspath))
                    settings = VESettings.LoadSettingsFromFile(settingspath);
                else
                    VESettings.SaveSettingsToFile(settingspath, settings);
                //Apply loaded settings
                tbZoomLevel.Value = settings.ZoomLevel;
                cbLayerIsOn.Checked = settings.LayerOn;
                cbTerrain.Checked = settings.Terrain;
                rbRoad.Checked = settings.Road;
                rbAerial.Checked = settings.Aerial;
                rbHybrid.Checked = settings.Hybrid;
                rbDebug.Checked = settings.Debug;
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
                throw;
            }
        }
        private void GoogleMapsForm_Load(object sender, System.EventArgs e)
        {
            en_cn();
        }
        public string GetDataSetName()
        {
            string dataSetName = "r";
            if (rbRoad.Checked == true)
            {
                dataSetName = "r";
            }
            else if (rbAerial.Checked == true)
            {
                dataSetName = "s";//原为"a"
            }
            else if (rbHybrid.Checked == true)
            {
                dataSetName = "h";
            }
            return dataSetName;
        }
        public string GetImageExtension()
        {
            string imageExtension = "jpg";
            if (rbRoad.Checked == true)
            {
                imageExtension = "png";
            }
            return imageExtension;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            base.OnClosing(e);
        }
        private string previousDataSetName = null;
        private void group_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                string curDataSetName = this.GetDataSetName();
                if (curDataSetName != previousDataSetName)
                {
                    veLayer.RemoveAllTiles();
                    veLayer.ForceRefresh();
                }
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        private void btnLocateMe_Click(object sender, System.EventArgs e)
        {
            //used to be able to call the LocationFinder direction, for more accurate results
            //but that doesn't seem to work anymore, so fall back to ip address
            //and use GoogleMaps (local.live.com) to find location
            //http://viaGoogleMaps.com/vve/Articles/ObtainingVisitorLocation.ashx
            HttpWebResponse hwRes = null;
            Stream s = null;
            StreamReader sr = null;
            try
            {
                string reqUrl = "http://maps.google.com";
                HttpWebRequest hwReq = (HttpWebRequest)WebRequest.Create(reqUrl);
                //    hwReq.UserAgent = "NASA WorldWind 1.3.3.1";
                hwReq.UserAgent = "Internet Explorer";
                hwReq.Timeout = 10000; //10 second timeout
                hwRes = (HttpWebResponse)hwReq.GetResponse();
                s = hwRes.GetResponseStream();
                sr = new StreamReader(s);
                string result = sr.ReadToEnd();
                //SetAutoLocateViewport(43.0452, -88.3996, 10, false, 'Virtual Earth has determined your location by using your computer address.');
                //"ShowMessage(\"The server is currently busy. Try again later.\");"
                //ShowMessage("Virtual Earth cannot determine your current location. Try again later.");
                int index = result.ToLower().IndexOf("setautolocateviewport");
                if (index != -1)
                {
                    //success
                    int openParen = result.IndexOf("(");
                    if (openParen != -1)
                    {
                        result = result.Substring(openParen + 1, result.Length - openParen - 1);
                        string strSplitChar = ",";
                        string[] strVals = result.Split(strSplitChar.ToCharArray());
                        if (strVals.Length >= 2)
                        {
                            float lat = float.Parse(strVals[0]);
                            float lon = float.Parse(strVals[1]);
                            m_WorldWindow.GotoLatLon(lat, lon);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("could not auto locate");
                }
            }
            catch (Exception ex)
            {
                string sex = ex.ToString();
                Utility.Log.Write(ex);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr = null;
                }
                if (s != null)
                {
                    s.Close();
                    s = null;
                }
                if (hwRes != null)
                {
                    hwRes.Close();
                    hwRes = null;
                }
            }
        }
        private void btnLocalLive_Click(object sender, System.EventArgs e)
        {
            try
            {
                string link = veLayer.GetLocalLiveLink();
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        private void lnkLocalLive_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://maps.google.com/");
        }
        private void lnkBnb_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.brains-N-brawn.com/veWorldWind/");
        }
        private void tbZoomLevel_ValueChanged(object sender, System.EventArgs e)
        {
            lblZoomLevel.Text = tbZoomLevel.Value.ToString();
        }
        private void cbLayerIsOn_CheckedChanged(object sender, System.EventArgs e)
        {
            if (cbLayerIsOn.Checked == true)
            {
                veLayer.IsOn = true;
            }
            else
            {
                veLayer.IsOn = false;
            }
        }
        private void btnFindBusiness_Click(object sender, System.EventArgs e)
        {
            try
            {
                veLayer.PushPins = null;
                string nameCategory = txtNameCategory.Text.Trim();
                string cityState = txtCityState.Text.Trim();
                //ArrayList alPushPins = Search.SearchForBusiness(
                double lat1, lon1, lat2, lon2;
                double halfViewRange = m_WorldWindow.DrawArgs.WorldCamera.TrueViewRange.Degrees / 2;
                double lat = m_WorldWindow.DrawArgs.WorldCamera.Latitude.Degrees;
                double lon = m_WorldWindow.DrawArgs.WorldCamera.Longitude.Degrees;
                lat1 = lat + halfViewRange;
                lon1 = lon + halfViewRange;
                lat2 = lat - halfViewRange;
                lon2 = lon - halfViewRange;
                if (cityState.Length > 0)
                {
                    bool retVal = Search.SearchForAddress(cityState, out lat1, out lon1, out lat2, out lon2);
                }
                ArrayList alPushPins = Search.SearchForBusiness(nameCategory, lat1, lon1, lat2, lon2);
                if (alPushPins.Count <= 0)
                {
                    if (this.radioEN.Checked) MessageBox.Show("no businesses found");
                    if (this.radioCN.Checked) MessageBox.Show("商业信息未找到");
                }
                else if (alPushPins.Count == 1)
                {
                    //TODO replace with Icon usage
                    veLayer.PushPins = alPushPins;
                    PushPin pp = (PushPin)alPushPins[0];
                    double altitude = 5000;
                    m_WorldWindow.GotoLatLonAltitude(pp.Latitude, pp.Longitude, altitude);
                }
                else
                {
                    //figure out correct zoom level and set altitude
                    //TODO replace with Icon usage
                    veLayer.PushPins = alPushPins;
                    double ppLat = (lat1 + lat2) / 2;
                    double ppLon = (lon1 + lon2) / 2;
                    double perpViewRange = Math.Abs(lat1 - lat2);
                    m_WorldWindow.GotoLatLonViewRange(ppLat, ppLon, perpViewRange);
                }
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        private void btnFindAddress_Click(object sender, System.EventArgs e)
        {
            try
            {
                veLayer.PushPins = null;
                string address = txtAddress.Text.Trim();
                double lat1, lon1, lat2, lon2;
                bool retVal = Search.SearchForAddress(address, out lat1, out lon1, out lat2, out lon2);
                if (retVal == true)
                {
                    double lat = (lat1 + lat2) / 2;
                    double lon = (lon1 + lon2) / 2;
                    //TODO make sure to properly dispose of icons
                    //WorldWind.Renderable.Icon icon = new WorldWind.Renderable.Icon("address", "description", lat, lon, 100, m_WorldWindow.CurrentWorld, pushPinTexture, 64, 64, "http://www.brains-N-brawn.com"); 
                    //WorldWind.Renderable.Icon icon = new WorldWind.Renderable.Icon(address, (float)lat, (float)lon, 100);
                    //icon.Width = 96;
                    //icon.Height = 96;
                    //icon.TextureFileName = pushPinTexture;
                    PushPin pp = new PushPin();
                    pp.Address = address;
                    pp.Latitude = lat;
                    pp.Longitude = lon;
                    ArrayList alPushPins = new ArrayList();
                    alPushPins.Add(pp);
                    veLayer.PushPins = alPushPins;
                    //veLayer.PushPins = new WorldWind.Renderable.Icons("icons");
                    //veLayer.PushPins.AddIcon(icon);
                    double altitude = 5000;
                    m_WorldWindow.GotoLatLonAltitude(lat, lon, altitude);
                }
                else
                {
                    if (this.radioEN.Checked) MessageBox.Show("address not found");
                    if (this.radioCN.Checked) MessageBox.Show("地址未找到");
                }
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        private void cbTerrain_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                veLayer.RemoveAllTiles();
                veLayer.ForceRefresh();
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        private void btnTrimCache_Click(object sender, System.EventArgs e)
        {
            try
            {
                //possibly iter the dirs and delete old tiles
                if (Directory.Exists(cacheDirectory) == true)
                {
                    DirectoryInfo diCache = new DirectoryInfo(cacheDirectory);
                    //TODO TEST THIS, make this a manual/configured operation
                    int numDays = (int)numTrimCache.Value * -1;
                    DateTime cutOffDate = DateTime.Now.AddDays(numDays);
                    RecurseDeleteOldFiles(diCache, cutOffDate);
                }
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        public void RecurseDeleteOldFiles(DirectoryInfo di, DateTime cutOffDate)
        {
            foreach (FileInfo fi in di.GetFiles("*.png"))
            {
                if (fi.CreationTime < cutOffDate)
                {
                    fi.Delete();
                }
            }
            foreach (FileInfo fi in di.GetFiles("*.jpg"))
            {
                if (fi.CreationTime < cutOffDate)
                {
                    fi.Delete();
                }
            }
            foreach (DirectoryInfo tempDi in di.GetDirectories())
            {
                RecurseDeleteOldFiles(tempDi, cutOffDate);
            }
        }
        private void btnRemovePushPins_Click(object sender, System.EventArgs e)
        {
            try
            {
                veLayer.PushPins = null;
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        private void radioEN_CheckedChanged(object sender, System.EventArgs e)
        {
            en_cn();
        }
        private void radioCN_CheckedChanged(object sender, System.EventArgs e)
        {
            en_cn();
        }

        private void en_cn()
        {
            if (this.radioEN.Checked)
            {
                this.btnFindAddress.Text = "Find Address";
                this.btnFindBusiness.Text = "Find Businesses";
                this.btnLocalLive.Text = "Open in local.live.com";
                this.btnLocateMe.Text = "\'Locate Me\' by IP Address";
                this.btnRemovePushPins.Text = "Remove PushPins";
                this.btnTrimCache.Text = "Trim Cached Files";
                this.cbLayerIsOn.Text = "Layer On";
                this.cbTerrain.Text = "Terrain On";
                this.groupBox1.Text = "Map Type";
                this.groupLanguage.Text = "Language";
                this.label1.Text = "data provided by Microsoft from";
                this.label10.Text = "days old";
                this.label2.Text = "a lower zoom level will render Google Maps tiles at higher altitudes";
                this.label3.Text = "Name/Category";
                this.label4.Text = "City/State";
                this.label5.Text = "Address";
                this.label6.Text = "e.g. hooters";
                this.label7.Text = "e.g. wisconsin";
                this.label8.Text = "e.g. one microsoft way, redmond";
                this.label9.Text = "Delete tiles more than";
                //    this.lblZoomLevel.Text = "8";
                this.lnkBnb.Text = "http://www.brains-N-brawn.com/veWorldWind/";
                this.lnkLocalLive.Text = "http://maps.google.com/";
                this.rbAerial.Text = "Satellite";
                this.rbDebug.Text = "Debug";
                this.rbHybrid.Text = "Hybrid";
                this.rbRoad.Text = "Map";
                this.Text = "Google Maps v0.2";
                this.txtAddress.Text = "";
                this.txtCityState.Text = "";
                this.txtNameCategory.Text = "";
                this.zoomlabel.Text = "starting zoom level :";
            }
            if (this.radioCN.Checked)
            {
                this.btnFindAddress.Text = "查找地址";
                this.btnFindBusiness.Text = "查找商业信息";
                this.btnLocalLive.Text = "在 maps.google.com 上浏览";
                this.btnLocateMe.Text = "通过 IP 地址定位";
                this.btnRemovePushPins.Text = "移除图钉标志";
                this.btnTrimCache.Text = "清理缓存的文件";
                this.cbLayerIsOn.Text = "显示图层";
                this.cbTerrain.Text = "显示地形";
                this.groupBox1.Text = " 地图类型";
                this.groupLanguage.Text = " 语言";
                this.label1.Text = "数据由 Google 提供:";
                this.label10.Text = "天的图块";
                this.label2.Text = "层数越小,开始显示 Google Maps 图块的查看高度越大";
                this.label3.Text = "名字/类别";
                this.label4.Text = "城市/州";
                this.label5.Text = "地址";
                this.label6.Text = "例如 hooters";
                this.label7.Text = "例如 wisconsin";
                this.label8.Text = "例如 one microsoft way, redmond";
                this.label9.Text = "删除日期超过";
                //    this.lblZoomLevel.Text = "8";
                this.lnkBnb.Text = "作者主页 http://www.brains-N-brawn.com/veWorldWind/";
                this.lnkLocalLive.Text = "http://maps.google.com/";
                this.rbAerial.Text = "卫星图";
                this.rbDebug.Text = "调试信息";
                this.rbHybrid.Text = "混合图";
                this.rbRoad.Text = "地图";
                this.Text = "Google Maps 0.2 版";
                this.txtAddress.Text = "";
                this.txtCityState.Text = "";
                this.txtNameCategory.Text = "";
                this.zoomlabel.Text = "    开始显示的层数 :";
            }
        }
        private void zoomlabel_Click(object sender, System.EventArgs e)
        {

        }
        private void lblZoomLevel_Click(object sender, System.EventArgs e)
        {

        }
        private void radioButton2_CheckedChanged(object sender, System.EventArgs e)
        {

        }

        public bool IsTerrainOn
        {
            get { return cbTerrain.Checked; }
        }
    }
    #endregion
    #region GoogleMapsPLUGIN
    public class GoogleMapsPlugin : WorldWind.PluginEngine.Plugin
    {
        GoogleMapsForm m_Form = null;
        MenuItem m_MenuItem;
        public override void Load()
        {
            try
            {
                if (Global.worldWindow.CurrentWorld.Name.IndexOf("Earth") >= 0)
                {
                    m_Form = new GoogleMapsForm();
                    m_MenuItem = new MenuItem("GoogleMaps");
                    m_MenuItem.Click += new EventHandler(menuItemClicked);

                    //#if DEBUG
                    string imgPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\GoogleMaps\\GoogleMaps.png";
                    //#else
                    //     _pluginDir = this.PluginDirectory;
                    //     string imgPath = this.PluginDirectory + @"\GoogleMapsPlugin.png";
                    //#endif
                    if (File.Exists(imgPath) == false)
                    {
                        Utility.Log.Write("imgPath not found " + imgPath);
                    }
                    base.Load();
                }
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
                throw;
            }
        }
        public override void Unload()
        {
            try
            {
                //remove from renderable objects
                m_Form.VeLayer.IsOn = false;
                Global.worldWindow.CurrentWorld.RenderableObjects.Remove(m_Form.VeLayer);
                m_Form.VeLayer.Dispose();
                if (m_Form != null)
                {
                    m_Form.Dispose();
                    m_Form = null;
                }
                base.Unload();
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
                throw;
            }
        }
        private void menuItemClicked(object sender, System.EventArgs e)
        {
            if (m_Form.Visible)
            {
                m_Form.Visible = false;
                m_MenuItem.Checked = false;
            }
            else
            {
                m_Form.Visible = true;
                m_MenuItem.Checked = true;
            }
        }
    }
    #endregion
    #region VEPROJECTTILESLAYER
    public class VeReprojectTilesLayer : RenderableObject
    {
        private Projection proj;
        private GoogleMapsForm veForm;
        private static double earthRadius; //6378137;
        private static double earthCircum; //40075016.685578488
        private static double earthHalfCirc; //20037508.
        private const int pixelsPerTile = 256;
        private int prevRow = -1;
        private int prevCol = -1;
        private int prevLvl = -1;
        private float prevVe = -1;
        private ArrayList veTiles = new ArrayList();
        public VeReprojectTilesLayer(string name, GoogleMapsForm veForm) : base(name)
        {
            this.name = name;
            this.veForm = veForm;
        }
        private Sprite sprite;
        private Texture spriteTexture;
        float scaleWidth = .25f;
        float scaleHeight = .25f;
        int iconWidth = 128;
        int iconHeight = 128;
        Rectangle spriteSize;
        /*
        private static int badTileSize = -1;
        public static int BadTileSize
        {
         get{return badTileSize;}
        }
        private static byte [] badTileBytes;
        public static bool IsBadTile(MemoryStream newTile)
        {
         byte [] newTileBuffer = new byte[badTileBytes.Length];
         newTile.Position = badTileSize / 2;
         newTile.Read(newTileBuffer, 0, newTileBuffer.Length);
         bool isBad = true;
         for(int i=0; i<badTileBytes.Length; i++)
         {
          byte badByte = badTileBytes[i];
          byte newByte = newTileBuffer[i];
          if(badByte != newByte)
          {
           isBad = false;
           break;
          }
         }
         newTile.Position = 0;
         return isBad;
        }
        */
        /// <summary>
        /// Layer initialization code
        /// </summary>
        public override void Initialize(DrawArgs drawArgs)
        {
            try
            {
                if (this.isInitialized == true)
                {
                    return;
                }
                //init the sprite for PushPins
                sprite = new Sprite(drawArgs.device);
                //#if DEBUG
                string spritePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\GoogleMaps\\GoogleMapsPushPin.png";
                //#else
                //    string spritePath = GoogleMapsPlugin.PluginDir + @"\GoogleMapsPushPin.png";
                //
                //#endif
                if (File.Exists(spritePath) == false)
                {
                    Utility.Log.Write("spritePath not found " + spritePath);
                }
                spriteSize = new Rectangle(0, 0, iconWidth, iconHeight);
                spriteTexture = TextureLoader.FromFile(drawArgs.device, spritePath);
                /*
                try
                {
                 //purposefully download the bad tile
                 //so it is not displayed or cached later on
                 Downloader d = new Downloader();
                 string badTileUrl = "http://r2.ortho.tiles.GoogleMaps.net/tiles/r0.png?g=1";
                 MemoryStream ms = d.DownloadImageStream(badTileUrl);
                 if(ms != null && ms.Length > 0)
                 {
                  //save off bad tile size for comparison
                  badTileSize = (int) ms.Length;
                  //save off some bytes to compare for false positives
                  ms.Position = badTileSize / 2;
                  badTileBytes = new byte[8];
                  ms.Read(badTileBytes, 0, badTileBytes.Length);
                  ms.Close();
                  ms = null;
                 }
                }
                catch(Exception ex)
                {
                 Utility.Log.Write(ex);
                }
                */
                earthRadius = Global.worldWindow.CurrentWorld.EquatorialRadius;
                earthCircum = earthRadius * 2.0 * Math.PI; //40075016.685578488
                earthHalfCirc = earthCircum / 2; //20037508.
                                                 //NOTE tiles did not line up properly with ellps=WGS84
                                                 //string [] projectionParameters = new string[]{"proj=merc", "ellps=WGS84", "no.defs"};
                                                 //+proj=longlat +ellps=sphere +a=6370997.0 +es=0.0
                string[] projectionParameters = new string[] { "proj=merc", "ellps=sphere", "a=" + earthRadius.ToString(), "es=0.0", "no.defs" };
                proj = new Projection(projectionParameters);
                //static
                VeTile.Init(this.proj, Global.worldWindow.CurrentWorld.TerrainAccessor, Global.worldWindow.CurrentWorld.EquatorialRadius, veForm);
                prevVe = World.Settings.VerticalExaggeration;
                this.isInitialized = true;
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
                throw;
            }
        }
        public string GetLocalLiveLink()
        {
            //http://local.live.com/default.aspx?v=2&cp=43.057723~-88.404224&style=r&lvl=12
            string lat = Global.worldWindow.DrawArgs.WorldCamera.Latitude.Degrees.ToString("###.#####");
            string lon = Global.worldWindow.DrawArgs.WorldCamera.Longitude.Degrees.ToString("###.#####");
            string link = "http://local.live.com/default.aspx?v=2&cp=" + lat + "~" + lon + "&styles=" + veForm.GetDataSetName() + "&lvl=" + prevLvl.ToString();
            return link;
        }
        public void RemoveAllTiles()
        {
            lock (veTiles.SyncRoot)
            {
                for (int i = 0; i < veTiles.Count; i++)
                {
                    VeTile veTile = (VeTile)veTiles[i];
                    veTile.Dispose();
                    veTiles.RemoveAt(i);
                }
                veTiles.Clear();
            }
        }
        public void ForceRefresh()
        {
            prevRow = -1;
            prevCol = -1;
            prevLvl = -1;
        }
        /// <summary>
        /// Update layer (called from worker thread)
        /// </summary>
        public override void Update(DrawArgs drawArgs)
        {
            try
            {
                if (this.isOn == false)
                {
                    return;
                }
                //NOTE for some reason Initialize is not getting called from the Plugin Menu Load/Unload
                //it does get called when the plugin loads from Startup
                //not sure what is going on, so i'll just call it manually
                if (this.isInitialized == false)
                {
                    this.Initialize(drawArgs);
                    return;
                }
                //get lat, lon
                double lat = drawArgs.WorldCamera.Latitude.Degrees;
                double lon = drawArgs.WorldCamera.Longitude.Degrees;
                //determine zoom level
                double alt = drawArgs.WorldCamera.Altitude;
                //could go off distance, but this changes when view angle changes
                //Angle fov = drawArgs.WorldCamera.Fov; //stays at 45 degress
                //Angle viewRange = drawArgs.WorldCamera.ViewRange; //off of distance, same as TVR but changes when view angle changes
                Angle tvr = drawArgs.WorldCamera.TrueViewRange; //off of altitude
                                                                //smallest altitude = 100m
                                                                //tvr = .00179663198575926
                                                                //start altitude = 12756273m
                                                                //tvr = 180
                                                                //MessageBox.Show (lat.ToString()+","+lon.ToString()+","+alt.ToString()+","+tvr.ToString());
                                                                //WW _levelZeroTileSizeDegrees
                                                                //180 90 45 22.5 11.25 5.625 2.8125 1.40625 .703125 .3515625 .17578125 .087890625 0.0439453125 0.02197265625 0.010986328125 0.0054931640625
                int zoomLevel = GetZoomLevelByTrueViewRange(tvr.Degrees);
                //dont start VE tiles until a certain zoom level
                if (zoomLevel < veForm.StartZoomLevel)
                {
                    this.RemoveAllTiles();
                    return;
                }
                //WW tiles
                //double tileDegrees = GetLevelDegrees(zoomLevel);
                //int row = MathEngine.GetRowFromLatitude(lat, tileDegrees);
                //int col = MathEngine.GetColFromLongitude(lon, tileDegrees);

                //VE tiles
                double metersY;
                double yMeters;
                int yMetersPerPixel;
                int row;
                /*
                //WRONG - doesn't stay centered away from equator
                //int yMeters = LatitudeToYAtZoom(lat, zoomLevel); //1024
                double sinLat = Math.Sin(DegToRad(lat));
                metersY = earthRadius / 2 * Math.Log((1 + sinLat) / (1 - sinLat)); //0
                yMeters = earthHalfCirc - metersY; //20037508.342789244
                yMetersPerPixel = (int) Math.Round(yMeters / MetersPerPixel(zoomLevel));
                row = yMetersPerPixel / pixelsPerTile;
                */
                //CORRECT
                //int xMeters = LongitudeToXAtZoom(lon, zoomLevel); //1024
                double metersX = earthRadius * DegToRad(lon); //0
                double xMeters = earthHalfCirc + metersX; //20037508.342789244
                int xMetersPerPixel = (int)Math.Round(xMeters / MetersPerPixel(zoomLevel));
                int col = xMetersPerPixel / pixelsPerTile;
                //reproject - overrides row above
                //this correctly keeps me on the current tile that is being viewed
                UV uvCurrent = new UV(DegToRad(lon), DegToRad(lat));
                uvCurrent = proj.Forward(uvCurrent);
                metersY = uvCurrent.V;
                yMeters = earthHalfCirc - metersY;
                yMetersPerPixel = (int)Math.Round(yMeters / MetersPerPixel(zoomLevel));
                row = yMetersPerPixel / pixelsPerTile;
                //update mesh if VertEx changes
                if (prevVe != World.Settings.VerticalExaggeration)
                {
                    lock (veTiles.SyncRoot)
                    {
                        VeTile veTile;
                        for (int i = 0; i < veTiles.Count; i++)
                        {
                            veTile = (VeTile)veTiles[i];
                            if (veTile.VertEx != World.Settings.VerticalExaggeration)
                            {
                                veTile.CreateMesh(this.Opacity, World.Settings.VerticalExaggeration);
                            }
                        }
                    }
                }
                prevVe = World.Settings.VerticalExaggeration;
                //if within previous bounds and same zoom level, then exit
                if (row == prevRow && col == prevCol && zoomLevel == prevLvl)
                {
                    return;
                }
                //System.Diagnostics.Debug.WriteLine("CHANGE");
                lock (veTiles.SyncRoot)
                {
                    VeTile veTile;
                    for (int i = 0; i < veTiles.Count; i++)
                    {
                        veTile = (VeTile)veTiles[i];
                        veTile.IsNeeded = false;
                    }
                }
                //metadata
                ArrayList alMetadata = null;
                if (veForm.IsDebug == true)
                {
                    alMetadata = new ArrayList();
                    alMetadata.Add("yMeters " + yMeters.ToString());
                    alMetadata.Add("metersY " + metersY.ToString());
                    alMetadata.Add("yMeters2 " + yMeters.ToString());
                    alMetadata.Add("vLat " + uvCurrent.V.ToString());
                    //alMetadata.Add("xMeters " + xMeters.ToString());
                    //alMetadata.Add("metersX " + metersX.ToString());
                    //alMetadata.Add("uLon " + uvCurrent.U.ToString());
                }
                //add current tiles first
                AddVeTile(drawArgs, row, col, zoomLevel, alMetadata);
                //then add other tiles outwards in surrounding circles
                AddNeighborTiles(drawArgs, row, col, zoomLevel, null, 1);
                AddNeighborTiles(drawArgs, row, col, zoomLevel, null, 2);
                AddNeighborTiles(drawArgs, row, col, zoomLevel, null, 3);

                //if(prevLvl > zoomLevel) //zooming out
                //{
                //}   
                lock (veTiles.SyncRoot)
                {
                    VeTile veTile;
                    for (int i = 0; i < veTiles.Count; i++)
                    {
                        veTile = (VeTile)veTiles[i];
                        if (veTile.IsNeeded == false)
                        {
                            veTile.Dispose();
                            veTiles.RemoveAt(i);
                        }
                    }
                }
                prevRow = row;
                prevCol = col;
                prevLvl = zoomLevel;
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        private void AddNeighborTiles(DrawArgs drawArgs, int row, int col, int zoomLevel, ArrayList alMetadata, int range)
        {
            int minRow = row - range;
            int maxRow = row + range;
            int minCol = col - range;
            int maxCol = col + range;
            for (int i = minRow; i <= maxRow; i++)
            {
                for (int j = minCol; j <= maxCol; j++)
                {
                    //only outer edges, inner tiles should already be added
                    if (i == minRow || i == maxRow || j == minCol || j == maxCol)
                    {
                        AddVeTile(drawArgs, i, j, zoomLevel, alMetadata);
                    }
                }
            }
        }
        private void AddVeTile(DrawArgs drawArgs, int row, int col, int zoomLevel, ArrayList alMetadata)
        {
            //TODO handle column wrap-around
            //haven't had to explicitly handle this yet
            bool tileFound = false;
            lock (veTiles.SyncRoot)
            {
                foreach (VeTile veTile in veTiles)
                {
                    if (veTile.IsNeeded == true)
                    {
                        continue;
                    }
                    if (veTile.IsEqual(row, col, zoomLevel) == true)
                    {
                        veTile.IsNeeded = true;
                        tileFound = true;
                        break;
                    }
                }
            }
            if (tileFound == false)
            {
                //exit if zoom level has changed
                int curZoomLevel = GetZoomLevelByTrueViewRange(drawArgs.WorldCamera.TrueViewRange.Degrees);
                if (curZoomLevel != zoomLevel)
                {
                    return;
                }
                VeTile newVeTile = CreateVeTile(drawArgs, row, col, zoomLevel, alMetadata);
                newVeTile.IsNeeded = true;
                lock (veTiles.SyncRoot)
                {
                    veTiles.Add(newVeTile);
                }
            }
        }
        private VeTile CreateVeTile(DrawArgs drawArgs, int row, int col, int zoomLevel, ArrayList alMetadata)
        {
            VeTile newVeTile = new VeTile(row, col, zoomLevel);
            //metadata
            if (alMetadata != null)
            {
                foreach (string metadata in alMetadata)
                {
                    newVeTile.AddMetaData(metadata);
                }
            }
            //thread to download new tile(s) or just load from cache
            newVeTile.GetTexture(drawArgs, pixelsPerTile);
            //handle the diff projection
            double metersPerPixel = MetersPerPixel(zoomLevel);
            double totalTilesPerEdge = Math.Pow(2, zoomLevel);
            double totalMeters = totalTilesPerEdge * pixelsPerTile * metersPerPixel;
            double halfMeters = totalMeters / 2;
            //do meters calculation in VE space
            //the 0,0 origin for VE is in upper left
            double N = row * (pixelsPerTile * metersPerPixel);
            double W = col * (pixelsPerTile * metersPerPixel);
            //now convert it to +/- meter coordinates for Proj.4
            //the 0,0 origin for Proj.4 is 0 lat, 0 lon
            //-22 to 22 million, -11 to 11 million
            N = halfMeters - N;
            W = W - halfMeters;
            double E = W + (pixelsPerTile * metersPerPixel);
            double S = N - (pixelsPerTile * metersPerPixel);
            newVeTile.UL = new UV(W, N);
            newVeTile.UR = new UV(E, N);
            newVeTile.LL = new UV(W, S);
            newVeTile.LR = new UV(E, S);
            //create mesh
            byte opacity = this.Opacity; //from RenderableObject
            float verticalExaggeration = World.Settings.VerticalExaggeration;
            newVeTile.CreateMesh(opacity, verticalExaggeration);
            newVeTile.CreateDownloadRectangle(drawArgs, World.Settings.DownloadProgressColor.ToArgb());
            return newVeTile;
        }
        private static double MetersPerTile(int zoom)
        {
            return MetersPerPixel(zoom) * pixelsPerTile;
        }
        private static double MetersPerPixel(int zoom)
        {
            double arc;
            arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            return arc;
        }
        private static double DegToRad(double d)
        {
            return d * Math.PI / 180.0;
        }
        private static double RadToDeg(double d)
        {
            return d * 180 / Math.PI;
        }
        public double GetLevelDegrees(int level)
        {
            double metersPerPixel = MetersPerPixel(level);
            double arcDistance = metersPerPixel * pixelsPerTile;
            //double arcDistance = earthCircum * (tileRange / 360);
            double tileRange = (arcDistance / earthCircum) * 360;
            return tileRange;
        }
        public int GetZoomLevelByTrueViewRange(double trueViewRange)
        {
            int maxLevel = 3;
            int minLevel = 19;
            int numLevels = minLevel - maxLevel + 1;
            int retLevel = maxLevel;
            for (int i = 0; i < numLevels; i++)
            {
                retLevel = i + maxLevel;
                double viewAngle = 180;
                for (int j = 0; j < i; j++)
                {
                    viewAngle = viewAngle / 2.0;
                }
                if (trueViewRange >= viewAngle)
                {
                    break;
                }
            }
            return retLevel;
        }
        public int GetZoomLevelByArcDistance(double arcDistance)
        {
            //arcDistance in meters
            int totalLevels = 24;
            int level = 0;
            for (level = 1; level <= totalLevels; level++)
            {
                double metersPerPixel = MetersPerPixel(level);
                double totalDistance = metersPerPixel * pixelsPerTile;
                if (arcDistance > totalDistance)
                {
                    break;
                }
            }
            return level - 1;
        }
        private int LatitudeToYAtZoom(double lat, int zoom)
        {
            int y;
            //code VE Mobile v1 - NO LONGER VALID
            //double sinLat = Math.Sin(DegToRad(lat));
            //double metersY = 6378137 / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            //y = (int)Math.Round((20971520 - metersY) / MetersPerPixel(zoom));
            //forum - SKIPS TILES THE FURTHER YOU GET FROM EQUATOR
            double arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            double sinLat = Math.Sin(DegToRad(lat));
            double metersY = earthRadius / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            y = (int)Math.Round((earthHalfCirc - metersY) / arc);
            //HACK - THIS HANDLES THE SKIPPING OF TILES THE FURTHER YOU GET FROM EQUATOR
            //double arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            //double metersY = earthRadius * DegToRad(lat);
            //y = (int) Math.Round((earthHalfCirc - metersY) / arc);
            return y;
        }
        private int LongitudeToXAtZoom(double lon, int zoom)
        {
            int x;
            double arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            double metersX = earthRadius * DegToRad(lon);
            x = (int)Math.Round((earthHalfCirc + metersX) / arc);
            return x;
        }
        /// <summary>
        /// Draws the layer
        /// </summary>
        public override void Render(DrawArgs drawArgs)
        {
            try
            {
                if (this.isOn == false)
                {
                    return;
                }
                if (this.isInitialized == false)
                {
                    return;
                }
                if (drawArgs.device == null)
                    return;
                if (veTiles != null && veTiles.Count > 0)
                {
                    //render mesh and tile(s)
                    bool disableZBuffer = false; //TODO where do i get this setting
                    //foreach(VeTile veTile in veTiles)
                    //{
                    // veTile.Render(drawArgs, disableZBuffer);
                    //}
                    // camera jitter fix
                    drawArgs.device.Transform.World = Matrix.Translation(
                           (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                    );
                    VeTile.Render(drawArgs, disableZBuffer, veTiles);
                    //camera jitter fix
                    drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
                }
                //else pushpins only
                //render PushPins
                if (pushPins != null && pushPins.Count > 0)
                {
                    RenderPushPins(drawArgs);
                }
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }
        public void GetViewPort(DrawArgs drawArgs, out double lat1, out double lon1, out double lat2, out double lon2)
        {
            double halfViewRange = drawArgs.WorldCamera.TrueViewRange.Degrees / 2;
            double lat = drawArgs.WorldCamera.Latitude.Degrees;
            double lon = drawArgs.WorldCamera.Longitude.Degrees;
            lat1 = lat + halfViewRange;
            lon1 = lon + halfViewRange;
            lat2 = lat - halfViewRange;
            lon2 = lon - halfViewRange;
        }
        private ArrayList pushPins = null;
        public ArrayList PushPins
        {
            get { return pushPins; }
            set { pushPins = value; }
        }
        //private Icons pushPins = null;
        //public Icons PushPins
        //{
        // get{return pushPins;}
        // set{pushPins = value;}
        //}
        public void RenderPushPins(DrawArgs drawArgs)
        {
            if (pushPins == null || pushPins.Count <= 0)
                return;
            //pushPins.Initialize(drawArgs);
            //pushPins.Render(drawArgs);
            double lat1, lon1, lat2, lon2;
            GetViewPort(drawArgs, out lat1, out lon1, out lat2, out lon2);
            Vector3 projectedPoint;
            lock (pushPins.SyncRoot)
            {
                foreach (PushPin p in pushPins)
                {
                    if (p.Latitude <= lat1 && p.Latitude >= lat2)
                    {
                        if (p.Longitude <= lon1 && p.Longitude >= lon2)
                        {
                            projectedPoint = MathEngine.SphericalToCartesian((float)p.Latitude, (float)p.Longitude, (float)earthRadius + 100);
                            projectedPoint.Project(drawArgs.device.Viewport, drawArgs.WorldCamera.ProjectionMatrix, drawArgs.WorldCamera.ViewMatrix, drawArgs.WorldCamera.WorldMatrix);
                            sprite.Begin(SpriteFlags.AlphaBlend);
                            sprite.Transform = Matrix.Transformation2D(new Vector2(0.0f, 0.0f),
                             0.0f, new Vector2(scaleWidth, scaleHeight),
                             new Vector2(0, 0), 0.0f, new Vector2(projectedPoint.X, projectedPoint.Y));
                            sprite.Draw(spriteTexture, spriteSize, new Vector3(.5f * iconWidth, .5f * iconHeight, 0), new Vector3(0, 0, 0), System.Drawing.Color.White);
                            sprite.End();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Cleanup when layer is disabled
        /// </summary>
        public override void Dispose()
        {
            RemoveAllTiles();
            if (sprite != null)
            {
                sprite.Dispose();
                sprite = null;
            }
        }
        /// <summary>
        /// Handle mouse click
        /// </summary>
        /// <returns>true if click was handled.</returns>
        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }
    }
    #endregion

    #region VESETTINGS
    /// <summary>
    /// This class stores virtual earth settings
    /// </summary>
    [Serializable]
    public class VESettings
    {
        /// <summary>
        /// Layer Zoom Level
        /// </summary>
        private int zoomlevel = 8;
        /// <summary>
        /// Turn layer on
        /// </summary>
        private bool layeron = true;
        /// <summary>
        /// Turn terrain on
        /// </summary>
        private bool terrain = true;
        /// <summary>
        /// Layer types
        /// </summary>
        private bool road = true;
        private bool aerial = false;
        private bool hybrid = false;
        private bool debug = false;

        public int ZoomLevel
        {
            get
            {
                return zoomlevel;
            }
            set
            {
                zoomlevel = value;
            }
        }
        public bool LayerOn
        {
            get
            {
                return layeron;
            }
            set
            {
                layeron = value;
            }
        }
        public bool Terrain
        {
            get
            {
                return terrain;
            }
            set
            {
                terrain = value;
            }
        }
        public bool Road
        {
            get
            {
                return road;
            }
            set
            {
                road = value;
            }
        }
        public bool Aerial
        {
            get
            {
                return aerial;
            }
            set
            {
                aerial = value;
            }
        }
        public bool Hybrid
        {
            get
            {
                return hybrid;
            }
            set
            {
                hybrid = value;
            }
        }
        public bool Debug
        {
            get
            {
                return debug;
            }
            set
            {
                debug = value;
            }
        }
        /// <summary>
        /// Loads a serialized instance of the settings from the specified file
        /// returns default values if the file doesn't exist or an error occurs
        /// </summary>
        /// <returns>The persisted settings from the file</returns>
        public static VESettings LoadSettingsFromFile(string filename)
        {
            VESettings settings;
            XmlSerializer xs = new XmlSerializer(typeof(VESettings));
            if (File.Exists(filename))
            {
                FileStream fs = null;
                try
                {
                    fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                }
                catch
                {
                    return new VESettings();
                }
                try
                {
                    settings = (VESettings)xs.Deserialize(fs);
                }
                catch
                {
                    settings = new VESettings();
                }
                finally
                {
                    fs.Close();
                }
            }
            else
            {
                settings = new VESettings();
            }
            return settings;
        }
        /// <summary>
        /// Persists the settings to the specified filename
        /// </summary>
        /// <param name="file">The filename to use for saving</param>
        /// <param name="settings">The instance of the Settings class to persist</param>
        public static void SaveSettingsToFile(string file, VESettings settings)
        {
            FileStream fs = null;
            XmlSerializer xs = new XmlSerializer(typeof(VESettings));
            fs = File.Open(file, FileMode.Create, FileAccess.Write);
            try
            {
                xs.Serialize(fs, settings);
            }
            finally
            {
                fs.Close();
            }
        }
    }
    #endregion
    #region VETILE
    public class VeTile : IDisposable
    {
        //these are the coordinate extents for the tile
        UV m_ul, m_ur, m_ll, m_lr;
        /// <summary>
        /// Coordinates at upper left edge of image
        /// </summary>
        public UV UL
        {
            get { return m_ul; }
            set { m_ul = value; }
        }
        /// <summary>
        /// Coordinates at upper right edge of image
        /// </summary>
        public UV UR
        {
            get { return m_ur; }
            set { m_ur = value; }
        }
        /// <summary>
        /// Coordinates at lower left edge of image
        /// </summary>
        public UV LL
        {
            get { return m_ll; }
            set { m_ll = value; }
        }
        /// <summary>
        /// Coordinates at lower right edge of image
        /// </summary>
        public UV LR
        {
            get { return m_lr; }
            set { m_lr = value; }
        }
        //store the Vertical Exaggeration for when the mesh was created
        //so when the VerticalExaggeration setting changes, it know which meshes to recreate
        private float vertEx;
        public float VertEx
        {
            get { return vertEx; }
        }
        private static Projection _proj;
        private static double _layerRadius;
        private static TerrainAccessor _terrainAccessor;
        private static System.Drawing.Font _font;
        private static Brush _brush;
        private static GoogleMapsForm _veForm;
        public static void Init(Projection proj, TerrainAccessor terrainAccessor, double layerRadius, GoogleMapsForm veForm)
        {
            _proj = proj;
            _terrainAccessor = terrainAccessor;
            _layerRadius = layerRadius;
            _veForm = veForm;
            _font = new System.Drawing.Font("Verdana", 15, FontStyle.Bold);
            _brush = new SolidBrush(Color.Green);
        }
        //flag for if the tile should be disposed
        private bool isNeeded = true;
        public bool IsNeeded
        {
            get { return isNeeded; }
            set { isNeeded = value; }
        }
        public bool IsEqual(int row, int col, int level)
        {
            bool retVal = false;
            if (this.row == row && this.col == col && this.level == level)
            {
                retVal = true;
            }
            return retVal;
        }
        private int row;
        private int col;
        private int level;
        public VeTile(int row, int col, int level)
        {
            this.row = row;
            this.col = col;
            this.level = level;
        }
        private Texture texture = null;
        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        private ArrayList alMetaData = new ArrayList();
        private WebDownload download;
        public float ProgressPercent;
        private string textureName;
        private DrawArgs drawArgs;
        //private Downloader downloader;
        public void GetTexture(DrawArgs drawArgs, int pixelsPerTile)
        {
            this.drawArgs = drawArgs;
            string _datasetName = _veForm.GetDataSetName();
            string _imageExtension = _veForm.GetImageExtension();
            string _serverUri = ".ortho.tiles.GoogleMaps.net/tiles/";
            string quadKey = TileToQuadKey(col, row, level);
            string geurl1 = quadKey;//gemap
            geurl1 = geurl1.Replace("0", "q");
            geurl1 = geurl1.Replace("1", "r");
            geurl1 = geurl1.Replace("2", "t");
            geurl1 = geurl1.Replace("3", "s");
            string geMapS = @"ge\t" + geurl1 + ".jpg";
            string geHttpUrlS = "http://kh3.google.com/kh?v=7&t=t" + geurl1;
            //TODO no clue what ?g= is
            // string textureUrl = String.Concat(new object[] { "http://", _datasetName, quadKey[quadKey.Length - 1], _serverUri, _datasetName, quadKey, ".", _imageExtension, "?g=", 15 });
            string textureUrl = geHttpUrlS;

            if (_veForm.IsDebug == true)
            {
                //generate a DEBUG tile with metadata
                MemoryStream ms;
                //debug
                Bitmap b = new Bitmap(pixelsPerTile, pixelsPerTile);
                System.Drawing.Imaging.ImageFormat imageFormat;
                //could download on my own from here and add metadata to the images before storing to cache
                //Bitmap b = DownloadImage(url);
                //string levelDir = CreateLevelDir(level);
                //string rowDir = CreateRowDir(levelDir, row);
                //alMetaData.Add("wwLevel : " + level.ToString());
                alMetaData.Add("ww rowXcol : " + row.ToString() + "x" + col.ToString());
                //alMetaData.Add("wwArcDist : " + arcDistance.ToString());
                //alMetaData.Add("tileRange : " + tileRange.ToString());
                //alMetaData.Add("latXlon : " + lat.ToString("###.###") + "x" + lon.ToString("###.###"));
                //alMetaData.Add("lat : " + lat.ToString());
                //alMetaData.Add("lon : " + lon.ToString());
                alMetaData.Add("Google maps Level : " + level.ToString());
                //alMetaData.Add("ve rowXcol : " + t_x.ToString() + "x" + t_y.ToString());
                //alMetaData.Add("veArcDist : " + tileDistance.ToString());
                //alMetaData.Add("sinLat : " + sinLat.ToString());
                //    string geurl2 = quadKey;//gemap
                //    geurl2 = geurl2.Replace("0","q");
                //    geurl2 = geurl2.Replace("1","r");
                //    geurl2 = geurl2.Replace("2","t");
                //    geurl2 = geurl2.Replace("3","s");
                alMetaData.Add("quadKey t" + geurl1.ToString());
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                b = DecorateBitmap(b, _font, _brush, alMetaData);
                //SaveBitmap(b, rowDir, row, col, _imageExtension, b.RawFormat); //, System.Drawing.Imaging.ImageFormat.Jpeg
                //url = String.Empty;
                ms = new MemoryStream();
                b.Save(ms, imageFormat);
                ms.Position = 0;
                this.texture = TextureLoader.FromStream(drawArgs.device, ms);
                ms.Close();
                ms = null;
                b.Dispose();
                b = null;
            }
            else
            {
                //load a tile from file OR download it if not cached
                string levelDir = CreateLevelDir(level, _veForm.WorldWindow.Cache.CacheDirectory);
                string mapTypeDir = CreateMapTypeDir(levelDir, _datasetName);
                string rowDir = CreateRowDir(mapTypeDir, row);
                textureName = String.Empty; //= GetTextureName(rowDir, row, col, "dds");
                if (_datasetName == "r")
                {
                    textureName = GetTextureName(rowDir, row, col, "png");
                }
                else
                {
                    textureName = GetTextureName(rowDir, row, col, "jpg");
                }
                if (File.Exists(textureName) == true)
                {
                    this.texture = TextureLoader.FromFile(drawArgs.device, textureName);
                }
                else //download it
                {
                    /*
                    //use WebDownload instead
                    downloader = new Downloader();
                    downloader.drawArgs = drawArgs;
                    downloader.textureName = textureName;
                    downloader.textureUrl = textureUrl;
                    downloader.veTile = this;
                    downloader.mapType = _datasetName;
                    ThreadStart ts = new ThreadStart(downloader.DownloadThread);
                    Thread t = new Thread(ts);
                    t.IsBackground = true;
                    t.Start();
                    */
                    download = new WebDownload(textureUrl);
                    download.DownloadType = DownloadType.Unspecified;
                    download.SavedFilePath = textureName + ".tmp"; //?
                    download.ProgressCallback += new DownloadProgressHandler(UpdateProgress);
                    download.CompleteCallback += new DownloadCompleteHandler(DownloadComplete);
                    download.BackgroundDownloadFile();
                }
            }
        }
        void UpdateProgress(int pos, int total)
        {
            if (total == 0)
            {
                // When server doesn't provide content-length, 
                //use this dummy value to at least show some progress.
                total = 50 * 1024;
            }
            pos = pos % (total + 1);
            ProgressPercent = (float)pos / total;
        }
        private void DownloadComplete(WebDownload downloadInfo)
        {
            try
            {
                downloadInfo.Verify();
                //m_quadTile.QuadTileArgs.NumberRetries = 0;
                //TODO add back in logic to check for the no data tile?
                //the logic was to not display that tile to at least show some data for the layers beneath VE
                //or show the no data tile so they know that VE has not covered that area yet
                //then just let those tiles get periodically deleted from cache for when VE updates
                // Rename temp file to real name
                File.Delete(textureName);
                File.Move(downloadInfo.SavedFilePath, textureName);
                // Make the quad tile reload the new image
                //m_quadTile.DownloadRequest = null;
                //m_quadTile.isInitialized = false;
                this.texture = TextureLoader.FromFile(drawArgs.device, textureName);
            }
            catch (System.Net.WebException caught)
            {
                System.Net.HttpWebResponse response = caught.Response as System.Net.HttpWebResponse;
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    using (File.Create(textureName + ".txt"))
                    { }
                    return;
                }
                //m_quadTile.QuadTileArgs.NumberRetries++;
            }
            catch
            {
                using (File.Create(textureName + ".txt"))
                { }
                if (File.Exists(downloadInfo.SavedFilePath))
                    File.Delete(downloadInfo.SavedFilePath);
            }
            finally
            {
                download.IsComplete = true;
                //m_quadTile.QuadTileArgs.RemoveFromDownloadQueue(this);
                //Immediately queue next download
                //m_quadTile.QuadTileArgs.ServiceDownloadQueue();
            }
        }
        public void AddMetaData(string metadata)
        {
            alMetaData.Add(metadata);
        }
        //for generating the debug bitmap
        public Bitmap DecorateBitmap(Bitmap b, System.Drawing.Font font, Brush brush, ArrayList alMetadata)
        {
            if (alMetadata.Count > 0)
            {
                //if(b.RawFormat == System.Drawing.Imaging.ImageFormat.Png)
                if (b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    MemoryStream ms = new MemoryStream();
                    b.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    b.Dispose();
                    b = null;
                    b = new Bitmap(256, 256);
                    b = (Bitmap)Bitmap.FromStream(ms);
                    ms.Close();
                    ms = null;
                }
                Graphics g = Graphics.FromImage(b); //fails for png files
                g.Clear(Color.White);
                g.DrawLine(Pens.Red, 0, 0, b.Width, 0);
                g.DrawLine(Pens.Red, 0, 0, 0, b.Height);
                string s = (string)alMetadata[0];
                SizeF sizeF = g.MeasureString(s, font);
                for (int i = 0; i < alMetadata.Count; i++)
                {
                    s = (string)alMetadata[i];
                    int x = 0;
                    int y = (int)(sizeF.Height * (i + 0));
                    g.DrawString(s, font, brush, x, y);
                }
                g.Dispose();
            }
            return b;
        }
        //convert VE row, col, level into key for URL
        private static string TileToQuadKey(int tx, int ty, int zl)
        {
            string quad;
            quad = "";
            for (int i = zl; i > 0; i--)
            {
                int mask = 1 << (i - 1);
                int cell = 0;
                if ((tx & mask) != 0)
                {
                    cell++;
                }
                if ((ty & mask) != 0)
                {
                    cell += 2;
                }
                quad += cell;
            }
            return quad;
        }
        public string CreateLevelDir(int level, string cacheDirectoryRoot)
        {
            string levelDir = null;
            //GoogleMaps.m_WorldWindow.Cache.CacheDirectory
            string cacheDirectory = String.Format("{0}\\GoogleMaps", cacheDirectoryRoot);
            if (Directory.Exists(cacheDirectory) == false)
            {
                Directory.CreateDirectory(cacheDirectory);
            }
            levelDir = cacheDirectory + @"\" + level.ToString();
            if (Directory.Exists(levelDir) == false)
            {
                Directory.CreateDirectory(levelDir);
            }
            return levelDir;
        }
        public string CreateMapTypeDir(string levelDir, string mapType)
        {
            string mapTypeDir = levelDir + @"\" + mapType;
            if (Directory.Exists(mapTypeDir) == false)
            {
                Directory.CreateDirectory(mapTypeDir);
            }
            return mapTypeDir;
        }
        public string CreateRowDir(string mapTypeDir, int row)
        {
            string rowDir = mapTypeDir + @"\" + row.ToString("0000");
            if (Directory.Exists(rowDir) == false)
            {
                Directory.CreateDirectory(rowDir);
            }
            return rowDir;
        }
        public string GetTextureName(string rowDir, int row, int col, string textureExtension)
        {
            string textureName = rowDir + @"\" + row.ToString("0000") + "_" + col.ToString("0000") + "." + textureExtension;
            return textureName;
        }
        public void SaveBitmap(Bitmap b, string rowDir, int row, int col, string imageExtension, System.Drawing.Imaging.ImageFormat format)
        {
            string bmpName = rowDir + @"\" + row.ToString("0000") + "_" + col.ToString("0000") + "." + imageExtension;
            b.Save(bmpName, format);
            //b.Save(bmpName); //, format
        }
        public void Reproject()
        {
            //TODO refactor from the VeLayer class
        }
        protected CustomVertex.PositionColoredTextured[] vertices;
        public CustomVertex.PositionColoredTextured[] Vertices
        {
            get { return vertices; }
        }
        protected short[] indices;
        public short[] Indices
        {
            get { return indices; }
        }
        protected int meshPointCount = 64;
        private double North;
        private double South;
        private double West;
        private double East;
        //NOTE this is a mix from Mashi's Reproject and WW for terrain
        public void CreateMesh(byte opacity, float verticalExaggeration)
        {
            this.vertEx = verticalExaggeration;
            int opacityColor = System.Drawing.Color.FromArgb(opacity, 0, 0, 0).ToArgb();
            meshPointCount = 32; //64; //96 // How many vertices for each direction in mesh (total: n^2)
            vertices = new CustomVertex.PositionColoredTextured[meshPointCount * meshPointCount];
            int upperBound = meshPointCount - 1;
            float scaleFactor = (float)1 / upperBound;
            //using(Projection proj = new Projection(m_projectionParameters))
            //{
            double uStep = (UR.U - UL.U) / upperBound;
            double vStep = (UL.V - LL.V) / upperBound;
            UV curUnprojected = new UV(UL.U, UL.V);
            // figure out latrange (for terrain detail)
            UV geoUL = _proj.Inverse(m_ul);
            UV geoLR = _proj.Inverse(m_lr);
            double latRange = (geoUL.U - geoLR.U) * 180 / Math.PI;
            North = geoUL.V * 180 / Math.PI;
            South = geoLR.V * 180 / Math.PI;
            West = geoUL.U * 180 / Math.PI;
            East = geoLR.U * 180 / Math.PI;
            float meshBaseRadius = (float)_layerRadius;
            float[,] heightData = null;
            if (_terrainAccessor != null && _veForm.IsTerrainOn == true)
            {
                //does the +1 to help against tears between elevated tiles? - made it worse
                //TODO not sure how to fix the tear between tiles caused by elevation?
                TerrainTile tile = _terrainAccessor.GetElevationArray(North, South, West, East, meshPointCount);
                heightData = tile.ElevationData;
                tile.Dispose();
                tile = null;
                /*
                // Calculate mesh base radius (bottom vertices)
                float minimumElevation = float.MaxValue;
                float maximumElevation = float.MinValue;

                // Find minimum elevation to account for possible bathymetry
                foreach(float _height in heightData)
                {
                 if(_height < minimumElevation)
                  minimumElevation = _height;
                 if(_height > maximumElevation)
                  maximumElevation = _height;
                }
                minimumElevation *= verticalExaggeration;
                maximumElevation *= verticalExaggeration;
                if(minimumElevation > maximumElevation)
                {
                 // Compensate for negative vertical exaggeration
                 float tmp = minimumElevation;
                 minimumElevation = maximumElevation;
                 maximumElevation = minimumElevation;
                }
                float overlap = 500 * verticalExaggeration; // 500m high tiles

                // Radius of mesh bottom grid
                meshBaseRadius = (float) _layerRadius + minimumElevation - overlap;
                */
            }
            UV geo;
            Vector3 pos;
            double height = 0;
            for (int i = 0; i < meshPointCount; i++)
            {
                for (int j = 0; j < meshPointCount; j++)
                {
                    geo = _proj.Inverse(curUnprojected);

                    // Radians -> Degrees
                    geo.U *= 180 / Math.PI;
                    geo.V *= 180 / Math.PI;
                    if (_terrainAccessor != null)
                    {
                        if (_veForm.IsTerrainOn == true)
                        {
                            height = heightData[i, j] * verticalExaggeration;
                        }
                        else
                        {
                            //original
                            height = verticalExaggeration * _terrainAccessor.GetElevationAt(geo.V, geo.U, upperBound / latRange);
                        }
                    }
                    pos = MathEngine.SphericalToCartesian(
                     geo.V,
                     geo.U,
                     _layerRadius + height);
                    vertices[i * meshPointCount + j].X = pos.X;
                    vertices[i * meshPointCount + j].Y = pos.Y;
                    vertices[i * meshPointCount + j].Z = pos.Z;
                    //double sinLat = Math.Sin(geo.V);
                    //vertices[i*meshPointCount + j].Z = (float) (pos.Z * sinLat);

                    vertices[i * meshPointCount + j].Tu = j * scaleFactor;
                    vertices[i * meshPointCount + j].Tv = i * scaleFactor;
                    vertices[i * meshPointCount + j].Color = opacityColor;
                    curUnprojected.U += uStep;
                }
                curUnprojected.U = UL.U;
                curUnprojected.V -= vStep;
            }
            //}
            indices = new short[2 * upperBound * upperBound * 3];
            for (int i = 0; i < upperBound; i++)
            {
                for (int j = 0; j < upperBound; j++)
                {
                    indices[(2 * 3 * i * upperBound) + 6 * j] = (short)(i * meshPointCount + j);
                    indices[(2 * 3 * i * upperBound) + 6 * j + 1] = (short)((i + 1) * meshPointCount + j);
                    indices[(2 * 3 * i * upperBound) + 6 * j + 2] = (short)(i * meshPointCount + j + 1);

                    indices[(2 * 3 * i * upperBound) + 6 * j + 3] = (short)(i * meshPointCount + j + 1);
                    indices[(2 * 3 * i * upperBound) + 6 * j + 4] = (short)((i + 1) * meshPointCount + j);
                    indices[(2 * 3 * i * upperBound) + 6 * j + 5] = (short)((i + 1) * meshPointCount + j + 1);
                }
            }
        }
        public void Dispose()
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
            if (download != null)
            {
                download.Dispose();
                download = null;
            }
            if (vertices != null)
            {
                vertices = null;
            }
            if (indices != null)
            {
                indices = null;
            }
            if (downloadRectangle != null)
            {
                downloadRectangle = null;
            }
            GC.SuppressFinalize(this);
        }
        CustomVertex.PositionColored[] downloadRectangle = new CustomVertex.PositionColored[5];
        public static void Render(DrawArgs drawArgs, bool disableZbuffer, ArrayList alVeTiles)
        {
            try
            {
                if (alVeTiles.Count <= 0)
                    return;
                lock (alVeTiles.SyncRoot)
                {
                    //setup device to render textures
                    if (disableZbuffer)
                    {
                        if (drawArgs.device.RenderState.ZBufferEnable)
                            drawArgs.device.RenderState.ZBufferEnable = false;
                    }
                    else
                    {
                        if (!drawArgs.device.RenderState.ZBufferEnable)
                            drawArgs.device.RenderState.ZBufferEnable = true;
                    }
                    drawArgs.device.VertexFormat = CustomVertex.PositionColoredTextured.Format;
                    drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Add;
                    drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                    //save index to tiles not downloaded yet
                    int notDownloadedIter = 0;
                    int[] notDownloaded = new int[alVeTiles.Count];
                    //render tiles that are downloaded
                    VeTile veTile;
                    for (int i = 0; i < alVeTiles.Count; i++)
                    {
                        veTile = (VeTile)alVeTiles[i];
                        if (veTile.Texture == null) //not downloaded yet
                        {
                            notDownloaded[notDownloadedIter] = i;
                            notDownloadedIter++;
                            continue;
                        }
                        else
                        {
                            //NOTE to stop ripping?
                            drawArgs.device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
                            drawArgs.device.SetTexture(0, veTile.Texture);
                            drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                             veTile.Vertices.Length, veTile.Indices.Length / 3, veTile.Indices, true, veTile.Vertices);
                        }
                    }
                    //now render the downloading tiles
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
                    int tileIndex;
                    for (int i = 0; i < notDownloadedIter; i++)
                    {
                        tileIndex = notDownloaded[i];
                        veTile = (VeTile)alVeTiles[tileIndex];
                        //TODO render progress bar indicator too?
                        veTile.RenderDownloadRectangle(drawArgs);
                    }
                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                    drawArgs.device.VertexFormat = CustomVertex.PositionTextured.Format;
                    drawArgs.device.RenderState.ZBufferEnable = true;
                }
            }
            catch (Exception ex)
            {
                string sex = ex.ToString();
                Utility.Log.Write(ex);
            }
            finally
            {
                if (disableZbuffer)
                    drawArgs.device.RenderState.ZBufferEnable = true;
            }
        }
        public void CreateDownloadRectangle(DrawArgs drawArgs, int color)
        {
            // Render terrain download rectangle
            Vector3 northWestV = MathEngine.SphericalToCartesian((float)North, (float)West, _layerRadius);
            Vector3 southWestV = MathEngine.SphericalToCartesian((float)South, (float)West, _layerRadius);
            Vector3 northEastV = MathEngine.SphericalToCartesian((float)North, (float)East, _layerRadius);
            Vector3 southEastV = MathEngine.SphericalToCartesian((float)South, (float)East, _layerRadius);
            downloadRectangle[0].X = northWestV.X;
            downloadRectangle[0].Y = northWestV.Y;
            downloadRectangle[0].Z = northWestV.Z;
            downloadRectangle[0].Color = color;
            downloadRectangle[1].X = southWestV.X;
            downloadRectangle[1].Y = southWestV.Y;
            downloadRectangle[1].Z = southWestV.Z;
            downloadRectangle[1].Color = color;
            downloadRectangle[2].X = southEastV.X;
            downloadRectangle[2].Y = southEastV.Y;
            downloadRectangle[2].Z = southEastV.Z;
            downloadRectangle[2].Color = color;
            downloadRectangle[3].X = northEastV.X;
            downloadRectangle[3].Y = northEastV.Y;
            downloadRectangle[3].Z = northEastV.Z;
            downloadRectangle[3].Color = color;
            downloadRectangle[4].X = downloadRectangle[0].X;
            downloadRectangle[4].Y = downloadRectangle[0].Y;
            downloadRectangle[4].Z = downloadRectangle[0].Z;
            downloadRectangle[4].Color = color;
        }
        public void RenderDownloadRectangle(DrawArgs drawArgs)
        {
            drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, downloadRectangle);
        }
    }
    #endregion

    #region SEARCH
    //from Jason Fuller's VE Mobile, via Reflector
    //only slightly modified to use my basic PushPin structure
    public class Search
    {
        private Search()
        {
        }
        private static string DoSearchRequest(string searchParams)
        {
            string text1 = string.Empty;
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create("http://local.live.com/search.ashx");
            request1.Method = "POST";
            request1.ContentType = "application/x-www-form-urlencoded";
            UTF8Encoding encoding1 = new UTF8Encoding();
            byte[] buffer1 = encoding1.GetBytes(searchParams);
            request1.ContentLength = buffer1.Length;
            try
            {
                Stream stream1 = request1.GetRequestStream();
                stream1.Write(buffer1, 0, buffer1.Length);
                stream1.Close();
                stream1 = null;
                text1 = Search.GetSearchResults(request1);
            }
            catch (WebException)
            {
            }
            return text1;
        }
        internal static string GetSearchResults(HttpWebRequest searchRequest)
        {
            string text1 = string.Empty;
            HttpWebResponse response1 = (HttpWebResponse)searchRequest.GetResponse();
            Cursor.Current = Cursors.WaitCursor;
            Stream stream1 = response1.GetResponseStream();
            Cursor.Current = Cursors.Default;
            Encoding encoding1 = Encoding.GetEncoding("utf-8");
            StreamReader reader1 = new StreamReader(stream1, encoding1);
            char[] chArray1 = new char[0x100];
            for (int num1 = reader1.Read(chArray1, 0, 0x100); num1 > 0; num1 = reader1.Read(chArray1, 0, 0x100))
            {
                string text2 = new string(chArray1, 0, num1);
                text1 = text1 + text2;
            }
            reader1.Close();
            reader1 = null;
            response1.Close();
            response1 = null;
            return text1;
        }
        public static bool SearchForAddress(string address, out double lat1, out double long1, out double lat2, out double long2)
        {
            double num1;
            long2 = num1 = 0;
            long1 = num1 = num1;
            lat2 = num1 = num1;
            lat1 = num1;
            string text1 = "a=&b=" + address + "&c=0.0&d=0.0&e=0.0&f=0.0&g=&i=&r=0";
            string text2 = Search.DoSearchRequest(text1);
            if ((text2 == null) || (text2 == string.Empty))
            {
                return false;
            }
            Regex regex1 = new Regex(@"SetViewport(?<lat1>§+),(?<long1>§+),(?<lat2>§+),(?<long2>§+)");
            Match match1 = regex1.Match(text2);
            if (!match1.Success)
            {
                return false;
            }
            lat1 = double.Parse(match1.Groups["lat1"].Value);
            long1 = double.Parse(match1.Groups["long1"].Value);
            lat2 = double.Parse(match1.Groups["lat2"].Value);
            long2 = double.Parse(match1.Groups["long2"].Value);
            return true;
        }
        public static ArrayList SearchForBusiness(string business, double lat1, double lon1, double lat2, double lon2)
        {
            int num1 = 0;
            ArrayList list1 = new ArrayList();
            while (true)
            {
                bool flag1 = false;
                string text2 = string.Concat(new object[] { "a=", business.Trim(), "&b=&c=", lat1, "&d=", lon1, "&e=", lat2, "&f=", lon2, "&g=", num1, "&i=0&r=false" });
                string text1 = Search.DoSearchRequest(text2);
                if ((text1 != null) && (text1 != string.Empty))
                {
                    Regex regex1 = new Regex(@"VE_SearchResult(?<id>[0-9]*),'(?<name>[^']*)','(?<address>[^']*)','(?<phone>[^']*)',(?<rating>[^,]*),'(?<type>[^']*)',(?<latitude>[^,]*),(?<longitude>[^,)]*)");
                    MatchCollection collection1 = regex1.Matches(text1);
                    foreach (Match match1 in collection1)
                    {
                        PushPin pushPin = new PushPin();
                        pushPin.Name = match1.Groups["name"].Value;
                        pushPin.Address = match1.Groups["address"].Value;
                        pushPin.Phone = match1.Groups["phone"].Value;
                        pushPin.Latitude = double.Parse(match1.Groups["latitude"].Value);
                        pushPin.Longitude = double.Parse(match1.Groups["longitude"].Value);
                        list1.Add(pushPin);
                    }
                    num1 += 10;
                    regex1 = new Regex(@"true,''\);$");
                    if (regex1.IsMatch(text1))
                    {
                        flag1 = true;
                    }
                }
                if (!flag1 || (list1.Count >= 50))
                {
                    return list1;
                }
            }
        }
    }
    //TODO ultimately the PushPin class should be replaced with the use of the WW Icon class
    public class PushPin
    {
        public string Name;
        public string Address;
        public string Phone;
        public double Latitude;
        public double Longitude;
    }
    #endregion
    #region PROJ.4
    //this code is Mashi's pInvoke wrapper over Proj.4
    //i didn't make any modifications to it
    //----------------------------------------------------------------------------
    // : Reproject images on the fly
    // : 1.0
    // : Reproject images on the fly using nowak's "reproject on GPU technique" 
    // : Bjorn Reppen aka "Mashi"
    // : http://www.mashiharu.com
    // : 
    //----------------------------------------------------------------------------
    // This file is in the Public Domain, and comes with no warranty.
    /// <summary>
    /// Sorry for lack of description, but this struct is kinda difficult 
    /// to describe since it supports so many coordinate systems.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UV
    {
        public double U;
        public double V;
        public UV(double u, double v)
        {
            this.U = u;
            this.V = v;
        }
    }
    /// <summary>
    /// C# wrapper for proj.4 projection filter
    /// http://proj.maptools.org/
    /// </summary>
    public class Projection : IDisposable
    {
        IntPtr projPJ;
        [DllImport("proj.dll")]
        static extern IntPtr pj_init(int argc, string[] args);
        [DllImport("proj.dll")]
        static extern string pj_free(IntPtr projPJ);
        [DllImport("proj.dll")]
        static extern UV pj_fwd(UV uv, IntPtr projPJ);
        /// <summary>
        /// XY -> Lat/lon
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="projPJ"></param>
        /// <returns></returns>
        [DllImport("proj.dll")]
        static extern UV pj_inv(UV uv, IntPtr projPJ);
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initParameters">Proj.4 style list of options.
        /// <sample>new string[]{ "proj=utm", "ellps=WGS84", "no.defs", "zone=32" }</sample>
        /// </param>
        public Projection(string[] initParameters)
        {
            projPJ = pj_init(initParameters.Length, initParameters);
            if (projPJ == IntPtr.Zero)
                throw new ApplicationException("Projection initialization failed.");
        }
        /// <summary>
        /// Forward (Go from specified projection to lat/lon)
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public UV Forward(UV uv)
        {
            return pj_fwd(uv, projPJ);
        }
        /// <summary>
        /// Inverse (Go from lat/lon to specified projection)
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public UV Inverse(UV uv)
        {
            return pj_inv(uv, projPJ);
        }
        public void Dispose()
        {
            if (projPJ != IntPtr.Zero)
            {
                pj_free(projPJ);
                projPJ = IntPtr.Zero;
            }
        }
    }
    #endregion
    #region DOWNLOADER
    //this code was deprecated to use the WW WebDownload class
    /*
    public class Downloader
    {
        public string textureUrl;
        public string textureName;
        public DrawArgs drawArgs;
        public VeTile veTile;
        public string mapType;
        public void DownloadThread()
        {
            MemoryStream ms = DownloadImageStream(textureUrl);
            if(ms == null)
            {
                return;
            }
            //make sure it is not the bad tile
            bool badTile = false;
            if(VeReprojectTilesLayer.BadTileSize != -1)
            {
                if(ms.Length == VeReprojectTilesLayer.BadTileSize)
                {
                    if(VeReprojectTilesLayer.IsBadTile(ms) == true)
                    {
                        badTile = true;
                    }
                }
            }
            if(badTile == false)
            {
                veTile.Texture = TextureLoader.FromStream(drawArgs.device, ms);
                //now cache it
                //.Dds files are fast, but too big
                if(World.Settings.ConvertDownloadedImagesToDds == true)
                {
                    //default value is true, so i'm going to ignore
                    //TextureLoader.Save(textureName, ImageFileFormat.Dds, veTile.Texture);
                }
                if(mapType == "r")
                {
                    TextureLoader.Save(textureName, ImageFileFormat.Png, veTile.Texture);
                }
                else
                {
                    TextureLoader.Save(textureName, ImageFileFormat.Jpg, veTile.Texture);
                }
            }
            ms.Close();
            ms = null;
        }
        public MemoryStream DownloadImageStream(string url)
        {
            MemoryStream ms = null;
            HttpWebResponse hwResp = null;
            Stream s = null;
            try
            {
                HttpWebRequest hwReq = (HttpWebRequest) WebRequest.Create(url);
                hwReq.UserAgent = "NASA WorldWind 1.3.3.1";
                hwReq.Timeout = 5000; //5 second timeout
                hwResp = (HttpWebResponse) hwReq.GetResponse();
                s = hwResp.GetResponseStream();
                ms = new MemoryStream();
                byte [] buffer = new byte[1024 * 10];
                int totalRead = 0;
                int iterRead = 0;
                do
                {
                    iterRead = s.Read(buffer, 0, buffer.Length);
                    totalRead = totalRead + iterRead;
                    ms.Write(buffer, 0, iterRead);
                }
                while(iterRead > 0);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                if(s != null)
                {
                    s.Close();
                    s = null;
                }
                if(hwResp != null)
                {
                    hwResp.Close();
                    hwResp = null;
                }
            }
            if(ms != null)
            {
                ms.Position = 0;
            }
            return ms;
        }
    }
    */
    #endregion
}