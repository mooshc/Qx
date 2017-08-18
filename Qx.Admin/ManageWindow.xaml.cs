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

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for ManageWindow.xaml
    /// </summary>
    public partial class ManageWindow : Window
    {
        public ManageWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Exit prompt", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
                e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
