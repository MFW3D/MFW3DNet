using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;
using WorldWind;
using WorldWind.PluginEngine;
using WorldWind.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Threading;
using OGR;

namespace OgrVectorImporter
{
    //OGR 数据插件
    public class OgrImporter : WorldWind.PluginEngine.Plugin
    {
        static string Name = "Imported vector files";
        RenderableObjectList layer;
        World world;
        string labelFieldName;
        int labelFieldIndex;
        ArrayList shapeRecords = new ArrayList();
        OpenFileDialog ofDialog;
        VectorInfoSelector infoSelector;
        MenuItem menuItem;

        // datasource variable
        string fileName;
        RenderableObjectList fileSubList;
        RenderableObjectList styleSubList;
        Icons dataSubList;
        DataSource ds;
        Layer ogrLayer;
        OGR.CoordinateTransformation coordTransform;
        string outWkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",7030]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",6326]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",8901]],UNIT[\"DMSH\",0.0174532925199433,AUTHORITY[\"EPSG\",9108]],AXIS[\"Lat\",NORTH],AXIS[\"Long\",EAST],AUTHORITY[\"EPSG\",4326]]";
        bool needsTransformation = false;
        WaitMessage waitMessage;
        string fwtoolsDir;

        // data variables
        string filterString = "";
        TextFilterType textFilterType = TextFilterType.Exact;
        string keyFieldName;
        int keyFieldIndex;
        DataType keyDataType = DataType.Text;
        Color minLineColor, maxLineColor, minPolygonColor, maxPolygonColor, noDataPolygonColor, textPolygonColor, noDataLineColor, textLineColor;
        float minLineWidth, maxLineWidth, noDataLineWidth, textLineWidth;
        bool minOutlinePolygons, maxOutlinePolygons, noDataOutlinePolygons, textOutlinePolygons;
        double noDataValue;
        double minDataValue, maxDataValue;

        public override void Load()
        {
            // Setup Drag&Drop functionality
            Global.worldWindow.DragEnter += new DragEventHandler(OgrVectorImporter_DragEnter);
            Global.worldWindow.DragDrop += new DragEventHandler(OgrVectorImporter_DragDrop);
            world = Global.worldWindow.CurrentWorld;
            layer = new RenderableObjectList(Name);
            world.RenderableObjects.Add(layer);
            waitMessage = new WaitMessage();
            layer.Add(waitMessage);
            LoadSaveXmlConfig(true);
        }

        public override void Unload()
        {
            world.RenderableObjects.Remove(layer);
            menuItem.Dispose();
            menuItem = null;
        }

        #region 拖拽方法
        /// <summary>
        /// Checks if the object being dropped is a kml or kmz file
        /// </summary>
        private void OgrVectorImporter_DragEnter(object sender, DragEventArgs e)
        {
            if (DragDropIsValid(e))
                e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// Handles dropping of a kml/kmz file (by loading that file)
        /// </summary>
        private void OgrVectorImporter_DragDrop(object sender, DragEventArgs e)
        {
            if (DragDropIsValid(e))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                fileName = files[0];
                LoadFile();
            }
        }

