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
using System.Windows.Forms;
using Qx.Common;

namespace Qx.Client
{
    /// <summary>
    /// Interaction logic for QxMessageBox.xaml
    /// </summary>
    public partial class QxMessageBox : Window
    {
        public QxMessageBox(BitmapImage src, AnswerControl a)
        {
            InitializeComponent();
            if(((((a.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent is ContentControl))
                Owner = (((((((a.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as ContentControl).Parent as Grid).Parent as Border).Parent as StackPanel).Parent as ExtraQuestionWindow;
            else
                Owner = ((((((a.Parent as Grid).Parent as Grid).Parent as QuestionControl).Parent as StackPanel).Parent as Grid).Parent as Border).Parent as ModuleWindow;          
            var img = new Image() { Source = src, Height = src.PixelHeight, Width = src.PixelWidth };
            img.MouseDown += new MouseButtonEventHandler(img_MouseDown);
            Grid1.Children.Add(img);
        }

        void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
