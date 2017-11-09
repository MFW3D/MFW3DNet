using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OgrVectorImporter
{
    public partial class NoDataForm : Form
    {
        public VectorStyleParameters NoDataStyle;
            
        public NoDataForm(VectorStyleParameters inputStyle)
        {
            InitializeComponent();

            NoDataStyle = inputStyle;

        }

        private void btnNoDataPolyColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                NoDataStyle.PolygonColor = Color.FromArgb((int)numNoDataLineAlpha.Value, colorDialog1.Color);
                UpdateParameters();
            }
        }

        private void UpdateParameters()
        {
            btnNoDataLineColor.BackColor = NoDataStyle.LineColor;
            btnNoDataPolyColor.BackColor = NoDataStyle.PolygonColor;
            numNoDataLineAlpha.Value = NoDataStyle.LineColor.A;
            numNoDataPolyAlpha.Value = NoDataStyle.PolygonColor.A;
            chbNoDataOutlinePolygon.Checked = NoDataStyle.OutlinePolygons;
            numNoDataLineWidth.Value = (decimal)NoDataStyle.LineWidth;
        }

        private void btnNoDataLineColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                NoDataStyle.LineColor = Color.FromArgb((int)numNoDataLineAlpha.Value, colorDialog1.Color);
                UpdateParameters();
            }
        }

        private void numNoDataPolyAlpha_ValueChanged(object sender, EventArgs e)
        {
            NoDataStyle.PolygonColor = Color.FromArgb((int)numNoDataPolyAlpha.Value, NoDataStyle.PolygonColor);
            UpdateParameters();
        }

        private void numNoDataLineAlpha_ValueChanged(object sender, EventArgs e)
        {
            NoDataStyle.LineColor = Color.FromArgb((int)numNoDataLineAlpha.Value, NoDataStyle.LineColor);
            UpdateParameters();
        }

        private void chbNoDataOutlinePolygon_CheckedChanged(object sender, EventArgs e)
        {
            NoDataStyle.OutlinePolygons = chbNoDataOutlinePolygon.Checked;
        }

        private void numNoDataLineWidth_ValueChanged(object sender, EventArgs e)
        {
            NoDataStyle.LineWidth = (float)numNoDataLineWidth.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            NoDataStyle.NoDataValue = (double)numericUpDown1.Value;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}