using System;

namespace Qx.Common
{
    [Serializable]
    public class PhysicalExaminationInAnamnesis
    {
        public virtual Module PhysicalExaminationModule { set; get; }
        public virtual int Ordering { set; get; }

        public PhysicalExaminationInAnamnesis()
        {

        }

        public PhysicalExaminationInAnamnesis(Module pysicalEx, int order)
        {
            PhysicalExaminationModule = pysicalEx;
            Ordering = order;
        }

        public override bool Equals(object obj)
        {
            return obj is PhysicalExaminationInAnamnesis && (obj as PhysicalExaminationInAnamnesis).PhysicalExaminationModule == PhysicalExaminationModule;
        }
    }
}
