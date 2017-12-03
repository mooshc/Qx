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
using System.Threading;
using System.IO;
using Qx.Client.LocalEntities;
using System.Configuration;
using Newtonsoft.Json;

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for ModuleWindow.xaml
    /// </summary>
    public partial class ModuleWindow : Window
    {
        delegate void SendNextElement();
        public Module Module;
        public List<QuestionControl> questionControls = new List<QuestionControl>();
        public List<QuestionControl> extraQuestionControls = new List<QuestionControl>();
        public List<Combination> Combinations = new List<Combination>();
        List<List<QuestionInModule>> Pages = new List<List<QuestionInModule>>();
        private FinishUserControl finsihControl;
        LocalHistory history;
        int currentPage = 0;
        public bool ShowErrors = false;
        public int mainOrder = 0;
        private string caseId;

        private BitmapImage next = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "Next.png"));
        private BitmapImage nextHover = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "NextHover.png")); 
        private BitmapImage back = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "Back.png"));
        private BitmapImage backHover = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "BackHover.png"));

        public ModuleWindow(Module module, string caseId)
        {
            InitializeComponent();
            Left = Session.windowPosition.X;
            Top = Session.windowPosition.Y;
            Module = module;
            HeaderLabelEnmnesia.Content = HeaderLabelPhysicalEx.Content = module.ModuleType.Name + " | " + module.ModuleHebText;
            if(module.ModuleType.Name.Contains("אנמנזה"))
                EnmnesiaHeader.Visibility = System.Windows.Visibility.Visible;
            else
                PhysicalExHeader.Visibility = System.Windows.Visibility.Visible;
            Combinations.AddRange(module.Combinations.Where(c => !c.IsDeleted));
            var Questions = module.Questions.OrderBy(q => q.Ordering).ToList();
            SetPages(Questions);
            this.KeyDown += new System.Windows.Input.KeyEventHandler(ModuleWindow_KeyDown);
            Loaded += new RoutedEventHandler(ModuleWindow_Loaded);

            this.caseId = caseId;
        }

        void ModuleWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
            if (Keyboard.FocusedElement as UIElement == this && this.MoveFocus(request) && e != null)
            {
                e.Handled = true;
            }
        }

        private void MoveToNextUIElement(System.Windows.Input.KeyEventArgs e)
        {
            SendKeys.Send("{TAB}");
            if (e != null)
            {
                e.Handled = true;
            }
        }

        void ModuleWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.RightCtrl))
            {
                SendKeys.Send(" ");
                if (e != null)
                {
                    e.Handled = true;
                }
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                e.Handled = true;
            }
            if (Keyboard.IsKeyDown(Key.Down))
            {
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.J))
            {
                System.Windows.MessageBox.Show(this.GenerateText(true));
                return;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.K))
            {
                System.Windows.MessageBox.Show(this.GenerateText(false));
                return;
            }
            if (Keyboard.IsKeyDown(Key.Return) || Keyboard.IsKeyDown(Key.Next))
            {
                if(currentPage < Pages.Count -1)
                    this.NextButton_Click(null, null);
                return;
            }
            if (Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.Prior))
            {
                if(currentPage > 0)
                    this.BackButton_Click(null, null);
            }
        }

        public ModuleWindow(List<Module> PhysicalEx, string caseId)
        {
            InitializeComponent();
            Left = Session.windowPosition.X;
            Top = Session.windowPosition.Y;
            var Questions = new List<QuestionInModule>();
            var specialPhysEx = new List<int>() {34,35,40};
            history = new LocalHistory() { ModuleId = PhysicalEx.Select(m => m.ID).ToList(), MedicalCaseId = caseId, FileName = Session.fileName };
            if (PhysicalEx[0].ModuleType.ID == 2 && PhysicalEx.Exists(m => !specialPhysEx.Contains(m.ID)))
                Questions.AddRange(Session.permanentQuestions.Where(p => p.Ordering == 0));
            Module = new Module(PhysicalEx[0].ModuleType.ID) { ModuleType = PhysicalEx[0].ModuleType, Name = "" };
            var heb = "";
            foreach (var mod in PhysicalEx)
            {
                //mod.Questions[mod.Questions.Count - 1].IsPageBreak = true;
                Module.Name += "," + mod.Name;
                heb += ", " + mod.ModuleHebText;
                Questions.AddRange(mod.Questions.Where(qu => !Questions.Select(qq => qq.Question).Contains(qu.Question)).OrderBy(q => q.Ordering));
                Combinations.AddRange(mod.Combinations.Where(c => !c.IsDeleted));
                if (mod.IsMale != null)
                    Module.IsMale = (Module.IsMale ?? (mod.IsMale ?? true)) || (mod.IsMale ?? true);
            }
            if (PhysicalEx.Exists(m => !specialPhysEx.Contains(m.ID)))
            {
                if (PhysicalEx[0].ModuleType.ID == 1)
                    Questions.AddRange(Session.permanentQuestions.Where(p => p.Question.Name == "h_OtherRemarks"));
            }
            Questions.Add(new QuestionInModule(new Question(), 101, false));
            if(PhysicalEx.Count == 1)
                HeaderLabelEnmnesia.Content = HeaderLabelPhysicalEx.Content = (PhysicalEx[0].ModuleType.ID == 1 ? "Anamnese" : "בדיקה גופנית") + " | " + heb.Substring(2);
            else
                HeaderLabelEnmnesia.Content = HeaderLabelPhysicalEx.Content = (PhysicalEx[0].ModuleType.ID == 1 ? "אנמנזות" : "בדיקות גופניות") + " | " + heb.Substring(2) ;
            //if (Session.LastModule != null && PhysicalEx[0].ModuleType.ID == 2)
                //HeaderLabelPhysicalEx.Content = "בדיקות גופניות" + " | " + Session.LastModule.ModuleHebText;
            if(PhysicalEx[0].ModuleType.ID == 2)
                PhysicalExHeader.Visibility = System.Windows.Visibility.Visible;
            else
                EnmnesiaHeader.Visibility = System.Windows.Visibility.Visible;       
            SetPages(Questions);
            this.KeyDown += new System.Windows.Input.KeyEventHandler(ModuleWindow_KeyDown);
            Loaded += new RoutedEventHandler(ModuleWindow_Loaded);

            this.caseId = caseId;
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
                    if (page.Sum(p => p.Question.CalculatedHight) + (question.Ordering == 101 ? 100 : question.Question.CalculatedHight) < 530)
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
                NextButton.Visibility = System.Windows.Visibility.Hidden;
                /*if (Module.IsMale == null)
                {
                    MaleButton.Visibility = System.Windows.Visibility.Visible;
                    FemaleButton.Visibility = System.Windows.Visibility.Visible;
                }
                else if(Module.IsMale == true)
                    MaleButton.Visibility = System.Windows.Visibility.Visible;
                else
                    FemaleButton.Visibility = System.Windows.Visibility.Visible;
                 */
            }
        }

        private void BuildCurrentPage()
        {
            QuestionControl qc;
            QuestionsArea.Children.Clear();
            foreach(var question in Pages[currentPage])
            {
                if (question.Ordering == 101)
                {
                    finsihControl = new FinishUserControl(Module.ModuleType.ID == 2, new RoutedEventHandler(FinishButton_Click), Module.IsMale);
                    QuestionsArea.Children.Add(finsihControl);
                }
                else
                {
                    qc = new QuestionControl(question.Question) { Name = "Q" + mainOrder.ToString() };
                    QuestionsArea.Children.Add(qc);
                    questionControls.Add(qc);
                    mainOrder++;
                }
            }
        }

        public void NextButton_Click(object sender, RoutedEventArgs e)
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
                NextButton.Visibility = System.Windows.Visibility.Hidden;
                //MaleButton.Visibility = (Module.IsMale ?? true) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                //FemaleButton.Visibility = (!Module.IsMale ?? true) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
                if (currentPage + 1 == Pages.Count)
                    QuestionsArea.Children.Add(finsihControl);
            }
            else
                BuildCurrentPage();
            ShowErrors = false;
            new Thread(SendNextElementThread).Start();
        }

        private void SendNextElementThread()
        {
            Thread.Sleep(100);
            var send = new SendNextElement(SendNextElementFunc);
            Dispatcher.Invoke(send);
        }

        private void SendNextElementFunc()
        {
            SendKeys.Send("{TAB}");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (--currentPage == 0)
                BackButton.Visibility = System.Windows.Visibility.Hidden;
            if (currentPage + 1 != Pages.Count)
            {
                NextButton.Visibility = System.Windows.Visibility.Visible;
                //MaleButton.Visibility = System.Windows.Visibility.Hidden;
                //FemaleButton.Visibility = System.Windows.Visibility.Hidden;
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
            try
            {
                DragMove();
                Session.windowPosition.X = Left;
                Session.windowPosition.Y = Top;
            }
            catch
            {
            }
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

            var output = GenerateText(!(sender as System.Windows.Controls.Image).Name.Contains("Female"));
            try
            {
                System.Windows.Clipboard.Clear();
                System.Windows.Clipboard.SetDataObject(output);
            }
            catch
            {
                System.Windows.MessageBox.Show("המערכת נתקלה בבעיה בהעתקת התוצאה\nאנא נסו שוב.");
                    return;
            }
            Close();
            Thread.Sleep(500);
            SendKeys.SendWait("^V");
        }

        private void SaveHistory()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    bool shouldWorkLocally = ConfigurationManager.AppSettings["WorkLocally"].Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase);
                    bool shouldSaveHistory = ConfigurationManager.AppSettings["SaveHistory"].Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase);
                    string filePath = ConfigurationManager.AppSettings["HistoryFilePath"];
                    string fileName = Environment.MachineName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    string fileAddress = System.IO.Path.Combine(filePath, fileName);
                    if (shouldWorkLocally && shouldSaveHistory)
                    {
                        string historyJson = JsonConvert.SerializeObject(history, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                DefaultValueHandling = DefaultValueHandling.Ignore,
                                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                            });
                        historyJson = historyJson.Replace(")\\/", "").Replace("\\/Date(", "");

                        File.AppendAllLines(fileAddress, new[] { historyJson });
                        return;
                    }
                }
                catch (Exception ex)
                {
                    var stream = new StreamWriter("Log.txt", true);
                    stream.WriteLine(Environment.UserName + " -> " + Session.User.UserName + " -> " + DateTime.Now + ":>" + ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message) + "\n\n");
                    stream.Close();
                }
            }
        }

        private string GenerateText(bool IsMale)
        {
            var output = "";

            history.PatientGender = IsMale ? 'M' : 'F';

            #region TAKE ALL MODULE ANSWERS
            var answers = new List<AnswerControl>();
            foreach (var question in questionControls)
            {
                answers.AddRange(question.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ac => ac.tb.IsChecked.Value));
                foreach (var a in question.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ac => ac.tb.IsChecked.Value))
                    history.DoctorAnswers.Add(new LocalDoctorAnswer(a.Answer.TimeStamp, a.Answer.ID, a.textBox == null ? null : a.textBox.Text.Replace("'", "").Replace("`", "")));
            }
            foreach (var question in extraQuestionControls)
            {
                foreach (var a in question.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ac => ac.tb.IsChecked.Value))
                    history.DoctorAnswers.Add(new LocalDoctorAnswer(a.Answer.TimeStamp, a.Answer.ID, a.textBox == null ? null : a.textBox.Text.Replace("'", "").Replace("`", ""), question.SendingAnswerID));
            }
            new Thread(SaveHistory).Start();
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
                    output += qc.Question.IsWithoutEndingChar ? "" : (qc.Question.EndingChar ?? "").ToString();
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
                        output += qc.Question.IsWithoutEndingChar ? "" : ((qc.Question.EndingChar ?? "").ToString() == "\0" ? "" : (qc.Question.EndingChar ?? "").ToString());
                        if (qc.Question.IsEnter)
                            output += "\r\n";
                        continue;
                    }
                    foreach (var ans in questionAnswers)
                    {
                        if(ans.Answer.RecomendedPhysicalEx != null) Session.RecomendedPhysicalEx.Add(ans.Answer.RecomendedPhysicalEx);
                        #region EXTRA_QUESTION
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
                                output += extraQuestionControl.Question.IsWithoutEndingChar ? "" : ((extraQuestionControl.Question.EndingChar ?? "").ToString() == "\0" ? "" : (extraQuestionControl.Question.EndingChar ?? "").ToString());

                                if (extraQuestionControl.Question.IsEnter)
                                    output += "\r\n";
                                #endregion
                            }
                            else
                            {
                                if (extraQuestionControl.Question.QuestionType.Name == "שאלה פתוחה")
                                {
                                    output += extraQuestionControl.OpenQuestionTextBox.Text;
                                    output += extraQuestionControl.Question.IsWithoutEndingChar ? "" : ((extraQuestionControl.Question.EndingChar ?? "").ToString() == "\0" ? "" : (extraQuestionControl.Question.EndingChar ?? "").ToString());
                                    if (extraQuestionControl.Question.IsEnter)
                                        output += "\r\n";
                                    continue;
                                }
                                output += " " + GetText(IsMale, ans);
                                var checkedExtraAnswers = extraQuestionControl.AnswersStackPanel.Children.OfType<AnswerControl>().Where(ansCon => ansCon.tb.IsChecked ?? false).ToList();
                                if (extraQuestionControl.Question.PreQuestionHebText != "" && !extraQuestionControl.Question.PreQuestionHebText.Contains("לא נמצא במילון") && checkedExtraAnswers.Where(a => !a.Answer.IsSingular).Count() > 0)
                                    output += " " + (IsMale
                                                        ?
                                                        extraQuestionControl.Question.PreQuestionHebText
                                                        :
                                                            (extraQuestionControl.Question.PreQuestionHebTextFemale.Contains("לא נמצא במילון") || extraQuestionControl.Question.PreQuestionHebTextFemale == ""
                                                               ?
                                                               extraQuestionControl.Question.PreQuestionHebText
                                                               :
                                                               extraQuestionControl.Question.PreQuestionHebTextFemale)
                                                    );
                                
                                foreach (var a in checkedExtraAnswers)
                                {
                                    if (a.Answer.RecomendedPhysicalEx != null) Session.RecomendedPhysicalEx.Add(a.Answer.RecomendedPhysicalEx);
                                    if (checkedExtraAnswers.Count >= 2 && checkedExtraAnswers[checkedExtraAnswers.Count - 2] == a)
                                        output += " " + GetText(IsMale, a) + ((a.Answer.Question.QuestionType.ID ==1 || a.Answer.Question.QuestionType.ID ==5) ? " e" : "");
                                    else
                                        output += " " + GetText(IsMale, a) + ((a.Answer.Question.QuestionType.ID == 1 || a.Answer.Question.QuestionType.ID == 5) ? "," : "");
                                }

                                #region POINT AT END OF QUESTION - REPLACE ,
                                if (output.Length > 0 && checkedExtraAnswers.Count() > 0)
                                {
                                    if (output[output.Length - 1] == ',')
                                        output = output.Substring(0, output.Length - 1) + (extraQuestionControl.Question.IsWithoutEndingChar ? "" : ((extraQuestionControl.Question.EndingChar ?? "").ToString() == "\0" ? "" : (extraQuestionControl.Question.EndingChar ?? "").ToString()));
                                    else
                                        output += extraQuestionControl.Question.IsWithoutEndingChar ? "" : ((extraQuestionControl.Question.EndingChar ?? "").ToString() == "\0" ? "" : (extraQuestionControl.Question.EndingChar ?? "").ToString());

                                    if (extraQuestionControl.Question.IsEnter)
                                        output += "\r\n";
                                }
                                #endregion
                            }

                            if (questionAnswers.Count >= 2 && questionAnswers[questionAnswers.Count - 2] == ans && (ans.Answer.Question.QuestionType.ID == 1 || ans.Answer.Question.QuestionType.ID == 5))
                                output += " e";
                            else
                                output += ", ";
                        }
                        #endregion
                        else //if (GetText(IsMale, ans) != "")
                        {
                            if (questionAnswers.Count >= 2 && questionAnswers[questionAnswers.Count - 2] == ans)
                                output += " " + GetText(IsMale, ans) + ((ans.Answer.Question.QuestionType.ID == 1 || ans.Answer.Question.QuestionType.ID == 5) ? " e" : "");
                            else
                                output += " " + GetText(IsMale, ans) + ((ans.Answer.Question.QuestionType.ID ==1 || ans.Answer.Question.QuestionType.ID ==5) ? "," : "");

                        }
                    }
                    #region POINT AT END OF QUESTION - REPLACE ,
                    if (output.Length > 0 && questionAnswers.Count > 0)
                    {
                        if (output[output.Length - 1] == ',')
                            output = output.Substring(0, output.Length - 1) + (qc.Question.IsWithoutEndingChar ? "" : ((qc.Question.EndingChar ?? "").ToString() == "\0" ? "" : (qc.Question.EndingChar ?? "").ToString()));
                        else
                            output += qc.Question.IsWithoutEndingChar ? "" : ((qc.Question.EndingChar ?? "").ToString() == "\0" ? "" : (qc.Question.EndingChar ?? "").ToString());

                        if (qc.Question.IsEnter)
                            output += "\r\n";
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            if(Module.ModuleType.ID == 2)
                Session.LastModule = null;
            for (int i = 0; i < 3; i++)
            {
                output = output.Trim().Replace("\r\n,", "\r\n").Replace("\r\n.", ".").Replace("\r\n,", "\r\n").
                    Replace("..", ".").Replace(",.", ".").Replace(". .", ".").Replace("  ", " ").Replace(" ,", ",").Replace(" .", ".").Replace(".,", ".").
                    Replace(",  ", ", ").Replace("\r\n ", "\r\n").Replace(",,",",").Replace("'","");
            }
            return output;
        }

        private string GetText(bool IsMale, AnswerControl ans)
        {
            var text = (IsMale ? ans.Answer.ResultMaleHebText :
                        (ans.Answer.ResultFemaleHebText.Contains("לא נמצא במילון") ?
                         ans.Answer.ResultMaleHebText : ans.Answer.ResultFemaleHebText))
                        .Replace("{text}", (ans.textBox ?? new System.Windows.Controls.TextBox()).Text);
            return (text.Contains("לא נמצא במילון") ? "" : text).Replace("  "," ");
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

        private void NextButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            NextButton.Source = nextHover;
        }

        private void NextButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            NextButton.Source = next;
        }

        private void BackButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BackButton.Source = backHover;
        }

        private void BackButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BackButton.Source = back;
        }
    }
}
