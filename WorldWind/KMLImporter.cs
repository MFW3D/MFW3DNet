//----------------------------------------------------------------------------
// NAME: KMLImporter
// DESCRIPTION: KMLImporter allows you to import Placemarks from KML and KMZ files
// DEVELOPER: ShockFire
// WEBSITE: http://shockfire.blogger.com
// VERSION: 1.08
//----------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using WorldWind;
using WorldWind.Net;
using WorldWind.Renderable;
using WorldWind.KMLReader;
using Utility;

namespace KMLPlugin
{
	/// <summary>
	/// Main plugin class
	/// </summary>
	public class KMLImporter : WorldWind.PluginEngine.Plugin
	{
		private const string version = "1.08";				// The version of this plugin

		// private const int IconSizeConstant = 32;			// The default icon size used for scaling

		private Icons m_KMLIcons;							// Main Icon container

		private MenuItem tempMenu = new MenuItem();			// Temp menu item for storing file MenuItems
		private MenuItem aboutMenuItem = new MenuItem();	// About menu item
		private MenuItem pluginMenuItem = new MenuItem();	// Plugin menu item (for children)
		private MenuItem napalmMenuItem = new MenuItem();	// Napalm enable/disable menu item
		private MenuItem labelMenuItem = new MenuItem();	// drawAllLabels enable/disable menu item

		internal string KMLPath;							// Temp internal argument passing variable

        private KMLLoader m_loader = new KMLLoader();

        private KMLParser m_parser = new KMLParser();

		// private World m_world;								// The world this KMLImporter is associated with

		static string KmlDirectory = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "kml");

		#region Plugin methods
		/// <summary>
		/// Loads this plugin. Initializes variables and adds layers and menu items
		/// </summary>
		public override void Load()
		{
			// Load settings
			Settings.LoadSettings(Path.Combine(KmlDirectory, "KMLImporter.xml"));

			// Initialize the main Icons layer
			m_KMLIcons = new Icons("KML Icons");
			m_KMLIcons.IsOn = false;

            // Setup Drag&Drop functionality
            Global.worldWindow.DragEnter += new DragEventHandler(WorldWindow_DragEnter);
            Global.worldWindow.DragDrop += new DragEventHandler(WorldWindow_DragDrop);

			// Add a menu item to the File menu and the Help menu
			MenuItem loadMenuItem = new MenuItem();
			loadMenuItem.Text = "Import KML/KMZ file...";
			loadMenuItem.Click += new EventHandler(loadMenu_Click);
			aboutMenuItem.Text = "About KMLImporter";
			aboutMenuItem.Click += new EventHandler(aboutMenu_Click);
			int mergeOrder = 0;
			// Napalm enable/disable menu item
			bool bEnabled = Napalm.NapalmIsEnabled(KmlDirectory);
			if (bEnabled)
				napalmMenuItem.Text = "Disable KMLImporter autoupdate";
			else
				napalmMenuItem.Text = "Enable KMLImporter autoupdate";
			napalmMenuItem.Click += new EventHandler(napalmMenu_Click);
			pluginMenuItem.MenuItems.Add(napalmMenuItem);

			// Allways show labels enable/disable menu item
			labelMenuItem.Text = "Show all labels";
			labelMenuItem.Checked = Settings.ShowAllLabels;
			labelMenuItem.Click += new EventHandler(labelMenuItem_Click);
			pluginMenuItem.MenuItems.Add(labelMenuItem);

			// Some magic to provide backward compability
			Type typecontroller = typeof(MainApplication);
			System.Reflection.PropertyInfo finfo = typecontroller.GetProperty("CmdArgs", BindingFlags.Static|BindingFlags.Public|BindingFlags.GetProperty);
			string[] temp = null;
			if(finfo != null)
			{
				temp = (string[])finfo.GetValue(null, null);

				// If command line arguments are available, try to find one pointing to a kml/kmz file
				if (temp != null)
				{
					foreach (string arg in temp)
					{
						if (!File.Exists(arg))
							continue;

						string fExt = Path.GetExtension(arg);

						if (fExt != ".kml" && fExt != ".kmz")
							continue;

						LoadDiskKM(arg);
						break;
					}
				}
			}

            // Add the main Icons layer to the globe
            Global.worldWindow.CurrentWorld.RenderableObjects.Add(m_KMLIcons);

			//Set the currentworld
			// m_world = m_Global.worldWindow.CurrentWorld;

			base.Load();
		}

