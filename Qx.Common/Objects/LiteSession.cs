using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qx.Common.Objects
{
    public class LiteSession
    {
        public LiteUser User { get; set; }
        public List<QuestionInModule> PermanentQuestions { get; set; } = new List<QuestionInModule>();
    }
}
