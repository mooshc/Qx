using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Qx.Common
{
    public static class DynamicConfigurationProvider
    {
        private static string url = null;

        public static string GetUrl()
        {
            EnsureInitialized();
            return url;
        }

        private static void EnsureInitialized()
        {
            if (DynamicConfigurationProvider.url == null)
            {
                var url = File.ReadAllLines("RemoteIP.txt").FirstOrDefault();
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new Exception("The file 'RemoteIP.txt' is empty");
                }

                DynamicConfigurationProvider.url = url;
            }
        }
    }
}
