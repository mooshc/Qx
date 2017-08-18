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

namespace Qx.Admin
{
    /// <summary>
    /// Interaction logic for CombinationObjectEdit.xaml
    /// </summary>
    public partial class CombinationObjectEdit : Window
    {
        public CombinationObjectEdit(Combination combination)
        {
            InitializeComponent();
            DataContext = combination;
            if (combination.Question != null)
                OrderRow.Height = new GridLength(0);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var comb = DataContext as Combination;
            comb.ResultFemaleHebText = HebFemaleOutputTextBox.Text;
            comb.ResultMaleHebText = HebMaleOutputTextBox.Text;
            if (comb.Question != null)
                if (!comb.Question.Combinations.Contains(comb))
                    comb.Question.Combinations.Add(comb);
            if (comb.Module != null)
                if (!comb.Module.Combinations.Contains(comb))
                    comb.Module.Combinations.Add(comb);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Answers_Click(object sender, RoutedEventArgs e)
        {
            new CombinatedAnswersManagement((DataContext as Combination).CombinatedAnswers,
                (DataContext as Combination).Module == null ? (DataContext as Combination).Question.Answers : (DataContext as Combination).Module.GetAllAnswers()).ShowDialog();
        }
    }
}
