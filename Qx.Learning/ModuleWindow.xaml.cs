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
using System.Windows.Forms;
using Frameworks;

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for ModuleWindow.xaml
    /// </summary>
    public partial class ModuleWindow : Window
    {
        public Module Module;
        public List<QuestionControl> questionControls = new List<QuestionControl>();
        public List<QuestionControl> extraQuestionControls = new List<QuestionControl>();
        public List<Combination> Combinations = new List<Combination>();
        List<List<QuestionInModule>> Pages = new List<List<QuestionInModule>>();
        public IList<DoctorAnswer> ModuleAnswers;
        int currentPage = 0;
        public bool ShowErrors = false;
        List<DoctorAnswer> WrongAnswers; 
        List<DoctorAnswer> ExpectedAnswers;
        List<DoctorAnswer> ChoosedAnswers;
        bool isFeedback;
        int mainOrder = 0;

        public ModuleWindow(Module module)
        {
            InitializeComponent();
            Module = module;
            HeaderLabelEnmnesia.Content = HeaderLabelPhysicalEx.Content = module.ModuleType.Name + " | " + ContentDictionary.GetContent(module.Name, Session.Lang);
            if(module.ModuleType.Name.Contains("אנמנזה"))
                EnmnesiaHeader.Visibility = System.Windows.Visibility.Visible;
            else
                PhysicalExHeader.Visibility = System.Windows.Visibility.Visible;
            Combinations.AddRange(module.Combinations.Where(c => !c.IsDeleted));
            var Questions = module.Questions.OrderBy(q => q.Ordering).ToList();
            SetPages(Questions);
            this.KeyDown += new System.Windows.Input.KeyEventHandler(ModuleWindow_KeyDown);
            isFeedback = false;
        }

        public ModuleWindow(List<Module> PhysicalEx)
        {
            InitializeComponent();
            HeaderLabelPhysicalEx.Content = "בדיקות גופניות" + " - " + (Session.LastModule == null ? "" : ContentDictionary.GetContent(Session.LastModule.Name, Session.Lang));
            PhysicalExHeader.Visibility = System.Windows.Visibility.Visible;
            var Questions = new List<QuestionInModule>();
            Questions.AddRange(Session.permanentQuestions);
            foreach (var mod in PhysicalEx)
            {
                //mod.Questions[mod.Questions.Count - 1].IsPageBreak = true;
                Questions.AddRange(mod.Questions.Where(qu => !Questions.Select(qq => qq.Question).Contains(qu.Question)).OrderBy(q => q.Ordering));
                Combinations.AddRange(mod.Combinations.Where(c => !c.IsDeleted));
            }
            SetPages(Questions);
            this.KeyDown += new System.Windows.Input.KeyEventHandler(ModuleWindow_KeyDown);
            Session.LastModule = null;
            Module = new Module() { ModuleType = PhysicalEx[0].ModuleType };
            isFeedback = false;
        }

        public ModuleWindow(Module module, List<DoctorAnswer> wrongAnswers, List<DoctorAnswer> choosedAnswers, List<DoctorAnswer> expectedAnswers, bool isAnamnesis)
        {
            InitializeComponent();
            Module = module;
            HeaderLabelEnmnesia.Content = HeaderLabelPhysicalEx.Content = module.ModuleType.Name + " | " + ContentDictionary.GetContent(module.Name, Session.Lang);
            if (module.ModuleType.Name.Contains("אנמנזה"))
                EnmnesiaHeader.Visibility = System.Windows.Visibility.Visible;
            else
                PhysicalExHeader.Visibility = System.Windows.Visibility.Visible;
            var Questions = module.Questions.OrderBy(q => q.Ordering).ToList();
            WrongAnswers = wrongAnswers;
            ExpectedAnswers = expectedAnswers;
            ChoosedAnswers = choosedAnswers;
            isFeedback = true;
            SetPages(Questions);
            //QuestionsArea.IsEnabled = false;
        }

        void ModuleWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.Right)
                SendKeys.SendWait(" ");
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.J))
                System.Windows.MessageBox.Show(GenerateText(true));
        }

        private void SetPages(List<QuestionInModule> Questions)
        {    
            QuestionInModule question;
            bool isFull = false;
            do
            {
                isFull = false;
                var page = new List<QuestionInModule>();
                do
                {
                    question = Questions.FirstOrDefault();
                    if (question == null)
                    {
                        isFull = true;
                        continue;
                    }
                    if (page.Sum(p => p.Question.CalculatedHight) + question.Question.CalculatedHight < 535)
                    {
                        page.Add(question);
                        Questions.Remove(question);
                    }
                    else
                        isFull = true;
                } while (!isFull && !question.IsPageBreak);

                Pages.Add(page);
                currentPage++;
            } while (Questions.Count > 0);
            currentPage = 0;
            BuildCurrentPage();
            if (Pages.Count == 1)
            {
                if (!isFeedback)
                {
                    NextButton.Visibility = System.Windows.Visibility.Hidden;
                    MaleButton.Visibility = System.Windows.Visibility.Visible;
                    FemaleButton.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    NextButton.Visibility = System.Windows.Visibility.Hidden;
                    FinishButton.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void BuildCurrentPage()
        {
            QuestionControl qc;
            QuestionsArea.Children.Clear();
            foreach(var question in Pages[currentPage])
            {
                /*if(isFeedback && )
                    qc = new QuestionControl(question.Question, false, ) { Name = "Q" + currentPage.ToString() + question.Ordering.ToString() };
                else*/
                qc = new QuestionControl(question.Question) { Name = "Q" + mainOrder.ToString() };
                #region FEEDBACK
                if (isFeedback)
                {
                    foreach (var ac in qc.AnswersStackPanel.Children.OfType<AnswerControl>())
                    {
                        ac.tb.IsEnabled = false;
                        var ans = ChoosedAnswers.Where(a => a.AnswerID == ac.Answer.ID).FirstOrDefault();
                        if (ans != null)
                        {
                            ac.tb.IsChecked = true;
                            if (ac.textBox != null)
                                ac.textBox.Text = ans.Text;
                            if (WrongAnswers.Select(da => da.AnswerID).Contains(ac.Answer.ID))
                            {
                                ac.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
                                var expected = ExpectedAnswers.Where(e => e.AnswerID == ac.Answer.ID).FirstOrDefault();
                                if(expected != null)
                                {
                                    ac.WarningLabel = new System.Windows.Controls.Label()
                                    {
                                        Name = "WarningLabel",
                                        FontWeight = FontWeights.Bold,
                                        Foreground = new SolidColorBrush(Colors.GreenYellow),
                                        FontStyle = FontStyles.Italic,
                                        Margin = new Thickness(20, -3, 0, -3),
                                        Content = "תשובה נכונה: " + expected.Text
                                    };
                                    ac.AnswerPanel.Children.Add(ac.WarningLabel);
                                }
                            }
                            else
                                ac.Background = new SolidColorBrush(Colors.GreenYellow) { Opacity = 0.3 };
                        }
                        else 
                        {
                            var wrong = WrongAnswers.Where(w => w.AnswerID == ac.Answer.ID).FirstOrDefault();
                            if (wrong != null)
                            {
                                ac.Background = new SolidColorBrush(Colors.GreenYellow) { Opacity = 0.3 };
                                if (ac.textBox != null)
                                    ac.textBox.Text = wrong.Text;
                            }
                        }
                    }
                }
                #endregion
                QuestionsArea.Children.Add(qc);
                questionControls.Add(qc);
                mainOrder++;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ShowErrors = true;
            var hasErrors = false;
            foreach (var qc in questionControls.Where(q => Pages[currentPage].Select(qim => qim.Question).Contains(q.Question)))
                if (!qc.IsValid)
                {
                    qc.BorderBrush = new SolidColorBrush(Colors.Red);
                    hasErrors = true;
                }
                else
                    qc.BorderBrush = new SolidColorBrush(Colors.Transparent);

            if (hasErrors) return;
                    
            currentPage++;
            if (currentPage + 1 == Pages.Count)
            {
                if (!isFeedback)
                {
                    NextButton.Visibility = System.Windows.Visibility.Hidden;
                    MaleButton.Visibility = (Module.IsMale ?? true) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                    FemaleButton.Visibility = (!Module.IsMale ?? true) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                }
                else
                {
                    NextButton.Visibility = System.Windows.Visibility.Hidden;
                    FinishButton.Visibility = System.Windows.Visibility.Visible;
                }
            }
            if (currentPage == 1)
                BackButton.Visibility = System.Windows.Visibility.Visible;

            if (questionControls.Where(qc => qc.Question == Pages[currentPage].First().Question).Count() > 0)
            {
                QuestionsArea.Children.Clear();
                foreach (var qc in questionControls.Where(q => Pages[currentPage].Select(qim => qim.Question).Contains(q.Question)).ToList())
                {
                    QuestionsArea.Children.Add(qc);
                }
            }
            else
                BuildCurrentPage();
            ShowErrors = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (--currentPage == 0)
                BackButton.Visibility = System.Windows.Visibility.Hidden;
            if (currentPage + 1 != Pages.Count)
            {
                if (!isFeedback)
                {
                    NextButton.Visibility = System.Windows.Visibility.Visible;
                    MaleButton.Visibility = System.Windows.Visibility.Hidden;
                    FemaleButton.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    NextButton.Visibility = System.Windows.Visibility.Visible;
                    FinishButton.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            QuestionsArea.Children.Clear();
            foreach (var qc in questionControls.Where(q => Pages[currentPage].Select(qim => qim.Question).Contains(q.Question)).ToList())
            {
                QuestionsArea.Children.Add(qc);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            ShowErrors = true;
            var hasErrors = false;
            foreach (var qc in questionControls.Where(q => Pages[currentPage].Select(qim => qim.Question).Contains(q.Question)))
                if (!qc.IsValid)
                {
                    qc.BorderBrush = new SolidColorBrush(Colors.Red);
                    hasErrors = true;
                }
                else
                    qc.BorderBrush = new SolidColorBrush(Colors.Transparent);

            if (hasErrors) return;

            ShowErrors = false;
            DialogResult = true;
            var output = GenerateText(!(sender as System.Windows.Controls.Image).Name.Contains("Female"));
            Close();

            Session.LastResult = output;
        }

        private string GenerateText(bool IsMale)
        {
            var output = "";

            #region TAKE ALL MODULE ANSWERS
            var history = new History() { Module = Module, User = Session.User };
            var answers = new List<AnswerControl>();
            foreach (var question in questionControls)
            {
                answers.AddRange(question.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ac => ac.tb.IsChecked.Value));
                foreach (var a in question.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ac => ac.tb.IsChecked.Value))
                    history.DoctorAnswers.Add(new DoctorAnswer(a.Answer.TimeStamp, a.Answer.ID, (a.textBox ?? new System.Windows.Controls.TextBox()).Text,a.Answer));
            }
            foreach (var question in extraQuestionControls)
            {
                foreach (var a in question.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ac => ac.tb.IsChecked.Value))
                    history.DoctorAnswers.Add(new DoctorAnswer(a.Answer.TimeStamp,  a.Answer.ID, (a.textBox ?? new System.Windows.Controls.TextBox()).Text, a.Answer, question.SendingAnswerID));
            }
            ModuleAnswers = history.DoctorAnswers;
            Session.LastAnswers = history.DoctorAnswers.ToList();
            #endregion
            #region CHECK ALL MODULE COMBINATIONS
            foreach (var com in Combinations)
                if (IsCombination(com, answers))
                {
                    com.IsExisting = true;
                    var ansInCom = com.CombinatedAnswers.Select(ca => ca.Answer);
                    foreach (var qc in answers.Where(a => ansInCom.Contains(a.Answer)).Select(ac => ac.qc))
                        qc.IsCombinated = true;
                }
            #endregion
            #region FOR ALL QUESTIONS
            foreach (var qc in questionControls.OrderBy(qc => Int32.Parse(qc.Name.Substring(1))))
            {
                #region INSERT TO OUTPUT MODULE'S COMBINATIONS
                var combs = Combinations.Where(c => c.IsExisting && c.Order < Decimal.Parse(qc.Name.Substring(1)) && c.Order > Decimal.Parse(qc.Name.Substring(1)) - 1);
                foreach (var comb in combs)
                    output += " " + (IsMale ? comb.ResultMaleHebText : (comb.ResultFemaleHebText.Contains("לא נמצא במילון") ? comb.ResultMaleHebText : comb.ResultFemaleHebText));
                #endregion
                if (qc.IsCombinated) continue;
                var questionAnswers = qc.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ac => ac.tb.IsChecked ?? false).ToList();
                #region CHECK QUESTION COMBINATIONS
                foreach (var com in qc.Question.Combinations.Where(c => !c.IsDeleted))
                {
                    if (IsCombination(com, questionAnswers))
                    {
                        com.IsExisting = true;
                        qc.HasCombinations = true;
                    }
                }
                #endregion
                #region INSERT TO OUTPUT QUESTION ITSELF
                if (qc.HasCombinations)
                {
                    foreach (var comb in qc.Question.Combinations.Where(c => c.IsExisting))
                        output += " " + (IsMale ? comb.ResultMaleHebText : (comb.ResultFemaleHebText.Contains("לא נמצא במילון") ? comb.ResultMaleHebText : comb.ResultFemaleHebText));
                    output += qc.Question.IsWithoutEndingChar ? "" : qc.Question.EndingChar.ToString();
                    if (qc.Question.IsEnter)
                        output += "\r\n";
                }
                else
                {
                    if (qc.Question.PreQuestionHebText != "" && !qc.Question.PreQuestionHebText.Contains("לא נמצא במילון") && questionAnswers.Where(a => !a.Answer.IsSingular).Count() > 0)
                        output += " " + (IsMale 
                                            ? 
                                            qc.Question.PreQuestionHebText 
                                            :
                                                (qc.Question.PreQuestionHebTextFemale.Contains("לא נמצא במילון") || qc.Question.PreQuestionHebTextFemale == ""
                                                   ? 
                                                   qc.Question.PreQuestionHebText 
                                                   : 
                                                   qc.Question.PreQuestionHebTextFemale)
                                        );

                    if (qc.Question.QuestionType.Name == "שאלה פתוחה")
                    {
                        output += qc.OpenQuestionTextBox.Text;
                        output += qc.Question.IsWithoutEndingChar ? "" : (qc.Question.EndingChar.ToString() == "\0" ? "" : qc.Question.EndingChar.ToString());
                        if (qc.Question.IsEnter)
                            output += "\r\n";
                        continue;
                    }
                    foreach (var ans in questionAnswers)
                    {
                        if(ans.Answer.RecomendedPhysicalEx != null) Session.RecomendedPhysicalEx.Add(ans.Answer.RecomendedPhysicalEx);
                        if (ans.Answer.ExtraQuestion != null)
                        {
                            var extraQuestionControl = extraQuestionControls.Where(q => q.SendingAnswerName == ans.Answer.Name).FirstOrDefault();
                            foreach (var com in extraQuestionControl.Question.Combinations.Where(c => !c.IsDeleted))
                            {
                                #region SEARCH COMBS IN EXTRA QUESTION
                                if (IsCombination(com, questionAnswers))
                                {
                                    com.IsExisting = true;
                                    extraQuestionControl.HasCombinations = true;
                                }
                                #endregion
                            }
                            if (extraQuestionControl.HasCombinations)
                            {
                                #region WRITE COMBS OF EXTRA QUESTION
                                foreach (var comb in extraQuestionControl.Question.Combinations.Where(c => c.IsExisting))
                                    output += " " + (IsMale ? comb.ResultMaleHebText : (comb.ResultFemaleHebText.Contains("לא נמצא במילון") ? comb.ResultMaleHebText : comb.ResultFemaleHebText));
                                output += extraQuestionControl.Question.IsWithoutEndingChar ? "" : (extraQuestionControl.Question.EndingChar.ToString() == "\0" ? "" : extraQuestionControl.Question.EndingChar.ToString());

                                if (extraQuestionControl.Question.IsEnter)
                                    output += "\r\n";
                                #endregion
                            }
                            else
                            {
                                if (extraQuestionControl.Question.QuestionType.Name == "שאלה פתוחה")
                                {
                                    output += extraQuestionControl.OpenQuestionTextBox.Text;
                                    output += extraQuestionControl.Question.IsWithoutEndingChar ? "" : (extraQuestionControl.Question.EndingChar.ToString() == "\0" ? "" : extraQuestionControl.Question.EndingChar.ToString());
                                    if (extraQuestionControl.Question.IsEnter)
                                        output += "\r\n";
                                    continue;
                                }
                                output += " " + GetText(IsMale, ans);
                                var checkedExtraAnswers = extraQuestionControl.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ansCon => ansCon.tb.IsChecked ?? false).ToList();
                                foreach (var a in checkedExtraAnswers)
                                {
                                    if (a.Answer.RecomendedPhysicalEx != null) Session.RecomendedPhysicalEx.Add(a.Answer.RecomendedPhysicalEx);
                                    if (checkedExtraAnswers.Count >= 2 && checkedExtraAnswers[checkedExtraAnswers.Count - 2] == a)
                                        output += " " + GetText(IsMale, a) + (a.Answer.Question.QuestionType.Name.Contains("בחירה מרובה") ? " ו" : "");
                                    else
                                        output += " " + GetText(IsMale, a) + (a.Answer.Question.QuestionType.Name.Contains("בחירה מרובה") ? "," : "");
                                }

                                #region POINT AT END OF QUESTION - REPLACE ,
                                if (output.Length > 0 && checkedExtraAnswers.Count() > 0)
                                {
                                    if (output[output.Length - 1] == ',')
                                        output = output.Substring(0, output.Length - 1) + (extraQuestionControl.Question.IsWithoutEndingChar ? "" : (extraQuestionControl.Question.EndingChar.ToString() == "\0" ? "" : extraQuestionControl.Question.EndingChar.ToString()));
                                    else
                                        output += extraQuestionControl.Question.IsWithoutEndingChar ? "" : (extraQuestionControl.Question.EndingChar.ToString() == "\0" ? "" : extraQuestionControl.Question.EndingChar.ToString());

                                    if (extraQuestionControl.Question.IsEnter)
                                        output += "\r\n";
                                }
                                #endregion
                            }

                            if (questionAnswers.Count >=2 && questionAnswers[questionAnswers.Count - 2] == ans && ans.Answer.Question.QuestionType.Name.Contains("בחירה מרובה"))
                                output += " ו";
                        }
                        else //if (GetText(IsMale, ans) != "")
                        {
                            if (questionAnswers.Count >= 2 && questionAnswers[questionAnswers.Count - 2] == ans)
                                output += " " + GetText(IsMale, ans) + (ans.Answer.Question.QuestionType.Name.Contains("בחירה מרובה") ? " ו" : "");
                            else
                                output += " " + GetText(IsMale, ans) + (ans.Answer.Question.QuestionType.Name.Contains("בחירה מרובה") ? "," : "");

                        }
                    }
                    #region POINT AT END OF QUESTION - REPLACE ,
                    if (output.Length > 0 && questionAnswers.Count > 0)
                    {
                        if (output[output.Length - 1] == ',')
                            output = output.Substring(0, output.Length - 1) + (qc.Question.IsWithoutEndingChar ? "" : (qc.Question.EndingChar.ToString() == "\0" ? "" : qc.Question.EndingChar.ToString()));
                        else
                            output += qc.Question.IsWithoutEndingChar ? "" : (qc.Question.EndingChar.ToString() == "\0" ? "" : qc.Question.EndingChar.ToString());

                        if (qc.Question.IsEnter)
                            output += "\r\n";
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            return output.Trim().Replace(" ו  ", " ו").Replace(" ו ", " ו").Replace(" ב  ", " ב").Replace("\r\n.", "\r\n").Replace(" . ", " ").Replace("..", ".").Replace(". .", ".");
        }

        private string GetText(bool IsMale, AnswerControl ans)
        {
            var text = (IsMale ? ans.Answer.ResultMaleHebText :
                        (ans.Answer.ResultFemaleHebText.Contains("לא נמצא במילון") ?
                         ans.Answer.ResultMaleHebText : ans.Answer.ResultFemaleHebText))
                        .Replace("{text}", (ans.textBox ?? new System.Windows.Controls.TextBox()).Text);
            return text.Contains("לא נמצא במילון") ? "" : text;
        }

        private bool IsCombination(Combination combination, IList<AnswerControl> ac)
        {
            var result = true;
            foreach (var ca in combination.CombinatedAnswers)
            {
                if (ca.IsNot)
                {
                    if (ac.Select(a => a.Answer).Contains(ca.Answer))
                        result = false;
                }
                else
                {
                    if (!ac.Select(a => a.Answer).Contains(ca.Answer))
                        result = false;
                }
            }
            return result;
        }

        private void TextPreview_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(GenerateText(true));
        }

        public void CheckErrors()
        {
            foreach (var qc in questionControls.Where(q => Pages[currentPage].Select(qim => qim.Question).Contains(q.Question)))
                if (!qc.IsValid)
                    qc.BorderBrush = new SolidColorBrush(Colors.Red);
                else
                    qc.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void FinishButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
