namespace OgrVectorImporter
{
    partial class FilterDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbExactMatch = new System.Windows.Forms.RadioButton();
            this.rbContains = new System.Windows.Forms.RadioButton();
            this.rbRegex = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbFilterString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(68, 106);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(149, 106);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // rbExactMatch
            // 
            this.rbExactMatch.AutoSize = true;
            this.rbExactMatch.Checked = true;
            this.rbExactMatch.Location = new System.Drawing.Point(6, 9);
            this.rbExactMatch.Name = "rbExactMatch";
            this.rbExactMatch.Size = new System.Drawing.Size(52, 17);
            this.rbExactMatch.TabIndex = 2;
            this.rbExactMatch.TabStop = true;
            this.rbExactMatch.Text = "Exact";
            this.rbExactMatch.UseVisualStyleBackColor = true;
            // 
            // rbContains
            // 
            this.rbContains.AutoSize = true;
            this.rbContains.Location = new System.Drawing.Point(6, 32);
            this.rbContains.Name = "rbContains";
            this.rbContains.Size = new System.Drawing.Size(66, 17);
            this.rbContains.TabIndex = 3;
            this.rbContains.TabStop = true;
            this.rbContains.Text = "Contains";
            this.rbContains.UseVisualStyleBackColor = true;
            // 
            // rbRegex
            // 
            this.rbRegex.AutoSize = true;
            this.rbRegex.Location = new System.Drawing.Point(6, 55);
            this.rbRegex.Name = "rbRegex";
            this.rbRegex.Size = new System.Drawing.Size(57, 17);
            this.rbRegex.TabIndex = 4;
            this.rbRegex.TabStop = true;
            this.rbRegex.Text = "RegEx";
            this.rbRegex.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbRegex);
            this.groupBox1.Controls.Add(this.rbContains);
            this.groupBox1.Controls.Add(this.rbExactMatch);
            this.groupBox1.Location = new System.Drawing.Point(201, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(79, 78);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // tbFilterString
            // 
            this.tbFilterString.Location = new System.Drawing.Point(14, 48);
            this.tbFilterString.Name = "tbFilterString";
            this.tbFilterString.Size = new System.Drawing.Size(181, 20);
            this.tbFilterString.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "If key data matches:";
            // 
            // FilterDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 140);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbFilterString);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.Name = "FilterDialog";
            this.Text = "Filter data";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbExactMatch;
        private System.Windows.Forms.RadioButton rbContains;
        private System.Windows.Forms.RadioButton rbRegex;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbFilterString;
        private System.Windows.Forms.Label label1;
    }
}