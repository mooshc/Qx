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
using Qx.Common;

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for FinishUserControl.xaml
    /// </summary>
    public partial class FinishUserControl : UserControl
    {
        private RoutedEventHandler func;

        private BitmapImage male = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "FinishMale.png"));
        private BitmapImage maleHover = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "FinishMaleHover.png"));
        private BitmapImage female = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "FinishFemale.png"));
        private BitmapImage femaleHover = new BitmapImage(new Uri(CommonFunctions.GraphicsNativePath + "FinishFemaleHover.png"));

        public FinishUserControl(bool isPhysicalEx, RoutedEventHandler finishFunc, bool? isMale)
        {
            InitializeComponent();
            FinishLabel.Content += isPhysicalEx ? "בדיקה הגופנית" : "אנמנזה";
            func = finishFunc;
            if (isMale.HasValue)
            {
                if (isMale.Value)
                    FinishFemale.Visibility = Visibility.Hidden;
                else
                    FinishMale.Visibility = Visibility.Hidden;
            }
        }

        private void FinishMale_MouseDown(object sender, MouseButtonEventArgs e)
        {
            func.Invoke(sender, null);
        }

        private void FinishMale_MouseEnter(object sender, MouseEventArgs e)
        {
            FinishMale.Source = maleHover;
        }

        private void FinishMale_MouseLeave(object sender, MouseEventArgs e)
        {
            FinishMale.Source = male;
        }

        private void FinishFemale_MouseEnter(object sender, MouseEventArgs e)
        {
            FinishFemale.Source = femaleHover;
        }

        private void FinishFemale_MouseLeave(object sender, MouseEventArgs e)
        {
            FinishFemale.Source = female;
        }
    }
}
