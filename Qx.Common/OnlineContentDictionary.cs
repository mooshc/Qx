using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qx.Common
{
    public class OnlineContentDictionary : IContentDictionary
    {
        public string GetContent(string objectName, Language lang)
        {
            return ContentDictionary.GetContent(objectName, lang);
        }

        public void PopulateDictionary(Language lang)
        {
            ContentDictionary.PopulateDictionary(lang);
        }

        public void SaveOrUpdateValue(string objectName, string text, Language lang)
        {
            ContentDictionary.SaveOrUpdateValue(objectName, text, lang);
        }
    }
}
