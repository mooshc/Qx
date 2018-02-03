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

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for ModuleSelectWindow.xaml
    /// </summary>
    public partial class ModuleSelectWindow : Window
    {
        private char ModulesType;
        private List<int> selected = new List<int>();
        List<Module> physicalEx = new List<Module>();
        private BitmapImage next = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "Next.png"));
        private BitmapImage nextHover = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "NextHover.png"));
        private int currentSelectedIndex = -1;
        private List<List<Module>> pages;
        private int currentPage;
        private string caseId;

        public ModuleSelectWindow(char modulesType, string caseId)
        {
            InitializeComponent();
            Left = Session.windowPosition.X;
            Top = Session.windowPosition.Y;
            ModulesType = modulesType;
            switch (modulesType)
            {
                case 'Q': HeaderLabelEnmnesia.Content = "Selecione a queixa principal";
                          EnmnesiaHeader.Visibility = System.Windows.Visibility.Visible;
                          SetModulesInPages(Session.User.Modules/*.Where(m => m.IsAuthorized).Select(m => m.Module)*/.Where(m => m.ModuleType.ID == 1 && m.ID != 1).ToList());
                          break;

                case 'E': HeaderLabelPhysicalEx.Content = "Selecione o exame físico";
                          PhysicalExHeader.Visibility = System.Windows.Visibility.Visible;
                          //var physicalEx = new List<Module>();
                          foreach (var mod in Session.User.Modules/*.Where(m => m.IsAuthorized).Select(m => m.Module)*/)
                              if (mod.PhysicalExaminations != null)
                                  foreach (Module PhEx in mod.PhysicalExaminations.Select(p => p.PhysicalExaminationModule))
                                      if(!physicalEx.Select(m => m.ID).Contains(PhEx.ID))
                                        physicalEx.Add(PhEx);
                          /*var tmp = Session.User.Modules.Select(m => m.Questions.Select(qim => qim.Question).Select(q => q.Answers));

                          foreach (var item in tmp.Select(t => t.Select(t1 => t1.Where(t3 => t3.RecomendedPhysicalEx != null).Select(t2 => t2.RecomendedPhysicalEx))))
                          {
                              foreach (var item2 in item)
                              {
                                  foreach (var item3 in item2)
                                  {
                                      if (!physicalEx.Contains(item3))
                                          physicalEx.Add(item3);
                                  }
                              }
                          }*/
                          SetModulesInPages(physicalEx);
                          break;

                default: break;
            }
            Loaded += new RoutedEventHandler(ModuleSelectWindow_Loaded);
            ModulesArea.KeyDown += new KeyEventHandler(ModulesArea_KeyDown);

            this.caseId = caseId;
        }

        void ModulesArea_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter: OKButton_Click(null, null);
                    return;

                case Key.RightCtrl:
                case Key.Space: Button_Clicked(ModulesArea.Children.OfType<Label>().ToList()[currentSelectedIndex], null);
                    break;

                case Key.Down: if(currentSelectedIndex < ModulesArea.Children.Count-1) currentSelectedIndex++;
                    break;

                case Key.Up: if (currentSelectedIndex > 0) currentSelectedIndex--;
                    break;

                case Key.Right: if (currentPage > 0) PrevLabel_MouseDown(null, null);
                    break;

                case Key.Left: if (currentPage+1 < pages.Count) NextLabel_MouseDown(null, null);
                    break;

                default: break;
            }
            if(currentSelectedIndex >=0 && currentSelectedIndex < ModulesArea.Children.Count)
                ModulesArea.Children.OfType<Label>().ToList()[currentSelectedIndex].Focus();
        }

        void ModuleSelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(ModulesArea.Children.Count > 0)
                ModulesArea.Children[0].Focus();
        }

        private void SetModulesInGrid()
        {
            this.ModulesArea.Children.Clear();
            foreach (Module module in this.pages[this.currentPage])
            {
                Label b = new Label()
                {
                    Focusable = true
                };
                int d = module.ID;
                b.Name = string.Concat("Label", d.ToString());
                b.Content = module.ModuleHebText;
                b.BorderThickness = new Thickness(1);
                b.BorderBrush = new SolidColorBrush(Colors.Transparent);
                b.Margin = new Thickness(10, 0, 10, 0);
                b.Padding = new Thickness(10, 5, 10, 5);
                b.MouseDown += new MouseButtonEventHandler(this.Button_Clicked);
                b.MouseDoubleClick += new MouseButtonEventHandler(this.b_MouseDoubleClick);
                if (!this.selected.Contains(module.ID))
                {
                    if (module.ModuleType.ID == 2 && Session.LastModule != null)
                    {
                        if ((
                            from peim in Session.LastModule.PhysicalExaminations
                            select peim.PhysicalExaminationModule).Contains<Module>(module) || Session.RecomendedPhysicalEx.Contains(module))
                        {
                            b.Background = (this.EnmnesiaHeader.Visibility == Visibility.Visible ? (Brush)(new BrushConverter()).ConvertFrom("#e6f1f2") : (Brush)(new BrushConverter()).ConvertFrom("#dcdae6"));
                            this.selected.Add(module.ID);
                            b.FontWeight = FontWeights.Bold;
                            b.BorderBrush = new SolidColorBrush(Colors.Black);
                        }
                    }
                    b.Background = (Brush)(new BrushConverter()).ConvertFrom("#ebebeb");
                    this.ModulesArea.Children.Add(b);
                }
            }
            this.SetNextPrevButtons();
            if (this.ModulesArea.Children.Count > 0)
            {
                this.ModulesArea.Children[0].Focus();
                currentSelectedIndex = 0;
            }
        }

        private void b_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ModulesType == 'E')
            {
                return;
            }
            int moduleID = int.Parse((sender as Label).Name.Substring("Label".Length));
            this.selected.Clear();
            this.selected.Add(moduleID);
            this.OKButton_Click(sender, null);
        }

        private void SetModulesInPages(List<Module> modules)
        {
            this.pages = new List<List<Module>>();
            modules = (
                from m in modules
                orderby m.SeverityLevel
                select m).ToList<Module>();
            while (modules.Count > 0)
            {
                this.pages.Add(modules.Take<Module>(17).ToList<Module>());
                modules.RemoveRange(0, Math.Min(modules.Count, 17));
            }
            this.SetModulesInGrid();
        }

        private void SetNextPrevButtons()
        {
            if (this.currentPage + 1 != this.pages.Count)
            {
                this.NextLabel.Background = (Brush)(new BrushConverter()).ConvertFrom("#b3d9de");
                this.NextLabel.IsEnabled = true;
            }
            else
            {
                this.NextLabel.Background = Brushes.LightGray;
                this.NextLabel.IsEnabled = false;
            }
            if (this.currentPage == 0)
            {
                this.PrevLabel.Background = Brushes.LightGray;
                this.PrevLabel.IsEnabled = false;
                return;
            }
            this.PrevLabel.Background = (Brush)(new BrushConverter()).ConvertFrom("#b3d9de");
            this.PrevLabel.IsEnabled = true;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var moduleID = Int32.Parse((sender as Label).Name.Substring("Label".Length));
            if (selected.Contains(moduleID))
            {
                selected.Remove(moduleID);
                (sender as Label).Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
                (sender as Label).FontWeight = FontWeights.Normal;
                (sender as Label).BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                if (ModulesType == 'Q')
                {
                    foreach (var l in ModulesArea.Children.OfType<Label>())
                    {
                        l.Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
                        l.FontWeight = FontWeights.Normal;
                        l.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    }
                    selected.Clear();
                }
                selected.Add(moduleID);
                (sender as Label).Background = EnmnesiaHeader.Visibility == System.Windows.Visibility.Visible ? (Brush)new BrushConverter().ConvertFrom("#e6f1f2") : (Brush)new BrushConverter().ConvertFrom("#dcdae6");
                (sender as Label).FontWeight = FontWeights.Bold;
                (sender as Label).BorderBrush = new SolidColorBrush(Colors.Black);
            }
            OKButton.IsEnabled = (selected.Count > 0 && ModulesType == 'E') || (selected.Count == 1 && ModulesType == 'Q') ;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (selected.Count == 0)
                return;
            var ModulesByLastEnmnesia = Session.LastModule == null ? new List<PhysicalExaminationInAnamnesis>() : Session.LastModule.PhysicalExaminations.Where(pe => selected.Contains(pe.PhysicalExaminationModule.ID));
            var modules1 = Session.User.Modules/*.Where(m => m.IsAuthorized)*/;
            var modules = new List<Module>();
            switch (ModulesType)
            {
                case 'Q': modules = modules1/*.Select(m => m.Module)*/.Where(m => selected.Contains(m.ID) && !ModulesByLastEnmnesia.Select(mia => mia.PhysicalExaminationModule).Contains(m)).ToList();
                          Session.LastModule = modules[0];
                          Hide();
                          Session.RecomendedPhysicalEx.Clear();
                          if (!new ModuleWindow(modules, caseId).ShowDialog().Value)
                            Close();
                          else
                            Show();
                          break;

                case 'E': modules = physicalEx.Where(m => selected.Contains(m.ID) && !ModulesByLastEnmnesia.Select(mia => mia.PhysicalExaminationModule).Contains(m)).ToList();
                          Session.RecomendedPhysicalEx.Clear();
                          
                          var mods = ModulesByLastEnmnesia.OrderBy(mi => mi.Ordering).Select(mm => mm.PhysicalExaminationModule).ToList();
                          mods.AddRange(modules);
                          //if (mods.Count == 1)
                          //    new ModuleWindow(mods[0]).ShowDialog();
                          //else
                          Hide();
                          if(!new ModuleWindow(mods, caseId).ShowDialog().Value)
                            Close();
                          else
                            Show();
                          break;

                default: break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
                Session.windowPosition.X = Left;
                Session.windowPosition.Y = Top;
            }
            catch
            {
            }
        }

        private void OKButton_MouseEnter(object sender, MouseEventArgs e)
        {
            OKButton.Source = nextHover;
        }

        private void OKButton_MouseLeave(object sender, MouseEventArgs e)
        {
            OKButton.Source = next;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
                this.Close();
        }

        private void PrevLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.currentPage > 0)
            {
                ModuleSelectWindow moduleSelectWindow = this;
                moduleSelectWindow.currentPage = moduleSelectWindow.currentPage - 1;
            }
            this.currentSelectedIndex = 0;
            this.SetModulesInGrid();
        }

        private void NextLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.currentPage < this.pages.Count)
            {
                ModuleSelectWindow moduleSelectWindow = this;
                moduleSelectWindow.currentPage = moduleSelectWindow.currentPage + 1;
            }
            this.currentSelectedIndex = 0;
            this.SetModulesInGrid();
        }
    }
}
