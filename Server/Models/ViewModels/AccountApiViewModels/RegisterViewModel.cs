using System;

namespace netcore.Models.ViewModels.AccountApiViewModels
{
    public class RegisterViewModel
    {
        public string Password { get; set; }
        public string Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
        public int Grade { get; set; } = 0;
        public string DisplayName { get; set; } = "";
        public uint Gender { get; set; } = 1;

    }
}