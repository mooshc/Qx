using System;
using System.Collections.Generic;

namespace Qx.Common
{
    [Serializable]
    public class History
    {
        public virtual int ID { private set; get; }

        public virtual User User { set; get; }

        public virtual int PatientAge { set; get; }

        public virtual char PatientGender { set; get; }

        public virtual string MedicalCaseId { set; get; }

        public virtual DateTime SaveTime { set; get; }

        public virtual Module Module { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<DoctorAnswer> DoctorAnswers { set; get; }

        public History()
        {
            DoctorAnswers = new List<DoctorAnswer>();
            SaveTime = DateTime.Now;
        }
    }
}
