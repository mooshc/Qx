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
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Threading;

namespace Qx.Client
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
        private Label WarningLabel;
        private Label DigitsLabel;
        private Expander expander;
        private BigTextBox btb;
        private bool isMouse;
        System.Windows.Style style;
        delegate void InsertText();
        delegate void SendNextElement();
        Brush brush;
        public bool IsValid 
        { 
            get 
            {
                decimal num;
                return !Answer.IsContainsTextBox || !tb.IsChecked.Value
                    || (tb.IsChecked.Value && textBox.Text != "" && Answer.IsTextBoxDigitsOnly && Decimal.TryParse(textBox.Text, out num))
                    || (tb.IsChecked.Value && !Answer.IsTextBoxDigitsOnly);
            } 
        }

        public AnswerControl()
        {
            InitializeComponent();
        }

        public AnswerControl(Answer answer, QuestionControl qc1)
        {
            Loaded += new RoutedEventHandler(AnswerControl_Loaded);
            qc = qc1;
            Answer = answer;
            InitializeComponent();
            tb = ((answer.Question.QuestionType.ID == 1 && !answer.Name.Contains("#single")) || (answer.Question.QuestionType.ID == 5 && !answer.Name.Contains("#")) ) ? (ToggleButton)new CheckBox() : new RadioButton();
            tb.KeyDown += new KeyEventHandler(tb_PreviewKeyDown);
            tb.FocusVisualStyle = null;
            if (!qc.IsExtraQuestion)
            {
                MinWidth = AnswerPanel.MinWidth = 380;
                //tb.Width = 380;
            }
            if(answer.IsSingular && (answer.Question.QuestionType.ID == 1 || answer.Question.QuestionType.ID == 5))
                tb.BorderBrush = new SolidColorBrush(Colors.Black);
            var content = answer.AnswerHebText;
            Image pic = null;
            Label l = null;
            if (answer.Question.QuestionType.ID == 5)
            {
                if (answer.Name.ToLower().EndsWith("#false"))
                {
                    l = new Label() { Content = "Não", FontWeight = FontWeights.Bold, Margin = new Thickness(-8,0,0,0) };
                    pic = new Image() { Source = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "X.png")), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Height = 10 };
                }
                else if (answer.Name.ToLower().EndsWith("#true"))
                {
                    l = new Label() { Content = "Sim", FontWeight = FontWeights.Bold, Margin = new Thickness(-8, 0, 0, 0) };
                    pic = new Image() { Source = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "V.png")), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Height = 10 };
                }
            }
            if (answer.IsContainsTextBox)
            {
                textBox = new TextBox() 
                { 
                    Width = answer.IsTextBoxDigitsOnly ? 50 : 160, Height = 17, VerticalAlignment = System.Windows.VerticalAlignment.Top, FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    IsEnabled = false,
                    Background = new SolidColorBrush(Colors.Transparent)
                    //, BorderBrush = (Brush)new BrushConverter().ConvertFrom("#F0F0F0"), BorderThickness = new Thickness(1)
                };
                textBox.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput);
                textBox.KeyUp += new KeyEventHandler(textBox_KeyDown);
                textBox.TextChanged += TextBox_TextChanged;
                textBox.GotMouseCapture += new MouseEventHandler(textBox_GotMouseCapture);
                textBox.GotFocus += new RoutedEventHandler(textBox_GotFocus);
                if (!answer.IsTextBoxDigitsOnly)  textBox.MouseDoubleClick += new MouseButtonEventHandler(textBox_MouseDoubleClick);
                //if (answer.IsTextBoxDigitsOnly) textBox.ToolTip = new ToolTip() { Content = ContentDictionary.GetContent("DigitsOnly", Session.Lang) };
                if (content.Contains("{text}"))
                {
                    textBox.Margin = new Thickness(0, 2, 0, 0);
                    var parts = content.Split(new[] { "{text}" }, StringSplitOptions.None);
                    var sp = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, -1, 0, -5) };
                    if (l != null)
                        sp.Children.Add(l);
                    sp.Children.Add(new Label() { Content = parts[0] });
                    sp.Children.Add(textBox);
                    if(parts.Count() > 1) sp.Children.Add(new Label() { Content = parts[1] });
                    //sp.Margin = new Thickness(-4, -5, 0, -5);
                    tb.Content = sp;
                    AnswerPanel.Children.Add(tb);
                }
                else
                {
                    textBox.Margin = new Thickness(10, 0, 0, 0);
                    tb.Content = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, -1, 0, -5) };
                    if (l != null)
                        (tb.Content as StackPanel).Children.Add(l);
                    (tb.Content as StackPanel).Children.Add(new Label() { Content = content });
                    AnswerPanel.Children.Add(tb);
                    AnswerPanel.Children.Add(textBox);
                }
            }
            else if (answer.Name.ToLower().EndsWith("#false"))
            {
                tb.Margin = new Thickness(0, -2, 0, 2);
                tb.Content = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, -1, 0, -5) };
                if (l != null)
                {
                    //pic.Margin = new Thickness(0, -5, 0, -5);
                    (tb.Content as StackPanel).Children.Add(l);
                }
                //(tb.Content as StackPanel).Children.Add(new Label() { Content = content });
                AnswerPanel.Children.Add(tb);
            }
            else
            {
                tb.Content = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, -1, 0, -5) };
                if (l != null)
                    (tb.Content as StackPanel).Children.Add(l);
                (tb.Content as StackPanel).Children.Add(new Label() { Content = content });
                AnswerPanel.Children.Add(tb);
            }

            if (answer.Question.QuestionType.ID == 2 || (answer.Question.QuestionType.ID == 1 && answer.Name.Contains("#single")))
                (tb as RadioButton).GroupName = answer.Question.Name;
            if (answer.Question.QuestionType.ID == 5 && answer.Name.Contains("#"))
                (tb as RadioButton).GroupName = answer.Name.Substring(0, answer.Name.IndexOf("#"));
            tb.Unchecked += TB_Clicked;
            tb.Checked += TB_Clicked;
            if (answer.ImageFileName != null && answer.ImageFileName != "")
            {
                //NameScope.SetNameScope(this, new NameScope());
                var bmImg = CommonFunctions.GetBmpImageByFileName(answer.ImageFileName);
                var img = new Image() { Source = bmImg, HorizontalAlignment = HorizontalAlignment.Center, Name=answer.ImageFileName.Replace(".","000"), Height = 20 };
                img.MouseDown += new MouseButtonEventHandler(img_MouseDown);
                AnswerPanel.Children.Add(img);
            }
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
            if (textBox != null)
            {
                DigitsLabel = new Label()
                {
                    Name = "DigitsLabel",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Red),
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(0, -3, 0, -3),
                    Content = answer.IsTextBoxDigitsOnly ? "Somente números" : null,
                    Visibility = System.Windows.Visibility.Hidden
                };
                AnswerPanel.Children.Add(DigitsLabel);
            }
            if (textBox != null)
            {
                textBox.Background = (Brush)new BrushConverter().ConvertFrom("#F4F4F4");
                if(DigitsLabel != null)
                    DigitsLabel.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // System.Windows.Forms.InputLanguage.CurrentInputLanguage = GetInputLanguageByName("he");
        }

        void textBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            // System.Windows.Forms.InputLanguage.CurrentInputLanguage = GetInputLanguageByName("he");
        }

        public static System.Windows.Forms.InputLanguage GetInputLanguageByName(string inputName)
        {
            foreach (System.Windows.Forms.InputLanguage lang in System.Windows.Forms.InputLanguage.InstalledInputLanguages)
            {
                if (lang.Culture.EnglishName.ToLower().StartsWith(inputName))
                    return lang;
            }
            return null;
        }

        void AnswerControl_Loaded(object sender, RoutedEventArgs e)
        {
            Trigger t = new Trigger();
            style = new Style();
            style.TargetType = this.GetType();

            t.Property = AnswerControl.IsMouseOverProperty;
            t.Value = true;
            
            if (!qc.IsExtraQuestion)
            {
                var win = ((((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
                brush = win.Module.ModuleType.Name.Contains("אנמנזה") ? (Brush)new BrushConverter().ConvertFrom("#b2d9de") : (Brush)new BrushConverter().ConvertFrom("#b5afcb");  
            }
            else
            {
                var win = (((((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as ContentControl).Parent as Grid).Parent as Border).Parent as StackPanel).Parent as ExtraQuestionWindow;
                brush = win.IsPhysicalExam ? (Brush)new BrushConverter().ConvertFrom("#b5afcb") : (Brush)new BrushConverter().ConvertFrom("#b2d9de");
                Width = (((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).ActualWidth;
            }
            Setter setter = new Setter(AnswerControl.BackgroundProperty, brush);
            t.Setters.Add(setter);
            style.Triggers.Add(t);
            this.Style = style;
        }

        void tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                //if (qc.Question.QuestionType.Name == "שלילה אקטיבית")
                //if(Keyboard.FocusedElement is AnswerControl
                  //  ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                e.Handled = true;
            }
            else if (e.Key == Key.RightCtrl && sender is ToggleButton)
            {
                System.Windows.Forms.SendKeys.SendWait(" ");
                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.Left))
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                e.Handled = true;
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.OemPlus)
            {
                textBox.Text += e.Key == Key.OemMinus ? "-" : "+";
                textBox.CaretIndex = textBox.Text.Length;
                e.Handled = true;
            }
        }

        void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
                var tmp = (sender as TextBox).CaretIndex;
                if ((sender as TextBox).SelectionLength > 0)
                    (sender as TextBox).Text = (sender as TextBox).Text.Remove((sender as TextBox).SelectionStart, (sender as TextBox).SelectionLength);

                (sender as TextBox).CaretIndex = tmp;
                (sender as TextBox).Text = (sender as TextBox).Text.Insert((sender as TextBox).CaretIndex, e.Text);
                (sender as TextBox).CaretIndex = tmp + 1;
                
            }
        }

        void  img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new QxMessageBox(new BitmapImage(new Uri(CommonFunctions.ImageNativePath + (sender as Image).Name.Replace("000", "."))),this).Show();
        }

        void InserTextFunc()
        {
            textBox.Text = btb.FinelText;
        }

        void test()
        {
            while (!btb.FinishFlag) ;
            var insertText = new InsertText(InserTextFunc);
            Dispatcher.Invoke(insertText, DispatcherPriority.Normal);
        }

        void textBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent is ContentControl))
            {
                var win = ((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as UIElement);
                var p =textBox.TranslatePoint(new Point(), win);
                btb = new BigTextBox(textBox.Text, win.PointToScreen(new Point(p.X,p.Y)));
                btb.Show();
                var thread = new Thread(test);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else
            {
                var win = ((((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
                var p =textBox.TranslatePoint(new Point(), win);
                btb = new BigTextBox(textBox.Text, new Point(win.Left + p.X, win.Top + p.Y));
                btb.Show();
                var thread = new Thread(test);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            
        }

        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox.Text.Length >= 23 && e.Key != Key.Enter)
                textBox_MouseDoubleClick(null, null);
            if (e.Key == Key.Down) Down(e);
            if (e.Key == Key.Up) Up(e);
        }

        [STAThread]
        private void Down(KeyEventArgs e)
        {
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Down);

            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

            if (elementWithFocus != null)
            {
                elementWithFocus.MoveFocus(request);
                if (e!=null) e.Handled = true;
            }
        }

        [STAThread]
        private void Up(KeyEventArgs e)
        {
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Up);

            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

            if (elementWithFocus != null)
            {
                elementWithFocus.MoveFocus(request);
                if (e != null) e.Handled = true;
            }
        }

        protected void TB_Clicked(object sender, EventArgs e)
        {
            Answer.TimeStamp = DateTime.Now;
            ModuleWindow win = null;
            if(!qc.IsExtraQuestion)
                win = ((((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
            if (Answer.ExtraQuestion != null)
            {
                if ((sender as ToggleButton).IsChecked ?? false)
                {
                    //Background = win.Module.ModuleType.Name.Contains("אנמנזה") ? (Brush)new BrushConverter().ConvertFrom("#b5afcb") : (Brush)new BrushConverter().ConvertFrom("#b2d9de");
                    //Background.Opacity = 0.3;

                    (tb.Content as StackPanel).Background = win.Module.ModuleType.Name.Contains("אנמנזה") ? (Brush)new BrushConverter().ConvertFrom("#b5afcb") : (Brush)new BrushConverter().ConvertFrom("#b2d9de");
                    (tb.Content as StackPanel).Background.Opacity = 0.3;

                    var originalQuestionControl = (((this.Parent as Grid).Parent as Grid).Parent as QuestionControl);
                    var point = this.TranslatePoint(new Point(), win);
                    var qc2 = new QuestionControl(Answer.ExtraQuestion, true, Answer.Name, Answer.ID, Answer.AnswerHebText);
                    win.extraQuestionControls.Add(qc2);
                    var newWin = new ExtraQuestionWindow(qc2, new Point(win.Left + win.ActualWidth, win.Top + point.Y + this.ActualHeight/2),win.Module.ModuleType.Name.Contains("בדיקה גופנית"));
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
                        expander.PreviewKeyDown += new KeyEventHandler(expander_PreviewKeyDown);
                        expander.ExpandDirection = ExpandDirection.Right;
                        expander.Margin = new Thickness(0, 0, 0, -8);
                        expander.Expanded += new RoutedEventHandler(expander_Expanded);
                        AnswerPanel.Children.Add(expander);
                    }
                    //Background = new SolidColorBrush(Colors.Transparent);
                    (tb.Content as StackPanel).Background = new SolidColorBrush(Colors.Transparent);

                    new Thread(SendNextElementThread).Start();

                }
                else
                {
                    win.extraQuestionControls.RemoveAll(q => q.Question.Equals(Answer.ExtraQuestion) && q.SendingAnswerName.Contains(Answer.Name));
                    Background = new SolidColorBrush(Colors.Transparent);
                    if(DigitsLabel != null)
                        DigitsLabel.Visibility = System.Windows.Visibility.Hidden;
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
                    WarningLabel.Content = con.ConditionHebText;
                }
            }
            else
            {
                if (Answer.IsContainsTextBox)
                {
                    textBox.Background = (Brush)new BrushConverter().ConvertFrom("#F4F4F4");
                    DigitsLabel.Visibility = System.Windows.Visibility.Hidden;
                    textBox.Text = "";
                    textBox.IsEnabled = false;
                }
                Background = new SolidColorBrush(Colors.Transparent);
                if (WarningLabel != null) WarningLabel.Content = null;
            }
            if(win != null && win.ShowErrors)
                win.CheckErrors();

            //this.Style = style;

            //Keyboard.Focus(this.tb);
            //this.tb.Focus();
            //this.MouseEnter += UserControl_MouseEnter;
            //this.MouseLeave += UserControl_MouseLeave;
        }

        private void SendNextElementThread()
        {
            Thread.Sleep(100);
            var send = new SendNextElement(SendNextElementFunc);
            Dispatcher.Invoke(send);
        }

        private void SendNextElementFunc()
        {
            System.Windows.Forms.SendKeys.Send("{TAB}");
        }

        void expander_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var win = ((((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
                win.NextButton_Click(null, null);
                e.Handled = true;
            }
        }

        void expander_Expanded(object sender, RoutedEventArgs e)
        {
            var win = ((((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;

            expander.IsExpanded = false;

            Background = win.Module.ModuleType.Name.Contains("אנמנזה") ? (Brush)new BrushConverter().ConvertFrom("#b2d9de") : (Brush)new BrushConverter().ConvertFrom("#b5afcb");
            Background.Opacity = 0.3;
            var originalQuestionControl = (((this.Parent as Grid).Parent as Grid).Parent as QuestionControl);
            var point = originalQuestionControl.TranslatePoint(new Point(), win);
            //var qc = new QuestionControl(Answer.ExtraQuestion, true);
            var newWin = new ExtraQuestionWindow(win.extraQuestionControls.Where(qc => qc.Question == Answer.ExtraQuestion).ToList().FirstOrDefault(), new Point(win.Left + win.ActualWidth, win.Top + point.Y + originalQuestionControl.ActualHeight / 2),win.Module.ModuleType.Name.Contains("בדיקה גופנית"));
            var result = newWin.ShowDialog();
            Background = new SolidColorBrush(Colors.Transparent);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e != null)
                e.Handled = true;

            if (tb.IsChecked ?? false)
            {
                if (Answer.IsTextBoxDigitsOnly)
                {
                    if (tb.IsChecked.Value && textBox.Text == "")
                    {
                        textBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                        DigitsLabel.Visibility = System.Windows.Visibility.Visible;
                        //(textBox.ToolTip as ToolTip).IsOpen = true;
                        return;
                    }
                    decimal num;
                    if (Decimal.TryParse(textBox.Text, out num) || textBox.Text == "")
                    {
                        textBox.Background = (Brush)new BrushConverter().ConvertFrom("#F4F4F4");
                        DigitsLabel.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        textBox.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                        DigitsLabel.Visibility = System.Windows.Visibility.Visible;
                        //(textBox.ToolTip as ToolTip).IsOpen = true;
                        return;
                    }
                }
                else
                {
                    textBox.ToolTip = null;
                }

                textBox.Background = (Brush)new BrushConverter().ConvertFrom("#F4F4F4");
                DigitsLabel.Visibility = System.Windows.Visibility.Hidden;
                var conditions = Answer.WarningConditions.Where(c => c.ConditionType != 'V' && c.ConditionType != 'X').ToList();
                if (conditions.Count > 0)
                    foreach (var con in conditions)
                        if (IsCondition(con))
                        {
                            textBox.Background = new SolidColorBrush(con.Color.GetColor()) { Opacity = 0.3 };
                            //DigitsLabel.Visibility = System.Windows.Visibility.Visible;
                            //textBox.ToolTip = new ToolTip() { Content = ContentDictionary.GetContent("DigitsOnly", Session.Lang) };
                        }
            }

            if (!qc.IsExtraQuestion)
            {
                var questionControlElement = (((this.Parent as Grid).Parent as Grid).Parent as QuestionControl);
                if(questionControlElement.Parent == null)
                    return;
                var win = (((questionControlElement.Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
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

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tb.IsChecked = !tb.IsChecked;
        }

        private void AnswerPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isMouse)
            {
                isMouse = false;
                return;
            }
            if(((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent is ContentControl))
            {
                var win = ((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as UIElement);
                var p =(sender as StackPanel).TranslatePoint(new Point(), win);
                var point = win.PointToScreen(new Point(p.X,p.Y));
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int) point.X - 3,(int) point.Y);
            }
            else
            {
                var win = ((((((this.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;
                var p = (sender as StackPanel).TranslatePoint(new Point(), win);
                var point = win.PointToScreen(new Point(p.X, p.Y));
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)point.X - 3, (int)point.Y);
            }
        }

        private void AnswerPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouse = true;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Brush tmp = Background;
            Background = brush;
            brush = tmp;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Brush tmp = Background;
            Background = brush;
            brush = tmp;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.Up)
            {
                this.Up(e);
                e.Handled = true;
            }*/
        }
    }
}
