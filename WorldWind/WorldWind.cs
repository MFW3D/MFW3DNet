using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Net.Monitor;
using WorldWind.Configuration;
using WorldWind.PluginEngine;
using WorldWind.Camera;
using WorldWind.Menu;
using WorldWind.Net;
using WorldWind.Net.Wms;
using WorldWind.Terrain;
using WorldWind.Renderable;
using WorldWind.DataSource;
using Utility;
using ICSharpCode.SharpZipLib.Zip;

namespace WorldWind
{
    public class MainApplication : System.Windows.Forms.Form, IGlobe
    {
        #region WindowsForms variables
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.SplitContainer splitContainer;
        private InternalWebBrowserPanel webBrowserPanel;
        private System.Windows.Forms.ImageList imageListFunctions;
        private System.Windows.Forms.ToolBarButton toolBarButtonSearch;
        private System.Windows.Forms.ToolBarButton toolBarButtonPosition;
        private System.Windows.Forms.ToolBarButton toolBarButtonLatLonLines;
        private System.Windows.Forms.ToolBarButton toolBarButtonWebsite;
        private System.Windows.Forms.ToolBarButton toolBarButtonKeyChart;
        private System.Windows.Forms.ToolBarButton toolBarButtonLayerManager;
        private System.Windows.Forms.ToolBarButton toolBarButtonWMS;
        private System.Windows.Forms.ToolBarButton toolBarButtonAnimatedEarth;
        private System.Windows.Forms.ToolBarButton toolBarButtonRapidFireModis;
        private System.Windows.Forms.ToolBarButton toolBarButtonAddons;
        private System.Windows.Forms.ContextMenu contextMenuAddons;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItemFile;
        private System.Windows.Forms.MenuItem menuItemLoadFile;
        private System.Windows.Forms.MenuItem menuItemView;
        private System.Windows.Forms.MenuItem menuItemShowPosition;
        private System.Windows.Forms.MenuItem menuItemShowCrosshairs;
        private System.Windows.Forms.MenuItem menuItemModisHotSpots;
        private System.Windows.Forms.MenuItem menuItemConstantMotion;
        private System.Windows.Forms.MenuItem menuItemPointGoTo;
        private System.Windows.Forms.MenuItem menuItemShowLatLonLines;
        private System.Windows.Forms.MenuItem menuItemVerticalExaggeration;
        private System.Windows.Forms.MenuItem menuItemPlanetAxis;
        private System.Windows.Forms.MenuItem menuItemSpacer2;
        private System.Windows.Forms.MenuItem menuItemSpacer3;
        private System.Windows.Forms.MenuItem menuItemAnimatedEarth;
        private System.Windows.Forms.MenuItem menuItemCoordsToClipboard;
        private System.Windows.Forms.MenuItem menuItemQuit;
        private System.Windows.Forms.MenuItem menuItemSaveScreenShot;

        //Added by James Evans
        private System.Windows.Forms.MenuItem menuItemWorkOffline;

        private System.Windows.Forms.MenuItem menuItemShowToolbar;
        private System.Windows.Forms.MenuItem menuItemWMS;
        private System.Windows.Forms.MenuItem menuItemWMSImporter;
        private System.Windows.Forms.MenuItem menuItemSpacer11;
        private System.Windows.Forms.MenuItem menuItemAlwaysOnTop;
        private System.Windows.Forms.MenuItem menuItemEditPaste;
        private System.Windows.Forms.MenuItem menuItemEdit;
        private System.Windows.Forms.MenuItem menuItemTools;
        private System.Windows.Forms.MenuItem menuItemInertia;
        private System.Windows.Forms.MenuItem menuItemLockPlanetAxis;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItemReset;
        private System.Windows.Forms.MenuItem menuItemLayerManager;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItemCameraBanks;
        private System.Windows.Forms.MenuItem menuItemSpacer4;
        private System.Windows.Forms.MenuItem menuItemSunShading;
        private System.Windows.Forms.MenuItem menuItemAtmosphericScattering;
        private System.Windows.Forms.MenuItem menuItemOptions;
        private System.Windows.Forms.MenuItem menuItemPlayScript;
        private System.Windows.Forms.MenuItem menuItemStopScript;
        private System.Windows.Forms.MenuItem menuItemRefreshCurrentView;
        private System.Windows.Forms.MenuItem menuItemPluginManager;
        private System.Windows.Forms.MenuItem menuItemDownloadPlugins;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItemPlugins;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem5;
        #endregion

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            if (animatedEarthMananger != null)
            {
                animatedEarthMananger.Dispose();
                animatedEarthMananger = null;
            }
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (this.webBrowserPanel.IsTyping())
                e.Handled = true;
            else e.Handled = HandleKeyUp(e);