		/// <summary>
		/// Unloads this plugin. Removes layers and menu items
		/// </summary>
		public override void Unload()
		{
			// Cleanup
			Cleanup();

            m_parser.Cleanup();

			// Save settings
			Settings.SaveSettings(Path.Combine(KmlDirectory, "KMLImporter.xml"));

            // Remove the icon layer
            Global.worldWindow.CurrentWorld.RenderableObjects.Remove(m_KMLIcons);

            // Disable Drag&Drop functionality
            Global.worldWindow.DragEnter -= new DragEventHandler(WorldWindow_DragEnter);
            Global.worldWindow.DragDrop -= new DragEventHandler(WorldWindow_DragDrop);

			tempMenu.MenuItems.Clear();
			
			try
			{
				// Delete the temp kmz extract directory
				if (Directory.Exists(Path.Combine(KmlDirectory, "kmz")))
					Directory.Delete(Path.Combine(KmlDirectory, "kmz"), true);

				foreach (string kmlfile in Directory.GetFiles(KmlDirectory, "*.kml"))
				{
					try
					{
						File.Delete(kmlfile);
					}
					catch (System.IO.IOException)
					{	}
				}

				foreach (string kmzfile in Directory.GetFiles(KmlDirectory, "*.kmz"))
				{
					try
					{
						File.Delete(kmzfile);
					}
					catch (System.IO.IOException)
					{	}
				}
			}
			catch (Exception) {}

			base.Unload();
		}

		#endregion

		#region KMx loading methods
		/// <summary>
		/// Loads either a KML or KMZ file from disk
		/// </summary>
		/// <param name="filename"></param>
		private void LoadDiskKM(string filename)
		{
            Spawn_LoadKML(filename);
            m_KMLIcons.IsOn = true;
		}

		/// <summary>
		/// Loads a KML file in a new thread
		/// </summary>
		/// <param name="path">The path to the KML file to load</param>
		private void Spawn_LoadKML(string path)
		{
			KMLPath = path;

			ThreadStart threadStart = new ThreadStart(LoadKMLFile);
			Thread kmlThread = new System.Threading.Thread(threadStart);
			kmlThread.Name = "KMLImporter worker thread";
			kmlThread.IsBackground = true;
			kmlThread.Start();

			Napalm.Update(KmlDirectory, version);
		}

		/// <summary>
		/// Loads a KML file
		/// </summary>
		private void LoadKMLFile()
		{
			if (KMLPath == null || m_KMLIcons == null)
				return;

			Cleanup();
            m_parser.Cleanup();

			WaitMessage waitMessage = new WaitMessage();
			m_KMLIcons.Add(waitMessage);

			// Create a reader to read the file
			try
			{
                string kml = m_loader.LoadKML(KMLPath);
                KMLPath = m_loader.RealKMLPath;
                if (kml != null)
                {
                    try
                    {
                        // Load the actual kml data
                        m_parser.ReadKML(kml, m_KMLIcons, KMLPath);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(Log.Levels.Error, "KMLImporter: " + ex.ToString());
                        MessageBox.Show(
                            String.Format(CultureInfo.InvariantCulture, "Error loading KML file '{0}':\n\n{1}", KMLPath, ex.ToString()),
                            "KMLImporter error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1,
                            true ? MessageBoxOptions.RtlReading : MessageBoxOptions.ServiceNotification);
                    }
                }
			}
			catch(Exception ex) // Catch error if stream reader failed
			{
				Log.Write(Log.Levels.Error, "KMLImporter: " + ex.ToString());
				MessageBox.Show(
					String.Format(CultureInfo.InvariantCulture, "Error opening KML file '{0}':\n\n{1}", KMLPath, ex.ToString()), 
					"KMLImporter error", 
					MessageBoxButtons.OK,
					MessageBoxIcon.Error,
					MessageBoxDefaultButton.Button1,
					true ? MessageBoxOptions.RtlReading : MessageBoxOptions.ServiceNotification);
			}

			// Cleanup
			m_KMLIcons.Remove(waitMessage);
			KMLPath = null;				
		}


