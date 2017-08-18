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
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Threading;
using System.IO;

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User User;
        delegate void MoveToInVisible();
        delegate void ReturnToNormal();
        private string userName;
        private string password;

        public MainWindow()
        {
            var processlist = Process.GetProcesses();
            if (processlist.Where(p => p.ProcessName == "Qx.Learning").Count() > 1)
            {
                Close();
                return;
            }
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var proc = Process.GetProcessesByName("Qx.Learning");
            foreach (var p in proc)
                p.Kill();
        }

        private void MoveToInVisibleFunc()
        {
            new ModuleSelectWindow('L').Show();
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void ReturnToNormalFunc()
        {
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            usernameTextBox.IsEnabled = passwordTextBox.IsEnabled = true;
            WrongUserNameOrPass.Visibility = System.Windows.Visibility.Visible;
        }

        private void OKButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WrongUserNameOrPass.Visibility = System.Windows.Visibility.Hidden;
            LoadingLabel.Visibility = System.Windows.Visibility.Visible;
            usernameTextBox.IsEnabled = passwordTextBox.IsEnabled = false;

            userName = usernameTextBox.Text;
            password = passwordTextBox.Password;

            var thread = new Thread(DoWork);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();            
        }

        void DoWork()
        {
            try
            {
                CallContextLight.Current = new CallContextLight(Guid.Empty);
                //User = RemoteObjectProvider.GetUserAccess().IsLoginCorrect(userName, password);
                User = RemoteObjectProvider.GetUserAccess().IsLoginCorrect("Learning", "123");
                if (User == null)
                {
                    var returnToNormal = new ReturnToNormal(ReturnToNormalFunc);
                    Dispatcher.Invoke(returnToNormal, DispatcherPriority.Normal);
                    return;
                }
                RemoteObjectProvider.UserGuid = User.Guid;
                Session.User = User;
                Session.Lang = User.Language;
                ContentDictionary.PopulateDictionary(User.Language);

                var hide = new MoveToInVisible(MoveToInVisibleFunc);
                Dispatcher.Invoke(hide, DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\n\n" + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }
    }
}
