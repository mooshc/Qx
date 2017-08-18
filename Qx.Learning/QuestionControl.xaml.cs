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
using Qx.Common;
using System.Windows.Controls.Primitives;

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for QuestionControl.xaml
    /// </summary>
    public partial class QuestionControl : UserControl
    {
        public Question Question;
        public TextBox OpenQuestionTextBox;
        public bool IsValid
        {
            get
            {
                if (Question.QuestionType.Name.Contains("שאלה פתוחה"))
                    return OpenQuestionTextBox.Text != "";
                bool valid = true;
                bool isChecked = false;
                foreach (AnswerControl a in AnswersStackPanel.Children)
                {
                    valid = valid && a.IsValid;
                    isChecked = isChecked || a.tb.IsChecked.Value;
                }
                return valid && isChecked;
            }
        }
        public bool IsCombinated = false;
        public bool HasCombinations = false;
        public bool IsExtraQuestion = false;
        public string SendingAnswerName = null;
        public int SendingAnswerID;

        public QuestionControl(Question question, bool IsExtraQuestion = false, string questenTitle = "", int sendingID = 0)
        {
            InitializeComponent();
            if (IsExtraQuestion)
            {
                this.IsExtraQuestion = true;
                SendingAnswerID = sendingID;
                SendingAnswerName = questenTitle == "" ? null : questenTitle;
                if (question.QuestionHebText == "" || question.QuestionHebText == "לא נמצא במילון")
                    QuestionLabel.Content = ContentDictionary.GetContent(questenTitle, Session.Lang);
                else
                    QuestionLabel.Content = question.QuestionHebText;
                Background = new SolidColorBrush(Colors.White);
                Padding = new Thickness(25, 0, 0, 0);
                MinWidth = 200;
            }
            else
            {
                MinHeight = 50;
                MinWidth = 380;
                QuestionLabel.Content = ((question.QuestionHebText == "" || question.QuestionHebText == "לא נמצא במילון") && questenTitle != "") 
                                                                ? ContentDictionary.GetContent(questenTitle,Session.Lang) : question.QuestionHebText;
                GotFocus += UserControl_GotFocus;
                LostFocus += UserControl_LostFocus;
                //MouseLeftButtonDown += UserControl_GotFocus;
            }
            Question = question;
            switch (Question.QuestionType.ID)
            {
                case 1:
                case 2: CreateToggleQuestion();
                    break;

                case 3: CreateOpenQuestion();
                    break;

                default: break;
            }
        }

        private void CreateToggleQuestion()
        {
            AnswerControl ac;
            foreach (var answer in Question.Answers.Where(a => !a.IsDeleted))
            {
                ac = new AnswerControl(answer, this);
                AnswersStackPanel.Children.Add(ac);
                if (answer.IsSingular)
                    ac.tb.Click += SingularAnswer_Clicked;
            }
        }

        protected void SingularAnswer_Clicked(object sender, EventArgs e)
        {
            if (!(sender as ToggleButton).IsChecked ?? false)
            {
                foreach (AnswerControl ac in AnswersStackPanel.Children)
                    ac.tb.IsEnabled = true;
            }
            else
            {
                foreach (AnswerControl ac in AnswersStackPanel.Children)
                {
                    if (ac.tb != sender)
                    {
                        ac.tb.IsChecked = false;
                        ac.tb.IsEnabled = false;
                    }
                }
            }
        }

        private void CreateOpenQuestion()
        {
            OpenQuestionTextBox = new TextBox();
            OpenQuestionTextBox.Width = 330;
            OpenQuestionTextBox.Margin = new Thickness(20, 0, 0, 0);
            AnswersStackPanel.Children.Add(OpenQuestionTextBox);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var win = (((this.Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
            Background = win.Module.ModuleType.Name.Contains("אנמנזה") ? (Brush)new BrushConverter().ConvertFrom("#e6f1f2") : (Brush)new BrushConverter().ConvertFrom("#dcdae6"); 
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            Background = (Brush)new BrushConverter().ConvertFrom("#ebebeb");
        }
    }
}
