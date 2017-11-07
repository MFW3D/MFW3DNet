//----------------------------------------------------------------------------
// NAME: Movie Recorder
// VERSION: 1.4
// DESCRIPTION: Movie recording script.  Adds itself to Plugins menu.  
// DEVELOPER: Bjorn Reppen aka "Mashi". UI improvements by Isaac Mann.
// WEBSITE: http://www.mashiharu.com, http://www.apogee.com.au
//----------------------------------------------------------------------------
//
// This file is in the Public Domain, and comes with no warranty. 
//
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System;
using WorldWind.Camera;
using WorldWind;
using WorldWind.Net;
using WorldWind.PluginEngine;
using Utility;
using AviFile;

namespace Apogee.MovieCreator
{
    /// <summary>
    /// Movie Recorder dialog
    /// </summary>
    public class MovieRecorderDialog : System.Windows.Forms.Form
    {
        #region Private variables
        private static string WINDOW = "window";
        private static string PREVIEW = "preview";
        
        private AviManager aviManager;
        private DirectoryInfo pngDirInfo;
        private Plugin plugin;
        private VideoStream aviStream;
        private WorldWindow worldWindow;

        private bool doMovie = false;
        private Size previousSize;
        private string lastSizing;

        private System.Windows.Forms.Button buttonBrowseScript;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonRecord;
        private System.Windows.Forms.Button buttonStop;
        private IContainer components;

        PathCamera camera;
        private System.Windows.Forms.TextBox scriptFileTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label5;

        /// <summary>
        /// Height of rendered images (pixels).
        /// </summary>
        private System.Windows.Forms.NumericUpDown frameHeight;
        /// <summary>
        /// Width of rendered images (pixels).
        /// </summary>
        private System.Windows.Forms.NumericUpDown frameWidth;
        private System.Windows.Forms.NumericUpDown frameWaitTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown frameEnd;
        private System.Windows.Forms.NumericUpDown frameStart;
        private System.Windows.Forms.Label outputPngLabel;
        private System.Windows.Forms.TextBox outputPngTextBox;
        private System.Windows.Forms.Button buttonEdit;
        private Button buttonBrowsePngOutput;
        private Button buttonPreview;
        private Button buttonWindow;
        private Button buttonUndo;
        private Button convertButton;
        private Button buttonBrowseMovieOutput;
        private Label outputMovieLabel;
        private TextBox outputMovieTextBox;
        private Label frameRateLabel;
        private TextBox frameRateTextBox;
        private Label perSecLabel;
        private ToolTip toolTip1;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        #endregion

        public string movieFileName;
        public string imageFileNames = "_{0:0000}.png";

