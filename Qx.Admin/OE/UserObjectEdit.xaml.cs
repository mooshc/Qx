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
    /// Interaction logic for UserObjectEdit.xaml
    /// </summary>
    public partial class UserObjectEdit : Window
    {
        public ObservableCollection<ModuleInUser> OCModules 
        {
            get { return new ObservableCollection<ModuleInUser>((DataContext as User).Modules); }
        }

        public UserObjectEdit()
        {
            DataContext = new User();
            Initialize();
        }

        public UserObjectEdit(User User)
        {
            DataContext = User;
            Initialize();
        }

        void Initialize()
        {
            InitializeComponent();
            CompanyComboBox.ItemsSource = RemoteObjectProvider.GetCompanyAccess().LoadAll().Where(c => !c.IsDeleted);
            LanguageComboBox.ItemsSource = RemoteObjectProvider.GetLanguageAccess().LoadAll().Where(l => !l.IsDeleted);
            ModulesCombobox.ItemsSource = RemoteObjectProvider.GetModuleAccess().GetAnamnesisModules().Where(m => !(DataContext as User).Modules.Select(mo => mo.Module).Select(mo => mo.Name).Contains(m)).ToList();
            ModulesListBox.ItemsSource = OCModules;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var user = DataContext as User;
            RemoteObjectProvider.GetUserAccess().SaveOrUpdate(user);
            Close();
        }

        private void AddModuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModulesCombobox.SelectedItem != null)
            {
                var module = RemoteObjectProvider.GetModuleAccess().LoadModuleByName(ModulesCombobox.SelectedItem.ToString());
                (DataContext as User).Modules.Add(new ModuleInUser(module, false));
                (ModulesCombobox.ItemsSource as List<string>).Remove(ModulesCombobox.SelectedItem.ToString());
            }
        }

        private void RemoveModuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModulesListBox.SelectedItem as ModuleInUser != null)
            {
                (ModulesCombobox.ItemsSource as List<string>).Add((ModulesListBox.SelectedItem as ModuleInUser).Module.Name);
                (DataContext as User).Modules.Remove(ModulesListBox.SelectedItem as ModuleInUser);
            }
        }

        private void PermitModuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModulesListBox.SelectedItem as ModuleInUser != null)
            {
                (ModulesListBox.SelectedItem as ModuleInUser).IsAuthorized = !(ModulesListBox.SelectedItem as ModuleInUser).IsAuthorized;
                ModulesListBox.ItemsSource = OCModules;
            }
        }


    }
}
