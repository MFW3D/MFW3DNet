/* AssemblyInfo.cs
 * ===============
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
using Rss;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
 
[assembly: AssemblyTitle("RSS.NET")]
[assembly: AssemblyDescription("A reusable .NET assembly for RSS feeds")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("http://rss-net.sf.net")]
[assembly: AssemblyProduct("RSS.NET")]
[assembly: AssemblyCopyright("Copyright © 2002, 2003 George Tsiokos. All Rights Reserved.  Modifcations and Additions: Copyright © 2007 The Johns Hopkins University. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("0.86.*")]
[assembly: AssemblyDelaySign(false)]
#if !DEBUG
// Seems unneeded and causes a release build to fail unless you manually sign RSS.NET
// [assembly: AssemblyKeyFile("..\\..\\rss.snk")]
#endif
[assembly: AssemblyKeyName("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum, Execution=true)]
[assembly: AllowPartiallyTrustedCallers()]