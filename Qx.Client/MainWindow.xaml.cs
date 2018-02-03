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
using System.Runtime.InteropServices;
using System.Configuration;
using Qx.EntitySerialization;
using Qx.Common.Objects;
using System.Net;

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LiteUser User;
        delegate void MoveToInVisible();
        delegate void ReturnToNormal();
        private string userName;
        private string password;
        private int loginState = 1;
        private bool shouldWorkLocally;

        private BitmapImage next = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "Next.png"));
        private BitmapImage nextHover = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "NextHover.png"));

        public MainWindow()
        {
            var processlist = Process.GetProcesses();
            if (processlist.Where(p => p.ProcessName == "Qx.Client" && GetUserName(p.ProcessName + ".exe").Equals(Environment.UserName)).Count() > 1)
            {
                System.Windows.MessageBox.Show("Client is open, press ctrl + 0");
                Close();
                return;
            }
            InitializeComponent();
            
            Session.windowPosition.X = Left;
            Session.windowPosition.Y = Top;
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);

            string fileUri = ConfigurationManager.AppSettings["FileUri"];
            string dbFileType = ConfigurationManager.AppSettings["DbFileType"];
            string dbFileFolder = ConfigurationManager.AppSettings["DbFileFolder"];
            
            if (!string.IsNullOrWhiteSpace(fileUri))
            {
                CopyFileLocallyFromUri(fileUri);

                dbFileType = "RemoteFile";
                dbFileFolder = ".";
            }

            shouldWorkLocally = ConfigurationManager.AppSettings["WorkLocally"].Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase);
            if(shouldWorkLocally)
            {
                CommonFunctions.HebLang = new Language() { Name = "עברית", IsDeleted = false };
                string fileName;
                var liteSession = EntitySerializer.DeserializeFromFile<LiteSession>(dbFileType, dbFileFolder, out fileName);
                Session.User = liteSession.User;
                Session.permanentQuestions = liteSession.PermanentQuestions;
                Session.fileName = fileName;
                MoveToInVisibleFunc();
            }
        }

        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OKButton_MouseDown(null, null);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            usernameTextBox.Focus();
            if (loginState == 2)
                approveCheckBox.Focus();
        }

        private void Listen()
        {
            while (true)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Q))
                {
                    var thread = new Thread(test);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                }

                if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.E))
                {
                    var thread = new Thread(test2);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                }

                /*if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.D0))
                {
                    System.Windows.MessageBox.Show("DocQx Closed.");
                    var proc = Process.GetProcessesByName("Qx.Client");
                    foreach (var p in proc)
                        p.Kill();
                }*/
            }
        }

        private void test2()
        {
            try
            {
                //if (Session.LastModule == null)
                new ModuleSelectWindow('E', string.Empty).ShowDialog();
                //else
                //    new LastModuleWindow().ShowDialog();
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

        private void test()
        {
            try
            {
                new ModuleSelectWindow('Q', string.Empty).ShowDialog();
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("DocQx נתקל בבעיה, נא פנה לתמיכה" + "\n\n" + Environment.UserName + "--" + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message));
                var stream = new StreamWriter("Log.txt", true);
                stream.WriteLine(Environment.UserName + " -> " + Session.User.UserName + " -> " + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message) + "\n\n");
                stream.Close();
                Close();
                var processlist = Process.GetProcesses();
                //Process.Start(new ProcessStartInfo("kill.bat") { CreateNoWindow = true }).Start();
                var proc = processlist.Where(p => p.ProcessName == "Qx.Client.exe" && GetUserName(p.ProcessName + ".exe").Equals(Environment.UserName)).FirstOrDefault();
                if (proc != null)
                    proc.Kill();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                Session.windowPosition.X = Left;
                Session.windowPosition.Y = Top;
            }
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            Environment.Exit(Environment.ExitCode);
        }

        private void MoveToInVisibleFunc()
        {
            HelloLabel.Content = "Conexão realizada com sucesso";
            LoginGrid.Visibility = System.Windows.Visibility.Hidden;
            AfterLoginPanel.Visibility = System.Windows.Visibility.Visible;
            WrongUserNameOrPass.Visibility = System.Windows.Visibility.Hidden;
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            OKButton.Margin = new Thickness(0, 0, 15, -16);
            approveLabel.Focus();
            loginState++;
            //Hide();
            /*var pi = new ProcessStartInfo("Qx.Onliner.exe");
            pi.CreateNoWindow = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(pi);*/
        }

        private void ReturnToNormalFunc()
        {
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            usernameTextBox.IsEnabled = passwordTextBox.IsEnabled = true;
            WrongUserNameOrPass.Visibility = System.Windows.Visibility.Visible;
        }

        [STAThread]
        private void OKButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (loginState == 3)
            {
                Hide();
                /*var listenThread = new Thread(Listen);
                listenThread.SetApartmentState(ApartmentState.STA);
                listenThread.Start();
                listenThread.Join();*/
                new InvisibleForm().ShowDialog();
                Close();

                Process.Start(new ProcessStartInfo("kill.bat") { CreateNoWindow = true }).Start();
            }
            if (loginState == 2)
            {
                if (approveCheckBox.IsChecked ?? false)
                {
                    AfterLoginPanel.Visibility = System.Windows.Visibility.Hidden;
                    FinalStackPanel.Visibility = System.Windows.Visibility.Visible;
                    loginState++;
                }
                else
                {
                    approveCheckBox.BorderBrush = new SolidColorBrush(Colors.Red);
                    approveCheckBox.BorderThickness = new Thickness(2);
                    approveLabel.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                }
                return;
            }
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
                TranslatedObject.UseOnlineContentDictionary = true;
                CallContextLight.Current = new CallContextLight(Guid.Empty);
                // User = RemoteObjectProvider.GetLiteUserAccess().IsLoginCorrect("moosh", "thirnzho");
                User = RemoteObjectProvider.GetLiteUserAccess().IsLoginCorrect(userName, password);
                if (User == null)
                {
                    var returnToNormal = new ReturnToNormal(ReturnToNormalFunc);
                    Dispatcher.Invoke(returnToNormal, DispatcherPriority.Normal);
                    return;
                }
                RemoteObjectProvider.UserGuid = User.Guid;
                Session.User = User;
                Session.Lang = User.Language;
                Session.permanentQuestions = RemoteObjectProvider.GetQuestionAccess().LoadPermanentQuestions();
                ContentDictionary.PopulateDictionary(User.Language);

                var hide = new MoveToInVisible(MoveToInVisibleFunc);
                Dispatcher.Invoke(hide, DispatcherPriority.Normal);

                
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("DocQx נתקל בבעיה, נא פנה לתמיכה" + "\n\n" + Environment.UserName + "--" + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message));
                var stream = new StreamWriter("Log.txt", true);
                stream.WriteLine(Environment.UserName + " -> " + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message) + "\n\n");
                stream.Close();
                //Close();
                var processlist = Process.GetProcesses();
                //Process.Start(new ProcessStartInfo("Reload.bat") { CreateNoWindow = false }).Start();
                var proc = processlist.Where(p => p.ProcessName == "Qx.Client.exe" && GetUserName(p.ProcessName + ".exe").Equals(Environment.UserName)).FirstOrDefault();
                if (proc != null)
                    proc.Kill();
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

        private void usernameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                OKButton_MouseDown(null, null);
        }

        private void OKButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            OKButton.Source = nextHover;
        }

        private void OKButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            OKButton.Source = next;
        }

        private void DeleteAllQxdbFiles(string except = "...")
        {
            try
            {
                foreach (var oldFile in Directory.GetFiles(".", "1_RemoteFile_RemoteFile_*.qxdb").Where(f => !f.Contains(except)))
                {
                    File.Delete(oldFile);
                }
            }
            catch
            {

            }
        }

        private void CopyFileLocallyFromUri(string fileUri)
        {
            string exception = "NoException";
            string localFileName = string.Format("1_RemoteFile_RemoteFile_{0}.qxdb", DateTime.Now.ToString(EntityFileMetaData.CREATION_DATE_FORMAT));

            try
            {
                WebClient webClient = new WebClient();
                webClient.UseDefaultCredentials = true;
                webClient.DownloadFile(fileUri, localFileName);

                DeleteAllQxdbFiles(localFileName);
            }
            catch (Exception e)
            {
                exception = e.ToString();
            }

            if (exception != "NoException")
            {
                List<string> files = Directory.GetFiles(".", "1_RemoteFile_RemoteFile_*.qxdb").Select(f => System.IO.Path.GetFileName(f)).ToList();
                if (files.Count > 0)
                {
                    DateTime dateOfNewestFile = files.Max(f => EntityFileMetaData.FromFileName(f).CreationTime);
                    string newestFile = files.First(f => f.Contains(dateOfNewestFile.ToString(EntityFileMetaData.CREATION_DATE_FORMAT)));

                    if (dateOfNewestFile.AddDays(3) < DateTime.Now)
                    {
                        string errorMessage = string.Format("No valid files to use!        Failed to download remote file from {0}. Error is {1}", fileUri, exception);
                        System.Windows.MessageBox.Show(errorMessage);
                        DeleteAllQxdbFiles();
                        throw new Exception(errorMessage);
                    }
                    else
                    {
                        DeleteAllQxdbFiles(newestFile);
                        System.Windows.MessageBox.Show(string.Format("Using file from {1}!      Failed to download remote file from {0}. Error is {2}", fileUri, dateOfNewestFile.ToShortDateString(), exception));
                    }
                }
            }
        }
    }
}
