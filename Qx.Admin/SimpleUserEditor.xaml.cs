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
    /// Interaction logic for SimpleUserEditor.xaml
    /// </summary>
    public partial class SimpleUserEditor : Window
    {
        List<LiteUser> users;

        public SimpleUserEditor(List<LiteUser> allUsers)
            : this(new LiteUser(), allUsers) 
        {
            UserNameTextBox.IsEnabled = true;
            UserNameTextBox.TextChanged += UserNameTextBox_TextChanged;
        }

        public SimpleUserEditor(LiteUser user, List<LiteUser> allUsers)
        {
            DataContext = user;
            if (user.ID == 0)
                users = allUsers;
            else
                users = allUsers.Where(u => u.ID != user.ID).ToList();
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var user = DataContext as LiteUser;
            if ((UserNameTextBox.IsEnabled && users.Select(u => u.UserName).Contains(user.UserName)) 
                || user.UserName == "" 
                || user.FirstName == "" 
                || user.LastName == "" 
                || user.Password == "" 
                || (UserNameTextBox.IsEnabled 
                    && (   PIDTextBox.Text == "" 
                        || LisenceTextBox.Text == "" 
                        || EmpNumTextBox.Text == "" 
                        || ProfTextBox.Text == "")))
            {
                MessageBox.Show("נא למלא את כל השדות ולוודא ששם המשתמש לא קיים!");
                return;
            }

            if (UserNameTextBox.IsEnabled)
            {
                user.Company = RemoteObjectProvider.GetCompanyAccess().Load(1);
                user.IsAdmin = false;
                user.IsDeleted = false;
                user.IsLocked = false;
                user.Language = RemoteObjectProvider.GetLanguageAccess().Load(1);
            }

            if(user.ID == 0)
                RemoteObjectProvider.GetLiteUserAccess().Save(user);
            else
                RemoteObjectProvider.GetLiteUserAccess().SaveOrUpdate(user);
            Close();
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (users.Select(u => u.UserName).Contains(UserNameTextBox.Text))
            {
                UserNameTextBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                ToolTip = "שם משתמש תפוס";
            }
            else
            {
                UserNameTextBox.Background = new SolidColorBrush(Colors.Transparent);
                ToolTip = null;
            }
        }

        private void LisenceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (users.Select(u => u.License).Contains(LisenceTextBox.Text))
            {
                LisenceTextBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                ToolTip = "רשיון קיים במערכת";
            }
            else
            {
                LisenceTextBox.Background = new SolidColorBrush(Colors.Transparent);
                ToolTip = null;
            }
        }

        private void EmpNumTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (users.Select(u => u.Field1).Contains(EmpNumTextBox.Text))
            {
                EmpNumTextBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                ToolTip = "מספר עובד קיים במערכת";
            }
            else
            {
                EmpNumTextBox.Background = new SolidColorBrush(Colors.Transparent);
                ToolTip = null;
            }
        }

        private void PIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (users.Select(u => u.PID).Contains(PIDTextBox.Text))
            {
                PIDTextBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                PIDTextBox.ToolTip = "תעודת זהות קיימת במערכת";
            }
            else
            {
                PIDTextBox.Background = new SolidColorBrush(Colors.Transparent);
                PIDTextBox.ToolTip = null;
            }
        }
    }
}
