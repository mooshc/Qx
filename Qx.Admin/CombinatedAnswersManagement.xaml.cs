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
using System.Collections.ObjectModel;
using Qx.Common;

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for CombinatedAnswersManagement.xaml
    /// </summary>
    public partial class CombinatedAnswersManagement : Window
    {
        public ObservableCollection<CombinatedAnswer> ExistingAnswers { get { return new ObservableCollection<CombinatedAnswer>((DataContext as IList<CombinatedAnswer>)); } }
        public ObservableCollection<Answer> NotExistingAnswers { get; set; }

        public CombinatedAnswersManagement(IList<CombinatedAnswer> existing, IList<Answer> allAnswersAvailable)
        {
            DataContext = existing;
            InitializeComponent();
            ExistingAnswersListBox.ItemsSource = ExistingAnswers;
            NotExistingAnswers = new ObservableCollection<Answer>(allAnswersAvailable.Where(a => existing.Count(ex => ex.Answer == a) == 0));
            NotExistingAnswersListBox.ItemsSource = NotExistingAnswers;
        }

        private void GetInButton_Click(object sender, RoutedEventArgs e)
        {
            var combinatedAnswer = new CombinatedAnswer() { Answer = (Answer)NotExistingAnswersListBox.SelectedItem };
            (DataContext as IList<CombinatedAnswer>).Add(combinatedAnswer);
            NotExistingAnswers.Remove((Answer)NotExistingAnswersListBox.SelectedItem);
            ExistingAnswersListBox.ItemsSource = ExistingAnswers;
        }

        private void GetOutButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as IList<CombinatedAnswer>).Remove((ExistingAnswersListBox.SelectedItem as CombinatedAnswer));
            NotExistingAnswers.Add((ExistingAnswersListBox.SelectedItem as CombinatedAnswer).Answer);
            ExistingAnswersListBox.ItemsSource = ExistingAnswers;
        }

        private void ToggleIsNot_Click(object sender, RoutedEventArgs e)
        {
            var qim = ExistingAnswersListBox.SelectedItem as CombinatedAnswer;
            qim.IsNot = !qim.IsNot;
            ExistingAnswersListBox.ItemsSource = ExistingAnswers;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