        public MovieRecorderDialog(Plugin plugin)
        {
            InitializeComponent();
            this.plugin = plugin;
            this.worldWindow = Global.worldWindow;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.components = new System.ComponentModel.Container();
            this.scriptFileTextBox = new System.Windows.Forms.TextBox();
            this.buttonBrowseScript = new System.Windows.Forms.Button();
            this.frameWaitTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.frameWidth = new System.Windows.Forms.NumericUpDown();
            this.frameHeight = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.frameEnd = new System.Windows.Forms.NumericUpDown();
            this.frameStart = new System.Windows.Forms.NumericUpDown();
            this.outputPngLabel = new System.Windows.Forms.Label();
            this.outputPngTextBox = new System.Windows.Forms.TextBox();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonBrowsePngOutput = new System.Windows.Forms.Button();
            this.convertButton = new System.Windows.Forms.Button();
            this.buttonBrowseMovieOutput = new System.Windows.Forms.Button();
            this.outputMovieLabel = new System.Windows.Forms.Label();
            this.outputMovieTextBox = new System.Windows.Forms.TextBox();
            this.frameRateLabel = new System.Windows.Forms.Label();
            this.frameRateTextBox = new System.Windows.Forms.TextBox();
            this.perSecLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonPreview = new System.Windows.Forms.Button();
            this.buttonWindow = new System.Windows.Forms.Button();
            this.buttonUndo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.frameWaitTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameStart)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // scriptFileTextBox
            // 
            this.scriptFileTextBox.Location = new System.Drawing.Point(6, 29);
            this.scriptFileTextBox.Name = "scriptFileTextBox";
            this.scriptFileTextBox.Size = new System.Drawing.Size(227, 21);
            this.scriptFileTextBox.TabIndex = 1;
            this.scriptFileTextBox.Text = "MovieScript.sc";
            this.toolTip1.SetToolTip(this.scriptFileTextBox, "Location of your Movie Recorder script file.");
            // 
            // buttonBrowseScript
            // 
            this.buttonBrowseScript.ForeColor = System.Drawing.Color.Black;
            this.buttonBrowseScript.Location = new System.Drawing.Point(240, 28);
            this.buttonBrowseScript.Name = "buttonBrowseScript";
            this.buttonBrowseScript.Size = new System.Drawing.Size(90, 23);
            this.buttonBrowseScript.TabIndex = 2;
            this.buttonBrowseScript.Text = "&Browse";
            this.toolTip1.SetToolTip(this.buttonBrowseScript, "Browse computer for script file.");
            this.buttonBrowseScript.Click += new System.EventHandler(this.buttonBrowseScript_Click);
            // 
            // frameWaitTime
            // 
            this.frameWaitTime.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.frameWaitTime.Location = new System.Drawing.Point(98, 9);
            this.frameWaitTime.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.frameWaitTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameWaitTime.Name = "frameWaitTime";
            this.frameWaitTime.Size = new System.Drawing.Size(77, 21);
            this.frameWaitTime.TabIndex = 4;
            this.toolTip1.SetToolTip(this.frameWaitTime, "Time between screen captures.  If you are using an external screen capture progra" +
        "m use a low value for smooth playback.");
            this.frameWaitTime.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(7, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pre-roll time:";
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(179, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "ms";
            // 
            // buttonRecord
            // 
            this.buttonRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRecord.Enabled = false;
            this.buttonRecord.ForeColor = System.Drawing.Color.Black;
            this.buttonRecord.Location = new System.Drawing.Point(122, 198);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(96, 25);
            this.buttonRecord.TabIndex = 11;
            this.buttonRecord.Text = "&Record";
            this.toolTip1.SetToolTip(this.buttonRecord, "Record movie as a series of image files, a high Pre-roll time may increase qualit" +
        "y.");
            this.buttonRecord.Click += new System.EventHandler(this.buttonRecord_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlay.Enabled = false;
            this.buttonPlay.ForeColor = System.Drawing.Color.Black;
            this.buttonPlay.Location = new System.Drawing.Point(11, 198);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(96, 25);
            this.buttonPlay.TabIndex = 10;
            this.buttonPlay.Text = "&Play";
            this.toolTip1.SetToolTip(this.buttonPlay, "Play script, a large Pre-roll time will result in jerky motion.");
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.Enabled = false;
            this.buttonStop.ForeColor = System.Drawing.Color.Black;
            this.buttonStop.Location = new System.Drawing.Point(234, 198);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(96, 25);
            this.buttonStop.TabIndex = 12;
            this.buttonStop.Text = "&Stop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(6, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Script file:";
            // 
            // frameWidth
            // 
            this.frameWidth.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.frameWidth.Location = new System.Drawing.Point(133, 83);
            this.frameWidth.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.frameWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameWidth.Name = "frameWidth";
            this.frameWidth.Size = new System.Drawing.Size(67, 21);
            this.frameWidth.TabIndex = 7;
            this.toolTip1.SetToolTip(this.frameWidth, "Height.");
            this.frameWidth.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
            // 
            // frameHeight
            // 
            this.frameHeight.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.frameHeight.Location = new System.Drawing.Point(234, 83);
            this.frameHeight.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.frameHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameHeight.Name = "frameHeight";
            this.frameHeight.Size = new System.Drawing.Size(67, 21);
            this.frameHeight.TabIndex = 9;
            this.toolTip1.SetToolTip(this.frameHeight, "Width.");
            this.frameHeight.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(208, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "X";
            // 
            // label6
            // 
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(7, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(130, 19);
            this.label6.TabIndex = 6;
            this.label6.Text = "Frame dimensions:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Script files|*.sc";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(7, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Frame range:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(175, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 18);
            this.label7.TabIndex = 15;
            this.label7.Text = "-";
            // 
            // frameEnd
            // 
            this.frameEnd.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.frameEnd.Location = new System.Drawing.Point(202, 45);
            this.frameEnd.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.frameEnd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameEnd.Name = "frameEnd";
            this.frameEnd.Size = new System.Drawing.Size(67, 21);
            this.frameEnd.TabIndex = 16;
            this.toolTip1.SetToolTip(this.frameEnd, "End frame.");
            this.frameEnd.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // frameStart
            // 
            this.frameStart.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.frameStart.Location = new System.Drawing.Point(101, 45);
            this.frameStart.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.frameStart.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameStart.Name = "frameStart";
            this.frameStart.Size = new System.Drawing.Size(67, 21);
            this.frameStart.TabIndex = 14;
            this.toolTip1.SetToolTip(this.frameStart, "Start frame");
            this.frameStart.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // outputPngLabel
            // 
            this.outputPngLabel.ForeColor = System.Drawing.Color.Black;
            this.outputPngLabel.Location = new System.Drawing.Point(7, 150);
            this.outputPngLabel.Name = "outputPngLabel";
            this.outputPngLabel.Size = new System.Drawing.Size(114, 17);
            this.outputPngLabel.TabIndex = 17;
            this.outputPngLabel.Text = "Output PNG files:";
            // 
            // outputPngTextBox
            // 
            this.outputPngTextBox.Location = new System.Drawing.Point(12, 170);
            this.outputPngTextBox.Name = "outputPngTextBox";
            this.outputPngTextBox.Size = new System.Drawing.Size(215, 21);
            this.outputPngTextBox.TabIndex = 18;
            this.outputPngTextBox.Text = "movie_{0:0000}.png";
            this.toolTip1.SetToolTip(this.outputPngTextBox, "Location of saved images.  Use an application such as Virtualdub to create an avi" +
        ".");
            // 
            // buttonEdit
            // 
            this.buttonEdit.Enabled = false;
            this.buttonEdit.ForeColor = System.Drawing.Color.Black;
            this.buttonEdit.Location = new System.Drawing.Point(240, 57);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(90, 23);
            this.buttonEdit.TabIndex = 19;
            this.buttonEdit.Text = "&Edit";
            this.toolTip1.SetToolTip(this.buttonEdit, "Opens script in text editor.");
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonBrowsePngOutput
            // 
            this.buttonBrowsePngOutput.ForeColor = System.Drawing.Color.Black;
            this.buttonBrowsePngOutput.Location = new System.Drawing.Point(234, 169);
            this.buttonBrowsePngOutput.Name = "buttonBrowsePngOutput";
            this.buttonBrowsePngOutput.Size = new System.Drawing.Size(96, 23);
            this.buttonBrowsePngOutput.TabIndex = 23;
            this.buttonBrowsePngOutput.Text = "&Browse";
            this.toolTip1.SetToolTip(this.buttonBrowsePngOutput, "Browse for directory.");
            this.buttonBrowsePngOutput.Click += new System.EventHandler(this.buttonBrowseOutput_Click);
            // 
            // convertButton
            // 
            this.convertButton.Enabled = false;
            this.convertButton.ForeColor = System.Drawing.Color.Black;
            this.convertButton.Location = new System.Drawing.Point(7, 84);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(323, 25);
            this.convertButton.TabIndex = 24;
            this.convertButton.Text = "Convert to movie";
            this.toolTip1.SetToolTip(this.convertButton, "Convert current images to a raw avi file.  Note file can be a maximum of 2GB.");
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // buttonBrowseMovieOutput
            // 
            this.buttonBrowseMovieOutput.ForeColor = System.Drawing.Color.Black;
            this.buttonBrowseMovieOutput.Location = new System.Drawing.Point(240, 25);
            this.buttonBrowseMovieOutput.Name = "buttonBrowseMovieOutput";
            this.buttonBrowseMovieOutput.Size = new System.Drawing.Size(90, 22);
            this.buttonBrowseMovieOutput.TabIndex = 27;
            this.buttonBrowseMovieOutput.Text = "&Browse";
            this.toolTip1.SetToolTip(this.buttonBrowseMovieOutput, "Browse for location to save avi.");
            this.buttonBrowseMovieOutput.Click += new System.EventHandler(this.buttonBrowseMovieOutput_Click);
            // 
            // outputMovieLabel
            // 
            this.outputMovieLabel.ForeColor = System.Drawing.Color.Black;
            this.outputMovieLabel.Location = new System.Drawing.Point(4, 4);
            this.outputMovieLabel.Name = "outputMovieLabel";
            this.outputMovieLabel.Size = new System.Drawing.Size(129, 18);
            this.outputMovieLabel.TabIndex = 25;
            this.outputMovieLabel.Text = "Output movie file:";
            // 
            // outputMovieTextBox
            // 
            this.outputMovieTextBox.Location = new System.Drawing.Point(4, 25);
            this.outputMovieTextBox.Name = "outputMovieTextBox";
            this.outputMovieTextBox.Size = new System.Drawing.Size(229, 21);
            this.outputMovieTextBox.TabIndex = 26;
            this.outputMovieTextBox.Text = "movie.avi";
            this.toolTip1.SetToolTip(this.outputMovieTextBox, "Location to save movie file, this will be a large avi file and may be clipped at " +
        "2GB.");
            // 
            // frameRateLabel
            // 
            this.frameRateLabel.AutoSize = true;
            this.frameRateLabel.ForeColor = System.Drawing.Color.Black;
            this.frameRateLabel.Location = new System.Drawing.Point(4, 59);
            this.frameRateLabel.Name = "frameRateLabel";
            this.frameRateLabel.Size = new System.Drawing.Size(71, 12);
            this.frameRateLabel.TabIndex = 28;
            this.frameRateLabel.Text = "Frame Rate:";
            // 
            // frameRateTextBox
            // 
            this.frameRateTextBox.Location = new System.Drawing.Point(89, 56);
            this.frameRateTextBox.Name = "frameRateTextBox";
            this.frameRateTextBox.Size = new System.Drawing.Size(29, 21);
            this.frameRateTextBox.TabIndex = 29;
            this.frameRateTextBox.Text = "25";
            this.toolTip1.SetToolTip(this.frameRateTextBox, "Frame rate for avi.  Note this has no effect on saved images.");
            // 
            // perSecLabel
            // 
            this.perSecLabel.AutoSize = true;
            this.perSecLabel.ForeColor = System.Drawing.Color.Black;
            this.perSecLabel.Location = new System.Drawing.Point(125, 59);
            this.perSecLabel.Name = "perSecLabel";
            this.perSecLabel.Size = new System.Drawing.Size(107, 12);
            this.perSecLabel.TabIndex = 30;
            this.perSecLabel.Text = "frames per second";
            // 
            // buttonPreview
            // 
            this.buttonPreview.ForeColor = System.Drawing.Color.Black;
            this.buttonPreview.Location = new System.Drawing.Point(140, 111);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(96, 25);
            this.buttonPreview.TabIndex = 24;
            this.buttonPreview.Text = "Preview Size";
            this.toolTip1.SetToolTip(this.buttonPreview, "Browse for directory.");
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            // 
            // buttonWindow
            // 
            this.buttonWindow.ForeColor = System.Drawing.Color.Black;
            this.buttonWindow.Location = new System.Drawing.Point(11, 111);
            this.buttonWindow.Name = "buttonWindow";
            this.buttonWindow.Size = new System.Drawing.Size(122, 25);
            this.buttonWindow.TabIndex = 25;
            this.buttonWindow.Text = "Get Window Size";
            this.toolTip1.SetToolTip(this.buttonWindow, "Browse for directory.");
            this.buttonWindow.Click += new System.EventHandler(this.buttonWindow_Click);
            // 
            // buttonUndo
            // 
            this.buttonUndo.ForeColor = System.Drawing.Color.Black;
            this.buttonUndo.Location = new System.Drawing.Point(244, 111);
            this.buttonUndo.Name = "buttonUndo";
            this.buttonUndo.Size = new System.Drawing.Size(86, 25);
            this.buttonUndo.TabIndex = 26;
            this.buttonUndo.Text = "Undo";
            this.toolTip1.SetToolTip(this.buttonUndo, "Browse for directory.");
            this.buttonUndo.Click += new System.EventHandler(this.buttonUndo_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.buttonEdit);
            this.panel1.Controls.Add(this.scriptFileTextBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.buttonBrowseScript);
            this.panel1.Location = new System.Drawing.Point(14, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(339, 89);
            this.panel1.TabIndex = 31;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.buttonUndo);
            this.panel2.Controls.Add(this.buttonWindow);
            this.panel2.Controls.Add(this.buttonPreview);
            this.panel2.Controls.Add(this.frameWidth);
            this.panel2.Controls.Add(this.outputPngLabel);
            this.panel2.Controls.Add(this.outputPngTextBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.frameEnd);
            this.panel2.Controls.Add(this.frameStart);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.buttonBrowsePngOutput);
            this.panel2.Controls.Add(this.frameHeight);
            this.panel2.Controls.Add(this.buttonStop);
            this.panel2.Controls.Add(this.buttonPlay);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.frameWaitTime);
            this.panel2.Controls.Add(this.buttonRecord);
            this.panel2.Location = new System.Drawing.Point(14, 109);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(339, 238);
            this.panel2.TabIndex = 32;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.perSecLabel);
            this.panel3.Controls.Add(this.frameRateTextBox);
            this.panel3.Controls.Add(this.frameRateLabel);
            this.panel3.Controls.Add(this.buttonBrowseMovieOutput);
            this.panel3.Controls.Add(this.outputMovieLabel);
            this.panel3.Controls.Add(this.outputMovieTextBox);
            this.panel3.Controls.Add(this.convertButton);
            this.panel3.Location = new System.Drawing.Point(14, 365);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(339, 124);
            this.panel3.TabIndex = 33;
            // 
            // MovieRecorderDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(361, 509);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MovieRecorderDialog";
            this.Text = "Movie Recorder";
            ((System.ComponentModel.ISupportInitialize)(this.frameWaitTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameStart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        void InstallPathCamera()
        {
            // Install our custom camera
            camera = new PathCamera(worldWindow, this);
            string scriptPath = scriptFileTextBox.Text;
            if (!Path.IsPathRooted(scriptPath))
                scriptPath = Path.Combine(plugin.PluginDirectory, scriptPath);
            camera.LoadScript(scriptPath);
            camera.PreRollTime = TimeSpan.FromMilliseconds((int)frameWaitTime.Value);
            camera.StartFrame = (int)frameStart.Value;
            camera.EndFrame = (int)frameEnd.Value;
            worldWindow.DrawArgs.WorldCamera = camera;

            // Resize window to our movie size
            //worldWind.ClientSize = new Size((int)frameWidth.Value, 
            //    (int)frameHeight.Value);
        }

        #region GUI event handlers
        private void buttonPlay_Click(object sender, System.EventArgs e)
        {
            this.doMovie = false;
            InstallPathCamera();
        }

        private void buttonRecord_Click(object sender, System.EventArgs e)
        {
            InstallPathCamera();
            camera.IsRecording = true;
            if (!doMovie)
                camera.OutputFilePattern = outputPngTextBox.Text;
            else
                camera.OutputFilePattern = 
                    outputPngTextBox.Text.Split(new char[] { '.' })[0] + imageFileNames;
        }

        private void buttonStop_Click(object sender, System.EventArgs e)
        {
            if (worldWindow.DrawArgs.WorldCamera != camera)
                // Our camera is not running
                return;

            // remove our camera
            camera.InstallDefaultCamera();
            camera = null;
        }

        private void buttonBrowseScript_Click(object sender, System.EventArgs e)
        {
            openFileDialog.FileName = scriptFileTextBox.Text;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                scriptFileTextBox.Text = openFileDialog.FileName;
                buttonEdit.Enabled = true;
            }
        }

        /// <summary>
        /// Launch notepad with script for edit
        /// </summary>
        private void buttonEdit_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", scriptFileTextBox.Text);
        }

        private void pngRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            outputPngTextBox.Text = "movie_{0:0000}.png";
            this.doMovie = false;
        }

        private void movieRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            outputPngTextBox.Text = "movie.avi";
            this.doMovie = true;
        }

        private void buttonBrowseOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog chooser = new FolderBrowserDialog();
            chooser.Description = "Save Files in Directory";
            if (chooser.ShowDialog(MainApplication.ActiveForm) == DialogResult.OK)
            {
                buttonPlay.Enabled = true;
                buttonRecord.Enabled = true;
                buttonStop.Enabled = true;
                outputPngTextBox.Text = chooser.SelectedPath + 
                    "\\movie" + imageFileNames;
                pngDirInfo = new DirectoryInfo(chooser.SelectedPath);
            }
        }

