namespace OgrVectorImporter
{
    partial class NoDataForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.pnlPolygon = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.numNoDataPolyAlpha = new System.Windows.Forms.NumericUpDown();
            this.chbNoDataOutlinePolygon = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnNoDataPolyColor = new System.Windows.Forms.Button();
            this.pnlLine = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.numNoDataLineAlpha = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numNoDataLineWidth = new System.Windows.Forms.NumericUpDown();
            this.btnNoDataLineColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.pnlPolygon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoDataPolyAlpha)).BeginInit();
            this.pnlLine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoDataLineAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNoDataLineWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Value representing \"no data\":";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(93, 31);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(60, 21);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // pnlPolygon
            // 
            this.pnlPolygon.Controls.Add(this.label5);
            this.pnlPolygon.Controls.Add(this.numNoDataPolyAlpha);
            this.pnlPolygon.Controls.Add(this.chbNoDataOutlinePolygon);
            this.pnlPolygon.Controls.Add(this.label4);
            this.pnlPolygon.Controls.Add(this.btnNoDataPolyColor);
            this.pnlPolygon.Location = new System.Drawing.Point(9, 67);
            this.pnlPolygon.Name = "pnlPolygon";
            this.pnlPolygon.Size = new System.Drawing.Size(228, 60);
            this.pnlPolygon.TabIndex = 11;
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
            // numNoDataPolyAlpha
            // 
            this.numNoDataPolyAlpha.Location = new System.Drawing.Point(171, 9);
            this.numNoDataPolyAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNoDataPolyAlpha.Name = "numNoDataPolyAlpha";
            this.numNoDataPolyAlpha.Size = new System.Drawing.Size(46, 21);
            this.numNoDataPolyAlpha.TabIndex = 9;
            this.numNoDataPolyAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNoDataPolyAlpha.ValueChanged += new System.EventHandler(this.numNoDataPolyAlpha_ValueChanged);
            // 
            // chbNoDataOutlinePolygon
            // 
            this.chbNoDataOutlinePolygon.AutoSize = true;
            this.chbNoDataOutlinePolygon.Checked = true;
            this.chbNoDataOutlinePolygon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbNoDataOutlinePolygon.Location = new System.Drawing.Point(71, 37);
            this.chbNoDataOutlinePolygon.Name = "chbNoDataOutlinePolygon";
            this.chbNoDataOutlinePolygon.Size = new System.Drawing.Size(120, 16);
            this.chbNoDataOutlinePolygon.TabIndex = 8;
            this.chbNoDataOutlinePolygon.Text = "Outline Polygons";
            this.chbNoDataOutlinePolygon.UseVisualStyleBackColor = true;
            this.chbNoDataOutlinePolygon.CheckedChanged += new System.EventHandler(this.chbNoDataOutlinePolygon_CheckedChanged);
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
            // btnNoDataPolyColor
            // 
            this.btnNoDataPolyColor.BackColor = System.Drawing.Color.Blue;
            this.btnNoDataPolyColor.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNoDataPolyColor.Location = new System.Drawing.Point(78, 7);
            this.btnNoDataPolyColor.Name = "btnNoDataPolyColor";
            this.btnNoDataPolyColor.Size = new System.Drawing.Size(32, 21);
            this.btnNoDataPolyColor.TabIndex = 7;
            this.btnNoDataPolyColor.UseVisualStyleBackColor = false;
            this.btnNoDataPolyColor.Click += new System.EventHandler(this.btnNoDataPolyColor_Click);
            // 
            // pnlLine
            // 
            this.pnlLine.Controls.Add(this.label6);
            this.pnlLine.Controls.Add(this.numNoDataLineAlpha);
            this.pnlLine.Controls.Add(this.label2);
            this.pnlLine.Controls.Add(this.numNoDataLineWidth);
            this.pnlLine.Controls.Add(this.btnNoDataLineColor);
            this.pnlLine.Controls.Add(this.label3);
            this.pnlLine.Location = new System.Drawing.Point(9, 138);
            this.pnlLine.Name = "pnlLine";
            this.pnlLine.Size = new System.Drawing.Size(228, 70);
            this.pnlLine.TabIndex = 10;
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
            // numNoDataLineAlpha
            // 
            this.numNoDataLineAlpha.Location = new System.Drawing.Point(173, 8);
            this.numNoDataLineAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNoDataLineAlpha.Name = "numNoDataLineAlpha";
            this.numNoDataLineAlpha.Size = new System.Drawing.Size(46, 21);
            this.numNoDataLineAlpha.TabIndex = 11;
            this.numNoDataLineAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numNoDataLineAlpha.ValueChanged += new System.EventHandler(this.numNoDataLineAlpha_ValueChanged);
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
            // numNoDataLineWidth
            // 
            this.numNoDataLineWidth.DecimalPlaces = 1;
            this.numNoDataLineWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numNoDataLineWidth.Location = new System.Drawing.Point(114, 37);
            this.numNoDataLineWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numNoDataLineWidth.Name = "numNoDataLineWidth";
            this.numNoDataLineWidth.Size = new System.Drawing.Size(54, 21);
            this.numNoDataLineWidth.TabIndex = 7;
            this.numNoDataLineWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNoDataLineWidth.ValueChanged += new System.EventHandler(this.numNoDataLineWidth_ValueChanged);
            // 
            // btnNoDataLineColor
            // 
            this.btnNoDataLineColor.BackColor = System.Drawing.Color.Red;
            this.btnNoDataLineColor.Location = new System.Drawing.Point(78, 6);
            this.btnNoDataLineColor.Name = "btnNoDataLineColor";
            this.btnNoDataLineColor.Size = new System.Drawing.Size(32, 21);
            this.btnNoDataLineColor.TabIndex = 5;
            this.btnNoDataLineColor.UseVisualStyleBackColor = false;
            this.btnNoDataLineColor.Click += new System.EventHandler(this.btnNoDataLineColor_Click);
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
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(44, 236);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 21);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(127, 236);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // NoDataForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(246, 275);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pnlPolygon);
            this.Controls.Add(this.pnlLine);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "NoDataForm";
            this.Text = "NoDataForm";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.pnlPolygon.ResumeLayout(false);
            this.pnlPolygon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoDataPolyAlpha)).EndInit();
            this.pnlLine.ResumeLayout(false);
            this.pnlLine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoDataLineAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNoDataLineWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Panel pnlPolygon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numNoDataPolyAlpha;
        private System.Windows.Forms.CheckBox chbNoDataOutlinePolygon;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnNoDataPolyColor;
        private System.Windows.Forms.Panel pnlLine;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numNoDataLineAlpha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numNoDataLineWidth;
        private System.Windows.Forms.Button btnNoDataLineColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}