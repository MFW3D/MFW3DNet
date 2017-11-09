using System;
using System.ComponentModel;

namespace MFW3D.Terrain
{
	/// <summary>
	/// Terrain (elevation) interface
	/// </summary>
	public abstract class TerrainAccessor : IDisposable
	{
        private bool isOn = true;
		protected string m_name;
		protected double m_north;
		protected double m_south;
		protected double m_east;
		protected double m_west;
        protected TerrainAccessor[] m_higherResolutionSubsets;

		/// <summary>
		/// Terrain model name
		/// </summary>
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		/// <summary>
		/// North boundary
		/// </summary>
		public double North
		{
			get
			{
				return m_north;
			}
			set
			{
				m_north = value;
			}
		}

		/// <summary>
		/// South boundary
		/// </summary>
		public double South
		{
			get
			{
				return m_south;
			}
			set
			{
				m_south = value;
			}
		}

		/// <summary>
		/// West boundary
		/// </summary>
		public double West
		{
			get
			{
				return m_west;
			}
			set
			{
				m_west = value;
			}
		}

		/// <summary>
		/// East boundary
		/// </summary>
		public double East
		{
			get
			{
				return m_east;
			}
			set
			{
				m_east = value;
			}
		}


        public int SubsetCount
        {
            get 
            {
                if (m_higherResolutionSubsets != null)
                    return m_higherResolutionSubsets.Length;
                else
                    return 0;
            }
        }

        public TerrainAccessor[] HighResSubsets
        {
            get { return m_higherResolutionSubsets; }
        }

        /// <summary>
        /// Hide/Show this object.
        /// </summary>
        [Description("This layer's enabled status.")]
        public virtual bool IsOn
        {
            get { return this.isOn; }
            set
            {
                //if (isOn && !value)
                //    this.Dispose();
                this.isOn = value;
            }
        }

		/// <summary>
		/// Gets the terrain elevation at a given Latitude, Longitude, 
		/// and resolution accuracy in the latitude/longitude geographic frame of reference.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="targetSamplesPerDegree"></param>
		/// <returns>Returns 0 if the tile is not available on disk.</returns>
		public abstract float GetElevationAt(double latitude, double longitude, double targetSamplesPerDegree);

		/// <summary>
		/// Get terrain elevation at specified location.  
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <returns>Returns 0 if the tile is not available on disk.</returns>
		public virtual float GetElevationAt(double latitude, double longitude)
		{
			return GetElevationAt(latitude, longitude, 0);
		}

		/// <summary>
        /// Get fast terrain elevation at specified location from cached data. 
        /// Will not trigger any download or file loading from cache - just memory.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees.</param>
        /// <param name="longitude">Longitude in decimal degrees.</param>
        /// <returns>Returns 0 if the tile is not available in cache.</returns>
        public virtual float GetCachedElevationAt(double latitude, double longitude)
        {
            return 0f;
        }

        /// <summary>
		/// Gets the elevation array for given geographic bounding box and resolution.
		/// </summary>
		/// <param name="north">North edge in decimal degrees.</param>
		/// <param name="south">South edge in decimal degrees.</param>
		/// <param name="west">West edge in decimal degrees.</param>
		/// <param name="east">East edge in decimal degrees.</param>
		/// <param name="samples"></param>
		public virtual TerrainTile GetElevationArray(double north, double south, double west, double east, int samples)
		{
			TerrainTile res = null;
			res = new TerrainTile(null);
			res.North = north;
			res.South = south;
			res.West = west;
			res.East = east;
			res.SamplesPerTile = samples;
			res.IsInitialized = true;
			res.IsValid = true;

			double latrange = Math.Abs(north - south);
			double lonrange = Math.Abs(east - west);

			float[,] data = new float[samples,samples];
			float scaleFactor = (float)1.0/(samples - 1);
			for (int x = 0; x < samples; x++)
			{
				for (int y = 0; y < samples; y++)
				{
					double curLat = north - scaleFactor * latrange * x;
					double curLon = west + scaleFactor * lonrange * y;

					data[x, y] = GetElevationAt(curLat, curLon, 0);
				}
			}
			res.ElevationData = data;

			return res;
		}

		public virtual void Dispose()
		{
		}

        /// <summary>
        /// This method appends to the array of higher resolution
        /// subsets for runtime addition of terrain layers
        /// </summary>
        /// <param name="newHighResSubset"></param>
        public void AddHigherResolutionSubset(TerrainAccessor newHighResSubset)
        {
            //need to lock array here
            if (m_higherResolutionSubsets == null)
                m_higherResolutionSubsets = new TerrainAccessor[0];
            lock (m_higherResolutionSubsets)
            {
                TerrainAccessor[] temp_highres = new TerrainAccessor[m_higherResolutionSubsets.Length + 1];
                for (int i = 0; i < m_higherResolutionSubsets.Length; i++)
                {
                    temp_highres[i] = m_higherResolutionSubsets[i];
                }
                temp_highres[temp_highres.Length - 1] = newHighResSubset;
                m_higherResolutionSubsets = temp_highres;
            }
        }

        /// <summary>
        /// Method removes the specified high resolution subset from the
        /// array of terrain layers, decreases the array size by 1
        /// </summary>
        /// <param name="highResSubset"></param>
        public void RemoveHigherResolutionSubset(TerrainAccessor highResSubset)
        {
            // lock array here
            if (m_higherResolutionSubsets == null)
                m_higherResolutionSubsets = new TerrainAccessor[0];
            lock (m_higherResolutionSubsets)
            {
                TerrainAccessor[] temp_highres = new TerrainAccessor[m_higherResolutionSubsets.Length + 1];
                for (int i = 0; i < m_higherResolutionSubsets.Length; i++)
                {
                    if (m_higherResolutionSubsets[i] != highResSubset)
                        temp_highres[i] = m_higherResolutionSubsets[i];
                }
                m_higherResolutionSubsets = temp_highres;
            }
        }
    }
}