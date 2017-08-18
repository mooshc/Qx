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
using System.Diagnostics;

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for ModuleSelectWindow.xaml
    /// </summary>
    public partial class ModuleSelectWindow : Window
    {
        private char ModulesType;
        private List<int> selected = new List<int>();

        public ModuleSelectWindow(char modulesType, string TrainingModuleName = "")
        {
            InitializeComponent();
            ModulesType = modulesType;
            switch (modulesType)
            {
                case 'Q': HeaderLabelEnmnesia.Content = "בחירת אנמנזה";
                          EnmnesiaHeader.Visibility = System.Windows.Visibility.Visible;
                          SetModulesInGrid(Session.User.Modules.Where(m => m.IsAuthorized || m.Module.Name.Equals(TrainingModuleName)).Select(m => m.Module).ToList());
                          break;

                case 'E': HeaderLabelPhysicalEx.Content = "בחירת בדיקה גופנית";
                          PhysicalExHeader.Visibility = System.Windows.Visibility.Visible;
                          var physicalEx = new List<Module>();
                          foreach (var mod in Session.User.Modules.Where(m => m.IsAuthorized || m.Module.Name.Equals(TrainingModuleName)).Select(m => m.Module))
                              if(mod.PhysicalExaminations != null)
                                physicalEx.AddRange(mod.PhysicalExaminations.Select(p => p.PhysicalExaminationModule));

                          SetModulesInGrid(physicalEx);
                          break;

                case 'L': HeaderLabelEnmnesia.Content = "בחירת אנמנזה לתרגול";
                          EnmnesiaHeader.Visibility = System.Windows.Visibility.Visible;
                          SetModulesInGrid(Session.User.Modules.Where(m => !m.IsAuthorized).Select(m => m.Module).ToList());
                          MainGrid.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Grphics/LearningMainWindow.png")));
                          Height = 770;
                          Width = 1060;
                          MainGrid.Height = 701;
                          MainGrid.Width = 1001;
                          HeaderRow.Height = new GridLength(64);
                          MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(56) });
                          SideBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                          SideBorder.Width = 245;
                          SideBorder.Height = 537;
                          SideBorder.Margin = new Thickness(10, 8, 10, 10);
                          SideBorder.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#cfcfcf");
                          SideBorder.BorderThickness = new Thickness(2);
                          HeadersGrid.Margin = new Thickness(-10, -2, 0, 0);
                          OKButton.Margin = new Thickness(0, 0, 25, 30);
                          ModulesArea.Height = 440;
                          BlankPic.Visibility = System.Windows.Visibility.Visible;
                          WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                          break;

                default: break;
            }
        }

        private void SetModulesInGrid(List<Module> modules)
        {

            Label b;
            foreach (var module in modules)
            {
                b = new Label();
                b.Name = "Label" + module.ID.ToString();
                b.Content = ContentDictionary.GetContent(module.Name, Session.Lang);
                b.Margin = new Thickness(10);
                b.MouseDown += Button_Clicked;
                if (module.ModuleType.ID == 2 && Session.LastModule != null && 
                    (Session.LastModule.PhysicalExaminations.Select(peim => peim.PhysicalExaminationModule).Contains(module) || Session.RecomendedPhysicalEx.Contains(module)))
                {
                    b.Background = EnmnesiaHeader.Visibility == System.Windows.Visibility.Visible ? (Brush)new BrushConverter().ConvertFrom("#e6f1f2") : (Brush)new BrushConverter().ConvertFrom("#dcdae6");
                    selected.Add(module.ID);
                    b.FontWeight = FontWeights.Bold;
                }
                else
                    b.Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
                ModulesArea.Children.Add(b);
            }
            OKButton.IsEnabled = selected.Count > 0;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var moduleID = Int32.Parse((sender as Label).Name.Substring("Label".Length));
            if (selected.Contains(moduleID))
            {
                selected.Remove(moduleID);
                (sender as Label).Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
                (sender as Label).FontWeight = FontWeights.Normal;
            }
            else
            {
                selected.Add(moduleID);
                (sender as Label).Background = EnmnesiaHeader.Visibility == System.Windows.Visibility.Visible ? (Brush)new BrushConverter().ConvertFrom("#e6f1f2") : (Brush)new BrushConverter().ConvertFrom("#dcdae6");
                (sender as Label).FontWeight = FontWeights.Bold;
            }
            OKButton.IsEnabled = selected.Count > 0;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var ModulesByLastEnmnesia = Session.LastModule == null ? new List<PhysicalExaminationInAnamnesis>() : Session.LastModule.PhysicalExaminations.Where(pe => selected.Contains(pe.PhysicalExaminationModule.ID));
            var modules = Session.User.Modules.Select(m => m.Module).Where(m => selected.Contains(m.ID) && !ModulesByLastEnmnesia.Select(mia => mia.PhysicalExaminationModule).Contains(m)).ToList();
            switch (ModulesType)
            {
                case 'Q': Session.LastModule = modules[0];
                          WindowState = System.Windows.WindowState.Minimized;
                          DialogResult = new ModuleWindow(modules[0]).ShowDialog();
                          Close();
                        break;

                case 'E': Session.RecomendedPhysicalEx.Clear();
                          WindowState = System.Windows.WindowState.Minimized;
                          var mods = ModulesByLastEnmnesia.OrderBy(mi => mi.Ordering).Select(mm => mm.PhysicalExaminationModule).ToList();
                          mods.AddRange(modules);
                          if (mods.Count == 1)
                              DialogResult = new ModuleWindow(mods[0]).ShowDialog();
                          else
                              DialogResult = new ModuleWindow(mods).ShowDialog();
                          Close();
                        break;

                case 'L': if (selected.Count > 1)
                          {
                              MessageBox.Show("נא לבחור אחד בלבד");
                              return;
                          }
                          new TrainingWindow(modules[0], this).Show();
                          WindowState = System.Windows.WindowState.Minimized;
                        break;

                default: break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            switch (ModulesType)
            {
                case 'L':
                    var proc = Process.GetProcessesByName("Qx.Learning");
                    foreach (var p in proc)
                        p.Kill();
                    break;

                default: Close(); break;
            }
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
    }
}
