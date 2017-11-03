//----------------------------------------------------------------------------------------------------------------------------
// NAME: GPSTracker
// DEVELOPER: Javier Santoro
// WEBSITE: http://www.worldwindcentral.com/wiki/Add-on:GPS_Tracker_(plugin)
// VERSION: V04R06 (May 16, 2007)
// COPYRIGHT NOTICE: Copyright 2007, Javier Santoro
//                   Permission is granted to use GpsTracker for non-commercial purposes. 
//                   Permission is granted to use GpsTracker source code for educational and non-commercial purposes.
//----------------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.PluginEngine;
using System.Net;
using System.Net.Sockets;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System.Xml;
using System.Data;
using System.Collections;
using System.Diagnostics;
using Org.Mentalis.Security.Ssl;
using System.Text;
using System.Net.Mail;
using System.Security.Cryptography;

namespace GpsTracker
{
    //
    // Main User interface, entry point GpsTracker class

	public class GpsTracker : System.Windows.Forms.Form
	{
	
		#region Member Variables definitions

		public	static int m_iMaxDevices	=512;
		public	GpsTrackerPlugin			m_gpsTrackerPlugin;
		public	GpsTrackerNMEA				m_NMEA;
		private	GpsTrackerUDPTCP			m_UDPTCP;
		public	GpsTrackerFile				m_File;
		public	ArrayList					m_gpsSourceList = new ArrayList();
		public  MessageMonitor				m_MessageMonitor;

		private TreeNode					m_treeNodeCOM;
		private TreeNode					m_treeNodeUDP;
		private TreeNode					m_treeNodeTCP;
		private TreeNode					m_treeNodeFile;
		private TreeNode					m_treeNodePOI;
        private TreeNode                    m_treeNodeUSB;
        private TreeNode                    m_treeNodeTrack;

        private ArrayList                   m_arrayGpsBabelFormats = new ArrayList();
		private Object						LockPOI = new Object();
		private Object						LockShowIcon = new Object();
		private Object						LockCOM = new Object();


        public bool m_bTrackOnTop;
		private bool m_bHandleControlValueChangeEvent=false;
		private int m_iSourceNameCount;
		private float m_fVerticalExaggeration;
		public bool m_bTrackHeading;
		public bool m_bTrackLine;
		public bool m_bRecordSession;
		public bool m_bInfoText;
		private StreamWriter m_swRecorder;
		private StreamReader m_srReader;
		public bool m_fPlayback;
		private String m_sPlaybackFile;
		public int m_iPlaybackSpeed;
        public int m_iPositionUnit;
        public int m_iDistanceUnit;

		private Thread m_hPOIThread;
        private AutoResetEvent m_eventPOIThreadSync;
		public	bool m_fCloseThreads;
		private int m_iLocked;
		private int m_iPrevLocked;
		System.Threading.Timer m_timerLocked;

		#region UserInterface Variables definitions
		private System.Windows.Forms.Button StartStop;
		private System.Windows.Forms.Label labelTrackCode;
		private System.ComponentModel.IContainer components;
		private bool m_fInitialized=false;
		private System.Windows.Forms.ProgressBar progressBarSetup;
		private System.Windows.Forms.Label labelSettingup;
		private System.Windows.Forms.ColorDialog colorPicker;
		//private System.Windows.Forms.ComboBox comboBoxGpsFile;
		private System.Windows.Forms.ImageList imageListGpsIcons;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.PictureBox pictureBoxLogo;
		private System.Windows.Forms.TabControl tabControlGPS;
		private System.Windows.Forms.TabPage tabPageCOM;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Button buttonTrackColorCOM;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.ComboBox comboBoxFlowControl;
		private System.Windows.Forms.Button buttonAutoDetect;
		private System.Windows.Forms.ProgressBar progressBarAutoDetect;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxStopBits;
		private System.Windows.Forms.ComboBox comboParity;
		private System.Windows.Forms.ComboBox comboBoxByteSize;
		private System.Windows.Forms.ComboBox comboBoxBaudRate;
		private System.Windows.Forms.ComboBox comboBoxCOMPort;
		private System.Windows.Forms.TabPage tabPageUDP;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Button buttonTrackColorUDP;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDownUDPPort;
		private System.Windows.Forms.TabPage tabPageTCP;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Button buttonTrackColorTCP;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownTCPPort;
        private System.Windows.Forms.TabPage tabPageFile;
		private System.Windows.Forms.TabPage tabPageUpdate;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBoxVersionInfo;
        private System.Windows.Forms.Button buttonCheckForUpdates;
		private System.Windows.Forms.TabPage tabPageUsage;
		private System.Windows.Forms.TextBox textBoxUsage;
		private System.Windows.Forms.TreeView treeViewSources;
		private System.Windows.Forms.ContextMenu contextMenuSourceTree;
		private System.Windows.Forms.MenuItem menuItemAdd;
		private System.Windows.Forms.MenuItem menuItemRename;
		private System.Windows.Forms.MenuItem menuItemSetIcon;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemSetTrack;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Button buttonApplyUDP;
        private System.Windows.Forms.Button buttonApplyTCP;
		private System.Windows.Forms.TabPage tabPagePOI;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.TextBox textBoxLatitud;
		private System.Windows.Forms.TextBox textBoxLongitud;
		private System.Windows.Forms.Button buttonApplyPOI;
		private System.Windows.Forms.TabPage tabPageCOMHelp;
        private System.Windows.Forms.TabPage tabPagePOIHelp;
		private System.Windows.Forms.TabPage tabPageFileHelp;
		private System.Windows.Forms.TabPage tabPageTCPHelp;
		private System.Windows.Forms.TabPage tabPageUDPHelp;
		private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.PictureBox pictureBoxHelpLogo;
        private System.Windows.Forms.CheckBox checkBoxSecureSocket;
		private System.Windows.Forms.ComboBox comboBoxTcpIP;
        private Label label37;
        private Label label36;
        private NumericUpDown numericUpDownReload;
        private Button buttonApplyFile;
        private Label label19;
        private Button buttonTrackColor;
        private CheckBox checkBoxForcePreprocessing;
        private Label labelPreprocessing;
        private ProgressBar progressBarPreprocessing;
        private CheckBox checkBoxTrackAtOnce;
        private CheckBox checkBoxNoDelay;
        private ComboBox comboBoxFile;
        private Label label13;
        private Label label12;
        private TrackBar trackBarFileSpeed;
        private Button buttonBrowseGpsFile;
        private Label label9;
        private Label label14;
        private GroupBox groupBox5;
        private ComboBox comboBoxTrackFileType;
        private Label label41;
        private TabPage tabPageUSB;
        private Button buttonTestUSB;
        private Button buttonApplyUSB;
        private Label label42;
        private Button buttonTrackColorUSB;
        private Label label43;
        private ComboBox comboBoxUSBDevice;
        private TabPage tabPageUSBHelp;
        private TextBox textBox8;
        private TabPage tabPageWaypoints;
        private ListView listViewWaypoints;
        private ColumnHeader columnHeaderNumber;
        private ColumnHeader columnHeaderLatitude;
        private ColumnHeader columnHeaderLonguitude;
        private ColumnHeader columnHeaderDescription;
        private GroupBox groupBox7;
        private Button button1;
        private Button buttonApplyWaypoints;
        private ComboBox comboBoxWaypointsFileType;
        private Label label44;
        private ComboBox comboBoxWaypointsFile;
        private Button button3;
        private Label label45;
        private CheckBox checkBoxCOMExport;
        private CheckBox checkBoxTCPExport;
        private CheckBox checkBoxTrackExport;
        private CheckBox checkBoxUDPExport;
        private CheckBox checkBoxPOIExport;
        private CheckBox checkBoxUSBExport;
        private CheckBox checkBoxWaypointsExport;
        private FolderBrowserDialog folderBrowser;
        private TabPage tabPageGeoFence;
        private ListView listViewGeoFence;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private Button buttonApplyGeoFence;
        private Label label50;
        private TextBox editBoxGeoFenceList;
        private TabPage tabPageGeneral;
        private TabControl tabControl1;
        private TabPage tabPageGeneralSettings;
        private CheckBox checkBoxMessagesMonitor;
        private CheckBox checkBoxRecordSession;
        private CheckBox checkBoxSetAltitud;
        private Label label39;
        private Label label38;
        private NumericUpDown numericUpDownAltitud;
        private GroupBox groupBox3;
        private GroupBox groupBox1;
        private Label label59;
        private CheckBox checkBoxTrackOnTop;
        private Label label16;
        private Label label15;
        private Label label10;
        private CheckBox checkBoxTrackLine;
        private CheckBox checkBoxTrackHeading;
        private CheckBox checkBoxVExaggeration;
        private GroupBox groupBox6;
        private GroupBox groupBox8;
        private CheckBox checkBoxInformationText;
        private TabPage tabPageNMEAExport;
        private Button buttonExportPathBrowse;
        private TextBox textBoxNMEAExportPath;
        private TabPage tabPageKeyboardShortcuts;
        private Label label35;
        private Label label46;
        private Label label33;
        private Label label17;
        private Label label61;
        private Label label58;
        private TabPage tabPageConfiguration;
        private ComboBox comboBoxConfigurationFile;
        private Button buttonLoadConfiguration;
        private Label label67;
        private GroupBox groupBox17;
        private TextBox textBoxSaveConfiguration;
        private Button buttonSaveConfiguration;
        private Label label68;
        private GroupBox groupBox16;
        private Button buttonLoadConfigurationFile;
        private TabPage tabPageInformationText;
        private Label label71;
        private CheckBox checkBoxTrackDistance;
        private CheckBox checkBoxTimeDate;
        private CheckBox checkBoxHeading;
        private CheckBox checkBoxSpeed;
        private CheckBox checkBoxPosition;
        private CheckBox checkBoxShowName;
        private Button buttonColorInformationText;
        private Button buttonApplyInformationText;
        private ComboBox comboBoxFontSize;
        private Label label75;
        private CheckBox checkBoxShowComment;
        private Label label76;
        private Button buttonTrackColorTrackLine;
        private Label label78;
        private Label label79;
        private Button buttonSaveConfig;
        private TabPage tabPageUnits;
        private Label label82;
        private ComboBox comboBoxPositionUnits;
        private Label label83;
        private ComboBox comboBoxDistanceUnits;
        private TabPage tabPageTrackLineHelp;
        private TextBox textBox10;
        private Label label18;
        private Label label24;
        private GroupBox groupBox19;
		#endregion
		#endregion

		#region Constructor and Form functions
		//
		//Constructor for the GpsTracker class
		//
		//public GpsTracker(WorldWindow.WorldWindow worldWindow)
		public GpsTracker(GpsTrackerPlugin gpsPlugin)
		{
			m_gpsTrackerPlugin = gpsPlugin;

			// Required for Windows Form Designer support
			InitializeComponent();
			
			m_iSourceNameCount=0;
			SetupTree();

			Bitmap image = new Bitmap(GpsTrackerPlugin.m_sPluginDirectory + "\\satellite.png");
			pictureBoxLogo.Image = image;
			pictureBoxHelpLogo.Image= image;

            m_gpsSourceList.Clear();
			m_NMEA = new GpsTrackerNMEA(this);
			m_UDPTCP = new GpsTrackerUDPTCP(this);
			m_File = new GpsTrackerFile(this);

			m_fVerticalExaggeration=World.Settings.VerticalExaggeration;

            GetGpsBabelFormats();

			m_timerLocked=null;
			m_hPOIThread=null;
		
			m_iLocked=0;
			m_iPrevLocked=0;

			progressBarAutoDetect.Value=0;
			progressBarAutoDetect.Width=0; //using visible to hide the progress bar does not seem to work very well

			//StartStop.Enabled=false;
            StartStop.Enabled = true;
			LoadSettings(null,true); //load user settings

			m_swRecorder=null;
			m_srReader=null;
			m_fPlayback=false;

            textBoxVersionInfo.Text = "Your Version: " + GpsTrackerPlugin.m_sVersion;

			m_MessageMonitor = null;

			SetDefaultSettings(true);
		}


        void GetGpsBabelFormats()
        {
            ArrayList listFormats = new ArrayList();
            try
            {
                StreamReader sReader = File.OpenText(GpsTrackerPlugin.m_sPluginDirectory + "\\SupportedGpsBabelFormats.txt");
                string sLine;
                do
                {
                    sLine = sReader.ReadLine();
                    if (sLine != null)
                        listFormats.Add(sLine);
                } while (sLine != null);
            }
            catch (Exception)
            {
            }

            comboBoxTrackFileType.Items.Clear();
            comboBoxWaypointsFileType.Items.Clear();
            if (listFormats.Count > 0)
            {
                char[] cSep = { '\t' };
                for (int i = 0; i < listFormats.Count; i++)
                {
                    string sFormat = (string)listFormats[i];
                    string[] sFormatData;
                    sFormatData = sFormat.Split(cSep);
                    if (sFormatData.Length == 3)
                    {
                        m_arrayGpsBabelFormats.Add(sFormatData);
                        comboBoxTrackFileType.Items.Add(sFormatData[2]);
                        comboBoxWaypointsFileType.Items.Add(sFormatData[2]);
                    }
                }
            }
        }

		//
		//Form load, unload, close, etc
		//
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		//if the window is going visible then move it to 100,100 (this position should 
		//depend on the Main Window size and position).
		//if the window is going invisible (user clicked on the Window Close button (X) or
		//on the main toolbar button to stop tracking), it means the user wants to cancel, 
		//then deinitialize.
		protected override void OnVisibleChanged(EventArgs e)
		{
#if !DEBUG	
			try
			{
#endif
			if (this.Visible==false)
				Deinitialize();
			else
			{
                this.ShowInTaskbar = true;
				this.Left=100;
				this.Top=100;
				labelTrackCode.Text="";
				m_gpsTrackerPlugin.m_fGpsTrackerRunning=true;
				LoadSettings(null,true);
			}

			base.OnVisibleChanged (e);

#if !DEBUG	
			}
			catch(Exception)
			{
			}
#endif
		}

		//OnClosing just hide the window but do not close it.
		//See comment in OnVisibleChanged
		protected override void OnClosing(CancelEventArgs e)
		{
#if !DEBUG	
			try
			{
#endif
			e.Cancel = true;

			this.Hide();

			base.OnClosing(e);
#if !DEBUG	
			}
			catch(Exception)
			{
			}
#endif

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
#if !DEBUG	
			try
			{
#endif
			base.Dispose( disposing );
#if !DEBUG	
			}
			catch(Exception)
			{
			}
#endif
		}

		private void GpsTracker_Load(object sender, System.EventArgs e)
		{
            this.Text = "GPSTracker :: Version: " + GpsTrackerPlugin.m_sVersion; //window title
			m_gpsTrackerPlugin.m_fGpsTrackerRunning=true;
            m_gpsSourceList.Clear();
		}

