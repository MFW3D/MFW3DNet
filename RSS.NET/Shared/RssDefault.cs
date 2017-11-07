using System;

namespace Rss
{
	/// <summary>Contains default values and methods for maintaining data consistency</summary>
	[Serializable()]
	public class RssDefault
	{
		/// <summary>Default value for a string in all RSS classes</summary>
		/// <value>empty string</value>
		/// <remarks>If an element in the RSS class library has the value of RssDefault.String, consider the element as "not entered", "null", or empty.</remarks>
		public const string String = "";
		/// <summary>Default value for an int in all RSS classes</summary>
		/// <value>-1</value>
		/// <remarks>If an element in the RSS class library has the value of RssDefault.Int, consider the element as "not entered", "null", or empty.</remarks>
		public const int Int = -1;
		/// <summary>Default value for a DateTime in all RSS classes</summary>
		/// <value>DateTime.MinValue</value>
		/// <remarks>If an element in the RSS class library has the value of RssDefault.DateTime, consider the element as "not entered", "null", or empty.</remarks>
		public static readonly DateTime DateTime = DateTime.MinValue;
		/// <summary>Default value for a Uri in all RSS classes</summary>
		/// <value>gopher://rss-net.sf.net</value>
		/// <remarks>If an element in the RSS class library has the value of RssDefault.Uri, consider the element as "not entered", "null", or empty.</remarks>
		public static readonly Uri Uri = new Uri("gopher://rss-net.sf.net");
		/// <summary>Verifies the string passed is not null</summary>
		/// <param name="input">string to verify</param>
		/// <returns>RssDefault.String if input is null, otherwise input</returns>
		/// <remarks>Method is used in properties to prevent a null value</remarks>
		public static string Check(string input)
		{
			return input == null ? String : input;
		}
		/// <summary>Verifies the int passed is greater than or equal to -1</summary>
		/// <param name="input">int to verify</param>
		/// <returns>RssDefault.Int if int is less than -1, else input</returns>
		/// <remarks>Method is used in properties to prevent values less than -1</remarks>
		public static int Check(int input)
		{
			return input < -1 ? Int : input;
		}
		/// <summary>Verifies the Uri passed is not null</summary>
		/// <param name="input">Uri to verify</param>
		/// <returns>RssDefault.Uri if input is null, otherwise input</returns>
		/// <remarks>Method is used in all properties to prevent a null value</remarks>
		public static Uri Check(Uri input)
		{
			return input == null ? Uri : input;
		}
	}
}
