using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using MFW3D.Renderable;
using Utility;
using System.Windows.Forms;
using System.Globalization;

namespace MFW3D.KMLReader
{
    public class KMLLoader
    {
        static string KmlDirectory = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "kml");
        private string m_realkmlpath;

        public string RealKMLPath
        {
            get
            {
                return m_realkmlpath;
            }
        }


        public string LoadKML(string filename)
        {
            if (Path.GetExtension(filename) == ".kmz")
            {
                bool shouldReturn;
                string ExtractedKMLPath = ExtractKMZ(filename, out shouldReturn);
                if (shouldReturn)
                    return null;
                m_realkmlpath = ExtractedKMLPath;
                return LoadKMLFile(ExtractedKMLPath);
            }
            else
            {
                m_realkmlpath = filename;
                return LoadKMLFile(filename);
            }
        }

        internal string ExtractKMZ(string filename, out bool bError)
        {
            bError = false;

            FileInfo fileInfo = new FileInfo(filename);
            // Create a folder 'kmz' to extract the kmz file to
            string ExtractPath = Path.Combine(KmlDirectory, "kmz\\" + fileInfo.Name);
            if (!Directory.Exists(ExtractPath))
                Directory.CreateDirectory(ExtractPath);

            // Extract the kmz file
            FastZip fz = new FastZip();
            fz.ExtractZip(filename, ExtractPath, "");

            // Try to find the extracted kml file to load
            string ExtractedKMLPath = null;
            if (File.Exists(Path.Combine(ExtractPath, "doc.kml")))
                ExtractedKMLPath = Path.Combine(ExtractPath, "doc.kml");
            else
            {
                ExtractedKMLPath = GetKMLFromDirectory(ExtractPath);
                if (ExtractedKMLPath == null)
                    bError = true;
            }

            return ExtractedKMLPath;
        }

        private string GetKMLFromDirectory(string ExtractPath)
        {
            string[] folders = Directory.GetDirectories(ExtractPath);
            foreach (string folder in folders)
            {
                string tempPath = GetKMLFromDirectory(folder);
                if (tempPath != null)
                    return tempPath;
            }

            string[] kmlfiles = Directory.GetFiles(ExtractPath, "*.kml");
            if (kmlfiles.Length > 0)
                return kmlfiles[0];
            else
                return null;
        }

        private string LoadKMLFile(string KMLPath)
        {
            if (KMLPath == null)
                return null;

            string kml = null;

            // Create a reader to read the file
            try
            {
                System.IO.StreamReader sr = new StreamReader(KMLPath);

                // Read all data from the reader
                kml = sr.ReadToEnd();

                // Close the reader
                sr.Close();
            }
            catch (Exception ex) // Catch error if stream reader failed
            {
                Log.Write(Log.Levels.Error, "KMLImporter: " + ex.ToString());

                MessageBox.Show(
                    String.Format(CultureInfo.InvariantCulture, "Error opening KML file '{0}':\n\n{1}", KMLPath, ex.ToString()),
                    "KMLImporter error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);

                // TODO add back in RTL handling
                // base.Application.RightToLeft == RightToLeft.Yes ? MessageBoxOptions.RtlReading : MessageBoxOptions.ServiceNotification);

                kml = null;
            }

            return kml;
        }
    }
}
