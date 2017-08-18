using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.Admin
{
    class ScenarioOM : ObjectGrid, IWpfObjectGrid
    {
        public void New()
        {
            new ScenarioObjectEdit().ShowDialog();
        }

        public void Edit()
        {
            new ScenarioObjectEdit(GetSelectedItem() as Scenario).ShowDialog();
        }

        public void Delete()
        {
            var scenarios = (GetSelectedItems() as List<Scenario>);
            foreach (Scenario s in scenarios)
                s.IsDeleted = true;
            RemoteObjectProvider.GetScenarioAccess().SaveOrUpdate(scenarios);
        }

        protected override object ItemSource()
        {
            return RemoteObjectProvider.GetScenarioAccess().LoadAll();
        }
    }
}
