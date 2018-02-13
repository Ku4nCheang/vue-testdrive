using System;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Core.Authentications
{
    public class CustomLifetimeValidator
    {
        public CustomLifetimeValidator(IServiceProvider provider)
        {
        }

        public bool ValidateAsync (DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters parameters)
        {
            
            //
            return true;
        }
    }
}