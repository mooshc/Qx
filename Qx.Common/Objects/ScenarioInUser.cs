using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qx.Common
{
    [Serializable]
    public class ScenarioInUser
    {
        public virtual int ScenarioID { set; get; }
        public virtual int Mistakes { set; get; }

        public ScenarioInUser()
        {

        }

        public ScenarioInUser(int scenarioID, int mistakes)
        {
            ScenarioID = scenarioID;
            Mistakes = mistakes;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return (obj as ScenarioInUser).ScenarioID == ScenarioID && (obj as ScenarioInUser).Mistakes == Mistakes;
            }
            catch
            {
                return false;
            }
        }
    }
}
