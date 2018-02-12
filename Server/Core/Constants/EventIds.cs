using System;
using Microsoft.Extensions.Logging;

namespace netcore.Core.Contants
{
    public class EventIds 
    {
        public static EventId DEFAULT = new EventId(0, nameof(DEFAULT));
        public static EventId Register = new EventId(1, nameof(Register));
        public static EventId RegisterError = new EventId(2, nameof(RegisterError));
    }
}