using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace netcore.Models
{
    // Add profile data for application users by adding properties to the user class
    public class User : IdentityUser
    {
        public DateTime JoinedAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Grade { get; set; } = 0;
        public string DisplayName { get; set; }
        public uint Gender { get; set; } = 1;
        public bool Deleted { get; set; } = false;
    }
}