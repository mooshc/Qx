using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Qx.Common.Objects
{
    [Serializable]
    public abstract class TranslatedObject
    {
        private LocalContentDictionary localContentDictionary;
        private static OnlineContentDictionary onlineContentDictionary = new OnlineContentDictionary();

        [JsonIgnore]
        public static bool UseOnlineContentDictionary
        {
            set; get;
        }

        [JsonIgnore]
        protected IContentDictionary ContentDictionary
        {
            get
            {
                if (UseOnlineContentDictionary)
                    return onlineContentDictionary;
                localContentDictionary = localContentDictionary ?? new LocalContentDictionary();
                return localContentDictionary;
            }
        }

        protected void SaveOrUpdateByName(string name, string text, Language lang)
        {
            if (UseOnlineContentDictionary)
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName(name, text, lang);
            ContentDictionary.SaveOrUpdateValue(name, text, lang);
        }
    }
}
