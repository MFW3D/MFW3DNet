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
using System.Collections.Generic;

namespace WorldWind
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                //IntPtr handle = MainApplication.GetWWHandle();
                //if (!System.IntPtr.Zero.Equals(handle))
                //{
                //    if (args.Length > 0)
                //        NativeMethods.SendArgs(handle, string.Join("\n", args));
                //    return;
                //}

                if (BindingsCheck.FiftyBindingsWarning()) return;
                System.Threading.Thread.CurrentThread.Name = "Main Thread";
                Global.ParseArgs(args);
                //设置目录
                if (!Directory.Exists(Application.StartupPath + "/Setting"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "/Setting");
                }
                Global.CurrentSettingsDirectory = Application.StartupPath + "/Setting";
                if (Global.CurrentSettingsDirectory == null)
                {
                    Global.LoadSettings();
                    World.LoadSettings();
                }
                else
                {
                    Global.LoadSettings(Global.CurrentSettingsDirectory);
                    World.LoadSettings(Global.CurrentSettingsDirectory);
                }
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                MainApplication app = new MainApplication();
                Application.Idle += new EventHandler(app.WorldWindow.OnApplicationIdle);
                Application.Run(app);
                World.Settings.Save();
                DataProtector dp = new DataProtector(DataProtector.Store.USE_USER_STORE);
                Global.Settings.ProxyUsername = dp.TransparentEncrypt(Global.Settings.ProxyUsername);
                Global.Settings.ProxyPassword = dp.TransparentEncrypt(Global.Settings.ProxyPassword);
                Global.Settings.Save();
            }
            catch (NullReferenceException)
            {
            }
            catch (Exception caught)
            {
                Exception e;
                string errorMessages;
                try
                {
                    Log.Write(caught);
                }
                catch
                {
                }
                finally
                {
                    e = caught;
                    errorMessages = "The following error(s) occurred:";
                    do
                    {
                        errorMessages += "\r\n" + e.Message;
                        e = e.InnerException;
                    }
                    while (e != null);
                    Abort(errorMessages);
                }
            }
        }
        public static void Abort(string errorMessages)
        {
            ErrorDisplay errorDialog = new ErrorDisplay();
            errorDialog.errorMessages(errorMessages);
            errorDialog.ShowDialog();
            Environment.Exit(0);
        }
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log.Write(e.Exception);

            //HACK
            if (e.Exception is NullReferenceException)
                return;

            //TODO: Nice dialog with button to show debug info (stack trace)
            MessageBox.Show(e.Exception.Message, "An error has occurred", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }
}