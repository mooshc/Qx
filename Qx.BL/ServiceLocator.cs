using System;
using System.Text;
using NHibernate;
using NHibernate.Context;
using System.Reflection;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Engine;
using Qx.Common;

namespace Qx.BL
{
    internal static class ServiceLocator
    {
        internal static ISessionFactory SessionFactory { get; private set; }
        internal static ICurrentSessionContext CurrentSessionContext { get; private set; }

        static ServiceLocator()
        {
            var cfg = new Configuration().Configure(Program.NHibernateConfig);

            cfg.AddDirectory(new DirectoryInfo("ObjectAccess/ORM"));
            

            try
            {
                SessionFactory = cfg.BuildSessionFactory();
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine("Exception: \r\n" +
                                  e.LoaderExceptions[0]);
                throw;
            }

#if DEBUG
            var schemaUpdate = new NHibernate.Tool.hbm2ddl.SchemaUpdate(cfg);

            var sb = new StringBuilder();
            schemaUpdate.Execute(s => sb.AppendLine(s), false);
#endif
            CurrentSessionContext = new OperationSessionProvider(SessionFactory as ISessionFactoryImplementor)
            {
                InterceptorFactory = () => new TypedStateInterceptor()
            };

            InitServices();
        }

        internal static IAnswerAccess AnswerAccess { get; private set; }
        internal static IColorAccess ColorAccess { get; private set; }
        internal static ICombinatedAnswerAccess CombinatedAnswerAccess { get; private set; }
        internal static ICombinationAccess CombinationAccess { get; private set; }
        internal static ICompanyAccess CompanyAccess { get; private set; }
        internal static IConditionAccess ConditionAccess { get; private set; }
        internal static IDictionaryAccess DictionaryAccess { get; private set; }
        internal static IDoctorAnswerAccess DoctorAnswerAccess { get; private set; }
        internal static IHistoryAccess HistoryAccess { get; private set; }
        internal static ILanguageAccess LanguageAccess { get; private set; }
        internal static IModuleAccess ModuleAccess { get; private set; }
        internal static ILiteModuleAccess LiteModuleAccess { get; private set; }
        internal static IModuleTypeAccess ModuleTypeAccess { get; private set; }
        internal static IQuestionAccess QuestionAccess { get; private set; }
        internal static ILiteQuestionAccess LiteQuestionAccess { get; private set; }
        internal static IQuestionTypeAccess QuestionTypeAccess { get; private set; }
        internal static IUserAccess UserAccess { get; private set; }
        internal static ILiteUserAccess LiteUserAccess { get; private set; }
        internal static IScenarioAccess ScenarioAccess { get; private set; }

        private static void InitServices()
        {
            AnswerAccess = new AnswerAccess(CurrentSessionContext);
            ColorAccess = new ColorAccess(CurrentSessionContext);
            CombinatedAnswerAccess = new CombinatedAnswerAccess(CurrentSessionContext);
            CombinationAccess = new CombinationAccess(CurrentSessionContext);
            CompanyAccess = new CompanyAccess(CurrentSessionContext);
            ConditionAccess = new ConditionAccess(CurrentSessionContext);
            DictionaryAccess = new DictionaryAccess(CurrentSessionContext);
            DoctorAnswerAccess = new DoctorAnswerAccess(CurrentSessionContext);
            HistoryAccess = new HistoryAccess(CurrentSessionContext);
            LanguageAccess = new LanguageAccess(CurrentSessionContext);
            ModuleAccess = new ModuleAccess(CurrentSessionContext);
            LiteModuleAccess = new LiteModuleAccess(CurrentSessionContext);
            ModuleTypeAccess = new ModuleTypeAccess(CurrentSessionContext);
            QuestionAccess = new QuestionAccess(CurrentSessionContext);
            LiteQuestionAccess = new LiteQuestionAccess(CurrentSessionContext);
            QuestionTypeAccess = new QuestionTypeAccess(CurrentSessionContext);
            UserAccess = new UserAccess(CurrentSessionContext);
            LiteUserAccess = new LiteUserAccess(CurrentSessionContext);
            ScenarioAccess = new ScenarioAccess(CurrentSessionContext);
        }
    }
}
