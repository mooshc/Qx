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
using System.Diagnostics;

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for MedicalFileWindow.xaml
    /// </summary>
    public partial class MedicalFileWindow : Window
    {
        Scenario Scenario;
        List<DoctorAnswer> AnamnesisAnswers;
        List<DoctorAnswer> PhysicalExAnswers;
        bool isPassedAnamnesis = false;
        bool isPassedPhysicalEx = false;
        TrainingWindow Win;

        public MedicalFileWindow(Scenario scenario, TrainingWindow win)
        {
            InitializeComponent();
            Scenario = scenario;
            Win = win;
            ChatPic.Source = CommonFunctions.GetBmpImageByFileName(scenario.FileName, true);
        }

        private void EnamnesisTextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Q))
            {
                Session.LastAnswers = null;
                if (!(new ModuleSelectWindow('Q', Scenario.ModuleName).ShowDialog() ?? false))
                    return;
                AnamnesisAnswers = Session.LastAnswers;
                EnamnesisTextBlock.Text = Session.LastResult;
                var wrongAnswers = new List<DoctorAnswer>();
                foreach (DoctorAnswer da in AnamnesisAnswers)
                    if (!Scenario.AnamnesisAnswers.Contains(da))
                        wrongAnswers.Add(da);
                foreach(DoctorAnswer da in Scenario.AnamnesisAnswers)
                    if (!AnamnesisAnswers.Contains(da) && !wrongAnswers.Select(wa => wa.AnswerID).Contains(da.AnswerID))
                        wrongAnswers.Add(da);

                if (Scenario.IsTest)
                {
                    MessageBox.Show(wrongAnswers.Count > 5 ? "נכשלת באנמנזה עם " + wrongAnswers.Count + "טעויות" : "עברת את האנמנזה");
                    isPassedAnamnesis = !(wrongAnswers.Count > 5);
                }
                else
                    Feedback(wrongAnswers, true);
            }
        }

        private void PhysicalExTextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.E))
            {
                Session.LastAnswers = null;
                if (!(new ModuleSelectWindow('E', Scenario.ModuleName).ShowDialog() ?? false))
                    return;
                PhysicalExAnswers = Session.LastAnswers;
                PhysicalExTextBlock.Text = Session.LastResult;
                var wrongAnswers = new List<DoctorAnswer>();
                foreach (DoctorAnswer da in PhysicalExAnswers)
                    if (!Scenario.PhysicalExAnswers.Contains(da))
                        wrongAnswers.Add(da);
                foreach (DoctorAnswer da in Scenario.PhysicalExAnswers)
                    if (!PhysicalExAnswers.Contains(da) && !wrongAnswers.Select(wa => wa.AnswerID).Contains(da.AnswerID))
                        wrongAnswers.Add(da);

                if (Scenario.IsTest)
                {
                    MessageBox.Show(wrongAnswers.Count > 5 ? "נכשלת בבדיקה הגופנית עם " + wrongAnswers.Count + "טעויות" : "עברת את הבדיקה הגופנית בהצלחה");
                    isPassedPhysicalEx = !(wrongAnswers.Count > 5);
                }
                else
                    Feedback(wrongAnswers, false);
            }
        }

        private void Feedback(List<DoctorAnswer> wrongAnswers, bool isAnamnesis)
        {
            var module = new Module() { ModuleType = RemoteObjectProvider.GetModuleTypeAccess().Load(isAnamnesis ? 1 : 2), Name = "Feedback" };
            int order = 1;
            foreach (var a in wrongAnswers)
            {
                if (!module.Questions.Select(q => q.Question).Contains(a.Answer.Question))
                {
                    module.Questions.Add(new QuestionInModule(a.Answer.Question,order,false));
                    order++;
                }
            }
            new ModuleWindow(module, wrongAnswers, isAnamnesis ? AnamnesisAnswers : PhysicalExAnswers,
                isAnamnesis ? Scenario.AnamnesisAnswers.ToList() : Scenario.PhysicalExAnswers.ToList(), isAnamnesis).Show();
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Scenario.IsTest)
            {
                Win.WindowState = System.Windows.WindowState.Normal;
                Close();
                return;
            }
            if (isPassedAnamnesis && isPassedPhysicalEx)
            {
                MessageBox.Show("עברת את המבחן בהצלחה! הינך מורשה להשתמש במודול זה.");
                RemoteObjectProvider.GetUserAccess().PermitUserToAnamnesis(Session.User, Session.User.Modules.Select(m => m.Module).Where(m => m.Name == Scenario.ModuleName).First());
                Close();
            }
            else
                MessageBox.Show("נכשלת במבחן, נא בצע אותו שוב כדי לקבל רשות לעבוד עם מודול זה");
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var proc = Process.GetProcessesByName("Qx.Learning");
            foreach (var p in proc)
                p.Kill();
        }
    }
}
