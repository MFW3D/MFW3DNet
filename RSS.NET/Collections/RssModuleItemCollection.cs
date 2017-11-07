using System;
using System.Collections;

namespace Rss
{
	/// <summary>A strongly typed collection of <see cref="RssModuleItem"/> objects</summary>
	public class RssModuleItemCollection : System.Collections.CollectionBase
	{
		private ArrayList _alBindTo = new ArrayList();

		/// <summary>Gets or sets the item at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>An item at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">index is not a valid index.</exception>
		public RssModuleItem this[int index]
		{
			get { return ((RssModuleItem)(List[index])); }
			set { List[index] = value; }
		}

		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="rssModuleItem">The item to add.</param>
		/// <returns>The zero-based index of the added item.</returns>
		public int Add(RssModuleItem rssModuleItem)
		{
			return List.Add(rssModuleItem);
		}

		/// <summary>Determines whether the RssModuleItemCollection contains a specific element.</summary>
		/// <param name="rssModuleItem">The RssModuleItem to locate in the RssModuleItemCollection.</param>
		/// <returns>true if the RssModuleItemCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(RssModuleItem rssModuleItem)
		{
			return List.Contains(rssModuleItem);
		}

		/// <summary>Copies the entire RssModuleItemCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional RssModuleItem Array that is the destination of the elements copied from RssModuleItemCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source RssModuleItemCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(RssModuleItem[] array, int index)
		{
			List.CopyTo(array, index);
		}

		/// <summary>Searches for the specified RssModuleItem and returns the zero-based index of the first occurrence within the entire RssModuleItemCollection.</summary>
		/// <param name="rssModuleItem">The RssModuleItem to locate in the RssModuleItemCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssModuleItem within the entire RssModuleItemCollection, if found; otherwise, -1.</returns>
		public int IndexOf(RssModuleItem rssModuleItem)
		{
			return List.IndexOf(rssModuleItem);
		}

		/// <summary>Inserts an item into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the item.</param>
		/// <param name="rssModuleItem">The item to insert into this collection.</param>
		public void Insert(int index, RssModuleItem rssModuleItem)
		{
			List.Insert(index, rssModuleItem);
		}

		/// <summary>Removes a specified item from this collection.</summary>
		/// <param name="rssModuleItem">The item to remove.</param>
		public void Remove(RssModuleItem rssModuleItem)
		{
			List.Remove(rssModuleItem);
		}

		/// <summary>Bind a particular item to this module</summary>
		/// <param name="itemHashCode">Hash code of the item</param>
		public void BindTo(int itemHashCode)
		{
			this._alBindTo.Add(itemHashCode);
		}

		/// <summary>Check if a particular item is bound to this module</summary>
		/// <param name="itemHashCode">Hash code of the item</param>
		/// <returns>true if this item is bound to this module, otherwise false</returns>
		public bool IsBoundTo(int itemHashCode)
		{
			return (this._alBindTo.BinarySearch(0, this._alBindTo.Count, itemHashCode, null) >= 0);
		}
	}
}
