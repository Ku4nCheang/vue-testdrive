
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace netcore.Core.Utilities 
{
    public static class JwtSecurityKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}