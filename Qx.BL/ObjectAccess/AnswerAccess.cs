﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;
using Qx.Common;
using NHibernate.Context;
using NHibernate.Linq;

namespace Qx.BL
{
    public class AnswerAccess : ObjectAccessNHibernate<Answer> , IAnswerAccess
    {
        public AnswerAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public string GetAnswerName(int id)
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Answer>().Where(a => a.ID == id).Select(a => a.Name).ToList().FirstOrDefault();
        }

        public int GetAnswerID(string name)
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Answer>().Where(a => a.Name == name).Select(a => a.ID).ToList().FirstOrDefault();
        }

        public List<string> GetAllNames()
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Answer>().Select(a => a.Name).ToList();
        }
    }
}
