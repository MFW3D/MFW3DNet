using System;
using System.Collections;

namespace Rss
{
	/// <summary>A strongly typed collection of <see cref="RssFeed"/> objects</summary>
	[Serializable()]
	public class RssFeedCollection : CollectionBase
	{
		/// <summary>Gets or sets the feed at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>A feed at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">index is not a valid index.</exception>
		public RssFeed this[int index]
		{
			get { return ((RssFeed)(List[index])); }
			set { List[index] = value; }
		}
		/// <summary>Gets or sets the feed with the given name.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="url">The url of the feed to access.</param>
		/// <value>A feed at each valid url. If the feed does not exist, null.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		public RssFeed this[string url]
		{
			get
			{
				for (int i=0; i<List.Count; i++)
				{
					if (((RssFeed)List[i]).Url == url)
					{
						return this[i];
					}
				}
				return null;
			}
		}
		/// <summary>Adds a specified feed to this collection.</summary>
		/// <param name="feed">The feed to add.</param>
		/// <returns>The zero-based index of the added feed.</returns>
		public int Add(RssFeed feed)
		{
			return List.Add(feed);
		}
		/// <summary>Determines whether the RssFeedCollection contains a specific element.</summary>
		/// <param name="rssFeed">The RssFeed to locate in the RssFeedCollection.</param>
		/// <returns>true if the RssFeedCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(RssFeed rssFeed)
		{
			return List.Contains(rssFeed);
		}
		/// <summary>Copies the entire RssFeedCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional RssFeed Array that is the destination of the elements copied from RssFeedCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source RssFeedCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(RssFeed[] array, int index)
		{
			List.CopyTo(array, index);
		}
		/// <summary>Searches for the specified RssFeed and returns the zero-based index of the first occurrence within the entire RssFeedCollection.</summary>
		/// <param name="rssFeed">The RssFeed to locate in the RssFeedCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssFeed within the entire RssFeedCollection, if found; otherwise, -1.</returns>
		public int IndexOf(RssFeed rssFeed)
		{
			return List.IndexOf(rssFeed);
		}
		/// <summary>Inserts a feed into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the feed.</param>
		/// <param name="feed">The feed to insert into this collection.</param>
		public void Insert(int index, RssFeed feed)
		{
			List.Insert(index, feed);
		}

		/// <summary>Removes a specified category from this collection.</summary>
		/// <param name="feed">The category to remove.</param>
		public void Remove(RssFeed feed)
		{
			List.Remove(feed);
		}
	}
}
