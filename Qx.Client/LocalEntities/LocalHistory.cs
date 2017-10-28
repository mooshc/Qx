using System;
using System.Collections.Generic;

namespace Qx.Client.LocalEntities
{
    public class LocalHistory
    {
        public virtual int PatientAge { set; get; }

        public virtual char PatientGender { set; get; }

        public virtual string MedicalCaseId { set; get; }

        public virtual DateTime SaveTime { set; get; }

        public virtual List<int> ModuleId { set; get; }

        public virtual List<LocalDoctorAnswer> DoctorAnswers { set; get; }

        public LocalHistory()
        {
            ModuleId = new List<int>();
            DoctorAnswers = new List<LocalDoctorAnswer>();
            SaveTime = DateTime.Now;
            PatientGender = 'N';
        }
    }
}
