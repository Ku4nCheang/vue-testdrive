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
        // private readonly ICacheRepository _CacheRepo;
        public CustomLifetimeValidator(IServiceProvider provider)
        {
            // this._CacheRepo = provider.GetService(typeof(ICacheRepository)) as ICacheRepository;
        }

        public bool ValidateAsync (DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters parameters)
        {
            // var jwtToken = token as JwtSecurityToken;
            // var signature = (from c in jwtToken.Claims
            //                 where c.Type == "Signature"
            //                 select c.Value).FirstOrDefault();

            // // validate user
            // var result = this._CacheRepo.ValidateUserLoginAsync(signature, jwtToken.RawData);
            // result.Wait();
            // return result.Result;
            return true;
        }
    }
}