/* Hour.cs
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
    /// Hour of the week enumeration.  
    /// </summary>
    /// <remarks>Instances of this class are used to represent zero or more
    /// days of the week.  Assignments and comparisons may be made using bitwise
    /// operators.
    /// </remarks>
    public class Hour : IFormattable
    {
        /// <summary>Hour zero.</summary>
        public static readonly Hour Zero = new Hour(0, 1);
        /// <summary>Hour 1.</summary>
        public static readonly Hour One = new Hour(1, 2);
        /// <summary>Hour 2.</summary>
        public static readonly Hour Two = new Hour(2, 4);
        /// <summary>Hour 3.</summary>
        public static readonly Hour Three = new Hour(3, 8);
        /// <summary>Hour 4.</summary>
        public static readonly Hour Four = new Hour(4, 16);
        /// <summary>Hour 5.</summary>
        public static readonly Hour Five = new Hour(5, 32);
        /// <summary>Hour 6.</summary>
        public static readonly Hour Six = new Hour(6, 64);
        /// <summary>Hour 7.</summary>
        public static readonly Hour Seven = new Hour(7, 128);
        /// <summary>Hour 8.</summary>
        public static readonly Hour Eight = new Hour(8, 256);
        /// <summary>Hour 9.</summary>
        public static readonly Hour Nine = new Hour(9, 512);
        /// <summary>Hour 10.</summary>
        public static readonly Hour Ten = new Hour(10, 1024);
        /// <summary>Hour 11.</summary>
        public static readonly Hour Eleven = new Hour(11, 2048);
        /// <summary>Hour 12.</summary>
        public static readonly Hour Twelve = new Hour(12, 4096);
        /// <summary>Hour 13.</summary>
        public static readonly Hour Thirteen = new Hour(13, 8192);
        /// <summary>Hour 14.</summary>
        public static readonly Hour Fourteen = new Hour(14, 16384);
        /// <summary>Hour 15.</summary>
        public static readonly Hour Fifteen = new Hour(15, 32768);
        /// <summary>Hour 16.</summary>
        public static readonly Hour Sixteen = new Hour(16, 65536);
        /// <summary>Hour 17.</summary>
        public static readonly Hour Seventeen = new Hour(17, 131072);
        /// <summary>Hour 18.</summary>
        public static readonly Hour Eighteen = new Hour(18, 262144);
        /// <summary>Hour 19.</summary>
        public static readonly Hour Nineteen = new Hour(19, 524288);
        /// <summary>Hour 20.</summary>
        public static readonly Hour Twenty = new Hour(20, 1048576);
        /// <summary>Hour 21.</summary>
        public static readonly Hour TwentyOne = new Hour(21, 2097152);
        /// <summary>Hour 22.</summary>
        public static readonly Hour TwentyTwo = new Hour(22, 4194304);
        /// <summary>Hour 23.</summary>
        public static readonly Hour TwentyThree = new Hour(23, 8388608);

        private static readonly ListDictionary values;
        private readonly int code;
        private int name;

        private int Name { get { return name; }}

        /// <summary>
        /// A <see cref="ICollection"/> of all Hour objects.
        /// </summary>
        public static ICollection Values
        {
            get
            {
                return values.Values;
            }
        }

        static Hour()
        {
            values = new ListDictionary();    
            values.Add(Zero.Code, Zero);
            values.Add(One.Code, One);
            values.Add(Two.Code, Two);
            values.Add(Three.Code, Three);
            values.Add(Four.Code, Four);
            values.Add(Five.Code, Five);
            values.Add(Six.Code, Six);
            values.Add(Seven.Code, Seven);
            values.Add(Eight.Code, Eight);
            values.Add(Nine.Code, Nine);
            values.Add(Ten.Code, Ten);
            values.Add(Eleven.Code, Eleven);
            values.Add(Twelve.Code, Twelve);
            values.Add(Thirteen.Code, Thirteen);
            values.Add(Fourteen.Code, Fourteen);
            values.Add(Fifteen.Code, Fifteen);
            values.Add(Sixteen.Code, Sixteen);
            values.Add(Seventeen.Code, Seventeen);
            values.Add(Eighteen.Code, Eighteen);
            values.Add(Nineteen.Code, Nineteen);
            values.Add(Twenty.Code, Twenty);
            values.Add(TwentyOne.Code, TwentyOne);
            values.Add(TwentyTwo.Code, TwentyTwo);
            values.Add(TwentyThree.Code, TwentyThree);
        }

        /// <summary>
        /// Creates a new <see cref="Hour"/> instance.
        /// </summary>
        public Hour()
        {
            code = 0;
            name = -1;
        }

        private Hour (int code) : this(-1, code){}

        private Hour (int name, int code)
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
        /// Gets the Hour instance for the specified code.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <returns>The associated Hour instance.</returns>
        public static Hour Value(int code)
        {
            return (Hour) values[code];
        }

        /// <summary>
        /// Performs a bitwise OR on two instances.
        /// </summary>
        /// <param name="d1">A Hour instance.</param>
        /// <param name="d2">A second Hour instance.</param>
        /// <returns>A new Hour instance resulting from the operation.</returns>
        public static Hour operator |(Hour d1, Hour d2)
        {
            int newCode = d1.code | d2.code;
            return new Hour(newCode);
        }

        /// <summary>
        /// Determines if an instance is equal to this instance.
        /// </summary>
        /// <param name="obj">The instance for comparison.</param>
        /// <returns>true, if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            return (GetHashCode() == ((Hour)obj).GetHashCode());
        }
        
        /// <summary>
        /// Performs a bitwise AND on two instances.
        /// </summary>
        /// <param name="d1">A Hour instance.</param>
        /// <param name="d2">A second Hour instance.</param>
        /// <returns>A new Hour instance resulting from the operation.</returns>
        public static Hour operator &(Hour d1, Hour d2)
        {
            int newCode = d1.code & d2.code;
            return new Hour(newCode);
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
        /// Returns the string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString("G", null);
        }


        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>A string representation of the instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (Name != -1)
                return Name.ToString(format, formatProvider);

            return base.ToString();
        }

        /// <summary>
        /// Indicates if the object contains the specified Hour.
        /// </summary>
        /// <param name="d">The <see cref="Hour"/> instance for comparison.</param>
        /// <returns>true, if the input Hour is found; false, otherwise.</returns>
        public bool Contains(Hour d)
        {
            return (d.Equals(this & d));
        }

        /// <summary>
        /// Parses the specified string into an <see cref="Hour"/> instance.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The Hour representation.</returns>
        public static Hour Parse(string s)
        {
            return Parse(s, null);
        }

        /// <summary>
        /// Parses the specified string into an <see cref="Hour"/> instance.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>The <see cref="Hour"/> instance.</returns>
        /// <exception cref="FormatException">Indicates <code>s</code> could not be parsed.</exception>

        public static Hour Parse(string s, IFormatProvider provider)
        {
            if (s == null) throw new ArgumentNullException("s");

            foreach(Hour h in Hour.Values)
                if (h.ToString("G", provider) == s)
                    return h;
            
            throw new FormatException("The input could not be parsed.");
        }

        /// <summary>
        /// Gets a list of <see cref="Hour"/> instances.
        /// </summary>
        /// <value>The hours contained in this instance.  List returned will be a subset
        /// of the <see cref="Values"/> static member.</value>
        public IList Hours
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach(Hour h in Hour.Values)
                    if (this.Contains(h))
                        list.Add(h);
                return ArrayList.ReadOnly(list);
            }
        }
    }
}