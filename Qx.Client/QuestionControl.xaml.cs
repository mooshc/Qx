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

namespace Qx.Client
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
                while (AnswersStackPanel.Children.OfType<Border>().Count() > 0)
                    AnswersStackPanel.Children.Remove(AnswersStackPanel.Children.OfType<Border>().FirstOrDefault());
                if (Question.QuestionType.Name.Contains("שאלה פתוחה"))
                    return OpenQuestionTextBox.Text != "";
                if (Question.QuestionType.Name.Contains("שלילה אקטיבית"))
                {
                    bool valid1 = true;
                    List<int> rowIndexes = new List<int>();
                    foreach (AnswerControl a in AnswersStackPanel.Children.OfType<AnswerControl>())
                    {
                        if ((a.tb.IsChecked ?? false) && a.Answer.IsSingular && a.IsValid)
                            return true;
                        valid1 = valid1 && a.IsValid;
                        if (a.Answer.Name.Contains("#true"))
                        {
                            if (!((a.tb.IsChecked ?? false) || (AnswersStackPanel.Children.OfType<AnswerControl>().Where(an => an.Answer.Name.Contains("#") && !an.Answer.Name.Contains(a.Answer.Name)).Where(an => an.Answer.Name.Contains(a.Answer.Name.Substring(0, a.Answer.Name.IndexOf("#")))).FirstOrDefault().tb.IsChecked ?? false)))
                            {
                                valid1 = false;
                                rowIndexes.Add((int)a.GetValue(Grid.RowProperty));
                            }
                            
                        }
                    }
                    if(Question.QuestionType.Name.Contains("שלילה אקטיבית"))
                    {
                        foreach(int index in rowIndexes)
                        {
                                var b = new Border() { BorderBrush = new SolidColorBrush(Colors.Red), BorderThickness = new Thickness(1) };
                                b.SetValue(Grid.RowProperty, index);
                                b.SetValue(Grid.ColumnSpanProperty, 2);
                                AnswersStackPanel.Children.Add(b);
                        }
                    }
                    return valid1;
                }
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
                MinWidth = 100;
            }
            else
            {
                MinHeight = 50;
                MinWidth = 380;
                QuestionLabel.Content = ContentDictionary.GetContent(question.Name, Session.Lang);
                GotFocus += UserControl_GotFocus;
                LostFocus += UserControl_LostFocus;
                //MouseLeftButtonDown += UserControl_GotFocus;
            }
            Question = question;

            QuestionLabel.ToolTip = question.ID;

            switch (Question.QuestionType.ID)
            {
                case 1:
                case 2: CreateToggleQuestion();
                    break;

                case 3: CreateOpenQuestion();
                    break;

                case 5: CreateActiveNegationQuestion();
                    break;

                default: break;
            }
        }

        private void CreateActiveNegationQuestion()
        {
            AnswerControl ac;
            var totalQuestions = Question.Answers.Where(a => !a.IsDeleted && !a.Name.Contains("#")).Count();
            totalQuestions += Question.Answers.Where(a => !a.IsDeleted && a.Name.Contains("#")).Count() / 2;
            for (int i = 0 ; i < totalQuestions ; i++)
            {
                AnswersStackPanel.RowDefinitions.Add(new RowDefinition());
            }
            AnswersStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            AnswersStackPanel.ColumnDefinitions.Add(new ColumnDefinition());
            int rowCounter = 0;
            foreach (Answer ans in Question.Answers.Where(a => !a.IsDeleted))
            {
                if (ans.Name.Contains("#"))
                {
                    if (ans.Name.ToLower().Contains("#true"))
                    {
                        ac = new AnswerControl(ans, this);
                        ac.SetValue(Grid.RowProperty, rowCounter);
                        ac.SetValue(Grid.ColumnProperty, 1);
                        AnswersStackPanel.Children.Add(ac);

                        var a = Question.Answers.Where(aa => aa.Name.Contains(ans.Name.Substring(0, ans.Name.IndexOf("#"))) && aa.Name.ToLower().EndsWith("#false")).FirstOrDefault();
                        if (a != null)
                        {
                            ac = new AnswerControl(a, this);
                            ac.Margin = new Thickness(0, 2, 0, 0);
                            ac.SetValue(Grid.RowProperty, rowCounter);
                            ac.tb.KeyDown += new KeyEventHandler(tb_KeyDown);
                            //ac.SetValue(VerticalAlignmentProperty, VerticalAlignment.Bottom);
                            AnswersStackPanel.Children.Add(ac);
                        }
                    }
                    else continue;
                }
                else
                {
                    ac = new AnswerControl(ans, this);
                    ac.SetValue(Grid.RowProperty, rowCounter);
                    ac.SetValue(Grid.ColumnSpanProperty, 2);
                    ac.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    AnswersStackPanel.Children.Add(ac);
                    if (ans.IsSingular)
                        ac.tb.Click += SingularAnswer_Clicked;
                }
                rowCounter++;
            }
        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.Down)
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                e.Handled = true;
                return;
            }*/
            /*if (e.Key == Key.Up)
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                e.Handled = true;
            }*/
        }

        private void CreateToggleQuestion()
        {
            AnswerControl ac;
            for (int i = 0; i < Question.Answers.Where(a => !a.IsDeleted).Count(); i++)
            {
                AnswersStackPanel.RowDefinitions.Add(new RowDefinition());
            }
            int index = 0;
            foreach (var answer in Question.Answers.Where(a => !a.IsDeleted))
            {
                ac = new AnswerControl(answer, this);
                ac.SetValue(Grid.RowProperty, index++);
                AnswersStackPanel.Children.Add(ac);
                if (answer.IsSingular && Question.QuestionType.ID == 1)
                    ac.tb.Click += SingularAnswer_Clicked;
            }
        }

        protected void SingularAnswer_Clicked(object sender, EventArgs e)
        {
            if (!(sender as ToggleButton).IsChecked ?? false)
            {
                foreach (AnswerControl ac in AnswersStackPanel.Children.OfType<AnswerControl>())
                    ac.tb.IsEnabled = ac.IsEnabled = true;
            }
            else
            {
                foreach (AnswerControl ac in AnswersStackPanel.Children.OfType<AnswerControl>())
                {
                    if (ac.tb != sender)
                    {
                        ac.tb.IsChecked = false;
                        ac.tb.IsEnabled = false;
                        ac.IsEnabled = false;
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
