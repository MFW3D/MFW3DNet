using System;
using System.Collections.Generic;
using System.Text;
using WorldWind.Renderable;
using System.Xml;
using System.Drawing;
using Utility;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using WorldWind.Net;

namespace WorldWind.KMLReader
{
    public class KMLParser
    {
        private const int IconSizeConstant = 32;			// The default icon size used for scaling

        private Hashtable iconStyles = new Hashtable();		// The main Style storage

        private Hashtable bitmapCache = new Hashtable();	// Hashtable to cache Bitmaps from various sources

        private ArrayList networkLinks = new ArrayList();	// Stores created NetworkLinks

        private Icons m_layer;

        private string m_path;

        public static string KmlDirectory = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "kml");

        public void ReadKML(string kml, Icons layer, string path)
        {
            m_layer = layer;
            m_path = path;

            kml = kml.Replace("xmlns=\"http://earth.google.com/kml/2.0\"", "");	// HACK
            kml = kml.Replace("xmlns='http://earth.google.com/kml/2.0'", "");	// DOUBLE HACK
            kml = kml.Replace("xmlns=\"http://earth.google.com/kml/2.1\"", "");	// MULTI HACK!
            kml = kml.Replace("xmlns='http://earth.google.com/kml/2.1'", "");	// M-M-M-M-M-M-M-MONSTER HACK!!!!
            kml = kml.Replace("xmlns='http://earth.google.com/kml/2.2'", "");	// HACKFEST CONTINUES!!!!
            kml = kml.Replace("xmlns=\"http://earth.google.com/kml/2.2\"", "");	// HACKFEST CONTINUES!!!!

            // Open the downloaded xml in an XmlDocument to allow for XPath searching
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(kml);

            // Try to find some sort of name for this kml from various places
            XmlNode node = doc.SelectSingleNode("//Document[name]/name");

            if (layer.Name == null || layer.Name.Length == 0 || layer.Name.Equals("KML Icons"))
            {
                if (node != null)
                    layer.Name = node.InnerText;
            }
            // Parse Style and StyleMap nodes and store them
            ParseStyles(doc, path);

            // Load Placemarks recursively and put them in folders
            XmlNode inNode = doc.SelectSingleNode("/kml/Document");
            if (inNode == null)
                inNode = doc.SelectSingleNode("/kml");
            if(inNode == null)
            	inNode = doc.SelectSingleNode("/Document");
            if (inNode == null)
                inNode = doc.SelectSingleNode("/kml/Folder");
            if (inNode != null)
                ParseRenderables(inNode, layer, path);
        }

        public void Cleanup()
        {
            if (m_layer != null)
            {
                foreach (RenderableObject ro in m_layer.ChildObjects)
                {
                    ro.Dispose();
                }
                m_layer.ChildObjects.Clear();
            }

            foreach (Object bit in bitmapCache)
            {
                Bitmap bitmap = bit as Bitmap;
                if (bitmap != null)
                    bitmap.Dispose();
            }
            bitmapCache.Clear();

            foreach (KMLNetworkLink netLink in networkLinks)
            {
                netLink.Dispose();
            }
            networkLinks.Clear();

            iconStyles.Clear();
        }


        #region Parsing methods
        /// <summary>
        /// Parses Styles and StyleMaps and stores them
        /// </summary>
        /// <param name="doc">The document to load styles from</param>
        /// <param name="KmlPath">The path to the KML file that is being loaded</param>
        private void ParseStyles(XmlDocument doc, string KmlPath)
        {
            // Load IconStyle elements and extract the images
            XmlNodeList styles = doc.SelectNodes("//Style[@id]");
            foreach (XmlNode xstyle in styles)
            {
                string name = xstyle.Attributes.GetNamedItem("id").InnerText;
                if (iconStyles.ContainsKey(name))
                    continue;

                Style style = GetStyle(xstyle, new Style(), KmlPath);
                if (style != null)
                    iconStyles.Add(name, style);

            }

            // Load StyleMaps and extract the images linked
            XmlNodeList stylemaps = doc.SelectNodes("//StyleMap[@id]");
            foreach (XmlNode stylemap in stylemaps)
            {
                string name = stylemap.Attributes.GetNamedItem("id").InnerText;
                if (iconStyles.ContainsKey(name))
                    continue;

                System.Xml.XmlNode stylemapNode = stylemap.SelectSingleNode("Pair[key=\"normal\"]/styleUrl");
                if (stylemapNode == null)
                    continue;

                string normalName = stylemapNode.InnerText.Replace("#", "");
                XmlNode normalNode = doc.SelectSingleNode("//Style[@id='" + normalName + "']");

                Style style = GetStyle(normalNode, new Style(), KmlPath);
                if (style != null)
                    iconStyles.Add(name, style);
            }

        }

        /// <summary>
        /// Parses everything that is not a style
        /// </summary>
        /// <param name="inNode">The node containing renderables</param>
        /// <param name="layer">The layer to add the resulting renderables to</param>
        /// <param name="KmlPath">The path to the KML file that is being loaded</param>
        private void ParseRenderables(XmlNode inNode, Icons layer, string KmlPath)
        {
            // Extract and set layer visibility for the current layer
            XmlNode visible = inNode.SelectSingleNode("visibility");
            if (visible != null)
            {
                if (visible.InnerText == "0")
                    layer.IsOn = false;
            }

            // Parse all Folders
            ParseFolders(inNode, layer, KmlPath);

            // Parse NetworkLinks
            ParseNetworkLinks(inNode, layer);

            // Parse GroundOverlays
            ParseGroundOverlays(inNode, layer);

            //Parse ScreenOverlays
            ParseScreenOverlays(inNode, layer);

            // Parse Placemarks
            ParsePlacemarks(inNode, layer, KmlPath);

            // Parse LineStrings
            ParseLineStrings(inNode, layer);

            // Parse Polygons
            ParsePolygons(inNode, layer);

            // Parse MultiGeometry
            ParseMultiGeometry(inNode, layer);

            // Update metadata for this layer
            layer.MetaData["Child count"] = layer.ChildObjects.Count;
        }

        /// <summary>
        /// Locates Folders and parses them recursively
        /// </summary>
        /// <param name="inNode">The XmlNode to extract the Folders from</param>
        /// <param name="layer">The layer to add the folders to</param>
        /// <param name="KmlPath">The path to the KML file that is being loaded</param>
        private void ParseFolders(XmlNode inNode, Icons layer, string KmlPath)
        {
            // Find Folders and initialize them recursively
            XmlNodeList folders = inNode.SelectNodes("Folder");
            foreach (XmlNode node in folders)
            {
                try
                {
                    // Find the name of the folder
                    string foldername = "Folder";
                    XmlNode nameNode = node.SelectSingleNode("name");
                    if (nameNode != null)
                        foldername = nameNode.InnerText;

                    // See if the folder already exists
                    Icons folder = null;
                    foreach (RenderableObject ro in layer.ChildObjects)
                    {
                        Icons Icons = ro as Icons;
                        if ((Icons != null) && (ro.Name == foldername))
                        {
                            folder = Icons;
                        }
                    }

                    // Create a new folder if it doesn't exist yet
                    if (folder == null)
                    {
                        folder = new Icons(foldername);
                        layer.Add(folder);
                    }

                    XmlNode visibilityNode = node.SelectSingleNode("visibility");
                    if (visibilityNode != null)
                        layer.IsOn = (visibilityNode.InnerText == "1" ? true : false);

                    //TODO: Folders may have temporal extents parse these
                    XmlNode timeSpanNode = node.SelectSingleNode("TimeSpan");
                    if (timeSpanNode != null)
                    {
                        try
                        {
                            DateTime folderStartTime = DateTime.Parse(timeSpanNode.SelectSingleNode("begin").InnerText);
                            DateTime folderEndTime = DateTime.Parse(timeSpanNode.SelectSingleNode("end").InnerText);
                            folder.EarliestTime = folderStartTime;
                            folder.LatestTime = folderEndTime;
                        }
                        catch(Exception ex)
                        {
                            Log.Write(Log.Levels.Error, "KMLParser: Time parse failure;" + ex.ToString()); 
                        }
                    }

                    //Set selectability
                    folder.isSelectable = true;

                    // Parse placemarks into the folder
                    ParseRenderables(node, folder, KmlPath);
                }
                catch (Exception ex)
                { Log.Write(Log.Levels.Error, "KMLParser: " + ex.ToString()); }
            }
        }

