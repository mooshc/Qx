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
    /// Interaction logic for DictionaryObjectEdit.xaml
    /// </summary>
    public partial class DictionaryObjectEdit : Window
    {
        public DictionaryObjectEdit()
        {
            Initialize();
            DataContext = new Dictionary();
        }

        public DictionaryObjectEdit(Dictionary Dictionary)
        {
            Initialize();
            DataContext = Dictionary;
        }

        void Initialize()
        {
            InitializeComponent();
            LanguageCombobox.ItemsSource = RemoteObjectProvider.GetLanguageAccess().LoadAll().Where(l => !l.IsDeleted);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var dic = DataContext as Dictionary;
            RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdate(dic);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
