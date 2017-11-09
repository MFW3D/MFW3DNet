using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Utility;
using WorldWind.Configuration;
using WorldWind.Net;
using WorldWind.Renderable;

namespace WorldWind
{
    public static class Global
    {
        public static WorldWindow worldWindow;
        public static string CurrentSettingsDirectory;
        public static bool issetCurrentSettingsDirectory;
        public static WorldWindSettings Settings = new WorldWindSettings();
        public static readonly string DirectoryPath = Path.GetDirectoryName(Application.ExecutablePath);

        public static WorldWindUri worldWindUri;
        public static string[] cmdArgs;
        public static void BrowseTo(string url)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = url;
            psi.Verb = "open";
            psi.UseShellExecute = true;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }

        #region 加载参数
        public static void LoadSettings()
        {
            try
            {
                Global.Settings = (WorldWindSettings)SettingsBase.Load(Global.Settings, SettingsBase.LocationType.User);

                if (!File.Exists(Global.Settings.FileName))
                {
                    Global.Settings.PluginsLoadedOnStartup.Add("ShapeFileInfoTool");
                    Global.Settings.PluginsLoadedOnStartup.Add("OverviewFormLoader");
                    Global.Settings.PluginsLoadedOnStartup.Add("Atmosphere");
                    Global.Settings.PluginsLoadedOnStartup.Add("SkyGradient");
                    Global.Settings.PluginsLoadedOnStartup.Add("BmngLoader");
                    Global.Settings.PluginsLoadedOnStartup.Add("Compass");
                    Global.Settings.PluginsLoadedOnStartup.Add("ExternalLayerManagerLoader");
                    Global.Settings.PluginsLoadedOnStartup.Add("MeasureTool");
                    Global.Settings.PluginsLoadedOnStartup.Add("MovieRecorder");
                    Global.Settings.PluginsLoadedOnStartup.Add("NRLWeatherLoader");
                    Global.Settings.PluginsLoadedOnStartup.Add("ShapeFileLoader");
                    Global.Settings.PluginsLoadedOnStartup.Add("Stars3D");
                    Global.Settings.PluginsLoadedOnStartup.Add("GlobalClouds");
                    Global.Settings.PluginsLoadedOnStartup.Add("PlaceFinderLoader");
                    Global.Settings.PluginsLoadedOnStartup.Add("LightController");

                    Global.Settings.PluginsLoadedOnStartup.Add("KMLImporter");
                    Global.Settings.PluginsLoadedOnStartup.Add("doublezoom");
                    Global.Settings.PluginsLoadedOnStartup.Add("PlanetaryRings");
                    Global.Settings.PluginsLoadedOnStartup.Add("TimeController");
                    Global.Settings.PluginsLoadedOnStartup.Add("WavingFlags");
                    Global.Settings.PluginsLoadedOnStartup.Add("ScaleBarLegend");
                    Global.Settings.PluginsLoadedOnStartup.Add("Compass3D");
                    Global.Settings.PluginsLoadedOnStartup.Add("AnaglyphStereo");
                }
                DataProtector dp = new DataProtector(DataProtector.Store.USE_USER_STORE);
                if (Global.Settings.ProxyUsername.Length > 0) Global.Settings.ProxyUsername = dp.TransparentDecrypt(Global.Settings.ProxyUsername);
                if (Global.Settings.ProxyPassword.Length > 0) Global.Settings.ProxyPassword = dp.TransparentDecrypt(Global.Settings.ProxyPassword);
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }
        public static void LoadSettings(string directory)
        {
            try
            {
                Global.Settings = (WorldWindSettings)SettingsBase.LoadFromPath(Global.Settings, directory);
                DataProtector dp = new DataProtector(DataProtector.Store.USE_USER_STORE);
                if (Global.Settings.ProxyUsername.Length > 0) Global.Settings.ProxyUsername = dp.TransparentDecrypt(Global.Settings.ProxyUsername);
                if (Global.Settings.ProxyPassword.Length > 0) Global.Settings.ProxyPassword = dp.TransparentDecrypt(Global.Settings.ProxyPassword);
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }
        public static void ParseArgs(string[] args)
        {
            try
            {
                NLT.Plugins.ShapeFileLoaderGUI.m_ShapeLoad.ParseUri(args);
            }
            catch
            {
            }
            cmdArgs = args;

            foreach (string rawArg in args)
            {
                string arg = rawArg.Trim();
                if (arg.Length <= 0)
                    continue;
                try
                {
                    //check for url call
                    // TODO: do not hardcode the URI scheme here
                    if (arg.StartsWith("worldwind://"))
                    {
                        worldWindUri = WorldWindUri.Parse(arg);
                    }
                    else if (arg.StartsWith("/"))
                    {
                        if (arg.Length <= 1)
                        {
                            throw new ArgumentException("Empty command line option.");
                        }

                        string key = arg.Substring(1, 1).ToLower(CultureInfo.CurrentCulture);
                        switch (key)
                        {
                            case "s":
                                if (Global.issetCurrentSettingsDirectory)
                                {
                                    continue;
                                }
                                if (arg.Length < 6)
                                {
                                    throw new ArgumentException("Invalid value(too short) for command line option /S: " + arg);
                                }
                                if (arg.Substring(2, 1) != "=")
                                {
                                    throw new ArgumentException("Invalid value(no = after S) for command line option /S: " + arg);
                                }
                                // TODO: test value via regex?
                                Global.CurrentSettingsDirectory = arg.Substring(3);
                                Global.issetCurrentSettingsDirectory = true;
                                break;
                            default:
                                throw new ArgumentException("Unknown command line option: " + arg);
                        }
                    }
                    else
                        throw new ArgumentException("Unknown command line option: " + arg);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }
        }
        #endregion

        public static Microsoft.DirectX.Direct3D.FontDescription GetLayerFontDescription(LayerSet.Type_DisplayFont2 displayFont)
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

        #region Uri 句柄
        public static void QuickInstall(string path)
        {
            if (Global.worldWindUri == null)
            {
                Global.worldWindUri = new WorldWindUri();
            }
            Global.worldWindUri.PreserveCase = "worldwind://install=" + path;
            ProcessInstallEncodedUri();
        }
        public static void ProcessWorldWindUri()
        {
            if (Global.worldWindUri.RawUrl.IndexOf("wmsimage") >= 0)
                ProcessWmsEncodedUri();

            if (Global.worldWindUri.RawUrl.IndexOf("install") >= 0)
                ProcessInstallEncodedUri();

            worldWindow.Goto(Global.worldWindUri);
            Global.worldWindUri = null;
        }
        public static void ProcessInstallEncodedUri()
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
                            string targetLocation = Global.DirectoryPath + Path.DirectorySeparatorChar + "Config" + Path.DirectorySeparatorChar + worldWindow.CurrentWorld.Name + Path.DirectorySeparatorChar + source.Name;
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
        public static void ProcessWmsEncodedUri()
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
                            worldWindow.CurrentWorld,
                            (float)heightAboveSurface,
                            texturePath,
                            (float)bb_south.Value2.DoubleValue(),
                            (float)bb_north.Value2.DoubleValue(),
                            (float)bb_west.Value2.DoubleValue(),
                            (float)bb_east.Value2.DoubleValue(),
                            0.01f * (100.0f - transparencyPercent),
                            worldWindow.CurrentWorld.TerrainAccessor);
                        newLayer.ImageUrl = wmslink;

                        RenderableObjectList downloadedImagesRol = (RenderableObjectList)worldWindow.CurrentWorld.RenderableObjects.GetObject("Downloaded WMS Images");
                        if (downloadedImagesRol == null)
                            downloadedImagesRol = new RenderableObjectList("Downloaded WMS Images");

                        worldWindow.CurrentWorld.RenderableObjects.Add(newLayer);

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

        #region 公共方法
        public static void webBrowserVisible(bool newStatus)
        {
            if (newStatus && World.Settings.BrowserVisible)
                return;
            else
            {
                World.Settings.BrowserVisible = !World.Settings.BrowserVisible;
                worldWindow.Render();
            }
        }
        public static void LoadAddon(string fileName)
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
    }
}
