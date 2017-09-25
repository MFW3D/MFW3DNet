namespace WorldWind.GeoRSS
{
    partial class GeoRSSFeedControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.feedDataGridView = new System.Windows.Forms.DataGridView();
            this.Icon = new System.Windows.Forms.DataGridViewImageColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.interval = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastUpdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updateNow = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.addGroupBox = new System.Windows.Forms.GroupBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.iconTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.feedDataGridView)).BeginInit();
            this.addGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(47, 13);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(338, 20);
            this.nameTextBox.TabIndex = 1;
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(339, 83);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(46, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // feedDataGridView
            // 
            this.feedDataGridView.AllowUserToAddRows = false;
            this.feedDataGridView.AllowUserToDeleteRows = false;
            this.feedDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.feedDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.feedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.feedDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Icon,
            this.name,
            this.url,
            this.interval,
            this.lastUpdate,
            this.updateNow});
            this.feedDataGridView.Location = new System.Drawing.Point(7, 138);
            this.feedDataGridView.MultiSelect = false;
            this.feedDataGridView.Name = "feedDataGridView";
            this.feedDataGridView.ReadOnly = true;
            this.feedDataGridView.RowHeadersVisible = false;
            this.feedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.feedDataGridView.Size = new System.Drawing.Size(384, 200);
            this.feedDataGridView.TabIndex = 3;
            // 
            // Icon
            // 
            this.Icon.HeaderText = "Icon";
            this.Icon.Name = "Icon";
            this.Icon.ReadOnly = true;
            this.Icon.Width = 36;
            // 
            // name
            // 
            this.name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.name.HeaderText = "Name";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // url
            // 
            this.url.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.url.HeaderText = "URL";
            this.url.Name = "url";
            this.url.ReadOnly = true;
            // 
            // interval
            // 
            this.interval.HeaderText = "Refresh Interval";
            this.interval.Name = "interval";
            this.interval.ReadOnly = true;
            this.interval.Width = 80;
            // 
            // lastUpdate
            // 
            this.lastUpdate.HeaderText = "Last Update";
            this.lastUpdate.Name = "lastUpdate";
            this.lastUpdate.ReadOnly = true;
            this.lastUpdate.Width = 80;
            // 
            // updateNow
            // 
            this.updateNow.HeaderText = "Update";
            this.updateNow.Name = "updateNow";
            this.updateNow.ReadOnly = true;
            this.updateNow.Width = 50;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Feeds";
            // 
            // addGroupBox
            // 
            this.addGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.addGroupBox.Controls.Add(this.browseButton);
            this.addGroupBox.Controls.Add(this.iconTextBox);
            this.addGroupBox.Controls.Add(this.label4);
            this.addGroupBox.Controls.Add(this.urlTextBox);
            this.addGroupBox.Controls.Add(this.label3);
            this.addGroupBox.Controls.Add(this.label1);
            this.addGroupBox.Controls.Add(this.nameTextBox);
            this.addGroupBox.Controls.Add(this.addButton);
            this.addGroupBox.Location = new System.Drawing.Point(3, 3);
            this.addGroupBox.Name = "addGroupBox";
            this.addGroupBox.Size = new System.Drawing.Size(391, 112);
            this.addGroupBox.TabIndex = 5;
            this.addGroupBox.TabStop = false;
            this.addGroupBox.Text = "Add Feed";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(356, 57);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(28, 21);
            this.browseButton.TabIndex = 7;
            this.browseButton.Text = "...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // iconTextBox
            // 
            this.iconTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.iconTextBox.Location = new System.Drawing.Point(47, 58);
            this.iconTextBox.Name = "iconTextBox";
            this.iconTextBox.Size = new System.Drawing.Size(304, 20);
            this.iconTextBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Icon";
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.Location = new System.Drawing.Point(47, 36);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(338, 20);
            this.urlTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "URL";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // GeoRSSFeedControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.addGroupBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.feedDataGridView);
            this.Name = "GeoRSSFeedControl";
            this.Size = new System.Drawing.Size(397, 346);
            ((System.ComponentModel.ISupportInitialize)(this.feedDataGridView)).EndInit();
            this.addGroupBox.ResumeLayout(false);
            this.addGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.DataGridView feedDataGridView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox addGroupBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox iconTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewImageColumn Icon;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn url;
        private System.Windows.Forms.DataGridViewTextBoxColumn interval;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdate;
        private System.Windows.Forms.DataGridViewButtonColumn updateNow;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}