        private void buttonBrowseMovieOutput_Click(object sender, EventArgs e)
        {
            // save as (movie) file
            SaveFileDialog chooser = new SaveFileDialog();
            chooser.Filter = "AVI files (*.avi)|*.avi|All files (*.*)|*.*";
            chooser.FilterIndex = 1;
            chooser.RestoreDirectory = true;
            chooser.Title = "Save Movie";
            if (chooser.ShowDialog(MainApplication.ActiveForm) == DialogResult.OK)
            {
                convertButton.Enabled = true;
                outputMovieTextBox.Text = chooser.FileName;
                outputMovieTextBox.Enabled = true;
            }
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            if (pngDirInfo == null)
            {
                MessageBox.Show("Directoy containing frames not specified.", "Error");
                return;
            }
            FileInfo movieFileInfo = new FileInfo(outputMovieTextBox.Text);

            try
            {
                // Create the avi manager 
                aviManager = new AviManager(outputMovieTextBox.Text, false);

                // get the file information            
                FileInfo[] fi = pngDirInfo.GetFiles();
                // go through the png files and add them as frames
                bool firstFrame = true;
                foreach (FileInfo f in fi)
                {
                    if (f.Extension == ".png")
                    {
                        Bitmap bitmap = (Bitmap)Image.FromFile(Path.Combine(
                            f.DirectoryName, f.Name));
                        int frameRate = Convert.ToInt16(frameRateTextBox.Text);
                        if (firstFrame == true)
                            aviStream = aviManager.AddVideoStream(true, frameRate, bitmap);
                        else
                            aviStream.AddFrame(bitmap);
                        bitmap.Dispose();
                        firstFrame = false;
                    }
                }
                aviManager.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (aviManager != null)
                    aviManager.Close();
                MessageBox.Show("Conversion failed.", "Error");
            }
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {
            //previousSize = worldWind.ClientSize;
            // Resize window to our movie size for preview
            //if (worldWind.WindowState != FormWindowState.Normal)
            //    worldWind.WindowState = FormWindowState.Normal;
            //worldWind.ClientSize = new Size((int)frameWidth.Value,
            //   (int)frameHeight.Value);
            lastSizing = PREVIEW;
        }

        private void buttonWindow_Click(object sender, EventArgs e)
        {
            previousSize = new Size((int)frameWidth.Value,
                (int)frameHeight.Value);
            //frameHeight.Value = worldWind.ClientSize.Height;
            //frameWidth.Value = worldWind.ClientSize.Width;
            lastSizing = WINDOW;
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            //if (lastSizing.Equals(PREVIEW))
            //    worldWind.ClientSize = previousSize;
            //else if (lastSizing.Equals(WINDOW))
            //{
                frameHeight.Value = previousSize.Height;
                frameWidth.Value = previousSize.Width;
            //}
        }
        #endregion
    }

