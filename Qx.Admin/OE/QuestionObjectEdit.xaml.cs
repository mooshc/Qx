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
using Qx.Learning;

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for QuestionObjectEdit.xaml
    /// </summary>
    public partial class QuestionObjectEdit : Window
    {
        private List<string> AllQuestions;
        public QuestionObjectEdit()
        {
            DataContext = new Question();
            Initialize();
        }

        public QuestionObjectEdit(Question Question, bool isCopy = false)
        {
            DataContext = Question;
            Initialize();
            QuestionTypeComboBox.SelectedIndex = Question.QuestionType.ID - 1;
            CheckBox_Checked(NoCharCheckBox, null);
            if (isCopy)
            {
                (DataContext as Question).ZeroID();
                (DataContext as Question).Name = NameTextBox.Text = "הכנס שם";
                foreach (Answer a in (DataContext as Question).Answers)
                    a.ZeroID();
                /*var question = new Question();
                question.Answers = CopyAnswers(q.Answers);
                question.Combinations = CopyCombinations(q.Combinations);
                question.EndingChar = q.EndingChar;
                question.IsEnter = q.IsEnter;
                question.IsWithoutEndingChar = q.IsWithoutEndingChar;
                question.Name = "";
                question*/
            }
        }

        void Initialize()
        {
            InitializeComponent();
            QuestionTypeComboBox.ItemsSource = RemoteObjectProvider.GetQuestionTypeAccess().LoadAll().Where(qt => !qt.IsDeleted && !qt.Name.Contains("ללא"));
            AllQuestions = RemoteObjectProvider.GetQuestionAccess().GetAllQuestionsNames();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var question = DataContext as Question;
            question.QuestionHebText = HebQuestionTextBox.Text;
            question.PreQuestionHebText = HebPreQuestionTextBox.Text;
            question.PreQuestionHebTextFemale = HebPreQuestionFemaleTextBox.Text;
            question.ToolTipHebText = TooltipTextBox.Text;
            foreach (var ans in question.Answers)
            {
                var text = ans.AnswerHebText;
                var male = ans.ResultMaleHebText;
                var female = ans.ResultFemaleHebText;
                var split = ans.Name.Split('.');
                ans.Name = question.Name + "." + (split.Length == 1 ? split[0] : split.Last());
                ans.ResultFemaleHebText = female;
                ans.ResultMaleHebText = male;
                ans.AnswerHebText = text;
            }
            RemoteObjectProvider.GetQuestionAccess().SaveOrUpdate(question);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AnswersButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionTypeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("בחר סוג שאלה");
                return;
            }
            if (NameTextBox.Text == "הכנס שם")
            {
                MessageBox.Show("הכנס שם לפני עריכת התשובות");
                return;
            }
            var question = DataContext as Question;
            question.QuestionHebText = HebQuestionTextBox.Text;
            question.PreQuestionHebText = HebPreQuestionTextBox.Text;
            question.PreQuestionHebTextFemale = HebPreQuestionFemaleTextBox.Text;
            foreach (var ans in question.Answers)
            {
                var text = ans.AnswerHebText;
                var male = ans.ResultMaleHebText;
                var female = ans.ResultFemaleHebText;
                var split = ans.Name.Split('.');
                ans.Name = question.Name + "." + (split.Length == 1 ? split[0] : split.Last());
                ans.ResultFemaleHebText = female;
                ans.ResultMaleHebText = male;
                ans.AnswerHebText = text;
            }
            DataContext = RemoteObjectProvider.GetQuestionAccess().SaveOrUpdate(question);
            new AnswersOM(DataContext as Question).ShowDialog();
        }

        private void CombinationButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionTypeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("בחר סוג שאלה");
                return;
            }
            var question = DataContext as Question;
            question.QuestionHebText = HebQuestionTextBox.Text;
            question.PreQuestionHebText = HebPreQuestionTextBox.Text;
            DataContext = RemoteObjectProvider.GetQuestionAccess().SaveOrUpdate(question);
            new CombinationsOM(DataContext as Question).ShowDialog();
        }

        private void PreviewQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDictionary.PopulateDictionary(CommonFunctions.HebLang);
            var question = DataContext as Question;
            question.QuestionHebText = HebQuestionTextBox.Text;
            question.PreQuestionHebText = HebPreQuestionTextBox.Text;
            question = RemoteObjectProvider.GetQuestionAccess().SaveOrUpdate(question);

            var testModule = RemoteObjectProvider.GetModuleAccess().LoadModuleByName("TestModule");
            testModule.Questions.Clear();
            testModule.Questions.Add(new QuestionInModule(question, 1, false));
            new ModuleWindow(testModule).ShowDialog();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CharTextBox == null) return;
            if ((sender as CheckBox).IsChecked ?? false)
                CharTextBox.Visibility = System.Windows.Visibility.Hidden;
            else
                CharTextBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(AllQuestions != null)
            if (AllQuestions.Contains(NameTextBox.Text))
                NameTextBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
            else
                NameTextBox.Background = new SolidColorBrush(Colors.White);
        }
    }
}
