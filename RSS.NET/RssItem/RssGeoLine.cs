using System;
using System.Collections.Generic;
using System.Text;

namespace Rss
{
    /// <summary>
    /// The Geospatial Line associated with this item
    /// </summary>
    public class RssGeoLine
    {
        /// <summary>
        /// The points in this line
        /// </summary>
        public List<RssGeoPoint> Points
        {
            get { return m_points; }
            set { if (m_points != null) m_points = value; }
        }
        List<RssGeoPoint> m_points = new List<RssGeoPoint>();

        /// <summary>
        /// Whether this is a valid geo point
        /// </summary>
        public bool IsValid
        {
            get 
            {
                bool isValid = true;
                foreach (RssGeoPoint point in m_points)
                {
                    if (!point.IsValid)
                        isValid = false;
                }
                return isValid; 
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RssGeoLine() { }

    }
}
