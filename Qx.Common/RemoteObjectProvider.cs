using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using Frameworks;
using System.IO;

namespace Qx.Common
{
    public class RemoteObjectProvider
    {
        //private const string LOCAL_HOST = "tcp://localhost:8080";
        private const string LOCAL_HOST = "tcp://192.168.5.88:8080"; // Bikurofe
        //private const string LOCAL_HOST = "tcp://31.168.73.231:8080"; // Alon

        public static Guid UserGuid;

        private static T GetLocalService<T>(string serviceName)
        {
            CallContextLight.Current = new CallContextLight(UserGuid);
            string hostAddress = string.Format("tcp://{0}", DynamicConfigurationProvider.GetUrl());
            return (T)RemotingServices.Connect(typeof(T), string.Format("{0}/{1}.rem", hostAddress, serviceName));
        }

        #region Services
        public static IAnswerAccess GetAnswerAccess()
        {
            return GetLocalService<IAnswerAccess>("AnswerAccess");
        }
        public static IColorAccess GetColorAccess()
        {
            return GetLocalService<IColorAccess>("ColorAccess");
        }
        public static ICombinatedAnswerAccess GetCombinatedAnswerAccess()
        {
            return GetLocalService<ICombinatedAnswerAccess>("CombinatedAnswerAccess");
        }
        public static ICombinationAccess GetCombinationAccess()
        {
            return GetLocalService<ICombinationAccess>("CombinationAccess");
        }
        public static ICompanyAccess GetCompanyAccess()
        {
            return GetLocalService<ICompanyAccess>("CompanyAccess");
        }
        public static IConditionAccess GetConditionAccess()
        {
            return GetLocalService<IConditionAccess>("ConditionAccess");
        }
        public static IDictionaryAccess GetDictionaryAccess()
        {
            return GetLocalService<IDictionaryAccess>("DictionaryAccess");
        }
        public static IDoctorAnswerAccess GetDoctorAnswerAccess()
        {
            return GetLocalService<IDoctorAnswerAccess>("DoctorAnswerAccess");
        }
        public static IHistoryAccess GetHistoryAccess()
        {
            return GetLocalService<IHistoryAccess>("HistoryAccess");
        }
        public static ILanguageAccess GetLanguageAccess()
        {
            return GetLocalService<ILanguageAccess>("LanguageAccess");
        }
        public static IModuleAccess GetModuleAccess()
        {
            return GetLocalService<IModuleAccess>("ModuleAccess");
        }
        public static ILiteModuleAccess GetLiteModuleAccess()
        {
            return GetLocalService<ILiteModuleAccess>("LiteModuleAccess");
        }
        public static IModuleTypeAccess GetModuleTypeAccess()
        {
            return GetLocalService<IModuleTypeAccess>("ModuleTypeAccess");
        }
        public static IQuestionAccess GetQuestionAccess()
        {
            return GetLocalService<IQuestionAccess>("QuestionAccess");
        }
        public static ILiteQuestionAccess GetLiteQuestionAccess()
        {
            return GetLocalService<ILiteQuestionAccess>("LiteQuestionAccess");
        }
        public static IQuestionTypeAccess GetQuestionTypeAccess()
        {
            return GetLocalService<IQuestionTypeAccess>("QuestionTypeAccess");
        }
        public static IUserAccess GetUserAccess()
        {
            return GetLocalService<IUserAccess>("UserAccess");
        }
        public static ILiteUserAccess GetLiteUserAccess()
        {
            return GetLocalService<ILiteUserAccess>("LiteUserAccess");
        }
        public static IScenarioAccess GetScenarioAccess()
        {
            return GetLocalService<IScenarioAccess>("ScenarioAccess");
        }
        #endregion
    }
}
