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
using System.Threading;
//using System.Windows.Forms;

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for AnswerControl.xaml
    /// </summary>
    public partial class AnswerControl : UserControl
    {
        public QuestionControl qc;
        public ToggleButton tb;
        public TextBox textBox;
        public Answer Answer;
        public Label WarningLabel;
        private Expander expander;
        public bool IsValid 
        { 
            get 
            {
                int num;
                return !Answer.IsContainsTextBox || !tb.IsChecked.Value
                    || (tb.IsChecked.Value && textBox.Text != "" && Answer.IsTextBoxDigitsOnly && Int32.TryParse(textBox.Text, out num))
                    || (tb.IsChecked.Value && !Answer.IsTextBoxDigitsOnly);
            } 
        }

        public AnswerControl()
        {
            InitializeComponent();
        }

        public AnswerControl(Answer answer, QuestionControl qc1)
        {
            qc = qc1;
            Answer = answer;
            InitializeComponent();
            tb = answer.Question.QuestionType.ID == 1 ? (ToggleButton)new CheckBox() : new RadioButton();
            if(answer.IsSingular)
                tb.BorderBrush = new SolidColorBrush(Colors.Black);
            var content = ContentDictionary.GetContent(answer.Name, Session.Lang);
            if (answer.IsContainsTextBox)
            {
                textBox = new TextBox() 
                { 
                    Width = answer.IsTextBoxDigitsOnly ? 50 : 160, Height = 17, VerticalAlignment = System.Windows.VerticalAlignment.Top, FontSize = 10, IsEnabled = false,
                        /*BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = new SolidColorBrush(Colors.Black),*/ Background = new SolidColorBrush(Colors.Transparent)
                };
                textBox.KeyUp += new KeyEventHandler(textBox_KeyDown);
                textBox.TextChanged += TextBox_TextChanged;
                if(answer.IsTextBoxDigitsOnly) textBox.ToolTip = ContentDictionary.GetContent("DigitsOnly", Session.Lang);

                if (content.Contains("{text}"))
                {
                    textBox.Margin = new Thickness(0, 5, 0, 0);
                    var parts = content.Split(new[] { "{text}" }, StringSplitOptions.None);
                    var sp = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, -5, 0, -5) };
                    sp.Children.Add(new Label() { Content = parts[0] });
                    sp.Children.Add(textBox);
                    if(parts.Count() > 1) sp.Children.Add(new Label() { Content = parts[1] });
                    sp.Margin = new Thickness(-4, -5, 0, -5);
                    tb.Content = sp;
                    AnswerPanel.Children.Add(tb);
                }
                else
                {
                    textBox.Margin = new Thickness(10, 0, 0, 0);
                    tb.Content = content;
                    AnswerPanel.Children.Add(tb);
                    AnswerPanel.Children.Add(textBox);
                }
            }
            else
            {
                tb.Content = content;
                AnswerPanel.Children.Add(tb);
            }

            if (answer.Question.QuestionType.ID == 2)
                (tb as RadioButton).GroupName = answer.Question.Name;
            tb.Unchecked += TB_Clicked;
            tb.Checked += TB_Clicked;
            if (answer.ImageFileName != null)
                AnswerPanel.Children.Add(new Image() { Source = CommonFunctions.GetBmpImageByFileName(answer.ImageFileName), HorizontalAlignment = HorizontalAlignment.Center });
            if (answer.WarningConditions.Where(con => con.ConditionType == 'V').Count() > 0)
            {
                WarningLabel = new Label()
                {
                    Name = "WarningLabel",
                    FontWeight = FontWeights.Bold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#D00000"),
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(20, -3, 0, -3)
                };
                AnswerPanel.Children.Add(WarningLabel);
            }
            if(textBox != null)
                textBox.Background = new SolidColorBrush(Colors.Transparent);
        }

        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            Thread thread = null;
            if(e.Key == Key.Up)
                thread = new Thread(Up);
            else if( e.Key == Key.Down)
                thread = new Thread(Down);

            if (thread == null) return;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private void Down()
        {
            System.Windows.Forms.SendKeys.SendWait("{TAB}");
        }

        private void Up()
        {
            System.Windows.Forms.SendKeys.SendWait("+{TAB}");
        }

        protected void TB_Clicked(object sender, EventArgs e)
        {
            if ((((this.Parent as StackPanel).Parent as Grid).Parent as QuestionControl).Parent == null)
                return;
            ModuleWindow win = null;
            if(!qc.IsExtraQuestion)
                win = ((((((this.Parent as StackPanel).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
            if (Answer.ExtraQuestion != null)
            {
                if ((sender as ToggleButton).IsChecked ?? false)
                {
                    Background = win.Module.ModuleType.Name.Contains("אנמנזה") ? (Brush)new BrushConverter().ConvertFrom("#b5afcb") : (Brush)new BrushConverter().ConvertFrom("#b2d9de");
                    Background.Opacity = 0.3;
                    var originalQuestionControl = (((this.Parent as StackPanel).Parent as Grid).Parent as QuestionControl);
                    var point = this.TranslatePoint(new Point(), win);
                    var qc2 = new QuestionControl(Answer.ExtraQuestion, true, Answer.Name, Answer.ID);
                    win.extraQuestionControls.Add(qc2);
                    var newWin = new ExtraQuestionWindow(qc2, new Point(win.Left, win.Top + point.Y + this.ActualHeight/2));
                    var result = newWin.ShowDialog();
                    if (!result.Value)
                    {
                        win.extraQuestionControls.RemoveAll(q => q.SendingAnswerName.Equals(Answer.Name));
                        tb.IsChecked = false;
                    }
                    else
                    {
                        expander = new Expander();
                        //expander.Height = expander.Width = 15;
                        expander.ExpandDirection = ExpandDirection.Right;
                        expander.Margin = new Thickness(0, -5, 0, -8);
                        expander.Expanded += new RoutedEventHandler(expander_Expanded);
                        AnswerPanel.Children.Add(expander);
                    }
                    Background = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    win.extraQuestionControls.RemoveAll(q => q.Question.Equals(Answer.ExtraQuestion) && q.SendingAnswerName.Contains(Answer.Name));
                    Background = new SolidColorBrush(Colors.Transparent);
                    AnswerPanel.Children.Remove(expander);
                }
            }

            if ((sender as ToggleButton).IsChecked ?? false)
            {
                if (Answer.IsContainsTextBox)
                {
                    TextBox_TextChanged(null, null);
                    textBox.IsEnabled = true;
                }
                var con = Answer.WarningConditions.FirstOrDefault(c => c.ConditionType == 'V');
                if (con != null)
                {
                    Background = new SolidColorBrush(con.Color.GetColor()) { Opacity = 0.2 };
                    WarningLabel.Content = ContentDictionary.GetContent(con.Name, Session.Lang);
                }
            }
            else
            {
                if (Answer.IsContainsTextBox)
                {
                    textBox.Background = new SolidColorBrush(Colors.Transparent);
                    textBox.Text = "";
                    textBox.IsEnabled = false;
                }
                Background = new SolidColorBrush(Colors.Transparent);
                if (WarningLabel != null) WarningLabel.Content = null;
            }
            if(win != null && win.ShowErrors)
                win.CheckErrors();
        }

        void expander_Expanded(object sender, RoutedEventArgs e)
        {
            var win = ((((((this.Parent as StackPanel).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;

            expander.IsExpanded = false;

            Background = win.Module.ModuleType.Name.Contains("אנמנזה") ? (Brush)new BrushConverter().ConvertFrom("#b2d9de") : (Brush)new BrushConverter().ConvertFrom("#b5afcb");
            Background.Opacity = 0.3;
            var originalQuestionControl = (((this.Parent as StackPanel).Parent as Grid).Parent as QuestionControl);
            var point = originalQuestionControl.TranslatePoint(new Point(), win);
            //var qc = new QuestionControl(Answer.ExtraQuestion, true);
            var newWin = new ExtraQuestionWindow(win.extraQuestionControls.Where(qc => qc.Question == Answer.ExtraQuestion).ToList().FirstOrDefault(), new Point(win.Left, win.Top + point.Y + originalQuestionControl.ActualHeight / 2));
            var result = newWin.ShowDialog();
            Background = new SolidColorBrush(Colors.Transparent);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((((this.Parent as StackPanel).Parent as Grid).Parent as QuestionControl).Parent == null)
                return;

            if (tb.IsChecked ?? false)
            {
                if (Answer.IsTextBoxDigitsOnly)
                {
                    if (tb.IsChecked.Value && textBox.Text == "")
                    {
                        textBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                        return;
                    }
                    decimal num;
                    if (Decimal.TryParse(textBox.Text, out num) || textBox.Text == "")
                        textBox.Background = new SolidColorBrush(Colors.Transparent);
                    else
                    {
                        textBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                        return;
                    }
                }

                textBox.Background = new SolidColorBrush(Colors.Transparent);
                textBox.ToolTip = null;
                var conditions = Answer.WarningConditions.Where(c => c.ConditionType != 'V' && c.ConditionType != 'X').ToList();
                if (conditions.Count > 0)
                    foreach (var con in conditions)
                        if (IsCondition(con))
                        {
                            textBox.Background = new SolidColorBrush(con.Color.GetColor()) { Opacity = 0.3 };
                            textBox.ToolTip = ContentDictionary.GetContent(con.Name, Session.Lang);
                        }
            }

            if (!qc.IsExtraQuestion)
            {
                var win = ((((((this.Parent as StackPanel).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
                if (win.ShowErrors)
                    win.CheckErrors();
            }
        }

        private bool IsCondition(Qx.Common.Condition con)
        {
            var textBoxValue = Decimal.Parse(textBox.Text);
            switch (con.ConditionType)
            {
                case '>': if (textBoxValue > con.Value)
                            return true;
                            break;

                case '<': if (textBoxValue < con.Value)
                            return true;
                            break;

                case '=': if (textBoxValue == con.Value)
                            return true;
                            break;

                case 'B': if (textBoxValue > con.Value && textBoxValue < con.SecondValue)
                            return true;
                            break;

                default: break;
            }

            return false;
        }
    }
}
