using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using System.Windows.Controls;
using Qx.Learning;

namespace Qx.Admin
{
    public class ScenarioAnswerOM : ObjectGrid, IWpfObjectGrid
    {
        bool IsAnamnesis;

        public ScenarioAnswerOM()
        {
            toolBar.Items.Add(new Separator());
            var btn = new Button { Content = "הזן מודול" };
            btn.Click += new System.Windows.RoutedEventHandler(btn_Click);
            toolBar.Items.Add(btn);
        }

        void btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var module = RemoteObjectProvider.GetModuleAccess().LoadModuleByName((DataContext as Scenario).ModuleName);
            ModuleWindow mw;
            if (IsAnamnesis)
                mw = new ModuleWindow(module);
            else
                mw = new ModuleWindow(module.PhysicalExaminations.OrderBy(p => p.Ordering).Select(p => p.PhysicalExaminationModule).ToList());

            mw.ShowDialog();

            var s = (DataContext as Scenario);
            mw.ModuleAnswers = RemoteObjectProvider.GetDoctorAnswerAccess().Save(mw.ModuleAnswers).ToList();
            if (IsAnamnesis)
                foreach(var ma in mw.ModuleAnswers)
                    s.AnamnesisAnswers.Add(ma);
            else
                foreach (var ma in mw.ModuleAnswers)
                    s.PhysicalExAnswers.Add(ma);

            RemoteObjectProvider.GetScenarioAccess().SaveOrUpdate(s);
            RefreshItemSource();
        }

        public void SetSenario(Scenario scenario, bool isAnamnesis)
        {
            DataContext = scenario;
            IsAnamnesis = isAnamnesis;
            RefreshItemSource();
        }

        protected override object ItemSource()
        {
            return IsAnamnesis ? (DataContext as Scenario).AnamnesisAnswers : (DataContext as Scenario).PhysicalExAnswers;
        }

        public void New()
        {
            new DoctorAnswerObjectEdit(DataContext as Scenario, IsAnamnesis).ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() != null)
                new DoctorAnswerObjectEdit(DataContext as Scenario, IsAnamnesis, GetSelectedItem() as DoctorAnswer).ShowDialog();
        }

        public void Delete()
        {
            foreach(DoctorAnswer da in GetSelectedItems())
            {
                if (IsAnamnesis)
                    (DataContext as Scenario).AnamnesisAnswers.Remove(da);
                else
                    (DataContext as Scenario).PhysicalExAnswers.Remove(da);
                RefreshItemSource();
            }
        }
    }
}
