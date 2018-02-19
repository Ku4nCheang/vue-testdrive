using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using netcore.Core.Configurations;
using netcore.Core.Constants;
using netcore.Core.Contants;
using netcore.Core.ErrorDescribers;
using netcore.Core.Extensions;
using netcore.Core.Services;
using netcore.Core.Utilities;
using netcore.Models;
using netcore.Models.ViewModels.AccountApiViewModels;
using netcore.Models.ViewModels.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace netcore.Controllers.API
{
    // make sure the authorization schema is using jwt bearer, otherwise cookie authentication will be used.
    [Route("api/v1/account")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountApiController : ApiController<AccountApiController>
    {
        public AccountApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ErrorDescriber = new AuthErrorDescriber();
        }

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

            var shouldRetry = false;
            User user = null;

            do
            {
                // prepare user information
                var utcNow = DateTimeOffset.UtcNow;
                var username = utcNow.ToUnixTimeMilliseconds().ToString();
                var password = model.Password ?? Helpers.EncodeToHashId((int)utcNow.ToUnixTimeSeconds(), AppSettings.Server.Node.ToInt());

                user = new User
                {
                    UserName = username,
                    Email = model.Email,
                    JoinedAt = utcNow.UtcDateTime,
                    DateOfBirth = model.DateOfBirth,
                    Grade = model.Grade,
                    DisplayName = model.DisplayName,
                    Gender = model.Gender
                };

                var result = await UserManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    if (result.Errors.Count() == 1 && result.Errors.FirstOrDefault().Code == nameof(IdentityErrorDescriber.DuplicateUserName))
                    {

                    }
                    var errors = result.Errors.TransformIdentityErrors();
                    return this.JsonError(errors.FirstOrDefault());
                }

                // if error is duplicated username, we try again until created the user successfully
                shouldRetry = result.Errors.Count() == 1 && result.Errors.FirstOrDefault().Code == nameof(IdentityErrorDescriber.DuplicateUserName);
            } while (shouldRetry);


            // log success message
            Logger.LogInformation(EventIds.Register, $"User ({user?.Email}) has been created.");
            // return sucess response
            return this.JsonCreated(new
            {
                Token = await _BuildJwtBearerToken(user),
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
            var usernameAcc = await UserManager.FindByNameAsync(model.UserNameOrEmail);
            var user = usernameAcc ?? await UserManager.FindByEmailAsync(model.UserNameOrEmail);

            if (user == null)
            {
                Logger.LogWarning(EventIds.LoginError, $"User was not found with an username or email: ({model.UserNameOrEmail}).");
                return this.JsonError(ErrorDescriber.AccountNotFound());
            }
            else if (user.Deleted)
            {
                Logger.LogWarning(EventIds.LoginError, $"User ({user.Id}) was already deactivated.");
                return this.JsonError(ErrorDescriber.AccountAlreadyDeactivated());
            }

            // checking user password
            var isValidPassword = await UserManager.CheckPasswordAsync(user, model.Password);

            if (!isValidPassword)
            {
                return this.JsonError(ErrorDescriber.IncorrectPassword());
            }

            // log success message
            Logger.LogInformation(EventIds.Login, $"User ({user.Id}) was logon successfully.");
            // return sucess response
            return this.JsonSuccess(new
            {
                Token = await _BuildJwtBearerToken(user),
                User = Mapper.Map<UserViewModel>(user)
            });
        }

        //
        // ─── LOGOUT API ──────────────────────────────────────────────────
        //

        [HttpGet("logout")]
        public JsonResult Logout()
        {
            // do something when logout
            // clear up something or mark user has loggout.
            var userId = User.FindFirstValue(JwtClaimTypes.Identifier);
            // log success message
            Logger.LogInformation(EventIds.Logout, $"User ({userId}) was logouted successfully.");
            // return sucess response
            return this.JsonAccepted(new { });
        }

        //
        // ─── CHANGE PASSWORD API ─────────────────────────────────────────
        //

        [HttpPost("changepwd")]
        public async Task<JsonResult> ChangePWD([FromBody]ChangePwdViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Logger.LogInformation(EventIds.ChangePasswordError, "Change password action was aborted since invalid data.");
                return this.JsonInvalidModelState(ModelState);
            }

            var user = await GetCurrentUserAsync();
            var result = await UserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.TransformIdentityErrors();
                return this.JsonError(errors.FirstOrDefault());
            }

            // log success message
            Logger.LogInformation(EventIds.ChangePassword, $"User ({user.Id}) has changed a new password successfully.");
            // return sucess response
            return this.JsonAccepted(new { });
        }

        //
        // ─── PRIVATE METHODS ─────────────────────────────────────────────
        //

        private async Task<string> _BuildJwtBearerToken(User user)
        {
            // create a user identity for jwt bearer
            var roles = await UserManager.GetRolesAsync(user);
            var roleClaims = roles.Map<Claim>((role) =>
            {
                return new Claim(JwtClaimTypes.Role, role);
            });


            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Name, user.Id),
                new Claim(JwtClaimTypes.Identifier, user.Id),
                new Claim(JwtClaimTypes.UserName, user.UserName)
            };

            claims.AddRange(roleClaims);

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
