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
    /// Interaction logic for QuestionSelector.xaml
    /// </summary>
    public partial class QuestionSelector : Window
    {
        public ObservableCollection<QuestionInModule> ExistingQuestions { get { return new ObservableCollection<QuestionInModule>((DataContext as IList<QuestionInModule>).OrderBy(qim => qim.Ordering)); } }

        public IOrderedEnumerable<string> NotExistingQuestionsSource { get; }
        public ObservableCollection<string> NotExistingQuestions { get; set; }

        public QuestionSelector(IList<QuestionInModule> Questions)
        {
            DataContext = Questions;
            InitializeComponent();
            ExistingQuestionsListBox.ItemsSource = ExistingQuestions;
            NotExistingQuestionsSource = RemoteObjectProvider.GetQuestionAccess().GetAllQuestionsNames().Where(Q => !Q.Contains("ללא") && (DataContext as IList<QuestionInModule>).Count(q => q.Question.Name == Q) == 0).OrderByDescending(n => n);
            NotExistingQuestions = new ObservableCollection<string>(NotExistingQuestionsSource);
            NotExistingQuestionsListBox.ItemsSource = NotExistingQuestions;
        }

        private void GetInButton_Click(object sender, RoutedEventArgs e)
        {
            var qim = new QuestionInModule(RemoteObjectProvider.GetQuestionAccess().LoadByName(NotExistingQuestionsListBox.SelectedItem.ToString()),
                (DataContext as IList<QuestionInModule>).Count == 0 ? 1 : (DataContext as IList<QuestionInModule>).Max(q => q.Ordering) + 1, false);
            (DataContext as IList<QuestionInModule>).Add(qim);
            NotExistingQuestions.Remove(NotExistingQuestionsListBox.SelectedItem.ToString());
            ExistingQuestionsListBox.ItemsSource = ExistingQuestions;
        }

        private void GetOutButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as IList<QuestionInModule>).Remove((ExistingQuestionsListBox.SelectedItem as QuestionInModule));
            NotExistingQuestions.Add((ExistingQuestionsListBox.SelectedItem as QuestionInModule).Question.Name);
            ExistingQuestionsListBox.ItemsSource = ExistingQuestions;
            var index = 1;
            foreach (var qim in (DataContext as IList<QuestionInModule>).OrderBy(q => q.Ordering))
            {
                qim.Ordering = index;
                index++;
            }
        }

        private void GetDownButton_Click(object sender, RoutedEventArgs e)
        {
            var first = ExistingQuestionsListBox.SelectedItem as QuestionInModule;
            if (first.Ordering == (DataContext as IList<QuestionInModule>).Count) return;
            var second = (DataContext as IList<QuestionInModule>).Where(qim => qim.Ordering == first.Ordering + 1).ToList().FirstOrDefault();
            first.Ordering++;
            second.Ordering--;
            ExistingQuestionsListBox.ItemsSource = ExistingQuestions;
        }

        private void GetUpButton_Click(object sender, RoutedEventArgs e)
        {
            var first = ExistingQuestionsListBox.SelectedItem as QuestionInModule;
            if (first.Ordering == 1) return;
            var second = (DataContext as IList<QuestionInModule>).Where(qim => qim.Ordering == first.Ordering - 1).ToList().FirstOrDefault();
            first.Ordering--;
            second.Ordering++;
            ExistingQuestionsListBox.ItemsSource = ExistingQuestions;
        }

        private void ToggleBreakPage_Click(object sender, RoutedEventArgs e)
        {
            var qim = ExistingQuestionsListBox.SelectedItem as QuestionInModule;
            qim.IsPageBreak = !qim.IsPageBreak;
            ExistingQuestionsListBox.ItemsSource = ExistingQuestions;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTextbox.Text = "";
        }

        private void FilterTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterTextbox.Text == "")
            {
                NotExistingQuestions = new ObservableCollection<string>(NotExistingQuestionsSource);
            }
            else
            {
                string textToSearch = FilterTextbox.Text.ToLower();
                NotExistingQuestions = new ObservableCollection<string>(NotExistingQuestionsSource.Where(n => n.ToLower().Contains(textToSearch)));
            }
            NotExistingQuestionsListBox.ItemsSource = NotExistingQuestions;
        }
    }
}
