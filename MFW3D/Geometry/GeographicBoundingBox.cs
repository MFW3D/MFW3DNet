using System;

namespace MFW3D
{
	/// <summary>
	/// Summary description for GeographicBoundingBox.
	/// </summary>
	public class GeographicBoundingBox
	{
		public double North;
		public double South;
		public double West;
		public double East;
		public double MinimumAltitude;
		public double MaximumAltitude;

		public GeographicBoundingBox()
		{
			North = 90;
			South = -90;
			West = -180;
			East = 180;
		}

		public GeographicBoundingBox(double north, double south, double west, double east)
		{
			North = north;
			South = south;
			West = west;
			East = east;
		}

		public GeographicBoundingBox(double north, double south, double west, double east, double minAltitude, double maxAltitude)
		{
			North = north;
			South = south;
			West = west;
			East = east;
			MinimumAltitude = minAltitude;
			MaximumAltitude = maxAltitude;
		}

		public bool Intersects(GeographicBoundingBox boundingBox)
		{
			if(North <= boundingBox.South ||
				South >= boundingBox.North ||
				West >= boundingBox.East ||
				East <= boundingBox.West)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

        public bool Contains(Point3d p)
        {
            if (North < p.Y ||
                South > p.Y ||
                West > p.X ||
                East < p.X ||
                MaximumAltitude < p.Z ||
                MinimumAltitude > p.Z)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
	}
}
