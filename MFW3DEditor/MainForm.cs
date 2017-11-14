using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using MFW3D.Net.Monitor;
using MFW3D.PluginEngine;
using MFW3D.Menu;
using MFW3D.Net.Wms;
using MFW3D.Terrain;
using MFW3D.Renderable;
using Utility;
using MFW3D;
using MFW3D.NewWidgets;

namespace MFW3DEditor
{
    public partial class MainForm : RibbonForm
    {
        public MainForm()
        {
            //配置
            Application.DoEvents();
            InitializeComponent();
            InitSkinGallery();
            //设置global
            Global.worldWindow = worldWindow;
            long CacheUpperLimit = (long)Global.Settings.CacheSizeMegaBytes * 1024L * 1024L;
            long CacheLowerLimit = (long)Global.Settings.CacheSizeMegaBytes * 768L * 1024L;    //75% of upper limit
                                                                                               //Set up the cache
            worldWindow.Cache = new Cache(
                 Global.Settings.CachePath,
                CacheLowerLimit,
                CacheUpperLimit,
                 Global.Settings.CacheCleanupInterval,
                 Global.Settings.TotalRunTime);

            MFW3D.Net.WebDownload.Log404Errors = World.Settings.Log404Errors;

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
                    World w = MFW3D.ConfigurationLoader.Load(
                        worldXmlDescriptorFile.FullName, worldWindow.Cache, worldXmlSchema, layerSetSchema);
                    if (!availableWorldList.Contains(w.Name))
                        this.availableWorldList.Add(w.Name, worldXmlDescriptorFile.FullName);

                    w.Dispose();
                    worldIndex++;
                }
                catch (Exception caught)
                {
                    Log.Write(caught);
                }
            }

            Log.Write(Log.Levels.Debug, "CONF", "loading startup world...");
            OpenStartupWorld();
            worldWindow.Render();
            WorldWindow.Focus();
            Rectangle screenBounds = Screen.GetBounds(this);
        }
        #region 引擎参数
        private System.Collections.Hashtable availableWorldList = new Hashtable();
        private PluginCompiler compiler;
        private RapidFireModisManager rapidFireModisManager;
        private AnimatedEarthManager animatedEarthMananger;
        private WMSBrowser wmsBrowser;
        public WorldWindow WorldWindow
        {
            get
            {
                return worldWindow;
            }
        }
        private void InitializePluginCompiler()
        {
            Log.Write(Log.Levels.Debug, "CONF", "initializing plugin compiler...");
            string pluginRoot = Path.Combine(MFW3D.Global.Settings.DirectoryPath, "Plugins");
            compiler = new PluginCompiler(pluginRoot);
            //加载默认插件
            if (File.Exists(Application.StartupPath + "/Plugins.dll"))
            {
                Assembly assembly = Assembly.LoadFrom(Application.StartupPath + "/Plugins.dll");
                compiler.FindPlugins(assembly);
                compiler.FindPlugins();
                compiler.LoadStartupPlugins();
            }

            //加载所有插件的内容
            DirectoryInfo TheFolder = new DirectoryInfo(pluginRoot);
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Name.Length < 3)
                    continue;
                if (NextFile.Name.Substring(NextFile.Name.Length - 4, 4) != ".dll")
                    continue;
                Assembly assembly = Assembly.LoadFrom(
                    AppDomain.CurrentDomain.BaseDirectory + NextFile.Name);
                compiler.FindPlugins(assembly);
                compiler.FindPlugins();
                compiler.LoadStartupPlugins();
            }
        }
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
                try
                {
                    this.worldWindow.CurrentWorld.Dispose();
                }
                catch
                { }
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
            worldWindow.CurrentWorld = MFW3D.ConfigurationLoader.Load(worldXmlFile, worldWindow.Cache);
            InitializePluginCompiler();
            foreach (RenderableObject worldRootObject in this.worldWindow.CurrentWorld.RenderableObjects.ChildObjects)
            {
                //添加图层
            }
        }
        #endregion

        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }
        bool m_IsLL = false;

        private void m_showLL_ItemClick(object sender, ItemClickEventArgs e)
        {
            worldWindow.SetLatLonGridShow(m_IsLL = !m_IsLL);
        }

        private void m_zhibeizhen_ItemClick(object sender, ItemClickEventArgs e)
        {


        }

        private void m_showlayers_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void m_scollerbar_ItemClick(object sender, ItemClickEventArgs e)
        {
        }
    }
}