using System;
using System.Collections.Generic;
using System.Text;

namespace Rss
{
    /// <summary>
    /// A geospatial point for an item
    /// </summary>
    public class RssGeoPoint : RssElement
    {
        /// <summary>
        /// The Latitude of this geo point
        /// </summary>
        public double Lat
        {
            get { return m_lat; }
            set 
            {
                if ((value >= -90f) && (value <= 90f))
                {
                    m_lat = value;
                }
            }
        }
        double m_lat = float.NaN;

        /// <summary>
        /// The longitude of this geo point
        /// </summary>
        public double Lon
        {
            get { return m_lon; }
            set 
            {
                if ((value >= -180f) && (value <= 180f))
                {
                    m_lon = value;
                }
            }
        }
        double m_lon = float.NaN;

        /// <summary>
        /// The altitude for this point
        /// </summary>
        public double Alt
        {
            get { return m_alt; }
            set { m_alt = value; }
        }
        double m_alt = 0;

        /// <summary>
        /// The radius around this point for this item
        /// </summary>
        public double Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
        double m_radius = 0;

        /// <summary>
        /// Whether this is a valid geo point
        /// </summary>
        public bool IsValid
        {
            get { return (!m_lat.Equals(float.NaN) & !m_lon.Equals(float.NaN)); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RssGeoPoint() {}

    }
}
