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
using System.IO;
using System.Diagnostics;

namespace Qx.Learning
{ 
    /// <summary>
    /// Interaction logic for TrainingWindow.xaml
    /// </summary>
    public partial class TrainingWindow : Window
    {
        Module Module;
        IList<Scenario> Scenarios;
        List<int> IDs = new List<int>();
        ModuleSelectWindow Win;

        public TrainingWindow(Module module, ModuleSelectWindow win)
        {
            InitializeComponent();
            Module = module;
            HeaderLabel.Content = "תרגולים ומבחנים";

            var scenarios = Scenarios = RemoteObjectProvider.GetScenarioAccess().GetScenariosByModuleName(module.Name);
            Label b;
            #region EXERCISES
            b = new Label();
            b.Content = "תרגולים - " + module.ModuleHebText;
            b.FontWeight = FontWeights.Bold;
            ScenarioArea.Children.Add(b);
            foreach (var scenario in scenarios.Where(s => !s.IsTest))
            {
                b = new Label();
                b.Name = "Label" + scenario.ID.ToString();
                b.Content = scenario.Name;
                b.Margin = new Thickness(10);
                b.MouseDown += Button_Clicked;
                b.Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
                ScenarioArea.Children.Add(b);
            }
            #endregion
            #region TESTS
            b = new Label();
            b.Content = "מבחנים - " + module.ModuleHebText;
            b.FontWeight = FontWeights.Bold;
            ScenarioArea.Children.Add(b);
            foreach (var scenario in scenarios.Where(s => s.IsTest))
            {
                b = new Label();
                b.Name = "Label" + scenario.ID.ToString();
                b.Content = scenario.Name;
                b.Margin = new Thickness(10);
                b.MouseDown += Button_Clicked;
                b.Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
                if (Session.User.Scenarios.ToList().Exists(s => s.ScenarioID == scenario.ID && s.Mistakes == 0))
                    b.IsEnabled = false;
                ScenarioArea.Children.Add(b);
            }
            #endregion
            Win = win;
        }
            
        private void Button_Clicked(object sender, EventArgs e)
        {
            var moduleID = Int32.Parse((sender as Label).Name.Substring("Label".Length));
            if (IDs.Contains(moduleID))
            {
                IDs.Remove(moduleID);
                (sender as Label).Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
                (sender as Label).FontWeight = FontWeights.Normal;
            }
            else
            {
                IDs.Add(moduleID);
                (sender as Label).Background = (Brush)new BrushConverter().ConvertFrom("#e6f1f2");
                (sender as Label).FontWeight = FontWeights.Bold;
            }
            OKButton.IsEnabled = IDs.Count > 0;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var proc = Process.GetProcessesByName("Qx.Learning");
            foreach (var p in proc)
                p.Kill();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
            }
        }

        private void BackButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Win.WindowState = System.Windows.WindowState.Normal;
            this.WindowState = WindowState.Minimized;
        }

        private void OKButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(IDs.Count > 1)
            {
                MessageBox.Show("נא לבחור אחד בלבד");
                return;
            }
            var sn = Scenarios.Where(s => s.ID == IDs[0]).FirstOrDefault();
            new MedicalFileWindow(sn, this).Show();
            this.WindowState = WindowState.Minimized;
        }
    }
}
