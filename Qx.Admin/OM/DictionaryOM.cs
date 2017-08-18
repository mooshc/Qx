using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.Admin
{
    class DictionaryOM : ObjectGrid, IWpfObjectGrid
    {
        protected override object ItemSource()
        {
            return RemoteObjectProvider.GetDictionaryAccess().LoadAll();
        }

        public void New()
        {
            new DictionaryObjectEdit().ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() == null) return;
            new DictionaryObjectEdit(GetSelectedItem() as Dictionary).ShowDialog();
        }

        public void Delete()
        {
            foreach (Dictionary dic in GetSelectedItems())
                RemoteObjectProvider.GetDictionaryAccess().Delete(dic.ID);
            RefreshItemSource();
        }
    }
}
