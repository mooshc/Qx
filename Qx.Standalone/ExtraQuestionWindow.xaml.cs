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

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for ExtraQuestionWindow.xaml
    /// </summary>
    public partial class ExtraQuestionWindow : Window
    {
        private bool first = true;
        public bool IsPhysicalExam;
        private BitmapImage next = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "SmallNext.png"));
        private BitmapImage nextHover = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "SmallNextHover.png")); 

        public ExtraQuestionWindow(QuestionControl question, Point location, bool isPhysicalExam)
        {
            InitializeComponent();
            ExtraQuestionPlace.Content = question;
            IsPhysicalExam = isPhysicalExam;

            Left = location.X;
            Top = location.Y;

            this.KeyDown += new System.Windows.Input.KeyEventHandler(ExtraQuestionWindow_KeyDown);
            this.Loaded += new RoutedEventHandler(ExtraQuestionWindow_Loaded);
            this.Activated+=new EventHandler(ExtraQuestionWindow_Activated);
            ExtraQuestionPlace.GotFocus+=new RoutedEventHandler(ExtraQuestionPlace_GotFocus);
        }

        void ExtraQuestionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Left -= ActualWidth/3*2;
            //Left += 300 - ActualWidth;
            Top -= ActualHeight / 2;
        }

        void ExtraQuestionWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.Right)
                SendKeys.SendWait(" ");
            if (e.Key == Key.Escape)
                Close();
            if (e.Key == Key.Return)
                this.Button_Click(null, null);
        }

        private void ExtraQuestionPlace_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!this.first)
            {
                return;
            }
            this.first = false;
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
            {
                elementWithFocus.MoveFocus(request);
            }
            e.Handled = true;
        }

        private void ExtraQuestionWindow_Activated(object sender, EventArgs e)
        {
            this.ExtraQuestionPlace.Focus();
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

        private void ContinueButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ContinueButton.Source = nextHover;
        }

        private void ContinueButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ContinueButton.Source = next;
        }
    }
}