    /// <summary>
    /// Movie recording script.  Plays back a script file with camera positions 
    /// and records each frame to sequentially numbered Portable Network Graphics
    /// files on disk. Adds itself to the World Wind main menu's 
    /// Plugins menu -> Movie Recorder
    /// </summary>
    public class MovieRecorder : Plugin
    {
        System.Windows.Forms.MenuItem menuItemRecorder;
        MovieRecorderDialog dialog;

        /// <summary>
        /// Plugin entry point - All plugins must implement this function
        /// </summary>
        public override void Load()
        {
        }

        /// <summary>
        /// Close down
        /// </summary>
        public override void Unload()
        {
            // Reinstall default camera
            CameraBase camera = Global.worldWindow.DrawArgs.WorldCamera;
            if (camera is PathCamera)
                ((PathCamera)camera).InstallDefaultCamera();
        }

        /// <summary>
        /// User selected the recorder from the main menu.
        /// </summary>
        public void menuItemRecorder_Click(object sender, EventArgs e)
        {
            if (dialog != null && !dialog.IsDisposed)
                // Already open
                return;

            // Display the dialog
            dialog = new MovieRecorderDialog(this);
            dialog.Show();
        }
    }

    /// <summary>
    /// Camera that follows a path between a list of 
    /// key frames and triggers a save of each frame.
    /// </summary>
    public class PathCamera : CameraBase
    {
        /// <summary>
        /// Set to true to enable recording of frames to disk.
        /// </summary>
        public bool IsRecording;