        /// <summary>
        /// Determines if this plugin can handle the dropped item
        /// </summary>
        private static bool DragDropIsValid(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                if (((string[])e.Data.GetData(DataFormats.FileDrop)).Length == 1)
                {
                    string extension = Path.GetExtension(((string[])e.Data.GetData(DataFormats.FileDrop))[0]).ToLower(CultureInfo.InvariantCulture);
                    if (extension == ".shp")
                        return true;
                }
            }
            return false;
        }
        #endregion
        /// <summary>
        /// Gets filename and loads it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItem_Click(object sender, EventArgs e)
        {
            ofDialog = new OpenFileDialog();
            //ofDialog.Filter = "Shapefiles (*.shp)|*.shp";
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = ofDialog.FileName;
                LoadFile();
            }
        }

        void LoadFile()
        {
            FileInfo f = new FileInfo(fileName);
            fileSubList = null;

            // Create the LM branch for the file...
            foreach (RenderableObject l in layer.ChildObjects)
            {
                if (l.Name == f.Name)
                {
                    fileSubList = (RenderableObjectList)l;
                    break;
                }
            }

            if (fileSubList == null)
                fileSubList = new RenderableObjectList(f.Name);

            layer.Add(fileSubList);


            ogr.UseExceptions();

            try
            {
                ogr.RegisterAll();
                ds = ogr.Open(fileName, 0);
                ogrLayer = ds.GetLayerByIndex(0);
                SpatialReference inSrs = ogrLayer.GetSpatialRef();

                ArrayList fieldNames = GetFieldNames(ogrLayer);

                infoSelector = new VectorInfoSelector(ogrLayer);
                if (infoSelector.ShowDialog() != DialogResult.OK)
                    return;

                styleSubList = new RenderableObjectList(infoSelector.TextStyle.Layername);
                styleSubList.IsOn = false;
                fileSubList.Add(styleSubList);

                keyFieldName = infoSelector.TextStyle.KeyDataFieldName;
                keyDataType = infoSelector.TextStyle.DataType;
                labelFieldName = infoSelector.TextStyle.LabelFieldName;

                if (keyDataType == DataType.Text && infoSelector.TextStyle.TextFilter)
                {
                    filterString = infoSelector.TextStyle.TextFilterString;
                    textFilterType = infoSelector.TextStyle.TextFilterType;
                }

                maxPolygonColor = infoSelector.NumericMaxStyle.PolygonColor;
                maxLineColor = infoSelector.NumericMaxStyle.LineColor;
                maxLineWidth = infoSelector.NumericMaxStyle.LineWidth;
                maxOutlinePolygons = infoSelector.NumericMaxStyle.OutlinePolygons;

                minPolygonColor = infoSelector.NumericMinStyle.PolygonColor;
                minLineColor = infoSelector.NumericMinStyle.LineColor;
                minLineWidth = infoSelector.NumericMinStyle.LineWidth;
                minOutlinePolygons = infoSelector.NumericMinStyle.OutlinePolygons;

                noDataLineColor = infoSelector.NumericNoDataStyle.LineColor;
                noDataLineWidth = infoSelector.NumericNoDataStyle.LineWidth;
                noDataPolygonColor = infoSelector.NumericNoDataStyle.PolygonColor;
                noDataOutlinePolygons = infoSelector.NumericNoDataStyle.OutlinePolygons;
                noDataValue = infoSelector.NumericNoDataStyle.NoDataValue;

                textLineColor = infoSelector.TextStyle.LineColor;
                textLineWidth = infoSelector.TextStyle.LineWidth;
                textPolygonColor = infoSelector.TextStyle.PolygonColor;
                textOutlinePolygons = infoSelector.TextStyle.OutlinePolygons;


                if (infoSelector.Projection.Contains("EPSG"))
                {
                    string trimString = "EPSG:";
                    string epsgString = infoSelector.Projection.Trim(trimString.ToCharArray());
                    int epsg = int.Parse(epsgString);
                    inSrs = new SpatialReference("");
                    inSrs.ImportFromEPSG(epsg);
                }
                else if (infoSelector.Projection == "" || infoSelector.Projection == "(unknown)")
                {
                    inSrs = null;
                }
                else
                {
                    inSrs = new SpatialReference("");
                    inSrs.ImportFromProj4(infoSelector.Projection);
                }

                OGR.SpatialReference outSrs = new OGR.SpatialReference(outWkt);
                //outSrs.ImportFromEPSG(4326);

                if (inSrs != null)
                {
                    coordTransform = new CoordinateTransformation(inSrs, outSrs);
                    needsTransformation = true;
                    Console.WriteLine("Reprojecting...");
                }

                keyFieldIndex = GetFieldIndexFromString(keyFieldName, fieldNames);
                labelFieldIndex = GetFieldIndexFromString(labelFieldName, fieldNames);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nNo features imported.");
                return;
            }

            Thread t = new Thread(new System.Threading.ThreadStart(LoadVectors));
            t.IsBackground = true;
            t.Start();
        }

        private int GetFieldIndexFromString(string name, ArrayList names)
        {
            int i = 0;

            foreach (string n in names)
            {
                if (String.Compare(n, name) == 0)
                {
                    return i;
                }
                i++;
            }

            return -1;
        }
        /// <summary>
        /// Fetch the field names from a layer
        /// </summary>
        /// <param name="ogrLayer">the layer</param>
        /// <returns>Arraylist of strings</returns>
        private ArrayList GetFieldNames(Layer ogrLayer)
        {
            FeatureDefn featDef = ogrLayer.GetLayerDefn();
            ArrayList fNames = new ArrayList();

            for (int i = 0; i < featDef.GetFieldCount(); i++)
            {
                FieldDefn fieldDef = featDef.GetFieldDefn(i);
                fNames.Add(fieldDef.GetName());
            }

            return fNames;
        }


        /// <summary>
        /// Loads the shp and adds it to the LM
        /// </summary>
        private void LoadVectors()
        {
            Feature feat;

            try
            {
                if(keyDataType == DataType.Numeric)
                {
                    ogrLayer.ResetReading();

                    bool setRange = false;

                    while ((feat = ogrLayer.GetNextFeature()) != null)
                    {
                        double data = feat.GetFieldAsDouble(keyFieldIndex);

                        if (!setRange && data != noDataValue)
                        {
                            minDataValue = data;
                            maxDataValue = data;
                            setRange = true;
                        }
                        else if (data != noDataValue)
                        {
                            if (minDataValue > data)
                                minDataValue = data;

                            if (maxDataValue < data)
                                maxDataValue = data;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nCould not get data value range.");
            }


            try
            {
                int i = 0;
                int numFeatures = ogrLayer.GetFeatureCount(1);


                ogrLayer.ResetReading();

                while ((feat = ogrLayer.GetNextFeature()) != null)
                {
                    if (keyDataType == DataType.Text)
                    {
                        string keyValue = feat.GetFieldAsString(keyFieldIndex);
                        bool loadFeature = true;

                        if (filterString != null)
                        {
                            switch (textFilterType)
                            {
                                case TextFilterType.Exact:
                                    if (keyValue == filterString)
                                        loadFeature = true;
                                    else
                                        loadFeature = false;
                                    break;

                                case TextFilterType.Contains:
                                    if (keyValue.Contains(filterString))
                                        loadFeature = true;
                                    else
                                        loadFeature = false;
                                    break;

                                case TextFilterType.Regex:
                                    Match m = Regex.Match(keyValue, filterString);
                                    if (m.Success)
                                        loadFeature = true;
                                    else
                                        loadFeature = false;
                                    break;
                            }
                        }

                        if (loadFeature)
                        {
                            string labelValue = feat.GetFieldAsString(labelFieldIndex);
                            OGR.Geometry geom = feat.GetGeometryRef();
                            //Console.WriteLine("Parsing {0} of {1} features.", i+1, numFeatures);
                            waitMessage.Text = string.Format("Parsing {0} of {1} features.", i + 1, numFeatures);
                            ParseGeometry(geom, keyValue, 0, labelValue);
                            feat.Dispose();
                            i++;
                        }
                    }

                    if (keyDataType == DataType.Numeric)
                    {
                        double keyDataValue = feat.GetFieldAsDouble(keyFieldIndex);
                        bool loadFeature = true;

                        // numeric filtering would happen here...

                        if (loadFeature)
                        {
                            string labelValue = feat.GetFieldAsString(labelFieldIndex);
                            OGR.Geometry geom = feat.GetGeometryRef();
                            //Console.WriteLine("Parsing {0} of {1} features.", i+1, numFeatures);
                            waitMessage.Text = string.Format("Parsing {0} of {1} features.", i + 1, numFeatures);
                            ParseGeometry(geom, keyDataValue.ToString(), keyDataValue, labelValue);
                            feat.Dispose();
                            i++;
                        }
                    }
                }

                waitMessage.Text = "Loaded " + i + " features of " + numFeatures + ".";

                ds.Dispose();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nLoading aborted.");
            }
        }

        private void ParseGeometry(OGR.Geometry geom, string keyStringValue, double keyNumericValue, string labelValue)
        {
            int geomType = geom.GetGeometryType();

            if (geomType == ogr.wkbUnknown)
            {
                return;
            }

            // points
            else if (geomType == ogr.wkbPoint || geomType == ogr.wkbPoint25D)
            {
                bool dataSubListExists = false;

                foreach (Icons l in styleSubList.ChildObjects)
                {
                    if (l.Name == keyStringValue)
                    {
                        dataSubList = l;
                        dataSubListExists = true;
                    }
                }

                if (!dataSubListExists)
                {
                    dataSubList = new Icons(keyStringValue);
                    //subSubList.DisableExpansion = true;
                    styleSubList.Add(dataSubList);
                }

                double[] p = { geom.GetX(0), geom.GetY(0) };
                if (needsTransformation)
                    coordTransform.TransformPoint(p);

                WorldWind.Renderable.Icon ic = new WorldWind.Renderable.Icon(labelValue, p[1], p[0],0);
                //ic.TextureFileName = Path.Combine(this.PluginDirectory, "icon.png");
                ic.TextureFileName = "";
                //ic.Width = 12;
                //ic.Height = 12;

                dataSubList.Add(ic);
                //Console.WriteLine("  Found point, contains " + geom.GetPointCount());
                waitMessage.Text += "\nFound " + geom.GetGeometryName();
            }


            // linestring or multi-line
            else if (geomType == ogr.wkbLineString ||
                geomType == ogr.wkbLineString25D)
            {
                //Console.WriteLine("  Found " + geom.GetGeometryName() + ", contains " + geom.GetPointCount() + "points, " + geom.GetGeometryCount() + " sub-geometries");
                waitMessage.Text += "\nFound " + geom.GetGeometryName() + ", contains " + geom.GetPointCount() + " points, " + geom.GetGeometryCount() + " sub-geometries";
                bool dataSubListExists = false;

                foreach (Icons l in styleSubList.ChildObjects)
                {
                    if (l.Name == keyStringValue)
                    {
                        dataSubList = l;
                        dataSubListExists = true;
                    }
                }

                if (!dataSubListExists)
                {
                    dataSubList = new Icons(keyStringValue);
                    //subSubList.DisableExpansion = true;
                    styleSubList.Add(dataSubList);
                }

                int pointCount = geom.GetPointCount();
                Point3d[] p = new Point3d[pointCount];

                //TODO: handle 2.5/3D?
                for (int i = 0; i < pointCount; i++)
                {
                    double[] point = { geom.GetX(i), geom.GetY(i) };
                    if (needsTransformation)
                        coordTransform.TransformPoint(point);

                    p[i] = new Point3d(point[0], point[1], 0);
                }

                //Color lineColor = Color.FromArgb(infoSelector.LineAlpha, infoSelector.LineColor);
                Color lineColor = InterpolateColor(keyNumericValue, true);

                LineFeature lf = new LineFeature(labelValue,
                                    world,
                                    p,
                                    lineColor);
                lf.LineWidth = infoSelector.NumericMinStyle.LineWidth;
                dataSubList.Add(lf);
            }

            // polygon...
            else if (geomType == ogr.wkbPolygon || geomType == ogr.wkbPolygon25D)
            {
                bool dataSubListExists = false;

                foreach (Icons l in styleSubList.ChildObjects)
                {
                    if (l.Name == keyStringValue)
                    {
                        dataSubList = l;
                        dataSubListExists = true;
                    }
                }

                if (!dataSubListExists)
                {
                    dataSubList = new Icons(keyStringValue);
                    //subSubList.DisableExpansion = true;
                    styleSubList.Add(dataSubList);
                }

                //Console.WriteLine("  Found " + geom.GetGeometryName() + ", contains " + geom.GetGeometryCount() + " sub-geometries");
                waitMessage.Text += "\nFound " + geom.GetGeometryName() + ", contains " + geom.GetPointCount() + " points, " + geom.GetGeometryCount() + " sub-geometries";

                int numInnerRings = geom.GetGeometryCount() - 1;

                LinearRing[] innerRings;
                if (numInnerRings != 0)
                    innerRings = new LinearRing[numInnerRings];
                else
                    innerRings = null;

                LinearRing outerRing = new LinearRing();

                OGR.Geometry ring;

                //outer ring...
                ring = geom.GetGeometryRef(0);
                waitMessage.Text += "\nFound " + ring.GetGeometryName() + ", contains " + ring.GetPointCount() + " points, " + ring.GetGeometryCount() + " sub-geometries";

                int pointCount = ring.GetPointCount();
                Point3d[] p = new Point3d[pointCount];

                for (int k = 0; k < pointCount; k++)
                {
                    double x = ring.GetX(k);
                    double y = ring.GetY(k);
                    double[] point = { x, y };
                    if (needsTransformation)
                        coordTransform.TransformPoint(point);

                    p[k] = new Point3d(point[0], point[1], 0);
                }

                outerRing.Points = p;

                //inner rings...
                if (innerRings != null)
                {
                    for (int i = 1; i < geom.GetGeometryCount(); i++)
                    {
                        ring = geom.GetGeometryRef(i);
                        waitMessage.Text += "\nFound " + ring.GetGeometryName() + ", contains " + ring.GetPointCount() + " points, " + ring.GetGeometryCount() + " sub-geometries";
                        for (int j = 0; j < ring.GetPointCount(); j++)
                        {
                            int innerRingPointCount = ring.GetPointCount();
                            Point3d[] q = new Point3d[innerRingPointCount];

                            for (int k = 0; k < innerRingPointCount; k++)
                            {
                                double x = ring.GetX(k);
                                double y = ring.GetY(k);
                                double[] point = { x, y };
                                if (needsTransformation)
                                    coordTransform.TransformPoint(point);

                                q[k] = new Point3d(point[0], point[1], 0);
                            }

                            LinearRing r = new LinearRing();
                            r.Points = q;

                            innerRings[i - 1] = r;
                        }
                    }
                }

                Color fillColor = InterpolateColor(keyNumericValue, false);
                Color lineColor = InterpolateColor(keyNumericValue, true);

                PolygonFeature pf = new PolygonFeature(
                                    labelValue,
                                    world,
                                    outerRing,
                                    innerRings,
                                    fillColor);

                pf.Outline = infoSelector.NumericMinStyle.OutlinePolygons;
                pf.OutlineColor = lineColor;
                dataSubList.Add(pf);
            }

            else if (geomType == ogr.wkbMultiPoint ||
                geomType == ogr.wkbMultiPoint25D ||
                geomType == ogr.wkbMultiLineString ||
                geomType == ogr.wkbMultiLineString25D ||
                geomType == ogr.wkbMultiPolygon ||
                geomType == ogr.wkbMultiPolygon25D)
            {
                waitMessage.Text += "\nFound " + geom.GetGeometryName() + ", contains " + geom.GetGeometryCount() + " sub-geometries";
                ParseGeometry(geom, keyStringValue, keyNumericValue, labelValue);
            }

        }

        private Color InterpolateColor(double keyNumericValue, bool isLine)
        {
            if(keyDataType == DataType.Text)
            {
                if (isLine)
                    return textLineColor;
                else
                    return textPolygonColor;
            }

            if (keyNumericValue == noDataValue)
            {
                if (isLine)
                    return noDataLineColor;
                else
                    return noDataPolygonColor;
            }

            //determine fraction of range above minimum
            double dataRange = maxDataValue - minDataValue;
            double interp = ((keyNumericValue - minDataValue) / dataRange);

            if (isLine)
            {
                double minR = (double)minLineColor.R;
                double minG = (double)minLineColor.G;
                double minB = (double)minLineColor.B;
                double minA = (double)minLineColor.A;
                double maxR = (double)maxLineColor.R;
                double maxG = (double)maxLineColor.G;
                double maxB = (double)maxLineColor.B;
                double maxA = (double)maxLineColor.A;

                double rRange = maxR - minR;
                double gRange = maxG - minG;
                double bRange = maxB - minB;
                double aRange = maxA - minA;

                double R = rRange * interp + minR;
                double G = gRange * interp + minG;
                double B = bRange * interp + minB;
                double A = aRange * interp + minA;

                return Color.FromArgb((int)A, (int)R, (int)G, (int)B);
            }
            else
            {
                double minR = (double)minPolygonColor.R;
                double minG = (double)minPolygonColor.G;
                double minB = (double)minPolygonColor.B;
                double minA = (double)minPolygonColor.A;
                double maxR = (double)maxPolygonColor.R;
                double maxG = (double)maxPolygonColor.G;
                double maxB = (double)maxPolygonColor.B;
                double maxA = (double)maxPolygonColor.A;

                double rRange = maxR - minR;
                double gRange = maxG - minG;
                double bRange = maxB - minB;
                double aRange = maxA - minA;

                double R = rRange * interp + minR;
                double G = gRange * interp + minG;
                double B = bRange * interp + minB;
                double A = aRange * interp +minA;

                return Color.FromArgb((int)A, (int)R, (int)G, (int)B);
            }
        }

        /// <summary>
        /// Gets fwtools dir location from user
        /// </summary>
        private void getFWLoc()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select FWTools Location";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                fwtoolsDir = folderDialog.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(fwtoolsDir);

                if (di.Exists)
                {
                    return;
                }
                else
                    throw (new Exception("Fwtools directory not located"));
            }
        }

        /// <summary>
        /// Restore/Save XML Configuration
        /// </summary>
        /// <param name="load">Specifies whether the settings are being loaded (true) or saved (false)</param>
        private void LoadSaveXmlConfig(bool load)
        {
            try
            {
                //TODO: Need a better way to locate Plugin Directory and Load settings
                //					if(m_layer.isInitialized)
                //					{
                string PluginDirectory = this.PluginDirectory;
                string settings = Path.Combine(PluginDirectory, "Settings.xml");
                //Console.WriteLine(settings);
                if (System.IO.File.Exists(settings))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(settings);
                    XmlNodeList list = doc.GetElementsByTagName("FWTools");
                    foreach (XmlNode node in list)
                    {
                        if (load)
                            fwtoolsDir = node.InnerText;
                        else
                        {
                            node.InnerText = fwtoolsDir;
                            doc.Save(settings);
                        }
                    }
                }

                //create file if it does not exist
                if (!System.IO.File.Exists(settings))
                {
                    getFWLoc();
                    XmlDocument doc = new XmlDocument();
                    XmlNode fwtools = doc.CreateElement("FWTools");
                    fwtools.InnerText = fwtoolsDir;
                    doc.AppendChild(fwtools);
                    doc.Save(settings);
                }
                
                string path = System.Environment.GetEnvironmentVariable("PATH");
                DirectoryInfo d = new DirectoryInfo(Path.Combine(fwtoolsDir, "bin"));
                if (d.Exists && !path.Contains(d.FullName))
                {
                    try
                    {
                        System.Environment.SetEnvironmentVariable("PATH", path + ";" + d.FullName);
                    }
                    catch (Exception  ex)
                    {
                        MessageBox.Show(ex.Message + "\nRequired system environment variable not set.");
                        waitMessage.Text = "GDAL environment not loaded successfully";
                        return;
                    }
                }

                if (System.Environment.GetEnvironmentVariable("GDAL_DATA") == null)
                {
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(fwtoolsDir, "data"));
                    if (di.Exists)
                    {
                        try
                        {
                            System.Environment.SetEnvironmentVariable("GDAL_DATA", Path.Combine(fwtoolsDir, "data"));
                            waitMessage.Text = "GDAL environment loaded successfully.";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\nRequired system environment variable not set.");
                            waitMessage.Text = "GDAL environment not loaded successfully";
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("FWTools data dir not found.");
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }

    public enum TextFilterType
    {
        Exact,
        Contains,
        Regex
    }

    public enum DataType
    {
        Text,
        Numeric
    }
}
