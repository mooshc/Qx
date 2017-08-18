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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Qx.Common;
using Frameworks;
using System.Diagnostics;
using System.IO;

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var processlist = Process.GetProcesses();
            if (processlist.Where(p => p.ProcessName == "Qx.Admin").Count() > 1)
            {
                Close();
                return;
            }
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CallContextLight.Current = new CallContextLight(Guid.Empty);
                //var User = RemoteObjectProvider.GetUserAccess().IsLoginCorrect(usernameTextBox.Text, passwordTextBox.Password);
                var User = RemoteObjectProvider.GetUserAccess().IsLoginCorrect("admin", "thirnzho");
                if (User == null)
                    return;

                RemoteObjectProvider.UserGuid = User.Guid;
                Hide();
                new UsersManager().ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                var stream = new StreamWriter("Log.txt", true);
                stream.WriteLine(ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message) + "\n\n");
                stream.Close();
                Close();
            }
        }

        private void usernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(null, null);
        }
    }
}
