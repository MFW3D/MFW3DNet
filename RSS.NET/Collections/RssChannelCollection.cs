using System;
using System.Collections;

namespace Rss
{
	/// <summary>A strongly typed collection of <see cref="RssChannel"/> objects</summary>
	[Serializable()]
	public class RssChannelCollection : CollectionBase
	{
		/// <summary>Gets or sets the channel at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>A channel at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">index is not a valid index.</exception>
		public RssChannel this[int index]
		{
			get { return ((RssChannel)(List[index])); }
			set { List[index] = value; }
		}
		/// <summary>Adds a specified channel to this collection.</summary>
		/// <param name="channel">The channel to add.</param>
		/// <returns>The zero-based index of the added channel.</returns>
		public int Add(RssChannel channel)
		{
			return List.Add(channel);
		}
		/// <summary>Determines whether the RssChannelCollection contains a specific element.</summary>
		/// <param name="rssChannel">The RssChannel to locate in the RssChannelCollection.</param>
		/// <returns>true if the RssChannelCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(RssChannel rssChannel)
		{
			return List.Contains(rssChannel);
		}
		/// <summary>Copies the entire RssChannelCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional RssChannel Array that is the destination of the elements copied from RssChannelCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source RssChannelCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(RssChannel[] array, int index)
		{
			List.CopyTo(array, index);
		}
		/// <summary>Searches for the specified RssChannel and returns the zero-based index of the first occurrence within the entire RssChannelCollection.</summary>
		/// <param name="rssChannel">The RssChannel to locate in the RssChannelCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssChannel within the entire RssChannelCollection, if found; otherwise, -1.</returns>
		public int IndexOf(RssChannel rssChannel)
		{
			return List.IndexOf(rssChannel);
		}
		/// <summary>Inserts a channel into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the channel.</param>
		/// <param name="channel">The channel to insert into this collection.</param>
		public void Insert(int index, RssChannel channel)
		{
			List.Insert(index, channel);
		}
		/// <summary>Removes a specified channel from this collection.</summary>
		/// <param name="channel">The channel to remove.</param>
		public void Remove(RssChannel channel)
		{
			List.Remove(channel);
		}
	}
}
