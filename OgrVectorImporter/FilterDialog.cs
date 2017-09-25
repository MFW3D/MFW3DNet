using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OgrVectorImporter
{
    public partial class FilterDialog : Form
    {
        public FilterDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets string for filter
        /// </summary>
        public string FilterString
        {
            get { return tbFilterString.Text; }
        }

        /// <summary>
        /// Gets type of filtering to perform
        /// </summary>
        public FilterType FilterType
        {
            get
            {
                if (rbExactMatch.Checked)
                    return FilterType.Exact;
                else if (rbContains.Checked)
                    return FilterType.Contains;
                else
                    return FilterType.Regex;
            }
        }
    }
}