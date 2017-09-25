//========================= (UNCLASSIFIED) ==============================
// Copyright © 2007 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2007 The Johns Hopkins University. 
//
// This software was developed at The Johns Hopkins University/Applied 
// Physics Laboratory (“JHU/APL”) that is the author thereof under the 
// “work made for hire” provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the “Software”), to use the 
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
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Collab.jhuapl.Whiteboard
{
	/// <summary>
	/// Summary description for HotspotForm.
	/// </summary>
	public partial class HotspotForm : Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.TextBox description;

		public HotspotForm()
		{
			InitializeComponent();
		}


		private void OK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;		
			this.Hide();
		}

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;		
			this.Hide();
		}

		public string HotspotName
		{
			get { return name.Text; }
			set { name.Text = value; }
		}

		public string Description
		{
			get { return description.Text; }
			set { description.Text = value; }
		}

        public bool SaveCameraAngle
        {
            get { return cameraAngleCheckBox.Checked; }
            set { cameraAngleCheckBox.Checked = value; }
        }

        public string URL
        {
            get { return urlTextBox.Text; }
            set { urlTextBox.Text = value; }
        }
	}
}
