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
    /// Interaction logic for ScenarioAnswersOM.xaml
    /// </summary>
    public partial class ScenarioAnswersOM : Window
    {
        public ScenarioAnswersOM(Scenario scenario, bool isAnamnesis)
        {
            InitializeComponent();
            DoctorAnswersGridControl.SetSenario(scenario, isAnamnesis);
        }
    }
}
