using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.Admin
{
    class CombinationOM : ObjectGrid, IWpfObjectGrid
    {
        Module Module;
        Question Question;

        public void SetDataSource(Module module)
        {
            Module = module;
        }

        public void SetDataSource(Question question)
        {
            Question = question;
        }

        protected override object ItemSource()
        {
            return Module == null ? Question.Combinations.Where(c => !c.IsDeleted) : Module.Combinations.Where(c => !c.IsDeleted);
        }

        public void New()
        {
            var com = new Combination();
            if(Module == null)
                com.Question = Question;
            else
                com.Module = Module;
            new CombinationObjectEdit(com).ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() == null) return;
            new CombinationObjectEdit(GetSelectedItem() as Combination).ShowDialog();
        }

        public void Delete()
        {
            foreach (Combination com in GetSelectedItems())
            {
                com.IsDeleted = true;
            }
            RefreshItemSource();
        }
    }
}
