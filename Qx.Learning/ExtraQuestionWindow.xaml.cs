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

namespace Qx.Learning
{
    /// <summary>
    /// Interaction logic for ExtraQuestionWindow.xaml
    /// </summary>
    public partial class ExtraQuestionWindow : Window
    {
        public ExtraQuestionWindow(QuestionControl question, Point location)
        {
            InitializeComponent();
            ExtraQuestionPlace.Content = question;

            Left = location.X;
            Top = location.Y;

            this.KeyDown += new System.Windows.Input.KeyEventHandler(ExtraQuestionWindow_KeyDown);
            this.Loaded += new RoutedEventHandler(ExtraQuestionWindow_Loaded);
        }

        void ExtraQuestionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Left -= ActualWidth/3*2;
            Top -= ActualHeight / 2;
            (ExtraQuestionPlace.Content as QuestionControl).Focus();
        }

        void ExtraQuestionWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.Right)
                SendKeys.SendWait(" ");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((ExtraQuestionPlace.Content as QuestionControl).IsValid)
            {
                DialogResult = true;
                Close();
                ExtraQuestionPlace.Content = null;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
            ExtraQuestionPlace.Content = null;
        }
    }
}
