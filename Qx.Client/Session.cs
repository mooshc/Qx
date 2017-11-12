using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using System.Windows;

namespace Qx.Client
{
    public static class Session
    {
        public static LiteUser User;

        public static Module LastModule;

        public static List<Module> RecomendedPhysicalEx = new List<Module>();

        public static char Gender;

        public static int Age;

        public static Language Lang;

        public static List<QuestionInModule> permanentQuestions;// = RemoteObjectProvider.GetQuestionAccess().LoadPermanentQuestions();

        public static Point windowPosition;

        public static string fileName;
    }
}
