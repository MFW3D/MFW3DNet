using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using WorldWind;
using WorldWind.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Threading;
using OGR;

namespace OgrVectorImporter
{
	/// <summary>
	/// Form to display a list of key fields from the dbf
	/// </summary>
    public class VectorInfoSelector : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTextKeyField;
        private System.Windows.Forms.Button btnEnter;
        private Label label2;
        private Button btnTextLineColor;
        private Label label3;
        private System.Windows.Forms.Button btnCancel;
        private Panel pnlLine;
        private Panel pnlPolygon;
        private Label label4;
        private Button btnTextPolyColor;
        private ColorDialog colorDialog;
        private CheckBox chbTextOutlinePolygon;
        private NumericUpDown numTextLineWidth;
        private Label label5;
        private NumericUpDown numTextPolyAlpha;
        private Label label6;
        private NumericUpDown numTextLineAlpha;
        private TextBox tbProjection;
        private Label label7;
        private ComboBox cbLabelField;
        private Label label8;
        private Button btnTextFilter;
        private CheckBox chbTextFilter;
        private Label label9;
        private TextBox tbLayerName;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label10;
        private ComboBox cbNumKeyField;
        private Panel panel1;
        private Label label11;
        private NumericUpDown numNumMinPolyAlpha;
        private CheckBox chbNumMinOutlinePoly;
        private Label label12;
        private Button btnNumMinPolyColor;
        private Panel panel2;
        private Label label13;
        private NumericUpDown numNumMinLineAlpha;
        private Label label14;
        private NumericUpDown numNumMinLineWidth;
        private Button btnNumMinLineColor;
        private Label label15;
        private Button btnNumericFilter;
        private CheckBox chbNumericFilter;
        private Panel panel3;
        private Label label16;
        private NumericUpDown numNumMaxPolyAlpha;
        private CheckBox chbNumMaxOutlinePoly;
        private Label label17;
        private Button btnNumMaxPolyColor;
        private Panel panel4;
        private Label label18;
        private NumericUpDown numNumMaxLineAlpha;
        private Label label19;
        private NumericUpDown numNumMaxLineWidth;
        private Button btnNumMaxLineColor;
        private Label label20;
        private GroupBox groupBox1;
        private RadioButton rbLinear;
        private RadioButton rbLogDec;
        private RadioButton rbLogInc;
        private Button btnNoDataValue;
        private CheckBox chbNoDataValue;
        private GroupBox groupBox3;
        private GroupBox groupBox2;

        private string keyFieldName;
        //private TextFilterType tFType = TextFilterType.Exact;
        //private DataType keyDataType = DataType.Text;
        //private string filterString = null;

