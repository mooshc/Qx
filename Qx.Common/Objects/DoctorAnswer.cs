using System;

namespace Qx.Common
{
    [Serializable]
    public class DoctorAnswer
    {
        public virtual int ID { private set; get; }

        public virtual Answer _answer { set; get; }

        public virtual Answer Answer 
        { 
            get 
            { 
                if(_answer == null) 
                    _answer = RemoteObjectProvider.GetAnswerAccess().Load(AnswerID);
                return _answer;
            }
        }

        public virtual string AnswerName { get { return RemoteObjectProvider.GetAnswerAccess().GetAnswerName(AnswerID); } }

        public virtual int AnswerID { set; get; }

        public virtual int RelatedAnswerID { set; get; }

        public virtual string RelatedAnswerName { get { return RemoteObjectProvider.GetAnswerAccess().GetAnswerName(RelatedAnswerID); } }

        public virtual string Text { set; get; }
        
        public virtual DateTime TimeStamp { set; get; }

        public DoctorAnswer()
        {

        }

        public DoctorAnswer(DateTime timeStamp,  int answerID, string text, int relatedID = 0)
        {
            TimeStamp = timeStamp;
            AnswerID = answerID;
            RelatedAnswerID = relatedID;
            Text = text;
        }

        public DoctorAnswer(DateTime timeStamp,  int answerID, string text, Answer answer, int relatedID = 0)
        {
            TimeStamp = timeStamp;
            AnswerID = answerID;
            RelatedAnswerID = relatedID;
            Text = text;
            _answer = answer;
        }

        public override bool Equals(object obj)
        {
            int i;
            return  (obj is DoctorAnswer) &&
                    (obj as DoctorAnswer).AnswerID == AnswerID &&
                    (obj as DoctorAnswer).RelatedAnswerID == RelatedAnswerID &&
                    (((obj as DoctorAnswer).Text != null && Int32.TryParse(Text, out i) && (obj as DoctorAnswer).Text == Text) || ((obj as DoctorAnswer).Text != null && !Int32.TryParse(Text, out i)));
        }
    }
}
