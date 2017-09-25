namespace Collab.jhuapl.Whiteboard
{
    partial class HotspotForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HotspotForm));
            this.label1 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.description = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cameraAngleCheckBox = new System.Windows.Forms.CheckBox();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hotspot Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(93, 7);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(220, 20);
            this.name.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description";
            // 
            // description
            // 
            this.description.Location = new System.Drawing.Point(93, 35);
            this.description.Multiline = true;
            this.description.Name = "description";
            this.description.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.description.Size = new System.Drawing.Size(220, 62);
            this.description.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(253, 135);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 20);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.OK_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(187, 135);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(62, 20);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // cameraAngleCheckBox
            // 
            this.cameraAngleCheckBox.AutoSize = true;
            this.cameraAngleCheckBox.Location = new System.Drawing.Point(10, 138);
            this.cameraAngleCheckBox.Name = "cameraAngleCheckBox";
            this.cameraAngleCheckBox.Size = new System.Drawing.Size(120, 17);
            this.cameraAngleCheckBox.TabIndex = 6;
            this.cameraAngleCheckBox.Text = "Save Camera Angle";
            this.cameraAngleCheckBox.UseVisualStyleBackColor = true;
            // 
            // urlTextBox
            // 
            this.urlTextBox.Location = new System.Drawing.Point(93, 106);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(220, 20);
            this.urlTextBox.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "URL";
            // 
            // HotspotForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(321, 164);
            this.Controls.Add(this.urlTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cameraAngleCheckBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.description);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HotspotForm";
            this.Text = "Add Hotspot";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.CheckBox cameraAngleCheckBox;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label label3;
    }
}