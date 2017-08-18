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
using System.Collections.ObjectModel;

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for ModuleObjectEdit.xaml
    /// </summary>
    public partial class ModuleObjectEdit : Window
    {
        public ObservableCollection<PhysicalExaminationInAnamnesis> OCPhysicalEx
        {
            get { return new ObservableCollection<PhysicalExaminationInAnamnesis>((DataContext as Module).PhysicalExaminations.OrderBy(ph => ph.Ordering)); }
        }
        public ObservableCollection<string> CBPhysicalEx;

        public ModuleObjectEdit()
        {
            DataContext = new Module();
            Initialize();
        }

        public ModuleObjectEdit(Module Module)
        {
            DataContext = Module;
            Initialize();
            if (Module.IsMale == null)
                Bisexual.IsChecked = true;
            else if ((bool)Module.IsMale)
                Male.IsChecked = true;
            else
                Female.IsChecked = true;
        }

        private void Initialize()
        {
            InitializeComponent();
            ModuleTypeComboBox.ItemsSource = RemoteObjectProvider.GetModuleTypeAccess().LoadAll().Where(mt => !mt.IsDeleted);
            PhysicalExComboBox.ItemsSource = CBPhysicalEx = new ObservableCollection<string>(RemoteObjectProvider.GetModuleAccess().GetModulesNames().
                Where(m => m.Contains("E_") && !(DataContext as Module).PhysicalExaminations.Select(ph => ph.PhysicalExaminationModule.Name).Contains(m)).ToList());
            PhysicalExListBox.ItemsSource = OCPhysicalEx;
            ModuleTypeComboBox.SelectionChanged += new SelectionChangedEventHandler(ModuleTypeComboBox_SelectionChanged);
            ModuleTypeComboBox_SelectionChanged(this, null);
        }

        void ModuleTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModuleTypeComboBox.SelectedItem == null) return;
            if ((ModuleTypeComboBox.SelectedItem as ModuleType).ID == 2)
                PhysicalExGroupBox.Visibility = System.Windows.Visibility.Collapsed;
            else
                PhysicalExGroupBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void AddPysicalExButton_Click(object sender, RoutedEventArgs e)
        {
            if (PhysicalExComboBox.SelectedItem != null)
            {
                var module = RemoteObjectProvider.GetModuleAccess().LoadModuleByName(PhysicalExComboBox.SelectedItem.ToString());
                CBPhysicalEx.Remove(module.Name);
                (DataContext as Module).PhysicalExaminations.Add(new PhysicalExaminationInAnamnesis(module, 
                    (DataContext as Module).PhysicalExaminations.Count == 0 ? 1 :(DataContext as Module).PhysicalExaminations.Max(ph => ph.Ordering)+1));
            }
        }

        private void RemovePysicalExButton_Click(object sender, RoutedEventArgs e)
        {
            if (PhysicalExListBox.SelectedItem as PhysicalExaminationInAnamnesis != null)
            {
                var p = PhysicalExListBox.SelectedItem as PhysicalExaminationInAnamnesis;
                (DataContext as Module).PhysicalExaminations.Remove(p);
                CBPhysicalEx.Add(p.PhysicalExaminationModule.Name);               
                var index = 1;
                foreach (var phe in (DataContext as Module).PhysicalExaminations.OrderBy(ph => ph.Ordering))
                {
                    phe.Ordering = index;
                    index++;
                }
            }
        }

        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            new QuestionSelector((DataContext as Module).Questions).ShowDialog();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var module = DataContext as Module;
            module.ModuleHebText = HebTextBox.Text;
            if (Bisexual.IsChecked ?? false)
                module.IsMale = null;
            else if (Male.IsChecked ?? false)
                module.IsMale = true;
            else
                module.IsMale = false;
            RemoteObjectProvider.GetModuleAccess().SaveOrUpdate(module);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MoveUpPhysicalEx_Click(object sender, RoutedEventArgs e)
        {
            var first = PhysicalExListBox.SelectedItem as PhysicalExaminationInAnamnesis;
            if (first.Ordering == 1) return;
            var second = (DataContext as Module).PhysicalExaminations.Where(phe => phe.Ordering == first.Ordering - 1).ToList().FirstOrDefault();
            first.Ordering--;
            second.Ordering++;
            PhysicalExListBox.ItemsSource = OCPhysicalEx;
        }

        private void MoveDownPhysicalEx_Click(object sender, RoutedEventArgs e)
        {
            var first = PhysicalExListBox.SelectedItem as PhysicalExaminationInAnamnesis;
            if (first.Ordering == (DataContext as Module).PhysicalExaminations.Count) return;
            var second = (DataContext as Module).PhysicalExaminations.Where(phe => phe.Ordering == first.Ordering + 1).ToList().FirstOrDefault();
            first.Ordering++;
            second.Ordering--;
            PhysicalExListBox.ItemsSource = OCPhysicalEx;
        }

        private void CombinationButton_Click(object sender, RoutedEventArgs e)
        {
            new CombinationsOM(DataContext as Module).ShowDialog();
        }
    }
}
