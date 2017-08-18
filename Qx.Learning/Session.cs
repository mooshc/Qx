using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.Learning
{
    public static class Session
    {
        public static User User;

        public static Module LastModule;

        public static List<Module> RecomendedPhysicalEx = new List<Module>();

        public static char Gender;

        public static int Age;

        public static Language Lang;

        public static List<QuestionInModule> permanentQuestions = RemoteObjectProvider.GetQuestionAccess().LoadPermanentQuestions();

        public static List<DoctorAnswer> LastAnswers;

        public static string LastResult;
    }
}