        /// <summary>
        /// How long to wait after moving camera to allow imagery to load
        /// </summary>
        public TimeSpan PreRollTime;


        /// <summary>
        /// Frame to start recording from.
        /// </summary>
        public int StartFrame;

        /// <summary>
        /// Frame to end recording on.
        /// </summary>
        public int EndFrame = int.MaxValue;
        
        /// <summary>
        /// Saved filename pattern. "{0}" will 
        /// be replaced with the frame number
        /// </summary>
        public string OutputFilePattern = @"movie_{0:0000}.png";

        MovieRecorderDialog dialog;
        WorldWind.WorldWindow worldWindow;
        ArrayList keyFrames = new ArrayList();
        KeyFrame q0, q1, q2, q3;
        int currentFrameNumber = int.MinValue;
        int currentKeyNumber;
        DateTime frameStart = DateTime.MinValue;
        bool isPreRollComplete;
        bool isFrameRecorded = true;

        /// <summary>
        /// Our camera constructor
        /// </summary>
        /// <param name="worldWindow"></param>
        public PathCamera(WorldWind.WorldWindow worldWindow, 
            MovieRecorderDialog dialog)
            : base(worldWindow.CurrentWorld.Position, 
            worldWindow.CurrentWorld.EquatorialRadius)
        {
            this.worldWindow = worldWindow;
            this.dialog = dialog;
        }

