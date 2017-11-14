using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Utility;
using MFW3D.Configuration;
using MFW3D.Net;
using MFW3D.PluginEngine;
using MFW3D.Renderable;

namespace MFW3D
{
    public static class Global
    {
        public static WorldWindow worldWindow;
        public static string CurrentSettingsDirectory;
        public static bool issetCurrentSettingsDirectory;
        public static WorldWindSettings Settings = new WorldWindSettings();

        public static WorldWindUri worldWindUri;
        public static string[] cmdArgs;
        #region 加载参数
        public static void LoadSettings()
        {
            try
            {
                Global.Settings = (WorldWindSettings)SettingsBase.Load(Global.Settings, SettingsBase.LocationType.User);

                if (!File.Exists(Global.Settings.FileName))
                {
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("ShapeFileInfoTool");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("OverviewFormLoader");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("Atmosphere");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("SkyGradient");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("BmngLoader");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("Compass");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("ExternalLayerManagerLoader");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("MeasureTool");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("MovieRecorder");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("NRLWeatherLoader");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("ShapeFileLoader");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("Stars3D");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("GlobalClouds");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("PlaceFinderLoader");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("LightController");

                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("KMLImporter");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("doublezoom");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("PlanetaryRings");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("TimeController");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("WavingFlags");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("ScaleBarLegend");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("Compass3D");
                    MFW3D.Global.Settings.PluginsLoadedOnStartup.Add("AnaglyphStereo");
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
            cmdArgs = args;

            foreach (string rawArg in args)
            {
                string arg = rawArg.Trim();
                if (arg.Length <= 0)
                    continue;
                try
                {
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
