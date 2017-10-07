using System;

namespace Qx.Common
{
    [Serializable]
    public class QuestionInModule
    {
        public virtual Question Question { set; get; }
        public virtual int Ordering { set; get; }
        public virtual bool IsPageBreak { set; get; }

        public QuestionInModule()
        {

        }

        public QuestionInModule(Question question, int ordering, bool isPageBreak)
        {
            Question = question;
            Ordering = ordering;
            IsPageBreak = isPageBreak;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return (obj as QuestionInModule).Question == Question;
            }
            catch
            {
                return false;
            }
        }
    }
}
