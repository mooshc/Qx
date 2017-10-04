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
using System.Threading;

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for BigTextBox.xaml
    /// </summary>
    public partial class BigTextBox : Window
    {
        public string FinelText;
        public bool FinishFlag = false;

        public BigTextBox(string text, System.Windows.Point p )
        {
            InitializeComponent();
            Text.Text = text;
            Text.Focus();
            Text.CaretIndex = Text.Text.Length;
            Left = p.X-220;
            Top = p.Y + 0;
        }

        private void Text_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Close();
            FinishFlag = true;
            FinelText = Text.Text;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                Close();
                FinishFlag = true;
                FinelText = Text.Text;
            }
            catch
            {
            }
        }

        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                Close();
                FinishFlag = true;
                FinelText = Text.Text;
            }
            catch (Exception)
            {

            }
        }

        private void Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Text_MouseDoubleClick(null, null);
            }
        }
    }
}
        