using System;

namespace netcore.Models.ViewModels.SharedViewModels
{
    public class UserViewModel
    {
        public DateTime JoinedAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Grade { get; set; } = 0;
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Id { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public uint Gender { get; set; } = 1;
    }
}