using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    [Serializable]
    public sealed class SMSException : Exception
    {
        public SMSException() { }
        public SMSException(string message) : base(message) { }
        public SMSException(string message, Exception inner) : base(message, inner) { }
        //protected SMSException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
