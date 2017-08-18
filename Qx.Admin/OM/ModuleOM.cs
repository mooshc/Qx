using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.Admin
{
    class ModuleOM : ObjectGrid, IWpfObjectGrid
    {
        protected override object ItemSource()
        {
            return RemoteObjectProvider.GetLiteModuleAccess().LoadAll().Where(m => !m.IsDeleted);
        }

        public void New()
        {
            new ModuleObjectEdit().ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() == null) return;
            new ModuleObjectEdit(RemoteObjectProvider.GetModuleAccess().Load((GetSelectedItem() as LiteModule).ID)).ShowDialog();
        }

        public void Delete()
        {
            foreach (LiteModule module in GetSelectedItems())
            {
                module.IsDeleted = true;
                RemoteObjectProvider.GetLiteModuleAccess().SaveOrUpdate(module);
            }
            RefreshItemSource();
        }
    }
}