        /// <summary>
        /// Override the camera update position code with our own.
        /// </summary>
        /// <param name="device">The Direct3D device.</param>
        public override void Update(Device device)
        {
            if (currentFrameNumber == int.MinValue)
                currentFrameNumber = StartFrame - 1;

            try
            {
                isPreRollComplete = DateTime.Now.Subtract(frameStart) > PreRollTime;
                if (isPreRollComplete && isFrameRecorded)
                {
                    isPreRollComplete = false;
                    isFrameRecorded = false;
                    frameStart = DateTime.Now;
                    currentFrameNumber++;
                    if (currentFrameNumber > EndFrame)
                    {
                        q1 = null;
                        currentKeyNumber = keyFrames.Count;
                    }

                    while (q1 == null || currentFrameNumber > q2.FrameNumber)
                    {
                        if (currentKeyNumber + 3 < keyFrames.Count)
                        {
                            // Move to next pair of keys
                            q0 = (KeyFrame)keyFrames[currentKeyNumber];
                            q1 = (KeyFrame)keyFrames[currentKeyNumber + 1];
                            q2 = (KeyFrame)keyFrames[currentKeyNumber + 2];
                            q3 = (KeyFrame)keyFrames[currentKeyNumber + 3];
                            currentKeyNumber++;
                        }
                        else
                        {
                            // Done, reinstall original camera
                            InstallDefaultCamera();
                            return;
                        }
                    }
                    // Interpolate between key frames
                    float t = (float)(currentFrameNumber - q1.FrameNumber) / 
                        (q2.FrameNumber - q1.FrameNumber);
                    Altitude = InterpolateLog(t, q1.Altitude, q2.Altitude);

                    // Interpolate the orientation
                    Quaternion a = Quaternion.Zero;
                    Quaternion b = Quaternion.Zero;
                    Quaternion c = Quaternion.Zero;

                    //		Quaternion4d a = new Quaternion4d();
                    //		Quaternion4d b = new Quaternion4d();
                    //		Quaternion4d c = new Quaternion4d();

                    //minor edits by Chris Maxwell to allow for compliance with revised CameraBase class
                    Quaternion orientation = new Quaternion(
                        (float)m_Orientation.X,
                        (float)m_Orientation.Y,
                        (float)m_Orientation.Z,
                        (float)m_Orientation.W);

                    Quaternion.SquadSetup(ref a, ref b, ref c,
                        q0.Orientation, q1.Orientation, q2.Orientation, q3.Orientation);
                    orientation = Quaternion.Squad(q1.Orientation, a, b, c, t);

                    Quaternion.SquadSetup(ref a, ref b, ref c,
                        q0.CameraOrientation, q1.CameraOrientation, 
                        q2.CameraOrientation, q3.CameraOrientation);

                    Vector3 cr = MathEngine.QuaternionToEuler(Quaternion.Squad(
                        q1.CameraOrientation, a, b, c, t));

                    /*		Quaternion4d.SquadSetup(ref a, ref b, ref c,
                                q0.Orientation, q1.Orientation, q2.Orientation, q3.Orientation);

                            m_Orientation = Quaternion4d.Squad(q1.Orientation, a, b, c, t);

                            Quaternion4d.SquadSetup(ref a, ref b, ref c,
                                q0.CameraOrientation, q1.CameraOrientation, q2.CameraOrientation, q3.CameraOrientation);

                            Point3d cr = Quaternion4d.QuaternionToEuler(Quaternion4d.Squad(q1.CameraOrientation, a, b, c, t));
        */
                    m_Orientation = new Quaternion4d(orientation.X, 
                        orientation.Y, orientation.Z, orientation.W);

                    _tilt.Radians = cr.X;
                    _bank.Radians = cr.Y;
                }

                base.Update(device);

                if (isPreRollComplete)
                {
                    if (IsRecording)
                    {
                        // Save the current render
                        string frameName = string.Format(OutputFilePattern, currentFrameNumber);
                        worldWindow.SaveScreenshot(frameName);
                    }
                    isFrameRecorded = true;
                }
            }
            catch (Exception caught)
            {
                Log.Write(caught);

                // Stop recording
                q1 = null;
                currentKeyNumber = keyFrames.Count;
            }
        }

