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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Qx.Common;
using System.Collections;
using Microsoft.Windows.Controls;

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for ObjectGrid.xaml
    /// </summary>
    public abstract partial class ObjectGrid
    {
        public static readonly DependencyProperty CanCreateProperty = DependencyProperty.Register("CanCreate", typeof(bool), typeof(ObjectGrid));
        public static readonly DependencyProperty CanEditProperty = DependencyProperty.Register("CanEdit", typeof(bool), typeof(ObjectGrid));
        public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register("CanDelete", typeof(bool), typeof(ObjectGrid));

        protected ObjectGrid()
        {
            InitializeComponent();
            dataGrid.SelectionMode = Microsoft.Windows.Controls.DataGridSelectionMode.Single;
            //dataGrid.ItemsSource = ItemSource() as IEnumerable;
            dataGrid.MouseDoubleClick += EditButton_Click;
        }

        public bool CanCreate
        {
            private get { return (bool)GetValue(CanCreateProperty); }
            set { SetValue(CanCreateProperty, value); }
        }

        public bool CanEdit
        {
            private get { return (bool)GetValue(CanEditProperty); }
            set { SetValue(CanEditProperty, value); }
        }

        public bool CanDelete
        {
            private get { return (bool)GetValue(CanDeleteProperty); }
            set { SetValue(CanDeleteProperty, value); }
        }

        public bool AutoGenerateColumns
        {
            get { return dataGrid.AutoGenerateColumns; }
            set { dataGrid.AutoGenerateColumns = value; }
        }

        public ObservableCollection<Microsoft.Windows.Controls.DataGridColumn> Columns
        {
            get { return dataGrid.Columns; }
            set
            {
                foreach (var dataGridColumn in value)
                    dataGrid.Columns.Add(dataGridColumn);
            }
        }

        protected abstract object ItemSource();

        protected object GetSelectedItem()
        {
            if (dataGrid.SelectedItem is Question && dataGrid.DataContext != null)
                return (dataGrid.DataContext as List<Question>).Where(q => q.ID == (dataGrid.SelectedItem as Question).ID).FirstOrDefault();
            return dataGrid.SelectedItem;
        }

        protected int? GetSelectedId()
        {
            if (dataGrid.SelectedItem == null) return null;

            var IdProperty = dataGrid.SelectedItem.GetType().GetProperty("ID");
            return IdProperty.GetValue(dataGrid.SelectedItem, null) as int?;
        }

        protected IList GetSelectedItems()
        {
            return dataGrid.SelectedItems;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ((IWpfObjectGrid)this).New();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ((IWpfObjectGrid)this).Edit();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(String.Format("Do u want to delete {0} items?", GetSelectedItems().Count), "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            ((IWpfObjectGrid)this).Delete();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshItemSource();
        }

        public void RefreshItemSource()
        {
            try
            {
                dataGrid.ItemsSource = ItemSource() as IEnumerable;
                dataGrid.Items.Refresh();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occourd here r the details: " + e.Message);
            }
        }

        protected void SetItemSource(object itemSource)
        {
            dataGrid.ItemsSource = itemSource as IEnumerable;
        }

        private bool HasRefreshButton()
        {
            return toolBar.Items.Cast<object>().Any(control => (control as Button) != null && (control as Button).Name == "RefreshButton");
        }

        private void objectGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (HasRefreshButton()) return;

            toolBar.Items.Add(new Separator());
            var bttn = new Button { Content = "Refresh", Width = 45, Name = "RefreshButton" };
            bttn.Click += RefreshButton_Click;
            toolBar.Items.Add(bttn);
        }
    }
}
