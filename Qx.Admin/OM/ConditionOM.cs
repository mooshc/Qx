using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.Admin
{
    public class ConditionOM : ObjectGrid, IWpfObjectGrid
    {
        protected override object ItemSource()
        {
            return RemoteObjectProvider.GetConditionAccess().LoadAll();
        }

        public void New()
        {
            new ConditionObjectEdit().ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() == null) return;
            new ConditionObjectEdit(GetSelectedItem() as Condition).ShowDialog();
        }

        public void Delete()
        {
            foreach (Condition dic in GetSelectedItems())
                RemoteObjectProvider.GetConditionAccess().Delete(dic.ID);
            RefreshItemSource();
        }
    }
}
