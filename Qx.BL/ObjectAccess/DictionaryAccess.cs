using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
using NHibernate.Context;
using NHibernate.Linq;
namespace Qx.BL
{
    public class DictionaryAccess : ObjectAccessNHibernate<Dictionary> , IDictionaryAccess
    {
        public DictionaryAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public List<Dictionary> LoadByLang(Language lang, int? iteration = null)
        {
            var s = _sessionContext.CurrentSession();
            if (ServerStatics.Dictionary != null)
                return ServerStatics.Dictionary;
            if (iteration.HasValue)
                return s.Linq<Dictionary>().Where(d => d.Language == lang && d.ID >= iteration.Value * 1000 && d.ID < (iteration.Value + 1) * 1000).ToList();
            else
                throw new Exception("No iteration provided!!");
        }

        public bool LoadByLangToServer(Language lang, int iteration)
        {
            var s = _sessionContext.CurrentSession();
            if (ServerStatics.Dictionary == null)
                ServerStatics.Dictionary = new List<Dictionary>();
            var data = s.Linq<Dictionary>().Where(d => d.Language == lang && d.ID >= iteration * 1000 && d.ID < (iteration + 1) * 1000);
            ServerStatics.Dictionary.AddRange(data);
            return data.Count() > 0;
        }

        public void SaveOrUpdateByName(string Name, string Text, Language Lang)
        {
            var s = _sessionContext.CurrentSession();

            var obj = s.Linq<Dictionary>().Where(d => d.ObjectName == Name).FirstOrDefault();
            if (obj == null)
                Save(new Dictionary() { Language = Lang, ObjectName = Name, Text = Text });
            else
            {
                obj.Text = Text;
                Update(obj);
            }
        }

        public int GetTotalCount()
        {
            var s = _sessionContext.CurrentSession();
            return s.Linq<Dictionary>().Count();
        }
    }
}
