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

namespace Qx.Learning
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
            Thread.Sleep(3000);
            try
            {
                new MainWindow().Show();
                WindowState = System.Windows.WindowState.Minimized;

            }
            catch (Exception ex)
            {
                var stream = new StreamWriter("Log.txt", true);
                stream.WriteLine(ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message) + "\n\n");
                stream.Close();
                Close();
            }
        }
    }
}
