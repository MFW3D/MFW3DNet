using WorldWind.Renderable;
using System;
using System.Globalization;
using WorldWind.DataSource;
using System.IO;
using WorldWind.Terrain;
using OSGeo.GDAL;

namespace WorldWind.Terrain
{
    /// <summary>
    /// Reads GDAL based terrain data and writes out
    /// .bil cache files
    /// </summary>
    public class GDALTerrainAccessor : TerrainAccessor
    {
        #region private members
        private GDALTerrainTileService m_terrainTileService;
        #endregion

        public GDALTerrainAccessor(
            double north,
            double south,
            double east,
            double west,
            OSGeo.GDAL.Dataset dataset,
            string datasetName,
            double lztsd,
            int nlevels,
            string tiledir)
        {
            m_north = north;
            m_south = south;
            m_east = east;
            m_west = west;
            m_terrainTileService = new GDALTerrainTileService(
                dataset,
                datasetName,
                lztsd,
                nlevels,
                tiledir);
        }

        /// <summary>
        /// Get terrain elevation at specified location.  
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees.</param>
        /// <param name="longitude">Longitude in decimal degrees.</param>
        /// <param name="targetSamplesPerDegree"></param>
        /// <returns>Returns 0 if the tile is not available on disk.</returns>
        public override float GetElevationAt(double latitude, double longitude, double targetSamplesPerDegree)
        {
            try
            {
                if (m_terrainTileService == null || targetSamplesPerDegree < World.Settings.MinSamplesPerDegree)
                    return 0;

                TerrainTile tt = m_terrainTileService.GetTerrainTile(latitude, longitude, targetSamplesPerDegree);
                if (!tt.IsInitialized)
                    tt.Initialize();
                return tt.GetElevationAt(latitude, longitude);
            }
            catch (Exception)
			{
			}
            return 0;
        }
    }

    /// <summary>
    /// This class backs up the GDALTerrain accessor to
    /// tile DEM files into .bil tiles
    /// </summary>
    class GDALTerrainTileService : TerrainTileService
    {
        #region private variables

        private OSGeo.GDAL.Dataset m_dataset;
        private int m_lines;
        private int m_pixels;
        private double[] m_transform = new double[6];

        #endregion

        public GDALTerrainTileService(OSGeo.GDAL.Dataset dataset, string datasetName, double lztsd, int nlevels, string tiledir)
            :
           base(null, datasetName, lztsd, 256,
            ".bil", nlevels, tiledir, new TimeSpan(), "")
        {
            m_dataset = dataset;
            m_lines = dataset.RasterYSize;
            m_pixels = dataset.RasterXSize;
            dataset.GetGeoTransform(m_transform);
            if (dataset.GetRasterBand(1).DataType == (DataType)OSGeo.GDAL.GdalConst.GDT_Int16)
                this.m_dataType = "Int16";
            else if (dataset.GetRasterBand(1).DataType == (DataType)OSGeo.GDAL.GdalConst.GDT_Float32)
                this.m_dataType = "Float32";
        }

        public new TerrainTile GetTerrainTile(double latitude, double longitude, double samplesPerDegree)
        {
            //Get Empty Terrain Tile
            TerrainTile tt = base.GetTerrainTile(latitude, longitude, samplesPerDegree);
            //Populate target .bil file with data from DataSet
            string cachefile = tt.TerrainTileFilePath;
            
            //work out lines and pixels to extract from VRT
            int startpixel = (int)Math.Floor((tt.West - m_transform[0]) / m_transform[1]);
            int endpixel = (int)Math.Ceiling((tt.East - m_transform[0]) / m_transform[1]);
            int startline = (int)Math.Floor((tt.North - m_transform[3]) / m_transform[5]);
            int endline = (int)Math.Ceiling((tt.South - m_transform[3]) / m_transform[5]);
            

            int xsize = endpixel - startpixel;
            int ysize = endline - startline;
            
            //Allow partial raster access for beyond edge cases
            int realstartpixel = Math.Max(0, startpixel);
            int realstartline = Math.Max(0, startline);
            int realendpixel = Math.Min(m_pixels, endpixel);
            int realendline = Math.Min(m_lines, endline);

            //Scale target window in case of partial access
            int realxsize = realendpixel - realstartpixel;
            int realysize = realendline - realstartline;

            //Scale buffer window
            int bufxsize = (int)Math.Round(256.0 * (double)realxsize / (double)xsize);
            int bufysize = (int)Math.Round(256.0 * (double)realysize / (double)ysize);
            int xoff = (int)Math.Round(256.0 * (double)(realstartpixel - startpixel) / (double)xsize);
            int yoff = (int)Math.Round(256.0 * (double)(realstartline - startline) / (double)ysize);
            int totalbuf = bufxsize * bufysize;

            //use pixel space and line space instead of separate buffers
            Int16[] dembuffer = new Int16[totalbuf];


            //extract data from vrt
            try
            {
                m_dataset.GetRasterBand(1).ReadRaster(realstartpixel, realstartline, realxsize, realysize, dembuffer, bufxsize, bufysize, 0, 0);
            }
            catch
            {
                Console.WriteLine("Raster Could not be accessed");
            }

            //write out cache file
            Driver memdriver = Gdal.GetDriverByName("ENVI");
            string[] options = new string[0];
            //TODO: Add compression

            OSGeo.GDAL.Dataset tile_ds = memdriver.Create(cachefile, 256, 256, 1, (DataType)OSGeo.GDAL.GdalConst.GDT_Int16, options);
            tile_ds.GetRasterBand(1).WriteRaster(xoff, yoff, bufxsize, bufysize, dembuffer, bufxsize, bufysize, 0, 0);
            tile_ds.Dispose();

            return tt;
        }
    }
}
