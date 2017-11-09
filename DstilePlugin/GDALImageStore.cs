using WorldWind.Renderable;
using System;
using System.Globalization;
using WorldWind.DataSource;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Xml;
namespace WorldWind
{
    /// <summary>
    /// 从GDAL数据库中加载数据
    /// </summary>
    class GDALImageStore : ImageStore
    {
        #region Private Members
        string m_dataSetName;
        OSGeo.GDAL.Dataset m_dataset;
        double[] m_transform = new double[6];
        int m_lines;
        int m_pixels;
        int m_bands;
        //ImageFileFormat m_tileformat = ImageFileFormat.Png;
        string m_sourcefilename;
        #endregion

        public override bool IsDownloadableLayer
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Initialise ImageStore with a GDAL dataset
        /// </summary>
        /// <param name="dataset">Dataset in EPSG:4326</param>
        public GDALImageStore(
            string dataSetName,
            string sourcefilename,
            OSGeo.GDAL.Dataset dataset,
            string format)
        {
            //TODO: Allow serialization and deserialization of this class for persistency
            m_dataSetName = dataSetName;
            m_dataset = dataset;
            m_sourcefilename = sourcefilename;
            dataset.GetGeoTransform(m_transform);
            m_lines = dataset.RasterYSize;
            m_pixels = dataset.RasterXSize;
            m_imageFileExtension = "png";
            m_bands = dataset.RasterCount;
            if (format.Equals("jpg"))
            {
                m_imageFileExtension = "jpg";
                //m_tileformat = ImageFileFormat.Jpg;
            }
            else if (format.Equals("bmp"))
            {
                m_imageFileExtension = "bmp";
                //m_tileformat = ImageFileFormat.Bmp;
            }
            else if (format.Equals("dds"))
            {
                m_imageFileExtension = "dds";
                //m_tileformat = ImageFileFormat.Dds;
            }
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public GDALImageStore()
        {
            //TODO: Initialize things that must be initialized
        }
        
        /// <summary>
        /// Overriden ImageAccessor method to extract and put tiles in place
        /// using GDAL
        /// </summary>
        /// <param name="qt"></param>
        /// <returns></returns>
        public override string GetLocalPath(QuadTile qt)
        {
            // Check for Cached file in the expected path
            // TODO: Sane way to figure out expected path
            if (qt.Level >= m_levelCount)
                throw new ArgumentException(string.Format("Level {0} not available.",
                    qt.Level));
            string relativePath = String.Format(@"{0}\{1:D4}\{1:D4}_{2:D4}.{3}",
                qt.Level, qt.Row, qt.Col, m_imageFileExtension);

            if (m_dataDirectory != null)
            {
                // Search data directory first
                string rawFullPath = Path.Combine(m_dataDirectory, relativePath);
                string dir = Path.GetDirectoryName(rawFullPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                // If cache is not found cache file from GDAL Dataset
                if (!File.Exists(rawFullPath))
                {
                    // work out lines and pixels to extract from VRT
                    int startpixel = (int)Math.Floor((qt.West - m_transform[0]) / m_transform[1]);
                    int endpixel = (int)Math.Ceiling((qt.East - m_transform[0]) / m_transform[1]);
                    int startline = (int)Math.Floor((qt.North - m_transform[3]) / m_transform[5]);
                    int endline = (int)Math.Ceiling((qt.South - m_transform[3]) / m_transform[5]);

                    int xsize = endpixel - startpixel;
                    int ysize = endline - startline;
                    
                    // Allow partial raster access for beyond edge cases
                    int realstartpixel = Math.Max(0, startpixel);
                    int realstartline = Math.Max(0, startline);
                    int realendpixel = Math.Min(m_pixels, endpixel);
                    int realendline = Math.Min(m_lines, endline);

                    // Scale target window in case of partial access
                    int realxsize = realendpixel - realstartpixel;
                    int realysize = realendline - realstartline;

                    // Scale buffer window
                    int bufxsize = (int)Math.Round(256.0 * (double)realxsize / (double)xsize);
                    int bufysize = (int)Math.Round(256.0 * (double)realysize / (double)ysize);
                    int xoff = (int)Math.Round(256.0 * (double)(realstartpixel - startpixel) / (double)xsize);
                    int yoff = (int)Math.Round(256.0 * (double)(realstartline - startline) / (double)ysize);
                    int totalbuf = bufxsize * bufysize;

                    // use pixel space and line space instead of separate buffers
                    byte[] redbuffer = new byte[totalbuf];
                    byte[] greenbuffer = new byte[totalbuf];
                    byte[] bluebuffer = new byte[totalbuf];

                    // extract data from vrt
                    try
                    {
                        // Handle RGB
                        if (m_bands >= 3)
                        {
                            m_dataset.GetRasterBand(1).ReadRaster(realstartpixel, realstartline, realxsize, realysize, redbuffer, bufxsize, bufysize, 0, 0);
                            m_dataset.GetRasterBand(2).ReadRaster(realstartpixel, realstartline, realxsize, realysize, greenbuffer, bufxsize, bufysize, 0, 0);
                            m_dataset.GetRasterBand(3).ReadRaster(realstartpixel, realstartline, realxsize, realysize, bluebuffer, bufxsize, bufysize, 0, 0);
                        }
                        // Handle GrayScale
                        else
                        {
                            m_dataset.GetRasterBand(1).ReadRaster(realstartpixel, realstartline, realxsize, realysize, redbuffer, bufxsize, bufysize, 0, 0);
                            greenbuffer = redbuffer;
                            bluebuffer = redbuffer;
                        }
                        // TODO: Handle PCT/Indexed Colour
                    }
                    catch
                    {
                        Console.WriteLine("Raster Could not be accessed");
                    }
                    Texture texture = new Texture(DrawArgs.Device, 256, 256, 0, Usage.AutoGenerateMipMap, Format.A8R8G8B8, Pool.Managed);
                    // Write to GraphicsStream
                    Surface surf = texture.GetSurfaceLevel(0);
                    unsafe
                    {
                        GraphicsStream gs = surf.LockRectangle(LockFlags.None);
                        byte* mem = (byte*)gs.InternalDataPointer;

                        for (int x = xoff; x < xoff + bufxsize; x++)
                        {
                            for (int y = yoff; y < yoff + bufysize; y++)
                            {
                                int bufx = x - xoff; int bufy = y - yoff;
                                mem[4 * (x + 256 * y) + 0] = bluebuffer[bufy * bufxsize + bufx]; //B
                                mem[4 * (x + 256 * y) + 1] = greenbuffer[bufy * bufxsize + bufx]; //G
                                mem[4 * (x + 256 * y) + 2] = redbuffer[bufy * bufxsize + bufx]; //R
                                mem[4 * (x + 256 * y) + 3] = 1; //A
                            }
                        }
                        surf.UnlockRectangle();
                    }
                    texture.GenerateMipSubLevels();
                    // Write to file
                    TextureLoader.Save(rawFullPath, (ImageFileFormat)m_textureFormat, texture);
                }
                //Return path to cache
                return rawFullPath;
            }
            return null;
        }

        /// <summary>
        /// Save Dynamic GDALImageStores on Exit
        /// </summary>
        /// <param name="worldDoc">Create Relevant ImageAccessor Node</param>
        /// <returns></returns>
        public override System.Xml.XmlNode ToXml(System.Xml.XmlDocument worldDoc)
        {
            System.Xml.XmlNode baseNode = base.ToXml(worldDoc);
            System.Xml.XmlNode sourceFileNode = worldDoc.CreateElement("GDALFileName");
            System.Xml.XmlNode datasetnameNode = worldDoc.CreateElement("DatasetName");
            sourceFileNode.AppendChild(worldDoc.CreateTextNode(this.m_sourcefilename));
            datasetnameNode.AppendChild(worldDoc.CreateTextNode(this.m_dataSetName));
            baseNode.AppendChild(sourceFileNode);
            baseNode.AppendChild(datasetnameNode);
            return baseNode;
        }

        /// <summary>
        /// Restore object parameters from XML
        /// </summary>
        /// <param name="istoreNode"></param>
        /// <returns></returns>
        public void FromXml(System.Xml.XmlNode istoreNode)
        {
            //TODO: Populate variables using XML information
            string dataSetName = istoreNode.SelectSingleNode("DataSetName").FirstChild.ToString();
            
            m_dataSetName = dataSetName;

            /*
            m_dataset = dataset;
            m_sourcefilename = sourcefilename;
            dataset.GetGeoTransform(m_transform);
            m_lines = dataset.RasterYSize;
            m_pixels = dataset.RasterXSize;
            m_imageFileExtension = "png";
            m_bands = dataset.RasterCount;
            if (format.Equals("jpg"))
            {
                m_imageFileExtension = "jpg";
                m_tileformat = ImageFileFormat.Jpg;
            }
            else if (format.Equals("bmp"))
            {
                m_imageFileExtension = "bmp";
                m_tileformat = ImageFileFormat.Bmp;
            }
            else if (format.Equals("dds"))
            {
                m_imageFileExtension = "dds";
                m_tileformat = ImageFileFormat.Dds;
            }
            */
        }
    }
}