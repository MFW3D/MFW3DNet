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
[assembly: AssemblyCopyright("Copyright ?2002, 2003 George Tsiokos. All Rights Reserved.  Modifcations and Additions: Copyright ?2007 The Johns Hopkins University. All Rights Reserved.")]
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