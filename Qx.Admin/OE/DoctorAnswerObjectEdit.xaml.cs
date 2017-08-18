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
    /// Interaction logic for DoctorAnswerObjectEdit.xaml
    /// </summary>
    public partial class DoctorAnswerObjectEdit : Window
    {
        Scenario Scenario;
        bool IsAnamnesis;
        public DoctorAnswerObjectEdit(Scenario scenario, bool isAnamnesis, DoctorAnswer da = null)
        {
            InitializeComponent();
            DataContext = da ?? new DoctorAnswer();
            Scenario = scenario;
            IsAnamnesis = isAnamnesis;
            AnswerNameComboBox.ItemsSource = RelatedAnswerNameComboBox.ItemsSource = RemoteObjectProvider.GetAnswerAccess().GetAllNames();
            if (da != null)
            {
                AnswerNameComboBox.SelectedItem = da.Answer.Name;
                RelatedAnswerNameComboBox.SelectedItem = da.RelatedAnswerName;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            AnswerNameComboBox.Background = RelatedAnswerNameComboBox.Background = new SolidColorBrush(Colors.Transparent);
            if(AnswerNameComboBox.SelectedItem == null)
            {
                AnswerNameComboBox.Background = new SolidColorBrush(Colors.Red);
                return;
            }
            if (RelatedAnswerNameComboBox.SelectedItem == null && RelatedAnswerNameComboBox.Text != "")
            {
                RelatedAnswerNameComboBox.Background = new SolidColorBrush(Colors.Red);
                return;
            }
            var da = (DataContext as DoctorAnswer);
            da.AnswerID = RemoteObjectProvider.GetAnswerAccess().GetAnswerID(AnswerNameComboBox.SelectedItem.ToString());
            if (RelatedAnswerNameComboBox.SelectedItem != null)
                da.RelatedAnswerID = RemoteObjectProvider.GetAnswerAccess().GetAnswerID(RelatedAnswerNameComboBox.SelectedItem.ToString());

            if((DataContext as DoctorAnswer).ID == 0)
            {
                var docAns = RemoteObjectProvider.GetDoctorAnswerAccess().Save((DataContext as DoctorAnswer));
                if (IsAnamnesis)
                    Scenario.AnamnesisAnswers.Add(docAns);
                else
                    Scenario.PhysicalExAnswers.Add(docAns);
            }
            RemoteObjectProvider.GetScenarioAccess().SaveOrUpdate(Scenario);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
