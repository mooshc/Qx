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
using Qx.Common;

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for ScenarioObjectEdit.xaml
    /// </summary>
    public partial class ScenarioObjectEdit : Window
    {
        public ScenarioObjectEdit()
        {
            DataContext = new Scenario();
            InitializeComponent();
            ModuleComboBox.ItemsSource = RemoteObjectProvider.GetModuleAccess().GetAnamnesisModules();
        }

        public ScenarioObjectEdit(Scenario scenario)
        {
            DataContext = scenario;
            InitializeComponent();
            ModuleComboBox.ItemsSource = RemoteObjectProvider.GetModuleAccess().GetAnamnesisModules();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            RemoteObjectProvider.GetScenarioAccess().SaveOrUpdate(DataContext as Scenario);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AnswersButton_Click(object sender, RoutedEventArgs e)
        {
            new ScenarioAnswersOM(DataContext as Scenario, (sender as Button).Name == "AnamnesisAnswersButton").ShowDialog();
        }
    }
}
