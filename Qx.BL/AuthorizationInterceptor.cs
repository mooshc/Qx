using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Castle.Core.Interceptor;
using Nachshon.Nachshon2;
using Frameworks;
using log4net;
using System.Collections.Generic;

namespace Qx.BL
{
    public class AuthorizationInterceptor : IInterceptor
    {
        private List<string> AllowedFunctions = new List<string>() { "LoadByLang", "LoadPermanentQuestions", "SaveOrUpdateByName", "SaveHistory" };

        public void Intercept(IInvocation invocation)
        {
            var env = CallContext.GetData("Env") as CallContextLight;
            if (env != null && (invocation.Method.Name.Equals("IsLoginCorrect") ||
                (ServiceLocator.UserAccess.IsGuidCorrect(env.UserID) && AllowedFunctions.Contains(invocation.Method.Name)) ||
                ServiceLocator.UserAccess.IsAdminUser(env.UserID)))
                invocation.Proceed();
            else
                throw new UnauthorizedAccessException("You don't have permission to run this command.");
        }
    }
}
