using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qx.Common
{
    public static class ContentDictionary
    {
        static Dictionary<Language, Dictionary<string, Dictionary>> Dictionary = new Dictionary<Language, Dictionary<string, Dictionary>>();
        static Language Heb;

        public static string GetContent(string objectName, Language lang)
        {
            if (lang == null)
                Heb = RemoteObjectProvider.GetLanguageAccess().Load(1);
            lang = lang ?? Heb;
            if(!Dictionary.ContainsKey(lang))
                PopulateDictionary(lang);
            if (!Dictionary.ContainsKey(lang)) return "שפה לא נמצאת במילון";
            return ((Dictionary[lang].ContainsKey(objectName ?? "NotInDictionary")) ? Dictionary[lang][objectName].Text : "לא נמצא במילון").ToString();
        }

        public static void SaveOrUpdateValue(string objectName, string text, Language lang)
        {
            if (!Dictionary.ContainsKey(lang))
                PopulateDictionary(lang);
            if (!Dictionary.ContainsKey(lang)) return;
            try
            {
                Dictionary[lang][objectName].Text= text;
            }
            catch(KeyNotFoundException)
            {
                Dictionary[lang].Add(objectName,new Dictionary() { ObjectName = objectName, Text = text, Language = lang});
            }
        }

        public static void PopulateDictionary(Language lang)
        {
            Heb = lang;
            if (!Dictionary.ContainsKey(lang))
            {
                var all = new List<Dictionary>();
                List<Dictionary> data;
                int iteration = 0;
                do
                {
                    data = RemoteObjectProvider.GetDictionaryAccess().LoadByLang(lang, iteration++);
                    all.AddRange(data);
                }while(data.Count > 0 && data.Count<=1000);
                CommonFunctions.HebLang = lang;
                //Dictionary.Remove(lang);
                Dictionary.Add(lang, all.ToDictionary<Dictionary, string>(d => d.ObjectName));
            }
        }
    }
}
