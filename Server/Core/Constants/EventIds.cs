using System;
using Microsoft.Extensions.Logging;

namespace netcore.Core.Contants
{
    public class EventIds 
    {
        public static EventId Default = new EventId(0, nameof(Default));
        public static EventId Register = new EventId(1, nameof(Register));
        public static EventId RegisterError = new EventId(2, nameof(RegisterError));
        public static EventId Login = new EventId(3, nameof(Login));
        public static EventId LoginError = new EventId(4, nameof(LoginError));
        public static EventId Logout = new EventId(5, nameof(Logout));
        public static EventId LogoutError = new EventId(6, nameof(LogoutError));
        public static EventId ChangePassword = new EventId(7, nameof(ChangePassword));
        public static EventId ChangePasswordError = new EventId(8, nameof(ChangePasswordError));
    }
}