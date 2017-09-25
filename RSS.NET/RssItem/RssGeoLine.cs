#region Copyright and Licenses
//========================= (UNCLASSIFIED) ==============================
// Copyright � 2005-2007 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// RSS.NET Source Code - Copyright � 2002 - 2005 George Tsiokos. All Rights Reserved.
// Modified under the Attribution/Share Alike Creative Commons license
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2007 The Johns Hopkins University. 
//
// This software was developed at The Johns Hopkins University/Applied 
// Physics Laboratory (�JHU/APL�) that is the author thereof under the 
// �work made for hire� provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the �Software�), to use the 
// Software without restriction, including without limitation the rights 
// to copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit others to do so, subject to the 
// following conditions: 
//
// 1.  This LICENSE AND DISCLAIMER, including the copyright notice, shall 
//     be included in all copies of the Software, including copies of 
//     substantial portions of the Software; 
//
// 2.  JHU/APL assumes no obligation to provide support of any kind with 
//     regard to the Software.  This includes no obligation to provide 
//     assistance in using the Software nor to provide updated versions of 
//     the Software; and 
//
// 3.  THE SOFTWARE AND ITS DOCUMENTATION ARE PROVIDED AS IS AND WITHOUT 
//     ANY EXPRESS OR IMPLIED WARRANTIES WHATSOEVER.  ALL WARRANTIES 
//     INCLUDING, BUT NOT LIMITED TO, PERFORMANCE, MERCHANTABILITY, FITNESS
//     FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT ARE HEREBY DISCLAIMED.  
//     USERS ASSUME THE ENTIRE RISK AND LIABILITY OF USING THE SOFTWARE.  
//     USERS ARE ADVISED TO TEST THE SOFTWARE THOROUGHLY BEFORE RELYING ON 
//     IT.  IN NO EVENT SHALL THE JOHNS HOPKINS UNIVERSITY BE LIABLE FOR 
//     ANY DAMAGES WHATSOEVER, INCLUDING, WITHOUT LIMITATION, ANY LOST 
//     PROFITS, LOST SAVINGS OR OTHER INCIDENTAL OR CONSEQUENTIAL DAMAGES, 
//     ARISING OUT OF THE USE OR INABILITY TO USE THE SOFTWARE. 
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Rss
{
    /// <summary>
    /// The Geospatial Line associated with this item
    /// </summary>
    public class RssGeoLine
    {
        /// <summary>
        /// The points in this line
        /// </summary>
        public List<RssGeoPoint> Points
        {
            get { return m_points; }
            set { if (m_points != null) m_points = value; }
        }
        List<RssGeoPoint> m_points = new List<RssGeoPoint>();

        /// <summary>
        /// Whether this is a valid geo point
        /// </summary>
        public bool IsValid
        {
            get 
            {
                bool isValid = true;
                foreach (RssGeoPoint point in m_points)
                {
                    if (!point.IsValid)
                        isValid = false;
                }
                return isValid; 
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RssGeoLine() { }

    }
}
