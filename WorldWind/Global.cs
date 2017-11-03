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

namespace WorldWind
{
    public static class Global
    {
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

                    Global.Settings.PluginsLoadedOnStartup.Add("Earthquake_2.0.2.1");
                    Global.Settings.PluginsLoadedOnStartup.Add("Historical_Earthquake_2.0.2.2");
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
    }
}
