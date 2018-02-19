using System;

namespace netcore.Models.ViewModels.UserApiViewModels
{
    public class UpdateProfileViewModel
    {
        public DateTime? DateOfBirth { get; set; } = null;
        public string DisplayName { get; set; } = null;
        public string Email { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public uint? Gender { get; set; } = null;
    }
}