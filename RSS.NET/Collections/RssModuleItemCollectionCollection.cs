using System;
using System.Collections;

namespace Rss
{
	/// <summary>A strongly typed collection of <see cref="RssModuleItemCollection"/> objects</summary>
	public class RssModuleItemCollectionCollection : System.Collections.CollectionBase
	{
		/// <summary>Gets or sets the item at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>An item at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">index is not a valid index.</exception>
		public RssModuleItemCollection this[int index]
		{
			get { return ((RssModuleItemCollection)(List[index])); }
			set { List[index] = value; }
		}

		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="rssModuleItemCollection">The item to add.</param>
		/// <returns>The zero-based index of the added item.</returns>
		public int Add(RssModuleItemCollection rssModuleItemCollection)
		{
			return List.Add(rssModuleItemCollection);
		}

		/// <summary>Determines whether the RssModuleItemCollectionCollection contains a specific element.</summary>
		/// <param name="rssModuleItemCollection">The RssModuleItemCollection to locate in the RssModuleItemCollectionCollection.</param>
		/// <returns>true if the RssModuleItemCollectionCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(RssModuleItemCollection rssModuleItemCollection)
		{
			return List.Contains(rssModuleItemCollection);
		}

		/// <summary>Copies the entire RssModuleItemCollectionCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional RssModuleItemCollection Array that is the destination of the elements copied from RssModuleItemCollectionCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source RssModuleItemCollectionCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(RssModuleItemCollection[] array, int index)
		{
			List.CopyTo(array, index);
		}

		/// <summary>Searches for the specified RssModuleItemCollection and returns the zero-based index of the first occurrence within the entire RssModuleItemCollectionCollection.</summary>
		/// <param name="rssModuleItemCollection">The RssModuleItemCollection to locate in the RssModuleItemCollectionCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssModuleItemCollection within the entire RssModuleItemCollectionCollection, if found; otherwise, -1.</returns>
		public int IndexOf(RssModuleItemCollection rssModuleItemCollection)
		{
			return List.IndexOf(rssModuleItemCollection);
		}

		/// <summary>Inserts an item into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the item.</param>
		/// <param name="rssModuleItemCollection">The item to insert into this collection.</param>
		public void Insert(int index, RssModuleItemCollection rssModuleItemCollection)
		{
			List.Insert(index, rssModuleItemCollection);
		}

		/// <summary>Removes a specified item from this collection.</summary>
		/// <param name="rssModuleItemCollection">The item to remove.</param>
		public void Remove(RssModuleItemCollection rssModuleItemCollection)
		{
			List.Remove(rssModuleItemCollection);
		}
	}
}
