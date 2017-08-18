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
    /// Interaction logic for ConditionObjectEdit.xaml
    /// </summary>
    public partial class ConditionObjectEdit : Window
    {
        public ConditionObjectEdit()
        {
            DataContext = new Qx.Common.Condition();
            InitializeComponent();
            ColorComboBox.ItemsSource = RemoteObjectProvider.GetColorAccess().LoadAll();
        }

        public ConditionObjectEdit(Qx.Common.Condition condition)
        {
            DataContext = condition;
            InitializeComponent();
            ColorComboBox.ItemsSource = RemoteObjectProvider.GetColorAccess().LoadAll();
        }

        private void ConditionTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConditionTypeTextBox.ToolTip = "ערך של תו אחד בלבד מהתווים: \n\r >,<,=,B,X,V";
            if (ConditionTypeTextBox.Text.Length > 1)
                ConditionTypeTextBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
            else
                ConditionTypeTextBox.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var con = DataContext as Qx.Common.Condition;
            con.ConditionHebText = ConditionTextBox.Text;
            RemoteObjectProvider.GetConditionAccess().SaveOrUpdate(con);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
