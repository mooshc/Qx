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

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for FollowingWindow.xaml
    /// </summary>
    public partial class FollowingWindow : Window
    {
        private readonly InvisibleForm invisibleForm;

        public FollowingWindow(InvisibleForm invisibleForm)
        {
            InitializeComponent();
            this.invisibleForm = invisibleForm;
        }

        private void QxImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            invisibleForm.Anamnesis("ManuallyTriggered");
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "Grphics/OnHoldClick.png")));
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "Grphics/OnHold.png")));
        }
    }
}
