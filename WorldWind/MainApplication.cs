using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Net.Monitor;
using WorldWind.Configuration;
using WorldWind.PluginEngine;
using WorldWind.Camera;
using WorldWind.Menu;
using WorldWind.Net;
using WorldWind.Net.Wms;
using WorldWind.Terrain;
using WorldWind.Renderable;
using WorldWind.DataSource;
using Utility;
using ICSharpCode.SharpZipLib.Zip;

namespace WorldWind
{
    public class MainApplication : System.Windows.Forms.Form, IGlobe
    {
        #region WindowsForms variables
        private System.ComponentModel.IContainer components;
        #endregion

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            if (animatedEarthMananger != null)
            {
                animatedEarthMananger.Dispose();
                animatedEarthMananger = null;
            }
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Global.worldWindUri != null)
            {
                ProcessWorldWindUri();
            }

            base.OnLoad(e);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            if (worldWindow != null)
                worldWindow.Focus();
            base.OnGotFocus(e);
        }

        #endregion

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainApplication));
            this.worldWindow = new WorldWind.WorldWindow();
            this.SuspendLayout();
            // 
            // worldWindow
            // 
            this.worldWindow.AllowDrop = true;
            this.worldWindow.Cache = null;
            this.worldWindow.Caption = "";
            this.worldWindow.CurrentWorld = null;
            this.worldWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worldWindow.IsRenderDisabled = false;
            this.worldWindow.Location = new System.Drawing.Point(0, 0);
            this.worldWindow.Name = "worldWindow";
            this.worldWindow.ShowLayerManager = false;
            this.worldWindow.Size = new System.Drawing.Size(990, 524);
            this.worldWindow.TabIndex = 0;
            this.Controls.Add(worldWindow);
            // 
            // MainApplication
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(992, 526);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(240, 215);
            this.Name = "MainApplication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NASA World Wind";
            this.ResumeLayout(false);
        }
        #endregion

        #region 变量
        private WorldWindow worldWindow;
        private System.Collections.Hashtable availableWorldList = new Hashtable();
        private PluginCompiler compiler;
        #endregion

        #region 窗体的对话框和管理器
        private Splash splashScreen;
        private PlaceBuilder placeBuilderDialog;
        private RapidFireModisManager rapidFireModisManager;
        private AnimatedEarthManager animatedEarthMananger;
        private GotoDialog gotoDialog;
        private PathMaker pathMaker;
        private WMSBrowser wmsBrowser;
        private WMSBrowserNG wmsImporter;
        private PluginDialog pluginManager;
        private ProgressMonitor queueMonitor;
        private FileLoader fileLoaderDialog;
        #endregion

        public MainApplication()
        {
            //配置
            if (Global.Settings.ConfigurationWizardAtStartup)
            {
                if (!File.Exists(Global.Settings.FileName))
                {
                    Global.Settings.ConfigurationWizardAtStartup = false;
                }
                ConfigurationWizard.Wizard wizard = new ConfigurationWizard.Wizard(Global.Settings);
                wizard.TopMost = true;
                wizard.ShowInTaskbar = true;
                wizard.ShowDialog();
            }

            using (this.splashScreen = new Splash())
            {
                this.splashScreen.Owner = this;
                this.splashScreen.Show();
                this.splashScreen.SetText("Initializing...");

                Application.DoEvents();
                InitializeComponent();

                long CacheUpperLimit = (long)Global.Settings.CacheSizeMegaBytes * 1024L * 1024L;
                long CacheLowerLimit = (long)Global.Settings.CacheSizeMegaBytes * 768L * 1024L;    //75% of upper limit
                                                                                            //Set up the cache
                worldWindow.Cache = new Cache(
                     Global.Settings.CachePath,
                    CacheLowerLimit,
                    CacheUpperLimit,
                     Global.Settings.CacheCleanupInterval,
                     Global.Settings.TotalRunTime);

                WorldWind.Net.WebDownload.Log404Errors = World.Settings.Log404Errors;

                DirectoryInfo worldsXmlDir = new DirectoryInfo(Global.Settings.ConfigPath);
                if (!worldsXmlDir.Exists)
                    throw new ApplicationException(
                        string.Format(CultureInfo.CurrentCulture,
                        "World Wind configuration directory '{0}' could not be found.", worldsXmlDir.FullName));

                FileInfo[] worldXmlDescriptorFiles = worldsXmlDir.GetFiles("*.xml");
                int worldIndex = 0;
                foreach (FileInfo worldXmlDescriptorFile in worldXmlDescriptorFiles)
                {
                    try
                    {
                        Log.Write(Log.Levels.Debug + 1, "CONF", "checking world " + worldXmlDescriptorFile.FullName + " ...");
                        string worldXmlSchema = null;
                        string layerSetSchema = null;
                        if (Global.Settings.ValidateXML)
                        {
                            worldXmlSchema = Global.Settings.ConfigPath + "\\WorldXmlDescriptor.xsd";
                            layerSetSchema = Global.Settings.ConfigPath + "\\Earth\\LayerSet.xsd";
                        }
                        World w = WorldWind.ConfigurationLoader.Load(
                            worldXmlDescriptorFile.FullName, worldWindow.Cache, worldXmlSchema, layerSetSchema);
                        if (!availableWorldList.Contains(w.Name))
                            this.availableWorldList.Add(w.Name, worldXmlDescriptorFile.FullName);

                        w.Dispose();
                        System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem(w.Name, new System.EventHandler(OnWorldChange));
                        worldIndex++;
                    }
                    catch (Exception caught)
                    {
                        splashScreen.SetError(worldXmlDescriptorFile + ": " + caught.Message);
                        Log.Write(caught);
                    }
                }

                Log.Write(Log.Levels.Debug, "CONF", "loading startup world...");
                OpenStartupWorld();

                this.worldWindow.ShowLayerManager = World.Settings.ShowLayerManager;
                while (!this.splashScreen.IsDone)
                    System.Threading.Thread.Sleep(50);
                // Force initial render to avoid showing random contents of frame buffer to user.
                worldWindow.Render();
                WorldWindow.Focus();
            }

            // Center the main window
            Rectangle screenBounds = Screen.GetBounds(this);
            this.Location = new Point(screenBounds.Width / 2 - this.Size.Width / 2, screenBounds.Height / 2 - this.Size.Height / 2);

        }

        #region 公共参数
        public WorldWindow WorldWindow
        {
            get
            {
                return worldWindow;
            }
        }
        public float VerticalExaggeration
        {
            get
            {
                return World.Settings.VerticalExaggeration;
            }
            set
            {
                World.Settings.VerticalExaggeration = value;
                this.worldWindow.Invalidate();
            }
        }
        public Splash SplashScreen { get { return splashScreen; } }
        #endregion

        #region 公共方法
        public void webBrowserVisible(bool newStatus)
        {
            if (newStatus && World.Settings.BrowserVisible)
                return;
            else
            {
                World.Settings.BrowserVisible = !World.Settings.BrowserVisible;
                worldWindow.Render();
            }
        }
        public void LoadAddon(string fileName)
        {
            try
            {
                RenderableObjectList layers = ConfigurationLoader.getRenderableFromLayerFile(fileName, worldWindow.CurrentWorld, worldWindow.Cache);
                worldWindow.CurrentWorld.RenderableObjects.Add(layers);
                layers.IsOn = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading layer from file");
            }
        }
        #endregion

        #region 插件管理器
        private void InitializePluginCompiler()
        {
            Log.Write(Log.Levels.Debug, "CONF", "initializing plugin compiler...");
            this.splashScreen.SetText("Initializing plugins...");
            string pluginRoot = Path.Combine(Global.DirectoryPath, "Plugins");
            compiler = new PluginCompiler(this, pluginRoot);

            //#if DEBUG
            // Search for plugins in worldwind.exe (plugin development/debugging aid)
            compiler.FindPlugins(Assembly.GetExecutingAssembly());
            //#endif

            compiler.FindPlugins();
            compiler.LoadStartupPlugins();
        }
        private void OnWorldChange(object sender, System.EventArgs e)
        {
            System.Windows.Forms.MenuItem curMenuItem = (System.Windows.Forms.MenuItem)sender;

            //WorldXmlDescriptor.WorldType worldType = this.availableWorldList[curMenuItem.Text] as WorldXmlDescriptor.WorldType;
            string curWorld = availableWorldList[curMenuItem.Text] as string;

            if (curWorld != null)
            {
                OpenWorld(curWorld);
            }
        }
        private void AddInternalPluginMenuButtons()
        {
            if (this.worldWindow.CurrentWorld.IsEarth)
            {
                this.rapidFireModisManager = new RapidFireModisManager(this.worldWindow);
                this.rapidFireModisManager.Icon = this.Icon;
                this.worldWindow.MenuBar.AddToolsMenuButton(new WindowsControlMenuButton("Rapid Fire MODIS", Global.DirectoryPath + "\\Data\\Icons\\Interface\\modis.png", this.rapidFireModisManager));
            }
            this.wmsBrowser = new WMSBrowser(this.worldWindow);
            this.wmsBrowser.Icon = this.Icon;
            this.worldWindow.MenuBar.AddToolsMenuButton(new WindowsControlMenuButton("WMS Browser", Global.DirectoryPath + "\\Data\\Icons\\Interface\\wms.png", this.wmsBrowser));

            if (this.worldWindow.CurrentWorld.IsEarth)
            {
                this.animatedEarthMananger = new AnimatedEarthManager(this.worldWindow);
                this.animatedEarthMananger.Icon = this.Icon;
                this.worldWindow.MenuBar.AddToolsMenuButton(new WindowsControlMenuButton("Scientific Visualization Studio", Global.DirectoryPath + "\\Data\\Icons\\Interface\\svs2.png", this.animatedEarthMananger));
            }
        }

        #endregion

        #region IGlobe 成员
        public void SetVerticalExaggeration(double exageration)
        {
            World.Settings.VerticalExaggeration = (float)exageration;
        }
        public void SetDisplayMessages(System.Collections.IList messages)
        {
            this.worldWindow.SetDisplayMessages(messages);
        }
        public void SetLayers(System.Collections.IList layers)
        {
            this.worldWindow.SetLayers(layers);
        }
        public void SetWmsImage(WmsDescriptor imageA,
            WmsDescriptor imageB, double alpha)
        {
            this.SetWmsImage(imageA, imageB, alpha);
        }
        public void SetViewDirection(string type, double horiz, double vert, double elev)
        {
            this.worldWindow.SetViewDirection(type, horiz, vert, elev);
        }
        public void SetViewPosition(double degreesLatitude, double degreesLongitude,
            double metersElevation)
        {
            this.worldWindow.SetViewPosition(degreesLatitude, degreesLongitude,
                metersElevation);
        }
        public void SetLatLonGridShow(bool show)
        {
            World.Settings.ShowLatLonLines = show;
            if (this.worldWindow != null)
            {
                this.worldWindow.Invalidate();
            }
        }
        #endregion

        #region Uri 句柄
        public void QuickInstall(string path)
        {
            if (Global.worldWindUri == null)
            {
                Global.worldWindUri = new WorldWindUri();
            }
            Global.worldWindUri.PreserveCase = "worldwind://install=" + path;
            ProcessInstallEncodedUri();
        }
        private void ProcessWorldWindUri()
        {
            if (Global.worldWindUri.RawUrl.IndexOf("wmsimage") >= 0)
                ProcessWmsEncodedUri();

            if (Global.worldWindUri.RawUrl.IndexOf("install") >= 0)
                ProcessInstallEncodedUri();

            worldWindow.Goto(Global.worldWindUri);
            Global.worldWindUri = null;
        }
        private void ProcessInstallEncodedUri()
        {
            //parse install URI
            string urls = Global.worldWindUri.PreserveCase.Substring(20, Global.worldWindUri.PreserveCase.Length - 20);
            urls.Replace(";", ",");
            string[] urllist = urls.Split(',');
            WebDownload zipURL = new WebDownload();

            string zipFilePath = "";

            foreach (string cururl in urllist)
            {
                DialogResult result = MessageBox.Show("Do you want to install the addon from '" + cururl +
                    "'?\n\nWARNING: This will overwrite existing files.", "Installing Add-ons",
                    MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:

                        zipFilePath = cururl;   //default to the url

                        //Go ahead and download if remote and not in offline mode
                        if (cururl.StartsWith("http") && !World.Settings.WorkOffline)
                        {
                            try
                            {
                                //It's a web file - download it first
                                zipURL.Url = cururl;

                                if (cururl.EndsWith("zip"))
                                {
                                    zipFilePath = Path.Combine(Path.GetTempPath(), "WWAddon.zip");
                                }
                                else if (cururl.EndsWith("xml"))
                                {
                                    zipFilePath = Path.Combine(Path.GetTempPath(), "WWAddon.xml");
                                }

                                //MessageBox.Show("Click OK to begin downloading.  World Wind may be unresponsive while it is downloading - please wait.","Downloading...");
                                zipURL.DownloadFile(zipFilePath);
                                //MessageBox.Show("File downloaded!  Click OK to install.", "File done!");
                            }
                            catch
                            {
                                MessageBox.Show("Could not download file.\nError: " + zipURL.Exception.Message + "\nURL: " + cururl, "Error");
                            }
                        }

                        if (zipFilePath.EndsWith("xml"))
                        {
                            FileInfo source = new FileInfo(zipFilePath);
                            string targetLocation = Global.DirectoryPath + Path.DirectorySeparatorChar + "Config" + Path.DirectorySeparatorChar + WorldWindow.CurrentWorld.Name + Path.DirectorySeparatorChar + source.Name;
                            FileInfo target = new FileInfo(targetLocation);
                            if (target.Exists)
                                target.Delete();

                            source.MoveTo(target.FullName);
                            LoadAddon(source.FullName);
                            MessageBox.Show("Install completed");
                            return;
                        }
                        else if (zipFilePath.EndsWith("cs"))
                        {
                            MessageBox.Show("TODO: load plugins maybe?");
                        }

                        // handle zipped files on disk
                        try
                        {
                            FastZip fz = new FastZip();
                            fz.ExtractZip(zipFilePath, Global.DirectoryPath, "");

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error unzipping add-on:\n\n" + ex.Message);
                            return;
                        }

                        try
                        {
                            string ManifestFile = Path.Combine(Global.DirectoryPath, "manifest.txt");
                            if (File.Exists(ManifestFile))
                            {
                                StreamReader fs = new StreamReader(ManifestFile);
                                string line;
                                while ((line = fs.ReadLine()) != null)
                                {
                                    line.Trim();

                                    if (line.Length > 0)
                                    {
                                        if (line.StartsWith("#") || line.StartsWith("//") || line.StartsWith("\t"))
                                            continue;

                                        FileInfo fi = new FileInfo(Global.DirectoryPath + Path.DirectorySeparatorChar + line);
                                        if (fi.Exists && fi.Extension == ".xml")
                                        {
                                            LoadAddon(fi.FullName);
                                        }
                                        else if (fi.Exists && fi.Extension == ".cs")
                                        {
                                            MessageBox.Show("TODO: load plugins maybe?");
                                        }
                                        else
                                        {
                                            MessageBox.Show("File listed in manifest does not exist or is of an unknown type: \n\n" + line, "Error reading manifest");
                                        }
                                    }
                                }
                                fs.Close();
                                File.Delete(Path.Combine(Global.DirectoryPath, "manifest.txt"));
                            }
                            else
                            {
                                MessageBox.Show("Add-on manifest not found.  Restart World Wind to use this add-on.", "Restart required");
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + "\n\nRestart World Wind to use this add-on.", "Error reading manifest file");
                        }

                        //delete the temp file, if installing from local zip then do not delete
                        if (cururl.StartsWith("http"))
                            File.Delete(zipFilePath);

                        MessageBox.Show("Install completed succussfully.");
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        return; //They hit cancel - stop downloading stuff
                }
            }
        }
        private void ProcessWmsEncodedUri()
        {
            //first parse the rawUri string looking for "functions"
            string uri = Global.worldWindUri.RawUrl.Substring(12, Global.worldWindUri.RawUrl.Length - 12);
            uri = uri.Replace("wmsimage=", "wmsimage|");
            uri = uri.Replace("&wmsimage", "#wmsimage");
            string[] uriFunctions = uri.Split('#');

            foreach (string uriFunction in uriFunctions)
            {
                string[] paramValuePair = uriFunction.Split('|');

                if (String.Compare(paramValuePair[0], "wmsimage", true, CultureInfo.InvariantCulture) == 0)
                {
                    string displayName = null;
                    int transparencyPercent = 0;
                    double heightAboveSurface = 0.0;
                    string wmslink = "";
                    string[] wmsImageParams = new string[0];
                    if (paramValuePair[1].IndexOf("://") > 0)
                    {
                        wmsImageParams = paramValuePair[1].Replace("%26", "|").Split('|');
                    }
                    else
                    {
                        wmsImageParams = System.Web.HttpUtility.UrlDecode(paramValuePair[1]).Replace("%26", "|").Split('|');
                    }
                    foreach (string p in wmsImageParams)
                    {
                        string new_p = p.Replace("%3d", "|");
                        char[] deliminator = new char[1] { '|' };
                        string[] functionParam = new_p.Split(deliminator, 2);

                        if (String.Compare(functionParam[0], "displayname", true, CultureInfo.InvariantCulture) == 0)
                        {
                            displayName = functionParam[1];
                        }
                        else if (String.Compare(functionParam[0], "transparency", true, CultureInfo.InvariantCulture) == 0)
                        {
                            transparencyPercent = Int32.Parse(functionParam[1], CultureInfo.InvariantCulture);
                        }
                        else if (String.Compare(functionParam[0], "altitude", true, CultureInfo.InvariantCulture) == 0)
                        {
                            heightAboveSurface = Double.Parse(functionParam[1], CultureInfo.InvariantCulture);
                        }
                        else if (String.Compare(functionParam[0], "link", true, CultureInfo.InvariantCulture) == 0)
                        {
                            wmslink = functionParam[1];
                            if (wmslink.EndsWith("/"))
                                wmslink = wmslink.Substring(0, wmslink.Length - 1);
                        }
                    }

                    try
                    {
                        string[] wmslinkParams = wmslink.Split('?')[1].Split('&');

                        string wmsLayerName = null;
                        LayerSet.Type_LatitudeCoordinate2 bb_north = new LayerSet.Type_LatitudeCoordinate2();
                        LayerSet.Type_LatitudeCoordinate2 bb_south = new LayerSet.Type_LatitudeCoordinate2();
                        LayerSet.Type_LongitudeCoordinate2 bb_west = new LayerSet.Type_LongitudeCoordinate2();
                        LayerSet.Type_LongitudeCoordinate2 bb_east = new LayerSet.Type_LongitudeCoordinate2();

                        foreach (string wmslinkParam in wmslinkParams)
                        {
                            string linkParamUpper = wmslinkParam.ToUpper(CultureInfo.InvariantCulture);
                            if (linkParamUpper.IndexOf("BBOX") >= 0)
                            {
                                string[] bb_parts = wmslinkParam.Split('=')[1].Split(',');
                                bb_west.AddValue2(new LayerSet.ValueType4(bb_parts[0]));
                                bb_south.AddValue2(new LayerSet.ValueType3(bb_parts[1]));
                                bb_east.AddValue2(new LayerSet.ValueType4(bb_parts[2]));
                                bb_north.AddValue2(new LayerSet.ValueType3(bb_parts[3]));
                            }
                            else if (linkParamUpper.IndexOf("LAYERS") >= 0)
                            {
                                wmsLayerName = wmslinkParam.Split('=')[1];
                            }
                        }

                        string path = String.Format(CultureInfo.InvariantCulture,
                            @"{0}\{1}\___DownloadedWMSImages.xml", Global.Settings.ConfigPath, "");//this.currentWorld.LayerDirectory.Value);

                        string texturePath = string.Format(CultureInfo.InvariantCulture,
                            @"{0}\Data\DownloadedWMSImages\{1}", Global.DirectoryPath, System.DateTime.Now.ToFileTimeUtc());

                        if (!File.Exists(path))
                        {
                            LayerSet.LayerSetDoc newDoc = new LayerSet.LayerSetDoc();
                            LayerSet.Type_LayerSet root = new LayerSet.Type_LayerSet();

                            root.AddName(new LayerSet.NameType2("Downloaded WMS Images"));
                            root.AddShowAtStartup(new Altova.Types.SchemaBoolean(true));
                            root.AddShowOnlyOneLayer(new Altova.Types.SchemaBoolean(false));
                            newDoc.SetRootElementName("", "LayerSet");
                            newDoc.Save(path, root);
                        }

                        LayerSet.LayerSetDoc doc = new LayerSet.LayerSetDoc();
                        LayerSet.Type_LayerSet curRoot = new LayerSet.Type_LayerSet(doc.Load(path));

                        if (displayName == null)
                        {
                            displayName = wmslink.Split('?')[0] + " - " + wmsLayerName + " : " + System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToLongTimeString();
                        }

                        for (int i = 0; i < curRoot.ImageLayerCount; i++)
                        {
                            LayerSet.Type_ImageLayer curImageLayerType = (LayerSet.Type_ImageLayer)curRoot.GetImageLayerAt(i);
                            if (curImageLayerType.Name.Value.Equals(displayName))
                            {
                                displayName += String.Format(CultureInfo.CurrentCulture, " : {0} {1}", System.DateTime.Now.ToShortDateString(), System.DateTime.Now.ToLongTimeString());
                            }
                        }

                        LayerSet.Type_ImageLayer newImageLayer = new LayerSet.Type_ImageLayer();
                        newImageLayer.AddShowAtStartup(new Altova.Types.SchemaBoolean(false));

                        if (bb_north.Value2.DoubleValue() - bb_south.Value2.DoubleValue() > 90 ||
                            bb_east.Value2.DoubleValue() - bb_west.Value2.DoubleValue() > 90)
                            heightAboveSurface = 10000.0;

                        newImageLayer.AddName(new LayerSet.NameType(
                            displayName));
                        newImageLayer.AddDistanceAboveSurface(new Altova.Types.SchemaDecimal(heightAboveSurface));

                        LayerSet.Type_LatLonBoundingBox2 bb = new LayerSet.Type_LatLonBoundingBox2();

                        bb.AddNorth(bb_north);
                        bb.AddSouth(bb_south);
                        bb.AddWest(bb_west);
                        bb.AddEast(bb_east);
                        newImageLayer.AddBoundingBox(bb);
                        newImageLayer.AddTexturePath(new Altova.Types.SchemaString(
                            texturePath));

                        byte opacityValue = (byte)((100.0 - transparencyPercent) * 0.01 * 255);
                        newImageLayer.AddOpacity(new LayerSet.OpacityType(opacityValue.ToString(CultureInfo.InvariantCulture)));
                        newImageLayer.AddTerrainMapped(new Altova.Types.SchemaBoolean(false));

                        curRoot.AddImageLayer(newImageLayer);
                        doc.Save(path, curRoot);

                        ImageLayer newLayer = new ImageLayer(
                            displayName,
                            this.worldWindow.CurrentWorld,
                            (float)heightAboveSurface,
                            texturePath,
                            (float)bb_south.Value2.DoubleValue(),
                            (float)bb_north.Value2.DoubleValue(),
                            (float)bb_west.Value2.DoubleValue(),
                            (float)bb_east.Value2.DoubleValue(),
                            0.01f * (100.0f - transparencyPercent),
                            this.worldWindow.CurrentWorld.TerrainAccessor);
                        newLayer.ImageUrl = wmslink;

                        RenderableObjectList downloadedImagesRol = (RenderableObjectList)this.worldWindow.CurrentWorld.RenderableObjects.GetObject("Downloaded WMS Images");
                        if (downloadedImagesRol == null)
                            downloadedImagesRol = new RenderableObjectList("Downloaded WMS Images");

                        this.worldWindow.CurrentWorld.RenderableObjects.Add(newLayer);

                        Global.worldWindUri.Latitude = Angle.FromDegrees(0.5 * (bb_north.Value2.DoubleValue() + bb_south.Value2.DoubleValue()));
                        Global.worldWindUri.Longitude = Angle.FromDegrees(0.5 * (bb_west.Value2.DoubleValue() + bb_east.Value2.DoubleValue()));

                        if (bb_north.Value2.DoubleValue() - bb_south.Value2.DoubleValue() > bb_east.Value2.DoubleValue() - bb_west.Value2.DoubleValue())
                            Global.worldWindUri.ViewRange = Angle.FromDegrees(bb_north.Value2.DoubleValue() - bb_south.Value2.DoubleValue());
                        else
                            Global.worldWindUri.ViewRange = Angle.FromDegrees(bb_east.Value2.DoubleValue() - bb_west.Value2.DoubleValue());

                        if (Global.worldWindUri.ViewRange.Degrees > 180)
                            Global.worldWindUri.ViewRange = Angle.FromDegrees(180);

                        System.Threading.Thread.Sleep(10);
                    }
                    catch
                    { }
                }
            }
        }
        #endregion

        #region 世界句柄方法
        private void OpenStartupWorld()
        {
            string startupWorldName = null;
            if (Global.worldWindUri != null)
            {
                foreach (string curWorld in availableWorldList.Keys)
                    if (string.Compare(Global.worldWindUri.World, curWorld, true, CultureInfo.InvariantCulture) == 0)
                    {
                        startupWorldName = curWorld;
                        break;
                    }
                if (startupWorldName == null)
                {
                    //	Log.Write(startupWorldName + " - 1");
                    //	MessageBox.Show(this,
                    //		String.Format("Unable to find data for planet '{0}', loading first available planet.", worldWindUri.World));
                    //	throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, "Unable to find data for planet '{0}'.", worldWindUri.World ) );
                }
            }

            if (startupWorldName == null && availableWorldList.Contains(Global.Settings.DefaultWorld))
            {
                startupWorldName = Global.Settings.DefaultWorld;
            }

            if (startupWorldName == null)
            {
                // Pick the first planet found in config
                foreach (string curWorld in availableWorldList.Keys)
                {
                    startupWorldName = curWorld;
                    break;
                }
            }

            this.splashScreen.SetText("Initializing " + startupWorldName + "...");
            if (startupWorldName != null)
            {
                //WorldXmlDescriptor.WorldType worldDescriptor = (WorldXmlDescriptor.WorldType)this.availableWorldList[startupWorldName];
                string curWorldFile = availableWorldList[startupWorldName] as string;
                if (curWorldFile == null)
                {
                    throw new ApplicationException(
                        string.Format(CultureInfo.CurrentCulture, "Unable to load planet {0} configuration file from '{1}'.",
                        startupWorldName,
                         Global.Settings.ConfigPath));
                }

                OpenWorld(curWorldFile);
            }
        }
        private void OpenWorld(string worldXmlFile)
        {
            if (this.worldWindow.CurrentWorld != null)
            {
                try
                {
                    this.worldWindow.ResetToolbar();
                }
                catch
                { }
                try
                {
                    foreach (PluginInfo p in this.compiler.Plugins)
                    {
                        try
                        {
                            if (p.Plugin.IsLoaded)
                                p.Plugin.Unload();
                        }
                        catch
                        { }
                    }
                }
                catch
                { }

                try
                {
                    this.worldWindow.CurrentWorld.Dispose();
                }
                catch
                { }
            }
            if (this.gotoDialog != null)
            {
                this.gotoDialog.Dispose();
                this.gotoDialog = null;
            }
            if (this.rapidFireModisManager != null)
            {
                this.rapidFireModisManager.Dispose();
                this.rapidFireModisManager = null;
            }
            if (this.animatedEarthMananger != null)
            {
                this.animatedEarthMananger.Dispose();
                this.animatedEarthMananger = null;
            }
            if (this.wmsBrowser != null)
            {
                this.wmsBrowser.Dispose();
                this.wmsBrowser = null;
            }
            worldWindow.CurrentWorld = WorldWind.ConfigurationLoader.Load(worldXmlFile, worldWindow.Cache);
            this.splashScreen.SetText("Initializing menus...");
            InitializePluginCompiler();
            foreach (RenderableObject worldRootObject in this.worldWindow.CurrentWorld.RenderableObjects.ChildObjects)
            {
                this.AddLayerMenuButtons(this.worldWindow, worldRootObject);
            }
            this.AddInternalPluginMenuButtons();
        }
        #endregion

        #region 其他方法
        private void AddLayerMenuButtons(WorldWindow ww, RenderableObject ro)
        {
            if (ro.MetaData.Contains("ToolBarImagePath"))
            {
                string imagePath = Path.Combine(Global.DirectoryPath, (string)ro.MetaData["ToolBarImagePath"]);
                if (File.Exists(imagePath))
                {
                    LayerShortcutMenuButton button = new LayerShortcutMenuButton(imagePath, ro);
                    ww.MenuBar.AddLayersMenuButton(button);
                    //HACK: Temporary fix
                    if (ro.Name == "Placenames")
                        button.SetPushed(World.Settings.ShowPlacenames);
                    if (ro.Name == "Boundaries")
                        button.SetPushed(World.Settings.ShowBoundaries);
                }
            }

            if (ro.GetType() == typeof(RenderableObjectList))
            {
                RenderableObjectList rol = (RenderableObjectList)ro;
                foreach (RenderableObject child in rol.ChildObjects)
                    AddLayerMenuButtons(ww, child);
            }
        }
        private NltTerrainAccessor getTerrainAccessorFromXML(WorldXmlDescriptor.TerrainAccessor curTerrainAccessorType)
        {
            double east = curTerrainAccessorType.LatLonBoundingBox.East.Value.DoubleValue();
            double west = curTerrainAccessorType.LatLonBoundingBox.West.Value.DoubleValue();
            double north = curTerrainAccessorType.LatLonBoundingBox.North.Value.DoubleValue();
            double south = curTerrainAccessorType.LatLonBoundingBox.South.Value.DoubleValue();

            NltTerrainAccessor[] subsets = null;
            if (curTerrainAccessorType.HasHigherResolutionSubsets())
            {
                subsets = new NltTerrainAccessor[curTerrainAccessorType.HigherResolutionSubsetsCount];
                for (int i = 0; i < curTerrainAccessorType.HigherResolutionSubsetsCount; i++)
                {
                    subsets[i] = this.getTerrainAccessorFromXML(curTerrainAccessorType.GetHigherResolutionSubsetsAt(i));
                }
            }

            if (curTerrainAccessorType.HasDownloadableWMSSet())
            {
                /*	WMSLayerAccessor wmsLayer = new WMSLayerAccessor();

                    wmsLayer.ImageFormat = curTerrainAccessorType.DownloadableWMSSet.ImageFormat.Value;
                    wmsLayer.IsTransparent = curTerrainAccessorType.DownloadableWMSSet.UseTransparency.Value;
                    wmsLayer.ServerGetMapUrl = curTerrainAccessorType.DownloadableWMSSet.ServerGetMapUrl.Value;
                    wmsLayer.Version = curTerrainAccessorType.DownloadableWMSSet.Version.Value;
                    wmsLayer.WMSLayerName = curTerrainAccessorType.DownloadableWMSSet.WMSLayerName.Value;

                    if(curTerrainAccessorType.DownloadableWMSSet.HasUsername())
                        wmsLayer.Username = curTerrainAccessorType.DownloadableWMSSet.Username.Value;

                    if(curTerrainAccessorType.DownloadableWMSSet.HasPassword())
                        wmsLayer.Password = curTerrainAccessorType.DownloadableWMSSet.Password.Value;

                    if(curTerrainAccessorType.DownloadableWMSSet.HasWMSLayerStyle())
                        wmsLayer.WMSLayerStyle = curTerrainAccessorType.DownloadableWMSSet.WMSLayerStyle.Value;
                    else
                        wmsLayer.WMSLayerStyle = "";

                    if(curTerrainAccessorType.DownloadableWMSSet.HasBoundingBoxOverlap())
                        wmsLayer.BoundingBoxOverlap = curTerrainAccessorType.DownloadableWMSSet.BoundingBoxOverlap.DoubleValue();

                    return new NltTerrainAccessor(
                        curTerrainAccessorType.Name.Value,
                        west,
                        south,
                        east,
                        north,
                        wmsLayer,
                        subsets
                        );
                        */
            }
            else if (curTerrainAccessorType.HasTerrainTileService())
            {
                /*string serverUrl = curTerrainAccessorType.TerrainTileService.ServerUrl.Value;
				double levelZeroTileSizeDegrees = curTerrainAccessorType.TerrainTileService.LevelZeroTileSizeDegrees.DoubleValue();
				int numberLevels = curTerrainAccessorType.TerrainTileService.NumberLevels.Value;
				int samplesPerTile = curTerrainAccessorType.TerrainTileService.SamplesPerTile.Value;
				string fileExtension = curTerrainAccessorType.TerrainTileService.FileExtension.Value;

				TerrainTileService tts = new TerrainTileService(
					serverUrl,
					curTerrainAccessorType.TerrainTileService.DataSetName.Value,
					levelZeroTileSizeDegrees,
					samplesPerTile,
					fileExtension,
					numberLevels,
					Path.Combine(this.worldWindow.Cache.CacheDirectory,
					Path.Combine(Path.Combine( worldWindow.CurrentWorld.Name, "TerrainAccessor"), curTerrainAccessorType.Name.Value )));

				return new NltTerrainAccessor(
					curTerrainAccessorType.Name.Value,
					west,
					south,
					east,
					north,
					tts,
					subsets
					);*/
            }

            return null;
        }
        private RenderableObject getRenderableObjectListFromLayerSet(World curWorld, LayerSet.Type_LayerSet curLayerSet, string layerSetFile)//ref TreeNode treeNode)
        {
            RenderableObjectList rol = null;

            // If the layer set has icons, use the icon list layer as parent
            if (curLayerSet.HasIcon())
            {
                rol = new Icons(curLayerSet.Name.Value);
                rol.RenderPriority = RenderPriority.Icons;
            }
            else
                rol = new RenderableObjectList(curLayerSet.Name.Value);

            if (curLayerSet.HasShowOnlyOneLayer())
                rol.ShowOnlyOneLayer = curLayerSet.ShowOnlyOneLayer.Value;

            // HACK: This should be part of the settings
            if (curLayerSet.Name.ToString().ToUpper() == "PLACENAMES")
                rol.RenderPriority = RenderPriority.Placenames;

            if (curLayerSet.HasExtendedInformation())
            {
                if (curLayerSet.ExtendedInformation.HasToolBarImage())
                    rol.MetaData.Add("ToolBarImagePath", curLayerSet.ExtendedInformation.ToolBarImage.Value);
            }
            if (curLayerSet.HasImageLayer())
            {
                for (int i = 0; i < curLayerSet.ImageLayerCount; i++)
                {
                    LayerSet.Type_ImageLayer curImageLayerType = curLayerSet.GetImageLayerAt(i);

                    // <TexturePath> could contain Url, relative path, or absolute path
                    string imagePath = null;
                    string imageUrl = null;
                    if (curImageLayerType.TexturePath.Value.ToLower(System.Globalization.CultureInfo.InvariantCulture).StartsWith(("http://")))
                    {
                        imageUrl = curImageLayerType.TexturePath.Value;
                    }
                    else
                    {
                        imagePath = curImageLayerType.TexturePath.Value;
                        if (!Path.IsPathRooted(imagePath))
                            imagePath = Path.Combine(Global.DirectoryPath, imagePath);
                    }

                    int transparentColor = 0;

                    if (curImageLayerType.HasTransparentColor())
                    {
                        transparentColor = System.Drawing.Color.FromArgb(
                            curImageLayerType.TransparentColor.Red.Value,
                            curImageLayerType.TransparentColor.Green.Value,
                            curImageLayerType.TransparentColor.Blue.Value).ToArgb();

                    }

                    ImageLayer newImageLayer = new ImageLayer(
                        curImageLayerType.Name.Value,
                        curWorld,
                        (float)curImageLayerType.DistanceAboveSurface.Value,
                        imagePath,
                        (float)curImageLayerType.BoundingBox.South.Value2.DoubleValue(),
                        (float)curImageLayerType.BoundingBox.North.Value2.DoubleValue(),
                        (float)curImageLayerType.BoundingBox.West.Value2.DoubleValue(),
                        (float)curImageLayerType.BoundingBox.East.Value2.DoubleValue(),
                        (byte)curImageLayerType.Opacity.Value,
                        (curImageLayerType.TerrainMapped.Value ? curWorld.TerrainAccessor : null));

                    newImageLayer.ImageUrl = imageUrl;
                    newImageLayer.TransparentColor = transparentColor;
                    newImageLayer.IsOn = curImageLayerType.ShowAtStartup.Value;
                    if (curImageLayerType.HasLegendImagePath())
                        newImageLayer.LegendImagePath = curImageLayerType.LegendImagePath.Value;

                    if (curImageLayerType.HasExtendedInformation() && curImageLayerType.ExtendedInformation.HasToolBarImage())
                        newImageLayer.MetaData.Add("ToolBarImagePath", Path.Combine(Global.DirectoryPath, curImageLayerType.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(newImageLayer);
                }
            }

            if (curLayerSet.HasQuadTileSet())
            {
                for (int i = 0; i < curLayerSet.QuadTileSetCount; i++)
                {
                    LayerSet.Type_QuadTileSet2 curQtsType = curLayerSet.GetQuadTileSetAt(i);

                    /*ImageAccessor imageAccessor = null;

					string permDirPath = null;
					if(curQtsType.ImageAccessor.HasPermanantDirectory())
					{
						permDirPath = curQtsType.ImageAccessor.PermanantDirectory.Value;
						if(!Path.IsPathRooted(permDirPath))
							permDirPath = Path.Combine( DirectoryPath, permDirPath );
					}

					string cacheDirPath = Path.Combine(worldWindow.Cache.CacheDirectory,
						Path.Combine(curWorld.Name,
						Path.Combine(rol.Name, curQtsType.Name.Value )));

					int transparentColor = 0;
					if(curQtsType.HasTransparentColor())
					{
						transparentColor = System.Drawing.Color.FromArgb(
							curQtsType.TransparentColor.Red.Value,
							curQtsType.TransparentColor.Green.Value,
							curQtsType.TransparentColor.Blue.Value).ToArgb();

					}
					if(curQtsType.ImageAccessor.HasWMSAccessor())
					{
						WMSLayerAccessor wmsLayerAccessor = null;
						wmsLayerAccessor = new WMSLayerAccessor();
						wmsLayerAccessor.ImageFormat = curQtsType.ImageAccessor.WMSAccessor.ImageFormat.Value;
						wmsLayerAccessor.IsTransparent = curQtsType.ImageAccessor.WMSAccessor.UseTransparency.Value;
						wmsLayerAccessor.ServerGetMapUrl = curQtsType.ImageAccessor.WMSAccessor.ServerGetMapUrl.Value;
						wmsLayerAccessor.Version = curQtsType.ImageAccessor.WMSAccessor.Version.Value;
						wmsLayerAccessor.WMSLayerName = curQtsType.ImageAccessor.WMSAccessor.WMSLayerName.Value;

						if(curQtsType.ImageAccessor.WMSAccessor.HasUsername())
							wmsLayerAccessor.Username = curQtsType.ImageAccessor.WMSAccessor.Username.Value;

						if(curQtsType.ImageAccessor.WMSAccessor.HasPassword())
							wmsLayerAccessor.Password = curQtsType.ImageAccessor.WMSAccessor.Password.Value;

						if(curQtsType.ImageAccessor.WMSAccessor.HasWMSLayerStyle())
							wmsLayerAccessor.WMSLayerStyle = curQtsType.ImageAccessor.WMSAccessor.WMSLayerStyle.Value;
						else
							wmsLayerAccessor.WMSLayerStyle = "";

						if(curQtsType.ImageAccessor.WMSAccessor.HasServerLogoFilePath())
						{
							string logoPath = Path.Combine(DirectoryPath, curQtsType.ImageAccessor.WMSAccessor.ServerLogoFilePath.Value);
							if(File.Exists(logoPath))
								wmsLayerAccessor.LogoFilePath = logoPath;
						}

						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath,
							wmsLayerAccessor);
					}
					else if(curQtsType.ImageAccessor.HasImageTileService())
					{
						string logoPath = null;
						if(curQtsType.ImageAccessor.ImageTileService.HasServerLogoFilePath())
							logoPath = Path.Combine( DirectoryPath, curQtsType.ImageAccessor.ImageTileService.ServerLogoFilePath.Value);

						ImageTileService imageTileService = new ImageTileService(
							curQtsType.ImageAccessor.ImageTileService.DataSetName.Value,
							curQtsType.ImageAccessor.ImageTileService.ServerUrl.Value,
							logoPath );

						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath,
							imageTileService);
					}
					else if(curQtsType.ImageAccessor.HasDuplicateTilePath())
					{
						string dupePath = curQtsType.ImageAccessor.DuplicateTilePath.Value;
						if(!Path.IsPathRooted(dupePath))
							dupePath = Path.Combine(DirectoryPath, dupePath);
						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath,
							dupePath);
					}
					else
					{
						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath);
					}

					QuadTileSet qts = new QuadTileSet(
						curQtsType.Name.Value,
						curWorld,
						curQtsType.DistanceAboveSurface.DoubleValue(),
						curQtsType.BoundingBox.North.Value2.DoubleValue(),
						curQtsType.BoundingBox.South.Value2.DoubleValue(),
						curQtsType.BoundingBox.West.Value2.DoubleValue(),
						curQtsType.BoundingBox.East.Value2.DoubleValue(),
						(curQtsType.TerrainMapped.Value ? curWorld.TerrainAccessor : null),
						imageAccessor);

					qts.TransparentColor = transparentColor;

					if(curQtsType.ShowAtStartup.Value)
						qts.IsOn = true;
					else
						qts.IsOn = false;


					if(curQtsType.HasExtendedInformation() && curQtsType.ExtendedInformation.HasToolBarImage())
					{
						try
						{
							string fileName = Path.Combine(DirectoryPath, curQtsType.ExtendedInformation.ToolBarImage.Value);
							if (File.Exists(fileName))
								qts.MetaData.Add("ToolBarImagePath", fileName);
						}
						catch
						{
							// TODO: Log or display warning
						}
					}

					rol.Add(qts);*/
                }
            }

            if (curLayerSet.HasPathList())
            {
                for (int i = 0; i < curLayerSet.PathListCount; i++)
                {
                    LayerSet.Type_PathList2 newPathList = curLayerSet.GetPathListAt(i);

                    PathList pl = new PathList(
                        newPathList.Name.Value,
                        curWorld,
                        newPathList.MinDisplayAltitude.DoubleValue(),
                        newPathList.MaxDisplayAltitude.DoubleValue(),
                        Global.DirectoryPath + "//" + newPathList.PathsDirectory.Value,
                        newPathList.DistanceAboveSurface.DoubleValue(),
                        (newPathList.HasWinColorName() ? System.Drawing.Color.FromName(newPathList.WinColorName.Value) : System.Drawing.Color.FromArgb(newPathList.RGBColor.Red.Value, newPathList.RGBColor.Green.Value, newPathList.RGBColor.Blue.Value)),
                        curWorld.TerrainAccessor);

                    pl.IsOn = newPathList.ShowAtStartup.Value;

                    if (newPathList.HasExtendedInformation() && newPathList.ExtendedInformation.HasToolBarImage())
                        pl.MetaData.Add("ToolBarImagePath", Path.Combine(Global.DirectoryPath, newPathList.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(pl);
                }
            }

            if (curLayerSet.HasShapeFileLayer())
            {
                for (int i = 0; i < curLayerSet.ShapeFileLayerCount; i++)
                {
                    LayerSet.Type_ShapeFileLayer2 newShapefileLayer = curLayerSet.GetShapeFileLayerAt(i);
                    Microsoft.DirectX.Direct3D.FontDescription fd = GetLayerFontDescription(newShapefileLayer.DisplayFont);
                    Microsoft.DirectX.Direct3D.Font font = worldWindow.DrawArgs.CreateFont(fd);
                    ShapeLayer sp = new ShapeLayer(
                        newShapefileLayer.Name.Value,
                        curWorld,
                        newShapefileLayer.DistanceAboveSurface.DoubleValue(),
                        newShapefileLayer.MasterFilePath.Value,
                        newShapefileLayer.MinimumViewAltitude.DoubleValue(),
                        newShapefileLayer.MaximumViewAltitude.DoubleValue(),
                        font,
                        (newShapefileLayer.HasWinColorName() ? System.Drawing.Color.FromName(newShapefileLayer.WinColorName.Value) : System.Drawing.Color.FromArgb(newShapefileLayer.RGBColor.Red.Value, newShapefileLayer.RGBColor.Green.Value, newShapefileLayer.RGBColor.Blue.Value)),
                        (newShapefileLayer.HasScalarKey() ? newShapefileLayer.ScalarKey.Value : null),
                        (newShapefileLayer.HasShowBoundaries() ? newShapefileLayer.ShowBoundaries.Value : false),
                        (newShapefileLayer.HasShowFilledRegions() ? newShapefileLayer.ShowFilledRegions.Value : false));

                    sp.IsOn = newShapefileLayer.ShowAtStartup.BoolValue();

                    if (newShapefileLayer.HasExtendedInformation() && newShapefileLayer.ExtendedInformation.HasToolBarImage())
                        sp.MetaData.Add("ToolBarImagePath", Path.Combine(Global.DirectoryPath, newShapefileLayer.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(sp);
                }
            }

            if (curLayerSet.HasIcon())
            {
                Icons icons = (Icons)rol;

                for (int i = 0; i < curLayerSet.IconCount; i++)
                {
                    LayerSet.Type_Icon newIcon = curLayerSet.GetIconAt(i);

                    string textureFullPath = newIcon.TextureFilePath.Value;
                    if (textureFullPath.Length > 0 && !Path.IsPathRooted(textureFullPath))
                        // Use absolute path to icon image
                        textureFullPath = Path.Combine(Global.DirectoryPath, newIcon.TextureFilePath.Value);

                    WorldWind.Renderable.Icon ic = new WorldWind.Renderable.Icon(
                        newIcon.Name.Value,
                        (float)newIcon.Latitude.Value2.DoubleValue(),
                        (float)newIcon.Longitude.Value2.DoubleValue(),
                        (float)newIcon.DistanceAboveSurface.DoubleValue());

                    ic.TextureFileName = textureFullPath;
                    ic.Width = newIcon.IconWidthPixels.Value;
                    ic.Height = newIcon.IconHeightPixels.Value;
                    ic.IsOn = newIcon.ShowAtStartup.Value;
                    if (newIcon.HasDescription())
                        ic.Description = newIcon.Description.Value;
                    if (newIcon.HasClickableUrl())
                        ic.ClickableActionURL = newIcon.ClickableUrl.Value;
                    if (newIcon.HasMaximumDisplayAltitude())
                        ic.MaximumDisplayDistance = (float)newIcon.MaximumDisplayAltitude.Value;
                    if (newIcon.HasMinimumDisplayAltitude())
                        ic.MinimumDisplayDistance = (float)newIcon.MinimumDisplayAltitude.Value;

                    icons.Add(ic);
                }
            }

            if (curLayerSet.HasTiledPlacenameSet())
            {
                for (int i = 0; i < curLayerSet.TiledPlacenameSetCount; i++)
                {
                    LayerSet.Type_TiledPlacenameSet2 newPlacenames = curLayerSet.GetTiledPlacenameSetAt(i);

                    string filePath = newPlacenames.PlacenameListFilePath.Value;
                    if (!Path.IsPathRooted(filePath))
                        filePath = Path.Combine(Global.DirectoryPath, filePath);

                    Microsoft.DirectX.Direct3D.FontDescription fd = GetLayerFontDescription(newPlacenames.DisplayFont);
                    TiledPlacenameSet tps = new TiledPlacenameSet(
                        newPlacenames.Name.Value,
                        curWorld,
                        newPlacenames.DistanceAboveSurface.DoubleValue(),
                        newPlacenames.MaximumDisplayAltitude.DoubleValue(),
                        newPlacenames.MinimumDisplayAltitude.DoubleValue(),
                        filePath,
                        fd,
                        (newPlacenames.HasWinColorName() ? System.Drawing.Color.FromName(newPlacenames.WinColorName.Value) : System.Drawing.Color.FromArgb(newPlacenames.RGBColor.Red.Value, newPlacenames.RGBColor.Green.Value, newPlacenames.RGBColor.Blue.Value)),
                        (newPlacenames.HasIconFilePath() ? newPlacenames.IconFilePath.Value : null));

                    if (newPlacenames.HasExtendedInformation() && newPlacenames.ExtendedInformation.HasToolBarImage())
                        tps.MetaData.Add("ToolBarImagePath", Path.Combine(Global.DirectoryPath, newPlacenames.ExtendedInformation.ToolBarImage.Value));

                    tps.IsOn = newPlacenames.ShowAtStartup.Value;
                    rol.Add(tps);
                }
            }

            if (curLayerSet.HasChildLayerSet())
            {
                for (int i = 0; i < curLayerSet.ChildLayerSetCount; i++)
                {
                    LayerSet.Type_LayerSet ls = curLayerSet.GetChildLayerSetAt(i);

                    rol.Add(getRenderableObjectListFromLayerSet(curWorld, ls, layerSetFile));
                }
            }

            rol.IsOn = curLayerSet.ShowAtStartup.Value;
            return rol;
        }
        protected static Microsoft.DirectX.Direct3D.FontDescription GetLayerFontDescription(LayerSet.Type_DisplayFont2 displayFont)
        {
            Microsoft.DirectX.Direct3D.FontDescription fd = new Microsoft.DirectX.Direct3D.FontDescription();
            fd.FaceName = displayFont.Family.Value;
            fd.Height = (int)((float)displayFont.Size.Value * 1.5f);
            if (displayFont.HasStyle())
            {
                LayerSet.StyleType2 layerStyle = displayFont.Style;
                if (displayFont.Style.HasIsItalic() && layerStyle.IsItalic.Value)
                    fd.IsItalic = true;
                else
                    fd.IsItalic = false;

                if (displayFont.Style.HasIsBold() && layerStyle.IsBold.Value)
                    fd.Weight = Microsoft.DirectX.Direct3D.FontWeight.Bold;
                else
                    fd.Weight = Microsoft.DirectX.Direct3D.FontWeight.Regular;
            }
            else
            {
                fd.Weight = Microsoft.DirectX.Direct3D.FontWeight.Regular;
            }
            return fd;
        }
        protected bool HandleKeyUp(KeyEventArgs e)
        {
            // keep keypresses inside browser url bar
            if (e.Handled)
                return true;

            if (e.Alt)
            {
                // Alt key down
                switch (e.KeyCode)
                {
                    case Keys.Q:
                        using (PropertyBrowserForm worldWindSettings = new PropertyBrowserForm(Global.Settings, "World Wind Settings"))
                        {
                            worldWindSettings.Icon = this.Icon;
                            worldWindSettings.ShowDialog();
                        }
                        return true;
                    case Keys.Enter:
                        return true;
                    case Keys.F4:
                        Close();
                        return true;
                }
            }
            else if (e.Control)
            {
                // Control key down
                switch (e.KeyCode)
                {
                    case Keys.F:
                        return true;
                    case Keys.H:
                        if (queueMonitor != null)
                        {
                            bool wasVisible = queueMonitor.Visible;
                            queueMonitor.Close();
                            queueMonitor.Dispose();
                            queueMonitor = null;
                            if (wasVisible)
                                return true;
                        }

                        queueMonitor = new ProgressMonitor();
                        queueMonitor.Icon = this.Icon;
                        queueMonitor.Show();
                        return true;
                }
            }
            else
            {
                // Other or no modifier key
                switch (e.KeyCode)
                {
                    case Keys.P:
                        if (this.pathMaker == null)
                        {
                            this.pathMaker = new PathMaker(this.worldWindow);
                            this.pathMaker.Icon = this.Icon;
                        }
                        this.pathMaker.Visible = !this.pathMaker.Visible;
                        return true;
                    case Keys.V:
                        if (this.placeBuilderDialog == null)
                        {
                            this.placeBuilderDialog = new PlaceBuilder(this.worldWindow);
                            this.placeBuilderDialog.Icon = this.Icon;
                        }

                        this.placeBuilderDialog.Visible = !this.placeBuilderDialog.Visible;
                        return true;
                    case Keys.Escape:
                        return true;
                    case Keys.D1:
                    case Keys.NumPad1:
                        this.VerticalExaggeration = 1.0f;
                        return true;
                    case Keys.D2:
                    case Keys.NumPad2:
                        this.VerticalExaggeration = 2.0f;
                        return true;
                    case Keys.D3:
                    case Keys.NumPad3:
                        this.VerticalExaggeration = 3.0f;
                        return true;
                    case Keys.D4:
                    case Keys.NumPad4:
                        this.VerticalExaggeration = 4.0f;
                        return true;
                    case Keys.D5:
                    case Keys.NumPad5:
                        this.VerticalExaggeration = 5.0f;
                        return true;
                    case Keys.D6:
                    case Keys.NumPad6:
                        this.VerticalExaggeration = 6.0f;
                        return true;
                    case Keys.D7:
                    case Keys.NumPad7:
                        this.VerticalExaggeration = 7.0f;
                        return true;
                    case Keys.D8:
                    case Keys.NumPad8:
                        this.VerticalExaggeration = 8.0f;
                        return true;
                    case Keys.D9:
                    case Keys.NumPad9:
                        this.VerticalExaggeration = 9.0f;
                        return true;
                    case Keys.D0:
                    case Keys.NumPad0:
                        this.VerticalExaggeration = 0.0f;
                        return true;
                }
            }
            return false;
        }
        private void resetQuadTileSetCache(RenderableObject ro)
        {
            if (ro.IsOn && ro is QuadTileSet)
            {
                QuadTileSet qts = (QuadTileSet)ro;
                qts.ResetCacheForCurrentView(worldWindow.DrawArgs.WorldCamera);
            }
            else if (ro is RenderableObjectList)
            {
                RenderableObjectList rol = (RenderableObjectList)ro;
                foreach (RenderableObject curRo in rol.ChildObjects)
                {
                    resetQuadTileSetCache(curRo);
                }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // To close queueMonitor to avoid threads lock problems
            if (queueMonitor != null) this.queueMonitor.Close();
            if (compiler != null)
                compiler.Dispose();
        }
        #endregion
    }
}
