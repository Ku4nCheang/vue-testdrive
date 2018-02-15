using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace netcore.Models
{
    public class UserRole : IdentityRole<string>
    {
        public UserRole()
        {

        }
        
        public UserRole(string roleName) : base(roleName)
        {

        }
    }
}