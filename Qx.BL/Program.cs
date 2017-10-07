using System;
using System.ServiceProcess;
using log4net;
using System.Reflection;
using System.IO;
using System.Runtime.Remoting;
using Castle.DynamicProxy;
using Qx.Common;
using Castle.Core.Interceptor;

namespace Qx.BL
{
    class Program : ServiceBase
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Qx-Config");
        internal static string RemotingConfig = Path.Combine(ConfigPath, "QxRemotingConfig.xml");
        internal static string NHibernateConfig = Path.Combine(ConfigPath, "QxNHibernateConfig.xml");

        static void Main(string[] args)
        {            
            var program = new Program();

            if (args.Length > 0 && args[0] == "-s")
            {
                Run(program);
            }
            else
            {
                program.OnStart(null);
                Console.ReadLine();
                program.OnStop();
            }
        }

        protected override void OnStart(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            log4net.Config.XmlConfigurator.Configure(new FileInfo(NHibernateConfig));
            RemotingConfiguration.Configure(RemotingConfig, false);
            var gen = new ProxyGenerator();
            var opI = new OperationInterceptor();
            var transI = new TransactionInterceptor();
            var authI = new AuthorizationInterceptor();

            ProxyAndMarshalService(gen, ServiceLocator.AnswerAccess, "AnswerAccess.rem", typeof(IAnswerAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.ColorAccess, "ColorAccess.rem", typeof(IColorAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.CombinatedAnswerAccess, "CombinatedAnswerAccess.rem", typeof(ICombinatedAnswerAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.CombinationAccess, "CombinationAccess.rem", typeof(ICombinationAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.CompanyAccess, "CompanyAccess.rem", typeof(ICompanyAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.ConditionAccess, "ConditionAccess.rem", typeof(IConditionAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.DictionaryAccess, "DictionaryAccess.rem", typeof(IDictionaryAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.DoctorAnswerAccess, "DoctorAnswerAccess.rem", typeof(IDoctorAnswerAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.HistoryAccess, "HistoryAccess.rem", typeof(IHistoryAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.LanguageAccess, "LanguageAccess.rem", typeof(ILanguageAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.ModuleAccess, "ModuleAccess.rem", typeof(IModuleAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.LiteModuleAccess, "LiteModuleAccess.rem", typeof(ILiteModuleAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.ModuleTypeAccess, "ModuleTypeAccess.rem", typeof(IModuleTypeAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.QuestionAccess, "QuestionAccess.rem", typeof(IQuestionAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.LiteQuestionAccess, "LiteQuestionAccess.rem", typeof(ILiteQuestionAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.QuestionTypeAccess, "QuestionTypeAccess.rem", typeof(IQuestionTypeAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.UserAccess, "UserAccess.rem", typeof(IUserAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.LiteUserAccess, "LiteUserAccess.rem", typeof(ILiteUserAccess), transI, opI, authI);
            ProxyAndMarshalService(gen, ServiceLocator.ScenarioAccess, "ScenarioAccess.rem", typeof(IScenarioAccess), transI, opI, authI);
            
            log.Info("Service Started ...");
        }

        public static object ProxyAndMarshalService(ProxyGenerator gen, object obj, string uri, Type interfaceType, params IInterceptor[] interceptors)
        {
            MarshalByRefObject marshalByRefObject = (MarshalByRefObject)ProxyService(gen, obj, interfaceType, interceptors);
            RemotingServices.Marshal(marshalByRefObject, uri, interfaceType);
            return (object)marshalByRefObject;
        }

        public static object ProxyService(ProxyGenerator gen, object obj, Type interfaceType, params IInterceptor[] interceptors)
        {
            ProxyGenerationOptions options = new ProxyGenerationOptions()
            {
                BaseTypeForInterfaceProxy = typeof(MarshalByRefSingleton)
            };
            return gen.CreateInterfaceProxyWithTarget(interfaceType, obj, options, interceptors);
        }
    }
}
