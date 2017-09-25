/* Day.cs
 * ============
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright © 2002 - 2005 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Rss
{
    /// <summary>
    /// Day of the week enumeration.  
    /// </summary>
    /// <remarks>Instances of this class are used to represent zero or more
    /// days of the week.  Assignments and comparisons may be made using bitwise
    /// operators.
    /// </remarks>
    public class Day
    {
        /// <summary>Monday.</summary>
        public static readonly Day Monday = new Day("Monday", 1);
        /// <summary>Tuesday.</summary>
        public static readonly Day Tuesday = new Day("Tuesday", 2);
        /// <summary>Wednesday.</summary>
        public static readonly Day Wednesday = new Day("Wednesday", 4);
        /// <summary>Thursday.</summary> 
        public static readonly Day Thursday = new Day("Thursday", 8);
        /// <summary>Friday.</summary>
        public static readonly Day Friday = new Day("Friday", 16);
        /// <summary>Saturday.</summary>
        public static readonly Day Saturday = new Day("Saturday", 32);
        /// <summary>Sunday.</summary>
        public static readonly Day Sunday = new Day("Sunday", 64);
        private static readonly ListDictionary values;
        private string name;
        private string Name { get { return name; }}

        private readonly int code;

        /// <summary>
        /// A <see cref="ICollection"/> of all Day objects.
        /// </summary>
        public static ICollection Values
        {
            get
            {
                return values.Values;
            }
        }

        static Day()
        {
            values = new ListDictionary();    
            values.Add(Monday.Code, Monday);
            values.Add(Tuesday.Code, Tuesday);
            values.Add(Wednesday.Code, Wednesday);
            values.Add(Thursday.Code, Thursday);
            values.Add(Friday.Code, Friday);
            values.Add(Saturday.Code, Saturday);
            values.Add(Sunday.Code, Sunday);
        }

        /// <summary>
        /// Creates a new <see cref="Day"/> instance.
        /// </summary>
        public Day() {}

        private Day (int code) : this(null, code){}

        private Day (string name, int code)
        {
            this.code = code;
            this.name = name;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value></value>
        public int Code { get { return this.code; }}

        /// <summary>
        /// Gets the Day instance for the specified code.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <returns>The associated Day instance.</returns>
        public static Day Value(int code)
        {
            return (Day) values[code];
        }

        /// <summary>
        /// Performs a bitwise OR on two instances.
        /// </summary>
        /// <param name="d1">A Day instance.</param>
        /// <param name="d2">A second Day instance.</param>
        /// <returns>A new Day instance resulting from the operation.</returns>
        public static Day operator |(Day d1, Day d2)
        {
            int newCode = d1.code | d2.code;
            return new Day(newCode);
        }

        /// <summary>
        /// Determines if an instance is equal to this instance.
        /// </summary>
        /// <param name="obj">The instance for comparison.</param>
        /// <returns>true, if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            return (GetHashCode() == ((Day)obj).GetHashCode());
        }
        
        /// <summary>
        /// Performs a bitwise AND on two instances.
        /// </summary>
        /// <param name="d1">A Day instance.</param>
        /// <param name="d2">A second Day instance.</param>
        /// <returns>A new Day instance resulting from the operation.</returns>
        public static Day operator &(Day d1, Day d2)
        {
            int newCode = d1.code & d2.code;
            return new Day(newCode);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The unique hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.Code;
        }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>A string representation of the instance.</returns>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <param name="provider">The format provider.</param>
        /// <returns>A string representation of the instance.</returns>
        public string ToString(IFormatProvider provider)
        {
            if (Name != null)
                return Name.ToString(provider);

            return base.ToString();
        }

        /// <summary>
        /// Indicates if the object contains the specified Day.
        /// </summary>
        /// <param name="d">The <see cref="Day"/> instance for comparison.</param>
        /// <returns>true, if the input Day is found; false, otherwise.</returns>
        public bool Contains(Day d)
        {
            return (d.Equals(this & d));
        }

        /// <summary>
        /// Gets a list of <see cref="Day"/> instances.
        /// </summary>
        /// <value>The days contained in this instance.  List returned will be a subset
        /// of the <see cref="Values"/> static member.</value>
        public IList Days
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach(Day d in Day.Values)
                {
                    if (this.Contains(d))
                        list.Add(d);
                }
                return ArrayList.ReadOnly(list);
            }
        }

        /// <summary>
        /// Parses the string into a <see cref="Day"/> instance.
        /// </summary>
        /// <param name="s">The day string representation.</param>
        /// <returns>The Day instance.</returns>
        public static Day Parse(string s)
        {
            return Parse(s, null);
        }

        /// <summary>
        /// Parses the string into a <see cref="Day"/> instance.
        /// </summary>
        /// <param name="s">The day string representation.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>The Day instance.</returns>
        /// <exception cref="FormatException">Indicates <code>s</code> could not be parsed.</exception>
        public static Day Parse(string s, IFormatProvider provider)
        {
            if (s == null) throw new ArgumentNullException("s");

            foreach(Day d in Day.Values)
                if (d.ToString(provider) == s)
                    return d;

            throw new FormatException("The specified string cannot be parsed.");
        }
    }
}