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
using System.Windows.Forms;
using Qx.Common;

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for QxMessageBox.xaml
    /// </summary>
    public partial class QxMessageBox : Window
    {
        public QxMessageBox(string header)
        {
            InitializeComponent();
            ContentLabel.Content = header;
            YesButton.Content = ContentDictionary.GetContent("Yes", Session.Lang);
            NoButton.Content = ContentDictionary.GetContent("No", Session.Lang);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
