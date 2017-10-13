using System.Collections.Generic;
using System.IO;

namespace Qx.EntitySerialization
{
    public static class SerializationConfiguration
    {
        private static Dictionary<ConfigKey, string> _configDictionary = null;
        private const string CONFIG_FILE_NAME = @"serializtion.conf";
        private static readonly Dictionary<ConfigKey, string> DEFAULT_CONF = new Dictionary<ConfigKey, string>
        {
            { ConfigKey.NextVersion, "1" }
        };

        public enum ConfigKey { NextVersion };


        public static string Get(ConfigKey key)
        {
            if (_configDictionary == null)
            {
                Load();
            }
            return _configDictionary[key];
        }


        public static void Set(ConfigKey key, string value)
        {
            _configDictionary[key] = value;
            Save();
        }

        private static void InitDefaults()
        {
            _configDictionary = DEFAULT_CONF;
            Save();
        }
        
        private static void Load()
        {
            if (!File.Exists(CONFIG_FILE_NAME))
                InitDefaults();
            string json = File.ReadAllText(CONFIG_FILE_NAME);
            _configDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<ConfigKey, string>>(json);
        }

        private static void Save()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(_configDictionary, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(CONFIG_FILE_NAME, json);
        }
    }
}