            base.OnKeyUp(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            // Use menu items as master settings, slave toolbar button states
            this.menuItemWMS.Checked = this.wmsBrowser != null && this.wmsBrowser.Visible;
            this.menuItemAnimatedEarth.Checked = this.animatedEarthMananger != null && this.animatedEarthMananger.Visible;
            this.menuItemModisHotSpots.Checked = this.rapidFireModisManager != null && this.rapidFireModisManager.Visible;
            UpdateToolBarStates();

            base.OnActivated(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (worldWindUri != null)
            {
                ProcessWorldWindUri();
            }

            base.OnLoad(e);
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainApplication));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.webBrowserPanel = new WorldWind.InternalWebBrowserPanel();
            this.worldWindow = new WorldWind.WorldWindow();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemLoadFile = new System.Windows.Forms.MenuItem();
            this.menuItemSaveScreenShot = new System.Windows.Forms.MenuItem();
            this.menuItemWorkOffline = new System.Windows.Forms.MenuItem();
            this.menuItemSpacer11 = new System.Windows.Forms.MenuItem();
            this.menuItemQuit = new System.Windows.Forms.MenuItem();
            this.menuItemEdit = new System.Windows.Forms.MenuItem();
            this.menuItemCoordsToClipboard = new System.Windows.Forms.MenuItem();
            this.menuItemEditPaste = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemRefreshCurrentView = new System.Windows.Forms.MenuItem();
            this.menuItemView = new System.Windows.Forms.MenuItem();
            this.menuItemShowToolbar = new System.Windows.Forms.MenuItem();
            this.menuItemLayerManager = new System.Windows.Forms.MenuItem();
            this.menuItemWebBrowser = new System.Windows.Forms.MenuItem();
            this.menuItemBrowserVisible = new System.Windows.Forms.MenuItem();
            this.menuItemBrowserOrientation = new System.Windows.Forms.MenuItem();
            this.menuItemUseInternalBrowser = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemShowLatLonLines = new System.Windows.Forms.MenuItem();
            this.menuItemPlanetAxis = new System.Windows.Forms.MenuItem();
            this.menuItemShowCrosshairs = new System.Windows.Forms.MenuItem();
            this.menuItemShowPosition = new System.Windows.Forms.MenuItem();
            this.menuItemSpacer3 = new System.Windows.Forms.MenuItem();
            this.menuItemConstantMotion = new System.Windows.Forms.MenuItem();
            this.menuItemInertia = new System.Windows.Forms.MenuItem();
            this.menuItemPointGoTo = new System.Windows.Forms.MenuItem();
            this.menuItemLockPlanetAxis = new System.Windows.Forms.MenuItem();
            this.menuItemCameraBanks = new System.Windows.Forms.MenuItem();
            this.menuItemSpacer4 = new System.Windows.Forms.MenuItem();
            this.menuItemSunShading = new System.Windows.Forms.MenuItem();
            this.menuItemAtmosphericScattering = new System.Windows.Forms.MenuItem();
            this.menuItemSpacer2 = new System.Windows.Forms.MenuItem();
            this.menuItemVerticalExaggeration = new System.Windows.Forms.MenuItem();
            this.menuItemAlwaysOnTop = new System.Windows.Forms.MenuItem();
            this.menuItemReset = new System.Windows.Forms.MenuItem();
            this.menuItemTools = new System.Windows.Forms.MenuItem();
            this.menuItemWMS = new System.Windows.Forms.MenuItem();
            this.menuItemWMSImporter = new System.Windows.Forms.MenuItem();
            this.menuItemAnimatedEarth = new System.Windows.Forms.MenuItem();
            this.menuItemModisHotSpots = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemConfigWizard = new System.Windows.Forms.MenuItem();
            this.menuItemOptions = new System.Windows.Forms.MenuItem();
            this.menuItemPlugins = new System.Windows.Forms.MenuItem();
            this.menuItemPluginManager = new System.Windows.Forms.MenuItem();
            this.menuItemDownloadPlugins = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItemPlayScript = new System.Windows.Forms.MenuItem();
            this.menuItemStopScript = new System.Windows.Forms.MenuItem();
            this.toolBarButtonAddons = new System.Windows.Forms.ToolBarButton();
            this.contextMenuAddons = new System.Windows.Forms.ContextMenu();
            this.toolBarButtonLayerManager = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonWMS = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonAnimatedEarth = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonRapidFireModis = new System.Windows.Forms.ToolBarButton();
            this.imageListFunctions = new System.Windows.Forms.ImageList(this.components);
            this.toolBarButtonKeyChart = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonWebsite = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSearch = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonPosition = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonLatLonLines = new System.Windows.Forms.ToolBarButton();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.webBrowserPanel);
            this.splitContainer.Panel1Collapsed = true;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.worldWindow);
            this.splitContainer.Size = new System.Drawing.Size(992, 631);
            this.splitContainer.TabIndex = 0;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // webBrowserPanel
            // 
            this.webBrowserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserPanel.Location = new System.Drawing.Point(0, 0);
            this.webBrowserPanel.Name = "webBrowserPanel";
            this.webBrowserPanel.Size = new System.Drawing.Size(48, 98);
            this.webBrowserPanel.TabIndex = 0;
            this.webBrowserPanel.Navigate += new WorldWind.InternalWebBrowserPanel.BrowserNavigateHandler(this.webBrowserPanel_Navigated);
            this.webBrowserPanel.Close += new WorldWind.InternalWebBrowserPanel.BrowserCloseHandler(this.webBrowserPanel_Close);
            // 
            // worldWindow
            // 
            this.worldWindow.AllowDrop = true;
            this.worldWindow.Cache = null;
            this.worldWindow.Caption = "";
            this.worldWindow.CurrentWorld = null;
            this.worldWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worldWindow.IsRenderDisabled = false;
            this.worldWindow.Location = new System.Drawing.Point(0, 0);
            this.worldWindow.Name = "worldWindow";
            this.worldWindow.ShowLayerManager = false;
            this.worldWindow.Size = new System.Drawing.Size(990, 629);
            this.worldWindow.TabIndex = 0;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFile,
            this.menuItemEdit,
            this.menuItemView,
            this.menuItemTools,
            this.menuItemPlugins});
            // 
            // menuItemFile
            // 
            this.menuItemFile.Index = 0;
            this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLoadFile,
            this.menuItemSaveScreenShot,
            this.menuItemWorkOffline,
            this.menuItemSpacer11,
            this.menuItemQuit});
            this.menuItemFile.Text = "&�ļ�";
            // 
            // menuItemLoadFile
            // 
            this.menuItemLoadFile.Index = 0;
            this.menuItemLoadFile.Text = "&�����ļ�...";
            this.menuItemLoadFile.Click += new System.EventHandler(this.menuItemLoadFile_Click);
            // 
            // menuItemSaveScreenShot
            // 
            this.menuItemSaveScreenShot.Index = 1;
            this.menuItemSaveScreenShot.Text = "&���ٽ���...\tCtrl+S";
            this.menuItemSaveScreenShot.Click += new System.EventHandler(this.menuItemSaveScreenShot_Click);
            // 
            // menuItemWorkOffline
            // 
            this.menuItemWorkOffline.Index = 2;
            this.menuItemWorkOffline.Text = "��������";
            this.menuItemWorkOffline.Click += new System.EventHandler(this.menuItemWorkOffline_Click);
            // 
            // menuItemSpacer11
            // 
            this.menuItemSpacer11.Index = 3;
            this.menuItemSpacer11.Text = "-";
            // 
            // menuItemQuit
            // 
            this.menuItemQuit.Index = 4;
            this.menuItemQuit.Text = "&�˳�\tAlt+F4";
            this.menuItemQuit.Click += new System.EventHandler(this.menuItemQuit_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.Index = 1;
            this.menuItemEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemCoordsToClipboard,
            this.menuItemEditPaste,
            this.menuItem2,
            this.menuItemRefreshCurrentView});
            this.menuItemEdit.Text = "&�༭";
            // 
            // menuItemCoordsToClipboard
            // 
            this.menuItemCoordsToClipboard.Index = 0;
            this.menuItemCoordsToClipboard.Text = "&��ֵ����ϵ\tCtrl+C";
            this.menuItemCoordsToClipboard.Click += new System.EventHandler(this.menuItemCoordsToClipboard_Click);
            // 
            // menuItemEditPaste
            // 
            this.menuItemEditPaste.Index = 1;
            this.menuItemEditPaste.Text = "&ճ������ϵ\tCtrl+V";
            this.menuItemEditPaste.Click += new System.EventHandler(this.menuItemEditPaste_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 2;
            this.menuItem2.Text = "-";
            // 
            // menuItemRefreshCurrentView
            // 
            this.menuItemRefreshCurrentView.Index = 3;
            this.menuItemRefreshCurrentView.Text = "&ˢ�³���\tF5";
            this.menuItemRefreshCurrentView.Click += new System.EventHandler(this.menuItemRefreshCurrentView_Click);
            // 
            // menuItemView
            // 
            this.menuItemView.Index = 2;
            this.menuItemView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemShowToolbar,
            this.menuItemLayerManager,
            this.menuItemWebBrowser,
            this.menuItem3,
            this.menuItemShowLatLonLines,
            this.menuItemPlanetAxis,
            this.menuItemShowCrosshairs,
            this.menuItemShowPosition,
            this.menuItemSpacer3,
            this.menuItemConstantMotion,
            this.menuItemInertia,
            this.menuItemPointGoTo,
            this.menuItemLockPlanetAxis,
            this.menuItemCameraBanks,
            this.menuItemSpacer4,
            this.menuItemSunShading,
            this.menuItemAtmosphericScattering,
            this.menuItemSpacer2,
            this.menuItemVerticalExaggeration,
            this.menuItemAlwaysOnTop,
            this.menuItemReset});
            this.menuItemView.Text = "&��ͼ";
            this.menuItemView.Popup += new System.EventHandler(this.menuItemView_Popup);
            // 
            // menuItemShowToolbar
            // 
            this.menuItemShowToolbar.Checked = true;
            this.menuItemShowToolbar.Index = 0;
            this.menuItemShowToolbar.Text = "&������\tCtrl+T";
            this.menuItemShowToolbar.Click += new System.EventHandler(this.menuItemShowToolbar_Click);
            // 
            // menuItemLayerManager
            // 
            this.menuItemLayerManager.Index = 1;
            this.menuItemLayerManager.Text = "&ͼ�������\tL";
            this.menuItemLayerManager.Click += new System.EventHandler(this.menuItemLayerManager_Click);
            // 
            // menuItemWebBrowser
            // 
            this.menuItemWebBrowser.Index = 2;
            this.menuItemWebBrowser.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemBrowserVisible,
            this.menuItemBrowserOrientation,
            this.menuItemUseInternalBrowser});
            this.menuItemWebBrowser.Text = "web�����";
            // 
            // menuItemBrowserVisible
            // 
            this.menuItemBrowserVisible.Index = 0;
            this.menuItemBrowserVisible.Text = "������ɽ�";
            this.menuItemBrowserVisible.Click += new System.EventHandler(this.webBrowserVisible_Click);
            // 
            // menuItemBrowserOrientation
            // 
            this.menuItemBrowserOrientation.Enabled = false;
            this.menuItemBrowserOrientation.Index = 1;
            this.menuItemBrowserOrientation.Text = "���ض�λ";
            this.menuItemBrowserOrientation.Click += new System.EventHandler(this.menuItemBrowserOrientation_Click);
            // 
            // menuItemUseInternalBrowser
            // 
            this.menuItemUseInternalBrowser.Index = 2;
            this.menuItemUseInternalBrowser.Text = "�ڴ�����";
            this.menuItemUseInternalBrowser.Click += new System.EventHandler(this.menuItemUseInternalBrowser_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.Text = "-";
            // 
            // menuItemShowLatLonLines
            // 
            this.menuItemShowLatLonLines.Index = 4;
            this.menuItemShowLatLonLines.Text = "��γ��\tF7";
            this.menuItemShowLatLonLines.Click += new System.EventHandler(this.menuItemShowLatLonLines_Click);
            // 
            // menuItemPlanetAxis
            // 
            this.menuItemPlanetAxis.Index = 5;
            this.menuItemPlanetAxis.Text = "����ϵ\tF8";
            this.menuItemPlanetAxis.Click += new System.EventHandler(this.menuItemPlanetAxis_Click);
            // 
            // menuItemShowCrosshairs
            // 
            this.menuItemShowCrosshairs.Index = 6;
            this.menuItemShowCrosshairs.Text = "ʮ����\tF9";
            this.menuItemShowCrosshairs.Click += new System.EventHandler(this.menuItemShowCrosshairs_Click);
            // 
            // menuItemShowPosition
            // 
            this.menuItemShowPosition.Index = 7;
            this.menuItemShowPosition.Text = "λ��\tF10";
            this.menuItemShowPosition.Click += new System.EventHandler(this.menuItemShowPosition_Click);
            // 
            // menuItemSpacer3
            // 
            this.menuItemSpacer3.Index = 8;
            this.menuItemSpacer3.Text = "-";
            // 
            // menuItemConstantMotion
            // 
            this.menuItemConstantMotion.Index = 9;
            this.menuItemConstantMotion.Text = "&�˶�\tF11";
            this.menuItemConstantMotion.Click += new System.EventHandler(this.menuItemConstantMotion_Click);
            // 
            // menuItemInertia
            // 
            this.menuItemInertia.Index = 10;
            this.menuItemInertia.Text = "����";
            this.menuItemInertia.Click += new System.EventHandler(this.menuItemInertia_Click);
            // 
            // menuItemPointGoTo
            // 
            this.menuItemPointGoTo.Checked = true;
            this.menuItemPointGoTo.Index = 11;
            this.menuItemPointGoTo.Text = "��λ\tF12";
            this.menuItemPointGoTo.Click += new System.EventHandler(this.menuItemPointGoTo_Click);
            // 
            // menuItemLockPlanetAxis
            // 
            this.menuItemLockPlanetAxis.Index = 12;
            this.menuItemLockPlanetAxis.Text = "Ť������";
            this.menuItemLockPlanetAxis.Click += new System.EventHandler(this.menuItemLockPlanetAxis_Click);
            // 
            // menuItemCameraBanks
            // 
            this.menuItemCameraBanks.Index = 13;
            this.menuItemCameraBanks.Text = "&��������";
            this.menuItemCameraBanks.Click += new System.EventHandler(this.menuItemCameraBanking_Click);
            // 
            // menuItemSpacer4
            // 
            this.menuItemSpacer4.Index = 14;
            this.menuItemSpacer4.Text = "-";
            // 
            // menuItemSunShading
            // 
            this.menuItemSunShading.Index = 15;
            this.menuItemSunShading.Text = "̫��\tShift+S";
            this.menuItemSunShading.Click += new System.EventHandler(this.menuItemSunShading_Click);
            // 
            // menuItemAtmosphericScattering
            // 
            this.menuItemAtmosphericScattering.Index = 16;
            this.menuItemAtmosphericScattering.Text = "����\tShift+A";
            this.menuItemAtmosphericScattering.Click += new System.EventHandler(this.menuItemAtmosphericScattering_Click);
            // 
            // menuItemSpacer2
            // 
            this.menuItemSpacer2.Index = 17;
            this.menuItemSpacer2.Text = "-";
            // 
            // menuItemVerticalExaggeration
            // 
            this.menuItemVerticalExaggeration.Index = 18;
            this.menuItemVerticalExaggeration.Text = "��ֱ";
            // 
            // menuItemAlwaysOnTop
            // 
            this.menuItemAlwaysOnTop.Index = 19;
            this.menuItemAlwaysOnTop.Text = "����\tAlt+A";
            this.menuItemAlwaysOnTop.Click += new System.EventHandler(this.menuItemAlwaysOnTop_Click);
            // 
            // menuItemReset
            // 
            this.menuItemReset.Index = 20;
            this.menuItemReset.Text = "�ָ���ʾ\tSpace";
            this.menuItemReset.Click += new System.EventHandler(this.menuItemReset_Click);
            // 
            // menuItemTools
            // 
            this.menuItemTools.Index = 3;
            this.menuItemTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemWMS,
            this.menuItemWMSImporter,
            this.menuItemAnimatedEarth,
            this.menuItemModisHotSpots,
            this.menuItem5,
            this.menuItemConfigWizard,
            this.menuItemOptions});
            this.menuItemTools.Text = "&����";
            // 
            // menuItemWMS
            // 
            this.menuItemWMS.Index = 0;
            this.menuItemWMS.RadioCheck = true;
            this.menuItemWMS.Text = "&WMS �����\tB";
            this.menuItemWMS.Click += new System.EventHandler(this.menuItemWMS_Click);
            // 
            // menuItemWMSImporter
            // 
            this.menuItemWMSImporter.Index = 1;
            this.menuItemWMSImporter.Text = "&���� WMS url ��ͼ��...";
            this.menuItemWMSImporter.Click += new System.EventHandler(this.menuItemWMSImporter_Click);
            // 
            // menuItemAnimatedEarth
            // 
            this.menuItemAnimatedEarth.Index = 2;
            this.menuItemAnimatedEarth.RadioCheck = true;
            this.menuItemAnimatedEarth.Text = "&���⻯����\tF1";
            this.menuItemAnimatedEarth.Click += new System.EventHandler(this.menuItemAnimatedEarth_Click);
            // 
            // menuItemModisHotSpots
            // 
            this.menuItemModisHotSpots.Index = 3;
            this.menuItemModisHotSpots.RadioCheck = true;
            this.menuItemModisHotSpots.Text = "&����MODIS\tF2";
            this.menuItemModisHotSpots.Click += new System.EventHandler(this.menuItemModisHotSpots_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // menuItemConfigWizard
            // 
            this.menuItemConfigWizard.Index = 5;
            this.menuItemConfigWizard.Text = "&������";
            this.menuItemConfigWizard.Click += new System.EventHandler(this.menuItemConfigWizard_Click);
            // 
            // menuItemOptions
            // 
            this.menuItemOptions.Index = 6;
            this.menuItemOptions.Text = "&����\tAlt+W";
            this.menuItemOptions.Click += new System.EventHandler(this.menuItemOptions_Click);
            // 
            // menuItemPlugins
            // 
            this.menuItemPlugins.Index = 4;
            this.menuItemPlugins.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemPluginManager,
            this.menuItemDownloadPlugins,
            this.menuItem1,
            this.menuItem7});
            this.menuItemPlugins.Text = "���";
            // 
            // menuItemPluginManager
            // 
            this.menuItemPluginManager.Index = 0;
            this.menuItemPluginManager.Text = "&���ء�ж��...";
            this.menuItemPluginManager.Click += new System.EventHandler(this.menuItemPluginManager_Click);
            // 
            // menuItemDownloadPlugins
            // 
            this.menuItemDownloadPlugins.Index = 1;
            this.menuItemDownloadPlugins.Text = "���ز��...";
            this.menuItemDownloadPlugins.Click += new System.EventHandler(this.menuItemDownloadPlugins_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.Text = "-";
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 3;
            this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemPlayScript,
            this.menuItemStopScript});
            this.menuItem7.Text = "�ű�";
            // 
            // menuItemPlayScript
            // 
            this.menuItemPlayScript.Index = 0;
            this.menuItemPlayScript.Text = "&ִ�нű�...";
            this.menuItemPlayScript.Click += new System.EventHandler(this.menuItemPlayScript_Click);
            // 
            // menuItemStopScript
            // 
            this.menuItemStopScript.Enabled = false;
            this.menuItemStopScript.Index = 1;
            this.menuItemStopScript.Text = "ֹͣ��ǰ�ű�";
            this.menuItemStopScript.Click += new System.EventHandler(this.menuItemStopScript_Click);
            // 
            // toolBarButtonAddons
            // 
            this.toolBarButtonAddons.DropDownMenu = this.contextMenuAddons;
            this.toolBarButtonAddons.ImageIndex = 0;
            this.toolBarButtonAddons.Name = "toolBarButtonAddons";
            this.toolBarButtonAddons.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            this.toolBarButtonAddons.Text = "Add-Ons";
            this.toolBarButtonAddons.ToolTipText = "Show Add-Ons";
            // 
            // toolBarButtonLayerManager
            // 
            this.toolBarButtonLayerManager.ImageIndex = 3;
            this.toolBarButtonLayerManager.Name = "toolBarButtonLayerManager";
            this.toolBarButtonLayerManager.ToolTipText = "Show Layer Manager";
            // 
            // toolBarButtonWMS
            // 
            this.toolBarButtonWMS.ImageIndex = 4;
            this.toolBarButtonWMS.Name = "toolBarButtonWMS";
            this.toolBarButtonWMS.ToolTipText = "Show WMS Browser";
            // 
            // toolBarButtonAnimatedEarth
            // 
            this.toolBarButtonAnimatedEarth.ImageIndex = 5;
            this.toolBarButtonAnimatedEarth.Name = "toolBarButtonAnimatedEarth";
            this.toolBarButtonAnimatedEarth.ToolTipText = "Show NASA Scientific Visualization Studio";
            // 
            // toolBarButtonRapidFireModis
            // 
            this.toolBarButtonRapidFireModis.ImageIndex = 6;
            this.toolBarButtonRapidFireModis.Name = "toolBarButtonRapidFireModis";
            this.toolBarButtonRapidFireModis.ToolTipText = "Show Rapid Fire MODIS";
            // 
            // imageListFunctions
            // 
            this.imageListFunctions.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListFunctions.ImageStream")));
            this.imageListFunctions.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListFunctions.Images.SetKeyName(0, "");
            this.imageListFunctions.Images.SetKeyName(1, "");
            this.imageListFunctions.Images.SetKeyName(2, "");
            this.imageListFunctions.Images.SetKeyName(3, "");
            this.imageListFunctions.Images.SetKeyName(4, "");
            this.imageListFunctions.Images.SetKeyName(5, "");
            this.imageListFunctions.Images.SetKeyName(6, "");
            this.imageListFunctions.Images.SetKeyName(7, "");
            this.imageListFunctions.Images.SetKeyName(8, "");
            // 
            // toolBarButtonKeyChart
            // 
            this.toolBarButtonKeyChart.ImageIndex = 7;
            this.toolBarButtonKeyChart.Name = "toolBarButtonKeyChart";
            this.toolBarButtonKeyChart.ToolTipText = "Show Key Chart";
            // 
            // toolBarButtonWebsite
            // 
            this.toolBarButtonWebsite.ImageIndex = 8;
            this.toolBarButtonWebsite.Name = "toolBarButtonWebsite";
            this.toolBarButtonWebsite.ToolTipText = "Show World Wind Website";
            // 
            // toolBarButtonSearch
            // 
            this.toolBarButtonSearch.ImageIndex = 0;
            this.toolBarButtonSearch.Name = "toolBarButtonSearch";
            this.toolBarButtonSearch.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonSearch.ToolTipText = "Search For a Place";
            // 
            // toolBarButtonPosition
            // 
            this.toolBarButtonPosition.ImageIndex = 1;
            this.toolBarButtonPosition.Name = "toolBarButtonPosition";
            this.toolBarButtonPosition.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonPosition.ToolTipText = "Show Current Position";
            // 
            // toolBarButtonLatLonLines
            // 
            this.toolBarButtonLatLonLines.ImageIndex = 2;
            this.toolBarButtonLatLonLines.Name = "toolBarButtonLatLonLines";
            this.toolBarButtonLatLonLines.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonLatLonLines.ToolTipText = "Show Latitude/Longitude Lines";
            // 
            // MainApplication
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(992, 631);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(240, 215);
            this.Name = "MainApplication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NASA World Wind";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Window�����ڲ���Ϣ����
        public static IntPtr GetWWHandle()
        {
            return NativeMethods.FindWindow(null, "NASA World Wind");
        }
        public static string[] StringFromCopyData(Message m)
        {
            NativeMethods.CopyDataStruct cds = (NativeMethods.CopyDataStruct)
                m.GetLParam(typeof(NativeMethods.CopyDataStruct));
            string[] args = Marshal.PtrToStringAuto(cds.lpData).Split('\n');
            return args;
        }
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true), SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_ACTIVATEAPP:
                    // Disable rendering when application isn't active
                    if (worldWindow != null)
                        worldWindow.IsRenderDisabled = m.WParam.ToInt32() == 0;
                    break;
                case NativeMethods.WM_COPYDATA:
                    // Use cds here
                    string[] args = StringFromCopyData(m);
                    ParseArgs(args);

                    if (worldWindUri != null)
                    {
                        ProcessWorldWindUri();
                        this.Activate();            // makes the task bar entry flash
                    }

                    m.Result = (IntPtr)1;
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion

        #region ���ز���
        public static void LoadSettings()
        {
            try
            {
                Settings = (WorldWindSettings)SettingsBase.Load(Settings, SettingsBase.LocationType.User);

                if (!File.Exists(Settings.FileName))
                {
                    Settings.PluginsLoadedOnStartup.Add("ShapeFileInfoTool");
                    Settings.PluginsLoadedOnStartup.Add("OverviewFormLoader");
                    Settings.PluginsLoadedOnStartup.Add("Atmosphere");
                    Settings.PluginsLoadedOnStartup.Add("SkyGradient");
                    Settings.PluginsLoadedOnStartup.Add("BmngLoader");
                    Settings.PluginsLoadedOnStartup.Add("Compass");
                    Settings.PluginsLoadedOnStartup.Add("ExternalLayerManagerLoader");
                    Settings.PluginsLoadedOnStartup.Add("MeasureTool");
                    Settings.PluginsLoadedOnStartup.Add("MovieRecorder");
                    Settings.PluginsLoadedOnStartup.Add("NRLWeatherLoader");
                    Settings.PluginsLoadedOnStartup.Add("ShapeFileLoader");
                    Settings.PluginsLoadedOnStartup.Add("Stars3D");
                    Settings.PluginsLoadedOnStartup.Add("GlobalClouds");
                    Settings.PluginsLoadedOnStartup.Add("PlaceFinderLoader");
                    Settings.PluginsLoadedOnStartup.Add("LightController");

                    Settings.PluginsLoadedOnStartup.Add("Earthquake_2.0.2.1");
                    Settings.PluginsLoadedOnStartup.Add("Historical_Earthquake_2.0.2.2");
                    Settings.PluginsLoadedOnStartup.Add("KMLImporter");
                    Settings.PluginsLoadedOnStartup.Add("doublezoom");
                    Settings.PluginsLoadedOnStartup.Add("PlanetaryRings");
                    Settings.PluginsLoadedOnStartup.Add("TimeController");
                    Settings.PluginsLoadedOnStartup.Add("WavingFlags");
                    Settings.PluginsLoadedOnStartup.Add("ScaleBarLegend");
                    Settings.PluginsLoadedOnStartup.Add("Compass3D");
                    Settings.PluginsLoadedOnStartup.Add("AnaglyphStereo");
                }
                DataProtector dp = new DataProtector(DataProtector.Store.USE_USER_STORE);
                if (Settings.ProxyUsername.Length > 0) Settings.ProxyUsername = dp.TransparentDecrypt(Settings.ProxyUsername);
                if (Settings.ProxyPassword.Length > 0) Settings.ProxyPassword = dp.TransparentDecrypt(Settings.ProxyPassword);
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }
        public static void LoadSettings(string directory)
        {
            try
            {
                Settings = (WorldWindSettings)SettingsBase.LoadFromPath(Settings, directory);
                DataProtector dp = new DataProtector(DataProtector.Store.USE_USER_STORE);
                if (Settings.ProxyUsername.Length > 0) Settings.ProxyUsername = dp.TransparentDecrypt(Settings.ProxyUsername);
                if (Settings.ProxyPassword.Length > 0) Settings.ProxyPassword = dp.TransparentDecrypt(Settings.ProxyPassword);
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }
        public static void ParseArgs(string[] args)
        {
            try
            {
                NLT.Plugins.ShapeFileLoaderGUI.m_ShapeLoad.ParseUri(args);
            }
            catch
            {
            }
            cmdArgs = args;

            foreach (string rawArg in args)
            {
                string arg = rawArg.Trim();
                if (arg.Length <= 0)
                    continue;
                try
                {
                    //check for url call
                    // TODO: do not hardcode the URI scheme here
                    if (arg.StartsWith("worldwind://"))
                    {
                        worldWindUri = WorldWindUri.Parse(arg);
                    }
                    else if (arg.StartsWith("/"))
                    {
                        if (arg.Length <= 1)
                        {
                            throw new ArgumentException("Empty command line option.");
                        }

                        string key = arg.Substring(1, 1).ToLower(CultureInfo.CurrentCulture);
                        switch (key)
                        {
                            case "s":
                                if (issetCurrentSettingsDirectory)
                                {
                                    continue;
                                }
                                if (arg.Length < 6)
                                {
                                    throw new ArgumentException("Invalid value(too short) for command line option /S: " + arg);
                                }
                                if (arg.Substring(2, 1) != "=")
                                {
                                    throw new ArgumentException("Invalid value(no = after S) for command line option /S: " + arg);
                                }
                                // TODO: test value via regex?
                                CurrentSettingsDirectory = arg.Substring(3);
                                issetCurrentSettingsDirectory = true;
                                break;
                            default:
                                throw new ArgumentException("Unknown command line option: " + arg);
                        }
                    }
                    else
                        throw new ArgumentException("Unknown command line option: " + arg);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }
        }
        #endregion

        #region ����
        public static string CurrentSettingsDirectory;
        public static bool issetCurrentSettingsDirectory;
        public static WorldWindSettings Settings = new WorldWindSettings();
        private static WorldWindUri worldWindUri;
        private static string[] cmdArgs;
        private WorldWindow worldWindow;
        //private TreeNode[] layerTreeNodes;
        private System.Collections.Hashtable availableWorldList = new Hashtable();
        private System.Windows.Forms.MenuItem menuItemConfigWizard;
        private MenuItem menuItemWebBrowser;
        private MenuItem menuItemBrowserVisible;
        private MenuItem menuItemBrowserOrientation;
        private MenuItem menuItemUseInternalBrowser;
        private PluginCompiler compiler;
        #endregion

        #region ����ĶԻ���͹�����
        private Splash splashScreen;
        private PlaceBuilder placeBuilderDialog;
        private RapidFireModisManager rapidFireModisManager;
        private AnimatedEarthManager animatedEarthMananger;
        private GotoDialog gotoDialog;
        private PathMaker pathMaker;
        private WMSBrowser wmsBrowser;
        private WMSBrowserNG wmsImporter;
        private PluginDialog pluginManager;
        private ProgressMonitor queueMonitor;
        private FileLoader fileLoaderDialog;
        #endregion

        public static readonly string DirectoryPath = Path.GetDirectoryName(Application.ExecutablePath);

        public MainApplication()
        {
            //����
            if (Settings.ConfigurationWizardAtStartup)
            {
                if (!File.Exists(Settings.FileName))
                {
                    Settings.ConfigurationWizardAtStartup = false;
                }
                ConfigurationWizard.Wizard wizard = new ConfigurationWizard.Wizard(Settings);
                wizard.TopMost = true;
                wizard.ShowInTaskbar = true;
                wizard.ShowDialog();
            }

            using (this.splashScreen = new Splash())
            {
                this.splashScreen.Owner = this;
                this.splashScreen.Show();
                this.splashScreen.SetText("Initializing...");

                Application.DoEvents();
                InitializeComponent();

                long CacheUpperLimit = (long)Settings.CacheSizeMegaBytes * 1024L * 1024L;
                long CacheLowerLimit = (long)Settings.CacheSizeMegaBytes * 768L * 1024L;    //75% of upper limit
                                                                                            //Set up the cache
                worldWindow.Cache = new Cache(
                    Settings.CachePath,
                    CacheLowerLimit,
                    CacheUpperLimit,
                    Settings.CacheCleanupInterval,
                    Settings.TotalRunTime);

                WorldWind.Net.WebDownload.Log404Errors = World.Settings.Log404Errors;

                DirectoryInfo worldsXmlDir = new DirectoryInfo(Settings.ConfigPath);
                if (!worldsXmlDir.Exists)
                    throw new ApplicationException(
                        string.Format(CultureInfo.CurrentCulture,
                        "World Wind configuration directory '{0}' could not be found.", worldsXmlDir.FullName));

                FileInfo[] worldXmlDescriptorFiles = worldsXmlDir.GetFiles("*.xml");
                int worldIndex = 0;
                menuItemFile.MenuItems.Add(
                    0, new System.Windows.Forms.MenuItem("-"));
                foreach (FileInfo worldXmlDescriptorFile in worldXmlDescriptorFiles)
                {
                    try
                    {
                        Log.Write(Log.Levels.Debug + 1, "CONF", "checking world " + worldXmlDescriptorFile.FullName + " ...");
                        string worldXmlSchema = null;
                        string layerSetSchema = null;
                        if (Settings.ValidateXML)
                        {
                            worldXmlSchema = Settings.ConfigPath + "\\WorldXmlDescriptor.xsd";
                            layerSetSchema = Settings.ConfigPath + "\\Earth\\LayerSet.xsd";
                        }
                        World w = WorldWind.ConfigurationLoader.Load(worldXmlDescriptorFile.FullName, worldWindow.Cache, worldXmlSchema, layerSetSchema);
                        if (!availableWorldList.Contains(w.Name))
                            this.availableWorldList.Add(w.Name, worldXmlDescriptorFile.FullName);

                        w.Dispose();
                        System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem(w.Name, new System.EventHandler(OnWorldChange));
                        menuItemFile.MenuItems.Add(worldIndex, mi);
                        worldIndex++;
                    }
                    catch (Exception caught)
                    {
                        splashScreen.SetError(worldXmlDescriptorFile + ": " + caught.Message);
                        Log.Write(caught);
                    }
                }

                Log.Write(Log.Levels.Debug, "CONF", "loading startup world...");
                OpenStartupWorld();

                // Set up vertical exaggeration sub-menu
                float[] verticalExaggerationMultipliers = { 0.0f, 1.0f, 1.5f, 2.0f, 3.0f, 5.0f, 7.0f, 10.0f };
                foreach (float multiplier in verticalExaggerationMultipliers)
                {
                    MenuItem curItem = new MenuItem(multiplier.ToString("f1", CultureInfo.CurrentCulture) + "x", new EventHandler(this.menuItemVerticalExaggerationChange));
                    curItem.RadioCheck = true;
                    this.menuItemVerticalExaggeration.MenuItems.Add(curItem);
                    if (Math.Abs(multiplier - World.Settings.VerticalExaggeration) < 0.1f)
                        curItem.Checked = true;
                }

                // Load defaults from settings
                this.menuItemPointGoTo.Checked = World.Settings.CameraIsPointGoto;
                this.menuItemInertia.Checked = World.Settings.CameraHasInertia;
                this.menuItemConstantMotion.Checked = World.Settings.CameraHasMomentum;
                this.menuItemCameraBanks.Checked = World.Settings.CameraBankLock;
                this.menuItemSunShading.Checked = World.Settings.EnableSunShading;
                this.menuItemAtmosphericScattering.Checked = World.Settings.EnableAtmosphericScattering;
                this.menuItemPlanetAxis.Checked = World.Settings.ShowPlanetAxis;
                this.menuItemShowLatLonLines.Checked = World.Settings.ShowLatLonLines;
                this.menuItemShowToolbar.Checked = World.Settings.ShowToolbar;
                this.menuItemLayerManager.Checked = World.Settings.ShowLayerManager;
                this.worldWindow.ShowLayerManager = World.Settings.ShowLayerManager;
                this.menuItemLockPlanetAxis.Checked = World.Settings.CameraTwistLock;
                this.menuItemShowCrosshairs.Checked = World.Settings.ShowCrosshairs;
                this.menuItemShowPosition.Checked = World.Settings.ShowPosition;
                this.menuItemBrowserVisible.Checked = World.Settings.BrowserVisible;
                this.menuItemUseInternalBrowser.Checked = World.Settings.UseInternalBrowser;
                this.splitContainer.Panel1Collapsed = !World.Settings.BrowserVisible;
                this.splitContainer.Orientation = getWebBrowserOrientationFromSetting(World.Settings.BrowserOrientationHorizontal);
                this.menuItemWorkOffline.Checked = World.Settings.WorkOffline;
                while (!this.splashScreen.IsDone)
                    System.Threading.Thread.Sleep(50);
                // Force initial render to avoid showing random contents of frame buffer to user.
                worldWindow.Render();
                WorldWindow.Focus();
            }

            // Center the main window
            Rectangle screenBounds = Screen.GetBounds(this);
            this.Location = new Point(screenBounds.Width / 2 - this.Size.Width / 2, screenBounds.Height / 2 - this.Size.Height / 2);

        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (worldWindow != null)
                worldWindow.Focus();
            base.OnGotFocus(e);
        }

        #region ��������
        public WorldWindow WorldWindow
        {
            get
            {
                return worldWindow;
            }
        }

        /// <summary>
        /// ���õ���
        /// </summary>
        public float VerticalExaggeration
        {
            get
            {
                return World.Settings.VerticalExaggeration;
            }
            set
            {
                World.Settings.VerticalExaggeration = value;
                this.worldWindow.Invalidate();
                string label = value.ToString("f1", CultureInfo.CurrentCulture) + "x";
                foreach (MenuItem m in this.menuItemVerticalExaggeration.MenuItems)
                    m.Checked = (label == m.Text);
            }
        }

        public Splash SplashScreen { get { return splashScreen; } }

        public MainMenu MainMenu { get { return mainMenu; } }

        public MenuItem ToolsMenu { get { return menuItemTools; } }

        public MenuItem ViewMenu { get { return menuItemView; } }

        public MenuItem PluginsMenu { get { return menuItemPlugins; } }

        #endregion

        #region ��������

        public void BrowseTo(string url)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = url;
            psi.Verb = "open";
            psi.UseShellExecute = true;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }

        public void webBrowserVisible(bool newStatus)
        {
            if (newStatus && World.Settings.BrowserVisible)
                return;
            else
            {
                World.Settings.BrowserVisible = !World.Settings.BrowserVisible;
                this.splitContainer.Panel1Collapsed = !World.Settings.BrowserVisible;
                this.menuItemBrowserVisible.Checked = World.Settings.BrowserVisible;
                this.menuItemBrowserOrientation.Enabled = World.Settings.BrowserVisible;
                this.splitContainer.SplitterDistance = World.Settings.BrowserSize;
                worldWindow.Render();
            }
        }
        public void LoadAddon(string fileName)
        {
            try
            {
                RenderableObjectList layers = ConfigurationLoader.getRenderableFromLayerFile(fileName, worldWindow.CurrentWorld, worldWindow.Cache);
                worldWindow.CurrentWorld.RenderableObjects.Add(layers);
                layers.IsOn = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading layer from file");
            }
        }
        #endregion

        #region ���������
        private void InitializePluginCompiler()
        {
            Log.Write(Log.Levels.Debug, "CONF", "initializing plugin compiler...");
            this.splashScreen.SetText("Initializing plugins...");
            string pluginRoot = Path.Combine(DirectoryPath, "Plugins");
            compiler = new PluginCompiler(this, pluginRoot);

            //#if DEBUG
            // Search for plugins in worldwind.exe (plugin development/debugging aid)
            compiler.FindPlugins(Assembly.GetExecutingAssembly());
            //#endif

            compiler.FindPlugins();
            compiler.LoadStartupPlugins();
        }

        private void OnWorldChange(object sender, System.EventArgs e)
        {
            System.Windows.Forms.MenuItem curMenuItem = (System.Windows.Forms.MenuItem)sender;

            //WorldXmlDescriptor.WorldType worldType = this.availableWorldList[curMenuItem.Text] as WorldXmlDescriptor.WorldType;
            string curWorld = availableWorldList[curMenuItem.Text] as string;

            if (curWorld != null)
            {
                OpenWorld(curWorld);
            }
        }

        private void AddInternalPluginMenuButtons()
        {
            if (this.worldWindow.CurrentWorld.IsEarth)
            {
                this.rapidFireModisManager = new RapidFireModisManager(this.worldWindow);
                this.rapidFireModisManager.Icon = this.Icon;
                this.worldWindow.MenuBar.AddToolsMenuButton(new WindowsControlMenuButton("Rapid Fire MODIS", DirectoryPath + "\\Data\\Icons\\Interface\\modis.png", this.rapidFireModisManager));
            }
            this.wmsBrowser = new WMSBrowser(this.worldWindow);
            this.wmsBrowser.Icon = this.Icon;
            this.worldWindow.MenuBar.AddToolsMenuButton(new WindowsControlMenuButton("WMS Browser", DirectoryPath + "\\Data\\Icons\\Interface\\wms.png", this.wmsBrowser));

            if (this.worldWindow.CurrentWorld.IsEarth)
            {
                this.animatedEarthMananger = new AnimatedEarthManager(this.worldWindow);
                this.animatedEarthMananger.Icon = this.Icon;
                this.worldWindow.MenuBar.AddToolsMenuButton(new WindowsControlMenuButton("Scientific Visualization Studio", DirectoryPath + "\\Data\\Icons\\Interface\\svs2.png", this.animatedEarthMananger));
            }
        }

        #endregion

        #region �˵�����Ӧ
        private void menuItemView_Popup(object sender, System.EventArgs e)
        {
            this.menuItemPointGoTo.Checked = World.Settings.CameraIsPointGoto;
            this.menuItemInertia.Checked = World.Settings.CameraHasInertia;
            this.menuItemConstantMotion.Checked = World.Settings.CameraHasMomentum;
            this.menuItemCameraBanks.Checked = World.Settings.CameraBankLock;
            this.menuItemSunShading.Checked = World.Settings.EnableSunShading;
            this.menuItemAtmosphericScattering.Checked = World.Settings.EnableAtmosphericScattering;
            this.menuItemPlanetAxis.Checked = World.Settings.ShowPlanetAxis;
            this.menuItemShowLatLonLines.Checked = World.Settings.ShowLatLonLines;
            this.menuItemShowToolbar.Checked = World.Settings.ShowToolbar;
            this.menuItemLayerManager.Checked = World.Settings.ShowLayerManager;
            this.worldWindow.ShowLayerManager = World.Settings.ShowLayerManager;
            this.menuItemLockPlanetAxis.Checked = World.Settings.CameraTwistLock;
            this.menuItemShowCrosshairs.Checked = World.Settings.ShowCrosshairs;
            this.menuItemShowPosition.Checked = World.Settings.ShowPosition;

            //James Evans
            this.menuItemWorkOffline.Checked = World.Settings.WorkOffline;

        }
        private void menuItemShowPosition_Click(object sender, System.EventArgs e)
        {
            World.Settings.ShowPosition = !World.Settings.ShowPosition;
            this.toolBarButtonPosition.Pushed = World.Settings.ShowPosition;
            this.menuItemShowPosition.Checked = World.Settings.ShowPosition;
            this.worldWindow.Invalidate();
        }
        private void menuItemShowCrosshairs_Click(object sender, System.EventArgs e)
        {
            World.Settings.ShowCrosshairs = !World.Settings.ShowCrosshairs;
            this.menuItemShowCrosshairs.Checked = World.Settings.ShowCrosshairs;
            this.worldWindow.Invalidate();
        }
        private void menuItemConstantMotion_Click(object sender, System.EventArgs e)
        {
            World.Settings.CameraHasMomentum = !World.Settings.CameraHasMomentum;
            this.menuItemConstantMotion.Checked = World.Settings.CameraHasMomentum;
        }
        private void menuItemPointGoTo_Click(object sender, System.EventArgs e)
        {
            this.worldWindow.DrawArgs.WorldCamera.IsPointGoto = !this.worldWindow.DrawArgs.WorldCamera.IsPointGoto;
            this.menuItemPointGoTo.Checked = this.worldWindow.DrawArgs.WorldCamera.IsPointGoto;
        }
        private void menuItemInertia_Click(object sender, System.EventArgs e)
        {
            World.Settings.CameraHasInertia = !World.Settings.CameraHasInertia;
            this.menuItemInertia.Checked = World.Settings.CameraHasInertia;
        }
        private void menuItemShowLatLonLines_Click(object sender, System.EventArgs e)
        {
            this.SetLatLonGridShow(!World.Settings.ShowLatLonLines);
        }
        private void menuItemVerticalExaggerationChange(object sender, System.EventArgs e)
        {
            MenuItem multiplier = (MenuItem)sender;
            this.VerticalExaggeration = Single.Parse(multiplier.Text.Replace("x", ""), CultureInfo.CurrentCulture);
        }
        private void menuItemPlanetAxis_Click(object sender, System.EventArgs e)
        {
            World.Settings.ShowPlanetAxis = !World.Settings.ShowPlanetAxis;
            this.menuItemPlanetAxis.Checked = World.Settings.ShowPlanetAxis;
        }
        private void menuItemCoordsToClipboard_Click(object sender, System.EventArgs e)
        {
            if (this.worldWindow.CurrentWorld == null)
                return;
            WorldWindUri uri = new WorldWindUri(worldWindow.CurrentWorld.Name, worldWindow.DrawArgs.WorldCamera);
            string uriString = uri.ToString();
            Clipboard.SetDataObject(uriString, true);
        }
        private void menuItemQuit_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        private void menuItemLayerManager_Click(object sender, System.EventArgs e)
        {
            World.Settings.ShowLayerManager = !World.Settings.ShowLayerManager;
            this.worldWindow.ShowLayerManager = World.Settings.ShowLayerManager;
            this.menuItemLayerManager.Checked = World.Settings.ShowLayerManager;
        }
        private void menuItemModisHotSpots_Click(object sender, System.EventArgs e)
        {
            if (this.animatedEarthMananger != null && this.animatedEarthMananger.Visible)
            {
                this.menuItemAnimatedEarth.Checked = false;
                this.animatedEarthMananger.Reset();
                this.animatedEarthMananger.Visible = false;
            }

            if (this.wmsBrowser != null && this.wmsBrowser.Visible)
            {
                this.menuItemWMS.Checked = false;
                //this.wmsBrowser.Reset();
                this.wmsBrowser.Visible = false;
            }

            if (this.rapidFireModisManager == null)
            {
                this.rapidFireModisManager = new RapidFireModisManager(this.worldWindow);
                this.rapidFireModisManager.Icon = this.Icon;
            }

            menuItemModisHotSpots.Checked = !menuItemModisHotSpots.Checked;
            this.rapidFireModisManager.Visible = this.menuItemModisHotSpots.Checked;
            this.rapidFireModisManager.WindowState = FormWindowState.Normal;
            this.toolBarButtonRapidFireModis.Pushed = true;
            UpdateToolBarStates();
        }
        private void menuItemAnimatedEarth_Click(object sender, System.EventArgs e)
        {
            if (this.rapidFireModisManager != null && this.rapidFireModisManager.Visible)
            {
                this.menuItemModisHotSpots.Checked = false;
                this.rapidFireModisManager.Reset();
                this.rapidFireModisManager.Visible = false;
            }

            if (this.wmsBrowser != null && this.wmsBrowser.Visible)
            {
                this.menuItemWMS.Checked = false;
                //this.wmsBrowser.Reset();
                this.wmsBrowser.Visible = false;
            }

            if (this.animatedEarthMananger == null)
            {
                this.animatedEarthMananger = new AnimatedEarthManager(this.worldWindow);
                this.animatedEarthMananger.Icon = this.Icon;
            }

            menuItemAnimatedEarth.Checked = !menuItemAnimatedEarth.Checked;
            this.animatedEarthMananger.Visible = this.menuItemAnimatedEarth.Checked;
            this.animatedEarthMananger.WindowState = FormWindowState.Normal;
            UpdateToolBarStates();
        }
        private void menuItemWMS_Click(object sender, System.EventArgs e)
        {
            if (this.rapidFireModisManager != null && this.rapidFireModisManager.Visible)
            {
                this.menuItemModisHotSpots.Checked = false;
                this.rapidFireModisManager.Reset();
                this.rapidFireModisManager.Visible = false;
            }

            if (this.animatedEarthMananger != null && this.animatedEarthMananger.Visible)
            {
                this.menuItemAnimatedEarth.Checked = false;
                this.animatedEarthMananger.Reset();
                this.animatedEarthMananger.Visible = false;
            }

            if (this.wmsBrowser == null)
            {
                this.wmsBrowser = new WMSBrowser(this.worldWindow);
                this.wmsBrowser.Icon = this.Icon;
            }

            menuItemWMS.Checked = !menuItemWMS.Checked;
            this.wmsBrowser.Visible = menuItemWMS.Checked;
            this.wmsBrowser.WindowState = FormWindowState.Normal;
            UpdateToolBarStates();
        }
        private void menuItemWMSImporter_Click(object sender, System.EventArgs e)
        {
            if (this.wmsImporter == null)
            {
                this.wmsImporter = new WMSBrowserNG(this.worldWindow);
                this.wmsImporter.Icon = this.Icon;
            }

            this.wmsImporter.Visible = true;
            this.wmsImporter.WindowState = FormWindowState.Normal;
        }
        private void menuItemLoadFile_Click(object sender, System.EventArgs e)
        {
            fileLoaderDialog = new FileLoader(this);
            fileLoaderDialog.Show();
        }
        private void menuItemSaveScreenShot_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save screenshot as";
            dlg.RestoreDirectory = true;
            dlg.Filter = "Portable Network Graphics (*.png)|*.png|JPEG (*.jpg)|*.jpg|Windows Bitmap (*.bmp)|*.bmp|Targa (*.tga)|*.tga";
            dlg.FileName = string.Format(CultureInfo.InvariantCulture,
                "{0:f5}{1}_{2:f5}{3}",
                Math.Abs(worldWindow.DrawArgs.WorldCamera.Longitude.Degrees),
                worldWindow.DrawArgs.WorldCamera.Longitude > Angle.Zero ? "E" : "W",
                Math.Abs(worldWindow.DrawArgs.WorldCamera.Latitude.Degrees),
                worldWindow.DrawArgs.WorldCamera.Latitude > Angle.Zero ? "N" : "S");

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            // SaveFileDialog fails to add extension when the filename contains dots.
            string ext = "*" + Path.GetExtension(dlg.FileName).ToLower(CultureInfo.InvariantCulture);
            if (dlg.Filter.IndexOf(ext) < 0)
            {
                // Extension missing, add it
                ext = dlg.Filter.Split('|')[dlg.FilterIndex * 2 - 1].Substring(1);
                dlg.FileName += ext;
            }

            this.worldWindow.SaveScreenshot(dlg.FileName);
        }
        private void menuItemWorkOffline_Click(object sender, System.EventArgs e)
        {
            World.Settings.WorkOffline = !this.menuItemWorkOffline.Checked;
            this.menuItemWorkOffline.Checked = World.Settings.WorkOffline;
            World.Settings.ShowDownloadIndicator = World.Settings.WorkOffline ?
                false : true;
        }
        private void menuItemShowToolbar_Click(object sender, System.EventArgs e)
        {
            World.Settings.ShowToolbar = !World.Settings.ShowToolbar;
            this.menuItemShowToolbar.Checked = World.Settings.ShowToolbar;
        }
        private void menuItemAlwaysOnTop_Click(object sender, System.EventArgs e)
        {
            this.menuItemAlwaysOnTop.Checked = !this.menuItemAlwaysOnTop.Checked;
            this.TopMost = this.menuItemAlwaysOnTop.Checked;
        }
        private void menuItemLockPlanetAxis_Click(object sender, System.EventArgs e)
        {
            World.Settings.CameraTwistLock = !World.Settings.CameraTwistLock;
            menuItemLockPlanetAxis.Checked = World.Settings.CameraTwistLock;
            if (World.Settings.CameraTwistLock)
            {
                // Reset North = Up
                worldWindow.DrawArgs.WorldCamera.SetPosition(double.NaN, double.NaN, 0, double.NaN, double.NaN, 0);
            }
        }
        private void menuItemCameraBanking_Click(object sender, System.EventArgs e)
        {
            World.Settings.CameraBankLock = !World.Settings.CameraBankLock;
            menuItemCameraBanks.Checked = World.Settings.CameraBankLock;
        }
        private void menuItemSunShading_Click(object sender, System.EventArgs e)
        {
            World.Settings.EnableSunShading = !World.Settings.EnableSunShading;
            menuItemSunShading.Checked = World.Settings.EnableSunShading;
        }
        private void menuItemAtmosphericScattering_Click(object sender, System.EventArgs e)
        {
            World.Settings.EnableAtmosphericScattering = !World.Settings.EnableAtmosphericScattering;
            menuItemAtmosphericScattering.Checked = World.Settings.EnableAtmosphericScattering;
        }
        private void menuItemReset_Click(object sender, System.EventArgs e)
        {
            worldWindow.DrawArgs.WorldCamera.Reset();
        }
        private void menuItemRefreshCurrentView_Click(object sender, System.EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show(this,
                "Warning: This will delete the section of cached data for this area.",
                "WARNING",
                MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                //Iterate through all "On" QuadTileSet layers
                foreach (RenderableObject ro in worldWindow.CurrentWorld.RenderableObjects.ChildObjects)
                {
                    if (ro.IsOn)
                        resetQuadTileSetCache(ro);
                }
            }
        }
        private void menuItemOptions_Click(object sender, System.EventArgs e)
        {
            using (PropertyBrowserForm worldSettings = new PropertyBrowserForm(World.Settings, "World Settings"))
            {
                worldSettings.Icon = this.Icon;
                worldSettings.ShowDialog();
            }
        }
        private void menuItemConfigWizard_Click(object sender, System.EventArgs e)
        {
            ConfigurationWizard.Wizard wizard = new ConfigurationWizard.Wizard(Settings);
            wizard.ShowDialog();
        }
        void menuItemEditPaste_Click(object sender, System.EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (!iData.GetDataPresent(DataFormats.Text))
                return;
            string clipBoardString = (string)iData.GetData(DataFormats.Text);
            try
            {
                worldWindUri = WorldWindUri.Parse(clipBoardString);
                ProcessWorldWindUri();
            }
            catch (UriFormatException caught)
            {
                MessageBox.Show(caught.Message, "Unable to paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void menuItemPluginManager_Click(object sender, System.EventArgs e)
        {
            if (pluginManager != null)
            {
                pluginManager.Dispose();
                pluginManager = null;
            }

            pluginManager = new PluginDialog(compiler);
            pluginManager.Icon = this.Icon;
            pluginManager.Show();
        }
        private void menuItemDownloadPlugins_Click(object sender, System.EventArgs e)
        {
            string message = "The following list of additional plugins is developed and maintained\nby the World Wind open source community and is neither guaranteed\nnor endorsed by NASA.\n\nContinue to open the plugin web site?";
            string caption = "Navigation warning";
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons);

            if (result == DialogResult.OK)
            {
                BrowseTo("http://www.worldwindcentral.com/wiki/Add-on_Launchpad");
            }
        }
        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            World.Settings.BrowserSize = this.splitContainer.SplitterDistance;
            // keep worldWindow panel from getting junky when resized
            worldWindow.Render();
            worldWindow.Focus();
        }
        private void menuItemBrowserOrientation_Click(object sender, EventArgs e)
        {
            World.Settings.BrowserOrientationHorizontal = !World.Settings.BrowserOrientationHorizontal;
            this.splitContainer.Orientation = getWebBrowserOrientationFromSetting(World.Settings.BrowserOrientationHorizontal);
            worldWindow.Render();

        }
        private Orientation getWebBrowserOrientationFromSetting(bool isHorizontal)
        {
            if (isHorizontal)
                return Orientation.Horizontal;
            else return Orientation.Vertical;
        }
        private void menuItemUseInternalBrowser_Click(object sender, EventArgs e)
        {
            World.Settings.UseInternalBrowser = !World.Settings.UseInternalBrowser;
            menuItemUseInternalBrowser.Checked = World.Settings.UseInternalBrowser;
        }
        private void webBrowserVisible_Click(object sender, EventArgs e)
        {
            webBrowserVisible(!World.Settings.BrowserVisible);
        }
        private void webBrowserPanel_Navigated(string url)
        {
            if (!url.StartsWith(@"worldwind://"))
                webBrowserVisible(true);
        }
        private void webBrowserPanel_Close()
        {
            webBrowserVisible(false);
        }
        #endregion

        #region IGlobe ��Ա
        public void SetVerticalExaggeration(double exageration)
        {
            World.Settings.VerticalExaggeration = (float)exageration;
        }
        public void SetDisplayMessages(System.Collections.IList messages)
        {
            this.worldWindow.SetDisplayMessages(messages);
        }
        public void SetLayers(System.Collections.IList layers)
        {
            this.worldWindow.SetLayers(layers);
        }
        public void SetWmsImage(WmsDescriptor imageA,
            WmsDescriptor imageB, double alpha)
        {
            this.SetWmsImage(imageA, imageB, alpha);
        }
        public void SetViewDirection(string type, double horiz, double vert, double elev)
        {
            this.worldWindow.SetViewDirection(type, horiz, vert, elev);
        }
        public void SetViewPosition(double degreesLatitude, double degreesLongitude,
            double metersElevation)
        {
            this.worldWindow.SetViewPosition(degreesLatitude, degreesLongitude,
                metersElevation);
        }
        public void SetLatLonGridShow(bool show)
        {
            World.Settings.ShowLatLonLines = show;
            if (this.worldWindow != null)
            {
                this.toolBarButtonLatLonLines.Pushed = World.Settings.ShowLatLonLines;
                this.menuItemShowLatLonLines.Checked = World.Settings.ShowLatLonLines;
                this.worldWindow.Invalidate();
            }
        }
        #endregion

        #region �ű�ʵ����
        private Timeline.ScriptPlayer currentScriptPlayer;
        private void menuItemPlayScript_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML Script Files (*.xml)|*.xml";
            dialog.CheckFileExists = true;
            dialog.InitialDirectory = Path.Combine(DirectoryPath, "Scripts");

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                String scriptPath = dialog.FileName;
                this.currentScriptPlayer =
                    new Timeline.ScriptPlayer(scriptPath, worldWindow);
                this.currentScriptPlayer.StatusChanged +=
                    new Timeline.ScriptPlayer.StatusChangeHandler(
                    this.scriptPlayerStatusChange);
                this.currentScriptPlayer.Start();
            }
        }
        private void menuItemStopScript_Click(object sender, System.EventArgs e)
        {
            if (this.currentScriptPlayer != null)
                this.currentScriptPlayer.Stop();
        }
        private void scriptPlayerStatusChange(Timeline.ScriptPlayer player,
            Timeline.ScriptPlayer.StatusChange change)
        {
            switch (change)
            {
                case Timeline.ScriptPlayer.StatusChange.ScriptStarted:
                    this.menuItemPlayScript.Enabled = false;
                    this.menuItemStopScript.Enabled = true;
                    break;
                case Timeline.ScriptPlayer.StatusChange.ScriptStopped:
                case Timeline.ScriptPlayer.StatusChange.ScriptEnded:
                    this.menuItemPlayScript.Enabled = true;
                    this.menuItemStopScript.Enabled = false;
                    this.restorePreScriptState();
                    break;
            }
        }
        private void restorePreScriptState()
        {
            // TODO: This strategy of explicitly restoring certain script
            // parameters is very weak. A more robust strategy -- such as a
            // stack-based state capture -- is needed for the long term.

            // Certain parameters should probably not be restored. These
            // probably include view position and direction, and vertical
            // exaggeration. The parameters that must be restored, however
            // are those that have no control through the user interface.
            // These include -- as of 3 February, 2005 -- DisplayMessage.
            this.SetDisplayMessages(null);
        }
        #endregion

        #region Uri ���
        public void QuickInstall(string path)
        {
            if (worldWindUri == null)
            {
                worldWindUri = new WorldWindUri();
            }
            worldWindUri.PreserveCase = "worldwind://install=" + path;
            ProcessInstallEncodedUri();
        }
        private void ProcessWorldWindUri()
        {
            if (worldWindUri.RawUrl.IndexOf("wmsimage") >= 0)
                ProcessWmsEncodedUri();

            if (worldWindUri.RawUrl.IndexOf("install") >= 0)
                ProcessInstallEncodedUri();

            worldWindow.Goto(worldWindUri);
            worldWindUri = null;
        }
        private void ProcessInstallEncodedUri()
        {
            //parse install URI
            string urls = worldWindUri.PreserveCase.Substring(20, worldWindUri.PreserveCase.Length - 20);
            urls.Replace(";", ",");
            string[] urllist = urls.Split(',');
            WebDownload zipURL = new WebDownload();

            string zipFilePath = "";

            foreach (string cururl in urllist)
            {
                DialogResult result = MessageBox.Show("Do you want to install the addon from '" + cururl +
                    "'?\n\nWARNING: This will overwrite existing files.", "Installing Add-ons",
                    MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:

                        zipFilePath = cururl;   //default to the url

                        //Go ahead and download if remote and not in offline mode
                        if (cururl.StartsWith("http") && !World.Settings.WorkOffline)
                        {
                            try
                            {
                                //It's a web file - download it first
                                zipURL.Url = cururl;

                                if (cururl.EndsWith("zip"))
                                {
                                    zipFilePath = Path.Combine(Path.GetTempPath(), "WWAddon.zip");
                                }
                                else if (cururl.EndsWith("xml"))
                                {
                                    zipFilePath = Path.Combine(Path.GetTempPath(), "WWAddon.xml");
                                }

                                //MessageBox.Show("Click OK to begin downloading.  World Wind may be unresponsive while it is downloading - please wait.","Downloading...");
                                zipURL.DownloadFile(zipFilePath);
                                //MessageBox.Show("File downloaded!  Click OK to install.", "File done!");
                            }
                            catch
                            {
                                MessageBox.Show("Could not download file.\nError: " + zipURL.Exception.Message + "\nURL: " + cururl, "Error");
                            }
                        }

                        if (zipFilePath.EndsWith("xml"))
                        {
                            FileInfo source = new FileInfo(zipFilePath);
                            string targetLocation = DirectoryPath + Path.DirectorySeparatorChar + "Config" + Path.DirectorySeparatorChar + WorldWindow.CurrentWorld.Name + Path.DirectorySeparatorChar + source.Name;
                            FileInfo target = new FileInfo(targetLocation);
                            if (target.Exists)
                                target.Delete();

                            source.MoveTo(target.FullName);
                            LoadAddon(source.FullName);
                            MessageBox.Show("Install completed");
                            return;
                        }
                        else if (zipFilePath.EndsWith("cs"))
                        {
                            MessageBox.Show("TODO: load plugins maybe?");
                        }

                        // handle zipped files on disk
                        try
                        {
                            FastZip fz = new FastZip();
                            fz.ExtractZip(zipFilePath, MainApplication.DirectoryPath, "");

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error unzipping add-on:\n\n" + ex.Message);
                            return;
                        }

                        try
                        {
                            string ManifestFile = Path.Combine(MainApplication.DirectoryPath, "manifest.txt");
                            if (File.Exists(ManifestFile))
                            {
                                StreamReader fs = new StreamReader(ManifestFile);
                                string line;
                                while ((line = fs.ReadLine()) != null)
                                {
                                    line.Trim();

                                    if (line.Length > 0)
                                    {
                                        if (line.StartsWith("#") || line.StartsWith("//") || line.StartsWith("\t"))
                                            continue;

                                        FileInfo fi = new FileInfo(DirectoryPath + Path.DirectorySeparatorChar + line);
                                        if (fi.Exists && fi.Extension == ".xml")
                                        {
                                            LoadAddon(fi.FullName);
                                        }
                                        else if (fi.Exists && fi.Extension == ".cs")
                                        {
                                            MessageBox.Show("TODO: load plugins maybe?");
                                        }
                                        else
                                        {
                                            MessageBox.Show("File listed in manifest does not exist or is of an unknown type: \n\n" + line, "Error reading manifest");
                                        }
                                    }
                                }
                                fs.Close();
                                File.Delete(Path.Combine(MainApplication.DirectoryPath, "manifest.txt"));
                            }
                            else
                            {
                                MessageBox.Show("Add-on manifest not found.  Restart World Wind to use this add-on.", "Restart required");
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + "\n\nRestart World Wind to use this add-on.", "Error reading manifest file");
                        }

                        //delete the temp file, if installing from local zip then do not delete
                        if (cururl.StartsWith("http"))
                            File.Delete(zipFilePath);

                        MessageBox.Show("Install completed succussfully.");
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        return; //They hit cancel - stop downloading stuff
                }
            }
        }
        private void ProcessWmsEncodedUri()
        {
            //first parse the rawUri string looking for "functions"
            string uri = worldWindUri.RawUrl.Substring(12, worldWindUri.RawUrl.Length - 12);
            uri = uri.Replace("wmsimage=", "wmsimage|");
            uri = uri.Replace("&wmsimage", "#wmsimage");
            string[] uriFunctions = uri.Split('#');

            foreach (string uriFunction in uriFunctions)
            {
                string[] paramValuePair = uriFunction.Split('|');

                if (String.Compare(paramValuePair[0], "wmsimage", true, CultureInfo.InvariantCulture) == 0)
                {
                    string displayName = null;
                    int transparencyPercent = 0;
                    double heightAboveSurface = 0.0;
                    string wmslink = "";
                    string[] wmsImageParams = new string[0];
                    if (paramValuePair[1].IndexOf("://") > 0)
                    {
                        wmsImageParams = paramValuePair[1].Replace("%26", "|").Split('|');
                    }
                    else
                    {
                        wmsImageParams = System.Web.HttpUtility.UrlDecode(paramValuePair[1]).Replace("%26", "|").Split('|');
                    }
                    foreach (string p in wmsImageParams)
                    {
                        string new_p = p.Replace("%3d", "|");
                        char[] deliminator = new char[1] { '|' };
                        string[] functionParam = new_p.Split(deliminator, 2);

                        if (String.Compare(functionParam[0], "displayname", true, CultureInfo.InvariantCulture) == 0)
                        {
                            displayName = functionParam[1];
                        }
                        else if (String.Compare(functionParam[0], "transparency", true, CultureInfo.InvariantCulture) == 0)
                        {
                            transparencyPercent = Int32.Parse(functionParam[1], CultureInfo.InvariantCulture);
                        }
                        else if (String.Compare(functionParam[0], "altitude", true, CultureInfo.InvariantCulture) == 0)
                        {
                            heightAboveSurface = Double.Parse(functionParam[1], CultureInfo.InvariantCulture);
                        }
                        else if (String.Compare(functionParam[0], "link", true, CultureInfo.InvariantCulture) == 0)
                        {
                            wmslink = functionParam[1];
                            if (wmslink.EndsWith("/"))
                                wmslink = wmslink.Substring(0, wmslink.Length - 1);
                        }
                    }

                    try
                    {
                        string[] wmslinkParams = wmslink.Split('?')[1].Split('&');

                        string wmsLayerName = null;
                        LayerSet.Type_LatitudeCoordinate2 bb_north = new LayerSet.Type_LatitudeCoordinate2();
                        LayerSet.Type_LatitudeCoordinate2 bb_south = new LayerSet.Type_LatitudeCoordinate2();
                        LayerSet.Type_LongitudeCoordinate2 bb_west = new LayerSet.Type_LongitudeCoordinate2();
                        LayerSet.Type_LongitudeCoordinate2 bb_east = new LayerSet.Type_LongitudeCoordinate2();

                        foreach (string wmslinkParam in wmslinkParams)
                        {
                            string linkParamUpper = wmslinkParam.ToUpper(CultureInfo.InvariantCulture);
                            if (linkParamUpper.IndexOf("BBOX") >= 0)
                            {
                                string[] bb_parts = wmslinkParam.Split('=')[1].Split(',');
                                bb_west.AddValue2(new LayerSet.ValueType4(bb_parts[0]));
                                bb_south.AddValue2(new LayerSet.ValueType3(bb_parts[1]));
                                bb_east.AddValue2(new LayerSet.ValueType4(bb_parts[2]));
                                bb_north.AddValue2(new LayerSet.ValueType3(bb_parts[3]));
                            }
                            else if (linkParamUpper.IndexOf("LAYERS") >= 0)
                            {
                                wmsLayerName = wmslinkParam.Split('=')[1];
                            }
                        }

                        string path = String.Format(CultureInfo.InvariantCulture,
                            @"{0}\{1}\___DownloadedWMSImages.xml", Settings.ConfigPath, "");//this.currentWorld.LayerDirectory.Value);

                        string texturePath = string.Format(CultureInfo.InvariantCulture,
                            @"{0}\Data\DownloadedWMSImages\{1}", DirectoryPath, System.DateTime.Now.ToFileTimeUtc());

                        if (!File.Exists(path))
                        {
                            LayerSet.LayerSetDoc newDoc = new LayerSet.LayerSetDoc();
                            LayerSet.Type_LayerSet root = new LayerSet.Type_LayerSet();

                            root.AddName(new LayerSet.NameType2("Downloaded WMS Images"));
                            root.AddShowAtStartup(new Altova.Types.SchemaBoolean(true));
                            root.AddShowOnlyOneLayer(new Altova.Types.SchemaBoolean(false));
                            newDoc.SetRootElementName("", "LayerSet");
                            newDoc.Save(path, root);
                        }

                        LayerSet.LayerSetDoc doc = new LayerSet.LayerSetDoc();
                        LayerSet.Type_LayerSet curRoot = new LayerSet.Type_LayerSet(doc.Load(path));

                        if (displayName == null)
                        {
                            displayName = wmslink.Split('?')[0] + " - " + wmsLayerName + " : " + System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToLongTimeString();
                        }

                        for (int i = 0; i < curRoot.ImageLayerCount; i++)
                        {
                            LayerSet.Type_ImageLayer curImageLayerType = (LayerSet.Type_ImageLayer)curRoot.GetImageLayerAt(i);
                            if (curImageLayerType.Name.Value.Equals(displayName))
                            {
                                displayName += String.Format(CultureInfo.CurrentCulture, " : {0} {1}", System.DateTime.Now.ToShortDateString(), System.DateTime.Now.ToLongTimeString());
                            }
                        }

                        LayerSet.Type_ImageLayer newImageLayer = new LayerSet.Type_ImageLayer();
                        newImageLayer.AddShowAtStartup(new Altova.Types.SchemaBoolean(false));

                        if (bb_north.Value2.DoubleValue() - bb_south.Value2.DoubleValue() > 90 ||
                            bb_east.Value2.DoubleValue() - bb_west.Value2.DoubleValue() > 90)
                            heightAboveSurface = 10000.0;

                        newImageLayer.AddName(new LayerSet.NameType(
                            displayName));
                        newImageLayer.AddDistanceAboveSurface(new Altova.Types.SchemaDecimal(heightAboveSurface));

                        LayerSet.Type_LatLonBoundingBox2 bb = new LayerSet.Type_LatLonBoundingBox2();

                        bb.AddNorth(bb_north);
                        bb.AddSouth(bb_south);
                        bb.AddWest(bb_west);
                        bb.AddEast(bb_east);
                        newImageLayer.AddBoundingBox(bb);
                        newImageLayer.AddTexturePath(new Altova.Types.SchemaString(
                            texturePath));

                        byte opacityValue = (byte)((100.0 - transparencyPercent) * 0.01 * 255);
                        newImageLayer.AddOpacity(new LayerSet.OpacityType(opacityValue.ToString(CultureInfo.InvariantCulture)));
                        newImageLayer.AddTerrainMapped(new Altova.Types.SchemaBoolean(false));

                        curRoot.AddImageLayer(newImageLayer);
                        doc.Save(path, curRoot);

                        ImageLayer newLayer = new ImageLayer(
                            displayName,
                            this.worldWindow.CurrentWorld,
                            (float)heightAboveSurface,
                            texturePath,
                            (float)bb_south.Value2.DoubleValue(),
                            (float)bb_north.Value2.DoubleValue(),
                            (float)bb_west.Value2.DoubleValue(),
                            (float)bb_east.Value2.DoubleValue(),
                            0.01f * (100.0f - transparencyPercent),
                            this.worldWindow.CurrentWorld.TerrainAccessor);
                        newLayer.ImageUrl = wmslink;

                        RenderableObjectList downloadedImagesRol = (RenderableObjectList)this.worldWindow.CurrentWorld.RenderableObjects.GetObject("Downloaded WMS Images");
                        if (downloadedImagesRol == null)
                            downloadedImagesRol = new RenderableObjectList("Downloaded WMS Images");

                        this.worldWindow.CurrentWorld.RenderableObjects.Add(newLayer);

                        worldWindUri.Latitude = Angle.FromDegrees(0.5 * (bb_north.Value2.DoubleValue() + bb_south.Value2.DoubleValue()));
                        worldWindUri.Longitude = Angle.FromDegrees(0.5 * (bb_west.Value2.DoubleValue() + bb_east.Value2.DoubleValue()));

                        if (bb_north.Value2.DoubleValue() - bb_south.Value2.DoubleValue() > bb_east.Value2.DoubleValue() - bb_west.Value2.DoubleValue())
                            worldWindUri.ViewRange = Angle.FromDegrees(bb_north.Value2.DoubleValue() - bb_south.Value2.DoubleValue());
                        else
                            worldWindUri.ViewRange = Angle.FromDegrees(bb_east.Value2.DoubleValue() - bb_west.Value2.DoubleValue());

                        if (worldWindUri.ViewRange.Degrees > 180)
                            worldWindUri.ViewRange = Angle.FromDegrees(180);

                        System.Threading.Thread.Sleep(10);
                    }
                    catch
                    { }
                }
            }
        }
        #endregion

        #region ���·���
        private void UpdateToolBarStates()
        {
            // Use menu items as master settings, slave toolbar button states
            this.toolBarButtonLayerManager.Pushed = this.menuItemLayerManager.Checked;
            this.toolBarButtonWMS.Pushed = this.menuItemWMS.Checked;
            this.toolBarButtonAnimatedEarth.Pushed = this.menuItemAnimatedEarth.Checked;
            this.toolBarButtonRapidFireModis.Pushed = this.menuItemModisHotSpots.Checked;
        }
        #endregion

        #region ����������
        private void OpenStartupWorld()
        {
            string startupWorldName = null;
            if (worldWindUri != null)
            {
                foreach (string curWorld in availableWorldList.Keys)
                    if (string.Compare(worldWindUri.World, curWorld, true, CultureInfo.InvariantCulture) == 0)
                    {
                        startupWorldName = curWorld;
                        break;
                    }
                if (startupWorldName == null)
                {
                    //	Log.Write(startupWorldName + " - 1");
                    //	MessageBox.Show(this,
                    //		String.Format("Unable to find data for planet '{0}', loading first available planet.", worldWindUri.World));
                    //	throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, "Unable to find data for planet '{0}'.", worldWindUri.World ) );
                }
            }

            if (startupWorldName == null && availableWorldList.Contains(Settings.DefaultWorld))
            {
                startupWorldName = Settings.DefaultWorld;
            }

            if (startupWorldName == null)
            {
                // Pick the first planet found in config
                foreach (string curWorld in availableWorldList.Keys)
                {
                    startupWorldName = curWorld;
                    break;
                }
            }

            this.splashScreen.SetText("Initializing " + startupWorldName + "...");
            if (startupWorldName != null)
            {
                //WorldXmlDescriptor.WorldType worldDescriptor = (WorldXmlDescriptor.WorldType)this.availableWorldList[startupWorldName];
                string curWorldFile = availableWorldList[startupWorldName] as string;
                if (curWorldFile == null)
                {
                    throw new ApplicationException(
                        string.Format(CultureInfo.CurrentCulture, "Unable to load planet {0} configuration file from '{1}'.",
                        startupWorldName,
                        Settings.ConfigPath));
                }

                OpenWorld(curWorldFile);
            }
        }
        private void OpenWorld(string worldXmlFile)
        {
            if (this.worldWindow.CurrentWorld != null)
            {
                try
                {
                    this.worldWindow.ResetToolbar();
                }
                catch
                { }
                try
                {
                    foreach (PluginInfo p in this.compiler.Plugins)
                    {
                        try
                        {
                            if (p.Plugin.IsLoaded)
                                p.Plugin.Unload();
                        }
                        catch
                        { }
                    }
                }
                catch
                { }

                try
                {
                    this.worldWindow.CurrentWorld.Dispose();
                }
                catch
                { }
            }
            if (this.gotoDialog != null)
            {
                this.gotoDialog.Dispose();
                this.gotoDialog = null;
            }
            if (this.rapidFireModisManager != null)
            {
                this.rapidFireModisManager.Dispose();
                this.rapidFireModisManager = null;
            }
            if (this.animatedEarthMananger != null)
            {
                this.animatedEarthMananger.Dispose();
                this.animatedEarthMananger = null;
            }
            if (this.wmsBrowser != null)
            {
                this.wmsBrowser.Dispose();
                this.wmsBrowser = null;
            }
            worldWindow.CurrentWorld = WorldWind.ConfigurationLoader.Load(worldXmlFile, worldWindow.Cache);
            this.splashScreen.SetText("Initializing menus...");
            InitializePluginCompiler();
            foreach (RenderableObject worldRootObject in this.worldWindow.CurrentWorld.RenderableObjects.ChildObjects)
            {
                this.AddLayerMenuButtons(this.worldWindow, worldRootObject);
            }
            this.AddInternalPluginMenuButtons();
            this.menuItemModisHotSpots.Enabled = worldWindow.CurrentWorld.IsEarth;
            this.menuItemAnimatedEarth.Enabled = worldWindow.CurrentWorld.IsEarth;
        }
        #endregion

        #region ��������
        private void AddLayerMenuButtons(WorldWindow ww, RenderableObject ro)
        {
            if (ro.MetaData.Contains("ToolBarImagePath"))
            {
                string imagePath = Path.Combine(DirectoryPath, (string)ro.MetaData["ToolBarImagePath"]);
                if (File.Exists(imagePath))
                {
                    LayerShortcutMenuButton button = new LayerShortcutMenuButton(imagePath, ro);
                    ww.MenuBar.AddLayersMenuButton(button);
                    //HACK: Temporary fix
                    if (ro.Name == "Placenames")
                        button.SetPushed(World.Settings.ShowPlacenames);
                    if (ro.Name == "Boundaries")
                        button.SetPushed(World.Settings.ShowBoundaries);
                }
            }

            if (ro.GetType() == typeof(RenderableObjectList))
            {
                RenderableObjectList rol = (RenderableObjectList)ro;
                foreach (RenderableObject child in rol.ChildObjects)
                    AddLayerMenuButtons(ww, child);
            }
        }
        private NltTerrainAccessor getTerrainAccessorFromXML(WorldXmlDescriptor.TerrainAccessor curTerrainAccessorType)
        {
            double east = curTerrainAccessorType.LatLonBoundingBox.East.Value.DoubleValue();
            double west = curTerrainAccessorType.LatLonBoundingBox.West.Value.DoubleValue();
            double north = curTerrainAccessorType.LatLonBoundingBox.North.Value.DoubleValue();
            double south = curTerrainAccessorType.LatLonBoundingBox.South.Value.DoubleValue();

            NltTerrainAccessor[] subsets = null;
            if (curTerrainAccessorType.HasHigherResolutionSubsets())
            {
                subsets = new NltTerrainAccessor[curTerrainAccessorType.HigherResolutionSubsetsCount];
                for (int i = 0; i < curTerrainAccessorType.HigherResolutionSubsetsCount; i++)
                {
                    subsets[i] = this.getTerrainAccessorFromXML(curTerrainAccessorType.GetHigherResolutionSubsetsAt(i));
                }
            }

            if (curTerrainAccessorType.HasDownloadableWMSSet())
            {
                /*	WMSLayerAccessor wmsLayer = new WMSLayerAccessor();

                    wmsLayer.ImageFormat = curTerrainAccessorType.DownloadableWMSSet.ImageFormat.Value;
                    wmsLayer.IsTransparent = curTerrainAccessorType.DownloadableWMSSet.UseTransparency.Value;
                    wmsLayer.ServerGetMapUrl = curTerrainAccessorType.DownloadableWMSSet.ServerGetMapUrl.Value;
                    wmsLayer.Version = curTerrainAccessorType.DownloadableWMSSet.Version.Value;
                    wmsLayer.WMSLayerName = curTerrainAccessorType.DownloadableWMSSet.WMSLayerName.Value;

                    if(curTerrainAccessorType.DownloadableWMSSet.HasUsername())
                        wmsLayer.Username = curTerrainAccessorType.DownloadableWMSSet.Username.Value;

                    if(curTerrainAccessorType.DownloadableWMSSet.HasPassword())
                        wmsLayer.Password = curTerrainAccessorType.DownloadableWMSSet.Password.Value;

                    if(curTerrainAccessorType.DownloadableWMSSet.HasWMSLayerStyle())
                        wmsLayer.WMSLayerStyle = curTerrainAccessorType.DownloadableWMSSet.WMSLayerStyle.Value;
                    else
                        wmsLayer.WMSLayerStyle = "";

                    if(curTerrainAccessorType.DownloadableWMSSet.HasBoundingBoxOverlap())
                        wmsLayer.BoundingBoxOverlap = curTerrainAccessorType.DownloadableWMSSet.BoundingBoxOverlap.DoubleValue();

                    return new NltTerrainAccessor(
                        curTerrainAccessorType.Name.Value,
                        west,
                        south,
                        east,
                        north,
                        wmsLayer,
                        subsets
                        );
                        */
            }
            else if (curTerrainAccessorType.HasTerrainTileService())
            {
                /*string serverUrl = curTerrainAccessorType.TerrainTileService.ServerUrl.Value;
				double levelZeroTileSizeDegrees = curTerrainAccessorType.TerrainTileService.LevelZeroTileSizeDegrees.DoubleValue();
				int numberLevels = curTerrainAccessorType.TerrainTileService.NumberLevels.Value;
				int samplesPerTile = curTerrainAccessorType.TerrainTileService.SamplesPerTile.Value;
				string fileExtension = curTerrainAccessorType.TerrainTileService.FileExtension.Value;

				TerrainTileService tts = new TerrainTileService(
					serverUrl,
					curTerrainAccessorType.TerrainTileService.DataSetName.Value,
					levelZeroTileSizeDegrees,
					samplesPerTile,
					fileExtension,
					numberLevels,
					Path.Combine(this.worldWindow.Cache.CacheDirectory,
					Path.Combine(Path.Combine( worldWindow.CurrentWorld.Name, "TerrainAccessor"), curTerrainAccessorType.Name.Value )));

				return new NltTerrainAccessor(
					curTerrainAccessorType.Name.Value,
					west,
					south,
					east,
					north,
					tts,
					subsets
					);*/
            }

            return null;
        }
        private RenderableObject getRenderableObjectListFromLayerSet(World curWorld, LayerSet.Type_LayerSet curLayerSet, string layerSetFile)//ref TreeNode treeNode)
        {
            RenderableObjectList rol = null;

            // If the layer set has icons, use the icon list layer as parent
            if (curLayerSet.HasIcon())
            {
                rol = new Icons(curLayerSet.Name.Value);
                rol.RenderPriority = RenderPriority.Icons;
            }
            else
                rol = new RenderableObjectList(curLayerSet.Name.Value);

            if (curLayerSet.HasShowOnlyOneLayer())
                rol.ShowOnlyOneLayer = curLayerSet.ShowOnlyOneLayer.Value;

            // HACK: This should be part of the settings
            if (curLayerSet.Name.ToString().ToUpper() == "PLACENAMES")
                rol.RenderPriority = RenderPriority.Placenames;

            if (curLayerSet.HasExtendedInformation())
            {
                if (curLayerSet.ExtendedInformation.HasToolBarImage())
                    rol.MetaData.Add("ToolBarImagePath", curLayerSet.ExtendedInformation.ToolBarImage.Value);
            }
            if (curLayerSet.HasImageLayer())
            {
                for (int i = 0; i < curLayerSet.ImageLayerCount; i++)
                {
                    LayerSet.Type_ImageLayer curImageLayerType = curLayerSet.GetImageLayerAt(i);

                    // <TexturePath> could contain Url, relative path, or absolute path
                    string imagePath = null;
                    string imageUrl = null;
                    if (curImageLayerType.TexturePath.Value.ToLower(System.Globalization.CultureInfo.InvariantCulture).StartsWith(("http://")))
                    {
                        imageUrl = curImageLayerType.TexturePath.Value;
                    }
                    else
                    {
                        imagePath = curImageLayerType.TexturePath.Value;
                        if (!Path.IsPathRooted(imagePath))
                            imagePath = Path.Combine(DirectoryPath, imagePath);
                    }

                    int transparentColor = 0;

                    if (curImageLayerType.HasTransparentColor())
                    {
                        transparentColor = System.Drawing.Color.FromArgb(
                            curImageLayerType.TransparentColor.Red.Value,
                            curImageLayerType.TransparentColor.Green.Value,
                            curImageLayerType.TransparentColor.Blue.Value).ToArgb();

                    }

                    ImageLayer newImageLayer = new ImageLayer(
                        curImageLayerType.Name.Value,
                        curWorld,
                        (float)curImageLayerType.DistanceAboveSurface.Value,
                        imagePath,
                        (float)curImageLayerType.BoundingBox.South.Value2.DoubleValue(),
                        (float)curImageLayerType.BoundingBox.North.Value2.DoubleValue(),
                        (float)curImageLayerType.BoundingBox.West.Value2.DoubleValue(),
                        (float)curImageLayerType.BoundingBox.East.Value2.DoubleValue(),
                        (byte)curImageLayerType.Opacity.Value,
                        (curImageLayerType.TerrainMapped.Value ? curWorld.TerrainAccessor : null));

                    newImageLayer.ImageUrl = imageUrl;
                    newImageLayer.TransparentColor = transparentColor;
                    newImageLayer.IsOn = curImageLayerType.ShowAtStartup.Value;
                    if (curImageLayerType.HasLegendImagePath())
                        newImageLayer.LegendImagePath = curImageLayerType.LegendImagePath.Value;

                    if (curImageLayerType.HasExtendedInformation() && curImageLayerType.ExtendedInformation.HasToolBarImage())
                        newImageLayer.MetaData.Add("ToolBarImagePath", Path.Combine(DirectoryPath, curImageLayerType.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(newImageLayer);
                }
            }

            if (curLayerSet.HasQuadTileSet())
            {
                for (int i = 0; i < curLayerSet.QuadTileSetCount; i++)
                {
                    LayerSet.Type_QuadTileSet2 curQtsType = curLayerSet.GetQuadTileSetAt(i);

                    /*ImageAccessor imageAccessor = null;

					string permDirPath = null;
					if(curQtsType.ImageAccessor.HasPermanantDirectory())
					{
						permDirPath = curQtsType.ImageAccessor.PermanantDirectory.Value;
						if(!Path.IsPathRooted(permDirPath))
							permDirPath = Path.Combine( DirectoryPath, permDirPath );
					}

					string cacheDirPath = Path.Combine(worldWindow.Cache.CacheDirectory,
						Path.Combine(curWorld.Name,
						Path.Combine(rol.Name, curQtsType.Name.Value )));

					int transparentColor = 0;
					if(curQtsType.HasTransparentColor())
					{
						transparentColor = System.Drawing.Color.FromArgb(
							curQtsType.TransparentColor.Red.Value,
							curQtsType.TransparentColor.Green.Value,
							curQtsType.TransparentColor.Blue.Value).ToArgb();

					}
					if(curQtsType.ImageAccessor.HasWMSAccessor())
					{
						WMSLayerAccessor wmsLayerAccessor = null;
						wmsLayerAccessor = new WMSLayerAccessor();
						wmsLayerAccessor.ImageFormat = curQtsType.ImageAccessor.WMSAccessor.ImageFormat.Value;
						wmsLayerAccessor.IsTransparent = curQtsType.ImageAccessor.WMSAccessor.UseTransparency.Value;
						wmsLayerAccessor.ServerGetMapUrl = curQtsType.ImageAccessor.WMSAccessor.ServerGetMapUrl.Value;
						wmsLayerAccessor.Version = curQtsType.ImageAccessor.WMSAccessor.Version.Value;
						wmsLayerAccessor.WMSLayerName = curQtsType.ImageAccessor.WMSAccessor.WMSLayerName.Value;

						if(curQtsType.ImageAccessor.WMSAccessor.HasUsername())
							wmsLayerAccessor.Username = curQtsType.ImageAccessor.WMSAccessor.Username.Value;

						if(curQtsType.ImageAccessor.WMSAccessor.HasPassword())
							wmsLayerAccessor.Password = curQtsType.ImageAccessor.WMSAccessor.Password.Value;

						if(curQtsType.ImageAccessor.WMSAccessor.HasWMSLayerStyle())
							wmsLayerAccessor.WMSLayerStyle = curQtsType.ImageAccessor.WMSAccessor.WMSLayerStyle.Value;
						else
							wmsLayerAccessor.WMSLayerStyle = "";

						if(curQtsType.ImageAccessor.WMSAccessor.HasServerLogoFilePath())
						{
							string logoPath = Path.Combine(DirectoryPath, curQtsType.ImageAccessor.WMSAccessor.ServerLogoFilePath.Value);
							if(File.Exists(logoPath))
								wmsLayerAccessor.LogoFilePath = logoPath;
						}

						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath,
							wmsLayerAccessor);
					}
					else if(curQtsType.ImageAccessor.HasImageTileService())
					{
						string logoPath = null;
						if(curQtsType.ImageAccessor.ImageTileService.HasServerLogoFilePath())
							logoPath = Path.Combine( DirectoryPath, curQtsType.ImageAccessor.ImageTileService.ServerLogoFilePath.Value);

						ImageTileService imageTileService = new ImageTileService(
							curQtsType.ImageAccessor.ImageTileService.DataSetName.Value,
							curQtsType.ImageAccessor.ImageTileService.ServerUrl.Value,
							logoPath );

						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath,
							imageTileService);
					}
					else if(curQtsType.ImageAccessor.HasDuplicateTilePath())
					{
						string dupePath = curQtsType.ImageAccessor.DuplicateTilePath.Value;
						if(!Path.IsPathRooted(dupePath))
							dupePath = Path.Combine(DirectoryPath, dupePath);
						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath,
							dupePath);
					}
					else
					{
						imageAccessor = new ImageAccessor(
							permDirPath,
							curQtsType.ImageAccessor.TextureSizePixels.Value,
							curQtsType.ImageAccessor.LevelZeroTileSizeDegrees.DoubleValue(),
							curQtsType.ImageAccessor.NumberLevels.Value,
							curQtsType.ImageAccessor.ImageFileExtension.Value,
							cacheDirPath);
					}

					QuadTileSet qts = new QuadTileSet(
						curQtsType.Name.Value,
						curWorld,
						curQtsType.DistanceAboveSurface.DoubleValue(),
						curQtsType.BoundingBox.North.Value2.DoubleValue(),
						curQtsType.BoundingBox.South.Value2.DoubleValue(),
						curQtsType.BoundingBox.West.Value2.DoubleValue(),
						curQtsType.BoundingBox.East.Value2.DoubleValue(),
						(curQtsType.TerrainMapped.Value ? curWorld.TerrainAccessor : null),
						imageAccessor);

					qts.TransparentColor = transparentColor;

					if(curQtsType.ShowAtStartup.Value)
						qts.IsOn = true;
					else
						qts.IsOn = false;


					if(curQtsType.HasExtendedInformation() && curQtsType.ExtendedInformation.HasToolBarImage())
					{
						try
						{
							string fileName = Path.Combine(DirectoryPath, curQtsType.ExtendedInformation.ToolBarImage.Value);
							if (File.Exists(fileName))
								qts.MetaData.Add("ToolBarImagePath", fileName);
						}
						catch
						{
							// TODO: Log or display warning
						}
					}

					rol.Add(qts);*/
                }
            }

            if (curLayerSet.HasPathList())
            {
                for (int i = 0; i < curLayerSet.PathListCount; i++)
                {
                    LayerSet.Type_PathList2 newPathList = curLayerSet.GetPathListAt(i);

                    PathList pl = new PathList(
                        newPathList.Name.Value,
                        curWorld,
                        newPathList.MinDisplayAltitude.DoubleValue(),
                        newPathList.MaxDisplayAltitude.DoubleValue(),
                        DirectoryPath + "//" + newPathList.PathsDirectory.Value,
                        newPathList.DistanceAboveSurface.DoubleValue(),
                        (newPathList.HasWinColorName() ? System.Drawing.Color.FromName(newPathList.WinColorName.Value) : System.Drawing.Color.FromArgb(newPathList.RGBColor.Red.Value, newPathList.RGBColor.Green.Value, newPathList.RGBColor.Blue.Value)),
                        curWorld.TerrainAccessor);

                    pl.IsOn = newPathList.ShowAtStartup.Value;

                    if (newPathList.HasExtendedInformation() && newPathList.ExtendedInformation.HasToolBarImage())
                        pl.MetaData.Add("ToolBarImagePath", Path.Combine(DirectoryPath, newPathList.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(pl);
                }
            }

            if (curLayerSet.HasShapeFileLayer())
            {
                for (int i = 0; i < curLayerSet.ShapeFileLayerCount; i++)
                {
                    LayerSet.Type_ShapeFileLayer2 newShapefileLayer = curLayerSet.GetShapeFileLayerAt(i);
                    Microsoft.DirectX.Direct3D.FontDescription fd = GetLayerFontDescription(newShapefileLayer.DisplayFont);
                    Microsoft.DirectX.Direct3D.Font font = worldWindow.DrawArgs.CreateFont(fd);
                    ShapeLayer sp = new ShapeLayer(
                        newShapefileLayer.Name.Value,
                        curWorld,
                        newShapefileLayer.DistanceAboveSurface.DoubleValue(),
                        newShapefileLayer.MasterFilePath.Value,
                        newShapefileLayer.MinimumViewAltitude.DoubleValue(),
                        newShapefileLayer.MaximumViewAltitude.DoubleValue(),
                        font,
                        (newShapefileLayer.HasWinColorName() ? System.Drawing.Color.FromName(newShapefileLayer.WinColorName.Value) : System.Drawing.Color.FromArgb(newShapefileLayer.RGBColor.Red.Value, newShapefileLayer.RGBColor.Green.Value, newShapefileLayer.RGBColor.Blue.Value)),
                        (newShapefileLayer.HasScalarKey() ? newShapefileLayer.ScalarKey.Value : null),
                        (newShapefileLayer.HasShowBoundaries() ? newShapefileLayer.ShowBoundaries.Value : false),
                        (newShapefileLayer.HasShowFilledRegions() ? newShapefileLayer.ShowFilledRegions.Value : false));

                    sp.IsOn = newShapefileLayer.ShowAtStartup.BoolValue();

                    if (newShapefileLayer.HasExtendedInformation() && newShapefileLayer.ExtendedInformation.HasToolBarImage())
                        sp.MetaData.Add("ToolBarImagePath", Path.Combine(DirectoryPath, newShapefileLayer.ExtendedInformation.ToolBarImage.Value));

                    rol.Add(sp);
                }
            }

            if (curLayerSet.HasIcon())
            {
                Icons icons = (Icons)rol;

                for (int i = 0; i < curLayerSet.IconCount; i++)
                {
                    LayerSet.Type_Icon newIcon = curLayerSet.GetIconAt(i);

                    string textureFullPath = newIcon.TextureFilePath.Value;
                    if (textureFullPath.Length > 0 && !Path.IsPathRooted(textureFullPath))
                        // Use absolute path to icon image
                        textureFullPath = Path.Combine(DirectoryPath, newIcon.TextureFilePath.Value);

                    WorldWind.Renderable.Icon ic = new WorldWind.Renderable.Icon(
                        newIcon.Name.Value,
                        (float)newIcon.Latitude.Value2.DoubleValue(),
                        (float)newIcon.Longitude.Value2.DoubleValue(),
                        (float)newIcon.DistanceAboveSurface.DoubleValue());

                    ic.TextureFileName = textureFullPath;
                    ic.Width = newIcon.IconWidthPixels.Value;
                    ic.Height = newIcon.IconHeightPixels.Value;
                    ic.IsOn = newIcon.ShowAtStartup.Value;
                    if (newIcon.HasDescription())
                        ic.Description = newIcon.Description.Value;
                    if (newIcon.HasClickableUrl())
                        ic.ClickableActionURL = newIcon.ClickableUrl.Value;
                    if (newIcon.HasMaximumDisplayAltitude())
                        ic.MaximumDisplayDistance = (float)newIcon.MaximumDisplayAltitude.Value;
                    if (newIcon.HasMinimumDisplayAltitude())
                        ic.MinimumDisplayDistance = (float)newIcon.MinimumDisplayAltitude.Value;

                    icons.Add(ic);
                }
            }

            if (curLayerSet.HasTiledPlacenameSet())
            {
                for (int i = 0; i < curLayerSet.TiledPlacenameSetCount; i++)
                {
                    LayerSet.Type_TiledPlacenameSet2 newPlacenames = curLayerSet.GetTiledPlacenameSetAt(i);

                    string filePath = newPlacenames.PlacenameListFilePath.Value;
                    if (!Path.IsPathRooted(filePath))
                        filePath = Path.Combine(DirectoryPath, filePath);

                    Microsoft.DirectX.Direct3D.FontDescription fd = GetLayerFontDescription(newPlacenames.DisplayFont);
                    TiledPlacenameSet tps = new TiledPlacenameSet(
                        newPlacenames.Name.Value,
                        curWorld,
                        newPlacenames.DistanceAboveSurface.DoubleValue(),
                        newPlacenames.MaximumDisplayAltitude.DoubleValue(),
                        newPlacenames.MinimumDisplayAltitude.DoubleValue(),
                        filePath,
                        fd,
                        (newPlacenames.HasWinColorName() ? System.Drawing.Color.FromName(newPlacenames.WinColorName.Value) : System.Drawing.Color.FromArgb(newPlacenames.RGBColor.Red.Value, newPlacenames.RGBColor.Green.Value, newPlacenames.RGBColor.Blue.Value)),
                        (newPlacenames.HasIconFilePath() ? newPlacenames.IconFilePath.Value : null));

                    if (newPlacenames.HasExtendedInformation() && newPlacenames.ExtendedInformation.HasToolBarImage())
                        tps.MetaData.Add("ToolBarImagePath", Path.Combine(DirectoryPath, newPlacenames.ExtendedInformation.ToolBarImage.Value));

                    tps.IsOn = newPlacenames.ShowAtStartup.Value;
                    rol.Add(tps);
                }
            }

            if (curLayerSet.HasChildLayerSet())
            {
                for (int i = 0; i < curLayerSet.ChildLayerSetCount; i++)
                {
                    LayerSet.Type_LayerSet ls = curLayerSet.GetChildLayerSetAt(i);

                    rol.Add(getRenderableObjectListFromLayerSet(curWorld, ls, layerSetFile));
                }
            }

            rol.IsOn = curLayerSet.ShowAtStartup.Value;
            return rol;
        }
        protected static Microsoft.DirectX.Direct3D.FontDescription GetLayerFontDescription(LayerSet.Type_DisplayFont2 displayFont)
        {
            Microsoft.DirectX.Direct3D.FontDescription fd = new Microsoft.DirectX.Direct3D.FontDescription();
            fd.FaceName = displayFont.Family.Value;
            fd.Height = (int)((float)displayFont.Size.Value * 1.5f);
            if (displayFont.HasStyle())
            {
                LayerSet.StyleType2 layerStyle = displayFont.Style;
                if (displayFont.Style.HasIsItalic() && layerStyle.IsItalic.Value)
                    fd.IsItalic = true;
                else
                    fd.IsItalic = false;

                if (displayFont.Style.HasIsBold() && layerStyle.IsBold.Value)
                    fd.Weight = Microsoft.DirectX.Direct3D.FontWeight.Bold;
                else
                    fd.Weight = Microsoft.DirectX.Direct3D.FontWeight.Regular;
            }
            else
            {
                fd.Weight = Microsoft.DirectX.Direct3D.FontWeight.Regular;
            }
            return fd;
        }
        protected bool HandleKeyUp(KeyEventArgs e)
        {
            // keep keypresses inside browser url bar
            if (e.Handled)
                return true;

            if (e.Alt)
            {
                // Alt key down
                switch (e.KeyCode)
                {
                    case Keys.A:
                        menuItemAlwaysOnTop_Click(this, e);
                        return true;
                    case Keys.Q:
                        using (PropertyBrowserForm worldWindSettings = new PropertyBrowserForm(Settings, "World Wind Settings"))
                        {
                            worldWindSettings.Icon = this.Icon;
                            worldWindSettings.ShowDialog();
                        }
                        return true;
                    case Keys.W:
                        menuItemOptions_Click(this, EventArgs.Empty);
                        return true;
                    case Keys.Enter:
                        return true;
                    case Keys.F4:
                        Close();
                        return true;

                    //James Evans
                    case Keys.O:
                        menuItemWorkOffline_Click(this, e);
                        return true;

                }
            }
            else if (e.Control)
            {
                // Control key down
                switch (e.KeyCode)
                {
                    case Keys.C:
                    case Keys.Insert:
                        menuItemCoordsToClipboard_Click(this, e);
                        return true;
                    case Keys.F:
                        return true;
                    case Keys.H:
                        if (queueMonitor != null)
                        {
                            bool wasVisible = queueMonitor.Visible;
                            queueMonitor.Close();
                            queueMonitor.Dispose();
                            queueMonitor = null;
                            if (wasVisible)
                                return true;
                        }

                        queueMonitor = new ProgressMonitor();
                        queueMonitor.Icon = this.Icon;
                        queueMonitor.Show();
                        return true;
                    case Keys.I:
                        menuItemConfigWizard_Click(this, e);
                        return true;
                    case Keys.N:
                        menuItemOptions_Click(this, e);
                        return true;
                    case Keys.T:
                        menuItemShowToolbar_Click(this, e);
                        return true;
                    case Keys.V:
                        menuItemEditPaste_Click(this, e);
                        return true;
                    case Keys.S:
                        menuItemSaveScreenShot_Click(this, e);
                        return true;
                }
            }
            else if (e.Shift)
            {
                // Shift key down
                switch (e.KeyCode)
                {
                    case Keys.Insert:
                        menuItemEditPaste_Click(this, e);
                        return true;
                    case Keys.S:
                        menuItemSunShading_Click(this, e);
                        return true;
                    case Keys.A:
                        menuItemAtmosphericScattering_Click(this, e);
                        return true;
                }
            }
            else
            {
                // Other or no modifier key
                switch (e.KeyCode)
                {
                    //case Keys.B:
                    //	menuItemWMS_Click(this, e);
                    //	return true;
                    case Keys.G:
                        return true;
                    case Keys.L:
                        menuItemLayerManager_Click(this, e);
                        return true;
                    case Keys.P:
                        if (this.pathMaker == null)
                        {
                            this.pathMaker = new PathMaker(this.worldWindow);
                            this.pathMaker.Icon = this.Icon;
                        }
                        this.pathMaker.Visible = !this.pathMaker.Visible;
                        return true;
                    case Keys.V:
                        if (this.placeBuilderDialog == null)
                        {
                            this.placeBuilderDialog = new PlaceBuilder(this.worldWindow);
                            this.placeBuilderDialog.Icon = this.Icon;
                        }

                        this.placeBuilderDialog.Visible = !this.placeBuilderDialog.Visible;
                        return true;
                    case Keys.Escape:
                        return true;
                    case Keys.D1:
                    case Keys.NumPad1:
                        this.VerticalExaggeration = 1.0f;
                        return true;
                    case Keys.D2:
                    case Keys.NumPad2:
                        this.VerticalExaggeration = 2.0f;
                        return true;
                    case Keys.D3:
                    case Keys.NumPad3:
                        this.VerticalExaggeration = 3.0f;
                        return true;
                    case Keys.D4:
                    case Keys.NumPad4:
                        this.VerticalExaggeration = 4.0f;
                        return true;
                    case Keys.D5:
                    case Keys.NumPad5:
                        this.VerticalExaggeration = 5.0f;
                        return true;
                    case Keys.D6:
                    case Keys.NumPad6:
                        this.VerticalExaggeration = 6.0f;
                        return true;
                    case Keys.D7:
                    case Keys.NumPad7:
                        this.VerticalExaggeration = 7.0f;
                        return true;
                    case Keys.D8:
                    case Keys.NumPad8:
                        this.VerticalExaggeration = 8.0f;
                        return true;
                    case Keys.D9:
                    case Keys.NumPad9:
                        this.VerticalExaggeration = 9.0f;
                        return true;
                    case Keys.D0:
                    case Keys.NumPad0:
                        this.VerticalExaggeration = 0.0f;
                        return true;
                    case Keys.F1:
                        this.menuItemAnimatedEarth_Click(this, e);
                        return true;
                    case Keys.F2:
                        this.menuItemModisHotSpots_Click(this, e);
                        return true;
                    case Keys.F5:
                        this.menuItemRefreshCurrentView_Click(this, e);
                        return true;
                    case Keys.F6:
                        return true;
                    case Keys.F7:
                        this.menuItemShowLatLonLines_Click(this, e);
                        return true;
                    case Keys.F8:
                        this.menuItemPlanetAxis_Click(this, e);
                        return true;
                    case Keys.F9:
                        this.menuItemShowCrosshairs_Click(this, e);
                        return true;
                    case Keys.F10:
                        this.menuItemShowPosition_Click(this, e);
                        return true;
                    case Keys.F11:
                        this.menuItemConstantMotion_Click(this, e);
                        return true;
                    case Keys.F12:
                        this.menuItemPointGoTo_Click(this, e);
                        return true;
                }
            }
            return false;
        }
        private void resetQuadTileSetCache(RenderableObject ro)
        {
            if (ro.IsOn && ro is QuadTileSet)
            {
                QuadTileSet qts = (QuadTileSet)ro;
                qts.ResetCacheForCurrentView(worldWindow.DrawArgs.WorldCamera);
            }
            else if (ro is RenderableObjectList)
            {
                RenderableObjectList rol = (RenderableObjectList)ro;
                foreach (RenderableObject curRo in rol.ChildObjects)
                {
                    resetQuadTileSetCache(curRo);
                }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // To close queueMonitor to avoid threads lock problems
            if (queueMonitor != null) this.queueMonitor.Close();
            if (compiler != null)
                compiler.Dispose();
        }
        #endregion
    }
}