        /// <summary>
        /// Loads the script file.
        /// </summary>
        /// <param name="scriptFile">Path and filename of the script 
        /// file containing the list of key frames.
        /// Format: FrameNumber;World Wind uri 
        /// The file must be sorted (frame number ascending)
        /// </param>
        public void LoadScript(string scriptFile)
        {
            using (TextReader tr = File.OpenText(scriptFile))
            {
                int lineNumber = 0;
                while (true)
                {
                    string line = tr.ReadLine();
                    if (line == null)
                        break;

                    lineNumber++;

                    if (line.Trim().Length <= 0)
                        continue;

                    try
                    {
                        keyFrames.Add(KeyFrame.FromString(line));
                    }
                    catch (Exception caught)
                    {
                        string msg = string.Format(
                            "Error in {0}, line {1}: {2}",
                            scriptFile, lineNumber, caught.Message);
                        throw new ApplicationException(msg);
                    }
                }
                if (keyFrames.Count <= 0)
                    throw new ArgumentException("No key frames found in movie script.");

                // Duplicate first and last keys for our spline operations.
                keyFrames.Insert(0, keyFrames[0]);
                keyFrames.Add(keyFrames[keyFrames.Count - 1]);
            }
        }

        /// <summary>
        /// Find the interpolated value between start and end.
        /// </summary>
        /// <param name="t">0..1 (0=start, 1=end, anything between = interpolate)</param>
        /// <param name="start">Starting value</param>
        /// <param name="end">Ending value</param>
        /// <returns></returns>
        float InterpolateLog(float t, double start, double end)
        {
            double logStart = Math.Log10(start);
            double logEnd = Math.Log10(end);
            double res = Math.Pow(10, t * (logEnd - logStart) + logStart);
            return (float)res;
        }

