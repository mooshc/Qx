using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;
using System.Windows.Media;

namespace Qx.Common
{
    [Serializable]
    public class Color : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual System.Windows.Media.Color GetColor()
        {
            switch (Name)
            {
                case "Red": return Colors.Red;

                case "Yellow": return Colors.Yellow;

                case "Orange": return Colors.Orange;

                case "Green": return Colors.Green;

                case "Blue": return Colors.Blue;

            }
            return Colors.Transparent;
        }

        protected override object GetObjectId()
        {
            return ID;
        }
    }
}
