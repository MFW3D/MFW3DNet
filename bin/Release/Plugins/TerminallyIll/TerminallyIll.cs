//----------------------------------------------------------------------------
// NAME: TerminallyIll WW Python Console
// VERSION: 0.3
// DESCRIPTION: Embedding Python into World Wind
// DEVELOPER: Ranen Ghosh
// WEBSITE: http://ranenghosh.com/WorldWind/TerminallyIll/
//----------------------------------------------------------------------------
// 0.3  Mar 01 2007     Added basic env setup
// 0.2  Feb 25 2007     Removed bogus REFERENCES line
// 0.1  Feb 25 2007     First version by Ranen Ghosh
//----------------------------------------------------------------------------
//
using WorldWind;
using System;
using System.Windows.Forms;
using System.Reflection;

namespace WWPython.TerminallyIll
{
    public class TerminalWindow : System.Windows.Forms.Form
    {
        Assembly IronTextBox = Assembly.LoadFrom("IronTextBox.dll");
        TerminallyIll ti;
        string m_PluginDirectory;

        public TerminalWindow(TerminallyIll argTi, string PluginDirectory)
        {
            this.ti = argTi;
            InitializeComponent();
            m_PluginDirectory = PluginDirectory;
        }

        private System.Windows.Forms.Control itb;
        private Type itbType;

        private System.ComponentModel.IContainer components =  null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected void InitializeComponent()
        {
            this.Visible = false;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TerminalWindow));
            Type[] types = IronTextBox.GetExportedTypes();
            itbType = null;
            foreach (Type type in types)
            {
                if (type.Name == "IronTextBoxControl")
                    itbType = type;
            }
            if (itbType == null)
                return;

            this.itb = (System.Windows.Forms.Control)(IronTextBox.CreateInstance("UIIronTextBox.IronTextBoxControl"));
            this.SuspendLayout();
            // 
            // itb
            // 
            itbType.InvokeMember("ConsoleTextBackColor",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                    Type.DefaultBinder,
                    itb,
                    new System.Object[] { System.Drawing.Color.White });
            itbType.InvokeMember("ConsoleTextFont",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                    Type.DefaultBinder,
                    itb,
                    new System.Object[] { 
                            new System.Drawing.Font(
                                    "Lucida Console",
                                    8.25F,
                                    System.Drawing.FontStyle.Regular,
                                    System.Drawing.GraphicsUnit.Point,
                                    ((byte)(0))
                                    )
                            });
            itbType.InvokeMember("ConsoleTextForeColor",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                    Type.DefaultBinder,
                    itb,
                    new System.Object[] { System.Drawing.SystemColors.WindowText });
            //this.itb.defBuilder = ((System.Text.StringBuilder)(resources.GetObject("itb.defBuilder")));
            this.itb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itb.Location = new System.Drawing.Point(0, 0);
            this.itb.Name = "itb";
            itbType.InvokeMember("Prompt",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                    Type.DefaultBinder,
                    itb,
                    new System.Object[] { ">>>" });
            this.itb.TabIndex = 0;
            // 
            // TerminalWindow
            // 
            this.ClientSize = new System.Drawing.Size(572, 333);
            this.Controls.Add(this.itb);
            this.Name = "TerminalWindow";
            this.Text = "TerminallyIll : WW Python Console";
            this.Load += new System.EventHandler(this.TerminalWindow_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.TerminalWindow_Closing);
            this.ResumeLayout(false);

        }

        private void TerminalWindow_Load(object sender, System.EventArgs e)
        {
            EnterText("import sys");
            EnterText("sys.path.append('" + m_PluginDirectory.Replace("\\", "\\\\") + "')");
            EnterText("import env");
            EnterText("cls");
            EnterText("help");
        }

        private void EnterText(string s)
        {
            itbType.InvokeMember("WriteText",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    Type.DefaultBinder,
                    itb,
                    new System.Object[] { s + "\r\n" });
            itbType.InvokeMember("SimEnter",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    Type.DefaultBinder,
                    itb,
                    new System.Object[] { });
        }

        private void TerminalWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            this.ti.SetMenuItemChecked(false);
        }
    }

    public class TerminallyIll : WorldWind.PluginEngine.Plugin
    {
        TerminalWindow tw;
        MenuItem mi;

        public override void Load()
        {
            tw = new TerminalWindow(this, this.m_PluginDirectory);

            mi = new MenuItem("Co&nsole", new System.EventHandler(this.mi_Click));
            mi.RadioCheck = true;
            ParentApplication.ViewMenu.MenuItems.Add(3, mi);
        }

        private void mi_Click(Object sender, System.EventArgs e)
        {
            mi.Checked = ! mi.Checked;
            tw.Visible = mi.Checked;
        }

        public void SetMenuItemChecked(bool chk)
        {
            mi.Checked = chk;
        }

        public override void Unload()
        {
            ParentApplication.ViewMenu.MenuItems.Remove(mi);
            tw.Close();
        }
    }
}