        /// <summary>
        /// Restores the original camera for normal operation.
        /// </summary>
        public void InstallDefaultCamera()
        {
            worldWindow.DrawArgs.WorldCamera = new MomentumCamera(
                worldWindow.CurrentWorld.Position, 
                worldWindow.CurrentWorld.EquatorialRadius);
            worldWindow.DrawArgs.WorldCamera.Update(worldWindow.DrawArgs.device);
        }
    }

    /// <summary>
    /// Contains one key frame (line from script file)
    /// </summary>
    public class KeyFrame
    {
        public double Altitude;
        public Angle Latitude;
        public Angle Longitude;
        public Angle Bank;
        public Angle Direction;
        public Angle Tilt;
        public int FrameNumber;

        public static char FieldSeparator = ';';

        /// <summary>
        /// Constructs a key frame from a script line.
        /// </summary>
        public static KeyFrame FromString(string scriptLine)
        {
            string[] fields = scriptLine.Split(new char[] { FieldSeparator }, 2);
            WorldWindUri wu = WorldWindUri.Parse(fields[1]);

            KeyFrame kf = new KeyFrame();
            kf.Altitude = wu.Altitude;
            if (double.IsNaN(kf.Altitude)) kf.Altitude = 10000;
            kf.Bank = wu.Bank;
            if (Angle.IsNaN(kf.Bank)) kf.Bank = Angle.Zero;
            kf.Direction = wu.Direction;
            if (Angle.IsNaN(kf.Direction)) kf.Direction = Angle.Zero;
            kf.Latitude = wu.Latitude;
            if (Angle.IsNaN(kf.Latitude)) kf.Latitude = Angle.Zero;
            kf.Longitude = wu.Longitude;
            if (Angle.IsNaN(kf.Longitude)) kf.Longitude = Angle.Zero;
            kf.Tilt = wu.Tilt;
            if (Angle.IsNaN(kf.Tilt)) kf.Tilt = Angle.Zero;
            kf.FrameNumber = int.Parse(fields[0]);
            return kf;
        }


        /// <summary>
        /// The key frame rotation quaternion
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                return MathEngine.EulerToQuaternion(Longitude.Radians, Latitude.Radians, Direction.Radians);
                //	return Quaternion4d.EulerToQuaternion(Longitude.Radians, Latitude.Radians, Direction.Radians);
            }
        }

        /// <summary>
        /// The key frame rotation quaternion
        /// </summary>
        public Quaternion CameraOrientation
        {
            get
            {
                return MathEngine.EulerToQuaternion(Tilt.Radians, Bank.Radians, 0);
                //	return Quaternion4d.EulerToQuaternion(Tilt.Radians, Bank.Radians, 0);
            }
        }
    }
}