        #region Drag&Drop handling methods
        /// <summary>
        /// Checks if the object being dropped is a kml or kmz file
        /// </summary>
        private void WorldWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (DragDropIsValid(e))
                e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// Handles dropping of a kml/kmz file (by loading that file)
        /// </summary>
        private void WorldWindow_DragDrop(object sender, DragEventArgs e)
        {
            if (DragDropIsValid(e))
            {
                // transfer the filenames to a string array
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0 && File.Exists(files[0]))
                {
                    LoadDiskKM(files[0]);
                }
            }
        }

        /// <summary>
        /// Determines if this plugin can handle the dropped item
        /// </summary>
        private static bool DragDropIsValid(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                if (((string[])e.Data.GetData(DataFormats.FileDrop)).Length == 1)
                {
                    string extension = Path.GetExtension(((string[])e.Data.GetData(DataFormats.FileDrop))[0]).ToLower(CultureInfo.InvariantCulture);
                    if ((extension == ".kml") || (extension == ".kmz"))
                        return true;
                }
            }
            return false;
        }
        #endregion


        /// <summary>
        /// Handles selecting and loading the selected KML/KMZ file
        /// </summary>
        private void loadMenu_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.CheckFileExists = true;
            fileDialog.Filter = "KML/KMZ files (*.kml *.kmz)|*.kml;*.kmz";
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            DialogResult result = fileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                LoadDiskKM(fileDialog.FileName);
            }
        }

        /// <summary>
        /// Cleans up used resources
        /// </summary>
        private void Cleanup()
        {
            m_KMLIcons.RemoveAll();
        }

        /// <summary>
        /// Shows information about KMLImporter on a Form
        /// </summary>
        private void aboutMenu_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        /// <summary>
        /// Toggles the Napalm enabled state
        /// </summary>
        private void napalmMenu_Click(object sender, EventArgs e)
        {
            bool bEnabled = Napalm.NapalmChangeStatus(KmlDirectory, napalmMenuItem.Text.StartsWith("Enable"));
            if (bEnabled)
                napalmMenuItem.Text = "Disable KMLImporter autoupdate";
            else
                napalmMenuItem.Text = "Enable KMLImporter autoupdate";
        }

        /// <summary>
        /// Toggles the 'drawAllLabels' state
        /// </summary>
        private void labelMenuItem_Click(object sender, EventArgs e)
        {
            labelMenuItem.Checked = Settings.ShowAllLabels = !labelMenuItem.Checked;
        }

        #endregion
    }

    /// <summary>
    /// Napalm autoupdater code
    /// </summary>
    class Napalm
    {
        private const string plugName = "KMLImporter";
        private const string baseUrl = "http://worldwind.arc.nasa.gov";
        private delegate void UpdateDelegate(string PluginDirectory, string version);

        /// <summary>
        /// Empty private constructor, because this class only contains static methods
        /// </summary>
        private Napalm()
        {

        }

        /// <summary>
        /// Starts an async update
        /// </summary>
        /// <param name="PluginDirectory"></param>
        /// <param name="version"></param>
        internal static void Update(string PluginDirectory, string version)
        {
            UpdateDelegate udel = new UpdateDelegate(WebUpdate);
            udel.BeginInvoke(PluginDirectory, version, null, null);
        }
        /// <summary>
        /// Updates this plugin (and supporting files) from the internet
        /// </summary>
        private static void WebUpdate(string PluginDirectory, string version)
        {
            CultureInfo icy = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Now go check for a new version (in the background)
            try
            {
                if (!NapalmIsEnabled(PluginDirectory))
                    return;

                if (!File.Exists(Path.Combine(PluginDirectory, plugName + ".cs")))
                    return;

                // Download the versioning file
                string URL = String.Format(icy, "{0}/{1}/{1}_ver.txt", baseUrl, plugName);
                WebClient verDownloader = new WebClient();
                Stream response = new MemoryStream(verDownloader.DownloadData(URL));

                // Create a reader to read the response from the server
                System.IO.StreamReader sr = new StreamReader(response);

                string ver = sr.ReadLine();

                // Try to update if this version appears to be out of date
                if (ver != version)
                {
                    try
                    {
                        if (Convert.ToSingle(ver, CultureInfo.InvariantCulture) < Convert.ToSingle(version, CultureInfo.InvariantCulture))
                            return;
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    // Download the new .cs file
                    string CsURL = String.Format(icy, "{0}/{1}/{1}.cs", baseUrl, plugName);
                    string CsPath = Path.Combine(PluginDirectory, String.Format(icy, "{0}.cs_", plugName));
                    WebClient csDownloader = new WebClient();
                    csDownloader.DownloadFile(CsURL, CsPath);

                    // Napalm v2.0 secure autoupdater block
                    try
                    {
                        // Create a hash of the file that was downloaded
                        System.IO.StreamReader streamreader = new StreamReader(CsPath);
                        byte[] testStringBytes = GetHashBytes(streamreader.ReadToEnd());

                        RSAParameters RSAKeyInfo = new RSAParameters();
                        System.IO.StreamReader keyreader = new System.IO.StreamReader(Path.Combine(PluginDirectory, "key"));

                        RSAKeyInfo.Modulus = Convert.FromBase64String(keyreader.ReadLine());
                        RSAKeyInfo.Exponent = Convert.FromBase64String(keyreader.ReadLine());
                        byte[] SignedHashValue = Convert.FromBase64String(sr.ReadLine());

                        RSACryptoServiceProvider RSAdecr = new RSACryptoServiceProvider();
                        RSAdecr.ImportParameters(RSAKeyInfo);

                        RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSAdecr);
                        RSADeformatter.SetHashAlgorithm("SHA1");

                        if (!RSADeformatter.VerifySignature(testStringBytes, SignedHashValue))
                        {
                            Log.Write(Log.Levels.Error, String.Format(icy, "{0}: The file signature is not valid!", plugName));
                            return;
                        }
                    }
                    catch (Exception ex)		// General signature checking / cryptography error
                    {
                        Log.Write(Log.Levels.Error, "Signature checking error:\n" + ex);
                        return;
                    }

                    // Backup the current file, and replace it with the file that was just downloaded
                    if ((File.Exists(CsPath)) && (new FileInfo(CsPath).Length > 2))
                    {
                        // Just delete a backup of the same version if it already exists
                        string bcpPath = Path.Combine(PluginDirectory, plugName + "_v" + version + ".cs_");
                        if (File.Exists(bcpPath))
                            File.Delete(bcpPath);

                        string tempPath = Path.Combine(PluginDirectory, plugName + ".cs_");
                        string plugPath = Path.Combine(PluginDirectory, plugName + ".cs");

                        // Move the old file out of the way and replace it with the new version
                        File.Move(plugPath, bcpPath);
                        for (int i = 0; (i < 5) && (File.Exists(tempPath)); i++)
                        {
                            try
                            {
                                File.Move(tempPath, plugPath);
                            }
                            catch (Exception)
                            {
                                System.Threading.Thread.Sleep(800);
                            }
                        }
                    }

                    // Notify the user, because we love users
                    string message = String.Format(icy, "The {0} plugin has been updated.\n", plugName);
                    message += "If you experience any problems it is recommended that you reload the plugin.\n";
                    MessageBox.Show(message,
                        plugName + " updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.ServiceNotification);
                }
                else if (ver != version)		// This means this is probably an internal plugin, so we can't autoupdate it
                {
                    if (MessageBox.Show("A new version of the " + plugName + " plugin is available.\nWould you like to go to the website to download the latest version?",
                        "PlaneTracker update available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.ServiceNotification) == DialogResult.Yes)
                        System.Diagnostics.Process.Start("http://www.worldwindcentral.com/wiki/Add-on:KMLImporter");
                }
            }
            catch (Exception)		// We don't really care if this failed
            { }
        }

        /// <summary>
        /// Gets a byte array representing the hash of a given string
        /// </summary>
        /// <param name="s">The string to hash</param>
        /// <returns>A byte array containing the hash</returns>
        private static byte[] GetHashBytes(string s)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);
            byte[] key = Convert.FromBase64String("szLIWrCoPJ3DSWInZx5Ye7sRz0MKBG3JpmgP2KgzlcWGvuJMqNiD77DVQuIRFvgbc5UCEFRhS5Ii5khitfOXhg==");	// A random key that needs to be kept constant
            byte[] hash = new HMACSHA1(key).ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// Checks whether Napalm is enabled
        /// </summary>
        /// <param name="PluginDirectory">The directory that contains the key file</param>
        /// <returns>true if Napalm is enabled, false if Napalm is not enabled</returns>
        internal static bool NapalmIsEnabled(string PluginDirectory)
        {
            string keyPath = Path.Combine(PluginDirectory, "key");
            if (!File.Exists(keyPath))
                return false;

            StreamReader reader = new StreamReader(keyPath);
            string keyline1 = reader.ReadLine();
            string keyline2 = reader.ReadLine();
            string keyline3 = reader.ReadLine();
            reader.Close();

            if ((keyline1 != null) && (keyline2 != null) && (keyline3 != null) && (keyline3.Length > 0))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Sets Napalm's enabled state
        /// </summary>
        /// <param name="PluginDirectory">The directory that contains the key file</param>
        /// <param name="bEnabled">Whether to enable Napalm</param>
        /// <returns>Whether Napalm was enabled</returns>
        internal static bool NapalmChangeStatus(string PluginDirectory, bool bEnabled)
        {
            string keyPath = Path.Combine(PluginDirectory, "key");
            if (!File.Exists(keyPath))
                return false;

            StreamReader reader = new StreamReader(keyPath);
            string keyline1 = reader.ReadLine();
            string keyline2 = reader.ReadLine();
            reader.Close();

            if ((keyline1 == null) || (keyline2 == null))
                return false;

            StreamWriter writer = new StreamWriter(keyPath);
            writer.WriteLine(keyline1);
            writer.WriteLine(keyline2);
            if (!bEnabled)
            {
                string[] possibleText = new string[] {
                                                         "DisableNapalm",
                                                         "You see; random characters",
                                                         "WARNING: Do not try to read the above lines out loud",
                                                         "\"Sharks with frickin' laser beams attached to their heads!\"",
                                                         "\"Oh, my, yes.\"",
                                                         "\"Windmills do not work that way! Good night!\"",
                                                         "\"I am Holly, the ship's computer, with an IQ of 6000, the same IQ as 6000 PE teachers.\"",
                                                         "\"Spoon!\"",
                                                         "\"Contrary to popular opinion, cats cannot see in the dark. They just know where you are.\""};
                Random rand = new Random();
                string disabledString = possibleText[rand.Next(possibleText.Length - 1)];
                writer.WriteLine(disabledString);
            }
            writer.Flush();
            writer.Close();

            return bEnabled;
        }
    }

    /// <summary>
    /// Stores settings and has methods to save/load these settings to/from a file
    /// </summary>
    class Settings
    {
        internal static bool ShowAllLabels;				// Whether to draw all labels for icons

        /// <summary>
        /// Empty private constructor, because this class only contains static methods
        /// </summary>
        private Settings()
        {
        }

        /// <summary>
        /// Loads settings from an XML file
        /// </summary>
        /// <param name="file">The file to load from</param>
        internal static void LoadSettings(string file)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(file);
                XmlNode node;

                // ShowAllLabels 
                node = xmldoc.SelectSingleNode("KMLImporter/ShowAllLabels");
                if (node != null)
                {
                    ShowAllLabels = System.Convert.ToBoolean(node.InnerText, CultureInfo.InvariantCulture);
                }
            }
            catch (System.IO.IOException)
            { }
            catch (System.Xml.XmlException)
            { }
        }

        /// <summary>
        /// Saves settings to an XML file
        /// </summary>
        /// <param name="file">The file to save to</param>
        internal static void SaveSettings(string file)
        {
            try
            {
                // Open writer
                System.Xml.XmlTextWriter xmlwriter = new System.Xml.XmlTextWriter(file, System.Text.Encoding.Default);
                xmlwriter.Formatting = System.Xml.Formatting.Indented;

                // Start document
                xmlwriter.WriteStartDocument();
                xmlwriter.WriteStartElement("KMLImporter");

                // ShowAllLabels
                xmlwriter.WriteStartElement("ShowAllLabels");
                xmlwriter.WriteString(ShowAllLabels.ToString(CultureInfo.InvariantCulture));
                xmlwriter.WriteEndElement();

                // End document
                xmlwriter.WriteEndElement();
                xmlwriter.WriteEndDocument();

                // Close writer
                xmlwriter.Flush();
                xmlwriter.Close();
            }
            catch (System.IO.IOException)
            { }
            catch (System.Xml.XmlException)
            { }
        }
    }

	/// <summary>
	/// Renders a message to the lower right corner
	/// </summary>
	class WaitMessage : RenderableObject
	{
		#region Private members
		private string _Text = "Please wait, loading KML file.";
		private int color = Color.White.ToArgb();
		private int distanceFromCorner = 25;
		#endregion

		/// <summary>
		/// Creates a new WaitMessage
		/// </summary>
		internal WaitMessage() : base("KML WaitMessage", Vector3.Empty, Quaternion.Identity)
		{
			// We want to be drawn on top of everything else
			this.RenderPriority = RenderPriority.Icons;

			// true to make this layer active on startup, this is equal to the checked state in layer manager
			this.IsOn = true;
		}


		#region RenderableObject methods
		/// <summary>
		/// This is where we do our rendering 
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			// Draw the current text using default font in lower right corner
			Rectangle bounds = drawArgs.defaultDrawingFont.MeasureString(null, _Text, DrawTextFormat.None, 0);
			drawArgs.defaultDrawingFont.DrawText(null, _Text, 
				drawArgs.screenWidth-bounds.Width-distanceFromCorner, drawArgs.screenHeight-bounds.Height-distanceFromCorner,
				color );
		}

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}
		#endregion
	}

	/// <summary>
	/// A Form with information about KMLImporter
	/// </summary>
	class AboutForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label3;
		private System.ComponentModel.Container components = null;

		internal AboutForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void linkLabel1_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("http://shockfire.blogspot.com/");
			}
			catch (Exception) {}
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.button1 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(120, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(160, 48);
			this.label1.TabIndex = 0;
			this.label1.Text = "KMLImporter";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(96, 144);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(224, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "Created by Tim van den Hamer (ShockFire)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// linkLabel1
			// 
			this.linkLabel1.Location = new System.Drawing.Point(96, 176);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(224, 16);
			this.linkLabel1.TabIndex = 2;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://shockfire.blogspot.com/";
			this.linkLabel1.Click += new EventHandler(linkLabel1_Click);
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(168, 216);
			this.button1.Name = "button1";
			this.button1.TabIndex = 3;
			this.button1.Text = "OK";
			this.button1.Click += new EventHandler(button1_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(96, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(224, 64);
			this.label3.TabIndex = 4;
            this.label3.Text = "KMLImporter is a NASA World Wind plugin that allows you to read kml/kmz files. It is still under development and as such doesnt support all features of kml.";
			// 
			// AboutForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 265);
			this.ControlBox = false;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.ShowInTaskbar = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.Text = "About KMLImporter";
			this.ResumeLayout(false);
		}
		#endregion
	}
}
