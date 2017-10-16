using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qx.Common
{
    [Serializable]
    public class LocalContentDictionary : IContentDictionary
    {
        static Dictionary<Language, Dictionary<string, Dictionary>> Dictionary = new Dictionary<Language, Dictionary<string, Dictionary>>();
        static Language Heb;

        public string GetContent(string objectName, Language lang)
        {
            if (lang == null)
                Heb = CommonFunctions.HebLang;
            lang = lang ?? Heb;
            if (!Dictionary.ContainsKey(lang))
                PopulateDictionary(lang);
            if (!Dictionary.ContainsKey(lang)) return "שפה לא נמצאת במילון";
            return ((Dictionary[lang].ContainsKey(objectName ?? "NotInDictionary")) ? Dictionary[lang][objectName].Text : "לא נמצא במילון").ToString();
        }

        public void SaveOrUpdateValue(string objectName, string text, Language lang)
        {
            if (!Dictionary.ContainsKey(lang))
                PopulateDictionary(lang);
            try
            {
                Dictionary[lang][objectName].Text = text;
            }
            catch (KeyNotFoundException)
            {
                Dictionary[lang].Add(objectName, new Dictionary() { ObjectName = objectName, Text = text, Language = lang });
            }
        }

        public void PopulateDictionary(Language lang)
        {
            Dictionary.Add(lang, new Dictionary<string, Common.Dictionary>());
            CommonFunctions.HebLang = lang;
        }
    }
}
