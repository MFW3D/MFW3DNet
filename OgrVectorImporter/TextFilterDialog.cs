using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OgrVectorImporter
{
    public partial class TextFilterDialog : Form
    {
        public TextFilterDialog()
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
        public TextFilterType FilterType
        {
            get
            {
                if (rbExactMatch.Checked)
                    return TextFilterType.Exact;
                else if (rbContains.Checked)
                    return TextFilterType.Contains;
                else
                    return TextFilterType.Regex;
            }
        }
    }
}