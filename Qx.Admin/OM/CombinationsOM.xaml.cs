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
    /// Interaction logic for CombinationsOM.xaml
    /// </summary>
    public partial class CombinationsOM : Window
    {
        public CombinationsOM(Module module)
        {
            InitializeComponent();
            CombinationsGridControl.SetDataSource(module);
            Title = "קומבינציות עבור מודול : " + module.ModuleHebText;
        }

        public CombinationsOM(Question question)
        {
            InitializeComponent();
            CombinationsGridControl.SetDataSource(question);
            Title = "קומבינציות עבור שאלה : " + question.QuestionHebText;
        }
    }
}
