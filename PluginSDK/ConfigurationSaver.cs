using System;
using System.Collections.Generic;
using System.Text;
using WorldWind.Renderable;
using System.Xml;
using System.Globalization;

namespace WorldWind
{
    /// <summary>
    /// This class serializes the in-memory Renderable objects
    /// to their source XML parsed by the ConfigurationLoader
    /// for persistence
    /// </summary>
    class ConfigurationSaver
    {
        /// <summary>
        /// Save settings for a particular world
        /// </summary>
        /// <param name="world">World to be serialized</param>
        public static void Save(World world)
        {
            foreach(RenderableObject ro in world.RenderableObjects.ChildObjects)
            {
                XmlDocument worldDoc = new XmlDocument();
                worldDoc.AppendChild((worldDoc.CreateXmlDeclaration("1.0", "utf-8", null)));

                //Call appropriate serialization function
                if(ro is RenderableObjectList)
                    worldDoc.AppendChild(saveLayerSet((RenderableObjectList)ro, worldDoc));
            }
        }

        /// <summary>
        /// Save a subset of the LM to a file.
        /// </summary>
        /// <param name="ro">RenderableObjectList to save</param>
        /// <param name="file">Location for output</param>
        public static void SaveAs(RenderableObject ro, string file)
        {
            XmlDocument worldDoc = new XmlDocument();
            worldDoc.AppendChild((worldDoc.CreateXmlDeclaration("1.0", "utf-8", null)));

            if (ro is RenderableObjectList)
            {
                //worldDoc.AppendChild(saveLayer((RenderableObjectList)ro, worldDoc));
                worldDoc.AppendChild(ro.ToXml(worldDoc));
            }
            else
            {
                RenderableObjectList rol = new RenderableObjectList("Saved");
                rol.Add(ro);
                worldDoc.AppendChild(saveLayer(rol, worldDoc));
            }

            worldDoc.Save(file);
        }


        //TODO: Quadtiles Sets and indeed any layer may be sourced
        //from multiple XML files and merged at runtime. No track 
        //of the source XML's is kept. Merged XML's can be written
        //back to the original config load point(causing duplicates)
        //or to Application Data folder. Important design issues here
        //need to be decided on.]

        //TODO: Serialize "ExtendedInformation" nodes?  What is that for anyway?

        /// <summary>
        /// Serializes Renderable Object Lists to LayersSets at the top level
        /// </summary>
        /// <param name="layerSet">Layerset Renderable Object to be serialized</param>
        /// <param name="worldDoc">World Document to which node is added</param>
        /// <returns>Node for Root Layerset</returns>
        private static XmlNode saveLayerSet(RenderableObjectList layerSet,XmlDocument worldDoc)
        {
            XmlNode layerSetNode = worldDoc.CreateElement("LayerSet");
            
            // good default values?
            XmlAttribute name = worldDoc.CreateAttribute("Name");
            name.Value = "LayerSet";
            XmlAttribute showAtStartup = worldDoc.CreateAttribute("ShowAtStartup");
            showAtStartup.Value = "true";
            XmlAttribute showOnlyOneLayer = worldDoc.CreateAttribute("ShowOnlyOneLayer");
            showOnlyOneLayer.Value = "true";
            XmlAttribute xsi = worldDoc.CreateAttribute("xmlns:xsi");
            xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
            XmlAttribute xsi2 = worldDoc.CreateAttribute("xsi:noNamespaceSchemaLocation");
            xsi2.Value = "LayerSet.xsd";

            layerSetNode.Attributes.Append(name);
            layerSetNode.Attributes.Append(showAtStartup);
            layerSetNode.Attributes.Append(showOnlyOneLayer);
            layerSetNode.Attributes.Append(xsi);
            layerSetNode.Attributes.Append(xsi2);

            foreach (RenderableObject ro in layerSet.ChildObjects)
            {
                layerSetNode.AppendChild(ro.ToXml(worldDoc));
            }
            return layerSetNode;
        }

