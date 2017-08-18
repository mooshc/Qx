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
    /// Interaction logic for UsersManager.xaml
    /// </summary>
    public partial class UsersManager : Window
    {
        LiteUser selectedUser;
        
        public UsersManager()
        {
            InitializeComponent();
            UsersDataGrid.MouseDoubleClick += new MouseButtonEventHandler(UsersDataGrid_MouseDoubleClick);

            var allUsers = RemoteObjectProvider.GetLiteUserAccess().LoadAll();
            UsersDataGrid.ItemsSource = allUsers.Where(u => !u.IsDeleted);
        }

        void UsersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditButton_Click(sender, null);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            new SimpleUserEditor((UsersDataGrid.ItemsSource as IEnumerable<LiteUser>).ToList()).ShowDialog();
            RefreshButton_Click(sender, e);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null) return;
            new SimpleUserEditor(selectedUser, (UsersDataGrid.ItemsSource as IEnumerable<LiteUser>).ToList()).ShowDialog();
            RefreshButton_Click(sender, e);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null) return;
            selectedUser.IsDeleted = true;
            RemoteObjectProvider.GetLiteUserAccess().SaveOrUpdate(selectedUser);
            RefreshButton_Click(sender, e);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UsersDataGrid.ItemsSource = RemoteObjectProvider.GetLiteUserAccess().LoadAll().Where(u => !u.IsDeleted);
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedUser = (LiteUser)e.AddedItems[0];
            }
            else
            {
                selectedUser = null;
            }
        }
    }
}
