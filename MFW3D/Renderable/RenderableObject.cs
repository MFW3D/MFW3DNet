using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.DirectX;
using MFW3D.Menu;
using MFW3D.VisualControl;

namespace MFW3D.Renderable
{
    /// <summary>
    /// 可渲染对象.
    /// </summary>
    public abstract class RenderableObject : IRenderable, IComparable
    {
        /// <summary>
        /// 对象是否可以渲染
        /// </summary>
        public bool isInitialized;

        /// <summary>
        /// 对象是否可以被选择
        /// </summary>
        public bool isSelectable;

        public RenderableObjectList ParentList;

        public string dbfPath = "";
        public bool dbfIsInZip = false;


        protected string name;
        protected string m_description = null;
        protected Hashtable _metaData = new Hashtable();
        protected Vector3 position;
        protected Quaternion orientation;
        protected bool isOn = true;
        protected byte m_opacity = 255;
        private RenderPriority m_renderPriority = RenderPriority.SurfaceImages;
        protected Form m_propertyBrowser;

        protected Image m_thumbnailImage;
        protected string m_iconImagePath;
        protected Image m_iconImage;
        protected World m_world;
        string m_thumbnail;

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.RenderableObject"/> class.
        /// </summary>
        /// <param name="name">Object description</param>
        protected RenderableObject(string name)
        {
            this.name = name;
        }

