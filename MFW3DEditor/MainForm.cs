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

namespace MFW3DEditor
{
    public partial class MainForm : RibbonForm
    {
        public MainForm()
        {
            //配置
            using (this.splashScreen = new Splash())
            {
                this.splashScreen.Owner = this;
                //this.splashScreen.Show();
                this.splashScreen.SetText("Initializing...");

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
        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }

        #region 变量
        private System.Collections.Hashtable availableWorldList = new Hashtable();
        private PluginCompiler compiler;
        #endregion

        #region 窗体的对话框和管理器
        private Splash splashScreen;
        private RapidFireModisManager rapidFireModisManager;
        private AnimatedEarthManager animatedEarthMananger;
        private GotoDialog gotoDialog;
        private WMSBrowser wmsBrowser;
        private ProgressMonitor queueMonitor;
        #endregion

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

        #region 插件管理器
        private void InitializePluginCompiler()
        {
            Log.Write(Log.Levels.Debug, "CONF", "initializing plugin compiler...");
            string pluginRoot = Path.Combine(MFW3D.Global.Settings.DirectoryPath, "Plugins");
            compiler = new PluginCompiler(pluginRoot);
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
            worldWindow.CurrentWorld = MFW3D.ConfigurationLoader.Load(worldXmlFile, worldWindow.Cache);
            this.splashScreen.SetText("Initializing menus...");
            InitializePluginCompiler();
            foreach (RenderableObject worldRootObject in this.worldWindow.CurrentWorld.RenderableObjects.ChildObjects)
            {
                this.AddLayerMenuButtons(this.worldWindow, worldRootObject);
            }
        }
        #endregion

        #region 其他方法
        private void AddLayerMenuButtons(WorldWindow ww, RenderableObject ro)
        {
            if (ro.MetaData.Contains("ToolBarImagePath"))
            {
                string imagePath = Path.Combine(MFW3D.Global.Settings.DirectoryPath, (string)ro.MetaData["ToolBarImagePath"]);
                if (File.Exists(imagePath))
                {
                    LayerShortcutMenuButton button = new LayerShortcutMenuButton(imagePath, ro);
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
                            imagePath = Path.Combine(MFW3D.Global.Settings.DirectoryPath, imagePath);
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
                        newImageLayer.MetaData.Add("ToolBarImagePath", Path.Combine(MFW3D.Global.Settings.DirectoryPath, curImageLayerType.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(newImageLayer);
                }
            }

            if (curLayerSet.HasQuadTileSet())
            {
                for (int i = 0; i < curLayerSet.QuadTileSetCount; i++)
                {
                    LayerSet.Type_QuadTileSet2 curQtsType = curLayerSet.GetQuadTileSetAt(i);
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
                        MFW3D.Global.Settings.DirectoryPath + "//" + newPathList.PathsDirectory.Value,
                        newPathList.DistanceAboveSurface.DoubleValue(),
                        (newPathList.HasWinColorName() ? System.Drawing.Color.FromName(newPathList.WinColorName.Value) : System.Drawing.Color.FromArgb(newPathList.RGBColor.Red.Value, newPathList.RGBColor.Green.Value, newPathList.RGBColor.Blue.Value)),
                        curWorld.TerrainAccessor);

                    pl.IsOn = newPathList.ShowAtStartup.Value;

                    if (newPathList.HasExtendedInformation() && newPathList.ExtendedInformation.HasToolBarImage())
                        pl.MetaData.Add("ToolBarImagePath", Path.Combine(MFW3D.Global.Settings.DirectoryPath, newPathList.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(pl);
                }
            }

            if (curLayerSet.HasShapeFileLayer())
            {
                for (int i = 0; i < curLayerSet.ShapeFileLayerCount; i++)
                {
                    LayerSet.Type_ShapeFileLayer2 newShapefileLayer = curLayerSet.GetShapeFileLayerAt(i);
                    Microsoft.DirectX.Direct3D.FontDescription fd = Global.GetLayerFontDescription(newShapefileLayer.DisplayFont);
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
                        sp.MetaData.Add("ToolBarImagePath", Path.Combine(MFW3D.Global.Settings.DirectoryPath, newShapefileLayer.ExtendedInformation.ToolBarImage.Value));

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
                        textureFullPath = Path.Combine(MFW3D.Global.Settings.DirectoryPath, newIcon.TextureFilePath.Value);

                    MFW3D.Renderable.Icon ic = new MFW3D.Renderable.Icon(
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
                        filePath = Path.Combine(MFW3D.Global.Settings.DirectoryPath, filePath);

                    Microsoft.DirectX.Direct3D.FontDescription fd = Global.GetLayerFontDescription(newPlacenames.DisplayFont);
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
                        tps.MetaData.Add("ToolBarImagePath", Path.Combine(MFW3D.Global.Settings.DirectoryPath, newPlacenames.ExtendedInformation.ToolBarImage.Value));

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
        #endregion

    }
}