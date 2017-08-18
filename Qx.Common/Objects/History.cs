using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class History : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual User User { set; get; }

        public virtual int PatientAge { set; get; }

        public virtual char PatientGender { set; get; }

        public virtual DateTime SaveTime { set; get; }

        public virtual Module Module { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<DoctorAnswer> DoctorAnswers { set; get; }

        public History()
        {
            DoctorAnswers = new List<DoctorAnswer>();
            SaveTime = DateTime.Now;
        }

        protected override object GetObjectId()
        {
            return ID;
        }
    }
}