		private void GpsTracker_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Deinitialize();
			m_gpsTrackerPlugin.pluginWorldWindowFocus();
			m_gpsTrackerPlugin.m_fGpsTrackerRunning=false;
            //m_gpsSourceList.Clear();
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpsTracker));
            this.StartStop = new System.Windows.Forms.Button();
            this.imageListGpsIcons = new System.Windows.Forms.ImageList(this.components);
            this.labelTrackCode = new System.Windows.Forms.Label();
            this.progressBarSetup = new System.Windows.Forms.ProgressBar();
            this.labelSettingup = new System.Windows.Forms.Label();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.treeViewSources = new System.Windows.Forms.TreeView();
            this.contextMenuSourceTree = new System.Windows.Forms.ContextMenu();
            this.menuItemAdd = new System.Windows.Forms.MenuItem();
            this.menuItemRename = new System.Windows.Forms.MenuItem();
            this.menuItemSetIcon = new System.Windows.Forms.MenuItem();
            this.menuItemSetTrack = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.tabControlGPS = new System.Windows.Forms.TabControl();
            this.tabPageCOM = new System.Windows.Forms.TabPage();
            this.checkBoxCOMExport = new System.Windows.Forms.CheckBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.buttonTrackColorCOM = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBoxFlowControl = new System.Windows.Forms.ComboBox();
            this.buttonAutoDetect = new System.Windows.Forms.Button();
            this.progressBarAutoDetect = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.comboParity = new System.Windows.Forms.ComboBox();
            this.comboBoxByteSize = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.comboBoxCOMPort = new System.Windows.Forms.ComboBox();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageGeneralSettings = new System.Windows.Forms.TabPage();
            this.label35 = new System.Windows.Forms.Label();
            this.checkBoxMessagesMonitor = new System.Windows.Forms.CheckBox();
            this.checkBoxRecordSession = new System.Windows.Forms.CheckBox();
            this.checkBoxSetAltitud = new System.Windows.Forms.CheckBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.numericUpDownAltitud = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label59 = new System.Windows.Forms.Label();
            this.checkBoxTrackOnTop = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxTrackLine = new System.Windows.Forms.CheckBox();
            this.checkBoxTrackHeading = new System.Windows.Forms.CheckBox();
            this.checkBoxVExaggeration = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBoxInformationText = new System.Windows.Forms.CheckBox();
            this.tabPageUnits = new System.Windows.Forms.TabPage();
            this.label83 = new System.Windows.Forms.Label();
            this.comboBoxDistanceUnits = new System.Windows.Forms.ComboBox();
            this.label82 = new System.Windows.Forms.Label();
            this.comboBoxPositionUnits = new System.Windows.Forms.ComboBox();
            this.tabPageNMEAExport = new System.Windows.Forms.TabPage();
            this.buttonExportPathBrowse = new System.Windows.Forms.Button();
            this.textBoxNMEAExportPath = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.tabPageConfiguration = new System.Windows.Forms.TabPage();
            this.buttonSaveConfig = new System.Windows.Forms.Button();
            this.buttonLoadConfigurationFile = new System.Windows.Forms.Button();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.textBoxSaveConfiguration = new System.Windows.Forms.TextBox();
            this.buttonSaveConfiguration = new System.Windows.Forms.Button();
            this.label68 = new System.Windows.Forms.Label();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.comboBoxConfigurationFile = new System.Windows.Forms.ComboBox();
            this.buttonLoadConfiguration = new System.Windows.Forms.Button();
            this.label67 = new System.Windows.Forms.Label();
            this.tabPageKeyboardShortcuts = new System.Windows.Forms.TabPage();
            this.label18 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label78 = new System.Windows.Forms.Label();
            this.label79 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.tabPageFileHelp = new System.Windows.Forms.TabPage();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.tabPageUsage = new System.Windows.Forms.TabPage();
            this.textBoxUsage = new System.Windows.Forms.TextBox();
            this.pictureBoxHelpLogo = new System.Windows.Forms.PictureBox();
            this.tabPageTCP = new System.Windows.Forms.TabPage();
            this.checkBoxTCPExport = new System.Windows.Forms.CheckBox();
            this.comboBoxTcpIP = new System.Windows.Forms.ComboBox();
            this.checkBoxSecureSocket = new System.Windows.Forms.CheckBox();
            this.buttonApplyTCP = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.buttonTrackColorTCP = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownTCPPort = new System.Windows.Forms.NumericUpDown();
            this.tabPageFile = new System.Windows.Forms.TabPage();
            this.checkBoxTrackExport = new System.Windows.Forms.CheckBox();
            this.comboBoxTrackFileType = new System.Windows.Forms.ComboBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.numericUpDownReload = new System.Windows.Forms.NumericUpDown();
            this.buttonApplyFile = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.buttonTrackColor = new System.Windows.Forms.Button();
            this.checkBoxForcePreprocessing = new System.Windows.Forms.CheckBox();
            this.labelPreprocessing = new System.Windows.Forms.Label();
            this.progressBarPreprocessing = new System.Windows.Forms.ProgressBar();
            this.checkBoxTrackAtOnce = new System.Windows.Forms.CheckBox();
            this.checkBoxNoDelay = new System.Windows.Forms.CheckBox();
            this.comboBoxFile = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.trackBarFileSpeed = new System.Windows.Forms.TrackBar();
            this.buttonBrowseGpsFile = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabPageUDPHelp = new System.Windows.Forms.TabPage();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.tabPageCOMHelp = new System.Windows.Forms.TabPage();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabPagePOIHelp = new System.Windows.Forms.TabPage();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tabPageTCPHelp = new System.Windows.Forms.TabPage();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.tabPageUDP = new System.Windows.Forms.TabPage();
            this.checkBoxUDPExport = new System.Windows.Forms.CheckBox();
            this.buttonApplyUDP = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.buttonTrackColorUDP = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownUDPPort = new System.Windows.Forms.NumericUpDown();
            this.tabPageUpdate = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxVersionInfo = new System.Windows.Forms.TextBox();
            this.buttonCheckForUpdates = new System.Windows.Forms.Button();
            this.tabPagePOI = new System.Windows.Forms.TabPage();
            this.checkBoxPOIExport = new System.Windows.Forms.CheckBox();
            this.buttonApplyPOI = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.textBoxLongitud = new System.Windows.Forms.TextBox();
            this.textBoxLatitud = new System.Windows.Forms.TextBox();
            this.tabPageUSB = new System.Windows.Forms.TabPage();
            this.checkBoxUSBExport = new System.Windows.Forms.CheckBox();
            this.buttonTestUSB = new System.Windows.Forms.Button();
            this.buttonApplyUSB = new System.Windows.Forms.Button();
            this.label42 = new System.Windows.Forms.Label();
            this.buttonTrackColorUSB = new System.Windows.Forms.Button();
            this.label43 = new System.Windows.Forms.Label();
            this.comboBoxUSBDevice = new System.Windows.Forms.ComboBox();
            this.tabPageUSBHelp = new System.Windows.Forms.TabPage();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.tabPageWaypoints = new System.Windows.Forms.TabPage();
            this.checkBoxWaypointsExport = new System.Windows.Forms.CheckBox();
            this.comboBoxWaypointsFileType = new System.Windows.Forms.ComboBox();
            this.label44 = new System.Windows.Forms.Label();
            this.comboBoxWaypointsFile = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.listViewWaypoints = new System.Windows.Forms.ListView();
            this.columnHeaderNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLatitude = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLonguitude = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.buttonApplyWaypoints = new System.Windows.Forms.Button();
            this.label45 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tabPageGeoFence = new System.Windows.Forms.TabPage();
            this.label76 = new System.Windows.Forms.Label();
            this.buttonTrackColorTrackLine = new System.Windows.Forms.Button();
            this.editBoxGeoFenceList = new System.Windows.Forms.TextBox();
            this.listViewGeoFence = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonApplyGeoFence = new System.Windows.Forms.Button();
            this.label50 = new System.Windows.Forms.Label();
            this.tabPageInformationText = new System.Windows.Forms.TabPage();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.checkBoxShowComment = new System.Windows.Forms.CheckBox();
            this.buttonColorInformationText = new System.Windows.Forms.Button();
            this.buttonApplyInformationText = new System.Windows.Forms.Button();
            this.comboBoxFontSize = new System.Windows.Forms.ComboBox();
            this.label75 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.checkBoxTrackDistance = new System.Windows.Forms.CheckBox();
            this.checkBoxTimeDate = new System.Windows.Forms.CheckBox();
            this.checkBoxHeading = new System.Windows.Forms.CheckBox();
            this.checkBoxSpeed = new System.Windows.Forms.CheckBox();
            this.checkBoxPosition = new System.Windows.Forms.CheckBox();
            this.checkBoxShowName = new System.Windows.Forms.CheckBox();
            this.tabPageTrackLineHelp = new System.Windows.Forms.TabPage();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.tabControlGPS.SuspendLayout();
            this.tabPageCOM.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneralSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAltitud)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.tabPageUnits.SuspendLayout();
            this.tabPageNMEAExport.SuspendLayout();
            this.tabPageConfiguration.SuspendLayout();
            this.tabPageKeyboardShortcuts.SuspendLayout();
            this.tabPageFileHelp.SuspendLayout();
            this.tabPageUsage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelpLogo)).BeginInit();
            this.tabPageTCP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTCPPort)).BeginInit();
            this.tabPageFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFileSpeed)).BeginInit();
            this.tabPageUDPHelp.SuspendLayout();
            this.tabPageCOMHelp.SuspendLayout();
            this.tabPagePOIHelp.SuspendLayout();
            this.tabPageTCPHelp.SuspendLayout();
            this.tabPageUDP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUDPPort)).BeginInit();
            this.tabPageUpdate.SuspendLayout();
            this.tabPagePOI.SuspendLayout();
            this.tabPageUSB.SuspendLayout();
            this.tabPageUSBHelp.SuspendLayout();
            this.tabPageWaypoints.SuspendLayout();
            this.tabPageGeoFence.SuspendLayout();
            this.tabPageInformationText.SuspendLayout();
            this.tabPageTrackLineHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartStop
            // 
            this.StartStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartStop.Location = new System.Drawing.Point(683, 436);
            this.StartStop.Name = "StartStop";
            this.StartStop.Size = new System.Drawing.Size(67, 53);
            this.StartStop.TabIndex = 0;
            this.StartStop.Text = "Track";
            this.StartStop.Click += new System.EventHandler(this.StartStop_Click);
            // 
            // imageListGpsIcons
            // 
            this.imageListGpsIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListGpsIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListGpsIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // labelTrackCode
            // 
            this.labelTrackCode.BackColor = System.Drawing.SystemColors.Control;
            this.labelTrackCode.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelTrackCode.Location = new System.Drawing.Point(86, 433);
            this.labelTrackCode.Name = "labelTrackCode";
            this.labelTrackCode.Size = new System.Drawing.Size(579, 29);
            this.labelTrackCode.TabIndex = 3;
            // 
            // progressBarSetup
            // 
            this.progressBarSetup.Location = new System.Drawing.Point(235, 466);
            this.progressBarSetup.Name = "progressBarSetup";
            this.progressBarSetup.Size = new System.Drawing.Size(430, 18);
            this.progressBarSetup.Step = 1;
            this.progressBarSetup.TabIndex = 5;
            // 
            // labelSettingup
            // 
            this.labelSettingup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSettingup.Location = new System.Drawing.Point(113, 463);
            this.labelSettingup.Name = "labelSettingup";
            this.labelSettingup.Size = new System.Drawing.Size(115, 26);
            this.labelSettingup.TabIndex = 31;
            this.labelSettingup.Text = "Setting up...";
            this.labelSettingup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(-29, 414);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1018, 8);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(12, 429);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(67, 60);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 36;
            this.pictureBoxLogo.TabStop = false;
            // 
            // treeViewSources
            // 
            this.treeViewSources.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewSources.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewSources.HideSelection = false;
            this.treeViewSources.ImageIndex = 0;
            this.treeViewSources.ImageList = this.imageListGpsIcons;
            this.treeViewSources.Indent = 15;
            this.treeViewSources.Location = new System.Drawing.Point(0, 0);
            this.treeViewSources.Name = "treeViewSources";
            this.treeViewSources.SelectedImageIndex = 0;
            this.treeViewSources.Size = new System.Drawing.Size(221, 411);
            this.treeViewSources.TabIndex = 39;
            this.treeViewSources.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewSources_AfterLabelEdit);
            this.treeViewSources.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewSources_MouseDown);
            this.treeViewSources.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewSources_MouseUp);
            // 
            // contextMenuSourceTree
            // 
            this.contextMenuSourceTree.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAdd,
            this.menuItemRename,
            this.menuItemSetIcon,
            this.menuItemSetTrack,
            this.menuItemDelete});
            // 
            // menuItemAdd
            // 
            this.menuItemAdd.Index = 0;
            this.menuItemAdd.Text = "Add";
            this.menuItemAdd.Click += new System.EventHandler(this.menuItemAdd_Click);
            // 
            // menuItemRename
            // 
            this.menuItemRename.Index = 1;
            this.menuItemRename.Text = "Rename";
            this.menuItemRename.Click += new System.EventHandler(this.menuItemRename_Click);
            // 
            // menuItemSetIcon
            // 
            this.menuItemSetIcon.Index = 2;
            this.menuItemSetIcon.Text = "Icon";
            this.menuItemSetIcon.Click += new System.EventHandler(this.menuItemSetIcon_Click);
            // 
            // menuItemSetTrack
            // 
            this.menuItemSetTrack.Index = 3;
            this.menuItemSetTrack.Text = "Track";
            this.menuItemSetTrack.Click += new System.EventHandler(this.menuItemSetTrack_Click);
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Index = 4;
            this.menuItemDelete.Text = "Delete";
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // tabControlGPS
            // 
            this.tabControlGPS.Controls.Add(this.tabPageCOM);
            this.tabControlGPS.Controls.Add(this.tabPageGeneral);
            this.tabControlGPS.Controls.Add(this.tabPageFileHelp);
            this.tabControlGPS.Controls.Add(this.tabPageUsage);
            this.tabControlGPS.Controls.Add(this.tabPageTCP);
            this.tabControlGPS.Controls.Add(this.tabPageFile);
            this.tabControlGPS.Controls.Add(this.tabPageUDPHelp);
            this.tabControlGPS.Controls.Add(this.tabPageCOMHelp);
            this.tabControlGPS.Controls.Add(this.tabPagePOIHelp);
            this.tabControlGPS.Controls.Add(this.tabPageTCPHelp);
            this.tabControlGPS.Controls.Add(this.tabPageUDP);
            this.tabControlGPS.Controls.Add(this.tabPageUpdate);
            this.tabControlGPS.Controls.Add(this.tabPagePOI);
            this.tabControlGPS.Controls.Add(this.tabPageUSB);
            this.tabControlGPS.Controls.Add(this.tabPageUSBHelp);
            this.tabControlGPS.Controls.Add(this.tabPageWaypoints);
            this.tabControlGPS.Controls.Add(this.tabPageGeoFence);
            this.tabControlGPS.Controls.Add(this.tabPageInformationText);
            this.tabControlGPS.Controls.Add(this.tabPageTrackLineHelp);
            this.tabControlGPS.Location = new System.Drawing.Point(223, 0);
            this.tabControlGPS.Name = "tabControlGPS";
            this.tabControlGPS.SelectedIndex = 0;
            this.tabControlGPS.Size = new System.Drawing.Size(527, 411);
            this.tabControlGPS.TabIndex = 40;
            // 
            // tabPageCOM
            // 
            this.tabPageCOM.Controls.Add(this.checkBoxCOMExport);
            this.tabPageCOM.Controls.Add(this.buttonApply);
            this.tabPageCOM.Controls.Add(this.label23);
            this.tabPageCOM.Controls.Add(this.buttonTrackColorCOM);
            this.tabPageCOM.Controls.Add(this.label20);
            this.tabPageCOM.Controls.Add(this.comboBoxFlowControl);
            this.tabPageCOM.Controls.Add(this.buttonAutoDetect);
            this.tabPageCOM.Controls.Add(this.progressBarAutoDetect);
            this.tabPageCOM.Controls.Add(this.label5);
            this.tabPageCOM.Controls.Add(this.label4);
            this.tabPageCOM.Controls.Add(this.label3);
            this.tabPageCOM.Controls.Add(this.label2);
            this.tabPageCOM.Controls.Add(this.label1);
            this.tabPageCOM.Controls.Add(this.comboBoxStopBits);
            this.tabPageCOM.Controls.Add(this.comboParity);
            this.tabPageCOM.Controls.Add(this.comboBoxByteSize);
            this.tabPageCOM.Controls.Add(this.comboBoxBaudRate);
            this.tabPageCOM.Controls.Add(this.comboBoxCOMPort);
            this.tabPageCOM.Location = new System.Drawing.Point(4, 22);
            this.tabPageCOM.Name = "tabPageCOM";
            this.tabPageCOM.Size = new System.Drawing.Size(519, 385);
            this.tabPageCOM.TabIndex = 0;
            this.tabPageCOM.Text = "COM";
            this.tabPageCOM.UseVisualStyleBackColor = true;
            // 
            // checkBoxCOMExport
            // 
            this.checkBoxCOMExport.Location = new System.Drawing.Point(164, 212);
            this.checkBoxCOMExport.Name = "checkBoxCOMExport";
            this.checkBoxCOMExport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxCOMExport.Size = new System.Drawing.Size(209, 18);
            this.checkBoxCOMExport.TabIndex = 47;
            this.checkBoxCOMExport.Text = "Export as NMEA to File";
            this.checkBoxCOMExport.UseVisualStyleBackColor = true;
            this.checkBoxCOMExport.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(446, 354);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(68, 26);
            this.buttonApply.TabIndex = 46;
            this.buttonApply.Text = "Apply";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(95, 182);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(115, 17);
            this.label23.TabIndex = 45;
            this.label23.Text = "Track Color:";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrackColorCOM
            // 
            this.buttonTrackColorCOM.Location = new System.Drawing.Point(220, 182);
            this.buttonTrackColorCOM.Name = "buttonTrackColorCOM";
            this.buttonTrackColorCOM.Size = new System.Drawing.Size(153, 14);
            this.buttonTrackColorCOM.TabIndex = 44;
            this.buttonTrackColorCOM.BackColorChanged += new System.EventHandler(this.ControlValueChanged);
            this.buttonTrackColorCOM.Click += new System.EventHandler(this.button3_Click);
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(85, 145);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(125, 26);
            this.label20.TabIndex = 25;
            this.label20.Text = "Flow Control:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxFlowControl
            // 
            this.comboBoxFlowControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFlowControl.ItemHeight = 12;
            this.comboBoxFlowControl.Items.AddRange(new object[] {
            "None",
            "Hardware",
            "Software (XOn|XOff)"});
            this.comboBoxFlowControl.Location = new System.Drawing.Point(220, 145);
            this.comboBoxFlowControl.Name = "comboBoxFlowControl";
            this.comboBoxFlowControl.Size = new System.Drawing.Size(153, 20);
            this.comboBoxFlowControl.TabIndex = 24;
            this.comboBoxFlowControl.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonAutoDetect
            // 
            this.buttonAutoDetect.Location = new System.Drawing.Point(22, 300);
            this.buttonAutoDetect.Name = "buttonAutoDetect";
            this.buttonAutoDetect.Size = new System.Drawing.Size(57, 26);
            this.buttonAutoDetect.TabIndex = 22;
            this.buttonAutoDetect.Text = "Auto Detect";
            this.buttonAutoDetect.Visible = false;
            // 
            // progressBarAutoDetect
            // 
            this.progressBarAutoDetect.Cursor = System.Windows.Forms.Cursors.Default;
            this.progressBarAutoDetect.Location = new System.Drawing.Point(22, 268);
            this.progressBarAutoDetect.Maximum = 32;
            this.progressBarAutoDetect.Name = "progressBarAutoDetect";
            this.progressBarAutoDetect.Size = new System.Drawing.Size(134, 26);
            this.progressBarAutoDetect.Step = 1;
            this.progressBarAutoDetect.TabIndex = 23;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(124, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 25);
            this.label5.TabIndex = 21;
            this.label5.Text = "Stop Bits:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(124, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 26);
            this.label4.TabIndex = 20;
            this.label4.Text = "Parity:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(124, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 26);
            this.label3.TabIndex = 19;
            this.label3.Text = "Data Bits:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(124, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 26);
            this.label2.TabIndex = 18;
            this.label2.Text = "Baud Rate:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(124, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 26);
            this.label1.TabIndex = 17;
            this.label1.Text = "Port Number:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxStopBits
            // 
            this.comboBoxStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStopBits.ItemHeight = 12;
            this.comboBoxStopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.comboBoxStopBits.Location = new System.Drawing.Point(220, 120);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(153, 20);
            this.comboBoxStopBits.TabIndex = 16;
            this.comboBoxStopBits.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboParity
            // 
            this.comboParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboParity.ItemHeight = 12;
            this.comboParity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.comboParity.Location = new System.Drawing.Point(220, 94);
            this.comboParity.Name = "comboParity";
            this.comboParity.Size = new System.Drawing.Size(153, 20);
            this.comboParity.TabIndex = 15;
            this.comboParity.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboBoxByteSize
            // 
            this.comboBoxByteSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxByteSize.ItemHeight = 12;
            this.comboBoxByteSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8"});
            this.comboBoxByteSize.Location = new System.Drawing.Point(220, 68);
            this.comboBoxByteSize.Name = "comboBoxByteSize";
            this.comboBoxByteSize.Size = new System.Drawing.Size(153, 20);
            this.comboBoxByteSize.TabIndex = 14;
            this.comboBoxByteSize.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBaudRate.ItemHeight = 12;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "56000",
            "57600",
            "115200",
            "128000",
            "256000"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(220, 42);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(153, 20);
            this.comboBoxBaudRate.TabIndex = 13;
            this.comboBoxBaudRate.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboBoxCOMPort
            // 
            this.comboBoxCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCOMPort.ItemHeight = 12;
            this.comboBoxCOMPort.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32"});
            this.comboBoxCOMPort.Location = new System.Drawing.Point(220, 16);
            this.comboBoxCOMPort.Name = "comboBoxCOMPort";
            this.comboBoxCOMPort.Size = new System.Drawing.Size(153, 20);
            this.comboBoxCOMPort.TabIndex = 12;
            this.comboBoxCOMPort.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.tabControl1);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(519, 385);
            this.tabPageGeneral.TabIndex = 6;
            this.tabPageGeneral.Text = "Settings";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            this.tabPageGeneral.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageGeneralSettings);
            this.tabControl1.Controls.Add(this.tabPageUnits);
            this.tabControl1.Controls.Add(this.tabPageNMEAExport);
            this.tabControl1.Controls.Add(this.tabPageConfiguration);
            this.tabControl1.Controls.Add(this.tabPageKeyboardShortcuts);
            this.tabControl1.Location = new System.Drawing.Point(4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(501, 377);
            this.tabControl1.TabIndex = 65;
            // 
            // tabPageGeneralSettings
            // 
            this.tabPageGeneralSettings.Controls.Add(this.label35);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxMessagesMonitor);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxRecordSession);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxSetAltitud);
            this.tabPageGeneralSettings.Controls.Add(this.label39);
            this.tabPageGeneralSettings.Controls.Add(this.label38);
            this.tabPageGeneralSettings.Controls.Add(this.numericUpDownAltitud);
            this.tabPageGeneralSettings.Controls.Add(this.groupBox3);
            this.tabPageGeneralSettings.Controls.Add(this.groupBox1);
            this.tabPageGeneralSettings.Controls.Add(this.label59);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxTrackOnTop);
            this.tabPageGeneralSettings.Controls.Add(this.label16);
            this.tabPageGeneralSettings.Controls.Add(this.label15);
            this.tabPageGeneralSettings.Controls.Add(this.label10);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxTrackLine);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxTrackHeading);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxVExaggeration);
            this.tabPageGeneralSettings.Controls.Add(this.groupBox6);
            this.tabPageGeneralSettings.Controls.Add(this.checkBoxInformationText);
            this.tabPageGeneralSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneralSettings.Name = "tabPageGeneralSettings";
            this.tabPageGeneralSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneralSettings.Size = new System.Drawing.Size(493, 351);
            this.tabPageGeneralSettings.TabIndex = 0;
            this.tabPageGeneralSettings.Text = "General";
            this.tabPageGeneralSettings.UseVisualStyleBackColor = true;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(143, 198);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(185, 12);
            this.label35.TabIndex = 84;
            this.label35.Text = "- Open Messages Monitor Window";
            // 
            // checkBoxMessagesMonitor
            // 
            this.checkBoxMessagesMonitor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxMessagesMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxMessagesMonitor.Location = new System.Drawing.Point(-18, 196);
            this.checkBoxMessagesMonitor.Name = "checkBoxMessagesMonitor";
            this.checkBoxMessagesMonitor.Size = new System.Drawing.Size(154, 19);
            this.checkBoxMessagesMonitor.TabIndex = 83;
            this.checkBoxMessagesMonitor.Text = "Messages Monitor";
            this.checkBoxMessagesMonitor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxRecordSession
            // 
            this.checkBoxRecordSession.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxRecordSession.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxRecordSession.Location = new System.Drawing.Point(-18, 158);
            this.checkBoxRecordSession.Name = "checkBoxRecordSession";
            this.checkBoxRecordSession.Size = new System.Drawing.Size(154, 18);
            this.checkBoxRecordSession.TabIndex = 80;
            this.checkBoxRecordSession.Text = "Record Session";
            this.checkBoxRecordSession.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxRecordSession.CheckedChanged += new System.EventHandler(this.checkBoxRecordSession_CheckedChanged);
            // 
            // checkBoxSetAltitud
            // 
            this.checkBoxSetAltitud.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxSetAltitud.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxSetAltitud.Location = new System.Drawing.Point(310, 118);
            this.checkBoxSetAltitud.Name = "checkBoxSetAltitud";
            this.checkBoxSetAltitud.Size = new System.Drawing.Size(58, 18);
            this.checkBoxSetAltitud.TabIndex = 79;
            this.checkBoxSetAltitud.Text = "Set";
            this.checkBoxSetAltitud.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label39
            // 
            this.label39.Location = new System.Drawing.Point(272, 118);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(41, 18);
            this.label39.TabIndex = 78;
            this.label39.Text = "Km.";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(-24, 118);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(211, 18);
            this.label38.TabIndex = 77;
            this.label38.Text = "Start altitude on tracked source:";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownAltitud
            // 
            this.numericUpDownAltitud.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownAltitud.Location = new System.Drawing.Point(188, 116);
            this.numericUpDownAltitud.Maximum = new decimal(new int[] {
            13000,
            0,
            0,
            0});
            this.numericUpDownAltitud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownAltitud.Name = "numericUpDownAltitud";
            this.numericUpDownAltitud.Size = new System.Drawing.Size(77, 21);
            this.numericUpDownAltitud.TabIndex = 76;
            this.numericUpDownAltitud.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(-35, 179);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(549, 8);
            this.groupBox3.TabIndex = 82;
            this.groupBox3.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(-35, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(549, 9);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            // 
            // label59
            // 
            this.label59.Location = new System.Drawing.Point(145, 83);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(234, 17);
            this.label59.TabIndex = 74;
            this.label59.Text = "Track always on top of Terrain";
            // 
            // checkBoxTrackOnTop
            // 
            this.checkBoxTrackOnTop.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxTrackOnTop.Checked = true;
            this.checkBoxTrackOnTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTrackOnTop.Enabled = false;
            this.checkBoxTrackOnTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTrackOnTop.Location = new System.Drawing.Point(-18, 82);
            this.checkBoxTrackOnTop.Name = "checkBoxTrackOnTop";
            this.checkBoxTrackOnTop.Size = new System.Drawing.Size(154, 18);
            this.checkBoxTrackOnTop.TabIndex = 73;
            this.checkBoxTrackOnTop.Text = "Track On Top";
            this.checkBoxTrackOnTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(145, 64);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(211, 17);
            this.label16.TabIndex = 71;
            this.label16.Text = "[Left Alt+Click on GPS Icon]";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(145, 45);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(211, 17);
            this.label15.TabIndex = 70;
            this.label15.Text = "[Right Control+Click on GPS Icon]";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(145, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(221, 17);
            this.label10.TabIndex = 69;
            this.label10.Text = "[Left Control+Click on GPS Icon]";
            // 
            // checkBoxTrackLine
            // 
            this.checkBoxTrackLine.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxTrackLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTrackLine.Location = new System.Drawing.Point(-18, 64);
            this.checkBoxTrackLine.Name = "checkBoxTrackLine";
            this.checkBoxTrackLine.Size = new System.Drawing.Size(154, 17);
            this.checkBoxTrackLine.TabIndex = 68;
            this.checkBoxTrackLine.Text = "Track line";
            this.checkBoxTrackLine.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxTrackHeading
            // 
            this.checkBoxTrackHeading.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxTrackHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTrackHeading.Location = new System.Drawing.Point(11, 45);
            this.checkBoxTrackHeading.Name = "checkBoxTrackHeading";
            this.checkBoxTrackHeading.Size = new System.Drawing.Size(125, 17);
            this.checkBoxTrackHeading.TabIndex = 65;
            this.checkBoxTrackHeading.Text = "Track Heading";
            this.checkBoxTrackHeading.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxVExaggeration
            // 
            this.checkBoxVExaggeration.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxVExaggeration.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxVExaggeration.Location = new System.Drawing.Point(-18, 9);
            this.checkBoxVExaggeration.Name = "checkBoxVExaggeration";
            this.checkBoxVExaggeration.Size = new System.Drawing.Size(154, 18);
            this.checkBoxVExaggeration.TabIndex = 66;
            this.checkBoxVExaggeration.Text = "2D Map";
            this.checkBoxVExaggeration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox8);
            this.groupBox6.Location = new System.Drawing.Point(-32, 99);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(586, 9);
            this.groupBox6.TabIndex = 72;
            this.groupBox6.TabStop = false;
            // 
            // groupBox8
            // 
            this.groupBox8.Location = new System.Drawing.Point(0, -11);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(442, 9);
            this.groupBox8.TabIndex = 30;
            this.groupBox8.TabStop = false;
            // 
            // checkBoxInformationText
            // 
            this.checkBoxInformationText.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxInformationText.Checked = true;
            this.checkBoxInformationText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxInformationText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxInformationText.Location = new System.Drawing.Point(-18, 27);
            this.checkBoxInformationText.Name = "checkBoxInformationText";
            this.checkBoxInformationText.Size = new System.Drawing.Size(154, 17);
            this.checkBoxInformationText.TabIndex = 67;
            this.checkBoxInformationText.Text = "Information Text";
            this.checkBoxInformationText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPageUnits
            // 
            this.tabPageUnits.Controls.Add(this.label83);
            this.tabPageUnits.Controls.Add(this.comboBoxDistanceUnits);
            this.tabPageUnits.Controls.Add(this.label82);
            this.tabPageUnits.Controls.Add(this.comboBoxPositionUnits);
            this.tabPageUnits.Location = new System.Drawing.Point(4, 22);
            this.tabPageUnits.Name = "tabPageUnits";
            this.tabPageUnits.Size = new System.Drawing.Size(493, 351);
            this.tabPageUnits.TabIndex = 5;
            this.tabPageUnits.Text = "Units";
            this.tabPageUnits.UseVisualStyleBackColor = true;
            // 
            // label83
            // 
            this.label83.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label83.Location = new System.Drawing.Point(77, 79);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(205, 14);
            this.label83.TabIndex = 3;
            this.label83.Text = "Distance, Speed, Altitude:";
            // 
            // comboBoxDistanceUnits
            // 
            this.comboBoxDistanceUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDistanceUnits.FormattingEnabled = true;
            this.comboBoxDistanceUnits.Items.AddRange(new object[] {
            "Metric (Meters, Kilometers,  Kms/Hour)",
            "Imperial/US (Miles,Miles/Hour, Feet)",
            "Maritime (Nautical Miles, Knots, Feet)"});
            this.comboBoxDistanceUnits.Location = new System.Drawing.Point(77, 96);
            this.comboBoxDistanceUnits.Name = "comboBoxDistanceUnits";
            this.comboBoxDistanceUnits.Size = new System.Drawing.Size(343, 20);
            this.comboBoxDistanceUnits.TabIndex = 2;
            // 
            // label82
            // 
            this.label82.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label82.Location = new System.Drawing.Point(77, 23);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(189, 14);
            this.label82.TabIndex = 1;
            this.label82.Text = "Position:";
            // 
            // comboBoxPositionUnits
            // 
            this.comboBoxPositionUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPositionUnits.FormattingEnabled = true;
            this.comboBoxPositionUnits.Items.AddRange(new object[] {
            "Degrees Decimal",
            "Degrees Minutes"});
            this.comboBoxPositionUnits.Location = new System.Drawing.Point(77, 40);
            this.comboBoxPositionUnits.Name = "comboBoxPositionUnits";
            this.comboBoxPositionUnits.Size = new System.Drawing.Size(343, 20);
            this.comboBoxPositionUnits.TabIndex = 0;
            // 
            // tabPageNMEAExport
            // 
            this.tabPageNMEAExport.Controls.Add(this.buttonExportPathBrowse);
            this.tabPageNMEAExport.Controls.Add(this.textBoxNMEAExportPath);
            this.tabPageNMEAExport.Controls.Add(this.label46);
            this.tabPageNMEAExport.Location = new System.Drawing.Point(4, 22);
            this.tabPageNMEAExport.Name = "tabPageNMEAExport";
            this.tabPageNMEAExport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNMEAExport.Size = new System.Drawing.Size(493, 351);
            this.tabPageNMEAExport.TabIndex = 1;
            this.tabPageNMEAExport.Text = "NMEA Export";
            this.tabPageNMEAExport.UseVisualStyleBackColor = true;
            // 
            // buttonExportPathBrowse
            // 
            this.buttonExportPathBrowse.Location = new System.Drawing.Point(446, 41);
            this.buttonExportPathBrowse.Name = "buttonExportPathBrowse";
            this.buttonExportPathBrowse.Size = new System.Drawing.Size(28, 23);
            this.buttonExportPathBrowse.TabIndex = 64;
            this.buttonExportPathBrowse.Text = "...";
            this.buttonExportPathBrowse.UseVisualStyleBackColor = true;
            this.buttonExportPathBrowse.Click += new System.EventHandler(this.buttonExportPathBrowse_Click);
            // 
            // textBoxNMEAExportPath
            // 
            this.textBoxNMEAExportPath.Location = new System.Drawing.Point(11, 41);
            this.textBoxNMEAExportPath.Name = "textBoxNMEAExportPath";
            this.textBoxNMEAExportPath.Size = new System.Drawing.Size(428, 21);
            this.textBoxNMEAExportPath.TabIndex = 63;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.Location = new System.Drawing.Point(7, 24);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(140, 13);
            this.label46.TabIndex = 67;
            this.label46.Text = "NMEA Export File Path:";
            // 
            // tabPageConfiguration
            // 
            this.tabPageConfiguration.Controls.Add(this.buttonSaveConfig);
            this.tabPageConfiguration.Controls.Add(this.buttonLoadConfigurationFile);
            this.tabPageConfiguration.Controls.Add(this.groupBox17);
            this.tabPageConfiguration.Controls.Add(this.textBoxSaveConfiguration);
            this.tabPageConfiguration.Controls.Add(this.buttonSaveConfiguration);
            this.tabPageConfiguration.Controls.Add(this.label68);
            this.tabPageConfiguration.Controls.Add(this.groupBox16);
            this.tabPageConfiguration.Controls.Add(this.comboBoxConfigurationFile);
            this.tabPageConfiguration.Controls.Add(this.buttonLoadConfiguration);
            this.tabPageConfiguration.Controls.Add(this.label67);
            this.tabPageConfiguration.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfiguration.Name = "tabPageConfiguration";
            this.tabPageConfiguration.Size = new System.Drawing.Size(493, 351);
            this.tabPageConfiguration.TabIndex = 4;
            this.tabPageConfiguration.Text = "Configuration";
            this.tabPageConfiguration.UseVisualStyleBackColor = true;
            // 
            // buttonSaveConfig
            // 
            this.buttonSaveConfig.Location = new System.Drawing.Point(355, 173);
            this.buttonSaveConfig.Name = "buttonSaveConfig";
            this.buttonSaveConfig.Size = new System.Drawing.Size(84, 23);
            this.buttonSaveConfig.TabIndex = 44;
            this.buttonSaveConfig.Text = "Save";
            this.buttonSaveConfig.UseVisualStyleBackColor = true;
            this.buttonSaveConfig.Click += new System.EventHandler(this.buttonSaveConfig_Click);
            // 
            // buttonLoadConfigurationFile
            // 
            this.buttonLoadConfigurationFile.Location = new System.Drawing.Point(355, 65);
            this.buttonLoadConfigurationFile.Name = "buttonLoadConfigurationFile";
            this.buttonLoadConfigurationFile.Size = new System.Drawing.Size(84, 22);
            this.buttonLoadConfigurationFile.TabIndex = 43;
            this.buttonLoadConfigurationFile.Text = "Load";
            this.buttonLoadConfigurationFile.UseVisualStyleBackColor = true;
            this.buttonLoadConfigurationFile.Click += new System.EventHandler(this.buttonLoadConfigurationFile_Click);
            // 
            // groupBox17
            // 
            this.groupBox17.Enabled = false;
            this.groupBox17.Location = new System.Drawing.Point(-32, 202);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(553, 9);
            this.groupBox17.TabIndex = 42;
            this.groupBox17.TabStop = false;
            // 
            // textBoxSaveConfiguration
            // 
            this.textBoxSaveConfiguration.Location = new System.Drawing.Point(7, 145);
            this.textBoxSaveConfiguration.Name = "textBoxSaveConfiguration";
            this.textBoxSaveConfiguration.Size = new System.Drawing.Size(432, 21);
            this.textBoxSaveConfiguration.TabIndex = 41;
            this.textBoxSaveConfiguration.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSaveConfiguration_KeyUp);
            // 
            // buttonSaveConfiguration
            // 
            this.buttonSaveConfiguration.Location = new System.Drawing.Point(446, 145);
            this.buttonSaveConfiguration.Name = "buttonSaveConfiguration";
            this.buttonSaveConfiguration.Size = new System.Drawing.Size(35, 22);
            this.buttonSaveConfiguration.TabIndex = 40;
            this.buttonSaveConfiguration.Text = "...";
            this.buttonSaveConfiguration.Click += new System.EventHandler(this.buttonSaveConfiguration_Click);
            // 
            // label68
            // 
            this.label68.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label68.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label68.Location = new System.Drawing.Point(4, 125);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(373, 17);
            this.label68.TabIndex = 39;
            this.label68.Text = "Save current Configuration as...";
            this.label68.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox16
            // 
            this.groupBox16.Enabled = false;
            this.groupBox16.Location = new System.Drawing.Point(-32, 99);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(537, 9);
            this.groupBox16.TabIndex = 38;
            this.groupBox16.TabStop = false;
            // 
            // comboBoxConfigurationFile
            // 
            this.comboBoxConfigurationFile.Location = new System.Drawing.Point(7, 36);
            this.comboBoxConfigurationFile.Name = "comboBoxConfigurationFile";
            this.comboBoxConfigurationFile.Size = new System.Drawing.Size(432, 20);
            this.comboBoxConfigurationFile.TabIndex = 35;
            this.comboBoxConfigurationFile.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxConfigurationFile_KeyUp);
            // 
            // buttonLoadConfiguration
            // 
            this.buttonLoadConfiguration.Location = new System.Drawing.Point(446, 36);
            this.buttonLoadConfiguration.Name = "buttonLoadConfiguration";
            this.buttonLoadConfiguration.Size = new System.Drawing.Size(35, 22);
            this.buttonLoadConfiguration.TabIndex = 34;
            this.buttonLoadConfiguration.Text = "...";
            this.buttonLoadConfiguration.Click += new System.EventHandler(this.buttonLoadConfiguration_Click);
            // 
            // label67
            // 
            this.label67.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label67.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label67.Location = new System.Drawing.Point(4, 15);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(373, 17);
            this.label67.TabIndex = 33;
            this.label67.Text = "Load a Configuration File:";
            this.label67.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageKeyboardShortcuts
            // 
            this.tabPageKeyboardShortcuts.Controls.Add(this.label18);
            this.tabPageKeyboardShortcuts.Controls.Add(this.label24);
            this.tabPageKeyboardShortcuts.Controls.Add(this.label78);
            this.tabPageKeyboardShortcuts.Controls.Add(this.label79);
            this.tabPageKeyboardShortcuts.Controls.Add(this.label33);
            this.tabPageKeyboardShortcuts.Controls.Add(this.label17);
            this.tabPageKeyboardShortcuts.Controls.Add(this.label61);
            this.tabPageKeyboardShortcuts.Controls.Add(this.label58);
            this.tabPageKeyboardShortcuts.Location = new System.Drawing.Point(4, 22);
            this.tabPageKeyboardShortcuts.Name = "tabPageKeyboardShortcuts";
            this.tabPageKeyboardShortcuts.Size = new System.Drawing.Size(493, 351);
            this.tabPageKeyboardShortcuts.TabIndex = 3;
            this.tabPageKeyboardShortcuts.Text = "Keyboard Shortcuts";
            this.tabPageKeyboardShortcuts.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(239, 75);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(203, 12);
            this.label18.TabIndex = 75;
            this.label18.Text = "Right Ctrl + Click on Source Icon";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(29, 75);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(131, 13);
            this.label24.TabIndex = 74;
            this.label24.Text = "File Playback Control:";
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(239, 54);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(161, 12);
            this.label78.TabIndex = 73;
            this.label78.Text = "Right Alt + Click on World";
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label79.Location = new System.Drawing.Point(29, 54);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(124, 13);
            this.label79.TabIndex = 72;
            this.label79.Text = "Create a Track Line:";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(239, 33);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(167, 12);
            this.label33.TabIndex = 71;
            this.label33.Text = "Left Shift + Click on World";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(239, 13);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(197, 12);
            this.label17.TabIndex = 69;
            this.label17.Text = "Left Ctrl + Click on Source Icon";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.Location = new System.Drawing.Point(29, 33);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(169, 13);
            this.label61.TabIndex = 68;
            this.label61.Text = "Set a Point of Interest (POI):";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label58.Location = new System.Drawing.Point(29, 13);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(92, 13);
            this.label58.TabIndex = 66;
            this.label58.Text = "Source Option:";
            // 
            // tabPageFileHelp
            // 
            this.tabPageFileHelp.Controls.Add(this.textBox5);
            this.tabPageFileHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPageFileHelp.Name = "tabPageFileHelp";
            this.tabPageFileHelp.Size = new System.Drawing.Size(519, 385);
            this.tabPageFileHelp.TabIndex = 13;
            this.tabPageFileHelp.Text = "Help";
            this.tabPageFileHelp.UseVisualStyleBackColor = true;
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.Color.White;
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox5.Location = new System.Drawing.Point(10, 5);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox5.Size = new System.Drawing.Size(499, 375);
            this.textBox5.TabIndex = 6;
            this.textBox5.Text = resources.GetString("textBox5.Text");
            // 
            // tabPageUsage
            // 
            this.tabPageUsage.Controls.Add(this.textBoxUsage);
            this.tabPageUsage.Controls.Add(this.pictureBoxHelpLogo);
            this.tabPageUsage.Location = new System.Drawing.Point(4, 22);
            this.tabPageUsage.Name = "tabPageUsage";
            this.tabPageUsage.Size = new System.Drawing.Size(519, 385);
            this.tabPageUsage.TabIndex = 5;
            this.tabPageUsage.Text = "Help";
            this.tabPageUsage.UseVisualStyleBackColor = true;
            this.tabPageUsage.Visible = false;
            // 
            // textBoxUsage
            // 
            this.textBoxUsage.BackColor = System.Drawing.Color.White;
            this.textBoxUsage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxUsage.Location = new System.Drawing.Point(10, 5);
            this.textBoxUsage.Multiline = true;
            this.textBoxUsage.Name = "textBoxUsage";
            this.textBoxUsage.ReadOnly = true;
            this.textBoxUsage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxUsage.Size = new System.Drawing.Size(499, 375);
            this.textBoxUsage.TabIndex = 5;
            this.textBoxUsage.Text = resources.GetString("textBoxUsage.Text");
            // 
            // pictureBoxHelpLogo
            // 
            this.pictureBoxHelpLogo.Location = new System.Drawing.Point(10, 9);
            this.pictureBoxHelpLogo.Name = "pictureBoxHelpLogo";
            this.pictureBoxHelpLogo.Size = new System.Drawing.Size(57, 48);
            this.pictureBoxHelpLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxHelpLogo.TabIndex = 37;
            this.pictureBoxHelpLogo.TabStop = false;
            this.pictureBoxHelpLogo.Visible = false;
            // 
            // tabPageTCP
            // 
            this.tabPageTCP.Controls.Add(this.checkBoxTCPExport);
            this.tabPageTCP.Controls.Add(this.comboBoxTcpIP);
            this.tabPageTCP.Controls.Add(this.checkBoxSecureSocket);
            this.tabPageTCP.Controls.Add(this.buttonApplyTCP);
            this.tabPageTCP.Controls.Add(this.label22);
            this.tabPageTCP.Controls.Add(this.buttonTrackColorTCP);
            this.tabPageTCP.Controls.Add(this.label8);
            this.tabPageTCP.Controls.Add(this.label7);
            this.tabPageTCP.Controls.Add(this.numericUpDownTCPPort);
            this.tabPageTCP.Location = new System.Drawing.Point(4, 22);
            this.tabPageTCP.Name = "tabPageTCP";
            this.tabPageTCP.Size = new System.Drawing.Size(519, 385);
            this.tabPageTCP.TabIndex = 2;
            this.tabPageTCP.Text = "TCP";
            this.tabPageTCP.UseVisualStyleBackColor = true;
            this.tabPageTCP.Visible = false;
            // 
            // checkBoxTCPExport
            // 
            this.checkBoxTCPExport.Location = new System.Drawing.Point(158, 153);
            this.checkBoxTCPExport.Name = "checkBoxTCPExport";
            this.checkBoxTCPExport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTCPExport.Size = new System.Drawing.Size(201, 18);
            this.checkBoxTCPExport.TabIndex = 51;
            this.checkBoxTCPExport.Text = "Export as NMEA to File";
            this.checkBoxTCPExport.UseVisualStyleBackColor = true;
            this.checkBoxTCPExport.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboBoxTcpIP
            // 
            this.comboBoxTcpIP.Location = new System.Drawing.Point(205, 17);
            this.comboBoxTcpIP.Name = "comboBoxTcpIP";
            this.comboBoxTcpIP.Size = new System.Drawing.Size(250, 20);
            this.comboBoxTcpIP.TabIndex = 50;
            this.comboBoxTcpIP.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            this.comboBoxTcpIP.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // checkBoxSecureSocket
            // 
            this.checkBoxSecureSocket.Location = new System.Drawing.Point(52, 129);
            this.checkBoxSecureSocket.Name = "checkBoxSecureSocket";
            this.checkBoxSecureSocket.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxSecureSocket.Size = new System.Drawing.Size(307, 17);
            this.checkBoxSecureSocket.TabIndex = 49;
            this.checkBoxSecureSocket.Text = "Use SSL3 or TLS1 Secure TCP Connection";
            this.checkBoxSecureSocket.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonApplyTCP
            // 
            this.buttonApplyTCP.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyTCP.Name = "buttonApplyTCP";
            this.buttonApplyTCP.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyTCP.TabIndex = 48;
            this.buttonApplyTCP.Text = "Apply";
            this.buttonApplyTCP.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(80, 86);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(116, 17);
            this.label22.TabIndex = 43;
            this.label22.Text = "Track Color:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrackColorTCP
            // 
            this.buttonTrackColorTCP.Location = new System.Drawing.Point(205, 86);
            this.buttonTrackColorTCP.Name = "buttonTrackColorTCP";
            this.buttonTrackColorTCP.Size = new System.Drawing.Size(154, 14);
            this.buttonTrackColorTCP.TabIndex = 42;
            this.buttonTrackColorTCP.BackColorChanged += new System.EventHandler(this.ControlValueChanged);
            this.buttonTrackColorTCP.Click += new System.EventHandler(this.buttonTrackColorTCP_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(23, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(182, 26);
            this.label8.TabIndex = 22;
            this.label8.Text = "Gps Server IP/Name:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(109, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 26);
            this.label7.TabIndex = 20;
            this.label7.Text = "Port Number:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownTCPPort
            // 
            this.numericUpDownTCPPort.Location = new System.Drawing.Point(205, 52);
            this.numericUpDownTCPPort.Maximum = new decimal(new int[] {
            65519,
            0,
            0,
            0});
            this.numericUpDownTCPPort.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTCPPort.Name = "numericUpDownTCPPort";
            this.numericUpDownTCPPort.Size = new System.Drawing.Size(154, 21);
            this.numericUpDownTCPPort.TabIndex = 0;
            this.numericUpDownTCPPort.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTCPPort.ValueChanged += new System.EventHandler(this.ControlValueChanged);
            this.numericUpDownTCPPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlValueChanged);
            // 
            // tabPageFile
            // 
            this.tabPageFile.Controls.Add(this.checkBoxTrackExport);
            this.tabPageFile.Controls.Add(this.comboBoxTrackFileType);
            this.tabPageFile.Controls.Add(this.label41);
            this.tabPageFile.Controls.Add(this.label37);
            this.tabPageFile.Controls.Add(this.label36);
            this.tabPageFile.Controls.Add(this.numericUpDownReload);
            this.tabPageFile.Controls.Add(this.buttonApplyFile);
            this.tabPageFile.Controls.Add(this.label19);
            this.tabPageFile.Controls.Add(this.buttonTrackColor);
            this.tabPageFile.Controls.Add(this.checkBoxForcePreprocessing);
            this.tabPageFile.Controls.Add(this.labelPreprocessing);
            this.tabPageFile.Controls.Add(this.progressBarPreprocessing);
            this.tabPageFile.Controls.Add(this.checkBoxTrackAtOnce);
            this.tabPageFile.Controls.Add(this.checkBoxNoDelay);
            this.tabPageFile.Controls.Add(this.comboBoxFile);
            this.tabPageFile.Controls.Add(this.label13);
            this.tabPageFile.Controls.Add(this.label12);
            this.tabPageFile.Controls.Add(this.trackBarFileSpeed);
            this.tabPageFile.Controls.Add(this.buttonBrowseGpsFile);
            this.tabPageFile.Controls.Add(this.label9);
            this.tabPageFile.Controls.Add(this.label14);
            this.tabPageFile.Controls.Add(this.groupBox5);
            this.tabPageFile.Location = new System.Drawing.Point(4, 22);
            this.tabPageFile.Name = "tabPageFile";
            this.tabPageFile.Size = new System.Drawing.Size(519, 385);
            this.tabPageFile.TabIndex = 3;
            this.tabPageFile.Text = "Tracks | Routes | Sessions";
            this.tabPageFile.UseVisualStyleBackColor = true;
            this.tabPageFile.Visible = false;
            // 
            // checkBoxTrackExport
            // 
            this.checkBoxTrackExport.Location = new System.Drawing.Point(194, 282);
            this.checkBoxTrackExport.Name = "checkBoxTrackExport";
            this.checkBoxTrackExport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTrackExport.Size = new System.Drawing.Size(212, 18);
            this.checkBoxTrackExport.TabIndex = 55;
            this.checkBoxTrackExport.Text = "Export as NMEA to File";
            this.checkBoxTrackExport.UseVisualStyleBackColor = true;
            this.checkBoxTrackExport.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboBoxTrackFileType
            // 
            this.comboBoxTrackFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrackFileType.Location = new System.Drawing.Point(12, 27);
            this.comboBoxTrackFileType.Name = "comboBoxTrackFileType";
            this.comboBoxTrackFileType.Size = new System.Drawing.Size(502, 20);
            this.comboBoxTrackFileType.TabIndex = 54;
            this.comboBoxTrackFileType.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label41
            // 
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label41.Location = new System.Drawing.Point(8, 10);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(327, 17);
            this.label41.TabIndex = 53;
            this.label41.Text = "File Type:";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(161, 100);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(221, 17);
            this.label37.TabIndex = 52;
            this.label37.Text = "seconds after playback is done.";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(-1, 100);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(86, 17);
            this.label36.TabIndex = 51;
            this.label36.Text = "Reload after";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownReload
            // 
            this.numericUpDownReload.Location = new System.Drawing.Point(91, 100);
            this.numericUpDownReload.Maximum = new decimal(new int[] {
            1800,
            0,
            0,
            0});
            this.numericUpDownReload.Name = "numericUpDownReload";
            this.numericUpDownReload.Size = new System.Drawing.Size(67, 21);
            this.numericUpDownReload.TabIndex = 50;
            this.numericUpDownReload.ValueChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonApplyFile
            // 
            this.buttonApplyFile.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyFile.Name = "buttonApplyFile";
            this.buttonApplyFile.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyFile.TabIndex = 48;
            this.buttonApplyFile.Text = "Apply";
            this.buttonApplyFile.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(271, 226);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(115, 17);
            this.label19.TabIndex = 39;
            this.label19.Text = "Track Color";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrackColor
            // 
            this.buttonTrackColor.Location = new System.Drawing.Point(389, 227);
            this.buttonTrackColor.Name = "buttonTrackColor";
            this.buttonTrackColor.Size = new System.Drawing.Size(15, 14);
            this.buttonTrackColor.TabIndex = 38;
            this.buttonTrackColor.BackColorChanged += new System.EventHandler(this.ControlValueChanged);
            this.buttonTrackColor.Click += new System.EventHandler(this.buttonTrackColor_Click);
            // 
            // checkBoxForcePreprocessing
            // 
            this.checkBoxForcePreprocessing.Location = new System.Drawing.Point(156, 261);
            this.checkBoxForcePreprocessing.Name = "checkBoxForcePreprocessing";
            this.checkBoxForcePreprocessing.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxForcePreprocessing.Size = new System.Drawing.Size(250, 23);
            this.checkBoxForcePreprocessing.TabIndex = 37;
            this.checkBoxForcePreprocessing.Text = "Force Track at Once Preprocessing";
            this.checkBoxForcePreprocessing.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // labelPreprocessing
            // 
            this.labelPreprocessing.Location = new System.Drawing.Point(4, 354);
            this.labelPreprocessing.Name = "labelPreprocessing";
            this.labelPreprocessing.Size = new System.Drawing.Size(96, 18);
            this.labelPreprocessing.TabIndex = 36;
            this.labelPreprocessing.Text = "Preprocessing:";
            this.labelPreprocessing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressBarPreprocessing
            // 
            this.progressBarPreprocessing.Location = new System.Drawing.Point(108, 354);
            this.progressBarPreprocessing.Name = "progressBarPreprocessing";
            this.progressBarPreprocessing.Size = new System.Drawing.Size(329, 18);
            this.progressBarPreprocessing.Step = 1;
            this.progressBarPreprocessing.TabIndex = 35;
            // 
            // checkBoxTrackAtOnce
            // 
            this.checkBoxTrackAtOnce.Location = new System.Drawing.Point(194, 246);
            this.checkBoxTrackAtOnce.Name = "checkBoxTrackAtOnce";
            this.checkBoxTrackAtOnce.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTrackAtOnce.Size = new System.Drawing.Size(212, 17);
            this.checkBoxTrackAtOnce.TabIndex = 34;
            this.checkBoxTrackAtOnce.Text = "Track at Once";
            this.checkBoxTrackAtOnce.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // checkBoxNoDelay
            // 
            this.checkBoxNoDelay.Location = new System.Drawing.Point(194, 197);
            this.checkBoxNoDelay.Name = "checkBoxNoDelay";
            this.checkBoxNoDelay.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxNoDelay.Size = new System.Drawing.Size(212, 17);
            this.checkBoxNoDelay.TabIndex = 33;
            this.checkBoxNoDelay.Text = "Maximum Speed";
            this.checkBoxNoDelay.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboBoxFile
            // 
            this.comboBoxFile.Location = new System.Drawing.Point(12, 73);
            this.comboBoxFile.Name = "comboBoxFile";
            this.comboBoxFile.Size = new System.Drawing.Size(469, 20);
            this.comboBoxFile.TabIndex = 32;
            this.comboBoxFile.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            this.comboBoxFile.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label13
            // 
            this.label13.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label13.Location = new System.Drawing.Point(28, 155);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 26);
            this.label13.TabIndex = 30;
            this.label13.Text = "Real Time";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label12.Location = new System.Drawing.Point(28, 139);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(144, 17);
            this.label12.TabIndex = 29;
            this.label12.Text = "Playback Speed:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trackBarFileSpeed
            // 
            this.trackBarFileSpeed.LargeChange = 2;
            this.trackBarFileSpeed.Location = new System.Drawing.Point(85, 157);
            this.trackBarFileSpeed.Minimum = 1;
            this.trackBarFileSpeed.Name = "trackBarFileSpeed";
            this.trackBarFileSpeed.Size = new System.Drawing.Size(331, 45);
            this.trackBarFileSpeed.TabIndex = 28;
            this.trackBarFileSpeed.Value = 1;
            this.trackBarFileSpeed.ValueChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonBrowseGpsFile
            // 
            this.buttonBrowseGpsFile.Location = new System.Drawing.Point(488, 72);
            this.buttonBrowseGpsFile.Name = "buttonBrowseGpsFile";
            this.buttonBrowseGpsFile.Size = new System.Drawing.Size(26, 23);
            this.buttonBrowseGpsFile.TabIndex = 25;
            this.buttonBrowseGpsFile.Text = "...";
            this.buttonBrowseGpsFile.Click += new System.EventHandler(this.buttonBrowseGpsFile_Click);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label9.Location = new System.Drawing.Point(8, 53);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(394, 17);
            this.label9.TabIndex = 24;
            this.label9.Text = "File Name (use http:// for internet downloaded files):";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label14.Location = new System.Drawing.Point(424, 155);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(57, 26);
            this.label14.TabIndex = 31;
            this.label14.Text = "Faster";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox5
            // 
            this.groupBox5.Location = new System.Drawing.Point(-305, 125);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1018, 9);
            this.groupBox5.TabIndex = 49;
            this.groupBox5.TabStop = false;
            // 
            // tabPageUDPHelp
            // 
            this.tabPageUDPHelp.Controls.Add(this.textBox7);
            this.tabPageUDPHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPageUDPHelp.Name = "tabPageUDPHelp";
            this.tabPageUDPHelp.Size = new System.Drawing.Size(519, 385);
            this.tabPageUDPHelp.TabIndex = 15;
            this.tabPageUDPHelp.Text = "Help";
            this.tabPageUDPHelp.UseVisualStyleBackColor = true;
            // 
            // textBox7
            // 
            this.textBox7.BackColor = System.Drawing.Color.White;
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox7.Location = new System.Drawing.Point(10, 5);
            this.textBox7.Multiline = true;
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox7.Size = new System.Drawing.Size(499, 375);
            this.textBox7.TabIndex = 6;
            this.textBox7.Text = resources.GetString("textBox7.Text");
            // 
            // tabPageCOMHelp
            // 
            this.tabPageCOMHelp.Controls.Add(this.textBox2);
            this.tabPageCOMHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPageCOMHelp.Name = "tabPageCOMHelp";
            this.tabPageCOMHelp.Size = new System.Drawing.Size(519, 385);
            this.tabPageCOMHelp.TabIndex = 10;
            this.tabPageCOMHelp.Text = "Help";
            this.tabPageCOMHelp.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(10, 5);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox2.Size = new System.Drawing.Size(499, 375);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = resources.GetString("textBox2.Text");
            // 
            // tabPagePOIHelp
            // 
            this.tabPagePOIHelp.Controls.Add(this.textBox3);
            this.tabPagePOIHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPagePOIHelp.Name = "tabPagePOIHelp";
            this.tabPagePOIHelp.Size = new System.Drawing.Size(519, 385);
            this.tabPagePOIHelp.TabIndex = 11;
            this.tabPagePOIHelp.Text = "Help";
            this.tabPagePOIHelp.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.White;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(10, 5);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox3.Size = new System.Drawing.Size(499, 375);
            this.textBox3.TabIndex = 6;
            this.textBox3.Text = resources.GetString("textBox3.Text");
            // 
            // tabPageTCPHelp
            // 
            this.tabPageTCPHelp.Controls.Add(this.textBox6);
            this.tabPageTCPHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPageTCPHelp.Name = "tabPageTCPHelp";
            this.tabPageTCPHelp.Size = new System.Drawing.Size(519, 385);
            this.tabPageTCPHelp.TabIndex = 14;
            this.tabPageTCPHelp.Text = "Help";
            this.tabPageTCPHelp.UseVisualStyleBackColor = true;
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.Color.White;
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox6.Location = new System.Drawing.Point(10, 5);
            this.textBox6.Multiline = true;
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox6.Size = new System.Drawing.Size(499, 375);
            this.textBox6.TabIndex = 6;
            this.textBox6.Text = resources.GetString("textBox6.Text");
            // 
            // tabPageUDP
            // 
            this.tabPageUDP.Controls.Add(this.checkBoxUDPExport);
            this.tabPageUDP.Controls.Add(this.buttonApplyUDP);
            this.tabPageUDP.Controls.Add(this.label21);
            this.tabPageUDP.Controls.Add(this.buttonTrackColorUDP);
            this.tabPageUDP.Controls.Add(this.label6);
            this.tabPageUDP.Controls.Add(this.numericUpDownUDPPort);
            this.tabPageUDP.Location = new System.Drawing.Point(4, 22);
            this.tabPageUDP.Name = "tabPageUDP";
            this.tabPageUDP.Size = new System.Drawing.Size(519, 385);
            this.tabPageUDP.TabIndex = 1;
            this.tabPageUDP.Text = "UDP";
            this.tabPageUDP.UseVisualStyleBackColor = true;
            this.tabPageUDP.Visible = false;
            // 
            // checkBoxUDPExport
            // 
            this.checkBoxUDPExport.Location = new System.Drawing.Point(161, 87);
            this.checkBoxUDPExport.Name = "checkBoxUDPExport";
            this.checkBoxUDPExport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxUDPExport.Size = new System.Drawing.Size(219, 19);
            this.checkBoxUDPExport.TabIndex = 60;
            this.checkBoxUDPExport.Text = "Export as NMEA to File";
            this.checkBoxUDPExport.UseVisualStyleBackColor = true;
            this.checkBoxUDPExport.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonApplyUDP
            // 
            this.buttonApplyUDP.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyUDP.Name = "buttonApplyUDP";
            this.buttonApplyUDP.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyUDP.TabIndex = 47;
            this.buttonApplyUDP.Text = "Apply";
            this.buttonApplyUDP.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(101, 54);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(115, 17);
            this.label21.TabIndex = 41;
            this.label21.Text = "Track Color:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrackColorUDP
            // 
            this.buttonTrackColorUDP.Location = new System.Drawing.Point(226, 54);
            this.buttonTrackColorUDP.Name = "buttonTrackColorUDP";
            this.buttonTrackColorUDP.Size = new System.Drawing.Size(153, 14);
            this.buttonTrackColorUDP.TabIndex = 40;
            this.buttonTrackColorUDP.BackColorChanged += new System.EventHandler(this.ControlValueChanged);
            this.buttonTrackColorUDP.Click += new System.EventHandler(this.buttonTrackColorUDP_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(130, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 26);
            this.label6.TabIndex = 18;
            this.label6.Text = "Port Number:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownUDPPort
            // 
            this.numericUpDownUDPPort.Location = new System.Drawing.Point(226, 19);
            this.numericUpDownUDPPort.Maximum = new decimal(new int[] {
            65519,
            0,
            0,
            0});
            this.numericUpDownUDPPort.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownUDPPort.Name = "numericUpDownUDPPort";
            this.numericUpDownUDPPort.Size = new System.Drawing.Size(153, 21);
            this.numericUpDownUDPPort.TabIndex = 0;
            this.numericUpDownUDPPort.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownUDPPort.ValueChanged += new System.EventHandler(this.ControlValueChanged);
            this.numericUpDownUDPPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlValueChanged);
            // 
            // tabPageUpdate
            // 
            this.tabPageUpdate.Controls.Add(this.label11);
            this.tabPageUpdate.Controls.Add(this.textBoxVersionInfo);
            this.tabPageUpdate.Controls.Add(this.buttonCheckForUpdates);
            this.tabPageUpdate.Location = new System.Drawing.Point(4, 22);
            this.tabPageUpdate.Name = "tabPageUpdate";
            this.tabPageUpdate.Size = new System.Drawing.Size(519, 385);
            this.tabPageUpdate.TabIndex = 4;
            this.tabPageUpdate.Text = "Update";
            this.tabPageUpdate.UseVisualStyleBackColor = true;
            this.tabPageUpdate.Visible = false;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(19, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(202, 17);
            this.label11.TabIndex = 5;
            this.label11.Text = "Version Information:";
            // 
            // textBoxVersionInfo
            // 
            this.textBoxVersionInfo.BackColor = System.Drawing.Color.White;
            this.textBoxVersionInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxVersionInfo.Location = new System.Drawing.Point(19, 26);
            this.textBoxVersionInfo.Multiline = true;
            this.textBoxVersionInfo.Name = "textBoxVersionInfo";
            this.textBoxVersionInfo.ReadOnly = true;
            this.textBoxVersionInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxVersionInfo.Size = new System.Drawing.Size(480, 322);
            this.textBoxVersionInfo.TabIndex = 4;
            this.textBoxVersionInfo.WordWrap = false;
            // 
            // buttonCheckForUpdates
            // 
            this.buttonCheckForUpdates.Location = new System.Drawing.Point(19, 354);
            this.buttonCheckForUpdates.Name = "buttonCheckForUpdates";
            this.buttonCheckForUpdates.Size = new System.Drawing.Size(480, 26);
            this.buttonCheckForUpdates.TabIndex = 2;
            this.buttonCheckForUpdates.Text = "Check for Updates";
            this.buttonCheckForUpdates.Click += new System.EventHandler(this.buttonCheckForUpdates_Click);
            // 
            // tabPagePOI
            // 
            this.tabPagePOI.Controls.Add(this.checkBoxPOIExport);
            this.tabPagePOI.Controls.Add(this.buttonApplyPOI);
            this.tabPagePOI.Controls.Add(this.label34);
            this.tabPagePOI.Controls.Add(this.label29);
            this.tabPagePOI.Controls.Add(this.label25);
            this.tabPagePOI.Controls.Add(this.textBoxLongitud);
            this.tabPagePOI.Controls.Add(this.textBoxLatitud);
            this.tabPagePOI.Location = new System.Drawing.Point(4, 22);
            this.tabPagePOI.Name = "tabPagePOI";
            this.tabPagePOI.Size = new System.Drawing.Size(519, 385);
            this.tabPagePOI.TabIndex = 9;
            this.tabPagePOI.Text = "POI";
            this.tabPagePOI.UseVisualStyleBackColor = true;
            // 
            // checkBoxPOIExport
            // 
            this.checkBoxPOIExport.Location = new System.Drawing.Point(154, 143);
            this.checkBoxPOIExport.Name = "checkBoxPOIExport";
            this.checkBoxPOIExport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxPOIExport.Size = new System.Drawing.Size(219, 19);
            this.checkBoxPOIExport.TabIndex = 60;
            this.checkBoxPOIExport.Text = "Export as NMEA to File";
            this.checkBoxPOIExport.UseVisualStyleBackColor = true;
            this.checkBoxPOIExport.Visible = false;
            this.checkBoxPOIExport.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonApplyPOI
            // 
            this.buttonApplyPOI.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyPOI.Name = "buttonApplyPOI";
            this.buttonApplyPOI.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyPOI.TabIndex = 57;
            this.buttonApplyPOI.Text = "Apply";
            this.buttonApplyPOI.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(20, 23);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(474, 17);
            this.label34.TabIndex = 4;
            this.label34.Text = "Enter Latitude and Longitude of the POI in degrees decimal.";
            this.label34.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(122, 95);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(77, 26);
            this.label29.TabIndex = 3;
            this.label29.Text = "Longitude:";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(122, 60);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(77, 26);
            this.label25.TabIndex = 2;
            this.label25.Text = "Latitude:";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxLongitud
            // 
            this.textBoxLongitud.Location = new System.Drawing.Point(199, 95);
            this.textBoxLongitud.Name = "textBoxLongitud";
            this.textBoxLongitud.Size = new System.Drawing.Size(173, 21);
            this.textBoxLongitud.TabIndex = 1;
            this.textBoxLongitud.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // textBoxLatitud
            // 
            this.textBoxLatitud.Location = new System.Drawing.Point(199, 60);
            this.textBoxLatitud.Name = "textBoxLatitud";
            this.textBoxLatitud.Size = new System.Drawing.Size(173, 21);
            this.textBoxLatitud.TabIndex = 0;
            this.textBoxLatitud.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // tabPageUSB
            // 
            this.tabPageUSB.Controls.Add(this.checkBoxUSBExport);
            this.tabPageUSB.Controls.Add(this.buttonTestUSB);
            this.tabPageUSB.Controls.Add(this.buttonApplyUSB);
            this.tabPageUSB.Controls.Add(this.label42);
            this.tabPageUSB.Controls.Add(this.buttonTrackColorUSB);
            this.tabPageUSB.Controls.Add(this.label43);
            this.tabPageUSB.Controls.Add(this.comboBoxUSBDevice);
            this.tabPageUSB.Location = new System.Drawing.Point(4, 22);
            this.tabPageUSB.Name = "tabPageUSB";
            this.tabPageUSB.Size = new System.Drawing.Size(519, 385);
            this.tabPageUSB.TabIndex = 16;
            this.tabPageUSB.Text = "USB";
            this.tabPageUSB.UseVisualStyleBackColor = true;
            // 
            // checkBoxUSBExport
            // 
            this.checkBoxUSBExport.Location = new System.Drawing.Point(134, 111);
            this.checkBoxUSBExport.Name = "checkBoxUSBExport";
            this.checkBoxUSBExport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxUSBExport.Size = new System.Drawing.Size(240, 18);
            this.checkBoxUSBExport.TabIndex = 60;
            this.checkBoxUSBExport.Text = "Export as NMEA to File";
            this.checkBoxUSBExport.UseVisualStyleBackColor = true;
            // 
            // buttonTestUSB
            // 
            this.buttonTestUSB.Location = new System.Drawing.Point(7, 354);
            this.buttonTestUSB.Name = "buttonTestUSB";
            this.buttonTestUSB.Size = new System.Drawing.Size(163, 26);
            this.buttonTestUSB.TabIndex = 52;
            this.buttonTestUSB.Text = "Test USB Connection";
            this.buttonTestUSB.Click += new System.EventHandler(this.buttonTestUSB_Click);
            // 
            // buttonApplyUSB
            // 
            this.buttonApplyUSB.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyUSB.Name = "buttonApplyUSB";
            this.buttonApplyUSB.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyUSB.TabIndex = 51;
            this.buttonApplyUSB.Text = "Apply";
            this.buttonApplyUSB.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label42
            // 
            this.label42.Location = new System.Drawing.Point(96, 70);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(115, 17);
            this.label42.TabIndex = 50;
            this.label42.Text = "Track Color:";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrackColorUSB
            // 
            this.buttonTrackColorUSB.BackColor = System.Drawing.Color.DarkOrange;
            this.buttonTrackColorUSB.Location = new System.Drawing.Point(221, 70);
            this.buttonTrackColorUSB.Name = "buttonTrackColorUSB";
            this.buttonTrackColorUSB.Size = new System.Drawing.Size(153, 14);
            this.buttonTrackColorUSB.TabIndex = 49;
            this.buttonTrackColorUSB.UseVisualStyleBackColor = false;
            this.buttonTrackColorUSB.BackColorChanged += new System.EventHandler(this.ControlValueChanged);
            this.buttonTrackColorUSB.Click += new System.EventHandler(this.buttonTrackColorUSB_Click);
            // 
            // label43
            // 
            this.label43.Location = new System.Drawing.Point(86, 25);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(125, 26);
            this.label43.TabIndex = 48;
            this.label43.Text = "Gps Device:";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxUSBDevice
            // 
            this.comboBoxUSBDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUSBDevice.ItemHeight = 12;
            this.comboBoxUSBDevice.Items.AddRange(new object[] {
            "garmin"});
            this.comboBoxUSBDevice.Location = new System.Drawing.Point(221, 25);
            this.comboBoxUSBDevice.Name = "comboBoxUSBDevice";
            this.comboBoxUSBDevice.Size = new System.Drawing.Size(153, 20);
            this.comboBoxUSBDevice.TabIndex = 47;
            this.comboBoxUSBDevice.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // tabPageUSBHelp
            // 
            this.tabPageUSBHelp.Controls.Add(this.textBox8);
            this.tabPageUSBHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPageUSBHelp.Name = "tabPageUSBHelp";
            this.tabPageUSBHelp.Size = new System.Drawing.Size(519, 385);
            this.tabPageUSBHelp.TabIndex = 17;
            this.tabPageUSBHelp.Text = "Help";
            this.tabPageUSBHelp.UseVisualStyleBackColor = true;
            // 
            // textBox8
            // 
            this.textBox8.BackColor = System.Drawing.Color.White;
            this.textBox8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox8.Location = new System.Drawing.Point(10, 5);
            this.textBox8.Multiline = true;
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox8.Size = new System.Drawing.Size(499, 375);
            this.textBox8.TabIndex = 7;
            this.textBox8.Text = resources.GetString("textBox8.Text");
            // 
            // tabPageWaypoints
            // 
            this.tabPageWaypoints.Controls.Add(this.checkBoxWaypointsExport);
            this.tabPageWaypoints.Controls.Add(this.comboBoxWaypointsFileType);
            this.tabPageWaypoints.Controls.Add(this.label44);
            this.tabPageWaypoints.Controls.Add(this.comboBoxWaypointsFile);
            this.tabPageWaypoints.Controls.Add(this.button3);
            this.tabPageWaypoints.Controls.Add(this.listViewWaypoints);
            this.tabPageWaypoints.Controls.Add(this.button1);
            this.tabPageWaypoints.Controls.Add(this.buttonApplyWaypoints);
            this.tabPageWaypoints.Controls.Add(this.label45);
            this.tabPageWaypoints.Controls.Add(this.groupBox7);
            this.tabPageWaypoints.Location = new System.Drawing.Point(4, 22);
            this.tabPageWaypoints.Name = "tabPageWaypoints";
            this.tabPageWaypoints.Size = new System.Drawing.Size(519, 385);
            this.tabPageWaypoints.TabIndex = 18;
            this.tabPageWaypoints.Text = "Waypoints";
            this.tabPageWaypoints.UseVisualStyleBackColor = true;
            // 
            // checkBoxWaypointsExport
            // 
            this.checkBoxWaypointsExport.Location = new System.Drawing.Point(10, 92);
            this.checkBoxWaypointsExport.Name = "checkBoxWaypointsExport";
            this.checkBoxWaypointsExport.Size = new System.Drawing.Size(204, 18);
            this.checkBoxWaypointsExport.TabIndex = 62;
            this.checkBoxWaypointsExport.Text = "Export as NMEA to File";
            this.checkBoxWaypointsExport.UseVisualStyleBackColor = true;
            this.checkBoxWaypointsExport.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // comboBoxWaypointsFileType
            // 
            this.comboBoxWaypointsFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWaypointsFileType.Location = new System.Drawing.Point(10, 20);
            this.comboBoxWaypointsFileType.Name = "comboBoxWaypointsFileType";
            this.comboBoxWaypointsFileType.Size = new System.Drawing.Size(498, 20);
            this.comboBoxWaypointsFileType.TabIndex = 60;
            this.comboBoxWaypointsFileType.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label44.Location = new System.Drawing.Point(6, 3);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(326, 17);
            this.label44.TabIndex = 59;
            this.label44.Text = "File Type:";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxWaypointsFile
            // 
            this.comboBoxWaypointsFile.Location = new System.Drawing.Point(10, 64);
            this.comboBoxWaypointsFile.Name = "comboBoxWaypointsFile";
            this.comboBoxWaypointsFile.Size = new System.Drawing.Size(465, 20);
            this.comboBoxWaypointsFile.TabIndex = 58;
            this.comboBoxWaypointsFile.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(482, 64);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(26, 22);
            this.button3.TabIndex = 57;
            this.button3.Text = "...";
            this.button3.Click += new System.EventHandler(this.buttonBrowseGpsFile_Click);
            // 
            // listViewWaypoints
            // 
            this.listViewWaypoints.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewWaypoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderNumber,
            this.columnHeaderLatitude,
            this.columnHeaderLonguitude,
            this.columnHeaderDescription});
            this.listViewWaypoints.GridLines = true;
            this.listViewWaypoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewWaypoints.LabelWrap = false;
            this.listViewWaypoints.Location = new System.Drawing.Point(10, 134);
            this.listViewWaypoints.MultiSelect = false;
            this.listViewWaypoints.Name = "listViewWaypoints";
            this.listViewWaypoints.Size = new System.Drawing.Size(498, 214);
            this.listViewWaypoints.TabIndex = 56;
            this.listViewWaypoints.UseCompatibleStateImageBehavior = false;
            this.listViewWaypoints.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderNumber
            // 
            this.columnHeaderNumber.Text = "#";
            this.columnHeaderNumber.Width = 35;
            // 
            // columnHeaderLatitude
            // 
            this.columnHeaderLatitude.Text = "Latitude";
            // 
            // columnHeaderLonguitude
            // 
            this.columnHeaderLonguitude.Text = "Longuitude";
            this.columnHeaderLonguitude.Width = 69;
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "Description";
            this.columnHeaderDescription.Width = 676;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 354);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(147, 26);
            this.button1.TabIndex = 54;
            this.button1.Text = "Preview Waypoints";
            this.button1.Click += new System.EventHandler(this.buttonWaypointsList_Click);
            // 
            // buttonApplyWaypoints
            // 
            this.buttonApplyWaypoints.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyWaypoints.Name = "buttonApplyWaypoints";
            this.buttonApplyWaypoints.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyWaypoints.TabIndex = 53;
            this.buttonApplyWaypoints.Text = "Apply";
            this.buttonApplyWaypoints.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label45
            // 
            this.label45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label45.Location = new System.Drawing.Point(6, 46);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(394, 18);
            this.label45.TabIndex = 61;
            this.label45.Text = "File Name (use http:// for internet downloaded files):";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox7
            // 
            this.groupBox7.Location = new System.Drawing.Point(-302, 113);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(1017, 9);
            this.groupBox7.TabIndex = 55;
            this.groupBox7.TabStop = false;
            // 
            // tabPageGeoFence
            // 
            this.tabPageGeoFence.Controls.Add(this.label76);
            this.tabPageGeoFence.Controls.Add(this.buttonTrackColorTrackLine);
            this.tabPageGeoFence.Controls.Add(this.editBoxGeoFenceList);
            this.tabPageGeoFence.Controls.Add(this.listViewGeoFence);
            this.tabPageGeoFence.Controls.Add(this.buttonApplyGeoFence);
            this.tabPageGeoFence.Controls.Add(this.label50);
            this.tabPageGeoFence.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeoFence.Name = "tabPageGeoFence";
            this.tabPageGeoFence.Size = new System.Drawing.Size(519, 385);
            this.tabPageGeoFence.TabIndex = 19;
            this.tabPageGeoFence.Text = "Geo Fence Vertices";
            this.tabPageGeoFence.UseVisualStyleBackColor = true;
            // 
            // label76
            // 
            this.label76.Location = new System.Drawing.Point(-70, 355);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(116, 18);
            this.label76.TabIndex = 72;
            this.label76.Text = "Color:";
            this.label76.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrackColorTrackLine
            // 
            this.buttonTrackColorTrackLine.BackColor = System.Drawing.Color.DarkOrange;
            this.buttonTrackColorTrackLine.Location = new System.Drawing.Point(55, 358);
            this.buttonTrackColorTrackLine.Name = "buttonTrackColorTrackLine";
            this.buttonTrackColorTrackLine.Size = new System.Drawing.Size(154, 14);
            this.buttonTrackColorTrackLine.TabIndex = 71;
            this.buttonTrackColorTrackLine.UseVisualStyleBackColor = false;
            this.buttonTrackColorTrackLine.BackColorChanged += new System.EventHandler(this.ControlValueChanged);
            this.buttonTrackColorTrackLine.Click += new System.EventHandler(this.buttonTrackColorTrackLine_Click);
            // 
            // editBoxGeoFenceList
            // 
            this.editBoxGeoFenceList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editBoxGeoFenceList.Location = new System.Drawing.Point(240, 53);
            this.editBoxGeoFenceList.Name = "editBoxGeoFenceList";
            this.editBoxGeoFenceList.Size = new System.Drawing.Size(106, 21);
            this.editBoxGeoFenceList.TabIndex = 70;
            this.editBoxGeoFenceList.WordWrap = false;
            this.editBoxGeoFenceList.TextChanged += new System.EventHandler(this.ControlValueChanged);
            this.editBoxGeoFenceList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.editBoxGeoFenceList_KeyPress);
            this.editBoxGeoFenceList.Leave += new System.EventHandler(this.editBoxGeoFenceList_Leave);
            // 
            // listViewGeoFence
            // 
            this.listViewGeoFence.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewGeoFence.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewGeoFence.FullRowSelect = true;
            this.listViewGeoFence.GridLines = true;
            this.listViewGeoFence.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewGeoFence.LabelWrap = false;
            this.listViewGeoFence.Location = new System.Drawing.Point(10, 30);
            this.listViewGeoFence.MultiSelect = false;
            this.listViewGeoFence.Name = "listViewGeoFence";
            this.listViewGeoFence.Size = new System.Drawing.Size(504, 318);
            this.listViewGeoFence.TabIndex = 57;
            this.listViewGeoFence.UseCompatibleStateImageBehavior = false;
            this.listViewGeoFence.View = System.Windows.Forms.View.Details;
            this.listViewGeoFence.DoubleClick += new System.EventHandler(this.listViewGeoFence_DoubleClick);
            this.listViewGeoFence.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewGeoFence_MouseDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 31;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Latitude";
            this.columnHeader2.Width = 190;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Longuitude";
            this.columnHeader3.Width = 175;
            // 
            // buttonApplyGeoFence
            // 
            this.buttonApplyGeoFence.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyGeoFence.Name = "buttonApplyGeoFence";
            this.buttonApplyGeoFence.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyGeoFence.TabIndex = 58;
            this.buttonApplyGeoFence.Text = "Apply";
            this.buttonApplyGeoFence.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.Location = new System.Drawing.Point(6, 13);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(57, 13);
            this.label50.TabIndex = 69;
            this.label50.Text = "Vertices:";
            // 
            // tabPageInformationText
            // 
            this.tabPageInformationText.Controls.Add(this.groupBox19);
            this.tabPageInformationText.Controls.Add(this.checkBoxShowComment);
            this.tabPageInformationText.Controls.Add(this.buttonColorInformationText);
            this.tabPageInformationText.Controls.Add(this.buttonApplyInformationText);
            this.tabPageInformationText.Controls.Add(this.comboBoxFontSize);
            this.tabPageInformationText.Controls.Add(this.label75);
            this.tabPageInformationText.Controls.Add(this.label71);
            this.tabPageInformationText.Controls.Add(this.checkBoxTrackDistance);
            this.tabPageInformationText.Controls.Add(this.checkBoxTimeDate);
            this.tabPageInformationText.Controls.Add(this.checkBoxHeading);
            this.tabPageInformationText.Controls.Add(this.checkBoxSpeed);
            this.tabPageInformationText.Controls.Add(this.checkBoxPosition);
            this.tabPageInformationText.Controls.Add(this.checkBoxShowName);
            this.tabPageInformationText.Location = new System.Drawing.Point(4, 22);
            this.tabPageInformationText.Name = "tabPageInformationText";
            this.tabPageInformationText.Size = new System.Drawing.Size(519, 385);
            this.tabPageInformationText.TabIndex = 23;
            this.tabPageInformationText.Text = "Information Text";
            this.tabPageInformationText.UseVisualStyleBackColor = true;
            // 
            // groupBox19
            // 
            this.groupBox19.Location = new System.Drawing.Point(-29, 261);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(575, 8);
            this.groupBox19.TabIndex = 102;
            this.groupBox19.TabStop = false;
            // 
            // checkBoxShowComment
            // 
            this.checkBoxShowComment.AutoSize = true;
            this.checkBoxShowComment.Checked = true;
            this.checkBoxShowComment.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowComment.Location = new System.Drawing.Point(48, 184);
            this.checkBoxShowComment.Name = "checkBoxShowComment";
            this.checkBoxShowComment.Size = new System.Drawing.Size(66, 16);
            this.checkBoxShowComment.TabIndex = 101;
            this.checkBoxShowComment.Text = "Comment";
            this.checkBoxShowComment.UseVisualStyleBackColor = true;
            this.checkBoxShowComment.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // buttonColorInformationText
            // 
            this.buttonColorInformationText.BackColor = System.Drawing.Color.Yellow;
            this.buttonColorInformationText.Location = new System.Drawing.Point(48, 351);
            this.buttonColorInformationText.Name = "buttonColorInformationText";
            this.buttonColorInformationText.Size = new System.Drawing.Size(131, 14);
            this.buttonColorInformationText.TabIndex = 98;
            this.buttonColorInformationText.UseVisualStyleBackColor = false;
            this.buttonColorInformationText.BackColorChanged += new System.EventHandler(this.ControlValueChanged);
            this.buttonColorInformationText.Click += new System.EventHandler(this.buttonColorInformationText_Click);
            // 
            // buttonApplyInformationText
            // 
            this.buttonApplyInformationText.Location = new System.Drawing.Point(446, 354);
            this.buttonApplyInformationText.Name = "buttonApplyInformationText";
            this.buttonApplyInformationText.Size = new System.Drawing.Size(68, 26);
            this.buttonApplyInformationText.TabIndex = 97;
            this.buttonApplyInformationText.Text = "Apply";
            this.buttonApplyInformationText.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // comboBoxFontSize
            // 
            this.comboBoxFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFontSize.FormattingEnabled = true;
            this.comboBoxFontSize.Items.AddRange(new object[] {
            "Normal",
            "Small"});
            this.comboBoxFontSize.Location = new System.Drawing.Point(48, 314);
            this.comboBoxFontSize.Name = "comboBoxFontSize";
            this.comboBoxFontSize.Size = new System.Drawing.Size(131, 20);
            this.comboBoxFontSize.TabIndex = 96;
            this.comboBoxFontSize.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label75.Location = new System.Drawing.Point(44, 284);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(275, 13);
            this.label75.TabIndex = 95;
            this.label75.Text = "Select the information text Font Size and Color:";
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label71.Location = new System.Drawing.Point(44, 13);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(311, 13);
            this.label71.TabIndex = 6;
            this.label71.Text = "Select the information text to display with this source:";
            // 
            // checkBoxTrackDistance
            // 
            this.checkBoxTrackDistance.AutoSize = true;
            this.checkBoxTrackDistance.Checked = true;
            this.checkBoxTrackDistance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTrackDistance.Location = new System.Drawing.Point(48, 159);
            this.checkBoxTrackDistance.Name = "checkBoxTrackDistance";
            this.checkBoxTrackDistance.Size = new System.Drawing.Size(108, 16);
            this.checkBoxTrackDistance.TabIndex = 5;
            this.checkBoxTrackDistance.Text = "Track Distance";
            this.checkBoxTrackDistance.UseVisualStyleBackColor = true;
            this.checkBoxTrackDistance.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // checkBoxTimeDate
            // 
            this.checkBoxTimeDate.AutoSize = true;
            this.checkBoxTimeDate.Checked = true;
            this.checkBoxTimeDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTimeDate.Location = new System.Drawing.Point(48, 135);
            this.checkBoxTimeDate.Name = "checkBoxTimeDate";
            this.checkBoxTimeDate.Size = new System.Drawing.Size(102, 16);
            this.checkBoxTimeDate.TabIndex = 4;
            this.checkBoxTimeDate.Text = "Time and Date";
            this.checkBoxTimeDate.UseVisualStyleBackColor = true;
            this.checkBoxTimeDate.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // checkBoxHeading
            // 
            this.checkBoxHeading.AutoSize = true;
            this.checkBoxHeading.Checked = true;
            this.checkBoxHeading.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHeading.Location = new System.Drawing.Point(48, 110);
            this.checkBoxHeading.Name = "checkBoxHeading";
            this.checkBoxHeading.Size = new System.Drawing.Size(66, 16);
            this.checkBoxHeading.TabIndex = 3;
            this.checkBoxHeading.Text = "Heading";
            this.checkBoxHeading.UseVisualStyleBackColor = true;
            this.checkBoxHeading.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // checkBoxSpeed
            // 
            this.checkBoxSpeed.AutoSize = true;
            this.checkBoxSpeed.Checked = true;
            this.checkBoxSpeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSpeed.Location = new System.Drawing.Point(48, 85);
            this.checkBoxSpeed.Name = "checkBoxSpeed";
            this.checkBoxSpeed.Size = new System.Drawing.Size(54, 16);
            this.checkBoxSpeed.TabIndex = 2;
            this.checkBoxSpeed.Text = "Speed";
            this.checkBoxSpeed.UseVisualStyleBackColor = true;
            this.checkBoxSpeed.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // checkBoxPosition
            // 
            this.checkBoxPosition.AutoSize = true;
            this.checkBoxPosition.Checked = true;
            this.checkBoxPosition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPosition.Location = new System.Drawing.Point(48, 60);
            this.checkBoxPosition.Name = "checkBoxPosition";
            this.checkBoxPosition.Size = new System.Drawing.Size(264, 16);
            this.checkBoxPosition.TabIndex = 1;
            this.checkBoxPosition.Text = "Position (Latitude, Longitude, Altitude)";
            this.checkBoxPosition.UseVisualStyleBackColor = true;
            this.checkBoxPosition.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // checkBoxShowName
            // 
            this.checkBoxShowName.AutoSize = true;
            this.checkBoxShowName.Checked = true;
            this.checkBoxShowName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowName.Location = new System.Drawing.Point(48, 36);
            this.checkBoxShowName.Name = "checkBoxShowName";
            this.checkBoxShowName.Size = new System.Drawing.Size(48, 16);
            this.checkBoxShowName.TabIndex = 0;
            this.checkBoxShowName.Text = "Name";
            this.checkBoxShowName.UseVisualStyleBackColor = true;
            this.checkBoxShowName.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // tabPageTrackLineHelp
            // 
            this.tabPageTrackLineHelp.Controls.Add(this.textBox10);
            this.tabPageTrackLineHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrackLineHelp.Name = "tabPageTrackLineHelp";
            this.tabPageTrackLineHelp.Size = new System.Drawing.Size(519, 385);
            this.tabPageTrackLineHelp.TabIndex = 25;
            this.tabPageTrackLineHelp.Text = "Help";
            this.tabPageTrackLineHelp.UseVisualStyleBackColor = true;
            // 
            // textBox10
            // 
            this.textBox10.BackColor = System.Drawing.Color.White;
            this.textBox10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox10.Location = new System.Drawing.Point(8, 4);
            this.textBox10.Multiline = true;
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox10.Size = new System.Drawing.Size(500, 375);
            this.textBox10.TabIndex = 7;
            this.textBox10.Text = resources.GetString("textBox10.Text");
            // 
            // GpsTracker
            // 
            this.AccessibleName = "";
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(630, 460);
            this.Controls.Add(this.treeViewSources);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.labelSettingup);
            this.Controls.Add(this.progressBarSetup);
            this.Controls.Add(this.labelTrackCode);
            this.Controls.Add(this.StartStop);
            this.Controls.Add(this.tabControlGPS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GpsTracker";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.GpsTracker_Closing);
            this.Load += new System.EventHandler(this.GpsTracker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.tabControlGPS.ResumeLayout(false);
            this.tabPageCOM.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneralSettings.ResumeLayout(false);
            this.tabPageGeneralSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAltitud)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.tabPageUnits.ResumeLayout(false);
            this.tabPageNMEAExport.ResumeLayout(false);
            this.tabPageNMEAExport.PerformLayout();
            this.tabPageConfiguration.ResumeLayout(false);
            this.tabPageConfiguration.PerformLayout();
            this.tabPageKeyboardShortcuts.ResumeLayout(false);
            this.tabPageKeyboardShortcuts.PerformLayout();
            this.tabPageFileHelp.ResumeLayout(false);
            this.tabPageFileHelp.PerformLayout();
            this.tabPageUsage.ResumeLayout(false);
            this.tabPageUsage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelpLogo)).EndInit();
            this.tabPageTCP.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTCPPort)).EndInit();
            this.tabPageFile.ResumeLayout(false);
            this.tabPageFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFileSpeed)).EndInit();
            this.tabPageUDPHelp.ResumeLayout(false);
            this.tabPageUDPHelp.PerformLayout();
            this.tabPageCOMHelp.ResumeLayout(false);
            this.tabPageCOMHelp.PerformLayout();
            this.tabPagePOIHelp.ResumeLayout(false);
            this.tabPagePOIHelp.PerformLayout();
            this.tabPageTCPHelp.ResumeLayout(false);
            this.tabPageTCPHelp.PerformLayout();
            this.tabPageUDP.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUDPPort)).EndInit();
            this.tabPageUpdate.ResumeLayout(false);
            this.tabPageUpdate.PerformLayout();
            this.tabPagePOI.ResumeLayout(false);
            this.tabPagePOI.PerformLayout();
            this.tabPageUSB.ResumeLayout(false);
            this.tabPageUSBHelp.ResumeLayout(false);
            this.tabPageUSBHelp.PerformLayout();
            this.tabPageWaypoints.ResumeLayout(false);
            this.tabPageGeoFence.ResumeLayout(false);
            this.tabPageGeoFence.PerformLayout();
            this.tabPageInformationText.ResumeLayout(false);
            this.tabPageInformationText.PerformLayout();
            this.tabPageTrackLineHelp.ResumeLayout(false);
            this.tabPageTrackLineHelp.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region Start and Stop Plugin
		//
		// GPSTracker Start and Stop functions
		//

		//Close down threads and com port
		public void Deinitialize() 
		{
#if !DEBUG	
			try
			{
#endif
				m_fInitialized=false;
				m_gpsTrackerPlugin.m_fGpsTrackerRunning=false;

				Stop();
				m_gpsTrackerPlugin.pluginRemoveAllOverlay();
#if !DEBUG	
			}
			catch(Exception)
			{
			}
#endif
		}

        //
		//Start all the necessary threads for processing all the selected gps sources
		private bool Start()
		{
			bool fRet=false;
			GPSSource gpsSource;

			if (m_fInitialized)
			{
				progressBarSetup.Visible=false;
				labelSettingup.Visible=false;

				m_iLocked=0;
				m_iPrevLocked=0;
			
				m_fCloseThreads=false;

				m_fVerticalExaggeration=World.Settings.VerticalExaggeration;
				if (checkBoxVExaggeration.Checked==true)
					World.Settings.VerticalExaggeration=0F;

                m_bTrackOnTop = checkBoxTrackOnTop.Checked;
				labelSettingup.Visible=true;
				labelSettingup.Update();
				progressBarSetup.Visible=true;
				progressBarSetup.Update();
				progressBarSetup.Maximum=m_gpsSourceList.Count+1;
                m_iPositionUnit=comboBoxPositionUnits.SelectedIndex;
                m_iDistanceUnit = comboBoxDistanceUnits.SelectedIndex;
				if (m_fPlayback==false)
				{
					for (int i=0; i<m_gpsSourceList.Count; i++)
					{
						gpsSource=(GPSSource)m_gpsSourceList[i];
						gpsSource.datePosition = new DateTime(1771,6,9);
                        gpsSource.bTrackOnTop = checkBoxTrackOnTop.Checked;
                        gpsSource.iPositionUnit = m_iPositionUnit;
                        gpsSource.iDistanceUnit = m_iDistanceUnit;

						if (checkBoxSetAltitud.Checked && gpsSource.bTrack)
							gpsSource.iStartAltitud = Convert.ToInt32(numericUpDownAltitud.Value);
						else
							gpsSource.iStartAltitud=0;

						if (!gpsSource.bSetup)
							continue;

                        gpsSource.GpsPos.m_gpsTrack = null;
                        if (gpsSource.bSave && (i + 1 <= progressBarSetup.Maximum))
						    progressBarSetup.Value=i+1;

                        if (gpsSource.sType == "COM" && gpsSource.GpsCOM.IsOpen)
                            gpsSource.GpsCOM.StartRx();
						else
						if (gpsSource.sType=="POI") //Point of interest
						{
							if (m_hPOIThread==null)
							{
                                m_eventPOIThreadSync = new AutoResetEvent(false);
								m_hPOIThread = new Thread(new ThreadStart(this.threadPOI));
								m_hPOIThread.IsBackground = true;
								m_hPOIThread.Priority = System.Threading.ThreadPriority.Normal; 
								m_hPOIThread.Start();
							}
						}
						else
						{
							if (gpsSource.sType=="TCP")
							{
								SecurityOptions options;
								if (gpsSource.bSecureSocket)
									options = new SecurityOptions(
										SecureProtocol.Ssl3 | SecureProtocol.Tls1,	// use SSL3 or TLS1
										null,										// do not use client authentication
										ConnectionEnd.Client,						// this is the client side
										CredentialVerification.None,				// do not check the certificate -- this should not be used in a real-life application :-)
										null,										// not used with automatic certificate verification
										gpsSource.sTCPAddress,						// this is the common name of the Microsoft web server
										SecurityFlags.Default,						// use the default security flags
										SslAlgorithms.SECURE_CIPHERS,				// only use secure ciphers
										null);
								else
									options = new SecurityOptions(SecureProtocol.None, null, ConnectionEnd.Client);
								SecureSocket socketTCP = new SecureSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, options);
								
								IPHostEntry IPHost = Dns.GetHostEntry(gpsSource.sTCPAddress); 
								IPEndPoint ipEndPoint	= new IPEndPoint(IPHost.AddressList[0], gpsSource.iTCPPort);
								socketTCP.Blocking = true ;
								AsyncCallback callbackProc	= new AsyncCallback(m_UDPTCP.TcpConnectCallback);
                                gpsSource.tcpSockets = new TCPSockets();
                                gpsSource.tcpSockets.sStream = "";
                                gpsSource.tcpSockets.socket = socketTCP;
                                gpsSource.tcpSockets.iDeviceIndex = i;
                                gpsSource.tcpSockets.byTcpBuffer = new byte[1024];
                                gpsSource.tcpSockets.socket.BeginConnect(ipEndPoint, callbackProc, gpsSource.tcpSockets);	
							}
							else
							if (gpsSource.sType=="UDP")
							{
                                //
                                //This thread starts the UDP lisenter
                                gpsSource.eventThreadSync = new AutoResetEvent(false);
                                gpsSource.fileThread = new Thread(new ThreadStart(threadUDP));
                                gpsSource.fileThread.IsBackground = true;
                                gpsSource.fileThread.Priority = System.Threading.ThreadPriority.Normal;
                                gpsSource.fileThread.Name = i.ToString();
                                gpsSource.fileThread.Start(); 
                                 
							}
							else
							if (gpsSource.sType=="File")
							{
                                gpsSource.eventThreadSync = new AutoResetEvent(false);
								gpsSource.fileThread = new Thread(new ThreadStart(m_File.threadStartFile));
								gpsSource.fileThread.IsBackground = true;
								gpsSource.fileThread.Priority = System.Threading.ThreadPriority.Normal; 
								gpsSource.fileThread.Name= i.ToString(); 
								gpsSource.fileThread.Start();
							}
							else
							if (gpsSource.sType=="USB")
							{
                                gpsSource.eventThreadSync = new AutoResetEvent(false);
								gpsSource.usbThread = new Thread(new ThreadStart(m_File.threadStartUSB));
								gpsSource.usbThread.IsBackground = true;
								gpsSource.usbThread.Priority = System.Threading.ThreadPriority.Normal; 
								gpsSource.usbThread.Name= i.ToString(); 
								gpsSource.usbThread.Start();
							}
                            else
							if (gpsSource.sType=="GeoFence")
							{
                                gpsSource.GeoFence.sName = gpsSource.sDescription;
                                gpsSource.GeoFence.bShowInfo = checkBoxInformationText.Checked;
                                GPSRenderInformation renderInfo = new GPSRenderInformation();
                                renderInfo.bShowPosition = gpsSource.bShowPosition;
                                renderInfo.bShowName = gpsSource.bShowName;
                                renderInfo.bShowComment = gpsSource.bShowComment;
                                renderInfo.bShowSpeed = gpsSource.bShowSpeed;
                                renderInfo.bShowHeading = gpsSource.bShowHeading;
                                renderInfo.bShowTimeDate = gpsSource.bShowTimeDate;
                                renderInfo.bShowTrackDistance = gpsSource.bShowTrackDistance;
                                renderInfo.bShowReferenceAngles = gpsSource.bShowReferenceAngles;
                                renderInfo.bShowReferenceDistance = gpsSource.bShowReferenceDistance;
                                renderInfo.iInformationFontSize = gpsSource.iInformationFontSize;
                                renderInfo.colorInformation = gpsSource.colorInformation;
                                renderInfo.iDistanceUnit = gpsSource.iDistanceUnit;
                                renderInfo.iPositionUnit = gpsSource.iPositionUnit;
                                m_gpsTrackerPlugin.pluginAddGeoFence(gpsSource.GeoFence, renderInfo);
                            }
                            else
                                if (gpsSource.sType == "Track Line")
                                {
                                    gpsSource.GeoFence.sName = gpsSource.sDescription;
                                    gpsSource.GeoFence.bShowInfo = checkBoxInformationText.Checked;
                                    GPSRenderInformation renderInfo = new GPSRenderInformation();
                                    renderInfo.bShowPosition = gpsSource.bShowPosition;
                                    renderInfo.bShowName = gpsSource.bShowName;
                                    renderInfo.bShowComment = gpsSource.bShowComment;
                                    renderInfo.bShowSpeed = gpsSource.bShowSpeed;
                                    renderInfo.bShowHeading = gpsSource.bShowHeading;
                                    renderInfo.bShowTimeDate = gpsSource.bShowTimeDate;
                                    renderInfo.bShowTrackDistance = gpsSource.bShowTrackDistance;
                                    renderInfo.bShowReferenceAngles = gpsSource.bShowReferenceAngles;
                                    renderInfo.bShowReferenceDistance = gpsSource.bShowReferenceDistance;
                                    renderInfo.iInformationFontSize = gpsSource.iInformationFontSize;
                                    renderInfo.colorInformation = gpsSource.colorInformation;
                                    renderInfo.iDistanceUnit = gpsSource.iDistanceUnit;
                                    renderInfo.iPositionUnit = gpsSource.iPositionUnit;
                                    m_gpsTrackerPlugin.pluginAddGeoFence(gpsSource.GeoFence, renderInfo);
                            }
						}
					}
				}
				else
				{
					
					for (int i=0; i<m_gpsSourceList.Count; i++)
					{
                        gpsSource = (GPSSource)m_gpsSourceList[i];
                        gpsSource.iReload = 0;
                        gpsSource.datePosition = new DateTime(1771, 6, 9);
                        gpsSource.bTrackOnTop = checkBoxTrackOnTop.Checked;
                        gpsSource.iPositionUnit = m_iPositionUnit;
                        gpsSource.iDistanceUnit = m_iDistanceUnit;

                        if (checkBoxSetAltitud.Checked && gpsSource.bTrack)
                            gpsSource.iStartAltitud = Convert.ToInt32(numericUpDownAltitud.Value);
                        else
                            gpsSource.iStartAltitud = 0;

                        if (!gpsSource.bSetup)
                            continue;

                        gpsSource.GpsPos.m_gpsTrack = null;
                        if ((i + 1 <= progressBarSetup.Maximum))
						    progressBarSetup.Value=i+1;
						if (gpsSource.sType=="POI") //Point of interest
						{
							if (m_hPOIThread==null)
							{
                                m_eventPOIThreadSync = new AutoResetEvent(false);
								m_hPOIThread = new Thread(new ThreadStart(this.threadPOI));
								m_hPOIThread.IsBackground = true;
								m_hPOIThread.Priority = System.Threading.ThreadPriority.Normal; 
								m_hPOIThread.Start();
							}
						}
						else
                        if (gpsSource.sType == "GeoFence")
                        {
                            gpsSource.GeoFence.sName = gpsSource.sDescription;
                            gpsSource.GeoFence.bShowInfo = checkBoxInformationText.Checked;
                            GPSRenderInformation renderInfo = new GPSRenderInformation();
                            renderInfo.bShowPosition = gpsSource.bShowPosition;
                            renderInfo.bShowName = gpsSource.bShowName;
                            renderInfo.bShowComment = gpsSource.bShowComment;
                            renderInfo.bShowSpeed = gpsSource.bShowSpeed;
                            renderInfo.bShowHeading = gpsSource.bShowHeading;
                            renderInfo.bShowTimeDate = gpsSource.bShowTimeDate;
                            renderInfo.bShowTrackDistance = gpsSource.bShowTrackDistance;
                            renderInfo.bShowReferenceAngles = gpsSource.bShowReferenceAngles;
                            renderInfo.bShowReferenceDistance = gpsSource.bShowReferenceDistance;
                            renderInfo.iInformationFontSize = gpsSource.iInformationFontSize;
                            renderInfo.colorInformation = gpsSource.colorInformation;
                            renderInfo.iDistanceUnit = gpsSource.iDistanceUnit;
                            renderInfo.iPositionUnit = gpsSource.iPositionUnit;
                            m_gpsTrackerPlugin.pluginAddGeoFence(gpsSource.GeoFence, renderInfo);
                        }
						else
						{
							if (gpsSource.bTrackAtOnce!=true)
							{
                                gpsSource.eventThreadSync = new AutoResetEvent(false);
                                gpsSource.fileThread = new Thread(new ThreadStart(m_File.threadFile));
                                gpsSource.fileThread.IsBackground = true;
                                gpsSource.fileThread.Priority = System.Threading.ThreadPriority.Normal;
                                gpsSource.fileThread.Name = i.ToString(); 
								gpsSource.sFileNameSession=m_sPlaybackFile;
								if (gpsSource.sType=="File")
									gpsSource.iFilePlaySpeed=gpsSource.iPlaySpeed;
								else
									gpsSource.iFilePlaySpeed=1;
                                gpsSource.fileThread.Start();
							}
							else
							{
								gpsSource.sFileName=gpsSource.sFileName.Replace("*PLUGINDIR*",GpsTrackerPlugin.m_sPluginDirectory);
								if (File.Exists(gpsSource.sFileName))
								{
									//track at once
									m_File.PreprocessFile(i,gpsSource.sFileName,gpsSource.sFileName,true,gpsSource.bForcePreprocessing);

									GPSRenderInformation renderInfo = new GPSRenderInformation();
									renderInfo.bPOI=false;
									renderInfo.iIndex=i;
									renderInfo.sDescription=gpsSource.sDescription;
									renderInfo.fFix=false;
									renderInfo.bShowInfo=m_bInfoText;
									renderInfo.bTrackLine=false;
                                    renderInfo.gpsTrack = gpsSource.GpsPos.m_gpsTrack;
									renderInfo.bRestartTrack=false;
                                    renderInfo.iDay = gpsSource.GpsPos.m_iDay;
                                    renderInfo.iMonth = gpsSource.GpsPos.m_iMonth;
                                    renderInfo.iYear = gpsSource.GpsPos.m_iYear;
									renderInfo.colorTrack=gpsSource.colorTrack;
                                    renderInfo.fTrackOnTop = gpsSource.bTrackOnTop;
                                    renderInfo.bShowPosition = gpsSource.bShowPosition;
                                    renderInfo.bShowName = gpsSource.bShowName;
                                    renderInfo.bShowComment = gpsSource.bShowComment;
                                    renderInfo.bShowSpeed = gpsSource.bShowSpeed;
                                    renderInfo.bShowHeading = gpsSource.bShowHeading;
                                    renderInfo.bShowTimeDate = gpsSource.bShowTimeDate;
                                    renderInfo.bShowTrackDistance = gpsSource.bShowTrackDistance;
                                    renderInfo.bShowReferenceAngles = gpsSource.bShowReferenceAngles;
                                    renderInfo.bShowReferenceDistance = gpsSource.bShowReferenceDistance;
                                    renderInfo.iInformationFontSize = gpsSource.iInformationFontSize;
                                    renderInfo.colorInformation = gpsSource.colorInformation;
                                    renderInfo.iDistanceUnit = gpsSource.iDistanceUnit;
                                    renderInfo.iPositionUnit = gpsSource.iPositionUnit;
									m_gpsTrackerPlugin.pluginShowOverlay(renderInfo);

									if (gpsSource.bTrack==true)
                                        m_gpsTrackerPlugin.pluginWorldWindowGotoLatLonHeading(gpsSource.GpsPos.m_gpsTrack.m_fLat[0], gpsSource.GpsPos.m_gpsTrack.m_fLat[0], -1F, gpsSource.iStartAltitud);
									if (gpsSource.iStartAltitud>0)
									{
										Thread.Sleep(3000);
										gpsSource.iStartAltitud=0;
									}

								}
							}
						}
					}
				}
                if (m_gpsSourceList.Count + 1 <= progressBarSetup.Maximum)
				    progressBarSetup.Value=m_gpsSourceList.Count+1;

				if (m_timerLocked==null)
					m_timerLocked = new System.Threading.Timer(new TimerCallback(timerCallbackLocked),0, 500, 5000);
			
				fRet=true;
			}

			return fRet;
		}

        //
        //This thread starts the UDP lisenter
        public void threadUDP()
        {
            int iIndex = Int32.Parse(Thread.CurrentThread.Name);
            GPSSource gpsSource = (GPSSource)m_gpsSourceList[iIndex];

            try
            {
                EndPoint endPoint = new IPEndPoint(IPAddress.Any, gpsSource.iUDPPort);
                Socket socketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socketUDP.Bind(endPoint);
                gpsSource.tcpSockets = new TCPSockets();
                gpsSource.tcpSockets.sStream = "";
                gpsSource.tcpSockets.socketUDP = socketUDP;
                gpsSource.tcpSockets.iDeviceIndex = iIndex;
                gpsSource.tcpSockets.byTcpBuffer = new byte[1024];
                socketUDP.BeginReceiveFrom(gpsSource.tcpSockets.byTcpBuffer, 0, 1024, SocketFlags.None, ref endPoint, new AsyncCallback(m_UDPTCP.UdpReceiveData), gpsSource.tcpSockets);
            }
            catch (Exception)
            {
            }

            while (m_fCloseThreads == false)
                Thread.Sleep(100);

            gpsSource.eventThreadSync.Set();
        }
        //
        //The thread reading bytes from the COM ports will callback into this function with the latest data from the
        //COM port
		public void COMCallback(int iGPSIndex, string sRawData)
		{
			lock(LockCOM)
			{
                GPSSource gpsSource = (GPSSource)m_gpsSourceList[iGPSIndex];
                gpsSource.sCOMData = gpsSource.sCOMData + sRawData;
				int iIndex=-1;
				char [] cEOL = {'\n', '\r'};
				do
				{
                    iIndex = gpsSource.sCOMData.IndexOfAny(cEOL);
					if (iIndex>=0)
					{
                        string sData = gpsSource.sCOMData.Substring(0, iIndex);
						ShowGPSIcon(sData.ToCharArray(),sData.Length,false,iGPSIndex,false,true,0,0);
                        gpsSource.sCOMData = gpsSource.sCOMData.Remove(0, iIndex + 1);
					}
				}
				while(iIndex>=0);

			}
		}

		//Close down threads, ports, cleanup
		public bool Stop()
		{
			bool fRet=true;

			//Remove temporary waypoints
			int i=0;
			do
			{
				for (i = 0; i < m_gpsSourceList.Count; i++)
				{
					GPSSource gpsSource = (GPSSource)m_gpsSourceList[i];
					if (gpsSource.bSave==false)
					{
						m_gpsSourceList.Remove(gpsSource);
						break;
					}
				}
			} while(i<m_gpsSourceList.Count);


			try
			{
			    if (m_MessageMonitor!=null)
			    {
				    m_MessageMonitor.Close();
				    m_MessageMonitor=null;
			    }
			}
			catch (Exception)
			{
				m_MessageMonitor=null;
			}

			//Restore vertical exaggeration setting.
			if (checkBoxVExaggeration.Checked==true)
				World.Settings.VerticalExaggeration=m_fVerticalExaggeration;

			m_fCloseThreads=true;
			Thread.Sleep(500);

			if (m_timerLocked!=null)
			{
				m_timerLocked.Dispose();
				m_timerLocked=null;
			}

			CloseCOMs();

			if (m_hPOIThread!=null)
			{
                m_eventPOIThreadSync.WaitOne(5000, false);
				m_hPOIThread.Interrupt();
				m_hPOIThread.Abort();
				m_hPOIThread.Join(1000);
				m_hPOIThread=null;
			}

			for (i=0; i<m_gpsSourceList.Count; i++)
			{
                GPSSource gpsSource = (GPSSource)m_gpsSourceList[i];
                if (gpsSource.tcpSockets.socket != null)
				{
                    gpsSource.tcpSockets.socket.Close();
                    gpsSource.tcpSockets.socket = null;
                    gpsSource.tcpSockets.sStream = "";
				}
                if (gpsSource.tcpSockets.socketUDP != null)
                {
                    gpsSource.tcpSockets.socketUDP.Close();
                    gpsSource.tcpSockets.socketUDP = null;
                    gpsSource.tcpSockets.sStream = "";
                }
                if (gpsSource.fileThread != null)
                {
                    gpsSource.eventThreadSync.WaitOne(5000,false);
                    gpsSource.fileThread.Interrupt();
                    gpsSource.fileThread.Abort();
                    gpsSource.fileThread.Join(1000);
                    gpsSource.fileThread = null;
                }
				if (gpsSource.usbThread != null)
				{
                    gpsSource.eventThreadSync.WaitOne(5000, false);
					gpsSource.usbThread.Interrupt();
					gpsSource.usbThread.Abort();
					gpsSource.usbThread.Join(1000);
					gpsSource.usbThread = null;
				}
                if (gpsSource.swExport != null)
                {
                    gpsSource.swExport.Close();
                    gpsSource.swExport = null;
                }
			}

			if (m_swRecorder!=null)
			{
				m_swRecorder.Close();
				m_swRecorder=null;

				#if !DEBUG
				try
				#endif	
				{
					SaveFileDialog dlgSaveFile = new SaveFileDialog();
					dlgSaveFile.Title = "Select file to save recorded Session" ;
					dlgSaveFile.Filter = "GPSTrackerSession (*.GPSTrackerSession)|*.GPSTrackerSession" ;
					dlgSaveFile.FilterIndex = 0 ;
					dlgSaveFile.RestoreDirectory = true ;
					if(dlgSaveFile.ShowDialog() == DialogResult.OK)
                        File.Copy(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.recording", dlgSaveFile.FileName, true);
                    File.Delete(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.recording");
				}
				#if !DEBUG				
				catch(Exception)
				{
				}
				#endif

			}

			if (m_srReader!=null)
			{
				m_srReader.Close();
				m_srReader=null;
			}

			m_gpsTrackerPlugin.pluginRemoveAllOverlay();
			m_gpsTrackerPlugin.gpsOverlay.IsOn = false;

			SaveSettings(null);

            m_fInitialized = false;

			return fRet;
		}

        //Close COM ports
		private void CloseCOMs()
		{
            for (int i = 0; i < m_gpsSourceList.Count; i++)
            {
                GPSSource gpsSource = (GPSSource)m_gpsSourceList[i];
                gpsSource.sCOMData = "";
                if (gpsSource.GpsCOM != null)
				{
                    gpsSource.GpsCOM.Close();
                    gpsSource.GpsCOM = null;
				}
			}
		}

		#endregion

		#region WorldWind renderer functions and callbacks
		//
		// Callback from WorldWind render functions (GPSTrackerPlugin.WorldWind.cs)
		//

        public void SetFileLineNumber(int iIndex, int iLineNumber)
        {
            GPSSource gpsSource = (GPSSource)m_gpsSourceList[iIndex];
            gpsSource.bUpdateLineNumber = true;
            gpsSource.iLineNumber = iLineNumber;


        }

        public void SetFilePlayPause(int iIndex, bool bPause)
        {
            GPSSource gpsSource = (GPSSource)m_gpsSourceList[iIndex];
            gpsSource.bPlay = !bPause;

        }

		public void SetActiveTrack(int iIndex, bool bTrack)
		{
			GPSSource gpsSource;
			if (bTrack)
			{
				for (int i=0; i<m_gpsSourceList.Count; i++)
				{
					gpsSource = (GPSSource)m_gpsSourceList[i];
					if (gpsSource.treeNode!=null)
					gpsSource.treeNode.NodeFont = new System.Drawing.Font(treeViewSources.Font, FontStyle.Regular);
					gpsSource.bTrack=false;
				}
				gpsSource = (GPSSource)m_gpsSourceList[iIndex];
				gpsSource.bTrack=true;
				if (gpsSource.treeNode!=null)
				gpsSource.treeNode.NodeFont = new System.Drawing.Font(treeViewSources.Font, FontStyle.Bold);
			}
			else
			{
				gpsSource = (GPSSource)m_gpsSourceList[iIndex];
				if (gpsSource.treeNode!=null)
				gpsSource.treeNode.NodeFont = new System.Drawing.Font(treeViewSources.Font, FontStyle.Regular);
				gpsSource.bTrack=false;

			}

			SaveSettings(null);
		}

		public void SetTrackHeading( bool bTrackHeading)
		{
			m_bTrackHeading=bTrackHeading;
			checkBoxTrackHeading.Checked=m_bTrackHeading;
			SaveSettings(null);
		}

        public void SetShowCircles(int iIndex, bool bShowCircles)
        {
            GPSSource gpsSource = (GPSSource)m_gpsSourceList[iIndex];
            gpsSource.bShowCircles = bShowCircles;
            SaveSettings(null);
        }

        public void SetShowGrid(int iIndex, bool bShowGrid)
        {
            GPSSource gpsSource = (GPSSource)m_gpsSourceList[iIndex];
            gpsSource.bShowGrid = bShowGrid;
            SaveSettings(null);
        }

		public void SetTrackLine(bool bTrackLine)
		{
			m_bTrackLine=bTrackLine;
			checkBoxTrackLine.Checked=m_bTrackLine;
			SaveSettings(null);
		}

		public int GetActiveTrack()
		{
			int i=-1;
			for (i=0; i<m_gpsSourceList.Count; i++)
			{
				GPSSource gpsSource=(GPSSource)m_gpsSourceList[i];
				if (gpsSource.bTrack==true)
					break;
			}
			return i;
		}

		//Show the GPS icon in he World View
		public bool ShowGPSIcon(char [] cNMEAMessage, int iMsgLength, bool fCheck, int iIndex, bool bRestartTrack, bool bParse, int iTotalLines, int iCurrentLine)
		{
			bool bRet=false;

			lock(LockShowIcon)
			{
				GPSSource gpsSource = (GPSSource)m_gpsSourceList[iIndex];

#if !DEBUG
			try
#endif
			{
				if (bParse)
					bRet=m_NMEA.ParseGPSMessage(cNMEAMessage,iMsgLength,fCheck,iIndex);
				else
					bRet=true;
			
				if (bRet==true && m_fInitialized==true)
				{
					if(m_swRecorder!=null)
					{
						String sMsg = new String(cNMEAMessage);
						char [] cAny = new char[3];	cAny[0]='\r'; cAny[1]='\n';	cAny[2]='\0';
						int iEnd=sMsg.IndexOfAny(cAny);
						if (iEnd>0)
							sMsg=sMsg.Substring(0,iEnd);
						sMsg = Convert.ToString(iIndex) + "," + sMsg;
						m_swRecorder.WriteLine(sMsg);
					}

                    string sNMEAMsg = "";
                    if (gpsSource.bNMEAExport)
                        sNMEAMsg = m_NMEA.ConvertToGPGGA(gpsSource.GpsPos);

                    //Export to a raw NMEA file is selected
                    if (gpsSource.bNMEAExport && sNMEAMsg != "")
                    {
                        if (gpsSource.swExport == null)
                        {
                            try
                            {
                                string sPath = textBoxNMEAExportPath.Text;
                                if (sPath == "")
                                    sPath = GpsTrackerPlugin.m_sPluginDirectory + "\\NMEAExport\\" + gpsSource.sDescription + ".txt";
                                else
                                {
                                    if (sPath.EndsWith("\\"))
                                        sPath = sPath + gpsSource.sDescription + ".txt";
                                    else
                                        sPath = sPath + "\\" + gpsSource.sDescription + ".txt";
                                }
                                gpsSource.swExport = File.CreateText(sPath);
                            }
                            catch (Exception)
                            {
                                gpsSource.swExport = null;
                            }
                        }

                        try
                        {
                            if (gpsSource.swExport != null)
                            {
                                gpsSource.swExport.WriteLine(sNMEAMsg);
                                gpsSource.swExport.Flush();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

					m_gpsTrackerPlugin.pluginLocked(true);
								
					//Show the user selected icon for the gps device on the WorldView
					string sPortInfo=gpsSource.sType;
					switch (sPortInfo)
					{
						case "UDP":
							sPortInfo += ": Port " + Convert.ToString(gpsSource.iUDPPort);
							break;
						case "COM":
							sPortInfo += ": Port " + Convert.ToString(gpsSource.iCOMPort);
							break;
						case "USB":
							sPortInfo += ": USB Device " + gpsSource.sUSBDevice;
							break;
						case "TCP":
							sPortInfo += ": IP:Port " + gpsSource.sTCPAddress + ":" + Convert.ToString(gpsSource.iTCPPort);
							break;
						case "File":
							//Do not display port info for an IXSEA file.
							string sFileName=gpsSource.sFileName;
							int iIndexName = sFileName.LastIndexOf('\\');
							if (iIndexName>=0 && iIndexName<sFileName.Length-1)
								sFileName=sFileName.Substring(iIndexName+1);

							sPortInfo += ": " + sFileName;
							break;

					}

					if (m_fInitialized==true)
					{
						GPSRenderInformation renderInfo = new GPSRenderInformation();
						renderInfo.fTrack=gpsSource.bTrack;
						renderInfo.bPOI=false;
						renderInfo.iIndex=iIndex;
                        if (gpsSource.GpsPos.m_sName != "")
                            renderInfo.sDescription = gpsSource.GpsPos.m_sName;
						else
							renderInfo.sDescription=gpsSource.sDescription;
                        renderInfo.sComment = gpsSource.GpsPos.m_sComment;
                        renderInfo.iAPRSIconTable = gpsSource.GpsPos.m_iAPRSIconTable;
                        renderInfo.iAPRSIconCode = gpsSource.GpsPos.m_iAPRSIconCode;
						renderInfo.fFix=true;
                        renderInfo.fLat = gpsSource.GpsPos.m_fLat;
                        renderInfo.fLon = gpsSource.GpsPos.m_fLon;
                        renderInfo.fAlt = gpsSource.GpsPos.m_fAlt;
                        renderInfo.fRoll = gpsSource.GpsPos.m_fRoll;
                        renderInfo.fPitch = gpsSource.GpsPos.m_fPitch;
                        renderInfo.fDepth = gpsSource.GpsPos.m_fDepth;
                        renderInfo.sAltUnit = gpsSource.GpsPos.m_sAltUnit;
                        renderInfo.sSpeedUnit = gpsSource.GpsPos.m_sSpeedUnit;
                        renderInfo.fSpeed = gpsSource.GpsPos.m_fSpeed;
                        renderInfo.fESpeed = gpsSource.GpsPos.m_fESpeed;
                        renderInfo.fNSpeed = gpsSource.GpsPos.m_fNSpeed;
                        renderInfo.fVSpeed = gpsSource.GpsPos.m_fVSpeed;
                        renderInfo.fHeading = gpsSource.GpsPos.m_fHeading;
                        renderInfo.fTrackOnTop = gpsSource.bTrackOnTop;

						renderInfo.sIcon=gpsSource.sIconPath;

						renderInfo.iStartAltitud=gpsSource.iStartAltitud;
						renderInfo.sPortInfo=sPortInfo;
                        renderInfo.iHour = gpsSource.GpsPos.m_iHour;
                        renderInfo.iMin = gpsSource.GpsPos.m_iMin;
                        renderInfo.fSec = gpsSource.GpsPos.m_iSec;
						renderInfo.bShowInfo=m_bInfoText;
						renderInfo.bTrackLine=m_bTrackLine;
                        renderInfo.gpsTrack = gpsSource.GpsPos.m_gpsTrack;
						renderInfo.bRestartTrack=bRestartTrack;
                        renderInfo.iDay = gpsSource.GpsPos.m_iDay;
                        renderInfo.iMonth = gpsSource.GpsPos.m_iMonth;
                        renderInfo.iYear = gpsSource.GpsPos.m_iYear;
						renderInfo.colorTrack=gpsSource.colorTrack;
                        renderInfo.bShowCircles = gpsSource.bShowCircles;
                        renderInfo.iCirclesCount = gpsSource.iCirclesCount;
                        renderInfo.dCirclesEvery = gpsSource.dCirclesEvery;
                        renderInfo.iLinesEvery = gpsSource.iLinesEvery;
                        renderInfo.colorCircle = gpsSource.colorCircle;
                        renderInfo.bKms = gpsSource.bKms;
                        renderInfo.bShowPosition = gpsSource.bShowPosition;
                        renderInfo.bShowName = gpsSource.bShowName;
                        renderInfo.bShowComment = gpsSource.bShowComment;
                        renderInfo.bShowSpeed = gpsSource.bShowSpeed;
                        renderInfo.bShowHeading = gpsSource.bShowHeading;
                        renderInfo.bShowTimeDate = gpsSource.bShowTimeDate;
                        renderInfo.bShowTrackDistance = gpsSource.bShowTrackDistance;
                        renderInfo.bShowReferenceAngles = gpsSource.bShowReferenceAngles;
                        renderInfo.bShowReferenceDistance = gpsSource.bShowReferenceDistance;
                        renderInfo.iInformationFontSize = gpsSource.iInformationFontSize;
                        renderInfo.colorInformation = gpsSource.colorInformation;
                        renderInfo.iDistanceUnit = gpsSource.iDistanceUnit;
                        renderInfo.iPositionUnit = gpsSource.iPositionUnit;
                        renderInfo.iSpeedUnit = gpsSource.GpsPos.m_iSpeedUnit;
                        renderInfo.iAltitudeUnit = gpsSource.GpsPos.m_iAltitudeUnit;
                        renderInfo.iCurrentLine = iCurrentLine;
                        renderInfo.iTotalLines = iTotalLines;

						m_gpsTrackerPlugin.pluginShowOverlay(renderInfo);

						if (gpsSource.iStartAltitud>0)
						{
							Thread.Sleep(3000);
							gpsSource.iStartAltitud=0;
						}
					}
					m_iLocked++;

				}

						//Display Message in Message Monitor
				try
				{
					if (m_MessageMonitor!=null && bRet)
							m_MessageMonitor.AddMessage(cNMEAMessage, iMsgLength);
				}
				catch (Exception)
				{
					m_MessageMonitor=null;
				}

						
					}
#if !DEBUG
			catch (Exception)
			{
			}
#endif

			}
			return bRet;
		}

		//
		//POI functions
		//

		public void AddPOI(string sName, double fLat, double fLon, bool bAdd, GPSSource gpsSourceWaypointsFile)
		{
			lock (LockPOI)
			{
				GPSSource gpsSource = new GPSSource();

				gpsSource.sType="POI";
                gpsSource.iPositionUnit = m_iPositionUnit;
                gpsSource.iDistanceUnit = m_iDistanceUnit;

				gpsSource.bSave=bAdd;
				gpsSource.bTrack=false;
				gpsSource.bSetup=true;
				gpsSource.bPOISet=false;

				TreeNode treeNode=null;
				gpsSource.iNameIndex=GetAvailableIndex();
                if (sName == "")
                    gpsSource.sDescription = gpsSource.sType + " #" + Convert.ToString(gpsSource.iNameIndex);
                else
                    gpsSource.sDescription = sName;

				gpsSource.treeNode=null;
				if (bAdd)
				{
					treeNode = new TreeNode(gpsSource.sDescription);
				    treeNode.ImageIndex=1;
				    treeNode.SelectedImageIndex=1;
				    treeNode.Tag=gpsSource;
				    gpsSource.treeNode=treeNode;
				}
				gpsSource.bNeedApply=false;
				gpsSource.fLat=fLat;
				gpsSource.fLon=fLon;

				m_gpsSourceList.Add(gpsSource);
				if (bAdd)
				{
				    ApplySettings(gpsSource,true,false,true);
				    m_treeNodePOI.Nodes.Add(treeNode);				
				    m_treeNodePOI.ExpandAll();
				}
				m_iSourceNameCount++;

				if (gpsSourceWaypointsFile!=null)
				{
					gpsSource.bTrack=gpsSourceWaypointsFile.bTrack;
					gpsSource.sIconPath = gpsSourceWaypointsFile.sIconPath;
					gpsSource.GpsPos.m_gpsTrack = gpsSourceWaypointsFile.GpsPos.m_gpsTrack;
					gpsSource.GpsPos.m_iDay=gpsSourceWaypointsFile.GpsPos.m_iDay;
					gpsSource.GpsPos.m_iMonth=gpsSourceWaypointsFile.GpsPos.m_iMonth;
					gpsSource.GpsPos.m_iYear=gpsSourceWaypointsFile.GpsPos.m_iYear;
					gpsSource.colorTrack=gpsSourceWaypointsFile.colorTrack;
					gpsSource.iStartAltitud=gpsSourceWaypointsFile.iStartAltitud;
				}

				if (m_hPOIThread==null)
				{
                    m_eventPOIThreadSync = new AutoResetEvent(false);
					m_hPOIThread = new Thread(new ThreadStart(this.threadPOI));
					m_hPOIThread.IsBackground = true;
					m_hPOIThread.Priority = System.Threading.ThreadPriority.Normal; 
					m_hPOIThread.Start();
				}
			}

		}
		#endregion

		#region User settings
		//
		// User Settings functions
		//

		//Save user selections 
		public void SaveSettings(StreamWriter sWriter)
		{
			int i;

			if (m_fPlayback==false)
			{
				StreamWriter sw;
				#if !DEBUG
				try
				#endif
				{
					if (sWriter!=null)
						sw=sWriter;
					else
                        sw = File.CreateText(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.cfg");



                    sw.WriteLine("GPSTracker Version " + GpsTrackerPlugin.m_sVersion + " settings...");

                    sw.WriteLine("comboBoxPositionUnits=" + comboBoxPositionUnits.SelectedIndex.ToString());
                    sw.WriteLine("comboBoxDistanceUnits=" + comboBoxDistanceUnits.SelectedIndex.ToString());
                    sw.WriteLine("textBoxSaveConfiguration=" + textBoxSaveConfiguration.Text.Trim());
                    sw.WriteLine("textBoxNMEAExportPath=" + textBoxNMEAExportPath.Text.Trim());
					sw.WriteLine("comboBoxTrackFileType=" + comboBoxTrackFileType.Text.Trim());
					sw.WriteLine("comboBoxWaypointsFileType=" + comboBoxWaypointsFileType.Text.Trim());
					sw.WriteLine("checkBoxSecureSocket=" + checkBoxSecureSocket.Checked.ToString());
					sw.WriteLine("comboBoxCOMPort=" + comboBoxCOMPort.Text);
					sw.WriteLine("comboBoxBaudRate=" + comboBoxBaudRate.Text);
					sw.WriteLine("comboBoxByteSize=" + comboBoxByteSize.Text);
					sw.WriteLine("comboParity=" + comboParity.SelectedIndex.ToString());
					sw.WriteLine("comboBoxStopBits=" + comboBoxStopBits.SelectedIndex.ToString());
					sw.WriteLine("numericUpDownUDPPort=" + numericUpDownUDPPort.Value.ToString());
					sw.WriteLine("numericUpDownTCPPort=" + numericUpDownTCPPort.Value.ToString());
					sw.WriteLine("numericUpDownReload=" + numericUpDownReload.Value.ToString());
					sw.WriteLine("comboBoxTcpIP=" + comboBoxTcpIP.Text.Trim());
					sw.WriteLine("comboBoxFile=" + comboBoxFile.Text.Trim());
					sw.WriteLine("comboBoxWaypointsFile=" + comboBoxWaypointsFile.Text.Trim());
					sw.WriteLine("trackBarFileSpeed=" + trackBarFileSpeed.Value.ToString());
					sw.WriteLine("m_bTrackHeading=" + m_bTrackHeading.ToString());
					sw.WriteLine("m_bTrackLine=" + m_bTrackLine.ToString());
					sw.WriteLine("m_bRecordSession=" + m_bRecordSession.ToString());
					sw.WriteLine("m_bInfoText=" + m_bInfoText.ToString());
					sw.WriteLine("checkBoxNoDelay=" + checkBoxNoDelay.Checked.ToString());
					sw.WriteLine("comboBoxFlowControl=" + comboBoxFlowControl.SelectedIndex.ToString());
					sw.WriteLine("buttonTrackColorCOM=" + Convert.ToString(buttonTrackColorCOM.BackColor.ToArgb()));
					sw.WriteLine("buttonTrackColorTCP=" + Convert.ToString(buttonTrackColorTCP.BackColor.ToArgb()));
					sw.WriteLine("buttonTrackColorUDP=" + Convert.ToString(buttonTrackColorUDP.BackColor.ToArgb()));
					sw.WriteLine("buttonTrackColor=" + Convert.ToString(buttonTrackColor.BackColor.ToArgb()));
					sw.WriteLine("textBoxLongitud=" + textBoxLongitud.Text);
					sw.WriteLine("textBoxLatitud=" + textBoxLatitud.Text);
					sw.WriteLine("numericUpDownAltitud=" + numericUpDownAltitud.Value.ToString());
					sw.WriteLine("checkBoxSetAltitud=" + checkBoxSetAltitud.Checked.ToString());

					sw.WriteLine("FILE COMBOBOX COUNT=" + Convert.ToString(comboBoxFile.Items.Count));
					for (i=0; i<comboBoxFile.Items.Count; i++)
						sw.WriteLine((string)comboBoxFile.Items[i]);

					sw.WriteLine("WAYPOINTSFILE COMBOBOX COUNT=" + Convert.ToString(comboBoxWaypointsFile.Items.Count));
					for (i=0; i<comboBoxWaypointsFile.Items.Count; i++)
						sw.WriteLine((string)comboBoxWaypointsFile.Items[i]);

					sw.WriteLine("TCPIP COMBOBOX COUNT=" + Convert.ToString(comboBoxTcpIP.Items.Count));
					for (i=0; i<comboBoxTcpIP.Items.Count; i++)
						sw.WriteLine((string)comboBoxTcpIP.Items[i]);

                    sw.WriteLine("LOADCONFIGURATION COMBOBOX COUNT=" + Convert.ToString(comboBoxConfigurationFile.Items.Count));
                    for (i = 0; i < comboBoxConfigurationFile.Items.Count; i++)
                        sw.WriteLine((string)comboBoxConfigurationFile.Items[i]);
                    sw.WriteLine("comboBoxConfigurationFile=" + comboBoxConfigurationFile.Text.Trim());

					sw.WriteLine("END UI CONTROLS");



					sw.WriteLine("SOURCE COUNT=" + Convert.ToString(m_gpsSourceList.Count));
					for (i=0; i<m_gpsSourceList.Count; i++)
					{
						GPSSource gpsSource = (GPSSource)m_gpsSourceList[i];

						if (gpsSource.bSave)
						{
						    sw.WriteLine("iNameIndex=" + Convert.ToString(gpsSource.iNameIndex));
						    sw.WriteLine("bNeedApply=" + Convert.ToString(gpsSource.bNeedApply));
						    sw.WriteLine("bSetup=" + Convert.ToString(gpsSource.bSetup));
						    sw.WriteLine("sType=" + gpsSource.sType);
						    sw.WriteLine("sDescription=" + gpsSource.sDescription);
						    sw.WriteLine("sComment=" + gpsSource.sComment);
						    sw.WriteLine("sIconPath=" + gpsSource.sIconPath);
						    sw.WriteLine("colorTrack=" + Convert.ToString(gpsSource.colorTrack.ToArgb()));
                            sw.WriteLine("colorCircle=" + Convert.ToString(gpsSource.colorCircle.ToArgb()));
						    //sw.WriteLine("fLat=" + Convert.ToString(gpsSource.fLat));
						    //sw.WriteLine("fLon=" + Convert.ToString(gpsSource.fLon));
						    sw.WriteLine("bTrack=" + Convert.ToString(gpsSource.bTrack));
						    sw.WriteLine("bSave=" + Convert.ToString(gpsSource.bSave));
                            sw.WriteLine("bNMEAExport=" + Convert.ToString(gpsSource.bNMEAExport));
                            sw.WriteLine("bNMEAExportVCOM=" + Convert.ToString(gpsSource.bNMEAExportVCOM));
                            sw.WriteLine("bShowCircles=" + Convert.ToString(gpsSource.bShowCircles));
                            sw.WriteLine("iCirclesCount=" + Convert.ToString(gpsSource.iCirclesCount));
                            sw.WriteLine("iCirclesEvery=" + Convert.ToString(gpsSource.dCirclesEvery));
                            sw.WriteLine("iLinesEvery=" + Convert.ToString(gpsSource.iLinesEvery));
                            sw.WriteLine("bKms=" + Convert.ToString(gpsSource.bKms));
                            sw.WriteLine("bShowGrid=" + Convert.ToString(gpsSource.bShowGrid));
                            sw.WriteLine("iSquareCount=" + Convert.ToString(gpsSource.iSquareCount));
                            sw.WriteLine("iSquareSize=" + Convert.ToString(gpsSource.dSquareSize));
                            sw.WriteLine("bGridKms=" + Convert.ToString(gpsSource.bGridKms));
                            sw.WriteLine("colorGrid=" + Convert.ToString(gpsSource.colorGrid.ToArgb()));

                            sw.WriteLine("bShowComment=" + Convert.ToString(gpsSource.bShowComment));
                            sw.WriteLine("bShowName=" + Convert.ToString(gpsSource.bShowName));
                            sw.WriteLine("bShowPosition=" + Convert.ToString(gpsSource.bShowPosition));
                            sw.WriteLine("bShowSpeed=" + Convert.ToString(gpsSource.bShowSpeed));
                            sw.WriteLine("bShowHeading=" + Convert.ToString(gpsSource.bShowHeading));
                            sw.WriteLine("bShowTimeDate=" + Convert.ToString(gpsSource.bShowTimeDate));
                            sw.WriteLine("bShowTrackDistance=" + Convert.ToString(gpsSource.bShowTrackDistance));
                            sw.WriteLine("bShowReferenceDistance=" + Convert.ToString(gpsSource.bShowReferenceDistance));
                            sw.WriteLine("bShowReferenceAngles=" + Convert.ToString(gpsSource.bShowReferenceAngles));
                            sw.WriteLine("iInformationFontSize=" + Convert.ToString(gpsSource.iInformationFontSize));
                            sw.WriteLine("colorInformation=" + Convert.ToString(gpsSource.colorInformation.ToArgb()));

						    switch(gpsSource.sType)
						    {
							    case "USB":
								    sw.WriteLine("sUSBDevice=" + gpsSource.sUSBDevice);
								    break;
							    case "COM":
								    sw.WriteLine("iCOMPort=" + Convert.ToString(gpsSource.iCOMPort));
								    sw.WriteLine("iBaudRate=" + Convert.ToString(gpsSource.iBaudRate));
								    sw.WriteLine("iByteSize=" + Convert.ToString(gpsSource.iByteSize));
								    sw.WriteLine("iSelectedItem=" + Convert.ToString(gpsSource.iSelectedItem));
								    sw.WriteLine("iParity=" + Convert.ToString(gpsSource.iParity));
								    sw.WriteLine("iStopBits=" + Convert.ToString(gpsSource.iStopBits));
								    sw.WriteLine("iFlowControl=" + Convert.ToString(gpsSource.iFlowControl));
								    break;
							    case "UDP":
								    sw.WriteLine("iUDPPort=" + Convert.ToString(gpsSource.iUDPPort));
								    break;
							    case "TCP":
								    sw.WriteLine("sTCPAddress=" + gpsSource.sTCPAddress);
								    sw.WriteLine("iTCPPort=" + Convert.ToString(gpsSource.iTCPPort));
								    sw.WriteLine("bSecureSocket=" + Convert.ToString(gpsSource.bSecureSocket));
								    break;
							    case "File":
								    sw.WriteLine("sFileName=" + gpsSource.sFileName);
									    sw.WriteLine("bWaypoints=" + gpsSource.bWaypoints);

									    sw.WriteLine("babelType=" + gpsSource.saGpsBabelFormat[0]);
									    sw.WriteLine("babelExtension=" + gpsSource.saGpsBabelFormat[1]);
									    sw.WriteLine("babelDescription=" + gpsSource.saGpsBabelFormat[2]);

								    sw.WriteLine("bNoDelay=" + Convert.ToString(gpsSource.bNoDelay));
								    sw.WriteLine("bTrackAtOnce=" + Convert.ToString(gpsSource.bTrackAtOnce));
								    sw.WriteLine("iPlaySpeed=" + Convert.ToString(gpsSource.iPlaySpeed));
								    sw.WriteLine("iFilePlaySpeed=" + Convert.ToString(gpsSource.iFilePlaySpeed));
								    sw.WriteLine("sFileNameSession=" + gpsSource.sFileNameSession);
								    sw.WriteLine("iReload=" + Convert.ToString(gpsSource.iReload));
								    sw.WriteLine("bForcePreprocessing=" + Convert.ToString(gpsSource.bForcePreprocessing));
								    sw.WriteLine("bSession=" + Convert.ToString(gpsSource.bSession));
								    break;
							    case "POI":
								    sw.WriteLine("fLat=" + Convert.ToString(gpsSource.fLat));
								    sw.WriteLine("fLon=" + Convert.ToString(gpsSource.fLon));
								    break;
                                case "Track Line":
                                    sw.WriteLine("GpsTrackLine.colorLine=" + Convert.ToString(gpsSource.GeoFence.colorLine.ToArgb()));
                                    sw.WriteLine("GpsTrackLine.bGeoFence=" + gpsSource.GeoFence.bGeoFence);
                                    sw.WriteLine("GpsTrackLine.sName=" + gpsSource.GeoFence.sName);
                                    for (int iGeoFence = 0; iGeoFence < gpsSource.GeoFence.arrayLat.Count; iGeoFence++)
                                    {
                                        sw.WriteLine("GpsTrackLine.arrayLat=" + Convert.ToString((float)gpsSource.GeoFence.arrayLat[iGeoFence]));
                                        sw.WriteLine("GpsTrackLine.arrayLon=" + Convert.ToString((float)gpsSource.GeoFence.arrayLon[iGeoFence]));
                                    }
                                    sw.WriteLine("GpsTrackLine.Array DONE");
                                    break;
						    }
    						
						    sw.WriteLine("END SOURCE INDEX=" + Convert.ToString(i));
					    }
					}
					if (sWriter==null)
						sw.Close();
					else
						sw.WriteLine("--------------------------------------------------");
				} 
				#if !DEBUG
				catch(Exception) 
				{
				}
				#endif
			}
		}

		//Load user selected settings
		public bool LoadSettings(StreamReader srReader, bool bSet)
		{
			bool bRet=false;
			Bitmap image;
			StreamReader sr;

			m_bHandleControlValueChangeEvent=false;

			if (bSet)
			{
				SetupTree();
				m_bHandleControlValueChangeEvent=false;
				m_gpsSourceList.Clear();
				progressBarPreprocessing.Value=0;
				progressBarSetup.Value=0;
				comboBoxFile.Items.Clear();
				comboBoxWaypointsFile.Items.Clear();
				comboBoxTcpIP.Items.Clear();
				checkBoxNoDelay.Checked=false;
				checkBoxSecureSocket.Checked=false;
				progressBarSetup.Visible=false;
				labelSettingup.Visible=false;
				labelPreprocessing.Visible=false;
				progressBarPreprocessing.Visible=false;

                image = new Bitmap(GpsTrackerPlugin.m_sPluginDirectory + "\\satellite.png");
				imageListGpsIcons.Images.Add(image);
                image = new Bitmap(GpsTrackerPlugin.m_sPluginDirectory + "\\gpsnotset.png");
				imageListGpsIcons.Images.Add(image);
			}

			#if !DEBUG
			try
			#endif
			{ 
				if (srReader==null)
				{
                    if (!File.Exists(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.cfg"))
					{
						SetDefaultSettings(bSet);
						return false;
					}
                    sr = File.OpenText(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.cfg");
				}
				else
					sr=srReader;
				
			{
				string line=sr.ReadLine();
				if (line!=null && line.StartsWith("GPSTracker Version") )
				{
					if (bSet)
					{
						while(true)
						{
							line = sr.ReadLine();
							if (line==null || line.StartsWith("END UI CONTROLS"))
								break;

                            if (line.StartsWith("comboBoxPositionUnits="))
                                comboBoxPositionUnits.SelectedIndex = Convert.ToInt32(line.Remove(0, "comboBoxPositionUnits=".Length));
                            else
                            if (line.StartsWith("comboBoxDistanceUnits="))
                                comboBoxDistanceUnits.SelectedIndex = Convert.ToInt32(line.Remove(0, "comboBoxDistanceUnits=".Length));
                            else
                            if (line.StartsWith("comboBoxConfigurationFile="))
                                comboBoxConfigurationFile.Text = line.Remove(0, "comboBoxConfigurationFile=".Length);
                            else
                            if (line.StartsWith("textBoxSaveConfiguration="))
                                textBoxSaveConfiguration.Text = line.Remove(0, "textBoxSaveConfiguration=".Length);
                            else
                            if (line.StartsWith("checkBoxTrackOnTop="))
                                checkBoxTrackOnTop.Checked = Convert.ToBoolean(line.Remove(0, "checkBoxTrackOnTop=".Length));
                            else
                            if (line.StartsWith("checkBoxVExaggeration="))
                                checkBoxVExaggeration.Checked = Convert.ToBoolean(line.Remove(0, "checkBoxVExaggeration=".Length));
                            else
                            if (line.StartsWith("textBoxNMEAExportPath="))
                                textBoxNMEAExportPath.Text = line.Remove(0, "textBoxNMEAExportPath=".Length);
                            else
							if (line.StartsWith("comboBoxTrackFileType="))
								comboBoxTrackFileType.Text=line.Remove(0,"comboBoxTrackFileType=".Length);
							else
								if (line.StartsWith("comboBoxWaypointsFileType="))
								comboBoxWaypointsFileType.Text=line.Remove(0,"comboBoxWaypointsFileType=".Length);
							else
							if (line.StartsWith("checkBoxSecureSocket="))
								checkBoxSecureSocket.Checked=Convert.ToBoolean(line.Remove(0,"checkBoxSecureSocket=".Length));
							else
							if (line.StartsWith("comboBoxCOMPort="))
								comboBoxCOMPort.Text=line.Remove(0,"comboBoxCOMPort=".Length);
							else
								if (line.StartsWith("comboBoxBaudRate="))
								comboBoxBaudRate.Text=line.Remove(0,"comboBoxBaudRate=".Length);
							else
								if (line.StartsWith("comboBoxByteSize="))
								comboBoxByteSize.Text=line.Remove(0,"comboBoxByteSize=".Length);
							else
								if (line.StartsWith("comboParity="))
								comboParity.SelectedIndex=Convert.ToInt32(line.Remove(0,"comboParity=".Length));
							else
								if (line.StartsWith("comboBoxStopBits="))
								comboBoxStopBits.SelectedIndex=Convert.ToInt32(line.Remove(0,"comboBoxStopBits=".Length));
							else
								if (line.StartsWith("numericUpDownUDPPort="))
								numericUpDownUDPPort.Value=Convert.ToDecimal(line.Remove(0,"numericUpDownUDPPort=".Length));
							else
								if (line.StartsWith("numericUpDownTCPPort="))
								numericUpDownTCPPort.Value=Convert.ToDecimal(line.Remove(0,"numericUpDownTCPPort=".Length));
							else
								if (line.StartsWith("numericUpDownReload="))
								numericUpDownReload.Value=Convert.ToDecimal(line.Remove(0,"numericUpDownReload=".Length));
							else
								if (line.StartsWith("comboBoxTcpIP="))
								comboBoxTcpIP.Text=line.Remove(0,"comboBoxTcpIP=".Length);
							else
								if (line.StartsWith("comboBoxFile="))
								comboBoxFile.Text=line.Remove(0,"comboBoxFile=".Length);
							else
								if (line.StartsWith("comboBoxWaypointsFile="))
								comboBoxWaypointsFile.Text=line.Remove(0,"comboBoxWaypointsFile=".Length);
							else
								if (line.StartsWith("trackBarFileSpeed="))
								trackBarFileSpeed.Value=Convert.ToInt32(line.Remove(0,"trackBarFileSpeed=".Length));
							else
								if (line.StartsWith("m_bTrackHeading="))
								m_bTrackHeading=Convert.ToBoolean(line.Remove(0,"m_bTrackHeading=".Length));
							else
								if (line.StartsWith("m_bTrackLine="))
								m_bTrackLine=Convert.ToBoolean(line.Remove(0,"m_bTrackLine=".Length));
								//							else
								//								if (line.StartsWith("m_bRecordSession="))
								//								m_bRecordSession=Convert.ToBoolean(line.Remove(0,"m_bRecordSession=".Length));
							else
								if (line.StartsWith("m_bInfoText="))
								m_bInfoText=Convert.ToBoolean(line.Remove(0,"m_bInfoText=".Length));
							else
								if (line.StartsWith("checkBoxNoDelay="))
								checkBoxNoDelay.Checked=Convert.ToBoolean(line.Remove(0,"checkBoxNoDelay=".Length));
							else
								if (line.StartsWith("checkBoxNoDelay="))
								checkBoxNoDelay.Checked=Convert.ToBoolean(line.Remove(0,"checkBoxNoDelay=".Length));
							else
								if (line.StartsWith("comboBoxFlowControl="))
								comboBoxFlowControl.SelectedIndex=Convert.ToInt32(line.Remove(0,"comboBoxFlowControl=".Length));
							else
								if (line.StartsWith("buttonTrackColorCOM="))
								buttonTrackColorCOM.BackColor=Color.FromArgb(Convert.ToInt32(line.Remove(0,"buttonTrackColorCOM=".Length)));
							else
								if (line.StartsWith("buttonTrackColorTCP="))
								buttonTrackColorTCP.BackColor=Color.FromArgb(Convert.ToInt32(line.Remove(0,"buttonTrackColorTCP=".Length)));
							else
								if (line.StartsWith("buttonTrackColorUDP="))
								buttonTrackColorUDP.BackColor=Color.FromArgb(Convert.ToInt32(line.Remove(0,"buttonTrackColorUDP=".Length)));
							else
								if (line.StartsWith("buttonTrackColor="))
								buttonTrackColor.BackColor=Color.FromArgb(Convert.ToInt32(line.Remove(0,"buttonTrackColor=".Length)));
							else
								if (line.StartsWith("FILE COMBOBOX COUNT"))
							{
								int iCount=Convert.ToInt32(line.Remove(0,"FILE COMBOBOX COUNT=".Length));
								for (int i=0; i<iCount; i++)
									comboBoxFile.Items.Add(sr.ReadLine());
							}
							else
								if (line.StartsWith("WAYPOINTSFILE COMBOBOX COUNT"))
							{
								int iCount=Convert.ToInt32(line.Remove(0,"WAYPOINTSFILE COMBOBOX COUNT=".Length));
								for (int i=0; i<iCount; i++)
									comboBoxWaypointsFile.Items.Add(sr.ReadLine());
							}
							else
							if (line.StartsWith("TCPIP COMBOBOX COUNT"))
							{
								int iCount=Convert.ToInt32(line.Remove(0,"TCPIP COMBOBOX COUNT=".Length));
								for (int i=0; i<iCount; i++)
									comboBoxTcpIP.Items.Add(sr.ReadLine());
							}
                            else
                                if (line.StartsWith("LOADCONFIGURATION COMBOBOX COUNT"))
                                {
                                    int iCount = Convert.ToInt32(line.Remove(0, "LOADCONFIGURATION COMBOBOX COUNT=".Length));
                                    for (int i = 0; i < iCount; i++)
                                    {
                                        string sFileName = sr.ReadLine();

                                        int iFile;
                                        for (iFile = 0; iFile < comboBoxConfigurationFile.Items.Count; iFile++)
                                        {
                                            if ((string)comboBoxConfigurationFile.Items[iFile] == sFileName)
                                                break;
                                        }
                                        if (i == comboBoxConfigurationFile.Items.Count)
                                            comboBoxConfigurationFile.Items.Add(sFileName);
                                    }
                                    if (comboBoxConfigurationFile.Items.Count > 0)
                                        comboBoxConfigurationFile.SelectedIndex = 0;
                                }
							else
								if (line.StartsWith("textBoxLongitud="))
								textBoxLongitud.Text=line.Remove(0,"textBoxLongitud=".Length);
							else
								if (line.StartsWith("textBoxLatitud="))
								textBoxLatitud.Text=line.Remove(0,"textBoxLatitud=".Length);
							else
								if (line.StartsWith("numericUpDownAltitud="))
								numericUpDownAltitud.Value=Convert.ToDecimal(line.Remove(0,"numericUpDownAltitud=".Length));
							else
								if (line.StartsWith("checkBoxSetAltitud="))
								checkBoxSetAltitud.Checked=Convert.ToBoolean(line.Remove(0,"checkBoxSetAltitud=".Length));
						}
					}
					int iSourceCount=0;
					line = sr.ReadLine();
					if (line!=null && line.StartsWith("SOURCE COUNT"))
						iSourceCount=Convert.ToInt32(line.Remove(0,"SOURCE COUNT=".Length));
					for (int i=0; i<iSourceCount; i++)
					{
						GPSSource gpsS=new GPSSource();
						gpsS.bNeedApply=true;
						gpsS.saGpsBabelFormat = new string[3];
						m_gpsSourceList.Add(gpsS);
					}

					int iItem=0;
					while(true)
					{
						line = sr.ReadLine();
						if (line!=null && line.StartsWith("END SOURCE INDEX"))
						{
							iItem++;
                            if (iItem == iSourceCount)
                                break;
							m_iSourceNameCount++;
							line = sr.ReadLine();
						}

						if (line==null || (srReader!=null && line=="--------------------------------------------------"))
						{
							bRet=true;
							break;
						}

						if (bSet)
						{
							GPSSource gpsSource = (GPSSource)m_gpsSourceList[iItem];

                            if (line.StartsWith("bShowGrid="))
                                gpsSource.bShowGrid = Convert.ToBoolean(line.Remove(0, "bShowGrid=".Length));
                            else
                            if (line.StartsWith("iSquareCount="))
                                gpsSource.iSquareCount = Convert.ToInt32(line.Remove(0, "iSquareCount=".Length));
                            else
                            if (line.StartsWith("iSquareSize="))
                                gpsSource.dSquareSize = Convert.ToDouble(line.Remove(0, "iSquareSize=".Length));
                            else
                            if (line.StartsWith("bGridKms="))
                                gpsSource.bGridKms = Convert.ToBoolean(line.Remove(0, "bGridKms=".Length));
                            else
                            if (line.StartsWith("colorGrid="))
                                gpsSource.colorGrid = Color.FromArgb(Convert.ToInt32(line.Remove(0, "colorGrid=".Length)));
                            else
                            if (line.StartsWith("bShowComment="))
                                gpsSource.bShowComment = Convert.ToBoolean(line.Remove(0, "bShowComment=".Length));
                            else
                            if (line.StartsWith("bShowName="))
                                gpsSource.bShowName = Convert.ToBoolean(line.Remove(0, "bShowName=".Length));
                            else
                            if (line.StartsWith("bShowPosition="))
                                gpsSource.bShowPosition = Convert.ToBoolean(line.Remove(0, "bShowPosition=".Length));
                            else
                            if (line.StartsWith("bShowSpeed="))
                                gpsSource.bShowSpeed = Convert.ToBoolean(line.Remove(0, "bShowSpeed=".Length));
                            else
                            if (line.StartsWith("bShowHeading="))
                                gpsSource.bShowHeading = Convert.ToBoolean(line.Remove(0, "bShowHeading=".Length));
                            else
                            if (line.StartsWith("bShowTimeDate="))
                                gpsSource.bShowTimeDate = Convert.ToBoolean(line.Remove(0, "bShowTimeDate=".Length));
                            else
                            if (line.StartsWith("bShowTrackDistance="))
                                gpsSource.bShowTrackDistance = Convert.ToBoolean(line.Remove(0, "bShowTrackDistance=".Length));
                            else
                            if (line.StartsWith("bShowReferenceDistance="))
                                gpsSource.bShowReferenceDistance = Convert.ToBoolean(line.Remove(0, "bShowReferenceDistance=".Length));
                            else
                            if (line.StartsWith("bShowReferenceAngles="))
                                gpsSource.bShowReferenceAngles = Convert.ToBoolean(line.Remove(0, "bShowReferenceAngles=".Length));
                            else
                            if (line.StartsWith("iInformationFontSize="))
                                gpsSource.iInformationFontSize = Convert.ToInt32(line.Remove(0, "iInformationFontSize=".Length));
                            else
                            if (line.StartsWith("colorInformation="))
                                gpsSource.colorInformation = Color.FromArgb(Convert.ToInt32(line.Remove(0, "colorInformation=".Length)));
                            else
                            if (line.StartsWith("bKms="))
                                gpsSource.bKms = Convert.ToBoolean(line.Remove(0, "bKms=".Length));
                            else
                            if (line.StartsWith("bShowCircles="))
                                gpsSource.bShowCircles = Convert.ToBoolean(line.Remove(0, "bShowCircles=".Length));
                            else
                            if (line.StartsWith("iCirclesCount="))
                                gpsSource.iCirclesCount = Convert.ToInt32(line.Remove(0, "iCirclesCount=".Length));
                            else
                            if (line.StartsWith("iCirclesEvery="))
                                gpsSource.dCirclesEvery = Convert.ToDouble(line.Remove(0, "iCirclesEvery=".Length));
                            else
                            if (line.StartsWith("iLinesEvery="))
                                gpsSource.iLinesEvery = Convert.ToInt32(line.Remove(0, "iLinesEvery=".Length));
                            else
							if (line.StartsWith("iNameIndex="))
								gpsSource.iNameIndex=Convert.ToInt32(line.Remove(0,"iNameIndex=".Length));
							else
                            if (line.StartsWith("bNMEAExport="))
                                gpsSource.bNMEAExport = Convert.ToBoolean(line.Remove(0, "bNMEAExport=".Length));
                            else
                                if (line.StartsWith("bNMEAExportVCOM="))
                                    gpsSource.bNMEAExportVCOM = Convert.ToBoolean(line.Remove(0, "bNMEAExportVCOM=".Length));
                            else
								if (line.StartsWith("bSecureSocket="))
								gpsSource.bSecureSocket=Convert.ToBoolean(line.Remove(0,"bSecureSocket=".Length));
							else
								if (line.StartsWith("bNeedApply="))
								gpsSource.bNeedApply=Convert.ToBoolean(line.Remove(0,"bNeedApply=".Length));
							else
								if (line.StartsWith("bSetup="))
								gpsSource.bSetup=Convert.ToBoolean(line.Remove(0,"bSetup=".Length));
							else
								if (line.StartsWith("sType="))
								gpsSource.sType=line.Remove(0,"sType=".Length);
							else
								if (line.StartsWith("sDescription="))
								gpsSource.sDescription=line.Remove(0,"sDescription=".Length);
							else
								if (line.StartsWith("sComment="))
								gpsSource.sComment=line.Remove(0,"sComment=".Length);
							else
								if (line.StartsWith("sIconPath="))
								gpsSource.sIconPath=line.Remove(0,"sIconPath=".Length);
							else
								if (line.StartsWith("colorTrack="))
								gpsSource.colorTrack=Color.FromArgb(Convert.ToInt32(line.Remove(0,"colorTrack=".Length)));
							else
                                if (line.StartsWith("colorCircle="))
                                    gpsSource.colorCircle = Color.FromArgb(Convert.ToInt32(line.Remove(0, "colorCircle=".Length)));
                                else
								if (line.StartsWith("fLat="))
								gpsSource.fLat=Convert.ToDouble(line.Remove(0,"fLat=".Length));
							else
								if (line.StartsWith("fLon="))
								gpsSource.fLon=Convert.ToDouble(line.Remove(0,"fLon=".Length));
							else
								if (line.StartsWith("bTrack="))
								gpsSource.bTrack=Convert.ToBoolean(line.Remove(0,"bTrack=".Length));
							else
								if (line.StartsWith("sUSBDevice="))
								gpsSource.sUSBDevice=line.Remove(0,"sUSBDevice=".Length);
							else
								if (line.StartsWith("iCOMPort="))
								gpsSource.iCOMPort=Convert.ToInt32(line.Remove(0,"iCOMPort=".Length));
							else
								if (line.StartsWith("iBaudRate="))
								gpsSource.iBaudRate=Convert.ToInt32(line.Remove(0,"iBaudRate=".Length));
							else
								if (line.StartsWith("iByteSize="))
								gpsSource.iByteSize=Convert.ToInt32(line.Remove(0,"iByteSize=".Length));
							else
								if (line.StartsWith("iSelectedItem="))
								gpsSource.iSelectedItem=Convert.ToInt32(line.Remove(0,"iSelectedItem=".Length));
							else
								if (line.StartsWith("iParity="))
								gpsSource.iParity=Convert.ToInt32(line.Remove(0,"iParity=".Length));
							else
								if (line.StartsWith("iStopBits="))
								gpsSource.iStopBits=Convert.ToInt32(line.Remove(0,"iStopBits=".Length));
							else
								if (line.StartsWith("iFlowControl="))
								gpsSource.iFlowControl=Convert.ToInt32(line.Remove(0,"iFlowControl=".Length));
							else
								if (line.StartsWith("iUDPPort="))
								gpsSource.iUDPPort=Convert.ToInt32(line.Remove(0,"iUDPPort=".Length));
							else
								if (line.StartsWith("iTCPPort="))
								gpsSource.iTCPPort=Convert.ToInt32(line.Remove(0,"iTCPPort=".Length));
							else
								if (line.StartsWith("sTCPAddress="))
								gpsSource.sTCPAddress=line.Remove(0,"sTCPAddress=".Length);
							else
								if (line.StartsWith("sFileName="))
								gpsSource.sFileName=line.Remove(0,"sFileName=".Length);
							else
								if (line.StartsWith("sFileNameSession="))
								gpsSource.sFileNameSession=line.Remove(0,"sFileNameSession=".Length);
							else
								if (line.StartsWith("bNoDelay="))
								gpsSource.bNoDelay=Convert.ToBoolean(line.Remove(0,"bNoDelay=".Length));
							else
								if (line.StartsWith("bTrackAtOnce="))
								gpsSource.bTrackAtOnce=Convert.ToBoolean(line.Remove(0,"bTrackAtOnce=".Length));
							else
								if (line.StartsWith("iPlaySpeed="))
								gpsSource.iPlaySpeed=Convert.ToInt32(line.Remove(0,"iPlaySpeed=".Length));
							else
								if (line.StartsWith("iFilePlaySpeed="))
								gpsSource.iFilePlaySpeed=Convert.ToInt32(line.Remove(0,"iFilePlaySpeed=".Length));
							else
								if (line.StartsWith("sCallSign="))
								gpsSource.sCallSign=line.Remove(0,"sCallSign=".Length);
							else
								if (line.StartsWith("iRefreshRate="))
								gpsSource.iRefreshRate=Convert.ToInt32(line.Remove(0,"iRefreshRate=".Length));
							else
								if (line.StartsWith("iReload="))
								gpsSource.iReload=Convert.ToInt32(line.Remove(0,"iReload=".Length));
							else
								if (line.StartsWith("bForcePreprocessing="))
								gpsSource.bForcePreprocessing=Convert.ToBoolean(line.Remove(0,"bForcePreprocessing=".Length));
							else
								if (line.StartsWith("bWaypoints="))
								gpsSource.bWaypoints=Convert.ToBoolean(line.Remove(0,"bWaypoints=".Length));
							else
								if (line.StartsWith("bSave="))
								gpsSource.bSave=Convert.ToBoolean(line.Remove(0,"bSave=".Length));
							else
								if (line.StartsWith("bSession="))
								gpsSource.bSession=Convert.ToBoolean(line.Remove(0,"bSession=".Length));
							else
								if (line.StartsWith("babelType="))
								gpsSource.saGpsBabelFormat[0]=line.Remove(0,"babelType=".Length);
							else
								if (line.StartsWith("babelExtension="))
								gpsSource.saGpsBabelFormat[1]=line.Remove(0,"babelExtension=".Length);
							else
								if (line.StartsWith("babelDescription="))
								gpsSource.saGpsBabelFormat[2]=line.Remove(0,"babelDescription=".Length);
							else
								if (line.StartsWith("sCallSignFilter="))
							{
								CSVReader csvReader=new CSVReader();
								string [] sLines=csvReader.GetCSVLine(line.Remove(0,"sCallSignFilter=".Length));
								if (sLines!=null && sLines.Length>0)
									gpsSource.sCallSignFilterLines=(string[])sLines.Clone();
							}
							else
								if (line.StartsWith("sAPRSServerURL="))
								gpsSource.sAPRSServerURL=line.Remove(0,"sAPRSServerURL=".Length);
                            else
								if (line.StartsWith("bNMEAExport="))
    							gpsSource.bNMEAExport=Convert.ToBoolean(line.Remove(0,"bNMEAExport=".Length));
                            else
                                if (line.StartsWith("GeoFence.sSource="))
                                    gpsSource.GeoFence.sSource = line.Remove(0, "GeoFence.sSource=".Length);
						    else
                                    if (line.StartsWith("GeoFence.bGeoFence="))
                                        gpsSource.GeoFence.bGeoFence = Convert.ToBoolean(line.Remove(0, "GeoFence.bGeoFence=".Length));
                                    else
								if (line.StartsWith("GeoFence.sName="))
								gpsSource.GeoFence.sName=line.Remove(0,"GeoFence.sName=".Length);
						    else
                                if (line.StartsWith("GpsTrackLine.sName="))
                                    gpsSource.GeoFence.sName = line.Remove(0, "GpsTrackLine.sName=".Length);
                                else
                                    if (line.StartsWith("GpsTrackLine.bGeoFence="))
                                        gpsSource.GeoFence.bGeoFence = Convert.ToBoolean(line.Remove(0, "GpsTrackLine.bGeoFence=".Length));
                                    else
								if (line.StartsWith("GeoFence.sEmail="))
								gpsSource.GeoFence.sEmail=line.Remove(0,"GeoFence.sEmail=".Length);
						    else
								if (line.StartsWith("GeoFence.sSound="))
								gpsSource.GeoFence.sSound=line.Remove(0,"GeoFence.sSound=".Length);
                            else
                                if (line.StartsWith("GeoFence.sSoundOut="))
                                    gpsSource.GeoFence.sSoundOut = line.Remove(0, "GeoFence.sSoundOut=".Length);
						    else
								if (line.StartsWith("GeoFence.bEmailIn="))
								gpsSource.GeoFence.bEmailIn=Convert.ToBoolean(line.Remove(0,"GeoFence.bEmailIn=".Length));
                            else
                                if (line.StartsWith("GeoFence.bEmailOut="))
                                    gpsSource.GeoFence.bEmailOut = Convert.ToBoolean(line.Remove(0, "GeoFence.bEmailOut=".Length));
						    else
								if (line.StartsWith("GeoFence.bMsgBoxIn="))
								gpsSource.GeoFence.bMsgBoxIn=Convert.ToBoolean(line.Remove(0,"GeoFence.bMsgBoxIn=".Length));
                            else
                                if (line.StartsWith("GeoFence.bMsgBoxOut="))
                                    gpsSource.GeoFence.bMsgBoxOut = Convert.ToBoolean(line.Remove(0, "GeoFence.bMsgBoxOut=".Length));
						    else
								if (line.StartsWith("GeoFence.bSoundIn="))
								gpsSource.GeoFence.bSoundIn=Convert.ToBoolean(line.Remove(0,"GeoFence.bSoundIn=".Length));
                            else
                                if (line.StartsWith("GeoFence.bSoundOut="))
                                    gpsSource.GeoFence.bSoundOut = Convert.ToBoolean(line.Remove(0, "GeoFence.bSoundOut=".Length));
						    else
                                    if (line.StartsWith("GeoFence.colorLine="))
                                        gpsSource.GeoFence.colorLine = Color.FromArgb(Convert.ToInt32(line.Remove(0, "GeoFence.colorLine=".Length)));
                                    else
                                        if (line.StartsWith("GpsTrackLine.colorLine="))
                                            gpsSource.GeoFence.colorLine = Color.FromArgb(Convert.ToInt32(line.Remove(0, "GpsTrackLine.colorLine=".Length)));
                                        else
								if (line.StartsWith("GeoFence.arrayLat="))
                                {
                                    do
                                    {
                                        gpsSource.GeoFence.arrayLat.Add((float)Convert.ToDouble(line.Remove(0, "GeoFence.arrayLat=".Length)));
                                        line = sr.ReadLine();
                                        if (line != null && line.StartsWith("GeoFence.arrayLon="))
                                            gpsSource.GeoFence.arrayLon.Add((float)Convert.ToDouble(line.Remove(0, "GeoFence.arrayLon=".Length)));
                                        line = sr.ReadLine();
                                    } while (line != null && line.StartsWith("GeoFence.Array DONE") == false);
                                }
                                else
                                    if (line.StartsWith("GpsTrackLine.arrayLat="))
                                    {
                                        do
                                        {
                                            gpsSource.GeoFence.arrayLat.Add((float)Convert.ToDouble(line.Remove(0, "GpsTrackLine.arrayLat=".Length)));
                                            line = sr.ReadLine();
                                            if (line != null && line.StartsWith("GpsTrackLine.arrayLon="))
                                                gpsSource.GeoFence.arrayLon.Add((float)Convert.ToDouble(line.Remove(0, "GpsTrackLine.arrayLon=".Length)));
                                            line = sr.ReadLine();
                                        } while (line != null && line.StartsWith("GpsTrackLine.Array DONE") == false);
                                    }

						}
					}
				}
				else
				{
					SetDefaultSettings(bSet);
					m_bHandleControlValueChangeEvent=false;
				}
			}
				if (srReader==null)
					sr.Close();
			}
			#if !DEBUG
			catch(Exception) 
			{
				SetDefaultSettings(bSet);
				m_bHandleControlValueChangeEvent=false;
				if (m_gpsSourceList.Count>=1 && bSet)
					StartStop.Enabled=true;
				bRet=false;
			}
			#endif

			if (bSet)
			{
				int i;
				//set up tree view in different function
				for (i=0; i<m_gpsSourceList.Count; i++)
				{
					GPSSource gpsSource = (GPSSource)m_gpsSourceList[i];
					TreeNode treeNode;
					TreeNode treeNodeParent;

					treeNode = new TreeNode(gpsSource.sDescription);
					gpsSource.treeNode=treeNode;
					treeNode.Tag=gpsSource;

					switch (gpsSource.sType)
					{
						case "COM":
							treeNodeParent=m_treeNodeCOM;
							break;
						case "USB":
							treeNodeParent=m_treeNodeUSB;
							break;
						case "UDP":
							treeNodeParent=m_treeNodeUDP;
							break;
						case "TCP":
							treeNodeParent=m_treeNodeTCP;
							break;
						case "File":
							treeNodeParent=m_treeNodeFile;
							break;
						case "POI":
							treeNodeParent=m_treeNodePOI;
							break;
                        case "Track Line":
                            treeNodeParent = m_treeNodeTrack;
                            break;
						default:
							treeNodeParent=null;
							break;
					}

                    if (treeNodeParent != null)
                    {
                        treeNodeParent.Nodes.Add(treeNode);
                        if (gpsSource.sIconPath == "")
                            gpsSource.sIconPath = GpsTrackerPlugin.m_sPluginDirectory + "\\Gpsnotset.png";
                        if (!File.Exists(gpsSource.sIconPath))
                            gpsSource.sIconPath = GpsTrackerPlugin.m_sPluginDirectory + "\\Gpsx.png";
                        image = new Bitmap(gpsSource.sIconPath);
                        imageListGpsIcons.Images.Add(image);
                        treeNode.ImageIndex = imageListGpsIcons.Images.Count - 1;
                        treeNode.SelectedImageIndex = treeNode.ImageIndex;
                        if (gpsSource.bTrack)
                        {
                            treeNode.NodeFont = new System.Drawing.Font(treeViewSources.Font, FontStyle.Bold);
                            treeNode.Text = treeNode.Text + "     ";
                        }
                        treeNodeParent.ExpandAll();
                    }

				}

				checkBoxTrackHeading.Checked=m_bTrackHeading;
				checkBoxTrackLine.Checked=m_bTrackLine;
				checkBoxRecordSession.Checked=m_bRecordSession;
				checkBoxInformationText.Checked=m_bInfoText;
				for (i=0; i<comboBoxFile.Items.Count; i++)
                    if ((String)comboBoxFile.Items[i] == GpsTrackerPlugin.m_sPluginDirectory + "\\SampleSession.GPSTrackerSession")
						break;
				if (i==comboBoxFile.Items.Count)
                    comboBoxFile.Items.Add(GpsTrackerPlugin.m_sPluginDirectory + "\\SampleSession.GPSTrackerSession");

			}

			if (m_gpsSourceList.Count>=1 && bSet)
				StartStop.Enabled=true;

			m_bHandleControlValueChangeEvent=true;

			return bRet;
		}

		void SetDefaultSettings(bool bSet)
		{
			m_bHandleControlValueChangeEvent=false;

			if (bSet)
			{
				comboBoxUSBDevice.SelectedIndex=0;
				comboBoxCOMPort.Text="1";
				comboBoxBaudRate.Text="4800";
				comboBoxByteSize.Text="8";
				comboParity.SelectedIndex=0;
				comboBoxStopBits.SelectedIndex=0;
				trackBarFileSpeed.Value=1;
				numericUpDownUDPPort.Value=5555;
				numericUpDownTCPPort.Value=4444;
				comboBoxTcpIP.Text="";
				checkBoxSecureSocket.Checked=false;
                comboBoxFile.Text = GpsTrackerPlugin.m_sPluginDirectory + "\\SampleSession.GPSTrackerSession";
				m_bTrackHeading=false;
				m_bTrackLine=false;
				m_bRecordSession=false;
				m_bInfoText=true;
				//StartStop.Enabled=false;
                StartStop.Enabled = true;
				checkBoxNoDelay.Checked=false;
				comboBoxFlowControl.SelectedIndex=0;
				buttonTrackColorCOM.BackColor=Color.Blue;
				buttonTrackColorTCP.BackColor=Color.Yellow;
				buttonTrackColorUDP.BackColor=Color.LightGreen;
				buttonTrackColor.BackColor=Color.Red;


				checkBoxTrackHeading.Checked=m_bTrackHeading;
				checkBoxTrackLine.Checked=m_bTrackLine;
				checkBoxRecordSession.Checked=m_bRecordSession;
				checkBoxInformationText.Checked=m_bInfoText;

                comboBoxPositionUnits.SelectedIndex = 1;
                comboBoxDistanceUnits.SelectedIndex = 0;
			}

			m_bHandleControlValueChangeEvent=true;
		}

		#endregion

		#region User Interface
		//
		//User interface handlers
		//


		private void buttonBrowseGpsFile_Click(object sender, System.EventArgs e)
		{
			string [] sBabelData;
			int iSelectedIndex=-1;
			try
			{
				if (tabControlGPS.SelectedTab.Text=="Waypoints")
					iSelectedIndex=comboBoxWaypointsFileType.SelectedIndex;
				else
					iSelectedIndex=comboBoxTrackFileType.SelectedIndex;
			}
			catch(Exception)
			{
			}
			if (iSelectedIndex>=0 && iSelectedIndex<m_arrayGpsBabelFormats.Count)
				sBabelData = (string[])m_arrayGpsBabelFormats[iSelectedIndex];
			else
			{
				sBabelData = new string[3];
				sBabelData[0]= "text";
				sBabelData[1]= "txt";
				sBabelData[2]= "Text";
			}
		
				
			OpenFileDialog dlgOpenFile = new OpenFileDialog();
			dlgOpenFile.Title = "Select File" ;
			string sFilter;
			if (sBabelData[1]=="")
				sFilter="All files (*.*)|*.*";
			else
				sFilter="." + sBabelData[1] + " File (*." + sBabelData[1] + ")|*." + sBabelData[1] + "|All files (*.*)|*.*";
			dlgOpenFile.Filter = sFilter;
			dlgOpenFile.FilterIndex = 1 ;
			dlgOpenFile.RestoreDirectory = true ;
			if(dlgOpenFile.ShowDialog() == DialogResult.OK)
			{
				if (tabControlGPS.SelectedTab.Text=="Waypoints")
				{
					comboBoxWaypointsFile.Text=dlgOpenFile.FileName;
					int i;
					for (i=0; i<comboBoxWaypointsFile.Items.Count; i++)
						if ((String)comboBoxWaypointsFile.Items[i] == dlgOpenFile.FileName)
							break;
					if (i==comboBoxWaypointsFile.Items.Count)
						comboBoxWaypointsFile.Items.Add(dlgOpenFile.FileName);
				}
				else
				{
				comboBoxFile.Text=dlgOpenFile.FileName;
				int i;
				for (i=0; i<comboBoxFile.Items.Count; i++)
					if ((String)comboBoxFile.Items[i] == dlgOpenFile.FileName)
						break;
				if (i==comboBoxFile.Items.Count)
					comboBoxFile.Items.Add(dlgOpenFile.FileName);
			}

			}
		}

		private void buttonCheckForUpdates_Click(object sender, System.EventArgs e)
		{
            //Check to see if a new version is available
			WebUpdate();
		}

		private void tabControlGPS_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (tabControlGPS.SelectedTab.Name)
			{
				case "tabPageUpdate":
				buttonCheckForUpdates.Text="Check for Updates";
				labelTrackCode.Text="";
                textBoxVersionInfo.Text = "Your Version: " + GpsTrackerPlugin.m_sVersion + "\r\n";
					break;
			}
		}

		private void checkBoxTrackHeading_CheckedChanged(object sender, System.EventArgs e)
		{
			m_bTrackHeading=checkBoxTrackHeading.Checked;
		}

		private void checkBoxInformationText_CheckedChanged(object sender, System.EventArgs e)
		{
			m_bInfoText=checkBoxInformationText.Checked;
		}

		private void checkBoxTrackLine_CheckedChanged(object sender, System.EventArgs e)
		{
			m_bTrackLine=checkBoxTrackLine.Checked;
		}

		private void checkBoxRecordSession_CheckedChanged(object sender, System.EventArgs e)
		{
			m_bRecordSession=checkBoxRecordSession.Checked;
		}

		private void buttonTrackColor_Click(object sender, System.EventArgs e)
		{
			colorPicker.SolidColorOnly=true;
			colorPicker.Color=buttonTrackColor.BackColor;
			if (colorPicker.ShowDialog()== DialogResult.OK)
			{
				buttonTrackColor.BackColor=colorPicker.Color;
			}
		}


		private void button3_Click(object sender, System.EventArgs e)
		{
			colorPicker.SolidColorOnly=true;
			colorPicker.Color=buttonTrackColorCOM.BackColor;
			if (colorPicker.ShowDialog()== DialogResult.OK)
			{
				buttonTrackColorCOM.BackColor=colorPicker.Color;
			}		
		}

		private void buttonTrackColorUDP_Click(object sender, System.EventArgs e)
		{
			colorPicker.SolidColorOnly=true;
			colorPicker.Color=buttonTrackColorUDP.BackColor;
			if (colorPicker.ShowDialog()== DialogResult.OK)
			{
				buttonTrackColorUDP.BackColor=colorPicker.Color;
			}		
		}

		private void buttonTrackColorTCP_Click(object sender, System.EventArgs e)
		{
			colorPicker.SolidColorOnly=true;
			colorPicker.Color=buttonTrackColorTCP.BackColor;
			if (colorPicker.ShowDialog()== DialogResult.OK)
			{
				buttonTrackColorTCP.BackColor=colorPicker.Color;
			}		
		
		}
	
		private void menuItemAdd_Click(object sender, System.EventArgs e)
		{
			GPSSource gpsSource = new GPSSource();
			TreeNode treeNode;

			labelTrackCode.Text="";
			if (m_gpsSourceList.Count==m_iMaxDevices)
			{
				labelTrackCode.ForeColor = System.Drawing.Color.Red;
				labelTrackCode.Text="The max. number of GPS devices is " + m_iMaxDevices.ToString();
			}
			else
			{
				gpsSource.sType=treeViewSources.SelectedNode.Text;
				gpsSource.iNameIndex=GetAvailableIndex();
				gpsSource.sDescription=gpsSource.sType + " #" + gpsSource.iNameIndex;
				gpsSource.bTrack=false;
				gpsSource.bSetup=false;

				treeNode = new TreeNode(gpsSource.sDescription);
				treeNode.ImageIndex=1;
				treeNode.SelectedImageIndex=1;

				treeNode.Tag=gpsSource;
				gpsSource.treeNode=treeNode;
				gpsSource.bNeedApply=true;

                if (gpsSource.sType=="GeoFence")
                    listViewGeoFence.Items.Clear();

                if (gpsSource.sType == "Track Line")
                    listViewGeoFence.Items.Clear();

				m_gpsSourceList.Add(gpsSource);
				ApplySettings(gpsSource,false,false,false);

				treeViewSources.SelectedNode.Nodes.Add(treeNode);				
				treeViewSources.SelectedNode.ExpandAll();
				treeViewSources.SelectedNode=treeNode;
				SetupTabs();

				m_iSourceNameCount++;
				StartStop.Enabled=true;
			}
		}

		private bool ApplySettings(GPSSource gpsS, bool bNoSet, bool bCheck, bool bAddedPOI)
		{
			gpsS.bSetup=true;
			gpsS.bSave=true;

			if (bNoSet && bCheck)
			{
				labelTrackCode.Text="";
				int i;
				for (i=0; i<m_gpsSourceList.Count; i++)
				{
					GPSSource gpsSource=(GPSSource)m_gpsSourceList[i];
					GPSSource gpsSourceSelected=(GPSSource)treeViewSources.SelectedNode.Tag;

					if ( tabControlGPS.SelectedTab.Name == "tabPageCOM" && gpsSourceSelected!=gpsSource && gpsSource.bSetup==true &&
						(gpsSource.sType=="COM" && gpsSource.iCOMPort==Convert.ToInt32(comboBoxCOMPort.Text)))
					{
						labelTrackCode.ForeColor = System.Drawing.Color.Red;
						labelTrackCode.Text="COM Port " + comboBoxCOMPort.Text + " is already in use.";
						gpsS.bSetup=false;
						break;
					}

					if ( tabControlGPS.SelectedTab.Name == "tabPageUDP" && gpsSourceSelected!=gpsSource && gpsSource.bSetup==true &&
						( (gpsSource.sType=="UDP" && gpsSource.iUDPPort==(int)numericUpDownUDPPort.Value) ||
						(gpsSource.sType=="TCP" && gpsSource.iTCPPort==(int)numericUpDownUDPPort.Value) ) )
					{
						labelTrackCode.ForeColor = System.Drawing.Color.Red;
						labelTrackCode.Text="Port " + numericUpDownUDPPort.Value.ToString().Trim() + " is already in use.";
						gpsS.bSetup=false;
						break;
					}

					if ( tabControlGPS.SelectedTab.Name == "tabPageTCP" && gpsSourceSelected!=gpsSource && gpsSource.bSetup==true &&
						( (gpsSource.sType=="UDP" && gpsSource.iUDPPort==(int)numericUpDownTCPPort.Value) ||
						(gpsSource.sType=="TCP" && gpsSource.iTCPPort==(int)numericUpDownTCPPort.Value) ) )
					{
						labelTrackCode.ForeColor = System.Drawing.Color.Red;
						labelTrackCode.Text="Port " + numericUpDownTCPPort.Value.ToString().Trim() + " is already in use.";
						gpsS.bSetup=false;
						break;
					}
				}


#if !DEBUG
				try
#endif
			{
				float fLat;
				float fLon;
				if (tabControlGPS.SelectedTab.Name == "tabPagePOI")
				{
					fLat=Convert.ToSingle(textBoxLatitud.Text);
					fLon=Convert.ToSingle(textBoxLongitud.Text);
						
					if (fLat>=(float)-90 && fLat<=(float)90 &&
						fLon>=(float)-180 && fLon<=(float)180)
						gpsS.bSetup=true;
					else
					{
						gpsS.bSetup=false;
						labelTrackCode.ForeColor = System.Drawing.Color.Red;
						labelTrackCode.Text="Please enter a valid Latitud and Longitud.";
					}
				}
			}
#if !DEBUG
				catch (Exception)
				{
					gpsS.bSetup=false;
					labelTrackCode.ForeColor = System.Drawing.Color.Red;
					labelTrackCode.Text="Please enter a valid Latitud and Longitud.";
				}
#endif

			}
			
			if (gpsS.bSetup==true)
			{
                bool bExport = false;
                bool bExportVCOM = false;
                gpsS.bNMEAExport = false;
                gpsS.bNMEAExportVCOM = false;
                gpsS.sComment = "";
                gpsS.swExport = null;

				switch(gpsS.sType)
				{
					case "COM":
						gpsS.iCOMPort=Convert.ToInt32(comboBoxCOMPort.Text);
						gpsS.iBaudRate=Convert.ToInt32(comboBoxBaudRate.Text);
						gpsS.iByteSize=Convert.ToInt32(comboBoxByteSize.Text);
						gpsS.iParity=comboParity.SelectedIndex;
						gpsS.iStopBits=comboBoxStopBits.SelectedIndex;
						gpsS.iFlowControl=comboBoxFlowControl.SelectedIndex;
						gpsS.colorTrack=buttonTrackColorCOM.BackColor;
                        bExport = checkBoxCOMExport.Checked;
                        if (bNoSet == true)
                        {
                            gpsS.bShowPosition = checkBoxPosition.Checked;
                            gpsS.bShowName = checkBoxShowName.Checked;
                            gpsS.bShowComment = checkBoxShowComment.Checked;
                            gpsS.bShowSpeed = checkBoxSpeed.Checked;
                            gpsS.bShowHeading = checkBoxHeading.Checked;
                            gpsS.bShowTimeDate = checkBoxTimeDate.Checked;
                            gpsS.bShowTrackDistance = checkBoxTrackDistance.Checked;
                            gpsS.iInformationFontSize = comboBoxFontSize.SelectedIndex;
                            gpsS.colorInformation = buttonColorInformationText.BackColor;
                        }

						gpsS.bSetup=true;
						break;
					case "USB":
						gpsS.sUSBDevice=comboBoxUSBDevice.Text;
						gpsS.colorTrack=buttonTrackColorUSB.BackColor;
                        bExport = checkBoxUSBExport.Checked;
                        if (bNoSet == true)
                        {

                            gpsS.bShowPosition = checkBoxPosition.Checked;
                            gpsS.bShowName = checkBoxShowName.Checked;
                            gpsS.bShowComment = checkBoxShowComment.Checked;
                            gpsS.bShowSpeed = checkBoxSpeed.Checked;
                            gpsS.bShowHeading = checkBoxHeading.Checked;
                            gpsS.bShowTimeDate = checkBoxTimeDate.Checked;
                            gpsS.bShowTrackDistance = checkBoxTrackDistance.Checked;
                            gpsS.iInformationFontSize = comboBoxFontSize.SelectedIndex;
                            gpsS.colorInformation = buttonColorInformationText.BackColor;
                        }

						gpsS.bSetup=true;
						break;
                    case "Track Line":
                        int iVertices;
                        if (bCheck)
                        {
                            gpsS.bSetup = false;

                            for (iVertices = 0; iVertices < listViewGeoFence.Items.Count; iVertices++)
                            {
                                if (listViewGeoFence.Items[iVertices].SubItems[1].Text == "" || listViewGeoFence.Items[iVertices].SubItems[2].Text == "")
                                    break;
                            }
                            if (iVertices <= 1)
                            {
                                MessageBox.Show("Please, enter at least 2 vertices to create a Track Line.", "GpsTracker::Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }

                        if (bNoSet == true)
                        {
                            gpsS.bShowPosition = checkBoxPosition.Checked;
                            gpsS.bShowName = checkBoxShowName.Checked;
                            gpsS.bShowComment = checkBoxShowComment.Checked;
                            gpsS.bShowSpeed = checkBoxSpeed.Checked;
                            gpsS.bShowHeading = checkBoxHeading.Checked;
                            gpsS.bShowTimeDate = checkBoxTimeDate.Checked;
                            gpsS.bShowTrackDistance = checkBoxTrackDistance.Checked;
                            gpsS.iInformationFontSize = comboBoxFontSize.SelectedIndex;
                            gpsS.colorInformation = buttonColorInformationText.BackColor;
                        }
                        gpsS.colorTrack = buttonTrackColorTrackLine.BackColor;
                        gpsS.GeoFence.colorLine = gpsS.colorTrack;
                        gpsS.GeoFence.arrayLat.Clear();
                        gpsS.GeoFence.arrayLon.Clear();
                        for (iVertices = 0; iVertices < listViewGeoFence.Items.Count; iVertices++)
                        {
                            if (listViewGeoFence.Items[iVertices].SubItems[1].Text == "" || listViewGeoFence.Items[iVertices].SubItems[2].Text == "")
                                break;
                            gpsS.GeoFence.arrayLat.Add((float)Convert.ToDouble(listViewGeoFence.Items[iVertices].SubItems[1].Text));
                            gpsS.GeoFence.arrayLon.Add((float)Convert.ToDouble(listViewGeoFence.Items[iVertices].SubItems[2].Text));
                        }
                        gpsS.bSetup = true;
                        break;
					case "UDP":
						gpsS.iUDPPort=(int)numericUpDownUDPPort.Value;
						gpsS.colorTrack=buttonTrackColorUDP.BackColor;
                        bExport = checkBoxUDPExport.Checked;
                        if (bNoSet == true)
                        {

                            gpsS.bShowPosition = checkBoxPosition.Checked;
                            gpsS.bShowName = checkBoxShowName.Checked;
                            gpsS.bShowComment = checkBoxShowComment.Checked;
                            gpsS.bShowSpeed = checkBoxSpeed.Checked;
                            gpsS.bShowHeading = checkBoxHeading.Checked;
                            gpsS.bShowTimeDate = checkBoxTimeDate.Checked;
                            gpsS.bShowTrackDistance = checkBoxTrackDistance.Checked;
                            gpsS.iInformationFontSize = comboBoxFontSize.SelectedIndex;
                            gpsS.colorInformation = buttonColorInformationText.BackColor;
                        }

						gpsS.bSetup=true;
						break;
					case "TCP":
						gpsS.iTCPPort=(int)numericUpDownTCPPort.Value;
						gpsS.sTCPAddress=comboBoxTcpIP.Text.Trim();
						gpsS.colorTrack=buttonTrackColorTCP.BackColor;
						gpsS.bSecureSocket=checkBoxSecureSocket.Checked;
                        if (bNoSet == true)
                        {
                            gpsS.bShowPosition = checkBoxPosition.Checked;
                            gpsS.bShowName = checkBoxShowName.Checked;
                            gpsS.bShowComment = checkBoxShowComment.Checked;
                            gpsS.bShowSpeed = checkBoxSpeed.Checked;
                            gpsS.bShowHeading = checkBoxHeading.Checked;
                            gpsS.bShowTimeDate = checkBoxTimeDate.Checked;
                            gpsS.bShowTrackDistance = checkBoxTrackDistance.Checked;
                            gpsS.iInformationFontSize = comboBoxFontSize.SelectedIndex;
                            gpsS.colorInformation = buttonColorInformationText.BackColor;
                        }

						if (bNoSet==true && gpsS.sTCPAddress!="")
						{
							int i;
							for (i=0; i<comboBoxTcpIP.Items.Count; i++)
								if ((String)comboBoxTcpIP.Items[i] == gpsS.sTCPAddress)
									break;
							if (i==comboBoxTcpIP.Items.Count)
							{
								if (comboBoxTcpIP.Items.Count==10)
								{
									for (i=1; i<=9; i++)
										comboBoxTcpIP.Items[i-1]=comboBoxTcpIP.Items[i];
									comboBoxTcpIP.Items.RemoveAt(9);
								}
								comboBoxTcpIP.Items.Add(gpsS.sTCPAddress);
							}
						}
                        if (gpsS.sTCPAddress == "")
                            gpsS.bSetup = false;
                        else
                        {
                            bExport = checkBoxTCPExport.Checked;
                            gpsS.bSetup = true;
                        }
						break;
					case "File":
						gpsS.iReload=Convert.ToInt32(numericUpDownReload.Value);
						gpsS.bSession=false;
						string sFileName;
						if (tabControlGPS.SelectedTab.Text=="Waypoints")
						{
							gpsS.bWaypoints=true;
							gpsS.iReload=0;
							sFileName=comboBoxWaypointsFile.Text.Trim();
						}
						else
						{
							gpsS.bWaypoints=false;
							sFileName=comboBoxFile.Text.Trim();
						}


#if !DEBUG
						try
#endif	
					{
						if(File.Exists(sFileName) && bCheck && gpsS.bWaypoints==false)
						{
							StreamReader srReader = File.OpenText(comboBoxFile.Text.Trim());
							if (LoadSettings(srReader,false))
								gpsS.bSession=true;
							srReader.Close();
						}
					}
#if !DEBUG
						catch(Exception)
						{
							gpsS.bSetup=false;
						}
#endif
						gpsS.bSetup=true;
						if (!gpsS.bSetup)
						{
							labelTrackCode.ForeColor = System.Drawing.Color.Red;
							labelTrackCode.Text="Unable to open the selected file.";
						}
						else
						{
							string sDescription=gpsS.sDescription;
							if (!gpsS.bWaypoints && sDescription.StartsWith("Waypoints #"))
								sDescription="File #" + gpsS.sDescription.Substring(11);
							if (gpsS.bWaypoints && sDescription.StartsWith("File #"))
								sDescription="Waypoints #" + gpsS.sDescription.Substring(6);
							if (gpsS.bSession && sDescription.StartsWith("File #"))
								sDescription="Session #" + gpsS.sDescription.Substring(6);
							else
							if (checkBoxTrackAtOnce.Checked==true && gpsS.bSession==false && sDescription.StartsWith("File #"))
								sDescription="Track #" + gpsS.sDescription.Substring(6);

							gpsS.treeNode.Text=sDescription;
							gpsS.sDescription=sDescription;
							if (gpsS.bWaypoints==true)
								gpsS.sFileName=comboBoxWaypointsFile.Text.Trim();							
							else
							gpsS.sFileName=comboBoxFile.Text.Trim();							
							gpsS.bNoDelay=checkBoxNoDelay.Checked;
							gpsS.bTrackAtOnce=checkBoxTrackAtOnce.Checked;
							if (gpsS.bNoDelay)
								gpsS.iPlaySpeed=0;
							else
								gpsS.iPlaySpeed=trackBarFileSpeed.Value;
							gpsS.colorTrack=buttonTrackColor.BackColor;
							gpsS.bForcePreprocessing=checkBoxForcePreprocessing.Checked;

                            if (bNoSet == true)
                            {
                                gpsS.bShowPosition = checkBoxPosition.Checked;
                                gpsS.bShowName = checkBoxShowName.Checked;
                                gpsS.bShowComment = checkBoxShowComment.Checked;
                                gpsS.bShowSpeed = checkBoxSpeed.Checked;
                                gpsS.bShowHeading = checkBoxHeading.Checked;
                                gpsS.bShowTimeDate = checkBoxTimeDate.Checked;
                                gpsS.bShowTrackDistance = checkBoxTrackDistance.Checked;
                                gpsS.iInformationFontSize = comboBoxFontSize.SelectedIndex;
                                gpsS.colorInformation = buttonColorInformationText.BackColor;
                            }

							int iSelectedIndex=-1;
							try
							{
								if (gpsS.bWaypoints==true)
									iSelectedIndex=comboBoxWaypointsFileType.SelectedIndex;
								else
									iSelectedIndex=comboBoxTrackFileType.SelectedIndex;
							}
							catch(Exception)
							{
							}
							if (iSelectedIndex>=0 && iSelectedIndex<m_arrayGpsBabelFormats.Count)
								gpsS.saGpsBabelFormat = (string[])m_arrayGpsBabelFormats[iSelectedIndex];
							else
							{
								gpsS.saGpsBabelFormat = new string[3];
								gpsS.saGpsBabelFormat[0]="Unknown";
							}

							string sFormat="";
							if (gpsS.bWaypoints==true)
								sFormat = comboBoxWaypointsFileType.Text;
							else
								sFormat = comboBoxTrackFileType.Text;
							//Auto detect format from file extension, if possible
							if (sFormat=="Autodetect format from file name extension" && bCheck)
							{
								gpsS.saGpsBabelFormat[0]="";
								int iIndex=sFileName.LastIndexOf(".");
								if (iIndex>0 && iIndex<sFileName.Length)
								{
									string sExtension=sFileName.Substring(iIndex).ToLower();
									switch(sExtension)
									{
										case ".gpstrackersession":
											gpsS.saGpsBabelFormat[0]="GpsTracker";
											gpsS.saGpsBabelFormat[1]="GPSTrackerSession";
											gpsS.saGpsBabelFormat[2]="GpsTracker Session File";
											break;
										case ".trackatonce":
											gpsS.saGpsBabelFormat[0]="GpsTracker";
											gpsS.saGpsBabelFormat[1]="TrackAtOnce";
											gpsS.saGpsBabelFormat[2]="GpsTracker TrackAtOnce File";
											break;
										case ".nmea":
										case ".nmeatext":
										case ".nmeatxt":
											gpsS.saGpsBabelFormat[0]="nmea";
											gpsS.saGpsBabelFormat[1]="txt";
											gpsS.saGpsBabelFormat[2]="NMEA GPRMC and GPGGA sentences";
											break;
										case ".gpx":
											gpsS.saGpsBabelFormat[0]="gpx";
											gpsS.saGpsBabelFormat[1]="gpx";
											gpsS.saGpsBabelFormat[2]="GPX XML format";
											break;
										case ".kml":
											gpsS.saGpsBabelFormat[0]="kml";
											gpsS.saGpsBabelFormat[1]="kml";
											gpsS.saGpsBabelFormat[2]="Google Earth Markup Language";
											break;
										case ".pcx":
											gpsS.saGpsBabelFormat[0]="pcx";
											gpsS.saGpsBabelFormat[1]="pcx";
											gpsS.saGpsBabelFormat[2]="Garmin PCX5";
											break;
										case ".gpl":
											gpsS.saGpsBabelFormat[0]="gpl";
											gpsS.saGpsBabelFormat[1]="gpl";
											gpsS.saGpsBabelFormat[2]="DeLorme GPL";
											break;
										case ".upt":
											gpsS.saGpsBabelFormat[0]="magellanx";
											gpsS.saGpsBabelFormat[1]="upt";
											gpsS.saGpsBabelFormat[2]="Magellan SD files (as for eXplorist)";
											break;
										case ".gs":
											gpsS.saGpsBabelFormat[0]="maggeo";
											gpsS.saGpsBabelFormat[1]="gs";
											gpsS.saGpsBabelFormat[2]="Magellan Explorist Geocaching)";
											break;
										case ".usr":
											gpsS.saGpsBabelFormat[0]="lowranceusr";
											gpsS.saGpsBabelFormat[1]="usr";
											gpsS.saGpsBabelFormat[2]="Lowrance USR";
											break;
									}
								}
								if (gpsS.saGpsBabelFormat[0]=="")
								{
									MessageBox.Show("Could not auto detect the format of the file " + sFileName + "\r\nPlease try to manually select the correct format.","GpsTracker :: File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
									gpsS.bSetup=false;
									break;
								}
								else
								{
									if (gpsS.bWaypoints==true)
										comboBoxWaypointsFileType.Text = gpsS.saGpsBabelFormat[2];
									else
										comboBoxTrackFileType.Text = gpsS.saGpsBabelFormat[2];
								}

							}

							if (bNoSet==true && comboBoxFile.Text!="" && gpsS.bWaypoints==false)
							{
								int i;
								for (i=0; i<comboBoxFile.Items.Count; i++)
									if ((String)comboBoxFile.Items[i] == comboBoxFile.Text)
										break;

								if (i==comboBoxFile.Items.Count)
								{
									if (comboBoxFile.Items.Count==10)
									{
										for (i=2; i<=9; i++)
											comboBoxFile.Items[i-1]=comboBoxFile.Items[i];
										comboBoxFile.Items.RemoveAt(9);
									}

									comboBoxFile.Items.Add(comboBoxFile.Text);
								}
							}
							else
							if (bNoSet==true && comboBoxWaypointsFile.Text!="" && gpsS.bWaypoints==true)
							{
								int i;
								for (i=0; i<comboBoxWaypointsFile.Items.Count; i++)
									if ((String)comboBoxWaypointsFile.Items[i] == comboBoxWaypointsFile.Text)
										break;

								if (i==comboBoxWaypointsFile.Items.Count)
								{
									if (comboBoxWaypointsFile.Items.Count==10)
									{
										for (i=2; i<=9; i++)
											comboBoxWaypointsFile.Items[i-1]=comboBoxWaypointsFile.Items[i];
										comboBoxWaypointsFile.Items.RemoveAt(9);
									}

									comboBoxWaypointsFile.Items.Add(comboBoxWaypointsFile.Text);
								}
							}

                            if (gpsS.bSetup)
                            {
                                if (gpsS.bWaypoints == true)
                                {
                                    bExport = checkBoxWaypointsExport.Checked;
                                }
                                else
                                {
                                    bExport = checkBoxTrackExport.Checked;
                                }
                            }
						}

						break;
					case "POI":
						if (bNoSet==false)
						{
							gpsS.fLat=(float)0;
							gpsS.fLon=(float)0;
                            gpsS.bShowCircles = false;
						}
						else
						{
							if (!bAddedPOI)
							{
                                gpsS.bSetup = true;
                                bExport = checkBoxPOIExport.Checked;

                                gpsS.bShowPosition = checkBoxPosition.Checked;
                                gpsS.bShowName = checkBoxShowName.Checked;
                                gpsS.bShowComment = checkBoxShowComment.Checked;
                                gpsS.bShowSpeed = checkBoxSpeed.Checked;
                                gpsS.bShowHeading = checkBoxHeading.Checked;
                                gpsS.bShowTimeDate = checkBoxTimeDate.Checked;
                                gpsS.bShowTrackDistance = checkBoxTrackDistance.Checked;
                                gpsS.iInformationFontSize = comboBoxFontSize.SelectedIndex;
                                gpsS.colorInformation = buttonColorInformationText.BackColor;
							}
						}
						gpsS.bPOISet=false;
						break;
				}


                if (bNoSet == false)
                {
                    gpsS.bSetup = false;
                    gpsS.bNMEAExport = false;
                }

				if (gpsS.bSetup)
				{
					string sIconName;
					int iIconLimit;
					if (gpsS.sType=="POI")
					{
						sIconName="poi";
						iIconLimit=10;
					}
					else
					{
						sIconName="gps";
						iIconLimit=20;
					}

					if (gpsS.iNameIndex<=iIconLimit)
					{
                        gpsS.sIconPath = GpsTrackerPlugin.m_sPluginDirectory + "\\" + sIconName + Convert.ToString(gpsS.iNameIndex) + ".png";
						Bitmap image = new Bitmap(gpsS.sIconPath);
						imageListGpsIcons.Images.Add(image);
						gpsS.treeNode.ImageIndex=imageListGpsIcons.Images.Count-1;
						gpsS.treeNode.SelectedImageIndex=gpsS.treeNode.ImageIndex;
					}
					else
					{
                        gpsS.sIconPath = GpsTrackerPlugin.m_sPluginDirectory + "\\" + sIconName + "x.png";
						Bitmap image = new Bitmap(gpsS.sIconPath);
						imageListGpsIcons.Images.Add(image);
						gpsS.treeNode.ImageIndex=imageListGpsIcons.Images.Count-1;
						gpsS.treeNode.SelectedImageIndex=gpsS.treeNode.ImageIndex;
					}
                    gpsS.bNMEAExport = bExport;
                    gpsS.bNMEAExportVCOM = bExportVCOM;
				}
			}
			SaveSettings(null);

			return gpsS.bSetup;
		}

		private int GetAvailableIndex()
		{
			int iFreeIndex=0;

			if (m_gpsSourceList.Count>0)
			{
				for (int i=0; i<m_iMaxDevices; i++)
				{
					int ii;
					for (ii=0; ii<m_gpsSourceList.Count; ii++)
					{
						GPSSource gpsS = (GPSSource)m_gpsSourceList[ii];
						if (gpsS.iNameIndex==i)
							break;
					}
					if (ii==m_gpsSourceList.Count)
					{
						iFreeIndex=i;
						break;
					}
				}
			}

			return iFreeIndex;
		}

		private void treeViewSources_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (treeViewSources.GetNodeAt(e.X,e.Y)!=null)
			{
				treeViewSources.SelectedNode = treeViewSources.GetNodeAt(e.X,e.Y);
				SetupTabs();
			}
		}

		private void treeViewSources_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				switch (treeViewSources.SelectedNode.Tag.ToString())
				{
					case "COM":
					case "USB":
					case "TCP":
					case "UDP":
					case "File":
					case "POI":
                    case "TrackLine":
						menuItemAdd.Visible=true;
						menuItemDelete.Visible=true;
						menuItemRename.Visible=false;
						menuItemSetIcon.Visible=false;
						menuItemSetTrack.Visible=false;
						menuItemDelete.Text="Delete all " + treeViewSources.SelectedNode.Tag.ToString() + " sources";
						break;
					case "SOURCES":
						menuItemSetTrack.Visible=false;
						menuItemAdd.Visible=false;
						menuItemRename.Visible=false;
						menuItemSetIcon.Visible=false;
						menuItemDelete.Visible=true;
						menuItemDelete.Text="Delete all Sources";
						break;
					default:
                        switch (treeViewSources.SelectedNode.Parent.Text)
                        {
                            case "Track Line":
                                menuItemSetTrack.Visible = false;
                                menuItemAdd.Visible = false;
                                menuItemRename.Visible = true;
                                menuItemSetIcon.Visible = false;
                                menuItemDelete.Visible = true;
                                menuItemDelete.Text = "Delete " + treeViewSources.SelectedNode.Text;
                                break;
                            default:
						        menuItemSetTrack.Visible=true;
						        menuItemAdd.Visible=false;
						        menuItemRename.Visible=true;
						        menuItemSetIcon.Visible=true;
						        menuItemDelete.Visible=true;
						        menuItemDelete.Text="Delete " + treeViewSources.SelectedNode.Text;
                                break;
                        }
						break;
				}
				contextMenuSourceTree.Show(treeViewSources,new Point(e.X,e.Y));
			}

		}

		private void SetupTree()
		{

			treeViewSources.Nodes.Clear();
			m_gpsSourceList.Clear();
			m_iSourceNameCount=0;

			//StartStop.Enabled=false;
            StartStop.Enabled = true;

			TreeNode nodeSources=new TreeNode("Sources");
			nodeSources.Tag="SOURCES";
			treeViewSources.Nodes.Add(nodeSources);
			m_treeNodeCOM=new TreeNode("COM");
			m_treeNodeCOM.Tag="COM";
			nodeSources.Nodes.Add(m_treeNodeCOM);
			m_treeNodeUSB=new TreeNode("USB");
			m_treeNodeUSB.Tag="USB";
			nodeSources.Nodes.Add(m_treeNodeUSB);
			m_treeNodeUDP=new TreeNode("UDP");
			m_treeNodeUDP.Tag="UDP";
			nodeSources.Nodes.Add(m_treeNodeUDP);
			m_treeNodeTCP=new TreeNode("TCP");
			m_treeNodeTCP.Tag="TCP";
			nodeSources.Nodes.Add(m_treeNodeTCP);
			m_treeNodeFile=new TreeNode("File");
			m_treeNodeFile.Tag="File";
			nodeSources.Nodes.Add(m_treeNodeFile);
			m_treeNodePOI=new TreeNode("POI");
			m_treeNodePOI.Tag="POI";
			nodeSources.Nodes.Add(m_treeNodePOI);
            m_treeNodeTrack = new TreeNode("Track Line");
            m_treeNodeTrack.Tag = "TrackLine";
            nodeSources.Nodes.Add(m_treeNodeTrack);

			treeViewSources.SelectedNode=nodeSources;
			SetupTabs();

			treeViewSources.ExpandAll();
		}

		private void SetupTabs()
		{
			m_bHandleControlValueChangeEvent=false;
			
			if (treeViewSources.SelectedNode!=null)
			{
				switch (treeViewSources.SelectedNode.Tag.ToString())
				{
					case "COM":
					case "USB":
					case "TCP":
					case "UDP":
					case "File":
					case "POI":
                    case "TrackLine":
					case "SOURCES":
						try
						{
						tabControlGPS.TabPages.Clear();
						}
						catch(Exception)
						{
						}
						tabControlGPS.TabPages.Add(tabPageUsage);
						tabControlGPS.TabPages.Add(tabPageGeneral);
						tabControlGPS.TabPages.Add(tabPageUpdate);
						break;
					default:
						GPSSource gpsSource = (GPSSource)treeViewSources.SelectedNode.Tag;
						try
						{
						tabControlGPS.TabPages.Clear();
						}
						catch(Exception)
						{
						}

                        checkBoxPosition.Enabled = true;
                        checkBoxShowName.Enabled = true;
                        checkBoxShowComment.Enabled = true;
                        checkBoxSpeed.Enabled = true;
                        checkBoxHeading.Enabled = true;
                        checkBoxTimeDate.Enabled = true;
                        checkBoxTrackDistance.Enabled = true;

                        checkBoxPosition.Checked = gpsSource.bShowPosition;
                        checkBoxShowName.Checked = gpsSource.bShowName;
                        checkBoxShowComment.Checked = gpsSource.bShowComment;
                        checkBoxSpeed.Checked = gpsSource.bShowSpeed;
                        checkBoxHeading.Checked = gpsSource.bShowHeading;
                        checkBoxTimeDate.Checked = gpsSource.bShowTimeDate;
                        checkBoxTrackDistance.Checked = gpsSource.bShowTrackDistance;
                        comboBoxFontSize.SelectedIndex = gpsSource.iInformationFontSize;
                        buttonColorInformationText.BackColor = gpsSource.colorInformation;
					
					switch(gpsSource.sType)
					{
						case "USB":
							tabControlGPS.TabPages.Add(tabPageUSB);
                            tabControlGPS.TabPages.Add(tabPageInformationText); 
							tabControlGPS.TabPages.Add(tabPageGeneral);
							tabControlGPS.TabPages.Add(tabPageUSBHelp);


							if (gpsSource.bSetup)
							{
								comboBoxUSBDevice.Text=gpsSource.sUSBDevice;
								buttonTrackColorUSB.BackColor=gpsSource.colorTrack;

							}

                            checkBoxUSBExport.Checked = gpsSource.bNMEAExport;
							buttonApplyUSB.Enabled=gpsSource.bNeedApply;
                            buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
							break;

						case "COM":
							tabControlGPS.TabPages.Add(tabPageCOM);
                            tabControlGPS.TabPages.Add(tabPageInformationText); 
							tabControlGPS.TabPages.Add(tabPageGeneral);
							tabControlGPS.TabPages.Add(tabPageCOMHelp);
                            checkBoxCOMExport.Checked = gpsSource.bNMEAExport;

							if (gpsSource.bSetup)
							{
								comboBoxCOMPort.Text=Convert.ToString(gpsSource.iCOMPort);
								comboBoxBaudRate.Text=Convert.ToString(gpsSource.iBaudRate);
								comboBoxByteSize.Text=Convert.ToString(gpsSource.iByteSize);
								comboParity.SelectedIndex=gpsSource.iParity;
								comboBoxStopBits.SelectedIndex=gpsSource.iStopBits;
								comboBoxFlowControl.SelectedIndex=gpsSource.iFlowControl;
								buttonTrackColorCOM.BackColor=gpsSource.colorTrack;
							}

							buttonApply.Enabled=gpsSource.bNeedApply;
                            buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
							break;
						case "UDP":
							tabControlGPS.TabPages.Add(tabPageUDP);
                            tabControlGPS.TabPages.Add(tabPageInformationText); 
							tabControlGPS.TabPages.Add(tabPageGeneral);
							tabControlGPS.TabPages.Add(tabPageUDPHelp);
                            checkBoxUDPExport.Checked = gpsSource.bNMEAExport;

							if (gpsSource.bSetup)
							{
								numericUpDownUDPPort.Value=gpsSource.iUDPPort;
								buttonTrackColorUDP.BackColor=gpsSource.colorTrack;

							}
							buttonApplyUDP.Enabled=gpsSource.bNeedApply;
                            buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
							break;
						case "TCP":
							tabControlGPS.TabPages.Add(tabPageTCP);
                            tabControlGPS.TabPages.Add(tabPageInformationText); 
							tabControlGPS.TabPages.Add(tabPageGeneral);
							tabControlGPS.TabPages.Add(tabPageTCPHelp);
                            checkBoxTCPExport.Checked = gpsSource.bNMEAExport;

							if (gpsSource.bSetup)
							{
								numericUpDownTCPPort.Value=gpsSource.iTCPPort;
								comboBoxTcpIP.Text=gpsSource.sTCPAddress;
								buttonTrackColorTCP.BackColor=gpsSource.colorTrack;
							}

							buttonApplyTCP.Enabled=gpsSource.bNeedApply;
                            buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
							break;
						case "File":
							tabControlGPS.TabPages.Add(tabPageFile);
							tabControlGPS.TabPages.Add(tabPageWaypoints);
                            tabControlGPS.TabPages.Add(tabPageInformationText); 
							tabControlGPS.TabPages.Add(tabPageGeneral);
							tabControlGPS.TabPages.Add(tabPageFileHelp);
							comboBoxWaypointsFileType.SelectedIndex=0;
							comboBoxTrackFileType.SelectedIndex=0;

							if (gpsSource.bSetup)
							{
								if (gpsSource.bWaypoints==true)
								{
									comboBoxWaypointsFile.Text=gpsSource.sFileName;
									comboBoxWaypointsFileType.Text=gpsSource.saGpsBabelFormat[2];
									listViewWaypoints.Items.Clear();
									tabControlGPS.SelectedTab = tabPageWaypoints;
								}
								else
								{
								comboBoxFile.Text=gpsSource.sFileName;
									comboBoxTrackFileType.Text=gpsSource.saGpsBabelFormat[2];
									tabControlGPS.SelectedTab = tabPageFile;
								}
								if (gpsSource.iFilePlaySpeed>0)
									trackBarFileSpeed.Value=gpsSource.iFilePlaySpeed;
								checkBoxNoDelay.Checked=gpsSource.bNoDelay;
								checkBoxTrackAtOnce.Checked=gpsSource.bTrackAtOnce;
								checkBoxNoDelay.Checked=gpsSource.bNoDelay;
								buttonTrackColor.BackColor=gpsSource.colorTrack;
								checkBoxForcePreprocessing.Checked=gpsSource.bForcePreprocessing;
								numericUpDownReload.Value=gpsSource.iReload;
							}
                            if (gpsSource.bWaypoints == true)
                            {
                                checkBoxWaypointsExport.Checked = gpsSource.bNMEAExport;
                            }
                            else
                            {
                                checkBoxTrackExport.Checked = gpsSource.bNMEAExport;
                            }
							buttonApplyFile.Enabled=gpsSource.bNeedApply;
							buttonApplyWaypoints.Enabled=gpsSource.bNeedApply;
                            buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
							break;							
						case "POI":
							tabControlGPS.TabPages.Add(tabPagePOI);
             
                            tabControlGPS.TabPages.Add(tabPageInformationText); 
							tabControlGPS.TabPages.Add(tabPageGeneral);
							tabControlGPS.TabPages.Add(tabPagePOIHelp);
                            checkBoxPOIExport.Checked = gpsSource.bNMEAExport;

                            checkBoxSpeed.Enabled = false;
                            checkBoxSpeed.Checked = false;
                            checkBoxHeading.Enabled = false;
                            checkBoxHeading.Checked = false;
                            checkBoxTimeDate.Enabled = false;
                            checkBoxTimeDate.Checked = false;
                            checkBoxTrackDistance.Enabled = false;
                            checkBoxTrackDistance.Checked = false;
                            checkBoxShowComment.Enabled = false;
                            checkBoxShowComment.Checked = false;

							textBoxLatitud.Text=Convert.ToString(gpsSource.fLat);
							textBoxLongitud.Text=Convert.ToString(gpsSource.fLon);

							buttonApplyPOI.Enabled=gpsSource.bNeedApply;
                            buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
							break;
                        case "Track Line":
                            editBoxGeoFenceList.Hide();
                            tabPageGeoFence.Text = "Track Line Points";
                            tabControlGPS.TabPages.Add(tabPageGeoFence);
                            tabControlGPS.TabPages.Add(tabPageInformationText);
                            tabControlGPS.TabPages.Add(tabPageGeneral);
                            tabControlGPS.TabPages.Add(tabPageTrackLineHelp);
                            
                            checkBoxPosition.Enabled = false;
                            checkBoxPosition.Checked = false;
                            checkBoxSpeed.Enabled = false;
                            checkBoxSpeed.Checked = false;
                            checkBoxHeading.Enabled = false;
                            checkBoxHeading.Checked = false;
                            checkBoxTimeDate.Enabled = false;
                            checkBoxTimeDate.Checked = false;
                            checkBoxTrackDistance.Enabled = false;
                            checkBoxTrackDistance.Checked = false;
                            checkBoxShowComment.Enabled = false;
                            checkBoxShowComment.Checked = false;

                            if (gpsSource.bSetup)
                            {
                                buttonTrackColorTrackLine.BackColor = gpsSource.colorTrack;
                            }

                            listViewGeoFence.Items.Clear();
                            for (int i = 0; i < 100; i++)
                            {
                                ListViewItem lvItem = new ListViewItem();
                                lvItem.Text = Convert.ToString(i);
                                lvItem.SubItems.Add("");
                                lvItem.SubItems.Add("");
                                listViewGeoFence.Items.Add(lvItem);
                            }
                            for (int i = 0; i < gpsSource.GeoFence.arrayLat.Count; i++)
                            {
                                listViewGeoFence.Items[i].SubItems[1].Text = Convert.ToString(gpsSource.GeoFence.arrayLat[i]);
                                listViewGeoFence.Items[i].SubItems[2].Text = Convert.ToString(gpsSource.GeoFence.arrayLon[i]);
                            }


                            buttonApplyGeoFence.Enabled = gpsSource.bNeedApply;
                            buttonApplyInformationText.Enabled = gpsSource.bNeedApply;

                            break;				
					}

						break;
				}

				this.Text="GPSTracker :: " + treeViewSources.SelectedNode.Text;
			}

			m_bHandleControlValueChangeEvent=true;
		}

		private void menuItemDelete_Click(object sender, System.EventArgs e)
		{
			GPSSource gpsSource;

			switch (treeViewSources.SelectedNode.Tag.ToString())
			{
				case "COM":
				case "USB":
				case "TCP":
				case "UDP":
				case "File":
				case "POI":

					for (int i=0; i<m_gpsSourceList.Count; i++)
					{
						gpsSource = (GPSSource)m_gpsSourceList[i];
						if (gpsSource.sType==treeViewSources.SelectedNode.Tag.ToString())
						{
							treeViewSources.Nodes.Remove(gpsSource.treeNode);
							m_gpsSourceList.RemoveAt(i);
							i=-1;
						}
					}
					break;
				case "SOURCES":
					SetupTree();
					break;
				default:
					gpsSource = (GPSSource)treeViewSources.SelectedNode.Tag;
					treeViewSources.Nodes.Remove(gpsSource.treeNode);
					m_gpsSourceList.Remove(gpsSource);
					break;
			}

//			if (m_gpsSourceList.Count==0)
//				StartStop.Enabled=false;

			SetupTabs();

			SaveSettings(null);
		}

		private void menuItemSetTrack_Click(object sender, System.EventArgs e)
		{
			GPSSource gpsSource = (GPSSource)treeViewSources.SelectedNode.Tag;
			if (gpsSource.bTrack)
			{
				gpsSource.bTrack=false;
				treeViewSources.SelectedNode.NodeFont = new System.Drawing.Font(treeViewSources.Font, FontStyle.Regular);
			}
			else
			{
				for (int i=0; i<m_gpsSourceList.Count; i++)
				{
					gpsSource = (GPSSource)m_gpsSourceList[i];
					if (gpsSource.bTrack)
					{
						gpsSource.bTrack=false;
						gpsSource.treeNode.NodeFont = new System.Drawing.Font(treeViewSources.Font, FontStyle.Regular);
						break;
					}
				}
				gpsSource = (GPSSource)treeViewSources.SelectedNode.Tag;
				gpsSource.bTrack=true;
				treeViewSources.SelectedNode.Text+="     ";
				treeViewSources.SelectedNode.NodeFont = new System.Drawing.Font(treeViewSources.Font, FontStyle.Bold);
			}
		}

		private void menuItemSetIcon_Click(object sender, System.EventArgs e)
		{
#if !DEBUG
			try
#endif
		{
			OpenFileDialog dlgOpenFile = new OpenFileDialog();
			dlgOpenFile.Title = "Select Gps Device Icon" ;
			dlgOpenFile.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg" ;
			dlgOpenFile.FilterIndex = 0 ;
			dlgOpenFile.RestoreDirectory = true ;
			if(dlgOpenFile.ShowDialog() == DialogResult.OK)
			{
				Bitmap image = new Bitmap(dlgOpenFile.FileName);
				imageListGpsIcons.Images.Add(image);
				treeViewSources.SelectedNode.ImageIndex=imageListGpsIcons.Images.Count-1;
				treeViewSources.SelectedNode.SelectedImageIndex=treeViewSources.SelectedNode.ImageIndex;
				GPSSource gpsSource = (GPSSource)treeViewSources.SelectedNode.Tag;
				gpsSource.sIconPath=dlgOpenFile.FileName;

				SaveSettings(null);
			} 
		}
#if !DEBUG
			catch(Exception)
			{
			}		
#endif
		}

		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			GPSSource gpsSource=(GPSSource)treeViewSources.SelectedNode.Tag;
			EnableApply(!ApplySettings(gpsSource,true,true,false));
		}

		private void menuItemRename_Click(object sender, System.EventArgs e)
		{
			treeViewSources.LabelEdit=true;
			treeViewSources.SelectedNode.BeginEdit();
		}

		private void treeViewSources_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			if (e.Label!=null && e.Label.Length>0)
			{
				GPSSource gpsSource = (GPSSource)treeViewSources.SelectedNode.Tag;
				gpsSource.sDescription=e.Label;
				SaveSettings(null);
			}
			treeViewSources.LabelEdit=false;
	
		}

		private void ControlValueChanged(object sender, System.EventArgs e)
		{
			if (m_bHandleControlValueChangeEvent)
				EnableApply(true);
		}

		private void ControlValueChanged(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (m_bHandleControlValueChangeEvent)
				EnableApply(true);
		}

		private void EnableApply(bool bEnable)
		{
			GPSSource gpsSource = (GPSSource)treeViewSources.SelectedNode.Tag;
			gpsSource.bNeedApply=bEnable;
			
			switch(gpsSource.sType)
			{
				case "COM":
					buttonApply.Enabled=gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
					break;
                case "USB":
                    buttonApplyUSB.Enabled = gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
                    break;
				case "UDP":
					buttonApplyUDP.Enabled=gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
					break;
				case "TCP":
					buttonApplyTCP.Enabled=gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
					break;
				case "File":
					buttonApplyFile.Enabled=gpsSource.bNeedApply;
					buttonApplyWaypoints.Enabled=gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
					break;							
				case "POI":
					buttonApplyPOI.Enabled=gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
					break;
                case "GeoFence":
                    buttonApplyGeoFence.Enabled = gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
                    break;
                case "Track Line":
                    buttonApplyGeoFence.Enabled = gpsSource.bNeedApply;
                    buttonApplyInformationText.Enabled = gpsSource.bNeedApply;
                    break;	
			}

		}

		//
        //This is now the Track button handler
		private void StartStop_Click(object sender, System.EventArgs e)
		{
			bool fRet;
			GPSSource gpsSource;

			//if not initialized then Initialize
			if (m_fInitialized==false)
			{

				bool bContinue=true;
				for (int i=0; i<m_gpsSourceList.Count; i++)
				{
					gpsSource = (GPSSource)m_gpsSourceList[i];
					if (gpsSource.bSetup==false)
					{
						bContinue=false;
						break;
					}
				}
				if (bContinue==false)
				{
					if (MessageBox.Show("One or more sources have not been configured.\nContinue anyway?", "GPSTracker", MessageBoxButtons.YesNo,MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						bContinue=true;
				}

				if (bContinue)
				{
					labelTrackCode.Text="";
					fRet=true;

					m_gpsTrackerPlugin.gpsOverlay.RemoveAll();
					m_gpsTrackerPlugin.gpsOverlay.IsOn = true;
			
					//If we have COM devices then init the COM ports
					fRet=true;

					if (bContinue)
					{
						m_fInitialized=true;
						m_fPlayback=false;

						progressBarSetup.Maximum=m_gpsSourceList.Count+1;
						SaveSettings(null);
						m_iPlaybackSpeed=1;
						int i;
						for (i=0; i<m_gpsSourceList.Count; i++)
						{
							gpsSource = (GPSSource)m_gpsSourceList[i];
							if (gpsSource.bSetup)
							{
								if (gpsSource.sType=="File" && gpsSource.bSession==true)
								{
									if(File.Exists(gpsSource.sFileName))
									{
										m_iPlaybackSpeed = gpsSource.iPlaySpeed;
										m_sPlaybackFile = gpsSource.sFileName;
										m_srReader = File.OpenText(gpsSource.sFileName);
										if (LoadSettings(m_srReader,false))
										{
											m_srReader.Close();
											m_srReader = File.OpenText(m_sPlaybackFile);
											LoadSettings(m_srReader,true);
											m_fPlayback=true;
											m_bRecordSession=false;
											break;
										}

										m_srReader.Close();
										m_srReader=null;
									}
								}
							}
						}

						if (m_bRecordSession==false && m_fPlayback==false)
							LoadSettings(null,true);

						if (m_bRecordSession && m_fPlayback==false)
						{
                            m_swRecorder = File.CreateText(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.recording");
							SaveSettings(m_swRecorder);
						}

						for (i=0; i<m_gpsSourceList.Count && m_fPlayback==false; i++)
						{
							gpsSource = (GPSSource)m_gpsSourceList[i];
							if (gpsSource.sType=="COM" && gpsSource.bSetup)
							{
								// Instantiate base class event handlers.
								gpsSource.GpsCOM = new SerialPort(this, i, (uint)gpsSource.iCOMPort, (uint)gpsSource.iBaudRate, Convert.ToByte(gpsSource.iByteSize), Convert.ToByte(gpsSource.iParity), Convert.ToByte(gpsSource.iStopBits), (uint)gpsSource.iFlowControl);
								fRet = gpsSource.GpsCOM.Open();
								if (!fRet)
								{
									if (MessageBox.Show("Unable to initialize COM" + Convert.ToString(gpsSource.iCOMPort) + ":\nPlease check your port settings.\n\nContinue anyway?", "GPSTracker", MessageBoxButtons.YesNo,MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
										bContinue=true;
									else
									{
										CloseCOMs();
										bContinue=false;
										break;
									}
								}
							}
						}

                        if (bContinue)
                        {
                            Start(); //Start threads

                            if (checkBoxMessagesMonitor.Checked)
                            {
                                m_MessageMonitor = new MessageMonitor();
                                m_MessageMonitor.Show();
                            }
                            else
                                m_MessageMonitor = null;

                            //Hide the window
                            //this.Visible=false;
                            //Hide the window by moving it out of the screen
                            //By doing this instead of hideing the window then OnVisibleChanged
                            //does not get call with this.visible=false (this would deinitialize everything,
                            //see comment in OnVisibleChanged).
                            this.Left = -1000;
                            this.ShowInTaskbar = false;
                            //m_gpsTrackerPlugin.pluginAddOverlay();
                            m_gpsTrackerPlugin.pluginWorldWindowFocus();
                            progressBarSetup.Visible = false;
                            labelSettingup.Visible = false;
                        }
                        else
                        {
                            m_fInitialized = false;
                            Stop();
                        }
				    }
			    }
			}
			else
			{
				labelTrackCode.ForeColor = System.Drawing.Color.Black;
				labelTrackCode.Text="Tracking enabled...";
			}
		}

        private void buttonWaypointsList_Click(object sender, System.EventArgs e)
        {
            string sBabelCommandLine;
            string sFileName = comboBoxWaypointsFile.Text;
            int iSelectedIndex = -1;
            string[] sBabelFormat;
            try
            {
                iSelectedIndex = comboBoxWaypointsFileType.SelectedIndex;
            }
            catch (Exception)
            {
            }
            if (iSelectedIndex >= 0 && iSelectedIndex < m_arrayGpsBabelFormats.Count)
                sBabelFormat = (string[])m_arrayGpsBabelFormats[iSelectedIndex];
            else
            {
                sBabelFormat = new string[3];
                sBabelFormat[0] = "Unknown";
            }

            sBabelCommandLine = "-w;";
            sBabelCommandLine += "-i;" + sBabelFormat[0] + ";";
            sBabelCommandLine += "-f;" + sFileName + ";";
            sBabelCommandLine += "-o;csv;-F;" + sFileName + ".WaypointsCSV;";
            if (File.Exists(sFileName + ".WaypointsCSV"))
                File.Delete(sFileName + ".WaypointsCSV");

            m_File.m_GpsBabel.Execute(sBabelCommandLine);

            listViewWaypoints.Items.Clear();
            if (File.Exists(sFileName + ".WaypointsCSV"))
            {
                sFileName += ".WaypointsCSV";
                FileInfo fInfo = new FileInfo(sFileName);
                if (fInfo.Length > 0)
                {
                    ArrayList listWaypoints = new ArrayList();
                    StreamReader sReader = null;
                    try
                    {
                        sReader = File.OpenText(sFileName);
                        string sLine;
                        do
                        {
                            sLine = sReader.ReadLine();
                            if (sLine != null)
                                listWaypoints.Add(sLine);
                        } while (sLine != null);
                    }
                    catch (Exception)
                    {
                    }
                    if (sReader != null)
                        sReader.Close();

                    if (listWaypoints.Count > 0)
                    {
                        char[] cSep = { ',' };
                        for (int i = 0; i < listWaypoints.Count; i++)
                        {
                            string sFormat = (string)listWaypoints[i];
                            string[] sWaypointData;
                            sWaypointData = sFormat.Split(cSep);
                            if (sWaypointData.Length == 3)
                            {
                                ListViewItem lvItem = new ListViewItem();
                                lvItem.Text = Convert.ToString(i);
                                lvItem.SubItems.Add(sWaypointData[0].Trim());
                                lvItem.SubItems.Add(sWaypointData[1].Trim());
                                lvItem.SubItems.Add(sWaypointData[2].Trim());
                                listViewWaypoints.Items.Add(lvItem);
                            }
                        }
                    }
                }
            }
            else
                MessageBox.Show("Unable to Read Waypoint File: " + sFileName + "\r\nPlease, Check that the selected file format is correct.", "GpsTracker Error :: Waypoint File", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonTrackColorUSB_Click(object sender, System.EventArgs e)
        {
            colorPicker.SolidColorOnly = true;
            colorPicker.Color = buttonTrackColorUSB.BackColor;
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                buttonTrackColorUSB.BackColor = colorPicker.Color;
            }
        }

        private void buttonTestUSB_Click(object sender, System.EventArgs e)
        {
            //Use GpsBabel
            string sBabelCommandLine = "-T;-i;" + comboBoxUSBDevice.Text + ";-f;usb:;-o;kml;-F;" + GpsTrackerPlugin.m_sPluginDirectory + "\\USBTest.kml;";
            if (File.Exists(GpsTrackerPlugin.m_sPluginDirectory + "\\USBTest.kml"))
                File.Delete(GpsTrackerPlugin.m_sPluginDirectory + "\\USBTest.kml");
            m_File.m_GpsBabel.Execute(sBabelCommandLine);
            bool bError = true;
            if (File.Exists(GpsTrackerPlugin.m_sPluginDirectory + "\\USBTest.kml"))
            {
                FileInfo fInfo = new FileInfo(GpsTrackerPlugin.m_sPluginDirectory + "\\USBTest.kml");
                if (fInfo.Length > 0)
                    bError = false;
            }
            if (bError == true)
                MessageBox.Show("Unable to access the " + comboBoxUSBDevice.Text + " USB Gps Device or to get a Gps position from the device.\r\nPlease, confirm that the device is connected to a USB port in your computer and that it has a valid Gps lock.", "GpsTracker :: USB Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Successfully connected to the selected USB device.", "GpsTracker :: USB Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

		#endregion

		#region Misc functions
		//
		// Misc functions
		//
		public void timerCallbackLocked(Object obj)
		{
			if (m_iPrevLocked==m_iLocked)
			{
				m_gpsTrackerPlugin.pluginLocked(false);
				m_gpsTrackerPlugin.pluginShowFixInfo("GPSTracker: No Fix");
			}
			else
			{
				m_gpsTrackerPlugin.pluginLocked(true);
				m_gpsTrackerPlugin.pluginShowFixInfo("GPSTracker: Fix");				
			}

			m_iPrevLocked=m_iLocked;
		}

		private void threadPOI()
		{
            try
            {
                bool bStartAltitud = false;
                Thread.Sleep(500);
                while (!m_fCloseThreads)
                {
                    lock (LockPOI)
                    {
                        for (int i = 0; i < m_gpsSourceList.Count; i++)
                        {
                            GPSSource gpsSource = (GPSSource)m_gpsSourceList[i];
                            Thread.Sleep(100);

                            if (!gpsSource.bSetup)
                                continue;

                            if (gpsSource.sType == "POI" && gpsSource.bPOISet == false) //Point of interest
                            {
                                gpsSource.bPOISet = true;

                                GPSRenderInformation renderInfo = new GPSRenderInformation();
                                renderInfo.bPOI = true;
                                renderInfo.iIndex = i;
							    renderInfo.sDescription=gpsSource.sDescription.Trim();
                                renderInfo.fFix = false;
                                renderInfo.fLat = gpsSource.fLat;
                                renderInfo.fLon = gpsSource.fLon;
                                renderInfo.sIcon = gpsSource.sIconPath;
                                renderInfo.bShowInfo = m_bInfoText;
                                renderInfo.bTrackLine = m_bTrackLine;
                                renderInfo.gpsTrack = gpsSource.GpsPos.m_gpsTrack;
                                renderInfo.bRestartTrack = false;
                                renderInfo.iDay = gpsSource.GpsPos.m_iDay;
                                renderInfo.iMonth = gpsSource.GpsPos.m_iMonth;
                                renderInfo.iYear = gpsSource.GpsPos.m_iYear;
                                renderInfo.colorTrack = gpsSource.colorTrack;
                                m_gpsTrackerPlugin.pluginShowOverlay(renderInfo);
                                renderInfo.fTrackOnTop = gpsSource.bTrackOnTop;
                                renderInfo.bShowGrid = gpsSource.bShowGrid;
                                renderInfo.iSquareCount = gpsSource.iSquareCount;
                                renderInfo.dSquareSize = gpsSource.dSquareSize;
                                renderInfo.bGridKms = gpsSource.bGridKms;
                                renderInfo.colorGrid = gpsSource.colorGrid;
                                renderInfo.bShowCircles = gpsSource.bShowCircles;
                                renderInfo.iCirclesCount = gpsSource.iCirclesCount;
                                renderInfo.dCirclesEvery = gpsSource.dCirclesEvery;
                                renderInfo.iLinesEvery = gpsSource.iLinesEvery;
                                renderInfo.colorCircle = gpsSource.colorCircle;
                                renderInfo.bKms = gpsSource.bKms;
                                renderInfo.bShowPosition = gpsSource.bShowPosition;
                                renderInfo.bShowName = gpsSource.bShowName;
                                renderInfo.bShowComment = gpsSource.bShowComment;
                                renderInfo.bShowSpeed = gpsSource.bShowSpeed;
                                renderInfo.bShowHeading = gpsSource.bShowHeading;
                                renderInfo.bShowTimeDate = gpsSource.bShowTimeDate;
                                renderInfo.bShowTrackDistance = gpsSource.bShowTrackDistance;
                                renderInfo.bShowReferenceAngles = gpsSource.bShowReferenceAngles;
                                renderInfo.bShowReferenceDistance = gpsSource.bShowReferenceDistance;
                                renderInfo.iInformationFontSize = gpsSource.iInformationFontSize;
                                renderInfo.colorInformation = gpsSource.colorInformation;
                                renderInfo.iDistanceUnit = gpsSource.iDistanceUnit;
                                renderInfo.iPositionUnit = gpsSource.iPositionUnit;
                                if (gpsSource.bTrack)
                                {
                                    if (bStartAltitud == true)
                                        gpsSource.iStartAltitud = 0;
                                    m_gpsTrackerPlugin.pluginWorldWindowGotoLatLonHeading(Convert.ToSingle(gpsSource.fLat), Convert.ToSingle(gpsSource.fLon), -1F, gpsSource.iStartAltitud);
                                }

                            }
                            else
                                if (gpsSource.sType == "POI" && gpsSource.bTrack)
                                {
                                    if (bStartAltitud == true)
                                        gpsSource.iStartAltitud = 0;
                                    m_gpsTrackerPlugin.pluginWorldWindowGotoLatLonHeading(Convert.ToSingle(gpsSource.fLat), Convert.ToSingle(gpsSource.fLon), -1F, gpsSource.iStartAltitud);
                                }
                            if (gpsSource.iStartAltitud > 0 && bStartAltitud==false)
                            {
                                Thread.Sleep(3000);
                                bStartAltitud = true;
                                gpsSource.iStartAltitud = 0;
                            }


                            if (m_fCloseThreads)
                                break;
                        }


                    }

                    if (!m_fCloseThreads)
                        Thread.Sleep(1000);
                }
                m_eventPOIThreadSync.Set();
            }
            catch (Exception)
            {
            }
		}

        //
        //Check to see if a new version is available
		public void WebUpdate()
		{
			#if !DEBUG
			try
			#endif
			{
				WebClient myWebClient = new WebClient();
				if (buttonCheckForUpdates.Text=="Check for Updates")
				{
					labelTrackCode.Text="";
                    textBoxVersionInfo.Text = "Your Version: " + GpsTrackerPlugin.m_sVersion + "\r\n";
                    if (File.Exists(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTrackerManifest.txt"))
                        File.Delete(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTrackerManifest.txt");
                    myWebClient.DownloadFile("http://www.javier.santoro.efirehose.net/GpsTrackerWorldWind/GpsTrackerManifest.txt", GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTrackerManifest.txt"); 
				}

				#if !DEBUG
				try
				#endif
				{
                    if (!File.Exists(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTrackerManifest.txt"))
					{
						labelTrackCode.ForeColor = System.Drawing.Color.Red;
						labelTrackCode.Text="Unable to update GPSTracker...";
						return;
					}
                    using (StreamReader sr = File.OpenText(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTrackerManifest.txt"))
					{
						string line=sr.ReadLine();
						string sVersion=line;
                        char [] cSplit = {'R'};
                        //server version
                        string[] sVR = sVersion.Split(cSplit);
                        string sV = sVR[0].Substring(1);
                        string sR = sVR[1];
                        int iVersion = Convert.ToInt32(sV);
                        int iRevision = Convert.ToInt32(sR);
                        //current version
                        sVR = GpsTrackerPlugin.m_sVersion.Split(cSplit);
                        sV = sVR[0].Substring(1);
                        sR = sVR[1];
                        int iCVersion = Convert.ToInt32(sV);
                        int iCRevision = Convert.ToInt32(sR);

                        if (line != null && (iVersion > iCVersion || (iVersion == iCVersion && iRevision > iCRevision)))
						{
							sr.ReadLine();
							string sNotes="";
							do
							{
								line=sr.ReadLine();
								if (line.StartsWith("-")==true)
									break;
								sNotes+= line + "\r\n";
							} while(true);
							if (buttonCheckForUpdates.Text=="Check for Updates")
							{
								string sMsg="There's a new version of GPSTracker.\r\n";
                                sMsg += "Your Version: " + GpsTrackerPlugin.m_sVersion + "\r\n";
								sMsg+="New Version: " + sVersion + "\r\n\r\n";
								sMsg+="Version Notes:\r\n";
								sMsg+=sNotes;
								textBoxVersionInfo.Text=sMsg;

								buttonCheckForUpdates.Text="Go to Update Page";
							}
							else
								System.Diagnostics.Process.Start("http://www.worldwindcentral.com/wiki/Add-on:GPS_Tracker_(plugin)");
						}
						else
						if (line!=null)
						{
							string sMsg="There's no need for an update.\r\n";
                            sMsg += "Your Version: " + GpsTrackerPlugin.m_sVersion + "\r\n";
							sMsg+="Update Version: " + sVersion + "\r\n\r\n";
							textBoxVersionInfo.Text=sMsg;
						}
					}
				} 
				#if !DEBUG
				catch(Exception) 
				{
					labelTrackCode.ForeColor = System.Drawing.Color.Red;
					labelTrackCode.Text="Unable to update GPSTracker...";
					buttonCheckForUpdates.Text="Check for Updates";
				}
				#endif

			}
			#if !DEBUG
			catch(Exception)
			{
				labelTrackCode.ForeColor = System.Drawing.Color.Red;
				labelTrackCode.Text="Unable to update GPSTracker...";
				buttonCheckForUpdates.Text="Check for Updates";
			}
			#endif
		}

		#endregion

        #region GeoFence

        private void buttonExportPathBrowse_Click(object sender, EventArgs e)
        {
            folderBrowser.Description = "Select the path to the exported NMEA file(s).";
            folderBrowser.ShowDialog();
            textBoxNMEAExportPath.Text = folderBrowser.SelectedPath;
        }

        public void AddGeoFence(GPSGeoFenceData geoFence)
        {
            lock ("GeoFenceAccess")
            {
                m_bHandleControlValueChangeEvent = false;

                GPSSource gpsSource = new GPSSource();
                TreeNode treeNode;

                if (geoFence.bGeoFence)
                gpsSource.sType = "GeoFence";
                else
                    gpsSource.sType = "Track Line";
                gpsSource.sDescription = geoFence.sName;
                gpsSource.GeoFence = geoFence;
                gpsSource.iNameIndex = GetAvailableIndex();
                gpsSource.bSave = true;
                gpsSource.bTrack = false;
                gpsSource.bSetup = true;
                gpsSource.bNeedApply = false;
                gpsSource.iDistanceUnit = m_iDistanceUnit;
                gpsSource.iPositionUnit = m_iPositionUnit;
                gpsSource.iInformationFontSize = 0;

                treeNode = new TreeNode(gpsSource.sDescription);
                treeNode.ImageIndex = 1;
                treeNode.SelectedImageIndex = 1;
                treeNode.Tag = gpsSource;
                gpsSource.treeNode = treeNode;
                m_gpsSourceList.Add(gpsSource);


                m_treeNodeTrack.Nodes.Add(treeNode);
                m_treeNodeTrack.ExpandAll();


                listViewGeoFence.Items.Clear();
                for (int i = 0; i < 100; i++)
                {
                    ListViewItem lvItem = new ListViewItem();
                    lvItem.Text = Convert.ToString(i);
                    lvItem.SubItems.Add("");
                    lvItem.SubItems.Add("");
                    listViewGeoFence.Items.Add(lvItem);
                }
                for (int i = 0; i < gpsSource.GeoFence.arrayLat.Count; i++)
                {
                    listViewGeoFence.Items[i].SubItems[1].Text = Convert.ToString(gpsSource.GeoFence.arrayLat[i]);
                    listViewGeoFence.Items[i].SubItems[2].Text = Convert.ToString(gpsSource.GeoFence.arrayLon[i]);
                }

                ApplySettings(gpsSource, true, false, true);

                m_iSourceNameCount++;

                m_bHandleControlValueChangeEvent = true;
            }
        }

		private ListViewItem m_liGeoFenceVertices;
		private int m_iGeoFenceSelectedX=0;
        private int m_iGeoFenceSelectedY= 0;
		private int m_iGeoFenceSubItemSelected = 0 ; 

        private void listViewGeoFence_DoubleClick(object sender, EventArgs e)
        {
			// Check the subitem clicked .
            int iStart = m_iGeoFenceSelectedX;
			int iSpos = 0 ; 
			int iEpos = 0;
			for ( int i=0; i < listViewGeoFence.Columns.Count ; i++)
			{
                iSpos = iEpos;
                iEpos = iSpos + listViewGeoFence.Columns[i].Width;
                if (iStart > iSpos && iStart < iEpos) 
				{
                    m_iGeoFenceSubItemSelected = i;
					break; 
				}
			}

            if (m_iGeoFenceSubItemSelected != 0)
            {
                Rectangle r = new Rectangle(iSpos, m_liGeoFenceVertices.Bounds.Y, iEpos, m_liGeoFenceVertices.Bounds.Bottom);
                editBoxGeoFenceList.Size = new System.Drawing.Size(iEpos - iSpos, m_liGeoFenceVertices.Bounds.Bottom - m_liGeoFenceVertices.Bounds.Top);
                editBoxGeoFenceList.Location = new System.Drawing.Point(iSpos + listViewGeoFence.Left, m_liGeoFenceVertices.Bounds.Y + listViewGeoFence.Top);
                editBoxGeoFenceList.Show();
                editBoxGeoFenceList.Text = m_liGeoFenceVertices.SubItems[m_iGeoFenceSubItemSelected].Text;
                editBoxGeoFenceList.SelectAll();
                editBoxGeoFenceList.Focus();
            }
		}

        private void listViewGeoFence_MouseDown(object sender, MouseEventArgs e)
        {
            m_liGeoFenceVertices = listViewGeoFence.GetItemAt(e.X, e.Y);
            m_iGeoFenceSelectedX = e.X;
            m_iGeoFenceSelectedY = e.Y;        
        }

        private void editBoxGeoFenceList_KeyPress(object sender, KeyPressEventArgs e)
        {
			if ( e.KeyChar == 13 ) 
			{
                m_liGeoFenceVertices.SubItems[m_iGeoFenceSubItemSelected].Text = editBoxGeoFenceList.Text;
				editBoxGeoFenceList.Hide();
			}

			if ( e.KeyChar == 27 ) 
				editBoxGeoFenceList.Hide();    
        }

        private void editBoxGeoFenceList_Leave(object sender, EventArgs e)
        {
            m_liGeoFenceVertices.SubItems[m_iGeoFenceSubItemSelected].Text = editBoxGeoFenceList.Text;
            editBoxGeoFenceList.Hide();
        }


        #endregion

        #region Configuration Manager

        private void buttonSaveConfiguration_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();

            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "Save Current GpsTracker Configuration.";
            saveDlg.Filter = "GpsTracker config (*.cfg)|*.cfg" ;
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                textBoxSaveConfiguration.Text = saveDlg.FileName;
                SaveSettingsToFile(saveDlg.FileName);
            }
        }


        private void textBoxSaveConfiguration_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SaveSettingsToFile(textBoxSaveConfiguration.Text);
        }


        private void SaveSettingsToFile(string sFileName)
        {
            m_fPlayback = false;
            SaveSettings(null);
            try
            {
                File.Copy(GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.cfg", sFileName, true);
                MessageBox.Show("Current configuration saved to file: " + sFileName , "GpsTracker :: Configuration saved.", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to save the current configuration to file " + sFileName + ", Please check that the selected file path is valid.", "GpsTracker :: Error Message", MessageBoxButtons.OK);
            }
        }



        private void buttonLoadConfiguration_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Title = "Select a Configuration file to load.";
            openDlg.Filter = "GpsTracker config (*.cfg)|*.cfg";
            openDlg.FilterIndex = 0;
            openDlg.RestoreDirectory = true;
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                LoadConfiguration(openDlg.FileName);
            }
        }


        private void buttonLoadConfigurationFile_Click(object sender, EventArgs e)
        {
            LoadConfiguration(comboBoxConfigurationFile.Text);
        }


        private void comboBoxConfigurationFile_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadConfiguration(comboBoxConfigurationFile.Text);
        }

        void LoadConfiguration(string sFileName)
        {
            bool bError = true;
            LoadSettings(null, false);
            if (LoadSettings(null, false))
            {
                try
                {
                    comboBoxConfigurationFile.Text = sFileName;
                    File.Copy(sFileName, GpsTrackerPlugin.m_sPluginDirectory + "\\GpsTracker.cfg", true);
                    int i;
                    for (i=0; i<comboBoxConfigurationFile.Items.Count; i++)
                    {
                        if (comboBoxConfigurationFile.Text == (string)comboBoxConfigurationFile.Items[i])
                            break;
                    }
                    if (i==comboBoxConfigurationFile.Items.Count)
                        comboBoxConfigurationFile.Items.Add(sFileName);
                    LoadSettings(null, true);
                    bError = false;
                }
                catch (Exception)
                {
                }
            }
            if (bError)
                MessageBox.Show("Unable to load the configuration file " + sFileName + ", Please check that the selected file path is valid.", "GpsTracker :: Error Message", MessageBoxButtons.OK);
 
        }


        #endregion


        private void buttonColorInformationText_Click(object sender, EventArgs e)
        {
            colorPicker.SolidColorOnly = true;
            colorPicker.Color = buttonColorInformationText.BackColor;
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                buttonColorInformationText.BackColor = colorPicker.Color;
            }
        }

        private void buttonTrackColorTrackLine_Click(object sender, EventArgs e)
        {
            colorPicker.SolidColorOnly = true;
            colorPicker.Color = buttonTrackColorTrackLine.BackColor;
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                buttonTrackColorTrackLine.BackColor = colorPicker.Color;
            }
        }


        private void buttonSaveConfig_Click(object sender, EventArgs e)
        {
            SaveSettingsToFile(textBoxSaveConfiguration.Text);
        }

    }
}