        protected RenderableObject(string name, World parentWorld)
        {
            this.name = name;
            this.m_world = parentWorld;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Object description</param>
        /// <param name="position">位置信息，世界坐标</param>
        /// <param name="orientation">朝向</param>
        protected RenderableObject(string name, Vector3 position, Quaternion orientation)
        {
            this.name = name;
            this.position = position;
            this.orientation = orientation;
        }

        public abstract void Initialize(DrawArgs drawArgs);

        public abstract void Update(DrawArgs drawArgs);

        public abstract void Render(DrawArgs drawArgs);

        public virtual XmlNode ToXml(XmlDocument worldDoc)
        {
            return worldDoc.CreateComment("ERROR: RenderableObject \"" + Name + "\" does not implement the ToXML method and can not be serialized.");
        }

        public virtual bool Initialized
        {
            get
            {
                return isInitialized;
            }
        }

        /// <summary>
        /// The planet this layer is a part of.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual World World
        {
            get
            {
                return m_world;
            }
        }

        /// <summary>
        /// Path to a Thumbnail image(e.g. for use as a Toolbar button).
        /// </summary>
        public virtual string Thumbnail
        {
            get
            {
                return m_thumbnail;
            }
            set
            {
                m_thumbnail = ImageHelper.FindResource(value);
            }
        }

        /// <summary>
        /// The image referenced by Thumbnail. 
        /// </summary>
        public virtual Image ThumbnailImage
        {
            get
            {
                if (m_thumbnailImage == null)
                {
                    if (m_thumbnail == null)
                        return null;
                    try
                    {
                        if (File.Exists(m_thumbnail))
                            m_thumbnailImage = ImageHelper.LoadImage(m_thumbnail);
                    }
                    catch { }
                }
                return m_thumbnailImage;
            }
        }

        /// <summary>
        /// Path for an icon for the object, such as an image to be used in the Active Layer window.
        /// This can be different than the Thumbnail(e.g. an ImageLayer can have an IconImage, and no Thumbnail).
        /// </summary>
        public string IconImagePath
        {
            get
            {
                return m_iconImagePath;
            }
            set
            {
                m_iconImagePath = value;
            }
        }

        /// <summary>
        /// The icon image referenced by IconImagePath. 
        /// </summary>
        public Image IconImage
        {
            get
            {
                if (m_iconImage == null)
                {
                    if (m_iconImagePath == null)
                        return null;
                    try
                    {
                        if (File.Exists(m_iconImagePath))
                            m_iconImage = ImageHelper.LoadImage(m_iconImagePath);
                    }
                    catch { }
                }
                return m_iconImage;
            }
        }

        /// <summary>
        /// Called when object is disabled.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// User interaction (mouse click)
        /// </summary>
        public abstract bool PerformSelectionAction(DrawArgs drawArgs);

        public int CompareTo(object obj)
        {
            RenderableObject robj = obj as RenderableObject;
            if (obj == null)
                return 1;

            return this.m_renderPriority.CompareTo(robj.RenderPriority);
        }

        /// <summary>
        /// Permanently delete the layer
        /// </summary>
        public virtual void Delete()
        {

            RenderableObjectList list = this.ParentList;
            string xmlConfigFile = (string)this.MetaData["XmlSource"];
            if (this.ParentList.Name == "Earth" & xmlConfigFile != null)
            {
                string message = "Permanently delete layer '" + this.Name + "' and rename its .xml config file to .bak?";
                if (DialogResult.Yes != MessageBox.Show(message, "Delete layer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2))
                    return;
                if (xmlConfigFile.Contains("http"))
                    throw new Exception("Can't delete network layers.");
                if (File.Exists(xmlConfigFile.Replace(".xml", ".bak")))
                {
                    File.Delete(xmlConfigFile.Replace(".xml", ".bak"));
                }
                File.Move(xmlConfigFile, xmlConfigFile.Replace(".xml", ".bak"));
                this.ParentList.Remove(this);
            }
            else if (xmlConfigFile == null)
            {
                string message = "Delete plugin layer '" + this.Name + "'?\n\nThis may cause problems for a running plugin that expects the layer to be\nthere.  Restart the plugin in question to replace the layer after deleting.";
                if (DialogResult.Yes != MessageBox.Show(message, "Delete layer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2))
                    return;
                this.ParentList.Remove(this);
            }
            else
            {
                throw new Exception("Can't delete this sub-item from the layer manager.  Try deleting the top-level entry for this layer.");
            }


        }

        /// <summary>
        /// Returns a String that represents the current SelectedObject.
        /// </summary>
        public override string ToString()
        {
            return name;
        }

        #region 属性

        /// <summary>
        /// The object's render priority determining in what order it will be rendered
        /// compared to the other objects.
        /// </summary>
        [Description("The object's render priority determining in what order it will be rendered compared to the other objects.")]
        public virtual RenderPriority RenderPriority
        {
            get
            {
                return this.m_renderPriority;
            }
            set
            {
                this.m_renderPriority = value;
                //if(ParentList != null)
                //    ParentList.NeedsSort = true;
            }
        }

        /// <summary>
        /// How transparent this object should appear (0=invisible, 255=opaque)
        /// </summary>
        [Description("Controls the amount of light allowed to pass through this object. (0=invisible, 255=opaque).")]
        public virtual byte Opacity
        {
            get
            {
                return this.m_opacity;
            }
            set
            {
                this.m_opacity = value;
                if (value == 0)
                {
                    // invisible - turn off
                    if (this.isOn)
                        this.IsOn = false;
                }
                else
                {
                    // visible - turn back on
                    if (!this.isOn)
                        this.IsOn = true;
                }
            }
        }

        [Browsable(false)]
        public virtual Hashtable MetaData
        {
            get
            {
                return this._metaData;
            }
        }

        /// <summary>
        /// Hide/Show this object.
        /// </summary>
        [Description("This layer's enabled status.")]
        public virtual bool IsOn
        {
            get
            {
                return this.isOn;
            }
            set
            {
                // handled in Update() to avoid race conditions with background worker thread.
                // -step
                /*
				if(isOn && !value)
					this.Dispose();
                 */
                this.isOn = value;
            }
        }

        /// <summary>
        /// Describes this object
        /// </summary>
        [Description("This layer's name.")]
        public virtual string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Object position (XYZ world coordinates)
        /// </summary>
        [Browsable(false)]
        public virtual Vector3 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// Object rotation (Quaternion)
        /// </summary>
        [Browsable(false)]
        public virtual Quaternion Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                this.orientation = value;
            }
        }

        #endregion

        #region 功能选项操作

        ///<summary>
        ///  Goes to the Shapefiles's DBF Information Window
        /// </summary>
        public  void GetDbfInfo()
        {
            ShapeFileInfoDlg sfid = new ShapeFileInfoDlg(dbfPath, dbfIsInZip);
            sfid.Show();
        }

        ///<summary>
        ///  Goes to the extent specified by the bounding box for the QTS layer
        ///  or to the lat/lon for icons
        /// </summary>
        public  void GetGoto()
        {
            lock (this.ParentList.ChildObjects.SyncRoot)
            {
                for (int i = 0; i < this.ParentList.ChildObjects.Count; i++)
                {
                    RenderableObject ro = (RenderableObject)this.ParentList.ChildObjects[i];
                    if (ro.Name.Equals(name))
                    {
                        if (ro is QuadTileSet)
                        {
                            QuadTileSet qts = (QuadTileSet)ro;
                            DrawArgs.Camera.SetPosition((qts.North + qts.South) / 2, (qts.East + qts.West) / 2);
                            double perpendicularViewRange = (qts.North - qts.South > qts.East - qts.West ? qts.North - qts.South : qts.East - qts.West);
                            double altitude = qts.LayerRadius * Math.Sin(MathEngine.DegreesToRadians(perpendicularViewRange * 0.5));

                            DrawArgs.Camera.Altitude = altitude;

                            break;
                        }
                        if (ro is Icon)
                        {
                            Icon ico = (Icon)ro;
                            DrawArgs.Camera.SetPosition(ico.Latitude, ico.Longitude);
                            DrawArgs.Camera.Altitude /= 2;

                            break;
                        }
                        if (ro is ShapeFileLayer)
                        {
                            ShapeFileLayer slayer = (ShapeFileLayer)ro;
                            DrawArgs.Camera.SetPosition((slayer.North + slayer.South) / 2, (slayer.East + slayer.West) / 2);
                            double perpendicularViewRange = (slayer.North - slayer.South > slayer.East - slayer.West ? slayer.North - slayer.South : slayer.East - slayer.West);
                            double altitude = slayer.MaxAltitude;

                            DrawArgs.Camera.Altitude = altitude;

                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Layer info context menu item
        /// </summary>
        protected virtual void GetInfo()
        {
            LayerManagerItemInfo lmii = new LayerManagerItemInfo(MetaData);
            lmii.ShowDialog();
        }

        /// <summary>
        /// Layer properties context menu item
        /// </summary>
        protected virtual void GetProperties()
        {
            if (m_propertyBrowser != null)
                m_propertyBrowser.Dispose();

            m_propertyBrowser = new PropertyBrowser(this);
            m_propertyBrowser.Show();
        }

        /// <summary>
        /// Layer properties context menu item
        /// </summary>
        protected virtual void GetReloadShader()
        {
            QuadTileSet qts = this as QuadTileSet;
            if (qts != null)
            {
                qts.Effect = null;
            }
        }

        /// <summary>
        /// Delete layer context menu item
        /// </summary>
        protected virtual void GetDelete()
        {
            //World w = this.World;

            /*if (this.ParentList.Name != "Earth")
			{
				MessageBox.Show("Can't delete sub-items from layers.  Try deleting the top-level layer.", "Error deleting layer");
				return;
			}*/

            //MessageBox.Show("Delete click fired");


            try
            {
                this.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Layer Delete");
            }

        }

        /// <summary>
        /// Save item from LM to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void GetSaveAs()
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.DefaultExt = "xml";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ConfigurationSaver.SaveAs(this, fd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Save as...");
                }
            }
        }

        /// <summary>
        /// Show serialized item in browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void GetViewXml()
        {

            ConfigurationSaver.SaveAs(this, "temp.xml");

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "temp.xml";
            psi.Verb = "open";
            psi.UseShellExecute = true;

            psi.CreateNoWindow = true;
            System.Diagnostics.Process.Start(psi);
        }

        #endregion

    }

    /// <summary>
    /// 渲染顺序
    /// </summary>
    public enum RenderPriority
    {
        SurfaceImages = 0,
        TerrainMappedImages = 100,
        AtmosphericImages = 200,
        LinePaths = 300,
        Icons = 400,
        Placenames = 500,
        Custom = 600
    }
}
