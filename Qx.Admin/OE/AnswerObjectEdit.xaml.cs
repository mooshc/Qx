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
    /// Interaction logic for AnswerObjectEdit.xaml
    /// </summary>
    public partial class AnswerObjectEdit : Window
    {
        private Question Question;
        public ObservableCollection<Qx.Common.Condition> Conditions
        {
            get { return new ObservableCollection<Qx.Common.Condition>((DataContext as Answer).WarningConditions); }
        }
        

        public AnswerObjectEdit(Answer Answer, Question question)
        {
            Answer.Question = question;
            Question = question;
            DataContext = Answer;
            InitializeComponent();
            Title = "תשובה לשאלה: " + Answer.Question.QuestionHebText;
            var temp = RemoteObjectProvider.GetQuestionAccess().GetAllQuestionsNames().Where(n => n.Contains("Extra_") || n.Contains("Extra-")).OrderByDescending(n => n).ToList();
            temp.Add("ללא");
            ExtraQuestionComboBox.ItemsSource = temp;
            temp = RemoteObjectProvider.GetModuleAccess().GetModulesNames().Where(n => n.Contains("E_") || n.Contains("e_")).OrderByDescending(n => n).ToList();
            temp.Add("ללא");
            RecomendedModuleComboBox.ItemsSource = temp;
            WarningsComboBox.ItemsSource = RemoteObjectProvider.GetConditionAccess().LoadAll().Where(c => !(DataContext as Answer).WarningConditions.Contains(c)).ToList();
            WarningsListBox.ItemsSource = Conditions;
            if (Answer.Question.QuestionType.ID != 1 && Answer.Question.QuestionType.ID != 5)
                IsAndCheckBox.Visibility = Visibility.Hidden;
            if(Answer.Question.QuestionType.ID == 2)
                IsSingularCheckBox.Visibility = System.Windows.Visibility.Hidden;
            if(Answer.Question.QuestionType.ID == 3)
                IsContainsTextBoxCheckBox.Visibility = IsTextBoxDigitsOnlyCheckBox.Visibility = IsSingularCheckBox.Visibility = System.Windows.Visibility.Hidden;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var answer = DataContext as Answer;
            if (!answer.Question.Answers.Contains(answer))
            {
                answer.Name = answer.Question.Name + "." + answer.Name;
                Question.Answers.Add(answer);
            }
            if (ExtraQuestionComboBox.SelectedItem != null)
            {
                if (ExtraQuestionComboBox.SelectedItem.ToString() != "ללא")
                    answer.ExtraQuestion = RemoteObjectProvider.GetQuestionAccess().LoadByName(ExtraQuestionComboBox.SelectedItem.ToString());
                else
                    answer.ExtraQuestion = null;
            }
            if (RecomendedModuleComboBox.SelectedItem != null)
            {
                if (RecomendedModuleComboBox.SelectedItem.ToString() != "ללא")
                    answer.RecomendedPhysicalEx = RemoteObjectProvider.GetModuleAccess().LoadModuleByName(RecomendedModuleComboBox.SelectedItem.ToString());
                else
                    answer.RecomendedPhysicalEx = null;
            }
            answer.ResultFemaleHebText = ResultFemaleNameTextBox.Text;
            answer.ResultMaleHebText = ResultMaleNameTextBox.Text;
            answer.AnswerHebText = HebAnswerTextBox.Text;

            Question.Answers = RemoteObjectProvider.GetQuestionAccess().SaveOrUpdate(Question).Answers;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (WarningsComboBox.SelectedItem as Qx.Common.Condition != null)
            {
                (DataContext as Answer).WarningConditions.Add(WarningsComboBox.SelectedItem as Qx.Common.Condition);
                WarningsComboBox.ItemsSource = (WarningsComboBox.ItemsSource as List<Qx.Common.Condition>).Where(c => !(DataContext as Answer).WarningConditions.Contains(c));
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (WarningsListBox.SelectedItem as Qx.Common.Condition != null)
            {
                (DataContext as Answer).WarningConditions.Remove(WarningsListBox.SelectedItem as Qx.Common.Condition);
                WarningsComboBox.ItemsSource = RemoteObjectProvider.GetConditionAccess().LoadAll().Where(c => !(DataContext as Answer).WarningConditions.Contains(c)).ToList();
            }
        }

        private void IsContainsTextBoxCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (IsTextBoxDigitsOnlyCheckBox == null) return;
            if((sender as CheckBox).IsChecked ?? false)
                IsTextBoxDigitsOnlyCheckBox.Visibility = System.Windows.Visibility.Visible;
            else
                IsTextBoxDigitsOnlyCheckBox.Visibility = System.Windows.Visibility.Hidden;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
