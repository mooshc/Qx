using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qx.EntitySerialization
{
    class EntitySerializerException : Exception
    {
        public EntitySerializerException(string message) : base(message)
        {

        }
    }
}