        public VectorStyleParameters NumericMinStyle;
        public VectorStyleParameters NumericMaxStyle;
        public VectorStyleParameters NumericNoDataStyle;
        public VectorStyleParameters TextStyle;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cbTextKeyField = new System.Windows.Forms.ComboBox();
            this.btnEnter = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTextLineColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numTextLineWidth = new System.Windows.Forms.NumericUpDown();
            this.pnlLine = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.numTextLineAlpha = new System.Windows.Forms.NumericUpDown();
            this.pnlPolygon = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.numTextPolyAlpha = new System.Windows.Forms.NumericUpDown();
            this.chbTextOutlinePolygon = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTextPolyColor = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.tbProjection = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbLabelField = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnTextFilter = new System.Windows.Forms.Button();
            this.chbTextFilter = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbLayerName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.numNumMaxPolyAlpha = new System.Windows.Forms.NumericUpDown();
            this.chbNumMaxOutlinePoly = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnNumMaxPolyColor = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.numNumMaxLineAlpha = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.numNumMaxLineWidth = new System.Windows.Forms.NumericUpDown();
            this.btnNumMaxLineColor = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.numNumMinPolyAlpha = new System.Windows.Forms.NumericUpDown();
            this.chbNumMinOutlinePoly = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnNumMinPolyColor = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.numNumMinLineAlpha = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.numNumMinLineWidth = new System.Windows.Forms.NumericUpDown();
            this.btnNumMinLineColor = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.btnNoDataValue = new System.Windows.Forms.Button();
            this.chbNoDataValue = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbLogDec = new System.Windows.Forms.RadioButton();
            this.rbLogInc = new System.Windows.Forms.RadioButton();
            this.rbLinear = new System.Windows.Forms.RadioButton();
            this.btnNumericFilter = new System.Windows.Forms.Button();
            this.chbNumericFilter = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbNumKeyField = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numTextLineWidth)).BeginInit();
            this.pnlLine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTextLineAlpha)).BeginInit();
            this.pnlPolygon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTextPolyAlpha)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMaxPolyAlpha)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMaxLineAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMaxLineWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMinPolyAlpha)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMinLineAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMinLineWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select key data field:";
            // 
            // cbTextKeyField
            // 
            this.cbTextKeyField.AllowDrop = true;
            this.cbTextKeyField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextKeyField.Location = new System.Drawing.Point(126, 11);
            this.cbTextKeyField.MaxDropDownItems = 100;
            this.cbTextKeyField.Name = "cbTextKeyField";
            this.cbTextKeyField.Size = new System.Drawing.Size(141, 20);
            this.cbTextKeyField.TabIndex = 1;
            this.cbTextKeyField.SelectedIndexChanged += new System.EventHandler(this.cbTextKeyField_SelectedIndexChanged);
            // 
            // btnEnter
            // 
            this.btnEnter.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnEnter.Location = new System.Drawing.Point(76, 402);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(75, 21);
            this.btnEnter.TabIndex = 2;
            this.btnEnter.Text = "OK";
            this.btnEnter.UseVisualStyleBackColor = true;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(157, 402);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Line color:";
            // 
            // btnTextLineColor
            // 
            this.btnTextLineColor.BackColor = System.Drawing.Color.Red;
            this.btnTextLineColor.Location = new System.Drawing.Point(81, 6);
            this.btnTextLineColor.Name = "btnTextLineColor";
            this.btnTextLineColor.Size = new System.Drawing.Size(32, 21);
            this.btnTextLineColor.TabIndex = 5;
            this.btnTextLineColor.UseVisualStyleBackColor = false;
            this.btnTextLineColor.Click += new System.EventHandler(this.btnColorDialog_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Line width";
            // 
            // numTextLineWidth
            // 
            this.numTextLineWidth.DecimalPlaces = 1;
            this.numTextLineWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTextLineWidth.Location = new System.Drawing.Point(114, 37);
            this.numTextLineWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTextLineWidth.Name = "numTextLineWidth";
            this.numTextLineWidth.Size = new System.Drawing.Size(54, 21);
            this.numTextLineWidth.TabIndex = 7;
            this.numTextLineWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTextLineWidth.ValueChanged += new System.EventHandler(this.numTextLineWidth_ValueChanged);
            // 
            // pnlLine
            // 
            this.pnlLine.Controls.Add(this.label6);
            this.pnlLine.Controls.Add(this.numTextLineAlpha);
            this.pnlLine.Controls.Add(this.label2);
            this.pnlLine.Controls.Add(this.numTextLineWidth);
            this.pnlLine.Controls.Add(this.btnTextLineColor);
            this.pnlLine.Controls.Add(this.label3);
            this.pnlLine.Location = new System.Drawing.Point(26, 147);
            this.pnlLine.Name = "pnlLine";
            this.pnlLine.Size = new System.Drawing.Size(228, 70);
            this.pnlLine.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(124, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Opacity";
            // 
            // numTextLineAlpha
            // 
            this.numTextLineAlpha.Location = new System.Drawing.Point(173, 8);
            this.numTextLineAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numTextLineAlpha.Name = "numTextLineAlpha";
            this.numTextLineAlpha.Size = new System.Drawing.Size(46, 21);
            this.numTextLineAlpha.TabIndex = 11;
            this.numTextLineAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numTextLineAlpha.ValueChanged += new System.EventHandler(this.numTextLineAlpha_ValueChanged);
            // 
            // pnlPolygon
            // 
            this.pnlPolygon.Controls.Add(this.label5);
            this.pnlPolygon.Controls.Add(this.numTextPolyAlpha);
            this.pnlPolygon.Controls.Add(this.chbTextOutlinePolygon);
            this.pnlPolygon.Controls.Add(this.label4);
            this.pnlPolygon.Controls.Add(this.btnTextPolyColor);
            this.pnlPolygon.Location = new System.Drawing.Point(26, 77);
            this.pnlPolygon.Name = "pnlPolygon";
            this.pnlPolygon.Size = new System.Drawing.Size(228, 60);
            this.pnlPolygon.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(122, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Opacity";
            // 
            // numTextPolyAlpha
            // 
            this.numTextPolyAlpha.Location = new System.Drawing.Point(171, 9);
            this.numTextPolyAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numTextPolyAlpha.Name = "numTextPolyAlpha";
            this.numTextPolyAlpha.Size = new System.Drawing.Size(46, 21);
            this.numTextPolyAlpha.TabIndex = 9;
            this.numTextPolyAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numTextPolyAlpha.ValueChanged += new System.EventHandler(this.numTextPolyAlpha_ValueChanged);
            // 
            // chbTextOutlinePolygon
            // 
            this.chbTextOutlinePolygon.AutoSize = true;
            this.chbTextOutlinePolygon.Checked = true;
            this.chbTextOutlinePolygon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTextOutlinePolygon.Location = new System.Drawing.Point(71, 37);
            this.chbTextOutlinePolygon.Name = "chbTextOutlinePolygon";
            this.chbTextOutlinePolygon.Size = new System.Drawing.Size(120, 16);
            this.chbTextOutlinePolygon.TabIndex = 8;
            this.chbTextOutlinePolygon.Text = "Outline Polygons";
            this.chbTextOutlinePolygon.UseVisualStyleBackColor = true;
            this.chbTextOutlinePolygon.CheckedChanged += new System.EventHandler(this.cbOutlinePolygon_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Fill color:";
            // 
            // btnTextPolyColor
            // 
            this.btnTextPolyColor.BackColor = System.Drawing.Color.Blue;
            this.btnTextPolyColor.Location = new System.Drawing.Point(81, 6);
            this.btnTextPolyColor.Name = "btnTextPolyColor";
            this.btnTextPolyColor.Size = new System.Drawing.Size(32, 21);
            this.btnTextPolyColor.TabIndex = 7;
            this.btnTextPolyColor.UseVisualStyleBackColor = false;
            this.btnTextPolyColor.Click += new System.EventHandler(this.btnFillColor_Click);
            // 
            // tbProjection
            // 
            this.tbProjection.Location = new System.Drawing.Point(110, 369);
            this.tbProjection.Name = "tbProjection";
            this.tbProjection.Size = new System.Drawing.Size(190, 21);
            this.tbProjection.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 372);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "Input projection:";
            // 
            // cbLabelField
            // 
            this.cbLabelField.AllowDrop = true;
            this.cbLabelField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLabelField.Location = new System.Drawing.Point(110, 344);
            this.cbLabelField.MaxDropDownItems = 100;
            this.cbLabelField.Name = "cbLabelField";
            this.cbLabelField.Size = new System.Drawing.Size(190, 20);
            this.cbLabelField.TabIndex = 13;
            this.cbLabelField.SelectedIndexChanged += new System.EventHandler(this.cbLabelField_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 347);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "Select label field:";
            // 
            // btnTextFilter
            // 
            this.btnTextFilter.Enabled = false;
            this.btnTextFilter.Location = new System.Drawing.Point(140, 36);
            this.btnTextFilter.Name = "btnTextFilter";
            this.btnTextFilter.Size = new System.Drawing.Size(75, 21);
            this.btnTextFilter.TabIndex = 14;
            this.btnTextFilter.Text = "Set filter...";
            this.btnTextFilter.UseVisualStyleBackColor = true;
            this.btnTextFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // chbTextFilter
            // 
            this.chbTextFilter.AutoSize = true;
            this.chbTextFilter.Location = new System.Drawing.Point(46, 40);
            this.chbTextFilter.Name = "chbTextFilter";
            this.chbTextFilter.Size = new System.Drawing.Size(96, 16);
            this.chbTextFilter.TabIndex = 15;
            this.chbTextFilter.Text = "Filter input";
            this.chbTextFilter.UseVisualStyleBackColor = true;
            this.chbTextFilter.CheckedChanged += new System.EventHandler(this.chbFilter_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 321);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 12);
            this.label9.TabIndex = 16;
            this.label9.Text = "Layer name:";
            // 
            // tbLayerName
            // 
            this.tbLayerName.Location = new System.Drawing.Point(110, 321);
            this.tbLayerName.Name = "tbLayerName";
            this.tbLayerName.Size = new System.Drawing.Size(190, 21);
            this.tbLayerName.TabIndex = 17;
            this.tbLayerName.Text = "Layer Name";
            this.tbLayerName.TextChanged += new System.EventHandler(this.tbLayerName_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(284, 309);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cbTextKeyField);
            this.tabPage1.Controls.Add(this.btnTextFilter);
            this.tabPage1.Controls.Add(this.pnlPolygon);
            this.tabPage1.Controls.Add(this.pnlLine);
            this.tabPage1.Controls.Add(this.chbTextFilter);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(276, 283);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Text data fields";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.btnNoDataValue);
            this.tabPage2.Controls.Add(this.chbNoDataValue);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.btnNumericFilter);
            this.tabPage2.Controls.Add(this.chbNumericFilter);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.cbNumKeyField);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(276, 283);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Numeric data fields";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel3);
            this.groupBox3.Controls.Add(this.panel4);
            this.groupBox3.Location = new System.Drawing.Point(141, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(125, 168);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Maximum:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label16);
            this.panel3.Controls.Add(this.numNumMaxPolyAlpha);
            this.panel3.Controls.Add(this.chbNumMaxOutlinePoly);
            this.panel3.Controls.Add(this.label17);
            this.panel3.Controls.Add(this.btnNumMaxPolyColor);
            this.panel3.Location = new System.Drawing.Point(3, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(119, 74);
            this.panel3.TabIndex = 19;
            this.panel3.Tag = "";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 33);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(47, 12);
            this.label16.TabIndex = 10;
            this.label16.Text = "Opacity";
            // 
            // numNumMaxPolyAlpha
            // 
            this.numNumMaxPolyAlpha.Location = new System.Drawing.Point(64, 33);
            this.numNumMaxPolyAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMaxPolyAlpha.Name = "numNumMaxPolyAlpha";
            this.numNumMaxPolyAlpha.Size = new System.Drawing.Size(46, 21);
            this.numNumMaxPolyAlpha.TabIndex = 9;
            this.numNumMaxPolyAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMaxPolyAlpha.ValueChanged += new System.EventHandler(this.numNumMaxPolyAlpha_ValueChanged);
            // 
            // chbNumMaxOutlinePoly
            // 
            this.chbNumMaxOutlinePoly.AutoSize = true;
            this.chbNumMaxOutlinePoly.Checked = true;
            this.chbNumMaxOutlinePoly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbNumMaxOutlinePoly.Location = new System.Drawing.Point(9, 55);
            this.chbNumMaxOutlinePoly.Name = "chbNumMaxOutlinePoly";
            this.chbNumMaxOutlinePoly.Size = new System.Drawing.Size(120, 16);
            this.chbNumMaxOutlinePoly.TabIndex = 8;
            this.chbNumMaxOutlinePoly.Text = "Outline Polygons";
            this.chbNumMaxOutlinePoly.UseVisualStyleBackColor = true;
            this.chbNumMaxOutlinePoly.CheckedChanged += new System.EventHandler(this.chbNumMaxOutlinePoly_CheckedChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 9);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(71, 12);
            this.label17.TabIndex = 6;
            this.label17.Text = "Fill color:";
            // 
            // btnNumMaxPolyColor
            // 
            this.btnNumMaxPolyColor.BackColor = System.Drawing.Color.Blue;
            this.btnNumMaxPolyColor.Location = new System.Drawing.Point(78, 5);
            this.btnNumMaxPolyColor.Name = "btnNumMaxPolyColor";
            this.btnNumMaxPolyColor.Size = new System.Drawing.Size(32, 21);
            this.btnNumMaxPolyColor.TabIndex = 7;
            this.btnNumMaxPolyColor.UseVisualStyleBackColor = false;
            this.btnNumMaxPolyColor.Click += new System.EventHandler(this.btnNumMaxPolyColor_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label18);
            this.panel4.Controls.Add(this.numNumMaxLineAlpha);
            this.panel4.Controls.Add(this.label19);
            this.panel4.Controls.Add(this.numNumMaxLineWidth);
            this.panel4.Controls.Add(this.btnNumMaxLineColor);
            this.panel4.Controls.Add(this.label20);
            this.panel4.Location = new System.Drawing.Point(2, 91);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(120, 73);
            this.panel4.TabIndex = 18;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 30);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 12);
            this.label18.TabIndex = 12;
            this.label18.Text = "Opacity";
            // 
            // numNumMaxLineAlpha
            // 
            this.numNumMaxLineAlpha.Location = new System.Drawing.Point(64, 27);
            this.numNumMaxLineAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMaxLineAlpha.Name = "numNumMaxLineAlpha";
            this.numNumMaxLineAlpha.Size = new System.Drawing.Size(46, 21);
            this.numNumMaxLineAlpha.TabIndex = 11;
            this.numNumMaxLineAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMaxLineAlpha.ValueChanged += new System.EventHandler(this.numNumMaxLineAlpha_ValueChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 10);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(71, 12);
            this.label19.TabIndex = 4;
            this.label19.Text = "Line color:";
            // 
            // numNumMaxLineWidth
            // 
            this.numNumMaxLineWidth.DecimalPlaces = 1;
            this.numNumMaxLineWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numNumMaxLineWidth.Location = new System.Drawing.Point(69, 49);
            this.numNumMaxLineWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numNumMaxLineWidth.Name = "numNumMaxLineWidth";
            this.numNumMaxLineWidth.Size = new System.Drawing.Size(42, 21);
            this.numNumMaxLineWidth.TabIndex = 7;
            this.numNumMaxLineWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumMaxLineWidth.ValueChanged += new System.EventHandler(this.numNumMaxLineWidth_ValueChanged);
            // 
            // btnNumMaxLineColor
            // 
            this.btnNumMaxLineColor.BackColor = System.Drawing.Color.Red;
            this.btnNumMaxLineColor.Location = new System.Drawing.Point(78, 6);
            this.btnNumMaxLineColor.Name = "btnNumMaxLineColor";
            this.btnNumMaxLineColor.Size = new System.Drawing.Size(32, 21);
            this.btnNumMaxLineColor.TabIndex = 5;
            this.btnNumMaxLineColor.UseVisualStyleBackColor = false;
            this.btnNumMaxLineColor.Click += new System.EventHandler(this.btnNumMaxLineColor_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(1, 51);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(65, 12);
            this.label20.TabIndex = 6;
            this.label20.Text = "Line width";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Location = new System.Drawing.Point(7, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(127, 168);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Minimum:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.numNumMinPolyAlpha);
            this.panel1.Controls.Add(this.chbNumMinOutlinePoly);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.btnNumMinPolyColor);
            this.panel1.Location = new System.Drawing.Point(4, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(119, 74);
            this.panel1.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 12);
            this.label11.TabIndex = 10;
            this.label11.Text = "Opacity";
            // 
            // numNumMinPolyAlpha
            // 
            this.numNumMinPolyAlpha.Location = new System.Drawing.Point(64, 31);
            this.numNumMinPolyAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMinPolyAlpha.Name = "numNumMinPolyAlpha";
            this.numNumMinPolyAlpha.Size = new System.Drawing.Size(46, 21);
            this.numNumMinPolyAlpha.TabIndex = 9;
            this.numNumMinPolyAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMinPolyAlpha.ValueChanged += new System.EventHandler(this.numNumMinPolyAlpha_ValueChanged);
            // 
            // chbNumMinOutlinePoly
            // 
            this.chbNumMinOutlinePoly.AutoSize = true;
            this.chbNumMinOutlinePoly.Checked = true;
            this.chbNumMinOutlinePoly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbNumMinOutlinePoly.Location = new System.Drawing.Point(9, 55);
            this.chbNumMinOutlinePoly.Name = "chbNumMinOutlinePoly";
            this.chbNumMinOutlinePoly.Size = new System.Drawing.Size(120, 16);
            this.chbNumMinOutlinePoly.TabIndex = 8;
            this.chbNumMinOutlinePoly.Text = "Outline Polygons";
            this.chbNumMinOutlinePoly.UseVisualStyleBackColor = true;
            this.chbNumMinOutlinePoly.CheckedChanged += new System.EventHandler(this.chbNumMinOutlinePoly_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 12);
            this.label12.TabIndex = 6;
            this.label12.Text = "Fill color:";
            // 
            // btnNumMinPolyColor
            // 
            this.btnNumMinPolyColor.BackColor = System.Drawing.Color.Blue;
            this.btnNumMinPolyColor.Location = new System.Drawing.Point(78, 5);
            this.btnNumMinPolyColor.Name = "btnNumMinPolyColor";
            this.btnNumMinPolyColor.Size = new System.Drawing.Size(32, 21);
            this.btnNumMinPolyColor.TabIndex = 7;
            this.btnNumMinPolyColor.UseVisualStyleBackColor = false;
            this.btnNumMinPolyColor.Click += new System.EventHandler(this.btnNumMinPolyColor_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.numNumMinLineAlpha);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.numNumMinLineWidth);
            this.panel2.Controls.Add(this.btnNumMinLineColor);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Location = new System.Drawing.Point(3, 91);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(120, 73);
            this.panel2.TabIndex = 10;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 30);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 12);
            this.label13.TabIndex = 12;
            this.label13.Text = "Opacity";
            // 
            // numNumMinLineAlpha
            // 
            this.numNumMinLineAlpha.Location = new System.Drawing.Point(65, 28);
            this.numNumMinLineAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMinLineAlpha.Name = "numNumMinLineAlpha";
            this.numNumMinLineAlpha.Size = new System.Drawing.Size(46, 21);
            this.numNumMinLineAlpha.TabIndex = 11;
            this.numNumMinLineAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNumMinLineAlpha.ValueChanged += new System.EventHandler(this.numNumMinLineAlpha_ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 12);
            this.label14.TabIndex = 4;
            this.label14.Text = "Line color:";
            // 
            // numNumMinLineWidth
            // 
            this.numNumMinLineWidth.DecimalPlaces = 1;
            this.numNumMinLineWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numNumMinLineWidth.Location = new System.Drawing.Point(69, 49);
            this.numNumMinLineWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numNumMinLineWidth.Name = "numNumMinLineWidth";
            this.numNumMinLineWidth.Size = new System.Drawing.Size(42, 21);
            this.numNumMinLineWidth.TabIndex = 7;
            this.numNumMinLineWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumMinLineWidth.ValueChanged += new System.EventHandler(this.numNumMinLineWidth_ValueChanged);
            // 
            // btnNumMinLineColor
            // 
            this.btnNumMinLineColor.BackColor = System.Drawing.Color.Red;
            this.btnNumMinLineColor.Location = new System.Drawing.Point(78, 6);
            this.btnNumMinLineColor.Name = "btnNumMinLineColor";
            this.btnNumMinLineColor.Size = new System.Drawing.Size(32, 21);
            this.btnNumMinLineColor.TabIndex = 5;
            this.btnNumMinLineColor.UseVisualStyleBackColor = false;
            this.btnNumMinLineColor.Click += new System.EventHandler(this.btnNumMinLineColor_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(1, 51);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 6;
            this.label15.Text = "Line width";
            // 
            // btnNoDataValue
            // 
            this.btnNoDataValue.Enabled = false;
            this.btnNoDataValue.Location = new System.Drawing.Point(151, 50);
            this.btnNoDataValue.Name = "btnNoDataValue";
            this.btnNoDataValue.Size = new System.Drawing.Size(75, 21);
            this.btnNoDataValue.TabIndex = 23;
            this.btnNoDataValue.Text = "Set value...";
            this.btnNoDataValue.UseVisualStyleBackColor = true;
            this.btnNoDataValue.Click += new System.EventHandler(this.btnNoDataValue_Click);
            // 
            // chbNoDataValue
            // 
            this.chbNoDataValue.AutoSize = true;
            this.chbNoDataValue.Location = new System.Drawing.Point(144, 29);
            this.chbNoDataValue.Name = "chbNoDataValue";
            this.chbNoDataValue.Size = new System.Drawing.Size(114, 16);
            this.chbNoDataValue.TabIndex = 24;
            this.chbNoDataValue.Text = "\"no data\" value";
            this.chbNoDataValue.UseVisualStyleBackColor = true;
            this.chbNoDataValue.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbLogDec);
            this.groupBox1.Controls.Add(this.rbLogInc);
            this.groupBox1.Controls.Add(this.rbLinear);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(11, 245);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 35);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scaling";
            // 
            // rbLogDec
            // 
            this.rbLogDec.AutoSize = true;
            this.rbLogDec.Location = new System.Drawing.Point(171, 14);
            this.rbLogDec.Name = "rbLogDec";
            this.rbLogDec.Size = new System.Drawing.Size(71, 16);
            this.rbLogDec.TabIndex = 2;
            this.rbLogDec.TabStop = true;
            this.rbLogDec.Text = "Log Dec.";
            this.rbLogDec.UseVisualStyleBackColor = true;
            // 
            // rbLogInc
            // 
            this.rbLogInc.AutoSize = true;
            this.rbLogInc.Location = new System.Drawing.Point(88, 14);
            this.rbLogInc.Name = "rbLogInc";
            this.rbLogInc.Size = new System.Drawing.Size(71, 16);
            this.rbLogInc.TabIndex = 1;
            this.rbLogInc.TabStop = true;
            this.rbLogInc.Text = "Log Inc.";
            this.rbLogInc.UseVisualStyleBackColor = true;
            // 
            // rbLinear
            // 
            this.rbLinear.AutoSize = true;
            this.rbLinear.Location = new System.Drawing.Point(15, 14);
            this.rbLinear.Name = "rbLinear";
            this.rbLinear.Size = new System.Drawing.Size(59, 16);
            this.rbLinear.TabIndex = 0;
            this.rbLinear.TabStop = true;
            this.rbLinear.Text = "Linear";
            this.rbLinear.UseVisualStyleBackColor = true;
            // 
            // btnNumericFilter
            // 
            this.btnNumericFilter.Enabled = false;
            this.btnNumericFilter.Location = new System.Drawing.Point(31, 50);
            this.btnNumericFilter.Name = "btnNumericFilter";
            this.btnNumericFilter.Size = new System.Drawing.Size(75, 21);
            this.btnNumericFilter.TabIndex = 16;
            this.btnNumericFilter.Text = "Set filter...";
            this.btnNumericFilter.UseVisualStyleBackColor = true;
            this.btnNumericFilter.Click += new System.EventHandler(this.btnNumericFilter_Click);
            // 
            // chbNumericFilter
            // 
            this.chbNumericFilter.AutoSize = true;
            this.chbNumericFilter.Location = new System.Drawing.Point(33, 29);
            this.chbNumericFilter.Name = "chbNumericFilter";
            this.chbNumericFilter.Size = new System.Drawing.Size(96, 16);
            this.chbNumericFilter.TabIndex = 17;
            this.chbNumericFilter.Text = "Filter input";
            this.chbNumericFilter.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "Select key data field:";
            // 
            // cbNumKeyField
            // 
            this.cbNumKeyField.AllowDrop = true;
            this.cbNumKeyField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNumKeyField.Location = new System.Drawing.Point(126, 6);
            this.cbNumKeyField.MaxDropDownItems = 100;
            this.cbNumKeyField.Name = "cbNumKeyField";
            this.cbNumKeyField.Size = new System.Drawing.Size(141, 20);
            this.cbNumKeyField.TabIndex = 3;
            this.cbNumKeyField.SelectedIndexChanged += new System.EventHandler(this.cbNumKeyField_SelectedIndexChanged);
            // 
            // VectorInfoSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(308, 432);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.tbLayerName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cbLabelField);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbProjection);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEnter);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VectorInfoSelector";
            this.Text = "Style data";
            ((System.ComponentModel.ISupportInitialize)(this.numTextLineWidth)).EndInit();
            this.pnlLine.ResumeLayout(false);
            this.pnlLine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTextLineAlpha)).EndInit();
            this.pnlPolygon.ResumeLayout(false);
            this.pnlPolygon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTextPolyAlpha)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMaxPolyAlpha)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMaxLineAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMaxLineWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMinPolyAlpha)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMinLineAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumMinLineWidth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public VectorInfoSelector(Layer ogrLayer)
        {
            InitializeComponent();

            NumericMaxStyle = new VectorStyleParameters();
            NumericMinStyle = new VectorStyleParameters();
            NumericNoDataStyle = new VectorStyleParameters();
            TextStyle = new VectorStyleParameters();

            UpdateParameters();

            string proj4Srs;
            SpatialReference inSrs = ogrLayer.GetSpatialRef();
            if (inSrs != null)
                inSrs.ExportToProj4(out proj4Srs);
            else
                proj4Srs = "(unknown)";

            this.tbProjection.Text = proj4Srs;

            this.cbTextKeyField.Items.Add("<none>");
            this.cbNumKeyField.Items.Add("<none>");

            FeatureDefn featDef = ogrLayer.GetLayerDefn();

            for (int i = 0; i < featDef.GetFieldCount(); i++)
            {
                FieldDefn fieldDef = featDef.GetFieldDefn(i);
                int fieldType = fieldDef.GetFieldType();
                string fieldName = fieldDef.GetName();

                // add to numeric list if necessary
                if (fieldType == ogr.OFTReal || fieldType == ogr.OFTInteger)
                    this.cbNumKeyField.Items.Add(fieldName);

                this.cbTextKeyField.Items.Add(fieldName);
                this.cbLabelField.Items.Add(fieldName);
            }

            this.cbTextKeyField.Text = cbTextKeyField.Items[1].ToString();
            keyFieldName = cbTextKeyField.Text;
            
            this.cbNumKeyField.Text = cbNumKeyField.Items[0].ToString();
            this.cbLabelField.Text = cbLabelField.Items[0].ToString();

        }

        private void UpdateParameters()
        {
            numNumMinPolyAlpha.Value = NumericMinStyle.PolygonColor.A;
            numNumMinLineAlpha.Value = NumericMinStyle.LineColor.A;
            btnNumMinPolyColor.BackColor = NumericMinStyle.PolygonColor;
            btnNumMinLineColor.BackColor = NumericMinStyle.LineColor;
            numNumMinLineWidth.Value = (decimal)NumericMinStyle.LineWidth;
            chbNumMinOutlinePoly.Checked = NumericMinStyle.OutlinePolygons;

            numNumMaxPolyAlpha.Value = NumericMaxStyle.PolygonColor.A;
            numNumMaxLineAlpha.Value = NumericMaxStyle.LineColor.A;
            btnNumMaxPolyColor.BackColor = NumericMaxStyle.PolygonColor;
            btnNumMaxLineColor.BackColor = NumericMaxStyle.LineColor;
            numNumMaxLineWidth.Value = (decimal)NumericMaxStyle.LineWidth;
            chbNumMaxOutlinePoly.Checked = NumericMaxStyle.OutlinePolygons;

            numTextPolyAlpha.Value = TextStyle.PolygonColor.A;
            numTextLineAlpha.Value = TextStyle.LineColor.A;
            btnTextPolyColor.BackColor = TextStyle.PolygonColor;
            btnTextLineColor.BackColor = TextStyle.LineColor;
            numTextLineWidth.Value = (decimal)TextStyle.LineWidth;
            chbTextOutlinePolygon.Checked = TextStyle.OutlinePolygons;

            tbLayerName.Text = TextStyle.Layername;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnColorDialog_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                TextStyle.LineColor = Color.FromArgb((int)numTextLineAlpha.Value, colorDialog.Color);
                UpdateParameters();
            }
        }

        private void cbOutlinePolygon_CheckedChanged(object sender, EventArgs e)
        {
            TextStyle.OutlinePolygons = chbTextOutlinePolygon.Checked;
        }

        private void btnFillColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                TextStyle.PolygonColor = Color.FromArgb((int)numTextPolyAlpha.Value, colorDialog.Color);
                UpdateParameters();
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            TextFilterDialog tfd = new TextFilterDialog();
            if (tfd.ShowDialog() == DialogResult.OK)
            {
                TextStyle.TextFilterString = tfd.FilterString;
                TextStyle.TextFilterType = tfd.FilterType;
            }
        }


        private void chbFilter_CheckedChanged(object sender, EventArgs e)
        {
            btnTextFilter.Enabled = chbTextFilter.Checked;
            TextStyle.TextFilter = chbTextFilter.Checked;
        }

        private void cbTextKeyField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTextKeyField.SelectedIndex != 0)
            {
                cbNumKeyField.SelectedIndex = 0;
                TextStyle.DataType = DataType.Text;
                TextStyle.KeyDataFieldName = cbTextKeyField.Text;
            }
        }

        private void cbNumKeyField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbNumKeyField.SelectedIndex != 0)
            {
                cbTextKeyField.SelectedIndex = 0;
                NumericMinStyle.DataType = DataType.Numeric;
                NumericMinStyle.KeyDataFieldName = cbNumKeyField.Text;
            }
        }

        private void numTextPolyAlpha_ValueChanged(object sender, EventArgs e)
        {
            TextStyle.PolygonColor = Color.FromArgb((int)numTextPolyAlpha.Value, TextStyle.PolygonColor);
            UpdateParameters();
        }

        private void numTextLineAlpha_ValueChanged(object sender, EventArgs e)
        {
            TextStyle.LineColor = Color.FromArgb((int)numTextLineAlpha.Value, TextStyle.LineColor);
            UpdateParameters();
        }


        private void btnNumMinPolyColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                NumericMinStyle.PolygonColor = Color.FromArgb((int)numNumMinPolyAlpha.Value, colorDialog.Color);
                UpdateParameters();
            }
        }

        private void btnNumMaxPolyColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                NumericMaxStyle.PolygonColor = Color.FromArgb((int)numNumMaxPolyAlpha.Value, colorDialog.Color);
                UpdateParameters();
            }
        }

        private void btnNumMinLineColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                NumericMinStyle.LineColor = Color.FromArgb((int)numNumMinLineAlpha.Value, colorDialog.Color);
                UpdateParameters();
            }
        }

        private void btnNumMaxLineColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                NumericMaxStyle.LineColor = Color.FromArgb((int)numNumMaxLineAlpha.Value, colorDialog.Color);
                UpdateParameters();
            }
        }

        private void btnNoDataValue_Click(object sender, EventArgs e)
        {
            NoDataForm ndf = new NoDataForm(NumericNoDataStyle);
            if (ndf.ShowDialog() == DialogResult.OK)
            {
                NumericNoDataStyle = ndf.NoDataStyle;
                NumericNoDataStyle.NoData = true;
            }
        }

        private void btnNumericFilter_Click(object sender, EventArgs e)
        {
            //numeric filter parameters picked here
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            btnNoDataValue.Enabled = chbNoDataValue.Checked;
        }

        private void chbNumMinOutlinePoly_CheckedChanged(object sender, EventArgs e)
        {
            NumericMinStyle.OutlinePolygons = chbNumMinOutlinePoly.Checked;
        }

        private void chbNumMaxOutlinePoly_CheckedChanged(object sender, EventArgs e)
        {
            NumericMaxStyle.OutlinePolygons = chbNumMaxOutlinePoly.Checked;
        }

        private void numNumMinPolyAlpha_ValueChanged(object sender, EventArgs e)
        {
            NumericMinStyle.PolygonColor = Color.FromArgb((int)numNumMinPolyAlpha.Value, NumericMinStyle.PolygonColor);
            UpdateParameters();
        }

        private void numNumMaxPolyAlpha_ValueChanged(object sender, EventArgs e)
        {
            NumericMaxStyle.PolygonColor = Color.FromArgb((int)numNumMaxPolyAlpha.Value, NumericMaxStyle.PolygonColor);
            UpdateParameters();
        }

        private void numNumMinLineAlpha_ValueChanged(object sender, EventArgs e)
        {
            NumericMinStyle.LineColor = Color.FromArgb((int)numNumMinLineAlpha.Value, NumericMinStyle.LineColor);
            UpdateParameters();
        }

        private void numNumMaxLineAlpha_ValueChanged(object sender, EventArgs e)
        {
            NumericMaxStyle.LineColor = Color.FromArgb((int)numNumMaxLineAlpha.Value, NumericMaxStyle.LineColor);
            UpdateParameters();
        }

        private void numNumMinLineWidth_ValueChanged(object sender, EventArgs e)
        {
            NumericMinStyle.LineWidth = (float)numNumMinLineWidth.Value;
        }

        private void numNumMaxLineWidth_ValueChanged(object sender, EventArgs e)
        {
            NumericMaxStyle.LineWidth = (float)numNumMaxLineWidth.Value;
        }


        private void numTextLineWidth_ValueChanged(object sender, EventArgs e)
        {
            TextStyle.LineWidth = (float)numTextLineWidth.Value;
        }

        private void tbLayerName_TextChanged(object sender, EventArgs e)
        {
            TextStyle.Layername = tbLayerName.Text;
        }

        private void cbLabelField_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextStyle.LabelFieldName = cbLabelField.Text;
        }

        #region Public Properties

        public string Projection
        {
            get { return tbProjection.Text; }
        }
        #endregion
    }
}
