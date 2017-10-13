using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qx.Common
{
    public interface IContentDictionary
    {
        string GetContent(string objectName, Language lang);

        void SaveOrUpdateValue(string objectName, string text, Language lang);

        void PopulateDictionary(Language lang);
    }
}
