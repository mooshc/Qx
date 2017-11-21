using Qx.Common;
using Qx.Common.Objects;
using Qx.EntitySerialization;
using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Qx.QxdbGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WorkingFolderLabel.Content = "Output to: " + ConfigurationManager.AppSettings["DbFilesFolder"];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TypeTextBox.Text))
                {
                    MessageBox.Show("Type can't be empty");
                    return;
                }

                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text) || DescriptionTextBox.Text.Length > 25 || !Regex.IsMatch(DescriptionTextBox.Text, "^[a-zA-Z0-9_]+$"))
                {
                    MessageBox.Show("Description can't be empty, must be under 25 charcters long and can contain letters and numbers only");
                    return;
                }

                TranslatedObject.UseOnlineContentDictionary = true;
                var check = RemoteObjectProvider.GetLiteUserAccess().IsLoginCorrect("moosh", "thirnzho");
                if (check == null)
                    throw new Exception("User returned empty!");
                RemoteObjectProvider.UserGuid = check.Guid;
                LiteSession session = new LiteSession()
                {
                    User = check,
                    PermanentQuestions = RemoteObjectProvider.GetQuestionAccess().LoadPermanentQuestions()
                };

                ContentDictionary.PopulateDictionary(check.Language);

                EntitySerializer.SerializeToFile(session, TypeTextBox.Text, DescriptionTextBox.Text, ConfigurationManager.AppSettings["DbFilesFolder"]);

                MessageBox.Show("Succeeded!");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed!");
                File.AppendAllLines("Log.txt", new[] { ex.Message });
            }
        }
    }
}
