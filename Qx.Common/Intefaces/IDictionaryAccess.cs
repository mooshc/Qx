﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;

namespace Qx.Common
{
    public interface IDictionaryAccess : IObjectAccess<Dictionary>
    {
        List<Dictionary> LoadByLang(Language lang, int? iteration = null);
        void SaveOrUpdateByName(string Name, string Text, Language Lang);
        bool LoadByLangToServer(Language lang, int iteration);
        int GetTotalCount();
    }
}
