using System;
using System.Collections.Generic;
using System.Text;
using WorldWind.Renderable;
using Microsoft.DirectX;
using System.Globalization;
using System.IO;
using WorldWind.Net;
using Utility;
using System.Windows.Forms;

namespace WorldWind.KMLReader
{
    /// <summary>
    /// Represents a NetworkLink. Updates a layer from a remote source periodically.
    /// </summary>
    class KMLNetworkLink
    {
        private string url;
        private Icons layer;
        private System.Timers.Timer tickTimer = new System.Timers.Timer();
        private System.Timers.Timer viewTimer = new System.Timers.Timer();
        private KMLParser owner;
        private bool bUpdating = false;
        bool m_firedStartup = false;

        private Matrix lastView = Matrix.Identity;
        private bool bViewStopped = false;

        /// <summary>
        /// Creates and initializes a new NetworkLink
        /// </summary>
        /// <param name="owner">The owner of this NetworkLink</param>
        /// <param name="layer">The layer to update the NetworkLink to</param>
        /// <param name="url">The URL to update the NetworkLink from</param>
        /// <param name="tickTime">The interval, in milliseconds, at which the NetworkLink should update</param>
        /// <param name="viewTime">The time, in milliseconds, after the view stops moving which the NetworkLink should update</param>
        internal KMLNetworkLink(KMLParser owner, Icons layer, string url, int tickTime, int viewTime)
        {
            this.owner = owner;
            this.url = url;
            this.layer = layer;

            if (tickTime > 0)
            {
                tickTimer.Interval = (double)tickTime;
                tickTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                tickTimer.Start();
            }

            if (viewTime > 0)
            {
                viewTimer.Interval = (double)viewTime;
                viewTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                viewTimer.Start();
            }
        }

        /// <summary>
        /// Gets the visible bounding box for the application in lat/lon degrees.
        /// </summary>
        /// <returns>An array of Angles in minx.miny,maxx, maxy order</returns>
        private static string GetBBox()
        {
            CultureInfo ic = CultureInfo.InvariantCulture;

            // TODO: Correct the ViewRange for non-square windows.
            // Is is accurate horizontally but not vertically.
            Angle lat = DrawArgs.Camera.Latitude;
            Angle lon = DrawArgs.Camera.Longitude;
            Angle vr = DrawArgs.Camera.ViewRange;

            Angle North = lat + (0.5 * vr);
            Angle South = lat - (0.5 * vr);
            Angle East = lon + (0.5 * vr);
            Angle West = lon - (0.5 * vr);

            //minX(West), minY(South), maxX(East), MaxY(North)
            return "BBOX=" + West.Degrees.ToString(ic) + " " + South.Degrees.ToString(ic) + " " + East.Degrees.ToString(ic) + " " + North.Degrees.ToString(ic);
        }

        /// <summary>
        /// Fires off a download
        /// </summary>
        internal void Fire()
        {
            if (viewTimer.Enabled)
                timer_Elapsed(viewTimer, null);
            else
                timer_Elapsed(null, null);
        }

        /// <summary>
        /// Downloads a KML/KMZ file from the given URL
        /// </summary>
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (bUpdating)
                return;
            bUpdating = true;

            try
            {
                if (!m_firedStartup || (layer != null && layer.IsOn))
                {
                    string fullurl = url;
                    if (sender == viewTimer)
                    {
                        if (!bViewStopped)
                        {
                            if (DrawArgs.Camera.ViewMatrix != lastView)
                            {
                                lastView = DrawArgs.Camera.ViewMatrix;
                                bUpdating = false;
                                return;
                            }
                            bViewStopped = true;
                        }
                        else
                        {
                            if (DrawArgs.Camera.ViewMatrix != lastView)
                            {
                                lastView = DrawArgs.Camera.ViewMatrix;
                                bViewStopped = false;
                            }
                            bUpdating = false;
                            return;
                        }
                        fullurl += (fullurl.IndexOf('?') == -1 ? "?" : "&") + GetBBox();
                    }

                    string saveFile = Path.GetFileName(Uri.EscapeDataString(url));
                    if (saveFile == null || saveFile.Length == 0)
                        saveFile = "temp.kml";

                    saveFile = Path.Combine(KMLParser.KmlDirectory + "\\temp\\", saveFile);

                    FileInfo saveFileInfo = new FileInfo(saveFile);
                    if (!saveFileInfo.Directory.Exists)
                        saveFileInfo.Directory.Create();

                    // Offline check
                    if (World.Settings.WorkOffline)
                        throw new Exception("Offline mode active.");

                    WebDownload myClient = new WebDownload(fullurl);
                    myClient.DownloadFile(saveFile);

                    // Extract the file if it is a kmz file
                    string kmlFile = saveFile;

                    // Create a reader to read the file
                    KMLLoader loader = new KMLLoader();
                    if (Path.GetExtension(saveFile) == ".kmz")
                    {
                        bool bError = false;
                        kmlFile = loader.ExtractKMZ(saveFile, out bError);

                        if (bError)
                        {
                            return;
                        }
                    }

                    // Read all data from the reader
                    string kml = loader.LoadKML(kmlFile);

                    if (kml != null)
                    {
                        try
                        {
                            // Load the actual kml data
                            owner.ReadKML(kml, layer, kmlFile);
                        }
                        catch (Exception ex)
                        {
                            Log.Write(Log.Levels.Error, "KMLParser: " + ex.ToString());

                            MessageBox.Show(
                                String.Format(CultureInfo.InvariantCulture, "Error loading KML file '{0}':\n\n{1}", kmlFile, ex.ToString()),
                                "KMLParser error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.ServiceNotification);
                        }
                    }
                    m_firedStartup = true;
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.Levels.Error, "KMLParser: " + ex.ToString());
            }

            bUpdating = false;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        internal void Dispose()
        {
            tickTimer.Stop();
            viewTimer.Stop();
        }
    }

}