        /// <summary>
        /// Parse NetworkLinks
        /// </summary>
        /// <param name="inNode">The XmlNode to load NetworkLinks from</param>
        /// <param name="layer">The layer to add NetworkLinks to</param>
        private void ParseNetworkLinks(XmlNode inNode, Icons layer)
        {
            // Find NetworkLinks, initialize them and download them for the first time
            XmlNodeList networklinks = inNode.SelectNodes("NetworkLink");
            foreach (XmlNode node in networklinks)
            {
                try
                {
                    // Find out the name for this NetworkLink
                    string nlName = "NetworkLink";
                    XmlNode nameNode = node.SelectSingleNode("name");
                    if (nameNode != null)
                        nlName = nameNode.InnerText;

                    // See if a folder for this NetworkLink already exists
                    Icons folder = null;
                    foreach (RenderableObject ro in layer.ChildObjects)
                    {
                        Icons Icons = ro as Icons;
                        if ((Icons != null) && (ro.Name == nlName))
                        {
                            folder = Icons;
                        }
                    }

                    // Create a new folder if none is available
                    if (folder == null)
                    {
                        folder = new Icons(nlName);
                        layer.Add(folder);
                    }

                    XmlNode visibilityNode = node.SelectSingleNode("visibility");
                    if (visibilityNode != null)
                        folder.IsOn = (visibilityNode.InnerText == "1" ? true : false);

                    // Find the URL to download the file from
                    string loadFile = null;
                    XmlNode hrefNode = node.SelectSingleNode("Url/href");
                    if ((hrefNode != null) && (hrefNode.InnerText.Length > 0))
                        loadFile = hrefNode.InnerText;

                    // Give up if no URL can be found
                    if (loadFile == null)
                        continue;

                    int tickSeconds = -1;
                    int viewSeconds = -1;

                    bool fired = false;
                    if (node.SelectSingleNode("Url/refreshMode") != null)
                    {
                        if (node.SelectSingleNode("Url/refreshMode").InnerText == "onInterval")
                        {
                            string refreshText = node.SelectSingleNode("Url/refreshInterval").InnerText;
                            tickSeconds = Convert.ToInt32(refreshText, CultureInfo.InvariantCulture);
                        }
                        if (node.SelectSingleNode("Url/refreshMode").InnerText == "once")
                        {
                            KMLNetworkLink netLink = new KMLNetworkLink(this, folder, loadFile, -1, -1);
                            netLink.Fire();
                            netLink.Dispose();

                            fired = true;
                        }
                    }

                    if ((node.SelectSingleNode("Url/viewRefreshMode") != null) && (node.SelectSingleNode("Url/viewRefreshMode").InnerText == "onStop"))
                    {
                        string refreshText = node.SelectSingleNode("Url/viewRefreshTime").InnerText;
                        viewSeconds = Convert.ToInt32(refreshText, CultureInfo.InvariantCulture);
                    }

                    // Initialize the actual NetworkLink object to handle updates for us
                    if (tickSeconds != -1 || viewSeconds != -1)
                    {
                        KMLNetworkLink netLink = new KMLNetworkLink(this, folder, loadFile, tickSeconds * 1000, viewSeconds * 1000);
                        netLink.Fire();
                        networkLinks.Add(netLink);
                    }
                    else if (!fired)
                    {
                        KMLNetworkLink netLink = new KMLNetworkLink(this, folder, loadFile, -1, -1);
                        netLink.Fire();
                        netLink.Dispose();
                    }
                }
                catch (Exception ex)
                { Log.Write(Log.Levels.Error, "KMLParser: " + ex.ToString()); }
            }
        }

        /// <summary>
        /// Parses Placemarks
        /// </summary>
        /// <param name="inNode">The node containing Placemarks</param>
        /// <param name="layer">The layer to add the resulting icons or folders to</param>
        /// <param name="KmlPath">The path to the KML file that is being loaded</param>
        private void ParsePlacemarks(XmlNode inNode, Icons layer, string KmlPath)
        {
            foreach (WorldWind.Renderable.RenderableObject ro in layer.ChildObjects)
            {
                KMLIcon KMLIcon = ro as KMLIcon;
                if (KMLIcon != null)
                    KMLIcon.HasBeenUpdated = false;
            }

            // Parse all Placemarks that have a name and location
            XmlNodeList placemarks = inNode.SelectNodes("Placemark[name and Point]");
            foreach (XmlNode node in placemarks)
            {
                try
                {
                    string name = node.SelectSingleNode("name").InnerText;
                    KMLIcon update = null;

                    // Extract the location string for this Placemark node and split it up
                    string loc = node.SelectSingleNode("Point/coordinates").InnerText.Trim();

                    LLA lla = ParseCoordinate(loc);

                    string desc = null;
                    string uri = null;

                    // Extract a description and make sure it's not too long
                    XmlNode xnode = node.SelectSingleNode("description");
                    if (xnode != null)
                    {
                        string descRaw = xnode.InnerText;
                        uri = SearchUri(descRaw);

                        // Ashish Datta - commented so that the HTML stays in the description property.
                        desc = descRaw;

                        if (desc.Length > 2505)
                            desc = desc.Substring(0, 2500) + "...";
                    }

                    float rotation = 0;
                    bool bRotated = false;
                    string rotRaw = null;

                    // Locate a node containing rotation 
                    XmlNode rotNode1 = node.SelectSingleNode("Point/rotation");
                    if (rotNode1 != null)
                        rotRaw = rotNode1.InnerText;
                    else
                    {
                        XmlNode rotNode2 = node.SelectSingleNode("IconStyle/heading");
                        if (rotNode2 != null)
                            rotRaw = rotNode2.InnerText;
                        else
                        {
                            XmlNode rotNode3 = node.SelectSingleNode("Style/IconStyle/heading");
                            if (rotNode3 != null)
                                rotRaw = rotNode3.InnerText;
                        }
                    }

                    // If rotation was found parse it
                    if (rotRaw != null)
                    {
                        rotation = Convert.ToSingle(rotRaw, CultureInfo.InvariantCulture);
                        bRotated = true;
                    }

                    // Find a style for this icon
                    Style style = LocateStyle(node, KmlPath);

                    // Check if this icon has to be extruded
                    bool bExtrude = false;
                    XmlNode extrudeNode = node.SelectSingleNode("Point/extrude");
                    if (extrudeNode != null)
                    {
                        if (extrudeNode.InnerText == "1")
                            bExtrude = true;
                    }

                    // See if this icon already exists, and store it if it does
                    foreach (WorldWind.Renderable.RenderableObject ro in layer.ChildObjects)
                    {
                        KMLIcon KMLIcon = ro as KMLIcon;
                        if (KMLIcon != null)
                        {
                            if ((ro.Name == name) && ((style == null) || ((KMLIcon.NormalIcon == style.NormalIcon) && (!KMLIcon.HasBeenUpdated))))
                            {
                                update = KMLIcon;
                                update.HasBeenUpdated = true;

                                break;
                            }
                        }
                    }

                    // If a previous icons has been found update it's location
                    if (update != null)
                    {
                        update.IsRotated = bRotated;
                        if (bRotated)
                        {
                            update.Rotation = Angle.FromDegrees(rotation);
                        }
                        if (style != null)
                        {
                            update.Height = Double.IsNaN(style.NormalScale) ? IconSizeConstant : (int)(style.NormalScale * Math.Min(((Bitmap)bitmapCache[style.NormalIcon]).Height, IconSizeConstant));
                            update.Width = Double.IsNaN(style.NormalScale) ? IconSizeConstant : (int)(style.NormalScale * Math.Min(((Bitmap)bitmapCache[style.NormalIcon]).Width, IconSizeConstant));
                            update.Description = desc;
                            update.SetPosition(lla.lat, lla.lon, lla.alt);
                        }
                    }
                    else
                    {
                        // Create the icon with either the generated bitmap or the default dot
                        if (style != null)
                        {
                            CreateIcon(layer, name, desc, uri, lla.lat, lla.lon, lla.alt, style, bRotated, rotation, bExtrude);
                        }
                        else
                        {
                            // Use the default 'tack' icon if no style was found
                            string pal3Path = Path.Combine(KmlDirectory, "icons/palette-3.png");
                            if (File.Exists(pal3Path))
                            {
                                if (!bitmapCache.Contains(pal3Path))
                                    bitmapCache.Add(pal3Path, (Bitmap)Bitmap.FromFile(pal3Path));
                                Style pinStyle = new Style(GetSubImage(new Style(pal3Path), 448, 64, 64, 64));

                                CreateIcon(layer, name, desc, uri, lla.lat, lla.lon, lla.alt, pinStyle, bRotated, rotation, bExtrude);
                            }
                        }
                    }
                }
                catch (Exception ex)
                { Log.Write(Log.Levels.Error, "KMLParser: " + ex.ToString()); }
            }

            // Cleanup icons that have not been updated
            RemoveUnusedIcons(layer);
        }

