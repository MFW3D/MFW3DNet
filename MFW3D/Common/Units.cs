using System;
using System.Collections.Generic;
using System.Text;

namespace MFW3D
{
    public enum Units
    {
        English,
        Metric
    }

    public static class ConvertUnits
    {
        public static string GetDisplayString(double distance)
        {
            if (World.Settings.DisplayUnits == Units.Metric)
            {
                if (distance >= 1000)
                {
                    return string.Format("{0:,.0} km", distance / 1000);
                }
                else
                {
                    return string.Format("{0:f0} m", distance);
                }
            }
            else
            {
                double feetPerMeter = 3.2808399;
                double feetPerMile = 5280;

                distance *= feetPerMeter;

                if (distance >= feetPerMile)
                {
                    return string.Format("{0:,.0} miles", distance / feetPerMile);
                }
                else
                {
                    return string.Format("{0:f0} ft", distance);
                }
            }
        }

        public static string GetAreaDisplayString(double area)
        {
            if (World.Settings.DisplayUnits == Units.Metric)
            {
                if (area >= 1E6)
                {
                    return string.Format("{0:,.0} sq.km.", area / 1E6);
                }
                else
                {
                    return string.Format("{0:f0} sq.m", area);
                }
            }
            else
            {
                double sqfeetPerMeter = 10.763910449432011;
                double sqfeetPerMile = 27878400;

                area *= sqfeetPerMeter;

                if (area >= sqfeetPerMile)
                {
                    return string.Format("{0:,.0} sq.miles", area / sqfeetPerMile);
                }
                else
                {
                    return string.Format("{0:f0} sq.ft", area);
                }
            }
        }
    }
}
