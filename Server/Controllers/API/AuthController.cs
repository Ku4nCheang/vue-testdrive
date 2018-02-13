using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using netcore.Models;
using netcore.Core.Extensions;
using netcore.Models.ViewModels.AuthViewModels;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using netcore.Core.Contants;
using netcore.Core.ErrorDescribers;
using netcore.Core.Services;
using netcore.Models.ViewModels.SharedViewModels;
using System.Security.Claims;
using netcore.Core.Constants;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using netcore.Core.Configurations;
using netcore.Core.Utilities;

namespace netcore.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : ApiController<AuthController>
    {
        public AuthController(IServiceProvider serviceProvider, UserManager<User> userManager, AppSettings appSettings) :
            base(serviceProvider)
        {
            UserManager = userManager;
            AppSettings = appSettings;
            ErrorDescriber = new AuthErrorDescriber();
        }

        protected readonly UserManager<User> UserManager;
        protected readonly AppSettings AppSettings;
        protected readonly AuthErrorDescriber ErrorDescriber;

        //
        // ─── REGISTER API ────────────────────────────────────────────────
        //

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<JsonResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Logger.LogInformation(EventIds.Register, "Registration was aborted since invalid data.");
                return this.JsonInvalidModelState(ModelState);
            }

            var user = new User { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.TransformIdentityErrors();
                return this.JsonError(errors.FirstOrDefault());
            }

            // log success message
            Logger.LogInformation(EventIds.Register, $"User ({user.Email}) has been created.");
            // return sucess response
            return this.JsonSuccess(new
            {
                Token = _BuildJwtBearerToken(user),
                User = Mapper.Map<UserViewModel>(user)
            });
        }

        //
        // ─── LOGIN API ───────────────────────────────────────────────────
        //

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<JsonResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Logger.LogInformation(EventIds.Login, "Login was aborted since invalid data.");
                return this.JsonInvalidModelState(ModelState);
            }

            // checking user existent
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                Logger.LogWarning(EventIds.LoginError, $"User was not found by email ({model.Email}).");
                return this.JsonError(ErrorDescriber.UserNotFound());
            }

            // checking user password
            var isValidPassword = await UserManager.CheckPasswordAsync(user, model.Password);

            if (!isValidPassword)
            {
                return this.JsonError(ErrorDescriber.IncorrectPassword());
            }

            // log success message
            Logger.LogInformation(EventIds.Login, $"User ({user.Email}) was logon successfully.");
            // return sucess response
            return this.JsonSuccess(new
            {
                Token = _BuildJwtBearerToken(user),
                User = Mapper.Map<UserViewModel>(user)
            });
        }

        //
        // ─── LOGOUT API ──────────────────────────────────────────────────
        //

        [HttpGet("logout")]
        // make sure the authorization schema is using jwt bearer, otherwise cookie authentication will be used.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult Logout()
        {
            // do something when logout
            // clear up something or mark user has loggout.
            var userId = User.FindFirstValue(JwtClaimTypes.UserId);

            // log success message
            Logger.LogInformation(EventIds.Logout, $"User ({userId}) was logouted successfully.");
            // return sucess response
            return this.JsonAccepted(new { });
        }

        //
        // ─── PRIVATE METHODS ─────────────────────────────────────────────
        //

        private string _BuildJwtBearerToken(User user)
        {
            // create a user identity for jwt bearer
            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.UserName, user.UserName),
                new Claim(JwtClaimTypes.UserId, user.Id)
            };
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: AppSettings.Authenticate.JwtBearer.Issuer,
                audience: AppSettings.Authenticate.JwtBearer.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddDays(30),
                signingCredentials: new SigningCredentials(JwtSecurityKey.Create(AppSettings.Authenticate.JwtBearer.Secret), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
