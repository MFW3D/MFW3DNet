using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace WorldWind
{
    // Ray casting class - Patrick Murris - march 2007
    public sealed class RayCasting
    {
		/// <summary>
		/// This class has only static methods.
		/// </summary>
        private RayCasting()
		{
		}

        /// <summary>
        /// Find the intersection of a ray with the terrain.
        /// </summary>
        /// <param name="p1">Cartesian coordinate of starting point</param>
        /// <param name="p2">Cartesian coordinate of end point</param>
        /// <param name="samplingPrecision">Sample length in meter</param>
        /// <param name="resultPrecision">Final sampling length in meter</param>
        /// <param name="latitude">Out : intersection latitude</param>
        /// <param name="longitude">Out : intersection longitude</param>
        /// <param name="world">Current world</param>
        /// <returns>NaN if no intersection found</returns>
        public static void RayIntersectionWithTerrain(
            Point3d p1,
            Point3d p2,
            double samplingPrecision, 
            double resultPrecision, 
            out Angle latitude,
            out Angle longitude,
            World world)
        {
            // Check for sphere intersection first
            // Note : checks for world radius + highest possible elevation
            float vertEx = World.Settings.VerticalExaggeration;
            double maxRadius = world.EquatorialRadius + 9000 * vertEx; // Max altitude for earth - should be dependant on world
            double a = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y) + (p2.Z - p1.Z) * (p2.Z - p1.Z);
            double b = 2.0 * ((p2.X - p1.X) * (p1.X) + (p2.Y - p1.Y) * (p1.Y) + (p2.Z - p1.Z) * (p1.Z));
            double c = p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z - maxRadius * maxRadius;
            double discriminant = b * b - 4 * a * c;
            if (discriminant <= 0)
            {
                // No intersection with sphere
                latitude = Angle.NaN;
                longitude = Angle.NaN;
                return;
            }
            // Factor to intersection
            // Note : if t1 > 0 intersection is forward, < 0 is behind us
            double t1 = ((-1.0) * b - Math.Sqrt(discriminant)) / (2 * a);
            Point3d p1LatLon = MathEngine.CartesianToSphericalD(p1.X, p1.Y, p1.Z);
            if (t1 > 0 && p1LatLon.X > maxRadius)
            {
                // Looking from above max altitude : move p1 forward to intersection with max alt sphere
                p1 = new Point3d(p1.X + t1 * (p2.X - p1.X), p1.Y + t1 * (p2.Y - p1.Y), p1.Z + t1 * (p2.Z - p1.Z));
            }

            // Ray sample
            Vector3 sample = new Vector3((float)(p2.X - p1.X), (float)(p2.Y - p1.Y), (float)(p2.Z - p1.Z));
            double maxLength = sample.Length();     // Max length for ray tracing
            double sampleLength = samplingPrecision;// Sampling steps length
            sample.Normalize();
            sample.Scale((float)sampleLength);

            // Casting
            Point3d ray = p1;
            double rayLength = 0;
            while (rayLength < maxLength)
            {
                Point3d rayLatLon = MathEngine.CartesianToSphericalD(ray.X, ray.Y, ray.Z);
                // Altitude at ray position
                double rayAlt = rayLatLon.X - world.EquatorialRadius;
                // Altitude at terrain position - from cached data (no download)
                double terrainAlt = world.TerrainAccessor.GetCachedElevationAt(MathEngine.RadiansToDegrees(rayLatLon.Y), MathEngine.RadiansToDegrees(rayLatLon.Z)); // best loaded data
                if (double.IsNaN(terrainAlt)) terrainAlt = 0;
                terrainAlt *= vertEx;
                if (terrainAlt > rayAlt)
                {
                    // Intersection found
                    if (sampleLength > resultPrecision)
                    {
                        // Go back one step
                        ray.X -= sample.X;
                        ray.Y -= sample.Y;
                        ray.Z -= sample.Z;
                        rayLength -= sampleLength;
                        // and refine sampling
                        sampleLength /= 10;
                        sample.Normalize();
                        sample.Scale((float)sampleLength);
                    }
                    else
                    {
                        // return location
                        latitude = Angle.FromRadians(rayLatLon.Y);
                        longitude = Angle.FromRadians(rayLatLon.Z);
                        return;
                    }
                }
                // Move forward
                ray.X += sample.X;
                ray.Y += sample.Y;
                ray.Z += sample.Z;
                rayLength += sampleLength;
            }
            // No intersection with terrain found
            latitude = Angle.NaN;
            longitude = Angle.NaN;

        }
    }
}