        /// <summary>
        /// Serializes Renderable Objects that aren't from the top level
        /// </summary>
        /// <param name="rol">RenderableObjectList to be serialized</param>
        /// <param name="worldDoc">World Document to which node is added</param>
        /// <returns>Node for Root Layerset</returns>
        private static XmlNode saveLayer(RenderableObjectList rol, XmlDocument worldDoc)
        {
            XmlNode layerSetNode = worldDoc.CreateElement("LayerSet");

            XmlAttribute name = worldDoc.CreateAttribute("Name");
            name.Value = rol.Name;
            XmlAttribute showAtStartup = worldDoc.CreateAttribute("ShowAtStartup");
            showAtStartup.Value = rol.IsOn.ToString();
            XmlAttribute showOnlyOneLayer = worldDoc.CreateAttribute("ShowOnlyOneLayer");
            showOnlyOneLayer.Value = rol.ShowOnlyOneLayer.ToString();
            XmlAttribute xsi = worldDoc.CreateAttribute("xmlns:xsi");
            xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
            XmlAttribute xsi2 = worldDoc.CreateAttribute("xsi:noNamespaceSchemaLocation");
            xsi2.Value = "LayerSet.xsd";

            layerSetNode.Attributes.Append(name);
            layerSetNode.Attributes.Append(showAtStartup);
            layerSetNode.Attributes.Append(showOnlyOneLayer);
            layerSetNode.Attributes.Append(xsi);
            layerSetNode.Attributes.Append(xsi2);

            foreach (RenderableObject ro in rol.ChildObjects)
            {
                layerSetNode.AppendChild(ro.ToXml(worldDoc));
            }

            
            return layerSetNode;
        }

        /// <summary>
        /// Gets the xml for properties in the base RO class.
        /// </summary>
        /// <param name="ro">The RenderableObject to parse</param>
        /// <param name="roNode">the XmlNode for the object</param>
        public static void getRenderableObjectProperties(RenderableObject ro, XmlNode roNode)
        {
            XmlDocument worldDoc = roNode.OwnerDocument;

            // TODO: what about Thumbnail, ThumbnailImage, IconImagePath, etc.?
            // do those even get used for anything?

            XmlNode nameNode = worldDoc.CreateElement("Name");
            nameNode.AppendChild(worldDoc.CreateTextNode(ro.Name));
            roNode.AppendChild(nameNode);

            XmlNode descNode = worldDoc.CreateElement("Description");
            descNode.AppendChild(worldDoc.CreateTextNode(ro.Description));
            roNode.AppendChild(descNode);

            XmlNode opacityNode = worldDoc.CreateElement("Opacity");
            opacityNode.AppendChild(worldDoc.CreateTextNode(ro.Opacity.ToString(CultureInfo.InvariantCulture)));
            roNode.AppendChild(nameNode);

            XmlNode renderPriorityNode = worldDoc.CreateElement("RenderPriority");
            renderPriorityNode.AppendChild(worldDoc.CreateTextNode(ro.RenderPriority.ToString()));
            roNode.AppendChild(renderPriorityNode);

            // Attributes
            XmlAttribute isOnAttribute = worldDoc.CreateAttribute("ShowAtStartup");
            isOnAttribute.Value = ro.IsOn.ToString(CultureInfo.InvariantCulture);
            roNode.Attributes.Append(isOnAttribute);

            XmlAttribute infoUriAttribute = worldDoc.CreateAttribute("InfoUri");
            infoUriAttribute.Value = (string)ro.MetaData["InfoUri"];
            roNode.Attributes.Append(infoUriAttribute);

        }

        public static string createPointList(Point3d[] point3d)
        {
            string posList = "";

            for (int i = 0; i < point3d.Length; i++)
            {
                Point3d p = point3d[i];
                posList += p.X.ToString(CultureInfo.InvariantCulture) + "," + p.Y.ToString(CultureInfo.InvariantCulture) + "," + p.Z.ToString(CultureInfo.InvariantCulture) + " ";
            }

            return posList;
        }

        public static void createColorNode(XmlNode colorNode, System.Drawing.Color color)
        {
            XmlDocument worldDoc = colorNode.OwnerDocument;

            XmlNode redNode = worldDoc.CreateElement("Red");
            XmlNode blueNode = worldDoc.CreateElement("Blue");
            XmlNode greenNode = worldDoc.CreateElement("Green");

            redNode.AppendChild(worldDoc.CreateTextNode(color.R.ToString(CultureInfo.InvariantCulture)));
            blueNode.AppendChild(worldDoc.CreateTextNode(color.B.ToString(CultureInfo.InvariantCulture)));
            greenNode.AppendChild(worldDoc.CreateTextNode(color.G.ToString(CultureInfo.InvariantCulture)));

            //TODO: can opacity be set from the color xml?

            colorNode.AppendChild(redNode);
            colorNode.AppendChild(blueNode);
            colorNode.AppendChild(greenNode);
        }

    }
}
