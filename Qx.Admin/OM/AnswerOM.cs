using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qx.Admin
{
    class AnswerOM : ObjectGrid, IWpfObjectGrid
    {
        public AnswerOM()
        {
            toolBar.Items.Add(new Separator());
            var btn = new Button { Content = "הזז למטה" };
            btn.Click += new System.Windows.RoutedEventHandler(btn_Click);
            toolBar.Items.Add(btn);
            var btn1 = new Button { Content = "הזז למעלה" };
            btn1.Click += new System.Windows.RoutedEventHandler(btn1_Click);
            toolBar.Items.Add(btn1);

            toolBar.Background = new SolidColorBrush(Colors.Yellow) { Opacity = 0.7 };
        }

        void btn1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GetSelectedItem() == null) return;
            var ans = (Answer)GetSelectedItem();
            var question = DataContext as Question;
            var answers = question.Answers.Where(a => !a.IsDeleted).ToArray<Answer>();
            var index = -1;
            for (int i = 0; i < answers.Length; i++)
                if (answers[i].ID == ans.ID)
                    index = i;

            if (index == 0) return;
            Swap(question.Answers.Where(a => a.ID == ans.ID).FirstOrDefault(), question.Answers.Where(a => a.ID == answers[index-1].ID).FirstOrDefault());
            RemoteObjectProvider.GetQuestionAccess().SaveOrUpdate(DataContext as Question);
            RefreshItemSource();
        }

        void btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GetSelectedItem() == null) return;
            var ans = (Answer)GetSelectedItem();
            var question = DataContext as Question;
            var answers = question.Answers.Where(a => !a.IsDeleted).ToArray<Answer>();
            var index = -1;
            for (int i = 0; i < answers.Length; i++)
                if (answers[i].ID == ans.ID)
                    index = i;

            if (index == answers.Length-1) return;
            Swap(question.Answers.Where(a => a.ID == ans.ID).FirstOrDefault(), question.Answers.Where(a => a.ID == answers[index + 1].ID).FirstOrDefault());
            RemoteObjectProvider.GetQuestionAccess().SaveOrUpdate(DataContext as Question);
            RefreshItemSource();
        }

        void Swap(Answer first, Answer second)
        {
            var activeNegationLabelHebText = first.ActiveNegationLabelHebText;
            first.ActiveNegationLabelHebText = second.ActiveNegationLabelHebText;
            second.ActiveNegationLabelHebText = activeNegationLabelHebText;

            var recomendedPhysicalEx = first.RecomendedPhysicalEx;
            first.RecomendedPhysicalEx = second.RecomendedPhysicalEx;
            second.RecomendedPhysicalEx = recomendedPhysicalEx;

            var relatedModuleName = first.RelatedModuleName;
            first.RelatedModuleName = second.RelatedModuleName;
            second.RelatedModuleName = relatedModuleName;

            var singularOnCsv = first.SingularOnCsv;
            first.SingularOnCsv = second.SingularOnCsv;
            second.SingularOnCsv = singularOnCsv;


            var tempExtraQuestion = first.ExtraQuestion;
            first.ExtraQuestion = second.ExtraQuestion;
            second.ExtraQuestion = tempExtraQuestion;

            var extraQuestionInFlow = first.ExtraQuestionInFlow;
            first.ExtraQuestionInFlow = second.ExtraQuestionInFlow;
            second.ExtraQuestionInFlow = extraQuestionInFlow;

            var tempImage = first.ImageFileName;
            first.ImageFileName = second.ImageFileName;
            second.ImageFileName = tempImage;

            var tempBool = first.IsContainsTextBox;
            first.IsContainsTextBox = second.IsContainsTextBox;
            second.IsContainsTextBox = tempBool;

            tempBool = first.IsSingular;
            first.IsSingular = second.IsSingular;
            second.IsSingular = tempBool;

            tempBool = first.IsTextBoxDigitsOnly;
            first.IsTextBoxDigitsOnly = second.IsTextBoxDigitsOnly;
            second.IsTextBoxDigitsOnly = tempBool;

            tempBool = first.IsAnd;
            first.IsAnd = second.IsAnd;
            second.IsAnd = tempBool;

            var tempWarnings = first.WarningConditions;
            first.WarningConditions = second.WarningConditions;
            second.WarningConditions = tempWarnings;

            var tempName1 = (string) first.Name.Clone();
            var tempName2 = (string) second.Name.Clone();
            first.Name = second.Name + "temp";
            RemoteObjectProvider.GetAnswerAccess().SaveOrUpdate(first);
            second.Name = tempName1;
            RemoteObjectProvider.GetAnswerAccess().SaveOrUpdate(second);
            first.Name = tempName2;
            RemoteObjectProvider.GetAnswerAccess().SaveOrUpdate(first);
        }

        public void SetQuestion(Question question)
        {
            DataContext = question;
            RefreshItemSource();
        }

        protected override object ItemSource()
        {
            return (DataContext as Question).Answers.Where(a => !a.IsDeleted);
        }

        public void New()
        {
            new AnswerObjectEdit(new Answer(), DataContext as Question).ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() == null) return;
                new AnswerObjectEdit(GetSelectedItem() as Answer, DataContext as Question).ShowDialog();
        }

        public void Delete()
        {
            foreach (Answer ans in GetSelectedItems())
            {
                ans.IsDeleted = true;
            }
            RefreshItemSource();
        }
    }
}
