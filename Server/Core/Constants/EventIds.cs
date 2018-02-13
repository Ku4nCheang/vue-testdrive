using System;
using Microsoft.Extensions.Logging;

namespace netcore.Core.Contants
{
    public class EventIds 
    {
        public static EventId DEFAULT = new EventId(0, nameof(DEFAULT));
        public static EventId Register = new EventId(1, nameof(Register));
        public static EventId RegisterError = new EventId(2, nameof(RegisterError));
        public static EventId Login = new EventId(3, nameof(Login));
        public static EventId LoginError = new EventId(4, nameof(LoginError));
        public static EventId Logout = new EventId(5, nameof(Logout));
        public static EventId LogoutError = new EventId(6, nameof(LogoutError));
    }
}