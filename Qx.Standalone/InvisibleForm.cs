using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hotkeys;
using System.Windows.Forms.Integration;
using System.IO;
using System.Diagnostics;

namespace Qx.Client
{
    public partial class InvisibleForm : Form
    {
        private Hotkeys.GlobalHotkey ghkA;
        private Hotkeys.GlobalHotkey ghkPE;
        private bool isAnamnesisOpen = false;
        private bool isPhysicalExOpen = false;

        public InvisibleForm()
        {
            InitializeComponent();
            ghkA = new Hotkeys.GlobalHotkey(Constants.CTRL, Keys.Q, this);
            ghkPE = new Hotkeys.GlobalHotkey(Constants.CTRL, Keys.E, this);
            Hide();
        }

        private void Anamnesis(string caseId)
        {
            try
            {
                if (!isAnamnesisOpen && !isPhysicalExOpen)
                {
                    isAnamnesisOpen = true;
                    var wpfwindow = new ModuleSelectWindow('Q', caseId);
                    ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                    wpfwindow.ShowDialog();
                    isAnamnesisOpen = false;
                }
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("DocQx נתקל בבעיה, נא פנה לתמיכה" + "\n\n" + Environment.UserName + "--" + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message));
                var stream = new StreamWriter("Log.txt", true);
                stream.WriteLine(Environment.UserName + " -> " + Session.User.UserName + " -> " + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message) + "\n\n");
                stream.Close();
                Close();
                var processlist = Process.GetProcesses();
                //Process.Start(new ProcessStartInfo("Reload.bat") { CreateNoWindow = true }).Start();
                var proc = processlist.Where(p => p.ProcessName == "Qx.Client.exe" && GetUserName(p.ProcessName + ".exe").Equals(Environment.UserName)).FirstOrDefault();
                if (proc != null)
                    proc.Kill();
            }
        }

        private void PhysicalEx(string caseId)
        {
            try
            {
                if (!isAnamnesisOpen && !isPhysicalExOpen)
                {
                    isPhysicalExOpen = true;
                    //if (Session.LastModule == null)
                    new ModuleSelectWindow('E', caseId).ShowDialog();
                    //else
                    //    new LastModuleWindow().ShowDialog();
                    isPhysicalExOpen = false;
                }
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("DocQx נתקל בבעיה, נא פנה לתמיכה" + "\n\n" + Environment.UserName + "--" + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message));
                var stream = new StreamWriter("Log.txt", true);
                stream.WriteLine(Environment.UserName + " -> " + Session.User.UserName + " -> " + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message) + "\n\n");
                stream.Close();
                Close();
                var processlist = Process.GetProcesses();
                //Process.Start(new ProcessStartInfo("Reload.bat") { CreateNoWindow = true }).Start();
                var proc = processlist.Where(p => p.ProcessName == "Qx.Client.exe" && GetUserName(p.ProcessName + ".exe").Equals(Environment.UserName)).FirstOrDefault();
                if (proc != null)
                    proc.Kill();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Hotkeys.Constants.WM_HOTKEY_MSG_ID)
            {
                string caseId = Clipboard.GetText();
                if (m.LParam == new IntPtr(5308418))
                    Anamnesis(caseId);
                else
                    PhysicalEx(caseId);
            }
            base.WndProc(ref m);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ghkA.Unregiser() || !ghkPE.Unregiser())
                throw new Exception("Hotkey failed to unregister!");
        }

        private string GetUserName(string procName)
        {
            string query = "SELECT * FROM Win32_Process WHERE Name = \'" + procName + "\'";
            var procs = new System.Management.ManagementObjectSearcher(query);
            foreach (System.Management.ManagementObject p in procs.Get())
            {
                var path = p["ExecutablePath"];
                if (path != null)
                {
                    string executablePath = path.ToString();
                    string[] ownerInfo = new string[2];
                    p.InvokeMethod("GetOwner", (object[])ownerInfo);
                    return ownerInfo[0];
                }
            }
            return null;
        }

        private void InvisibleForm_Load(object sender, EventArgs e)
        {
            if (!ghkA.Register() || !ghkPE.Register())
                throw new Exception("Unable to register hotkey.");
        }
    }
}
