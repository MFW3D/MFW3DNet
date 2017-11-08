using WorldWind.Renderable;
using System;
using System.Globalization;
using System.Xml;

namespace WorldWind
{
	/// <summary>
	/// 以 NLT-style存储的urls格式
	/// </summary>
    public class NltImageStore : ImageStore
    {
        #region Private Members

        string m_dataSetName;
        string m_serverUri;
        string m_formatString;

        #endregion

        public override bool IsDownloadableLayer
        {
            get
            {
                return true;
            }
        }

        public NltImageStore(
            string dataSetName,
            string serverUri)
        {
            m_serverUri = serverUri;
            m_dataSetName = dataSetName;
            m_formatString = "{0}?T={1}&L={2}&X={3}&Y={4}";
        }

        public NltImageStore(
            string dataSetName,
            string serverUri,
            string formatString)
        {
            m_serverUri = serverUri;
            m_dataSetName = dataSetName;
            m_formatString = formatString;
        }

        protected override string GetDownloadUrl(QuadTile qt)
        {
            return string.Format(CultureInfo.InvariantCulture,
                m_formatString, m_serverUri,
                m_dataSetName, qt.Level, qt.Col, qt.Row,
                qt.West, qt.South, qt.East, qt.North);
        }

        public override XmlNode ToXml(XmlDocument worldDoc)
        {
            XmlNode iaNode = base.ToXml(worldDoc);


            XmlNode tileServiceNode = worldDoc.CreateElement("ImageTileService");

            XmlNode requestFormatNode = worldDoc.CreateElement("RequestFormat");
            requestFormatNode.AppendChild(worldDoc.CreateTextNode(m_formatString));
            tileServiceNode.AppendChild(requestFormatNode);

            XmlNode serverUriNode = worldDoc.CreateElement("ServerUrl");
            serverUriNode.AppendChild(worldDoc.CreateTextNode(m_serverUri));
            tileServiceNode.AppendChild(serverUriNode);

            XmlNode dataSetNameNode = worldDoc.CreateElement("DataSetName");
            dataSetNameNode.AppendChild(worldDoc.CreateTextNode(m_dataSetName));
            tileServiceNode.AppendChild(dataSetNameNode);

            // TODO: Find a way to handle <CacheDirectory> correctly like in
            //       zoomit.xml.  For now it can only use the default cache 
            //       location for the QTS.
            //XmlNode cacheDirNode = worldDoc.CreateElement("CacheDirectory");
            //cacheDirNode.AppendChild(worldDoc.CreateTextNode(m_cacheDirectory));
            //tileServiceNode.AppendChild(cacheDirNode);

            XmlNode serverLogoNode = worldDoc.CreateElement("ServerLogoFilePath");
            serverLogoNode.AppendChild(worldDoc.CreateTextNode(m_serverlogo));
            tileServiceNode.AppendChild(serverLogoNode);            
            
            iaNode.AppendChild(tileServiceNode);

            return iaNode;
        }
    }
}