        /// <summary>
        /// Parse Ground Overlays
        /// </summary>
        /// <param name="inNode">The node containing Ground Overlays</param>
        /// <param name="layer">The layer to add the resulting Ground Overlays to</param>
        private void ParseGroundOverlays(XmlNode inNode, Icons layer)
        {
            // Parse all Placemarks that have a name and LineString
            XmlNodeList groundOverlays = inNode.SelectNodes("GroundOverlay[name and LatLonBox]");
            foreach (XmlNode node in groundOverlays)
            {
                // Extract the name from this node
                XmlNode nameNode = node.SelectSingleNode("name");
                string name = nameNode.InnerText;

                XmlNode latLonBoxNode = node.SelectSingleNode("LatLonBox");
                //Parse Coordinates
                if (latLonBoxNode != null)
                {
                    XmlNode northNode = latLonBoxNode.SelectSingleNode("north");
                    XmlNode southNode = latLonBoxNode.SelectSingleNode("south");
                    XmlNode westNode = latLonBoxNode.SelectSingleNode("west");
                    XmlNode eastNode = latLonBoxNode.SelectSingleNode("east");

                    double north = ConfigurationLoader.ParseDouble(northNode.InnerText);
                    double south = ConfigurationLoader.ParseDouble(southNode.InnerText);
                    double west = ConfigurationLoader.ParseDouble(westNode.InnerText);
                    double east = ConfigurationLoader.ParseDouble(eastNode.InnerText);

                    // Create GroundOverlay

                    WorldWind.Renderable.ImageLayer imageLayer = new ImageLayer(
                        name,
                        DrawArgs.CurrentWorldStatic,
                        0,
                        null,
                        south,
                        north,
                        west,
                        east,
                        1.0,
                        DrawArgs.CurrentWorldStatic.TerrainAccessor
                        );

                    imageLayer.DisableZBuffer = true;
                    string filepath = node.SelectSingleNode("Icon/href").InnerText;
                    //check for local images
                    if (!filepath.StartsWith("http://"))
                    {
                        
                        //work out relative or absolute location
                        if (Path.IsPathRooted(filepath))
                            imageLayer.ImagePath = filepath;
                        else //if(Path.GetExtension(m_path).ToLowerInvariant() == "kml")
                            imageLayer.ImagePath = Path.Combine(Path.GetDirectoryName(this.m_path), filepath);
                        //else
                            //Load file from zip
                    }
                    else
                    {
                        imageLayer.ImageUrl = filepath;
                    }

                    XmlNode visibilityNode = node.SelectSingleNode("visibility");
                    if (visibilityNode != null)
                        imageLayer.IsOn = (visibilityNode.InnerText == "1" ? true : false);

                    layer.Add(imageLayer);
                }
            }
        }

        /// <summary>
        /// This Method parses screen overlays and adds to renderables
        ///  using ScreenOverlay Object
        /// </summary>
        /// <param name="inNode">The node containing the Screen Overlay</param>
        /// <param name="layer">The layer to add the resulting Screen Overlay to</param>
        private void ParseScreenOverlays(XmlNode inNode, Icons layer)
        {
            XmlNodeList screenOverlays = inNode.SelectNodes("ScreenOverlay");
            if (screenOverlays != null)
            {
                foreach (XmlNode screenOverlayNode in screenOverlays)
                {
                    XmlNode nameNode = screenOverlayNode.SelectSingleNode("name");
                    String name = "";
                    if (nameNode != null)
                        name = nameNode.InnerText;


                    XmlNode uriNode = screenOverlayNode.SelectSingleNode("Icon/href");
                    string uri = "";
                    if (uriNode != null)
                    {
                        string filepath = uriNode.InnerText;
                        //check for local images
                        if (!filepath.StartsWith("http://"))
                        {
                            //work out relative or absolute location
                            if (!Path.IsPathRooted(filepath))
                                uri = Path.Combine(Path.GetDirectoryName(this.m_path), filepath);
                            //else
                            //Load file from zip
                        }
                    }
                    

                    float posX = 0;
                    float posY = 0;
                    ScreenUnits posXUnits = ScreenUnits.Pixels;
                    ScreenUnits posYUnits = ScreenUnits.Pixels;

                    XmlNode positionNode = screenOverlayNode.SelectSingleNode("screenXY");
                    if (positionNode != null)
                    {
                        if (positionNode.Attributes["x"] != null)
                        {
                            posX = float.Parse(positionNode.Attributes["x"].InnerText, CultureInfo.InvariantCulture);

                            if (positionNode.Attributes["xunits"].InnerText.ToLower() == "fraction")
                            {
                                posXUnits = ScreenUnits.Fraction;
                            }
                        }

                        if (positionNode.Attributes["y"] != null)
                        {
                            posY = float.Parse(positionNode.Attributes["y"].InnerText, CultureInfo.InvariantCulture);

                            if (positionNode.Attributes["yunits"].InnerText.ToLower() == "fraction")
                            {
                                posYUnits = ScreenUnits.Fraction;
                            }
                        }
                    }

                    ScreenOverlay scoverlay = new ScreenOverlay(name, posX, posY, uri);
                    scoverlay.PositionXUnits = posXUnits;
                    scoverlay.PositionYUnits = posYUnits;
                    scoverlay.ShowHeader = false;
                    

                    XmlNode sizeNode = screenOverlayNode.SelectSingleNode("size");
                    if (sizeNode != null)
                    {
                        if (sizeNode.Attributes["x"] != null)
                        {
                            scoverlay.Width = float.Parse(sizeNode.Attributes["x"].InnerText, CultureInfo.InvariantCulture);

                            if (sizeNode.Attributes["xunits"].InnerText.ToLower() == "fraction")
                            {
                                scoverlay.SizeXUnits = ScreenUnits.Fraction;
                            }
                        }

                        if (sizeNode.Attributes["y"] != null)
                        {
                            scoverlay.Height = float.Parse(sizeNode.Attributes["y"].InnerText, CultureInfo.InvariantCulture);

                            if (sizeNode.Attributes["yunits"].InnerText.ToLower() == "fraction")
                            {
                                scoverlay.SizeYUnits = ScreenUnits.Fraction;
                            }
                        }
                    }

                    layer.Add(scoverlay);
                }
            }
        }

