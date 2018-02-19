using System;

namespace netcore.Models.ViewModels.AccountApiViewModels
{
    public class RegisterViewModel
    {
        public DateTime? DateOfBirth { get; set; }
        public int Grade { get; set; } = 0;
        public string DisplayName { get; set; } = "";
        public string Email { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string Password { get; set; }
        public uint Gender { get; set; } = 1;
    }
}