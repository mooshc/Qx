using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.Admin
{
    class UserOM : ObjectGrid, IWpfObjectGrid
    {
        protected override object ItemSource()
        {  
            return RemoteObjectProvider.GetLiteUserAccess().LoadAll().Where(u => !u.IsDeleted);
        }

        public void New()
        {
            new UserObjectEdit().ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() == null) return;
                new UserObjectEdit(RemoteObjectProvider.GetUserAccess().Load((GetSelectedItem() as LiteUser).ID)).ShowDialog();
        }

        public void Delete()
        {
            foreach (LiteUser user in GetSelectedItems())
            {
                user.IsDeleted = true;
                RemoteObjectProvider.GetLiteUserAccess().SaveOrUpdate(user);
            }
            RefreshItemSource();
        }
    }
}
