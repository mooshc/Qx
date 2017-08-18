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
using System.Windows.Threading;
using System.Threading;

namespace Qx.Controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int i;
        DateTime startTime;
        delegate void ShowMessage();
        delegate void UpdateTime();

        public MainWindow()
        {
            InitializeComponent();

            ReportsCombobox.Items.Add("Total Usage");
            ReportsCombobox.Items.Add("Usage By Module Type");
            ReportsCombobox.Items.Add("Usage By User And Module");

            MonitoredObjectsComboBox.Items.Add("chest pain/center/pressure");
            MonitoredObjectsComboBox.Items.Add("headech/sudden");

            AlertsComboBox.Items.Add("chest pain/center/pressure");
            AlertsComboBox.Items.Add("headech/sudden");

            UsersComboBox.Items.Add("Dr. Paley");
            UsersComboBox.Items.Add("Dr. Boiman");
            UsersComboBox.Items.Add("Dr. Cohen");

            AlertsUserComboBox.Items.Add("Dr. Paley");
            AlertsUserComboBox.Items.Add("Dr. Boiman");
            AlertsUserComboBox.Items.Add("Dr. Cohen");

            
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if(AlertsComboBox.SelectedItem != null)
                AlertsListBox.Items.Add(AlertsComboBox.SelectedItem);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if(MonitoredObjectsComboBox.SelectedItem != null)
                MonitoredListBox.Items.Add(MonitoredObjectsComboBox.SelectedItem);
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (AlertsUserComboBox.SelectedItem != null && AlertsListBox.Items.Count > 0)
            {
                var alerts = "";
                foreach (var item in AlertsListBox.Items)
                    alerts += item.ToString() + ",";
                alerts = alerts.Substring(0, alerts.Length - 1);
                MonitoredAlertsListBox.Items.Add("Monitoring " + AlertsUserComboBox.SelectedItem + ", For Alerts: " + alerts + ".(00:00:00)");
                startTime = DateTime.Now;
                var thread = new Thread(DoWork);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        void DoWork()
        {
            for (i = 0; i < 40; i++)
            {
                
                var time = new ShowMessage(UpdateTimeFunc);
                Dispatcher.Invoke(time, DispatcherPriority.Normal);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
            var hide = new ShowMessage(ShowMessageFunc);
            Dispatcher.Invoke(hide, DispatcherPriority.Normal);
        }

        void UpdateTimeFunc()
        {
            var str = (MonitoredAlertsListBox.Items[0] as string);
            var timeString = i.ToString();
            MonitoredAlertsListBox.Items[0] = str.Substring(0, str.Length - timeString.Length - 1) + i + ")";
        }

        void ShowMessageFunc()
        {

            MessageBox.Show("                                      -Attention-\nUser: Dr. Boiman marked chest pain/center/pressure.\n" + 
                DateTime.Now.ToString(),"QxController Notification Service");
        }
    }
}
