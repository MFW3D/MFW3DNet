namespace MFW3DEditor
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.appMenu = new DevExpress.XtraBars.Ribbon.ApplicationMenu(this.components);
            this.iExit = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonImageCollection = new DevExpress.Utils.ImageCollection(this.components);
            this.iAbout = new DevExpress.XtraBars.BarButtonItem();
            this.siStatus = new DevExpress.XtraBars.BarStaticItem();
            this.siInfo = new DevExpress.XtraBars.BarStaticItem();
            this.rgbiSkins = new DevExpress.XtraBars.RibbonGalleryBarItem();
            this.m_showLL = new DevExpress.XtraBars.BarButtonItem();
            this.m_zhibeizhen = new DevExpress.XtraBars.BarButtonItem();
            this.m_showlayers = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonImageCollectionLarge = new DevExpress.Utils.ImageCollection(this.components);
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageSkins = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.skinsRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.helpRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repositoryItemFloatingObjectOutlineWeight1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemFloatingObjectOutlineWeight();
            this.repositoryItemBorderLineStyle1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle();
            this.repositoryItemBorderLineWeight1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineWeight();
            this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
            this.repositoryItemRichEditFontSizeEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.spellChecker = new DevExpress.XtraSpellChecker.SpellChecker(this.components);
            this.richEditBarController1 = new DevExpress.XtraRichEdit.UI.RichEditBarController();
            this.worldWindow = new MFW3D.WorldWindow();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.m_scollerbar = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.appMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollectionLarge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemFloatingObjectOutlineWeight1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineWeight1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.richEditBarController1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl
            // 
            this.ribbonControl.ApplicationButtonDropDownControl = this.appMenu;
            this.ribbonControl.ApplicationButtonText = null;
            this.ribbonControl.ApplicationIcon = ((System.Drawing.Bitmap)(resources.GetObject("ribbonControl.ApplicationIcon")));
            this.ribbonControl.ExpandCollapseItem.Id = 0;
            this.ribbonControl.Images = this.ribbonImageCollection;
            this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl.ExpandCollapseItem,
            this.iExit,
            this.iAbout,
            this.siStatus,
            this.siInfo,
            this.rgbiSkins,
            this.m_showLL,
            this.m_zhibeizhen,
            this.m_showlayers,
            this.m_scollerbar});
            this.ribbonControl.LargeImages = this.ribbonImageCollectionLarge;
            this.ribbonControl.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl.MaxItemId = 310;
            this.ribbonControl.Name = "ribbonControl";
            this.ribbonControl.PageHeaderItemLinks.Add(this.iAbout);
            this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1,
            this.ribbonPageSkins});
            this.ribbonControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemFloatingObjectOutlineWeight1,
            this.repositoryItemBorderLineStyle1,
            this.repositoryItemBorderLineWeight1,
            this.repositoryItemFontEdit1,
            this.repositoryItemRichEditFontSizeEdit1});
            this.ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2007;
            this.ribbonControl.Size = new System.Drawing.Size(1100, 147);
            this.ribbonControl.StatusBar = this.ribbonStatusBar;
            // 
            // appMenu
            // 
            this.appMenu.ItemLinks.Add(this.iExit);
            this.appMenu.Name = "appMenu";
            this.appMenu.Ribbon = this.ribbonControl;
            this.appMenu.ShowRightPane = true;
            // 
            // iExit
            // 
            this.iExit.Caption = "退出";
            this.iExit.Description = "Closes this program after prompting you to save unsaved data.";
            this.iExit.Hint = "Closes this program after prompting you to save unsaved data";
            this.iExit.Id = 20;
            this.iExit.ImageIndex = 6;
            this.iExit.LargeImageIndex = 6;
            this.iExit.Name = "iExit";
            // 
            // ribbonImageCollection
            // 
            this.ribbonImageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("ribbonImageCollection.ImageStream")));
            this.ribbonImageCollection.Images.SetKeyName(0, "Ribbon_Exit_16x16.png");
            this.ribbonImageCollection.Images.SetKeyName(1, "Ribbon_Content_16x16.png");
            this.ribbonImageCollection.Images.SetKeyName(2, "Ribbon_Info_16x16.png");
            // 
            // iAbout
            // 
            this.iAbout.Caption = "关于";
            this.iAbout.Description = "Displays general program information.";
            this.iAbout.Hint = "Displays general program information";
            this.iAbout.Id = 24;
            this.iAbout.ImageIndex = 8;
            this.iAbout.LargeImageIndex = 8;
            this.iAbout.Name = "iAbout";
            // 
            // siStatus
            // 
            this.siStatus.Caption = "状态信息";
            this.siStatus.Id = 31;
            this.siStatus.Name = "siStatus";
            this.siStatus.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // siInfo
            // 
            this.siInfo.Caption = "信息";
            this.siInfo.Id = 32;
            this.siInfo.Name = "siInfo";
            this.siInfo.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // rgbiSkins
            // 
            this.rgbiSkins.Caption = "皮肤";
            // 
            // 
            // 
            this.rgbiSkins.Gallery.AllowHoverImages = true;
            this.rgbiSkins.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseFont = true;
            this.rgbiSkins.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseTextOptions = true;
            this.rgbiSkins.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.rgbiSkins.Gallery.ColumnCount = 4;
            this.rgbiSkins.Gallery.FixedHoverImageSize = false;
            this.rgbiSkins.Gallery.ImageSize = new System.Drawing.Size(32, 17);
            this.rgbiSkins.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Top;
            this.rgbiSkins.Gallery.RowCount = 4;
            this.rgbiSkins.Id = 60;
            this.rgbiSkins.Name = "rgbiSkins";
            // 
            // m_showLL
            // 
            this.m_showLL.Caption = "显示经纬网";
            this.m_showLL.Id = 306;
            this.m_showLL.Name = "m_showLL";
            this.m_showLL.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_showLL_ItemClick);
            // 
            // m_zhibeizhen
            // 
            this.m_zhibeizhen.Caption = "显示指北针";
            this.m_zhibeizhen.Id = 307;
            this.m_zhibeizhen.Name = "m_zhibeizhen";
            this.m_zhibeizhen.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_zhibeizhen_ItemClick);
            // 
            // m_showlayers
            // 
            this.m_showlayers.Caption = "显示图层";
            this.m_showlayers.Id = 308;
            this.m_showlayers.Name = "m_showlayers";
            this.m_showlayers.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_showlayers_ItemClick);
            // 
            // ribbonImageCollectionLarge
            // 
            this.ribbonImageCollectionLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.ribbonImageCollectionLarge.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("ribbonImageCollectionLarge.ImageStream")));
            this.ribbonImageCollectionLarge.Images.SetKeyName(0, "Ribbon_Exit_32x32.png");
            this.ribbonImageCollectionLarge.Images.SetKeyName(1, "Ribbon_Content_32x32.png");
            this.ribbonImageCollectionLarge.Images.SetKeyName(2, "Ribbon_Info_32x32.png");
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "场景操作";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.m_showLL);
            this.ribbonPageGroup1.ItemLinks.Add(this.m_zhibeizhen);
            this.ribbonPageGroup1.ItemLinks.Add(this.m_showlayers);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "显示操作";
            // 
            // ribbonPageSkins
            // 
            this.ribbonPageSkins.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.skinsRibbonPageGroup,
            this.helpRibbonPageGroup});
            this.ribbonPageSkins.Name = "ribbonPageSkins";
            this.ribbonPageSkins.Text = "设置";
            // 
            // skinsRibbonPageGroup
            // 
            this.skinsRibbonPageGroup.ItemLinks.Add(this.rgbiSkins);
            this.skinsRibbonPageGroup.Name = "skinsRibbonPageGroup";
            this.skinsRibbonPageGroup.ShowCaptionButton = false;
            this.skinsRibbonPageGroup.Text = "皮肤";
            // 
            // helpRibbonPageGroup
            // 
            this.helpRibbonPageGroup.ItemLinks.Add(this.iAbout);
            this.helpRibbonPageGroup.Name = "helpRibbonPageGroup";
            this.helpRibbonPageGroup.ShowCaptionButton = false;
            this.helpRibbonPageGroup.Text = "帮助";
            // 
            // repositoryItemFloatingObjectOutlineWeight1
            // 
            this.repositoryItemFloatingObjectOutlineWeight1.AutoHeight = false;
            this.repositoryItemFloatingObjectOutlineWeight1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemFloatingObjectOutlineWeight1.Control = null;
            this.repositoryItemFloatingObjectOutlineWeight1.Name = "repositoryItemFloatingObjectOutlineWeight1";
            // 
            // repositoryItemBorderLineStyle1
            // 
            this.repositoryItemBorderLineStyle1.AutoHeight = false;
            this.repositoryItemBorderLineStyle1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemBorderLineStyle1.Control = null;
            this.repositoryItemBorderLineStyle1.Name = "repositoryItemBorderLineStyle1";
            // 
            // repositoryItemBorderLineWeight1
            // 
            this.repositoryItemBorderLineWeight1.AutoHeight = false;
            this.repositoryItemBorderLineWeight1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemBorderLineWeight1.Control = null;
            this.repositoryItemBorderLineWeight1.Name = "repositoryItemBorderLineWeight1";
            // 
            // repositoryItemFontEdit1
            // 
            this.repositoryItemFontEdit1.AutoHeight = false;
            this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
            // 
            // repositoryItemRichEditFontSizeEdit1
            // 
            this.repositoryItemRichEditFontSizeEdit1.AutoHeight = false;
            this.repositoryItemRichEditFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemRichEditFontSizeEdit1.Control = null;
            this.repositoryItemRichEditFontSizeEdit1.Name = "repositoryItemRichEditFontSizeEdit1";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.siStatus);
            this.ribbonStatusBar.ItemLinks.Add(this.siInfo);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 669);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbonControl;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1100, 31);
            // 
            // spellChecker
            // 
            this.spellChecker.Culture = new System.Globalization.CultureInfo("ru-RU");
            this.spellChecker.ParentContainer = null;
            // 
            // worldWindow
            // 
            this.worldWindow.AllowDrop = true;
            this.worldWindow.Cache = null;
            this.worldWindow.Caption = "";
            this.worldWindow.CurrentWorld = null;
            this.worldWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worldWindow.IsRenderDisabled = false;
            this.worldWindow.Location = new System.Drawing.Point(0, 147);
            this.worldWindow.Name = "worldWindow";
            this.worldWindow.Size = new System.Drawing.Size(1100, 522);
            this.worldWindow.TabIndex = 2;
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.m_scollerbar);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "ribbonPageGroup2";
            // 
            // m_scollerbar
            // 
            this.m_scollerbar.Caption = "添加scollerbar";
            this.m_scollerbar.Id = 309;
            this.m_scollerbar.Name = "m_scollerbar";
            this.m_scollerbar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_scollerbar_ItemClick);
            // 
            // MainForm
            // 
            this.AllowFormGlass = DevExpress.Utils.DefaultBoolean.False;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.worldWindow);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbonControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Ribbon = this.ribbonControl;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "MFW3D应用";
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.appMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollectionLarge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemFloatingObjectOutlineWeight1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineStyle1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBorderLineWeight1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.richEditBarController1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
        private DevExpress.XtraBars.BarButtonItem iExit;
        private DevExpress.XtraBars.BarButtonItem iAbout;
        private DevExpress.XtraBars.BarStaticItem siStatus;
        private DevExpress.XtraBars.BarStaticItem siInfo;
        private DevExpress.XtraBars.RibbonGalleryBarItem rgbiSkins;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPageSkins;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup skinsRibbonPageGroup;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup helpRibbonPageGroup;
        private DevExpress.XtraBars.Ribbon.ApplicationMenu appMenu;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.Utils.ImageCollection ribbonImageCollection;
        private DevExpress.Utils.ImageCollection ribbonImageCollectionLarge;
        private DevExpress.XtraSpellChecker.SpellChecker spellChecker;
        private DevExpress.XtraRichEdit.Forms.Design.RepositoryItemFloatingObjectOutlineWeight repositoryItemFloatingObjectOutlineWeight1;
        private DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle repositoryItemBorderLineStyle1;
        private DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineWeight repositoryItemBorderLineWeight1;
        private DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit1;
        private DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit1;
        private DevExpress.XtraRichEdit.UI.RichEditBarController richEditBarController1;
        private MFW3D.WorldWindow worldWindow;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem m_showLL;
        private DevExpress.XtraBars.BarButtonItem m_zhibeizhen;
        private DevExpress.XtraBars.BarButtonItem m_showlayers;
        private DevExpress.XtraBars.BarButtonItem m_scollerbar;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
    }
}