        /// <summary>
        /// Parses LineStrings
        /// </summary>
        /// <param name="inNode">The node containing LineStrings</param>
        /// <param name="layer">The layer to add the resulting lines to</param>
        private void ParseLineStrings(XmlNode inNode, Icons layer)
        {
            // Parse all Placemarks that have a name and LineString
            XmlNodeList lineStrings = inNode.SelectNodes("Placemark[name and LineString]");
            foreach (XmlNode node in lineStrings)
            {
                // Extract the name from this node
                XmlNode nameNode = node.SelectSingleNode("name");
                string name = nameNode.InnerText;
                Style style = null;

                // get StyleUrl
                XmlNode styleUrlNode = node.SelectSingleNode("styleUrl");
                if (styleUrlNode != null)
                {
                    string styleUrlKey = styleUrlNode.InnerText.Trim();
                    if (styleUrlKey.StartsWith("#"))
                        styleUrlKey = styleUrlKey.Substring(1, styleUrlKey.Length - 1);

                    style = (Style)iconStyles[styleUrlKey];
                }
                else
                {
                    XmlNode styleNode = node.SelectSingleNode("Style");
                    if (styleNode != null)
                        style = GetStyle(styleNode, new Style(), "");
                }

                if (style == null)
                    style = new Style();

                if (style.LineStyle == null)
                    style.LineStyle = new LineStyle();

                if (style.PolyStyle == null)
                    style.PolyStyle = new PolyStyle();

                // See if this LineString has to be extruded to the ground
                bool extrude = false;
                XmlNode extrudeNode = node.SelectSingleNode("LineString/extrude");
                if (extrudeNode != null)
                    extrude = Convert.ToBoolean(Convert.ToInt16(extrudeNode.InnerText));

                //Parse Coordinates
                XmlNode outerRingNode = node.SelectSingleNode("LineString/coordinates");
                if (outerRingNode != null)
                {
                    // Parse the list of line coordinates
                    Point3d[] points = ParseCoordinates(outerRingNode);
                    LineFeature line = new LineFeature(name, DrawArgs.CurrentWorldStatic, points, System.Drawing.Color.FromArgb(style.LineStyle.Color.Color));

                    XmlNode altitudeModeNode = node.SelectSingleNode("LineString/altitudeMode");
                    line.AltitudeMode = GetAltitudeMode(altitudeModeNode);

                    line.LineWidth = (float)style.LineStyle.Width.Value;


                    if (extrude)
                    {
                        line.Extrude = true;

                        if (style.PolyStyle.Color != null)
                        {
                            line.PolygonColor = System.Drawing.Color.FromArgb(style.PolyStyle.Color.Color);
                        }
                    }

                    XmlNode visibilityNode = node.SelectSingleNode("visibility");
                    if (visibilityNode != null)
                        line.IsOn = (visibilityNode.InnerText == "1" ? true : false);

                    layer.Add(line);
                }
            }
        }

        /// <summary>
        /// Parses Multi Polygons
        /// </summary>
        /// <param name="inNode">The node containing Polygons</param>
        /// <param name="layer">The layer to add the resulting Polygons to</param>
        private void ParseMultiGeometry(XmlNode inNode, Icons layer)
        {
            // Parse all Placemarks that have a name and Polygon
            XmlNodeList placemarkNodes = inNode.SelectNodes("Placemark[name and MultiGeometry]");
            Random rand = new Random((int)DateTime.Now.Ticks);

            foreach (XmlNode placemarkNode in placemarkNodes)
            {
                XmlNode nameNode = placemarkNode.SelectSingleNode("name");
                string name = nameNode.InnerText;

                // change this to something that unifies the geometry into a single object instead of a user-accessible list
                Icons multiGeometryList = new Icons(name);

                Style style = null;

                // get StyleUrl
                XmlNode styleUrlNode = placemarkNode.SelectSingleNode("styleUrl");
                if (styleUrlNode != null)
                {
                    string styleUrlKey = styleUrlNode.InnerText.Trim();
                    if (styleUrlKey.StartsWith("#"))
                        styleUrlKey = styleUrlKey.Substring(1, styleUrlKey.Length - 1);

                    style = (Style)iconStyles[styleUrlKey];
                }
                else
                {
                    XmlNode styleNode = placemarkNode.SelectSingleNode("Style");
                    if (styleNode != null)
                        style = GetStyle(styleNode, new Style(), "");
                }

                if (style == null)
                    style = new Style();

                if (style.LineStyle == null)
                    style.LineStyle = new LineStyle();

                if (style.PolyStyle == null)
                    style.PolyStyle = new PolyStyle();

                XmlNodeList lineStringNodes = placemarkNode.SelectNodes("MultiGeometry/LineString");
                foreach (XmlNode lineStringNode in lineStringNodes)
                {
                    bool extrude = false;
                    XmlNode extrudeNode = lineStringNode.SelectSingleNode("extrude");
                    if (extrudeNode != null)
                        extrude = Convert.ToBoolean(Convert.ToInt16(extrudeNode.InnerText));

                    XmlNode coordinateNode = lineStringNode.SelectSingleNode("coordinates");
                    Point3d[] points = ParseCoordinates(coordinateNode);

                    XmlNode altitudeModeNode = lineStringNode.SelectSingleNode("altitudeMode");
                    AltitudeMode altitudeMode = GetAltitudeMode(altitudeModeNode);

                    if (points != null && points.Length > 0)
                    {
                        LineFeature line = new LineFeature(
                            name,
                            DrawArgs.CurrentWorldStatic,
                            points,
                            System.Drawing.Color.FromArgb(style.LineStyle.Color.Color)
                            );

                        line.AltitudeMode = altitudeMode;
                        if (style.PolyStyle.Color != null)
                            line.PolygonColor = System.Drawing.Color.FromArgb(style.PolyStyle.Color.Color);

                        line.LineWidth = (float)style.LineStyle.Width.Value;
                        line.Extrude = extrude;

                        multiGeometryList.Add(line);
                    }
                }

                XmlNodeList polygonNodes = placemarkNode.SelectNodes("MultiGeometry/Polygon");
                foreach (XmlNode polygonNode in polygonNodes)
                {
                    bool extrude = false;
                    XmlNode extrudeNode = polygonNode.SelectSingleNode("extrude");
                    if (extrudeNode != null)
                        extrude = Convert.ToBoolean(Convert.ToInt16(extrudeNode.InnerText));

                    XmlNode altitudeModeNode = polygonNode.SelectSingleNode("altitudeMode");
                    AltitudeMode altitudeMode = GetAltitudeMode(altitudeModeNode);

                    LinearRing outerRing = null;
                    LinearRing[] innerRings = null;

                    // Parse Outer Ring
                    XmlNode outerRingNode = polygonNode.SelectSingleNode("outerBoundaryIs/LinearRing/coordinates");
                    if (outerRingNode != null)
                    {
                        Point3d[] points = ParseCoordinates(outerRingNode);

                        outerRing = new LinearRing();
                        outerRing.Points = points;
                    }

                    // Parse Inner Ring
                    XmlNodeList innerRingNodes = polygonNode.SelectNodes("innerBoundaryIs");
                    if (innerRingNodes != null)
                    {
                        innerRings = new LinearRing[innerRingNodes.Count];
                        for (int i = 0; i < innerRingNodes.Count; i++)
                        {
                            Point3d[] points = ParseCoordinates(innerRingNodes[i]);

                            innerRings[i] = new LinearRing();
                            innerRings[i].Points = points;
                        }
                    }

                    if (outerRing != null)
                    {
                        PolygonFeature polygonFeature = new PolygonFeature(
                            name,
                            DrawArgs.CurrentWorldStatic,
                            outerRing,
                            innerRings,
                            (style.PolyStyle.Color != null ? System.Drawing.Color.FromArgb(style.PolyStyle.Color.Color) : System.Drawing.Color.Yellow)
                            );

                        polygonFeature.Extrude = extrude;
                        polygonFeature.AltitudeMode = altitudeMode;
                        polygonFeature.Outline = style.PolyStyle.Outline;
                        if (style.LineStyle.Color != null)
                            polygonFeature.OutlineColor = System.Drawing.Color.FromArgb(style.LineStyle.Color.Color);

                        multiGeometryList.Add(polygonFeature);
                    }
                }

                XmlNode visibilityNode = placemarkNode.SelectSingleNode("visibility");
                if (visibilityNode != null)
                    multiGeometryList.IsOn = (visibilityNode.InnerText == "1" ? true : false);

                layer.Add(multiGeometryList);
            }
        }

        private AltitudeMode GetAltitudeMode(XmlNode altitudeModeNode)
        {
            if (altitudeModeNode == null || altitudeModeNode.InnerText == null || altitudeModeNode.InnerText.Length == 0)
                return AltitudeMode.ClampedToGround;

            if (altitudeModeNode != null && altitudeModeNode.InnerText.Length > 0)
            {
                switch (altitudeModeNode.InnerText)
                {
                    case "clampedToGround":
                        return AltitudeMode.ClampedToGround;
                    case "relativeToGround":
                        return AltitudeMode.RelativeToGround;
                    case "absolute":
                        return AltitudeMode.Absolute;
                }
            }

            return AltitudeMode.ClampedToGround;
        }

