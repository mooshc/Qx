using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        delegate void MoveToInVisible();

        public OpenWindow()
        {
            InitializeComponent();
            var processlist = Process.GetProcesses();
            if (processlist.Where(p => p.ProcessName == "Qx.Client" && GetUserName(p.ProcessName + ".exe").Equals(Environment.UserName)).Count() > 1)
            {
                var pi = new ProcessStartInfo("Reload.bat");
                pi.CreateNoWindow = true;
                pi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(pi);
                Close();
                return;
            }
            var thread = new Thread(DoWork);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            //thread.Join();
        }

        void DoWork()
        {
            var hide = new MoveToInVisible(MoveToInVisibleFunc);
            Dispatcher.Invoke(hide, DispatcherPriority.Normal);
        }

        void MoveToInVisibleFunc()
        {
            Thread.Sleep(1500);
            try
            {
                var x = new MainWindow();
                Hide();
                x.ShowDialog();
            }
            catch (Exception ex)
            {
                var stream = new StreamWriter("Log.txt",true);
                stream.WriteLine(ex.Message +"\n"+ (ex.InnerException == null ? "" : ex.InnerException.Message) +"\n\n");
                stream.Close();
                MessageBox.Show("DocQx Closed.");
                Close();
                var proc = Process.GetProcessesByName("Qx.Client");
                foreach (var p in proc)
                    p.Kill();
            }
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
    }
}
