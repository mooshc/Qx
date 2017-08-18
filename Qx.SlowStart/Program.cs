using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using System.Threading;

namespace Qx.SlowStart
{
    class Program
    {
        static void Main(string[] args)
        {
            RemoteObjectProvider.UserGuid = new Guid("cff5b60e-3100-477f-8b4c-6cc0752a05dc");//Admin Guid
            var modNames = RemoteObjectProvider.GetModuleAccess().GetAnamnesisModules();
            foreach (var m in modNames)
            {
                Console.Out.WriteLine(m);
                Thread.Sleep(1000);
                if (/*m != "H_ChestPain-PNV" &&*/ !RemoteObjectProvider.GetModuleAccess().LoadToServerByName(m))
                    return;
            }
            Console.WriteLine("PermanentQuestions");
            RemoteObjectProvider.GetQuestionAccess().LoadPermanentQuestionsToServer();
            Console.Out.WriteLine("HebDicIterations");
            var count = (RemoteObjectProvider.GetDictionaryAccess().GetTotalCount() / 1000)+1;
            var hebLang = RemoteObjectProvider.GetLanguageAccess().Load(1);
            var i = 0;
            while(RemoteObjectProvider.GetDictionaryAccess().LoadByLangToServer(hebLang, i))
            {
                Console.Out.WriteLine("iteration: " + i++);
            }
        }
    }
}