        /// <summary>
        /// Parses Polygons
        /// </summary>
        /// <param name="inNode">The node containing Polygons</param>
        /// <param name="layer">The layer to add the resulting Polygons to</param>
        private void ParsePolygons(XmlNode inNode, Icons layer)
        {
            // Parse all Placemarks that have a name and Polygon
            XmlNodeList polygons = inNode.SelectNodes("Placemark[name and Polygon]");
            Random rand = new Random((int)DateTime.Now.Ticks);

            foreach (XmlNode node in polygons)
            {
                // Extract the name from this node
                XmlNode nameNode = node.SelectSingleNode("name");
                string name = nameNode.InnerText;

                Style style = null;

                // get StyleUrl
                XmlNode styleUrlNode = node.SelectSingleNode("styleUrl");
                if (styleUrlNode != null)
                {
                    string styleUrlKey = styleUrlNode.InnerText.Trim();
                    if (styleUrlKey.StartsWith("#"))
                        styleUrlKey = styleUrlKey.Substring(1, styleUrlKey.Length - 1);

                    style = (Style)iconStyles[styleUrlKey];
                }
                else
                {
                    XmlNode styleNode = node.SelectSingleNode("Style");
                    if (styleNode != null)
                        style = GetStyle(styleNode, new Style(), "");
                }

                if (style == null)
                    style = new Style();

                if (style.LineStyle == null)
                    style.LineStyle = new LineStyle();

                if (style.PolyStyle == null)
                    style.PolyStyle = new PolyStyle();

                // See if this LineString has to be extruded to the ground
                bool extrude = false;

                XmlNode extrudeNode = node.SelectSingleNode("Polygon/extrude");
                if (extrudeNode != null)
                    extrude = Convert.ToBoolean(Convert.ToInt16(extrudeNode.InnerText));

                XmlNode altitudeModeNode = node.SelectSingleNode("Polygon/altitudeMode");
                AltitudeMode altitudeMode = GetAltitudeMode(altitudeModeNode);

                LinearRing outerRing = null;
                LinearRing[] innerRings = null;

                // Parse Outer Ring
                XmlNode outerRingNode = node.SelectSingleNode("Polygon/outerBoundaryIs/LinearRing/coordinates");
                if (outerRingNode != null)
                {
                    Point3d[] points = ParseCoordinates(outerRingNode);
                    Console.WriteLine(points.Length);

                    outerRing = new LinearRing();
                    outerRing.Points = points;
                }

                // Parse Inner Ring
                XmlNodeList innerRingNodes = node.SelectNodes("Polygon/innerBoundaryIs");
                if (innerRingNodes != null)
                {
                    innerRings = new LinearRing[innerRingNodes.Count];
                    for (int i = 0; i < innerRingNodes.Count; i++)
                    {
                        Point3d[] points = ParseCoordinates(innerRingNodes[i]);
                        innerRings[i] = new LinearRing();
                        innerRings[i].Points = points;
                    }
                }

                if (outerRing != null)
                {
                    PolygonFeature polygonFeature = new PolygonFeature(
                        name, DrawArgs.CurrentWorldStatic,
                        outerRing,
                        innerRings,
                        System.Drawing.Color.FromArgb(style.PolyStyle.Color.Color));

                    polygonFeature.Extrude = extrude;
                    polygonFeature.AltitudeMode = altitudeMode;
                    polygonFeature.Outline = style.PolyStyle.Outline;
                    polygonFeature.Fill = style.PolyStyle.Fill;
                    if (style.LineStyle.Color != null)
                        polygonFeature.OutlineColor = System.Drawing.Color.FromArgb(style.LineStyle.Color.Color);

                    XmlNode visibilityNode = node.SelectSingleNode("visibility");
                    if (visibilityNode != null)
                        polygonFeature.IsOn = (visibilityNode.InnerText == "1" ? true : false);

                    layer.Add(polygonFeature);
                }
            }
        }

