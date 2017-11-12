using System;

namespace Qx.Client.LocalEntities
{
    public class LocalDoctorAnswer
    {
        public virtual int AnswerID { set; get; }

        public virtual int RelatedAnswerID { set; get; }

        public virtual string Text { set; get; }

        // public virtual DateTime TimeStamp { set; get; }

        public LocalDoctorAnswer()
        {

        }

        public LocalDoctorAnswer(DateTime timeStamp, int answerID, string text, int relatedID = 0)
        {
            // TimeStamp = timeStamp;
            AnswerID = answerID;
            RelatedAnswerID = relatedID;
            Text = text;
        }
    }
}
