//----------------------------------------------------------------------------
// NAME: Flickr Plugin
// VERSION: 0.1
// DESCRIPTION: Imports Georeferenced Flickr Images 
// DEVELOPER: Tisham Dhar aka "What_Nick"
// WEBSITE: http://whatnick.blogspot.com
// REFERENCES:  FlickrNET
//----------------------------------------------------------------------------
//
// 
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

//Windows Forms
using System.Windows.Forms;

//Flickr API
using FlickrNet;

//Worldwind Imports
using WorldWind;
using WorldWind.Renderable;
using Microsoft.DirectX;

namespace YahooFlickr.Plugins
{
    #region Windows GUI
    
    class FlickrGUI : Form
    {
        private Button searchButton;
        private DateTimePicker dateTimePicker1;
        private TextBox keyBox;

        public KeySetter PassKeywords;

        public FlickrGUI()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.keyBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // keyBox
            // 
            this.keyBox.Location = new System.Drawing.Point(12, 10);
            this.keyBox.Name = "keyBox";
            this.keyBox.Size = new System.Drawing.Size(225, 20);
            this.keyBox.TabIndex = 0;
            this.keyBox.Text = "(Keywords)";
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(260, 8);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(70, 28);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(12, 51);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(224, 20);
            this.dateTimePicker1.TabIndex = 2;
            // 
            // FlickrGUI
            // 
            this.ClientSize = new System.Drawing.Size(340, 93);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.keyBox);
            this.Name = "FlickrGUI";
            this.Text = "Flickr Preferences";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// Button Press callback set the flickr plugin layer keyword
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchButton_Click(object sender, EventArgs e)
        {
            if (!(keyBox.Text.Equals("(KeyWord)") || keyBox.Text.Trim().Equals("")))
            {
                this.PassKeywords(keyBox.Text);
            }
        }
    }

    #endregion

    #region Delegates

    public delegate void KeySetter(string text);

    #endregion

    #region Plugin Class
    /// <summary>
    /// Main Plugin class - maintains a GUI and FlickrLayer
    /// </summary> 
    public class FlickrPlugin: WorldWind.PluginEngine.Plugin
    {
        
        private FlickrIconsLayer layer;
        private System.Drawing.Bitmap Image;

        public override void Load()
        {
            if (ParentApplication.WorldWindow.CurrentWorld.IsEarth)
            {
                layer = new FlickrIconsLayer();
                ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Add(layer);
            }
            base.Load();
        }

        public override void Unload()
        {
            if (ParentApplication.WorldWindow.CurrentWorld.IsEarth && layer != null)
                ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Remove(layer);
            if (layer != null)
            {
                layer.Dispose();
                layer = null;
            }
            
            if(Image != null)
            {
                Image.Dispose();
                Image = null;
            }
            base.Unload();
        }

    }

    #endregion

    #region Renderable Flickr Layer
    /// <summary>
    /// Flickr Icons Display Layer. Shows Georeferenced Images
    /// </summary>
    class FlickrIconsLayer : WorldWind.Renderable.Icons
    {
        private static string apikey = @"ce45f112b3cd6e5e08e6f1680a5bd73a";
        private Flickr flickr;
        private PhotoSearchOptions searchOptions;
        private System.Drawing.Bitmap Image;
        private WorldWind.Angle lastUpdatelon,lastUpdatelat;
        private double lastUpdatealt;
        private string m_cachedir;
        private bool m_needsupdate;
        private double m_maxdistance;


        #region Properties
        public void KeyWords(string value)
        {
            if (searchOptions != null)
            {
                searchOptions.Tags = value;
                m_needsupdate = false;
                this.IsOn = false;
                try
                {
                    //Need lock on the folder to delete
                    lock(this)
                        if(Directory.Exists(m_cachedir)) Directory.Delete(m_cachedir,true);
                }
                finally
                {
                    m_needsupdate = true;
                }
                this.IsOn = true;
            }
                
        }
        #endregion
        /// <summary>
        /// Construct supporting Flickr objects
        /// </summary>
        public FlickrIconsLayer():base("Flickr Photos")
        {
            this.flickr = new Flickr(apikey);

            searchOptions = new PhotoSearchOptions();
            searchOptions.Extras |= PhotoSearchExtras.Geo;
            searchOptions.PerPage = 10;
            searchOptions.Page = 1;
            searchOptions.Tags = "";

            m_cachedir =
                 Directory.GetParent(Application.ExecutablePath) +
                 "\\Cache\\Earth\\Flickr\\";
            m_maxdistance = 100000.0;
        }

        /// <summary>
        /// Initialize graphics objects
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Initialize(WorldWind.DrawArgs drawArgs)
        {
            base.Initialize(drawArgs);
            lastUpdatelat = lastUpdatelon = WorldWind.Angle.Zero;
            lastUpdatealt = 0.0;
            m_needsupdate = true;
            Image = new System.Drawing.Bitmap(Directory.GetParent(Application.ExecutablePath)+"\\Plugins\\Flickr\\flickr.ico");
        }

        /// <summary>
        /// Dispose Graphics objects
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (Image != null)
                Image.Dispose();
        }

        /// <summary>
        /// This method updates the Icons to reflect only those in view
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Update(WorldWind.DrawArgs drawArgs)
        {
            base.Update(drawArgs);

            double distance = WorldWind.World.ApproxAngularDistance(drawArgs.WorldCamera.Latitude,
            drawArgs.WorldCamera.Longitude, this.lastUpdatelat, this.lastUpdatelon).Degrees;
            double altchange = Math.Abs(this.lastUpdatealt - drawArgs.WorldCamera.Altitude);


            if (distance > 10 || altchange < 10000.0 || m_needsupdate)
            {
                this.lastUpdatelon = drawArgs.WorldCamera.Longitude;
                this.lastUpdatelat = drawArgs.WorldCamera.Latitude;
                this.lastUpdatealt = drawArgs.WorldCamera.Altitude;
                this.m_needsupdate = false;

                double viewNorth = drawArgs.WorldCamera.Latitude.Degrees + drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;
                double viewSouth = drawArgs.WorldCamera.Latitude.Degrees - drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;
                double viewWest = drawArgs.WorldCamera.Longitude.Degrees - drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;
                double viewEast = drawArgs.WorldCamera.Longitude.Degrees + drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;



                //Need lock on the folder
                lock (this)
                {
                    //TODO: Implement temporary caching and redownload on request
                    PhotoCollection allPhotos = GetPhotoCollection(viewWest, viewSouth,
                        viewEast, viewNorth);

                    this.RemoveAll();
                    foreach (Photo photo in allPhotos)
                    {
                        double photolat =  Convert.ToDouble(photo.Latitude);
                        double photolon = Convert.ToDouble(photo.Longitude);
                        WorldWind.Renderable.Icon ic = new WorldWind.Renderable.Icon(photo.Title,photolat
                            ,photolon);
                        ic.Image = Image;
                        ic.Width = 16;
                        ic.Height = 16;
                        //ic.MinimumDisplayDistance = 100000;
                        //WorldWind.Renderable.ScreenOverlay overlay 
                        //= new WorldWind.Renderable.ScreenOverlay(ic.Name,0.0f,0.0f,photo.ThumbnailUrl);
                        //ic.AddOverlay(overlay);
                        ic.isSelectable = true;
                        ic.Description = "<img src=\""+photo.ThumbnailUrl+"\"/><br>"+
                            photo.License+"<br>"+
                            photo.DateTaken;
                        double distanceToIcon = Vector3.Length(ic.Position - drawArgs.WorldCamera.Position);

                        //If Camera is far render as icon
                        //if(ic.MinimumDisplayDistance < distanceToIcon)
                        this.Add(ic);
                        //if Camera is near render as textured polygon
                        /*
                        else
                        {
                            Point3d[] lineends = new Point3d[2];
                            lineends[0].X = photolon - 0.1; 
                            lineends[0].Y = photolat - 0.1; 
                            lineends[0].Z = 1000;
                            lineends[1].X = photolon + 0.1; 
                            lineends[1].Y = photolat + 0.1;
                            lineends[1].Z = 1000;

                            WorldWind.LineFeature line = new LineFeature(photo.Title,
                                m_world,lineends,photo.ThumbnailUrl);
                            line.AltitudeMode = AltitudeMode.RelativeToGround;
                            line.DistanceAboveSurface = 0;
                            line.Extrude = true;

                            this.Add(line);
                        }
                        */
                        
                        //Console.WriteLine("Photos title is " + photo.Title + photo.Latitude + photo.Longitude);
                    }

                }
            }
        }


        /// <summary>
        /// This method takes a bounding box and returns
        /// Photocollection covering the bounding box from
        /// cache or from downloaded data
        /// </summary>
        /// <param name="west"></param>
        /// <param name="south"></param>
        /// <param name="east"></param>
        /// <param name="north"></param>
        /// <returns></returns>
        private PhotoCollection GetPhotoCollection(double west,double south
            ,double east,double north)
        {
            PhotoCollection collection = new PhotoCollection();
            double tileSize = 10.0;
            /*
            //base the tile size on the max viewing distance
            double maxDistance = m_maxdistance;// Math.Sqrt(m_maximumDistanceSq);

            double factor = maxDistance / m_world.EquatorialRadius;
            // True view range 
            if (factor < 1)
                tileSize = Angle.FromRadians(Math.Abs(Math.Asin(maxDistance / m_world.EquatorialRadius)) * 2).Degrees;
            else
                tileSize = Angle.FromRadians(Math.PI).Degrees;

            tileSize = (180 / (int)(180 / tileSize));

            if (tileSize == 0)
                tileSize = 0.1;
             */
            //Log.Write(Log.Levels.Debug, string.Format("TS: {0} -> {1}", name, tileSize));
            //not working for some reason...
            //int startRow = MathEngine.GetRowFromLatitude(south, tileSize);
            //int endRow = MathEngine.GetRowFromLatitude(north, tileSize);
            //int startCol = MathEngine.GetColFromLongitude(west, tileSize);
            //int endCol = MathEngine.GetColFromLongitude(east, tileSize);

            double currentSouth = -90;
            //for (int row = 0; row <= endRow; row++)
            XmlSerializer serializer = new XmlSerializer(typeof(PhotoCollection));

            while (currentSouth < 90)
            {
                double currentNorth = currentSouth + tileSize;
                if (currentSouth > north || currentNorth < south)
                {
                    currentSouth += tileSize;
                    continue;
                }

                double currentWest = -180;
                while (currentWest < 180)
                //    for (int col = startCol; col <= endCol; col++)
                {
                    double currentEast = currentWest + tileSize;
                    if (currentWest > east || currentEast < west)
                    {
                        currentWest += tileSize;
                        continue;
                    }

                   

                    if (!Directory.Exists(m_cachedir))
                        Directory.CreateDirectory(m_cachedir);

                    string collectionFilename = m_cachedir
                        + currentEast + "_"
                        + currentWest + "_"
                        + currentNorth + "_"
                        + currentSouth +".xml" ;
                    PhotoCollection currentPhotoCollection;
                    if (File.Exists(collectionFilename))
                        currentPhotoCollection = (PhotoCollection)serializer.Deserialize(
                            new FileStream(collectionFilename,FileMode.Open));
                    else
                    {
                        searchOptions.BoundaryBox = new BoundaryBox(currentWest, currentSouth,
                            currentEast, currentNorth);
                        Photos photos = flickr.PhotosSearch(searchOptions);
                        currentPhotoCollection = photos.PhotoCollection;
                        serializer.Serialize(new FileStream(
                            collectionFilename, FileMode.Create),currentPhotoCollection);
                    }

                    collection.AddRange(currentPhotoCollection);

                    currentWest += tileSize;
                }
                currentSouth += tileSize;
            }
            return collection;
        }

        public override void BuildContextMenu(ContextMenu menu)
        {
            menu.MenuItems.Add("Properties", OnPropertiesClick);
        }

        
        protected override void OnPropertiesClick(object sender, EventArgs e)
        {
            if (m_propertyBrowser != null)
                m_propertyBrowser.Dispose();

            m_propertyBrowser = new FlickrGUI();
            ((FlickrGUI)m_propertyBrowser).PassKeywords = this.KeyWords;
            m_propertyBrowser.Show();

        }
        
    }
    #endregion
}