        /// <summary>
        /// Parse a list of coordinates
        /// </summary>
        /// <param name="coordinatesNode">The node containing coordinates to parse</param>
        private Point3d[] ParseCoordinates(XmlNode coordinatesNode)
        {
            string coordlist = coordinatesNode.InnerText.Trim();
            char[] splitters = { '\n', ' ', '\t', ',' };
            string[] lines = coordlist.Split(splitters);

            ArrayList tokenList = new ArrayList();
            ArrayList points = new ArrayList();

            int tokenCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string token = lines[i].Trim();
                if (token.Length == 0 || token == String.Empty)
                    continue;

                tokenCount++;
                tokenList.Add(token);
                if (tokenCount == 3)
                {
                    double lon = double.Parse((string)tokenList[tokenList.Count - 3], CultureInfo.InvariantCulture);
                    double lat = double.Parse((string)tokenList[tokenList.Count - 2], CultureInfo.InvariantCulture);
                    double alt = double.Parse((string)tokenList[tokenList.Count - 1], CultureInfo.InvariantCulture);

                    points.Add(new Point3d(lon, lat, alt));
                    tokenCount = 0;
                }
            }
            /*
			
                        for (int i = 0; i < lines.Length; i++)
                        {
                            string loc = lines[i].Trim();
                            if (loc == String.Empty)
                                continue;
				
                            LLA lla = ParseCoordinate(loc);

                            points.Add(new Point3d(lla.lon, lla.lat, lla.alt));
                        }
            */
            return (Point3d[])points.ToArray(typeof(Point3d));
        }

        /// <summary>
        /// Parses a string containing a coordinate
        /// </summary>
        /// <param name="loc">The string containing a coordinate</param>
        /// <returns>The parsed coordinate</returns>
        private LLA ParseCoordinate(string loc)
        {
            // get rid of a leading comma from bad user input
            if (loc.StartsWith(","))
                loc = loc.Substring(1, loc.Length - 1);

            // get rid of a trailing comma from bad user input
            if (loc.EndsWith(","))
                loc = loc.Substring(0, loc.Length - 1);

            string sLon = "0", sLat = "0", sAlt = "0";

            if (loc.Split(',').Length == 3)				// Includes altitude
            {
                sLon = loc.Substring(0, loc.IndexOf(",")).Trim();
                sLat = loc.Substring(loc.IndexOf(",") + 1, loc.LastIndexOf(",") - loc.IndexOf(",") - 1).Trim();
                sAlt = loc.Substring(loc.LastIndexOf(",") + 1, loc.Length - loc.LastIndexOf(",") - 1).Trim();
            }
            else										// Lat and Lon only (assume 0 altitude)
            {
                sLon = loc.Substring(0, loc.IndexOf(",")).Trim();
                sLat = loc.Substring(loc.LastIndexOf(",") + 1, loc.Length - loc.LastIndexOf(",") - 1).Trim();
            }

            // Convert extracted positions to numbers
            //Localize parse errors
            float lat = 0.0f, lon = 0.0f, alt = 0.0f;
            try
            {
                lat = Convert.ToSingle(sLat, CultureInfo.InvariantCulture);
                lon = Convert.ToSingle(sLon, CultureInfo.InvariantCulture);
                alt = Convert.ToSingle(sAlt, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                //Log.Write(Log.Levels.Error, "KMLParser: Location Parse Error;" + ex.ToString());
            }

            LLA lla = new LLA(lat, lon, alt);
            return lla;
        }
        #endregion

        #region Style handling methods
        /// <summary>
        /// Modifies a style with Style tags loaded from an XmlNode
        /// </summary>
        /// <param name="style">The XmlNode containing override information</param>
        /// <param name="oldStyle">The style to override</param>
        /// <param name="KmlPath">The path to the KML file that is being loaded</param>
        /// <returns>The style with overridden values</returns>
        private Style GetStyle(XmlNode style, Style oldStyle, string KmlPath)
        {
            try
            {
                if (style == null)
                    return null;

                Style overStyle = oldStyle;

                bool bPalette = false;

                // Determine the scale, if any, for this style
                XmlNode scaleNode = style.SelectSingleNode("IconStyle/scale");
                if (scaleNode != null)
                    overStyle.NormalScale = Convert.ToDouble(scaleNode.InnerText, CultureInfo.InvariantCulture);

                // Search for style tags in location 1
                XmlNode hrefNode = style.SelectSingleNode("IconStyle/Icon/href");
                if (hrefNode != null)
                {
                    string filename = hrefNode.InnerText;
                    if (filename.StartsWith("root://"))													// Use palette bitmap
                    {
                        filename = Path.Combine(KmlDirectory, filename.Remove(0, 7));
                        if (File.Exists(filename))
                        {
                            bPalette = true;
                            overStyle.NormalIcon = GetDiskImage(filename);
                        }
                    }
                    else if (filename.StartsWith("http://"))											// Use bitmap from the internet
                    {
                        overStyle.NormalIcon = GetWebImage(filename);
                    }
                    else if (File.Exists(Path.Combine(Path.GetDirectoryName(KmlPath), filename)))		// Use a file from disk
                    {
                        overStyle.NormalIcon = GetDiskImage(Path.Combine(Path.GetDirectoryName(KmlPath), filename));
                    }
                    else if (File.Exists(Path.Combine(KmlDirectory, filename)))					// Use a file from disk
                    {
                        overStyle.NormalIcon = GetDiskImage(Path.Combine(KmlDirectory, filename));
                    }
                }

                // See if we need to cut this style to a substyle
                XmlNode wNode = style.SelectSingleNode("IconStyle/Icon/w");
                XmlNode hNode = style.SelectSingleNode("IconStyle/Icon/h");
                if (wNode != null && hNode != null)
                {
                    int w = Convert.ToInt32(wNode.InnerText, CultureInfo.InvariantCulture);
                    int h = Convert.ToInt32(hNode.InnerText, CultureInfo.InvariantCulture);

                    int x = 0, y = 0;
                    XmlNode xNode = style.SelectSingleNode("IconStyle/Icon/x");
                    if (xNode != null)
                        x = Convert.ToInt32(xNode.InnerText, CultureInfo.InvariantCulture);
                    XmlNode yNode = style.SelectSingleNode("IconStyle/Icon/y");
                    if (yNode != null)
                        y = Convert.ToInt32(yNode.InnerText, CultureInfo.InvariantCulture);

                    if (bPalette)
                        overStyle.NormalIcon = GetSubImage(overStyle, x * 2, y * 2, w * 2, h * 2);
                    else
                        overStyle.NormalIcon = GetSubImage(overStyle, x, y, w, h);
                }

                // Search for style tags in a possible secondary location
                XmlNode iconNode = style.SelectSingleNode("icon");
                if (iconNode != null)
                {
                    string filename = iconNode.InnerText;
                    if (!filename.StartsWith("http://"))
                        return null;

                    overStyle.NormalIcon = GetWebImage(filename);
                }

                XmlNode balloonStyleNode = style.SelectSingleNode("BalloonStyle");
                if (balloonStyleNode != null)
                {
                    BalloonStyle balloonStyle = new BalloonStyle();

                    XmlNode balloonTextNode = balloonStyleNode.SelectSingleNode("text");
                    if (balloonTextNode != null)
                    {
                        TextElement textElement = new TextElement();

                        textElement.Text = balloonTextNode.InnerText;

                        XmlNode textNodeColor = balloonTextNode.SelectSingleNode("textColor");
                        if (textNodeColor != null)
                            textElement.TextColor = new ColorElement(ParseColor(textNodeColor.InnerText));

                        balloonStyle.Text = textElement;
                    }

                    XmlNode balloonTextColorNode = balloonStyleNode.SelectSingleNode("textColor");
                    if (balloonTextColorNode != null)
                        balloonStyle.TextColor = new ColorElement(ParseColor(balloonTextColorNode.InnerText));

                    XmlNode balloonColorNode = balloonStyleNode.SelectSingleNode("color");
                    if (balloonColorNode != null)
                        balloonStyle.Color = new ColorElement(ParseColor(balloonColorNode.InnerText));

                    overStyle.BalloonStyle = balloonStyle;
                }

                XmlNode iconStyleNode = style.SelectSingleNode("IconStyle");
                if (iconStyleNode != null)
                {
                    XmlNode iconElementNode = iconStyleNode.SelectSingleNode("Icon");
                    IconElement iconElement = new IconElement();

                    if (iconElementNode != null)
                    {
                        XmlNode iconElementHrefNode = iconElementNode.SelectSingleNode("href");
                        if (iconElementHrefNode != null)
                        {
                            string filename = iconElementHrefNode.InnerText;
                            if (filename.StartsWith("root://"))													// Use palette bitmap
                            {
                                filename = Path.Combine(KmlDirectory, filename.Remove(0, 7));
                                if (File.Exists(filename))
                                {
                                    bPalette = true;
                                    iconElement.href = GetDiskImage(filename);
                                }
                            }
                            else if (filename.StartsWith("http://"))											// Use bitmap from the internet
                            {
                                iconElement.href = GetWebImage(filename);
                            }
                            else if (File.Exists(Path.Combine(Path.GetDirectoryName(KmlPath), filename)))		// Use a file from disk
                            {
                                iconElement.href = GetDiskImage(Path.Combine(Path.GetDirectoryName(KmlPath), filename));
                            }
                            else if (File.Exists(Path.Combine(KmlDirectory, filename)))					// Use a file from disk
                            {
                                iconElement.href = GetDiskImage(Path.Combine(KmlDirectory, filename));
                            }
                        }

                        // See if we need to cut this style to a substyle
                        XmlNode iconElementWNode = iconElementNode.SelectSingleNode("w");
                        XmlNode iconElementHNode = iconElementNode.SelectSingleNode("h");
                        if (iconElementWNode != null && iconElementHNode != null)
                        {
                            int w = Convert.ToInt32(wNode.InnerText, CultureInfo.InvariantCulture);
                            int h = Convert.ToInt32(hNode.InnerText, CultureInfo.InvariantCulture);

                            int x = 0, y = 0;
                            XmlNode xNode = iconElementNode.SelectSingleNode("x");
                            if (xNode != null)
                                x = Convert.ToInt32(xNode.InnerText, CultureInfo.InvariantCulture);
                            XmlNode yNode = iconElementNode.SelectSingleNode("y");
                            if (yNode != null)
                                y = Convert.ToInt32(yNode.InnerText, CultureInfo.InvariantCulture);

                            if (bPalette)
                                iconElement.href = GetSubImage(overStyle, x * 2, y * 2, w * 2, h * 2);
                            else
                                iconElement.href = GetSubImage(overStyle, x, y, w, h);

                        }
                        IconStyle iconStyle = new IconStyle(iconElement);

                        XmlNode iconStyleColorNode = iconStyleNode.SelectSingleNode("color");
                        if (iconStyleColorNode != null)
                            iconStyle.Color = new ColorElement(ParseColor(iconStyleColorNode.InnerText));

                        XmlNode iconStyleColorModeNode = iconStyleNode.SelectSingleNode("colorMode");
                        if (iconStyleColorModeNode != null)
                        {
                            iconStyle.ColorMode = (iconStyleColorModeNode.InnerText.ToLower() == "random" ? ColorMode.Random : ColorMode.Normal);
                        }

                        XmlNode iconStyleHeadingNode = iconStyleNode.SelectSingleNode("heading");
                        if (iconStyleHeadingNode != null)
                            iconStyle.Heading = new DecimalElement(double.Parse(iconStyleHeadingNode.InnerText, CultureInfo.InvariantCulture));

                        XmlNode iconStyleScaleNode = iconStyleNode.SelectSingleNode("scale");
                        if (iconStyleScaleNode != null)
                            iconStyle.Scale = new DecimalElement(double.Parse(iconStyleScaleNode.InnerText, CultureInfo.InvariantCulture));

                        overStyle.IconStyle = iconStyle;
                    }
                }

                XmlNode labelStyleNode = style.SelectSingleNode("LabelStyle");
                if (labelStyleNode != null)
                {
                    LabelStyle labelStyle = new LabelStyle();

                    XmlNode labelColorNode = labelStyleNode.SelectSingleNode("color");
                    if (labelColorNode != null)
                        labelStyle.Color = new ColorElement(ParseColor(labelColorNode.InnerText));

                    XmlNode labelColorModeNode = labelStyleNode.SelectSingleNode("colorMode");
                    if (labelColorModeNode != null)
                        labelStyle.ColorMode = (labelColorModeNode.InnerText.ToLower() == "random" ? ColorMode.Random : ColorMode.Normal);


                    XmlNode labelScaleNode = labelStyleNode.SelectSingleNode("scale");
                    if (labelScaleNode != null)
                        labelStyle.Scale = new DecimalElement(double.Parse(labelScaleNode.InnerText, CultureInfo.InvariantCulture));

                    overStyle.LabelStyle = labelStyle;
                }

                XmlNode lineStyleNode = style.SelectSingleNode("LineStyle");
                if (lineStyleNode != null)
                {
                    LineStyle lineStyle = new LineStyle();

                    XmlNode lineColorNode = lineStyleNode.SelectSingleNode("color");
                    if (lineColorNode != null)
                        lineStyle.Color = new ColorElement(ParseColor(lineColorNode.InnerText));

                    XmlNode lineColorModeNode = lineStyleNode.SelectSingleNode("colorMode");
                    if (lineColorModeNode != null)
                        lineStyle.ColorMode = (lineColorModeNode.InnerText.ToLower() == "random" ? ColorMode.Random : ColorMode.Normal);

                    XmlNode lineWidthNode = lineStyleNode.SelectSingleNode("width");
                    if (lineWidthNode != null)
                        lineStyle.Width = new DecimalElement(double.Parse(lineWidthNode.InnerText, CultureInfo.InvariantCulture));

                    overStyle.LineStyle = lineStyle;
                }

                XmlNode polyStyleNode = style.SelectSingleNode("PolyStyle");
                if (polyStyleNode != null)
                {
                    PolyStyle polyStyle = new PolyStyle();

                    XmlNode polyColorNode = polyStyleNode.SelectSingleNode("color");
                    if (polyColorNode != null)
                        polyStyle.Color = new ColorElement(ParseColor(polyColorNode.InnerText));

                    XmlNode polyColorModeNode = polyStyleNode.SelectSingleNode("colorMode");
                    if (polyColorModeNode != null)
                        polyStyle.ColorMode = (polyColorModeNode.InnerText.ToLower() == "random" ? ColorMode.Random : ColorMode.Normal);

                    XmlNode polyFillNode = polyStyleNode.SelectSingleNode("fill");
                    if (polyFillNode != null)
                        polyStyle.Fill = (polyFillNode.InnerText == "1" ? true : false);

                    XmlNode polyOutlineNode = polyStyleNode.SelectSingleNode("outline");
                    if (polyOutlineNode != null)
                        polyStyle.Outline = (polyOutlineNode.InnerText == "1" ? true : false);

                    overStyle.PolyStyle = polyStyle;
                }

                return overStyle;
            }
            catch (System.Net.WebException ex)
            {
                Log.Write(Log.Levels.Error, "KMLParser: " + ex.ToString());
            }

            return null;
        }

        private int ParseColor(string s)
        {
            int bgrstart = 0;
            string a = "FF";

            if (s.Length > 6)
            {
                a = s.Substring(0, 2);
                bgrstart = 2;
            }

            string b = s.Substring(bgrstart, 2);
            string g = s.Substring(bgrstart + 2, 2);
            string r = s.Substring(bgrstart + 4, 2);


            return System.Drawing.Color.FromArgb(
                int.Parse(a, System.Globalization.NumberStyles.HexNumber),
                int.Parse(r, System.Globalization.NumberStyles.HexNumber),
                int.Parse(g, System.Globalization.NumberStyles.HexNumber),
                int.Parse(b, System.Globalization.NumberStyles.HexNumber)).ToArgb();

        }

        /// <summary>
        /// Extracts a rectangular selection from a given style
        /// </summary>
        /// <param name="style"></param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <returns>The generated bitmap</returns>
        private string GetSubImage(Style style, int x, int y, int w, int h)
        {
            // Try using a cached version
            string key = style.NormalIcon + "|"
                + x.ToString("D5", CultureInfo.InvariantCulture)
                + y.ToString("D5", CultureInfo.InvariantCulture)
                + w.ToString("D5", CultureInfo.InvariantCulture)
                + h.ToString("D5", CultureInfo.InvariantCulture);
            if (bitmapCache.ContainsKey(key))
                return key;

            // Create a new bitmap to draw into
            Bitmap outImage = new Bitmap(w, h);
            Graphics graphics = Graphics.FromImage(outImage);

            // Draw a region into the newly created bitmap
            RectangleF destinationRect = new RectangleF(0, 0, w, h);
            if (style.NormalIcon != null && bitmapCache.Contains(style.NormalIcon))
            {
                System.Drawing.Bitmap bit = ((Bitmap)bitmapCache[style.NormalIcon]);
                RectangleF sourceRect = new RectangleF(x, bit.Height - y - h, w, h);
                graphics.DrawImage((Bitmap)bitmapCache[style.NormalIcon], destinationRect, sourceRect, GraphicsUnit.Pixel);
                graphics.Flush();

                // Cache the generated bitmap
                bitmapCache.Add(key, outImage);

                return key;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a bitmap from cache or a file on disk
        /// </summary>
        /// <param name="filename">The filename to open the bitmap from</param>
        /// <returns>The found bitmap</returns>
        private string GetDiskImage(string filename)
        {
            // Try using a cached version
            if (bitmapCache.ContainsKey(filename))
                return filename;

            // Load the bitmap from disk
            Bitmap bit = (Bitmap)Bitmap.FromFile(filename);
            bitmapCache.Add(filename, bit);

            return filename;
        }

        /// <summary>
        /// Retrieves a bitmap from cache or the web
        /// </summary>
        /// <param name="filename">The URI to download the bitmap from</param>
        /// <returns>The downloaded bitmap</returns>
        private string GetWebImage(string filename)
        {
            // Try using a cached version
            if (bitmapCache.ContainsKey(filename))
                return filename;
            
            // Offline check
            // TODO : probably should do more than just this...
            if (World.Settings.WorkOffline)
                return "";

            // Download the file from the web
            WebDownload myDownload = new WebDownload(filename);
            myDownload.DownloadMemory();

            // Load it into a Bitmap
            Bitmap bit = (Bitmap)System.Drawing.Bitmap.FromStream(myDownload.ContentStream);

            // Cache the downloaded bitmap
            myDownload.Dispose();
            bitmapCache.Add(filename, bit);
            return filename;
        }

        /// <summary>
        /// Locates a referenced style
        /// </summary>
        /// <param name="node"></param>
        /// <param name="KmlPath"></param>
        /// <returns>The located style, or null if none was found</returns>
        private Style LocateStyle(XmlNode node, string KmlPath)
        {
            Style style = null;

            // Takes care of 'normal' IconStyles
            XmlNode styleNode = node.SelectSingleNode("styleUrl");
            if (styleNode != null)
            {
                string styleUrl = styleNode.InnerText.Replace("#", "");
                style = (Style)iconStyles[styleUrl];
            }

            // Check if there's an inline Style node
            styleNode = node.SelectSingleNode("Style");
            if (styleNode != null)
            {
                if (style != null)
                    style = GetStyle(styleNode, style, KmlPath);
                else
                    style = GetStyle(styleNode, new Style(), KmlPath);
            }
            return style;
        }
        #endregion


        /// <summary>
        /// Creates a scaled icon on the globe
        /// </summary>
        /// <param name="layer">The icons</param>
        /// <param name="Name">The name of the item</param>
        /// <param name="Desc">The description</param>
        /// <param name="Uri">The uri for the icon</param>
        /// <param name="Lat">The latitude for the icon</param>
        /// <param name="Lon">The longitude for the icon</param>
        /// <param name="Alt">The altitude to draw the icon at</param>
        /// <param name="style">The style to draw the icon</param>
        /// <param name="bRotated">The rotation flag</param>
        /// <param name="bExtrude">The extrude flag</param>
        private void CreateIcon(Icons layer, string Name, string Desc, string Uri, float Lat, float Lon, float Alt,
            Style style, bool bRotated, float rotation, bool bExtrude)
        {
            // Create the icon and set initial settings
            KMLIcon ic = new KMLIcon(
                Name,									// name
                Lat,										// latitude
                Lon,										// longitude
                style.NormalIcon,							// helper
                Alt);									// altitude

            // Set optional icon settings
            if (Desc != null)
                ic.Description = Desc;
            if (Uri != null)
                ic.ClickableActionURL = Uri;
            if (bRotated)
            {
                ic.Rotation = Angle.FromDegrees(rotation);
                ic.IsRotated = true;
            }
            ic.DrawGroundStick = bExtrude;
            if (style.NormalIcon != null && bitmapCache.Contains(style.NormalIcon))
            {
                ic.Image = (Bitmap)bitmapCache[style.NormalIcon];
                ic.Height = Double.IsNaN(style.NormalScale) ? IconSizeConstant : (int)(style.NormalScale * Math.Min(((Bitmap)bitmapCache[style.NormalIcon]).Height, IconSizeConstant));
                ic.Width = Double.IsNaN(style.NormalScale) ? IconSizeConstant : (int)(style.NormalScale * Math.Min(((Bitmap)bitmapCache[style.NormalIcon]).Width, IconSizeConstant));
            }

            // Add the icon to the layer
            layer.Add(ic);
        }

        /// <summary>
        /// Returns the URI found in the first href tag
        /// </summary>
        /// <param name="source">The string to search</param>
        /// <returns>The found URI, or null if no URI was found</returns>
        private static string SearchUri(string source)
        {
            int i = source.IndexOf("<a href");
            if (i != -1)
            {
                int start = source.Substring(i).IndexOf("\"") + i + 1;
                int end = source.Substring(start + 1).IndexOf("\"") + start + 1;
                return source.Substring(start, end - start);
            }

            int start2 = source.IndexOf("http://");
            if (start2 != -1)
            {
                int end1 = source.Substring(start2 + 1).IndexOf("\n");
                int end2 = source.Substring(start2 + 1).IndexOf(" ");
                int end3 = source.Length - 1;


                if (end1 == -1)
                    end1 = Int32.MaxValue;
                else
                    end1 += start2 + 1;
                if (end2 == -1)
                    end2 = Int32.MaxValue;
                else
                    end2 += start2 + 1;
                if (end3 == -1)
                    end3 = Int32.MaxValue;

                int compareend1 = (end1 < end2) ? end1 : end2;
                int compareend2 = (end3 < compareend1) ? end3 : compareend1;

                string uri = source.Substring(start2, compareend2 - start2);
                uri = uri.Replace(@"&amp;", @"&");
                uri = uri.Replace(@"&lt;", @"<");
                uri = uri.Replace(@"&gt;", @">");
                uri = uri.Replace(@"&apos;", @"'");
                uri = uri.Replace(@"&quot;", "\"");
                return uri;
            }

            return null;
        }

        /// <summary>
        /// Removes unused icons from a given layer
        /// </summary>
        /// <param name="layer">The layer to remove icons from</param>
        private static void RemoveUnusedIcons(Icons layer)
        {
            // Stores removed icons
            ArrayList VFD = new ArrayList();

            // Search for removed icons
            foreach (WorldWind.Renderable.RenderableObject ro in layer.ChildObjects)
            {
                KMLIcon KMLIcon = ro as KMLIcon;
                if ((KMLIcon != null) && (!KMLIcon.HasBeenUpdated))
                {
                    VFD.Add(ro);
                }
            }

            // Remove all icons that were found to be removed
            foreach (WorldWind.Renderable.RenderableObject ro in VFD)
            {
                layer.Remove(ro);
                ro.Dispose();
            }
        }

        /// <summary>
        /// Helper class. Represents a KML Style or StyleMap
        /// </summary>
        class Style
        {
            #region Public members
            public BalloonStyle BalloonStyle = null;
            public IconStyle IconStyle = null;
            public LabelStyle LabelStyle = null;
            public LineStyle LineStyle = null;
            public PolyStyle PolyStyle = null;

            internal string NormalIcon
            {
                get
                {
                    return normalIcon;
                }
                set
                {
                    normalIcon = value;
                }
            }

            internal double NormalScale
            {
                get
                {
                    return normalScale;
                }
                set
                {
                    normalScale = value;
                }
            }

            #endregion

            #region Private members
            private string normalIcon;
            private double normalScale = Double.NaN;
            #endregion

            /// <summary>
            /// Creates a new Style
            /// </summary>
            internal Style()
            {
                this.normalIcon = null;
            }

            /// <summary>
            /// Creates a new Style
            /// </summary>
            /// <param name="normalIcon">The normal Bitmap to use for this Style</param>
            internal Style(string normalIcon)
            {
                this.normalIcon = normalIcon;
            }

            /// <summary>
            /// Convert a hex string to a .NET Color object.
            /// </summary>
            /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
            public static Color HexToColor(string hexColor)
            {
                string hc = ExtractHexDigits(hexColor);
                if (hc.Length != 8)
                {
                    // you can choose whether to throw an exception
                    //throw new ArgumentException("hexColor is not exactly 6 digits.");
                    return Color.White;
                }
                string a = hc.Substring(0, 2);
                string r = hc.Substring(2, 2);
                string g = hc.Substring(4, 2);
                string b = hc.Substring(6, 2);
                Color color = Color.White;
                try
                {
                    int ai
                        = Int32.Parse(a, System.Globalization.NumberStyles.HexNumber);
                    int ri
                        = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                    int gi
                        = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                    int bi
                        = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                    color = Color.FromArgb(ri, gi, bi);
                }
                catch
                {
                    // you can choose whether to throw an exception
                    //throw new ArgumentException("Conversion failed.");
                    return Color.White;
                }
                return color;
            }
            /// <summary>
            /// Extract only the hex digits from a string.
            /// </summary>
            private static string ExtractHexDigits(string input)
            {
                // remove any characters that are not digits (like #)
                Regex isHexDigit
                    = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
                string newnum = "";
                foreach (char c in input)
                {
                    if (isHexDigit.IsMatch(c.ToString()))
                        newnum += c.ToString();
                }
                return newnum;
            }
        }

        /// <summary>
        /// LatLonAlt
        /// </summary>
        class LLA
        {
            internal float lat;
            internal float lon;
            internal float alt;

            /// <summary>
            /// Creates a new instance of LLA
            /// </summary>
            /// <param name="lat">Latitude</param>
            /// <param name="lon">Longitude</param>
            /// <param name="alt">Altitude</param>
            public LLA(float lat, float lon, float alt)
            {
                this.lat = lat;
                this.lon = lon;
                this.alt = alt;
            }
        }

        class BalloonStyle
        {
            public string id = null;
            public TextElement Text = null;
            public ColorElement TextColor = null;
            public ColorElement Color = null;
        }

        class IconStyle
        {
            public string id = null;
            public ColorElement Color = new ColorElement(System.Drawing.Color.White.ToArgb());
            public ColorMode ColorMode = ColorMode.Normal;
            public DecimalElement Heading = null;
            public DecimalElement Scale = new DecimalElement(1.0);
            public IconElement Icon = null;

            public IconStyle(IconElement icon)
            {
                Icon = icon;
            }
        }

        class LabelStyle
        {
            public string id = null;
            public ColorElement Color = new ColorElement(System.Drawing.Color.White.ToArgb());
            public ColorMode ColorMode = ColorMode.Normal;
            public DecimalElement Scale = new DecimalElement(1.0);
        }

        class LineStyle
        {
            public string id = null;
            public ColorElement Color = new ColorElement(System.Drawing.Color.Gray.ToArgb());
            public ColorMode ColorMode = ColorMode.Normal;
            public DecimalElement Width = new DecimalElement(1);
        }

        class PolyStyle
        {
            public string id = null;
            public ColorElement Color = new ColorElement(System.Drawing.Color.DarkGray.ToArgb());
            public ColorMode ColorMode = ColorMode.Normal;
            public bool Fill = true;
            public bool Outline = true;
        }

        class IconElement
        {
            public string href = null;
            public IntegerElement x = null;
            public IntegerElement y = null;
            public IntegerElement w = null;
            public IntegerElement h = null;
        }

        class TextElement
        {
            public string Text = null;
            public ColorElement TextColor = null;
        }

        class ColorElement
        {
            public int Color = System.Drawing.Color.Black.ToArgb();

            public ColorElement(int color)
            {
                Color = color;
            }
        }

        class IntegerElement
        {
            public int Value = 0;

            public IntegerElement(int v)
            {
                Value = v;
            }
        }

        class DecimalElement
        {
            public double Value = 0;

            public DecimalElement(double d)
            {
                Value = d;
            }
        }

        enum ColorMode
        {
            Normal,
            Random
        }

    }
}
